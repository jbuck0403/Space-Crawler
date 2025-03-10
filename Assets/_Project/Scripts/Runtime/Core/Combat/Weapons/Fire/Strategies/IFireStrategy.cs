// IFireStrategy.cs
using UnityEngine;

public interface IFireStrategy : IStrategy
{
    // Configuration
    FireConfig GetFireConfig();

    // State pattern methods
    void OnEnter(BaseWeapon weapon, Transform self, Transform target);
    void OnUpdate(BaseWeapon weapon, Transform self, Transform target);
    void OnExit();
}
