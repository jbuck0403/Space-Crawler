using UnityEngine;

public class WeaponFXManager : MonoBehaviour
{
    private WeaponHandler weaponHandler;

    private void Awake()
    {
        if (weaponHandler == null)
        {
            weaponHandler = gameObject.GetComponent<WeaponHandler>();
        }
    }

    private void Start()
    {
        AddListeners();
    }

    private void AddListeners()
    {
        weaponHandler.OnMuzzleFlareFX.AddListener(gameObject, HandleMuzzleFlareFX);
        weaponHandler.OnHitFX.AddListener(gameObject, HandleOnHitFX);
    }

    private void RemoveListeners()
    {
        weaponHandler.OnMuzzleFlareFX.RemoveListener(gameObject, HandleMuzzleFlareFX);
        weaponHandler.OnHitFX.RemoveListener(gameObject, HandleOnHitFX);
    }

    private void HandleMuzzleFlareFX(MuzzleFlareFXData fxData)
    {
        WeaponVFXHandler.HandleMuzzleFlare(
            fxData.vFXPrefab,
            fxData.firePoint,
            fxData.sourceTransform
        );
    }

    private void HandleOnHitFX(OnHitFXData fxData)
    {
        WeaponVFXHandler.HandleOnHitEffect(fxData.vFXPrefab, fxData.sourceTransform);
    }

    private void OnDisable()
    {
        RemoveListeners();
    }
}

public class FXData // common across "most" FX
{
    public GameObject vFXPrefab;
    public Transform sourceTransform;

    public FXData(GameObject vFXPrefab, Transform sourceTransform)
    {
        this.vFXPrefab = vFXPrefab;
        this.sourceTransform = sourceTransform;
    }
}
