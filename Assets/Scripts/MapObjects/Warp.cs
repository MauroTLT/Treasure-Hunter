using System.Collections;
using UnityEngine;

public class Warp : MonoBehaviour {
    [Space]
    [Header("Boolean and Zone Atributtes")]
    public bool stairs;
    public bool canPassObjects;
    public bool saveAsLastWarp = true;
    public bool changeScene;
    
    public Zone zoneTo;
    [Space]
    [Header("Game Objects")]
    public GameObject target;
    public GameObject targetMap;
    public GameObject doorToOpen;
    public GameObject activator;

    private GameObject crossfade;

    private void Awake() {
        if (!stairs) {
            GetComponent<SpriteRenderer>().enabled = false;
        }
        transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        crossfade = GameObject.FindGameObjectWithTag("Crossfade");
    }

    private IEnumerator OnTriggerEnter2D(Collider2D other) {
        // Check if the player can pass trough
        if (activator == null || (activator != null && activator.GetComponent<Switch>().activated)) {
            if (other.tag == "Player") {
                if (changeScene) {
                    StartCoroutine(ChangeScene());
                } else if (!other.GetComponent<Player>().interactingWithObject || (canPassObjects && other.GetComponent<Player>().interactingWithObject)) {
                    // Save the warp if you can, to use it when restarting the room
                    if (saveAsLastWarp) {
                        GameManager.lastWarp = this.name;
                    }

                    other.GetComponent<Animator>().enabled = false;
                    other.GetComponent<Player>().enabled = false;

                    crossfade.GetComponent<Crossfade>().Transition("Start");

                    yield return new WaitForSeconds(Crossfade.transitionTime);

                    TransportPlayer(other.gameObject);

                    crossfade.GetComponent<Crossfade>().Transition("End");

                    other.GetComponent<Animator>().enabled = true;
                    other.GetComponent<Player>().enabled = true;
                }
            }
        }
    }

    /*
     * Transition to next scene 
     */
    private IEnumerator ChangeScene() {
        crossfade.GetComponent<Crossfade>().Transition("Start");

        yield return new WaitForSeconds(Crossfade.transitionTime);

        GameManager.ChangeScene(zoneTo);
    }

    /*
     * Transport the player to warp linked with this
     * also prepare the camera to handle the new room
     */
    public void TransportPlayer(GameObject player) {
        if (doorToOpen != null) {
            doorToOpen.GetComponent<Door>().Open();
        }
        player.transform.position = new Vector3(
            target.transform.GetChild(0).transform.position.x,
            target.transform.GetChild(0).transform.position.y,
            0
        );
        Camera.main.GetComponent<MainCamera>().SetBounds(targetMap);
    }
}
