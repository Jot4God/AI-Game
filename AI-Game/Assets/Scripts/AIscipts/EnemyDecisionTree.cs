public class EnemyDecisionTree
{
    public enum Decision
    {
        Patrol,
        Chase,
        Attack,
        Flee
    }

    public Decision Evaluate(
        float distanceToPlayer,
        float healthRatio,
        float chaseDistance,
        float attackDistance
    )
    {
        // 1) Se a vida está baixa, foge
        if (healthRatio < 0.25f)
            return Decision.Flee;

        // 2) Se está ao alcance de ataque
        if (distanceToPlayer <= attackDistance)
            return Decision.Attack;

        // 3) Se está numa zona de perseguição
        if (distanceToPlayer <= chaseDistance)
            return Decision.Chase;

        // 4) Caso contrário, patrulha
        return Decision.Patrol;
    }
}
