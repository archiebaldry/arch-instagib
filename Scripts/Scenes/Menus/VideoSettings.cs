using Godot;

public class VideoSettings : Control
{
    // Node references
    private HSlider _sharpness;
    private OptionButton _msaa;
    private CheckButton _fxaa;
    private CheckButton _fullscreen;
    private CheckButton _vsync;
    private CheckButton _fps;
    
    public override void _Ready()
    {
        // Initialise node references
        _sharpness = GetNode<HSlider>("VBox/Sharpness/VBox/HSlider");
        _msaa = GetNode<OptionButton>("VBox/MSAA/VBox/OptionButton");
        _fxaa = GetNode<CheckButton>("VBox/FXAA/CheckButton");
        _fullscreen = GetNode<CheckButton>("VBox/Fullscreen/CheckButton");
        _vsync = GetNode<CheckButton>("VBox/VSync/CheckButton");
        _fps = GetNode<CheckButton>("VBox/FPS/CheckButton");
        
        // Connect signals
        GetNode<Button>("VBox/HBox/Back").Connect("pressed", this, nameof(BackPressed));
        GetNode<Button>("VBox/HBox/ApplySave").Connect("pressed", this, nameof(ApplySavePressed));
        
        // Populate fields from ProjectSettings, OS and Global.cs
        _sharpness.Value = (float) ProjectSettings.GetSetting("rendering/quality/filters/sharpen_intensity");
        _msaa.Selected = (int) ProjectSettings.GetSetting("rendering/quality/filters/msaa");
        _fxaa.Pressed = (bool) ProjectSettings.GetSetting("rendering/quality/filters/use_fxaa");
        _fullscreen.Pressed = OS.WindowFullscreen;
        _vsync.Pressed = OS.VsyncEnabled;
        _fps.Pressed = Global.ShowFPS;
    }

    private void BackPressed()
    {
        // Go back to settings
        GetTree().ChangeScene("res://Scenes/Menus/Settings.tscn");
    }

    private void ApplySavePressed()
    {
        // Save fields to ProjectSettings, OS and Global.cs
        ProjectSettings.SetSetting("rendering/quality/filters/sharpen_intensity", _sharpness.Value);
        ProjectSettings.SetSetting("rendering/quality/filters/msaa", _msaa.Selected);
        ProjectSettings.SetSetting("rendering/quality/filters/use_fxaa", _fxaa.Pressed);
        OS.WindowFullscreen = _fullscreen.Pressed;
        OS.VsyncEnabled = _vsync.Pressed;
        Global.ShowFPS = _fps.Pressed;
        
        BackPressed();
    }
}
