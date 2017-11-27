using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Interfaces;
using UnityEngine;

public class Player : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
    }

    public IBoard Board;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            HandleMove(Direction.Left);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            HandleMove(Direction.Right);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            HandleMove(Direction.Up);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            HandleMove(Direction.Down);
        }
    }

    enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public Vector2 Coord;

    public Action<Player, Vector2> MoveToAction;

    void HandleMove(Direction dir)
    {
        Vector2 desiredDelta;
        switch (dir)
        {
            case Direction.Up:
                desiredDelta = Vector2.down;
                break;
            case Direction.Down:
                desiredDelta = Vector2.up;
                break;
            case Direction.Left:
                desiredDelta = Vector2.left;
                break;
            case Direction.Right:
                desiredDelta = Vector2.right;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
        }

        MoveToAction(this, Coord + desiredDelta);
    }
}
