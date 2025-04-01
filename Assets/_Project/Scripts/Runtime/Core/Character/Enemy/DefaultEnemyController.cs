using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(BaseMovementController))]
[RequireComponent(typeof(BaseWeapon))]
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

        if (movementController == null || movementController.CurrentTarget == null)
            return;

        float distanceFromTarget = MovementUtils.GetDistanceToTarget(
            transform.position,
            movementController.CurrentTarget.position
        );

        if (
            distanceFromTarget <= maxFiringDistance
            && distanceFromTarget >= minFiringDistance
            && !shootingDisabledExternally
        )
        {
            EnableShooting(true);
        }

        HandleShooting(movementController.CurrentTarget);
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
}
