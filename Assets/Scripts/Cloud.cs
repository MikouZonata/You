using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
	public enum MaterialOptions { Good, Bad };
	public Material goodMaterial, badMaterial;

	new Renderer renderer;

	private void Awake ()
	{
		renderer = GetComponent<Renderer>();
	}

	public void SwitchMaterial (MaterialOptions option)
	{
		if (option == MaterialOptions.Bad) {
			renderer.material = badMaterial;
		} else {
			renderer.material = goodMaterial;
		}
	}
}