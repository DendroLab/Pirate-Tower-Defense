using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PilihMenu : MonoBehaviour
{
    public Button[] _btnLevel;

    public void Start()
    {
        int[] arrLevel = GameResources.Level;

        if (arrLevel.Length == 0)
        {
            arrLevel = new int[1];
            arrLevel[0] = 1;
            GameResources.Level = arrLevel;
        }
        int level = arrLevel[arrLevel.Length - 1];
        Debug.Log(level);

        if (level > 0)
        {
            for (int i = 0; i < level; i++)
            {
                _btnLevel[i].gameObject.SetActive(true);
            }
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }
}
