using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Utility;

public class MaanManager : MonoBehaviour
{
	Transform[] trackPieces;
	Maan maan;

	public GameObject kattoeprefab;
	public AudioClip[] kattoeClips;

	int activeAnimals = 15;

	public GameObject cloudPrefab;
	Transform cloudTrans;
	Cloud cloud;
	enum CloudStates { Dormant, Waiting, Chasing };
	CloudStates cloudState = CloudStates.Dormant;
	float _cloudTimer = 0, cloudTime;
	float minTimeBeforeCloudSpawn = 12, maxTimeBeforeCloudSpawn = 40;
	float cloudSpawningHeight = 30;

	float cloudWaitingSpeed = 4, cloudWaitingHeight = 0;
	float minTimeBeforeCloudChase = 0, maxTimeBeforeCloudChase = 0;

	float _cloudDescendSpeed = 0;
	float cloudDescendBaseSpeed = 1, cloudDescendAcceleration = .1f;

	float cloudDefaultSize = 36, cloudSmallSize = 20, cloudGrowthRate = 20;
	float _cloudSize;

	public void Init (Transform[] trackPieces, Maan maan)
	{
		this.trackPieces = trackPieces;
		this.maan = maan;

		Transform[] spawnPositions = Util.PickRandom(activeAnimals, false, trackPieces);
		for (int i = 0; i < activeAnimals; i++) {
			CreateKattoe(spawnPositions[i].position);
		}

		cloudTime = Random.Range(minTimeBeforeCloudSpawn, maxTimeBeforeCloudSpawn);
	}

	void CreateKattoe (Vector3 position)
	{
		Kattoe newKattoe = Instantiate(kattoeprefab, position, Quaternion.identity).GetComponent<Kattoe>();
		newKattoe.Init(Util.PickRandom(kattoeClips));
	}

	private void Update ()
	{
		Cloud();
		CloudVisuals();
	}

	void Cloud ()
	{
		switch (cloudState) {
			case CloudStates.Dormant:
				_cloudTimer += Time.deltaTime;
				if (_cloudTimer >= cloudTime) {
					StartCoroutine(SpawnCloud());
					_cloudTimer = 0;
					float baseTimeBeforeChase = (cloudWaitingHeight - cloudSpawningHeight) / cloudWaitingSpeed;
					cloudTime = baseTimeBeforeChase + Random.Range(minTimeBeforeCloudChase, maxTimeBeforeCloudChase);
					cloudState = CloudStates.Waiting;
				}
				break;
			case CloudStates.Waiting:
				_cloudTimer += Time.deltaTime;
				if (_cloudTimer >= cloudTime) {
					_cloudTimer = 0;
					_cloudDescendSpeed = cloudDescendBaseSpeed;
					cloudState = CloudStates.Chasing;
				}
				break;
			case CloudStates.Chasing:
				_cloudDescendSpeed += cloudDescendAcceleration * Time.deltaTime;
				Vector3 _cloudPosition = cloudTrans.position;
				_cloudPosition.y -= _cloudDescendSpeed * Time.deltaTime;
				_cloudPosition.x = maan.transform.position.x;
				_cloudPosition.z = maan.transform.position.z;
				cloudTrans.position = _cloudPosition;
				//cloudTrans.position = Vector3.MoveTowards(cloudTrans.position, maan.transform.position, _cloudChaseSpeed * Time.deltaTime);
				//cloudTrans.LookAt(maan.transform);
				if ((cloudTrans.position - maan.transform.position).sqrMagnitude < 16) {
					Debug.Log("I've caught myself a Maan");
					_cloudTimer = 0;
					_cloudDescendSpeed = cloudDescendBaseSpeed;
					Destroy(cloudTrans.gameObject);
					cloudState = CloudStates.Dormant;
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
			cloudTrans.localScale = new Vector3(_cloudSize, .1f, _cloudSize);

			maan.VisualReactionToCloud(distanceMaanToCloud);
		} else {
			maan.VisualReactionToCloud(1000);
		}
	}

	IEnumerator SpawnCloud ()
	{
		cloudTrans = Instantiate(cloudPrefab, Util.PickRandom(trackPieces).position + Vector3.up * cloudSpawningHeight, Quaternion.identity).transform;
		cloud = cloudTrans.GetComponent<Cloud>();
		cloudTrans.localScale = new Vector3(cloudDefaultSize, .1f, cloudDefaultSize);
		_cloudSize = cloudDefaultSize;
		float riseTime = (cloudWaitingHeight - cloudSpawningHeight) / cloudWaitingSpeed;
		for (float t = 0; t < riseTime; t += Time.deltaTime) {
			cloudTrans.position += Vector3.up * cloudWaitingSpeed * Time.deltaTime;
			yield return null;
		}
	}
}