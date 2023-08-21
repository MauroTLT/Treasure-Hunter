using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactController : MonoBehaviour {

    public Sprite arrowSprite, hookSprite, bombSprite, otherSprite;
    public Image actualArtfSprite, beforeArtfSprite, afterArtfSprite;
    private Player player;

    private void Start() {
        if (!GameManager.actualZone.canUseArtifacts) {
            gameObject.SetActive(false);
            enabled = false;
            return;
        }
        player = FindObjectOfType<Player>();
        SetSprites();
    }

    private void Update() {
        if (!GameManager.gamePaused && !player.artifactActing) {
            if (Input.GetButtonDown("Left Trigger")) {
                MoveToRight();
                SetSprites();
            } else if (Input.GetButtonDown("Right Trigger")) {
                MoveToLeft();
                SetSprites();
            }
        }
    }

    private void MoveToRight() {
        GameObject last = player.artifacts[player.artifacts.Length -1];
        for (int i = player.artifacts.Length - 2; i >= 0; i--) {
            player.artifacts[i + 1] = player.artifacts[i];
        }
        player.artifacts[0] = last;

        if (player.artifacts[0] == null) {
            int index = GetBeforeFirstPos();
            if (index != -1) {
                player.artifacts[0] = player.artifacts[index];
                player.artifacts[index] = null;
            }
        }
    }

    private void MoveToLeft() {
        GameObject first = player.artifacts[0];
        for (int i = 1; i < player.artifacts.Length; i++) {
            player.artifacts[i - 1] = player.artifacts[i];
        }
        player.artifacts[player.artifacts.Length - 1] = first;

        if (player.artifacts[0] == null) {
            player.artifacts[0] = player.artifacts[player.artifacts.Length - 1];
            player.artifacts[player.artifacts.Length - 1] = null;
        }
        Group();
    }

    private void Group() {
        for (int i = 0; i < player.artifacts.Length; i++) {
            if (player.artifacts[i] == null) {
                for (int j = i; j < player.artifacts.Length; j++) {
                    if (player.artifacts[j] != null) {
                        player.artifacts[i] = player.artifacts[j];
                        player.artifacts[j] = null;
                    }
                }
            }
        }
    }

    public void SetSprites() {
        Sprite sprite = GetSpriteByName(player.artifacts[0].name);
        if (sprite != null) {
            actualArtfSprite.sprite = sprite;
        }
        Sprite spriteBf = GetBeforeFirst();
        beforeArtfSprite.sprite = (spriteBf != null) ? spriteBf : sprite;

        Sprite spriteAf = GetAfterFirst();
        afterArtfSprite.sprite = (spriteAf != null) ? spriteAf : sprite;
    }

    private Sprite GetAfterFirst() {
        for (int i = 1; i < player.artifacts.Length; i++) {
            if (player.artifacts[i] != null) {
                return GetSpriteByName(player.artifacts[i].name);
            }
        }
        return null;
    }

    private Sprite GetBeforeFirst() {
        for (int i = player.artifacts.Length - 1; i > 0; i--) {
            if (player.artifacts[i] != null) {
                return GetSpriteByName(player.artifacts[i].name);
            }
        }
        return null;
    }

    private int GetBeforeFirstPos() {
        for (int i = player.artifacts.Length - 1; i > 0; i--) {
            if (player.artifacts[i] != null) {
                return i;
            }
        }
        return -1;
    }

    private Sprite GetSpriteByName(string name) {
        if (name.Equals("Arrow")) {
            return arrowSprite;
        } else if (name.Equals("Hook")) {
            return hookSprite;
        } else if (name.Equals("Bomb")) {
            return bombSprite;
        } else {
            return otherSprite;
        }
    }
    
}
