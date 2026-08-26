namespace EvolutionSim;

public enum FoodType
{
    Greenland,
    Iceland
}
public enum FoodStage
{
    Sprout,
    Seedling,
    Budding,
    Blossom,
}

public class Food : Entity
{
    private Dictionary<FoodStage, TimeSpan> _stageThresholds = new(){
        {FoodStage.Sprout, TimeSpan.Zero},
        {FoodStage.Seedling, TimeSpan.FromSeconds(Rng.Next(1, 10))},
        {FoodStage.Budding, TimeSpan.FromSeconds(Rng.Next(10, 15))},
        {FoodStage.Blossom, TimeSpan.FromSeconds(Rng.Next(15, 20))},
    };
    private TimeSpan _lifespan = TimeSpan.FromSeconds(Rng.Next(20, 30));

    public FoodType Type { get; set; }
    public FoodStage Stage { get; set; }
    public int SustenanceAmount
    {
        get
        {
            if (Stage == FoodStage.Blossom) return 50;
            if (Stage == FoodStage.Budding) return 30;
            return 10;
        }
    }
    
    private int _overcrowdingAmount = 4;
    private static float _crowdRadius = 40;
    private static int _reproductionRadius = 30;
    private int _crowdCount = 0;
    private int GetNearbyEntities(List<Entity>? entities)
    {
        float radiusSquared = _crowdRadius * _crowdRadius;
        return entities?
            .Where(e => e != this)
            .Where(e => e is Food)
            .Where(e => 
                GetSquaredDistBetween(Position, e.Position) <= radiusSquared
            )
            .ToList()
            .Count ?? 0;
    }

    public bool beingEaten = false;

    public Food(FoodType type, int x, int y) 
        : base(x, y, GetTexture("food.png"))
    {
        FrameSize = new(16, 16);
        Type = type;
        Stage = FoodStage.Sprout;
    }
    public Food(FoodType type, Vector2 pos) 
        : this(type, (int)pos.X, (int)pos.Y)
    {}

    public override void Update(List<Entity>? entities, World world, bool beingHovered)
    {
        base.Update(entities, world, beingHovered);

        Age += TimeSpan.FromSeconds(DeltaTime());
        _crowdCount = GetNearbyEntities(entities);

        //change stages
        for (int i = 0; i < _stageThresholds.Count; i++)
        {
            FoodStage stage = (FoodStage)i;
            if (Age >= _stageThresholds[stage])
                Stage = stage;
        }

        if (Age > _lifespan)
            shouldDie = true;
        if (_crowdCount > _overcrowdingAmount)
            shouldDie = true;
        if (beingEaten)
            shouldDie = true;

        // reproduce
        if (!beingEaten && shouldDie && _crowdCount < 3)
            for (int i = 0; i < 2; i++)
            {
                int range = _reproductionRadius;
                Vector2 spawnPoint = Position + new Vector2(
                    Rng.Next(-range, range + 1), 
                    Rng.Next(-range, range + 1)
                );
                
                // in bounds
                if (spawnPoint.X >= 0 && spawnPoint.X <= world.Width && 
                    spawnPoint.Y >= 0 && spawnPoint.Y <= world.Height)
                {
                    Children.Add(new Food(Type, spawnPoint));
                }
            }
        

    }
    public override void Draw()
    {
        if (_beingHovered) {
            DrawCircleLinesV(Position, _crowdRadius, Color.Red);
            BeginShaderMode(_outlineShader);
        }
        DrawTexturePro(
            Texture, 
            new(
                new(
                    (int)Type * FrameSize.X,
                    (int)Stage * FrameSize.Y
                ), 
                FrameSize
            ), 
            new(Position, FrameSize), 
            new(FrameSize.X / 2, FrameSize.Y), 0, Color.White
        );
        if (_beingHovered) EndShaderMode();   

        /*/tell us how many plants are neearby(degbugging)
        DrawFont(
            $"{_crowdCount}", 
            Color.SkyBlue, 1, 
            (int)Position.X, 
            (int)Position.Y + 4
        );//*/
    }
}