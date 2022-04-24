using Godot;

public class Music : AudioStreamPlayer
{
    public override void _Ready()
    {
        // Get our position from Global.cs
        Seek(Global.MenuMusicPosition);
    }
    
    public override void _ExitTree()
    {
        // Save our position to Global.cs
        Global.MenuMusicPosition = GetPlaybackPosition();
    }
}
