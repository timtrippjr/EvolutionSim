namespace EvolutionSim;

public class SimulationState : State
{
    private List<Entity> _entities = [];
    private Entity? _hover;
    private InfoPane _infoPane = new();

    public override void Enter()
    {
        
        for (int i = 0; i < 5; i++)
            _entities.Add(new Animal(GetRandomPosition()));
        for (int i = 0; i < 50; i++)
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
        if (IsKeyPressed(KeyboardKey.F)||IsKeyPressedRepeat(KeyboardKey.F))
            _entities.Add(new Food(
                (FoodType)Rng.Next(2), 
                GetRandomPosition()
            ));
        if (IsMouseButtonPressed(MouseButton.Right))
            _hover = null;
        if (IsMouseButtonPressed(MouseButton.Left))
            foreach (var e in _entities)
                if (CheckCollisionPointRec(
                    GetMousePosition() / WindowScale, 
                    new(e.Position - e.Origin, e.FrameSize)
                ))
                {
                    if (_hover == e) _hover = null;
                    else _hover = e;
                    break;
                }

        var snapshot = _entities.ToList();
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