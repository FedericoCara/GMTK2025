using UnityEngine;
using Weapons;

namespace PowerUp
{
    [CreateAssetMenu(menuName = "PowerUp/ShootSpeedPowerUp", fileName = "ShootSpeedPowerUp", order = 1)]
    public class ShootSpeedPowerUp : BasePowerUp
    {
        public float cadenceMultiplier = 0.1f;

        public override void Apply(GameObject player)
        {
            var autoShooter = player.GetComponentInChildren<AutoShooter>();
            autoShooter.shootFrequency *= (1 - cadenceMultiplier);
        }
    }
}