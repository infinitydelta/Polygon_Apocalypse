using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class AutoTileSetQuad : AutoTile {
	[Header("Tileset textures")]
	public Texture2D tilesetNormal;
	public Texture2D tilesetSlopes;
	public Texture2D tilesetBump;
	Material tempMaterial;	
	
	override protected void UpdateDisplay() {
		if (tempMaterial==null) {
			tempMaterial = new Material(renderer.sharedMaterial);
		}
		tempMaterial.mainTexture=renderer.sharedMaterial.mainTexture;
		tempMaterial.mainTextureScale=new Vector2(1f/8f,1f/6f);
		tempMaterial.mainTextureOffset=new Vector2(1f/8f*sx,1f/6f*sy);
		tempMaterial.shader=renderer.sharedMaterial.shader;
		if (!slopeCorners) {
			if (tilesetNormal!=null) {
				tempMaterial.mainTexture=tilesetNormal;
			}
		} else {
			if (tilesetSlopes!=null) {
				tempMaterial.mainTexture=tilesetSlopes;
			}
		}
		tempMaterial.mainTexture=renderer.sharedMaterial.mainTexture;
		
		if (tilesetBump!=null) {
			tempMaterial.SetTexture("_BumpMap", tilesetBump);
		}
		tempMaterial.SetTextureScale ("_BumpMap", new Vector2(1f/8f,1f/6f));
		tempMaterial.SetTextureOffset("_BumpMap", new Vector2(1f/8f*sx,1f/6f*sy));
		
		tempMaterial.shader=renderer.sharedMaterial.shader;
		renderer.sharedMaterial = tempMaterial;
	}
	 
}
