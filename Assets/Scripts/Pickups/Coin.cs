using UnityEngine;

public class Coin : MonoBehaviour, ICollidable {

    public bool IsActive { get; set; } = true;

    public Transform visuals;

    private void Start()
    {
        UpdateVisuals();
    }

    private void Update()
    {
        // Handle state changes
        if (!IsActive)
        {
            UpdateVisuals();
        }
    }

    public bool Collide(KartEntity kart) {
        if (IsActive) {
            kart.CoinCount++;
            kart.OnCoinCountChanged();

            IsActive = false;
            UpdateVisuals();
            
            // Destroy the coin
            Destroy(gameObject);
        }

        return true;
    }

    private void UpdateVisuals() {
        if (visuals != null)
        {
            visuals.gameObject.SetActive(IsActive);
        }

        if (!IsActive)
        {
            AudioManager.PlayAndFollow("coinSFX", transform, AudioManager.MixerTarget.SFX);
        }
    }
}

