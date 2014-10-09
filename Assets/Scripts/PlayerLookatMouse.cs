using UnityEngine;
using System.Collections;

public class PlayerLookatMouse : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	
	void faceDirection(int direction) {		
		Vector3 p = transform.position - camera.ScreenToWorldPoint(Input.mousePosition);
		//transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(p), rotationSpeed);
	}
}

