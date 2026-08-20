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
}