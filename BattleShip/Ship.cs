using System;
using Framework.Engine;

public class Ship : GameObject
{
    private readonly (int Small, int Medium, int Large) _shipSizes = (1, 3, 5);

    public int CurrentSize { get; private set; }
    public bool IsHorizontal { get; private set; } = true;

    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;
    public (int Small, int Medium, int Large) ShipSizes => _shipSizes;

    public Ship(Scene scene) : base(scene)
    {
        CurrentSize = _shipSizes.Small;
    }

    public void SetSize(int size)
    {
        if (size == _shipSizes.Small || size == _shipSizes.Medium || size == _shipSizes.Large)
        {
            CurrentSize = size;
        }
    }

    public void Rotate()
    {
        IsHorizontal = !IsHorizontal;
    }

    public override void Update(float deltaTime)
    {
    }

    public override void Draw(ScreenBuffer buffer)
    {
        int drawX = 6 + X;
        int drawY = 2 + Y;

        for (int i = 0; i < CurrentSize; i++)
        {
            int posX = IsHorizontal ? drawX + i : drawX;
            int posY = IsHorizontal ? drawY : drawY + i;

            if (posX < 6 + Board.k_Width && posY < 2 + Board.k_Height)
            {
                ConsoleColor shipColor = ConsoleColor.Green;
                if (CurrentSize == 3)
                {
                    shipColor = ConsoleColor.Magenta;
                    buffer.SetCell(posX, posY, '▣', shipColor, ConsoleColor.DarkGreen);
                    continue;
                }
                else if (CurrentSize == 5)
                {
                    shipColor = ConsoleColor.Yellow;
                    buffer.SetCell(posX, posY, '▣', shipColor, ConsoleColor.DarkGreen);
                    continue;
                }
                buffer.SetCell(posX, posY, '▣', shipColor, ConsoleColor.DarkGreen);
            }
        }
    }
}