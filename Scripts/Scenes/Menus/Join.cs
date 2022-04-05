using Godot;

public class Join : Control
{
    // Node references
    private LineEdit _address;
    private SpinBox _port;
    private CheckBox _upnp;
    private Alert _alert;
    
    public override void _Ready()
    {
        // Initialise node references
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
        _alert.Popup("Attempting to join...", false);
        
        // Save fields to Global.cs
        Global.Address = _address.Text;
        Global.Port = (int) _port.Value;
        Global.Upnp = _upnp.Pressed;
    }
}
