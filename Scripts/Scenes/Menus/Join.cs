using Godot;

public class Join : Control
{
    // Node references
    private Global _global;
    private LineEdit _address;
    private SpinBox _port;
    private CheckBox _upnp;
    private Alert _alert;
    
    public override void _Ready()
    {
        // Initialise node references
        _global = GetNode<Global>("/root/Global");
        _address = GetNode<LineEdit>("VBox/Address/VBox/LineEdit");
        _port = GetNode<SpinBox>("VBox/Port/VBox/HBox/SpinBox");
        _upnp = GetNode<CheckBox>("VBox/Port/VBox/HBox/CheckBox");
        _alert = GetNode<Alert>("Alert");
        
        // Connect signals
        GetNode<Button>("VBox/HBox/Back").Connect("pressed", this, nameof(BackPressed));
        GetNode<Button>("VBox/HBox/Join").Connect("pressed", this, nameof(JoinPressed));
        
        // Populate fields from Global.cs
        _address.Text = Global.Address;
        _port.Value = Global.Port;
        _upnp.Pressed = Global.Upnp;
    }

    public void BackPressed()
    {
        // Save fields to Global.cs
        Global.Address = _address.Text;
        Global.Port = (int) _port.Value;
        Global.Upnp = _upnp.Pressed;
        
        GetTree().ChangeScene("res://Scenes/Menus/Main.tscn");
    }

    public void JoinPressed()
    {
        _alert.Popup("Attempting to join...");
        
        // Save fields to Global.cs
        Global.Address = _address.Text;
        Global.Port = (int) _port.Value;
        Global.Upnp = _upnp.Pressed;

        // Create a client
        NetworkedMultiplayerENet eNet = new NetworkedMultiplayerENet();
        
        string address = Global.Address.Empty() ? Global.DefaultAddress : Global.Address; // If the address field was left blank, use the default address from Global.cs
        
        Error client = eNet.CreateClient(address, Global.Port);

        if (client != Error.Ok)
        {
            _alert.Popup("Failed to create client", true);
            return;
        }

        GetTree().NetworkPeer = eNet; // Set the tree's network peer to our client
        
        ConnectToServer(); // Attempt connection to server
    }

    private async void ConnectToServer()
    {
        _alert.Popup($"Connecting to server...\n(please allow up to {Global.ConnectionTimeout} seconds)");
        
        GetTree().Connect("connected_to_server", this, nameof(ConnectedToServer)); // Connect the 'connected_to_server' signal to our method
        
        // Start a timer for connecting to the server
        // If it runs out and we haven't heard back from the server, then we should give up
        // Without this timer, we may be here forever!
        await ToSignal(GetTree().CreateTimer(Global.ConnectionTimeout), "timeout");
        
        NetworkedMultiplayerENet eNet = (NetworkedMultiplayerENet) GetTree().NetworkPeer; // Get the ENet from the network peer
        eNet.CloseConnection(); // Close our connection with the server
        GetTree().NetworkPeer = null; // Clear our network peer
        
        GetTree().Disconnect("connected_to_server", this, nameof(ConnectedToServer)); // Disconnect from 'connected_to_server'
        
        _alert.Popup($"Failed to connect to server", true);
    }

    private void ConnectedToServer()
    {
        _global.SendClientInfo(); // Send my client info to the server

        _alert.Popup($"Sending client info...");
    }
}
