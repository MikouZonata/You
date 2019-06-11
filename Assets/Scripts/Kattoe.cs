using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MultiAudioListener;
using Utility;

public class Kattoe : MonoBehaviour
{
	MaanManager manager;
	MultiAudioSource audioSource;
	AudioClip[] callClips;
	NavMeshAgent navMeshAgent;
	Maan maan;
	Transform maanTrans;
	Transform parentPiece;

	public enum BehaviourStates { Roaming, Spotted, Near, Bonded, RunningAway };
	BehaviourStates behaviourState = BehaviourStates.Roaming;

	bool roamingSetup = false, roamingDestinationSet = false;
	float _roamingTimer = 0, roamingTime = 0;
	const float roamingMinTime = 1, roamingmaxTime = 3.6f;
	const float maxRoamingDistance = 5;
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
	const float spottedMinLureLimit = 5, spottedMaxLureLimit = 56, spottedTimePerLure = .1f;

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
	float bondedTimeBeforeLeaving, bondedMinTimeBeforeLeaving = 72, bondedMaxTimeBeforeLeaving = 130;

	const float runAwaySpeed = 20, runAwayTime = 1.2f;

	float callBasePitch, callPitchMax = 1.16f, callPitchMin = 0.9f, callPitchMaxDeviation = .08f;

	Vector3 basePosition;

	bool hopping = false, hopSetup = false;
	float hopHeight = .32f, hopTime = .24f;

	public void Init (MaanManager manager, AudioClip[] callClips, Transform maanTrans, Transform parentPiece)
	{
		this.manager = manager;
		this.maanTrans = maanTrans;
		maan = maanTrans.GetComponent<Maan>();
		this.parentPiece = parentPiece;

		audioSource = GetComponent<MultiAudioSource>();
		callBasePitch = Random.Range(callPitchMin, callPitchMax);
		this.callClips = callClips;

		navMeshAgent = GetComponent<NavMeshAgent>();

		basePosition = transform.position;
	}

	private void Update ()
	{
		Behaviour();
		Hop();
	}

	void Behaviour ()
	{
		switch (behaviourState) {
			//Idle behaviour in which the kattoe roams about doing its thing
			case BehaviourStates.Roaming:
				//Setup
				if (!roamingSetup) {
					navMeshAgent.enabled = true;
					maan.EngagedByKattoe(this, false);
					roamingDestinationSet = false;
					roamingSetup = true;
				}

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
					roamingSetup = false;
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
				if ((spottedMaanPosition - maanTrans.position).sqrMagnitude > 25) {
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
						spottedSetup = false;
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
					nearSetup = false;
					behaviourState = BehaviourStates.Roaming;
					goto case BehaviourStates.Roaming;
				}

				//If the nearTime is reached the kattoe bonds with Maan
				_nearTimer += Time.deltaTime;
				if (_nearTimer >= nearTimeBeforeBond) {
					Call();
					bondedTargetTrans = maanTrans.GetComponent<Maan>().KattoeRequestFlockAnchor();
					navMeshAgent.enabled = true;
					nearSetup = false;
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
					maan.KattoeLeaveFlock(bondedTargetTrans);
					StartCoroutine(RunAway());
				}
				break;
		}
	}

	void Hop ()
	{
		if (!hopSetup) {
			hopSetup = true;
			StartCoroutine(HopRoutine());
		}

		if (navMeshAgent.velocity.sqrMagnitude > 1) {
			hopping = true;
		} else {
			hopping = false;
		}
	}

	enum Directions { Down, Up };
	IEnumerator HopRoutine ()
	{
		Transform modelTrans = transform.GetChild(0);
		float hopSpeed = hopHeight * 2 / hopTime;
		Directions direction = Directions.Up;
		float _yPos = 0;

		while (true) {
			if (direction == Directions.Up && hopping) {
				_yPos = Mathf.MoveTowards(_yPos, hopHeight, hopSpeed * Time.deltaTime);
				if (_yPos >= hopHeight) {
					direction = Directions.Down;
				}
			} else {
				_yPos = Mathf.MoveTowards(_yPos, 0, hopSpeed * Time.deltaTime);
				if (_yPos <= 0) {
					direction = Directions.Up;
				}
			}

			modelTrans.localPosition = new Vector3(0, _yPos, 0);

			yield return null;
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
		if (_spottedTimesLured < spottedLureLimit) {
			_spottedAdvanceTimer += spottedTimePerLure;
			_spottedTimesLured++;
		}
	}

	void Call ()
	{
		audioSource.Pitch = callBasePitch + Random.Range(-callPitchMaxDeviation, callPitchMaxDeviation);
		audioSource.AudioClip = Util.PickRandom(callClips);
		audioSource.Play();
	}
}