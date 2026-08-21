namespace EvolutionSim;

public class Animal : Entity
{
    // existence stuff
    private Color _color;

    // GENOME STUFF
    private int _agility; // affects speed
    private int _sight; // affects how far it can see around

    public Animal(int x, int y) 
        : base(x, y, GetTexture("animal.png"))
    {
        _color = GetRandomColor();
    }
    public Animal(Vector2 pos) 
        : this((int)pos.X, (int)pos.Y) 
    {}

    public override void Draw()
    {
        Vector2 size = new Vector2(20, 30);
        DrawTexturePro(
            Texture, 
            new Rectangle(Vector2.Zero, size), 
            new Rectangle(Position, size), 
            size / 2, 0, _color
        );
    }
}