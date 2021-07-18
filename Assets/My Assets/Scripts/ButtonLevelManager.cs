
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonLevelManager : MonoBehaviour {
    public void ButtonMoveLevel(string level) {
        //Debug.Log(level);
        if (level == "0") {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        } else {
            SceneManager.LoadScene(level);
        }
    }
}