using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseBlock : MonoBehaviour {

    private Rigidbody2D rb2d;

    private void Start() {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            Vector3 pos = other.gameObject.transform.position;
            float diff =  pos.y - transform.position.y;
            
            rb2d.AddForce(new Vector2(0, diff) * 100);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            rb2d.velocity = Vector2.zero;
        }
    }
}
