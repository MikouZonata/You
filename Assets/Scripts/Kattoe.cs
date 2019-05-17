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

	float callFrequencyNotAttached = 2.5f, callFrequencyAttached = 1.2f, callFrequencyDeviation = .3f;
	float _callTimer; 
	float callPitchMax = 1.5f, callPitchMin = 0.2f;

	float minTimeBetweenRoaming = 1, maxTimeBetweenRoaming = 3.6f;
	Vector3 basePosition;
	float maxRoamingDistance = 7;

	int chanceToTempt = 40;

	public void Init (AudioClip callClip)
	{
		audioSource = GetComponent<AudioSource>();
		audioSource.pitch = Random.Range(callPitchMin, callPitchMax);

		this.callClip = callClip;
		audioSource.clip = callClip;
		navMeshAgent = GetComponent<NavMeshAgent>();

		_callTimer = Random.Range(-callFrequencyDeviation, callFrequencyDeviation);
		basePosition = transform.position;

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
		float waitTime = Random.Range(minTimeBetweenRoaming, maxTimeBetweenRoaming);
		float _timer = 0;
		while (true) {
			if (attachedToMaan) {
				StartCoroutine(FollowMaan());
				yield break;
			}
			_timer += Time.deltaTime;
			if (_timer >= waitTime) {
				break;
			}
			yield return null;
		}

		Vector2 randomPoint = Random.insideUnitCircle * maxRoamingDistance;
		Vector3 targetPosition = basePosition + new Vector3(randomPoint.x, 0, randomPoint.y);
		navMeshAgent.destination = targetPosition;

		while ((transform.position - targetPosition).sqrMagnitude > .5f) {
			if (attachedToMaan) {
				StartCoroutine(FollowMaan());
				yield break;
			}
			yield return null;
		}

		if (attachedToMaan) {
			StartCoroutine(FollowMaan());
		} else {
			StartCoroutine(Roam());
		}
	}

	IEnumerator FollowMaan ()
	{
		navMeshAgent.enabled = false;
		yield return null;
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