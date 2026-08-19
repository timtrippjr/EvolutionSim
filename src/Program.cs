using static Raylib_cs.Raylib;
using Color = Raylib_cs.Color;

namespace EvolutionSim;

internal static class Program
{
    [STAThread]
    public static void Main()
    {
        InitWindow(800, 480, "WOW!! IM ECSTATIC!!");

        while (!WindowShouldClose())
        {
            BeginDrawing();
            ClearBackground(Color.White);

            DrawText("Hello, world!", 12, 12, 20, Color.Black);

            EndDrawing();
        }

        CloseWindow();
    }
}