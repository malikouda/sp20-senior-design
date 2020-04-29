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
    private GameManager gm;
    private Vector2 normalA;
    private Vector2 normalB;

    public float maxVelocity = 30;
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
    public AudioClip[] sounds;

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
        gm = FindObjectOfType<GameManager>();
    }

    private void Update() {

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, mousePosition - pos, 5f, collisionMask);
        if (hit) {
            lr.startColor = lr.endColor = shotA ? Color.blue : Color.yellow;
            lr.startWidth = .03f;
            lr.SetPosition(0, pos);
            lr.SetPosition(1, hit.point);

            if (Input.GetMouseButtonDown(0) && !shotA) {
                portalA.transform.position = hit.point + hit.normal * .75f;
                portalA.SetActive(true);
                normalA = hit.normal;
                shotA = true;
                if (gm) {
                   gm.PlaySound(sounds[1]);
                }
                
            } else if (Input.GetMouseButtonDown(0) && shotA) {
                portalB.transform.position = hit.point + hit.normal * .75f;
                portalB.SetActive(true);
                normalB = hit.normal;
                shotA = false;
                portalsActive = true;
                if (gm) {
                    gm.PlaySound(sounds[2]);
                }
            }

        } else {
            lr.startColor = lr.endColor = Color.white;
            lr.SetPosition(0, pos);
            lr.SetPosition(1, ((mousePosition - pos).normalized * 5) + pos);
        }

        if (Input.GetKeyDown(KeyCode.R) && (portalA.activeSelf || portalB.activeSelf)) {
            portalA.SetActive(false);
            portalB.SetActive(false);
            shotA = false;
            portalsActive = false;
            inA = false;
            inB = false;
            if (gm) {
                gm.PlaySound(sounds[3]);
            }
        }

        if (controller.collisions.above || controller.collisions.below) {
            velocity.y = 0;
            anim.SetBool("isJumping", false);
        }

        if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below) {
            velocity.y = jumpVelocity;
            anim.SetBool("isJumping", true);
            if (gm) {
                gm.PlaySound(sounds[0]);
            }
        }

        if (teleportedA) {
            print(normalB);
            transform.position = portalB.transform.position;
            if (normalB.x == 0 && normalB.y != 0) {
                velocity.y *= -normalB.y;
            } else if (normalB.y == 0 && normalB.x != 0) {
                velocity.x *= -normalB.x;
            } else {
                velocity *= -normalB;
            }
        }

        if (teleportedB) {
            print(normalA);
            transform.position = portalA.transform.position;
            if (normalA.x == 0 && normalA.y != 0) {
                velocity.y *= -normalA.y;
            } else if (normalA.y == 0 && normalA.x != 0) {
                velocity.x *= -normalA.x;
            } else {
                velocity *= -normalA;
            }
        }

        velocity = Vector2.ClampMagnitude(velocity, 25);
        float targetVelocityX = input.x * moveSpeed;

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        teleportedB = false;
        teleportedA = false;

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

        if (collision.tag == "NextLevel") {
            gm.nextLevel = true;
        }

        if (collision.tag == "ResetLevel") {
            transform.position = gm.checkpoint.position;
            portalA.SetActive(false);
            portalB.SetActive(false);
            shotA = false;
            portalsActive = false;
            inA = false;
            inB = false;
        }

        if (collision.tag == "Checkpoint" || collision.tag == "Origin") {
            gm.checkpoint = collision.transform;
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
