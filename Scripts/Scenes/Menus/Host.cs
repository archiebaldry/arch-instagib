using Godot;

public class Host : Control
{
    // Node references
    private SpinBox _port;
    private CheckBox _upnp;
    private Alert _alert;
    
    public override void _Ready()
    {
        // Initialise node references
        _port = GetNode<SpinBox>("VBox/Port/VBox/HBox/SpinBox");
        _upnp = GetNode<CheckBox>("VBox/Port/VBox/HBox/CheckBox");
        _alert = GetNode<Alert>("Alert");
        
        // Connect signals
        GetNode<Button>("VBox/HBox/Back").Connect("pressed", this, nameof(BackPressed));
        GetNode<Button>("VBox/HBox/Host").Connect("pressed", this, nameof(HostPressed));
        
        // Populate fields from Global.cs
        _port.Value = Global.Port;
        _upnp.Pressed = Global.Upnp;
    }

    private void BackPressed()
    {
        // Save fields to Global.cs
        Global.Port = (int) _port.Value;
        Global.Upnp = _upnp.Pressed;
        
        GetTree().ChangeScene("res://Scenes/Menus/Main.tscn");
    }

    private void HostPressed()
    {
        _alert.Popup("Attempting to host...");
        
        // Save fields to Global.cs
        Global.Port = (int) _port.Value;
        Global.Upnp = _upnp.Pressed;
        
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
