using UnityEngine;

[CreateAssetMenu(fileName = "AOEData", menuName = "Game/AOE Data")]
public class AOEData : ScriptableObject
{
    [Header("Effect Settings")]
    public float radius = 5f;
    public bool triggerOnEnter = true;
    public bool triggerOnExit = false;
    public bool triggerOverTime = false;
    public float tickRate = 1f;
    public LayerMask targetLayers;
    public float duration = 2f;
    public bool destroyOnEnd = true;
}
