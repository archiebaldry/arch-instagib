using Godot;

public class Game : Spatial
{
    // Node references
    private Label _info;
    private Leaderboard _leaderboard;
    private RichTextLabel _killFeed;
    private RichTextLabel _chatFeed;
    private LineEdit _chatInput;
    private Label _fpsIndicator;
    
    public override void _Ready()
    {
        // Initalise node references
        _info = GetNode<Label>("UI/VBox/Info/Label");
        _leaderboard = GetNode<Leaderboard>("UI/VBox/Leaderboard");
        _killFeed = GetNode<RichTextLabel>("UI/KillFeed");
        _chatFeed = GetNode<RichTextLabel>("UI/Chat/Feed");
        _chatInput = GetNode<LineEdit>("UI/Chat/Input");
        _fpsIndicator = GetNode<Label>("UI/FpsIndicator");
        
        // Decide whether to show FPS indicator from Global.cs
        _fpsIndicator.Visible = Global.FpsIndicator;
        
        // Capture the mouse
        Input.SetMouseMode(Input.MouseMode.Captured);
        
        // Spawn the players
        foreach (Peer peer in Global.Players.Values)
        {
            CollisionObject player;
            
            // If we are this player (Player)
            if (peer.Id == GetTree().GetNetworkUniqueId())
            {
                // Get instance of our Player scene
                player = (KinematicBody) GD.Load<PackedScene>("res://Scenes/Player.tscn").Instance();
                
                // Set the camera fov to our global value
                player.GetNode<Camera>("Camera").Fov = Global.Fov;
            }
            // If this is another client on the network (Puppet)
            else
            {
                // Get instance of our Puppet scene
                player = (Area) GD.Load<PackedScene>("res://Scenes/Puppet.tscn").Instance();

                // Create a new material
                Material material = new SpatialMaterial();
                
                // Set the material's colour to the peer's colour
                material.Set("albedo_color", peer.Colour);
                
                // Apply the material to the puppet
                player.GetNode<MeshInstance>("CollisionShape/MeshInstance").SetSurfaceMaterial(0, material);
            }
            
            // Set their name to the owner's id
            player.Name = peer.Id.ToString();
            
            // Set their position to the one found in Global.cs Players
            player.GlobalTransform = new Transform(Basis.Identity, peer.Position);
            
            // Add the player to the scene
            AddChild(player);
        }
    }

    public override void _Process(float delta)
    {
        _fpsIndicator.Text = Performance.GetMonitor(Performance.Monitor.TimeFps).ToString();
    }
}
