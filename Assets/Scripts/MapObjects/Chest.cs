using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour {
    
    public int ID;
    public Item item;
    public bool isArtifact;
    private SoundController sound;
    private DialogController dialogController;

    private GameObject player;
    private Animator anim;
    private Collider2D coll;
    private bool isOpen;

    void Awake() {
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        dialogController = FindObjectOfType<DialogController>();
        sound = Camera.main.GetComponent<SoundController>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Start() {
        if (GameManager.IsChestOpen(ID)) {
            isOpen = true;
            anim.SetTrigger("StartOpen");
        }
    }

    private void Update() {
        // Check if the player interacts with the object
        if (!isOpen && !GameManager.gamePaused && !Crossfade.inTransition) {
            if (coll.IsTouching(player.GetComponent<Collider2D>())) {
                if (Input.GetButtonUp("Submit")) {
                    // If the object is special or artifact sounds an especial clip
                    if (item is Special || isArtifact) {
                        sound.SpecialItemRiseSound();
                    } else {
                        sound.ItemRiseSound();
                    }

                    // If the object is an artifact add it to the list
                    if (isArtifact) {
                        GameManager.NewArtifact(item.itemName);
                        Open();
                    } else {
                        // Save the item in the bag if is not full
                        if (Bag.Instance.PutInBag(item)) {
                            Open();
                        } else {
                            dialogController.ShowText("Your bag is full, you need to make some space!", false);
                        }
                    }
                }
            }
        }
    }

    /*
     * Method that triggers the animation to open a chest and show the item
     */
    private void Open() {
        GameManager.NewChestOpen(ID);
        if (anim != null) {
            anim.SetTrigger("StartOpen");
        } else {
            GetComponent<Renderer>().enabled = false;
            coll.enabled = false;
        }
        isOpen = true;
        StartCoroutine(FindObjectOfType<Player>().RiseItem(item));
    }
}
