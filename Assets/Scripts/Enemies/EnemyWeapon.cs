using System;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public int damage;
    public float cooldown;
    public bool active = true;
    private void OnTriggerEnter(Collider other)
    {
        if (!active)
        {
            return;
        }
        
        if (other.gameObject.tag.Equals("Player"))
        {
            var playerController = other.GetComponentInParent<PlayerController>();
            if (playerController != null)
            {
                playerController.Damage(damage);
                
                if (cooldown > 0)
                {
                    active = false;
                    Invoke(nameof(ReActive), cooldown);
                }
            }
        }
    }

    private void ReActive()
    {
        active = true;
    }
}
