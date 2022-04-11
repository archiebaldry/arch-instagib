using System.Collections.Generic;
using Godot;

public class Global : Node
{
    // Options
    public static string Username = "Player";
    public static Color Colour = Colors.Red;
    
    // Visual scale: 0.5 to 10 (step 0.5)
    // Actual scale: 0.01 to 0.2 (step 0.01)
    // i.e. Actual = Visual x 0.02
    public static Vector2 Sensitivity = new Vector2(3, 3); // Actual default is (0.06F, 0.06F)
    
    public static int Fov = 70;
    
    // Host and Join
    public const string DefaultAddress = "127.0.0.1";
    public const float ConnectionTimeout = 10F;
    public static string Address = "";
    public static int Port = 2710;
    public static bool Upnp = false;

    public static readonly Dictionary<int, Peer> Players = new Dictionary<int, Peer>(); // Holds information of every player in the server
    public static bool GamePaused = false; // Is the game paused?
    
    // Signals
    [Signal]
    public delegate void PlayersUpdated(); // A signal used to tell the lobby that we have updated Players
    
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
}
