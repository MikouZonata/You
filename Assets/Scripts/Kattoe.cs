using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kattoe : MonoBehaviour
{
	AudioClip callClip;
	AudioSource audioSource;

	bool attachedToMaan = false;
	float callFrequencyNotAttached = 2.5f, callFrequencyAttached = 1.2f;
	float _callTimer = 0;

	float callPitchMax = 1.5f, callPitchMin = 0.2f;

	public void Init (AudioClip callClip)
	{
		audioSource = GetComponent<AudioSource>();
		audioSource.pitch = Random.Range(callPitchMin, callPitchMax);

		this.callClip = callClip;
		audioSource.clip = callClip;
	}

	private void Update ()
	{
		Call();
	}

	void Call ()
	{
		_callTimer += Time.deltaTime;
		if (attachedToMaan) {
			if (_callTimer >= callFrequencyAttached) {
				audioSource.Play();
				_callTimer = 0;
			}
		} else {
			if (_callTimer >= callFrequencyNotAttached) {
				audioSource.Play();
				_callTimer = 0;
			}
		}
	}

	void Attach (Transform targetTrans)
	{

	}
}