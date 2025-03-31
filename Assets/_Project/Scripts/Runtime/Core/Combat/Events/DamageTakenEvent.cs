using UnityEngine;

[CreateAssetMenu(fileName = "DamageTakenEvent", menuName = "Game/Events/Damage Taken Event")]
public class DamageTakenEvent : BaseGameEvent<DamageTakenEventData> { }

public class DamageTakenEventData
{
    private float damageAmount;
    private DamageType damageType;
    private bool isCrit;

    public float DamageAmount => damageAmount;
    public DamageType DamageType => damageType;
    public bool IsCrit => isCrit;

    public DamageTakenEventData(float damageAmount, DamageType damageType, bool isCrit)
    {
        this.damageAmount = damageAmount;
        this.damageType = damageType;
        this.isCrit = isCrit;
    }
}
