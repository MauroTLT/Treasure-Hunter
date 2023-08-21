using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BootController : MonoBehaviour {
    [Space]
    [Header("Party Stats")]
    public PartyMember[] baseStats;
    [Space]
    public PartyMember[] saveStats;

    [Space]
    [Header("Milestones")]
    public MileStones mileStones;

    [Space]
    [Header("GameObjects")]
    public GameObject preOptions;
    public GameObject options;

    [Space]
    [Header("Buttons")]
    public Button newGame;
    public Button loadGame;
    public Button quit;

    private SoundController sound;

    private int optionIndex;

    private bool inFade;
    private bool axisInUse = false;

    private void Awake() {
        sound = Camera.main.GetComponent<SoundController>();
        string lastTime = PlayerPrefs.GetString("PlayTime", "");
        // Check if a game was saved, if not destroy the load button
        if (lastTime.Equals("")) {
            newGame.gameObject.transform.Translate(new Vector3(0, -50, 0));
            quit.gameObject.transform.Translate(new Vector3(0, 50, 0));
            Destroy(loadGame.gameObject);

            SelectButton(newGame);
        } else {
            SelectButton(loadGame);
        }
    }

    private void Update() {
        if (!inFade) {
            if (Input.anyKeyDown && preOptions.activeSelf) {
                sound.MenuSubmitSound();
                StartCoroutine(FadeTitle());
            } else {
                CheckButton();
                MovePointer();
            }
        }
    }

    private void CheckButton() {
        if (Input.GetButtonDown("Submit")) {
            // Check what option was selected
            switch (optionIndex) {
                case 0: NewGame(); break;
                case 1: LoadGame(); break;
                case 2: Quit(); break;
            }
        }
    }

    /*
     * Move the pointer between the options
     */
    private void MovePointer() {
        Vector2 mov = GameManager.GetMov();
        if (mov != Vector2.zero) {
            if (!preOptions.activeSelf && !axisInUse) {
                if (mov.y == 1) {
                    switch (optionIndex) {
                        case 0: SelectButton(quit); break;
                        case 1: SelectButton(newGame); break;
                        case 2: SelectButton((loadGame != null) ? loadGame : newGame); break;
                    }
                } else if (mov.y == -1) {
                    switch (optionIndex) {
                        case 0: SelectButton((loadGame != null) ? loadGame : quit); break;
                        case 1: SelectButton(quit); break;
                        case 2: SelectButton(newGame); break;
                    }
                }
                axisInUse = true;
            }
        } else {
            axisInUse = false;
        }
    } 

    /*
     * Prepare all to start a new game
     */
    public void NewGame() {
        sound.MenuSubmitSound();

        GameManager.party[0] = baseStats[0].Clone();
        GameManager.party[1] = baseStats[1].Clone();

        Zone merix = Resources.Load("ScriptableObjects/Zones/Merix") as Zone;
        GameManager.ChangeScene(merix);
    }

    /*
     * Load all the data saved of the last game
     */
    public void LoadGame() {
        sound.MenuSubmitSound();

        GameManager.LoadPlayerPrefs();
        if (mileStones.CanUseBomb(GameManager.lastMileStone)) {
            GameManager.NewArtifact("Bomb");
        } else if (mileStones.CanUseHook(GameManager.lastMileStone)) {
            GameManager.NewArtifact("Hook");
        }

        GameManager.party[0] = new PartyMember();
        GameManager.party[1] = new PartyMember();

        JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString("PartyMember_0"), GameManager.party[0]);
        JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString("PartyMember_1"), GameManager.party[1]);

        GameManager.ChangeScene(GameManager.actualZone);
    }

    public void Quit() {
        sound.MenuSubmitSound();
        Application.Quit();
    }

    /*
     * Animation that fades the title and shows the options
     */
    private IEnumerator FadeTitle() {
        inFade = true;
        preOptions.GetComponent<Animator>().SetTrigger("Fade");
        yield return new WaitForSeconds(1F);

        options.transform.localScale = new Vector3(0, 0, 1);
        options.SetActive(true);
        options.GetComponent<Animator>().SetTrigger("Fade");

        yield return new WaitForSeconds(0.5F);
        preOptions.SetActive(false);

        inFade = false;
    }

    /*
     * Move the cursor to the button passed by argument
     */
    public void SelectButton(Button button) {
        sound.MenuPointerSound();
        Button lastBtn = null;
        switch (optionIndex) {
            case 0: lastBtn = newGame; break;
            case 1: lastBtn = loadGame; break;
            case 2: lastBtn = quit; break;
        }
        foreach (var item in lastBtn.GetComponentsInChildren<Image>()) {
            item.enabled = false;
        }

        optionIndex = (button.Equals(newGame)? 0 : button.Equals(loadGame) ? 1 : 2);
        foreach (var item in button.GetComponentsInChildren<Image>()) {
            item.enabled = true;
        }
    }
}
