using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogController : MonoBehaviour {

    public enum Result { None = 0, Yes = 1, No = 2};

    private SoundController sound;
    private GameObject textDialog, itemDialog, battleDialog, confirmDialog, pointer;
    private Text message, itemMessage, battleMessage, txtYes, txtNo;
    private int pointerIndex;
    private bool axisInUse, textPrinting, fastText, alreadyGamePaused;
    [HideInInspector]
    public Result result;
    public static bool isTextShown = false;

    private void Awake() {
        sound = Camera.main.GetComponent<SoundController>();
        textDialog = transform.Find("TextDialog").gameObject;
        itemDialog = transform.Find("ItemDialog").gameObject;
        battleDialog = transform.Find("BattleDialog").gameObject;
        itemMessage = itemDialog.transform.GetChild(0).GetComponent<Text>();
        battleMessage = battleDialog.transform.GetChild(0).GetComponent<Text>();
        message = textDialog.transform.GetChild(0).GetComponent<Text>();
        confirmDialog = transform.Find("Confirmation").gameObject;
        txtYes = confirmDialog.transform.GetChild(0).GetComponent<Text>();
        txtNo = confirmDialog.transform.GetChild(1).GetComponent<Text>();
        pointer = txtYes.transform.GetChild(0).gameObject;
        ResetVariables();
    }

    private void ResetVariables() {
        Input.ResetInputAxes();
        pointerIndex = 0;
        ChangePointerLocation();
        fastText = false;
        axisInUse = false;
        textPrinting = false;
        alreadyGamePaused = false;
        result = Result.None;
    }

    private void Update() {
        if (isTextShown) {
            Vector2 mov = GameManager.GetMov();
            if (!textPrinting && (Input.GetButtonUp("Submit") || Input.GetButtonUp("Cancel"))) {
                if (confirmDialog.activeSelf) {
                    confirmDialog.SetActive(false);
                    result = (Input.GetButtonUp("Cancel")) ? Result.No : (pointerIndex == 0) ? Result.Yes : Result.No;
                    if (result == Result.No) {
                        sound.MenuCancelSound();
                    } else {
                        sound.MenuSubmitSound();
                    }
                } else {
                    sound.MenuSubmitSound();
                }
                Input.ResetInputAxes();
                textDialog.SetActive(false);
                itemDialog.SetActive(false);
                battleDialog.SetActive(false);
                if (!alreadyGamePaused) {
                    GameManager.gamePaused = false;
                }
                isTextShown = false;
            } else if (textPrinting && (Input.GetButtonUp("Submit") || Input.GetButtonUp("Cancel"))) {
                fastText = true;
            }
            if (mov != Vector2.zero && !textPrinting) {
                if (!axisInUse) {
                    if (mov.y != 0) {
                        sound.MenuPointerSound();
                        pointerIndex = Mathf.Abs(pointerIndex - 1);
                    }
                    ChangePointerLocation();
                    axisInUse = true;
                }
            } else {
                axisInUse = false;
            }
        }
    }

    public void ShowText(string text, bool showConfirm) {
        if (!isTextShown) {
            sound.MenuSubmitSound();
            ResetVariables();
            textDialog.SetActive(true);
            alreadyGamePaused = GameManager.gamePaused;
            if (!GameManager.gamePaused) {
                GameManager.gamePaused = true;
            }
            isTextShown = true;

            StartCoroutine(PrintText(text, showConfirm));
        }
    }

    public void ShowSubText(string text) {
        if (!isTextShown) {
            ResetVariables();
            itemDialog.SetActive(true);
            alreadyGamePaused = GameManager.gamePaused;
            if (!GameManager.gamePaused) {
                GameManager.gamePaused = true;
            }
            
            isTextShown = true;

            itemMessage.text = text;
        }
    }

    public void ShowBattleText(string text) {
        if (!isTextShown) {
            ResetVariables();
            battleDialog.SetActive(true);
            alreadyGamePaused = GameManager.gamePaused;
            if (!GameManager.gamePaused) {
                GameManager.gamePaused = true;
            }
            isTextShown = true;

            battleMessage.text = text;
        }
    }

    public void ShowItem(Item item) {
        ShowSubText("You found " + ((item.name[0].Equals("A")) ? "an " : "a ") + item.itemName);
    }

    public void PartyHealed() {
        ShowSubText("Your party is fully Healed!");
    }

    public void GameSaved() {
        ShowSubText("Game Saved");
    }

    public void TimeBackConfirm() {
        ShowText("Are you sure to travel back in time, to when you enter the room?", true);
    }

    IEnumerator PrintText(string text, bool showConfirm) {
        textPrinting = true;
        message.text = "";
        for (int i = 0; i < text.Length; i++) {
            if (fastText) {
                message.text = text;
                break;
            } else {
                message.text += text[i];
                yield return new WaitForSeconds(.025f);
            }
        }
        if (showConfirm) {
            confirmDialog.SetActive(true);
        }
        textPrinting = false;
    }

    private void ChangePointerLocation() {
        pointer.transform.SetParent((pointerIndex == 0) ? txtYes.transform : txtNo.transform);
        pointer.GetComponent<RectTransform>().localPosition = new Vector3(-105, 0, 0);
    }
}
