using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Kattoe : MonoBehaviour
{
	MaanManager manager;
	AudioSource audioSource;
	NavMeshAgent navMeshAgent;
	Maan maan;
	Transform maanTrans;
	Transform parentPiece;

	public enum BehaviourStates { Roaming, Spotted, Near, Bonded, RunningAway };
	BehaviourStates behaviourState = BehaviourStates.Roaming;

	bool roamingDestinationSet = false;
	float _roamingTimer = 0, roamingTime = 0;
	const float roamingMinTime = 1, roamingmaxTime = 3.6f;
	const float distanceBeforeSpotted = 16;

	bool spottedSetup = false, spottedAdvancing = false;
	Vector3 spottedMaanPosition, spottedTargetPosition;
	float _spottedAdvanceTimer = 0, spottedAdvanceTime;
	float _spottedMovingTimer = 0, spottedMovingTime;
	const float spottedMinAdvanceTime = .8f, spottedMaxAdvanceTime = 3;
	const float spottedMinAdvanceDistance = 2, spottedMaxAdvanceDistance = 6;
	const float spottedMoveSpeed = 3;
	const float spottedMinMoveTime = .4f, spottedMaxMoveTime = 1.2f;
	const float spottedTargetDistance = 3;
	float _spottedTimesLured = 0, spottedLureLimit;
	const float spottedMinLureLimit = 5, spottedMaxLureLimit = 18, spottedTimePerLure = .2f;

	bool nearSetup = false;
	Vector3 nearMaanPosition;
	const float nearMaxDistanceToBond = 8;
	float _nearTimer = 0;
	const float nearTimeBeforeBond = 2;

	public GameObject bondedFeedbackPrefab;
	float bondedFeedBackHeight = 2.2f;
	bool bondedSetup = false;
	Transform bondedTargetTrans;
	float bondedMinMovementSpeed = 7, bondedMaxMovementSpeed = 22;
	float _bondedNavTimer = 0, _bondedCallTimer = 0, _bondedLeaveTimer = 0;
	float bondedTimeBetweenRetargetting = 1;
	float bondedTimeBetweenCalls, bondedMinTimeBetweenCalls = 1.6f, bondedMaxTimeBetweenCalls = 3.6f;
	float bondedTimeBeforeLeaving, bondedMinTimeBeforeLeaving = 16, bondedMaxTimeBeforeLeaving = 80;

	const float runAwaySpeed = 20, runAwayTime = 1.2f;

	float callBasePitch, callPitchMax = 1.56f, callPitchMin = 0.4f, callPitchMaxDeviation = .11f;

	Vector3 basePosition;
	float maxRoamingDistance = 5;

	public void Init (MaanManager manager, AudioClip callClip, Transform maanTrans, Transform parentPiece)
	{
		this.manager = manager;
		this.maanTrans = maanTrans;
		maan = maanTrans.GetComponent<Maan>();
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
			//Idle behaviour in which the kattoe roams about doing its thing
			case BehaviourStates.Roaming:
				//Setup
				if (!roamingDestinationSet) {
					Vector2 randomPoint = Random.insideUnitCircle * maxRoamingDistance;
					Vector3 targetPosition = basePosition + new Vector3(randomPoint.x, 0, randomPoint.y);
					navMeshAgent.enabled = true;
					navMeshAgent.destination = targetPosition;
					roamingTime = Random.Range(roamingMinTime, roamingmaxTime);
					_roamingTimer = 0;
					maan.EngagedByKattoe(this, false);
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
			//Maan has been spotted and the kattoe will slowly approach and respond to being lured
			case BehaviourStates.Spotted:
				//Setup
				if (!spottedSetup) {
					navMeshAgent.enabled = false;
					spottedMaanPosition = maanTrans.position;
					_spottedAdvanceTimer = 0;
					spottedAdvanceTime = Random.Range(spottedMinAdvanceTime, spottedMaxAdvanceTime);
					spottedLureLimit = Random.Range(spottedMinLureLimit, spottedMaxLureLimit);
					maan.EngagedByKattoe(this, true);
					spottedSetup = true;
				}

				//If Maan moves away from her initial position after being spotted OR if the times lured exceedes the limit, run away
				if ((spottedMaanPosition - maanTrans.position).sqrMagnitude > 25 || spottedLureLimit < _spottedTimesLured) {
					StartCoroutine(RunAway());
				}

				//Keep looking at Maan
				transform.LookAt(maanTrans);

				//Every time the advance timer runs out advance a little bit towards Maan
				_spottedAdvanceTimer += Time.deltaTime;
				if (_spottedAdvanceTimer >= spottedAdvanceTime && !spottedAdvancing) {
					spottedAdvancing = true;
					_spottedAdvanceTimer = 0;
					_spottedMovingTimer = 0;
					spottedMovingTime = Random.Range(spottedMinMoveTime, spottedMaxMoveTime);
					spottedMaanPosition = maanTrans.position;
				}

				//Move towards Maan. If Kattoe gets close enough, advance to Near state ELSE reset the timer and wait for the next advance
				if (spottedAdvancing) {
					transform.position += transform.forward * spottedMoveSpeed * Time.deltaTime;

					//If Maan en Kattoe are close enough proceed to Near state
					if ((transform.position - maanTrans.position).sqrMagnitude < spottedTargetDistance * spottedTargetDistance) {
						Call();
						behaviourState = BehaviourStates.Near;
						goto case BehaviourStates.Near;
					}

					//Cut out the moving if time has expired
					_spottedMovingTimer += Time.deltaTime;
					if (_spottedMovingTimer > spottedMovingTime) {
						spottedAdvancing = false;
						_spottedAdvanceTimer = 0;
					}
				}
				break;
			//Trust between Kattoe and Maan has been established and it dances for a little while before joining her flock
			case BehaviourStates.Near:
				//Setup
				if (!nearSetup) {
					navMeshAgent.enabled = false;
					nearMaanPosition = maanTrans.position;
					_nearTimer = 0;
					maan.EngagedByKattoe(this, false);
					GameObject feedbackGO = Instantiate(bondedFeedbackPrefab, transform.position + Vector3.up * bondedFeedBackHeight, transform.rotation);
					Destroy(feedbackGO, nearTimeBeforeBond);
					nearSetup = true;
				}

				//Kattoe wiggles in excitement when near
				transform.Rotate(0, Mathf.Sin(_nearTimer * 8) * 1.5f, 0);

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
					navMeshAgent.enabled = true;
					behaviourState = BehaviourStates.Bonded;
					goto case BehaviourStates.Bonded;
				}
				break;
			//Kattoe and Maan have bonded, the Kattoe follows her around in her flock until it's time for it to leave
			case BehaviourStates.Bonded:
				if (!bondedSetup) {
					navMeshAgent.speed = Random.Range(bondedMinMovementSpeed, bondedMaxMovementSpeed);
					bondedTimeBetweenCalls = Random.Range(bondedMinTimeBetweenCalls, bondedMaxTimeBetweenCalls);
					bondedTimeBeforeLeaving = Random.Range(bondedMinTimeBeforeLeaving, bondedMaxTimeBeforeLeaving);
					navMeshAgent.destination = bondedTargetTrans.position;
					_bondedNavTimer = 0;
					_bondedCallTimer = 0;
					_bondedLeaveTimer = 0;
					maan.EngagedByKattoe(this, false);
					bondedSetup = true;
				}

				//Update the target position once every second
				_bondedNavTimer += Time.deltaTime;
				if (_bondedNavTimer >= bondedTimeBetweenRetargetting) {
					navMeshAgent.destination = bondedTargetTrans.position;
					_bondedNavTimer = 0;
				}

				//Emit a call every now and then
				_bondedCallTimer += Time.deltaTime;
				if (_bondedCallTimer >= bondedTimeBetweenCalls) {
					Call();
					_bondedCallTimer = 0;
				}

				//Leave when time is up
				_bondedLeaveTimer += Time.deltaTime;
				if (_bondedLeaveTimer >= bondedTimeBeforeLeaving) {
					Call();
					StartCoroutine(RunAway());
				}
				break;
		}
	}

	IEnumerator RunAway ()
	{
		behaviourState = BehaviourStates.RunningAway;
		navMeshAgent.enabled = false;

		Vector3 runningDirection;
		transform.LookAt(maanTrans);
		transform.forward *= -1;
		runningDirection = transform.forward;

		for (float t = 0; t < runAwayTime; t += Time.deltaTime) {
			transform.forward = runningDirection;
			transform.position += transform.forward * runAwaySpeed * Time.deltaTime;
			yield return null;
		}

		manager.KattoeRanAway(this, parentPiece);
		maan.EngagedByKattoe(this, false);
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