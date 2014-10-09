using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AutoTile : MonoBehaviour {
	[Header("AutoTile options")]
	public bool  onlyAtStart=true;
	public bool  slopeCorners=false;
	public bool  fitToGrid=true;
	public float tileSize=1;
	public LayerMask autoTileLayer = -1;
	
	[HideInInspector]public float sx=0, sy=5;
	protected CornerType cornerType=CornerType.None;
	protected Collider2D[] colliders;
	protected bool[] gates;
	protected Vector3 checkPosition;
	protected List<GameObject> contacts;
	protected bool dirty=false;
	protected bool checkedSlopeCorners=false;
	
	protected virtual void UpdateDisplay() {}
	
	protected void Awake() {
		if (colliders==null || colliders.Length<2) {
			colliders=GetComponents<Collider2D>();
		}
		contacts=new List<GameObject>();
		checkPosition=transform.position;
		gates=new bool[8];
		for (int i = 0; i < 8; i++) {
			gates[i]=true;
		}
		UpdateTile();
	}
	
	public void OnDestroy() {
		collider2D.enabled=false;
		UpdateTile();
	}
	
	protected void Update() {
		if (colliders==null) {
			colliders=GetComponents<Collider2D>();
		}
		if (!onlyAtStart) {
			if (fitToGrid) {
				transform.position=new Vector3(Mathf.Round(transform.position.x/tileSize)*tileSize, Mathf.Round(transform.position.y/tileSize)*tileSize, transform.position.z);
			}
			if (slopeCorners!=checkedSlopeCorners) {
				dirty=true;
				checkedSlopeCorners=slopeCorners;
			}
			if ((transform.position-checkPosition).magnitude!=0 || dirty) {
				checkPosition=transform.position;
				UpdateTile();
			}
		}
	}
	
	public void UpdateTile() {
		UpdateNeighbours();
		GetNeighbours(); 
		UpdateNeighbours();
		SelectTile();
		UpdateDisplay();
		Resources.UnloadUnusedAssets();
		dirty=false;
	}
	
	public void UpdateNeighbours() {
		foreach (GameObject contact in contacts) {
			if (contact!=null) {
				contact.SendMessage("UpdateBounds", SendMessageOptions.DontRequireReceiver);
			}
		}
	}
	
	public void UpdateBounds() {
		GetNeighbours();
		SelectTile();
	}
	
	//You can replace "GetNeighbours" with the method you want to use to find tile neirghbours
	public void GetNeighbours() {
		if (gates==null) {
			gates=new bool[8];
		}
		float x=transform.position.x;
		float y=transform.position.y;
		int i, j;
		Vector2 size=(Vector2)transform.lossyScale;
		GameObject target;
		List<GameObject> newContacts=new List<GameObject>();
		i=0;  j=1 ; try {target=Physics2D.OverlapCircle(new Vector2(x+i*size.x,y+j*size.y), 0.1f, autoTileLayer.value).gameObject; gates[0]=target!=null; if (gates[0]) {newContacts.Add(target);};} catch {gates[0]=false;}
		i=1;  j=0 ; try {target=Physics2D.OverlapCircle(new Vector2(x+i*size.x,y+j*size.y), 0.1f, autoTileLayer.value).gameObject; gates[1]=target!=null; if (gates[1]) {newContacts.Add(target);};} catch {gates[1]=false;}
		i=0;  j=-1; try {target=Physics2D.OverlapCircle(new Vector2(x+i*size.x,y+j*size.y), 0.1f, autoTileLayer.value).gameObject; gates[2]=target!=null; if (gates[2]) {newContacts.Add(target);};} catch {gates[2]=false;}
		i=-1; j=0 ; try {target=Physics2D.OverlapCircle(new Vector2(x+i*size.x,y+j*size.y), 0.1f, autoTileLayer.value).gameObject; gates[3]=target!=null; if (gates[3]) {newContacts.Add(target);};} catch {gates[3]=false;}
		i=-1; j=1 ; try {target=Physics2D.OverlapCircle(new Vector2(x+i*size.x,y+j*size.y), 0.1f, autoTileLayer.value).gameObject; gates[4]=target!=null; if (gates[4]) {newContacts.Add(target);};} catch {gates[4]=false;}
		i=1;  j=1 ; try {target=Physics2D.OverlapCircle(new Vector2(x+i*size.x,y+j*size.y), 0.1f, autoTileLayer.value).gameObject; gates[5]=target!=null; if (gates[5]) {newContacts.Add(target);};} catch {gates[5]=false;}
		i=1;  j=-1; try {target=Physics2D.OverlapCircle(new Vector2(x+i*size.x,y+j*size.y), 0.1f, autoTileLayer.value).gameObject; gates[6]=target!=null; if (gates[6]) {newContacts.Add(target);};} catch {gates[6]=false;}
		i=-1; j=-1; try {target=Physics2D.OverlapCircle(new Vector2(x+i*size.x,y+j*size.y), 0.1f, autoTileLayer.value).gameObject; gates[7]=target!=null; if (gates[7]) {newContacts.Add(target);};} catch {gates[7]=false;}
		
		contacts=newContacts;
	}
	
	void SelectTile() {
		sx=6; sy=2;
		
		if (Gates(3)){sx=2;sy=2; cornerType=CornerType.None;}
		if (Gates(0)){sx=3;sy=2; cornerType=CornerType.None;}
		if (Gates(1)){sx=4;sy=2; cornerType=CornerType.None;}
		if (Gates(2)){sx=5;sy=2; cornerType=CornerType.None;}
		
		if (Gates(1,3)){sx=0;sy=2; cornerType=CornerType.None;}
		if (Gates(0,2)){sx=1;sy=2; cornerType=CornerType.None;}
		
		if (Gates(0,3)){sx=7;sy=1;   cornerType=CornerType.Forward ;}
		if (Gates(1,0)){sx=0;sy=0;   cornerType=CornerType.Backward;}
		if (Gates(2,1)){sx=1;sy=0;   cornerType=CornerType.Forward ;}
		if (Gates(3,2)){sx=2;sy=0;   cornerType=CornerType.Backward;}
		if (Gates(2,3,7)){sx=4;sy=3; cornerType=CornerType.Backward;}
		if (Gates(3,0,4)){sx=5;sy=3; cornerType=CornerType.Forward ;}
		if (Gates(0,1,5)){sx=6;sy=3; cornerType=CornerType.Backward;}
		if (Gates(1,2,6)){sx=7;sy=3; cornerType=CornerType.Forward ;}
		
		if (Gates(0,1,3)){sx=3;sy=0; cornerType=CornerType.None;}
		if (Gates(1,2,0)){sx=4;sy=0; cornerType=CornerType.None;}
		if (Gates(2,3,1)){sx=5;sy=0; cornerType=CornerType.None;}
		if (Gates(3,0,2)){sx=6;sy=0; cornerType=CornerType.None;}
		
		if (Gates(0,1,2,3)){sx=7;sy=4; cornerType=CornerType.None;}
		if (Gates(0,2,3,7)){sx=7;sy=2; cornerType=CornerType.None;}
		if (Gates(1,3,0,4)){sx=0;sy=1; cornerType=CornerType.None;}
		if (Gates(2,0,1,5)){sx=1;sy=1; cornerType=CornerType.None;}
		if (Gates(3,1,2,6)){sx=2;sy=1; cornerType=CornerType.None;}
		if (Gates(0,1,3,5)){sx=3;sy=1; cornerType=CornerType.None;}
		if (Gates(1,2,0,6)){sx=4;sy=1; cornerType=CornerType.None;}
		if (Gates(2,3,1,7)){sx=5;sy=1; cornerType=CornerType.None;}
		if (Gates(3,0,2,4)){sx=6;sy=1; cornerType=CornerType.None;}
		
		if (Gates(1,2,3,6,7)){sx=0;sy=3; cornerType=CornerType.None;}
		if (Gates(0,2,3,4,7)){sx=1;sy=3; cornerType=CornerType.None;}
		if (Gates(0,1,3,4,5)){sx=2;sy=3; cornerType=CornerType.None;}
		if (Gates(0,1,2,5,6)){sx=3;sy=3; cornerType=CornerType.None;}
		
		if (Gates(0,1,2,3,7)){sx=3;sy=4; cornerType=CornerType.None;}
		if (Gates(0,1,2,3,4)){sx=4;sy=4; cornerType=CornerType.None;}
		if (Gates(0,1,2,3,5)){sx=5;sy=4; cornerType=CornerType.None;}
		if (Gates(0,1,2,3,6)){sx=6;sy=4; cornerType=CornerType.None;}
		
		if (Gates(0,1,2,3,5,7)){sx=1;sy=4; cornerType=CornerType.None;}
		if (Gates(0,1,2,3,4,6)){sx=2;sy=4; cornerType=CornerType.None;}
		
		if (Gates(0,1,2,3,6,7)){sx=5;sy=5; cornerType=CornerType.None;}
		if (Gates(0,1,2,3,4,7)){sx=6;sy=5; cornerType=CornerType.None;}
		if (Gates(0,1,2,3,4,5)){sx=7;sy=5; cornerType=CornerType.None;}
		if (Gates(0,1,2,3,5,6)){sx=0;sy=4; cornerType=CornerType.None;}
		
		if (Gates(0,1,2,3,5,6,7)){sx=1;sy=5; cornerType=CornerType.None;}
		if (Gates(0,1,2,3,4,6,7)){sx=2;sy=5; cornerType=CornerType.None;}
		if (Gates(0,1,2,3,4,5,7)){sx=3;sy=5; cornerType=CornerType.None;}
		if (Gates(0,1,2,3,4,5,6)){sx=4;sy=5; cornerType=CornerType.None;}
		if (Gates(0,1,2,3,4,5,6,7)){sx=0;sy=5; cornerType=CornerType.None;}
		
		UpdateColliders();
		
		UpdateDisplay();
	}
	
	public void UpdateColliders() {
		if (colliders==null || colliders.Length<2) {
			colliders=GetComponents<Collider2D>();
		}
		if (slopeCorners) {
			foreach (var item in colliders) {
				item.enabled=false;
			}
			switch (cornerType) {
			case CornerType.Forward:  colliders[2].enabled=true; break;
			case CornerType.Backward: colliders[1].enabled=true; break;
			case CornerType.None:     colliders[0].enabled=true; break;
			}
		} else {
			foreach (var item in colliders) {
				item.enabled=false;
			}
			colliders[0].enabled=true;
		}
	}
	
	bool Gates(params int[] nums) {
		if (nums==null) {return false;}
		for (int i = 0; i < nums.Length; i++) {
			if (!gates[nums[i]]) {return false;}
		}
		return true;
	}
	
	void SetDirty() {
		dirty=true;
	}
	
	//You can use this method if your map info is stored into an int[,] array
	//	public void GetNeighboursMatrix(int[,] matrix, int x, int y) {
	//		int i, j;
	//		try {i=0;  j=1 ; gates[0]=(matrix[x+i,y+j]!=0);} catch {gates[0]=false;}
	//		try {i=1;  j=0 ; gates[1]=(matrix[x+i,y+j]!=0);} catch {gates[1]=false;}
	//		try {i=0;  j=-1; gates[2]=(matrix[x+i,y+j]!=0);} catch {gates[2]=false;}
	//		try {i=-1; j=0 ; gates[3]=(matrix[x+i,y+j]!=0);} catch {gates[3]=false;}
	//		try {i=-1; j=1 ; gates[4]=(matrix[x+i,y+j]!=0);} catch {gates[4]=false;}
	//		try {i=1;  j=1 ; gates[5]=(matrix[x+i,y+j]!=0);} catch {gates[5]=false;}
	//		try {i=1;  j=-1; gates[6]=(matrix[x+i,y+j]!=0);} catch {gates[6]=false;}
	//		try {i=-1; j=-1; gates[7]=(matrix[x+i,y+j]!=0);} catch {gates[7]=false;}
	//	}
}

public enum CornerType {None, Forward, Backward}
