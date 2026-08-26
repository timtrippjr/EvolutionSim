namespace EvolutionSim;

public class SimulationState : State
{
    private List<Entity> _entities = [];
    private readonly List<Entity> _entitySnapshot = [];
    private readonly List<Entity> _newEntities = [];

    private Entity? _hover;
    private InfoPane _infoPane = new();

    private Camera2D _camera;
    public Camera2D Camera => _camera;
    private float _zoomTarget;

    private World _world = new(1280, 720);

    public override void Enter()
    {
        
        for (int i = 0; i < 2; i++)
            _entities.Add(new Animal(_world.GetRandomPosition()));
        for (int i = 0; i < 40; i++)
            _entities.Add(new Food(
                (FoodType)(i % 2), 
                _world.GetRandomPosition()
            ));

        _camera.Target = Vector2.Zero;
        _camera.Offset = Vector2.Zero;
        _camera.Zoom = 1.0f;
        _zoomTarget = 1.0f;

    }

    public void UpdateCamera()
    {
		Vector2 mouseScreen = GetMousePosition();
		Vector2 preZoomWorldPos = GetScreenToWorld2D(mouseScreen, _camera);

        _zoomTarget += GetMouseWheelMove() * 0.1f;
        _camera.Zoom = SmoothDamp(_camera.Zoom, _zoomTarget, 6);
        
		Vector2 postZoomWorldPos = GetScreenToWorld2D(mouseScreen, _camera);

		_camera.Target += preZoomWorldPos - postZoomWorldPos;
        if (IsMouseButtonDown(MouseButton.Right))
            _camera.Target -= GetMouseDelta() / _camera.Zoom;

        //reset buttons
        if (IsKeyPressed(KeyboardKey.One))
            _zoomTarget = 1;
        if (IsKeyPressed(KeyboardKey.Two))
            _zoomTarget = 2;
        if (IsKeyPressed(KeyboardKey.Three))
            _zoomTarget = 3;
        
    }

    public override void Update()
    {
        if (IsKeyPressed(KeyboardKey.Escape))
            TransitionTo(new TitleState());

        /*
        if (IsKeyPressed(KeyboardKey.A)||IsKeyPressedRepeat(KeyboardKey.A))
            _entities.Add(new Animal(_world.GetRandomPosition()));
        if (IsKeyPressed(KeyboardKey.F)||IsKeyPressedRepeat(KeyboardKey.F))
            _entities.Add(new Food(
                (FoodType)Rng.Next(2), 
                _world.GetRandomPosition()
            ));
        */
        
        if (IsMouseButtonPressed(MouseButton.Left))
            foreach (var e in _entities)
                if (CheckCollisionPointRec(
                    GetScreenToWorld2D(
                        GetMousePosition(), _camera
                    ),
                    new(e.Position - e.Origin, e.FrameSize)
                ))
                {
                    if (_hover == e) _hover = null;
                    else _hover = e;
                    break;
                }else _hover = null;

        //_camera
        UpdateCamera();
        //

        if (IsKeyPressed(KeyboardKey.H))
            if (_hover is Animal a) a.Energy += 20;
        if (_hover?.shouldDie ?? false) _hover = null;

        _entitySnapshot.Clear();
        _entitySnapshot.AddRange(_entities);
        foreach (var e in _entitySnapshot)
        {
            e.Update(_entitySnapshot, _world, _hover == e);
            
            if (e.Children is { Count: > 0 })
            {
                _newEntities.AddRange(e.Children);
                e.Children.Clear();
            }
        }
        _entities.RemoveAll(e => e.shouldDie);
        if (_newEntities.Count > 0)
        {
            _entities.AddRange(_newEntities);
            _newEntities.Clear();
        }
    }
    public override void Draw(RenderTexture2D buff)
    {
        ClearBackground(DarkerGray);

        BeginMode2D(_camera);
            _world.Draw();
            _entities
                .OrderBy(e => e.Position.Y)
                .ToList()
                .ForEach(e => e.Draw());
        EndMode2D();

        
        DrawToTexture(buff, () =>
            _infoPane.Draw(_hover, _camera)
        );
        
    }
}