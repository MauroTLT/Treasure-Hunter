using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walk : MonoBehaviour {

    [Space]
    [Header("Speed")]
    public float speed = 4.0f;
    [Space]
    [Header("Seconds To Wait Between Movement")]
    public float minTimeBetweenMove;
    public float maxTimeBetweenMove;
    [Space]
    [Header("Movement Duration")]
    public float minTimeToMove;
    public float maxTimeToMove;

    private Animator anim;
    private Rigidbody2D rb2d;

    private Vector2 mov;

    private void Start() {
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        if (speed > 0) {
            StartCoroutine(Movement());
        }
    }

    private void FixedUpdate() {
        if (!GameManager.gamePaused && !DialogController.isTextShown) {
            rb2d.MovePosition(rb2d.position + mov * speed * Time.deltaTime);
        }
    }

    /*
     * When colliding with something, stop movement
     */
    private void OnCollisionEnter2D(Collision2D collision) {
        if (anim.GetBool("walking")) {
            mov = Vector2.zero;
            Animations();
        }
    }

    /*
     * Coroutine that controls the movement of the character
     */
    private IEnumerator Movement() {
        while (true) {
            mov = Vector2.zero;
            Animations();
            if (!GameManager.gamePaused && !DialogController.isTextShown) {
                yield return new WaitForSeconds(Random.Range(minTimeBetweenMove, maxTimeBetweenMove));

                ChangeDirection();

                yield return new WaitForSeconds(Random.Range(minTimeToMove, maxTimeToMove));
            } else {
                yield return null;
            }
        }
    }

    /*
     * Change the direction randomly
     */
    private void ChangeDirection() {
        mov = Vector2.zero;
        switch (Random.Range(0, 5)) {
            case 0: mov.x = 1; break; // Right
            case 1: mov.x = -1; break; // Left
            case 2: mov.y = 1; break; // Up
            case 3: mov.y = -1; break; // Down
        }
        Animations();
    }

    /*
     * Change the direction at choice
     */
    public void ChangeDirection(int dir) {
        speed = 1;
        mov = Vector2.zero;
        switch (dir) {
            case 0: mov.x = 1; break; // Right
            case 1: mov.x = -1; break; // Left
            case 2: mov.y = 1; break; // Up
            case 3: mov.y = -1; break; // Down
        }
        Animations();
    }

    /*
     * Update the animations of the character
     */
    private void Animations() {
        if (mov != Vector2.zero) {
            anim.SetFloat("movX", mov.x);
            anim.SetFloat("movY", mov.y);
            anim.SetBool("walking", true);
        } else {
            anim.SetBool("walking", false);
        }
    }

    public void SetMov(Vector2 mov) {
        this.mov = mov;
        if (!anim.enabled) {
            anim.enabled = true;
        }
        anim.SetFloat("movX", mov.x);
        anim.SetFloat("movY", mov.y);
    }

}
