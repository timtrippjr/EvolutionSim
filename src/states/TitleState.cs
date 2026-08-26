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
    public override void Draw(RenderTexture2D buff)
    {
        ClearBackground(Color.Black);
        DrawText("this works?!", 12, 12, 10, Color.DarkGray);
        DrawFont("Evolution Simulator!", Color.Gray, 3, 12, 30);
    }
}