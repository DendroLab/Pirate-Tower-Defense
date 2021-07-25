using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("PilihMenu", LoadSceneMode.Single);
    }

    public void pilihLevel1()
    {
        SceneManager.LoadScene("Level 1", LoadSceneMode.Single);
    }

    public void pilihLevel2()
    {
        SceneManager.LoadScene("Level 2", LoadSceneMode.Single);
    }

    public void pilihLevel3()
    {
        SceneManager.LoadScene("Level 3", LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }
}
