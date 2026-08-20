namespace EvolutionSim;

public class Animal : Entity
{
    private Vector2 _position;
    private Color _color;
    private Texture2D _texture;


    public Animal(int x, int y)
    {
        _position = new Vector2(x, y);
        _texture = GetTexture("animal.png");
        _color = GetRandomColor();
    }

    public override void Draw()
    {
        // use position and color
        DrawTexture(_texture, (int)_position.X, (int)_position.Y, _color);
    }
}