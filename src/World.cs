namespace EvolutionSim;

public class World
{
    public int Width { get; set; }
    public int Height { get; set; }

    public World(int w, int h)
    {
        Width = w;
        Height = h;
    }
    
    public Vector2 GetRandomPosition()
    {
        return new Vector2(
            Rng.Next(Width),
            Rng.Next(Height)
        );
    }

    public void Draw()
    {
        DrawRectangle(0, 0, Width, Height, Color.DarkGray);
        DrawRectangleLinesEx(
            new(-1, -1, Width + 2, Height + 2), 
            1, 
            Color.Black
        );
    }
}