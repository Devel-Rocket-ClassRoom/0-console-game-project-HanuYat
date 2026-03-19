using System;
using Framework.Engine;

public class BattleShipGame : GameApp
{
    private readonly SceneManager<Scene> _scenes = new SceneManager<Scene>();

    public BattleShipGame() : base(80, 35)
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
        shipPlacement.GotoBattleRequested += ChangeToBattle;
        _scenes.ChangeScene(shipPlacement);
    }

    protected void ChangeToBattle()
    {
        var battle = new BattleScene();
        battle.PlayAgainRequested += ChangeToTitle;
        _scenes.ChangeScene(battle);
    }   
}