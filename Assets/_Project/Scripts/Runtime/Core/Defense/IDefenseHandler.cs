public interface IDefenseHandler
{
    float HandleDefense(float incomingDamage, DamageData damageData);
    float GetCritResistance();
}

// public class StandardDefenseHandler : IDefenseHandler
// {
//     private DefenseData defenseData;

//     public StandardDefenseHandler(DefenseData data) => defenseData = data;

//     public float HandleDefense(float incomingDamage, DamageData damageData)
//     {
//         // Current implementation
//         return 1f;
//     }
// }

// public class ReflectDefenseHandler : IDefenseHandler
// {
//     private DefenseData defenseData;

//     public ReflectDefenseHandler(DefenseData data) => defenseData = data;

//     public float HandleDefense(float incomingDamage, DamageData damageData)
//     {
//         // Different implementation that reflects damage
//         return 1f;
//     }
// }
