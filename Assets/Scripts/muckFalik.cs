using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class muckFalik : MonoBehaviour
{

    public GameObject astroButton;
    public GameObject rocketButton;
    public GameObject consoleButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow)) // checks if player presses left arrow key
        {
            transform.Translate(-8 * Time.deltaTime, 0, 0); // moves player to the left
        }
        if (Input.GetKey(KeyCode.RightArrow)) // checks if oplayer presses right arrow key
        {
            transform.Translate(8 * Time.deltaTime, 0, 0); // moves player to the right
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "astroSuit"){
            astroButton.SetActive(true);
            //possibly add other text such as "the suits are deflating"
        }

        if (other.tag == "rocketPanel"){
            rocketButton.SetActive(true);
            //possibly add other text such as "electricity flickers"
        }

        if (other.tag == "consoleWiring"){
            consoleButton.SetActive(true);
            //possibly add other text such as "electricity flickers"
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "astroSuit"){
            astroButton.SetActive(false);
        }

        if (other.tag == "rocketPanel"){
            rocketButton.SetActive(false);
        }

        if (other.tag == "consoleWiring"){
            consoleButton.SetActive(false);
        }
    }

    //all lead to respective minigame scene
    public void SuitButton(){
        Debug.Log("Astrosuits");
    }

    public void ShipButton(){
        Debug.Log("Rocketship");
    }

    public void WiringButton(){
        Debug.Log("Consolewiring");
    }
}
