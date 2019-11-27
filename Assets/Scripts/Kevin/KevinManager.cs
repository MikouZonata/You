using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Utility;
using FMODUnity;

public class KevinManager : MonoBehaviour
{
	bool firstTimeSetup = true;

	public GameObject[] enemyDriverPrefabs;
	public GameObject pickupPrefab, pickupFeedbackPrefab;
	const int numberOfDrivers = 6;
	const int numberOfAiDrivers = 5;
	Transform pickupFeedbackParent;
	int[] driverTargets = new int[numberOfDrivers];
	Kevin _kevin;
	Transform driverParent;
	NavMeshAgent[] _driverAgents;

	Transform[] pickupPool;

	Transform[] trackPieces;

	Transform _leaderboard;
	RectTransform[] leaderboardCards;
	string[] driverNames = new string[] { "Daniel", "Lenny", "Tim", "Valentijn", "Richard" };
	int[] driverStartingScores = new int[] { 52, 44, 36, 17, 8 };
	float[] driverBaseSpeeds = new float[] { 16.3f, 14.9f, 10.6f, 9.6f, 8.3f };
	Color scoreHightlightColor;
	int[] _scores;
	int[] _ranks;
	Text[][] _scoreDisplays;
	Image[] _scoreHighlights;

	const float scoreDownBaseTime = 12, scoreDownTimePerPoint = .075f;
	float[] _scoreDownTimers = new float[numberOfDrivers];

	const float driverSpeedFluxMax = 1.1f;
	const float driverSpeedFluxWavelength = 50;
	float[] _driverSpeedFluxTimers;

	GameObject[] pickupFeedbackPool = new GameObject[numberOfDrivers];

	//FMOD
	const string fmodKevinPickupEvent = "event:/Kevin/Pick-up";
	const string fmodDriverPickupEvent = "event:/Kevin/Pick-up_Opponent";
	FMOD.Studio.EventInstance fmodPickupInstance;

	public void Init (Transform[] trackPieces, Kevin _kevin)
	{
		this.trackPieces = trackPieces;
		this._kevin = _kevin;

		if (firstTimeSetup) {
			//Maak alle pickups
			pickupPool = new Transform[trackPieces.Length];
			GameObject pickupParent = new GameObject("PickupParent");
			for (int i = 0; i < pickupPool.Length; i++) {
				pickupPool[i] = Instantiate(pickupPrefab, trackPieces[i].position + Vector3.up * .5f, Quaternion.identity, pickupParent.transform).transform;
				pickupPool[i].name = i.ToString();
				pickupPool[i].gameObject.SetActive(false);
			}

			//Pickup feedback pool maken
			pickupFeedbackParent = new GameObject("PickupFeedbackParent").transform;
			for (int i = 0; i < pickupFeedbackPool.Length; i++) {
				pickupFeedbackPool[i] = Instantiate(pickupFeedbackPrefab);
				pickupFeedbackPool[i].transform.parent = pickupFeedbackParent;
				pickupFeedbackPool[i].SetActive(false);
			}

			//Setup stuff voor leaderboard
			leaderboardCards = new RectTransform[numberOfDrivers];
			_scoreDisplays = new Text[numberOfDrivers][];
			_scoreHighlights = new Image[numberOfDrivers];
			_ranks = new int[numberOfDrivers];

			//Setup een parent voor de drivers en ai driver speed flux
			driverParent = new GameObject("DriverParent").transform;
			_driverSpeedFluxTimers = new float[numberOfAiDrivers];
			for (int i = 0; i < numberOfAiDrivers; i++) {
				_driverSpeedFluxTimers[i] = Random.Range(0, driverSpeedFluxWavelength);
			}

			firstTimeSetup = false;
		}

		//Instantiate alle drivers
		_driverAgents = new NavMeshAgent[numberOfAiDrivers];
		for (int i = 0; i < numberOfAiDrivers; i++) {
			_driverAgents[i] = Instantiate(Util.PickRandom(enemyDriverPrefabs), Util.PickRandom(trackPieces).position, Quaternion.identity, driverParent.transform).GetComponent<NavMeshAgent>();
			_driverAgents[i].name = i.ToString();
			_driverAgents[i].speed = driverBaseSpeeds[i];
		}

		//Pickups for everyone
		for (int i = 0; i < numberOfDrivers; i++) {
			AssignNewPickup(i, true);
		}

		//Geef Kevin een naam die overeenkomt met zijn driverIndex
		_kevin.name = "5";

		//Vul het leaderboard met naampjes en stuff
		_leaderboard = _kevin.leaderboardFrame;
		_scores = new int[numberOfDrivers];
		for (int i = 0; i < _scoreDownTimers.Length; i++) {
			_scoreDownTimers[i] = 0;
		}

		for (int driver = 0; driver < numberOfDrivers; driver++) {
			leaderboardCards[driver] = _leaderboard.GetChild(driver).GetComponent<RectTransform>();
			_scoreHighlights[driver] = leaderboardCards[driver].GetComponent<LeaderboardCard>().highlightImage;
			_scoreDisplays[driver] = new Text[3];
			for (int i = 0; i < 3; i++) {
				_scoreDisplays[driver][i] = leaderboardCards[driver].GetComponent<LeaderboardCard>().scoreTexts[i];
			}

			if (driver < numberOfAiDrivers) {
				_scores[driver] = driverStartingScores[driver];
				foreach (Text t in _scoreDisplays[driver]) {
					t.text = _scores[driver].ToString();
				}
			} else {
				_scores[driver] = 0;
			}
		}
		scoreHightlightColor = _scoreHighlights[0].color;

		//Add 1 punt zodat het scoreboard zichzelf formateert.
		AddPointToScore(0);
	}

	public void Deactivate ()
	{
		StopAllCoroutines();

		foreach (NavMeshAgent agent in _driverAgents) {
			Destroy(agent.gameObject);
		}
		foreach (Transform t in pickupPool) {
			t.gameObject.SetActive(false);
		}

		fmodPickupInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

		_kevin.Destroy();
	}

	void Update ()
	{
		if (!StaticData.menuActive) {
			//Check of een ai driver een pickup bereikt.
			for (int i = 0; i < numberOfAiDrivers; i++) {
				if ((_driverAgents[i].transform.position - _driverAgents[i].destination).sqrMagnitude < 1.5f) {
					PickUpPickup(i, driverTargets[i]);
				}
			}

			ScoreDownTimers();
			DriverSpeedFlux();
		}
	}



	public void PickUpPickup (int driverIndex, int pickupIndex)
	{
		//Verwijder oude pickup.
		pickupPool[pickupIndex].gameObject.SetActive(false);

		foreach (GameObject feedbackGO in pickupFeedbackPool) {
			if (!feedbackGO.activeSelf) {
				feedbackGO.transform.position = pickupPool[pickupIndex].position;
				feedbackGO.SetActive(true);
				if (driverIndex != 5) {
					StartCoroutine(PickupFeedbackRoutine(feedbackGO, _driverAgents[driverIndex].transform));
					if (Vector3.Distance(_kevin.transform.position, _driverAgents[driverIndex].transform.position) < 45) {
						fmodPickupInstance = RuntimeManager.CreateInstance(fmodDriverPickupEvent);
						fmodPickupInstance.start();
					}
				} else {
					StartCoroutine(PickupFeedbackRoutine(feedbackGO, _kevin.transform));
					fmodPickupInstance = RuntimeManager.CreateInstance(fmodKevinPickupEvent);
					fmodPickupInstance.start();
				}
				break;
			}
		}

		AddPointToScore(driverIndex);

		for (int i = 0; i < numberOfDrivers; i++) {
			if (driverTargets[i] == pickupIndex) {
				AssignNewPickup(i);
			}
		}
	}

	IEnumerator PickupFeedbackRoutine (GameObject feedbackGO, Transform followTrans)
	{
		AnimatorStateInfo _animStateInfo;
		Animator animator = feedbackGO.GetComponent<Animator>();
		animator.enabled = true;
		animator.Play("PickUpFX", 0, 0);

		for (float t = 0; t < 3; t += Time.deltaTime) {
			feedbackGO.transform.position = followTrans.position + Vector3.up;

			if (animator.enabled) {
				_animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
				if (!_animStateInfo.IsName("PickUpFX")) {
					animator.enabled = false;
				}
			}

			yield return null;
		}

		feedbackGO.SetActive(false);
	}

	void AssignNewPickup (int driverIndex, bool firstTimeInitialization = false)
	{
		//Zet oude pickup op inactive en maak feedback
		if (!firstTimeInitialization) {
			pickupPool[driverTargets[driverIndex]].gameObject.SetActive(false);
		}

		//Vind nieuwe target in de pool
		int targetPickupIndex = Random.Range(0, pickupPool.Length);
		while (true) {
			if (!pickupPool[targetPickupIndex].gameObject.activeSelf) {
				break;
			} else {
				targetPickupIndex = Random.Range(0, pickupPool.Length);
			}
		}

		//Set nieuwe target en, if ai, maak dat ook de nav agent's destination
		driverTargets[driverIndex] = targetPickupIndex;
		pickupPool[driverTargets[driverIndex]].gameObject.SetActive(true);
		if (driverIndex < numberOfAiDrivers)
			_driverAgents[driverIndex].destination = pickupPool[targetPickupIndex].position;
	}

	void AddPointToScore (int driverIndex, int score = 1)
	{
		_scores[driverIndex] += score;
		if (_scores[driverIndex] < 0)
			_scores[driverIndex] = 0;
		_scoreDownTimers[driverIndex] = 0;
		if (score > 0) {
			StartCoroutine(HighlightScore(driverIndex));
		}

		//Creeer een nieuwe lijst met gesorteerde scores
		List<int> sortedScores = _scores.ToList();
		sortedScores.Sort(new GFG());

		//Claimedranks houd bij welke ranks er al bezet zijn om meerdere drivers op dezelfde rank te voorkomen
		List<int> claimedRanks = new List<int>();
		for (int driver = 0; driver < numberOfDrivers; driver++) {
			for (int rank = 0; rank < numberOfDrivers; rank++) {
				//Als hun score overeenkomt met een score in de gesorteerde lijst weet ik welke rank ze zouden moeten zijn. Claimedranks voorkomt duplicate ranks
				if (_scores[driver] == sortedScores[rank] && !claimedRanks.Contains(rank)) {
					_ranks[driver] = rank;
					claimedRanks.Add(rank);
					break;
				}
			}
		}

		//Update leaderboardCards naar hun juiste posities en vul strings in
		float distanceBetweenCards = -56, rankTwoBasePosition = -24;
		for (int driver = 0; driver < numberOfDrivers; driver++) {
			if (_ranks[driver] == 0) {
				leaderboardCards[driver].anchoredPosition = new Vector2(0, 0);
			} else {
				leaderboardCards[driver].anchoredPosition = new Vector2(0, rankTwoBasePosition + _ranks[driver] * distanceBetweenCards);
			}

			foreach (Text t in _scoreDisplays[driver]) {
				t.text = _scores[driver].ToString();
			}
		}
	}

	void ScoreDownTimers ()
	{
		for (int i = 0; i < numberOfDrivers; i++) {
			_scoreDownTimers[i] += Time.deltaTime;

			if (_scoreDownTimers[i] >= scoreDownBaseTime - _scores[i] * scoreDownTimePerPoint) {
				AddPointToScore(i, -1);
				_scoreDownTimers[i] = 0;
			}
		}
	}

	void DriverSpeedFlux ()
	{
		for (int i = 0; i < numberOfAiDrivers; i++) {
			_driverSpeedFluxTimers[i] += Time.deltaTime;
			_driverAgents[i].speed = driverBaseSpeeds[i] + Mathf.Sin(_driverSpeedFluxTimers[i] * (Mathf.PI * 2) / driverSpeedFluxWavelength) * driverSpeedFluxMax;
		}
	}

	//Als iemand een puntje haalt licht hun score counter even op
	IEnumerator HighlightScore (int driverIndex)
	{
		float highlightAlpha = .8f, highlightTime = .9f;

		for (float t = 0; t < highlightTime * .5f; t += Time.deltaTime) {
			_scoreHighlights[driverIndex].color = new Color(scoreHightlightColor.r, scoreHightlightColor.g, scoreHightlightColor.b, t * 2 / highlightTime * highlightAlpha);
			yield return null;
		}
		for (float t = highlightTime * .5f; t > 0; t -= Time.deltaTime) {
			_scoreHighlights[driverIndex].color = new Color(scoreHightlightColor.r, scoreHightlightColor.g, scoreHightlightColor.b, t * 2 / highlightTime * highlightAlpha);
			yield return null;
		}

		_scoreHighlights[driverIndex].color = new Color(scoreHightlightColor.r, scoreHightlightColor.g, scoreHightlightColor.b, 0);
	}

	public int GetKevinRank ()
	{
		return _ranks[5];
	}
}

class GFG : IComparer<int>
{
	public int Compare (int x, int y)
	{
		return y.CompareTo(x);
	}
}