namespace EvolutionSim;

public class Food : Entity
{
    private TimeSpan _lifespan = TimeSpan.FromSeconds(2);
    private static int _frameSize = 16;

    public Food(int x, int y) 
        : base(x, y, GetTexture("food.png"))
    {
        
    }

    public override void Draw()
    {
        Vector2 size = new(_frameSize, _frameSize);
        DrawTexturePro(
            _texture, 
            new Rectangle(Vector2.Zero, size), 
            new Rectangle(_position, size), 
            size / 2, 0, Color.White
        );
    }
}