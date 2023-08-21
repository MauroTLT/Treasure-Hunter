using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyActionController : MonoBehaviour {

    [HideInInspector]
    public string[] action = new string[2];
    [HideInInspector]
    public GameObject[] target = new GameObject[2];
    [HideInInspector]
    public ScriptableObject[] thing = new ScriptableObject[2];

    public int xpGained;
    public int goldGained;
    public Item[] itemsGained = new Item[4];
    
    public PartyStatsController partyStats;
    [HideInInspector]
    public bool finish;

    public bool IsDead(int index) {
        return GameManager.party[index].hp == 0;
    }

    /*
     * Perform the action selected for every member
     */
    public IEnumerator DoTheAction(int i) {
        switch (action[i]) {
            case "Attack":
                StartCoroutine(Attack(i));
                break;
            case "Defend":
                GameController.dialogController.ShowBattleText(GameManager.party[i].fullname + " defends.");
                while (DialogController.isTextShown) {
                    yield return null;
                }
                GameManager.party[i].defense *= 2;
                GameManager.party[i].resistance *= 2;
                finish = true;
                break;
            case "Item":
                StartCoroutine(Item(i));
                break;
            case "Skill":
                StartCoroutine(Skill(i));
                break;
        }
    }

    /*
     * Coroutine that attacks an enemy
     */
    private IEnumerator Attack(int i) {
        partyStats.SetSelectedDialog(i);
        EnemyBattleController ebc = target[i].GetComponent<EnemyBattleController>();
        
        // If the enemy is already dead choose another
        if (ebc.enemy.hp == 0) {
            EnemyBattleController[] others = FindObjectsOfType<EnemyBattleController>();
            foreach (EnemyBattleController e in others) {
                if (e.gameObject.activeSelf) {
                    ebc = e;
                    break;
                }
            }
        }

        // Hit the enemy if lives
        if (ebc.enemy.hp != 0) {
            ebc.HitAnim();
            yield return new WaitForSeconds(1.5f);
            int damage = GameController.CalculateDamage(GameManager.party[i], ebc.enemy);
            ebc.enemy.TakeDamage(damage);
            StartCoroutine(ebc.ShowNumber(damage, Color.red));
            yield return new WaitForSeconds(1);
            if (ebc.enemy.hp == 0) {
                xpGained += ebc.enemy.xpDrop;
                goldGained += ebc.enemy.gold;
                DropItem(ebc.enemy);
                ebc.Kill();
            }
        }
        partyStats.SetDefaultDialog(i);
        finish = true;
    }

    /*
     * Use the item selected in the member selected
     */
    private IEnumerator Item(int i) {
        Consumable item = thing[i] as Consumable;
        int index = (target[i].name.Contains("2") ? 1 : 0);
        partyStats.SetSelectedDialog(i);
        item.Use(GameManager.party[index], true);

        StartCoroutine(partyStats.ShowNumber(index, item.amount, Color.green));
        partyStats.SetPartyStats();
        yield return new WaitForSeconds(1);
        partyStats.SetDefaultDialog(i);
        finish = true;
    }

    /*
     * Coroutine that attacks an enemy with a selected skill
     */
    private IEnumerator Skill(int i) {
        partyStats.SetSelectedDialog(i);
        Skill skill = thing[i] as Skill;
        EnemyBattleController ebc = target[i].GetComponent<EnemyBattleController>();

        // If the enemy is already dead choose another
        if (ebc.enemy.hp == 0) {
            EnemyBattleController[] others = FindObjectsOfType<EnemyBattleController>();
            foreach (EnemyBattleController e in others) {
                if (e.gameObject.activeSelf) {
                    ebc = e;
                    break;
                }
            }
        }

        // Hit the enemy if lives
        if (ebc.enemy.hp != 0) {
            GameController.dialogController.ShowBattleText(GameManager.party[i].fullname + " uses " + skill.skillName);
            while (DialogController.isTextShown) {
                yield return null;
            }
            GameManager.party[i].sp -= skill.cost;
            partyStats.SetPartyStats();

            ebc.HitAnim();
            yield return new WaitForSeconds(1.5f);
            int damage = GameController.CalculateDamage(GameManager.party[i], ebc.enemy, skill);
            ebc.enemy.TakeDamage(damage);
            StartCoroutine(ebc.ShowNumber(damage, Color.red));
            yield return new WaitForSeconds(1);
            if (ebc.enemy.hp == 0) {
                xpGained += ebc.enemy.xpDrop;
                goldGained += ebc.enemy.gold;
                DropItem(ebc.enemy);
                ebc.Kill();
            }
        }
        partyStats.SetDefaultDialog(i);
        finish = true;
    }

    /*
     * Check if the enemy has dropped something
     */
    private void DropItem(Enemy enemy) {
        for (int i = 0; i < itemsGained.Length; i++) {
            if (itemsGained[i] == null) {
                if (enemy.drop != null && Random.Range(1, 101) < enemy.probItemDrop) {
                    itemsGained[i] = enemy.drop;
                }
                break;
            }
        }
    }

    /*
     * Reset the defense stats after Defend
     */
    public void ResetDefenseStats() {
        for (int i = 0; i < GameManager.party.Length; i++) {
            if (action[i].Equals("Defend")) {
                GameManager.party[i].defense /= 2;
                GameManager.party[i].resistance /= 2;
            }
        }
    }

}
