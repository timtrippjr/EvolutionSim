namespace EvolutionSim;

public class Game
{
    public State? state;
    public bool done = false;

    public void Init(State initState)
    {
        InitWindow(800, 480, "WOW!! IM ECSTATIC!!");
        SetExitKey(KeyboardKey.Null);
        SetTargetFPS(30);
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
        {
            Console.WriteLine("I am here okay?");
            SetState(state.next);
        }
    }

    public void Draw()
    {
        BeginDrawing();
            state?.Draw();
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