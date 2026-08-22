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
    private TimeSpan _lifespan = TimeSpan.FromSeconds(Rng.Next(90, 110));
    private TimeSpan _age = TimeSpan.Zero;

    private Dictionary<FoodStage, TimeSpan> _stageThresholds = new(){
        {FoodStage.Sprout, TimeSpan.Zero},
        {FoodStage.Seedling, TimeSpan.FromSeconds(Rng.Next(1, 30))},
        {FoodStage.Budding, TimeSpan.FromSeconds(Rng.Next(30, 35))},
        {FoodStage.Blossom, TimeSpan.FromSeconds(Rng.Next(35, 45))},
    };
    private FoodType _type;
    private FoodStage _stage;
    
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

    public Food(FoodType type, int x, int y) 
        : base(x, y, GetTexture("food.png"))
    {
        FrameSize = new(16, 16);
        _type = type;
        _stage = FoodStage.Sprout;
    }
    public Food(FoodType type, Vector2 pos) 
        : this(type, (int)pos.X, (int)pos.Y)
    {}

    public override void Update(List<Entity>? entities, bool beingHovered)
    {
        base.Update(entities, beingHovered);

        _age += TimeSpan.FromSeconds(DeltaTime());
        _crowdCount = GetNearbyEntities(entities);

        //change stages
        for (int i = 0; i < _stageThresholds.Count; i++)
        {
            FoodStage stage = (FoodStage)i;
            if (_age >= _stageThresholds[stage])
                _stage = stage;
        }

        if (_age > _lifespan)
            shouldDie = true;
        if (_crowdCount > _overcrowdingAmount)
            shouldDie = true;

        // reproduce
        if (shouldDie && _crowdCount < 3)
            for (int i = 0; i < 2; i++)
            {
                int range = _reproductionRadius;
                Vector2 spawnPoint = Position + new Vector2(
                    Rng.Next(-range, range + 1), 
                    Rng.Next(-range, range + 1)
                );
                
                // in bounds
                if (spawnPoint.X is >= 0 and <= WindowWidth && 
                    spawnPoint.Y is >= 0 and <= WindowHeight)
                {
                    Children.Add(new Food(_type, spawnPoint));
                }
            }
        

    }
    public override void Draw()
    {
        if (_beingHovered) BeginShaderMode(_outlineShader);
        DrawTexturePro(
            Texture, 
            new(
                new(
                    (int)_type * FrameSize.X,
                    (int)_stage * FrameSize.Y
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