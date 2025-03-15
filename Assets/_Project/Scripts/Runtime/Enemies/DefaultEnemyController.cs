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

    protected RetreatMovementStrategy retreatStrategy;
    private bool retreatInitialized;

    protected override void Start()
    {
        base.Start();

        InitializeRetreat();

        // ChangeMovementStrategy(MovementStrategyType.Charge);
    }

    private void InitializeRetreat()
    {
        retreatStrategy = GetMovementStrategy<RetreatMovementStrategy>();
        if (retreatStrategy != null)
        {
            retreatInitialized = true;
            healthSystem.SetLowHealthPercent(retreatStrategy.RetreatHealthThreshold);
            healthSystem.OnLowHealth.AddListener(gameObject, TriggerRetreat);
        }
    }

    private void Update()
    {
        if (!retreatInitialized)
        {
            InitializeRetreat();
        }

        if (movementController == null || target == null)
            return;

        float distanceFromTarget = MovementUtils.GetDistanceToTarget(
            transform.position,
            target.position
        );

        if (
            distanceFromTarget <= maxFiringDistance
            && distanceFromTarget >= minFiringDistance
            && !shootingDisabledExternally
        )
        {
            EnableShooting(true);
        }

        HandleShooting();
    }

    private void HandleRetreat()
    {
        ChangeMovementStrategy(MovementStrategyType.Retreat);
    }

    public void TriggerRetreat()
    {
        if (retreatStrategy.canRetreat)
        {
            HandleRetreat();
        }
    }

    private void OnDestroy()
    {
        if (healthSystem != null)
        {
            healthSystem.OnLowHealth.RemoveListener(gameObject, TriggerRetreat);
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
