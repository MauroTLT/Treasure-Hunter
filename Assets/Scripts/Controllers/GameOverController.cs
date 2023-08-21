using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour {

    [Space]
    [Header("Buttons")]
    public Button respawn;
    public Button exit;

    private SoundController sound;

    private int optionIndex;

    private bool inFade;
    private bool axisInUse = false;

    private void Start() {
        sound = Camera.main.GetComponent<SoundController>();
        SelectButton(respawn);
    }

    private void Update() {
        if (!inFade) {
            CheckButton();
            MovePointer();
        }
    }

    private void CheckButton() {
        if (Input.GetButtonDown("Submit")) {
            sound.MenuSubmitSound();
            switch (optionIndex) {
                case 0: Respawn(); break;
                case 1: Exit(); break;
            }
        }
    }

    private void MovePointer() {
        Vector2 mov = GameManager.GetMov();
        if (mov != Vector2.zero) {
            if (!axisInUse) {
                if (mov.x != 0) {
                    optionIndex = Mathf.Abs(optionIndex - 1);
                    SelectButton((optionIndex == 0)? respawn : exit);
                }
                axisInUse = true;
            }
        } else {
            axisInUse = false;
        }
    }

    /*
     * Move the pointer to the button passed by argument
     */
    public void SelectButton(Button button) {
        sound.MenuPointerSound();
        Button lastBtn = (button == respawn) ? exit : respawn;
        foreach (var item in lastBtn.GetComponentsInChildren<Image>()) {
            item.enabled = false;
        }

        foreach (var item in button.GetComponentsInChildren<Image>()) {
            item.enabled = true;
        }
    }

    /*
     * Return to last save location
     */
    public void Respawn() {
        sound.MenuSubmitSound();

        string zone = PlayerPrefs.GetString("Zone", "Merix");
        GameManager.actualZone = Resources.Load("ScriptableObjects/Zones/" + zone) as Zone;
        GameManager.isGameLoaded = true;
        GameManager.ChangeScene(GameManager.actualZone);
    }

    /*
     * Return to boot scene
     */
    public void Exit() {
        sound.MenuSubmitSound();
        GameManager.ChangeScene("Boot");
    }
}
