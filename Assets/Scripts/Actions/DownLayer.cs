using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownLayer : MonoBehaviour {

    private void Awake() {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    /*
     * Change the players actual layer when trigger
     */
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            other.GetComponent<Player>().inUpLayer = false;
        }
    }
}
