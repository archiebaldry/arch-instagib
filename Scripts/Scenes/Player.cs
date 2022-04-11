using Godot;

public class Player : KinematicBody
{
    // Private constants
    private const int Speed = 5;
    
    public override void _PhysicsProcess(float delta)
    {
        // Get our direction from keyboard input
        Vector2 direction = Input.GetVector("move_left", "move_right", "move_forward", "move_back");

        Vector3 velocity = new Vector3(direction.x, 0, direction.y);
        
        MoveAndSlide(velocity * Speed, Vector3.Up);
    }
}
