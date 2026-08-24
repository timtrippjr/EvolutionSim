namespace EvolutionSim;

public class SimulationState : State
{
    private List<Entity> _entities = [];
    private Entity? _hover;
    private InfoPane _infoPane = new InfoPane();

    public override void Enter()
    {
        
        for (int i = 0; i < 5; i++)
            _entities.Add(new Animal(GetRandomPosition()));
        for (int i = 0; i < 8; i++)
            _entities.Add(new Food(
                (FoodType)(i % 2), 
                GetRandomPosition()
            ));

    }

    public override void Update()
    {
        if (IsKeyPressed(KeyboardKey.Escape))
            TransitionTo(new TitleState());

        if (IsKeyPressed(KeyboardKey.A)||IsKeyPressedRepeat(KeyboardKey.A))
            _entities.Add(new Animal(GetRandomPosition()));
        if (IsMouseButtonPressed(MouseButton.Right))
            _entities.Add(new Animal(GetMousePosition() / WindowScale));
        if (IsMouseButtonPressed(MouseButton.Left))
            _entities.Add(new Food(
                (FoodType)Rng.Next(2), 
                GetMousePosition() / WindowScale
            ));

        if (IsKeyPressed(KeyboardKey.H))
            _hover = null;

        var snapshot = _entities.ToList();
        foreach (var e in snapshot)
        {
            
            if (CheckCollisionPointRec(
                GetMousePosition() / WindowScale, 
                new(e.Position - e.Origin, e.FrameSize)
            ))
            {
                _hover = e;
                break;
            }

        };
        snapshot.ForEach(e => e.Update(snapshot, _hover == e));

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
        
        _entities
            .OrderBy(e => e.Position.Y)
            .ToList()
            .ForEach(e => e.Draw());

        _infoPane.Draw(_hover);
        
    }
}