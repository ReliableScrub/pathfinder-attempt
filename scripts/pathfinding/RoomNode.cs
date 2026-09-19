using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

public class RoomNode
{
    public string Name { get; }
    public int X { get; }
    public int Y { get; }
    public List<RoomConnection> Connections { get; } = new();

    public RoomNode(string name, int x, int y)
    {
        Name = name;
        X = x;
        Y = y;
    }
}