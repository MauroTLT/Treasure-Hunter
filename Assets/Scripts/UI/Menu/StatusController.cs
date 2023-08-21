using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusController : MonoBehaviour {

    public GameObject mainMenu, statsDialog, partyMember, xpDialog;
    public GameObject atkStat, dexStat, defStat, resStat;
    public GameObject[] equipmentSlots;

    private SoundController sound;
    private PartyMember member;
    private bool axisInUse = false;

    private const string ICONS_PATH = "Sprites/Menu/Icons/ItemIcons/";

    private void OnEnable() {
        sound = Camera.main.GetComponent<SoundController>();
        if (member == null) {
            member = GameManager.party[0];
        }
        axisInUse = false;
        XpStats();
        ShowMemberStats();
        PopulateEquipment();
        
    }

    void LateUpdate() {
        if (!GameManager.showingItem) {
            CheckButtons();
        }
    }

    private void CheckButtons() {
        if (!axisInUse) {
            // The user pressed Cancel
            if (Input.GetButtonUp("Cancel")) {
                sound.MenuCancelSound();
                gameObject.SetActive(false);
                mainMenu.SetActive(true);
            } else if (Input.GetButtonUp("Left Trigger") || Input.GetButtonUp("Right Trigger")) {
                sound.MenuSubmitSound();
                member = (member == GameManager.party[0]) ? GameManager.party[1] : GameManager.party[0];
                OnEnable();
            }
        }
    }

    private void XpStats() {
        xpDialog.transform.GetChild(0).GetComponent<Text>().text = member.totalXp.ToString();
        xpDialog.transform.GetChild(1).GetComponent<Text>().text = GameController.XpToNextLvl(member.totalXp).ToString();
    }

    private void ShowMemberStats() {
        partyMember.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = member.sprite;
        Transform stats = partyMember.transform.Find("Stats");
        stats.GetChild(0).GetComponent<Text>().text = member.fullname;
        stats.GetChild(1).GetComponent<Text>().text = "Lvl: " + member.level;

        stats.GetChild(3).GetComponent<Slider>().maxValue = member.maxHp;
        stats.GetChild(3).GetComponent<Slider>().value = member.hp;
        stats.GetChild(3).GetComponentInChildren<Text>().text = member.hp + "/" + member.maxHp;

        stats.GetChild(5).GetComponent<Slider>().maxValue = member.maxSp;
        stats.GetChild(5).GetComponent<Slider>().value = member.sp;
        stats.GetChild(5).GetComponentInChildren<Text>().text = member.sp + "/" + member.maxSp;

        atkStat.transform.Find("Amount").GetComponent<Text>().text = member.attack.ToString();
        dexStat.transform.Find("Amount").GetComponent<Text>().text = member.dextery.ToString();
        defStat.transform.Find("Amount").GetComponent<Text>().text = member.defense.ToString();
        resStat.transform.Find("Amount").GetComponent<Text>().text = member.resistance.ToString();
    }

    private void PopulateEquipment() {
        GameObject slot = equipmentSlots[0];
        if (member.weapon != null) {
            slot.transform.GetChild(0).GetComponent<Image>().sprite = member.weapon.sprite;
            slot.transform.GetChild(1).GetComponent<Text>().text = member.weapon.itemName;
        } else {
            slot.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(ICONS_PATH + "sword_placeholder");
            slot.transform.GetChild(1).GetComponent<Text>().text = "Not equipped";
        }
        slot = equipmentSlots[1];
        if (member.helmet != null) {
            slot.transform.GetChild(0).GetComponent<Image>().sprite = member.helmet.sprite;
            slot.transform.GetChild(1).GetComponent<Text>().text = member.helmet.itemName;
        } else {
            slot.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(ICONS_PATH + "helmet_placeholder");
            slot.transform.GetChild(1).GetComponent<Text>().text = "Not equipped";
        }
        slot = equipmentSlots[2];
        if (member.chestplate != null) {
            slot.transform.GetChild(0).GetComponent<Image>().sprite = member.chestplate.sprite;
            slot.transform.GetChild(1).GetComponent<Text>().text = member.chestplate.itemName;
        } else {
            slot.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(ICONS_PATH + "chestplate_placeholder");
            slot.transform.GetChild(1).GetComponent<Text>().text = "Not equipped";
        }
        slot = equipmentSlots[3];
        if (member.shield != null) {
            slot.transform.GetChild(0).GetComponent<Image>().sprite = member.shield.sprite;
            slot.transform.GetChild(1).GetComponent<Text>().text = member.shield.itemName;
        } else {
            slot.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(ICONS_PATH + "shield_placeholder");
            slot.transform.GetChild(1).GetComponent<Text>().text = "Not equipped";
        }
        slot = equipmentSlots[4];
        if (member.necklace != null) {
            slot.transform.GetChild(0).GetComponent<Image>().sprite = member.necklace.sprite;
            slot.transform.GetChild(1).GetComponent<Text>().text = member.necklace.itemName;
        } else {
            slot.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(ICONS_PATH + "necklace_placeholder");
            slot.transform.GetChild(1).GetComponent<Text>().text = "Not equipped";
        }
    }

}
