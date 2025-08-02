using System;
using UnityEngine;

namespace PowerUp
{
    public abstract class BasePowerUp : ScriptableObject
    {
        public Rarity rarity = Rarity.Common;
        public int cost = 1;
        public PowerUpRepresentation representation;
    }

    [Serializable]
    public class PowerUpRepresentation
    {
        public GameObject icon;
    }

    public enum Rarity
    {
        Common,
        Rare,
        Epic
    }
}
