using Godot;

public class Player : KinematicBody
{
    // Private constants
    private const int Speed = 5;
    
    // Node references
    private Global _global;

    public override void _Ready()
    {
        // Initialise node references
        _global = GetNode<Global>("/root/Global");
    }

    public override void _PhysicsProcess(float delta)
    {
        // Get our direction from keyboard input
        Vector2 direction = Input.GetVector("move_left", "move_right", "move_forward", "move_back");

        Vector3 velocity = new Vector3(direction.x, 0, direction.y);
        
        // Move the player 
        MoveAndSlide(velocity * Speed, Vector3.Up);
        
        // Check if we actually moved
        if (!velocity.Equals(Vector3.Zero))
        {
            // Tell everyone on the network that we've moved and send them our position
            _global.RpcUnreliable(nameof(Global.SetPlayerPosition), GlobalTransform.origin);
        }
    }
}
