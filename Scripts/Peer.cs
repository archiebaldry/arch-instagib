using Godot;

public class Peer
{
    public readonly int Id;
    public readonly string Username;
    public readonly Color Colour;
    
    public Peer(int id, string username, Color colour)
    {
        Id = id;
        Username = username;
        Colour = colour;
    }
}
