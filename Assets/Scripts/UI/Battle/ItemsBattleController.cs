using UnityEngine;
using UnityEngine.UI;

public class ItemsBattleController : MonoBehaviour {

    public GameObject pointerItems;
    public GameObject[] slots;
    public GameObject itemDescriptor;

    public PartyStatsController partySC;
    public PartyActionController partyAC;
    public BattleController battle;

    private int member;
    private int indexToMove = -1;

    private int itemIndex = 0;
    private int page = 0;
    private bool selMember = false;
    private bool axisInUse = false;

    private void Start() {
        itemDescriptor = (Resources.FindObjectsOfTypeAll(typeof(ItemDescriptor))[0] as ItemDescriptor).gameObject;
    }

    private void OnEnable() {
        page = 0;
        itemIndex = 0;
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
                UseItem();
            } else if (Input.GetButtonUp("Cancel")) {
                // If its selecting a member
                if (selMember) {
                    selMember = false;
                    partySC.HideCursor(0);
                    partySC.HideCursor(1);
                } else {
                    enabled = false;
                    gameObject.SetActive(false);
                }
            } else if (Input.GetButtonUp("Y - Artifact")) {
                // Show the description of the item
                Item item = ActualItem();
                if (item != null) {
                    ItemDescriptor.item = item;
                    itemDescriptor.SetActive(true);
                }
            }
        }
    }

    /*
     * Use the actual item in someone
     */
    private void UseItem() {
        Item item = ActualItem();
        // If the item selected exists
        if (item != null) {
            // If the item is one that can be used
            if (item is Consumable) {
                // If the member to use the item is already select
                if (selMember) {
                    partyAC.target[battle.GetPlayerIndex()] = partySC.partyMembers[member];
                    partyAC.thing[battle.GetPlayerIndex()] = item;

                    selMember = false;
                    partySC.HideCursor(member);
                    indexToMove = -1;
                    PopulateSlots();
                    gameObject.SetActive(false);
                    battle.NextTurn();
                } else {
                    // Prepare the UI to select a member, the first by default
                    slots[itemIndex].transform.GetChild(1).GetComponent<Text>().color = Color.cyan;
                    partySC.ShowCursor(battle.GetPlayerIndex());
                    member = battle.GetPlayerIndex();
                    selMember = true;
                }
            }
        }
    }

    /*
     * Update the actual page and the list of items
     */
    private void ChangePage() {
        GameObject.Find("PageNumber").GetComponent<Text>().text = (page + 1) + " / 10";
        PopulateSlots();
    }

    /*
     * Move the pointer between items
     */
    private void MovePointer() {
        Vector2 mov = GameManager.GetMov();
        if (mov != Vector2.zero) {
            if (!axisInUse) {
                if (!selMember) {
                    if (mov.x == 1) {
                        if (itemIndex % 2 == 0) {
                            itemIndex++;
                        } else {
                            if (page != 9) {
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
                    ChangePointerLocation();
                } else {
                    if (mov.x == 1) {
                        partySC.SelectMemberCursor(1);
                        member = 1;
                    } else if (mov.x == -1) {
                        partySC.SelectMemberCursor(0);
                        member = 0;
                    }
                }
                axisInUse = true;
            }
        } else {
            axisInUse = false;
        }
    }

    /*
     * Move the pointer to the actual item
     */
    private void ChangePointerLocation() {
        pointerItems.transform.localPosition = new Vector3(
            slots[itemIndex].transform.localPosition.x - 205,
            slots[itemIndex].transform.localPosition.y,
            slots[itemIndex].transform.localPosition.z
        );
    }

    /*
     * Method that populates the slots with items and its information
     */
    private void PopulateSlots() {
        int numSlot = 0;
        for (int i = (page * slots.Length); i < ((page + 1) * slots.Length); i++) {
            BagSlot bagSlot = Bag.Instance.bag.slots[i];
            Item item = bagSlot.item;
            if (item != null) {
                slots[numSlot].transform.GetChild(0).gameObject.SetActive(true);
                slots[numSlot].transform.GetChild(0).GetComponent<Image>().sprite = item.sprite;
                slots[numSlot].transform.GetChild(1).GetComponent<Text>().text = item.itemName;
                slots[numSlot].transform.GetChild(1).GetComponent<Text>().color = (indexToMove == i) ? Color.cyan : (!(item is Consumable)) ? Color.gray : Color.white;
                slots[numSlot].transform.GetChild(2).GetComponent<Text>().text = bagSlot.amount.ToString();
            } else { // NO ITEM IN THIS POSITION
                slots[numSlot].transform.GetChild(0).gameObject.SetActive(false);
                slots[numSlot].transform.GetChild(1).GetComponent<Text>().text = "__________";
                slots[numSlot].transform.GetChild(1).GetComponent<Text>().color = (indexToMove == i) ? Color.cyan : Color.white;
                slots[numSlot].transform.GetChild(2).GetComponent<Text>().text = "-";
            }
            numSlot++;
        }
    }

    private Item ActualItem() {
        return Bag.Instance.bag.slots[itemIndex + (page * 8)].item;
    }
}
