using UnityEngine;
using System.Collections;

public class CircularProgressBar : MonoBehaviour 
{
	public float maxValue;
	public float currentValue;
	private GameObject piece0, piece1, piece2, piece3;

	// Use this for initialization
	void Start () 
	{
		piece0 = this.transform.GetChild(0).gameObject;
		piece1 = this.transform.GetChild(1).gameObject;
		piece2 = this.transform.GetChild(2).gameObject;
		piece3 = this.transform.GetChild(3).gameObject;

	}
	
	// Update is called once per frame
	void Update () 
	{
		//currentValue += Time.deltaTime;
		//updateRotations();
	}
	public void updateRotations()
	{
		if(currentValue > maxValue)
		{
			currentValue %= maxValue;
		}
		if(currentValue/maxValue < .5f)
		{
			piece0.transform.localRotation = Quaternion.Euler(0, 0, 180f);
			piece1.transform.localPosition = new Vector3(0, 0, 0);
			piece3.transform.localRotation = Quaternion.Euler(0, 0, 0);
		}
		else
		{
			piece0.transform.localRotation = Quaternion.Euler(0, 0, 0);
			piece1.transform.localPosition = new Vector3(0, 0, -.02f);
			piece3.transform.localRotation = Quaternion.Euler(0, 0, 180f);
		}
		piece2.transform.localRotation = Quaternion.Euler(0, 0, (currentValue/maxValue)*-360f);
	}
}
