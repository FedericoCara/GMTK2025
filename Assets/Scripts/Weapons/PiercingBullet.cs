using System.Collections.Generic;
using Enemies;
using UnityEngine;

namespace Weapons
{
    public class PiercingBullet : Bullet
    {
        private List<Enemy> enemiesHit = new();
        
        protected override void OnImpactedEnemy(Enemy other)
        {
            if (enemiesHit.Contains(other))
            {
                //ignore
            }
            else
            {
                other.ReceiveDamage(damage);
                enemiesHit.Add(other);
            }
        }
    }
}