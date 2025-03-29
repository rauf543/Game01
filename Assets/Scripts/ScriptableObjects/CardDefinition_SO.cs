using System.Collections.Generic;
using UnityEngine;

public enum TargetType { Self, EnemySingle, EnemyAll }

[System.Serializable]
public class CardEffectData {
    public enum EffectType { Damage, Shield, ApplyBurn }
    public EffectType effectType;
    public int Value;
    public float Duration;
    public TargetType EffectTarget;
}

[CreateAssetMenu(fileName = "CardDefinition_", menuName = "ScriptableObjects/CardDefinition_SO")]
public class CardDefinition_SO : ScriptableObject {
    public string CardID;
    public string CardName;
    public string Description;
    public int EnergyCost;
    public float Cooldown;
    public TargetType TargetType;
    public List<CardEffectData> Effects;
}