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
        DrawText("i am inside playstate!", 12, 12, 20, Color.Red);
        DrawTexture(_animal, 0, 0, Color.White);
    }
}