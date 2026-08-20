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

    public override void Draw()
    {
        Vector2 size = new Vector2(20, 30);
        DrawTexturePro(
            _texture, 
            new Rectangle(Vector2.Zero, size), 
            new Rectangle(_position, size), 
            size / 2, 0, _color
        );
    }
}