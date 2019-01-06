using System;
using UnityEngine;

namespace DefaultNamespace
{    
	[Obsolete("Got replaced by TexturedMonoBehaviour")]
	public class Ceiling : MonoBehaviour {       
		// Use this for initialization
		private void Start()
		{    
			//LoadTexture("STARS");
		}

		// Update is called once per frame
		private void Update() { }
        
		/// <summary>
		/// 
		/// </summary>
		public void LoadTexture(string materialName)
		{
			var material = TexturingUtility.LoadMaterialByName(materialName);
			
			Renderer rend = GetComponent<Renderer>();
			if (rend != null && material != null)
			{
				rend.material = material;
			}
		}
	}
}