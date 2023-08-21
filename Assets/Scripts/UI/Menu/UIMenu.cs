using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour {

    public GameObject background, principalMenu, pointer, statusDialog;
    public GameObject[] menus;
    public Text[] commands;
    public Text gold, time;

    private ArtifactController artifacts;
    private SoundController sound;

    private int index = 0;
    private bool axisInUse = false;

    private void Awake() {
        sound = Camera.main.GetComponent<SoundController>();
        GameManager.gamePaused = true;
        GameObject.FindGameObjectWithTag("Crossfade").GetComponent<Crossfade>().MenuTransition(background);
        gold.text = GameManager.gold.ToString();
        artifacts = FindObjectOfType<ArtifactController>();
        if (artifacts != null) {
            artifacts.gameObject.SetActive(false);
        }
        ChangePointerLocation();
        SetPartyStats();
    }

    private void LateUpdate() {
        if (principalMenu.activeSelf && !statusDialog.activeSelf) {
            statusDialog.SetActive(true);
            SetPartyStats();
        }
        if (background.activeSelf && principalMenu.activeSelf && !Crossfade.inTransition) {
            MovePointer();
            if (!axisInUse) {
                if (Input.GetButtonUp("Submit")) {
                    sound.MenuSubmitSound();
                    menus[index].SetActive(true);
                    if (index != 0) {
                        statusDialog.SetActive(false);
                    }
                    principalMenu.SetActive(false);
                } else if (Input.GetButtonUp("X - Menu") || Input.GetButtonUp("Cancel")) {
                    sound.MenuCancelSound();
                    StartCoroutine(CloseMenu());
                }
            }
            
        }
        time.text = GameManager.playTime;
    }

    private IEnumerator CloseMenu() {
        GameObject.FindGameObjectWithTag("Crossfade").GetComponent<Crossfade>().Transition("Start");
        yield return new WaitForSeconds(Crossfade.transitionTime);
        GameObject.FindGameObjectWithTag("Crossfade").GetComponent<Crossfade>().Transition("End");
        Destroy(gameObject);
    }

    public static void SetPartyStats() {
        GameObject[] partyMembers = GameObject.FindGameObjectsWithTag("UI-PartyMember");
        for (int i = 0; i < partyMembers.Length; i++) {
            PartyMember member = GameManager.party[i];
            Transform stats = partyMembers[i].transform.GetChild(0);

            GameObject.Find("CurrentZone").GetComponentInChildren<Text>().text = GameManager.actualZone.zoneName;
            stats.Find("Name").GetComponent<Text>().text = member.fullname;
            stats.Find("Level").GetComponent<Text>().text = "Lvl: " + member.level;

            stats.Find("Health").GetComponent<Slider>().maxValue = member.maxHp;
            stats.Find("Health").GetComponent<Slider>().value = member.hp;
            stats.Find("Health").Find("Text").GetComponent<Text>().text = member.hp + "/" + member.maxHp;

            stats.Find("Magic").GetComponent<Slider>().maxValue = member.maxSp;
            stats.Find("Magic").GetComponent<Slider>().value = member.sp;
            stats.Find("Magic").Find("Text").GetComponent<Text>().text = member.sp + "/" + member.maxSp;
        }
    }

    private void MovePointer() {
        Vector2 mov = GameManager.GetMov();
        if (mov != Vector2.zero) {
            if (!axisInUse) {
                sound.MenuPointerSound();
                if (mov.x == 1) {
                    index = (index == 1) ? 0 : (index == 3) ? 2 : index + 1;
                } else if (mov.x == -1) {
                    index = (index == 0) ? 1 : (index == 2) ? 3 : index - 1;
                } else if (mov.y == 1) {
                    index = (index == 0) ? 2 : (index == 1) ? 3 : index - 2;
                } else if (mov.y == -1) {
                    index = (index == 2) ? 0 : (index == 3) ? 1 : index + 2;
                }
                ChangePointerLocation();
                axisInUse = true;
            }
        } else {
            axisInUse = false;
        }
    }

    private void ChangePointerLocation() {
        for (int i = 0; i < commands.Length; i++) {
            commands[i].GetComponent<Text>().color = (index == i) ? Color.white : Color.gray;
        }
        pointer.transform.SetParent(commands[index].gameObject.transform);
        pointer.GetComponent<RectTransform>().localPosition = new Vector3(-100, 0, 0);
    }

    private void OnDestroy() {
        if (artifacts != null) {
            artifacts.gameObject.SetActive(true);
        }
        GameManager.gamePaused = false;
    }
}
