using UnityEngine;

public class PressPlate : Switch {

    public bool stayToActivate;

    private void OnTriggerEnter2D(Collider2D other) {
        // Activate in case it doesn't need to stay triggered
        if (!stayToActivate && !other.isTrigger) {
            if (other.gameObject.tag != "SwitchActivator" && other.gameObject.tag != "PlayerActionBox") {
                ChangeState(true);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        // Activate if it needs to stay triggered
        if (stayToActivate && !other.isTrigger) {
            if (other.gameObject.tag != "SwitchActivator" && other.gameObject.tag != "PlayerActionBox") {
                ChangeState(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        // Deactivate only if needs to stay triggered and an object has exited
        if (stayToActivate && !other.isTrigger) {
            if (other.gameObject.tag != "SwitchActivator" && other.gameObject.tag != "PlayerActionBox") {
                ChangeState(false);
            }
        }
    }
}
