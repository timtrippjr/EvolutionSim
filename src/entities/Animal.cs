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
    public Color Color { get; set; }
    public int Speed { get; set; } // affects speed
    public int Sight { get; set; } // affects how far it can see around

    //
    public float MaxHealth { get; set; } = 100;
    public float Health { get; set; }
    public float MaxHunger { get; set; } = 100;
    private float _hunger;
    public float Hunger { get => _hunger; set
        {
            if (_hunger < 0)
            {
                float remainder = Math.Abs(Hunger);
                Health -= remainder;
                _hunger = 0;
            }
            else _hunger = value;
        } 
    }
    public float MaxThirst { get; set; } = 100;
    public float Thirst { get; set; }

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
        Color = color;
        Speed = speed;
        Sight = sight;
        Age = age;
        Hunger = MaxHunger;
        Thirst = MaxThirst;
        Health = MaxHealth;
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
        Age += newTime;

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
                //in my current direction, is my distance from origin bigger than Sight from origin
                Vector2 direction = new(
                    (float)Math.Cos(_moveDirection),
                    (float)Math.Sin(_moveDirection)
                );
                stateOver = 
                    GetSquaredDistBetween(Position, _moveOrigin) > 
                    Sight * Sight;

                //sight is the hypotenuse of the triangle
                //theta is the random direction we pick

                Hunger -= Speed * 0.001f;

                Position += direction * Speed * DeltaTime();

                if (stateOver)
                {
                    _state = AnimalState.Standing;
                    _stateTimeLeft = TimeSpan.FromSeconds(Rng.Next(2, 5));
                }
                break;
        }
        _stateTimeLeft -= newTime;

        if (Health < 0) shouldDie = true;

        //update sprite
        //todo
    }

    public override void Draw()
    {
        float scalar = (float)Age.TotalMinutes / 2 + (_baseFrameSize.X / 60);
        FrameSize = new(
            Math.Min(_baseFrameSize.X, _baseFrameSize.X * scalar),
            Math.Min(_baseFrameSize.Y, _baseFrameSize.Y * scalar)
        );
        
        if (_beingHovered) BeginShaderMode(_outlineShader);
            DrawTexturePro(Texture, 
                new(Vector2.Zero, _baseFrameSize), 
                new(Position, FrameSize), 
                Origin, 0, Color
            );
        if (_beingHovered) EndShaderMode();   
    }
}