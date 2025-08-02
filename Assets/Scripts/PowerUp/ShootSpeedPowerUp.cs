using UnityEngine;
using Weapons;

namespace PowerUp
{
    [CreateAssetMenu(menuName = "PowerUp/ShootSpeedPowerUp", fileName = "ShootSpeedPowerUp", order = 1)]
    public class ShootSpeedPowerUp : BasePowerUp
    {
        public float cadenceMultiplier = 0.1f;

        public override void Apply(PlayerPowerUps player)
        {
            var autoShooters = player.Shooter.GetComponents<AutoShooter>();
            foreach (var autoShooter in autoShooters)
                autoShooter.shootFrequency *= (1 - cadenceMultiplier);
        }
    }
}