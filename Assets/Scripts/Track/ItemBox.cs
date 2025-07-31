using UnityEngine;
using Random = UnityEngine.Random;

public class ItemBox : MonoBehaviour, ICollidable {
    
    public GameObject model;
    public ParticleSystem breakParticle;
    public float cooldown = 5f;
    public Transform visuals;

    public KartEntity Kart { get; set; }
    public float DisabledTimer { get; set; }
    
    private bool isDisabled = false;

    private void Start()
    {
        UpdateVisuals();
    }

    private void Update()
    {
        // Handle cooldown timer
        if (isDisabled)
        {
            DisabledTimer -= Time.deltaTime;
            if (DisabledTimer <= 0f)
            {
                isDisabled = false;
                Kart = null;
                UpdateVisuals();
            }
        }
    }

    public bool Collide(KartEntity kart) {
        if (kart != null && !isDisabled) {
            Kart = kart;
            DisabledTimer = cooldown;
            isDisabled = true;
            
            var powerUp = GetRandomPowerup();
            Kart.SetHeldItem(powerUp);
            
            UpdateVisuals();
        }

        return true;
    }

    private void UpdateVisuals() {
        if (visuals != null)
        {
            visuals.gameObject.SetActive(Kart == null);
        }

        if (Kart != null)
        {
            AudioManager.PlayAndFollow(
                Kart.HeldItem != null ? "itemCollectSFX" : "itemWasteSFX",
                transform,
                AudioManager.MixerTarget.SFX
            );

            if (breakParticle != null)
            {
                breakParticle.Play();
            }
        }
    }

    public void OnKartEnter(KartEntity kart)
    {
        Collide(kart);
    }

    private int GetRandomPowerup() {
        var powerUps = ResourceManager.Instance.powerups;
        
        return Random.Range(0, powerUps.Length);
    }
}