namespace EvolutionSim;

public struct BarItem
{
    public float Part;
    public float Whole;
    public Color Color;
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
    private Entity? _moveTarget;
    private AnimalState _state = AnimalState.Standing;
    private TimeSpan _stateTimeLeft = TimeSpan.FromSeconds(1);
    private TimeSpan _lifeExpectancy = TimeSpan.FromMinutes(10);
    private TimeSpan _mateExpectancy = TimeSpan.FromSeconds(30);
    private Vector2 _baseFrameSize = new(32, 32);
    private float _textureRotation = 0;
    private float _interactRadius = 7; // radius for eating, drinking, mating.

    // existence stuff
    public string Name { get; set; }
    public Color Color { get; set; }
    public int Speed { get; set; } // affects speed
    public int Sight { get; set; } // affects how far it can see around

    //bars
    private float _hunger;
    private float _thirst;
    private float _energy;
    public static float MaxHealth { get; set; } = 100;
    public static float MaxHunger = 100;
    public static float MaxThirst = 100;
    public static float MaxEnergy = 100;

    public bool ReadyToMate {
        get
        {
            return 
                Hunger >= 75 && 
                Energy >= 75 && 
                Age >= _mateExpectancy;
        } 
    }

    private float SetNeed(float value, float max)
    {
        if (value < 0)
        {
            float remainder = -value;
            Health -= remainder;
            return 0;
        }
        if (value > max) return max;

        return value;
    }

    public float Health { get; set; }
    public float Hunger
    {
        get => _hunger;
        set => _hunger = SetNeed(value, MaxHunger);
    }
    public float Thirst
    {
        get => _thirst;
        set => _thirst = SetNeed(value, MaxThirst);
    }
    public float Energy
    {
        get => _energy;
        set => _energy = SetNeed(value, MaxEnergy);
    }

    private readonly BarItem[] _bars = new BarItem[2]; //set to 3 for water
    public BarItem[] BarValues
    {
        get
        {
            _bars[0] = new(AnimalPriority.Energy, Energy, MaxEnergy, Color.Orange);
            _bars[1] = new(AnimalPriority.Hunger, Hunger, MaxHunger, Color.DarkBrown);
            //_bars[2] = new(AnimalPriority.Thirst, Thirst, _maxThirst, Color.SkyBlue);

            Array.Sort(_bars, (a, b) => a.Part.CompareTo(b.Part));

            return _bars;
        }
    }
    public AnimalPriority Priority
    {
        get
        {
            if (ReadyToMate) return AnimalPriority.Mating;
            return BarValues[0].Priority;
        } 
    }

    public Animal(
        int x, int y, 
        Color color, int speed, int sight, 
        TimeSpan age,
        float hunger, float thirst, float energy        
    ) 
        : base(x, y, GetTexture("animal.png"))
    {
        Name = NameGenerator.GetRandomName(3, 7);
        Color = color;
        Speed = speed;
        Sight = sight;
        Age = age;
        Hunger = hunger;
        Thirst = thirst;
        Energy = energy;
        Health = MaxHealth;
        _moveOrigin = Position;

        //Speak("i am new baby: h"+Hunger+" t"+Thirst+" e"+Energy);
    }
    public Animal(Vector2 pos) 
        : this(
            (int)pos.X, (int)pos.Y, 
            GetRandomColor(), Rng.Next(30, 50), Rng.Next(40, 100),
            TimeSpan.Zero,
            MaxHunger, MaxThirst, MaxEnergy
        ) 
    {}

    private void Speak(string word)
    {
        Console.WriteLine($"{Name}: {word}");
    }

    private Animal GetBaby(Animal a)
    {
        //create the child
        //for now, just an average of both parents, with some mutation
        //messy but its okay.
        int driftAmt = Rng.Next(10, 40);
        byte r = (byte)Math.Clamp(((Color.R + a.Color.R) / 2) + Rng.Next(-driftAmt, driftAmt), 0, 255);
        byte b = (byte)Math.Clamp(((Color.B + a.Color.B) / 2) + Rng.Next(-driftAmt, driftAmt), 0, 255);
        byte g = (byte)Math.Clamp(((Color.G + a.Color.G) / 2) + Rng.Next(-driftAmt, driftAmt), 0, 255);
        Color childColor = new Color(r, g, b);
        return new Animal(
            (int)(Position.X + a.Position.X) / 2,
            (int)(Position.Y + a.Position.Y) / 2,
            childColor,
            Math.Abs(((Speed + a.Speed) / 2) + Rng.Next(-driftAmt, driftAmt)),
            Math.Abs(((Sight + a.Sight) / 2) + Rng.Next(-driftAmt, driftAmt)),
            TimeSpan.Zero,
            50, 50, 50
        );
    }

    private void ActUponPriority(List<Entity>? entities, World world)
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
            foreach (Entity entity in entities ?? [])
            {
                if (entity == this) continue;

                if (GetSquaredDistBetween(Position, entity.Position) <= Sight * Sight)
                    if (
                        (Priority is AnimalPriority.Hunger && entity is Food) ||
                        (Priority is AnimalPriority.Mating && entity is Animal)
                        //|| (Priority is AnimalPriority.Thirst && entity is WaterBody)
                    ){
                        _moveTarget = entity;
                        break;
                    }
            }

            Vector2 target = world.GetRandomPosition();
            if (_moveTarget != null) target = _moveTarget.Position;

            _moveDirection = (float)Math.Atan2(
                target.Y - Position.Y, 
                target.X - Position.X
            );
        }

        // if in search of energy, SLEEP!!
        if (Priority is AnimalPriority.Energy)
        {
            //Speak("sleepy time");
            _state = AnimalState.Sleeping;
            _stateTimeLeft = TimeSpan.FromSeconds(Rng.Next(6, 20));
        }
    }

    private void UpdateStanding(List<Entity>? entities, World world, TimeSpan delta)
    {
        _stateTimeLeft -= delta;
        if (_stateTimeLeft >= TimeSpan.Zero) return; 
        
        //Speak("Exit Standing: "+Priority.ToString());
        
        ActUponPriority(entities, world);
    }
    private void UpdateWandering()
    {
        //stateOver = have I reached goal?
        //in my current direction, 
        //is my distance from origin bigger than Sight from origin
        Vector2 direction;
        if (_moveTarget != null)
        {
            Vector2 toTarget = _moveTarget.Position - Position;
            direction = Vector2.Normalize(toTarget);
        }
        else
        {
            direction = new Vector2(
                (float)Math.Cos(_moveDirection),
                (float)Math.Sin(_moveDirection)
            );
        }
        
        Hunger -= Speed * DeltaTime() * 0.1f;
        Energy -= Speed * DeltaTime() * 0.15f;

        Position += direction * Speed * DeltaTime();

        int sightRadiusSq = Sight * Sight;
        bool reachedGoal = GetSquaredDistBetween(Position, _moveOrigin) > sightRadiusSq;

        if (_moveTarget != null)
        {
            float distanceSq = GetSquaredDistBetween(Position, _moveTarget.Position);
            if (distanceSq > _interactRadius * _interactRadius) 
            {
                return;
            }
        }
        else
        {
            if (!reachedGoal) 
            {
                return; 
            }
        }
        
        //hasreached goal
        _state = AnimalState.Standing;
        _stateTimeLeft = TimeSpan.FromSeconds(Rng.Next(1, 20) / 10.0f);

        if (_moveTarget == null) return;
        
        //we are targetting an object
        if (Priority is AnimalPriority.Hunger && _moveTarget is Food f)
        {
            Hunger += f.SustenanceAmount;
            f.beingEaten = true;

            PlaySoundPitched("chomp-1.mp3");
            //Speak("I ATE THE FOOD FINALLYYYYY");
        }
        if (Priority is AnimalPriority.Mating && _moveTarget is Animal other)
        {
            if (!other.ReadyToMate) return;

            Energy -= 40;
            Hunger -= 30;
            Children.Add(GetBaby(other));

            PlaySoundPitched("dreaming-harp-8d.wav");
            //Speak("I REPRODUCED FINALLLYYY");
        }
        _moveTarget = null;
    }
    private void UpdateSleeping(List<Entity>? entities, World world, TimeSpan delta)
    {
        _textureRotation += Speed * 20 * DeltaTime();

        Energy += 2 * DeltaTime();
        Hunger -= 0.5f * DeltaTime();
        if (Energy >= MaxEnergy) _stateTimeLeft = TimeSpan.Zero;

        _stateTimeLeft -= delta;
        if (_stateTimeLeft >= TimeSpan.Zero) return; 
        
        //Speak("Exit Sleeping: "+Priority.ToString());

        _textureRotation = 0;
        ActUponPriority(entities, world);
    }

    public override void Update(List<Entity>? entities, World world, bool beingHovered)
    {
        base.Update(entities, world, beingHovered);

        TimeSpan newTime = TimeSpan.FromSeconds(DeltaTime());
        Age += newTime;
        
        switch (_state)
        {
            case AnimalState.Wandering: UpdateWandering(); break;
            case AnimalState.Standing: UpdateStanding(entities, world, newTime); break;
            case AnimalState.Sleeping: UpdateSleeping(entities, world, newTime); break;
        }

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