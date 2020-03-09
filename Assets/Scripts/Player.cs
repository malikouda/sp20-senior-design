using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour {

    #region private

    private Controller2D controller;

    private float gravity;
    private float jumpVelocity;
    private Vector2 velocity;
    private float velocityXSmoothing;
    private Animator anim;
    private float scaleX;
    private bool teleportedA;
    private bool teleportedB;
    private bool inA;
    private bool inB;

    #endregion

    #region public

    [Range(1, 10)]
    public float jumpHeight = 4;
    [Range(0.1f, 2f)]
    public float timeToJumpApex = .4f;
    [Range(2, 15)]
    public float moveSpeed = 6;
    [Range(0f, 0.5f)]
    public float accelerationTimeAirborne = .2f;
    [Range(0f, 0.5f)]
    public float accelerationTimeGrounded = .1f;
    public GameObject portalA;
    public GameObject portalB;

    #endregion

    private Vector2 input {
        get {
            return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
    }

    private void Start() {
        controller = GetComponent<Controller2D>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity * timeToJumpApex);

        anim = GetComponent<Animator>();
        scaleX = transform.localScale.x;

    }

    private void Update() {

        if (controller.collisions.above || controller.collisions.below) {
            velocity.y = 0;
            anim.SetBool("isJumping", false);
        }

        if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below) {
            velocity.y = jumpVelocity;
            anim.SetBool("isJumping", true);
        }

        if (teleportedA) {
            transform.position = portalB.transform.position;
            teleportedA = false;

            if (!controller.collisions.below) {
                velocity.y *= -1;
            }
        }

        if (teleportedB) {
            transform.position = portalA.transform.position;
            teleportedB = false;

            if (!controller.collisions.below) {
                velocity.y *= -1;
            }
        }

        float targetVelocityX = input.x * moveSpeed;

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (input.x != 0) {
            Vector3 newScale = new Vector3(Mathf.Sign(velocity.x) * scaleX, transform.localScale.y, transform.localScale.z);
            transform.localScale = newScale;

            if (controller.collisions.below) {
                anim.SetFloat("speed", Mathf.Clamp(Mathf.Abs(velocity.x * input.x), 0, 1f));
                anim.SetBool("isWalking", true);
            }
        } else {
            anim.SetBool("isWalking", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "portal") {
            if (collision.gameObject == portalA && !inA) {
                teleportedA = true;
                inB = true;
            } else if (collision.gameObject == portalB && !inB) {
                teleportedB = true;
                inA = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.tag == "portal") {
            if (collision.gameObject == portalA) {
                inA = false;
            } else if (collision.gameObject == portalB) {
                inB = false;
            }
        }
    }

}
