using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour {

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
    private bool shotA = false;
    private bool portalsActive = false;
    private LineRenderer lr;

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
    public LayerMask collisionMask;
    public Material lineMaterial;

    [HideInInspector]
    public Vector2 input {
        get {
            return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
    }

    private void Start() {
        controller = GetComponent<Controller2D>();
        lr = GetComponent<LineRenderer>();
        lr.material = lineMaterial;
        lr.startWidth = 0.05f;

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity * timeToJumpApex);

        anim = GetComponent<Animator>();
        scaleX = transform.localScale.x;

        portalA = Instantiate(portalA);
        portalB = Instantiate(portalB);
        portalA.SetActive(false);
        portalB.SetActive(false);
    }

    private void Update() {

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, mousePosition - pos, 5f, collisionMask);
        if (hit) {
            if ((hit.normal == Vector2.up || hit.normal == Vector2.down)) {
                lr.startColor = lr.endColor = shotA? Color.blue : Color.yellow;
                lr.SetPosition(0, pos);
                lr.SetPosition(1, hit.point);

                if (Input.GetMouseButtonDown(0) && !shotA) {
                    portalA.transform.position = hit.point + hit.normal * .5f;
                    portalA.SetActive(true);
                    shotA = true;
                    Instantiate(Resources.Load("Portal1Sound"), transform.position, transform.rotation);
                } else if (Input.GetMouseButtonDown(0) && shotA) {
                    portalB.transform.position = hit.point + hit.normal * .5f;
                    portalB.SetActive(true);
                    shotA = false;
                    portalsActive = true;
                    Instantiate(Resources.Load("Portal2Sound"), transform.position, transform.rotation);
                }
            } else {
                lr.startColor = lr.endColor = Color.white;
                lr.SetPosition(0, pos);
                lr.SetPosition(1, hit.point);
            }

        } else {
            lr.startColor = lr.endColor = Color.white;
            lr.SetPosition(0, pos);
            lr.SetPosition(1, ((mousePosition - pos).normalized * 5f) + pos);
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            portalA.SetActive(false);
            portalB.SetActive(false);
            shotA = false;
            portalsActive = false;
            inA = false;
            inB = false;
        }

        if (controller.collisions.above || controller.collisions.below) {
            velocity.y = 0;
            anim.SetBool("isJumping", false);
        }

        if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below) {
            velocity.y = jumpVelocity;
            anim.SetBool("isJumping", true);
            Instantiate(Resources.Load("JumpSound"), transform.position, transform.rotation);
        }

        if (teleportedA) {
            transform.position = portalB.transform.position;
            teleportedA = false;
            velocity.y *= -1;
        }

        if (teleportedB) {
            transform.position = portalA.transform.position;
            teleportedB = false;
            velocity.y *= -1;
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
            if (collision.gameObject == portalA && !inA && portalsActive) {
                teleportedA = true;
                inB = true;
            } else if (collision.gameObject == portalB && !inB && portalsActive) {
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
