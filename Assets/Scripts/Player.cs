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

        float targetVelocityX = input.x * moveSpeed;

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (input.x != 0 && controller.collisions.below) {
            Vector3 newScale = new Vector3(Mathf.Sign(velocity.x) * scaleX, transform.localScale.y, transform.localScale.z);
            transform.localScale = newScale;
            anim.SetFloat("speed", Mathf.Clamp(Mathf.Abs(velocity.x * input.x), 0, 1f));
            anim.SetBool("isWalking", true);
        } else {
            anim.SetBool("isWalking", false);
        }
    }

}
