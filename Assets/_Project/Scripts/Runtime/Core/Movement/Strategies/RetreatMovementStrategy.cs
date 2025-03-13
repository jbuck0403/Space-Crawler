using System.Collections;
using UnityEngine;

[CreateAssetMenu(
    fileName = "RetreatMovementStrategy",
    menuName = "SpaceShooter/Strategies/Movement Strategies/Retreat"
)]
public class RetreatMovementStrategy : BaseMovementStrategy
{
    [SerializeField]
    public float retreatDistance = 15f;

    [SerializeField]
    public float RetreatHealthThreshold = 10f;

    [SerializeField]
    public float timeToStopRetreating = 5f;

    public bool canRetreat = true;
    public bool retreating = false;

    // public Coroutine retreatCoroutine;

    public override void OnEnter(Transform self, Transform target)
    {
        base.OnEnter(self, target);
        retreating = true;

        enemyController.StartCoroutine(StopRetreatAfterTime());
    }

    public override void OnUpdate(Transform self, Transform target)
    {
        if (!isInitialized || target == null)
            return;

        // get direction from target to self (opposite of direction to target)
        Vector2 directionFromTarget = MovementUtils.GetTargetDirection(
            target.position,
            self.position
        );

        float distanceFromTarget = MovementUtils.GetDistanceToTarget(
            self.position,
            target.position
        );

        // only retreat if we're closer than retreat distance
        Vector2 targetDirection =
            distanceFromTarget < retreatDistance ? directionFromTarget : Vector2.zero;

        movementHandler.Move(self, targetDirection, Time.deltaTime);
    }

    public override void OnStrategyComplete()
    {
        MovementStrategyType type = MovementStrategyType.Cautious;
        if (enemyController.HasMovementStrategy(type))
        {
            enemyController.ChangeMovementStrategy(type);
        }
        else
        {
            enemyController.ChangeToDefaultStrategy();
        }

        base.OnStrategyComplete();
    }

    public IEnumerator StopRetreatAfterTime()
    {
        yield return new WaitForSeconds(timeToStopRetreating);
        isComplete = true;
        retreating = false;
    }
}
