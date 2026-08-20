namespace EvolutionSim;

public class State
{
    public State? next;
    public bool done = false;

    public State(){}

    public virtual void Enter(){}
    public virtual void Exit(){}
    public virtual void Update(){}
    public virtual void Draw(){}

    public void TransitionTo(State nextState)
    {
        next = nextState;
        done = true;
    }

    public void Quit()
    {
        done = true;
    }
}