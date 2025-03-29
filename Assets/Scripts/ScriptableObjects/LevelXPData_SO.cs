using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelXPRequirement {
    public int Level;
    public int XPRequired;
}

[CreateAssetMenu(fileName = "LevelXPData_", menuName = "ScriptableObjects/LevelXPData_SO")]
public class LevelXPData_SO : ScriptableObject {
    public List<LevelXPRequirement> LevelRequirements;
}