using UnityEngine;
using System.Collections;

public class PlayerHPBar : MonoBehaviour 
{
	Transform redbar;
	Transform yellowbar;
	public float max;
	public float current;

	// Use this for initialization
	void Start () 
	{
		redbar = transform.FindChild("redbar");
		yellowbar = transform.FindChild("yellowbar");
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(yellowbar.localScale.x > redbar.localScale.x)
		{
			yellowbar.localScale = new Vector3(yellowbar.localScale.x - Time.deltaTime/2, yellowbar.localScale.y, yellowbar.localScale.z);
		}
		else if (yellowbar.localScale.x < redbar.localScale.x)
		{
			yellowbar.localScale = new Vector3(redbar.localScale.x, 1, 1);
		}
		//redbar.localScale = new Vector3(current/max, 1, 1);
	}
	public void setHP(float newvalue)
	{
		current = newvalue;
		redbar.localScale = new Vector3(current/max, 1, 1);
	}
}
