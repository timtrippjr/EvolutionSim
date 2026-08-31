namespace EvolutionSim;

public class SimulationState : State
{
    private List<Entity> _entities = [];
    private readonly List<Entity> _entitySnapshot = [];
    private readonly List<Entity> _newEntities = [];

    private TimeSpan _spawnFoodWaitTime = TimeSpan.FromSeconds(30);
    private TimeSpan _spawnFoodTime;

    private Texture2D _ffTex = GetTexture("ff.png");

    private Entity? _hover;
    private InfoPane _infoPane = new();

    private Camera2D _camera;
    public Camera2D Camera => _camera;
    private float _zoomTarget;

    private World _world;

    private int MaxTimeMult = 16;

    public SimulationState(int worldWidth, int worldHeight)
    {
        _world = new(worldWidth, worldHeight);
    }

    public override void Enter()
    {
        base.Enter();
        
        for (int i = 0; i < 18; i++)
            _entities.Add(new Animal(_world.GetRandomLandPosition()));

        int plantAmount = (255 - _world.WaterLevel) / 4;
        Console.WriteLine("spawning plants: "+plantAmount);
        for (int i = 0; i < plantAmount; i++)
            _entities.Add(new Food(
                (FoodType)(i % 2), 
                _world.GetRandomGrassPosition()
            ));

        _camera.Target = Vector2.Zero;
        _camera.Offset = Vector2.Zero;
        _camera.Zoom = WindowScale;
        _zoomTarget = WindowScale;

        TimeMultiple = 1;

    }

    public override void Exit()
    {
        base.Exit();
        _world.Unload();
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
        if (IsKeyPressed(KeyboardKey.R))
            _camera.Target = Vector2.Zero;
        
    }

    public override void Update()
    {
        if (IsKeyPressed(KeyboardKey.Escape))
            TransitionTo(new TitleState());

        _spawnFoodTime -= TimeSpan.FromSeconds(DeltaTime());
        if (_spawnFoodTime < TimeSpan.Zero)
        {
            _spawnFoodTime = _spawnFoodWaitTime;
            Console.WriteLine("Spawned more food");
            
            for (int i = 0; i < 4; i++)
                _entities.Add(new Food(
                    (FoodType)Rng.Next(2), 
                    _world.GetRandomGrassPosition()
                ));
        }
        
        if (IsKeyPressed(KeyboardKey.A)||IsKeyPressedRepeat(KeyboardKey.A))
            _entities.Add(new Animal(_world.GetRandomLandPosition()));
        if (IsKeyPressed(KeyboardKey.F)||IsKeyPressedRepeat(KeyboardKey.F))
            _entities.Add(new Food(
                (FoodType)Rng.Next(2), 
                _world.GetRandomGrassPosition()
            ));

        if (IsKeyPressed(KeyboardKey.Right) || IsKeyPressedRepeat(KeyboardKey.Right))
        {
            TimeMultiple +=TimeMultiple < 1?0.25f:1;
            if (TimeMultiple > MaxTimeMult) TimeMultiple = MaxTimeMult;
        }
        if (IsKeyPressed(KeyboardKey.Left) || IsKeyPressedRepeat(KeyboardKey.Left))
        {
            TimeMultiple -=TimeMultiple <= 1?0.25f:1;
            if (TimeMultiple < 0) TimeMultiple = 0;
        }
        
        
        
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
        {
            _infoPane.Draw(_hover, _camera);
            if (TimeMultiple != 1)
            {
                DrawTextureEx(_ffTex, new(50,0), 0, 0.15f, Color.White);
                DrawFont("FF: x"+TimeMultiple, Color.White, 1, 70,0);
            }
        }
        );
        
    }
}