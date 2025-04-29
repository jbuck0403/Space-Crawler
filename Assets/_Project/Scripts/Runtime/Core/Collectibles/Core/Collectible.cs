using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField]
    protected CollectibleType type;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
            Destroy(gameObject);
        }
    }

    protected void Collect()
    {
        switch (type)
        {
            case CollectibleType.Weapon:
                CollectibleManager.Instance.HandleWeaponCollection();
                break;
            case CollectibleType.Ammo:
                CollectibleManager.Instance.HandleAmmoCollection();
                break;
            case CollectibleType.TalentPoint:
                CollectibleManager.Instance.HandleTalentPointCollection();
                break;
        }
    }
}
