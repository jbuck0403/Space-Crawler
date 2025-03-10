using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(BaseMovementController))]
public class DefaultEnemyController : BaseEnemyController
{
    [SerializeField]
    private float RetreatHealthThreshold = 10f;

    [SerializeField]
    private float timeToStopRetreating = 5f;
    private bool canRetreat = true;

    private Coroutine retreatCoroutine;
    private HealthSystem healthSystem;

    protected override void Start()
    {
        base.Start();

        healthSystem = GetComponent<HealthSystem>();
        healthSystem.OnLowHealth.AddListener(TriggerRetreat);
    }

    private void Update()
    {
        if (movementController == null || target == null)
            return;

        // StrategyTest();
    }

    protected override void InitializeStrategies()
    {
        availableMovementStrategies = new Dictionary<string, IMovementStrategy>
        {
            { MovementStrategies.Default, new DefaultMovementStrategy(movementConfig) },
            { MovementStrategies.Retreat, new RetreatMovementStrategy(movementConfig) }
        };
    }

    protected override void SetDefaultStrategy()
    {
        ChangeMovementStrategy(MovementStrategies.Default);
    }

    public void HandleRetreat()
    {
        ChangeMovementStrategy(MovementStrategies.Retreat);
        canRetreat = false;

        retreatCoroutine = StartCoroutine(StopRetreatAfterTime());
    }

    private void TriggerRetreat()
    {
        if (canRetreat)
        {
            HandleRetreat();
        }
    }

    private IEnumerator StopRetreatAfterTime()
    {
        if (retreatCoroutine == null)
        {
            yield return new WaitForSeconds(timeToStopRetreating);

            SetDefaultStrategy();
        }
    }

    private void StrategyTest()
    {
        float distanceToTarget = MovementUtils.GetDistanceToTarget(
            transform.position,
            target.position
        );
        if (distanceToTarget <= 7.5f)
        {
            ChangeMovementStrategy(MovementStrategies.Retreat);
        }
        else if (distanceToTarget >= 15f)
        {
            ChangeMovementStrategy(MovementStrategies.Default);
        }
    }

    private void OnDestroy()
    {
        if (healthSystem != null)
        {
            healthSystem.OnLowHealth.RemoveListener(TriggerRetreat);
        }
    }
}
