using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour {

    // Orientation in where the player will be jumping
    public enum Orientations { Up = 1, Down = 2, Left = 3, Right = 4 };
    public Orientations orientation;
    private Vector3 moveTo;

    private void Start() {
        moveTo = Vector3.zero;
        switch (orientation) {
            case Orientations.Up:
                moveTo = new Vector3(0, 2, 0);
                break;
            case Orientations.Down:
                moveTo = new Vector3(0, -2, 0);
                break;
            case Orientations.Left:
                moveTo = new Vector3(-1, -2, 0);
                break;
            case Orientations.Right:
                moveTo = new Vector3(1, -2, 0);
                break;
        }
    }

    IEnumerator OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            other.GetComponent<Animator>().enabled = false;
            other.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            other.GetComponent<Player>().enabled = false;

            if (moveTo.x != 0) {
                for (int i = 0; i < 10; i++) {
                    other.transform.position += new Vector3(moveTo.x / 10, 0, 0);
                    yield return new WaitForSeconds(0.05F);
                }
            }
            for (int i = 0; i < 10; i++) {
                other.transform.position += new Vector3(0, moveTo.y / 10, 0);
                yield return new WaitForSeconds(0.05F);
            }

            other.GetComponent<Animator>().enabled = true;
            other.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            other.GetComponent<Player>().enabled = true;
            other.GetComponent<Player>().inUpLayer = false;
        }
    }
}
