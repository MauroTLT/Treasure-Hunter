using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealTile : MonoBehaviour {

    private SoundController sound;

    private void Awake() {
        sound = Camera.main.GetComponent<SoundController>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // Heal the player when trigger by him
        if (other.name.Equals("Player")) {
            sound.HealSound();
            GameManager.party[0].HealAll();
            GameManager.party[1].HealAll();
            GameController.dialogController.PartyHealed();
        }
    }
}
