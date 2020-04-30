using UnityEngine;
using UnityEngine.UI;

public class TextPopup : MonoBehaviour {

    public GameObject text;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player") {
            text.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.tag == "Player") {
            text.SetActive(false);
        }
    }
}
