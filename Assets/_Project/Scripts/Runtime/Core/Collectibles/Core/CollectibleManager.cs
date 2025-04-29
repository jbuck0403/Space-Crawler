using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance { get; private set; }

    [Header("VFX Prefabs")]
    [SerializeField]
    private GameObject weaponCollectVFX;

    [SerializeField]
    private GameObject ammoCollectVFX;

    [SerializeField]
    private GameObject talentPointCollectVFX;

    [Header("Collectible Scriptable Objects")]
    [SerializeField]
    private WeaponDropSO weaponDropSO;

    [SerializeField]
    private AmmoDropSO ammoDropSO;

    [SerializeField]
    private TalentPointDropSO talentPointDropSO;

    [Header("References")]
    [SerializeField]
    private WeaponHandler weaponHandler;

    // public TalentTreeHandler talentTreeHandler;

    public WeaponHandler WeaponHandler => weaponHandler;

    // public TalentTreeHandler TalentTreeHandler => talentTreeHandler;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Static methods for other systems to use
    public static void SpawnRandomWeapon(Transform location)
    {
        Instance.SpawnWeaponCollectible(location);
    }

    public static void SpawnRandomAmmo(Transform location)
    {
        Instance.SpawnAmmoCollectible(location);
    }

    public static void SpawnTalentPoint(Transform location)
    {
        Instance.SpawnTalentPointCollectible(location);
    }

    // Internal spawn methods
    private void SpawnWeaponCollectible(Transform location)
    {
        weaponDropSO.SpawnCollectible(location);
    }

    private void SpawnAmmoCollectible(Transform location)
    {
        ammoDropSO.SpawnCollectible(location);
    }

    private void SpawnTalentPointCollectible(Transform location)
    {
        talentPointDropSO.SpawnCollectible(location);
    }

    // Collection handling methods
    public void HandleWeaponCollection()
    {
        weaponDropSO.HandleCollection();
    }

    public void HandleAmmoCollection()
    {
        ammoDropSO.HandleCollection();
    }

    public void HandleTalentPointCollection()
    {
        talentPointDropSO.HandleCollection();
    }
}
