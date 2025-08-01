using Enemies;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [SerializeField] private int damage;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(Enemy.EnemyTag))
        {
            other.GetComponentInParent<Enemy>().ReceiveDamage(damage);
        } 
    }
}
