using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Riseable : MonoBehaviour {

    private bool inUse;
    private GameObject player;
    private Rigidbody2D rb2d;
    private BoxCollider2D coll;

    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        rb2d = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
    }

    private void Update() {
        if (!inUse && Input.GetButtonDown("A - Action")) {
            if (coll.IsTouching(player.GetComponent<Collider2D>())) {
                StartCoroutine(RiseObject());
            }
        } else if (inUse && Input.GetButtonDown("A - Action")) {
            LeaveObject();
        }
    }

    /*
     * Coroutine that rises the object touched by the player
     */
    private IEnumerator RiseObject() {
        player.GetComponent<Animator>().SetTrigger("startAction");
        yield return new WaitForSeconds(0.25F);
        rb2d.bodyType = RigidbodyType2D.Kinematic;
        transform.parent = player.transform;
        coll.enabled = false;
        if (GetComponent<FixDepth>() != null) {
            GetComponent<FixDepth>().enabled = false;
        }
        GetComponent<SpriteRenderer>().sortingOrder = 3;
        transform.position = new Vector3(
            player.transform.position.x,
            player.transform.position.y + 0.7F,
            0
        );
        player.GetComponent<Animator>().SetBool("inAction", true);
        player.GetComponent<Player>().interactingWithObject = true;
        inUse = true;
    }

    /*
     * Coroutine that leaves the object based in the orientation of the player
     */
    private void LeaveObject() {
        rb2d.bodyType = RigidbodyType2D.Dynamic;
        transform.parent = null;
        if (GetComponent<FixDepth>() != null) {
            GetComponent<FixDepth>().enabled = true;
        }
        GetComponent<SpriteRenderer>().sortingOrder = 2;
        Vector2 mov = player.GetComponent<Player>().GetMovNormalized(false);
        transform.position = new Vector3(
            player.transform.position.x + ((mov.y == 0) ? (mov.x / 1.6F) : 0),
            player.transform.position.y + ((mov.x == 0) ? (mov.y / 1.6F) : 0),
            1
        );

        coll.enabled = true;
        player.GetComponent<Animator>().SetBool("inAction", false);
        player.GetComponent<Animator>().SetTrigger("startAction");
        player.GetComponent<Player>().interactingWithObject = false;
        inUse = false;
    }
}
