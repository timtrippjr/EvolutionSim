namespace EvolutionSim;

enum AnimalState
{
    Standing,
    Walking,
    Eating,
    Drinking,
    Reproducing,
    Fleeing
}

public class Animal : Entity
{
    private float _moveDirection;
    private Vector2 _moveOrigin;
    private Vector2 _baseFrameSize = new(32, 32);
    private AnimalState _state = AnimalState.Standing;
    private TimeSpan _stateTimeLeft = TimeSpan.FromSeconds(1);

    // existence stuff
    private Color _color;
    private int _speed; // affects speed
    private int _sight; // affects how far it can see around

    //
    private TimeSpan _age; // TimeSpan.Zero
    private float _hunger = 1;
    private float _thirst;

    public Animal(
        int x, 
        int y, 
        Color color, 
        int speed, 
        int sight, 
        TimeSpan age
    ) 
        : base(x, y, GetTexture("animal.png"))
    {
        _color = color;
        _speed = speed;
        _sight = sight;
        _age = age;
    }
    public Animal(Vector2 pos) 
        : this(
            (int)pos.X, 
            (int)pos.Y, 
            GetRandomColor(),
            Rng.Next(10, 20),
            Rng.Next(80, 100),
            TimeSpan.Zero
        ) 
    {}

    public override void Update(List<Entity>? entities, bool beingHovered)
    {
        base.Update(entities, beingHovered);

        TimeSpan newTime = TimeSpan.FromSeconds(DeltaTime());
        _age += newTime;

        bool stateOver = _stateTimeLeft < TimeSpan.Zero;
        switch (_state)
        {
            case AnimalState.Standing:
                if (stateOver)
                {
                    _state = AnimalState.Walking;
                    _moveOrigin = Position;
                    Vector2 target = GetRandomPosition();
                    _moveDirection = (float)Math.Atan2(
                        target.Y - Position.Y, 
                        target.X - Position.X
                    );
                }
                break;
            case AnimalState.Walking:
                //stateOver = have I reached goal?
                //in my current direction, is my distance from origin bigger than _sight from origin
                Vector2 direction = new(
                    (float)Math.Cos(_moveDirection),
                    (float)Math.Sin(_moveDirection)
                );
                stateOver = 
                    GetSquaredDistBetween(Position, _moveOrigin) > 
                    _sight * _sight;

                //sight is the hypotenuse of the triangle
                //theta is the random direction we pick

                Position += direction * _speed * DeltaTime();

                if (stateOver)
                {
                    _state = AnimalState.Standing;
                    _stateTimeLeft = TimeSpan.FromSeconds(Rng.Next(2, 5));
                }
                break;
        }
        _stateTimeLeft -= newTime;

        //update sprite
        //todo
    }

    public override void Draw()
    {
        float scalar = (float)_age.TotalMinutes / 2 + (_baseFrameSize.X / 60);
        FrameSize = new(
            Math.Min(_baseFrameSize.X, _baseFrameSize.X * scalar),
            Math.Min(_baseFrameSize.Y, _baseFrameSize.Y * scalar)
        );
        
        if (_beingHovered) BeginShaderMode(_outlineShader);
            DrawTexturePro(Texture, 
                new(Vector2.Zero, _baseFrameSize), 
                new(Position, FrameSize), 
                Origin, 0, _color
            );
        if (_beingHovered) EndShaderMode();   
    }
}