namespace EvolutionSim;

public class World
{
    private Shader _shader;
    // uniform locations
    private int _waterLevelLocation;
        
    private static readonly float _waterLevelMax = 255;
    private static readonly float _sandLevel = 128;
    private float _waterLevel;

    public int Width { get; set; }
    public int Height { get; set; }

    public float[,] NoiseMap { get; set; }
    public Texture2D NoiseTexture { get; set; }

    public float WaterLevel
    {
        get => _waterLevel;
        set
        {
            _waterLevel = value;
            if (_waterLevel > _waterLevelMax) _waterLevel = _waterLevelMax;
            if (_waterLevel < 0) _waterLevel = 0; 
            SetShaderValue(
                _shader, 
                _waterLevelLocation, 
                _waterLevel / _waterLevelMax, 
                ShaderUniformDataType.Float
            );
        }
    }

    private float[,] GetNewNoiseMap()
    {
        FastNoiseLite noise = new();
        noise.SetNoiseType(NoiseType.OpenSimplex2);
        noise.SetFractalType(FractalType.FBm);
        noise.SetFractalOctaves(4);
        noise.SetSeed(Rng.Next());
        
        float[,] map = new float[Width, Height];
        float frequency = 0.3f; 
        Parallel.For(0, Width, x =>
        {
            for (int y = 0; y < Height; y++)
            {
                float rawNoise = noise.GetNoise(x * frequency, y * frequency);
                map[x, y] = (rawNoise + 1.0f) / 2.0f;
            }
        });
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

        _waterLevelLocation = GetShaderLocation(_shader, "waterLevel");
        SetShaderValue(
            _shader, 
            GetShaderLocation(_shader, "sandLevel"), 
            _sandLevel / _waterLevelMax, 
            ShaderUniformDataType.Float
        );
        
        //setup world colors
        (string Location, Color Color)[] colorVars =
        {
            ("grassLight", new(0, 255, 0)),
            ("grassDark", new(0, 120, 0)),
            ("waterLight", new(170, 170, 200)),
            ("waterDark", new(30, 30, 200)),
            ("sandLight", new(255, 234, 163)),
            ("sandDark", new(205, 184, 113)),
        };
        foreach (var var in colorVars)
        {
            SetShaderValue(
                _shader, 
                GetShaderLocation(_shader, var.Location), 
                new Vector4(
                    var.Color.R / 255.0f, 
                    var.Color.G / 255.0f, 
                    var.Color.B / 255.0f, 
                    1.0f
                ), 
                ShaderUniformDataType.Vec4
            );
        }
    }

    public World(int w, int h)
    {
        Width = w;
        Height = h;
        SetupShaderUniforms();
        NoiseMap = GetNewNoiseMap();
        NoiseTexture = GenerateNoiseTexture();
        
        WaterLevel = _sandLevel - 16;
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

    public void Update()
    {
        if (IsKeyDown(KeyboardKey.Up))
            WaterLevel += 30 * DeltaTime();
        if (IsKeyDown(KeyboardKey.Down))
            WaterLevel -= 30 * DeltaTime();
        
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