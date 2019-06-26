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
	Transform pickupFeedbackParent;
	const int numberOfDrivers = 6;
	int numberOfAiDrivers;
	Kevin kevin;
	NavMeshAgent[] driverAgents;
	int[] driverTargets = new int[numberOfDrivers];

	Transform[] pickupPool;

	Transform[] trackPieces;

	Transform leaderboard;
	RectTransform[] leaderboardCards;
	string[] driverNames = new string[] { "Daniel", "Lenny", "Tim", "Valentijn", "Richard" };
	int[] driverStartingScores = new int[] { 52, 46, 38, 31, 21 };
	float[] driverBaseSpeeds = new float[] { 16.2f, 14.8f, 11f, 9.1f, 7.6f };
	int[] scores;
	int[] ranks;
	Text[][] scoreDisplays;
	Image[] scoreHighlights;

	float[] scoreDownTimers = new float[numberOfDrivers];
	float scoreDownBaseTime = 12, scoreDownTimePerPoint = .075f;

	float driverSpeedFluxMax = 1.2f;
	float[] driverSpeedFluxTimers;
	float driverSpeedFluxWavelength = 50;

	GameObject[] pickupFeedbackPool = new GameObject[numberOfDrivers];

	//FMOD
	string fmodKevinPickupEvent = "event:/Kevin/Pick-up";
	string fmodDriverPickupEvent = "event:/Kevin/Pick-up_Opponent";
	FMOD.Studio.EventInstance fmodPickupInstance;

	public void Init (Transform[] trackPieces, Kevin kevin)
	{
		this.trackPieces = trackPieces;
		this.kevin = kevin;

		if (firstTimeSetup) {
			numberOfAiDrivers = numberOfDrivers - 1;

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

			//Geef Kevin een naam die overeenkomt met zijn driverIndex
			kevin.name = 5.ToString();

			//Setup stuff voor leaderboard
			leaderboardCards = new RectTransform[numberOfDrivers];
			scoreDisplays = new Text[numberOfDrivers][];
			scoreHighlights = new Image[numberOfDrivers];
			ranks = new int[numberOfDrivers];

			//Setup ai driver speed flux
			driverSpeedFluxTimers = new float[numberOfAiDrivers];
			for (int i = 0; i < numberOfAiDrivers; i++) {
				driverSpeedFluxTimers[i] = Random.Range(0, driverSpeedFluxWavelength);
			}

			firstTimeSetup = false;
		}


		//Instantiate alle drivers
		driverAgents = new NavMeshAgent[numberOfAiDrivers];
		GameObject driverParent = new GameObject("DriverParent");
		for (int i = 0; i < numberOfAiDrivers; i++) {
			driverAgents[i] = Instantiate(Util.PickRandom(enemyDriverPrefabs), Util.PickRandom(trackPieces).position, Quaternion.identity, driverParent.transform).GetComponent<NavMeshAgent>();
			driverAgents[i].name = i.ToString();
			driverAgents[i].speed = driverBaseSpeeds[i];
		}

		//Pickups for everyone
		for (int i = 0; i < numberOfDrivers; i++) {
			AssignNewPickup(i, true);
		}

		//Vul het leaderboard met naampjes en stuff
		leaderboard = kevin.leaderboard;
		scores = new int[numberOfDrivers];
		for (int i = 0; i < scoreDownTimers.Length; i++) {
			scoreDownTimers[i] = 0;
		}

		for (int driver = 0; driver < numberOfDrivers; driver++) {
			leaderboardCards[driver] = leaderboard.GetChild(driver).GetComponent<RectTransform>();
			scoreHighlights[driver] = leaderboardCards[driver].GetComponent<LeaderboardCard>().highlightImage;
			scoreDisplays[driver] = new Text[3];
			for (int i = 0; i < 3; i++) {
				scoreDisplays[driver][i] = leaderboardCards[driver].GetComponent<LeaderboardCard>().scoreTexts[i];
			}

			if (driver < numberOfAiDrivers) {
				scores[driver] = driverStartingScores[driver];
				foreach (Text t in scoreDisplays[driver]) {
					t.text = scores[driver].ToString();
				}
			} else {
				scores[driver] = 0;
			}
		}

		//Add 1 punt zodat het scoreboard zichzelf formateert.
		AddPointToScore(0);
	}

	void Update ()
	{
		//Check of een ai driver een pickup bereikt.
		for (int i = 0; i < numberOfAiDrivers; i++) {
			if ((driverAgents[i].transform.position - driverAgents[i].destination).sqrMagnitude < 1.5f) {
				PickUpPickup(i, driverTargets[i]);
			}
		}

		ScoreDownTimers();
		DriverSpeedFlux();
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
					StartCoroutine(PickupFeedbackRoutine(feedbackGO, driverAgents[driverIndex].transform));
					if (Vector3.Distance(kevin.transform.position, driverAgents[driverIndex].transform.position) < 45) {
						fmodPickupInstance = RuntimeManager.CreateInstance(fmodDriverPickupEvent);
						fmodPickupInstance.start();
					}
				} else {
					StartCoroutine(PickupFeedbackRoutine(feedbackGO, kevin.transform));
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
			driverAgents[driverIndex].destination = pickupPool[targetPickupIndex].position;
	}

	void AddPointToScore (int driverIndex, int score = 1)
	{
		scores[driverIndex] += score;
		if (scores[driverIndex] < 0)
			scores[driverIndex] = 0;
		scoreDownTimers[driverIndex] = 0;
		if (score > 0) {
			StartCoroutine(HighlightScore(driverIndex));
		}

		//Creeer een nieuwe lijst met gesorteerde scores
		List<int> sortedScores = scores.ToList();
		sortedScores.Sort(new GFG());

		//Claimedranks houd bij welke ranks er al bezet zijn om meerdere drivers op dezelfde rank te voorkomen
		List<int> claimedRanks = new List<int>();
		for (int driver = 0; driver < numberOfDrivers; driver++) {
			for (int rank = 0; rank < numberOfDrivers; rank++) {
				//Als hun score overeenkomt met een score in de gesorteerde lijst weet ik welke rank ze zouden moeten zijn. Claimedranks voorkomt duplicate ranks
				if (scores[driver] == sortedScores[rank] && !claimedRanks.Contains(rank)) {
					ranks[driver] = rank;
					claimedRanks.Add(rank);
					break;
				}
			}
		}

		//Update leaderboardCards naar hun juiste posities en vul strings in
		float distanceBetweenCards = -52, rankTwoBasePosition = -8;
		for (int driver = 0; driver < numberOfDrivers; driver++) {
			if (ranks[driver] == 0) {
				leaderboardCards[driver].anchoredPosition = new Vector2(0, 0);
			} else {
				leaderboardCards[driver].anchoredPosition = new Vector2(0, rankTwoBasePosition + ranks[driver] * distanceBetweenCards);
			}

			foreach (Text t in scoreDisplays[driver]) {
				t.text = scores[driver].ToString();
			}
		}
	}

	void ScoreDownTimers ()
	{
		for (int i = 0; i < numberOfDrivers; i++) {
			scoreDownTimers[i] += Time.deltaTime;

			if (scoreDownTimers[i] >= scoreDownBaseTime - scores[i] * scoreDownTimePerPoint) {
				AddPointToScore(i, -1);
				scoreDownTimers[i] = 0;
			}
		}
	}

	void DriverSpeedFlux ()
	{
		for (int i = 0; i < numberOfAiDrivers; i++) {
			driverSpeedFluxTimers[i] += Time.deltaTime;
			driverAgents[i].speed = driverBaseSpeeds[i] + Mathf.Sin(driverSpeedFluxTimers[i] * (Mathf.PI * 2) / driverSpeedFluxWavelength) * driverSpeedFluxMax;
		}
	}

	//Als iemand een puntje haalt licht hun score counter even op
	IEnumerator HighlightScore (int driverIndex)
	{
		float highlightAlpha = 1, highlightTime = .7f;

		for (float t = 0; t < highlightTime * .5f; t += Time.deltaTime) {
			scoreHighlights[driverIndex].color = new Color(1, 1, 1, t * 2 / highlightTime * highlightAlpha);
			yield return null;
		}
		for (float t = highlightTime * .5f; t > 0; t -= Time.deltaTime) {
			scoreHighlights[driverIndex].color = new Color(1, 1, 1, t * 2 / highlightTime * highlightAlpha);
			yield return null;
		}

		scoreHighlights[driverIndex].color = new Color(1, 1, 1, 0);
	}

	public void Reset ()
	{
		foreach (NavMeshAgent agent in driverAgents) {
			Destroy(agent.gameObject);
		}
		foreach (Transform t in pickupPool) {
			t.gameObject.SetActive(false);
		}

		//kevin.transform.position = kevinSpawnPosition;
		kevin.transform.rotation = Quaternion.identity;
		Init(trackPieces, kevin);
	}
}

//Gestolen van interwebs hihihihihihi
class GFG : IComparer<int>
{
	public int Compare (int x, int y)
	{
		return y.CompareTo(x);
	}
}