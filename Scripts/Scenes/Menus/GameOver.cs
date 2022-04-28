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
        
        // Save stats to Global.cs
        Global.TotalGames += 1;
        Global.TotalShots += peer.Shots;
        Global.TotalFrags += peer.Frags;
        Global.TotalDeaths += peer.Deaths;
        _global.SaveDataToFile();

        GetNode<RichTextLabel>("VBox/RichText").Text = $"Position: N/A\nShots: {peer.Shots}\nFrags: {peer.Frags}\nAccuracy: {peer.GetAccuracy()}%\nDeaths: {peer.Deaths}"; // TODO: Add position
    }
    
    private void MainMenuPressed()
    {
        _global.Disconnect();
    }
    
    private void PerformancePressed()
    {
        _global.Disconnect(true);
        
        GetTree().ChangeScene("res://Scenes/Menus/Performance.tscn");
    }
    
    private void QuitPressed()
    {
        _global.Disconnect(true);
        
        GetTree().Quit();
    }
}
