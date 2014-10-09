using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AmmoCounter : MonoBehaviour 
{
	public int shotsPerClip;
	public float reloadTime;
	public GameObject radialbar;
	private CircularProgressBar cpb;
	private bool reloading;
	private Stack<GameObject> bars;
	private Weapon weapon;
	// Use this for initialization
	void Start () 
	{
		cpb = transform.GetChild(0).gameObject.GetComponent<CircularProgressBar>();
		bars = new Stack<GameObject>();
		cpb.maxValue = shotsPerClip;
		cpb.currentValue = shotsPerClip;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(reloading)
		{
			cpb.currentValue += Time.deltaTime;
			if(cpb.currentValue >= cpb.maxValue)
			{
				cpb.maxValue = shotsPerClip;
				cpb.currentValue = shotsPerClip;
				reloading = false;
				weapon.reload ();
			}
			cpb.updateRotations();
			return;
		}
	
	}
	
	public void reload()
	{
		if(!reloading)
		{
			reloading = true;
			weapon.reloading = true;
			cpb.currentValue = 0;
			cpb.maxValue = reloadTime;
		}

	}
	public void newWeapon(Weapon wep)
	{
		weapon = wep;
		while(bars.Count!=0)
		{
			Destroy (bars.Pop().gameObject);
		}

		shotsPerClip = wep.magSize;
		reloadTime = weapon.reloadTime;

		for(int x = 0; x < shotsPerClip; x++)
		{
			GameObject bar = (GameObject)Instantiate(radialbar, this.transform.position, Quaternion.Euler(this.transform.rotation.eulerAngles.x, this.transform.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.z + ((float)(x)*360f/shotsPerClip)));
			bar.transform.localScale = this.transform.localScale;
			bar.transform.parent = this.transform;
			bars.Push(bar);
		}

		cpb.maxValue = shotsPerClip;
		cpb.currentValue = wep.currentMag;
		cpb.updateRotations();
		reloading = false;
		weapon.reloading = false;
		if(cpb.currentValue <= 0)
		{
			reload();
		}
	}
	public void fire()
	{
		if(!reloading)
		{
			cpb.currentValue--;
			cpb.updateRotations();
			if(cpb.currentValue<=0)
			{
				reload();
			}
		}
	}
}
