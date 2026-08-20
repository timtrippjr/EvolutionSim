namespace EvolutionSim;

public class SimulationState : State
{
    private Entity[] _entities = [
        new Animal(40, 40),
        new Animal(90, 60)
    ];

    public override void Update()
    {
        if (IsKeyPressed(KeyboardKey.Escape))
            TransitionTo(new TitleState());

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