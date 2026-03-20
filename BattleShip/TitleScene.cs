using System;
using Framework.Engine;

public class TitleScene : Scene
{
    public event GameAction StartRequested;

    public override void Load()
    {
    }

    public override void Unload()
    {
    }

    public override void Update(float deltaTime)
    {
        if (Input.IsKeyDown(ConsoleKey.Enter))
        {
            StartRequested?.Invoke();
        }
    }

    public override void Draw(ScreenBuffer buffer)
    {
        buffer.WriteTextCentered(4, "  ┏━ ┏━┃━┏┛━┏┛┃  ┏━┛  ┏━┛┃ ┃┛┏━┃", ConsoleColor.Cyan);
        buffer.WriteTextCentered(5, "  ┏━┃┏━┃ ┃  ┃ ┃  ┏━┛  ━━┃┏━┃┃┏━┛", ConsoleColor.Cyan);
        buffer.WriteTextCentered(6, "━━ ┛ ┛ ┛  ┛ ━━┛━━┛  ━━┛┛ ┛┛┛", ConsoleColor.Cyan);
        buffer.WriteTextCentered(8, "  Eliminate Your ENEMY", ConsoleColor.DarkRed);        
        buffer.WriteTextCentered(13, "  Arrow Keys & Enter & R: Control");
        buffer.WriteTextCentered(15, "  ESC: Quit", ConsoleColor.DarkGray);
        buffer.WriteTextCentered(17, "  Press ENTER to Start", ConsoleColor.Yellow);
    }    
}