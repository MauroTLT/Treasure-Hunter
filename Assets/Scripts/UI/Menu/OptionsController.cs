using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour {

    [Header("Game Objects")]
    public GameObject[] options;
    [Space]
    [Header("Music Volume")]
    public Slider musicSlider;
    public Text musicValue;
    [Space]
    [Header("SFX Volume")]
    public Slider sfxSlider;
    public Text sfxValue;
    [Space]
    [Header("Battle Speed")]
    public Image normalSpeed;
    public Image fastSpeed;
    [Space]
    [Header("Screen Type")]
    public Image yesEnemies;
    public Image noEnemies;

    [Header("Others")]
    public GameObject mainMenu;
    public GameObject pointer;
    public GameObject button;

    private SoundController sound;
    private int optionIndex = 0;
    private bool axisInUse = false;
    private bool inNormalSpeed, enemies;

    private void OnEnable() {
        sound = GameController.soundController;
        optionIndex = 0;
        inNormalSpeed = GameManager.battleSpeed != 1;
        enemies = GameManager.enemies;

        ToggleBattleSpeed();
        ToggleEnemies();
    }

    private void Update() {
        CheckButton();
        MovePointer();
    }

    private void CheckButton() {
        if (Input.GetButtonDown("Submit")) {
            sound.MenuSubmitSound();
            switch (optionIndex) {
                case 2: ToggleBattleSpeed(); break;
                case 3: ToggleEnemies(); break;
                case 4: ApplyChanges(); break;
            }
        } else if (Input.GetButtonDown("Cancel")) {
            gameObject.SetActive(false);
            mainMenu.SetActive(true);
            Input.ResetInputAxes();
        }
    }

    private void MovePointer() {
        Vector2 mov = GameManager.GetMov();
        if (mov != Vector2.zero) {
            if (!axisInUse) {
                if (mov.y == 1) {
                    optionIndex--;
                    axisInUse = true;
                } else if (mov.y == -1) {
                    optionIndex++;
                    axisInUse = true;
                } else if (mov.x != 0) {
                    switch (optionIndex) {
                        case 0:
                            musicSlider.value += mov.x;
                            UpdateMusicValue();
                            break;
                        case 1:
                            sfxSlider.value += mov.x;
                            UpdateSFXValue();
                            break;
                    }
                }
                optionIndex = Mathf.Clamp(optionIndex, 0, options.Length - 1);
                ChangePointerLocation();
            }
        } else {
            axisInUse = false;
        }
    }

    private void ChangePointerLocation() {
        pointer.transform.SetParent(options[optionIndex].gameObject.transform);
        if (optionIndex == options.Length - 1) {
            pointer.GetComponent<RectTransform>().localPosition = new Vector3(-90, 30, 0);
        } else {
            pointer.GetComponent<RectTransform>().localPosition = new Vector3(-475, -62, 0);
        }
    }

    private void ToggleBattleSpeed() {
        fastSpeed.enabled = inNormalSpeed;
        inNormalSpeed = !inNormalSpeed;
        normalSpeed.enabled = inNormalSpeed;
    }

    private void ToggleEnemies() {
        yesEnemies.enabled = enemies;
        enemies = !enemies;
        noEnemies.enabled = enemies;
    }

    private void UpdateMusicValue() {
        musicValue.text = musicSlider.value.ToString() + "%";
    }

    private void UpdateSFXValue() {
        sfxValue.text = sfxSlider.value.ToString() + "%";
    }

    private void ApplyChanges() {
        GameManager.battleSpeed = (inNormalSpeed)? 1 : 2;
        GameManager.enemies = !enemies;
        GameManager.musicVolume = musicSlider.normalizedValue;
        GameManager.sfxVolume = sfxSlider.normalizedValue;

        sound.GetComponent<RandomBattleController>().enabled = GameManager.enemies;
        sound.SetVolume();
        
    }
}
