using UnityEngine;

[CreateAssetMenu(fileName = "AOEProfile", menuName = "Game/AOE Profile")]
public class AOEProfile : ScriptableObject
{
    public AOEData aoeData;
    public DamageProfile damageProfile;

    public DamageTypeEvent onDamageOverTimeTick;

    public bool followTarget = false;
}
