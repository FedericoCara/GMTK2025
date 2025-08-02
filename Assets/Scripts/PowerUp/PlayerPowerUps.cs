using System.Collections.Generic;
using KartGame.KartSystems;
using UnityEngine;

namespace PowerUp
{
    public class PlayerPowerUps : MonoBehaviour
    {
        public List<BasePowerUp> powerUps;
        
        public void Add(BasePowerUp powerUpSelected)
        {
            powerUps.Add(powerUpSelected);
            powerUpSelected.Apply(gameObject);
        }
    }
}
