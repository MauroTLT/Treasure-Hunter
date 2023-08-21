using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBattleController : MonoBehaviour {
    public enum Action { NotDecided, Nothing, Leave, Attack, Skill};

    public Enemy enemy;
    public static PartyStatsController partyStats;
    public Action result;
    [HideInInspector]
    public int target;

    private Animator anim;
    private GameObject text;
    private Vector3 initialPos;
    private bool inAnimation;

    private void Awake() {
        anim = GetComponentInChildren<Animator>();
        text = transform.Find("Text").gameObject;
        result = Action.NotDecided;
        initialPos = transform.position;
    }

    private void Start() {
        StartCoroutine(IdleMovement());
    }

    /*
     * Controls the idle animation of the enemy
     */
    private IEnumerator IdleMovement() {
        while (gameObject.activeSelf) {
            if (!inAnimation) {
                transform.position = new Vector3(
                    initialPos.x + (Random.Range(-1, 2) * 3),
                    initialPos.y + (Random.Range(-1, 2) * 3),
                    0
                );
            }
            yield return new WaitForSeconds(Random.Range(.1f, .5f));
        }
    }

    /*
     * Controls the attack animation of the enemy
     */
    private IEnumerator AttackAnim() {
        inAnimation = true;
        transform.position = new Vector3( initialPos.x - 15, initialPos.y - 15, 0);
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 3; i++) {
            transform.position = new Vector3(transform.position.x - 10, transform.position.y, 0);
            yield return new WaitForSeconds(0.1f);
            transform.position = new Vector3(transform.position.x + 10, transform.position.y, 0);
            yield return new WaitForSeconds(0.1f);
        }
        transform.position = initialPos;
        yield return new WaitForSeconds(0.5f);
        inAnimation = false;
    }

    /*
     * Method that chooses randomly an action for the enemy
     */
    public void DoSomething() {
        if (CheckEscape()) {
            StartCoroutine(Escape());
        } else if (CheckDoNothing()) {
            StartCoroutine(DoNothing());
        } else {
            target = Random.Range(0, 2);
            if (GameManager.party[0].hp == 0 && GameManager.party[1].hp == 0) {
                result = Action.Nothing;
                return;
            } else if (GameManager.party[target].hp == 0) {
                // If the target has died choose the other
                target = Mathf.Abs(target - 1);
            }

            // If know any skill
            if (enemy.skills.Count != 0) {
                // if can do a skill has a 1/3 of probability to use one
                if (enemy.CanDoAnySkill() && Random.Range(0, 3) == 0) {
                    StartCoroutine(Skill());
                    result = Action.Skill;
                } else {
                    StartCoroutine(Attack());
                }
            } else {
                StartCoroutine(Attack());
            }
        }
    }

    /*
     * Coroutine that attacks a target
     */
    private IEnumerator Attack() {
        int damage = GameController.CalculateDamage(enemy, GameManager.party[target]);
        GameManager.party[target].TakeDamage(damage);

        StartCoroutine(AttackAnim());
        while (inAnimation) {
            yield return null;
        }
        StartCoroutine(partyStats.ShowNumber(target, -damage, Color.red));
        partyStats.SetPartyStats();
        yield return new WaitForSeconds(1);
        result = Action.Attack;
    }

    /*
     * Coroutine that attacks a target with a random skill
     */
    private IEnumerator Skill() {
        Skill skill = enemy.GetRandomSkill();
        int damage = GameController.CalculateDamage(enemy, GameManager.party[target], skill);
        enemy.sp -= skill.cost;

        GameManager.party[target].TakeDamage(damage);

        GameController.dialogController.ShowBattleText(enemy.fullname + " uses " + skill.skillName);
        StartCoroutine(AttackAnim());
        while (inAnimation || DialogController.isTextShown) {
            yield return null;
        }
        
        StartCoroutine(partyStats.ShowNumber(target, -damage, Color.red));
        partyStats.SetPartyStats();
        yield return new WaitForSeconds(1);
        result = Action.Skill;
    }

    /*
     * Check if the enemy should escape
     */
    private bool CheckEscape() {
        if (GameManager.party[0].level > enemy.level) {
            int diff = GameManager.party[0].level - enemy.level;
            return Random.Range(0, 100) < (diff * 3);
        }
        return false;
    }

    /*
     * Coroutine that makes an enemy escape the battle
     */
    private IEnumerator Escape() {
        GameController.dialogController.ShowBattleText(enemy.fullname + " leaves the battle.");
        while (DialogController.isTextShown) {
            yield return null;
        }
        result = Action.Leave;
        gameObject.SetActive(false);
    }

    /*
     * Check if the enemy has to do nothing
     */
    private bool CheckDoNothing() {
        return Random.Range(0, 15) == 0;
    }

    /*
     * Coroutine that makes an enemy do nothing
     */
    private IEnumerator DoNothing() {
        GameController.dialogController.ShowBattleText(enemy.fullname + " do nothing.");
        while (DialogController.isTextShown) {
            yield return null;
        }
        result = Action.Nothing;
    }

    /*
     * Coroutine that shows a colored number in front of the enemy
     */
    public IEnumerator ShowNumber(int number, Color color) {
        Vector3 initialPos = text.transform.position;
        text.GetComponent<Text>().text = number.ToString();
        text.GetComponent<Text>().color = color;
        text.SetActive(true);
        for (int i = 0; i < 40; i++) {
            text.transform.position = new Vector3(
                text.transform.position.x,
                text.transform.position.y + 1,
                text.transform.position.z
            );
            yield return new WaitForSeconds(0.025f);
        }

        text.SetActive(false);
        text.transform.position = initialPos;
    }

    /*
     * Method that deactivates the enemy
     */
    public void Kill() {
        gameObject.SetActive(false);
    }

    /*
     * Controls the animation when the enemy is hit
     */
    public void HitAnim() {
        anim.SetTrigger("Slashed");
    }
}
