using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartEntity : KartComponent
{
	public static event Action<KartEntity> OnKartSpawned;
	public static event Action<KartEntity> OnKartDespawned;

    public event Action<int> OnHeldItemChanged;
    public event Action<int> OnCoinCountChanged;
    
	public KartAnimator Animator { get; private set; }
	public KartCamera Camera { get; private set; }
	public KartController Controller { get; private set; }
	public KartInput Input { get; private set; }
	public KartLapController LapController { get; private set; }
	public KartAudio Audio { get; private set; }
	public GameUI Hud { get; private set; }
	public KartItemController Items { get; private set; }
	public Rigidbody Rigidbody { get; private set; }

	public Powerup HeldItem =>
		HeldItemIndex == -1
			? null
			: ResourceManager.Instance.powerups[HeldItemIndex];

	public int HeldItemIndex { get; set; } = -1;
	public int CoinCount { get; set; }

	public Transform itemDropNode;

    private bool _despawned;
    
    private void OnHeldItemIndexChanged()
	{
		OnHeldItemChanged?.Invoke(HeldItemIndex);

		if (HeldItemIndex != -1)
		{
			foreach (var behaviour in GetComponentsInChildren<KartComponent>())
				behaviour.OnEquipItem(HeldItem, 3f);
		}
	}

	private void OnCoinCountChanged()
	{
		OnCoinCountChanged?.Invoke(CoinCount);
	}

	private void Awake()
	{
		// Set references before initializing all components
		Animator = GetComponentInChildren<KartAnimator>();
		Camera = GetComponent<KartCamera>();
		Controller = GetComponent<KartController>();
		Input = GetComponent<KartInput>();
		LapController = GetComponent<KartLapController>();
		Audio = GetComponentInChildren<KartAudio>();
		Items = GetComponent<KartItemController>();
		Rigidbody = GetComponent<Rigidbody>();

		// Initializes all KartComponents on or under the Kart prefab
		var components = GetComponentsInChildren<KartComponent>();
		foreach (var component in components) component.Init(this);
	}

	public static readonly List<KartEntity> Karts = new List<KartEntity>();

	private void Start()
	{
		// Create HUD for local player
		Hud = Instantiate(ResourceManager.Instance.hudPrefab);
		Hud.Init(this);

		Instantiate(ResourceManager.Instance.nicknameCanvasPrefab);

		Karts.Add(this);
		OnKartSpawned?.Invoke(this);
	}
	
	private void Update()
	{
		// Handle held item changes
		if (HeldItemIndex != -1)
		{
			OnHeldItemIndexChanged();
		}
	}
	
	private void OnDestroy()
	{
		if (!_despawned)
		{
			_despawned = true;
			Karts.Remove(this);
			OnKartDespawned?.Invoke(this);
		}
	}

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("ItemBox")) {
            var itemBox = other.GetComponent<ItemBox>();
            if (itemBox != null) {
                itemBox.OnKartEnter(this);
            }
        }
    }

    public bool SetHeldItem(int index)
	{
		if (index >= ResourceManager.Instance.powerups.Length) return false;
		
		HeldItemIndex = index;
		OnHeldItemIndexChanged();
		return true;
	}

	public void SpinOut()
	{
		StartCoroutine(OnSpinOut());
	}

	private IEnumerator OnSpinOut()
	{
		// Spin out logic here
		yield return new WaitForSeconds(2f);
	}
}