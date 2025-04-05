// Assets/Scripts/Combat/CharacterCombat.cs
using UnityEngine;
using System.Collections.Generic;
using Game01.Data; // For CharacterData
using Game01.Combat.Data; // For ActiveStatusEffect
using Game01.Combat; // For StatusEffectType

namespace Game01.Combat
{
    public class CharacterCombat : MonoBehaviour
    {
        // Public fields for runtime combat state
        public int CurrentHP;
        public int MaxHP;
        public int CurrentShield = 0;
        public List<ActiveStatusEffect> ActiveEffects = new List<ActiveStatusEffect>();
        public CharacterData Data { get; private set; }

        /// <summary>
        /// Initializes the character's combat state based on persistent data.
        /// Called at the start of combat.
        /// </summary>
        /// <param name="persistentData">The character's base stats and info.</param>
        public void Initialize(CharacterData persistentData)
        {
            if (persistentData != null)
            {
                Data = persistentData; // Store the reference
                // Use the maxHp field from CharacterData
                MaxHP = persistentData.maxHp;
                CurrentHP = persistentData.maxHp;
            } // End of if (persistentData != null)
            else
            {
                Debug.LogError("CharacterData is null during CharacterCombat initialization!", this);
                // Set default values to prevent errors, though this indicates a setup problem
                MaxHP = 100; // Default fallback
                CurrentHP = 100; // Default fallback
            }

            CurrentShield = 0;
            ActiveEffects.Clear();
        }

        /// <summary>
        /// Applies damage to the character, reducing shield first, then health.
        /// </summary>
        /// <param name="amount">The raw amount of damage to apply.</param>
        public void TakeDamage(int amount)
        {
            if (amount <= 0) return; // No damage or negative damage (healing?)

            int shieldDamage = Mathf.Min(amount, CurrentShield);
            CurrentShield -= shieldDamage;

            int healthDamage = amount - shieldDamage;
            if (healthDamage > 0)
            {
                CurrentHP -= healthDamage;
                CurrentHP = Mathf.Max(0, CurrentHP); // Prevent HP from going below 0
            }

            // Optional: Add event triggers here later for UI/VFX if needed
            // Debug.Log($"{gameObject.name} took {amount} damage. Shield: {CurrentShield}, HP: {CurrentHP}/{MaxHP}");
        }

        /// <summary>
        /// Adds shield points to the character's current shield.
        /// </summary>
        /// <param name="amount">The amount of shield to add.</param>
        public void ReceiveShield(int amount)
        {
            if (amount <= 0) return; // No shield added

            CurrentShield += amount;
            // Optional: Add event triggers here later for UI/VFX if needed
            // Debug.Log($"{gameObject.name} received {amount} shield. Total Shield: {CurrentShield}");
        }

        /// <summary>
        /// Applies a status effect to the character or updates an existing one.
        /// </summary>
        /// <param name="type">The type of status effect.</param>
        /// <param name="duration">The duration (in turns/ticks) of the effect.</param>
        /// <param name="stacks">The number of stacks to apply.</param>
        public void ReceiveStatusEffect(StatusEffectType type, int duration, int stacks)
        {
            if (stacks <= 0 || duration <= 0) return; // Cannot apply effect with no stacks or duration

            ActiveStatusEffect existingEffect = ActiveEffects.Find(e => e.Type == type);

            if (existingEffect != null)
            {
                // Found existing effect: Add stacks, reset duration
                existingEffect.Stacks += stacks;
                existingEffect.Duration = duration; // Reset duration as per instructions
                // Debug.Log($"{gameObject.name} updated {type}: Stacks={existingEffect.Stacks}, Duration={existingEffect.Duration}");
            }
            else
            {
                // Not found: Add new effect
                ActiveEffects.Add(new ActiveStatusEffect(type, duration, stacks));
                // Debug.Log($"{gameObject.name} received new {type}: Stacks={stacks}, Duration={duration}");
            }
        }

        /// <summary>
        /// Processes active status effects, applying their periodic logic (like damage over time)
        /// and decrementing their duration. Called typically at the start or end of a turn.
        /// </summary>
        public void TickStatusEffects()
        {
            // Iterate backwards for safe removal while iterating
            for (int i = ActiveEffects.Count - 1; i >= 0; i--)
            {
                ActiveStatusEffect effect = ActiveEffects[i];

                // --- Apply Periodic Effects ---
                // Example: Burn deals damage equal to its stacks each tick
                if (effect.Type == StatusEffectType.Burn)
                {
                    // Debug.Log($"{gameObject.name} taking {effect.Stacks} Burn damage.");
                    TakeDamage(effect.Stacks); // Apply burn damage
                }
                // Add other periodic effects here (e.g., healing over time)

                // --- Decrement Duration ---
                effect.Duration--;
                // Debug.Log($"{gameObject.name} ticked {effect.Type}: Duration remaining {effect.Duration}");


                // --- Remove Expired Effects ---
                if (effect.Duration <= 0)
                {
                    // Debug.Log($"{gameObject.name}'s {effect.Type} expired.");
                    ActiveEffects.RemoveAt(i);
                }
            }
        }
    }
}