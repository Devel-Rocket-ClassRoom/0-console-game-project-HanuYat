using System;
using Framework.Engine;

public class TitleScene : GameObject
{
    public event GameAction TitleChangeRequested;
    public TitleScene() : base()
    {
    }

    public TitleScene(Scene scene) : base(scene)
    {
    }

    public override void Update(float deltaTime)
    {
    }

    public override void Draw(ScreenBuffer buffer)
    {
        buffer.WriteTextCentered(8, "B.a.t.t.l.e S.h.i.p", ConsoleColor.DarkGreen);
        buffer.WriteTextCentered(13, "Destory Your ENEMY", ConsoleColor.DarkRed);
        buffer.WriteTextCentered(15, "Press ENTER to Start", ConsoleColor.Yellow);
    }
}