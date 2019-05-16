using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
	[HideInInspector]
	public bool destroyed = false;

	[HideInInspector]
	public bool maanInRange = false;
	Material mat;
	const float maxhealth = 100;
	float _health = maxhealth;
	float damagePerSecondUnlinked = 5, damagePerSecondLinked = 150;
	float regenerationPerSecond = 200;

	public Color baseColor, destroyedColor;

	float restorationTime = 69;

	private void Awake ()
	{
		mat = GetComponent<Renderer>().material;
	}

	private void Update ()
	{
		if (!destroyed) {
			if (!maanInRange) {
				_health = Mathf.MoveTowards(_health, maxhealth, regenerationPerSecond * Time.deltaTime);
				mat.color = Color.Lerp(destroyedColor, baseColor, _health / maxhealth);
			}
		}
	}

	public void Disintegrate (bool linked = false)
	{
		if (!destroyed) {
			if (linked) {
				_health = Mathf.MoveTowards(_health, 0, damagePerSecondLinked * Time.deltaTime);
			} else {
				_health = Mathf.MoveTowards(_health, 0, damagePerSecondUnlinked * Time.deltaTime);
			}

			mat.color = Color.Lerp(destroyedColor, baseColor, _health / maxhealth);

			if (_health <= 0) {
				destroyed = true;
				GetComponent<Collider>().enabled = false;
				GetComponent<Renderer>().enabled = false;
				StartCoroutine(Restore());
			}
		}
	}

	IEnumerator Restore ()
	{
		yield return new WaitForSeconds(restorationTime);
		destroyed = false;
		GetComponent<Collider>().enabled = true;
		GetComponent<Renderer>().enabled = true;
		_health = maxhealth;
		mat.color = baseColor;
	}
}