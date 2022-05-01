using Godot;

public class Performance : Control
{
    public override void _Ready()
    {
        // Connect signals
        GetNode<Button>("VBox/Back").Connect("pressed", this, nameof(BackPressed));
        
        // Calulate the player's stats from Global.cs values
        float fdRatio = Global.TotalDeaths == 0 ? 0 : Global.TotalFrags / (float) Global.TotalDeaths;
        float accuracy = Global.TotalShots == 0 ? 100 : Global.TotalFrags / (float) Global.TotalShots * 100;
        float avgShots = Global.TotalGames == 0 ? 0 : Global.TotalShots / (float) Global.TotalGames;
        float avgFrags = Global.TotalGames == 0 ? 0 : Global.TotalFrags / (float) Global.TotalGames;
        float avgDeaths = Global.TotalGames == 0 ? 0 : Global.TotalDeaths / (float) Global.TotalGames;

        // Display the player's stats in our RichTextLabel
        GetNode<RichTextLabel>("VBox/RichText").BbcodeText = $@"Total games: {Global.TotalGames}
Total shots: {Global.TotalShots}
Total frags: [color=aqua]{Global.TotalFrags}[/color]
Total deaths: [color=fuchsia]{Global.TotalDeaths}[/color]

F/D ratio: [color=aqua]{fdRatio:0.##}[/color]
Accuracy: [color=aqua]{accuracy:0.#}%[/color]

Avg. shots per game: {avgShots:0.##}
Avg. frags per game: [color=aqua]{avgFrags:0.##}[/color]
Avg. deaths per game: [color=fuchsia]{avgDeaths:0.##}[/color]

You should aim for the statistics highlighted in [color=aqua]blue[/color] to be high and the ones in [color=fuchsia]pink[/color] to be low.";
    }
    
    private void BackPressed()
    {
        GetTree().ChangeScene("res://Scenes/Menus/Main.tscn");
    }
}
