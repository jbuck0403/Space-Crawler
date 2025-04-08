using UnityEngine;

[CreateAssetMenu(fileName = "New Status Effect Data", menuName = "Combat/Status Effect Data")]
public class StatusEffectData : ScriptableObject
{
    [Header("Basic Properties")]
    [SerializeField]
    private StatusEffect effectType;

    [Header("Effect Settings")]
    [SerializeField]
    private float duration = 5f;

    [SerializeField]
    private float tickRate = 1f; // how often the effect triggers (for periodic effects)

    [SerializeField]
    private bool isStackable;

    [SerializeField]
    private int maxStacks = 1;

    [Header("UI Properties")]
    [SerializeField]
    private string description;

    [SerializeField]
    private Sprite icon;

    private Transform source;

    // Properties

    public StatusEffect EffectType => effectType;
    public string Description => description;
    public Sprite Icon => icon;
    public float Duration => duration;
    public float TickRate => tickRate;
    public bool IsStackable => isStackable;
    public int MaxStacks => maxStacks;
    public Transform Source => source;

    public void SetSource(Transform source)
    {
        this.source = source;
    }

    public virtual void ApplyStatusEffect(GameObject target)
    {
        Debug.Log($"#StatusEffect# Attempting to apply effect to {target.name}");
        var receiver = target.GetComponent<IStatusEffectReceiver>();

        if (receiver == null)
        {
            Debug.LogError(
                $"#StatusEffect# Target {target.name} does not have an IStatusEffectReceiver component!"
            );
            return;
        }

        Debug.Log($"#StatusEffect# Found receiver on {target.name}");
        receiver.ApplyStatusEffect(this);
    }
}
