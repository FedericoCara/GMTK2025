using Items;
using UnityEngine;

namespace Enemies
{
    public class Enemy : MonoBehaviour
    {
        public const string EnemyTag = "Enemy";
        public int health = 5;
        public Loot loot;
        
        public void ReceiveDamage(int damage)
        {
            health -= damage;
            if (health <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Destroy(gameObject);
            Instantiate(loot, transform.position, Quaternion.identity);
        }
    }
}
