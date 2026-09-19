using System;
using System.Collections.Generic;

// Generates a random room graph constrained to a fixed grid of rooms.
public static class RandomGraphGenerator
{
    // The generated graph is limited to this rectangular room grid.
    private const int Width = 4;
    private const int Height = 3;

    // The four cardinal directions used when placing rooms adjacent to each other.
    private static readonly (int X, int Y)[] _directions =
    {
        (1, 0),   // Right
        (-1, 0),  // Left
        (0, 1),   // Down
        (0, -1)   // Up
    };

    private static readonly Random _random = new();

    // Returns true if the two rooms are already directly connected.
    private static bool AreConnected(RoomNode roomA, RoomNode roomB)
    {
        foreach (RoomConnection connection in roomA.Connections)
        {
            if (connection.Target == roomB)
            {
                return true;
            }
        }

        return false;
    }

    // Optionally adds extra random adjacency links between neighboring rooms.
    private static void AddExtraConnections(RoomGraph graph)
    {
        foreach (RoomNode room in graph.Rooms)
        {
            foreach (RoomNode otherRoom in graph.Rooms)
            {
                if (room == otherRoom)
                {
                    continue;
                }

                int horizontalDistance = Math.Abs(room.X - otherRoom.X);
                int verticalDistance = Math.Abs(room.Y - otherRoom.Y);

                // Only consider neighboring rooms that are exactly one tile apart.
                bool directlyAdjacent = horizontalDistance + verticalDistance == 1;

                if (!directlyAdjacent)
                {
                    continue;
                }

                if (AreConnected(room, otherRoom))
                {
                    continue;
                }

                // Randomly decide whether to connect the rooms.
                bool shouldConnect = _random.NextDouble() < 0.35;

                if (shouldConnect)
                {
                    graph.ConnectRooms(room, otherRoom, GetRandomConnectionCost());
                }
            }
        }
    }

    // Creates a randomized graph containing the requested number of rooms.
    public static RoomGraph Create(int roomCount)
    {
        int maximumRooms = Width * Height;

        if (roomCount < 1 || roomCount > maximumRooms)
        {
            throw new ArgumentOutOfRangeException(
                nameof(roomCount),
                $"Room count must be between 1 and {maximumRooms}."
            );
        }

        RoomGraph graph = new RoomGraph();

        // Track which grid positions are already occupied so rooms do not overlap.
        HashSet<(int X, int Y)> occupiedPositions = new();

        // Start the graph with a single room at the origin.
        graph.AddRoom("A", 0, 0);
        occupiedPositions.Add((0, 0));

        // Build outward from the initial room by selecting a random valid adjacent grid spot.
        for (int i = 1; i < roomCount; i++)
        {
            List<(RoomNode Parent, int X, int Y)> possiblePlacements =
                FindPossiblePlacements(graph, occupiedPositions);

            var chosenPlacement = possiblePlacements[_random.Next(possiblePlacements.Count)];

            string roomName = ((char)('A' + i)).ToString();

            RoomNode newRoom = graph.AddRoom(
                roomName,
                chosenPlacement.X,
                chosenPlacement.Y
            );

            occupiedPositions.Add((chosenPlacement.X, chosenPlacement.Y));

            graph.ConnectRooms(chosenPlacement.Parent, newRoom, GetRandomConnectionCost());
        }

        AddExtraConnections(graph);

        return graph;
    }

    // Finds all legal positions for a new room adjacent to the existing graph.
    private static List<(RoomNode Parent, int X, int Y)> FindPossiblePlacements(
        RoomGraph graph,
        HashSet<(int X, int Y)> occupiedPositions)
    {
        List<(RoomNode Parent, int X, int Y)> placements = new();

        foreach (RoomNode room in graph.Rooms)
        {
            foreach ((int X, int Y) direction in _directions)
            {
                int newX = room.X + direction.X;
                int newY = room.Y + direction.Y;

                // Ensure the candidate position is still within the grid bounds.
                bool insideGrid =
                    newX >= 0 &&
                    newX < Width &&
                    newY >= 0 &&
                    newY < Height;

                if (!insideGrid)
                {
                    continue;
                }

                // Skip positions that are already occupied by another room.
                bool alreadyOccupied = occupiedPositions.Contains((newX, newY));

                if (alreadyOccupied)
                {
                    continue;
                }

                placements.Add((room, newX, newY));
            }
        }

        return placements;
    }

    private static float GetRandomConnectionCost()
    {
        return _random.Next(1, 6);
    }
}