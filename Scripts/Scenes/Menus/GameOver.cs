using Godot;

public class GameOver : Control
{
    // Node references
    private Global _global;
    
    public override void _Ready()
    {
        // Initialise node references
        _global = GetNode<Global>("/root/Global");
        
        // Connect signals
        GetNode<Button>("VBox/MainMenu").Connect("pressed", this, nameof(MainMenuPressed));
        GetNode<Button>("VBox/Performance").Connect("pressed", this, nameof(PerformancePressed));
        GetNode<Button>("VBox/Quit").Connect("pressed", this, nameof(QuitPressed));
        
        Peer peer = Global.Players[Global.NetId]; // Get our information from Global.cs
        
        GetNode<RichTextLabel>("VBox/RichText").Text = $"Position: N/A\nShots: {peer.Shots}\nFrags: {peer.Frags}\nAccuracy: {peer.GetAccuracy()}%\nDeaths: {peer.Deaths}"; // TODO: Add position
    }

    private void SaveAndDisconnect()
    {
        // Save
        // TODO: Save stats to performance
        
        // Disconnect
        _global.Disconnect(true);
    }
    
    private void MainMenuPressed()
    {
        SaveAndDisconnect();
        
        GetTree().ChangeScene("res://Scenes/Menus/Main.tscn");
    }
    
    private void PerformancePressed()
    {
        SaveAndDisconnect();
        
        GetTree().ChangeScene("res://Scenes/Menus/Performance.tscn");
    }
    
    private void QuitPressed()
    {
        SaveAndDisconnect();
        
        GetTree().Quit();
    }
}
