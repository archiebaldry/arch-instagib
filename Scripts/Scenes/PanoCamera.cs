using Godot;

public class PanoCamera : Camera
{
    // Private constants
    private const float Length = 200.0F;
    
    // Private variables
    private float _elapsed;
    
    public override void _Ready()
    {
        // Get our _elapsed from Global.cs
        _elapsed = Global.PanoCameraElapsed;
    }
    
    public override void _Process(float delta)
    {
        // Add delta (seconds since last frame) to _elapsed
        _elapsed += delta;

        // Loop back to 0 once we reach our Length
        if (_elapsed > Length)
        {
            _elapsed -= Length;
        }

        // Set camera rotation based on _elapsed
        Vector3 rotation = RotationDegrees;
        rotation.y = -360.0F * (_elapsed / Length);
        RotationDegrees = rotation;
    }

    public override void _ExitTree()
    {
        // Save our _elapsed to Global.cs
        Global.PanoCameraElapsed = _elapsed;
    }
}
