using Godot;

public class GameSettings : Control
{
    // Node references
    private LineEdit _username;
    private ColorPickerButton _colour;
    private SpinBox _sensitivityX;
    private SpinBox _sensitivityY;
    private HSlider _fov;
    private CheckButton _playerDebug;
    
    public override void _Ready()
    {
        // Initialise node references
        _username = GetNode<LineEdit>("VBox/Username/VBox/LineEdit");
        _colour = GetNode<ColorPickerButton>("VBox/Colour/VBox/ColorPicker");
        _sensitivityX = GetNode<SpinBox>("VBox/Sensitivity/VBox/HBox/X");
        _sensitivityY = GetNode<SpinBox>("VBox/Sensitivity/VBox/HBox/Y");
        _fov = GetNode<HSlider>("VBox/Fov/VBox/HSlider");
        _playerDebug = GetNode<CheckButton>("VBox/PlayerDebug/CheckButton");
        
        // Connect signals
        GetNode<Button>("VBox/HBox/Back").Connect("pressed", this, nameof(BackPressed));
        GetNode<Button>("VBox/HBox/Save").Connect("pressed", this, nameof(SavePressed));
        
        // Populate fields from Global.cs
        _username.Text = Global.Username;
        _colour.Color = Global.Colour;
        _sensitivityX.Value = Global.Sensitivity.x;
        _sensitivityY.Value = Global.Sensitivity.y;
        _fov.Value = Global.Fov;
        _playerDebug.Pressed = Global.PlayerDebug;
    }

    private void BackPressed()
    {
        // Go back to settings
        GetTree().ChangeScene("res://Scenes/Menus/Settings.tscn");
    }

    private void SavePressed()
    {
        // Save fields to Global.cs
        Global.Username = _username.Text;
        Global.Colour = _colour.Color;
        Global.Sensitivity.x = (float) _sensitivityX.Value;
        Global.Sensitivity.y = (float) _sensitivityY.Value;
        Global.Fov = (int) _fov.Value;
        Global.PlayerDebug = _playerDebug.Pressed;
        
        // Tell Global.cs to save to file
        GetNode<Global>("/root/Global").SaveDataToFile();
        
        BackPressed();
    }
}
