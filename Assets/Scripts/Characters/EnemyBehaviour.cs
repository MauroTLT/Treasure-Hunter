using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * UNUSED CLASS, JUST A PROOF OF CONCEPT FOR A POSSIBLE ENEMY SPECIFIC MOVEMENT
 */
public class EnemyBehaviour : MonoBehaviour {

    public float speed;
    private bool still;

    // Movement with Chasing
    public float visionRadius;
    public float attackRadius;
    private Vector3 initialPosition;

    // Movement random
    public bool randomMove;
    public float moveDuration = 4f;
    private float timeLeft;
    private Vector3 movement;

    private GameObject player;
    private Animator anim;
    private Rigidbody2D rb2d;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");

        initialPosition = transform.position;

        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.bodyType = (randomMove) ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic;
    }

    void Update() {
        if (!GameManager.gamePaused && !still) {
            if (randomMove) {
                timeLeft -= Time.deltaTime;
                if (timeLeft <= 0) {
                    movement = new Vector3(Random.Range(-1F, 1F), Random.Range(-1F, 1F));
                    timeLeft += moveDuration;
                }
                rb2d.MovePosition(transform.position + movement * speed * Time.deltaTime);
            } else {
                Vector3 target = nextDirection();

                float distance = Vector3.Distance(target, transform.position);
                Vector3 dir = (target - transform.position).normalized;

                if (target != initialPosition && distance < attackRadius) {
                    anim.speed = 2;
                } else {
                    anim.speed = 1;
                    rb2d.MovePosition(transform.position + dir * speed * Time.deltaTime);
                }

                if (target == initialPosition && distance < 0.02F) {
                    transform.position = initialPosition;
                }
                Debug.DrawLine(transform.position, target, Color.green);
            }
        }
    }

    private Vector3 nextDirection() {
        Vector3 target = initialPosition;
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            player.transform.position - transform.position,
            visionRadius,
            1 << LayerMask.NameToLayer("Default")
        );

        Vector3 forward = transform.TransformDirection(player.transform.position - transform.position);
        Debug.DrawRay(transform.position, forward, Color.red);

        if (hit.collider != null) {
            if (hit.collider.tag == "Player") {
                target = player.transform.position;
            }
        }
        return target;
    }

    private void OnBecameInvisible() { still = true; }

    private void OnBecameVisible() { still = false; }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
