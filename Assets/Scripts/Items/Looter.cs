using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Items
{
    public class Looter : MonoBehaviour
    {
        [SerializeField] private int coins;

        public event Action<int> OnCoinsUpdated;

        public int Coins
        {
            get => coins;
            set
            {
                coins = value;
                OnCoinsUpdated?.Invoke(coins);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag.Equals(Loot.LootTag))
            {
                TryPickup(other.GetComponentInParent<Loot>());
            }
        }

        private void TryPickup(Loot other)
        {
            if (other.PickedUp())
            {
                Coins++;
            }
        }
    }
}