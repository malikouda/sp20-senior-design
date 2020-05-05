using UnityEngine;
using System.Collections;

public class TextPopup : MonoBehaviour {

    public GameObject popup;

    public Player player;

    public float length;

    private bool active = false;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Return) && active) {
            popup.SetActive(false);
            player.active = true;
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player") {
            StartCoroutine(Popup(length));
        }
    }

    private IEnumerator Popup(float length) {
        active = true;
        popup.SetActive(true);
        player.active = false;
        yield return new WaitForSecondsRealtime(length);
        popup.SetActive(false);
        player.active = true;
        active = false;
        Destroy(this.gameObject);
    }
}
