using Godot;
using System.Collections.Generic;

// Renders the room graph and pathfinding visualization in the scene.
public partial class GraphRenderer : Node2D
{
    private RoomGraph _graph;
    private List<RoomNode> _path = new();
    private RoomNode _startRoom;
    private RoomNode _goalRoom;

    // Default font used for drawing room names and connection costs.
    private readonly Font _font = ThemeDB.FallbackFont;

    // Distance between adjacent room nodes in the grid layout.
    private const float GridSpacing = 180f;

    // Screen offset so the graph is centered within the viewport.
    private readonly Vector2 _screenOffset =
        new Vector2(150f, 150f);

    // Stores the graph and path data and re-renders the node.
    public void SetData(
        RoomGraph graph,
        List<RoomNode> path)
    {
        SetData(graph, path, null, null);
    }

    // Stores the graph, active path, and start/goal markers.
    public void SetData(
        RoomGraph graph,
        List<RoomNode> path,
        RoomNode startRoom,
        RoomNode goalRoom)
    {
        _graph = graph;
        _path = path;
        _startRoom = startRoom;
        _goalRoom = goalRoom;

        QueueRedraw();
    }

    // Converts a room's grid coordinates to screen-space coordinates.
    public Vector2 GetScreenPosition(RoomNode room)
    {
        return new Vector2(room.X * GridSpacing, room.Y * GridSpacing) + _screenOffset;
    }

    // Returns the room located under a given screen position, if any.
    public RoomNode GetRoomAtPosition(Vector2 position)
    {
        if (_graph == null)
        {
            return null;
        }

        foreach (RoomNode room in _graph.Rooms)
        {
            Vector2 roomPosition = GetScreenPosition(room);

            float distance = roomPosition.DistanceTo(position);

            // Treat a room as hit if the cursor is near its drawn circle.
            if (distance <= 32f)
            {
                return room;
            }
        }

        return null;
    }

    // Draws all room-to-room connections and their movement costs.
    private void DrawConnections()
    {
        foreach (RoomNode room in _graph.Rooms)
        {
            Vector2 roomPosition =
                GetScreenPosition(room);

            foreach (RoomConnection connection in room.Connections)
            {
                RoomNode targetRoom = connection.Target;

                // Each connection exists in both directions, so draw it once.
                if (string.Compare(room.Name, targetRoom.Name) >= 0)
                {
                    continue;
                }

                Vector2 targetPosition =
                    GetScreenPosition(targetRoom);

                DrawLine(
                    roomPosition,
                    targetPosition,
                    Colors.DarkGray,
                    4f
                );

                Vector2 middlePosition =
                    (roomPosition + targetPosition) / 2f;

                // Draw the cost label between the connected rooms.
                DrawString(
                    _font,
                    middlePosition + new Vector2(-15f, -10f),
                    connection.Cost.ToString(),
                    HorizontalAlignment.Center,
                    30f,
                    16
                );
            }
        }
    }

    // Draws the highlighted path as a thick green line.
    private void DrawPath()
    {
        for (int i = 0; i < _path.Count - 1; i++)
        {
            Vector2 currentPosition = GetScreenPosition(_path[i]);
            Vector2 endPosition = GetScreenPosition(_path[i + 1]);
            DrawLine(currentPosition, endPosition, Colors.Green, 10f);
        }
    }

    // Draws each room node with a color based on its state.
    private void DrawRooms()
    {
        foreach (RoomNode room in _graph.Rooms)
        {
            Vector2 position = GetScreenPosition(room);

            Color roomColor = Colors.White;

            // A room on the solved path is highlighted in green.
            if (_path.Contains(room))
            {
                roomColor = Colors.LimeGreen;
            }

            // Start room gets a blue highlight.
            if (room == _startRoom)
            {
                roomColor = Colors.DodgerBlue;
            }

            // Goal room gets an orange highlight.
            if (room == _goalRoom)
            {
                roomColor = Colors.Orange;
            }

            DrawCircle(position, 32f, roomColor);

            // Label each room with its name.
            DrawString(_font, position + new Vector2(-20f, 7f), room.Name, HorizontalAlignment.Center, 40f, 20);
        }
    }

    // Draws the graph whenever the node is redrawn.
    public override void _Draw()
    {
        if (_graph == null)
        {
            return;
        }

        DrawConnections();
        DrawPath();
        DrawRooms();
    }
}
