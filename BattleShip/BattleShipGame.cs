using System;
using System.Collections.Generic;
using Framework.Engine;

public class BattleShipGame : GameApp
{
    private SceneManager<Scene> _sceneManager = new SceneManager<Scene>();

    public BattleShipGame() : base(40, 40)
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
        ChangeToShipPlacement();
        ChangeToBattle();
    }

    protected override void Draw()
    {
    }

    private void ChangeToTitle()
    {
        var title = new TitleScene();        
    }

    private void ChangeToBattle()
    {

    }

    private void ChangeToShipPlacement()
    {

    }
}