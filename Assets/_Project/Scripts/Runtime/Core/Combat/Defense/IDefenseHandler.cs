public interface IDefenseHandler
{
    // core functionality
    float HandleDefense(float incomingDamage, DamageData damageData);
    float GetCritResistance();

    // physical resistance methods
    float GetPhysicalResistance();
    void SetPhysicalResistance(float value);
    void ModifyPhysicalResistance(float amount);

    // elemental resistance methods
    float GetElementalResistance(DamageType type);
    void SetElementalResistance(DamageType type, float value);
    void ModifyElementalResistance(DamageType type, float amount);

    // get the underlying defense data
    DefenseData GetDefenseData();
}
