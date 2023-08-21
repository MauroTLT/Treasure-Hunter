using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {

    public float secondsToExplode;

    private static int maxBombSimultaneous = 3;
    private static int actualBombs;
    private Collider2D coll;
    private Animator anim;
    private SpriteRenderer rend;

    private void Awake() {
        actualBombs++;
        // If already the max number of bombs placed, destroy
        if (actualBombs > maxBombSimultaneous) {
            Destroy(gameObject);
        }
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
        coll.enabled = false;
    }

    private void Start() {
        Invoke("Explode", secondsToExplode);
    }

    public void Explode() {
        if (!coll.enabled) {
            coll.enabled = true;
            rend.sortingOrder = 3;
            anim.SetTrigger("Explode");
            Invoke("Kill", 1F);
        }
    }

    private void Kill() {
        Destroy(gameObject);
    }

    private void OnDestroy() {
        actualBombs--;
    }

}
