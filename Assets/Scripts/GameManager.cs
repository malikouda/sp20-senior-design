using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private int currentLevel = 0;

    public int numLevels;

    public void Start() {
        DontDestroyOnLoad(gameObject);
    }

    public void ResetLevel() {
        SceneManager.LoadScene(currentLevel);
    }

    public void NextLevel() {
        if (currentLevel >= numLevels - 1) {
            currentLevel = 0;
        } else {
            currentLevel++;
        }

        SceneManager.LoadScene(currentLevel);
    }
}
