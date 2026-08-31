namespace EvolutionSim;

public class World
{
    private Shader _shader;
        
    private static readonly int _heightMax = 255;
    private int _waterLevel;
    private int _sandLevel;

    private List<Vector2> _grassPositions = [];
    private List<Vector2> _landPositions = [];

    public int Width { get; set; }
    public int Height { get; set; }
    public int WaterLevel { get => _waterLevel; }

    public float[,] NoiseMap { get; set; }
    public Texture2D NoiseTexture { get; set; }

    private float[,] GetNewNoiseMap()
    {
        FastNoiseLite noise = new();
        noise.SetNoiseType(NoiseType.OpenSimplex2);
        noise.SetFractalType(FractalType.FBm);
        noise.SetFractalOctaves(4);
        noise.SetSeed(Rng.Next());
        
        float[,] map = new float[Width, Height];
        float frequency = 0.3f; 
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                float z = (noise.GetNoise(x * frequency, y * frequency) + 1.0f) / 2.0f;
                map[x, y] = z;

                Vector2 pos = new(x, y);
                if (z * _heightMax > _waterLevel)
                {
                    _landPositions.Add(pos);

                    if (z * _heightMax > _sandLevel)
                    {
                        _grassPositions.Add(pos);
                    }
                }
            }
        }
        return map;
    }
    private Texture2D GenerateNoiseTexture()
    {
        Texture2D tex = LoadTextureFromImage(new()
        {
            Width = Width, Height = Height, Mipmaps = 1,
            Format = PixelFormat.UncompressedR8G8B8A8
        });
        Color[] pixelColors = new Color[Width * Height];
        Parallel.For(0, Height, y =>
        {
            for (int x = 0; x < Width; x++)
            {
                byte v = (byte)(NoiseMap[x, y] * 255);
                pixelColors[y * Width + x] = new Color(v, v, v, (byte)255);
            }
        });
        UpdateTexture(tex, pixelColors);
        return tex;
    }
    private void SetupShaderUniforms()
    {
        //setup shader (why is this complicated X2)
        _shader = GetShader("world.fs");

        (string Location, int Level)[] heightVars =
        {
            ("waterLevel", _waterLevel),
            ("sandLevel", _sandLevel),
        };
        foreach (var (Location, Level) in heightVars)
            SetShaderValue(
                _shader, 
                GetShaderLocation(_shader, Location), 
                (float)Level / _heightMax, 
                ShaderUniformDataType.Float
            );
        
        //setup world colors
        (string Location, Color Color)[] colorVars =
        {
            ("grassLight", new(0, 255, 0)),
            ("grassDark", new(0, 120, 0)),
            ("waterLight", new(120, 120, 200)),
            ("waterDark", new(0, 0, 200)),
            ("sandLight", new(255, 234, 163)),
            ("sandDark", new(205, 184, 113)),
        };
        foreach (var (Location, Color) in colorVars)
            SetShaderValue(
                _shader, 
                GetShaderLocation(_shader, Location), 
                new Vector4(
                    Color.R / 255.0f, 
                    Color.G / 255.0f, 
                    Color.B / 255.0f, 
                    1.0f
                ), 
                ShaderUniformDataType.Vec4
            );
    }

    public World(int w, int h)
    {
        Width = w;
        Height = h;

        //must be set before noisemap is generated
        _waterLevel = Rng.Next(50, 120);
        _sandLevel = _waterLevel + Rng.Next(12, 20);

        NoiseMap = GetNewNoiseMap();
        NoiseTexture = GenerateNoiseTexture();

        SetupShaderUniforms();

        Console.WriteLine(_waterLevel);
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
    
    public Vector2 GetRandomGrassPosition()
    {
        if (_grassPositions.Count == 0) return Vector2.Zero;
        return _grassPositions[Rng.Next(_grassPositions.Count)];
    }
    
    public Vector2 GetRandomLandPosition()
    {
        if (_landPositions.Count == 0) return Vector2.Zero;
        return _landPositions[Rng.Next(_landPositions.Count)];
    }

    public bool IsPositionUnderwater(Vector2 position)
    {
        if (IsPositionOutOfBounds(position)) return false;
        int x = (int)position.X;
        int y = (int)position.Y;
        return NoiseMap[x, y] * _heightMax < _waterLevel;
    }

    public bool IsPositionGrass(Vector2 position)
    {
        if (IsPositionOutOfBounds(position)) return false;
        int x = (int)position.X;
        int y = (int)position.Y;
        return NoiseMap[x, y] * _heightMax > _sandLevel;
    }
    
    public bool IsPositionOutOfBounds(Vector2 position)
    {
        int x = (int)position.X;
        int y = (int)position.Y;

        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return true;
        return false;
    }

    public void Draw()
    {
        DrawRectangle(0, 0, Width, Height, Color.DarkGray);
        BeginShaderMode(_shader);
            DrawTexture(NoiseTexture, 0, 0, Color.White);
        EndShaderMode();
        DrawRectangleLinesEx(
            new(-1, -1, Width + 2, Height + 2), 
            1, 
            Color.Black
        );
    }
}