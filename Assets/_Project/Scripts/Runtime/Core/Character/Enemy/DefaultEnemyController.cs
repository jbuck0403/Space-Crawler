using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(BaseMovementController))]
[RequireComponent(typeof(WeaponHandler))]
public class DefaultEnemyController : BaseEnemyController
{
    [SerializeField]
    private float maxFiringDistance = 15f;

    [SerializeField]
    private float minFiringDistance = 0f;

    protected RetreatMovementStrategy retreatStrategy;
    private bool retreatInitialized;

    // protected override void Start()
    // {
    //     base.Start();

    //     InitializeRetreat();
    // }

    public override bool Initialize(Transform defaultTarget)
    {
        bool baseInitialized = base.Initialize(defaultTarget);
        bool retreatInitialized = InitializeRetreat();

        return baseInitialized && retreatInitialized;
    }

    private bool InitializeRetreat()
    {
        BaseMovementStrategy baseMovementStrategy = GetMovementStrategyByAssignedType(
            MovementStrategyType.Retreat
        );
        if (baseMovementStrategy is RetreatMovementStrategy retreatMovementStrategy)
        {
            retreatStrategy = retreatMovementStrategy;
        }

        if (baseMovementStrategy != null)
        {
            if (retreatStrategy != null)
            {
                healthSystem.SetLowHealthPercent(retreatStrategy.RetreatHealthThreshold);
            }
            retreatInitialized = true;
            healthSystem.OnLowHealth.AddListener(gameObject, TriggerRetreat);

            return true;
        }

        return false;
    }

    private void Update()
    {
        if (!initialized)
            return;

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
        if (
            (retreatStrategy != null && retreatStrategy.canRetreat)
            || HasMovementStrategy(MovementStrategyType.Retreat)
        )
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
