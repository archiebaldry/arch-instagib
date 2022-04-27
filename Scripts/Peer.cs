using Godot;

public class Peer
{
    public readonly int Id;
    public readonly string Username;
    public readonly Color Colour;
    public readonly int FirstSpawnIndex;

    public int Shots = 0;
    public int Frags = 0;
    public int Deaths = 0;

    public Peer(int id, string username, Color colour, int firstSpawnIndex)
    {
        Id = id;
        Username = username;
        Colour = colour;
        FirstSpawnIndex = firstSpawnIndex;
    }

    public int GetAccuracy()
    {
        if (Shots == 0) return 100;
        return (int) (Frags / (double) Shots) * 100;
    }
}
