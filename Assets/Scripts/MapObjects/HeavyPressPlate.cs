using UnityEngine;

public class HeavyPressPlate : PressPlate {

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag != "SwitchActivator" && other.gameObject.tag != "PlayerActionBox") {
            // Activate if a riseable item triggers it
            if (other.gameObject.GetComponent<Riseable>() != null) {
                ChangeState(true);
            }
        }
    }

    // Override to prevent conflict with the base class
    private void OnTriggerStay2D(Collider2D other) { }

    private void OnTriggerExit2D(Collider2D other) {
        if (stayToActivate) {
            if (other.gameObject.tag != "SwitchActivator" && other.gameObject.tag != "PlayerActionBox") {
                // Deactivate it when a riseable object exits
                if (other.gameObject.GetComponent<Riseable>() != null) {
                    ChangeState(false);
                }
            }
        }
    }
}
