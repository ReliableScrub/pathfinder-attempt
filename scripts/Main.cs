using Godot;
using System.Collections.Generic;

public partial class Main : Node2D
{
    private RoomGraph _graph;
    private AStarPathfinder _pathfinder;
    private GraphRenderer _graphRenderer;
    private Courier _courier;

    private RoomNode _startRoom;
    private RoomNode _goalRoom;
    private Label _costLabel;

    private List<RoomNode> _currentPath = new();


    public override void _Ready()
    {
        _graph = RandomGraphGenerator.Create(10);
        _courier = GetNode<Courier>("GraphRenderer/Courier");
        _costLabel = GetNode<Label>("UI/CostLabel");
        _pathfinder = new AStarPathfinder();

        _graphRenderer =
            GetNode<GraphRenderer>("GraphRenderer");

        RefreshDisplay();

        GD.Print("Click a room to choose the start.");
    }


    private void HandleRoomClick(RoomNode room)
    {
        if (_startRoom == null || _goalRoom != null)
        {
            SelectStartRoom(room);
        }
        else
        {
            SelectGoalRoom(room);
        }
    }


    private void SelectStartRoom(RoomNode room)
    {
        _startRoom = room;
        _goalRoom = null;

        _currentPath.Clear();

        _courier.PlaceAt(room, _graphRenderer);
        _costLabel.Text = "Route cost: -";
        GD.Print($"Start room selected: {room.Name}");

        RefreshDisplay();
    }


    private void SelectGoalRoom(RoomNode room)
    {
        _goalRoom = room;

        GD.Print($"Goal room selected: {room.Name}");

        RecalculatePath();
    }


    private void RecalculatePath()
    {
        _currentPath = _pathfinder.FindPath(_startRoom, _goalRoom);

        float totalCost = _graph.CalculatePathCost(_currentPath);

        _costLabel.Text = $"Route cost: {totalCost}";

        _courier.StartPath(_currentPath, _graphRenderer);

        GD.Print("Final path:");

        foreach (RoomNode room in _currentPath)
        {
            GD.Print(room.Name);
        }

        RefreshDisplay();
    }


    private void RefreshDisplay()
    {
        _graphRenderer.SetData(
            _graph,
            _currentPath,
            _startRoom,
            _goalRoom
        );
    }


    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton &&
            mouseButton.ButtonIndex == MouseButton.Left &&
            mouseButton.Pressed)
        {
            Vector2 mousePosition =
                _graphRenderer.ToLocal(
                    mouseButton.Position
                );

            RoomNode clickedRoom =
                _graphRenderer.GetRoomAtPosition(
                    mousePosition
                );

            if (clickedRoom != null)
            {
                HandleRoomClick(clickedRoom);
            }
        }
    }
}