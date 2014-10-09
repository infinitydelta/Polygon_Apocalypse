using UnityEngine;
using System.Collections;
using Pathfinding;

public class zombie : MonoBehaviour {

	public GameObject[] targets;
	public GameObject target;
	public float maxSpeed;
	float detectableRange = 20;

	Path path;
	Seeker seeker;
	int currentWayPoint;
	public float rotationSpeed = 1f;

	Animator anim;
	public float health = 100;
	bool attacking = false;
	

	Vector3 targetLoc;
	bool moving = false;
	bool alive = true;
	// Use this for initialization
	void Start () {
		seeker = GetComponent<Seeker>();
		targetLoc = this.transform.position;
		anim = GetComponent<Animator>();
		//seeker.StartPath(transform.position, target.transform.position, OnPathComplete); //set path to target
		targets = GameObject.FindGameObjectsWithTag ("Player");
		target = targets[0];
		
	}
	void Update() {
		targets = GameObject.FindGameObjectsWithTag ("Player");
		if (targets.Length == 0) {
			this.enabled = false;
		}
		foreach (GameObject t in targets) {
			if (t != null) {
				if (target == null) {
					target = t;
				}
				if (Vector2.Distance(t.transform.position, transform.position) < Vector2.Distance(target.transform.position, transform.position)) {
					target = t;
				}
			}
		}
	
	}
	// Update is called once per frame
	//void Update () {
		
		/*
		//seeker.StartPath(transform.position, target.transform.position, OnPathComplete); //set path to target

		if (Vector3.Distance(transform.position, path.vectorPath[currentWayPoint]) < .3f) {
			currentWayPoint++;
		}
		faceDirection(target);
		rigidbody2D.AddForce((path.vectorPath[currentWayPoint] - transform.position).normalized * 500);
		*/
		
		//transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
		//print (rigidbody2D.velocity.magnitude);


	//}
	
	void FixedUpdate() 
	{
		moving = false;
		//Debug.Log(rigidbody2D.velocity);
		
		//		if (Vector2.Distance(target.transform.position, transform.position) <= 1) {
		//			rigidbody2D.velocity = Vector2.zero;
		//		}
		int layerMask = 1 << 10;
		layerMask = ~layerMask;
		if (inDetectableRange(target) && alive) {
			RaycastHit2D hit = Physics2D.Raycast(transform.position + (target.transform.position - transform.position).normalized, target.transform.position - transform.position, detectableRange, layerMask);
			//print (hit.collider.gameObject);
			Debug.DrawRay(transform.position, target.transform.position - transform.position);
			Debug.DrawRay(transform.position, targetLoc-transform.position, Color.green);
			//print (target.transform.rotation.eulerAngles.z - transform.rotation.eulerAngles.z);
			Vector2 dir = (new Vector2(Mathf.Cos(transform.rotation.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(transform.rotation.eulerAngles.z * Mathf.Deg2Rad)).normalized);
			if ((Vector2.Angle(target.transform.position - transform.position , dir)) <= 90 ) { //in front of
				if (hit.collider.gameObject.CompareTag("Player")) {
					
					targetLoc = target.transform.position;
				}
				else {
					
					//InvokeRepeating("findPath",10,10);
				}
			}
			if(Vector2.Distance(target.transform.position, transform.position) < 1.4f)
			{
				attackTarget(target);
			}
			else
			{
				moveTo(targetLoc);
			}
			
		}
		if (!moving) {
			//rigidbody2D.velocity *= .9f;
		}
		//print (moving);
		
		/*if (rigidbody2D.velocity.magnitude > maxSpeed) 
		{
			rigidbody2D.velocity = Vector2.ClampMagnitude(rigidbody2D.velocity, maxSpeed);
		}*/
	
	}
	
	void OnPathComplete(Path p) {
		if (!p.error) {
			path = p;
			currentWayPoint = 0;
			
		}
		else {
			Debug.Log(p.error);
		}
		
	}
	
	void faceDirection(Vector3 target) {		
		//aim in direction of movement
		//transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, direction * 45), rotationSpeed);

		Vector3 hurr = new Vector3( target.x, target.y, 0);
		float angle = Mathf.Atan2(hurr.y - transform.position.y, hurr.x - transform.position.x) * Mathf.Rad2Deg;
		//print (hurr);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0,0,angle), rotationSpeed);
		
		
	}
	
	bool inDetectableRange(GameObject target) {
		if (Vector3.Distance(target.transform.position, transform.position) <= detectableRange) {
			return true;		
		}
		return false;
	}
	
	void moveTo(Vector3 target) {
		moving = true;
		faceDirection(target);
		if (Vector2.Distance(target, transform.position) > .2f && rigidbody2D.velocity.magnitude < maxSpeed) {
			rigidbody2D.AddForce((target - transform.position).normalized * 3000);
		}
	}
	void attackTarget(GameObject t)
	{
		faceDirection(t.transform.position);
		if(!attacking)
		{
			attacking = true;
			anim.SetTrigger("Attack");
		}
	}
	
	void findPath() {
		seeker.StartPath(transform.position, target.transform.position, OnPathComplete); //set path to target
	}
	public void setTargetLoc(Vector3 newtarget)
	{
		targetLoc = newtarget;
	}

	public void damage(float dmg)
	{
		health -= dmg;
		
		if(health <= 0)
		{
			alive = false;
			rigidbody2D.mass = Random.Range(5f, 7f);
			//rigidbody2D.drag = .01f;
			rigidbody2D.AddTorque(Random.Range(-900f, 900f));
			//rigidbody2D.isKinematic = true;
			//this.rigidbody2D.drag = 0;
			//print (rigidbody2D.velocity);
			StartCoroutine( die() );
			
			transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
		}
	}
	public IEnumerator die()
	{
		yield return new WaitForSeconds(.5f);
		rigidbody2D.isKinematic = true;
		transform.GetChild(0).GetComponent<CircleCollider2D>().enabled = false;
		//transform.GetComponentInChildren<CircleCollider2D>().enabled = false;
		//this.GetComponent<CircleCollider2D>().enabled = false;
		anim.SetTrigger("Death");
		Destroy(this.gameObject, 5f);

	}
	public void endAttack()
	{
		attacking = false;
	}
}
