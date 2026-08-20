using static Raylib_cs.Raylib;
using Color = Raylib_cs.Color;

namespace EvolutionSim;

public class Game
{
    public void Init()
    {
        InitWindow(800, 480, "WOW!! IM ECSTATIC!!");
    }

    public void Loop()
    {
        while (!WindowShouldClose())
        {
            BeginDrawing();
            ClearBackground(Color.White);

            DrawText("i am inside game class!", 12, 12, 20, Color.Black);

            EndDrawing();
        }
    }

    public void Close()
    {
        CloseWindow();
    }

    public void Begin()
    {
        Init();
        Loop();
        Close();
    }

}