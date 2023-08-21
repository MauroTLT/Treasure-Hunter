using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {

    [HideInInspector]
    public Vector2 mov;

    public float speed;
    public float maxDistance;
    public float waitBeforeDestroy;

    private Animator anim;
    private GameObject player;
    private Vector3 initialPos;

    void Start() {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        initialPos = player.transform.position;
    }

    void Update() {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        Vector3 distance = (initialPos - transform.position);
        if (Mathf.Abs(distance.x) >= maxDistance || Mathf.Abs(distance.y) >= maxDistance) {
            Destroy(gameObject);
        } else if (stateInfo.IsName("Arrow_Destroy") && stateInfo.normalizedTime >= 1) {
            Destroy(gameObject);
        } else if (!stateInfo.IsName("Arrow_Destroy")) {
            transform.position += new Vector3(mov.x, mov.y, 0) * speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag != "Player" && other.tag != "PlayerActionBox" && other.tag != "InternColision" && other.tag != "NoColision") {
            anim.Play("Arrow_Destroy");
        } else if (other.tag == "InternColision" && !player.GetComponent<Player>().inUpLayer) {
            anim.Play("Arrow_Destroy");
        }
    }

    private void OnDestroy() {
        player.GetComponent<Player>().artifactActing = false;
    }
}
