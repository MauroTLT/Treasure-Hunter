using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Switch))]
public class MultiSwitch : MonoBehaviour {

    public Switch[] activates;
    private Collider2D coll;

    private void Start() {
        coll = GetComponent<Collider2D>();
    }

    private void Update() {
        if (!GameManager.gamePaused && !Crossfade.inTransition) {
            if (coll.IsTouching(GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>())) {
                if (Input.GetButtonUp("Submit")) {
                    ChangeAllState();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "PlayerActionBox") {
            ChangeAllState();
        } else if (other.tag == "SwitchActivator") {
            ChangeAllState();
        }
    }

    private void ChangeAllState() {
        foreach (var item in activates) {
            item.ChangeState(!item.activated);
        }
    }

}
