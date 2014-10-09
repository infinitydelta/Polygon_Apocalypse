using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {
	
	public GameObject player;
	public float movePredictionStr = .2f;
	public int mouseStr = 5;
	public float smoothing = 5f * 2;

	float multiplier = 20f;
	float cameraRecoil = 25f;
	float gunRecoil;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//player.rigidbody2D.velocity.x
		//transform.Translate ((playerPos.x + playerscript.currentspeed * 2.0f - transform.position.x)/10, (playerPos.y+1 - transform.position.y)/10 - .2f, 0);
		if (Input.GetMouseButton(0)) {
			//transform.Translate(Random.insideUnitCircle * cameraRecoil * Time.deltaTime,0);
		}
        //Vector3 moz = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //transform.Translate((player.transform.position.x + player.rigidbody2D.velocity.x * movePredictionStr - transform.position.x + (moz.x - player.transform.position.x) / mouseStr) / smoothing, (player.transform.position.y + player.rigidbody2D.velocity.y * movePredictionStr - transform.position.y + (moz.y - player.transform.position.y) / mouseStr) / smoothing, 0);

	}
	
	void FixedUpdate() {
	
		Vector3 moz = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		//Vector3 moz = new Vector3(player.transform.position.x + (Input.GetAxis("AimX") * multiplier), player.transform.position.y + (Input.GetAxis("AimY") * multiplier), 0);

		transform.Translate ( (player.transform.position.x + player.rigidbody2D.velocity.x * movePredictionStr - transform.position.x + (moz.x - player.transform.position.x )/mouseStr) /smoothing, 	(player.transform.position.y + player.rigidbody2D.velocity.y * movePredictionStr - transform.position.y + (moz.y - player.transform.position.y)/mouseStr) /smoothing, 0);

	}
}
