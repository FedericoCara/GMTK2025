using UnityEngine;
using UnityEngine.SceneManagement;

public class OfflineGameLauncher : MonoBehaviour
{
    [SerializeField] private string lobbySceneName = "Lobby";
    [SerializeField] private string gameSceneName = "Track01";
    
    public void StartOfflineGame()
    {
        // Set default game settings for offline mode
        if (ServerInfo.Instance != null)
        {
            ServerInfo.Instance.LobbyName = "Offline Game";
            ServerInfo.Instance.TrackId = 0;
            ServerInfo.Instance.GameMode = 0;
            ServerInfo.Instance.MaxUsers = 1;
        }
        
        // Load the game scene
        SceneManager.LoadScene(gameSceneName);
    }
    
    public void ReturnToLobby()
    {
        SceneManager.LoadScene(lobbySceneName);
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