using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        StrategyTest();
    }

    protected override void InitializeStrategies()
    {
        availableStrategies = new Dictionary<string, IMovementStrategy>
        {
            { Strategies.Default, new DefaultMovementStrategy(movementConfig) },
            { Strategies.Retreat, new RetreatMovementStrategy(movementConfig) }
        };
    }

    protected override void SetDefaultStrategy()
    {
        ChangeStrategy(Strategies.Default);
    }

    public void HandleRetreat()
    {
        ChangeStrategy(Strategies.Retreat);
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
            ChangeStrategy(Strategies.Retreat);
        }
        else if (distanceToTarget >= 15f)
        {
            ChangeStrategy(Strategies.Default);
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
