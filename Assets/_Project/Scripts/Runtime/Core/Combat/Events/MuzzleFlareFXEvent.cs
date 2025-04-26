using System;
using UnityEngine;

[CreateAssetMenu(fileName = "OnMuzzleFlareFXEvent", menuName = "Game/Events/MuzzleFlareFXEvent")]
public class MuzzleFlareFXEvent : BaseGameEvent<MuzzleFlareFXData> { }

[Serializable]
public class MuzzleFlareFXData : FXData
{
    public Transform firePoint;

    public MuzzleFlareFXData(
        GameObject muzzleFlareVFXPrefab,
        Transform sourceTransform,
        Transform firePoint
    )
        : base(muzzleFlareVFXPrefab, sourceTransform)
    {
        this.firePoint = firePoint;
    }
}
