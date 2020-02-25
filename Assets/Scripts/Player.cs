using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Player : MonoBehaviour {

    private BoxCollider2D collider;

    private Vector2 speed;

    private void Start() {
      collider = gameObject.GetComponent<BoxCollider2D>();
    }

    private void Update() {
        ComputeLimitBottom();
        ComputeLimitRight();
        ComputeLimitLeft();
        ComputeLimitTop();

        if (speed.x > 0) {
            ComputeLimitRight();
            Debug.Log("Going Right");
        }
    }

    private float ComputeLimitBottom() {

        Vector2 bottomLeft = new Vector2(collider.transform.position.x - collider.size.x / 2, collider.transform.position.y - collider.size.y / 2);
        Vector2 bottomRight = new Vector2(collider.transform.position.x + collider.size.x / 2, collider.transform.position.y - collider.size.y / 2);

        RaycastHit2D hit1 = Physics2D.Raycast(bottomLeft, Vector2.down, Mathf.Infinity);
        RaycastHit2D hit2 = Physics2D.Raycast(bottomRight, Vector2.down, Mathf.Infinity);

        if (hit1) {
            Debug.DrawLine(bottomLeft, hit1.point, Color.yellow);
        }

        if (hit2) {
            Debug.DrawLine(bottomRight, hit2.point, Color.yellow);
        }

        return -1;

    }

    private float ComputeLimitRight() {

        Vector2 topRight = new Vector2(collider.transform.position.x + collider.size.x / 2, collider.transform.position.y + collider.size.y / 2);
        Vector2 bottomRight = new Vector2(collider.transform.position.x + collider.size.x / 2, collider.transform.position.y - collider.size.y / 2);

        RaycastHit2D hit1 = Physics2D.Raycast(topRight, Vector2.right, Mathf.Infinity);
        RaycastHit2D hit2 = Physics2D.Raycast(bottomRight, Vector2.right, Mathf.Infinity);

        if (hit1) {
            Debug.DrawLine(topRight, hit1.point, Color.green);
        }

        if (hit2) {
            Debug.DrawLine(bottomRight, hit2.point, Color.green);
        }

        return -1;

    }

    private float ComputeLimitLeft() {

        Vector2 bottomLeft = new Vector2(collider.transform.position.x - collider.size.x / 2, collider.transform.position.y - collider.size.y / 2);
        Vector2 topLeft = new Vector2(collider.transform.position.x - collider.size.x / 2, collider.transform.position.y + collider.size.y / 2);

        RaycastHit2D hit1 = Physics2D.Raycast(bottomLeft, Vector2.left, Mathf.Infinity);
        RaycastHit2D hit2 = Physics2D.Raycast(topLeft, Vector2.left, Mathf.Infinity);

        if (hit1) {
            Debug.DrawLine(bottomLeft, hit1.point, Color.blue);
        }

        if (hit2) {
            Debug.DrawLine(topLeft, hit2.point, Color.blue);
        }

        return -1;

    }


    private float ComputeLimitTop() {

        Vector2 topLeft = new Vector2(collider.transform.position.x - collider.size.x / 2, collider.transform.position.y + collider.size.y / 2);
        Vector2 topRight = new Vector2(collider.transform.position.x + collider.size.x / 2, collider.transform.position.y + collider.size.y / 2);

        RaycastHit2D hit1 = Physics2D.Raycast(topLeft, Vector2.up, Mathf.Infinity);
        RaycastHit2D hit2 = Physics2D.Raycast(topRight, Vector2.up, Mathf.Infinity);

        if (hit1) {
            Debug.DrawLine(topLeft, hit1.point, Color.red);
        }

        if (hit2) {
            Debug.DrawLine(topRight, hit2.point, Color.red);
        }

        return -1;

    }
}