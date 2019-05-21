using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class MaanManager : MonoBehaviour
{
	Transform[] trackPieces;
	Maan[] maans;

	public GameObject kattoeprefab;
	public AudioClip[] kattoeClips;

	int activeAnimals = 20;

	public GameObject cloudPrefab;
	Transform cloudTrans;
	enum CloudStates { Dormant, Waiting, Chasing };
	CloudStates cloudState = CloudStates.Dormant;
	float _cloudTimer = 0, cloudTime;
	float minTimeBeforeCloudSpawn = 0, maxTimeBeforeCloudSpawn = 2;
	float cloudSpawningHeight = -11;

	float cloudWaitingSpeed = 4, cloudWaitingHeight = 15;
	float minTimeBeforeCloudChase = 0, maxTimeBeforeCloudChase = 2;

	float _cloudChaseSpeed = 0;
	float cloudChaseBaseSpeed = 3, cloudChaseAcceleration = .1f;

	public void Init (Transform[] trackPieces, params Maan[] maans)
	{
		this.trackPieces = trackPieces;
		this.maans = maans;

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
					_cloudChaseSpeed = cloudChaseBaseSpeed;
					cloudState = CloudStates.Chasing;
				}
				break;
			case CloudStates.Chasing:
				_cloudChaseSpeed += cloudChaseAcceleration * Time.deltaTime;
				cloudTrans.position = Vector3.MoveTowards(cloudTrans.position, maans[0].transform.position, _cloudChaseSpeed * Time.deltaTime);
				if ((cloudTrans.position - maans[0].transform.position).sqrMagnitude < 64) {
					Debug.Log("I've caught myself a Maan");
					_cloudTimer = 0;
					_cloudChaseSpeed = cloudChaseBaseSpeed;
					Destroy(cloudTrans.gameObject);
					cloudState = CloudStates.Dormant;
				}
				break;
		}
	}

	IEnumerator SpawnCloud ()
	{
		cloudTrans = Instantiate(cloudPrefab, Util.PickRandom(trackPieces).position - Vector3.up * cloudSpawningHeight, Quaternion.identity).transform;
		float riseTime = (cloudWaitingHeight - cloudSpawningHeight) / cloudWaitingSpeed;
		for (float t = 0; t < riseTime; t += Time.deltaTime) {
			cloudTrans.position += Vector3.up * cloudWaitingSpeed * Time.deltaTime;
			yield return null;
		}
	}
}