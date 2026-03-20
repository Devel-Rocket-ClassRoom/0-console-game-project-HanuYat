using System;
using Framework.Engine;

public class BattleScene : Scene
{
    private Board _playerBoard;
    private Board _enemyBoard;

    private int _attackX = 0;
    private int _attackY = 0;

    private int _playerRemainShip = ShipPlacementScene._maxShip;
    private int _enemyRemainShip = ShipPlacementScene._maxShip;

    private string _turnMessage = string.Empty;
    private string _battleMessage = string.Empty;

    private bool _isPlayerTurn = true;
    private bool _isHit = false;
    private bool _isGameOver = false;

    private float _attackTimer = 0f;

    private CellState[,] _playerSea;
    private ConsoleColor[,] _playerColor;

    public event GameAction PlayAgainRequested;

    public override void Load()
    {
        _playerBoard = new Board(this, 6, 2, true);
        if (_playerSea != null && _playerColor != null)
        {
            _playerBoard.CopyBoardData(_playerSea, _playerColor);
        }
        AddGameObject(_playerBoard);

        _enemyBoard = new Board(this, 50, 2, false);        
        AddGameObject(_enemyBoard);
        SetEnemyShips();
    }

    public override void Unload()
    {
        ClearGameObjects();
    }

    public override void Update(float deltaTime)
    {
        UpdateGameObjects(deltaTime);

        if (!_enemyBoard.HasRemainingShips())
        {
            _isGameOver = true;

            if (Input.IsKeyDown(ConsoleKey.Enter))
            {
                PlayAgainRequested?.Invoke();
            }
            return;
        }

        if (!_playerBoard.HasRemainingShips())
        {
            _isGameOver = true;

            if (Input.IsKeyDown(ConsoleKey.Enter))
            {
                PlayAgainRequested?.Invoke();
            }
            return;
        }

        if (_attackTimer > 0)
        {
            _attackTimer -= deltaTime;
            if (_attackTimer <= 0)
            {
                _isPlayerTurn = !_isPlayerTurn;
            }
            return;
        }

        _battleMessage = string.Empty;

        if (_isPlayerTurn)
        {
            HandlePlayerInput();
        }
        else
        {
            HandleEnemyTurn();
        }
    }

    public override void Draw(ScreenBuffer buffer)
    {
        DrawGameObjects(buffer);

        buffer.WriteText(6, 0, "=== Battle Phase ===", ConsoleColor.DarkMagenta);
        buffer.WriteText(_playerBoard.StartX, _playerBoard.StartY - 1, $"[ PLAYER BOARD ] (REMAINING SHIP: {_playerRemainShip})", ConsoleColor.White);
        buffer.WriteText(_enemyBoard.StartX, _enemyBoard.StartY - 1, $"[ ENEMY BOARD ] (REMAINING SHIP: {_enemyRemainShip})", ConsoleColor.DarkRed);

        if (!_enemyBoard.HasRemainingShips() && _isGameOver)
        {
            buffer.WriteText(6, Board.k_Height + 7, "~* CONGRATULATIONS! YOU WIN!! *~", ConsoleColor.Cyan);
            buffer.WriteText(6, Board.k_Height + 8, ">> PRESS ENTER TO RETURN TO TITLE <<", ConsoleColor.DarkYellow);
        }
        else if (!_playerBoard.HasRemainingShips() && _isGameOver)
        {
            buffer.WriteText(6, Board.k_Height + 7, "YOU LOSE..... :(", ConsoleColor.Magenta);
            buffer.WriteText(6, Board.k_Height + 8, ">> PRESS ENTER TO RETURN TO TITLE <<", ConsoleColor.DarkYellow);
        }
        else if (_isPlayerTurn)
        {
            buffer.SetCell(_enemyBoard.StartX + _attackX, _enemyBoard.StartY + _attackY, '+', ConsoleColor.Yellow, ConsoleColor.DarkRed);
        }
        
        string guideMessage = _isPlayerTurn ? "(Arrow Keys: Move, Enter: Attack)" : "";
        _turnMessage = _isPlayerTurn ? "- YOUR TURN -" : "- ENEMY TURN -";
        buffer.WriteText(6, Board.k_Height + 3, "====================================================================================", ConsoleColor.Gray);
        buffer.WriteText(6, Board.k_Height + 4, _turnMessage, _isPlayerTurn ? ConsoleColor.White : ConsoleColor.DarkRed);
        buffer.WriteText(23, Board.k_Height + 4, guideMessage, ConsoleColor.DarkGray);
        buffer.WriteText(6, Board.k_Height + 5, _battleMessage, _isPlayerTurn ? ConsoleColor.Yellow : ConsoleColor.Magenta);
    }

    private void SetEnemyShips()
    {
        Random rand = new Random();
        int[] shipSizes = { 1, 3, 5 };

        for (int i = 0; i < ShipPlacementScene._maxShip; i++)
        {
            bool placed = false;

            int randomSize = shipSizes[rand.Next(shipSizes.Length)];

            while (!placed)
            {
                int X = rand.Next(Board.k_Width);
                int Y = rand.Next(Board.k_Height);

                bool horizontal = rand.Next(2) == 0;

                bool isOverlap = false;
                for (int j = 0; j < randomSize; j++)
                {
                    int checkX = horizontal ? X + j : X;
                    int checkY = horizontal ? Y : Y + j;

                    if (checkX >= Board.k_Width || checkY >= Board.k_Height ||
                        _enemyBoard.Sea[checkX, checkY] == CellState.Ship)
                    {
                        isOverlap = true;
                        break;
                    }
                }

                if (!isOverlap)
                {
                    placed = _enemyBoard.HasPlaceShip(X, Y, randomSize, horizontal);
                }
            }
        }
    }

    private void HandlePlayerInput()
    {
        if ((Input.IsKeyDown(ConsoleKey.UpArrow) || Input.IsKey(ConsoleKey.UpArrow)) && _attackY > 0)
        {
            _attackY--;
        }
        if ((Input.IsKeyDown(ConsoleKey.DownArrow) || Input.IsKey(ConsoleKey.DownArrow)) && _attackY < Board.k_Height - 1)
        {
            _attackY++;
        }
        if ((Input.IsKeyDown(ConsoleKey.LeftArrow) || Input.IsKey(ConsoleKey.LeftArrow)) && _attackX > 0)
        {
            _attackX--;
        }
        if ((Input.IsKeyDown(ConsoleKey.RightArrow) || Input.IsKey(ConsoleKey.RightArrow)) && _attackX < Board.k_Width - 1)
        {
            _attackX++;
        }
        
        if (Input.IsKeyDown(ConsoleKey.Enter))
        {
            _isHit = _enemyBoard.Attack(_attackX, _attackY);
            if (_isHit)
            {
                if (_enemyBoard.IsShipSunk(_attackX, _attackY))
                {
                    _enemyRemainShip--;
                    _battleMessage = "HIT & SUNK! THE ENEMY SHIP DESTROYED!";
                }
                else
                {
                    _battleMessage = "HIT! YOU ATTACK THE ENEMY SHIP!!";
                }
            }
            else
            {
                _battleMessage = "MISS.....";
            }
            
            _attackTimer = 1.5f;
        }
    }

    private void HandleEnemyTurn()
    {
        Random rand = new Random();
        int X = rand.Next(Board.k_Width); 
        int Y = rand.Next(Board.k_Height);

        while (true)
        {
            X = rand.Next(Board.k_Width);
            Y = rand.Next(Board.k_Height);

            CellState targetState = _playerBoard.Sea[X, Y];

            if (targetState == CellState.Empty || targetState == CellState.Ship)
            {
                break;
            }
        }

        _isHit = _playerBoard.Attack(X, Y);
        if (_isHit)
        {
            if (_playerBoard.IsShipSunk(X, Y))
            {
                _playerRemainShip--;
                _battleMessage = "OH NO! YOUR SHIP WAS SUNK...";
            }
            else
            {
                _battleMessage = "OH NO! ENEMY HIT THE PLAYER SHIP...";
            }
        }
        else
        {
            _battleMessage = "ENEMY HAS MISSED!";
        }
        
        _attackTimer = 2.5f;
    }

    public void SetPlayerSea(CellState[,] sea, ConsoleColor[,] colors)
    {
        _playerSea = sea;
        _playerColor = colors;
    }
}