using UnityEngine;
using UnityEngine.SceneManagement;

public class OfflineGameManager : MonoBehaviour
{
    [SerializeField] private GameObject gameManagerPrefab;
    [SerializeField] private GameObject audioManagerPrefab;
    [SerializeField] private OfflinePlayerSpawner playerSpawner;
    
    private void Start()
    {
        InitializeOfflineGame();
    }
    
    private void InitializeOfflineGame()
    {
        // Create GameManager if it doesn't exist
        if (GameManager.Instance == null)
        {
            if (gameManagerPrefab != null)
            {
                Instantiate(gameManagerPrefab);
            }
            else
            {
                var gameManager = new GameObject("GameManager");
                gameManager.AddComponent<GameManager>();
            }
        }
        
        // Create AudioManager if it doesn't exist
        if (AudioManager.Instance == null)
        {
            if (audioManagerPrefab != null)
            {
                Instantiate(audioManagerPrefab);
            }
        }
        
        // Set up offline mode settings
        SetupOfflineMode();
        
        // Spawn player if spawner is available
        if (playerSpawner != null)
        {
            // Player spawner will handle spawning in its Start method
        }
        else
        {
            // Try to find spawner in scene
            playerSpawner = FindObjectOfType<OfflinePlayerSpawner>();
        }
        
        Debug.Log("Offline game initialized successfully!");
    }
    
    private void SetupOfflineMode()
    {
        // Set default game settings for offline mode
        if (ServerInfo.Instance != null)
        {
            ServerInfo.Instance.LobbyName = "Offline Game";
            ServerInfo.Instance.TrackId = 0;
            ServerInfo.Instance.GameMode = 0;
            ServerInfo.Instance.MaxUsers = 1;
        }
        
        // Disable networking-related components
        DisableNetworkingComponents();
    }
    
    private void DisableNetworkingComponents()
    {
        // Find and disable NetworkRunner components
        var networkRunners = FindObjectsOfType<NetworkRunner>();
        foreach (var runner in networkRunners)
        {
            runner.enabled = false;
        }
        
        // Find and disable other networking components
        var networkObjects = FindObjectsOfType<NetworkObject>();
        foreach (var networkObject in networkObjects)
        {
            networkObject.enabled = false;
        }
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Launch");
    }
    
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
} 