using Godot;

public class Alert : ColorRect
{
    private bool _dismissible; // Can this alert be dismissed?
    private Control _lastFocusOwner; // Which element held the focus of the scene before the alert?

    public override void _Input(InputEvent @event)
    {
        if (@event.IsAction("hide_alert") && _dismissible)
        {
            Visible = false;
            
            _lastFocusOwner?.GrabFocus(); // If there was previously an element with focus, make it grab the focus back
        }
    }

    public void Popup(string text, bool dismissible = false, bool error = false)
    {
        _dismissible = dismissible;
        _lastFocusOwner = GetParent<Control>().GetFocusOwner(); // Store the element which last held focus before the alert
        
        Label label = GetNode<Label>("Label");

        // If the alert is dismissible, add a dismiss message to the end of the text
        label.Text = dismissible ? text + "\n(click anywhere to hide)" : text;

        // If the alert is an error, make the text red
        label.AddColorOverride("font_color", error ? Colors.Red : GetColor("font_color", "Label"));

        Visible = true;
        
        GrabFocus(); // Take the focus of the scene
    }
}
