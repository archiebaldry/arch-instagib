using Godot;

public class Player : KinematicBody
{
    // Private constants
    private const int Speed = 7;
    private const int Acceleration = 7;
    private const int AirAcceleration = 1;
    private const float Gravity = 9.8F;
    private const int Jump = 5;
    
    // Private variables
    private int _acceleration;
    private Vector2 _direction;
    private Vector3 _velocity;
    private Vector3 _snap;
    private Vector3 _gravity;
    private Vector3 _movement;
    private Vector3 _moveResult;
    
    // Node references
    private Global _global;
    private Camera _camera;
    private RayCast _rayCast;
    private Label _debug;

    public override void _Ready()
    {
        // Initialise node references
        _global = GetNode<Global>("/root/Global");
        _camera = GetNode<Camera>("Camera");
        _rayCast = GetNode<RayCast>("Camera/RayCast");
        _debug = GetNode<Label>("Debug");
        
        // Decide whether to show debug from Global.cs
        _debug.Visible = Global.PlayerDebug;
    }

    public override void _Process(float delta)
    {
        _debug.Text = $"Acceleration: {_acceleration}\nDirection: {Vec2Str(_direction)}\nVelocity: {Vec3Str(_velocity)}\nSnap: {Vec3Str(_snap)}\nGravity: {Vec3Str(_gravity)}\nMovement: {Vec3Str(_movement)}\nMove Result: {Vec3Str(_moveResult)}";
    }

    public override void _PhysicsProcess(float delta)
    {
        // SHOOTING

        if (!Global.GamePaused && Input.IsActionJustPressed("fire"))
        {
            // Get the object colliding with our ray cast
            Object collider = _rayCast.GetCollider();

            // Hit
            if (collider is Area area && area.IsInGroup("Puppets"))
            {
                int target = area.Name.ToInt();
                
                // Tell everyone on the network that we've made a frag
                _global.Rpc(nameof(Global.PlayerShot), target);
            }
            // Miss
            else
            {
                // Tell everyone on the network that we've taken a shot and missed
                _global.Rpc(nameof(Global.PlayerShot), -1);
            }
        }
        
        // MOVEMENT
        
        // Provided the game is unpaused, get the direction of our keyboard input (W, A, S and D) and rotate it by the direction we are facing
        _direction = Global.GamePaused ? Vector2.Zero : Input.GetVector("move_left", "move_right", "move_forward", "move_back").Rotated(-Rotation.y);
        
        if (IsOnFloor()) // We are on the ground
        {
            _snap = -GetFloorNormal();
            _acceleration = Acceleration;
            _gravity = Vector3.Zero;
        }
        else // We are in the air
        {
            _snap = Vector3.Down;
            _acceleration = AirAcceleration;
            _gravity += Vector3.Down * Gravity * delta;
        }

        // Handle jumping
        if (!Global.GamePaused && Input.IsActionPressed("jump") && IsOnFloor())
        {
            _snap = Vector3.Zero;
            _gravity = Vector3.Up * Jump;
        }

        // Calculate velocity and movement
        _velocity = _velocity.LinearInterpolate(new Vector3(_direction.x, 0, _direction.y) * Speed, _acceleration * delta);
        _movement = _velocity + _gravity;

        // Apply movement
        _moveResult = MoveAndSlideWithSnap(_movement, _snap, Vector3.Up);

        // Check if we actually moved
        if (!_movement.Equals(Vector3.Zero))
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
    
    private static string Vec2Str(Vector2 v)
    {
        // Return vector as string with X and Y rounded to 2 d.p.
        return $"({v.x:0.##}, {v.y:0.##})";
    }
    
    private static string Vec3Str(Vector3 v)
    {
        // Return vector as string with X, Y and Z rounded to 2 d.p.
        return $"({v.x:0.##}, {v.y:0.##}, {v.z:0.##})";
    }
}
