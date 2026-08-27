namespace EvolutionSim;

public class TitleState : State
{
    private TextBox _worldWidth = new("world width: (default 1280)",30, 100, 50, 16);
    private TextBox _worldHeight = new("world height: (default 720)",30, 140, 50, 16);

    public override void Update()
    {
        _worldWidth.Update();
        _worldHeight.Update();

        if (IsKeyPressed(KeyboardKey.Enter))
            TransitionTo(new SimulationState(
                _worldWidth.IntValue == 0? 1280 : _worldWidth.IntValue,
                _worldHeight.IntValue == 0? 720 : _worldHeight.IntValue
            ));
        
        if (IsKeyPressed(KeyboardKey.Escape))
            Quit();
    }
    public override void Draw(RenderTexture2D buff)
    {
        DrawToTexture(buff, () =>
        {
            ClearBackground(Color.Black);
            DrawText("this works?!", 12, 12, 10, Color.DarkGray);
            DrawText("enter to begin", 12, 200, 10, Color.Green);
            DrawFont("Evolution Simulator!", Color.Gray, 3, 12, 30);
            _worldWidth.Draw();
            _worldHeight.Draw();
        });
    }
}