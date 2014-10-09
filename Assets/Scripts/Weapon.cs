using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

	public GameObject projectile;
	public float damage = 10;
	public float cooldown = 1f;
	public float accuracy = 5;
	public float recoil = 1;
	public int maxRecoil = 20;
	public float recoilRecoveryTime = .15f;
	public float recoilRecoverySpeed = .3f;
	public int numProjectiles = 20;
	public int knockback = 1;
	public int noise = 7;
	public float cameraRecoil = .1f;
	public float cameraRecoilAmount = .2f;
	public int magSize = 30;
	public int currentMag;
	public float reloadTime;
	public bool fullauto = true;
	public bool fired;
	public bool reloading;
	
	float spread;
	bool canShoot = true;
	float timer;
	float stoppedShootingTime = 0;
	
	
	// Use this for initialization
	void Start () {
		timer = cooldown;
		currentMag = magSize;
		spread = accuracy;
		transform.FindChild("GUI Text").guiText.enabled =false;
	}
	
	// Update is called once per frame
	void Update () {
		float r = 5;
		Vector3 endpoint = new Vector3(r*Mathf.Cos((transform.rotation.eulerAngles.z + 90)* Mathf.Deg2Rad), r*Mathf.Sin((transform.rotation.eulerAngles.z + 90) * Mathf.Deg2Rad), 0);
		Debug.DrawRay(transform.position + endpoint.normalized*.1f, endpoint );
		if(fired && Input.GetAxis("Fire1") == 0)
		{
			fired = false;
		}
	}
	
	void FixedUpdate() {
	
		if (timer >= cooldown) {
			canShoot = true;
			recoilRecovery();
		}
		else {
			timer += Time.fixedDeltaTime;
			canShoot = false;
			stoppedShootingTime = 0;
		}
	}
	
	void weaponControl() {
		if(fullauto)
		{
			if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) 
			{
				shoot ();
			} 
		}
		else
		{
			if(Input.GetMouseButtonDown(0))
			{
				shoot ();
			}
		}
	
	}
	
	public bool shoot() {
	
		if (canShoot && currentMag > 0 && !reloading) {
		//if (!audio.isPlaying) {
			audio.Play();
			currentMag--;
		//}
			Vector3 position = new Vector3(transform.position.x,transform.position.y,transform.position.z +1);
			for (int i = 0; i< numProjectiles;i++ ) {
				Quaternion rotation = transform.rotation * Quaternion.Euler(0,0,Random.Range(-spread,spread));
				GameObject proj = (GameObject)(Instantiate (projectile, position, rotation));
				proj.rigidbody2D.mass = knockback;
				proj.GetComponent<Projectile>().setDamage(damage);
			}
			float r = 5;
			Vector3 endpoint = new Vector3(r*Mathf.Cos((transform.rotation.eulerAngles.z + 90)* Mathf.Deg2Rad), r*Mathf.Sin((transform.rotation.eulerAngles.z + 90) * Mathf.Deg2Rad), 0);
			
			RaycastHit2D hit = Physics2D.Raycast(transform.position + endpoint.normalized*.1f, endpoint, 50);
			
			//Camera.main.transform.Translate(Random.insideUnitCircle * cameraRecoil * Time.deltaTime,0);
			Camera.main.GetComponent<CameraScript2>().shakeAmount = cameraRecoilAmount;
			Camera.main.GetComponent<CameraScript2>().shake = cameraRecoil;

			if (spread < maxRecoil) {
				spread += recoil;
			}
			
			//aggro zombies
			
			Collider2D[] inRange = Physics2D.OverlapCircleAll(new Vector2(this.transform.position.x, this.transform.position.y), noise);
			foreach(Collider2D col2d in inRange)
			{
				if(col2d.CompareTag("enemy"))
				{
					if (col2d.gameObject.GetComponent<zombie>() != null) {
						col2d.gameObject.GetComponent<zombie>().setTargetLoc(this.transform.position);
					}
				}
			}

			canShoot = false;
			timer = 0;
			return true;
		}
		return false;
	}
	
	public void reload() {
		currentMag = magSize;
		reloading = false;
	}
	
	void recoilRecovery() {
		if (stoppedShootingTime < recoilRecoveryTime) {
			stoppedShootingTime += Time.fixedDeltaTime;
		}
		if (stoppedShootingTime >= recoilRecoveryTime && spread > accuracy) {
			spread -= recoilRecoverySpeed;
			if (spread < accuracy) spread = accuracy;
		}
	}
}
