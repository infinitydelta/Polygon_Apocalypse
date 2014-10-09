using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {
	Player nearPlayer;
	Transform gui;
	// Use this for initialization

	
	void Start () {
		gui = gameObject.transform.GetChild(0);
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	void OnTriggerStay2D (Collider2D other) {
		if (other.gameObject.CompareTag("Player")) {
			nearPlayer = other.gameObject.GetComponent<Player>();
			//display ui

			gui.transform.position = Camera.main.WorldToViewportPoint(transform.position);
			gui.guiText.enabled = true;
			
			if ( nearPlayer.grabbing) {
				//print ("update grab");
				nearPlayer.grabObject(gameObject);
				gui.guiText.enabled = false;
				nearPlayer.grabbing = false;
			}

		}
	}
	
	void OnTriggerExit2D (Collider2D other) {
	
		if (nearPlayer != null) {
			nearPlayer = null;
			gui.guiText.enabled = false;
		}
	}
}
