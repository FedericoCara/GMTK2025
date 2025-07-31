using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KartInput : KartComponent
{
	public struct InputData
	{
		public const uint ButtonAccelerate = 1 << 0;
		public const uint ButtonReverse = 1 << 1;
		public const uint ButtonDrift = 1 << 2;
		public const uint ButtonLookbehind = 1 << 3;
        public const uint UseItem = 1 << 4;

		public uint Buttons;
		public uint OneShots;

		private int _steer;
		public float Steer
		{
			get => _steer * .001f;
			set => _steer = (int)(value * 1000);
		}

		public bool IsUp(uint button) => IsDown(button) == false;
		public bool IsDown(uint button) => (Buttons & button) == button;

		public bool IsDownThisFrame(uint button) => (OneShots & button) == button;
        
		public bool IsAccelerate => IsDown(ButtonAccelerate);
		public bool IsReverse => IsDown(ButtonReverse);
		public bool IsDriftPressed => IsDown(ButtonDrift);
		public bool IsDriftPressedThisFrame => IsDownThisFrame(ButtonDrift);
	}

	public Gamepad gamepad;

	[SerializeField] private InputAction accelerate;
	[SerializeField] private InputAction reverse;
	[SerializeField] private InputAction drift;
	[SerializeField] private InputAction steer;
	[SerializeField] private InputAction lookBehind;
	[SerializeField] private InputAction useItem;
	[SerializeField] private InputAction pause;

    private bool _useItemPressed;
	private bool _driftPressed;
	private InputData _currentInput;

	public override void Init(KartEntity kart)
	{
		base.Init(kart);

		accelerate = accelerate.Clone();
		reverse = reverse.Clone();
		drift = drift.Clone();
		steer = steer.Clone();
		lookBehind = lookBehind.Clone();
		useItem = useItem.Clone();
		pause = pause.Clone();

		accelerate.Enable();
		reverse.Enable();
		drift.Enable();
		steer.Enable();
		lookBehind.Enable();
		useItem.Enable();
		pause.Enable();
		
		useItem.started += UseItemPressed;
		drift.started += DriftPressed;
		pause.started += PausePressed;
	}

	private void OnDestroy()
	{
		DisposeInputs();
	}

    private void DisposeInputs()
	{
		accelerate.Dispose();
		reverse.Dispose();
		drift.Dispose();
		steer.Dispose();
		lookBehind.Dispose();
		useItem.Dispose();
		pause.Dispose();
	}

    private void UseItemPressed(InputAction.CallbackContext ctx) => _useItemPressed = true;
    private void DriftPressed(InputAction.CallbackContext ctx) => _driftPressed = true;

    private void PausePressed(InputAction.CallbackContext ctx)
	{
		// Handle pause functionality
		Debug.Log("Pause pressed");
	}

	public bool IsAcceleratePressed => ReadBool(accelerate);
	public bool IsReversePressed => ReadBool(reverse);
	public bool IsDriftPressed => ReadBool(drift);
	public float SteerInput => ReadFloat(steer);
	public bool IsLookBehindPressed => ReadBool(lookBehind);

	private static bool ReadBool(InputAction action) => action.ReadValue<float>() != 0;
	private static float ReadFloat(InputAction action) => action.ReadValue<float>();

    private void Update()
    {
        // Update input data for offline mode
        _currentInput.Buttons = 0;
        _currentInput.OneShots = 0;

        if (IsAcceleratePressed) _currentInput.Buttons |= InputData.ButtonAccelerate;
        if (IsReversePressed) _currentInput.Buttons |= InputData.ButtonReverse;
        if (IsDriftPressed) _currentInput.Buttons |= InputData.ButtonDrift;
        if (IsLookBehindPressed) _currentInput.Buttons |= InputData.ButtonLookbehind;
        if (_useItemPressed) _currentInput.Buttons |= InputData.UseItem;

        _currentInput.Steer = SteerInput;

        // Reset one-shot inputs
        if (_useItemPressed) _useItemPressed = false;
        if (_driftPressed) _driftPressed = false;
    }

    public InputData GetInput() => _currentInput;
}