using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchSprite : MonoBehaviour {

    public Sprite[] sprites = new Sprite[2];
    private int index;
    
    private Image image;

    private void Awake() {
        image = GetComponent<Image>();
    }

    public void SwapSprites() {
        SwapSprites(Mathf.Abs(index - 1));
    }

    public void SwapSprites(int i) {
        index = i;
        image.sprite = sprites[i];
    }

}
