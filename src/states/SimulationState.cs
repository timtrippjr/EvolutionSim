namespace EvolutionSim;

public class SimulationState : State
{
    private List<Entity> _entities = [];
    private Entity? _hover;
    private InfoPane _infoPane = new();

    private Camera2D _camera;
    public Camera2D Camera => _camera;
    private float _zoomTarget;

    public override void Enter()
    {
        
        for (int i = 0; i < 2; i++)
            _entities.Add(new Animal(GetRandomPosition()));
        for (int i = 0; i < 20; i++)
            _entities.Add(new Food(
                (FoodType)(i % 2), 
                GetRandomPosition()
            ));

        _camera.Target = Vector2.Zero;
        _camera.Offset = Vector2.Zero;
        _camera.Zoom = 1.0f;
        _zoomTarget = 1.0f;

    }

    public void UpdateCamera()
    {
		Vector2 mouseScreen = GetMousePosition() / WindowScale;
		Vector2 preZoomWorldPos = GetScreenToWorld2D(mouseScreen, _camera);

        _zoomTarget += GetMouseWheelMove() * 0.1f;
        _camera.Zoom = SmoothDamp(_camera.Zoom, _zoomTarget, 6);
        
		Vector2 postZoomWorldPos = GetScreenToWorld2D(mouseScreen, _camera);

		_camera.Target += preZoomWorldPos - postZoomWorldPos;
        if (IsMouseButtonDown(MouseButton.Right))
            _camera.Target -= GetMouseDelta() / WindowScale / _camera.Zoom;

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

        if (IsKeyPressed(KeyboardKey.A)||IsKeyPressedRepeat(KeyboardKey.A))
            _entities.Add(new Animal(GetRandomPosition()));
        if (IsKeyPressed(KeyboardKey.F)||IsKeyPressedRepeat(KeyboardKey.F))
            _entities.Add(new Food(
                (FoodType)Rng.Next(2), 
                GetRandomPosition()
            ));
        if (IsMouseButtonPressed(MouseButton.Left))
            foreach (var e in _entities)
                if (CheckCollisionPointRec(
                    GetScreenToWorld2D(
                        GetMousePosition() / WindowScale, _camera
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

        var snapshot = _entities.ToList();
        snapshot.ForEach(e => e.Update(snapshot, _hover == e));

        var newEntities = _entities
            .Where(e => e.shouldDie)
            .SelectMany(e => e.Children)
            .ToList();
        _entities.RemoveAll(e => e.shouldDie);

        _entities.AddRange(newEntities);
    }
    public override void Draw()
    {
        BeginMode2D(_camera);
            ClearBackground(DarkerGray);
            DrawRectangle(0, 0, WindowWidth, WindowHeight, Color.DarkGray);
            DrawRectangleLinesEx(
                new(-1, -1, WindowWidth + 1, WindowHeight + 1), 
                1, 
                Color.Black
            );
            _entities
                .OrderBy(e => e.Position.Y)
                .ToList()
                .ForEach(e => e.Draw());
        EndMode2D();

        _infoPane.Draw(_hover, _camera);
        
    }
}