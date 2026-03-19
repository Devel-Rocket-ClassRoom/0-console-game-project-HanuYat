using System;
using System.Threading;
using Framework.Engine;

public class BattleScene : ShipPlacementScene
{
    private Board _board;
    private Ship _ship;

    public event GameAction PlayAgainRequested;

    public override void Load()
    {
        _board = new Board(this);
        AddGameObject(_board);
    }

    public override void Unload()
    {
        ClearGameObjects();
    }

    public override void Update(float deltaTime)
    {        
    }

    public override void Draw(ScreenBuffer buffer)
    {
        DrawGameObjects(buffer);

        buffer.WriteText(6, 0, "=== Battle Phase ===", ConsoleColor.DarkMagenta);
    }
}