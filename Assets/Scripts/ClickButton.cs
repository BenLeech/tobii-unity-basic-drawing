using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;

public class ClickButton : MonoBehaviour {

    public GameObject brushObject;
    public GameObject brushManager;

	// Use this for initialization
	void Start () {
		if(brushObject==null)
        {
            brushObject = transform.GetChild(0).gameObject;
        }
	}
	
	// Update is called once per frame
	void Update () {

        GameObject focusedObject = TobiiAPI.GetFocusedObject();

        if(focusedObject == gameObject)
        {
            brushManager.GetComponent<PlaceObject>().drawObject = brushObject;
        }
        

    }
}
