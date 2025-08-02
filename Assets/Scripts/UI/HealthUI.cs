using System;
using Items;
using TMPro;
using UnityEngine;

namespace UI
{
    public class HealthUI : MonoBehaviour
    {
        public TMP_Text valueTxt;
        
        PlayerController playerController;

        private void Start()
        {
            playerController = FindFirstObjectByType<PlayerController>();
            UpdateText(playerController.Health);
            playerController.OnHealthChanged += UpdateText;
        }

        private void OnDestroy()
        {
            playerController.OnHealthChanged -= UpdateText;
        }

        private void UpdateText(int coins)
        {
            valueTxt.text = coins.ToString();
        }
    }
}
