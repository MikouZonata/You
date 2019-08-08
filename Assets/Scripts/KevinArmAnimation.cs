using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KevinArmAnimation : MonoBehaviour
{
	Kevin kevin;
	Vector3 basePos;
	float _sinTimer = 0;
	float _speed, _amplitude;
	const float minSpeed = .2f, maxSpeed = 1.8f;
	const float minAmplitude = .004f, maxAmplitude = .035f;

	private void Awake ()
	{
		kevin = GetComponentInParent<Kevin>();
		basePos = transform.localPosition;
	}

	void Update ()
	{
		_speed = minSpeed + kevin.GetFatigue() * (maxSpeed - minSpeed);
		_sinTimer += Mathf.PI * 2 * Time.deltaTime * _speed;
		_amplitude = minAmplitude + kevin.GetFatigue() * (maxAmplitude - minAmplitude);

		transform.localPosition = basePos + new Vector3(0, _amplitude * Mathf.Sin(_sinTimer), 0);
	}
}