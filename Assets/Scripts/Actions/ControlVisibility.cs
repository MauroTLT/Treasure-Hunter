using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlVisibility : MonoBehaviour {

    // Actions the object can do when activated
    public enum Type {
        ShowAndCollide = 0, // Enable the renderer and the collider
        HideAndCollide = 1, // Disable the renderer and enable the collider
        ShowAndNoCollide = 2, // Enable the renderer and disable the collider
        HideAndNoCollide = 3 // Disable the renderer and the collider
    };

    public Type type;
    public Sprite hide, show;

    private BoxCollider2D coll;
    private SpriteRenderer rend;

    public GameObject activator;

    private void Start() {
        coll = GetComponent<BoxCollider2D>();
        rend = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate() {
        bool activated = activator.GetComponent<Switch>().activated;
        // Do the action based on the Type option
        switch (type) {
            case Type.ShowAndCollide:
                if (activated) {
                    ShowAndCollide();
                } else {
                    HideAndNoCollide();
                }
                break;
            case Type.HideAndCollide:
                if (activated) {
                    HideAndCollide();
                } else {
                    ShowAndNoCollide();
                }
                break;
            case Type.ShowAndNoCollide:
                if (activated) {
                    ShowAndNoCollide();
                } else {
                    HideAndCollide();
                }
                break;
            case Type.HideAndNoCollide:
                if (activated) {
                    HideAndNoCollide();
                } else {
                    ShowAndCollide();
                }
                break;
        }
    }

    private void ShowAndCollide() {
        coll.enabled = true;
        ChangeSprite(true);
    }

    private void HideAndNoCollide() {
        coll.enabled = false;
        ChangeSprite(false);
    }

    private void ShowAndNoCollide() {
        coll.enabled = false;
        ChangeSprite(true);
    }

    private void HideAndCollide() {
        coll.enabled = true;
        ChangeSprite(false);
    }

    private void ChangeSprite(bool value) {
        if (value && show != null) {
            rend.enabled = true;
            rend.sprite = show;
        } else if (value && show == null) {
            rend.enabled = true;
        } else if (!value && hide != null) {
            rend.sprite = hide;
        } else if (!value && hide == null) {
            rend.enabled = false;
        }
    }
}
