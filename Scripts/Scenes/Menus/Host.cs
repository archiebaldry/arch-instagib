using Godot;

public class Host : Control
{
    // Node references
    private SpinBox _port;
    private CheckBox _upnp;
    
    public override void _Ready()
    {
        // Initialise node references
        _port = GetNode<SpinBox>("VBox/Port/VBox/HBox/SpinBox");
        _upnp = GetNode<CheckBox>("VBox/Port/VBox/HBox/CheckBox");
        
        // Connect signals
        GetNode<Button>("VBox/HBox/Back").Connect("pressed", this, nameof(BackPressed));
        GetNode<Button>("VBox/HBox/Host").Connect("pressed", this, nameof(HostPressed));
        
        // Populate fields from Global.cs
        _port.Value = Global.Port;
        _upnp.Pressed = Global.Upnp;
    }

    public void BackPressed()
    {
        // Save fields to Global.cs
        Global.Port = (int) _port.Value;
        Global.Upnp = _upnp.Pressed;
        
        GetTree().ChangeScene("res://Scenes/Menus/Main.tscn");
    }

    public void HostPressed()
    {
        // Save fields to Global.cs
        Global.Port = (int) _port.Value;
        Global.Upnp = _upnp.Pressed;
    }
}
