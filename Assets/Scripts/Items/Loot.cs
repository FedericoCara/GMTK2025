using UnityEngine;

namespace Items
{
    public class Loot : MonoBehaviour
    {
        public const string LootTag = "Loot";
        private bool alreadyPickedUp;

        public bool PickedUp()
        {
            if (alreadyPickedUp)
                return false;
            alreadyPickedUp = true;
            Destroy(gameObject);
            return true;
        }
    }
}