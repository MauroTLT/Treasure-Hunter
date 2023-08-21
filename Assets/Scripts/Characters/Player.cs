using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public float speed = 4.0f;
    public GameObject menuPrefab;

    [HideInInspector]
    public GameObject[] artifacts = new GameObject[4];

    [HideInInspector]
    public Vector2 mov;
    [HideInInspector]
    public bool inUpLayer = true;
    [HideInInspector]
    public bool interactingWithObject;
    [HideInInspector]
    public bool artifactActing;
    [HideInInspector]
    public bool movePrevent;

    private CircleCollider2D attackHitbox;
    private Image itemGetSprite;
    private Animator anim;
    private Rigidbody2D rb2d;
    private DialogController dialogController;

    private void Awake() {
        GetArtefacts();
    }

    private void Start() {
        dialogController = GameController.dialogController;
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();

        itemGetSprite = transform.GetComponentInChildren<Image>();
        itemGetSprite.enabled = false;
        attackHitbox = transform.GetChild(0).GetComponent<CircleCollider2D>();
        attackHitbox.enabled = false;
    }

    private void Update() {
        if (!GameManager.gamePaused && !Crossfade.inTransition) {
            Movement();

            // Check if the player can and wants to open the menu
            if (Input.GetButtonDown("X - Menu") && !interactingWithObject && !movePrevent) {
                Instantiate(menuPrefab);
                return;
            }

            // Check if the player can and wants to reset the room or use an artifact
            if (GameManager.actualZone.canUseArtifacts) {
                // Check if can use the reset button
                if (Input.GetButtonDown("Select") && !interactingWithObject && !movePrevent) {
                    StartCoroutine(TimeBack());
                }
                ArtifactAction();
            }

            SwordAttack();

            PreventMove();
        } else {
            mov = Vector2.zero;
        }
        Animations();
    }

    private void FixedUpdate() {
        // Move the player
        rb2d.MovePosition(rb2d.position + mov * speed * Time.deltaTime);
    }

    /*
     * Check if te player wants and can use the sword
     */
    private void SwordAttack() {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        bool attacking = stateInfo.IsName("Player_Attack");

        if (Input.GetButtonDown("B - Sword") && !attacking && !movePrevent) {
            if (!interactingWithObject) {
                anim.SetTrigger("attacking");
                movePrevent = true;
                StartCoroutine(EnableMovementAfter(0.5F));
            }
        }
        if (mov != Vector2.zero) {
            // Move the collider to the position the player is orientated
            Vector2 movNorm = GetMovNormalized(false);
            attackHitbox.offset = new Vector2(
                movNorm.x / 2,
                movNorm.y / 2
            );
        }
        if (attacking) {
            // Enable the collider only in a specific time of the animation
            float playBackTime = stateInfo.normalizedTime % 1;
            if (playBackTime > 0.22 && playBackTime < 0.77) {
                attackHitbox.enabled = true;
            } else {
                attackHitbox.enabled = false;
            }
        }
    }

    /*
     * Check if te player wants and can use an artifact
     */
    private void ArtifactAction() {
        if (Input.GetButtonDown("Y - Artifact") && !artifactActing && !movePrevent) {
            if (!interactingWithObject) {
                anim.SetTrigger("startAction");
                movePrevent = true;

                UseArtifact();
            }
        }
    }

    /*
     * Method to use the actual artifact
     */
    private void UseArtifact() {
        GameObject artifactPrefab = artifacts[0];
        GameObject artifact;

        // Check what artifact is
        if (artifactPrefab.name.Equals("Arrow")) {
            // Instantiate it with the angle and forward force
            artifact = Instantiate(
                artifactPrefab,
                transform.position,
                Quaternion.AngleAxis(GetAngle(true), Vector3.forward)
            );
            artifact.GetComponent<Arrow>().mov = GetMovNormalized(true);
            StartCoroutine(EnableMovementAfter(0.3F));
            artifactActing = true;
        } else if (artifactPrefab.name.Equals("Hook")) {
            // Instantiate it with the orthogonal direction and forward force
            artifact = Instantiate(
                artifactPrefab,
                transform.position,
                Quaternion.AngleAxis(GetAngle(false), Vector3.forward)
            );
            artifact.GetComponent<Hook>().mov = GetMovNormalized(false);
            artifact.GetComponent<Hook>().initialPos = transform.position;
            artifactActing = true;
        } else if (artifactPrefab.name.Equals("Bomb")) {
            // Only needed to Instantiate it
            Instantiate(
                artifactPrefab,
                transform.position,
                Quaternion.identity                
            );
            movePrevent = false;
        }
    }

    /*
     * Coroutine that rises an item obtained through a chest
     */
    public IEnumerator RiseItem(Item item) {
        itemGetSprite.enabled = true;
        itemGetSprite.sprite = item.sprite;
        anim.SetBool("risingItem", true);
        dialogController.ShowItem(item);

        while (DialogController.isTextShown) {
            yield return null;
        }

        anim.SetBool("risingItem", false);
        itemGetSprite.enabled = false;
    }

    private IEnumerator EnableMovementAfter(float seconds) {
        yield return new WaitForSeconds(seconds);
        movePrevent = false;
    }

    /*
     * Coroutine to reload the scene by petition of the player
     */
    private IEnumerator TimeBack() {
        dialogController.TimeBackConfirm();
        while (dialogController.result == DialogController.Result.None)
            yield return null;

        if (dialogController.result == DialogController.Result.Yes) {
            movePrevent = true;
            GameObject.FindGameObjectWithTag("Crossfade").GetComponent<Crossfade>().Transition("Start");
            yield return new WaitForSeconds(2.0f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    /*
     * Get the raw movement of the player
     */
    private void Movement() {
        mov = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );
    }

    /*
     * Update the player animations
     */
    public void Animations() {
        if (mov != Vector2.zero) {
            anim.SetFloat("movX", mov.x);
            anim.SetFloat("movY", mov.y);
            anim.SetBool("walking", true);
        } else {
            anim.SetBool("walking", false);
        }
    }

    private void PreventMove() {
        if (movePrevent) {
            mov = Vector2.zero;
        }
    }

    /*
     * Get the actual movement of the player normalized
     */
    public Vector2 GetMovNormalized(bool permitDiagonals) {
        Vector2 movAnim = new Vector2(anim.GetFloat("movX"), anim.GetFloat("movY"));
        if (!permitDiagonals) {
            if (Mathf.Abs(movAnim.x) < Mathf.Abs(movAnim.y)) {
                movAnim.x = 0;
            } else {
                movAnim.y = 0;
            }
        }
        return new Vector2(
            (movAnim.x == 0) ? 0 : (movAnim.x < 0) ? Mathf.Floor(movAnim.x) : Mathf.Ceil(movAnim.x),
            (movAnim.y == 0) ? 0 : (movAnim.y < 0) ? Mathf.Floor(movAnim.y) : Mathf.Ceil(movAnim.y)
        );
    }

    /*
     * Get the angle of the player
     */
    public float GetAngle(bool permitDiagonals) {
        mov = GetMovNormalized(permitDiagonals);
        return Mathf.Atan2(
            mov.y, mov.x
        ) * Mathf.Rad2Deg;
    }

    /*
     * Method that loads the GameObjects relative to the artifacts unlocked
     */
    public void GetArtefacts() {
        for (int i = 0; i < GameManager.artifactsUnlock.Length; i++) {
            if (GameManager.artifactsUnlock[i] != null) {
                artifacts[i] = Resources.Load("Prefabs/Artifacts/" + GameManager.artifactsUnlock[i]) as GameObject;
            }
        }
    }

}
