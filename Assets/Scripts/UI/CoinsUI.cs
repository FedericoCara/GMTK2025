using System;
using Items;
using TMPro;
using UnityEngine;

namespace UI
{
    public class CoinsUI : MonoBehaviour
    {
        public TMP_Text valueTxt;

        private void Start()
        {
            var looter = FindFirstObjectByType<Looter>();
            UpdateCoins(looter.coins);
            looter.OnLootUpdated += UpdateCoins;
        }

        private void UpdateCoins(int coins)
        {
            valueTxt.text = coins.ToString();
        }
    }
}
