using Godot;

public class Settings : Control
{
    public override void _Ready()
    {
        // Connect signals
        GetNode<Button>("VBox/Controls").Connect("pressed", this, nameof(ControlsPressed));
        GetNode<Button>("VBox/Game").Connect("pressed", this, nameof(GamePressed));
        GetNode<Button>("VBox/Video").Connect("pressed", this, nameof(VideoPressed));
        GetNode<Button>("VBox/Back").Connect("pressed", this, nameof(BackPressed));
    }

    private void ControlsPressed()
    {
        GetTree().ChangeScene("res://Scenes/Menus/Controls.tscn");
    }

    private void GamePressed()
    {
        GetTree().ChangeScene("res://Scenes/Menus/GameSettings.tscn");
    }

    private void VideoPressed()
    {
        GetTree().ChangeScene("res://Scenes/Menus/VideoSettings.tscn");
    }

    private void BackPressed()
    {
        GetTree().ChangeScene("res://Scenes/Menus/Main.tscn");
    }
}
