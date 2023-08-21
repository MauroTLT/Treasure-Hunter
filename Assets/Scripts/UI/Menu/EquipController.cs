using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipController : MonoBehaviour {

    public GameObject mainMenu, itemsDialog, equipedDialog, statsDialog, partyMember, pointerEquipment, pointerItems;
    public GameObject atkStat, dexStat, defStat, resStat;
    public GameObject[] equipmentSlots, itemSlots;
    public Sprite statUp, statDown;

    private GameObject itemDescriptor;
    private SoundController sound;
    private PartyMember member;
    private Item[] filterBag;
    private int equipmentIndex = 0;
    private int itemIndex = 0;
    private int page = 0;
    private bool inItems = false;
    private bool axisInUse = false;
    private const string ICONS_PATH = "Sprites/Menu/Icons/ItemIcons/";

    private void OnEnable() {
        itemDescriptor = GameController.itemDescriptor;
        sound = Camera.main.GetComponent<SoundController>();
        if (member == null) {
            member = GameManager.party[0];
        }
        filterBag = new Item[1];
        equipmentIndex = 0;
        itemIndex = 0;
        inItems = false;
        axisInUse = false;
        ShowMemberStats();
        PopulateEquipment();
        ChangePointerLocation();
        itemsDialog.SetActive(false);
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
            Item item = ActualItem();
            // The user pressed Submit
            if (Input.GetButtonUp("Submit")) {
                // If its selecting the equipment option
                if (!inItems) {
                    filterBag = Bag.Instance.GetAllOf(equipmentIndex);
                    if (filterBag.Length > 0) {
                        sound.MenuSubmitSound();
                        itemsDialog.SetActive(true);
                        equipedDialog.SetActive(false);
                        statsDialog.SetActive(true);
                        ChangePage();
                        inItems = true;
                        ChangePointerLocation();
                        SetMemberStats(ActualItem());
                    } else {
                        sound.ErrorSound();
                    }
                } else {
                    if (item != null) {
                        sound.MenuSubmitSound();
                        if (equipmentIndex == 0) {
                            if (((Weapon)item).canUse == Weapon.Type.Both ||
                                member == GameManager.party[0] && ((Weapon)item).canUse == Weapon.Type.Human ||
                                member == GameManager.party[1] && ((Weapon)item).canUse == Weapon.Type.Animal) {
                                member.SetWeapon((Weapon)ActualItem());
                            } else {
                                sound.ErrorSound();
                                return;
                            }
                        } else if (equipmentIndex == 1) {
                            member.SetHelmet((Helmet)ActualItem());
                        } else if (equipmentIndex == 2) {
                            member.SetChestplate((Chestplate)ActualItem());
                        } else if (equipmentIndex == 3) {
                            member.SetShield((Shield)ActualItem());
                        } else {
                            member.SetNecklace((Necklace)ActualItem());
                        }
                        filterBag = Bag.Instance.GetAllOf(equipmentIndex);
                        ChangePage();
                        PopulateEquipment();
                        ButtonBack();
                    } else {
                        sound.ErrorSound();
                    }
                }
                // The user pressed Cancel
            } else if (Input.GetButtonUp("Cancel")) {
                ButtonBack();
            } else if (Input.GetButtonUp("Y - Artifact")) {
                sound.MenuSubmitSound();
                if (inItems && item != null) {
                    ItemDescriptor.item = item;
                    itemDescriptor.SetActive(true);
                } else if (!inItems) {
                    switch (equipmentIndex) {
                        case 0: item = member.weapon; break;
                        case 1: item = member.helmet; break;
                        case 2: item = member.chestplate; break;
                        case 3: item = member.shield; break;
                        case 4: item = member.necklace; break;
                    }
                    if (item != null) {
                        ItemDescriptor.item = item;
                        itemDescriptor.SetActive(true);
                    }
                }
            } else if (Input.GetButtonUp("X - Menu")) {
                sound.MenuCancelSound();
                if (!inItems) {
                    switch (equipmentIndex) {
                        case 0: member.SetWeapon(null); break;
                        case 1: member.SetHelmet(null); break;
                        case 2: member.SetChestplate(null); break;
                        case 3: member.SetShield(null); break;
                        case 4: member.SetNecklace(null); break;
                    }
                    PopulateEquipment();
                }
            } else if (!inItems && (Input.GetButtonUp("Left Trigger") || Input.GetButtonUp("Right Trigger"))) {
                sound.MenuSubmitSound();
                member = (member == GameManager.party[0]) ? GameManager.party[1] : GameManager.party[0];
                OnEnable();
            }
        }
    }

    private void ButtonBack() {
        // If its selecting the command option
        sound.MenuCancelSound();
        if (!inItems) {
            gameObject.SetActive(false);
            mainMenu.SetActive(true);
        } else {
            itemsDialog.SetActive(false);
            equipedDialog.SetActive(true);
            statsDialog.SetActive(false);
            itemIndex = 0;
            page = 0;
            inItems = false;
        }
    }

    private void MovePointer() {
        Vector2 mov = GameManager.GetMov();
        if (mov != Vector2.zero) {
            if (!axisInUse) {
                axisInUse = true;
                if (!inItems) {
                    if (mov.y == -1) {
                        sound.MenuPointerSound();
                        equipmentIndex = (equipmentIndex != equipmentSlots.Length - 1) ? equipmentIndex + 1 : equipmentIndex;
                    } else if (mov.y == 1) {
                        sound.MenuPointerSound();
                        equipmentIndex = (equipmentIndex != 0) ? equipmentIndex - 1 : equipmentIndex;
                    }
                } else {
                    sound.MenuPointerSound();
                    if (mov.x == 1) {
                        if (page != MaxPage() - 1) {
                            page++;
                            ChangePage();
                        }
                    } else if (mov.x == -1) {
                        if (page != 0) {
                            page--;
                            ChangePage();
                        }
                    } else if (mov.y == -1) {
                        itemIndex = (itemIndex != itemSlots.Length - 1) ? itemIndex + 1 : itemIndex;
                    } else if (mov.y == 1) {
                        itemIndex = (itemIndex != 0) ? itemIndex - 1 : itemIndex;
                    }
                    SetMemberStats(ActualItem());
                }
                ChangePointerLocation();
            }
        } else {
            axisInUse = false;
        }
    }

    private void PopulateSlots() {
        int numSlot = 0;
        for (int i = (page * itemSlots.Length); i < ((page + 1) * itemSlots.Length); i++) {
            GameObject slot = itemSlots[numSlot];
            if (filterBag.Length > i) {
                Item item = filterBag[i];
                slot.SetActive(item != null);
                if (item != null) {
                    slot.transform.GetChild(0).GetComponent<Image>().sprite = item.sprite;
                    slot.transform.GetChild(1).GetComponent<Text>().text = item.itemName;
                    slot.transform.GetChild(2).GetComponent<Text>().text = Bag.Instance.GetAmountById(item.id).ToString();
                    if (item is Weapon) {
                        if (((Weapon)item).canUse == Weapon.Type.Both ||
                            member == GameManager.party[0] && ((Weapon)item).canUse == Weapon.Type.Human ||
                            member == GameManager.party[1] && ((Weapon)item).canUse == Weapon.Type.Animal) {
                            slot.transform.GetChild(1).GetComponent<Text>().color = Color.white;
                        } else {
                            slot.transform.GetChild(1).GetComponent<Text>().color = Color.grey;
                        }
                    } else {
                        slot.transform.GetChild(1).GetComponent<Text>().color = Color.white;
                    }
                }
            } else {
                slot.SetActive(false);
            }
            numSlot++;
        }
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

    private void ChangePage() {
        GameObject.Find("PageNumber").GetComponent<Text>().text = (page + 1) + " / " + MaxPage();
        PopulateSlots();
    }

    private void ChangePointerLocation() {
        if (!inItems) {
            pointerEquipment.transform.SetParent(equipmentSlots[equipmentIndex].gameObject.transform);
            pointerEquipment.GetComponent<RectTransform>().localPosition = new Vector3(-205, -25, 0);
        } else {
            pointerItems.transform.localPosition = new Vector3(
                itemSlots[itemIndex].gameObject.transform.localPosition.x - 210,
                itemSlots[itemIndex].gameObject.transform.localPosition.y - 25,
                itemSlots[itemIndex].gameObject.transform.localPosition.z
            );
        }
    }

    private void SetMemberStats(Item item) {
        SetStatsToDefault();
        if (item != null) {
            if (equipmentIndex == 0) {
                if (((Weapon)item).canUse == Weapon.Type.Both ||
                    member == GameManager.party[0] && ((Weapon)item).canUse == Weapon.Type.Human ||
                    member == GameManager.party[1] && ((Weapon)item).canUse == Weapon.Type.Animal) {
                    SetWeaponStats(item);
                }
            } else {
                SetArmorStats(item);
            }
        }
    }

    private void SetWeaponStats(Item item) {
        Weapon toEquip = (Weapon)item;
        atkStat.transform.Find("IconChange").gameObject.SetActive(true);
        dexStat.transform.Find("IconChange").gameObject.SetActive(true);

        if (member.weapon != null) {
            int diff = member.weapon.attack - toEquip.attack;
            if (diff == 0) {
                atkStat.transform.Find("IconChange").gameObject.SetActive(false);
                atkStat.transform.Find("NewAmount").GetComponent<Text>().text = "";
            } else {
                atkStat.transform.Find("IconChange").GetComponent<Image>().sprite = (diff > 0) ? statDown : statUp;
                atkStat.transform.Find("NewAmount").GetComponent<Text>().text = (member.attack - diff).ToString();
                atkStat.transform.Find("NewAmount").GetComponent<Text>().color = (diff > 0) ? Color.red : Color.green;
            }

            diff = member.weapon.dextery - toEquip.dextery;
            if (diff == 0) {
                dexStat.transform.Find("IconChange").gameObject.SetActive(false);
                dexStat.transform.Find("NewAmount").GetComponent<Text>().text = "";
            } else {
                dexStat.transform.Find("IconChange").GetComponent<Image>().sprite = (diff > 0) ? statDown : statUp;
                dexStat.transform.Find("NewAmount").GetComponent<Text>().text = (member.dextery - diff).ToString();
                dexStat.transform.Find("NewAmount").GetComponent<Text>().color = (diff > 0) ? Color.red : Color.green;
            }
        } else {
            if (toEquip.attack > 0) {
                atkStat.transform.Find("IconChange").GetComponent<Image>().sprite = statUp;
                atkStat.transform.Find("NewAmount").GetComponent<Text>().text = (member.attack + toEquip.attack).ToString();
                atkStat.transform.Find("NewAmount").GetComponent<Text>().color = Color.green;
            } else { atkStat.transform.Find("IconChange").gameObject.SetActive(false); }
            if (toEquip.dextery > 0) {
                dexStat.transform.Find("IconChange").GetComponent<Image>().sprite = statUp;
                dexStat.transform.Find("NewAmount").GetComponent<Text>().text = (member.dextery + toEquip.dextery).ToString();
                dexStat.transform.Find("NewAmount").GetComponent<Text>().color = Color.green;
            } else { dexStat.transform.Find("IconChange").gameObject.SetActive(false); }
        }
    }

    private void SetArmorStats(Item item) {
        Armor toEquip = (Armor)item;
        atkStat.transform.Find("IconChange").gameObject.SetActive(true);
        dexStat.transform.Find("IconChange").gameObject.SetActive(true);
        defStat.transform.Find("IconChange").gameObject.SetActive(true);
        resStat.transform.Find("IconChange").gameObject.SetActive(true);

        if ((equipmentIndex == 1 && member.helmet != null) ||
            (equipmentIndex == 2 && member.chestplate != null) ||
            (equipmentIndex == 3 && member.shield != null) ||
            (equipmentIndex == 4 && member.necklace != null)) {
            // COMPARING ATTACK
            int diff = ((equipmentIndex == 1) ? member.helmet.attack :
                        (equipmentIndex == 2) ? member.chestplate.attack :
                        (equipmentIndex == 3) ? member.shield.attack :
                        member.necklace.attack) - toEquip.attack;
            if (diff == 0) {
                atkStat.transform.Find("IconChange").gameObject.SetActive(false);
                atkStat.transform.Find("NewAmount").GetComponent<Text>().text = "";
            } else {
                atkStat.transform.Find("IconChange").GetComponent<Image>().sprite = (diff > 0) ? statDown : statUp;
                atkStat.transform.Find("NewAmount").GetComponent<Text>().text = (member.attack - diff).ToString();
                atkStat.transform.Find("NewAmount").GetComponent<Text>().color = (diff > 0) ? Color.red : Color.green;
            }
            // COMPARING DEXTERY
            diff = ((equipmentIndex == 1) ? member.helmet.dextery :
                    (equipmentIndex == 2) ? member.chestplate.dextery :
                    (equipmentIndex == 3) ? member.shield.dextery :
                    member.necklace.dextery) - toEquip.dextery;
            if (diff == 0) {
                dexStat.transform.Find("IconChange").gameObject.SetActive(false);
                dexStat.transform.Find("NewAmount").GetComponent<Text>().text = "";
            } else {
                dexStat.transform.Find("IconChange").GetComponent<Image>().sprite = (diff > 0) ? statDown : statUp;
                dexStat.transform.Find("NewAmount").GetComponent<Text>().text = (member.dextery - diff).ToString();
                dexStat.transform.Find("NewAmount").GetComponent<Text>().color = (diff > 0) ? Color.red : Color.green;
            }
            // COMPARING DEFENSE
            diff = ((equipmentIndex == 1) ? member.helmet.defense :
                    (equipmentIndex == 2) ? member.chestplate.defense :
                    (equipmentIndex == 3) ? member.shield.defense :
                    member.necklace.defense) - toEquip.defense;
            if (diff == 0) {
                defStat.transform.Find("IconChange").gameObject.SetActive(false);
                defStat.transform.Find("NewAmount").GetComponent<Text>().text = "";
            } else {
                defStat.transform.Find("IconChange").GetComponent<Image>().sprite = (diff > 0) ? statDown : statUp;
                defStat.transform.Find("NewAmount").GetComponent<Text>().text = (member.defense - diff).ToString();
                defStat.transform.Find("NewAmount").GetComponent<Text>().color = (diff > 0) ? Color.red : Color.green;
            }
            // COMPARING RESISTANCE
            diff = ((equipmentIndex == 1) ? member.helmet.resistance :
                    (equipmentIndex == 2) ? member.chestplate.resistance :
                    (equipmentIndex == 3) ? member.shield.resistance :
                    member.necklace.resistance) - toEquip.resistance;
            if (diff == 0) {
                resStat.transform.Find("IconChange").gameObject.SetActive(false);
                resStat.transform.Find("NewAmount").GetComponent<Text>().text = "";
            } else {
                resStat.transform.Find("IconChange").GetComponent<Image>().sprite = (diff > 0) ? statDown : statUp;
                resStat.transform.Find("NewAmount").GetComponent<Text>().text = (member.resistance - diff).ToString();
                resStat.transform.Find("NewAmount").GetComponent<Text>().color = (diff > 0) ? Color.red : Color.green;
            }
        } else {
            if (toEquip.attack > 0) {
                atkStat.transform.Find("IconChange").GetComponent<Image>().sprite = statUp;
                atkStat.transform.Find("NewAmount").GetComponent<Text>().text = (member.attack + toEquip.attack).ToString();
                atkStat.transform.Find("NewAmount").GetComponent<Text>().color = Color.green;
            } else { atkStat.transform.Find("IconChange").gameObject.SetActive(false); }
            if (toEquip.dextery > 0) {
                dexStat.transform.Find("IconChange").GetComponent<Image>().sprite = statUp;
                dexStat.transform.Find("NewAmount").GetComponent<Text>().text = (member.dextery + toEquip.dextery).ToString();
                dexStat.transform.Find("NewAmount").GetComponent<Text>().color = Color.green;
            } else { dexStat.transform.Find("IconChange").gameObject.SetActive(false); }
            if (toEquip.defense > 0) {
                defStat.transform.Find("IconChange").GetComponent<Image>().sprite = statUp;
                defStat.transform.Find("NewAmount").GetComponent<Text>().text = (member.defense + toEquip.defense).ToString();
                defStat.transform.Find("NewAmount").GetComponent<Text>().color = Color.green;
            } else { defStat.transform.Find("IconChange").gameObject.SetActive(false); }
            if (toEquip.resistance > 0) {
                resStat.transform.Find("IconChange").GetComponent<Image>().sprite = statUp;
                resStat.transform.Find("NewAmount").GetComponent<Text>().text = (member.resistance + toEquip.resistance).ToString();
                resStat.transform.Find("NewAmount").GetComponent<Text>().color = Color.green;
            } else { resStat.transform.Find("IconChange").gameObject.SetActive(false); }
        }
    }

    private void SetStatsToDefault() {
        atkStat.transform.Find("OldAmount").GetComponent<Text>().text = member.attack.ToString();
        dexStat.transform.Find("OldAmount").GetComponent<Text>().text = member.dextery.ToString();
        defStat.transform.Find("OldAmount").GetComponent<Text>().text = member.defense.ToString();
        resStat.transform.Find("OldAmount").GetComponent<Text>().text = member.resistance.ToString();

        atkStat.transform.Find("IconChange").gameObject.SetActive(false);
        dexStat.transform.Find("IconChange").gameObject.SetActive(false);
        defStat.transform.Find("IconChange").gameObject.SetActive(false);
        resStat.transform.Find("IconChange").gameObject.SetActive(false);

        atkStat.transform.Find("NewAmount").GetComponent<Text>().text = "";
        dexStat.transform.Find("NewAmount").GetComponent<Text>().text = "";
        defStat.transform.Find("NewAmount").GetComponent<Text>().text = "";
        resStat.transform.Find("NewAmount").GetComponent<Text>().text = "";
    }

    private Item ActualItem() {
        if (filterBag.Length <= (itemIndex + (page * itemSlots.Length))) {
            return null;
        }
        return filterBag[itemIndex + (page * itemSlots.Length)];
    }

    private int MaxPage() {
        int maxPage = (int)Mathf.Ceil(((float)filterBag.Length / (float)itemSlots.Length));
        return (maxPage == 0 ? 1 : maxPage);
    }

}
