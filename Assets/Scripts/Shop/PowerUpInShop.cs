using PowerUp;
using UnityEngine;

namespace Shop
{
    public class PowerUpInShop : MonoBehaviour
    {
        public Transform powerUpContainer;

        public void SetPowerUp(BasePowerUp powerUp)
        {
            ClearChilds();
            Instantiate(powerUp.representation.icon, powerUpContainer);
        }

        private void ClearChilds()
        {
            foreach (Transform child in powerUpContainer)
            {
                Destroy(child.gameObject);
            }
        }
    }
}