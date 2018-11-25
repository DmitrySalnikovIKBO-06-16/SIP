using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_script : MonoBehaviour {

    public Transform target;
    void Update () {
        transform.LookAt(target);

    }
}
