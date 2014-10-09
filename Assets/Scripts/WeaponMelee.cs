using UnityEngine;
using System.Collections;

public class WeaponMelee : MonoBehaviour {

	public GameObject projectile;
	public float cooldown = 0;
	public float accuracy = 5;
	public float recoil = 1;
	public int maxRecoil = 20;
	public float recoilRecoveryTime = .3f;
	public float recoilRecoverySpeed = .3f;
	float spread;
	bool canShoot = true;
	float timer;
	float stoppedShootingTime = 0;
	// Use this for initialization
	void Start () {
		spread = accuracy;
	}
	
	// Update is called once per frame
	void Update () {
		weaponControl();

//		print (spread);
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
		if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) {
			shoot ();
		} 
	
	}
	
	void shoot() {
		if (canShoot) {
			Vector3 position = new Vector3(transform.position.x,transform.position.y,transform.position.z +1);
			Quaternion rotation = transform.rotation * Quaternion.Euler(0,0,Random.Range(-spread,spread));
			//Instantiate (projectile, position, rotation);
			int angle = -300;
			int rotationSpeed = -50;
			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0,0,angle), rotationSpeed);
			
			//vel = new Vector2(-Mathf.Sin(transform.rotation.eulerAngles.z * Mathf.Deg2Rad) * speed, speed *Mathf.Cos(transform.rotation.eulerAngles.z * Mathf.Deg2Rad));
			//RaycastHit2D hit = Physics2D.Raycast(new Vector2(position.x, position.y), new Vector2(-Mathf.Sin(rotation.eulerAngles.z * Mathf.Deg2Rad) , Mathf.Cos(rotation.eulerAngles.z * Mathf.Deg2Rad)), 50f, 1);
//			if (hit) {
//				print (Vector3.Magnitude( hit.transform.position - transform.position));
//			}
			
			if (spread < maxRecoil) {
				spread += recoil;
			}
			canShoot = false;
			timer = 0;
		}
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
