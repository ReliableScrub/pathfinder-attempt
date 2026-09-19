using System.Collections.Generic;

public class RoomGraph
{
    public List<RoomNode> Rooms = new List<RoomNode>();

    public RoomNode AddRoom(string name, int x, int y)
    {
        RoomNode room = new RoomNode(name, x, y);
        Rooms.Add(room);
        return room;
    }

    public void ConnectRooms(RoomNode roomA, RoomNode roomB, float cost = 1)
    {
        roomA.Connections.Add(new RoomConnection(roomB, cost));
        roomB.Connections.Add(new RoomConnection(roomA, cost));
    }

    public float CalculatePathCost(List<RoomNode> path)
    {
        float totalCost = 0;

        for (int i = 0; i < path.Count - 1; i++)
        {
            RoomNode currentRoom = path[i];
            RoomNode nextRoom = path[i + 1];

            // Find the connection to the next room and add its cost.
            foreach (RoomConnection connection in currentRoom.Connections)
            {
                if (connection.Target == nextRoom)
                {
                    totalCost += connection.Cost;
                    break;
                }
            }
        }

        return totalCost;
    }
}
