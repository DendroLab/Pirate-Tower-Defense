using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonLevelManager : MonoBehaviour{
    public void ButtonMoveLevel(string level) {
        SceneManager.LoadScene(level);
    }
}
