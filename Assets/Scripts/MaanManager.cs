using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Utility;
using FMODUnity;

public class MaanManager : MonoBehaviour
{
	Transform[] _trackPieces;
	Maan _maan;
	bool _firstTimeInit = true;

	public GameObject[] kattoePrefabs;

	int activeKattoes = 7;
	Transform kattoeParent;
	List<Kattoe> kattoes = new List<Kattoe>();
	List<Transform> occupiedPieces = new List<Transform>();

	public GameObject cloudPrefab;
	Transform cloudTrans;
	Cloud cloud;
	enum CloudStates { Dormant, Waiting, Chasing };
	CloudStates cloudState = CloudStates.Dormant;

	float cloudSpawnDistance = 60;

	bool cloudDormantSetup = false;
	float _cloudDormantTimer = 0, cloudDormantTime;
	const float cloudMinTimeBeforeSpawn = 40, cloudMaxTimeBeforeSpawn = 84;
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
	float cloudImpactFadeTimeGood = 2, cloudImpactFadeTimeBad = 4f;

	int kattoeMusicThreshold = 1;

	//FMOD
	string fmodCloudPath = "event:/Maan/Stress_Monster";
	FMOD.Studio.EventInstance fmodCloudInstance;
	FMOD.Studio.ParameterInstance fmodCloudStateParameter; //0 == evil, 1 == good
	bool fmodCloudPlaying = false;

	string fmodKattoeMusicPath = "event:/Maan/Cat_Song";
	FMOD.Studio.EventInstance fmodKattoeMusicInstance;
	bool fmodKattoeMusicPlaying = false;

	public void Init (Transform[] trackPieces, Maan maan)
	{
		if (_firstTimeInit) {
			_trackPieces = trackPieces;

			_firstTimeInit = false;
		}

		_maan = maan;

		kattoeParent = new GameObject("KattoeParent").transform;

		Transform[] spawnPositions = Util.PickRandom(activeKattoes, false, trackPieces);
		for (int i = 0; i < activeKattoes; i++) {
			kattoes.Add(CreateKattoe(spawnPositions[i]));
			occupiedPieces.Add(spawnPositions[i]);
		}

		fmodCloudInstance = RuntimeManager.CreateInstance(fmodCloudPath);
		fmodCloudInstance.getParameter("Stress", out fmodCloudStateParameter);
		fmodKattoeMusicInstance = RuntimeManager.CreateInstance(fmodKattoeMusicPath);
	}

	private void Update ()
	{
		Cloud();
		CloudFeedback();
		KattoeMusic();
	}

	Kattoe CreateKattoe (Transform parentPiece)
	{
		Kattoe newKattoe = Instantiate(Util.PickRandom(kattoePrefabs), parentPiece.position, Quaternion.identity).GetComponent<Kattoe>();
		newKattoe.Init(this, _maan.transform, parentPiece);
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

				float distanceCloudToMaan = Vector3.Distance(_maan.transform.position, cloudTrans.position);
				Vector3 _cloudPosition = cloudTrans.position;

				_cloudPosition.y = Mathf.MoveTowards(_cloudPosition.y, cloudChasingHeight, cloudDescendSpeed * Time.deltaTime);

				Vector2 lateralCloudPos = new Vector2(_cloudPosition.x, _cloudPosition.z);
				lateralCloudPos = Vector2.MoveTowards(lateralCloudPos, new Vector2(_maan.transform.position.x, _maan.transform.position.z), _cloudChaseSpeed * Time.deltaTime);
				_cloudPosition.x = lateralCloudPos.x;
				_cloudPosition.z = lateralCloudPos.y;

				cloudTrans.position = _cloudPosition;
				cloudTrans.LookAt(_maan.transform);

				if (distanceCloudToMaan < cloudChasingDistanceToImpact) {
					StartCoroutine(_maan.FadeToBlack(
						StaticData.playersAreLinked ? cloudImpactFadeTimeGood : cloudImpactFadeTimeBad,
						StaticData.playersAreLinked ? new Color(.55f, .6f, .6f) : Color.black));
					Destroy(cloudTrans.gameObject);
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
			float distanceMaanToCloud = Vector3.Distance(cloud.transform.position, _maan.transform.position);
			_maan.VisualReactionToCloud(distanceMaanToCloud);
			if (StaticData.playersAreLinked) {
				_fmodCloudState = Mathf.MoveTowards(_fmodCloudState, 0, Time.deltaTime * 2);
			} else {
				_fmodCloudState = Mathf.MoveTowards(_fmodCloudState, 1, Time.deltaTime * 2);
			}
			fmodCloudStateParameter.setValue(_fmodCloudState);
		} else {
			_maan.VisualReactionToCloud(1000);
		}
	}

	IEnumerator SpawnCloud ()
	{
		Vector2 randomRadius = Random.insideUnitCircle.normalized * cloudSpawnDistance;
		Vector3 spawnPos = new Vector3(_maan.transform.position.x + randomRadius.x, cloudSpawningHeight, _maan.transform.position.z + randomRadius.y);
		cloudTrans = Instantiate(cloudPrefab, spawnPos, Quaternion.identity).transform;
		cloud = cloudTrans.GetComponent<Cloud>();

		RuntimeManager.AttachInstanceToGameObject(fmodCloudInstance, cloudTrans, cloudTrans.GetComponent<Rigidbody>());
		fmodCloudInstance.start();

		float _riseTime = (cloudWaitingHeight - cloudSpawningHeight) / cloudWaitingSpeed;
		for (float t = 0; t < _riseTime; t += Time.deltaTime) {
			cloudTrans.position += Vector3.up * cloudWaitingSpeed * Time.deltaTime;
			yield return null;
		}
	}

	void KattoeMusic ()
	{
		if (!fmodKattoeMusicPlaying && _maan.KattoesBonded >= kattoeMusicThreshold) {
			fmodKattoeMusicPlaying = true;
			fmodKattoeMusicInstance.start();
		}
		if (fmodKattoeMusicPlaying && _maan.KattoesBonded < kattoeMusicThreshold) {
			fmodKattoeMusicPlaying = false;
			fmodKattoeMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
	}

	public void Deactivate ()
	{

		_maan.Destroy();
	}
}