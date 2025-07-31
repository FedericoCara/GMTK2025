using System;
using Fusion;
using UnityEngine;

public class KartController : KartComponent
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

	public bool IsBumped => !(Time.time < bumpTimer);
	public bool IsBackfire => !(Time.time < BackfireTimer);
	public bool IsHopping => !(Time.time < HopTimer);
	public bool CanDrive => HasStartedRace && !HasFinishedRace && !IsSpinout && !IsBumped && !IsBackfire;
	public bool HasFinishedRace => Kart.LapController.EndRaceTick != 0;
	public bool HasStartedRace => Kart.LapController.StartRaceTick != 0;
	public float BoostTime => Math.Abs(BoostEndTime - (-1f)) < .01f ? 0f : BoostEndTime - Time.time;
	private float RealSpeed => transform.InverseTransformDirection(Rigidbody.linearVelocity).z;
	public bool IsDrifting => IsDriftingLeft || IsDriftingRight;
	public bool IsBoosting => BoostTierIndex != 0;
	public bool IsOffroad => IsGrounded && GroundResistance >= 0.2f;
	public float DriftStartTime; // reemplaza DriftStartTick

	public float DriftTime => Time.time - DriftStartTime;

	 public float MaxSpeed { get; set; }

	
	private int boostTierIndex;

	public int BoostTierIndex
	{
		get => boostTierIndex;
		set
		{
			if (boostTierIndex != value)
			{
				boostTierIndex = value;
				OnBoostTierIndexChanged?.Invoke(boostTierIndex);
			}
		}
	}

	public float BoostpadCooldown { get; set; }

	
	public int DriftTierIndex { get; set; } = -1;

	 public NetworkBool IsGrounded { get; set; }
	 public float GroundResistance { get; set; }
	public float BoostEndTime = -1f;

	
	public NetworkBool IsSpinout { get; set; }

	 public float TireYaw { get; set; }
	 public RoomPlayer RoomUser { get; set; }
	 public NetworkBool IsDriftingLeft { get; set; }
	 public NetworkBool IsDriftingRight { get; set; }
	 public int DriftStartTick { get; set; }

	public float BackfireTimer { get; set; }

	public float BumpTimer { get; set; }

	public float HopTimer = 1;

	 public float AppliedSpeed { get; set; }

	private KartInput.InputData Inputs;

	public event Action<int> OnDriftTierIndexChanged;
	public event Action<int> OnBoostTierIndexChanged;
	public event Action<bool> OnSpinoutChanged;
	public event Action<bool> OnBumpedChanged;
	public event Action<bool> OnHopChanged;
	public event Action<bool> OnBackfiredChanged;

	 private float SteerAmount { get; set; }
	public float AcceleratePressedTime = -1f;
	 private bool IsAccelerateThisFrame { get; set; }

	private const float TOLERANCE = 0.01f;

	private static void OnIsBackfireChangedCallback(KartController changed) =>
		changed.OnBackfiredChanged?.Invoke(changed.IsBackfire);

	private static void OnIsBumpedChangedCallback(KartController changed) =>
		changed.OnBumpedChanged?.Invoke(changed.IsBumped);

	private static void OnIsHopChangedCallback(KartController changed) =>
		changed.OnHopChanged?.Invoke(changed.IsHopping);

	private static void OnSpinoutChangedCallback(KartController changed) =>
		changed.OnSpinoutChanged?.Invoke(changed.IsSpinout);

	private static void OnDriftTierIndexChangedCallback(KartController changed) =>
		changed.OnDriftTierIndexChanged?.Invoke(changed.DriftTierIndex);

	private static void OnBoostTierIndexChangedCallback(KartController changed) =>
		changed.OnBoostTierIndexChanged?.Invoke(changed.BoostTierIndex);

	private void Awake()
	{
		collider = GetComponent<SphereCollider>();
	}

	public void Start()
	{
		//_changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
		MaxSpeed = maxSpeedNormal;
	}

	private void Update()
	{
		GroundNormalRotation();
		UpdateTireRotation();

		if (CanDrive)
		{
			if (Kart.Input.gamepad != null)
			{
				Kart.Input.gamepad.SetMotorSpeeds(IsOffroad ? AppliedSpeed / MaxSpeed : 0, 0);
			}
		}
		/*
		//ex-render
		
		foreach (var change in _changeDetector.DetectChanges(this))
		{
			switch (change)
			{
				case nameof(BoostTierIndex):
					OnBoostTierIndexChangedCallback(this);
					break;
				case nameof(DriftTierIndex):
					OnDriftTierIndexChangedCallback(this);
					break;
				case nameof(IsSpinout):
					OnSpinoutChangedCallback(this);
					break;
				case nameof(BackfireTimer):
					OnIsBackfireChangedCallback(this);
					break;
				case nameof(BumpTimer):
					OnIsBumpedChangedCallback(this);
					break;
				case nameof(HopTimer):
					OnIsHopChangedCallback(this);
					break;
			}
		}*/
	}

	private void OnCollisionStay(Collision collision)
	{
		//
		// OnCollisionEnter and OnCollisionExit are not reliable when trying to predict collisions, however we can
		// use OnCollisionStay reliably. This means we have to make sure not to run code every frame
		//

		var layer = collision.gameObject.layer;

		// We don't want to run any of this code if we're already in the process of bumping
		if (IsBumped) return;

		if (layer == GameManager.GroundLayer) return;
		if (layer == GameManager.KartLayer && collision.gameObject.TryGetComponent(out KartEntity otherKart))
		{
			//
			// Collision with another kart - if we are going slower than them, then we should bump!  
			//

			if (AppliedSpeed < otherKart.Controller.AppliedSpeed)
			{
				BumpTimer = Time.time + 0.4f;
			}
		}
		else
		{
			//
			// Collision with a wall of some sort - We should get the angle impact and apply a force backwards, only if 
			// we are going above 'speedToDrift' speed.
			//
			if (RealSpeed > speedToDrift)
			{
				var contact = collision.GetContact(0);
				var dot = Mathf.Max(0.25f, Mathf.Abs(Vector3.Dot(contact.normal, Rigidbody.transform.forward)));
				Rigidbody.AddForceAtPosition(contact.normal * AppliedSpeed * dot, contact.point, ForceMode.VelocityChange);

				BumpTimer = Time.time + 0.8f * dot;
			}
		}
	}

	private void FixedUpdate()
	{
		// Leer inputs desde el componente KartInput
		Inputs = Kart.Input.GetInput();

		if (CanDrive)
			Move(Inputs);
		else
			RefreshAppliedSpeed();

		HandleStartRace();
		SpinOut(Inputs);
		Boost(Inputs);
		Drift(Inputs);
		Steer(Inputs);
		UpdateTireYaw(Inputs);
		UseItems(Inputs);
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
		if (!HasStartedRace && Track.Current != null && Track.Current.StartRaceTimer < Time.time)
		{
			var components = GetComponentsInChildren<KartComponent>();
			foreach (var component in components) component.OnRaceStart();
		}
	}

	/// <summary>
	/// Handling spinout at the start of the race. We record the tick that we last pressed the Accelerate button down,
	/// and then calculate how long we have been pressing that button elsewhere.
	/// </summary>
	/// <param name="input"></param>
	private void SpinOut(KartInput.InputData input)
	{
		var isAccelerate = input.IsAccelerate;

		if (isAccelerate && !IsAccelerateThisFrame)
		{
			AcceleratePressedTime = Time.time;
		}

		if (Math.Abs(AcceleratePressedTime - (-1)) > TOLERANCE && !isAccelerate)
		{
			AcceleratePressedTime = -1;
		}

		IsAccelerateThisFrame = isAccelerate;
	}

	private float bumpTimer = -1f;


	public override void OnRaceStart()
	{
		base.OnRaceStart();

		// Siempre autoridad en offline
		AudioManager.PlayMusic(Track.Current.music);

		// Si el botón de acelerar estaba presionado antes de la largada
		if (AcceleratePressedTime != -1f)
		{
			float timeHeld = Time.time - AcceleratePressedTime;

			if (timeHeld < 0.15f)
			{
				GiveBoost(false);
			}
			else if (timeHeld < 0.3f)
			{
				BackfireTimer = Time.time + 0.8f; // dura 0.8 segundos
			}
		}
	}

	private void Move(KartInput.InputData input)
	{
		if (input.IsAccelerate)
		{
			AppliedSpeed = Mathf.Lerp(AppliedSpeed, MaxSpeed, acceleration * Time.fixedDeltaTime);
		}
		else if (input.IsReverse)
		{
			AppliedSpeed = Mathf.Lerp(AppliedSpeed, -reverseSpeed, acceleration * Time.fixedDeltaTime);
		}
		else
		{
			AppliedSpeed = Mathf.Lerp(AppliedSpeed, 0, deceleration * Time.fixedDeltaTime);
		}

		var resistance = 1 - (IsGrounded ? GroundResistance : 0);
		if (resistance < 1)
		{
			AppliedSpeed = Mathf.Lerp(AppliedSpeed, AppliedSpeed * resistance, Time.fixedDeltaTime * (IsDrifting ? 8 : 2));
		}

		// transform.forward is not reliable when using NetworkedRigidbody - instead use: NetworkRigidbody.Rigidbody.rotation * Vector3.forward
		var vel = (Rigidbody.rotation * Vector3.forward) * AppliedSpeed;
		vel.y = Rigidbody.linearVelocity.y;
		Rigidbody.linearVelocity = vel;
	}


	private void Steer(KartInput.InputData input)
	{
		var steerTarget = GetSteerTarget(input);

		if (SteerAmount != steerTarget)
		{
			var steerLerp = Mathf.Abs(SteerAmount) < Mathf.Abs(steerTarget) ? steerAcceleration : steerDeceleration;
			SteerAmount = Mathf.Lerp(SteerAmount, steerTarget, Time.fixedDeltaTime * steerLerp);
		}

		if (IsDrifting)
		{
			model.localEulerAngles = LerpAxis(Axis.Y, model.localEulerAngles, SteerAmount * 2,
				driftRotationLerpFactor * Time.fixedDeltaTime);
		}
		else
		{
			model.localEulerAngles = LerpAxis(Axis.Y, model.localEulerAngles, 0, 6 * Time.fixedDeltaTime);
		}

		if (CanDrive)
		{
			var rot = Quaternion.Euler(
				Vector3.Lerp(
					Rigidbody.rotation.eulerAngles,
					Rigidbody.rotation.eulerAngles + Vector3.up * SteerAmount,
					3 * Time.fixedDeltaTime)
			);

			Rigidbody.MoveRotation(rot);
		}
	}

	private float GetSteerTarget(KartInput.InputData input)
	{
		var steerFactor = steeringCurve.Evaluate(Mathf.Abs(RealSpeed) / maxSpeedNormal) * maxSteerStrength *
		                  Mathf.Sign(RealSpeed);

		if (IsHopping && RealSpeed < speedToDrift)
			return input.Steer * hopSteerStrength;

		if (IsDriftingLeft)
			return Remap(input.Steer, -1, 1, -driftInputRemap.y, -driftInputRemap.x) * maxSteerStrength;
		if (IsDriftingRight)
			return Remap(input.Steer, -1, 1, driftInputRemap.x, driftInputRemap.y) * maxSteerStrength;

		return input.Steer * steerFactor;
	}

	private void Drift(KartInput.InputData input)
	{
		var startDrift = input.IsDriftPressedThisFrame && CanDrive && !IsDrifting;
		if (startDrift && IsGrounded)
		{
			StartDrifting(input);
			DriftStartTime = Time.time;
			HopTimer = Time.time + 0.367f;
		}

		if (IsDrifting)
		{
			if (!input.IsDriftPressed || RealSpeed < speedToDrift)
			{
				StopDrifting();
			}
			else if (IsGrounded)
			{
				EvaluateDrift(DriftTime, out var index);
				if (DriftTierIndex != index) DriftTierIndex = index;
			}
		}
	}

	/// <summary>
	/// Handles when a boost is applied.
	/// </summary>
	/// <param name="input"></param>
	private void Boost(KartInput.InputData input)
	{
		if (BoostTime > 0)
		{
			MaxSpeed = maxSpeedBoosting;
			AppliedSpeed = Mathf.Lerp(AppliedSpeed, MaxSpeed, Time.deltaTime);
		}
		else if (Math.Abs(BoostEndTime - (-1)) > TOLERANCE)
		{
			StopBoosting();
		}
	}

	/// <summary>
	/// This corrects the kart visuals to the ground normal so the edges of the kart dont clip into the floor
	/// </summary>
	private void GroundNormalRotation()
	{
		var wasOffroad = IsOffroad;

		IsGrounded = Physics.SphereCast(collider.transform.TransformPoint(collider.center), collider.radius - 0.1f,
			Vector3.down, out var hit, 0.3f, ~LayerMask.GetMask("Kart"));

		if (IsGrounded)
		{
			Debug.DrawRay(hit.point, hit.normal, Color.magenta);
			GroundResistance = hit.collider.material.dynamicFriction;

			model.transform.rotation = Quaternion.Lerp(
				model.transform.rotation,
				Quaternion.FromToRotation(model.transform.up * 2, hit.normal) * model.transform.rotation,
				7.5f * Time.deltaTime);
		}

		if (wasOffroad != IsOffroad)
		{
			if (IsOffroad)
				Kart.Animator.PlayOffroad();
			else
				Kart.Animator.StopOffroad();
		}
	}

	private void UpdateTireYaw(KartInput.InputData input)
	{
		TireYaw = input.Steer * maxSteerStrength;
	}

	private void UpdateTireRotation()
	{
		tireYawFL.localEulerAngles = LerpAxis(tireYawAxis, tireYawFL.localEulerAngles, TireYaw, 5 * Time.deltaTime);
		tireYawFR.localEulerAngles = LerpAxis(tireYawAxis, tireYawFR.localEulerAngles, TireYaw, 5 * Time.deltaTime);

		if (CanDrive)
		{
			tireFL.Rotate(90 * Time.deltaTime * AppliedSpeed * 0.5f, 0, 0);
			tireFR.Rotate(90 * Time.deltaTime * AppliedSpeed * 0.5f, 0, 0);
			tireBL.Rotate(90 * Time.deltaTime * AppliedSpeed * 0.5f, 0, 0);
			tireBR.Rotate(90 * Time.deltaTime * AppliedSpeed * 0.5f, 0, 0);
		}
	}

	// One-Shot Functions

	private void StartDrifting(KartInput.InputData input)
	{
		if (AppliedSpeed < speedToDrift || input.Steer == 0)
		{
			StopDrifting();
			return;
		}

		IsDriftingRight = input.Steer > 0f;
		IsDriftingLeft = input.Steer < 0f;
	}

	private void StopDrifting()
	{
		BoostTierIndex = DriftTierIndex == -1 ? 0 : DriftTierIndex;
		BoostEndTime = BoostTierIndex == 0
			? -1
			: Time.time +
			  (int) (driftTiers[BoostTierIndex].boostDuration / Time.fixedDeltaTime);

		if (BoostTime <= 0) StopBoosting();

		DriftStartTick = -1;
		DriftTierIndex = -1;
		IsDriftingLeft = false;
		IsDriftingRight = false;
	}

	private void StopBoosting()
	{
		BoostTierIndex = 0;
		BoostEndTime = -1;
		MaxSpeed = maxSpeedNormal;
	}

	public void GiveBoost(bool isBoostpad, int tier = 1)
	{
		if (isBoostpad)
		{
			//
			// If we are given a boost from a boostpad, we need to add a cooldown to ensure that we dont get a boost
			// every frame we are in contact with the boost pad.
			// 
			if (BoostpadCooldown >= Time.time)
				return;

			BoostpadCooldown = Time.time + 4f;
		}

		// set the boost tier to 'tier' only if it's a higher tier than current
		BoostTierIndex = BoostTierIndex > tier ? BoostTierIndex : tier;

		if (BoostEndTime < 0f) BoostEndTime = Time.time;
		BoostEndTime += driftTiers[tier].boostDuration;
	}

	public void RefreshAppliedSpeed()
	{
		AppliedSpeed = transform.InverseTransformDirection(Rigidbody.linearVelocity).z;
	}

	// Utility functions

	private static Vector3 LerpAxis(Axis axis, Vector3 euler, float tgtVal, float t)
	{
		if (axis == Axis.X) return new Vector3(Mathf.LerpAngle(euler.x, tgtVal, t), euler.y, euler.z);
		if (axis == Axis.Y) return new Vector3(euler.x, Mathf.LerpAngle(euler.y, tgtVal, t), euler.z);
		return new Vector3(euler.x, euler.y, Mathf.LerpAngle(euler.z, tgtVal, t));
	}

	private static float Remap(float value, float srcMin, float srcMax, float destMin, float destMax, bool clamp = false)
	{
		if (clamp) value = Mathf.Clamp(value, srcMin, srcMax);
		return (value - srcMin) / (srcMax - srcMin) * (destMax - destMin) + destMin;
	}

	public DriftTier EvaluateDrift(float driftDuration, out int index)
	{
		var i = 0;
		var tier = driftTiers[0];
		while (i < driftTiers.Length)
		{
			if (driftDuration < tier.startTime)
			{
				tier = driftTiers[--i];
				break;
			}

			if (i < driftTiers.Length - 1)
				tier = driftTiers[++i];
			else
				break;
		}

		index = i;
		return tier;
	}

	public void ResetControllerState()
	{
		Rigidbody.linearVelocity = Vector3.zero;
		AppliedSpeed = 0;
		BoostEndTime = -1;
		BoostTierIndex = 0;
		transform.up = Vector3.up;
		model.transform.up = Vector3.up;
	}

	// type definitions

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

public class CustomInputData
{
	public bool IsAccelerating() => Input.GetKeyDown(KeyCode.W);
	public bool StartedDrifting => Input.GetKey(KeyCode.Space);
}