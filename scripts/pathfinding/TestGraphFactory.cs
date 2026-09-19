public static class TestGraphFactory
{
    public static RoomGraph Create()
    {
        RoomGraph graph = new RoomGraph();

        RoomNode roomA = graph.AddRoom("A", 0, 0);
        RoomNode roomB = graph.AddRoom("B", 1, 0);
        RoomNode roomC = graph.AddRoom("C", 2, 0);

        RoomNode roomD = graph.AddRoom("D", 0, 1);
        RoomNode roomE = graph.AddRoom("E", 1, 1);
        RoomNode roomF = graph.AddRoom("F", 2, 1);

        graph.ConnectRooms(roomA, roomB);
        graph.ConnectRooms(roomB, roomC, 10);

        graph.ConnectRooms(roomA, roomD);
        graph.ConnectRooms(roomB, roomE);
        graph.ConnectRooms(roomD, roomE);

        graph.ConnectRooms(roomE, roomF);
        graph.ConnectRooms(roomF, roomC);

        return graph;
    }
}