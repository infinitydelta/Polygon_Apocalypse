using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour 
{	
	void Update()
	{
		SetBar( Time.time/10f );
	}
	
	// use SetBar anywhere between 0 and 1
	public void SetBar( float v )
	{
		float offset = 0f;
		offset = Mathf.Clamp01( v/2f );
		renderer.material.mainTextureOffset = new Vector2(offset, 0);
	}
}
