using UnityEngine;

public interface IWeaponAbilityDataProvider
{
    public MovementHandler GetMovementHandler();
    public Vector2 GetAbilityTarget();
    public Transform GetWeaponOwnerTransform();
    public void SetApplyingExternalMovement(bool value, float disableTime = default);
    public Transform GetFirePoint();
}
