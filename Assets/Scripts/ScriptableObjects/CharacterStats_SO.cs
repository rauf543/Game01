using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats_", menuName = "ScriptableObjects/CharacterStats_SO")]
public class CharacterStats_SO : ScriptableObject {
    public string CharacterArchetypeID;
    public int BaseMaxHP;
    public int BaseMaxEnergy;
}