using UnityEngine;
using System.Collections;

public class GUIScale : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float newScale = Camera.main.GetComponent<Camera>().orthographicSize / 7f;
		transform.localScale = new Vector3(newScale, newScale, 1);
	}
}
