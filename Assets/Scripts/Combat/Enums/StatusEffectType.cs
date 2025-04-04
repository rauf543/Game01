// Assets/Scripts/Combat/Enums/StatusEffectType.cs
namespace Game01.Combat
{
    public enum StatusEffectType
    {
        Burn,
        Shielded, // Note: While 'Shielded' might seem like a status, the implementation uses a direct 'CurrentShield' variable. Including it here for potential future use or if the design changes, but the current implementation doesn't use this specific enum value for shield logic.
        AttackUp,
        DefenseDown
    }
}