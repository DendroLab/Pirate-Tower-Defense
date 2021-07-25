using UnityEngine;
using UnityEngine.SceneManagement;

public class ScanesManagement : MonoBehaviour
{

    public Scene[] scenes;

    public int GetLevel()
    {
        int[] currLevel = GameResources.Level;
        int level = 0;

        for (int i = 0; i < scenes.Length; i++)
        {
            if (scenes[i].name == SceneManager.GetActiveScene().name)
            {
                level = i + 1;
                break;
            }
        }

        return level;
    }
}
