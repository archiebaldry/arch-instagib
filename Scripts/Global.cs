using System.Collections.Generic;
using Godot;

public class Global : Node
{
    // Game settings
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
    public static readonly Dictionary<int, Peer> Players = new Dictionary<int, Peer>(); // Holds information of every player in the server
    public static bool GamePaused = false; // Is the game paused?
    public static float PanoCameraElapsed = 50;
    public static float MenuMusicPosition = 0;
    
    // Signals
    [Signal]
    public delegate void PlayersUpdated(); // A signal used to tell the lobby that we have updated Players

    [Signal]
    public delegate void PlayerStatsUpdated(); // A signal used to tell the leaderboard that a player's stats have changed

    public override void _UnhandledInput(InputEvent @event)
    {
        // If we press the fullscreen keybind
        if (@event.IsActionPressed("fullscreen"))
        {
            // Flip the value of OS.WindowFullscreen to toggle fullscreen
            OS.WindowFullscreen = !OS.WindowFullscreen;
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
        
        // Send the new player every existing player's information
        foreach (Peer player in Players.Values)
        {
            RpcId(newPlayerId, nameof(AddPlayer), player.Id, player.Username, player.Colour);
        }
        
        // Send the new player the frag and time limits
        RpcId(newPlayerId, nameof(SetLimits), FragLimit, TimeLimit);
        
        // Add the new player's information to the host's Players
        AddPlayer(newPlayerId, username, colour);

        // Send every player (including the new player) the new player's information
        Rpc(nameof(AddPlayer), newPlayerId, username, colour);
    }

    [Remote]
    private void AddPlayer(int id, string username, Color colour)
    {
        Players.Add(id, new Peer(id, username, colour)); // Add the player information to Players
        
        EmitSignal(nameof(PlayersUpdated)); // Tell everyone listening (i.e. the lobby, that we have updated Players)
        
        // If I am adding my own information (i.e. the server has responded to me sending my client info) then I should change my scene to the lobby scene
        if (id == GetTree().GetNetworkUniqueId())
        {
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
    public void RespawnPlayer()
    {
        int id = GetTree().GetRpcSenderId(); // Get the id of the player who needs respawning

        // Set the player's position to the spawn position
        GetTree().Root.GetNode<CollisionObject>("Game/" + id).GlobalTransform = new Transform(Basis.Identity, new Vector3(0, 7, 0));
    } 

    [RemoteSync]
    public void PlayerShot(int target) // Ideally we'd use "target = -1" here but RPC doesn't handle it well
    {
        // We are the player that has been shot
        if (target == GetTree().GetNetworkUniqueId())
        {
            // Tell everyone (including self) to respawn you
            Rpc(nameof(RespawnPlayer));
        }
        
        int shooter = GetTree().GetRpcSenderId(); // The shooter is the person who made the function call

        Players[shooter].Shots += 1; // Increment shots

        if (target != -1) // There was a target (i.e. it was a frag)
        {
            Players[shooter].Frags += 1; // Increment frags for shooter
            Players[target].Deaths += 1; // Increment deaths for target

            if (Players[shooter].Frags == FragLimit) // GAME OVER! - player reached the frag limit
            {
                EndGame(0);
            }
        }
        
        EmitSignal(nameof(PlayerStatsUpdated)); // Tell everyone listening (i.e. the leaderboard) that player stats have changed
    }

    [Remote]
    public void SetLimits(int fragLimit, int timeLimit)
    {
        FragLimit = fragLimit;
        TimeLimit = timeLimit;
    }

    public void Disconnect()
    {
        NetworkedMultiplayerENet eNet = (NetworkedMultiplayerENet) GetTree().NetworkPeer; // Get the ENet from the network peer
        eNet.CloseConnection(); // Close our connection with the server
        GetTree().NetworkPeer = null; // Clear our network peer
        GetTree().ChangeScene("res://Scenes/Menus/Main.tscn"); // Send the player back to the main menu
        Input.SetMouseMode(Input.MouseMode.Visible); // Make the mouse cursor visible
    }

    public void EndGame(int type)
    {
        // 0) Frag limit reached
        // 1) Time limit reached
        // 2) TODO: A player left the game
        
        GD.Print($"Game ended: reason {type}");
        
        Disconnect();
    }
}
