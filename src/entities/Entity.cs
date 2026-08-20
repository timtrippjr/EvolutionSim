namespace EvolutionSim;

public class Entity
{
    protected Vector2 _position;
    protected Texture2D _texture;

    public Entity(int x, int y, Texture2D tex)
    {
        _position = new Vector2(x, y);
        _texture = tex;
    }

    public virtual void Update(){}
    public virtual void Draw(){}
}