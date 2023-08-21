using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossfade : MonoBehaviour {

    private Animator transition;
    public static bool inTransition = false;
    public static float transitionTime = 0.35f;
    public static float battleTransTime = 0.75f;

    private void Awake() {
        transition = GetComponentInChildren<Animator>();
    }

    public void Transition(string trigger) {
        StartCoroutine(DoTransition(trigger));
    }

    public void MenuTransition(GameObject changeTo) {
        StartCoroutine(DoMenuTransition(changeTo));
    }

    public void BattleTransition(GameObject changeTo) {
        StartCoroutine(DoBattleTransition(changeTo));
    }

    /*
     * Do a Transition passed by argument
     */
    private IEnumerator DoTransition(string trigger) {
        inTransition = true;
        transition.SetTrigger(trigger);

        yield return new WaitForSeconds(transitionTime);

        inTransition = false;
    }

    /*
     * Transition to open the menu
     */
    private IEnumerator DoMenuTransition(GameObject changeTo) {
        StartCoroutine(DoTransition("Start"));

        yield return new WaitForSeconds(transitionTime);
        if (changeTo != null) {
            changeTo.SetActive(GameManager.gamePaused);
            UIMenu.SetPartyStats();
        }

        StartCoroutine(DoTransition("End"));
    }

    /*
     * Transition to open a battle
     */
    private IEnumerator DoBattleTransition(GameObject changeTo) {
        StartCoroutine(DoTransition("Start"));

        yield return new WaitForSeconds(battleTransTime);
        if (changeTo != null) {
            changeTo.SetActive(GameManager.gamePaused);
        }

        StartCoroutine(DoTransition("End"));
    }

    /*
     * Crossfade Transition
     */
    public IEnumerator DoCrossfade() {
        Transition("Start");
        while (inTransition) {
            yield return null;
        }
        Transition("End");
    }
}
