using Godot;

public class Options : Control
{
    // Node references
    private LineEdit _username;
    private ColorPickerButton _colour;
    private SpinBox _sensitivityX;
    private SpinBox _sensitivityY;
    private SpinBox _fovX;
    private SpinBox _fovY;
    
    public override void _Ready()
    {
        // Initialise node references
        _username = GetNode<LineEdit>("VBox/Username/VBox/LineEdit");
        _colour = GetNode<ColorPickerButton>("VBox/Colour/VBox/ColorPicker");
        _sensitivityX = GetNode<SpinBox>("VBox/Sensitivity/VBox/HBox/X");
        _sensitivityY = GetNode<SpinBox>("VBox/Sensitivity/VBox/HBox/Y");
        _fovX = GetNode<SpinBox>("VBox/Fov/VBox/HBox/X");
        _fovY = GetNode<SpinBox>("VBox/Fov/VBox/HBox/Y");
        
        // Connect signals
        GetNode<Button>("VBox/Back").Connect("pressed", this, nameof(BackPressed));
        
        // Populate fields from Global.cs
        _username.Text = Global.Username;
        _colour.Color = Global.Colour;
        _sensitivityX.Value = Global.Sensitivity.x;
        _sensitivityY.Value = Global.Sensitivity.y;
        _fovX.Value = Global.Fov.x;
        _fovY.Value = Global.Fov.y;
    }

    public void BackPressed()
    {
        // Save fields to Global.cs
        Global.Username = _username.Text;
        Global.Colour = _colour.Color;
        Global.Sensitivity.x = (float) _sensitivityX.Value;
        Global.Sensitivity.y = (float) _sensitivityY.Value;
        Global.Fov.x = (float) _fovX.Value;
        Global.Fov.y = (float) _fovY.Value;
        
        GetTree().ChangeScene("res://Scenes/Menus/Main.tscn");
    }
}
