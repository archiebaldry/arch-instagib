using System;
using Godot;

public class GameMusic : AudioStreamPlayer
{
    private int _trackNo; // Track we are currently on
    
    public override void _Ready()
    {
        // Connect finished() signal
        Connect("finished", this, nameof(TrackFinished));
        
        // Generate a random number between 1 and 15 to decide which track we start on
        _trackNo = new Random().Next(1, 16);
        
        // Play that track
        PlayTrack();
    }

    private void TrackFinished()
    {
        // Get the next track (make sure to loop back to 1 once we reach the end)
        _trackNo = Mathf.Clamp(_trackNo + 1, 1, 15);
        
        // Play that track
        PlayTrack();
    }

    private void PlayTrack()
    {
        // Set our stream to that track
        Stream = GD.Load<AudioStreamSample>($"res://Assets/Music/Rufus/Track {_trackNo}.wav");
        
        // Play the track
        Play();
    }
}
