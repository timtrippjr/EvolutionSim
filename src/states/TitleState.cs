namespace EvolutionSim;

public class TitleState : State
{
    public override void Update()
    {
        if (IsKeyPressed(KeyboardKey.Enter))
            TransitionTo(new SimulationState());
        
        if (IsKeyPressed(KeyboardKey.Escape))
            Quit();
    }
    public override void Draw()
    {
        ClearBackground(Color.White);
        DrawText("this works?!", 12, 12, 20, Color.Black);
        DrawFont("Evolution Simulator!", Color.Black, 3, 12, 30);
    }
}