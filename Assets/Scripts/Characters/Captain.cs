using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Captain : ShowText {

    [TextArea]
    public string confirmText;
    [TextArea]
    public string refuseText;

    public Zone zoneTo;

    private void Update() {
        // Check if the player is interacting with him
        if (!GameManager.gamePaused && !Crossfade.inTransition) {
            if (coll.IsTouching(GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>())) {
                if (Input.GetButtonUp("Submit")) {
                    StartCoroutine(WantToTravel());
                }
            }
        }
    }

    /*
     * Coroutine to move the player between scenes
     */
    private IEnumerator WantToTravel() {
        dialogController.ShowText(text, true);
        while (dialogController.result == DialogController.Result.None) {
            yield return null;
        }

        if (dialogController.result == DialogController.Result.Yes) {
            dialogController.ShowText(confirmText, false);
            while (DialogController.isTextShown) {
                yield return null;
            }

            GameObject.FindGameObjectWithTag("Crossfade").GetComponent<Crossfade>().Transition("Start");

            yield return new WaitForSeconds(Crossfade.transitionTime);

            GameManager.ChangeScene(zoneTo);
        } else {
            dialogController.ShowText(refuseText, false);
        }
    }

}
