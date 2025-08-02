using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Items
{
    public class Looter : MonoBehaviour
    {
        [FormerlySerializedAs("lootsCollected")] public int coins;

        public event Action<int> OnLootUpdated;

        private int LootsCollected
        {
            get => coins;
            set
            {
                coins = value;
                OnLootUpdated?.Invoke(coins);
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
                LootsCollected++;
            }
        }
    }
}