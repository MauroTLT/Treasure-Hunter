using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour {

    [HideInInspector]
    public Vector2 mov;
    [HideInInspector]
    public Vector3 initialPos;

    public float speed;
    public float maxDistance;
    public float waitBeforeDestroy;

    private GameObject player;
    private bool retract, hooked;
    public GameObject lrPrefab;
    private GameObject lineRenderer;

    private void Awake() {
        player = GameObject.FindGameObjectWithTag("Player");
        lineRenderer = Instantiate(lrPrefab);
        lineRenderer.GetComponent<HookLine>().origin = player.transform;
        lineRenderer.GetComponent<HookLine>().destination = transform;
    }

    void Update() {
        if (!hooked) {
            // Move the hook forward until the max distance is reached
            transform.position += ((retract) ? -1 : 1) * new Vector3(mov.x, mov.y, 0) * speed * Time.deltaTime;
            Vector3 distance = (initialPos - transform.position);
            if (Mathf.Abs(distance.x) >= maxDistance ||
                Mathf.Abs(distance.y) >= maxDistance) {
                if (retract) {
                    Destroy(gameObject);
                }
                retract = true;
            }
        } else {
            // Move the player towards the hook
            player.transform.position += new Vector3(mov.x, mov.y, 0) * speed * Time.deltaTime;
        }
        if ((((int)transform.position.x) == ((int)player.transform.position.x) &&
            ((int)transform.position.y) == ((int)player.transform.position.y)) && retract) {
            if (hooked) {
                ChangePlayerState();
            }
            Destroy(gameObject);
        }
        
    }

    /*
     * Check to retract the hook
     */
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Hookable") {
            ChangePlayerState();
            hooked = true;
            retract = true;
        } else if (other.tag != "Player" && other.tag != "PlayerActionBox" && other.tag != "InternColision" && other.tag != "NoColision") {
            retract = true;
        } else if (other.tag == "InternColision" && !player.GetComponent<Player>().inUpLayer) {
            retract = true;
        }
    }

    /*
     * Switch the player collider and rigidbody to move it
     */
    private void ChangePlayerState() {
        if (hooked) {
            player.transform.position = transform.position;
            player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            player.GetComponent<Collider2D>().enabled = true;
        } else {
            player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            player.GetComponent<Collider2D>().enabled = false;
        }
    }

    private void OnDestroy() {
        Destroy(lineRenderer);
        player.GetComponent<Player>().movePrevent = false;
        player.GetComponent<Player>().artifactActing = false;
    }
}
