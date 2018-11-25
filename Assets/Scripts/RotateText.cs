using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateText : MonoBehaviour {

    Transform tr;

	// Use this for initialization
	void Start () {
        tr = Camera.main.transform;
	}
	
	// Update is called once per frame
	void Update () {
        if (!tr)
            tr = Camera.main.transform;
        transform.rotation = Quaternion.LookRotation(transform.position - tr.position);

    }
}
