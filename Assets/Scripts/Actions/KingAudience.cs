using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingAudience : MonoBehaviour {

    [SerializeField]
    private GameObject king;
    [SerializeField]
    private GameObject chancellor;
    private GameObject player;
    private DialogController dialogController;

    private void Start() {
        dialogController = GameController.dialogController;
        player = GameObject.FindGameObjectWithTag("Player");
        GetComponent<Renderer>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag.Equals("Player")) {
            StartCoroutine(StartScene());
        }
    }

    private IEnumerator StartScene() {
        GameManager.gamePaused = true;
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < 150; i++) {
            player.GetComponent<Player>().mov = new Vector2(0, 0.75f);
            player.GetComponent<Player>().Animations();
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        chancellor.GetComponent<Walk>().ChangeDirection(3);

        yield return new WaitForSeconds(0.5f);

        dialogController.ShowText("Present yourself to the King young man.", false);
        while (DialogController.isTextShown) {
            yield return null;
        }
        chancellor.GetComponent<Walk>().ChangeDirection(-1);
        yield return new WaitForSeconds(0.5f);

        dialogController.ShowText("My name is " + GameManager.party[0].fullname + " sir, a treasure hunter of a far land.", false);
        while (DialogController.isTextShown) {
            yield return null;
        }

        chancellor.GetComponent<Walk>().ChangeDirection(3);
        yield return new WaitForSeconds(0.5f);
        
        dialogController.ShowText(
            "Then the rumors are true, we know who you are.\n" +
            "The tales of your talent have reached this land.\n" + 
            "Your Majesty, I think this man can help us with that...", false);
        while (DialogController.isTextShown) {
            yield return null;
        }

        chancellor.GetComponent<Walk>().ChangeDirection(-1);
        king.GetComponent<Walk>().ChangeDirection(3);
        yield return new WaitForSeconds(0.5f);

        dialogController.ShowText(GameManager.party[0].fullname +
            " As surely you already know, I was investigating about the Mystic Stones.\n" +
            "The people think are just a kid's tale, but it's not, those gems were property of my ancestor.\n" +
            "I have reasons to recover them as fast as possible." +
            "Will you use your experience to help us?", true);
        king.GetComponent<Walk>().ChangeDirection(-1);
        while (dialogController.result == DialogController.Result.None) {
            yield return null;
        }
        if (dialogController.result == DialogController.Result.Yes) {
            dialogController.ShowText(
            "I appreciate it. It's not an easy task, but we will help you as well.\n" +
            "The first one of the gems lies deep in the Molten Cave, you will have to traverse de Dark Forest to reach it.\n" +
            "I will tell the guard to open the pass. I desire you the best of lucks.", false);
        } else {
            dialogController.ShowText(
                "Is your choice, but I don't think anyone can do a better job than you.\n" +
                "The first one of the gems lies deep in the Molten Cave, you will have to traverse de Dark Forest to reach it.\n" +
                "I will tell the guard to open the pass. Do whatever you want.", false);
        }
        while (DialogController.isTextShown) {
            yield return null;
        }
        GameManager.lastMileStone++;
        GameManager.gamePaused = false;
        Destroy(gameObject);
    }
}
