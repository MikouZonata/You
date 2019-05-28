using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Utility;

public class MaanManager : MonoBehaviour
{
	Transform[] trackPieces;
	Maan maan;

	public GameObject kattoePrefab;
	public AudioClip[] kattoeClips;

	int activeKattoes = 7;
	List<Kattoe> kattoes = new List<Kattoe>();
	List<Transform> occupiedPieces = new List<Transform>();

	public GameObject cloudPrefab;
	Transform cloudTrans;
	Cloud cloud;
	enum CloudStates { Dormant, Waiting, Chasing };
	CloudStates cloudState = CloudStates.Dormant;

	bool cloudDormantSetup = false;
	float _cloudDormantTimer = 0, cloudDormantTime;
	const float cloudMinTimeBeforeSpawn = 30, cloudMaxTimeBeforeSpawn = 90;
	const float cloudSpawningHeight = 20;

	bool cloudWaitingSetup = false;
	float _cloudWaitingTimer = 0, cloudWaitingTime;
	float cloudWaitingSpeed = 4, cloudWaitingHeight = 0;
	float minTimeBeforeCloudChase = 3, maxTimeBeforeCloudChase = 6;

	bool cloudChaseSetup = false;
	float cloudDescendSpeed = 3, _cloudChaseSpeed = 0;
	float cloudChaseBaseSpeed = 3, cloudChaseAcceleration = .33f;
	float cloudChasingHeight = 14;

	float cloudDefaultSize = 24, cloudSmallSize = 18, cloudGrowthRate = 20;
	float _cloudSize;

	public void Init (Transform[] trackPieces, Maan maan)
	{
		this.trackPieces = trackPieces;
		this.maan = maan;

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
	}

	Kattoe CreateKattoe (Transform parentPiece)
	{
		Kattoe newKattoe = Instantiate(kattoePrefab, parentPiece.position, Quaternion.identity).GetComponent<Kattoe>();
		newKattoe.Init(this, Util.PickRandom(kattoeClips), maan.transform, parentPiece);
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

				if (distanceCloudToMaan < 24) {
					_cloudPosition.y = Mathf.MoveTowards(_cloudPosition.y, 0, cloudDescendSpeed * Time.deltaTime);
				} else {
					_cloudPosition.y = Mathf.MoveTowards(_cloudPosition.y, cloudChasingHeight, cloudDescendSpeed * Time.deltaTime);
				}

				Vector2 lateralCloudPos = new Vector2(_cloudPosition.x, _cloudPosition.z);
				lateralCloudPos = Vector2.MoveTowards(lateralCloudPos, new Vector2(maan.transform.position.x, maan.transform.position.z), _cloudChaseSpeed * Time.deltaTime);
				_cloudPosition.x = lateralCloudPos.x;
				_cloudPosition.z = lateralCloudPos.y;

				cloudTrans.position = _cloudPosition;
				cloudTrans.LookAt(maan.transform);

				if (distanceCloudToMaan < 10) {
					StartCoroutine(maan.FadeToBlack());
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
			float _cloudTargetSize;
			if (StaticData.playersAreLinked) {
				cloud.SwitchMaterial(global::Cloud.MaterialOptions.Good);
				_cloudTargetSize = cloudSmallSize;
			} else {
				cloud.SwitchMaterial(global::Cloud.MaterialOptions.Bad);
				_cloudTargetSize = cloudDefaultSize;
			}

			_cloudSize = Mathf.MoveTowards(_cloudSize, _cloudTargetSize, cloudGrowthRate * Time.deltaTime);
			cloudTrans.localScale = new Vector3(_cloudSize, _cloudSize, _cloudSize);

			maan.VisualReactionToCloud(distanceMaanToCloud);
		} else {
			maan.VisualReactionToCloud(1000);
		}
	}

	IEnumerator SpawnCloud ()
	{
		cloudTrans = Instantiate(cloudPrefab, Util.PickRandom(trackPieces).position + Vector3.up * cloudSpawningHeight, Quaternion.identity).transform;
		cloud = cloudTrans.GetComponent<Cloud>();
		cloudTrans.localScale = new Vector3(cloudDefaultSize, cloudDefaultSize, cloudDefaultSize);
		_cloudSize = cloudDefaultSize;
		float riseTime = (cloudWaitingHeight - cloudSpawningHeight) / cloudWaitingSpeed;
		for (float t = 0; t < riseTime; t += Time.deltaTime) {
			cloudTrans.position += Vector3.up * cloudWaitingSpeed * Time.deltaTime;
			yield return null;
		}
	}
}