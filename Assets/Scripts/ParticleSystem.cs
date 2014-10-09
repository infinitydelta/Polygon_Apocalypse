using UnityEngine;
using System.Collections;

public class ParticleSystem : MonoBehaviour {
	float timer = 0;
	// Use this for initialization
	void Start () {
		Destroy(gameObject, 1f);
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if (timer > .07f) {
			light.enabled = false;
			//Behaviour h = (Behaviour)GetComponent("Halo");
			//h.enabled = false;
		}
	}
}
