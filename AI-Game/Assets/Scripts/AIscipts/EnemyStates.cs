using UnityEngine;

public abstract class EnemyState
{
    protected EnemyController enemy;

    protected EnemyState(EnemyController e)
    {
        enemy = e;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}

public class EnemyPatrolState : EnemyState
{
    public EnemyPatrolState(EnemyController e) : base(e) { }

    public override void Enter()
    {
        enemy.SetDirection(Vector3.zero);
    }

    public override void Update()
    {
        enemy.DoPatrol();
    }

    public override void Exit() { }
}

public class EnemyChaseState : EnemyState
{
    public EnemyChaseState(EnemyController e) : base(e) { }

    public override void Enter() { }

    public override void Update()
    {
        enemy.DoChase();
    }

    public override void Exit() { }
}

public class EnemyAttackState : EnemyState
{
    public EnemyAttackState(EnemyController e) : base(e) { }

    public override void Enter()
    {
        enemy.SetDirection(Vector3.zero);
    }

    public override void Update()
    {
        enemy.DoAttack();
    }

    public override void Exit() { }
}

public class EnemyFleeState : EnemyState
{
    public EnemyFleeState(EnemyController e) : base(e) { }

    public override void Enter() { }

    public override void Update()
    {
        enemy.DoFlee();
    }

    public override void Exit() { }
}
