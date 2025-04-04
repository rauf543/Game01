// Assets/Scripts/Combat/CombatActions.cs
using UnityEngine;
using Game01.Combat; // For StatusEffectType and CharacterCombat

namespace Game01.Combat
{
    /// <summary>
    /// Static utility class for performing common combat actions on characters.
    /// Ensures actions are directed through the CharacterCombat component.
    /// </summary>
    public static class CombatActions
    {
        /// <summary>
        /// Deals damage to a target character.
        /// </summary>
        /// <param name="attacker">The GameObject initiating the damage (can be null if source is ambiguous).</param>
        /// <param name="target">The GameObject receiving the damage.</param>
        /// <param name="amount">The amount of damage to deal.</param>
        public static void DealDamage(GameObject attacker, GameObject target, int amount)
        {
            if (target == null)
            {
                Debug.LogError("DealDamage: Target GameObject is null.");
                return;
            }

            CharacterCombat targetCombat = target.GetComponent<CharacterCombat>();
            if (targetCombat == null)
            {
                Debug.LogError($"DealDamage: Target GameObject '{target.name}' does not have a CharacterCombat component.", target);
                return;
            }

            // Optional: Could add logic here based on attacker's stats/status effects later
            // Optional: Could add logic here based on target's stats/status effects (e.g., DefenseDown) later

            targetCombat.TakeDamage(amount);
        }

        /// <summary>
        /// Applies shield points to a target character.
        /// </summary>
        /// <param name="target">The GameObject receiving the shield.</param>
        /// <param name="amount">The amount of shield to apply.</param>
        public static void ApplyShield(GameObject target, int amount)
        {
            if (target == null)
            {
                Debug.LogError("ApplyShield: Target GameObject is null.");
                return;
            }

            CharacterCombat targetCombat = target.GetComponent<CharacterCombat>();
            if (targetCombat == null)
            {
                Debug.LogError($"ApplyShield: Target GameObject '{target.name}' does not have a CharacterCombat component.", target);
                return;
            }

            targetCombat.ReceiveShield(amount);
        }

        /// <summary>
        /// Applies a status effect to a target character.
        /// </summary>
        /// <param name="target">The GameObject receiving the status effect.</param>
        /// <param name="type">The type of status effect.</param>
        /// <param name="duration">The duration of the effect.</param>
        /// <param name="stacks">The number of stacks to apply.</param>
        public static void ApplyStatusEffect(GameObject target, StatusEffectType type, int duration, int stacks)
        {
            if (target == null)
            {
                Debug.LogError("ApplyStatusEffect: Target GameObject is null.");
                return;
            }

            CharacterCombat targetCombat = target.GetComponent<CharacterCombat>();
            if (targetCombat == null)
            {
                Debug.LogError($"ApplyStatusEffect: Target GameObject '{target.name}' does not have a CharacterCombat component.", target);
                return;
            }

            targetCombat.ReceiveStatusEffect(type, duration, stacks);
        }
    }
}