using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScanesManagement : MonoBehaviour{

    public Scene[] scenes;

    Entity entity;

    public int GetLevel() {
        int[] currLevel = entity.Level;
        int level = 0;

        for (int i = 0; i < scenes.Length; i++) {
            if (scenes[i].name == SceneManager.GetActiveScene().name) {
                level = i+1;
                break;
            }
        }

        return level;
    }

    // Start is called before the first frame update
    void Start(){
        entity = new Entity();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
