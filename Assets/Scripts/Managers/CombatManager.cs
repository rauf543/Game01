using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    // Static events for combat state changes
    public static event System.Action OnCombatStart;
    public static event System.Action<bool> OnCombatEnd;

    // Method to start combat with player team and enemy data
    public void StartCombat(List<CharacterData> playerTeam, List<EnemyData_SO> enemyData)
    {
        // Stub method - intentionally left empty as per requirements
    }

    // Method to end combat
    public void EndCombat()
    {
        // Stub method - intentionally left empty as per requirements
    }
}