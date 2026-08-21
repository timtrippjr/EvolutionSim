namespace EvolutionSim;

public class Entity
{
    public List<Entity> Children { get; set; } = new List<Entity>();
    public Vector2 Position { get; set; }
    public Texture2D Texture { get; set; }
    public bool shouldDie = false;

    public Entity(int x, int y, Texture2D tex)
    {
        Position = new Vector2(x, y);
        Texture = tex;
    }

    public virtual void Update(List<Entity>? entities){}
    public virtual void Draw(){}
}