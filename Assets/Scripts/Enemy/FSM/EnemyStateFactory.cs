public class EnemyStateFactory
{
    EnemyStateManager context;

    public EnemyStateFactory(EnemyStateManager currentContext) {
        context = currentContext;
    }

    public EnemyState Patrol() {
        return new EnemyPatrolState(context, this);
    }

    public EnemyState Detect() {
        return new EnemyDetectState(context, this);
    }

    public EnemyState Pursue() {
        return new EnemyPursueState(context, this);
    }

    public EnemyState Melee() {
        return new EnemyMeleeState(context, this);
    }

    public EnemyState Throw() {
        return new EnemyThrowState(context, this);
    }

    public EnemyState Return() {
        return new EnemyReturnState(context, this);
    }

    public EnemyState Confused() {
        return new EnemyConfusedState(context, this);
    }

    public EnemyState Death() {
        return new EnemyDeathState(context, this);
    }

}
