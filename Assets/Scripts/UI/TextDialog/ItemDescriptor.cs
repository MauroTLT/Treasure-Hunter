using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDescriptor : MonoBehaviour {

    public static Item item;
    private Text itemName, itemDescription;
    private Image itemSprite;
    private Image[] itemElement, canUse;

    private void Awake() {
        itemName = gameObject.transform.Find("ItemName").GetComponent<Text>();
        itemDescription = gameObject.transform.Find("ItemDescription").GetComponent<Text>();
        itemSprite = gameObject.transform.Find("ItemSprite").GetComponent<Image>();
        itemElement = gameObject.transform.Find("ItemElement").gameObject.GetComponentsInChildren<Image>(); ;
        canUse = gameObject.transform.Find("CanUse").gameObject.GetComponentsInChildren<Image>(); ;
    }

    private void OnEnable() {
        if (item != null) {
            GameManager.showingItem = true;
            ShowItem();
        }
    }

    private void LateUpdate() {
        if (Input.GetButtonUp("Submit") || Input.GetButtonUp("Cancel") || Input.GetButtonUp("Y - Artifact")) {
            GameManager.showingItem = false;
            gameObject.SetActive(false);
        }
    }

    private void ShowItem() {
        Input.ResetInputAxes();
        itemName.text = item.itemName;
        itemDescription.text = item.description;
        itemSprite.sprite = item.sprite;

        foreach (Image img in itemElement) {
            img.sprite = (item is Weapon) ? ((((Weapon)item).element != null) ? ((Weapon)item).element.sprite : null) : null;
            img.gameObject.SetActive(item is Weapon && ((Weapon)item).element != null);
        }

        canUse[0].gameObject.SetActive(
            (item is Weapon)
            ? ((Weapon)item).canUse == Weapon.Type.Human || ((Weapon)item).canUse == Weapon.Type.Both
            : true
        );

        canUse[1].gameObject.SetActive(
            (item is Weapon)
            ? ((Weapon)item).canUse == Weapon.Type.Animal || ((Weapon)item).canUse == Weapon.Type.Both
            : true
        );
    }

}
