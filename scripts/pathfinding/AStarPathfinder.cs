using Godot;
using System.Collections.Generic;

public class AStarPathfinder
{
    // Finds the lowest-cost route from the starting room to the goal room.
    public List<RoomNode> FindPath(RoomNode start, RoomNode goal)
    {
        // Tracks which rooms are still pending evaluation.
        List<RoomNode> roomsToCheck = new();

        // Stores the known cost to reach each room from the start.
        Dictionary<RoomNode, float> costSoFar = new();

        // Stores the previous room used to reach each room, allowing path reconstruction.
        Dictionary<RoomNode, RoomNode> cameFrom = new();

        // Initialize the search with the starting room.
        roomsToCheck.Add(start);
        costSoFar[start] = 0;
        cameFrom[start] = null;

        // Keep exploring until no more rooms are left to inspect.
        while (roomsToCheck.Count > 0)
        {
            // Select the room with the lowest estimated total cost.
            RoomNode currentRoom = GetBestRoom(roomsToCheck, goal, costSoFar);

            // Log the room being evaluated for debugging.
            GD.Print($"Checking room {currentRoom.Name}"); // Print the name of the room being checked

            // If the goal room is reached, reconstruct and return the path.
            if (currentRoom == goal)
            {
                return ReconstructPath(cameFrom, start, goal);
            }

            // Remove the current room once it has been processed.
            roomsToCheck.Remove(currentRoom);

            // Explore each connection from the current room to neighboring rooms.
            foreach (RoomConnection connection in currentRoom.Connections)
            {
                RoomNode neighbor = connection.Target;

                // Calculate the cost to reach the neighbor through the current room.
                float newCost = costSoFar[currentRoom] + connection.Cost;

                // Update the best known route to this neighbor when a cheaper path is found.
                if (!costSoFar.ContainsKey(connection.Target) || newCost < costSoFar[connection.Target])
                {
                    costSoFar[neighbor] = newCost;
                    cameFrom[neighbor] = currentRoom;

                    // Add the neighbor to the pending list if it is not already there.
                    if (!roomsToCheck.Contains(neighbor))
                    {
                        roomsToCheck.Add(neighbor);
                    }
                }
            }
        }

        // No valid path exists between the start and goal.
        return new List<RoomNode>(); // Return an empty path if no path is found
    }

    // Rebuilds the route from the goal back to the start using the parent map.
    private List<RoomNode> ReconstructPath(Dictionary<RoomNode, RoomNode> cameFrom, RoomNode start, RoomNode goal)
    {
        // Start at the goal and walk backward through the previous-room links.
        List<RoomNode> path = new();
        RoomNode currentRoom = goal;

        path.Add(currentRoom);

        while (currentRoom != start)
        {
            currentRoom = cameFrom[currentRoom];
            path.Add(currentRoom);
        }

        // Reverse the list so the path runs from start to goal.
        path.Reverse();

        return path;
    }

    // Chooses the room that appears to be the best next step based on cost + heuristic.
    private RoomNode GetBestRoom(
        List<RoomNode> roomsToCheck,
        RoomNode goal,
        Dictionary<RoomNode, float> costSoFar)
    {
        RoomNode bestRoom = roomsToCheck[0];

        foreach (RoomNode room in roomsToCheck)
        {
            float roomTotal = GetEstimatedTotalCost(room, goal, costSoFar);
            float bestTotal = GetEstimatedTotalCost(bestRoom, goal, costSoFar);

            if (roomTotal < bestTotal)
            {
                bestRoom = room;
            }
        }
        return bestRoom;
    }

    // Estimates the total cost from the current room to the goal.
    // It combines the known travel cost so far with a heuristic estimate of remaining distance.
    private float GetEstimatedTotalCost(
        RoomNode current,
        RoomNode goal,
        Dictionary<RoomNode, float> costSoFar)
    {
        float actualCost = costSoFar[current];

        float estimatedRemaining = PathfindingMath.ManhattanDistance(current, goal);

        return actualCost + estimatedRemaining;
    }
}
