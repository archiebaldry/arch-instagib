using System;
using System.Collections.Generic;
using Godot;

public class Game : Spatial
{
    // Private variables
    private List<Vector3> _spawnpoints = new List<Vector3>();

    // Node references
    private Global _global;
    private Label _info;
    private Leaderboard _leaderboard;
    private RichTextLabel _killFeed;
    private RichTextLabel _chatFeed;
    private LineEdit _chatInput;
    private Label _fpsIndicator;
    private Timer _gameTimer;
    
    public override void _Ready()
    {
        // Initalise node references
        _global = GetNode<Global>("/root/Global");
        _info = GetNode<Label>("UI/VBox/Info/Label");
        _leaderboard = GetNode<Leaderboard>("UI/VBox/Leaderboard");
        _killFeed = GetNode<RichTextLabel>("UI/KillFeed");
        _chatFeed = GetNode<RichTextLabel>("UI/Chat/Feed");
        _chatInput = GetNode<LineEdit>("UI/Chat/Input");
        _fpsIndicator = GetNode<Label>("UI/FpsIndicator");
        _gameTimer = GetNode<Timer>("GameTimer");
        
        // Connect signals
        _global.Connect(nameof(Global.PlayerRespawned), this, nameof(RespawnPlayer));
        _gameTimer.Connect("timeout", this, nameof(TimeLimitReached));
        
        _fpsIndicator.Visible = Global.FpsIndicator; // Decide whether to show FPS indicator from Global.cs
        
        Input.SetMouseMode(Input.MouseMode.Captured); // Capture the mouse

        _gameTimer.WaitTime = Global.TimeLimit * 60; // Set our wait time to whatever's in Global.cs
        _gameTimer.Start(); // Start the timer
        
        // Fetch our spawnpoints
        foreach (Position3D spawnpoint in GetNode<Spatial>("Spawnpoints").GetChildren())
        {
            _spawnpoints.Add(spawnpoint.GlobalTransform.origin);
        }
        
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
            player.GlobalTransform = new Transform(Basis.Identity, _spawnpoints[peer.FirstSpawnIndex]);
            
            // Add the player to the scene
            AddChild(player);
        }
    }

    public override void _Process(float delta)
    {
        // Update game info
        TimeSpan timeLeft = TimeSpan.FromSeconds(_gameTimer.TimeLeft);
        _info.Text = $"Deathmatch\nFrag Limit: {Global.FragLimit}\nTime Limit: {timeLeft:mm\\:ss}";
        
        // Update FPS indicator
        _fpsIndicator.Text = Performance.GetMonitor(Performance.Monitor.TimeFps).ToString();
    }

    private void TimeLimitReached()
    {
        _global.EndGame(1); // Tell Global.cs that the time limit has been reached and we should end the game
    }

    private void RespawnPlayer(int id, int spawnIndex)
    {
        GetNode<CollisionObject>(id.ToString()).GlobalTransform = new Transform(Basis.Identity, _spawnpoints[spawnIndex]);
    }
}
