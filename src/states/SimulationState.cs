namespace EvolutionSim;

public class SimulationState : State
{
    private List<Entity> _entities = [];
    private Random _rand = new();

    private void NewGuy()
    {
        _entities.Add(
            new Animal(
                _rand.Next(WindowWidth),
                _rand.Next(WindowHeight)
            )
        );
    }

    public override void Enter()
    {
        
        for (int i = 0; i < 5; i++)NewGuy();

    }

    public override void Update()
    {
        if (IsKeyPressed(KeyboardKey.Escape))
            TransitionTo(new TitleState());

        if (IsKeyPressed(KeyboardKey.F))NewGuy();

        foreach (Entity entity in _entities)
            entity.Update();
    }
    public override void Draw()
    {
        ClearBackground(Color.DarkGray);
        DrawFont("i am inside playstate!", Color.Red, 2, 40, 30);
        
        foreach (Entity entity in _entities)
            entity.Draw();
        
    }
}