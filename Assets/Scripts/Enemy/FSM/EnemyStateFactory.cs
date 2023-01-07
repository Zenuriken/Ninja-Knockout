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

    public EnemyState Attack() {
        return new EnemyAttackState(context, this);
    }

}
