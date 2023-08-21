using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemShopController : MonoBehaviour {

    [Space]
    [Header("Items")]
    public Item[] catalog = new Item[0];
    [Space]
    [Header("Pointers")]
    public GameObject pointerOptions;
    public GameObject pointerItems;
    [Space]
    [Header("Item Amount and Value")]
    public GameObject itemAmount;
    public GameObject itemValue;
    [Space]
    public Text moneyCount;
    public GameObject[] slots, options, memberStats;

    private int optionIndex, itemIndex, page, maxPage, amount;
    private bool axisInUse, inItems, inTransaction;

    private SoundController sound;
    private GameObject itemDescriptor;
    private DialogController dialogController;

    private void Awake() {
        sound = Camera.main.GetComponent<SoundController>();
        GameManager.gamePaused = true;
        itemDescriptor = GameController.itemDescriptor;
        dialogController = GameController.dialogController;
        moneyCount.text = GameManager.gold.ToString();
        amount = 1;
    }

    private void Start() {
        SetPlayerStats();
        ChangePage();
    }

    private void Update() {
        if (!GameManager.showingItem && !DialogController.isTextShown) {
            CheckButtons();
            MovePointer();
        }
    }

    private void MovePointer() {
        Vector2 mov = GameManager.GetMov();
        if (mov != Vector2.zero) {
            if (!axisInUse) {
                if (!inItems) {
                    if (mov.x == 1) {
                        optionIndex++;
                    } else if (mov.x == -1) {
                        optionIndex--;
                    }
                    optionIndex = Mathf.Clamp(optionIndex, 0, options.Length - 1);
                    ChangePage();
                } else if (inTransaction) {
                    if (mov.x == 1) { amount += 10; }
                    else if (mov.x == -1) { amount -= 10;}
                    else if (mov.y == 1) { amount++;}
                    else if (mov.y == -1) {amount--;}
                    SetCorrectAmount();
                } else {
                    if (mov.x == 1) {
                        if (itemIndex % 2 == 0) {
                            itemIndex++;
                        } else {
                            if (page != maxPage - 1) {
                                itemIndex--;
                                page++;
                                ChangePage();
                            }
                        }
                    } else if (mov.x == -1) {
                        if (itemIndex % 2 != 0) {
                            itemIndex--;
                        } else {
                            if (page != 0) {
                                itemIndex++;
                                page--;
                                ChangePage();
                            }
                        }
                    } else if (mov.y == 1) {
                        itemIndex = (itemIndex > 1) ? itemIndex - 2 : itemIndex;
                    } else if (mov.y == -1) {
                        itemIndex = (itemIndex < 6) ? itemIndex + 2 : itemIndex;
                    }
                    SetItemComparative();
                }
                ChangePointerLocation();
                axisInUse = true;
            }
        } else {
            axisInUse = false;
        }
    }

    private void CheckButtons() {
        if (!axisInUse) {
            if (Input.GetButtonUp("Submit")) {
                sound.MenuSubmitSound();
                // If its selecting the command option
                if (!inItems) {
                    inItems = true;
                    pointerItems.SetActive(true);
                } else if (!inTransaction) {
                    // The user has selected an item
                    SetAmountValue();
                } else {
                    // The user confirms the transaction
                    sound.CoinSound();
                    switch (optionIndex) {
                        case 0: Buy(); break;
                        case 1: Sell(); break;
                    }
                    moneyCount.text = GameManager.gold.ToString();
                    SwitchTransaction();
                }
            } else if (Input.GetButtonUp("Cancel")) {
                sound.MenuCancelSound();
                if (!inItems) {
                    // If its selecting the command option
                    Destroy(gameObject);
                } else if (!inTransaction) {
                    // If its selecting an item
                    page = 0;
                    inItems = false;
                    pointerItems.SetActive(false);
                } else {
                    // The user refuses the transaction
                    SwitchTransaction();
                }
            } else if (Input.GetButtonUp("Y - Artifact") && inItems) {
                sound.MenuSubmitSound();
                Item item = (optionIndex == 0) ? ActualItem() : ActualItemBag();
                if (item != null) {
                    ItemDescriptor.item = item;
                    itemDescriptor.SetActive(true);
                }
            }
        }
    }

    private void Buy() {
        Item item = ActualItem();
        if (item != null) {
            int price = (amount * item.price);
            if (price > GameManager.gold) {
                dialogController.ShowSubText("Not enough money.");
            } else {
                Bag.Instance.PutInBag(item, amount);
                GameManager.RestGold(price);
            }
        }
    }

    private void Sell() {
        Item item = ActualItemBag();
        if (item != null) {
            if (!(item is Special)) {
                int price = ((amount * item.price) / 2);
                Bag.Instance.RemoveFromBag(item, amount);
                GameManager.SumGold(price);
                ChangePage();
            } else {
                sound.ErrorSound();
            }
        }
    }

    private void SetCorrectAmount() {
        int maxValue = 99;
        if (optionIndex == 0) {
            if (ActualItem() != null) {
                maxValue -= Bag.Instance.GetAmountById(ActualItem().id);
            }
        } else {
            if (ActualItemBag() != null) {
                maxValue = Bag.Instance.GetAmountById(ActualItemBag().id);
            }
        }
        amount = Mathf.Clamp(amount, 1, maxValue);
        SetAmountValue();
        
    }

    private void SetAmountValue() {
        Item item = (optionIndex == 0) ? ActualItem() : ActualItemBag();
        if (item != null) {
            itemAmount.transform.parent.gameObject.SetActive(true);
            itemAmount.GetComponent<Text>().text = amount.ToString();
            itemValue.GetComponent<Text>().text = (optionIndex == 0) ? (item.price * amount).ToString() : (item.price * amount / 2).ToString();
            inTransaction = true;
        }
    }

    private void SetItemComparative() {
        if (optionIndex == 0) {
            ResetStats();
            Item item = ActualItem();
            if (item != null && !(item is Consumable)) {
                if (item is Weapon) {
                    SetItemStats(item as Weapon);
                } else if (item is Armor) {
                    for (int i = 0; i < memberStats.Length; i++) {
                        if (item is Helmet) {
                            SetItemStats(i, GameManager.party[i].helmet, item as Armor);
                        } else if (item is Chestplate) {
                            SetItemStats(i, GameManager.party[i].chestplate, item as Armor);
                        } else if (item is Shield) {
                            SetItemStats(i, GameManager.party[i].shield, item as Armor);
                        } else if (item is Necklace) {
                            SetItemStats(i, GameManager.party[i].necklace, item as Armor);
                        }
                    }  
                }
            }
        }
    }

    private void SetItemStats(Weapon item) {
        for (int i = 0; i < memberStats.Length; i++) {
            Text text;
            int diff;

            if ((i == 0 && (item.canUse == Weapon.Type.Human)) || (i == 1 && (item.canUse == Weapon.Type.Animal)) || item.canUse == Weapon.Type.Both) {
                text = memberStats[i].transform.GetChild(1).GetChild(1).GetComponent<Text>();
                diff = item.attack;
                if (GameManager.party[i].weapon != null) {
                    diff = item.attack - GameManager.party[i].weapon.attack;
                }
                SetText(text, diff, (GameManager.party[i].attack + diff));
            }

            if ((i == 0 && (item.canUse == Weapon.Type.Human)) || (i == 1 && (item.canUse == Weapon.Type.Animal)) || item.canUse == Weapon.Type.Both) {
                text = memberStats[i].transform.GetChild(2).GetChild(1).GetComponent<Text>();
                diff = item.dextery;
                if (GameManager.party[i].weapon != null) {
                    diff = item.dextery - GameManager.party[i].weapon.dextery;
                }
                SetText(text, diff, (GameManager.party[i].dextery + diff));
            }
        }
    }

    private void SetItemStats(int index, Armor memberArmor, Armor item) {
        print(memberArmor);
        Text text = memberStats[index].transform.GetChild(1).GetChild(1).GetComponent<Text>();
        int diff;
        if (memberArmor != null) {
            diff = item.attack - memberArmor.attack;
            SetText(text, diff, (GameManager.party[index].attack + diff));
        } else {
            SetText(text, item.attack, (GameManager.party[index].attack + item.attack));
        }

        text = memberStats[index].transform.GetChild(2).GetChild(1).GetComponent<Text>();
        if (memberArmor != null) {
            diff = item.dextery - memberArmor.dextery;
            SetText(text, diff, (GameManager.party[index].dextery + diff));
        } else {
            SetText(text, item.dextery, (GameManager.party[index].dextery + item.dextery));
        }

        text = memberStats[index].transform.GetChild(3).GetChild(1).GetComponent<Text>();
        if (memberArmor != null) {
            diff = item.defense - memberArmor.defense;
            SetText(text, diff, (GameManager.party[index].defense + diff));
        } else {
            SetText(text, item.defense, (GameManager.party[index].defense + item.defense));
        }

        text = memberStats[index].transform.GetChild(4).GetChild(1).GetComponent<Text>();
        if (memberArmor != null) {
            diff = item.resistance - memberArmor.resistance;
            SetText(text, diff, (GameManager.party[index].resistance + diff));
        } else {
            SetText(text, item.resistance, (GameManager.party[index].resistance + item.resistance));
        }
    }

    private void SetText(Text text, int diff, int amount) {
        if (diff != 0) {
            text.gameObject.SetActive(true);
            text.color = (diff == 0) ? Color.white : (diff < 0) ? Color.red : Color.green;
            text.text = "> " + " " + (amount).ToString();
        }
    }

    private void ResetStats() {
        for (int i = 0; i < memberStats.Length; i++) {
            memberStats[i].transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
            memberStats[i].transform.GetChild(2).GetChild(1).gameObject.SetActive(false);
            memberStats[i].transform.GetChild(3).GetChild(1).gameObject.SetActive(false);
            memberStats[i].transform.GetChild(4).GetChild(1).gameObject.SetActive(false);
        }
    }

    private void SetPlayerStats() {
        for (int i = 0; i < memberStats.Length; i++) {
            memberStats[i].GetComponentInChildren<Image>().sprite = GameManager.party[i].sprite;
            Text text = memberStats[i].transform.GetChild(1).GetChild(0).GetComponent<Text>();
            text.text = GameManager.party[i].attack.ToString();

            text = memberStats[i].transform.GetChild(2).GetChild(0).GetComponent<Text>();
            text.text = GameManager.party[i].dextery.ToString();

            text = memberStats[i].transform.GetChild(3).GetChild(0).GetComponent<Text>();
            text.text = GameManager.party[i].defense.ToString();

            text = memberStats[i].transform.GetChild(4).GetChild(0).GetComponent<Text>();
            text.text = GameManager.party[i].resistance.ToString();
        }
    }

    private void ChangePointerLocation() {
        if (!inItems) {
            pointerOptions.transform.SetParent(options[optionIndex].gameObject.transform);
            pointerOptions.GetComponent<RectTransform>().localPosition = new Vector3(-75, 0, 0);
        } else {
            pointerItems.transform.localPosition = new Vector3(
                slots[itemIndex].gameObject.transform.localPosition.x - 205,
                slots[itemIndex].gameObject.transform.localPosition.y,
                slots[itemIndex].gameObject.transform.localPosition.z
            );
        }

    }

    private void PopulateSlots() {
        if (optionIndex == 0) {
            maxPage = Mathf.CeilToInt(catalog.Length / 8F);
        } else {
            maxPage = Mathf.CeilToInt(Bag.Instance.bag.slots.Length / 8F);
        }
        int numSlot = 0;
        for (int i = (page * slots.Length); i < ((page + 1) * slots.Length); i++) {
            BagSlot bagSlot = Bag.Instance.bag.slots[i];
            Item item = (optionIndex == 0) ? ((i < catalog.Length) ? catalog[i] : null) : bagSlot.item;
            if (item != null) {
                slots[numSlot].transform.GetChild(0).gameObject.SetActive(true);
                slots[numSlot].transform.GetChild(0).GetComponent<Image>().sprite = item.sprite;
                slots[numSlot].transform.GetChild(1).GetComponent<Text>().text = item.itemName;
                if (optionIndex == 0) {
                    slots[numSlot].transform.GetChild(2).GetComponent<Text>().text = item.price.ToString();
                } else {
                    slots[numSlot].transform.GetChild(2).GetComponent<Text>().text = bagSlot.amount.ToString();
                }
            } else { // NO ITEM IN THIS POSITION
                slots[numSlot].transform.GetChild(0).gameObject.SetActive(false);
                slots[numSlot].transform.GetChild(1).GetComponent<Text>().text = "__________";
                slots[numSlot].transform.GetChild(2).GetComponent<Text>().text = "-";
            }
            numSlot++;
        }
    }

    private void SwitchTransaction() {
        inTransaction = !inTransaction;
        itemAmount.transform.parent.gameObject.SetActive(inTransaction);
        amount = 1;
    }

    private void ChangePage() {
        PopulateSlots();
        GameObject.Find("PageNumber").GetComponent<Text>().text = (page + 1) + " / " + maxPage;
    }
    

    private Item ActualItem() {
        if ((itemIndex + (page * 8)) < catalog.Length) {
            return catalog[itemIndex + (page * 8)];
        }
        return null;
    }

    private Item ActualItemBag() {
        return Bag.Instance.bag.slots[itemIndex + (page * 8)].item;
    }

    private void OnDestroy() {
        GameManager.gamePaused = false;
    }
}
