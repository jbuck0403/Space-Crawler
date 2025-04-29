using UnityEngine;

public abstract class Collectible : MonoBehaviour
{
    [SerializeField]
    protected CollectibleType type;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    protected abstract void Collect();
}
