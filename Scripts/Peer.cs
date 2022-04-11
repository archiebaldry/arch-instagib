using Godot;

public class Peer
{
    public readonly int Id;
    public readonly string Username;
    public readonly Color Colour;

    public Vector3 Position = Vector3.Zero;

    public Peer(int id, string username, Color colour)
    {
        Id = id;
        Username = username;
        Colour = colour;
    }
}
