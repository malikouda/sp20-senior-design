using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryTransition : MonoBehaviour {

    public float length = 10;

    GameManager gm;

    private void Start() {
        gm = FindObjectOfType<GameManager>();
        if (gm) {
            StartCoroutine(WaitTime(length));
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Return)) {
            if (gm) {
                gm.nextLevel = true;
            }
        }
    }

    public IEnumerator WaitTime(float length) {
        yield return new WaitForSecondsRealtime(length);
        gm.nextLevel = true;
    }
}
