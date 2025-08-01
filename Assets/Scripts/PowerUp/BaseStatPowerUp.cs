using KartGame.KartSystems;
using UnityEngine;

namespace PowerUp
{
    [CreateAssetMenu(menuName = "PowerUp/BaseStatPowerUp", fileName = "BasePowerUp", order = 0)]
    public class BaseStatPowerUp : BasePowerUp
    {
        public ArcadeKart.StatPowerup addedStats = new ArcadeKart.StatPowerup
        {
            MaxTime = float.MaxValue
        };
    }
}