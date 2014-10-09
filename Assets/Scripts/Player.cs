using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	float diag = Mathf.Sqrt(2);
	bool movingX = false;
	bool movingY = false;
	bool moving;
	float cos = 0f;
	Vector2 direction = new Vector2(0,0);
	int directionInt = 0;
	
	public GameObject arrow;
	public GameObject projectile;
	public int moveForce = 500000;
	public int maxSpeed = 10;
	public int rotationSpeed = 500;
	public float friction = .9f;

	
	public bool grabbing = false;
	public bool xbox = false;

	public GameObject ammoCounter;
	private AmmoCounter ac;
	public GameObject hpbar;
	private PlayerHPBar hpb;

	public int maxHealth = 100;
	public float health = 100;

	Transform currentWeapon = null;
	Collider2D[] enemiesInRange;
	
	// Use this for initialization
	void Start () {
		enemiesInRange = Physics2D.OverlapCircleAll(transform.position, 5);
		
		if (currentWeapon == null) {
			foreach (Transform child in transform) {
				if (child.gameObject.CompareTag("item")) {
					currentWeapon = child;
				}
			}
			//currentWeapon = 
		}
		ac = ammoCounter.GetComponent<AmmoCounter>();
		ac.newWeapon(currentWeapon.GetComponent<Weapon>());
		hpb = hpbar.GetComponent<PlayerHPBar>();
		hpb.max = health;
		hpb.current = hpb.max;
		hpb.setHP (health);
	}
	
	// Update is called once per frame
	void Update () {
		faceDirection(directionInt);
		checkGrabbing();
		weaponControl();
		//regenHP();

        //movement
        moving = (movingX || movingY);

        movingX = false;
        movingY = false;
        Controls();
        if (rigidbody2D.velocity.magnitude > maxSpeed)
        {
            rigidbody2D.velocity = Vector2.ClampMagnitude(rigidbody2D.velocity, maxSpeed);
        }

	}
	
	void FixedUpdate() {

		
		//controls max speed

		//rigidbody2D.velocity = Vector2.ClampMagnitude(rigidbody2D.velocity, rigidbody2D.velocity.magnitude * (1 + (cos * .6f)));
		//friction when not moving (ie stopping force)

        if (!moving)
        {
            this.gameObject.rigidbody2D.velocity *= (friction);
        }
	}
	
	void Controls() {
	
		#region movement
		//note: direction is dominated by up/down because they are later
		if (!xbox) {
			if (Input.GetKey (KeyCode.D)) {
				moveSide(1);
				directionInt = 0;
			}
			
			if (Input.GetKey (KeyCode.A)) {
				moveSide(-1);
				directionInt = 4;
			}
			
			if (Input.GetKey (KeyCode.W)) {
				moveVertical(1);
				directionInt = 2;
			}
			
			if (Input.GetKey (KeyCode.S)) {
				moveVertical(-1);
				directionInt = 6;
			}

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (Time.timeScale == 1)
                {
                    Time.timeScale = .1f;
                    Time.fixedDeltaTime = .01666667f * Time.timeScale;
                    //rigidbody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
                }
                else
                {
                    Time.timeScale = 1;
                    Time.fixedDeltaTime = 1 / 60f;
                  
                }
            }
			
			//cos = Mathf.Cos(Vector2.Angle(rigidbody2D.velocity, new Vector2(1, Mathf.Tan(transform.eulerAngles.z) )) * Mathf.Deg2Rad);
			
			//print (Mathf.Tan(transform.eulerAngles.z));
			//cos -= 1f;
			//cos/= 2;
			
			//diagonal direction keys
			
			if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D)) {
				directionInt = 1;
			}
			else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A)) {
				directionInt = 3;
			}
			else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D)) {
				directionInt = 7;
			}
			else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A)) {
				directionInt = 5;
			}
		}
		#endregion
		
		else {
		
			float movex = Input.GetAxis("MoveX");
			float movey = Input.GetAxis("MoveY");
			float aimx = Input.GetAxis("AimX");
			float aimy = Input.GetAxis ("AimY");
			moving = (movex != 0 || movey != 0);
	
			if(aimx != 0 || aimy != 0)
			{
				cos = Mathf.Cos (Vector2.Angle(new Vector2(movex, movey), new Vector2(aimx, aimy)) * Mathf.Deg2Rad);
				//Debug.Log (cos);
				cos -= 1f;
				cos /= 2;
			}
			else
			{
				cos = 0f;
			}
			if(moving)
			{
				Vector2 moveVector = new Vector2(movex, movey);
				moveVector *= moveForce;
				rigidbody2D.AddForce(moveVector);
			}
		
		}
	}
	
	void weaponControl() {
		if (xbox) {
			if(currentWeapon.GetComponent<Weapon>().fullauto)
			{
				if (Input.GetAxis("Fire1") > 0) 
				{
					if(currentWeapon.GetComponent<Weapon>().shoot ())
					{
						ac.fire ();
					}
				} 
			}
			else
			{
				if(Input.GetAxis("Fire1") > 0)
				{
					if(!currentWeapon.GetComponent<Weapon>().fired)
					{
						currentWeapon.GetComponent<Weapon>().fired = true;
						if(currentWeapon.GetComponent<Weapon>().shoot ())
						{
							ac.fire ();
						}					}
				}
				else if(Input.GetAxis ("Fire1")==0)
				{
					currentWeapon.GetComponent<Weapon>().fired = false;
				}
			}
			
			if (Input.GetButtonDown("Reload")) {
				//currentWeapon.GetComponent<Weapon>().reload();
				ac.reload();
			}
		}
		
		else {
			//fire
			if(currentWeapon.GetComponent<Weapon>().fullauto)
			{
				if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) 
				{
					if(currentWeapon.GetComponent<Weapon>().shoot ())
					{
						ac.fire ();
					}				
				} 
			}
			else
			{
				if(Input.GetMouseButtonDown(0))
				{
					if(currentWeapon.GetComponent<Weapon>().shoot ())
					{
						ac.fire ();
					}				
				}
			}
			
			if (Input.GetKeyDown(KeyCode.R)) {
				//currentWeapon.GetComponent<Weapon>().reload ();
				ac.reload();
			}
		}
	}
	
	
	
	void checkGrabbing() {
		if (!xbox) {
			if (Input.GetKeyDown( KeyCode.E)) {
				
				grabbing = true;
			}
			if (Input.GetKeyUp( KeyCode.E)) {
				grabbing = false;
			}
		}
		
		else {
			if (Input.GetButtonDown("Use")) {
				grabbing = true;
			}
			if (Input.GetButtonUp("Use")) {
				grabbing = false;
			}
		}
	}
	
	void moveSide(float multiplier) {
	
		if (movingY) {
			rigidbody2D.AddForce (new Vector2 (multiplier * moveForce * Time.deltaTime / diag, 0));
			direction = new Vector2(multiplier,direction.y);
			
		} else {
            rigidbody2D.AddForce(new Vector2(multiplier * moveForce * Time.deltaTime, 0));			
			direction = new Vector2(multiplier, 0);
			
			}		
		movingX = true;
	}
	
	void moveVertical(float multiplier) {
		
		if (movingX) {
            rigidbody2D.AddForce(new Vector2(0, multiplier * moveForce * Time.deltaTime / diag));
			direction = new Vector2(direction.x, multiplier);
			
			
		} else {
            rigidbody2D.AddForce(new Vector2(0, multiplier * moveForce * Time.deltaTime));			
			direction = new Vector2(0, multiplier);
			
			}
		movingY = true;
	}
	
	void faceDirection(int direction) {		
		//aim in direction of movement
		Vector3 hurr;
		//aim at mouse
		if (!xbox) {
			Vector3 moz = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			hurr = new Vector3( moz.x, moz.y, 0);
			float angle = Mathf.Atan2(hurr.y - transform.position.y, hurr.x - transform.position.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0,0,angle), rotationSpeed * Time.deltaTime);
			
		}
		
		//xbox
		else {
			
			float aimx = Input.GetAxis("AimX");
			float aimy = Input.GetAxis ("AimY");
			hurr = new Vector3(this.transform.position.x + aimx, this.transform.position.y + aimy, 0);
			if(aimx==0 && aimy==0)
			{
				aimx = Input.GetAxis("MoveX");
				aimy = Input.GetAxis ("MoveY");
				if(aimx!=0 || aimy!=0)
				{
					hurr = new Vector3(this.transform.position.x + aimx, this.transform.position.y + aimy, 0);
				}
			}
			float angle = Mathf.Atan2(hurr.y - transform.position.y, hurr.x - transform.position.x) * Mathf.Rad2Deg;
			
			if(aimx!=0 || aimy!=0)
			{
				transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0,0,angle), rotationSpeed * Time.deltaTime);
			}
		}
		Debug.DrawLine(this.transform.position, hurr, Color.red);
		/*
		float angle = Mathf.Atan2(hurr.y - transform.position.y, hurr.x - transform.position.x) * Mathf.Rad2Deg;
		if(aimx!=0 || aimy!=0)
		{
			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0,0,angle), rotationSpeed);
		}
		*/	

			
	}
	
	void turnLeft() {

	}
	
	void turnRight() {
	
	}
	
	void turn180() {
	
	}
	
	void shoot() {
	}
	
	public void grabObject(GameObject item) {
		

		currentWeapon.GetComponent<Weapon>().enabled = false;
		currentWeapon.GetComponent<Item>().enabled = true;
		currentWeapon.GetComponent<CircleCollider2D>().enabled = true;
		currentWeapon.parent = null;
		
		//gameObject.transform.FindChild(
		//gameObject.transform.GetComponentInChildren<Weapon>().enabled = false;
		//gameObject.transform.GetComponentInChildren<Item>().enabled = true;
		
		//gameObject.transform.GetChild(0).GetComponent<Weapon>().enabled = false;
		//gameObject.transform.GetChild(0).GetComponent<Item>().enabled = true;
		//gameObject.transform.GetChild(0).transform.parent = null;
		
		
	
		//print ("CALLLED");
		item.transform.parent = gameObject.transform;
		item.transform.localPosition = new Vector3(.5f, 0, 0);
		item.transform.localRotation = Quaternion.Euler(0,0,270);
		item.GetComponent<Weapon>().enabled = true;
		item.GetComponent<Item>().enabled = false;
		item.GetComponent<CircleCollider2D>().enabled = false;
		
		
		currentWeapon = item.transform;
		ac.newWeapon(currentWeapon.GetComponent<Weapon>());
		//Destroy(item.GetComponent<Item>());
		
	}
	
	void regenHP() {
		if (health < maxHealth) {
			health += Time.deltaTime /5f;
			hpb.setHP(health);
		}
	}
	
	void OnCollisionEnter2D (Collision2D other) {
		if (other.gameObject.CompareTag("enemy")) {
			print ("take damage");
			health-=2;
			if (health <= 0) {
				health = 0;
				death ();
			}
			hpb.setHP(health);
		}
	}
	

	
	void death() {
		Destroy (gameObject);
	}
	
}
