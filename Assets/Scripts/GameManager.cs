using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Continueu, Finish
} 
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState GameState;
    [SerializeField] private int nextSceneIndex;

    private void Awake()
    {
        #region Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        #endregion
    }
    public void RestartLevel()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.buildIndex);
        EventBus.ClearEvents();
    }
    public void LoadNextLevel()
    {
        SceneManager.LoadScene(nextSceneIndex);
        EventBus.ClearEvents();

    }
}
