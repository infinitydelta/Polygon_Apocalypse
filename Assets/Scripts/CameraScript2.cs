using UnityEngine;
using System.Collections;

public class CameraScript2 : MonoBehaviour {
	
	public float minSize = 4f;
	public float extraSize = 1f;
	public float yOffset = 2f;
	
	public float easeAmount = 10f;
	public GameObject[] myPlayers;
	
	private Vector3 targetPosition;
	private float xTargetSize;
	private float yTargetSize;
	
	// How long the object should shake for.
	public float shake = 0f;
	
	// Amplitude of the shake. A larger value shakes the camera harder.
	public float shakeAmount = 0.3f;
	public float decreaseFactor = 1.0f;
	Vector3 originalPos;
	
	// Use this for initialization
	void Start () {
		//myPlayers = GameObject.FindGameObjectsWithTag ("Player");
		//pos = transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		myPlayers = GameObject.FindGameObjectsWithTag ("Player");
        /*//death gui
		if (myPlayers.Length == 0) {
			transform.GetChild(0).position = new Vector3(.5f, .5f, 10);
			transform.GetChild(0).guiText.enabled = true;
			Destroy(this);
			return;
		}*/
        
        //muh camera shake
		if (shake > 0)
		{
			//
			transform.position = originalPos + Random.insideUnitSphere * shakeAmount;
			
			shake -= Time.fixedDeltaTime * decreaseFactor;
		}
		else
		{
			shake = 0f;
			originalPos = transform.position;
		}
		
		
		//center on multiple players
		float leftmostPos = myPlayers[0].transform.position.x;
		float rightmostPos = myPlayers[0].transform.position.x;
		float topmostPos = myPlayers[0].transform.position.y;
		float bottommostPos = myPlayers[0].transform.position.y;
		float xsum = 0;
		float ysum = 0;
		for (int i = 0; i < myPlayers.Length; i ++) {
			if (myPlayers[i].transform.position.x < leftmostPos) {
				leftmostPos = myPlayers[i].transform.position.x;
			}
			if (myPlayers[i].transform.position.x > rightmostPos) {
				rightmostPos = myPlayers[i].transform.position.x;
			}
			if (myPlayers[i].transform.position.y < bottommostPos) {
				bottommostPos = myPlayers[i].transform.position.y;
			}
			if (myPlayers[i].transform.position.y > topmostPos) {
				topmostPos = myPlayers[i].transform.position.y;
			}
			xsum += myPlayers[i].transform.position.x;
			ysum += myPlayers[i].transform.position.y;
		}
		xTargetSize = ((rightmostPos - leftmostPos) / 2f) + extraSize;
		if (xTargetSize < minSize)
			xTargetSize = minSize;
		yTargetSize = ((topmostPos - bottommostPos) / 2f) + extraSize;
		if (yTargetSize < minSize)
			yTargetSize = minSize;
		if (xTargetSize > yTargetSize)
			camera.orthographicSize += (xTargetSize - camera.orthographicSize) / easeAmount;
		else
			camera.orthographicSize += (yTargetSize - camera.orthographicSize) / easeAmount;

        
		targetPosition = new Vector3(xsum / myPlayers.Length, (ysum / myPlayers.Length), -10f);
		transform.position += (targetPosition - transform.position) / easeAmount;
        originalPos = transform.position;
		
	}
}