using UnityEngine;

public class PillarMove : MonoBehaviour {

    private Animator anim;
    private Rigidbody2D rb2d;
    private Collider2D coll;
    private bool canMove;

    void Start() {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
    }

    private void Update() {
        // Change the mass to allow the player to move it or not
        rb2d.mass = (canMove) ? 5 : 5000;

        // Check if the player is trying to move the object
        if (coll.IsTouching(anim.GetComponent<Collider2D>())) {
            if (Input.GetButton("A - Action")) {
                canMove = true;
                anim.SetTrigger("startAction");
                // Restricts the axis contrary, on which the player moves
                if (Input.GetAxisRaw("Horizontal") != 0) {
                    rb2d.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                } else if (Input.GetAxisRaw("Vertical") != 0) {
                    rb2d.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                }
            } else {
                canMove = false;
            }
        } else {
            canMove = false;
        }
    }
}
