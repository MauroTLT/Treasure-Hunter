using System.Collections;
using UnityEngine;

public class Priest : ShowText {

    [TextArea]
    public string confirmText;
    [TextArea]
    public string refuseText;

    private void Update() {
        // Check if the player is interacting with him
        if (!GameManager.gamePaused && !Crossfade.inTransition) {
            if (coll.IsTouching(GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>())) {
                if (Input.GetButtonUp("Submit")) {
                    StartCoroutine(WantToSave());
                }
            }
        }
    }

    /*
     * Coroutine to save the game
     */
    private IEnumerator WantToSave() {
        dialogController.ShowText(text, true);
        while (dialogController.result == DialogController.Result.None) {
            yield return null;
        }

        if (dialogController.result == DialogController.Result.Yes) {
            GameManager.SaveGame(true);

            Crossfade.transitionTime = 1F;
            StartCoroutine(GameObject.FindGameObjectWithTag("Crossfade").GetComponent<Crossfade>().DoCrossfade());
            sound.SaveSound();
            while (Crossfade.inTransition) {
                yield return null;
            }
            Crossfade.transitionTime = 0.35F;

            dialogController.ShowText(confirmText, false);
            while (DialogController.isTextShown) {
                yield return null;
            }
        } else {
            dialogController.ShowText(refuseText, false);
        }
    }

}
