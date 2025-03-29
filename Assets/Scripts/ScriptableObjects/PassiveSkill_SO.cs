using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PassiveModifierData {
    public enum ModifierType { AddMaxHP, AddMaxEnergy, ModifyEnergyCost }
    public ModifierType modifierType;
    public float ModificationValue;
    public string Condition;
}

[CreateAssetMenu(fileName = "PassiveSkill_", menuName = "ScriptableObjects/PassiveSkill_SO")]
public class PassiveSkill_SO : ScriptableObject {
    public string SkillID;
    public string SkillName;
    public string Description;
    public List<PassiveModifierData> Modifiers;
}