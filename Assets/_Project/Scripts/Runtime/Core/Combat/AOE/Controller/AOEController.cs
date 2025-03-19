using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Controller for managing AOE prefabs - handles positioning, sizing, parenting, and target following
/// </summary>
public class AOEController : BaseEnemyController
{
    // [SerializeField]
    // private bool followOnStart = false;

    [SerializeField]
    private Transform defaultFollowTarget;

    private BaseAOEZone currentAOE;

    private bool isFollowing;

    private float temp = 0f;

    protected override void Awake()
    {
        base.Awake();
        currentAOE = GetComponent<BaseAOEZone>();

        isFollowing = currentAOE.aoeProfile.followTarget;
    }

    protected override void Start()
    {
        base.Start();

        if (isFollowing && defaultFollowTarget != null)
        {
            UpdateTarget(defaultFollowTarget, true);
        }
    }

    public void SetFollowTarget(Transform target)
    {
        if (target != null)
        {
            UpdateTarget(target, true);
            ChangeToDefaultStrategy();
        }
    }

    public void ChangeFollowTarget(Transform newTarget)
    {
        if (newTarget != null)
        {
            UpdateTarget(newTarget);
        }
    }

    public void StopFollowing()
    {
        if (movementController != null)
        {
            movementController.ChangeDefaultTarget(null);
        }
        UpdateTarget(null);
    }

    public BaseAOEZone PositionAOE(Vector3 position)
    {
        if (currentAOE != null)
        {
            transform.position = position;
        }

        return currentAOE;
    }

    public void ResizeAOE(float radius)
    {
        if (currentAOE != null)
        {
            CircleCollider2D circleCollider = currentAOE.GetComponent<CircleCollider2D>();
            if (circleCollider != null)
            {
                circleCollider.radius = radius;
            }

            currentAOE.SetRadius(radius);

            AOEVisualizer visualizer = currentAOE.GetComponent<AOEVisualizer>();
            if (visualizer != null)
            {
                visualizer.UpdateSize(radius);
            }
        }
    }

    public void ReparentAOE(Transform parent, Vector3 localPosition = default)
    {
        if (currentAOE != null && parent != null)
        {
            transform.SetParent(parent);
            transform.localPosition = localPosition;
        }
    }

    public BaseAOEZone GetCurrentAOE()
    {
        return currentAOE;
    }

    public void EndCurrentAOE()
    {
        if (currentAOE != null)
        {
            currentAOE.DestroyZone();
            currentAOE = null;
        }
    }
}
