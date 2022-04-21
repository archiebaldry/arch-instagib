using Godot;

public class Main : Control
{
    public override void _Ready()
    {
        // Connect signals
        GetNode<Button>("VBox/Host").Connect("pressed", this, nameof(HostPressed));
        GetNode<Button>("VBox/Join").Connect("pressed", this, nameof(JoinPressed));
        GetNode<Button>("VBox/Performance").Connect("pressed", this, nameof(PerformancePressed));
        GetNode<Button>("VBox/Credits").Connect("pressed", this, nameof(CreditsPressed));
        GetNode<Button>("VBox/Settings").Connect("pressed", this, nameof(SettingsPressed));
        GetNode<Button>("VBox/Quit").Connect("pressed", this, nameof(QuitPressed));
    }

    private void HostPressed()
    {
        GetTree().ChangeScene("res://Scenes/Menus/Host.tscn");
    }

    private void JoinPressed()
    {
        GetTree().ChangeScene("res://Scenes/Menus/Join.tscn");
    }

    private void PerformancePressed()
    {
        GetTree().ChangeScene("res://Scenes/Menus/Performance.tscn");
    }

    private void CreditsPressed()
    {
        GetTree().ChangeScene("res://Scenes/Menus/Credits.tscn");
    }

    private void SettingsPressed()
    {
        GetTree().ChangeScene("res://Scenes/Menus/Settings.tscn");
    }

    private void QuitPressed()
    {
        GetTree().Quit();
    }
}
