using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckSwitches : Switch {

    public Switch[] needsToActivate;
    private void Update() {
        activated = AllSwitchesActivated();
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
