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
                // Get instance of our Player scene
                player = (KinematicBody) GD.Load<PackedScene>("res://Scenes/Player.tscn").Instance();
                
                // Set the camera fov to our global value
                player.GetNode<Camera>("Camera").Fov = Global.Fov;
            }
            // If this is another client on the network
            else
            {
                // Get instance of our Puppet scene
                player = (Area) GD.Load<PackedScene>("res://Scenes/Puppet.tscn").Instance();
            }
            
            // Set their name to the owner's id
            player.Name = peer.Id.ToString();
            
            // Set their position to the one found in Global.cs Players
            player.GlobalTransform = new Transform(Basis.Identity, peer.Position);
            
            // Add the player to the scene
            AddChild(player);
        }
    }
}
