using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryTransition : MonoBehaviour {

    public float length = 10;

    GameManager gm;

    public void Start() {
        gm = FindObjectOfType<GameManager>();
        if (gm) {
            StartCoroutine(WaitTime(length));
        }
    }

    public IEnumerator WaitTime(float length) {
        yield return new WaitForSecondsRealtime(length);
        gm.NextLevel();
    }
}
