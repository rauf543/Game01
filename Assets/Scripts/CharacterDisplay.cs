using UnityEngine;
using TMPro;
using Game01.Data;

public class CharacterDisplay : MonoBehaviour
{
    // Reference to the UI Text components
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI characterLevelText;
    
    // Internal data storage
    private CharacterData currentData;
    
    public void Initialize(CharacterData data)
    {
        // Store data
        currentData = data;
        
        // Update UI
        if (characterNameText != null)
            characterNameText.text = currentData.CharacterName;
        
        if (characterLevelText != null)
            characterLevelText.text = "Level: " + currentData.CurrentLevel.ToString();
    }
}