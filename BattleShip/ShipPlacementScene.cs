using System;
using System.Collections.Generic;
using Framework.Engine;

public class ShipPlacementScene : Scene
{
    private Board _board;    
    private Ship _ship;

    public const int _maxShip = 6;
    private int _count = 0;
    private bool _canPlace = true;
    private float _errorTimer = 2f;
    
    private Queue<int> _shipsToPlace = new Queue<int>();

    public event GameAction GotoBattleRequested;
    public override void Load()
    {
        _board = new Board(this);
        AddGameObject(_board);        
        _ship = new Ship(this);
        AddGameObject(_ship);

        _shipsToPlace.Enqueue(1);
        _shipsToPlace.Enqueue(3);
        _shipsToPlace.Enqueue(5);

        PrepareNextShip();

    }

    public override void Unload()
    {
        ClearGameObjects();
    }

    public override void Update(float deltaTime)
    {
        UpdateGameObjects(deltaTime);

        if (!_ship.IsActive)
        {
            if (Input.IsKeyDown(ConsoleKey.Enter))
            {
                GotoBattleRequested?.Invoke();
            }
            return;
        }

        HandleInput();
        
        if (Input.IsKeyDown(ConsoleKey.Enter))
        {
            bool isOverlap = false;
            for (int i = 0; i < _ship.CurrentSize; i++)
            {
                int checkX = _ship.IsHorizontal ? _ship.X + i : _ship.X;
                int checkY = _ship.IsHorizontal ? _ship.Y : _ship.Y + i;

                if (checkX >= Board.k_Width || checkY >= Board.k_Height)
                {
                    isOverlap = true;
                    break;
                }
                if (_board.Sea[checkX, checkY] == CellState.Ship)
                {
                    _canPlace = false;
                    isOverlap = true;
                    break;
                }
            }

            if (!_canPlace && isOverlap)
            {
                _errorTimer = 2.0f;
                _canPlace = false;
                return;
            }
                _errorTimer = 0f;

                bool success = _board.HasPlaceShip(_ship.X, _ship.Y, _ship.CurrentSize, _ship.IsHorizontal);
                if (success)
                {
                    PrepareNextShip();
                }
        }
    }

    public override void Draw(ScreenBuffer buffer)
    {
        DrawGameObjects(buffer);

        buffer.WriteText(6, 0, "=== Ship Placement Phase ===", ConsoleColor.Green);
        if (_ship.IsActive)
        {
            buffer.WriteText(6, Board.k_Height + 3, "1, 2, 3: Choose ShipSize", ConsoleColor.Gray);
            buffer.WriteText(6, Board.k_Height + 4, "R: Rotate Ship", ConsoleColor.Gray);
            buffer.WriteText(6, Board.k_Height + 5, "Arrow Keys & Enter: Choose Ship & Place", ConsoleColor.Yellow);
            if (!_canPlace && _errorTimer > 0)
            {
                buffer.WriteText(6, Board.k_Height + 6, "Alreay Placed!", ConsoleColor.Red);                
            }
        }
        else
        {
            buffer.WriteText(6, Board.k_Height + 3, "Ready To Battle!", ConsoleColor.DarkCyan);
            buffer.WriteText(6, Board.k_Height + 4, "Press ENTER to Start Battle", ConsoleColor.Yellow);
        }
    }

    private void HandleInput()
    {
        if ((Input.IsKeyDown(ConsoleKey.UpArrow) || Input.IsKey(ConsoleKey.UpArrow)) && _ship.Y > 0)
        {
            _ship.Y--;
        }
        if ((Input.IsKeyDown(ConsoleKey.DownArrow) || Input.IsKey(ConsoleKey.DownArrow)) && _ship.Y < Board.k_Height - 1)
        {
            _ship.Y++;
        }
        if ((Input.IsKeyDown(ConsoleKey.LeftArrow) || Input.IsKey(ConsoleKey.LeftArrow)) && _ship.X > 0)
        {
            _ship.X--;
        }
        if ((Input.IsKeyDown(ConsoleKey.RightArrow) || Input.IsKey(ConsoleKey.RightArrow)) && _ship.X < Board.k_Width - 1)
        {
            _ship.X++;
        }

        if (Input.IsKeyDown(ConsoleKey.D1))
        {
            _ship.SetSize(1);
        }
        if (Input.IsKeyDown(ConsoleKey.D2))
        {
            _ship.SetSize(3);
        }
        if (Input.IsKeyDown(ConsoleKey.D3))
        {
            _ship.SetSize(5);
        }

        if (Input.IsKeyDown(ConsoleKey.R))
        {
            _ship.Rotate();
        }
    }

    private void PrepareNextShip()
    {
        if (_count < _maxShip)
        {
            int nextSize = _shipsToPlace.Peek();
            _ship.SetSize(nextSize);            
        }
        else
        {
            _ship.IsActive = false;
        }
        _count++;
    }
}