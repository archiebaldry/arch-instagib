using Godot;

public class Options : Control
{
    // Node references
    private LineEdit _username;
    private ColorPickerButton _colour;
    private SpinBox _sensitivityX;
    private SpinBox _sensitivityY;
    private HSlider _fov;
    
    public override void _Ready()
    {
        // Initialise node references
        _username = GetNode<LineEdit>("VBox/Username/VBox/LineEdit");
        _colour = GetNode<ColorPickerButton>("VBox/Colour/VBox/ColorPicker");
        _sensitivityX = GetNode<SpinBox>("VBox/Sensitivity/VBox/HBox/X");
        _sensitivityY = GetNode<SpinBox>("VBox/Sensitivity/VBox/HBox/Y");
        _fov = GetNode<HSlider>("VBox/Fov/VBox/HSlider");
        
        // Connect signals
        GetNode<Button>("VBox/HBox/Back").Connect("pressed", this, nameof(BackPressed));
        GetNode<Button>("VBox/HBox/Save").Connect("pressed", this, nameof(SavePressed));
        
        // Populate fields from Global.cs
        _username.Text = Global.Username;
        _colour.Color = Global.Colour;
        _sensitivityX.Value = Global.Sensitivity.x;
        _sensitivityY.Value = Global.Sensitivity.y;
        _fov.Value = Global.Fov;
    }

    private void BackPressed()
    {
        // Go back to the main menu
        GetTree().ChangeScene("res://Scenes/Menus/Main.tscn");
    }

    private void SavePressed()
    {
        // Save fields to Global.cs
        Global.Username = _username.Text;
        Global.Colour = _colour.Color;
        Global.Sensitivity.x = (float) _sensitivityX.Value;
        Global.Sensitivity.y = (float) _sensitivityY.Value;
        Global.Fov = (int) _fov.Value;
        
        BackPressed();
    }
}
