using System;
using Framework.Engine;

public class BattleScene : Scene
{
    private Board _playerBoard;
    private Board _enemyBoard;

    private int _attackX = 0;
    private int _attackY = 0;

    private string _turnMessage = string.Empty;
    private string _battleMessage = string.Empty;

    private bool _isPlayerTurn = true;
    private bool _isHit = false;

    private float _attackTimer = 0f;

    public event GameAction PlayAgainRequested;

    public override void Load()
    {
        _playerBoard = new Board(this, 6, 2);
        AddGameObject(_playerBoard);

        _enemyBoard = new Board(this, 50, 2);        
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
        buffer.WriteText(_playerBoard.StartX, _playerBoard.StartY - 1, "[ PLAYER BOARD ]", ConsoleColor.White);        

        // 2. 적 보드 그리기 (오른쪽으로 밀어서 그리기)
        // Board 클래스를 수정하지 않으려면 Draw 내부의 좌표 계산에 오프셋을 더해야 합니다.
        // 여기서는 개념적으로 아래에 그리거나, 좌표를 인자로 받는 Draw를 Board에 추가해야 합니다.
        buffer.WriteText(_enemyBoard.StartX, _enemyBoard.StartY - 1, "[ ENEMY BOARD ]", ConsoleColor.DarkRed);

        if (_isPlayerTurn)
        {
            buffer.SetCell(_enemyBoard.StartX + _attackX, _enemyBoard.StartY + _attackY, '+', ConsoleColor.Yellow, ConsoleColor.DarkRed);
        }
        
        _turnMessage = _isPlayerTurn ? "- YOUR TURN -" : "- ENEMY TURN -";
        buffer.WriteText(6, 13, "==================================================================", ConsoleColor.Gray);
        buffer.WriteText(6, 14, _turnMessage, _isPlayerTurn ? ConsoleColor.White : ConsoleColor.DarkRed);
        buffer.WriteText(6, 15, _battleMessage, _isPlayerTurn ? ConsoleColor.Yellow : ConsoleColor.Magenta);
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
            _battleMessage = _isHit ? "HIT! YOU ATTACK THE ENEMY SHIP!!" : "MISS.....";
                        
            _attackTimer = 1.5f;
        }
    }

    private void HandleEnemyTurn()
    {
        Random rand = new Random();
        int X = rand.Next(Board.k_Width); 
        int Y = rand.Next(Board.k_Height);

        _isHit = _playerBoard.Attack(X, Y);
        _battleMessage = _isHit? "OH NO! ENEMY HIT THE PLAYER SHIP..." : "ENEMY HAS MISSED!";

        _attackTimer = 2.5f;
    }
}