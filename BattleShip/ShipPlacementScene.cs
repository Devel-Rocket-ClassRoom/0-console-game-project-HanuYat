using System;
using System.Collections.Generic;
using Framework.Engine;

public class ShipPlacementScene : Scene
{
    private Board _board;    
    private Ship _attack;

    public const int _maxShip = 6;
    private int _count = 0;
    private bool _canPlace = true;
    private float _errorTimer = 2f;
    
    private Queue<int> _shipsToPlace = new Queue<int>();

    public event GameAction GotoBattleRequested;
    public override void Load()
    {
        _board = new Board(this, 6, 2);
        AddGameObject(_board);        
        _attack = new Ship(this);
        AddGameObject(_attack);

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

        if (!_attack.IsActive)
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
            for (int i = 0; i < _attack.CurrentSize; i++)
            {
                int checkX = _attack.IsHorizontal ? _attack.X + i : _attack.X;
                int checkY = _attack.IsHorizontal ? _attack.Y : _attack.Y + i;

                if (checkX >= Board.k_Width || checkY >= Board.k_Height || _board.Sea[checkX, checkY] == CellState.Ship)
                {
                    isOverlap = true;
                    break;
                }
            }

            if (isOverlap)
            {
                _canPlace = false;
                _errorTimer = 2.0f;
            }
            else
            {
                _canPlace = true;
                _errorTimer = 0f;

                bool success = _board.HasPlaceShip(_attack.X, _attack.Y, _attack.CurrentSize, _attack.IsHorizontal);
                if (success)
                {
                    PrepareNextShip();
                }
            }
        }
    }

    public override void Draw(ScreenBuffer buffer)
    {
        DrawGameObjects(buffer);

        buffer.WriteText(6, 0, "=== Ship Placement Phase ===", ConsoleColor.Green);
        if (_attack.IsActive)
        {
            buffer.WriteText(6, Board.k_Height + 3, "1, 2, 3: Choose ShipSize", ConsoleColor.Gray);
            buffer.WriteText(6, Board.k_Height + 4, "R: Rotate Ship", ConsoleColor.Gray);
            buffer.WriteText(6, Board.k_Height + 5, "Arrow Keys & Enter: Choose Ship & Place", ConsoleColor.Yellow);
            if (!_canPlace && _errorTimer > 0)
            {
                buffer.WriteText(6, Board.k_Height + 6, "Already Placed!", ConsoleColor.Red);                
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
        if ((Input.IsKeyDown(ConsoleKey.UpArrow) || Input.IsKey(ConsoleKey.UpArrow)) && _attack.Y > 0)
        {
            _attack.Y--;
        }
        if ((Input.IsKeyDown(ConsoleKey.DownArrow) || Input.IsKey(ConsoleKey.DownArrow)) && _attack.Y < Board.k_Height - 1)
        {
            _attack.Y++;
        }
        if ((Input.IsKeyDown(ConsoleKey.LeftArrow) || Input.IsKey(ConsoleKey.LeftArrow)) && _attack.X > 0)
        {
            _attack.X--;
        }
        if ((Input.IsKeyDown(ConsoleKey.RightArrow) || Input.IsKey(ConsoleKey.RightArrow)) && _attack.X < Board.k_Width - 1)
        {
            _attack.X++;
        }

        if (Input.IsKeyDown(ConsoleKey.D1))
        {
            _attack.SetSize(1);
        }
        if (Input.IsKeyDown(ConsoleKey.D2))
        {
            _attack.SetSize(3);
        }
        if (Input.IsKeyDown(ConsoleKey.D3))
        {
            _attack.SetSize(5);
        }

        if (Input.IsKeyDown(ConsoleKey.R))
        {
            _attack.Rotate();
        }
    }

    private void PrepareNextShip()
    {
        if (_count < _maxShip)
        {
            int nextSize = _shipsToPlace.Peek();
            _attack.SetSize(nextSize);            
        }
        else
        {
            _attack.IsActive = false;
        }
        _count++;
    }
}