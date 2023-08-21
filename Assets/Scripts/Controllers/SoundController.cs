using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour {

    // Music and Sound clips
    public AudioClip battleMusic, bossMusic;
    public AudioClip menuSelect, menuBack, menuPointer;
    public AudioClip errorSound, coinSound;
    public AudioClip doorOpen, itemRise, specialItemRise;
    public AudioClip healSound, innSound;
    public AudioClip saveSound, loadSound;

    private AudioClip zoneMusic;
    private AudioSource musicSource;
    private AudioSource soundSource;

    private void Awake() {
        musicSource = GetComponents<AudioSource>()[0];
        soundSource = GetComponents<AudioSource>()[1];
        zoneMusic = musicSource.clip;
    }

    /*
     * Set the volume based on the values in the GameManager
     */
    public void SetVolume() {
        musicSource.volume = GameManager.musicVolume;
        soundSource.volume = GameManager.sfxVolume;
    }

    /*
     * Play a music clip
     */
    public void PlayMusic(AudioClip clip) {
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void BattleMusic() {
        PlayMusic(battleMusic);
    }

    public void ZoneMusic() {
        PlayMusic(zoneMusic);
    }

    /*
     * Play a sound clip
     */
    public void PlaySound(AudioClip clip) {
        if (soundSource.isPlaying) {
            soundSource.Stop();
        }
        soundSource.clip = clip;
        soundSource.Play();
    }

    public void MenuSubmitSound() {
        PlaySound(menuSelect);
    }

    public void MenuCancelSound() {
        PlaySound(menuBack);
    }

    public void MenuPointerSound() {
        PlaySound(menuPointer);
    }

    public void ErrorSound() {
        PlaySound(errorSound);
    }

    public void CoinSound() {
        PlaySound(coinSound);
    }

    public void DoorOpen() {
        PlaySound(doorOpen);
    }

    public void SaveSound() {
        PlaySound(saveSound);
    }

    public void LoadSound() {
        PlaySound(loadSound);
    }

    public void HealSound() {
        PlaySound(healSound);
    }

    public void ItemRiseSound() {
        PlaySound(itemRise);
    }

    public void SpecialItemRiseSound() {
        PlaySound(specialItemRise);
    }
}
