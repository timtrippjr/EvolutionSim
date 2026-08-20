namespace EvolutionSim;

public class TestState : State
{
    public override void Update()
    {
        if (IsKeyPressed(KeyboardKey.Q))
        {
            TransitionTo(new PlayState());
        }
    }
    public override void Draw()
    {
        ClearBackground(Color.DarkGray);
        DrawText("this works?!", 12, 12, 20, Color.Red);
        DrawTexture(GetTexture("banjo.png"), 0, 0, Color.White);
    }
}