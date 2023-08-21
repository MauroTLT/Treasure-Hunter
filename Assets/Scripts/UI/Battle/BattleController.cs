using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleController : MonoBehaviour {

    public enum Position { Center, Up, Left, Right, Down };
    
    private PartyActionController partyAC;

    [Header("Enemy Data")]
    public GameObject pointer;
    public Image[] enemySprites = new Image[4];
    private GameObject[] enemy = new GameObject[4];
    public Enemy boss;
    [Space]
    [Header("First Stage Buttons")]
    public SwitchSprite firstAttack;
    public SwitchSprite firstRun;
    public SwitchSprite firstChange;
    [Space]
    [Header("Second Stage Buttons")]
    public SwitchSprite secondAttack;
    public SwitchSprite secondSkill;
    public SwitchSprite secondItem;
    public SwitchSprite secondDefend;
    [Space]
    [Header("Item and Skill Dialogs")]
    public ItemsBattleController itemsDialog;
    public SkillsBattleController skillDialog;

    private ArtifactController artifacts;
    private SoundController sound;
    private DialogController dialogController;
    private PartyStatsController partyStats;
    private Position btnPos;
    private int playerIndex, enemyIndex, turn;
    private bool inFirstStage, inSecondStage, selectingEnemy, axisInUse;

    private void Awake() {
        partyAC = GetComponent<PartyActionController>();
        enemy[0] = enemySprites[2].gameObject;
        enemy[1] = enemySprites[0].gameObject;
        enemy[2] = enemySprites[1].gameObject;
        enemy[3] = enemySprites[3].gameObject;
        pointer.SetActive(false);
        partyStats = GetComponentInChildren<PartyStatsController>();
        EnemyBattleController.partyStats = partyStats;
        sound = GameController.soundController;
        dialogController = GameController.dialogController;
        artifacts = FindObjectOfType<ArtifactController>();
        artifacts.gameObject.SetActive(false);
    }

    private void Start() {
        GameManager.inBattle = true;
        inFirstStage = true;
        btnPos = Position.Center;
        partyStats.SetPartyStats();
        playerIndex = (partyAC.IsDead(0)) ? 1 : 0;
        partyStats.SelectMemberDialog(playerIndex);
        firstAttack.SwapSprites(1);

        Camera.main.GetComponent<SoundController>().BattleMusic();
        SetMonsters();
    }

    private void Update() {
        if (ContinueBattle()) {
            GameOver();
            MovePointer();
            if (Input.GetKeyDown(KeyCode.Space)) {
                SetMonsters();
            } else if (Input.GetButtonDown("Submit")) {
                if (inFirstStage) {
                    switch (btnPos) {
                        case Position.Center: sound.MenuSubmitSound(); ChangeStage(); break;
                        case Position.Up: sound.MenuSubmitSound(); StartCoroutine(ChangeBattleSpeed()); break;
                        case Position.Down: sound.MenuSubmitSound(); StartCoroutine(RunOutOfBattle()); break;
                    }
                } else if (inSecondStage) {
                    SecondStageOptions();
                }
            } else if (Input.GetButtonDown("Cancel")) {
                if (inSecondStage) {
                    sound.MenuCancelSound();
                    if (selectingEnemy) {
                        selectingEnemy = false;
                        pointer.SetActive(false);
                    } else {
                        if (playerIndex == 0 || partyAC.IsDead(0)) {
                            ChangeStage();
                        } else {
                            partyStats.SetDefaultDialog(playerIndex);
                            partyStats.SetSelectedDialog(0);
                            playerIndex = 0;
                        }
                    }
                }
            }
        } else if (!AnyEnemyLeft()) {
            GameWin();
        }
    }

    private void SecondStageOptions() {
        switch (btnPos) {
            case Position.Center:
                sound.MenuSubmitSound();
                if (!selectingEnemy) {
                    SelectEnemy();
                } else {
                    partyAC.action[playerIndex] = "Attack";
                    partyAC.target[playerIndex] = enemy[enemyIndex];
                    selectingEnemy = false;
                    pointer.SetActive(false);
                    NextTurn();
                }
                break;
            case Position.Up:
                sound.MenuSubmitSound();
                if (!selectingEnemy) {
                    partyAC.action[playerIndex] = "Skill";
                    skillDialog.gameObject.SetActive(true);
                    skillDialog.enabled = true;
                } else {
                    partyAC.target[playerIndex] = enemy[enemyIndex];
                    selectingEnemy = false;
                    pointer.SetActive(false);
                    NextTurn();
                }
                break;
            case Position.Left:
                sound.MenuSubmitSound();
                partyAC.action[playerIndex] = "Item";
                itemsDialog.gameObject.SetActive(true);
                itemsDialog.enabled = true;
                break;
            case Position.Right:
                sound.MenuSubmitSound();
                partyAC.action[playerIndex] = "Defend";
                NextTurn();
                break;
        }
    }

    public void SelectEnemy() {
        for (int i = 0; i < enemy.Length; i++) {
            if (enemy[i].activeSelf) {
                enemyIndex = i;
                break;
            }
        }
        selectingEnemy = true;
        pointer.SetActive(true);
        ChangeEnemyPointerLocation();
    }

    public void NextTurn() {
        int old = playerIndex;
        playerIndex = Mathf.Abs(playerIndex - 1);
        partyStats.SetDefaultDialog(old);
        if ((partyAC.IsDead(playerIndex) && playerIndex == 1) || old == 1) {
            playerIndex = 0;
            turn++;
            ChangeStage();
            StartCoroutine(PartyTurn());
        } else {
            partyStats.SelectMemberDialog(playerIndex);
        }
    }

    private IEnumerator EnemyTurn() {
        foreach (GameObject e in enemy) {
            if (e.activeSelf) {
                EnemyBattleController ebc = e.GetComponent<EnemyBattleController>();
                ebc.DoSomething();
                while (ebc.result == EnemyBattleController.Action.NotDecided) {
                    yield return null;
                }
                ebc.result = EnemyBattleController.Action.NotDecided;
            }
        }
        partyAC.ResetDefenseStats();
        if (AnyEnemyLeft()) {
            firstAttack.transform.parent.gameObject.SetActive(true);
            if (partyAC.IsDead(0)) {
                playerIndex = 1;
            }
            partyStats.SelectMemberDialog(playerIndex);
            turn++;
            btnPos = Position.Center;
        } else {
            GameWin();
        }
    }

    private IEnumerator PartyTurn() {
        for (int i = 0; i < GameManager.party.Length; i++) {
            if (!partyAC.IsDead(i)) {
                StartCoroutine(partyAC.DoTheAction(i));
                while (!partyAC.finish) {
                    yield return null;
                }
                partyAC.finish = false;
            }
        }
        if (AnyEnemyLeft()) {
            StartCoroutine(EnemyTurn());
        } else {
            partyAC.ResetDefenseStats();
            GameWin();
        }
    }

    private IEnumerator RunOutOfBattle() {
        if (boss == null) {
            bool canRun = Random.Range(0, 2) == 0;
            if (canRun) {
                dialogController.ShowBattleText("You escape without problems.");
            } else {
                dialogController.ShowBattleText("The monsters won't let you run away!");
            }
            firstAttack.transform.parent.gameObject.SetActive(false);
            partyStats.SetDefaultDialog(0);
            while (DialogController.isTextShown) {
                yield return null;
            }
            if (canRun) {
                Destroy(gameObject);
            } else {
                turn++;
                StartCoroutine(EnemyTurn());
            }
        } else {
            dialogController.ShowBattleText("The enemy is too powerful to run away!");
        }
    }

    private IEnumerator ChangeBattleSpeed() {
        GameManager.battleSpeed = (GameManager.battleSpeed == 1) ? 2 : 1;
        dialogController.ShowBattleText((GameManager.battleSpeed == 1) ?
            "Battle Speed Changed to Normal" :
            "Battle Speed Changed to Fast");
        while (DialogController.isTextShown) {
            yield return null;
        }
        Time.timeScale = GameManager.battleSpeed;
    }

    private void ChangeStage() {
        inFirstStage = !inFirstStage;
        inSecondStage = !inSecondStage;
        firstAttack.transform.parent.gameObject.SetActive(inFirstStage);
        secondAttack.transform.parent.gameObject.SetActive(inSecondStage);
        if (inSecondStage) {
            secondAttack.SwapSprites(1);
            secondDefend.SwapSprites(0);
            secondItem.SwapSprites(0);
            secondSkill.SwapSprites(0);
        }
        if (turn % 2 == 1) {
            firstAttack.transform.parent.gameObject.SetActive(false);
        }
    }

    private void MovePointer() {
        Vector2 mov = GameManager.GetMov();
        if (selectingEnemy) {
            if (mov != Vector2.zero) {
                if (!axisInUse) {
                    axisInUse = true;
                    switch (GetPosition()) {
                        case Position.Left: enemyIndex--; break;
                        case Position.Right: enemyIndex++; break;
                    }
                    enemyIndex = Mathf.Clamp(enemyIndex, 0, enemy.Length);
                    ChangeEnemyPointerLocation();
                }
            } else {
                axisInUse = false;
            }
        } else {
            if (mov != Vector2.zero) {
                Position actualPos = GetPosition();
                if (btnPos == Position.Center) {
                    btnPos = actualPos;
                    ChangePointerLocation(Position.Center);
                    ChangePointerLocation(btnPos);
                } else if (btnPos != actualPos) {
                    ChangePointerLocation(btnPos);
                    btnPos = actualPos;
                    ChangePointerLocation(btnPos);
                }
            } else if (btnPos != Position.Center) {
                ChangePointerLocation(btnPos);
                btnPos = Position.Center;
                ChangePointerLocation(btnPos);
            }
        }
    }

    private void ChangeEnemyPointerLocation() {
        if (enemy[enemyIndex].activeSelf) {
            pointer.transform.SetParent(enemy[enemyIndex].gameObject.transform);
            pointer.GetComponent<RectTransform>().localPosition = new Vector3(-50, -50, 0);
        } else {
            int old = enemyIndex;
            if (GetPosition() == Position.Left) {
                for (int i = enemyIndex - 1; i >= 0; i--) {
                    if (enemy[i].activeSelf) {
                        enemyIndex = i;
                        break;
                    }
                }
                if (old == enemyIndex) { enemyIndex++; }
            } else {
                for (int i = enemyIndex + 1; i < enemy.Length; i++) {
                    if (enemy[i].activeSelf) {
                        enemyIndex = i;
                        break;
                    }
                }
                if (old == enemyIndex) { enemyIndex--; }
            }
            ChangeEnemyPointerLocation();
        }
    }

    private void ChangePointerLocation(Position pos) {
        if (inFirstStage) {
            switch (pos) {
                case Position.Up: firstChange.SwapSprites(); break;
                case Position.Down: firstRun.SwapSprites(); break;
                default: firstAttack.SwapSprites(); break;
            }
        } else if (inSecondStage) {
            switch (pos) {
                case Position.Up: secondSkill.SwapSprites(); break;
                case Position.Left: secondItem.SwapSprites(); break;
                case Position.Right: secondDefend.SwapSprites(); break;
                default: secondAttack.SwapSprites(); break;
            }
        }
    }

    private bool ContinueBattle() {
        return !DialogController.isTextShown &&
            turn % 2 == 0 && 
            AnyEnemyLeft() &&
            !itemsDialog.gameObject.activeSelf;
    }

    private void GameOver() {
        if (partyAC.IsDead(0) && partyAC.IsDead(1)) {
            GameManager.ChangeScene("GameOver");
        }
    }

    private void GameWin() {
        if (!AnyEnemyLeft() && GameManager.inBattle) {
            GameManager.inBattle = false;
            StartCoroutine(EndBattle());
        }
    }

    private IEnumerator EndBattle() {
        dialogController.ShowBattleText("Battle Finished!");
        while (DialogController.isTextShown) {
            yield return null;
        }
        dialogController.ShowBattleText("You gained " + partyAC.goldGained + " pieces of gold.");
        while (DialogController.isTextShown) {
            yield return null;
        }
        GameManager.gold += partyAC.goldGained;

        dialogController.ShowBattleText("Each member gained " + partyAC.xpGained + " XP points.");
        while (DialogController.isTextShown) {
            yield return null;
        }

        for (int i = 0; i < 2; i++) {
            if (GameManager.party[i].hp != 0) {
                int newSkill = GameManager.party[i].skills.Count;
                if (GameManager.party[i].SumXP(partyAC.xpGained)) {
                    dialogController.ShowBattleText(GameManager.party[i].fullname + " has leveled up to level " + GameManager.party[i].level);
                    while (DialogController.isTextShown) {
                        yield return null;
                    }
                    if (newSkill < GameManager.party[i].skills.Count) {
                        dialogController.ShowBattleText(GameManager.party[i].fullname + " has learned the skill \"" + GameManager.party[i].skills[newSkill].skillName + "\"");
                        while (DialogController.isTextShown) {
                            yield return null;
                        }
                    }
                }
            }
        }

        foreach (Item item in partyAC.itemsGained) {
            if (item != null) {
                dialogController.ShowBattleText("A monster drop one " + item.itemName);
                while (DialogController.isTextShown) {
                    yield return null;
                }
                Bag.Instance.PutInBag(item);
            }
        }

        if (boss != null) {
            GameManager.lastMileStone++;
        }

        Destroy(gameObject);
    }

    private bool AnyEnemyLeft() {
        foreach (GameObject e in enemy) {
            if (e.activeSelf) {
                return true;
            }
        }
        return false;
    }

    private Position GetPosition() {
        Vector2 mov = GameManager.GetMov();
        if (mov.x == 1) {
            return Position.Right;
        } else if (mov.x == -1) {
            return Position.Left;
        } else if (mov.y == -1) {
            return Position.Down;
        } else if (mov.y == 1) {
            return Position.Up;
        }
        return Position.Center;
    }

    private void SetMonsters() {
        if (boss == null) {
            Enemy[] enemyStats = GameManager.actualZone.enemyTable.NewEncounter();

            for (int i = 0; i < enemySprites.Length; i++) {
                if (enemyStats[i] != null) {
                    enemySprites[i].gameObject.SetActive(true);
                    enemySprites[i].sprite = enemyStats[i].sprite;
                    enemySprites[i].SetNativeSize();
                    enemySprites[i].GetComponent<EnemyBattleController>().enemy = enemyStats[i];
                } else {
                    enemySprites[i].gameObject.SetActive(false);
                }
            }
        } else {
            enemySprites[1].gameObject.SetActive(false);
            enemySprites[2].gameObject.SetActive(false);
            enemySprites[3].gameObject.SetActive(false);

            enemySprites[0].gameObject.SetActive(true);
            enemySprites[0].sprite = boss.sprite;
            enemySprites[0].SetNativeSize();
            enemySprites[0].GetComponent<EnemyBattleController>().enemy = boss.Clone();
        }
    }

    public int GetPlayerIndex() {
        return playerIndex;
    }

    private void OnDestroy() {
        Time.timeScale = 1;
        GameManager.gamePaused = false;
        GameManager.inBattle = false;

        if (!(partyAC.IsDead(0) && partyAC.IsDead(1))) {
            artifacts.gameObject.SetActive(true);
            Camera.main.GetComponent<SoundController>().ZoneMusic();
            GameManager.party[0].hp = Mathf.Clamp(GameManager.party[0].hp, 1, GameManager.party[0].maxHp);
            GameManager.party[1].hp = Mathf.Clamp(GameManager.party[1].hp, 1, GameManager.party[1].maxHp);
        } else {
            for (int i = 0; i < 2; i++) {
                GameManager.party[i].HealAll();
            }
            GameManager.SaveGame(false);
        }
    }
}