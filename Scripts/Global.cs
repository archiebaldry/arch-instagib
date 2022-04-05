using Godot;

public class Global
{
    // Options
    public static string Username = "Player";
    public static Color Colour = new Color(255F, 0F, 0F);
    public static Vector2 Sensitivity = new Vector2(0.2F, 0.2F);
    public static Vector2 Fov = new Vector2(90, 90);
    
    // Host and Join
    public static string Address = "";
    public static int Port = 2710;
    public static bool Upnp = false;
}
