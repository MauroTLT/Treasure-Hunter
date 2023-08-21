using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour {

    // Delay time for the camera to cath up the player
    public float smoothTime = 1.0f;

    // Player
    private Transform target;
    // Corners of the actual room, used to restrict the camera
    private float topLeftX, topLeftY, bottomRightX, bottomRightY;

    private GameObject map;
    private Vector2 velocity;
    private float aspect;
    private bool adjust;

    void Awake() {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        aspect = Camera.main.aspect;
    }

    void Update() {
        // Emergency exit of the aplication
        if (Input.GetKey("escape")) Application.Quit();
        
        // Move the camera towards the player with the delay
        float posX = Mathf.Round(
            Mathf.SmoothDamp(
                transform.position.x,
                target.position.x,
                ref velocity.x,
                smoothTime)
            * 100) / 100;

        float posY = Mathf.Round(
            Mathf.SmoothDamp(
                transform.position.y,
                target.position.y,
                ref velocity.y,
                smoothTime)
            * 100) / 100;

        // If can adjust the camera to the room try it
        if (adjust) {
            // The game window has been modified, re-check the bounds
            if (aspect != Camera.main.aspect) {
                Debug.Log("ASPECT : " + Camera.main.aspect);
                AdjustBounds();
                aspect = Camera.main.aspect;
            }
            transform.position = new Vector3(
                Mathf.Clamp(posX, topLeftX, bottomRightX),
                Mathf.Clamp(posY, bottomRightY, topLeftY),
                transform.position.z
            );
        } else {
            // Just follow the player
            transform.position = new Vector3(
                posX,
                posY,
                transform.position.z
            );
        }
        
    }

    /*
     * Set the actual room and check if can adjust the camera inside it
     */
    public void SetBounds(GameObject map) {
        adjust = map.GetComponent<Tiled2Unity.TiledMap>().NumTilesWide > 20;
        this.map = map;
        AdjustBounds();
    }

    /*
     * Adjust the camera to the inside of the room
     * Calculate the bounds based on the camera aspect and size and the wide and high of the room, in tiles
     */
    private void AdjustBounds() {
        Tiled2Unity.TiledMap config = map.GetComponent<Tiled2Unity.TiledMap>();
        float x = ((1.7F * Camera.main.aspect) / 1.353065F) * Camera.main.aspect;
        float cameraSize = Camera.main.orthographicSize;

        topLeftX = map.transform.position.x + cameraSize + x;
        topLeftY = map.transform.position.y - cameraSize;
        bottomRightX = map.transform.position.x + config.NumTilesWide - cameraSize - x;
        bottomRightY = map.transform.position.y - config.NumTilesHigh + cameraSize;

        FastMove();
    }

    /*
     * Move the camera to player's position instantly
     */
    public void FastMove() {
        if (target == null) {
            Awake();
        }
        transform.position = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z
        );
    }
}
