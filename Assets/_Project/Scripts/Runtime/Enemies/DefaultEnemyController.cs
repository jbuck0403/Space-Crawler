using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(BaseMovementController))]
public class DefaultEnemyController : BaseEnemyController
{
    [SerializeField]
    private float maxFiringDistance = 15f;

    [SerializeField]
    private float minFiringDistance = 0f;

    [SerializeField]
    private bool shooting = false;

    protected RetreatMovementStrategy retreatStrategy;

    protected override void Start()
    {
        base.Start();

        retreatStrategy = GetMovementStrategy<RetreatMovementStrategy>();

        healthSystem.SetLowHealthPercent(retreatStrategy.RetreatHealthThreshold);
        healthSystem.OnLowHealth.AddListener(TriggerRetreat);

        // ChangeMovementStrategy(MovementStrategyType.Charge);
    }

    private void Update()
    {
        if (movementController == null || target == null)
            return;

        float distanceFromTarget = MovementUtils.GetDistanceToTarget(
            transform.position,
            target.position
        );
        if (
            shooting
            && distanceFromTarget <= maxFiringDistance
            && distanceFromTarget >= minFiringDistance
        )
        {
            FireWeapon();
        }
        else
        {
            StopFiringWeapon();
        }
    }

    private void HandleRetreat()
    {
        ChangeMovementStrategy(MovementStrategyType.Retreat);
    }

    public void TriggerRetreat()
    {
        print($"RETREATING: instance? {retreatStrategy.IsInstance()}");
        if (retreatStrategy.canRetreat)
        {
            HandleRetreat();
        }
    }

    private void OnDestroy()
    {
        if (healthSystem != null)
        {
            healthSystem.OnLowHealth.RemoveListener(TriggerRetreat);
        }
    }
    // testing purposes only
    // private void StrategyTest()
    // {
    //     float distanceToTarget = MovementUtils.GetDistanceToTarget(
    //         transform.position,
    //         target.position
    //     );
    //     print("DISTANCE TO TARGET " + distanceToTarget);
    //     if (distanceToTarget <= 7.5f)
    //     {
    //         HandleRetreat();
    //     }
    //     else if (distanceToTarget >= 15f)
    //     {
    //         ChangeMovementStrategy(MovementStrategyType.Default);
    //     }
    // }
    // private IEnumerator StrategyTest()
    // {
    //     print("SWAPPING TO DEFAULT");
    //     ChangeMovementStrategy(MovementStrategyType.Default);

    //     yield return new WaitForSeconds(5f);

    //     print("SWAPPING TO CAUTIOUS");
    //     ChangeMovementStrategy(MovementStrategyType.Cautious);

    //     yield return new WaitForSeconds(5f);

    //     StartCoroutine(StrategyTest());
    // }
}
