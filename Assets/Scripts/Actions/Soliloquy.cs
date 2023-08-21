using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soliloquy : ShowText {

    private void Start() {
        GetComponent<Renderer>().enabled = false;
    }

}
