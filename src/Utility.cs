namespace EvolutionSim;

// random

public static class Utility
{
    public static void DrawFont(
        string text, 
        Color color, 
        int scalar, 
        int x, int y
    )
    {
        DrawTextEx(
            GetFont(), 
            text, 
            new Vector2(x, y), 
            FontHeight * scalar, 
            FontSpacing * scalar, 
            color
        );
    }
    public static void DrawFontV(
        string text, 
        Color color, 
        int scalar,
        Vector2 pos
    )
    {
        DrawFont(text, color, scalar, (int)pos.X, (int)pos.Y);
    }

    public static Color GetRandomColor(int minimum = 0)
    {
        Random rand = new Random();
        return new Color(
            rand.Next(minimum, 255),
            rand.Next(minimum, 255),
            rand.Next(minimum, 255)
        );
    }
    
    public static Vector2 GetRandomPosition()
    {
        return new Vector2(
            Rng.Next(WindowWidth),
            Rng.Next(WindowHeight)
        );
    }
    
    public static float GetSquaredDistBetween(Vector2 me, Vector2 other)
    {
        float dx = other.X - me.X;
        float dy = other.Y - me.Y;
        return (dx * dx) + (dy * dy);
    }

    public static float DeltaTime()
    {
        return GetFrameTime() * TimeMultiple;
    }

    public static float SmoothDamp(float current, float target, float lambda)
    {
        return current + (target - current) * (
            1.0f - MathF.Exp(-lambda * DeltaTime())
        );
    }

    public static Vector2 SmoothDampV(Vector2 current, Vector2 target, float lambda)
    {
        return Vector2.Lerp(
            current, 
            target, 
            1.0f - MathF.Exp(-lambda * DeltaTime())
        );
    }
}