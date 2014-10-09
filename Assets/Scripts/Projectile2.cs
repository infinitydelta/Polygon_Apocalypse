using UnityEngine;
using System.Collections;

public class Projectile2 : MonoBehaviour
{
    public float speed = 100;
    public GameObject boom;
    public GameObject blood;

    Vector2 vel;
    bool moving = true;
    RaycastHit2D hit;
    Vector3 fuck = new Vector3(0, 0, 0);
    float damage;

    // Use this for initialization
    void Start()
    {
        vel = new Vector2(-Mathf.Sin(transform.rotation.eulerAngles.z * Mathf.Deg2Rad) * speed, speed * Mathf.Cos(transform.rotation.eulerAngles.z * Mathf.Deg2Rad));
        hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.up, 50f, 1);
        //rigidbody2D.AddForce(vel * 300);
        //rigidbody2D.velocity = vel * 250;
        Destroy(this.gameObject, 1);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(vel.x * Time.deltaTime, vel.y * Time.deltaTime, 0);
        /*if (moving) {
			
            rigidbody2D.velocity = vel * Time.deltaTime * 900;
        } */
    }

    void FixedUpdate()
    {


        //transform.position = Vector3.MoveTowards(transform.position, fuck, speed * Time.fixedDeltaTime);
        //transform.Translate(Vector2.up * Time.fixedDeltaTime * 20) ;
        //rigidbody2D.velocity = vel;
    }

    void OnCollisionEnter2D(Collision2D other)
    {

        //moving = false;
        float deathtime = 0.25f;
        //if wall
        if (other.gameObject.CompareTag("wall"))
        {
            Instantiate(boom, other.contacts[0].point, other.transform.rotation);
            Vector3 bounce = Vector3.Reflect(new Vector3(vel.x, vel.y, 0), new Vector3(other.contacts[0].normal.x, other.contacts[0].normal.y, 0));
            //vel = new Vector2(bounce.x, bounce.y);
            vel = Vector2.zero;
            //rigidbody2D.velocity = vel;
            //deathtime = Random.Range(-.5f, .1f);
            if (deathtime < 0.02f) deathtime = 0.02f;
        }
        else if (other.gameObject.CompareTag("enemy"))
        {
            other.gameObject.GetComponent<zombie>().damage(damage);
            GameObject bloodz = (GameObject)Instantiate(blood, other.contacts[0].point, Quaternion.identity);
            float angle = Mathf.Rad2Deg * Mathf.Atan2(vel.y, vel.x);
            bloodz.transform.rotation = Quaternion.Euler(angle, 90, 0);
            //other.gameObject.rigidbody2D.velocity = rigidbody2D.velocity * .5f;
        }

        //deathtime = 0;
        //moving = false;
        //rigidbody2D.velocity = Vector2.zero;

        Destroy(this.gameObject, deathtime);
    }
    public void setDamage(float dmg)
    {
        damage = dmg;
    }
}
