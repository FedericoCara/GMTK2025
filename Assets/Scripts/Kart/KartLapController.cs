using System;
using UnityEngine;

public class KartLapController : KartComponent
{
    public static event Action<KartLapController> OnRaceCompleted;

    public int Lap { get; private set; } = 1;

    public float[] LapTimes = new float[5]; // tamaño fijo

    public float StartRaceTime { get; private set; }

    public float EndRaceTime { get; private set; }

    private int CheckpointIndex = -1;

    public event Action<int, int> OnLapChanged;

    public bool HasFinished => EndRaceTime != 0f;

    private KartController Controller => Kart.Controller;
    private GameUI Hud => Kart.Hud;

    private Rigidbody _rb;

    private bool _raceStarted;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public override void OnRaceStart()
    {
        base.OnRaceStart();
        StartRaceTime = Time.time;
        _raceStarted = true;
    }

    public override void OnLapCompleted(int lap, bool isFinish)
    {
        base.OnLapCompleted(lap, isFinish);

        if (isFinish)
        {
            // Asumo que el jugador siempre tiene autoridad en offline
            AudioManager.Play("raceFinishedSFX", AudioManager.MixerTarget.SFX);
            Hud.ShowEndRaceScreen();

            Controller.RoomUser.HasFinished = true; // adaptá si no usás RoomUser

            EndRaceTime = Time.time;
        }
        else
        {
            AudioManager.Play("newLapSFX", AudioManager.MixerTarget.SFX);
        }

        OnRaceCompleted?.Invoke(this);
    }

    public void ResetToCheckpoint()
    {
        Transform tgt = CheckpointIndex == -1
            ? GameManager.CurrentTrack.finishLine.transform
            : GameManager.CurrentTrack.checkpoints[CheckpointIndex].transform;

        _rb.position = tgt.position;
        _rb.rotation = tgt.rotation;
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        Controller.ResetControllerState();
    }

    private void SetLap(int newLap)
    {
        int maxLaps = GameManager.Instance.GameType.lapCount;
        bool isPracticeMode = GameManager.Instance.GameType.IsPracticeMode();
        bool isFinish = !isPracticeMode && newLap - 1 == maxLaps;

        Lap = newLap;

        var behaviours = GetComponentsInChildren<KartComponent>();
        foreach (var b in behaviours)
            b.OnLapCompleted(newLap, isFinish);

        OnLapChanged?.Invoke(newLap, maxLaps);
    }

    public void ProcessCheckpoint(Checkpoint checkpoint)
    {
        if (GameManager.Instance.GameType.IsPracticeMode())
        {
            CheckpointIndex = checkpoint.index;
            return;
        }

        if (CheckpointIndex == checkpoint.index - 1)
        {
            CheckpointIndex++;
        }
    }

    public void ProcessFinishLine(FinishLine finishLine)
    {
        if (GameManager.Instance.GameType.IsPracticeMode())
        {
            CheckpointIndex = -1;
            return;
        }

        var checkpoints = GameManager.CurrentTrack.checkpoints;

        if (CheckpointIndex == checkpoints.Length - 1 || finishLine.debug)
        {
            if (!_raceStarted) return;

            LapTimes[Lap - 1] = Time.time;

            SetLap(Lap + 1);
            CheckpointIndex = -1;
        }
    }

    public float GetTotalRaceTime()
    {
        if (!_raceStarted || StartRaceTime == 0f)
            return 0f;

        float endTime = EndRaceTime == 0f ? Time.time : EndRaceTime;
        return endTime - StartRaceTime;
    }
}
