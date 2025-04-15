using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Bullet Time Talent - Slows down time when a critical hit occurs
/// This is an example of a tier 6 (mutually exclusive) talent
/// </summary>
[CreateAssetMenu(menuName = "SpaceShooter/Talents/Bullet Time")]
public class BulletTimeTalent : BaseTalent
{
    [Header("Bullet Time Settings")]
    [SerializeField]
    private float triggerChance = 0.1f; // 10% chance by default

    [SerializeField]
    private float slowdownFactor = 0.3f; // Time slows to 30% of normal speed

    [SerializeField]
    private float slowdownDuration = 2f; // Duration in real-time seconds

    [SerializeField]
    private float cooldown = 30f; // Cooldown in seconds

    private Coroutine activeSlowdownCoroutine;
    private float lastTriggerTime = -999f;
    private bool isOnCooldown => Time.time < lastTriggerTime + cooldown;

    public void OnValidate()
    {
        // This talent can only have 1 point (Tier 6 talent)
        maxDesignatedPoints = 1;
    }

    protected override List<TalentModifierData> GetModifierData(GameObject gameObject)
    {
        return new List<TalentModifierData>
        {
            new TalentModifierData(
                ModifierType.ON_CRITICAL_HIT,
                OnCriticalHitDelegate(),
                GetModifiable(gameObject)
            )
        };
    }

    private Delegate OnCriticalHitDelegate()
    {
        ModifierHelper.GameObjectInModifier fn = (GameObject source) =>
        {
            if (isOnCooldown)
                return;

            // Check random chance to trigger bullet time
            if (RandomUtils.Chance(triggerChance))
            {
                if (coroutineRunner != null)
                {
                    // Cancel existing bullet time if running
                    if (activeSlowdownCoroutine != null)
                    {
                        coroutineRunner.StopCoroutine(activeSlowdownCoroutine);
                    }

                    // Start new bullet time effect
                    activeSlowdownCoroutine = coroutineRunner.StartCoroutine(SlowDownTime());
                    lastTriggerTime = Time.time;
                }
            }
        };

        return fn;
    }

    private IEnumerator SlowDownTime()
    {
        // Store original timescale
        float originalTimeScale = Time.timeScale;

        // Apply slow-motion effect
        Time.timeScale = slowdownFactor;

        // Wait for the duration (using unscaled time so we're not affected by our own slowdown)
        yield return new WaitForSecondsRealtime(slowdownDuration);

        // Smoothly return to normal speed
        float lerpDuration = 0.5f;
        float lerpStartTime = Time.unscaledTime;

        while (Time.unscaledTime < lerpStartTime + lerpDuration)
        {
            float t = (Time.unscaledTime - lerpStartTime) / lerpDuration;
            Time.timeScale = Mathf.Lerp(slowdownFactor, originalTimeScale, t);
            yield return null;
        }

        // Ensure we're back to normal speed
        Time.timeScale = originalTimeScale;
        activeSlowdownCoroutine = null;
    }

    protected override IModifiable GetModifiable(GameObject gameObject)
    {
        // The DamageHandler handles critical hits
        var damageHandler = gameObject.GetComponent<DamageHandler>();
        return damageHandler as IModifiable;
    }
}
