namespace EvolutionSim;

public class World
{
    public int Width { get; set; }
    public int Height { get; set; }

    public float[,] NoiseMap { get; set; }
    public Texture2D NoiseTexture { get; set; }

    public World(int w, int h)
    {
        Width = w;
        Height = h;

        FastNoiseLite noise = new();
        noise.SetNoiseType(NoiseType.OpenSimplex2);
        noise.SetFractalType(FractalType.FBm);
        noise.SetFractalOctaves(4);
        noise.SetSeed(Rng.Next());
        
        NoiseMap = new float[w, h];
        float frequency = 0.3f; 
        Parallel.For(0, w, x =>
        {
            for (int y = 0; y < h; y++)
            {
                float rawNoise = noise.GetNoise(x * frequency, y * frequency);
                NoiseMap[x, y] = (rawNoise + 1.0f) / 2.0f;
            }
        });

        Color[] pixelColors = new Color[w * h];
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                byte v = (byte)(NoiseMap[x, y] * 255);
                pixelColors[y * w + x] = new Color(v, v, v, (byte)255);
            }
        }
        
        NoiseTexture = LoadTextureFromImage(new()
        {
            Width = w, Height = h, Mipmaps = 1,
            Format = PixelFormat.UncompressedR8G8B8A8
        });
        UpdateTexture(NoiseTexture, pixelColors);
    }
    
    public void Unload()
    {
        UnloadTexture(NoiseTexture);
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
        DrawTexture(NoiseTexture, 0, 0, Color.White);
        DrawRectangleLinesEx(
            new(-1, -1, Width + 2, Height + 2), 
            1, 
            Color.Black
        );
    }
}