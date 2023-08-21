using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookLine : MonoBehaviour {

    public Transform origin, destination;
    private LineRenderer lr;

    private void Awake() {
        lr = GetComponent<LineRenderer>();
    }

    /*
     * Update the line positions
     */
    private void Update() {
        lr.SetPosition(0, origin.position);
        lr.SetPosition(1, destination.position);
    }
}
