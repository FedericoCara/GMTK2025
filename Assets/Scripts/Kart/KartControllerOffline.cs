using System;
using UnityEngine;

public class KartControllerOffline : KartComponent
{
	public new SphereCollider collider;
	public DriftTier[] driftTiers;
	[SerializeField] private Axis tireYawAxis = Axis.Y;

	public Transform model;
	public Transform tireFL, tireFR, tireYawFL, tireYawFR, tireBL, tireBR;

	public float maxSpeedNormal;
	public float maxSpeedBoosting;
	public float reverseSpeed;
	public float acceleration;
	public float deceleration;

	[Tooltip("X-Axis: steering\nY-Axis: velocity\nCoordinate space is normalized")]
	public AnimationCurve steeringCurve = AnimationCurve.Linear(0, 0, 1, 1);

	public float maxSteerStrength = 35;
	public float steerAcceleration;
	public float steerDeceleration;
	public Vector2 driftInputRemap = new Vector2(0.5f, 1f);
	public float hopSteerStrength;
	public float speedToDrift;
	public float driftRotationLerpFactor = 10f;

	public Rigidbody Rigidbody;

	public bool IsBumped => Time.time < bumpEndTime;
	public bool IsBackfire => Time.time < backfireEndTime;
	public bool IsHopping => Time.time < hopEndTime;
	public bool CanDrive => HasStartedRace && !HasFinishedRace && !IsSpinout && !IsBumped && !IsBackfire;
	public bool HasFinishedRace => Kart.LapController.EndRaceTick != 0;
	public bool HasStartedRace => Kart.LapController.StartRaceTick != 0;
	public float BoostTime => boostEndTime == -1 ? 0f : boostEndTime - Time.time;
	private float RealSpeed => transform.InverseTransformDirection(Rigidbody.velocity).z;
	public bool IsDrifting => IsDriftingLeft || IsDriftingRight;
	public bool IsBoosting => BoostTierIndex != 0;
	public bool IsOffroad => IsGrounded && GroundResistance >= 0.2f;
	public float DriftTime => Time.time - driftStartTime;

	public float MaxSpeed { get; set; }
	public int BoostTierIndex { get; set; }
	public int DriftTierIndex { get; set; } = -1;
	public bool IsGrounded { get; set; }
	public float GroundResistance { get; set; }
	public bool IsSpinout { get; set; }
	public float TireYaw { get; set; }
	public RoomPlayer RoomUser { get; set; }
	public bool IsDriftingLeft { get; set; }
	public bool IsDriftingRight { get; set; }
	public float AppliedSpeed { get; set; }

	private float boostEndTime = -1f;
	private float backfireEndTime = -1f;
	private float bumpEndTime = -1f;
	private float hopEndTime = -1f;
	private float driftStartTime = -1f;

	private float steerAmount;
	private bool isAccelerateThisFrame;

	public event Action<int> OnDriftTierIndexChanged;
	public event Action<int> OnBoostTierIndexChanged;
	public event Action<bool> OnSpinoutChanged;
	public event Action<bool> OnBumpedChanged;
	public event Action<bool> OnHopChanged;
	public event Action<bool> OnBackfiredChanged;

	private void Awake()
	{
		Rigidbody = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		// Handle input and movement
		var input = Kart.Input.GetInput();
		HandleInput(input);
	}

	private void FixedUpdate()
	{
		// Physics-based movement
		var input = Kart.Input.GetInput();
		Move(input);
		Steer(input);
		Drift(input);
		Boost(input);
		GroundNormalRotation();
		UpdateTireYaw(input);
		UpdateTireRotation();
	}

	private void HandleInput(KartInput.InputData input)
	{
		UseItems(input);
		HandleStartRace();
		SpinOut(input);
	}

	private void UseItems(KartInput.InputData inputs)
	{
		if (inputs.IsDownThisFrame(KartInput.InputData.UseItem))
		{
			Kart.Items.UseItem();
		}
	}

	private void HandleStartRace()
	{
		if (HasStartedRace == false && Track.Current != null)
		{
			OnRaceStart();
		}
	}

	private void SpinOut(KartInput.InputData input)
	{
		if (input.IsDownThisFrame(KartInput.InputData.ButtonDrift) && input.IsDown(KartInput.InputData.ButtonAccelerate))
		{
			IsSpinout = true;
			OnSpinoutChanged?.Invoke(true);
		}
	}

	public override void OnRaceStart()
	{
		base.OnRaceStart();
		HasStartedRace = true;
	}

	private void Move(KartInput.InputData input)
	{
		if (!CanDrive) return;

		float targetSpeed = 0f;

		if (input.IsAccelerate)
		{
			targetSpeed = IsBoosting ? maxSpeedBoosting : maxSpeedNormal;
		}
		else if (input.IsReverse)
		{
			targetSpeed = -reverseSpeed;
		}

		AppliedSpeed = Mathf.MoveTowards(AppliedSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);

		Vector3 velocity = transform.forward * AppliedSpeed;
		Rigidbody.velocity = velocity;
	}

	private void Steer(KartInput.InputData input)
	{
		if (!CanDrive) return;

		float targetSteer = GetSteerTarget(input);
		steerAmount = Mathf.MoveTowards(steerAmount, targetSteer, steerAcceleration * Time.fixedDeltaTime);

		transform.Rotate(0, steerAmount * Time.fixedDeltaTime, 0);
	}

	private float GetSteerTarget(KartInput.InputData input)
	{
		float steerInput = input.Steer;
		float speedFactor = Mathf.Abs(RealSpeed) / maxSpeedNormal;
		float curveValue = steeringCurve.Evaluate(speedFactor);
		return steerInput * maxSteerStrength * curveValue;
	}

	private void Drift(KartInput.InputData input)
	{
		if (!CanDrive) return;

		if (input.IsDriftPressed && Mathf.Abs(RealSpeed) > speedToDrift)
		{
			StartDrifting(input);
		}
		else
		{
			StopDrifting();
		}
	}

	private void Boost(KartInput.InputData input)
	{
		if (IsBoosting && Time.time >= boostEndTime)
		{
			StopBoosting();
		}
	}

	private void GroundNormalRotation()
	{
		// Simplified ground normal rotation for offline mode
		RaycastHit hit;
		if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f))
		{
			IsGrounded = true;
			GroundResistance = 0f;
		}
		else
		{
			IsGrounded = false;
			GroundResistance = 0f;
		}
	}

	private void UpdateTireYaw(KartInput.InputData input)
	{
		TireYaw = Mathf.Lerp(TireYaw, steerAmount, Time.fixedDeltaTime * 10f);
	}

	private void UpdateTireRotation()
	{
		if (tireYawFL != null) tireYawFL.localRotation = Quaternion.Euler(0, TireYaw, 0);
		if (tireYawFR != null) tireYawFR.localRotation = Quaternion.Euler(0, TireYaw, 0);
	}

	private void StartDrifting(KartInput.InputData input)
	{
		if (!IsDrifting)
		{
			IsDriftingLeft = input.Steer < 0;
			IsDriftingRight = input.Steer > 0;
			driftStartTime = Time.time;
			DriftTierIndex = 0;
		}
	}

	private void StopDrifting()
	{
		if (IsDrifting)
		{
			IsDriftingLeft = false;
			IsDriftingRight = false;
			DriftTierIndex = -1;
		}
	}

	private void StopBoosting()
	{
		BoostTierIndex = 0;
		boostEndTime = -1f;
		OnBoostTierIndexChanged?.Invoke(0);
	}

	public void GiveBoost(bool isBoostpad, int tier = 1)
	{
		BoostTierIndex = tier;
		boostEndTime = Time.time + 3f; // 3 second boost
		OnBoostTierIndexChanged?.Invoke(tier);
	}

	public void ResetControllerState()
	{
		IsSpinout = false;
		IsBumped = false;
		IsBackfire = false;
		IsHopping = false;
		IsDriftingLeft = false;
		IsDriftingRight = false;
		BoostTierIndex = 0;
		DriftTierIndex = -1;
	}

	public enum Axis
	{
		X,
		Y,
		Z
	}

	[Serializable]
	public struct DriftTier
	{
		public Color color;
		public float boostDuration;
		public float startTime;
	}
} 