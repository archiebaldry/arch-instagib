using Godot;

public class Lobby : Control
{
    // Node references
    private Global _global;
    private RichTextLabel _playerList;
    private Button _start;
    
    public override void _Ready()
    {
        // Initialise node references
        _global = GetNode<Global>("/root/Global");
        _playerList = GetNode<RichTextLabel>("VBox/Panel/PlayerList");
        _start = GetNode<Button>("VBox/HBox/Start");
        
        // Connect signals
        _global.Connect(nameof(Global.PlayersUpdated), this, nameof(UpdatePlayerList));
        GetNode<Button>("VBox/HBox/Leave").Connect("pressed", this, nameof(LeavePressed));
        _start.Connect("pressed", this, nameof(StartPressed));
        
        UpdatePlayerList();
    }

    private void UpdatePlayerList()
    {
        string[] list = new string[Global.Players.Count];

        int i = 0;
        
        foreach (Peer player in Global.Players.Values)
        {
            list[i] = $"[color=#{player.Colour.ToHtml()}]{player.Username}";
            
            i += 1;
        }

        _playerList.ParseBbcode(string.Join("\n", list));

        // Only enable the start button if the user is the host and there is more than 1 player in the lobby
        _start.Disabled = !(GetTree().IsNetworkServer() && Global.Players.Count > 1);
    }
    
    private void LeavePressed()
    {
        _global.Disconnect(); // Tell Global.cs we wish to disconnect
    }

    private void StartPressed()
    {
        GetTree().RefuseNewNetworkConnections = true; // Stop new players from joining

        Rpc(nameof(StartGame)); // Tell everyone on the network to call StartGame(), including the server
    }

    [RemoteSync]
    private void StartGame()
    {
        Global.InGame = true; // Tell Global.cs the game has now started
        
        GetTree().ChangeScene("res://Scenes/Game.tscn"); // Change our scene to the game scene
    }
}
