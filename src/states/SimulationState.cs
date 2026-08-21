namespace EvolutionSim;

public class SimulationState : State
{
    private List<Entity> _entities = [];

    private Vector2 GetRandomPosition()
    {
        return new Vector2(
            Rng.Next(WindowWidth),
            Rng.Next(WindowHeight)
        );
    }

    public override void Enter()
    {
        
        for (int i = 0; i < 5; i++)
            _entities.Add(new Animal(GetRandomPosition()));
        for (int i = 0; i < 2; i++)
            _entities.Add(new Food(
                (FoodType)i, 
                GetRandomPosition()
            ));

    }

    public override void Update()
    {
        if (IsKeyPressed(KeyboardKey.Escape))
            TransitionTo(new TitleState());

        if (IsKeyPressed(KeyboardKey.A)||IsKeyPressedRepeat(KeyboardKey.A))
            _entities.Add(new Animal(GetRandomPosition()));
        if (IsMouseButtonPressed(MouseButton.Left))
            _entities.Add(new Food(
                (FoodType)Rng.Next(2), 
                GetMousePosition() / WindowScale
            ));

        var snapshot = _entities.ToList();
        snapshot.ForEach(e => e.Update(snapshot));

        var newEntities = _entities
            .Where(e => e.shouldDie)
            .SelectMany(e => e.Children)
            .ToList();
        _entities.RemoveAll(e => e.shouldDie);

        _entities.AddRange(newEntities);
    }
    public override void Draw()
    {
        ClearBackground(Color.DarkGray);
        DrawFont("i am inside playstate!", Color.Red, 2, 40, 30);
        
        _entities
            .OrderBy(e => e.Position.Y + (e.Texture.Height / 2))
            .ToList()
            .ForEach(e => e.Draw());
        
    }
}