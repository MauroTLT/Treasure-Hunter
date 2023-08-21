using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour {

    public string destroyStateName;
    public float timeForDisable;
    public bool onlyExplosions;

    private Animator anim;
    private bool canDestroy;

    void Start() {
        anim = GetComponent<Animator>();
    }

    private IEnumerator OnTriggerEnter2D(Collider2D other) {
        // Check if can be destroy by the player sword or an explosion
        if ((other.tag.Equals("PlayerActionBox") && !onlyExplosions) || other.tag.Equals("Explosion") && onlyExplosions) {
            if (anim != null) {
                anim.Play(destroyStateName);
            }
            yield return new WaitForSeconds(timeForDisable);

            foreach (Collider2D c in GetComponents<Collider2D>()) {
                c.enabled = false;
            }
            canDestroy = true;
        }
    }

    void Update() {
        if (anim != null) {
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName(destroyStateName) && (stateInfo.normalizedTime >= 1)) {
                Destroy(gameObject);
            }
        } else if (canDestroy) {
            Destroy(gameObject);
        }
    }
}
