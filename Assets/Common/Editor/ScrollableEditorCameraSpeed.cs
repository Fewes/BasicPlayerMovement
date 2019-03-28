using UnityEngine;
using UnityEditor;
using System.Reflection;

[InitializeOnLoad]
public static class ScrollableEditorCameraSpeed
{
	static ScrollableEditorCameraSpeed()
	{
		float cameraSpeed = 10.0f;
		const float cameraSpeedMin = 0.02f;
		const float cameraSpeedMax = 10000f;

		bool key_w = false;
		bool key_a = false;
		bool key_s = false;
		bool key_d = false;

		SceneView.onSceneGUIDelegate += view =>
		{
			Event e = Event.current;
			if (e != null)
			{
				// Seems silly but Input.GetKey() does not work in here so we keep our own states
				if (e.type == EventType.KeyDown)
				{
					if (e.keyCode == KeyCode.W) key_w = true;
					if (e.keyCode == KeyCode.A) key_a = true;
					if (e.keyCode == KeyCode.S) key_s = true;
					if (e.keyCode == KeyCode.D) key_d = true;
				}
				else if (e.type == EventType.KeyUp)
				{
					if (e.keyCode == KeyCode.W) key_w = false;
					if (e.keyCode == KeyCode.A) key_a = false;
					if (e.keyCode == KeyCode.S) key_s = false;
					if (e.keyCode == KeyCode.D) key_d = false;
				}

				bool moving = key_w || key_a || key_s || key_d;

				if (moving && e.type == EventType.ScrollWheel)
				{
					// Increase camera speed
					if (e.delta.y < 0)
					{
						cameraSpeed *= 1.5f;
						cameraSpeed = Mathf.Clamp(cameraSpeed, cameraSpeedMin, cameraSpeedMax);
						e.Use();

						// Camera hack to disable/counteract scroll wheel zoom
						//if (Camera.current != null)
						//{
						//	GameObject target = new GameObject();
						//	target.transform.position = Camera.current.transform.position - Camera.current.transform.forward * 0.45f;
						//	target.transform.rotation = Camera.current.transform.rotation;
						//	view.AlignViewToObject(target.transform);
						//	GameObject.DestroyImmediate(target);
						//}
					}
					// Decrease camera speed
					else
					{
						cameraSpeed *= 0.75f;
						cameraSpeed = Mathf.Clamp(cameraSpeed, cameraSpeedMin, cameraSpeedMax);
						e.Use();

						// Camera hack to disable/counteract scroll wheel zoom
						//if (Camera.current != null)
						//{
						//	GameObject target = new GameObject();
						//	target.transform.position = Camera.current.transform.position + Camera.current.transform.forward * 0.45f;
						//	target.transform.rotation = Camera.current.transform.rotation;
						//	view.AlignViewToObject(target.transform);
						//	GameObject.DestroyImmediate(target);
						//}
					}
				}
			}

			// -------------------------------------------------------------
			// Original code below from UnityEditorCameraSpeed.cs by Kalineh
			// All credit goes to him
			// https://github.com/kalineh
			// -------------------------------------------------------------

			if (Event.current.type != EventType.Layout)
				return;

			var tools_type = typeof(UnityEditor.Tools);
			var locked_view_tool_field = (FieldInfo)tools_type.GetField("s_LockedViewTool", BindingFlags.NonPublic | BindingFlags.Static);
			var locked_view_tool = (ViewTool)locked_view_tool_field.GetValue(null);

			if (locked_view_tool != ViewTool.FPS)
				return;

			var scene_view_assembly = Assembly.GetAssembly(typeof(UnityEditor.SceneView));
			var scene_view_motion_type = scene_view_assembly.GetType("UnityEditor.SceneViewMotion");

			var flyspeed_field = (FieldInfo)scene_view_motion_type.GetField("s_FlySpeed", BindingFlags.NonPublic | BindingFlags.Static);
			var flyspeed = flyspeed_field.GetValue(null);
			var flyspeed_modified = (float)flyspeed;

			flyspeed_modified = cameraSpeed;

			flyspeed_field.SetValue(null, flyspeed_modified);
		};
	}
}