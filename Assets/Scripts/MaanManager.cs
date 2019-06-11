using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Utility;
using MultiAudioListener;

public class MaanManager : MonoBehaviour
{
	Transform[] trackPieces;
	Maan maan;

	public GameObject[] kattoePrefabs;
	public AudioClip[] kattoeClips;

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
	const float cloudMinTimeBeforeSpawn = 0, cloudMaxTimeBeforeSpawn = 0;
	const float cloudSpawningHeight = 4;

	bool cloudWaitingSetup = false;
	float _cloudWaitingTimer = 0, cloudWaitingTime;
	float cloudWaitingSpeed = 4, cloudWaitingHeight = 0;
	float minTimeBeforeCloudChase = 1.0f, maxTimeBeforeCloudChase = 3.0f;

	bool cloudChaseSetup = false;
	float cloudDescendSpeed = 3, _cloudChaseSpeed = 0;
	float cloudChaseBaseSpeed = 3, cloudChaseAcceleration = .33f;
	float cloudChasingHeight = 0;
	float cloudChasingDistanceToImpact = 4;
	float cloudImpactFadeTimeGood = 2, cloudImpactFadeTimeBad = 4f;

	public MultiAudioSource kattoeMusicAudioSource;
	float _kattoeMusicVolume = 0, kattoeMusicVolumeMutationRate = .5f, kattoeMusicMaxVolume = .5f;
	int kattoeMusicThreshold = 3;

	public void Init (Transform[] trackPieces, Maan maan)
	{
		this.trackPieces = trackPieces;
		this.maan = maan;

		kattoeParent = new GameObject("KattoeParent").transform;

		Transform[] spawnPositions = Util.PickRandom(activeKattoes, false, trackPieces);
		for (int i = 0; i < activeKattoes; i++) {
			kattoes.Add(CreateKattoe(spawnPositions[i]));
			occupiedPieces.Add(spawnPositions[i]);
		}
	}

	private void Update ()
	{
		Cloud();
		CloudVisuals();
		KattoeMusic();
	}

	Kattoe CreateKattoe (Transform parentPiece)
	{
		Kattoe newKattoe = Instantiate(Util.PickRandom(kattoePrefabs), parentPiece.position, Quaternion.identity).GetComponent<Kattoe>();
		newKattoe.Init(this, Util.PickRandom(3, false, kattoeClips), maan.transform, parentPiece);
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
		Transform pieceAttempt = Util.PickRandom(trackPieces);
		while (occupiedPieces.Contains(pieceAttempt)) {
			pieceAttempt = Util.PickRandom(trackPieces);
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

				float distanceCloudToMaan = Vector3.Distance(maan.transform.position, cloudTrans.position);
				Vector3 _cloudPosition = cloudTrans.position;


				_cloudPosition.y = Mathf.MoveTowards(_cloudPosition.y, cloudChasingHeight, cloudDescendSpeed * Time.deltaTime);


				Vector2 lateralCloudPos = new Vector2(_cloudPosition.x, _cloudPosition.z);
				lateralCloudPos = Vector2.MoveTowards(lateralCloudPos, new Vector2(maan.transform.position.x, maan.transform.position.z), _cloudChaseSpeed * Time.deltaTime);
				_cloudPosition.x = lateralCloudPos.x;
				_cloudPosition.z = lateralCloudPos.y;

				cloudTrans.position = _cloudPosition;
				cloudTrans.LookAt(maan.transform);

				if (distanceCloudToMaan < cloudChasingDistanceToImpact) {
					StartCoroutine(maan.FadeToBlack(StaticData.playersAreLinked ? cloudImpactFadeTimeGood : cloudImpactFadeTimeBad));
					Destroy(cloudTrans.gameObject);
					cloudChaseSetup = false;
					cloudState = CloudStates.Dormant;
					goto case CloudStates.Dormant;
				}
				break;
		}
	}

	void CloudVisuals ()
	{
		if (cloudState != CloudStates.Dormant) {
			float distanceMaanToCloud = Vector3.Distance(cloud.transform.position, maan.transform.position);
			maan.VisualReactionToCloud(distanceMaanToCloud);
		} else {
			maan.VisualReactionToCloud(1000);
		}
	}

	IEnumerator SpawnCloud ()
	{
		Vector2 randomRadius = Random.insideUnitCircle.normalized * cloudSpawnDistance;
		Vector3 spawnPos = new Vector3(maan.transform.position.x + randomRadius.x, cloudSpawningHeight, maan.transform.position.z + randomRadius.y);
		cloudTrans = Instantiate(cloudPrefab, spawnPos, Quaternion.identity).transform;
		cloud = cloudTrans.GetComponent<Cloud>();
		float riseTime = (cloudWaitingHeight - cloudSpawningHeight) / cloudWaitingSpeed;
		for (float t = 0; t < riseTime; t += Time.deltaTime) {
			cloudTrans.position += Vector3.up * cloudWaitingSpeed * Time.deltaTime;
			yield return null;
		}
	}

	void KattoeMusic ()
	{
		if (StaticData.kattoesBondedToMaan >= kattoeMusicThreshold) {
			_kattoeMusicVolume = Mathf.MoveTowards(_kattoeMusicVolume, kattoeMusicMaxVolume, kattoeMusicVolumeMutationRate * Time.deltaTime);
		} else {
			_kattoeMusicVolume = Mathf.MoveTowards(_kattoeMusicVolume, 0, kattoeMusicVolumeMutationRate * Time.deltaTime);
		}
		kattoeMusicAudioSource.Volume = _kattoeMusicVolume;
	}
}