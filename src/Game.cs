namespace EvolutionSim;

public class Game
{
    public State? state;
    public bool done = false;

    public void Init(State initState)
    {
        InitWindow(WindowWidth, WindowHeight, WindowTitle);
        SetTargetFPS(30);
        SetWindowIcon(GetImage(IconName));
        SetExitKey(KeyboardKey.Null);
        SetState(initState);
    }

    public void Close()
    {
        CloseWindow();
    }

    public void Update()
    {
        state?.Update();

        if (state?.done == true)
            SetState(state.next);
    }

    public void Draw()
    {
        BeginDrawing();
            state?.Draw();
            DrawFPS(0, 0);
        EndDrawing();
    }

    public void SetState(State? next)
    {
        state?.Exit();
        state = next; 
        done = state is null;
        state?.Enter();  
    }

    public void Begin(State initState)
    {
        Init(initState);
        while (!WindowShouldClose() && !done)
        {
            Update();
            Draw();
        }
        Close();
    }

}