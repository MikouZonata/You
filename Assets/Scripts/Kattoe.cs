using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Kattoe : MonoBehaviour
{
	AudioClip callClip;
	AudioSource audioSource;
	NavMeshAgent navMeshAgent;

	bool attachedToMaan = false;

	float minTimeBetweenRoaming = 1, maxTimeBetweenRoaming = 3.6f;

	float callFrequencyNotAttached = 2.5f, callFrequencyAttached = 1.2f, callFrequencyDeviation = .2f;
	float _callTimer; 
	float callPitchMax = 1.5f, callPitchMin = 0.2f;
	int chanceToTempt = 40;

	public void Init (AudioClip callClip)
	{
		audioSource = GetComponent<AudioSource>();
		audioSource.pitch = Random.Range(callPitchMin, callPitchMax);

		this.callClip = callClip;
		audioSource.clip = callClip;

		_callTimer = Random.Range(-callFrequencyDeviation, callFrequencyDeviation);

		StartCoroutine(Roam());
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
				_callTimer = Random.Range(-callFrequencyDeviation, callFrequencyDeviation);
			}
		} else {
			if (_callTimer >= callFrequencyNotAttached) {
				audioSource.Play();
				_callTimer = Random.Range(-callFrequencyDeviation, callFrequencyDeviation);
			}
		}
	}

	IEnumerator Roam ()
	{
		while (true) {
			if (attachedToMaan) {
				StartCoroutine(FollowMaan());
				yield break;
			}

			yield return null;
		}
	}

	IEnumerator FollowMaan ()
	{

	}

	public bool Tempt ()
	{
		int roll = Random.Range(0, 100);
		if (roll > chanceToTempt) {
			return true;
		} else
			return false;
	}

	public void Attach (Transform targetTrans)
	{
		GetComponent<Collider>().enabled = false;
		attachedToMaan = true;
		transform.parent = targetTrans;
	}
}