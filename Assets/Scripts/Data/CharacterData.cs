using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game01.Data
{
    [Serializable]
    public class CharacterData {
    public string characterId;  // Unique ID for the character
    public string name;
    public int level;
    public int xp;
    public int hp;
    public int energy;
    public int maxHp;
    public int maxEnergy;
    public List<PassiveSkill_SO> LearnedPassives;
    public List<CardDefinition_SO> CurrentRunDeck;
  }
}