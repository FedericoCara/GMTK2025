using UnityEngine;

public class Track : MonoBehaviour, ICameraController
{
	public static Track Current { get; private set; }

	public float StartRaceTimer { get; set; }

	public CameraTrack[] introTracks;
	public Checkpoint[] checkpoints;
	public Transform[] spawnpoints;
	public FinishLine finishLine;
	public GameObject itemContainer;
	public GameObject coinContainer;

	public TrackDefinition definition;
	public TrackStartSequence sequence;

	public string music = "";
	public float introSpeed = 0.5f;

	private int _currentIntroTrack;
	private float _introIntervalProgress;

	private void Awake()
	{
		Current = this;
		InitCheckpoints();

		if (GameManager.Instance.GameType.hasPickups == false)
		{
			itemContainer.SetActive(false);
			coinContainer.SetActive(false);
		}

		// Initialize cutscene
		AudioManager.StopMusic();

		GameManager.SetTrack(this);
		GameManager.Instance.camera = Camera.main;
		StartIntro();
	}

	private void Start()
	{
		// Set race timer for offline mode
		StartRaceTimer = sequence.duration + 4f;
		sequence.StartSequence();
	}

	private void OnDestroy()
	{
		GameManager.SetTrack(null);
	}

	public void SpawnPlayer()
	{
		// For offline mode, spawn a single player
		var point = spawnpoints[0];

		var prefabId = 0; // Default kart
		var prefab = ResourceManager.Instance.kartDefinitions[prefabId].prefab;

		// Spawn player
		var entity = Instantiate(prefab, point.position, point.rotation);
		var kartController = entity.GetComponent<KartController>();
		
		// Set up local player
		kartController.RoomUser = null; // No room user in offline mode
		//kartController.GameState = RoomPlayer.EGameState.GameCutscene;

		Debug.Log($"Spawning kart for offline player as {entity.name}");
		entity.transform.name = $"Kart (Offline Player)";
	}

	private void InitCheckpoints()
	{
		for (int i = 0; i < checkpoints.Length; i++)
		{
			checkpoints[i].index = i;
		}
	}

	public bool ControlCamera(Camera cam)
	{
		cam.transform.position = Vector3.Lerp(
			introTracks[_currentIntroTrack].startPoint.position,
			introTracks[_currentIntroTrack].endPoint.position,
			_introIntervalProgress);

		cam.transform.LookAt(introTracks[_currentIntroTrack].transform);

		_introIntervalProgress += Time.deltaTime * introSpeed;

		if (_introIntervalProgress >= 1f)
		{
			_introIntervalProgress = 0f;
			_currentIntroTrack++;

			if (_currentIntroTrack >= introTracks.Length)
			{
				return false;
			}
		}

		return true;
	}

	public void StartIntro()
	{
		_currentIntroTrack = 0;
		_introIntervalProgress = 0f;
	}
}