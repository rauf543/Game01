// Assets/Scripts/Combat/Data/ActiveStatusEffect.cs
using Game01.Combat; // Assuming StatusEffectType is in this namespace

namespace Game01.Combat.Data
{
    [System.Serializable]
    public class ActiveStatusEffect
    {
        public StatusEffectType Type;
        public int Duration;
        public int Stacks;

        // Optional: Constructor for convenience
        public ActiveStatusEffect(StatusEffectType type, int duration, int stacks)
        {
            Type = type;
            Duration = duration;
            Stacks = stacks;
        }
    }
}