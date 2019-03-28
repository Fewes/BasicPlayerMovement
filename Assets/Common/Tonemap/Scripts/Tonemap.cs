using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OverCloud.ExampleContent
{
	[ExecuteInEditMode]
	#if UNITY_5_4_OR_NEWER
		[ImageEffectAllowedInSceneView]
	#endif
	public class Tonemap : MonoBehaviour
	{
		[SerializeField]
		Shader		shader;

		[SerializeField]
		float		exposure = 1;

		[SerializeField]
		float		gamma = 2;

		Material	_material;
		Material	material
		{
			get
			{
				if (!_material)
					_material = new Material(shader);
				return _material;
			}
		}

		protected void OnRenderImage (RenderTexture source, RenderTexture destination)
		{	
			if (!material)
			{
				Graphics.Blit(source, destination);
				return;
			}
			else
			{
				material.SetFloat("_Exposure", exposure);
				material.SetFloat("_Gamma", gamma);
				Graphics.Blit(source, destination, material);
			}
		}
	}
}