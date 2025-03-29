using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TeamRosterManager manages the player's team of characters.
/// It relies on the GameManager's singleton pattern and persistence mechanism.
/// </summary>
public class TeamRosterManager : MonoBehaviour
{
    // List to store all characters in the player's roster
    private List<CharacterData> teamRoster = new List<CharacterData>();

    // Public property to access the team roster
    public List<CharacterData> TeamRoster => teamRoster;

    private void Awake()
    {
        // Create default characters for testing purposes
        CreateDefaultCharacters();
    }

    /// <summary>
    /// Creates and adds default character data for testing
    /// </summary>
    private void CreateDefaultCharacters()
    {
        // Create first default character
        CharacterData warrior = new CharacterData
        {
            CharacterName = "Valiant Knight",
            CurrentLevel = 3,
            CurrentXP = 250,
            CurrentHP = 100,
            MaxHP = 100,
            CurrentEnergy = 50,
            MaxEnergy = 50,
            LearnedPassives = new List<PassiveSkill_SO>(),
            CurrentRunDeck = new List<CardDefinition_SO>()
        };

        // Create second default character
        CharacterData mage = new CharacterData
        {
            CharacterName = "Arcane Mage",
            CurrentLevel = 2,
            CurrentXP = 150,
            CurrentHP = 75,
            MaxHP = 75,
            CurrentEnergy = 60,
            MaxEnergy = 60,
            LearnedPassives = new List<PassiveSkill_SO>(),
            CurrentRunDeck = new List<CardDefinition_SO>()
        };

        // Create third default character
        CharacterData ranger = new CharacterData
        {
            CharacterName = "Swift Ranger",
            CurrentLevel = 2,
            CurrentXP = 180,
            CurrentHP = 85,
            MaxHP = 85,
            CurrentEnergy = 55,
            MaxEnergy = 55,
            LearnedPassives = new List<PassiveSkill_SO>(),
            CurrentRunDeck = new List<CardDefinition_SO>()
        };

        // Add default characters to the team roster
        AddCharacter(warrior);
        AddCharacter(mage);
        AddCharacter(ranger);
    }

    /// <summary>
    /// Adds a new character to the team roster
    /// </summary>
    /// <param name="character">The CharacterData instance to add</param>
    public void AddCharacter(CharacterData character)
    {
        if (character != null)
        {
            teamRoster.Add(character);
            Debug.Log($"Added character: {character.CharacterName} to the team roster");
        }
        else
        {
            Debug.LogWarning("Attempted to add a null character to the team roster");
        }
    }

    /// <summary>
    /// Gets a character from the team roster by index
    /// </summary>
    /// <param name="index">The index of the character</param>
    /// <returns>The CharacterData at the specified index, or null if index is out of range</returns>
    public CharacterData GetCharacter(int index)
    {
        if (index >= 0 && index < teamRoster.Count)
        {
            return teamRoster[index];
        }
        
        Debug.LogWarning($"Character index {index} is out of range");
        return null;
    }

    /// <summary>
    /// Gets a character from the team roster by name
    /// </summary>
    /// <param name="name">The name of the character to find</param>
    /// <returns>The first CharacterData with matching name, or null if not found</returns>
    public CharacterData GetCharacterByName(string name)
    {
        return teamRoster.Find(character => character.CharacterName == name);
    }

    /// <summary>
    /// Gets the current size of the team roster
    /// </summary>
    /// <returns>The number of characters in the team roster</returns>
    public int GetRosterSize()
    {
        return teamRoster.Count;
    }

    /// <summary>
    /// Removes a character from the team roster by index
    /// </summary>
    /// <param name="index">The index of the character to remove</param>
    /// <returns>True if removal was successful, false otherwise</returns>
    public bool RemoveCharacter(int index)
    {
        if (index >= 0 && index < teamRoster.Count)
        {
            string name = teamRoster[index].CharacterName;
            teamRoster.RemoveAt(index);
            Debug.Log($"Removed character: {name} from the team roster");
            return true;
        }
        
        Debug.LogWarning($"Failed to remove character. Index {index} is out of range");
        return false;
    }
}