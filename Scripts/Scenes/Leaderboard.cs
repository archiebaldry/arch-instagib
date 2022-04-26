using System.Collections.Generic;
using Godot;

public class Leaderboard : PanelContainer
{
    // Node references
    private Global _global;
    private VBoxContainer _players;
    
    public override void _Ready()
    {
        // Initalise node references
        _global = GetNode<Global>("/root/Global");
        _players = GetNode<VBoxContainer>("Players");
        
        // Connect signals
        _global.Connect(nameof(Global.PlayerStatsUpdated), this, nameof(UpdateBoard));

        UpdateBoard();
    }

    private void UpdateBoard()
    {
        // Clear existing players
        foreach (Node node in _players.GetChildren()) node.QueueFree();

        List<int> order = new List<int>();
        
        foreach (Peer player in Global.Players.Values)
        {
            for (int i = 0; i < order.Count; i++)
            {
                Peer playerToCompare = Global.Players[order[i]];
                
                // Insert player above playerToCompare (moving playerToCompare down) if either
                // of the following are true. Otherwise, look to the next player down.
                
                // A) They have more frags
                // B) They have the same amount of frags but better accuracy
                if (player.Frags > playerToCompare.Frags || player.Frags == playerToCompare.Frags && player.GetAccuracy() > playerToCompare.GetAccuracy())
                {
                    order.Insert(i, player.Id);
                    break;
                }
            }
            
            // Either _order was empty (i.e. this was the first player we tried to add to the board)
            // Or we decided this player is currently the worst and should be added to the end of _order
            order.Add(player.Id);
        }

        for (int i = 0; i < order.Count; i++)
        {
            // Position - Username - Accuracy% - Frags
            
            LeaderboardPlayer player = (LeaderboardPlayer) GD.Load<PackedScene>("res://Scenes/LeaderboardPlayer.tscn").Instance();
            
            player.Position = i + 1;
            player.Peer = Global.Players[order[i]];
            
            _players.AddChild(player);
        }
    }
}
