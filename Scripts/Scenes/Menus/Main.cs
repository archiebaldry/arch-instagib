using Godot;

public class Main : Control
{
    public override void _Ready()
    {
        // Connect signals
        GetNode<Button>("VBox/Host").Connect("pressed", this, nameof(HostPressed));
        GetNode<Button>("VBox/Join").Connect("pressed", this, nameof(JoinPressed));
        GetNode<Button>("VBox/Options").Connect("pressed", this, nameof(OptionsPressed));
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

    private void OptionsPressed()
    {
        GetTree().ChangeScene("res://Scenes/Menus/Options.tscn");
    }

    private void QuitPressed()
    {
        GetTree().Quit();
    }
}
