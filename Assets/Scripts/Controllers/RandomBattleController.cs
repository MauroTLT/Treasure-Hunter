using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomBattleController : MonoBehaviour {

    public GameObject battlePrefab;
    private float lastBattle;
    private const float SEC_AFTER_BATTLE = 4f;

    private void Start() {
        lastBattle = 0;
        // If there aren't monsters in the zone disable the script
        if (GameManager.actualZone.enemyTable == null) {
            enabled = false;
        }
    }

    private void LateUpdate() {
        if (!GameManager.gamePaused) {
            // Check if the player is moving
            if (GameManager.GetMov() != Vector2.zero) {
                lastBattle += Time.deltaTime;
                // If has pass enough time since the last battle
                if (lastBattle > SEC_AFTER_BATTLE) {
                    // 1 in 200 probability every tick
                    if (Random.Range(0, 200) == 0) {
                        InstantiateBattle();
                    }
                }
            }
        }
    }

    /*
     * Method to instantiate a random battle
     */
    private void InstantiateBattle() {
        lastBattle = 0f;
        GameManager.gamePaused = true;
        GameObject battle = Instantiate(battlePrefab);
        battle.transform.GetChild(0).GetComponent<Image>().sprite = GameManager.actualZone.GetBattleBackground();
        battle.SetActive(false);
        GameObject.FindGameObjectWithTag("Crossfade").GetComponent<Crossfade>().BattleTransition(battle);
        Time.timeScale = GameManager.battleSpeed;
    }
}
