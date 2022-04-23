using Godot;

public class LeaderboardPlayer : HBoxContainer
{
    public int Position = 1;
    public Peer Peer = new Peer(1, "", Colors.Red);
    
    public override void _Ready()
    {
        Name = Peer.Id.ToString();
        
        GetNode<Label>("Position").Text = Position.ToString();
        GetNode<RichTextLabel>("Username").BbcodeText = $"[color=#{Peer.Colour.ToHtml()}]{Peer.Username}[/color]";
        GetNode<Label>("Accuracy").Text = $"{Peer.GetAccuracy()}%";
        GetNode<Label>("Frags").Text = Peer.Frags.ToString();
    }
}
