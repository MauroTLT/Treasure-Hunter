using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowText : MonoBehaviour {

    protected SoundController sound;
    protected DialogController dialogController;
    [TextArea]
    public string text;
    protected Collider2D coll;
    protected GameObject player;

    private void Awake() {
        sound = Camera.main.GetComponent<SoundController>();
        coll = GetComponent<Collider2D>();
        dialogController = FindObjectOfType<DialogController>();
        player = FindObjectOfType<Player>().gameObject;
    }

    private void Update() {
        if (!GameManager.inBattle && !GameManager.gamePaused && !Crossfade.inTransition) {
            if (coll.IsTouching(GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>())) {
                if (Input.GetButtonUp("Submit")) {
                    // If is a character that can move orientate it towards the player
                    if (GetComponent<Walk>() != null) {
                        Orientate();
                    }
                    dialogController.ShowText(text, false);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag.Equals("Player")) {
            if (GetComponent<Soliloquy>() != null) {
                dialogController.ShowText(text, false);
                Destroy(gameObject);
            }
        }
    }

    protected void Orientate() {
        float x = transform.position.x - player.transform.position.x;
        float y = transform.position.y - player.transform.position.y;
        GetComponent<Walk>().SetMov(new Vector2(-x, -y));
    }
}
