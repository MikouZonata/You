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
	const float roamingMinTime = 1, roamingmaxTime = 3.6f;
	const float distanceBeforeSpotted = 16;

	bool spottedSetup = false, spottedAdvancing = false;
	Vector3 spottedMaanPosition, spottedTargetPosition;
	float _spottedAdvanceTimer = 0, spottedAdvanceTime;
	const float spottedMinAdvanceTime = .8f, spottedMaxAdvanceTime = 3;
	const float spottedMinAdvanceDistance = 2, spottedMaxAdvanceDistance = 6;
	const float spottedAdvanceSpeed = 3;
	const float spottedTargetDistance = 3;
	float _spottedTimesLured = 0, spottedLureLimit;
	const float spottedMinLureLimit = 5, spottedMaxLureLimit = 18, spottedTimePerLure = .2f;

	bool nearSetup = false;
	Vector3 nearMaanPosition;
	const float nearMaxDistanceToBond = 8;
	float _nearTimer = 0;
	const float nearTimeBeforeBond = 4;

	bool bondedSetup = false;
	Transform bondedTargetTrans;
	float _bondedNavTimer = 0, _bondedCallTimer = 0, _bondedLeaveTimer = 0;
	float bondedTimeBetweenRetargetting = 1;
	float bondedTimeBetweenCalls, bondedMinTimeBetweenCalls = .4f, bondedMaxTimeBetweenCalls = 2.8f;
	float bondedTimeBeforeLeaving, bondedMinTimeBeforeLeaving = 16, bondedMaxTimeBeforeLeaving = 80;

	const float runAwaySpeed = 15, runAwayTime = 2.4f;
	
	float callBasePitch, callPitchMax = 1.56f, callPitchMin = 0.1f, callPitchMaxDeviation = .11f;

	Vector3 basePosition;
	float maxRoamingDistance = 5;

	public void Init (MaanManager manager, AudioClip callClip, Transform maanTrans, Transform parentPiece)
	{
		this.manager = manager;
		this.maanTrans = maanTrans;
		this.parentPiece = parentPiece;

		audioSource = GetComponent<AudioSource>();
		callBasePitch = Random.Range(callPitchMin, callPitchMax);
		audioSource.clip = callClip;

		navMeshAgent = GetComponent<NavMeshAgent>();
		
		basePosition = transform.position;
	}

	private void Update ()
	{
		Behaviour();
	}

	void Behaviour ()
	{
		switch (behaviourState) {
			case BehaviourStates.Roaming:
				//Setup
				if (!roamingDestinationSet) {
					Vector2 randomPoint = Random.insideUnitCircle * maxRoamingDistance;
					Vector3 targetPosition = basePosition + new Vector3(randomPoint.x, 0, randomPoint.y);
					navMeshAgent.destination = targetPosition;
					roamingTime = Random.Range(roamingMinTime, roamingmaxTime);
					_roamingTimer = 0;
					roamingDestinationSet = true;
				}

				//Setup new destination if timer expires
				_roamingTimer += Time.deltaTime;
				if (_roamingTimer > roamingTime) {
					roamingDestinationSet = false;
				}

				//If Maan is close, enter Spotted state
				if ((maanTrans.position - transform.position).sqrMagnitude < distanceBeforeSpotted * distanceBeforeSpotted) {
					Call();
					behaviourState = BehaviourStates.Spotted;
					goto case BehaviourStates.Spotted;
				}
				break;
			case BehaviourStates.Spotted:
				//Setup
				if (!spottedSetup) {
					Debug.Log("Spotted");
					spottedMaanPosition = maanTrans.position;
					_spottedAdvanceTimer = 0;
					spottedAdvanceTime = Random.Range(spottedMinAdvanceTime, spottedMaxAdvanceTime);
					spottedLureLimit = Random.Range(spottedMinLureLimit, spottedMaxLureLimit);
				}

				//If Maan moves away from her initial position after being spotted OR if the times lured exceedes the limit, run away
				if ((spottedMaanPosition - maanTrans.position).sqrMagnitude > 6 || spottedLureLimit < _spottedTimesLured) {
					StartCoroutine(RunAway());
				}

				//Keep looking at Maan
				transform.LookAt(maanTrans);

				//Every time the advance timer runs out advance a little bit towards Maan
				_spottedAdvanceTimer += Time.deltaTime;
				if (_spottedAdvanceTimer >= spottedAdvanceTime && !spottedAdvancing) {
					spottedAdvancing = true;
					spottedTargetPosition = Vector3.MoveTowards(transform.position, spottedMaanPosition + transform.forward * -spottedTargetDistance, Random.Range(spottedMinAdvanceDistance, spottedMaxAdvanceDistance));
				}

				//Move towards Maan. If the end position is close enough, advance to Near state ELSE reset the timer and wait for the next advance
				if (spottedAdvancing) {
					transform.position = Vector3.MoveTowards(transform.position, spottedTargetPosition, spottedAdvanceSpeed * Time.deltaTime);
					if ((transform.position - spottedTargetPosition).sqrMagnitude < .5f) {
						if ((transform.position - maanTrans.position).sqrMagnitude < 2) {
							Call();
							behaviourState = BehaviourStates.Near;
							goto case BehaviourStates.Near;
						} else {
							spottedAdvancing = false;
							_spottedAdvanceTimer = 0;
						}
					}
				}
				break;
			case BehaviourStates.Near:
				//Setup
				if (!nearSetup) {
					Debug.Log("Near");
					nearMaanPosition = maanTrans.position;
					nearSetup = true;
				}

				//Kattoe wiggles in excitement when near
				transform.Rotate(0, Mathf.Sin(_nearTimer * 3), 0);

				//If Maan goes too far away the kattoe resets to roaming
				if ((transform.position - maanTrans.position).sqrMagnitude > nearMaxDistanceToBond * nearMaxDistanceToBond) {
					roamingDestinationSet = false;
					behaviourState = BehaviourStates.Roaming;
					goto case BehaviourStates.Roaming;
				}

				//If the nearTime is reached the kattoe bonds with Maan
				_nearTimer += Time.deltaTime;
				if (_nearTimer >= nearTimeBeforeBond) {
					Call();
					bondedTargetTrans = maanTrans.GetComponent<Maan>().KattoeRequestFlockAnchor();
				}
				break;
			case BehaviourStates.Bonded:
				Debug.Log("Bonded");
				if (!bondedSetup) {
					bondedTimeBetweenCalls = Random.Range(bondedMinTimeBetweenCalls, bondedMaxTimeBetweenCalls);
					bondedTimeBeforeLeaving = Random.Range(bondedMinTimeBeforeLeaving, bondedMaxTimeBeforeLeaving);
				}

				_bondedNavTimer += Time.deltaTime;
				if (_bondedNavTimer >= bondedTimeBetweenRetargetting) {
					navMeshAgent.destination = bondedTargetTrans.position;
					_bondedNavTimer = 0;
				}

				_bondedCallTimer += Time.deltaTime;
				if (_bondedCallTimer >= bondedTimeBetweenCalls) {
					Call();
					_bondedCallTimer = 0;
				}

				_bondedLeaveTimer += Time.deltaTime;
				if (_bondedLeaveTimer >= bondedTimeBeforeLeaving) {
					Call();
					RunAway();
				}
				break;
		}
	}

	IEnumerator RunAway ()
	{
		Vector3 direction = transform.position - maanTrans.position;
		transform.forward = direction;

		for (float t = 0; t < runAwayTime; t += Time.deltaTime) {
			transform.position += direction * runAwaySpeed * Time.deltaTime;
			yield return null;
		}

		manager.KattoeRanAway(this, parentPiece);
		Destroy(gameObject);
	}

	public void ReceiveLure ()
	{
		_spottedAdvanceTimer += spottedTimePerLure;
		_spottedTimesLured++;
	}

	void Call ()
	{
		audioSource.pitch = callBasePitch + Random.Range(-callPitchMaxDeviation, callPitchMaxDeviation);
		audioSource.Play();
	}
}