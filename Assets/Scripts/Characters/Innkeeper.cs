using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Innkeeper : ShowText {
    [TextArea]
    public string confirmText;
    [TextArea]
    public string refuseText;

    public int stayCost;

    private void Awake() {
        coll = GetComponent<Collider2D>();
        sound = Camera.main.GetComponent<SoundController>();
        dialogController = FindObjectOfType<DialogController>();
    }

    private void Update() {
        // Check if the player is interacting with him
        if (!GameManager.gamePaused && !Crossfade.inTransition) {
            if (coll.IsTouching(GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>())) {
                if (Input.GetButtonUp("Submit")) {
                    StartCoroutine(WantToRest());
                }
            }
        }
    }

    /*
     * Coroutine that heals the player and collect money for it
     */
    private IEnumerator WantToRest() {
        dialogController.ShowText(text.Replace("[stayCost]", stayCost.ToString()), true);
        while (dialogController.result == DialogController.Result.None) {
            yield return null;
        }

        if (dialogController.result == DialogController.Result.Yes) {
            // If can pay the amount required
            if (GameManager.RestGold(stayCost)) {
                GameManager.party[0].HealAll();
                GameManager.party[1].HealAll();
                dialogController.ShowText(confirmText, false);
                while (DialogController.isTextShown) {
                    yield return null;
                }
                Crossfade.transitionTime = 1F;
                StartCoroutine(GameObject.FindGameObjectWithTag("Crossfade").GetComponent<Crossfade>().DoCrossfade());
                sound.HealSound();
                while (Crossfade.inTransition) {
                    yield return null;
                }
                Crossfade.transitionTime = 0.35F;
            } else {
                dialogController.ShowText("Sorry but you don't have enough money.", false);
                while (DialogController.isTextShown) {
                    yield return null;
                }
            }
        } else {
            dialogController.ShowText(refuseText, false);
        }
    }

}
