using UnityEngine;
using System.Collections;

public class AutoTileSetManager : MonoBehaviour {
	[Header("Grid Options")]
	public Vector2 gridSize=Vector2.one;
	public Vector3 offset;
	public bool displayGrid=true;
	[Header("Draw Options")]
	public GameObject currentTile;

	void OnDrawGizmos() {
		if (displayGrid) { 
			Gizmos.color=Color.yellow*0.5f;
			DrawGrid();
		}
	}
	
	void OnDrawGizmosSelected() {
		if (displayGrid) { 
			Gizmos.color=Color.yellow;
			DrawGrid();
		}
	}
	
	void DrawGrid() {
		Vector3 pos = Camera.current.transform.position-Vector3.one;
		
		for (float y = pos.y - 800.0f; y < pos.y + 800.0f; y+= gridSize.y) {
			Gizmos.DrawLine(new Vector3(-1000000.0f, Mathf.Floor(y/gridSize.y) * gridSize.y+offset.y, offset.z),
			                new Vector3(1000000.0f,  Mathf.Floor(y/gridSize.y) * gridSize.y+offset.y, offset.z));
		}
		
		for (float x = pos.x - 1200.0f; x < pos.x + 1200.0f; x+= gridSize.x) {
			Gizmos.DrawLine (new Vector3 (Mathf.Floor (x / gridSize.x) * gridSize.x + offset.x, -1000000.0f, offset.z),
			                 new Vector3 (Mathf.Floor (x / gridSize.x) * gridSize.x + offset.x, 1000000.0f, offset.z));
				}
	}
}
