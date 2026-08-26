namespace EvolutionSim;

public struct BarItem
{
    public string Name;
    public float Part;
    public float Whole;
    public Color Color;
    public bool AscendingPriority;
    public BarItem(string name, float part, float whole, Color color, bool p)
    {
        Name = name;
        Part = part;
        Whole = whole;
        Color = color;
        AscendingPriority = p;
    }
    public BarItem(string n, float p, float w, Color c)
        : this(n, p, w, c, false){}
}

enum AnimalState
{
    Standing,
    Wandering,

    Eating,
    Drinking,
    Sleeping,
    Reproducing,
}

//something the animal aims to walk toward in its wandering state
enum AnimalDesire
{
    Food,
    Drink,
    Partner,
}

public class Animal : Entity
{
    private float _moveDirection;
    private Vector2 _moveOrigin;
    private Vector2 _baseFrameSize = new(32, 32);
    private AnimalState _state = AnimalState.Standing;
    private TimeSpan _stateTimeLeft = TimeSpan.FromSeconds(1);
    private TimeSpan _lifeExpectancy = TimeSpan.FromMinutes(5);

    // existence stuff
    public string Name { get; set; }
    public Color Color { get; set; }
    public int Speed { get; set; } // affects speed
    public int Sight { get; set; } // affects how far it can see around

    //bars
    private float _hunger;
    private float _thirst;
    private float _energy;
    private float _reproduce;
    public float MaxHealth { get; set; } = 100;
    public float _maxHunger = 100;
    public float _maxThirst = 100;
    public float _maxEnergy = 100;
    public float _maxReproduce = 100;

    private float SetNeed(float value, float max)
    {
        if (value < 0)
        {
            float remainder = -value;
            Health -= remainder;
            return 0;
        }
        //commented out temporarily for testing
        //if (value > max) return max;

        return value;
    }

    public float Hunger
    {
        get => _hunger;
        set => _hunger = SetNeed(value, _maxHunger);
    }
    public float Thirst
    {
        get => _thirst;
        set => _thirst = SetNeed(value, _maxThirst);
    }
    public float Energy
    {
        get => _energy;
        set => _energy = SetNeed(value, _maxEnergy);
    }
    public float Reproduce
    {
        get => _reproduce;
        set => _reproduce = SetNeed(value, _maxReproduce);
    }

    public float Health { get; set; }
    private readonly BarItem[] _bars = new BarItem[4];
    public BarItem[] BarValues
    {
        get
        {
            _bars[0] = new("Energy", Energy, _maxEnergy, Color.Orange);
            _bars[1] = new("Hunger", Hunger, _maxHunger, Color.DarkBrown);
            _bars[2] = new("Thirst", Thirst, _maxThirst, Color.SkyBlue);
            _bars[3] = new(
                "Reproduce drive", 
                Reproduce, 
                _maxReproduce, 
                Color.Pink, 
                true
            );
            
            //this is fine but barely works. find something better.
            Array.Sort(_bars, (a, b) =>
            {
                bool aDrive = a.AscendingPriority;
                bool bDrive = b.AscendingPriority;

                if (aDrive || bDrive)
                {
                    BarItem drive = aDrive ? a : b;
                    BarItem other = aDrive ? b : a;

                    if (drive.Part > other.Part)
                        return aDrive ? -1 : 1;

                    return aDrive ? 1 : -1;
                }

                return a.Part.CompareTo(b.Part);
            });

            return _bars;
        }
    }


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
        Name = NameGenerator.GetRandomName(2, 10);
        Hunger = _maxHunger;
        Thirst = _maxThirst;
        Energy = _maxEnergy;
        Reproduce = 0;
        Health = MaxHealth;
        _moveOrigin = Position;
    }
    public Animal(Vector2 pos) 
        : this(
            (int)pos.X, 
            (int)pos.Y, 
            GetRandomColor(),
            Rng.Next(10, 20),
            Rng.Next(20, 60),
            TimeSpan.Zero
        ) 
    {}

    public override void Update(List<Entity>? entities, bool beingHovered)
    {
        base.Update(entities, beingHovered);

        TimeSpan newTime = TimeSpan.FromSeconds(DeltaTime());
        Age += newTime;

        BarItem ta = BarValues[0];
        if (_beingHovered)
        {
            Console.WriteLine(ta.Name);
        }

        Reproduce += 1 * DeltaTime();
        Thirst -= 0.2f * DeltaTime();

        bool stateOver = _stateTimeLeft < TimeSpan.Zero;
        switch (_state)
        {
            case AnimalState.Standing:
                if (stateOver)
                {
                    _state = AnimalState.Wandering;
                    _moveOrigin = Position;
                    Vector2 target = GetRandomPosition();
                    _moveDirection = (float)Math.Atan2(
                        target.Y - Position.Y, 
                        target.X - Position.X
                    );
                }
                break;
            case AnimalState.Wandering:
                //stateOver = have I reached goal?
                //in my current direction, 
                //is my distance from origin bigger than Sight from origin
                Vector2 direction = new(
                    (float)Math.Cos(_moveDirection),
                    (float)Math.Sin(_moveDirection)
                );
                stateOver = 
                    GetSquaredDistBetween(Position, _moveOrigin) > 
                    Sight * Sight;

                //sight is the hypotenuse of the triangle
                //theta is the random direction we pick

                Hunger -= Speed * DeltaTime() * 0.1f;
                Energy -= Speed * DeltaTime() * 0.15f;

                Position += direction * Speed * DeltaTime();

                if (stateOver)
                {
                    _state = AnimalState.Standing;
                    _stateTimeLeft = TimeSpan.FromSeconds(Rng.Next(2, 5));
                }
                break;
        }
        _stateTimeLeft -= newTime;

        if (Health <= 0) shouldDie = true;
        if (Age > _lifeExpectancy) shouldDie = true;

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


        if (_beingHovered) 
        {
            Vector2 rawTexel = new(1.0f / Texture.Width, 1.0f / Texture.Height);
            Vector2 scale = FrameSize / _baseFrameSize;
            SetShaderValue(_outlineShader, 
                GetShaderLocation(_outlineShader, "texelSize"), 
                rawTexel / scale, 
                ShaderUniformDataType.Vec2
            );
            
            DrawCircleLinesV(_moveOrigin, 3, Color.Red);
            DrawCircleLinesV(Position, Sight, Color.White);
            DrawLineDashed(_moveOrigin, Position, 4, 4, Color.Red);
            BeginShaderMode(_outlineShader);
        }

        DrawTexturePro(Texture, 
            new(Vector2.Zero, _baseFrameSize),
            new(Position, FrameSize),
            Origin, 0, Color
        );
        if (_beingHovered) EndShaderMode();
    }
}