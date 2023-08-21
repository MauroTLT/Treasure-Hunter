using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour {
    protected SpriteRenderer rend;
    protected Collider2D coll;

    public bool activated = false;
    public bool isLeftOn;
    public Sprite on;
    public Sprite off;

    private GameObject player;

    private void Start() {
        rend = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update() {
        // Switch state when the player interact with the object
        if (!GameManager.gamePaused && !Crossfade.inTransition) {
            if (coll.IsTouching(player.GetComponent<Collider2D>())) {
                if (Input.GetButtonUp("Submit")) {
                    ChangeState(!activated);
                }
            }
        }
    }

    /*
     * Check if an object that can trigger the switch has collided
     */
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "PlayerActionBox") {
            ChangeState(!activated);
        } else if (other.tag == "SwitchActivator") {
            Vector3 center = other.bounds.center;
            // Check if the orientation is correct
            if (!isLeftOn) {
                ChangeState(coll.bounds.center.x < center.x);
            } else {
                ChangeState(coll.bounds.center.x > center.x);
            }
        }
    }

    public virtual void ChangeState(bool state) {
        activated = state;
        rend.sprite = (activated) ? on : off;
    }
}
