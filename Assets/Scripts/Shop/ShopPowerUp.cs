using System;
using System.Collections.Generic;
using System.Linq;
using Items;
using PowerUp;
using UnityEngine;

namespace Shop
{
    public class ShopPowerUp : MonoBehaviour
    {
        public List<PowerUpInShop> powerUpDisplay;
        public List<BasePowerUp> powerUpAvailable;
        public List<Transform> playerPositions;
        private Dictionary<BasePowerUp, int> _powerUpsGiven = new Dictionary<BasePowerUp, int>();
        private ObjectiveCompleteLaps _lapInfo;
        private bool _active;
        private int _lastHorizontal;
        private int _currentPlayerPosition;
        private Transform _player;
        private List<BasePowerUp> _options;
        private Looter _looter;
        private int _lastPlayerPosition = -1;
        private bool _purchaseMade;
        private Action _callback;

        private void Start()
        {
            _lapInfo = FindAnyObjectByType<ObjectiveCompleteLaps>();
        }

        public void Activate(Transform player, Action onPurchaseMade)
        {
            _active = true;
            _player = player;
            _looter = player.GetComponent<Looter>();
            _lastPlayerPosition = -1;
            _purchaseMade = false;
            _callback = onPurchaseMade;
            int currentLap = _lapInfo.currentLap;
            _options = GetOptions(powerUpDisplay.Count);
            for (int i = 0; i < powerUpDisplay.Count; i++)
            {
                powerUpDisplay[i].SetPowerUp(_options[i]);
            }

            _currentPlayerPosition = playerPositions.Count / 2;
            UpdatePlayerPosition();
            FocusPowerUp();
        }

        private void Update()
        {
            if (_active && !_purchaseMade)
            {
                var input = GetDiscreteHorizontalInput();
                if (input < 0 && _currentPlayerPosition>0)
                {
                    _lastPlayerPosition = _currentPlayerPosition;
                    _currentPlayerPosition--;
                    UpdatePlayerPosition();
                    FocusPowerUp();
                }else if (input > 0 && _currentPlayerPosition < playerPositions.Count - 1)
                {
                    _lastPlayerPosition = _currentPlayerPosition;
                    _currentPlayerPosition++;
                    UpdatePlayerPosition();
                    FocusPowerUp();
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    TryBuyPowerUp();
                }
            }
        }

        private void FocusPowerUp()
        {
            if (_lastPlayerPosition >= 0)
            {
                UnfocusPowerUp(_lastPlayerPosition);
            }
            if (IsFocusedPowerUpAffordable())
            {
                AffordablePowerUp(_currentPlayerPosition);
            }
            else
            {
                UnaffordablePowerUp(_currentPlayerPosition);
            }
        }

        private bool IsFocusedPowerUpAffordable() => FocusedPowerUp.cost<=_looter.Coins;

        private void UnfocusPowerUp(int currentPlayerPosition)
        {
            powerUpDisplay[currentPlayerPosition].Unfocus();
        }

        private void UnaffordablePowerUp(int currentPlayerPosition)
        {
            powerUpDisplay[currentPlayerPosition].Unaffordable();
        }

        private void AffordablePowerUp(int currentPlayerPosition)
        {
            powerUpDisplay[currentPlayerPosition].Affordable();
        }

        private BasePowerUp FocusedPowerUp => _options[_currentPlayerPosition];

        private void TryBuyPowerUp()
        {
            if (IsFocusedPowerUpAffordable())
            {
                var powerUpSelected = FocusedPowerUp;
                _powerUpsGiven[powerUpSelected]++;
                _player.GetComponent<Looter>().Coins -= powerUpSelected.cost;
                _player.GetComponent<PlayerPowerUps>().Add(powerUpSelected);
                ReproducePurchaseSuccessSfx();
                DisablePurchases();
                _callback?.Invoke();
                _purchaseMade = true;
            }
            else
            {
                ReproducePurchaseFailSfx();
            }

        }

        private void DisablePurchases()
        {
            foreach (PowerUpInShop powerUpInShop in powerUpDisplay)
            {
                powerUpInShop.Disable();
            }
        }

        private void ReproducePurchaseFailSfx()
        {
            //TODO
        }

        private void ReproducePurchaseSuccessSfx()
        {
            //TODO
        }

        public void Deactivate()
        {
            _active = false;
        }

        private void UpdatePlayerPosition()
        {
            Debug.Log($"Moving player from {_player.position} to {playerPositions[_currentPlayerPosition].position}");
            _player.position = playerPositions[_currentPlayerPosition].position;
        }

        private List<BasePowerUp> GetOptions(int count)
        {
            // Inicializar diccionario si faltan powerUps
            foreach (var powerUp in powerUpAvailable)
            {
                if (!_powerUpsGiven.ContainsKey(powerUp))
                    _powerUpsGiven[powerUp] = 0;
            }

            List<BasePowerUp> options = new();
            System.Random rng = new System.Random();

            var powerUpsGivenAndOffered = new Dictionary<BasePowerUp, int>(_powerUpsGiven);

            for (int i = 0; i < count; i++)
            {
                // Encontrar el mínimo de veces otorgado
                int minGiven = powerUpsGivenAndOffered.Values.Min();

                // Filtrar los powerUps con ese valor
                var leastGiven = powerUpAvailable
                    .Where(p => powerUpsGivenAndOffered[p] == minGiven)
                    .ToList();

                // Seleccionar aleatoriamente de los menos dados
                var chosen = leastGiven[rng.Next(leastGiven.Count)];

                options.Add(chosen);
                powerUpsGivenAndOffered[chosen]++; // Actualizamos el contador
            }

            return options;
        }

        private int GetDiscreteHorizontalInput()
        {
            return ( Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) ) ? -1
                : (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) ? 1 
                : 0;
        }
    }
}