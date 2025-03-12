using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(BaseMovementController))]
public class DefaultEnemyController : BaseEnemyController
{
    [SerializeField]
    private float RetreatHealthThreshold = 10f;

    [SerializeField]
    private float timeToStopRetreating = 5f;

    [SerializeField]
    private float maxFiringDistance = 15f;
    private bool canRetreat = true;
    private bool retreating = false;

    private Coroutine retreatCoroutine;

    [SerializeField]
    private bool shooting = false;

    protected override void Start()
    {
        base.Start();

        healthSystem.OnLowHealth.AddListener(TriggerRetreat);

        ChangeMovementStrategy(MovementStrategyType.Circle);
        // StartCoroutine(StrategyTest());
    }

    private void Update()
    {
        if (movementController == null || target == null)
            return;

        float distanceFromTarget = MovementUtils.GetDistanceToTarget(
            transform.position,
            target.position
        );
        if (shooting && distanceFromTarget <= maxFiringDistance && !retreating)
        {
            FireWeapon();
        }
        else
        {
            StopFiringWeapon();
        }
    }

    public void HandleRetreat()
    {
        ChangeMovementStrategy(MovementStrategyType.Retreat);
        retreating = true;
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

    public IEnumerator StopRetreatAfterTime()
    {
        if (retreatCoroutine == null)
        {
            yield return new WaitForSeconds(timeToStopRetreating);
            isComplete = true;
            retreating = false;
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

    private void OnDestroy()
    {
        if (healthSystem != null)
        {
            healthSystem.OnLowHealth.RemoveListener(TriggerRetreat);
        }
    }
}
