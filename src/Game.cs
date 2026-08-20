namespace EvolutionSim;

public class Game
{
    private bool _done = false;
    private State? _state;
    private RenderTexture2D _buffer;

    public void Init(State initState)
    {
        InitWindow(
            WindowWidth * WindowScale, 
            WindowHeight * WindowScale, 
            WindowTitle
        );
        SetTargetFPS(60);
        SetWindowIcon(GetImage(IconName));
        SetExitKey(KeyboardKey.Null);
        SetState(initState);

        _buffer = LoadRenderTexture(WindowWidth, WindowHeight);
    }

    public void Close()
    {
        CloseWindow();
    }

    public void Update()
    {
        _state?.Update();

        if (_state?.done == true)
            SetState(_state.next);
    }

    public void Draw()
    {
        BeginTextureMode(_buffer);
            _state?.Draw();
            DrawFont($"fps: {GetFPS()}", Color.White, 1, 0, 0);
        EndTextureMode();

        BeginDrawing();
            ClearBackground(Color.Black);
            DrawTexturePro(
                _buffer.Texture,
                new Rectangle(0, 0, 
                    _buffer.Texture.Width, 
                    -_buffer.Texture.Height
                ),
                new Rectangle(0, 0, 
                    WindowWidth * WindowScale, 
                    WindowHeight * WindowScale
                ),
                Vector2.Zero,
                0,
                Color.White
            );
        EndDrawing();
    }

    public void SetState(State? next)
    {
        _state?.Exit();
        _state = next; 
        _done = _state is null;
        _state?.Enter();  
    }

    public void Begin(State initState)
    {
        Init(initState);
        while (!WindowShouldClose() && !_done)
        {
            Update();
            Draw();
        }
        Close();
    }

}