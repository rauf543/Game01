using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game01.Data
{
    [Serializable]
    public class CharacterData {
    public string characterId;  // Unique ID for the character
    public string CharacterName;
    public int CurrentLevel;
    public int CurrentXP;
    public int CurrentHP;
    public int CurrentEnergy;
    public int MaxHP;
    public int MaxEnergy;
    public List<PassiveSkill_SO> LearnedPassives;
    public List<CardDefinition_SO> CurrentRunDeck;
  }
}