using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Utility;
using FMODUnity;

public class MaanManager : MonoBehaviour
{
	Transform[] _trackPieces;
	Maan maan;
	bool _firstTimeInit = true;

	public GameObject[] kattoePrefabs;

	float _happiness = 0;
	const float happinessGrowthRate = .5f;
	const float happinessWhenLinked = .5f;
	const float happinessBadCloudFar = -.3f, happinessBadCloudNear = -1.1f;
	const float happinessGoodCloudFar = -.1f, happinessGoodCloudNear = -.36f;
	const float happinessCloudNearDistance = 12, happinessCloudFarDistance = 55;
	const float happinessPerKattoe = 0.18f;
	const float happinessScreenShakeThreshold = .3f;
	const float happinessMusicThreshold = .3f;
	const float happinessMusicMaxThreshold = .6f;
	float _happinessMusicVolume = 0;
	const float happinessMusicVolumeGrowthRate = .4f;

	PostProcessVolume postProcessingVolume;
	public PostProcessProfile ppHappyProfile;
	public PostProcessProfile ppSadProfile;

	int activeKattoes = 7;
	Transform kattoeParent;
	List<Kattoe> kattoes;
	List<Transform> occupiedPieces;

	public GameObject cloudPrefab;
	Transform _cloudTrans;
	Cloud cloud;
	enum CloudStates { Dormant, Waiting, Chasing };
	CloudStates cloudState;

	float cloudSpawnDistance = 60;

	bool cloudDormantSetup = false;
	float _cloudDormantTimer = 0, cloudDormantTime;
	const float cloudMinTimeBeforeSpawn = 40, cloudMaxTimeBeforeSpawn = 8;
	const float cloudSpawningHeight = 4;

	bool cloudWaitingSetup = false;
	float _cloudWaitingTimer = 0, cloudWaitingTime;
	const float cloudWaitingSpeed = 4, cloudWaitingHeight = 0;
	const float minTimeBeforeCloudChase = 1.0f, maxTimeBeforeCloudChase = 2.2f;

	bool cloudChaseSetup = false;
	float cloudDescendSpeed = 3, _cloudChaseSpeed = 0;
	float cloudChaseBaseSpeed = 3, cloudChaseAcceleration = .33f;
	float cloudChasingHeight = 0;
	float cloudChasingDistanceToImpact = 4;
	float cloudImpactFadeTimeGood = 0, cloudImpactFadeTimeBad = 4f;

	//FMOD
	string fmodCloudPath = "event:/Maan/Stress_Monster";
	FMOD.Studio.EventInstance fmodCloudInstance;
	FMOD.Studio.ParameterInstance fmodCloudStateParameter; //0 == evil, 1 == good
	bool fmodCloudPlaying = false;

	string fmodKattoeMusicPath = "event:/Maan/Cat_Song";
	FMOD.Studio.EventInstance fmodKattoeMusicInstance;

	public void Init (Transform[] trackPieces, Maan maan)
	{
		if (_firstTimeInit) {
			_trackPieces = trackPieces;

			_firstTimeInit = false;
		}

		this.maan = maan;

		postProcessingVolume = maan.transform.GetComponentInChildren<PostProcessVolume>();

		kattoeParent = new GameObject("KattoeParent").transform;

		kattoes = new List<Kattoe>();
		occupiedPieces = new List<Transform>();
		Transform[] spawnPositions = Util.PickRandom(activeKattoes, false, trackPieces);
		for (int i = 0; i < activeKattoes; i++) {
			kattoes.Add(CreateKattoe(spawnPositions[i]));
			occupiedPieces.Add(spawnPositions[i]);
		}

		cloudState = CloudStates.Dormant;

		fmodCloudInstance = RuntimeManager.CreateInstance(fmodCloudPath);
		fmodCloudInstance.getParameter("Stress", out fmodCloudStateParameter);
		fmodKattoeMusicInstance = RuntimeManager.CreateInstance(fmodKattoeMusicPath);
		fmodKattoeMusicInstance.start();
	}

	public void Deactivate ()
	{
		StopAllCoroutines();

		Destroy(kattoeParent.gameObject);
		if (_cloudTrans != null)
			Destroy(_cloudTrans.gameObject);
		maan.Destroy();
	}

	private void Update ()
	{
		if (!StaticData.menuActive) {
			Cloud();
			CloudFeedback();
			Happiness();
		}
	}

	void Happiness ()
	{
		float targetHappiness = 0;

		//Kevin
		if (StaticData.playersAreLinked)
			targetHappiness += happinessWhenLinked;
		
		//Cloud
		if (cloudState != CloudStates.Dormant) {
			float distanceMaanToCloud = Vector3.Distance(maan.transform.position, _cloudTrans.position);
			if (distanceMaanToCloud >= happinessCloudFarDistance) {
				if (StaticData.playersAreLinked) {
					targetHappiness += happinessGoodCloudFar;
				} else {
					targetHappiness += happinessBadCloudFar;
				}
			} else if (distanceMaanToCloud <= happinessCloudNearDistance) {
				if (StaticData.playersAreLinked) {
					targetHappiness += happinessGoodCloudNear;
				} else {
					targetHappiness += happinessBadCloudNear;
				}
			} else {
				float factor = 1 - (distanceMaanToCloud - happinessCloudNearDistance) / (happinessCloudFarDistance - happinessCloudNearDistance);
				Debug.Log("factor " + factor.ToString());
				if (StaticData.playersAreLinked) {
					targetHappiness += (factor * (happinessGoodCloudNear - happinessGoodCloudFar)) + happinessGoodCloudFar;
				} else {
					targetHappiness += (factor * (happinessBadCloudNear - happinessBadCloudFar)) + happinessBadCloudFar;
				}
			}
		}

		//Kattoes
		targetHappiness += happinessPerKattoe * maan.KattoesBonded;

		//Calculate actual happiness
		_happiness = Mathf.MoveTowards(_happiness, targetHappiness, happinessGrowthRate * Time.deltaTime);
		_happiness = Mathf.Clamp(_happiness, -1, 1);

		//Apply happiness
		//PostProcessing
		if (_happiness >= 0) {
			postProcessingVolume.profile = ppHappyProfile;
		} else {
			postProcessingVolume.profile = ppSadProfile;
		}
		postProcessingVolume.weight = Mathf.Abs(_happiness);

		//ScreenShake
		if (_happiness <= -happinessScreenShakeThreshold) {
			float screenShakeIntensity = (Mathf.Abs(_happiness) - happinessScreenShakeThreshold) / (1 - happinessScreenShakeThreshold);
			maan.ScreenShake(screenShakeIntensity);
		} else {
			maan.ScreenShake(0);
		}

		//KattoeMusic
		float desiredVolume = 0;
		if (_happiness < happinessMusicThreshold) {
			desiredVolume = 0;
		} else if (_happiness > happinessMusicMaxThreshold) {
			desiredVolume = 1;
		} else {
			desiredVolume = .7f;
		}
		_happinessMusicVolume = Mathf.MoveTowards(_happinessMusicVolume, desiredVolume, happinessMusicVolumeGrowthRate * Time.deltaTime);
		fmodKattoeMusicInstance.setVolume(_happinessMusicVolume);
	}

	Kattoe CreateKattoe (Transform parentPiece)
	{
		Kattoe newKattoe = Instantiate(Util.PickRandom(kattoePrefabs), parentPiece.position, Quaternion.identity).GetComponent<Kattoe>();
		newKattoe.Init(this, maan.transform, parentPiece);
		newKattoe.transform.parent = kattoeParent;
		return newKattoe;
	}

	public void KattoeRanAway (Kattoe kattoe, Transform parentPiece)
	{
		kattoes.Remove(kattoe);
		occupiedPieces.Remove(parentPiece);
		StartCoroutine(ReplaceKattoe());
	}

	IEnumerator ReplaceKattoe ()
	{
		yield return new WaitForSeconds(9);
		Transform pieceAttempt = Util.PickRandom(_trackPieces);
		while (occupiedPieces.Contains(pieceAttempt)) {
			pieceAttempt = Util.PickRandom(_trackPieces);
		}
		kattoes.Add(CreateKattoe(pieceAttempt));
	}

	void Cloud ()
	{
		switch (cloudState) {
			case CloudStates.Dormant:
				if (!cloudDormantSetup) {
					_cloudDormantTimer = 0;
					cloudDormantTime = Random.Range(cloudMinTimeBeforeSpawn, cloudMaxTimeBeforeSpawn);
					cloudDormantSetup = true;
				}

				_cloudDormantTimer += Time.deltaTime;
				if (_cloudDormantTimer > cloudDormantTime) {
					StartCoroutine(SpawnCloud());
					cloudDormantSetup = false;
					cloudState = CloudStates.Waiting;
					goto case CloudStates.Waiting;
				}
				break;
			case CloudStates.Waiting:
				if (!cloudWaitingSetup) {
					_cloudWaitingTimer = 0;
					cloudWaitingTime = Random.Range(minTimeBeforeCloudChase, maxTimeBeforeCloudChase);
					cloudWaitingSetup = true;
				}
				_cloudWaitingTimer += Time.deltaTime;
				if (_cloudWaitingTimer > cloudWaitingTime) {
					cloudChaseSetup = false;
					cloudState = CloudStates.Chasing;
					goto case CloudStates.Chasing;
				}
				break;
			case CloudStates.Chasing:
				if (!cloudChaseSetup) {
					_cloudChaseSpeed = cloudChaseBaseSpeed;
					cloudChaseSetup = true;
				}
				_cloudChaseSpeed += cloudChaseAcceleration * Time.deltaTime;

				float distanceCloudToMaan = Vector3.Distance(maan.transform.position, _cloudTrans.position);
				Vector3 _cloudPosition = _cloudTrans.position;

				_cloudPosition.y = Mathf.MoveTowards(_cloudPosition.y, cloudChasingHeight, cloudDescendSpeed * Time.deltaTime);

				Vector2 lateralCloudPos = new Vector2(_cloudPosition.x, _cloudPosition.z);
				lateralCloudPos = Vector2.MoveTowards(lateralCloudPos, new Vector2(maan.transform.position.x, maan.transform.position.z), _cloudChaseSpeed * Time.deltaTime);
				_cloudPosition.x = lateralCloudPos.x;
				_cloudPosition.z = lateralCloudPos.y;

				_cloudTrans.position = _cloudPosition;
				_cloudTrans.LookAt(maan.transform);

				if (distanceCloudToMaan < cloudChasingDistanceToImpact) {
					StartCoroutine(maan.FadeToBlack(
						StaticData.playersAreLinked ? cloudImpactFadeTimeGood : cloudImpactFadeTimeBad,
						StaticData.playersAreLinked ? new Color(.55f, .6f, .6f) : Color.black));
					Destroy(_cloudTrans.gameObject);
					cloudChaseSetup = false;
					fmodCloudInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
					fmodCloudPlaying = false;
					cloudState = CloudStates.Dormant;
					goto case CloudStates.Dormant;
				}
				break;
		}
	}

	float _fmodCloudState = 1;
	void CloudFeedback ()
	{
		if (cloudState != CloudStates.Dormant) {
			float distanceMaanToCloud = Vector3.Distance(cloud.transform.position, maan.transform.position);
			if (StaticData.playersAreLinked) {
				_fmodCloudState = Mathf.MoveTowards(_fmodCloudState, 0, Time.deltaTime * 2);
			} else {
				_fmodCloudState = Mathf.MoveTowards(_fmodCloudState, 1, Time.deltaTime * 2);
			}
			fmodCloudStateParameter.setValue(_fmodCloudState);
		}
	}

	IEnumerator SpawnCloud ()
	{
		Vector2 randomRadius = Random.insideUnitCircle.normalized * cloudSpawnDistance;
		Vector3 spawnPos = new Vector3(maan.transform.position.x + randomRadius.x, cloudSpawningHeight, maan.transform.position.z + randomRadius.y);
		_cloudTrans = Instantiate(cloudPrefab, spawnPos, Quaternion.identity).transform;
		cloud = _cloudTrans.GetComponent<Cloud>();

		RuntimeManager.AttachInstanceToGameObject(fmodCloudInstance, _cloudTrans, _cloudTrans.GetComponent<Rigidbody>());
		fmodCloudInstance.start();

		float _riseTime = (cloudWaitingHeight - cloudSpawningHeight) / cloudWaitingSpeed;
		for (float t = 0; t < _riseTime; t += Time.deltaTime) {
			_cloudTrans.position += Vector3.up * cloudWaitingSpeed * Time.deltaTime;
			yield return null;
		}
	}
}