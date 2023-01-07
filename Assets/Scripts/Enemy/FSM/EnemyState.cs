public abstract class EnemyState
{
    protected EnemyStateManager ctx;
    protected EnemyStateFactory factory;

    public EnemyState(EnemyStateManager currContext, EnemyStateFactory stateFactory) {
        ctx = currContext;
        factory = stateFactory;
    }

    public abstract void EnterState();

    public abstract void UpdateState();

    public abstract void ExitState();

    public abstract void CheckSwitchStates();

    public abstract void InitializeSubState();

    void UpdateStates() {}

    protected void SwitchState(EnemyState newState) {
        // current state exits state
        ExitState();

        // new state enters state
        newState.EnterState();

        // switch current state of context
        ctx.CurrentState = newState;
    }

    protected void SetSuperState(){}

    protected void SetSubState(){}


}
