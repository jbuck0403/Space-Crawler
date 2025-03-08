# Combat System Documentation

## Overview

The Combat System is a modular framework for handling damage, health, defense, and status effects in a Unity game. It provides a flexible architecture that allows for easy customization and extension. The system is built for 2D games but can be adapted for 3D projects (see [Adapting for 3D](#adapting-for-3d)).

## Table of Contents

1. [Core Components](#core-components)
2. [Setting Up a Damageable Entity](#setting-up-a-damageable-entity)
3. [Dealing Damage](#dealing-damage)
4. [Defense System](#defense-system)
5. [Area of Effect (AOE) System](#area-of-effect-aoe-system)
6. [Status Effects](#status-effects)
7. [Event System](#event-system)
8. [Extensibility](#extensibility)
9. [Best Practices](#best-practices)
10. [Adapting for 3D](#adapting-for-3d)

## Core Components

The Combat System consists of several key components that work together:

### Health System
- Manages entity health points
- Provides events for health changes and death
- Handles both healing and damage application

### Damage System
- `DamageData`: Structure containing damage properties (amount, source, crit chance, etc.)
- `DamageHandler`: Processes incoming damage and applies defense calculations
- `DamageType`: Enum defining damage categories (Physical, True, Fire, Ice, etc.)
- Receivers: Components that receive and process damage
  - `BaseDamageReceiver`: Base implementation for receiving direct damage
  - `AOEReceiver`: Extended receiver that handles both direct and area-of-effect damage

### Defense System
- Handles damage mitigation through resistances
- Supports physical and elemental resistance types
- Uses `DefenseData` for storing resistance values
- Implements `IDefenseHandler` interface for defense calculations

### Status Effects
- Framework for applying, stacking, and removing temporary effects
- Uses factory pattern for creating different effect types
- Provides event-based notification system for effect changes

### Area of Effect (AOE)
- Base implementation for creating damage zones
- Supports different AOE behaviors (on enter, over time, on exit)
- Uses layer-based targeting for selective damage application
- Includes timing control for effect application

## Setting Up a Damageable Entity

To create an entity that can receive damage:

```csharp
// 1. Add required components to your GameObject
// - HealthSystem (for health tracking)
// - DamageHandler (for damage processing)
// - Choose ONE receiver type:
//   * BaseDamageReceiver for direct damage only
//   * AOEReceiver for both direct and AOE damage

// 2. Initialize the defense handler in Start() or Awake()
void Start()
{
    // Create defense data with specific resistances
    DefenseData defenseData = new DefenseData();
    defenseData.SetPhysicalResistance(0.2f);  // 20% physical damage reduction
    defenseData.SetElementalResistance(DamageType.Fire, 0.5f);  // 50% fire resistance
    
    // Create and initialize defense handler
    IDefenseHandler defenseHandler = new BaseDefenseHandler(defenseData);
    
    // Initialize the damage handler with the defense handler
    GetComponent<DamageHandler>().Initialize(defenseHandler);
}
```

## Dealing Damage

To deal damage to an entity:

```csharp
// 1. Create damage data
DamageData damageData = new DamageData(
    amount: 25f,                // Base damage amount
    source: transform,          // Source of the damage
    critMultiplier: 1.5f,       // Critical hit multiplier (1.5x = 50% extra damage)
    critChance: 15f,            // 15% chance to crit
    damageType: DamageType.Fire // Damage type (affects resistance calculations)
);

// 2. Find target and apply damage
BaseDamageReceiver receiver = hitObject.GetComponent<BaseDamageReceiver>();
if (receiver != null)
{
    receiver.ReceiveDamage(damageData);
    // The damage will be processed through:
    // 1. DamageHandler.HandleDamage()
    // 2. DefenseHandler.HandleDefense() for resistance calculations
    // 3. HealthSystem.ModifyHealth() to apply final damage
}
```

## Defense System

The defense system handles damage mitigation through resistances:

### Creating a Defense Handler

```csharp
// Create defense data with specific resistances
DefenseData defenseData = new DefenseData();
defenseData.SetPhysicalResistance(0.3f);  // 30% physical resistance
defenseData.SetElementalResistance(DamageType.Fire, 0.5f);  // 50% fire resistance

// Create defense handler with the data
IDefenseHandler defenseHandler = new BaseDefenseHandler(defenseData);
```

### Modifying Defenses

```csharp
// Get the defense handler from the damage handler
IDefenseHandler defenseHandler = GetComponent<DamageHandler>().GetDefenseHandler();

// Modify resistances (additive)
defenseHandler.ModifyPhysicalResistance(0.1f);  // Add 10% physical resistance
defenseHandler.ModifyElementalResistance(DamageType.Fire, 0.25f);  // Add 25% fire resistance

// Set absolute resistance values
defenseHandler.SetPhysicalResistance(0.5f);  // Set to 50% physical resistance
```

## Area of Effect (AOE) System

The AOE system creates area-based damage effects:

### Creating an AOE Zone

```csharp
// 1. Create damage data for the AOE
DamageData damageData = new DamageData(
    amount: 15f,
    source: transform,
    critMultiplier: 1.2f,
    critChance: 5f,
    damageType: DamageType.Poison
);

// 2. Create the AOE zone
DamageAOEZone aoeZone = BaseAOEZone.Create<DamageAOEZone>(
    owner: transform,           // Who created this AOE
    position: transform.position, // Where to place the AOE
    damageData: damageData,     // Damage to apply
    customRadius: 5f,           // Size of the effect
    customDuration: 10f         // How long it lasts
);
```

### Receiving AOE Damage

Entities need an `AOEReceiver` component to receive AOE damage:

```csharp
// Add AOEReceiver to your entity
// This component inherits from BaseDamageReceiver, so it handles both direct and AOE damage
AOEReceiver aoeReceiver = gameObject.AddComponent<AOEReceiver>();

// The receiver will automatically:
// - Process direct damage through the ReceiveDamage method
// - Detect and process AOE damage through trigger collisions with AOE zones
// - Control effect timing through the CanTriggerEffect method
```

### AOE Timing Control

The `AOEReceiver` includes timing control for effects:

```csharp
// This method is called internally to determine if an effect should trigger
// You can override this in derived classes for custom timing logic
public bool CanTriggerEffect(BaseAOEZone zone)
{
    // Check if enough time has passed since the last effect application
    // based on the zone's tick rate
    // This prevents effects from being applied too frequently
}
```

## Status Effects

The status effect system applies temporary effects to entities:

### Applying Status Effects

```csharp
// 1. Create status effect data
StatusEffectData effectData = new StatusEffectData(
    effectName: "Burning",      // Unique identifier
    effectType: StatusEffect.Burning, // Effect type (Burning, Stun, Invulnerable)
    duration: 5f,               // How long it lasts
    tickRate: 1f,               // How often it applies (once per second)
    stackable: true,            // Can multiple instances stack?
    maxStacks: 3                // Maximum number of stacks
);

// 2. Apply the effect to a target
IStatusEffectReceiver receiver = target.GetComponent<IStatusEffectReceiver>();
if (receiver != null)
{
    receiver.ApplyStatusEffect(effectData);
}
```

### Managing Status Effects

```csharp
// Get the status effect handler
StatusEffectHandler effectHandler = GetComponent<StatusEffectHandler>();

// Check if an effect is active
bool isBurning = effectHandler.HasStatusEffect("Burning");

// Remove a specific effect
effectHandler.RemoveStatusEffect("Burning");

// Clear all effects
effectHandler.ClearAllStatusEffects();
```

## Event System

The combat system uses events for communication between components:

### Subscribing to Events

```csharp
// Get the health system
HealthSystem healthSystem = GetComponent<HealthSystem>();

// Subscribe to health events
healthSystem.OnHealthChanged.AddListener(OnHealthChanged);
healthSystem.OnDeath.AddListener(OnDeath);

// Event handlers
private void OnHealthChanged(float newHealth)
{
    // Update UI or other game systems
}

private void OnDeath()
{
    // Handle death (play animation, drop loot, etc.)
}

// IMPORTANT: Remember to unsubscribe when your object is destroyed
private void OnDestroy()
{
    if (healthSystem != null)
    {
        healthSystem.OnHealthChanged.RemoveListener(OnHealthChanged);
        healthSystem.OnDeath.RemoveListener(OnDeath);
    }
}
```

### Important Note About VoidEvents

When using VoidEvents (parameterless events), always remove a listener before adding it again to the same event. This prevents potential issues with duplicate event handling:

```csharp
// Good practice - remove before adding
healthSystem.OnLowHealth.RemoveListener(HandleLowHealth);
healthSystem.OnLowHealth.AddListener(HandleLowHealth);

// Always unsubscribe in OnDestroy
private void OnDestroy()
{
    if (healthSystem != null)
    {
        healthSystem.OnLowHealth.RemoveListener(HandleLowHealth);
    }
}
```

This precaution is only necessary for VoidEvents (events without parameters). Other event types like FloatEvent or BoolEvent don't have this requirement since they pass parameters of their specific type directly to the listener.

## Extensibility

The Combat System is designed to be highly extensible. The current implementation provides the core framework and minimal implementations to illustrate how to extend the system:

### Extending Damage Types

The `DamageType` enum can be extended with additional damage types:

```csharp
// Current implementation
public enum DamageType
{
    // base types
    Physical,
    True,

    // elemental types
    Fire,
    Ice,
    Lightning,
    Poison,
    
    // Add your custom types here
    // Earth,
    // Water,
    // Shadow,
    // etc.
}
```

### Creating Custom Receivers

You can create custom damage receivers by inheriting from `BaseDamageReceiver`:

```csharp
// Example: A receiver that applies damage over time
public class DoTDamageReceiver : BaseDamageReceiver
{
    [SerializeField] private float tickRate = 1f;
    private float lastTickTime;
    private DamageData storedDamageData;
    private bool isDamageActive = false;

    public void StartDoTDamage(DamageData damageData, float duration)
    {
        storedDamageData = damageData;
        isDamageActive = true;
        lastTickTime = Time.time;
        
        // Stop the DoT after duration
        Invoke(nameof(StopDoTDamage), duration);
    }
    
    public void StopDoTDamage()
    {
        isDamageActive = false;
    }
    
    private void Update()
    {
        if (isDamageActive && Time.time >= lastTickTime + (1f / tickRate))
        {
            lastTickTime = Time.time;
            base.ReceiveDamage(storedDamageData);
        }
    }
}
```

### Damage Receiver Architecture

The damage receiver system is designed with extensibility in mind through inheritance:

1. **BaseDamageReceiver**: The foundation class that provides the core `ReceiveDamage` method which forwards damage to the DamageHandler.

2. **Specialized Receivers**: Inherit from BaseDamageReceiver to create receivers that handle specific damage sources:
   - **AOEReceiver**: Handles area-of-effect damage and effects
   - **Other Potential Receivers**: You can create your own specialized receivers for other damage sources
        - e.g. **ProjectileReceiver**: Handles projectile collisions

3. **Multiple Receivers**: You can add multiple different receiver types to the same GameObject to handle different damage sources. Each receiver:
   - Detects a specific type of damage source through collisions/triggers
   - Uses the inherited `ReceiveDamage` method to process damage
   - Forwards damage through the same DamageHandler pipeline

4. **Important Note**: Never add multiple receivers of the same type to a single GameObject, as this would cause damage to be applied multiple times for a single hit.

```csharp
// Example: Entity that could receive both projectile and AOE damage
public class Enemy : MonoBehaviour
{
    private void Awake()
    {
        // Add different receiver types for different damage sources
        gameObject.AddComponent<ProjectileReceiver>();
        gameObject.AddComponent<AOEReceiver>();
        
        // Both receivers will use the same DamageHandler component
        // All damage will flow through the same unified pipeline
    }
}
```

### Custom Status Effects

You can create custom status effects by extending the `BaseStatusEffect` class:

```csharp
// Example: A custom slow effect
public class SlowEffect : BaseStatusEffect
{
    private float slowPercentage;
    private Rigidbody2D targetRigidbody;
    private float originalSpeed;
    
    public SlowEffect(StatusEffectData data, float slowAmount) : base(data)
    {
        slowPercentage = slowAmount;
    }
    
    public override void OnApply(GameObject target)
    {
        base.OnApply(target);
        targetRigidbody = target.GetComponent<Rigidbody2D>();
        if (targetRigidbody != null)
        {
            // Store original speed and apply slow
            // Implementation depends on your movement system
        }
    }
    
    public override void OnRemove()
    {
        base.OnRemove();
        if (targetRigidbody != null)
        {
            // Restore original speed
        }
    }
}
```

## Best Practices

Key points to remember when using the Combat System:

1. **Damage Flow**: Always apply damage through the `ReceiveDamage` method on receivers, never modify health directly.

2. **Receiver Usage**: Be careful when adding multiple receivers of the same type to a single GameObject - each will respond to the same trigger events independently.

3. **Component Order**: Initialize the DamageHandler with a DefenseHandler before receiving damage - the system will log errors but won't crash if not initialized.

4. **AOE Configuration**: For persistent AOE zones, either set a reasonable duration or manually destroy them when no longer needed. For manual cleanup, use the `DestroyZone()` method:
   ```csharp
   // Example: Creating a persistent zone and destroying it later
   DamageAOEZone persistentZone = BaseAOEZone.Create<DamageAOEZone>(
       owner: transform,
       position: transform.position,
       damageData: damageData,
       customRadius: 5f
   );
   
   // Set to not auto-destroy
   persistentZone.AOEData.destroyOnEnd = false;
   
   // Later, when you want to destroy it:
   persistentZone.DestroyZone();
   ```

5. **Defense Values**: DamageType.True ignores all resistance calculations by design.

## Adapting for 3D

The Combat System is built for 2D games but can be easily adapted for 3D projects. The main changes required are in the collision detection methods:

### Modifying Receivers

For damage receivers, change the collision methods from 2D to 3D:

```csharp
// 2D version (current)
[RequireComponent(typeof(Collider2D))]
public class AOEReceiver : BaseDamageReceiver
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Collision handling
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        // Exit handling
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        // Stay handling
    }
}

// 3D version
[RequireComponent(typeof(Collider))]
public class AOEReceiver : BaseDamageReceiver
{
    private void OnTriggerEnter(Collider other)
    {
        // Same logic as 2D version but with 3D collider
    }
    
    private void OnTriggerExit(Collider other)
    {
        // Same logic as 2D version but with 3D collider
    }
    
    private void OnTriggerStay(Collider other)
    {
        // Same logic as 2D version but with 3D collider
    }
}
```

### Modifying AOE Zones

For AOE zones, replace CircleCollider2D with SphereCollider:

```csharp
// 2D version (current)
protected virtual void Awake()
{
    effectCollider = gameObject.GetComponent<CircleCollider2D>();
    if (effectCollider == null)
        effectCollider = gameObject.AddComponent<CircleCollider2D>();

    effectCollider.isTrigger = true;
    effectCollider.radius = aoeData.radius;
}

// 3D version
protected virtual void Awake()
{
    var sphereCollider = gameObject.GetComponent<SphereCollider>();
    if (sphereCollider == null)
        sphereCollider = gameObject.AddComponent<SphereCollider>();

    sphereCollider.isTrigger = true;
    sphereCollider.radius = aoeData.radius;
}
```

### Component References

Update component references and GetComponent calls to use 3D equivalents:

- Replace Rigidbody2D with Rigidbody
- Replace Collider2D with Collider
- Update any physics-based calculations to use 3D vectors (Vector3 instead of Vector2)
- Adjust raycasts and other physics queries to use Physics instead of Physics2D

### Physics Layers

Ensure your layer masks are configured for 3D physics interactions:

```csharp
// Update layer masks in AOEData and other configuration objects
aoeData.targetLayers = LayerMask.GetMask("Player", "Enemy", "NPC");
```

### Testing Conversion

When converting from 2D to 3D:
1. Update all collision methods systematically
2. Test each component individually
3. Verify that damage application works correctly
4. Check that AOE zones affect the right targets
5. Ensure status effects are applied and removed properly 