using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Utility;

public class KevinManager : MonoBehaviour
{
	bool firstTimeSetup = true;
	public GameObject enemyDriverPrefab, pickupPrefab, pickupFeedbackPrefab;
	const int numberOfDrivers = 6;
	int numberOfAiDrivers;
	Kevin kevin;
	NavMeshAgent[] driverAgents;
	int[] driverTargets = new int[numberOfDrivers];

	Transform[] pickupPool;

	Transform[] trackPieces;

	Transform leaderboard;
	RectTransform[] leaderboardCards;
	int minStartingScore = 34, maxStartingScore = 107;
	string[] driverNames = new string[] { "Twem", "Bosje", "Valentijn", "Herman", "Richard", "Bojan", "Arie", "Tuur", "Luan", "Earl", "Aran", "Micah", "Tijmen", "Lenny", "Romy", "Elmar", "Larissa", "Eva", "Robert" };
	string[] shuffledDriverNames;
	int driverNamesIndex = 0;
	int[] scores;
	int[] ranks;
	Text[] scoreDisplays;
	Image[] scoreHighlights;

	GameObject[] pickupFeedbackPool = new GameObject[numberOfDrivers];

	Vector3 kevinSpawnPosition;

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
				pickupPool[i] = Instantiate(pickupPrefab, trackPieces[i].position, Quaternion.identity, pickupParent.transform).transform;
				pickupPool[i].name = i.ToString();
				pickupPool[i].gameObject.SetActive(false);
			}

			for (int i = 0; i < pickupFeedbackPool.Length; i++) {
				pickupFeedbackPool[i] = Instantiate(pickupFeedbackPrefab);
				pickupFeedbackPool[i].SetActive(false);
			}
			firstTimeSetup = false;
		}


		//Instantiate alle drivers
		driverAgents = new NavMeshAgent[numberOfAiDrivers];
		GameObject driverParent = new GameObject("DriverParent");
		for (int i = 0; i < numberOfAiDrivers; i++) {
			driverAgents[i] = Instantiate(enemyDriverPrefab, Util.PickRandom(trackPieces).position, Quaternion.identity, driverParent.transform).GetComponent<NavMeshAgent>();
			driverAgents[i].name = i.ToString();
		}

		//Geef Kevin een naam die overeenkomt met zijn driverIndex
		kevin.name = 5.ToString();

		//Pickups for everyone
		for (int i = 0; i < numberOfDrivers; i++) {
			AssignNewPickup(i, true);
		}

		//Setup stuff voor leaderboard
		leaderboardCards = new RectTransform[numberOfDrivers];
		scoreDisplays = new Text[numberOfDrivers];
		scoreHighlights = new Image[numberOfDrivers];
		ranks = new int[numberOfDrivers];

		//Vul het leaderboard met naampjes en stuff
		leaderboard = kevin.leaderboard;
		shuffledDriverNames = Util.Shuffle(driverNames);
		scores = new int[numberOfDrivers];

		for (int driver = 0; driver < numberOfDrivers; driver++) {
			leaderboardCards[driver] = leaderboard.GetChild(driver).GetComponent<RectTransform>();
			scoreHighlights[driver] = leaderboardCards[driver].GetChild(2).GetComponent<Image>();
			scoreDisplays[driver] = leaderboardCards[driver].GetChild(3).GetComponent<Text>();

			if (driver < numberOfAiDrivers) {
				leaderboardCards[driver].GetChild(1).GetComponent<Text>().text = shuffledDriverNames[driverNamesIndex];
				driverNamesIndex++;
				if (driverNamesIndex >= shuffledDriverNames.Length)
					driverNamesIndex = 0;
				scores[driver] = Random.Range(minStartingScore, maxStartingScore);
				scoreDisplays[driver].text = scores[driver].ToString();
			} else {
				leaderboardCards[driver].GetChild(1).GetComponent<Text>().text = "Kevin";
				scores[driver] = 0;
			}
		}

		//Add 1 punt zodat het scoreboard zichzelf formateert.
		AddPointToScore(0);

		kevinSpawnPosition = kevin.transform.position;
	}

	void Update ()
	{
		//Check of een ai driver een pickup bereikt.
		for (int i = 0; i < numberOfAiDrivers; i++) {
			if ((driverAgents[i].transform.position - driverAgents[i].destination).sqrMagnitude < 1.5f) {
				PickUpPickup(i, driverTargets[i]);
			}
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
				StartCoroutine(SetActiveWithDelay(feedbackGO, false, 1));
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

	void AddPointToScore (int driverIndex)
	{
		scores[driverIndex]++;
		StartCoroutine(HighlightScore(driverIndex));

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
		float distanceBetweenCards = -116;
		for (int driver = 0; driver < numberOfDrivers; driver++) {
			leaderboardCards[driver].anchoredPosition = new Vector2(0, ranks[driver] * distanceBetweenCards);
			scoreDisplays[driver].text = scores[driver].ToString();
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

	IEnumerator SetActiveWithDelay (GameObject gameObject, bool state, float delay)
	{
		yield return new WaitForSeconds(delay);
		gameObject.SetActive(state);
	}

	public void Reset ()
	{
		foreach (NavMeshAgent agent in driverAgents) {
			Destroy(agent.gameObject);
		}
		foreach (Transform t in pickupPool) {
			Destroy(t.gameObject);
		}

		kevin.transform.position = kevinSpawnPosition;
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