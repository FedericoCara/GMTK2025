using PowerUp;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Shop
{
    public class PowerUpInShop : MonoBehaviour
    {
        public Transform powerUpContainer;
        public Image focusImage;
        public TMP_Text costText;
        public Color unfocusedColor = Color.gray;
        public Color unaffordableColor = Color.red;
        public Color affordableColor = Color.white;
        public Color unfocusedCostTextColor = Color.white;
        public Color unaffordableCostTextColor = Color.white;
        public Color affordableCostTextColor = Color.black;

        public void SetPowerUp(BasePowerUp powerUp)
        {
            ClearChilds();
            costText.text = powerUp.cost.ToString();
            Instantiate(powerUp.representation.icon, powerUpContainer);
            Enable();
        }

        private void ClearChilds()
        {
            foreach (Transform child in powerUpContainer)
            {
                Destroy(child.gameObject);
            }
        }

        public void Unfocus()
        {
            focusImage.color = unfocusedColor;
            costText.color = unfocusedCostTextColor;
        }

        public void Unaffordable()
        {
            focusImage.color = unaffordableColor;
            costText.color = unaffordableCostTextColor;
        }

        public void Affordable()
        {
            focusImage.color = affordableColor;
            costText.color = affordableCostTextColor;
        }

        private void Enable()
        {
            Unfocus();
        }

        public void Disable()
        {
            focusImage.color = Color.clear;
            costText.color = Color.clear;
        }
    }
}