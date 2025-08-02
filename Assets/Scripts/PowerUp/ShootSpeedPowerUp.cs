using UnityEngine;

namespace PowerUp
{
    [CreateAssetMenu(menuName = "PowerUp/ShootSpeedPowerUp", fileName = "ShootSpeedPowerUp", order = 1)]
    public class ShootSpeedPowerUp : BasePowerUp
    {
        public float cadenceMultiplier = 0.1f;
    }
}