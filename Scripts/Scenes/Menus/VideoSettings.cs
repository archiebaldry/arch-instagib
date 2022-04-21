using Godot;

public class VideoSettings : Control
{
    // Node references
    private HSlider _sharpenIntensity;
    private OptionButton _msaa;
    private CheckButton _fxaa;
    private CheckButton _fullscreen;
    
    public override void _Ready()
    {
        // Initialise node references
        _sharpenIntensity = GetNode<HSlider>("VBox/SharpenIntensity/VBox/HSlider");
        _msaa = GetNode<OptionButton>("VBox/MSAA/VBox/OptionButton");
        _fxaa = GetNode<CheckButton>("VBox/FXAA/CheckButton");
        _fullscreen = GetNode<CheckButton>("VBox/Fullscreen/CheckButton");
        
        // Connect signals
        GetNode<Button>("VBox/HBox/Back").Connect("pressed", this, nameof(BackPressed));
        GetNode<Button>("VBox/HBox/ApplySave").Connect("pressed", this, nameof(ApplySavePressed));
        
        // Populate fields from ProjectSettings and OS
        _sharpenIntensity.Value = (float) ProjectSettings.GetSetting("rendering/quality/filters/sharpen_intensity");
        _msaa.Selected = (int) ProjectSettings.GetSetting("rendering/quality/filters/msaa");
        _fxaa.Pressed = (bool) ProjectSettings.GetSetting("rendering/quality/filters/use_fxaa");
        _fullscreen.Pressed = OS.WindowFullscreen;
    }

    private void BackPressed()
    {
        // Go back to settings
        GetTree().ChangeScene("res://Scenes/Menus/Settings.tscn");
    }

    private void ApplySavePressed()
    {
        // Save fields to ProjectSettings and OS
        ProjectSettings.SetSetting("rendering/quality/filters/sharpen_intensity", _sharpenIntensity.Value);
        ProjectSettings.SetSetting("rendering/quality/filters/msaa", _msaa.Selected);
        ProjectSettings.SetSetting("rendering/quality/filters/use_fxaa", _fxaa.Pressed);
        OS.WindowFullscreen = _fullscreen.Pressed;
        
        BackPressed();
    }
}
