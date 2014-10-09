using UnityEngine;
using System.Collections;

public class WeaponXbox : MonoBehaviour {
	
	public GameObject projectile;
	public float damage = 10;
	public float cooldown = 1f;
	public float accuracy = 5;
	public float recoil = 1;
	public int maxRecoil = 20;
	public float recoilRecoveryTime = .3f;
	public float recoilRecoverySpeed = .3f;
	public float cameraRecoil = 25f;
	public int numProjectiles = 20;
	public int knockback = 1;
	public int noise = 7;
	public bool fullauto = true;
	private bool fired;
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
		float r = 5;
		Vector3 endpoint = new Vector3(r*Mathf.Cos((transform.rotation.eulerAngles.z + 90)* Mathf.Deg2Rad), r*Mathf.Sin((transform.rotation.eulerAngles.z + 90) * Mathf.Deg2Rad), 0);
		Debug.DrawRay(transform.position + endpoint.normalized*.1f, endpoint );
		if(fired && Input.GetAxis("Fire1") == 0)
		{
			fired = false;
		}
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
		if(fullauto)
		{
			if (Input.GetAxis("Fire1") > 0) 
			{
				shoot ();
			} 
		}
		else
		{
			if(Input.GetAxis("Fire1") > 0)
			{
				if(!fired)
				{
					fired = true;
					shoot();
				}
			}
		}
		
	}
	
	void shoot() {
		if (canShoot) {
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
			
			Camera.main.transform.Translate(Random.insideUnitCircle * cameraRecoil * Time.deltaTime,0);
			
			//if (hit) print ("shoot: " + hit.collider.gameObject);
			
			//Debug.DrawRay(transform.position , endpoint, Color.red);
			//vel = new Vector2(-Mathf.Sin(transform.rotation.eulerAngles.z * Mathf.Deg2Rad) * speed, speed *Mathf.Cos(transform.rotation.eulerAngles.z * Mathf.Deg2Rad));
			//RaycastHit2D hit = Physics2D.Raycast(new Vector2(position.x, position.y), new Vector2(-Mathf.Sin(rotation.eulerAngles.z * Mathf.Deg2Rad) , Mathf.Cos(rotation.eulerAngles.z * Mathf.Deg2Rad)), 50f, 1);
			//			if (hit) {
			//				print (Vector3.Magnitude( hit.transform.position - transform.position));
			//			}
			
			if (spread < maxRecoil) {
				spread += recoil;
			}
			
			Collider2D[] inRange = Physics2D.OverlapCircleAll(new Vector2(this.transform.position.x, this.transform.position.y), noise);
			foreach(Collider2D col2d in inRange)
			{
				if(col2d.CompareTag("enemy"))
				{
					col2d.gameObject.GetComponent<zombie>().setTargetLoc(this.transform.position);
				}
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
