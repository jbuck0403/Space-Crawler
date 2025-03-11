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

    protected override void SetDefaultStrategy()
    {
        ChangeMovementStrategy(MovementStrategyType.Default);
    }

    public void HandleRetreat()
    {
        ChangeMovementStrategy(MovementStrategyType.Retreat);
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
        print("DISTANCE TO TARGET " + distanceToTarget);
        if (distanceToTarget <= 7.5f)
        {
            print("RETREAT");
            HandleRetreat();
        }
        else if (distanceToTarget >= 15f)
        {
            ChangeMovementStrategy(MovementStrategyType.Default);
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
