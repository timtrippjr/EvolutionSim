namespace EvolutionSim;

public class Animal : Entity
{
    // existence stuff
    private Vector2 _position;
    private Color _color;
    private Texture2D _texture;

    // GENOME STUFF
    private int _agility; // affects speed
    private int _sight; // affects how far it can see around

    public Animal(int x, int y)
    {
        _position = new Vector2(x, y);
        _texture = GetTexture("animal.png");
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