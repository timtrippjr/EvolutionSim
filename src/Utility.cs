namespace EvolutionSim;

// random

public static class Utility
{
    public static void DrawFont(string text, Color color, int scalar, int x, int y)
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

    public static Color GetRandomColor(int minimum = 0)
    {
        Random rand = new Random();
        return new Color(
            rand.Next(minimum, 255),
            rand.Next(minimum, 255),
            rand.Next(minimum, 255)
        );
    }
}