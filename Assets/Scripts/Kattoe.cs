using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Kattoe : MonoBehaviour
{
	MaanManager manager;
	AudioSource audioSource;
	NavMeshAgent navMeshAgent;
	Transform maanTrans;
	Transform parentPiece;

	public enum BehaviourStates { Roaming, Spotted, Near, Bonded };
	BehaviourStates behaviourState = BehaviourStates.Roaming;

	bool roamingDestinationSet = false;
	float _roamingTimer = 0, roamingTime = 0;
	float roamingMinTime = 1, roamingmaxTime = 3.6f;
	float distanceBeforeSpotted = 16;

	bool spottedSetup = false;
	Vector3 spottedMaanPosition;



	float runAwaySpeed = 15, runAwayTime = 2.4f;

	float callFrequencyNotAttached = 2.5f, callFrequencyAttached = 1.2f, callFrequencyDeviation = .3f;
	float _callTimer; 
	float callPitchMax = 1.5f, callPitchMin = 0.2f;

	Vector3 basePosition;
	float maxRoamingDistance = 5;

	public void Init (MaanManager manager, AudioClip callClip, Transform maanTrans, Transform parentPiece)
	{
		this.manager = manager;
		this.maanTrans = maanTrans;

		audioSource = GetComponent<AudioSource>();
		audioSource.pitch = Random.Range(callPitchMin, callPitchMax);
		
		audioSource.clip = callClip;
		navMeshAgent = GetComponent<NavMeshAgent>();

		_callTimer = Random.Range(-callFrequencyDeviation, callFrequencyDeviation);
		basePosition = transform.position;

		StartCoroutine(Roam());
	}

	private void Update ()
	{
		Behaviour();
	}

	void Behaviour ()
	{
		switch (behaviourState) {
			case BehaviourStates.Roaming:
				if (!roamingDestinationSet) {
					Vector2 randomPoint = Random.insideUnitCircle * maxRoamingDistance;
					Vector3 targetPosition = basePosition + new Vector3(randomPoint.x, 0, randomPoint.y);
					navMeshAgent.destination = targetPosition;
					roamingTime = Random.Range(roamingMinTime, roamingmaxTime);
					_roamingTimer = 0;
					roamingDestinationSet = true;
				}

				_roamingTimer += Time.deltaTime;
				if (_roamingTimer > roamingTime) {
					roamingDestinationSet = false;
				}

				if ((maanTrans.position - transform.position).sqrMagnitude < distanceBeforeSpotted * distanceBeforeSpotted) {
					behaviourState = BehaviourStates.Spotted;
					goto case BehaviourStates.Spotted;
				}
				break;
			case BehaviourStates.Spotted:
				if (!spottedSetup) {
					spottedMaanPosition = maanTrans.position;
				}
				if ((spottedMaanPosition - maanTrans.position).sqrMagnitude > 4) {
					StartCoroutine(RunAway());
				}


				break;
			case BehaviourStates.Near:
				break;
			case BehaviourStates.Bonded:
				break;
		}
	}

	IEnumerator RunAway ()
	{
		Vector3 direction = transform.position - maanTrans.position;

		for (float t = 0; t < runAwayTime; t += Time.deltaTime) {
			transform.position += direction * runAwaySpeed * Time.deltaTime;
			yield return null;
		}

		manager.KattoeRanAway(this, parentPiece);
		Destroy(gameObject);
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
		float waitTime = Random.Range(roamingMinTime, roamingmaxTime);
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
}