using Godot;

public class Player : KinematicBody
{
    // Private constants
    private const int Speed = 5;
    
    // Node references
    private Global _global;
    private Camera _camera;

    public override void _Ready()
    {
        // Initialise node references
        _global = GetNode<Global>("/root/Global");
        _camera = GetNode<Camera>("Camera");
    }

    public override void _PhysicsProcess(float delta)
    {
        // Get the direction of our keyboard input (W, A, S and D) and rotate it by the direction we are facing
        Vector2 direction = Input.GetVector("move_left", "move_right", "move_forward", "move_back").Rotated(-Rotation.y);
        
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
    
    public override void _Input(InputEvent @event)
    {
        // Ignore input if the game is paused or the input wasn't mouse movement
        if (Global.GamePaused || !(@event is InputEventMouseMotion mouseEvent)) return;
        
        // Rotate our camera for pitch and rotate our body for yaw
        // Must be done in this order to avoid unexpected behaviour
        _camera.RotateX(Mathf.Deg2Rad(-mouseEvent.Relative.y * Global.Sensitivity.y * 0.02F)); // Pitch
        RotateY(Mathf.Deg2Rad(-mouseEvent.Relative.x * Global.Sensitivity.x * 0.02F)); // Yaw
        
        // Clamp our pitch between looking straight up at the sky and
        // looking straight down to the floor to avoid weird behaviour
        // where the player can roll their head more than 180 degrees
        Vector3 headRotation = _camera.RotationDegrees;
        headRotation.x = Mathf.Clamp(headRotation.x, -89.0F, 89.0F);
        _camera.RotationDegrees = headRotation;
    }
}
