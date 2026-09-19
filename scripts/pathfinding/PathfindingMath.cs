using Godot;
using System;

public static class PathfindingMath
{
    public static float ManhattanDistance(RoomNode a, RoomNode b)
    {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }
}
