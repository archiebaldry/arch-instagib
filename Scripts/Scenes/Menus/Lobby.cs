using Godot;

public class Lobby : Control
{
    // Node references
    private RichTextLabel _playerList;
    private Button _start;
    
    public override void _Ready()
    {
        // Initialise node references
        _playerList = GetNode<RichTextLabel>("VBox/Panel/PlayerList");
        _start = GetNode<Button>("VBox/HBox/Start");
        
        // Connect signals
        GetNode<Button>("VBox/HBox/Leave").Connect("pressed", this, nameof(LeavePressed));
        _start.Connect("pressed", this, nameof(StartPressed));
    }

    public void LeavePressed()
    {
        if (GetTree().IsNetworkServer())
        {
            // Send the host back to Host.tscn
            GetTree().ChangeScene("res://Scenes/Menus/Host.tscn");
        }
        else
        {
            // Send any clients back to Join.tscn
            GetTree().ChangeScene("res://Scenes/Menus/Join.tscn");
        }
    }

    public void StartPressed()
    {
        
    }
}
