using System.Collections.Generic;
using UnityEngine;

public enum AIBehavior { SimpleAttack, Defensive }

[CreateAssetMenu(fileName = "EnemyData_", menuName = "ScriptableObjects/EnemyData_SO")]
public class EnemyData_SO : ScriptableObject {
    public string EnemyID;
    public string EnemyName;
    public int BaseMaxHP;
    public int BaseMaxEnergy;
    public List<CardDefinition_SO> EnemyDeck;
    public AIBehavior AIBehavior;
}