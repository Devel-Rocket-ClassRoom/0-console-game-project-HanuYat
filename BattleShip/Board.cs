using System;
using Framework.Engine;

public class Board : GameObject
{
    public const int k_Width = 40;
    public const int k_Height = 10;
    
    private int _startX;
    private int _startY;
    private float _waveTimer = 0f;

    private readonly CellState[,] _sea;
    private readonly ConsoleColor[,] _shipColor;

    public CellState[,] Sea => _sea;
    public int StartX => _startX;
    public int StartY => _startY;

    public Board(Scene scene, int startX, int startY) : base(scene)
    {
        _startX = startX;
        _startY = startY;

        _sea = new CellState[k_Width, k_Height];
        _shipColor = new ConsoleColor[k_Width, k_Height];

        for (int i = 0; i < k_Width; i++)
        {
            for (int j = 0; j < k_Height; j++)
            {
                _sea[i, j] = CellState.Empty;
                _shipColor[i, j] = ConsoleColor.Blue;
            }
        }
    }

    public override void Update(float deltaTime)
    {
        _waveTimer += deltaTime;
    }

    public override void Draw(ScreenBuffer buffer)
    {
        buffer.FillRect(_startX, _startY, k_Width, k_Height, ' ', ConsoleColor.Black, ConsoleColor.DarkBlue);

        for (int x = 0; x < k_Width; x++)
        {
            for (int y = 0; y < k_Height; y++)
            {
                char icon = ' ';
                ConsoleColor fg = ConsoleColor.White;
                ConsoleColor bg = ConsoleColor.DarkBlue;

                switch (_sea[x, y])
                {
                    case CellState.Empty:
                        float waveValue = (float)Math.Sin(_waveTimer * 1.0f + (x * 0.2f + y * 0.1f));
                        if (waveValue > 0.7f)
                        {
                            icon = '^';
                            fg = ConsoleColor.Cyan;
                        }
                        else if (waveValue > 0.3f)
                        {
                            icon = '~';
                            fg = ConsoleColor.Blue;
                        }
                        else
                        {
                            icon = '.';
                        }
                        break;

                    case CellState.Ship:
                        icon = '▣';
                        fg = _shipColor[x, y];
                        break;

                    case CellState.Hit:
                        icon = 'X';
                        fg = ConsoleColor.Red;
                        bg = _shipColor[x, y];
                        break;

                    case CellState.Miss:
                        icon = 'O';
                        fg = ConsoleColor.White;
                        bg = ConsoleColor.DarkGray;
                        break;
                }

                buffer.SetCell(_startX + x, _startY + y, icon, fg, bg);
            }
        }
    }

    public bool Attack(int x, int y)
    {
        if (x < 0 || x >= k_Width || y < 0 || y >= k_Height)
        {
            return false;
        }

        if (_sea[x, y] == CellState.Ship)
        {
            _sea[x, y] = CellState.Hit;
            return true;
        }
        else if (_sea[x, y] == CellState.Empty)
        {
            _sea[x, y] = CellState.Miss;
        }
        return false;
    }

    public bool HasPlaceShip(int x, int y, int size, bool isHorizontal)
    {
        if (isHorizontal && x + size > k_Width)
        {
            return false;
        }
        if (!isHorizontal && y + size > k_Height)
        {
            return false;
        }

        ConsoleColor shipColor = ConsoleColor.Green;
        if (size == 3)
        {
            shipColor = ConsoleColor.Magenta;
        }

        else if (size == 5)
        {
            shipColor = ConsoleColor.Yellow;
        }

        for (int i = 0; i < size; i++)
        {
            if (isHorizontal)
            {
                _sea[x + i, y] = CellState.Ship;
                _shipColor[x + i, y] = shipColor;
            }
            else
            {
                _sea[x, y + i] = CellState.Ship;
                _shipColor[x, y + i] = shipColor;
            }
        }
        return true;
    }
}