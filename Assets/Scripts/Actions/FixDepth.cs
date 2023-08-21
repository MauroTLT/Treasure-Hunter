using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixDepth : MonoBehaviour {

    // Change only the Z coordinate
    public bool relativeToPlayer;

    private SpriteRenderer sprite;
    private GameObject player;

    void Start() {
        sprite = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void FixedUpdate() {
        // Change the sorting order based in the Y coordinate of the player
        if (relativeToPlayer) {
            transform.position = new Vector3(
                transform.position.x,
                transform.position.y,
                player.transform.position.z + ((player.transform.position.y < transform.position.y) ? 1 : -1)
            );
        } else {
            sprite.sortingOrder = player.GetComponent<SpriteRenderer>().sortingOrder +
                ((player.transform.position.y < transform.position.y) ? -1 : 1);
        }
    }
}
