using Godot;

public class Alert : ColorRect
{
    // Public variables
    public bool Dismissable = false; // Can this alert be dismissed?

    // Private variables
    private Control _lastFocusOwner; // Which element held the focus of the scene before the alert?

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsAction("hide_alert") && Dismissable)
        {
            Visible = false;
            
            _lastFocusOwner?.GrabFocus(); // If there was previously an element with focus, make it grab the focus back
        }
    }

    public void Popup(string text, bool dismissable = true)
    {
        _lastFocusOwner = GetParent<Control>().GetFocusOwner(); // Store the element which last held focus before the alert
        
        GetNode<Label>("Label").Text = text;

        Visible = true;
        
        GrabFocus(); // Take the focus of the scene
    }
}
