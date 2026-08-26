namespace EvolutionSim;

public struct BarItem
{
    public float Part;
    public float Whole;
    public Color Color;
    public bool IsAscendingPriority;
    public AnimalPriority Priority;
    public BarItem(AnimalPriority priority, float part, float whole, Color color)
    {
        Priority = priority;
        Part = part;
        Whole = whole;
        Color = color;
    }
}

public enum AnimalPriority
{
    Hunger,
    Thirst,
    Energy,
    Mating
}

enum AnimalState
{
    Standing,
    Wandering,
    Sleeping,
}

public class Animal : Entity
{
    private float _moveDirection;
    private Vector2 _moveOrigin;
    private AnimalState _state = AnimalState.Standing;
    private TimeSpan _stateTimeLeft = TimeSpan.FromSeconds(1);
    private TimeSpan _lifeExpectancy = TimeSpan.FromMinutes(5);
    private Vector2 _baseFrameSize = new(32, 32);
    private float _textureRotation = 0;
    private float _interactRadius = 10; // radius for eating, drinking, mating.

    // existence stuff
    public string Name { get; set; }
    public Color Color { get; set; }
    public int Speed { get; set; } // affects speed
    public int Sight { get; set; } // affects how far it can see around

    //bars
    private float _hunger;
    private float _thirst;
    private float _energy;
    private float _matingDrive;
    public float MaxHealth { get; set; } = 100;
    public float _maxHunger = 100;
    public float _maxThirst = 100;
    public float _maxEnergy = 100;
    public float _maxMating = 100;

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
    public float Mating
    {
        get => _matingDrive;
        set => _matingDrive = SetNeed(value, _maxMating);
    }

    public float Health { get; set; }
    private readonly BarItem[] _bars = new BarItem[3]; //set to 4 for water
    public BarItem[] BarValues
    {
        get
        {
            _bars[0] = new(AnimalPriority.Energy, Energy, _maxEnergy, Color.Orange);
            _bars[1] = new(AnimalPriority.Hunger, Hunger, _maxHunger, Color.DarkBrown);
            _bars[2] = new(AnimalPriority.Mating, Mating, _maxMating, Color.Pink)
            { IsAscendingPriority = true };
            //_bars[3] = new(AnimalPriority.Thirst, Thirst, _maxThirst, Color.SkyBlue);

            //this is fine but barely works. find something better.
            Array.Sort(_bars, (a, b) =>
            {
                bool aDrive = a.IsAscendingPriority;
                bool bDrive = b.IsAscendingPriority;

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
    public AnimalPriority Priority
    {
        get => BarValues[0].Priority;   
    }

    public Animal(int x, int y, Color color, int speed, int sight, TimeSpan age) 
        : base(x, y, GetTexture("animal.png"))
    {
        Color = color;
        Speed = speed;
        Sight = sight;
        Age = age;
        Name = NameGenerator.GetRandomName(3, 7);
        Hunger = _maxHunger;
        Thirst = _maxThirst;
        Energy = _maxEnergy;
        Mating = 0;
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

    private void Speak(string word)
    {
        Console.WriteLine($"{Name}: {word}");
    }

    private void ActUponPriority(List<Entity>? entities)
    {
        if (Priority is 
            AnimalPriority.Hunger or 
            AnimalPriority.Thirst or 
            AnimalPriority.Mating
        )
        {   
            _state = AnimalState.Wandering;
            _moveOrigin = Position;

            //if there is our priority IN SIGHT RADIUS, use its position
            //else, random.
            Vector2 target = GetRandomPosition();

            foreach (Entity entity in entities ?? [])
            {
                if (Priority is AnimalPriority.Hunger && entity is Food){
                    // if food within sight, set its position to be target
                }
                if (Priority is AnimalPriority.Mating && entity is Animal){
                    // if animal within sight, set its position to be target
                }
                /*
                if (Priority is AnimalPriority.Thirst && entity is WaterBody){
                    // if body of water within sight, set any of its cells positions to be target
                }
                */
            }

            _moveDirection = (float)Math.Atan2(
                target.Y - Position.Y, 
                target.X - Position.X
            );
        }

        // if in search of energy, SLEEP!!
        if (Priority is AnimalPriority.Energy)
        {
            Speak("sleepy time");
            _state = AnimalState.Sleeping;
            _stateTimeLeft = TimeSpan.FromSeconds(Rng.Next(6, 20));
        }
    }

    private void UpdateStanding(List<Entity>? entities, TimeSpan delta)
    {
        _stateTimeLeft -= delta;
        if (_stateTimeLeft >= TimeSpan.Zero) return; 
        
        Speak("Exit Standing: "+Priority.ToString());
        
        ActUponPriority(entities);
    }
    private void UpdateWandering()
    {
        //stateOver = have I reached goal?
        //in my current direction, 
        //is my distance from origin bigger than Sight from origin
        Vector2 direction = new(
            (float)Math.Cos(_moveDirection),
            (float)Math.Sin(_moveDirection)
        );
        
        Hunger -= Speed * DeltaTime() * 0.1f;
        Energy -= Speed * DeltaTime() * 0.15f;

        Position += direction * Speed * DeltaTime();

        if (GetSquaredDistBetween(Position, _moveOrigin) > Sight * Sight)
        {
            _state = AnimalState.Standing;
            _stateTimeLeft = TimeSpan.FromSeconds(Rng.Next(1, 20) / 10.0f);
        }
    }
    private void UpdateSleeping(List<Entity>? entities, TimeSpan delta)
    {
        _textureRotation += Speed * 20 * DeltaTime();

        Energy += 2 * DeltaTime();
        if (Energy >= _maxEnergy) _stateTimeLeft = TimeSpan.Zero;

        _stateTimeLeft -= delta;
        if (_stateTimeLeft >= TimeSpan.Zero) return; 
        
        Speak("Exit Sleeping: "+Priority.ToString());

        _textureRotation = 0;
        ActUponPriority(entities);
    }

    public override void Update(List<Entity>? entities, bool beingHovered)
    {
        base.Update(entities, beingHovered);

        TimeSpan newTime = TimeSpan.FromSeconds(DeltaTime());
        Age += newTime;
        
        switch (_state)
        {
            case AnimalState.Standing: UpdateStanding(entities, newTime); break;
            case AnimalState.Wandering: UpdateWandering(); break;
            case AnimalState.Sleeping: UpdateSleeping(entities, newTime); break;
        }

        Mating += 0.4f * DeltaTime();
        //Thirst -= 0.2f * DeltaTime();

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
            DrawCircleLinesV(Position, _interactRadius, Color.Yellow);
            DrawLineDashed(_moveOrigin, Position, 4, 4, Color.Red);
            BeginShaderMode(_outlineShader);
        }

        DrawTexturePro(Texture, 
            new(Vector2.Zero, _baseFrameSize),
            new(Position, FrameSize),
            Origin, _textureRotation, Color
        );
        if (_beingHovered) EndShaderMode();
    }
}