public class RoomConnection
{
    public RoomNode Target { get; }
    public float Cost { get; }

    public RoomConnection(RoomNode target, float cost)
    {
        Target = target;
        Cost = cost;
    }
}
