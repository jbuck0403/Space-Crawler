using System;
using UnityEngine;

[CreateAssetMenu(fileName = "OnHitFXEvent", menuName = "Game/Events/OnHitFXEvent")]
public class OnHitFXEvent : BaseGameEvent<OnHitFXData> { }

[Serializable]
public class OnHitFXData : FXData
{
    public OnHitFXData(GameObject onHitVFXPrefab, Transform sourceTransform)
        : base(onHitVFXPrefab, sourceTransform) { }
}
