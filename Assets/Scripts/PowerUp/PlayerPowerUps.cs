using System.Collections.Generic;
using KartGame.KartSystems;
using UnityEngine;

namespace PowerUp
{
    public class PlayerPowerUps : MonoBehaviour
    {
        public List<BasePowerUp> powerUps;
        [SerializeField] private GameObject shooter;
        public GameObject Shooter => shooter;
        [SerializeField] private ArcadeKart arcadeKart;
        public ArcadeKart ArcadeKart => arcadeKart;

        public void Add(BasePowerUp powerUpSelected)
        {
            powerUps.Add(powerUpSelected);
            powerUpSelected.Apply(this);
        }
    }
}
