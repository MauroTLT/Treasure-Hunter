using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour {

    private SoundController sound;
    private Animator anim;
    private BoxCollider2D coll;
    private SpriteRenderer rend;

    [Space]
    [Header("Behaviour")]
    public float openSpeed = 0.15F;
    public bool hideWhenOpen = true;
    public bool bloqued = false;
    [Space]
    [Header("Who Can Open It?")]
    public GameObject activator;

    private void Awake() {
        sound = Camera.main.GetComponent<SoundController>();
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
        rend = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate() {
        if (activator != null) {
            bool activated = activator.GetComponent<Switch>().activated;
            coll.enabled = !activated;
            rend.enabled = !activated;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Player" && !bloqued) {
            Open();
        }
    }

    public void Open() {
        sound.DoorOpen();
        bloqued = false;
        StartCoroutine(ChangeState(true));
    }

    public void Close() {
        StartCoroutine(ChangeState(false));
    }

    IEnumerator ChangeState(bool state) {
        if (anim == null) {
            Awake();
        }
        anim.SetBool("Open", state);
        yield return new WaitForSeconds(openSpeed);
        coll.enabled = !state;
        if (hideWhenOpen) {
            rend.enabled = !state;
        }
    }
}
