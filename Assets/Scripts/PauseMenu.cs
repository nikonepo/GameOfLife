using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject PausePanel;

    public bool IsPaused = false;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
            {
                Unpause();
            }
            else
            {
                Pause();
            }
        }
    }
    
    public void Pause()
    {
        IsPaused = true;
        PausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Unpause()
    {
        IsPaused = false;
        PausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void MainMenu()
    {
        Unpause();
        SceneManager.LoadScene("MainMenu");
        FindFirstObjectByType<Game>().ReloadGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
