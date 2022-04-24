using Godot;

public class Credits : Control
{
    public override void _Ready()
    {
        // Connect signals
        GetNode<RichTextLabel>("VBox/RichText").Connect("meta_clicked", this, nameof(MetaClicked));
        GetNode<Button>("VBox/Back").Connect("pressed", this, nameof(BackPressed));
    }
    
    private void MetaClicked(string meta)
    {
        // Open hyperlink
        OS.ShellOpen(meta);
    }
    
    private void BackPressed()
    {
        GetTree().ChangeScene("res://Scenes/Menus/Main.tscn");
    }
}
