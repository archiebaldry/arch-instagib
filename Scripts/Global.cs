using System;
using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;

public class Global : Node
{
    // Performance
    public static int TotalGames;
    public static int TotalShots;
    public static int TotalFrags;
    public static int TotalDeaths;
    
    // Game settings
    public static int NetId;
    public static string Username = "Player";
    public static Color Colour = Colors.Red;
    public static Vector2 Sensitivity = new Vector2(3, 3);
    // Actual default is (0.06F, 0.06F)
    // Visual scale: 0.5 to 10 (step 0.5)
    // Actual scale: 0.01 to 0.2 (step 0.01)
    // i.e. Actual = Visual x 0.02
    public static int Fov = 70;
    public static bool PlayerDebug = true;
    
    // Video settings
    public static bool FpsIndicator = true;
    
    // Host/join constants and variables
    public const string DefaultAddress = "127.0.0.1";
    public const float ConnectionTimeout = 10F;
    public static string Address = "";
    public static int Port = 2710;
    public static bool Upnp = false;
    public static int FragLimit = 30;
    public static int TimeLimit = 20;

    // Other public variables
    public static readonly System.Collections.Generic.Dictionary<int, Peer> Players = new System.Collections.Generic.Dictionary<int, Peer>(); // Holds information of every player in the server
    public static bool GamePaused; // Is the game paused?
    public static float PanoCameraElapsed = 50;
    public static float MenuMusicPosition;
    public static bool InGame; // Are we in the game or the lobby?
    
    // Data JSON template
    private const string DataTemplate = @"{{
    ""username"": ""{0}"",
    ""colour"": ""#{1}"",
    ""sensitivity"": [{2}, {3}],
    ""fov"": {4},
    ""playerDebug"": {5},
    ""sharpness"": {6},
    ""msaa"": {7},
    ""fxaa"": {8},
    ""fullscreen"": {9},
    ""vsync"": {10},
    ""fpsIndicator"": {11},
    ""totalGames"": {12},
    ""totalShots"": {13},
    ""totalFrags"": {14},
    ""totalDeaths"": {15}
}}";
    
    // Signals
    [Signal]
    public delegate void PlayersUpdated(); // A signal used to tell the lobby that we have updated Players

    [Signal]
    public delegate void PlayerStatsUpdated(); // A signal used to tell the leaderboard that a player's stats have changed

    [Signal]
    public delegate void PlayerRespawned(int id, int spawnIndex); // A signal used to tell Game.cs that a player has respawned

    public override void _Ready()
    {
        // Connect signals
        GetTree().Connect("network_peer_disconnected", this, nameof(PeerDisconnected)); // A player disconnected
        GetTree().Connect("server_disconnected", this, nameof(HostDisconnected)); // The server went offline (i.e. the host disconnected)
        
        // Read data from file
        ReadDataFromFile();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // If we press the fullscreen keybind
        if (@event.IsActionPressed("fullscreen"))
        {
            // Flip the value of OS.WindowFullscreen to toggle fullscreen
            OS.WindowFullscreen = !OS.WindowFullscreen;
            
            // Save their choice to data.json
            SaveDataToFile();
        }
    }

    private void PeerDisconnected(int id)
    {
        if (InGame) // In the game
        {
            EndGame(2); // End the game and tell the method why
        }
        else // In the lobby
        {
            Players.Remove(id); // Remove the player's info
            EmitSignal(nameof(PlayersUpdated)); // Tell lobby to update player list
        }
    }

    private void HostDisconnected()
    {
        if (InGame) // In the game
        {
            EndGame(2); // End the game and tell the method why
        }
        else // In the lobby
        {
            Disconnect(); // Disconnect from the lobby
        }
    }

    public void SendClientInfo()
    {
        RpcId(1, nameof(ReceiveClientInfo), Username, Colour);
    }

    [Remote]
    private void ReceiveClientInfo(string username, Color colour)
    {
        int newPlayerId = GetTree().GetRpcSenderId();
        int firstSpawnIndex = new Random().Next(24);
        
        // Send the new player every existing player's information
        foreach (Peer player in Players.Values)
        {
            RpcId(newPlayerId, nameof(AddPlayer), player.Id, player.Username, player.Colour, player.FirstSpawnIndex);
        }
        
        // Send the new player the frag and time limits
        RpcId(newPlayerId, nameof(SetLimits), FragLimit, TimeLimit);
        
        // Add the new player's information to the host's Players
        AddPlayer(newPlayerId, username, colour, firstSpawnIndex);

        // Send every player (including the new player) the new player's information
        Rpc(nameof(AddPlayer), newPlayerId, username, colour, firstSpawnIndex);
    }

    [Remote]
    private void AddPlayer(int id, string username, Color colour, int firstSpawnIndex)
    {
        Players.Add(id, new Peer(id, username, colour, firstSpawnIndex)); // Add the player information to Players
        
        EmitSignal(nameof(PlayersUpdated)); // Tell everyone listening (i.e. the lobby, that we have updated Players)
        
        // If I am adding my own information (i.e. the server has responded to me sending my client info) then I should change my scene to the lobby scene
        if (id == GetTree().GetNetworkUniqueId())
        {
            NetId = GetTree().GetNetworkUniqueId();
            
            GetTree().ChangeScene("res://Scenes/Menus/Lobby.tscn");
        }
    }

    [Remote]
    public void SetPlayerPosition(Vector3 position)
    {
        int id = GetTree().GetRpcSenderId(); // Get the id of the player who's moved

        // Set the player's position to the position passed in the function
        GetTree().Root.GetNode<Area>("Game/" + id).GlobalTransform = new Transform(Basis.Identity, position);
    }

    [RemoteSync]
    public void RespawnPlayer(int spawnIndex)
    {
        EmitSignal(nameof(PlayerRespawned), GetTree().GetRpcSenderId(), spawnIndex); // Tell our Game.cs that a player has respawned
    }
    
    [RemoteSync]
    public void PlayerShot(int shooter, int target)
    {
        if (target == NetId) // We are the player that has been shot
        {
            Rpc(nameof(RespawnPlayer), new Random().Next(24)); // Tell yourself, and everyone else on the network, to respawn you
        }
        
        if (shooter == 0) // The shooter is not a player (e.g. falling off map, lava pool)
        {
            Players[target].Deaths += 1; // Record a death for the target
        }
        else // The shooter is a player
        {
            Players[shooter].Shots += 1; // Record a shot for the shooter

            if (target != 0) // The shooter hit another player (i.e. frag)
            {
                Players[shooter].Frags += 1; // Record a frag for the shooter
                Players[target].Deaths += 1; // Record a death for the target
                
                if (Players[shooter].Frags == FragLimit) // The shooter has reached the frag limit
                {
                    EndGame(0); // End the game
                }
            }
        }
        
        EmitSignal(nameof(PlayerStatsUpdated)); // Tell the leaderboard that player stats have changed
    }

    [Remote]
    public void SetLimits(int fragLimit, int timeLimit)
    {
        FragLimit = fragLimit;
        TimeLimit = timeLimit;
    }

    public void Disconnect(bool skipSceneChange = false)
    {
        NetworkedMultiplayerENet eNet = (NetworkedMultiplayerENet) GetTree().NetworkPeer; // Get the ENet from the network peer
        eNet.CloseConnection(); // Close our connection with the server
        GetTree().NetworkPeer = null; // Clear our network peer
        if (!skipSceneChange) GetTree().ChangeScene("res://Scenes/Menus/Main.tscn"); // Send the player back to the main menu
        Input.SetMouseMode(Input.MouseMode.Visible); // Make the mouse cursor visible
        
        // Reset the global values
        Players.Clear();
        GamePaused = false;
        PanoCameraElapsed = 50;
        MenuMusicPosition = 0;
        InGame = false;
    }

    public void EndGame(int type)
    {
        // 0) Frag limit reached
        // 1) Time limit reached
        // 2) A player left the game
        
        GD.Print($"Game ended: reason {type}");
        
        GetTree().ChangeScene("res://Scenes/Menus/GameOver.tscn");
    }

    public void SaveDataToFile()
    {
        File file = new File(); // Make new file object
        
        file.Open("user://data.json", File.ModeFlags.Write); // Open data.json for writing in special user folder

        file.StoreString(string.Format(DataTemplate, Username, Colour.ToHtml(), Sensitivity.x, Sensitivity.y, Fov, PlayerDebug.ToString().ToLower(), (float) ProjectSettings.GetSetting("rendering/quality/filters/sharpen_intensity"), (int) ProjectSettings.GetSetting("rendering/quality/filters/msaa"), ((bool) ProjectSettings.GetSetting("rendering/quality/filters/use_fxaa")).ToString().ToLower(), FpsIndicator.ToString().ToLower(), OS.WindowFullscreen.ToString().ToLower(), OS.VsyncEnabled.ToString().ToLower(), TotalGames, TotalShots, TotalFrags, TotalDeaths)); // Store the data
        
        file.Close(); // Close the file
    }

    private static void ReadDataFromFile()
    {
        File file = new File(); // Make new file object

        if (!file.FileExists("user://data.json")) return; // If the file does not exist there is no data to read

        file.Open("user://data.json", File.ModeFlags.Read); // Open data.json for reading in special user folder

        string contents = file.GetAsText(); // Get the contents of the file as a string

        JSONParseResult parseResult = JSON.Parse(contents); // Attempt to parse the contents as JSON

        if (parseResult.Error == Error.Ok && parseResult.Result is Dictionary data) // The parse was a success!
        {
            // We read any values we know to be ints as floats, and then cast them to ints, as just jumping straight to ints wasn't working 
            
            // Username
            if (data.Contains("username") && data["username"] is string username) Username = username;
            
            // Colour
            if (data.Contains("colour") && data["colour"] is string colour) Colour = new Color(colour);
            
            // Sensitivity
            if (data.Contains("sensitivity") && data["sensitivity"] is Array sensitivity)
            {
                if (sensitivity.Count == 2 && sensitivity[0] is float x && sensitivity[1] is float y)
                {
                    Sensitivity.x = x;
                    Sensitivity.y = y;
                }
            }
            
            // Field of view
            if (data.Contains("fov") && data["fov"] is float fov) Fov = (int) fov;
            
            // Player debug
            if (data.Contains("playerDebug") && data["playerDebug"] is bool playerDebug) PlayerDebug = playerDebug;
            
            // Sharpness
            if (data.Contains("sharpness") && data["sharpness"] is float sharpness) ProjectSettings.SetSetting("rendering/quality/filters/sharpen_intensity", sharpness);
            
            // MSAA
            if (data.Contains("msaa") && data["msaa"] is float msaa) ProjectSettings.SetSetting("rendering/quality/filters/msaa", (int) msaa);
            
            // FXAA
            if (data.Contains("fxaa") && data["fxaa"] is bool fxaa) ProjectSettings.SetSetting("rendering/quality/filters/use_fxaa", fxaa);
            
            // Fullscreen
            if (data.Contains("fullscreen") && data["fullscreen"] is bool fullscreen) OS.WindowFullscreen = fullscreen;
            
            // VSync
            if (data.Contains("vsync") && data["vsync"] is bool vsync) OS.VsyncEnabled = vsync;
            
            // FPS indicator
            if (data.Contains("fpsIndicator") && data["fpsIndicator"] is bool fpsIndicator) FpsIndicator = fpsIndicator;
            
            // Total games
            if (data.Contains("totalGames") && data["totalGames"] is float totalGames) TotalGames = (int) totalGames;
            
            // Total shots
            if (data.Contains("totalShots") && data["totalShots"] is float totalShots) TotalShots = (int) totalShots;
            
            // Total frags
            if (data.Contains("totalFrags") && data["totalFrags"] is float totalFrags) TotalFrags = (int) totalFrags;
            
            // Total deaths
            if (data.Contains("totalDeaths") && data["totalDeaths"] is float totalDeaths) TotalDeaths = (int) totalDeaths;
        }
        
        file.Close(); // Close the file
    }
}
