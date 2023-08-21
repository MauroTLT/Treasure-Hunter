using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemsController : MonoBehaviour {
    /*
     * ----- VARIABLES -----
     * 
     * @mainMenu -> Main Window of the Menu
     * @pointerOptions -> Pointer to select the options in @options
     * @pointerItems -> Pointer to select the items in the list
     * 
     * @slots -> The items slot, should be 8
     * @options -> The options commands, should be 3 (USE, ORDER, THROW)
     * @memberFrames -> Frames of the 2 members in the party
     * 
     * @dialogSel -> Dialog Sprite to use when member is selected
     * @dialogNormal -> Dialog Sprite to use when member is not selected
     * 
     * @member -> PartyMember selected, to use the items in them
     * @indexToMove -> Index of the item selected and prepared to move or use
     * 
     * @optionIndex -> Index to control the option to use
     * @itemIndex -> Index to control the item selected
     * @inItems -> When true the user is selecting an item
     * @selMember -> When true the user is selecting the member to use the item
     * @axisInUse -> Field to control the directional input
     */
    public GameObject mainMenu, pointerOptions, pointerItems;
    public GameObject[] slots, options, memberFrames;
    public Sprite dialogSel, dialogNormal;

    private GameObject itemDescriptor;
    private SoundController sound;
    private PartyMember member;
    private int indexToMove = -1;

    private int optionIndex = 0;
    private int itemIndex = 0;
    private int page = 0;
    private bool inItems = false;
    private bool selMember = false;
    private bool axisInUse = false;

    private void OnEnable() {
        sound = GameController.soundController;
        itemDescriptor = GameController.itemDescriptor;
        page = 0;
        optionIndex = 0;
        itemIndex = 0;
        inItems = false;
        axisInUse = false;
        pointerItems.GetComponent<Image>().enabled = false;
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
                sound.MenuSubmitSound();
                // If its selecting the command option
                if (!inItems) {
                    switch (optionIndex) {
                        case 1: Bag.Instance.OrderBag(); PopulateSlots(); break; // ORDER OPTION
                        case 0: case 2: // USE AND THROW OPTION
                            inItems = true;
                            pointerItems.GetComponent<Image>().enabled = true;
                            break;
                    }
                    // The user has selected an item
                } else {
                    switch (optionIndex) {
                        case 0: UseOrMoveItem(); break; // USE OPTION
                        case 2: Bag.Instance.RemoveFromBag(ActualItem()); PopulateSlots(); break; // THROW OPTION
                    }
                }
                // The user pressed Cancel
            } else if (Input.GetButtonUp("Cancel")) {
                sound.MenuCancelSound();
                // If its selecting the command option
                if (!inItems) {
                    gameObject.SetActive(false);
                    mainMenu.SetActive(true);
                } else {
                    // If its selecting a member
                    if (selMember) {
                        selMember = false;
                        memberFrames[0].GetComponent<Image>().sprite = dialogNormal;
                        memberFrames[1].GetComponent<Image>().sprite = dialogNormal;
                    } else {
                        // If a item is selected to move
                        if (indexToMove != -1) {
                            indexToMove = -1;
                            PopulateSlots();
                        } else {
                            // If its selecting an item
                            inItems = false;
                            pointerItems.GetComponent<Image>().enabled = false;
                        }
                    }
                }
            } else if (Input.GetButtonUp("Y - Artifact")) {
                sound.MenuSubmitSound();
                Item item = ActualItem();
                if (inItems && item != null) {
                    ItemDescriptor.item = item;
                    itemDescriptor.SetActive(true);
                }
            }
        }
    }

    private void UseOrMoveItem() {
        if (indexToMove == -1) {
            slots[itemIndex].transform.GetChild(1).GetComponent<Text>().color = Color.cyan;
            indexToMove = itemIndex + (page * 8);
        } else {
            if (indexToMove == itemIndex + (page * 8)) {
                UseItem();
            } else {
                BagSlot bagSlot = Bag.Instance.bag.slots[itemIndex + (page * 8)];
                BagSlot toMove = Bag.Instance.bag.slots[indexToMove];

                Item itemReplaced = bagSlot.item;
                int itemReplacedNum = bagSlot.amount;

                bagSlot.item = toMove.item;
                bagSlot.amount = toMove.amount;

                toMove.item = itemReplaced;
                toMove.amount = itemReplacedNum;

                indexToMove = -1;
            }
            PopulateSlots();
        }
    }

    private void UseItem() {
        Item item = ActualItem();
        // If the item selected exists
        if (item != null) {
            // If the item is one that can be used
            if (item is Consumable) {
                // If the member to use the item is already select
                if (selMember) {
                    if ((item as Consumable).Use(member, false)) {
                        selMember = false;
                        memberFrames[0].GetComponent<Image>().sprite = dialogNormal;
                        memberFrames[1].GetComponent<Image>().sprite = dialogNormal;
                        indexToMove = -1;
                        UIMenu.SetPartyStats();
                        PopulateSlots();
                    } else {
                        sound.ErrorSound();
                    }
                    // Prepare the UI to select a member, the first by default
                } else {
                    memberFrames[0].GetComponent<Image>().sprite = dialogSel;
                    member = GameManager.party[0];
                    selMember = true;
                }
            }
        }
    }

    private void ChangePage() {
        GameObject.Find("PageNumber").GetComponent<Text>().text = (page + 1) + " / 10";
        PopulateSlots();
    }

    private void MovePointer() {
        Vector2 mov = GameManager.GetMov();
        if (mov != Vector2.zero) {
            if (!axisInUse) {
                sound.MenuPointerSound();
                if (!selMember) {
                    if (!inItems) {
                        if (mov.x == 1) {
                            optionIndex = (optionIndex != options.Length - 1) ? optionIndex + 1 : optionIndex;
                        } else if (mov.x == -1) {
                            optionIndex = (optionIndex != 0) ? optionIndex - 1 : optionIndex;
                        }
                    } else {
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
                    }
                    ChangePointerLocation();
                } else {
                    if (mov.x == 1) {
                        memberFrames[0].GetComponent<Image>().sprite = dialogNormal;
                        memberFrames[1].GetComponent<Image>().sprite = dialogSel;
                        member = GameManager.party[1];
                    } else if (mov.x == -1) {
                        memberFrames[0].GetComponent<Image>().sprite = dialogSel;
                        memberFrames[1].GetComponent<Image>().sprite = dialogNormal;
                        member = GameManager.party[0];
                    }
                }
                axisInUse = true;
            }
        } else {
            axisInUse = false;
        }
    }

    private void ChangePointerLocation() {
        if (!inItems) {
            pointerOptions.transform.SetParent(options[optionIndex].gameObject.transform);
            pointerOptions.GetComponent<RectTransform>().localPosition = new Vector3(-100, 0, 0);
        } else {
            pointerItems.transform.localPosition = new Vector3(
                slots[itemIndex].gameObject.transform.localPosition.x - 205,
                slots[itemIndex].gameObject.transform.localPosition.y,
                slots[itemIndex].gameObject.transform.localPosition.z
            );
        }
        
    }

    private void PopulateSlots() {
        int numSlot = 0;
        for (int i = (page*slots.Length); i < ((page+1)*slots.Length); i++) {
            BagSlot bagSlot = Bag.Instance.bag.slots[i];
            Item item = bagSlot.item;
            if (item != null) {
                slots[numSlot].transform.GetChild(0).gameObject.SetActive(true);
                slots[numSlot].transform.GetChild(0).GetComponent<Image>().sprite = item.sprite;
                slots[numSlot].transform.GetChild(1).GetComponent<Text>().text = item.itemName;
                slots[numSlot].transform.GetChild(1).GetComponent<Text>().color = (indexToMove == i)? Color.cyan : (!(item is Consumable))? Color.gray : Color.white;
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
