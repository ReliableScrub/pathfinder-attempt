using Godot;
using System.Collections.Generic;

public partial class Courier : AnimatedSprite2D
{
    private List<RoomNode> _path = new();
    private GraphRenderer _graphRenderer;
    private int _targetIndex;
    private bool _isMoving;
    private const float MoveSpeed = 140f;

    public override void _Ready()
    {
        Hide();
    }

    public void PlaceAt(RoomNode room, GraphRenderer graphRenderer)
    {
        _graphRenderer = graphRenderer;
        Position = _graphRenderer.GetScreenPosition(room);
        _isMoving = false;
        Show();
        Play("idle");
    }

    public void StartPath(List<RoomNode> path, GraphRenderer graphRenderer)
    {
        _path = path;
        _graphRenderer = graphRenderer;

        if (_path.Count == 0)
        {
            _isMoving = false;
            Play("idle");
            return;
        }

        Position = _graphRenderer.GetScreenPosition(_path[0]);

        _targetIndex = 1;
        _isMoving = _path.Count > 1;

        if (!_isMoving)
        {
            Play("idle");
        }
    }

    private void UpdateMovementAnimation(Vector2 direction)
    {
        string animationName;

        if (Mathf.Abs(direction.X) > Mathf.Abs(direction.Y))
        {
            if (direction.X > 0)
            {
                animationName = "walk_right";
            }
            else
            {
                animationName = "walk_left";
            }
        }
        else
        {
            if (direction.Y > 0)
            {
                animationName = "walk_down";
            }
            else
            {
                animationName = "walk_up";
            }
        }

        if (Animation != animationName)
        {
            Play(animationName);
        }
    }

    public override void _Process(double delta)
    {
        if (!_isMoving)
        {
            return;
        }

        RoomNode targetRoom = _path[_targetIndex];

        Vector2 targetPosition = _graphRenderer.GetScreenPosition(targetRoom);

        Vector2 direction = (targetPosition - Position);

        UpdateMovementAnimation(direction);

        Position = Position.MoveToward(targetPosition, MoveSpeed * (float)delta);

        if (Position.DistanceTo(targetPosition) < 1f)
        {
            Position = targetPosition;
            _targetIndex++;

            if (_targetIndex >= _path.Count)
            {
                _isMoving = false;
                Play("idle");
            }
        }
    }


}
