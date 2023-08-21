using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveTile : MonoBehaviour {

    private SoundController sound;

    private void Awake() {
        sound = Camera.main.GetComponent<SoundController>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // Save the game when the player triggers this object
        if (other.name.Equals("Player")) {
            sound.SaveSound();
            GameManager.SaveGame(true);
            GameController.dialogController.GameSaved();
        }
    }
}
