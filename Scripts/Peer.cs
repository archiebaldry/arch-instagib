using Godot;

public class Peer
{
    public readonly int Id;
    public readonly string Username;
    public readonly Color Colour;

    public Vector3 Position = new Vector3(0, 7, 0);
    public int Shots = 0;
    public int Frags = 0;

    public Peer(int id, string username, Color colour)
    {
        Id = id;
        Username = username;
        Colour = colour;
    }

    public int GetAccuracy()
    {
        if (Shots == 0) return 100;
        return Frags / Shots;
    }
}
