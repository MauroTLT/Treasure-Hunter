using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckMilestone : MonoBehaviour {
    /*
     * Do something when a milestone is reached
     */
    public enum Action { DESTROY, HIDE, REMOVECOLL, SHOW};

    public MileStones mileStones;

    public int activateIf = 0;
    public Action action;

    private Collider2D coll;
    private Renderer rend;

    private void Awake() {
        coll = GetComponent<Collider2D>();
        rend = GetComponent<SpriteRenderer>();
        if (GameManager.lastMileStone >= activateIf) {
            switch (action) {
                case Action.DESTROY:
                    Destroy(gameObject);
                    break;
                case Action.HIDE:
                    rend.enabled = false;
                    break;
                case Action.REMOVECOLL:
                    coll.enabled = false;
                    break;
                case Action.SHOW:
                    rend.enabled = true;
                    coll.enabled = true;
                    break;
                default:
                    break;
            }
        }
    }
}
