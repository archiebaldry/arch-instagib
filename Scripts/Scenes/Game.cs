using Godot;

public class Game : Spatial
{
    public override void _Ready()
    {
        foreach (Peer peer in Global.Players.Values)
        {
            CollisionObject player;
            
            // If we are this player
            if (peer.Id == GetTree().GetNetworkUniqueId())
            {
                player = (KinematicBody) GD.Load<PackedScene>("res://Scenes/Player.tscn").Instance();
            }
            // If this is another client on the network
            else
            {
                player = (Area) GD.Load<PackedScene>("res://Scenes/Puppet.tscn").Instance();
            }
            
            // Set their position to the one found in Global.cs Players
            player.GlobalTransform = new Transform(Basis.Identity, peer.Position);
            
            // Add the player to the scene
            AddChild(player);
        }
    }
}
