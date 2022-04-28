using Godot;

public class LavaPool : Area
{
    // Node references
    private Global _global;
    
    public override void _Ready()
    {
        // Initialise node references
        _global = GetNode<Global>("/root/Global");
        
        // Connect signals
        Connect("body_entered", this, nameof(BodyEntered));
    }

    private void BodyEntered(Node body)
    {
        // The player this client controls touched the lava pool
        if (body.Name == Global.NetId.ToString())
        {
            // Tell everyone on the network that we touched the lava pool
            _global.Rpc(nameof(Global.PlayerShot), 0, Global.NetId.ToString());
        }
    }
}
