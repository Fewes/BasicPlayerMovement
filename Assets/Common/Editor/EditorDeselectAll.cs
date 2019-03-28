 using UnityEditor;
 using UnityEngine;
 
[InitializeOnLoad]
public static class EditorDeselectAll
{
	static EditorDeselectAll()
	{
		SceneView.onSceneGUIDelegate += view =>
		{
			Event e = Event.current;
			if (e != null && e.keyCode != KeyCode.None)
			{
				// Deselect All
				if (e.keyCode == KeyCode.Escape)
					Selection.activeGameObject = null;
			}
		};
	}
}