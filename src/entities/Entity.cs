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
        SetShaderValue(
            _outlineShader, 
            GetShaderLocation(_outlineShader, "texelSize"), 
            new Vector2(1.0f / Texture.Width, 1.0f / Texture.Height), 
            ShaderUniformDataType.Vec2
        );
        SetShaderValue(
            _outlineShader, 
            GetShaderLocation(_outlineShader, "outlineColor"), 
            new Vector4(1.0f, 1.0f, 1.0f, 1.0f), 
            ShaderUniformDataType.Vec4
        );
    }

    public virtual void Update(List<Entity>? entities, World world, bool beingHovered)
    {
        _beingHovered = beingHovered;
    }
    public virtual void Draw(){}
}