using Fusion;

public class PlayerInfo
{
    public string username;
    public int xp;
    public PlayerRef playerRef;

    public PlayerInfo(string username, int xp, PlayerRef playerRef)
    {
        this.username = username;
        this.xp = xp;
        this.playerRef = playerRef;
    }
}