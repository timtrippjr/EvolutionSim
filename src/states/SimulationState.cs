namespace EvolutionSim;

public class SimulationState : State
{
    private Texture2D _animal = GetTexture("banjo.png");

    public override void Update()
    {
        if (IsKeyPressed(KeyboardKey.Escape))
            TransitionTo(new TitleState());
    }
    public override void Draw()
    {
        ClearBackground(Color.DarkGray);
        DrawFont("i am inside playstate!", Color.Red, 2, 40, 30);
        DrawTexture(_animal, 0, 0, Color.White);
    }
}