using UnityEngine;

public class OfflineCollisionHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var kart = other.GetComponent<KartEntity>();
        if (kart != null)
        {
            HandleKartCollision(kart);
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        var kart = other.GetComponent<KartEntity>();
        if (kart != null)
        {
            HandleKartCollision(kart);
        }
    }
    
    private void HandleKartCollision(KartEntity kart)
    {
        // Check for different types of collidables
        var collidable = GetComponent<ICollidable>();
        if (collidable != null)
        {
            collidable.Collide(kart);
        }
        
        // Handle specific collision types
        if (CompareTag("ItemBox"))
        {
            var itemBox = GetComponent<ItemBox>();
            if (itemBox != null)
            {
                itemBox.OnKartEnter(kart);
            }
        }
        else if (CompareTag("Coin"))
        {
            var coin = GetComponent<Coin>();
            if (coin != null)
            {
                coin.Collide(kart);
            }
        }
        else if (CompareTag("Boostpad"))
        {
            var boostpad = GetComponent<Boostpad>();
            if (boostpad != null)
            {
                boostpad.OnKartEnter(kart);
            }
        }
    }
} 