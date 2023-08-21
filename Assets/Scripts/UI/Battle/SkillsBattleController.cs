using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillsBattleController : MonoBehaviour {

    public GameObject pointerSkills;
    public GameObject[] slots;
    private GameObject itemDescriptor;

    public PartyStatsController partySC;
    public PartyActionController partyAC;
    public BattleController battle;

    private PartyMember member;
    private int indexToMove = -1;

    private int skillIndex;
    private int page;
    private bool axisInUse = false;

    private void Start() {
        itemDescriptor = (Resources.FindObjectsOfTypeAll(typeof(ItemDescriptor))[0] as ItemDescriptor).gameObject;
    }

    private void OnEnable() {
        member = GameManager.party[battle.GetPlayerIndex()];
        page = 0;
        skillIndex = 0;
        indexToMove = -1;
        axisInUse = false;
        ChangePointerLocation();
        ChangePage();
    }

    void LateUpdate() {
        if (!GameManager.showingItem) {
            CheckButtons();
            MovePointer();
        }
    }

    // Checks the Submit and Cancel buttons
    private void CheckButtons() {
        if (!axisInUse) {
            // The user pressed Submit
            if (Input.GetButtonUp("Submit")) {
                UseSkill();
            } else if (Input.GetButtonUp("Cancel")) {
                enabled = false;
                gameObject.SetActive(false);
            } else if (Input.GetButtonUp("Y - Artifact")) {
                /*Skill skill = ActualSkill();
                if (skill != null) {
                    ItemDescriptor.item = skill;
                    itemDescriptor.SetActive(true);
                }*/
            }
        }
    }

    private void UseSkill() {
        Skill skill = ActualSkill();
        // If the item selected exists
        if (skill != null) {
            // If the item is one that can be used
            if (CanUseSkill(skill)) {
                partyAC.thing[battle.GetPlayerIndex()] = skill;

                enabled = false;
                gameObject.SetActive(false);
                battle.SelectEnemy();
            }
        }
    }

    private void MovePointer() {
        Vector2 mov = GameManager.GetMov();
        if (mov != Vector2.zero) {
            if (!axisInUse) {
                if (mov.x == 1) {
                    if (skillIndex % 2 == 0) {
                        skillIndex++;
                    } else {
                        if (page != 4) {
                            skillIndex--;
                            page++;
                            ChangePage();
                        }
                    }
                } else if (mov.x == -1) {
                    if (skillIndex % 2 != 0) {
                        skillIndex--;
                    } else {
                        if (page != 0) {
                            skillIndex++;
                            page--;
                            ChangePage();
                        }
                    }
                } else if (mov.y == 1) {
                    skillIndex = (skillIndex > 1) ? skillIndex - 2 : skillIndex;
                } else if (mov.y == -1) {
                    skillIndex = (skillIndex < 6) ? skillIndex + 2 : skillIndex;
                }
                ChangePointerLocation();
                axisInUse = true;
            }
        } else {
            axisInUse = false;
        }
    }

    private void ChangePointerLocation() {
        pointerSkills.transform.localPosition = new Vector3(
            slots[skillIndex].transform.localPosition.x - 205,
            slots[skillIndex].transform.localPosition.y,
            slots[skillIndex].transform.localPosition.z
        );
    }

    private void PopulateSlots() {
        int numSlot = 0;
        for (int i = (page * slots.Length); i < ((page + 1) * slots.Length); i++) {
            Skill skill = null;
            if (i < member.skills.Count) {
                skill = member.skills[i];
            }
            slots[numSlot].transform.GetChild(0).gameObject.SetActive(false);
            if (skill != null) {
                slots[numSlot].transform.GetChild(1).GetComponent<Text>().text = skill.skillName;
                slots[numSlot].transform.GetChild(1).GetComponent<Text>().color = (indexToMove == i) ? Color.cyan : (!CanUseSkill(skill)) ? Color.gray : Color.white;
                slots[numSlot].transform.GetChild(2).GetComponent<Text>().text = skill.cost.ToString();
            } else { // NO ITEM IN THIS POSITION
                slots[numSlot].transform.GetChild(1).GetComponent<Text>().text = "__________";
                slots[numSlot].transform.GetChild(1).GetComponent<Text>().color = (indexToMove == i) ? Color.cyan : Color.white;
                slots[numSlot].transform.GetChild(2).GetComponent<Text>().text = "-";
            }
            numSlot++;
        }
    }

    private Skill ActualSkill() {
        if ((skillIndex + (page * 8)) < member.skills.Count) {
            return member.skills[skillIndex + (page * 8)];
        }
        return null;
    }

    private void ChangePage() {
        GameObject.Find("PageNumber").GetComponent<Text>().text = (page + 1) + " / 5";
        PopulateSlots();
    }

    private bool CanUseSkill(Skill skill) {
        return (skill.cost <= member.sp);
    }
}
