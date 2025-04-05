using TMPro;
using UnityEngine;
using Game01.Data; // Corrected namespace for CharacterData

namespace Game.UI
{
    /// <summary>
    /// Controls the UI elements for a single character entry in the roster.
    /// </summary>
    public class CharacterRosterEntry : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private TextMeshProUGUI energyText;

        private CharacterData associatedCharacter; // Optional: Store reference if needed later

        /// <summary>
        /// Sets up the UI elements with the provided character data.
        /// </summary>
        /// <param name="characterData">The data of the character to display.</param>
        public void Setup(CharacterData characterData)
        {
            if (characterData == null)
            {
                Debug.LogError("CharacterRosterEntry: Provided CharacterData is null.");
                // Optionally disable the GameObject or show placeholder text
                gameObject.SetActive(false);
                return;
            }

            associatedCharacter = characterData; // Store reference

            // Ensure Text components are assigned
            if (characterNameText == null || levelText == null || hpText == null || energyText == null)
            {
                Debug.LogError("CharacterRosterEntry: One or more TextMeshProUGUI components are not assigned in the inspector.", this);
                return;
            }

            characterNameText.text = characterData.name;
            UpdateLevel(characterData.level);
            UpdateHP(characterData.hp, characterData.maxHp); // Use correct field names
            UpdateEnergy(characterData.energy, characterData.maxEnergy); // Use correct field names

            gameObject.SetActive(true); // Ensure it's active if previously disabled
        }

        /// <summary>
        /// Updates the level display. Can be called externally on level up.
        /// </summary>
        public void UpdateLevel(int newLevel)
        {
            if (levelText != null)
            {
                levelText.text = $"Lvl: {newLevel}";
            }
        }

        /// <summary>
        /// Updates the HP display.
        /// </summary>
        public void UpdateHP(int currentHP, int maxHP)
        {
             if (hpText != null)
            {
                hpText.text = $"HP: {currentHP}/{maxHP}";
            }
        }

         /// <summary>
        /// Updates the Energy display.
        /// </summary>
        public void UpdateEnergy(int currentEnergy, int maxEnergy)
        {
             if (energyText != null)
            {
                energyText.text = $"Energy: {currentEnergy}/{maxEnergy}";
            }
        }

        // Optional: Add a method to get the associated character ID if needed for updates
        public string GetCharacterId()
        {
            return associatedCharacter?.characterId; // Use correct field name
        }
    }
}