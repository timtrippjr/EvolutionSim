namespace EvolutionSim;

public class PlayState : State
{
    public override void Update()
    {
        if (IsKeyPressed(KeyboardKey.F))
        {
            Console.WriteLine("Dude, I'm pressing the freakin' F key.");

            TransitionTo(new TestState());
        }

        if (IsKeyPressed(KeyboardKey.Q))
        {
            Quit();
        }
    }
    public override void Draw()
    {
        ClearBackground(Color.White);
        DrawText("i am inside game class!", 12, 12, 20, Color.Black);
    }
}