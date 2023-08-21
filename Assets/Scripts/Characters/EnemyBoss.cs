using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBoss : ShowText {

    [TextArea]
    public string triggerText;
    public GameObject battlePrefab;
    public Enemy stats;
    private void Awake() {
        coll = GetComponent<Collider2D>();
        dialogController = FindObjectOfType<DialogController>();
        player = FindObjectOfType<Player>().gameObject;
    }
    private void Start() {
        if (!GameManager.enemies) {
            GameManager.lastMileStone++;
            Destroy(gameObject);
        }
    }

    private void Update() {
        // Check if the player is interacting with him
        if (!GameManager.gamePaused && !Crossfade.inTransition) {
            if (coll.IsTouching(GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>())) {
                if (Input.GetButtonUp("Submit")) {
                    
                    StartCoroutine(InstantiateBattle(text));
                }
            }
        }
    }

    // Check if the player has trigger the trap
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag.Equals("Player")) {
            StartCoroutine(InstantiateBattle(triggerText));
        }
    }

    /*
     * Coroutine that instantiates the battle with the boss
     */
    private IEnumerator InstantiateBattle(string text) {
        if (GetComponent<Walk>() != null) {
            Orientate();
        }
        dialogController.ShowText(text, false);
        while (DialogController.isTextShown) {
            yield return null;
        }

        GameManager.gamePaused = true;
        GameObject battle = Instantiate(battlePrefab);
        battle.transform.GetChild(0).GetComponent<Image>().sprite = GameManager.actualZone.GetBattleBackground();
        battle.GetComponent<BattleController>().boss = stats;
        battle.SetActive(false);
        GameObject.FindGameObjectWithTag("Crossfade").GetComponent<Crossfade>().BattleTransition(battle);
        Time.timeScale = GameManager.battleSpeed;

        Destroy(gameObject);
    }
}
