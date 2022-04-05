using Godot;

public class Alert : ColorRect
{
    // Public variables
    public bool Dismissible = false; // Can this alert be dismissed?

    // Private variables
    private Control _lastFocusOwner; // Which element held the focus of the scene before the alert?

    public override void _Input(InputEvent @event)
    {
        if (@event.IsAction("hide_alert") && Dismissible)
        {
            Visible = false;
            
            _lastFocusOwner?.GrabFocus(); // If there was previously an element with focus, make it grab the focus back
        }
    }

    public void Popup(string text, bool dismissible = false)
    {
        _lastFocusOwner = GetParent<Control>().GetFocusOwner(); // Store the element which last held focus before the alert
        
        Label label = GetNode<Label>("Label");

        if (dismissible)
        {
            label.Text = text + "\n(click anywhere to hide)";
        }
        else
        {
            label.Text = text;
        }

        Dismissible = dismissible;
        Visible = true;
        
        GrabFocus(); // Take the focus of the scene
    }
}
