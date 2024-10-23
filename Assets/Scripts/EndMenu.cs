using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndMenu : MonoBehaviour
{
    public GameObject EndPanel;
    public TextMeshProUGUI EndText;

    public void Active()
    {
        EndPanel.SetActive(true);
    }

    public void MainMenu()
    {
        EndPanel.SetActive(false);
        SceneManager.LoadScene("MainMenu");
        FindFirstObjectByType<Game>().ReloadGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}