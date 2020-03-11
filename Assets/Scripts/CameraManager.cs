using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    public GameObject initialCamera;
    [HideInInspector]
    public bool transition = false;
    [HideInInspector]
    public Vector3 newPos;
    [HideInInspector]
    public GameObject mainCamera;

    void Start() {
        mainCamera = Camera.main.gameObject;
        mainCamera.transform.position = initialCamera.transform.position;
    }

    private void Update() {
        if (mainCamera.transform.position == newPos) {
            transition = false;
        }

        if (transition) {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, newPos, 5f * Time.deltaTime);
        }
    }
}
