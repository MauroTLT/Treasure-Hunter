using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shopkeeper : ShowText {

    public GameObject menuPrefab;
    [TextArea]
    public string lastText;

    [Space]
    [Header("Items")]
    public Item[] catalog = new Item[0];

    private void Update() {
        if (!GameManager.gamePaused && !Crossfade.inTransition) {
            if (coll.IsTouching(GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>())) {
                if (Input.GetButtonUp("Submit")) {
                    StartCoroutine(OpenShop());
                }
            }
        }
    }

    /*
     * Open the Shop menu
     */
    private IEnumerator OpenShop() {
        dialogController.ShowText(text, false);
        while (DialogController.isTextShown) {
            yield return null;
        }

        GameObject go = Instantiate(menuPrefab);
        go.GetComponent<ItemShopController>().catalog = catalog;

        while (GameManager.gamePaused) {
            yield return null;
        }
        dialogController.ShowText(lastText, false);
        while (DialogController.isTextShown) {
            yield return null;
        }
    }

}
