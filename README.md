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
8. [Best Practices](#best-practices)
9. [Adapting for 3D](#adapting-for-3d)

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
  - `AOEDamageReceiver`: Extended receiver that handles both direct and area-of-effect damage

### Defense System
- Handles damage mitigation through resistances
- Supports physical and elemental resistance types
- Uses customizable defense profiles for different entity types

### Status Effects
- Framework for applying, stacking, and removing temporary effects
- Uses factory pattern for creating different effect types
- Provides event-based notification system for effect changes

### Area of Effect (AOE)
- Base implementation for creating damage zones
- Supports different AOE behaviors (on enter, over time, on exit)
- Uses layer-based targeting for selective damage application

## Setting Up a Damageable Entity

To create an entity that can receive damage:

```csharp
// 1. Add required components to your GameObject
// - HealthSystem (for health tracking)
// - DamageHandler (for damage processing)
// - Choose ONE receiver type:
//   * BaseDamageReceiver for direct damage only
//   * AOEDamageReceiver for both direct and AOE damage

// 2. Initialize the defense handler in Start() or Awake()
void Start()
{
    // Create a defense profile (or load from ScriptableObject)
    DefenseProfile defenseProfile = new DefenseProfile();
    defenseProfile.PhysicalResistance = 0.2f;  // 20% physical damage reduction
    
    // Create defense data from profile
    DefenseData defenseData = new DefenseData(defenseProfile);
    
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

// 2. Find target and apply damage using IDamageReceiver
IDamageReceiver receiver = hitObject.GetComponent<IDamageReceiver>();
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

Entities need an `AOEDamageReceiver` component to receive AOE damage:

```csharp
// Add AOEDamageReceiver to your entity
// This component inherits from BaseDamageReceiver, so it handles both direct and AOE damage
AOEDamageReceiver aoeReceiver = gameObject.AddComponent<AOEDamageReceiver>();

// The receiver will automatically:
// - Process direct damage through the ReceiveDamage method
// - Detect and process AOE damage through trigger collisions with AOE zones
```

## Status Effects

The status effect system applies temporary effects to entities:

### Applying Status Effects

```csharp
// 1. Create status effect data
StatusEffectData effectData = new StatusEffectData(
    effectName: "Burning",      // Unique identifier
    effectType: StatusEffect.Burning, // Effect type enum
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

## Best Practices

1. **Component Organization**:
   - Add all required components to your entity in the correct order
   - Initialize defense handlers in Start() or Awake()
   - Use RequireComponent attributes to ensure dependencies

2. **Damage Types**:
   - Use appropriate damage types for different attacks
   - `DamageType.Physical`: Standard physical damage (affected by physical resistance)
   - `DamageType.True`: Damage that ignores resistances
   - Elemental types: For specialized damage with corresponding resistances

3. **Defense Balancing**:
   - Keep resistance values between 0 and 1 (0% to 100%)
   - Consider diminishing returns for high resistance values
   - Balance resistances against damage values for appropriate difficulty

4. **Status Effects**:
   - Use descriptive names for status effects
   - Consider effect interactions (e.g., Wet + Fire = Steam)
   - Limit the number of simultaneous effects for performance

5. **AOE Usage**:
   - Use appropriate layer masks for targeting specific entity types
   - Consider performance with many AOE zones
   - Destroy AOE zones when no longer needed

6. **Event Handling**:
   - Always unsubscribe from events when objects are destroyed
   - Use events for loose coupling between systems
   - Consider using ScriptableObject-based events for global communication

7. **Damage Receivers**:
   - `BaseDamageReceiver` is for entities that only need to receive direct damage
   - `AOEDamageReceiver` is for entities that need to receive both direct and AOE damage
   - Choose the appropriate receiver based on your entity's needs

## Adapting for 3D

The Combat System is built for 2D games but can be easily adapted for 3D projects. The main changes required are in the collision detection methods:

### Modifying Receivers

For damage receivers, change the collision methods from 2D to 3D:

```csharp
// 2D version (current)
[RequireComponent(typeof(Collider2D))]
public class AOEDamageReceiver : BaseDamageReceiver
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
public class AOEDamageReceiver : BaseDamageReceiver
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