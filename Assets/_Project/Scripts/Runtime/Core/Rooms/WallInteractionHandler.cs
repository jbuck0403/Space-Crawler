using UnityEngine;

public enum WallInteractionType
{
    Destroy,
    Reflect,
    Multiply
    // Add more interaction types as needed
}

public class WallInteractionHandler : MonoBehaviour
{
    [SerializeField]
    private WallInteractionType interactionType = WallInteractionType.Destroy;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Projectile>(out var projectile))
        {
            HandleProjectileInteraction(projectile);
        }
    }

    // private void OnCollisionEnter2D(Collision2D other)
    // {
    //     print("COLLISION");
    //     if (other.gameObject.TryGetComponent<Projectile>(out var projectile))
    //     {
    //         HandleProjectileInteraction(projectile);
    //     }
    // }

    private void HandleProjectileInteraction(Projectile projectile)
    {
        switch (interactionType)
        {
            case WallInteractionType.Destroy:
                DestroyProjectile(projectile);
                break;

            case WallInteractionType.Reflect:
                ReflectProjectile(projectile);
                break;

            case WallInteractionType.Multiply:
                MultiplyProjectile(projectile);
                break;

            default:
                DestroyProjectile(projectile);
                break;
        }
    }

    private void DestroyProjectile(Projectile projectile)
    {
        projectile.DestroyProjectile();
    }

    private void ReflectProjectile(Projectile projectile)
    {
        // Get the projectile's rigidbody
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 incomingVelocity = rb.velocity;

            Vector2 normal = transform.up;

            Vector2 reflectedVelocity = Vector2.Reflect(incomingVelocity, normal);

            rb.velocity = reflectedVelocity;

            // Rotate the projectile to match its new direction
            float angle = Mathf.Atan2(reflectedVelocity.y, reflectedVelocity.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void MultiplyProjectile(Projectile projectile)
    {
        //TBI
    }
}
