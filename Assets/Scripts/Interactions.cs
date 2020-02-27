using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactions : MonoBehaviour
{

    public GameObject astroButton;
    public GameObject rocketButton;
    public GameObject consoleButton;
    Animator anim;

    private bool facingRight;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        facingRight = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow)) // checks if player presses left arrow key
        {
            transform.Translate(-4 * Time.deltaTime, 0, 0); // moves player to the left
            anim.SetBool("isWalking", true);    
        }
        else if (Input.GetKey(KeyCode.RightArrow)) // checks if oplayer presses right arrow key
        {
            transform.Translate(4 * Time.deltaTime, 0, 0); // moves player to the right
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
            //anim.SetBool("idle", true);
        }
        float horizontal = Input.GetAxis("Horizontal");
        Flip(horizontal);
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

    private void Flip(float horizontal){
        if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight){
            facingRight = !facingRight;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;

        }
    }
}
