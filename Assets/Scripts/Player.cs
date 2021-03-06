﻿using UnityEngine;
using UnityEngine.SceneManagement;

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
    private bool hasPortal;
    private bool canExitLevel;
    private int numPickups = 3;
    private int checkpointNum = 0;
    private GameObject lastCheckpoint = null;

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
    public bool active = true;

    [HideInInspector]
    public Vector2 input {
        get {
            return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
    }

    private void Start() {
        gm = FindObjectOfType<GameManager>();
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

        if (gm && gm.currentLevel == 2) {
            hasPortal = false;
            anim.SetLayerWeight(0, 1);
            anim.SetLayerWeight(1, 0);
        } else {
            hasPortal = true;
            anim.SetLayerWeight(0, 0);
            anim.SetLayerWeight(1, 1);
        }

        if (gm && gm.currentLevel == 4) {
            canExitLevel = false;
            numPickups = 3;
        } else {
            canExitLevel = true;
            numPickups = 0;
        }
    }

    private void Update() {
        if (active) {
            if (!lr.enabled) {
                lr.enabled = true;
            }
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 pos = new Vector2(transform.position.x, transform.position.y);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, mousePosition - pos, 5f, collisionMask);
            if (hit && hit.collider.tag != "noPortal" && hasPortal) {
                lr.startColor = lr.endColor = shotA ? Color.blue : Color.yellow;
                lr.startWidth = .03f;
                lr.SetPosition(0, pos);
                lr.SetPosition(1, hit.point);

                if (Input.GetMouseButtonDown(0) && !shotA) {
                    portalA.transform.position = hit.point + hit.normal * .70f;
                    portalA.SetActive(true);
                    normalA = hit.normal;
                    shotA = true;
                    if (gm) {
                        gm.PlaySound(sounds[1]);
                    }

                } else if (Input.GetMouseButtonDown(0) && shotA) {
                    portalB.transform.position = hit.point + hit.normal * .70f;
                    portalB.SetActive(true);
                    normalB = hit.normal;
                    shotA = false;
                    portalsActive = true;
                    if (gm) {
                        gm.PlaySound(sounds[2]);
                    }
                }

            } else if (hasPortal) {
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
        } else {
            lr.enabled = false;
            anim.SetBool("isWalking", false);
            anim.SetBool("isJumping", false);
            velocity.x = 0;
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
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

        if (collision.tag == "NextLevel" && canExitLevel) {
            if (gm) {
                gm.nextLevel = true;
            }
            checkpointNum = 0;
        }

        if (collision.tag == "ResetLevel") {
            checkpointNum = 0;
            if (gm) {
                transform.position = gm.checkpoint.position;
            }
            portalA.SetActive(false);
            portalB.SetActive(false);
            shotA = false;
            portalsActive = false;
            inA = false;
            inB = false;
        }

        if (collision.tag == "Checkpoint") {
            if (gm) {
                if (lastCheckpoint != collision.gameObject) {
                    gm.PlaySound(sounds[(checkpointNum % 3) + 4]);
                }
            }

            if (lastCheckpoint == null || lastCheckpoint != collision.gameObject) {
                lastCheckpoint = collision.gameObject;
                checkpointNum++;
            }
        }

        if (collision.tag == "Checkpoint" || collision.tag == "Origin") {
            if (gm) {
                gm.checkpoint = collision.transform;
            }
        }

        if (collision.tag == "gloves") {
            Destroy(collision.gameObject);
            hasPortal = true;
            anim.SetLayerWeight(0, 0);
            anim.SetLayerWeight(1, 1);
        }

        if (collision.tag == "pickup") {
            if (gm) {
                gm.PlaySound(sounds[7]);
            }
            Destroy(collision.gameObject);
            numPickups--;
            if (numPickups == 0) {
                canExitLevel = true;
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
