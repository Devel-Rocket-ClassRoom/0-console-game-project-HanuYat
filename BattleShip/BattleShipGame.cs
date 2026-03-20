using System;
using Framework.Engine;

public class BattleShipGame : GameApp
{
    private readonly SceneManager<Scene> _scenes = new SceneManager<Scene>();

    public BattleShipGame() : base(90, 35)
    {        
    }

    public BattleShipGame(int width, int height) : base(width, height)
    {
    }

    protected override void Initialize()
    {
        ChangeToTitle();
    }

    protected override void Update(float deltaTime)
    {
        if (Input.IsKeyDown(ConsoleKey.Escape))
        {
            Quit();
            return;
        }

        _scenes.CurrentScene?.Update(deltaTime);
    }

    protected override void Draw()
    {
        _scenes.CurrentScene?.Draw(Buffer);
    }

    protected void ChangeToTitle()
    {
        var title = new TitleScene();
        title.StartRequested += ChangeToShipPlacement;
        _scenes.ChangeScene(title);
    }

    protected void ChangeToShipPlacement()
    {
        var shipPlacement = new ShipPlacementScene();


        shipPlacement.GotoBattleRequested += () =>
        {
            var board = shipPlacement.GetBoard();
            ChangeToBattle(board.Sea, board.ShipColor);
        };

        _scenes.ChangeScene(shipPlacement);
    }

    protected void ChangeToBattle(CellState[,] playerSea, ConsoleColor[,] colors)
    {
        var battle = new BattleScene();
        battle.SetPlayerSea(playerSea, colors);
        battle.PlayAgainRequested += ChangeToTitle;
        _scenes.ChangeScene(battle);
    }   
}