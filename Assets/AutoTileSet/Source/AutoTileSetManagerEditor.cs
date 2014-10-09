using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AutoTileSetManager))]
public class AutoTileSetManagerEditor : Editor {

	System.Action TileTool;

	void OnSceneGUI() { 
		Event current = Event.current;
		int controlID = GUIUtility.GetControlID(FocusType.Passive);
		HandleUtility.AddDefaultControl(controlID);
		EventType currentEventType=current.GetTypeForControl(controlID);
		bool skip=false;
		int saveControl=GUIUtility.hotControl;
		
		if (currentEventType==EventType.Layout) {skip=true;}
		if (currentEventType==EventType.ScrollWheel) {skip=true;}
		if (current.button!=0) {skip=true;}
		
		if (current.button==1) {
			ColorPickTile();
		}
		
		if (style==null) {
			style = new GUIStyle();
			style.normal.textColor = Color.red;
			style.normal.background=new Texture2D(1,1);
			style.normal.background.SetPixel(0,0,Color.white);
			style.normal.background.Apply();
		}
		
		if (SceneView.currentDrawingSceneView.in2DMode) {
			style.normal.textColor = Color.black;
			Handles.Label(HandleUtility.GUIPointToWorldRay(Vector3.zero).origin, "LMB: Draw/Erase , RMB: Tilepicker", style);
			if (current.button==0 && !skip) {
				GUIUtility.hotControl=controlID;
				
				switch (current.type) {
				case EventType.KeyDown: if (current.keyCode==KeyCode.Escape) {Selection.activeGameObject=null; Event.current.Use(); } break;
				case EventType.MouseDown: GetTool();  break;
				case EventType.MouseDrag: TileTool(); break;
				case EventType.MouseUp:	  TileTool=null; break;
				}
			}
		} else {
			style.normal.textColor = Color.red;
			Handles.Label(HandleUtility.GUIPointToWorldRay(Vector3.zero).origin, "Tile editing requires 2D mode", style);
		}
		
		GUIUtility.hotControl=saveControl;
		if (GUI.changed) {
			EditorUtility.SetDirty(target);
		}
	}
	GUIStyle style;
	
	void GetTool() {
		RaycastHit2D hit=Physics2D.GetRayIntersection(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), Mathf.Infinity);
		if (!hit) {
			TileTool=DrawTool;
		} else {
			TileTool=EraseTool;
		}
		TileTool();
	}
	
	void ColorPickTile() {
		RaycastHit2D hit=Physics2D.GetRayIntersection(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), Mathf.Infinity);
		if (hit) {
			GameObject newPrefab=hit.collider.gameObject;
			((AutoTileSetManager)serializedObject.targetObject).currentTile=(GameObject)PrefabUtility.GetPrefabParent(newPrefab);
		}
	}

	void DrawTool() {
		RaycastHit2D hit=Physics2D.GetRayIntersection(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), Mathf.Infinity);
		if (!hit) {
			GameObject newObject=(GameObject)serializedObject.FindProperty("currentTile").objectReferenceValue;
			try {
				newObject=(GameObject)PrefabUtility.InstantiatePrefab(newObject);
				newObject.transform.position=HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
				newObject.transform.rotation=Quaternion.identity;
				newObject.transform.position=new Vector3(newObject.transform.position.x, newObject.transform.position.y, 0);
				newObject.transform.parent=((Component)serializedObject.targetObject).gameObject.transform;
				Undo.RegisterCreatedObjectUndo(newObject, "Created new prefab tile");
			} catch {
				newObject=(GameObject)Instantiate(newObject, HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin, Quaternion.identity);
				newObject.transform.position=new Vector3(newObject.transform.position.x, newObject.transform.position.y, 0);
				newObject.transform.parent=((Component)serializedObject.targetObject).gameObject.transform;
				newObject.name=newObject.name.Replace("(Clone)", "");
				Undo.RegisterCreatedObjectUndo(newObject, "Created new tile");
			}
		}
	}
	
	void EraseTool() {
		RaycastHit2D hit=Physics2D.GetRayIntersection(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), Mathf.Infinity);
		if (hit) {
			DestroyImmediate(hit.collider.gameObject);
		}
	}
	
	Tool savedTool;
	void OnEnable() {
		savedTool=Tools.current;
		Tools.current=Tool.None;
	}
	
	void OnDisable() {
		Tools.current=savedTool;
	}
}
