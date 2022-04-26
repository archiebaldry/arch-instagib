using Godot;

public class Host : Control
{
    // Node references
    private SpinBox _port;
    private CheckBox _upnp;
    private SpinBox _fragLimit;
    private SpinBox _timeLimit;
    private Alert _alert;
    
    public override void _Ready()
    {
        // Initialise node references
        _port = GetNode<SpinBox>("VBox/Port/VBox/HBox/SpinBox");
        _upnp = GetNode<CheckBox>("VBox/Port/VBox/HBox/CheckBox");
        _fragLimit = GetNode<SpinBox>("VBox/Limits/VBox/HBox/Frag");
        _timeLimit = GetNode<SpinBox>("VBox/Limits/VBox/HBox/Time");
        _alert = GetNode<Alert>("Alert");
        
        // Connect signals
        GetNode<Button>("VBox/HBox/Back").Connect("pressed", this, nameof(BackPressed));
        GetNode<Button>("VBox/HBox/Host").Connect("pressed", this, nameof(HostPressed));
        
        // Populate fields from Global.cs
        _port.Value = Global.Port;
        _upnp.Pressed = Global.Upnp;
        _fragLimit.Value = Global.FragLimit;
        _timeLimit.Value = Global.TimeLimit;
    }

    private void BackPressed()
    {
        // Save fields to Global.cs
        Global.Port = (int) _port.Value;
        Global.Upnp = _upnp.Pressed;
        Global.FragLimit = (int) _fragLimit.Value;
        Global.TimeLimit = (int) _timeLimit.Value;
        
        GetTree().ChangeScene("res://Scenes/Menus/Main.tscn");
    }

    private void HostPressed()
    {
        _alert.Popup("Attempting to host...");
        
        // Save fields to Global.cs
        Global.Port = (int) _port.Value;
        Global.Upnp = _upnp.Pressed;
        Global.FragLimit = (int) _fragLimit.Value;
        Global.TimeLimit = (int) _timeLimit.Value;
        
        // Create a server
        NetworkedMultiplayerENet eNet = new NetworkedMultiplayerENet();
        
        Error server = eNet.CreateServer(Global.Port);

        if (server != Error.Ok)
        {
            _alert.Popup("Failed to create server", true, true);
            return;
        }

        GetTree().NetworkPeer = eNet; // Set the tree's network peer to our server
        
        Global.Players.Clear(); // Clear any player information left behind from previous session
        Global.Players.Add(1, new Peer(1, Global.Username, Global.Colour)); // Add our info to Players
        
        GetTree().ChangeScene("res://Scenes/Menus/Lobby.tscn");
    }
}
