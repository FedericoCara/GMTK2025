using System;
using UnityEngine;

namespace Items
{
    public class Looter : MonoBehaviour
    {
        public int lootsCollected;

        public event Action<int> OnLootUpdated;

        private int LootsCollected
        {
            get => lootsCollected;
            set
            {
                lootsCollected = value;
                OnLootUpdated?.Invoke(lootsCollected);
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