using UnityEngine;
using System.Collections;

public class DestroyAfterTime : MonoBehaviour 
{
	public float time = 1f;
	// Use this for initialization
	void Start () 
	{
		Destroy(this.gameObject, time);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
