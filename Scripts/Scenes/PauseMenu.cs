using Godot;

public class PauseMenu : ColorRect
{
    private VBoxContainer _main;
    private VBoxContainer _options;
    private SpinBox _sensitivityX;
    private SpinBox _sensitivityY;
    private HSlider _fov;

    public override void _Ready()
    {
        // Initialise node references
        _main = GetNode<VBoxContainer>("Main");
        _options = GetNode<VBoxContainer>("Options");
        _sensitivityX = _options.GetNode<SpinBox>("Sensitivity/VBox/HBox/X");
        _sensitivityY = _options.GetNode<SpinBox>("Sensitivity/VBox/HBox/Y");
        _fov = _options.GetNode<HSlider>("Fov/VBox/HSlider");
        
        // Connect signals
        _main.GetNode<Button>("Resume").Connect("pressed", this, nameof(ResumePressed));
        _main.GetNode<Button>("Options").Connect("pressed", this, nameof(OptionsPressed));
        _main.GetNode<Button>("Disconnect").Connect("pressed", this, nameof(DisconnectPressed));
        _fov.Connect("value_changed", this, nameof(SetPlayerFov));
        _options.GetNode<Button>("HBox/Back").Connect("pressed", this, nameof(BackPressed));
        _options.GetNode<Button>("HBox/Save").Connect("pressed", this, nameof(SavePressed));
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // Only listen for pause action
        if (!@event.IsActionPressed("pause")) return;

        // Resume
        if (Visible)
        {
            SetPlayerFov(Global.Fov); // Revert player's fov back to global value
            ResumePressed(); // Resume the game
        }
        
        // Pause
        else
        {
            Global.GamePaused = true;
            Input.SetMouseMode(Input.MouseMode.Visible);
            
            _main.Visible = true; // Show main buttons
            _options.Visible = false; // Hide options
            
            Visible = true; // Show pause menu
        }
    }

    private void ResumePressed()
    {
        Global.GamePaused = false;
        Input.SetMouseMode(Input.MouseMode.Captured);
        
        // Hide pause menu
        Visible = false;
    }

    private void OptionsPressed()
    {
        // Populate fields from Global.cs
        _sensitivityX.Value = Global.Sensitivity.x;
        _sensitivityY.Value = Global.Sensitivity.y;
        _fov.Value = Global.Fov;
        
        _main.Visible = false; // Hide main buttons
        _options.Visible = true; // Show options
    }

    private void DisconnectPressed()
    {
        
    }

    private void BackPressed()
    {
        // Revert player's fov back to global value
        SetPlayerFov(Global.Fov);

        _main.Visible = true; // Show main buttons
        _options.Visible = false; // Hide options
    }

    private void SavePressed()
    {
        // Save fields to Global.cs
        Global.Sensitivity.x = (float) _sensitivityX.Value;
        Global.Sensitivity.y = (float) _sensitivityY.Value;
        Global.Fov = (int) _fov.Value;
        
        _main.Visible = true; // Show main buttons
        _options.Visible = false; // Hide options
    }

    private void SetPlayerFov(int value)
    {
        // Get the player's camera and set it's fov to the value passed
        GetTree().Root.GetNode<Camera>("Game/" + GetTree().GetNetworkUniqueId() + "/Camera").Fov = value;
    }
}
