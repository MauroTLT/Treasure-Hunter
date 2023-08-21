using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrainLava : MonoBehaviour
{
    public Switch[] needsToActivate;
    public GameObject lavaLayer;

    public Warp teleport;

    public GameObject toWarp;
    public GameObject toMap;

    private void Update() {
        if (AllSwitchesActivated()) {
            teleport.targetMap = toMap;
            teleport.target = toWarp;

            Destroy(lavaLayer);
            Destroy(gameObject);
        }
    }

    private bool AllSwitchesActivated() {
        foreach (var item in needsToActivate) {
            if (!item.activated) {
                return false;
            }
        }
        return true;
    }
}
