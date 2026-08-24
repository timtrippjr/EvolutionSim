namespace EvolutionSim;

public class Entity
{
    public List<Entity> Children { get; set; } = new List<Entity>();
    public TimeSpan Age { get; set; } = TimeSpan.Zero;
    public Vector2 Position { get; set; }
    public Texture2D Texture { get; set; }
    public Vector2 FrameSize { get; set; }
    public Vector2 Origin { 
        get
        {
            return new(FrameSize.X / 2, FrameSize.Y);
        }
    }
    public bool shouldDie = false;

    protected bool _beingHovered = false;
    protected Shader _outlineShader;

    public Entity(int x, int y, Texture2D tex)
    {
        Position = new Vector2(x, y);
        Texture = tex;
        
        //setup shader (why is this complicated)
        _outlineShader = GetShader("outline.fs");
        int texelSizeLoc = GetShaderLocation(_outlineShader, "texelSize");
        int outlineColorLoc = GetShaderLocation(_outlineShader, "outlineColor");
        Vector2 texelSize = new(1.0f / Texture.Width, 1.0f / Texture.Height);
        Vector4 colorData = new(1.0f, 1.0f, 1.0f, 1.0f); 
        SetShaderValue(
            _outlineShader, 
            texelSizeLoc, 
            texelSize, 
            ShaderUniformDataType.Vec2
        );
        SetShaderValue(
            _outlineShader, 
            outlineColorLoc, 
            colorData, 
            ShaderUniformDataType.Vec4
        );
    }

    public virtual void Update(List<Entity>? entities, bool beingHovered)
    {
        _beingHovered = beingHovered;
    }
    public virtual void Draw(){}
}