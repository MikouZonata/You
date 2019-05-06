using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Utility;

public class KevinManager : MonoBehaviour
{
	public GameObject enemyDriverPrefab, pickupPrefab, pickupFeedbackPrefab;
	const int numberOfDrivers = 6;
	int numberOfAiDrivers, numberOfKevins;
	Kevin[] kevins;
	NavMeshAgent[] driverAgents;
	int[] driverTargets = new int[numberOfDrivers];

	Transform[] pickupPool;

	Transform[] trackPieces;

	Transform[] leaderboards;
	RectTransform[,] leaderboardCards;
	int minStartingScore = 34, maxStartingScore = 107;
	string[] driverNames = new string[] { "Tim", "Bosje", "Valentijn", "Herman", "Richard", "Bojan", "Arie", "Tuur", "Luan", "Earl", "Aran", "Micah" };
	int[] scores = new int[numberOfDrivers];
	int[] ranks;
	Text[,] scoreDisplays;
	Image[,] scoreHighlights;

	public void Init (Transform[] trackPieces, params Kevin[] kevins)
	{
		this.trackPieces = trackPieces;
		this.kevins = kevins;
		numberOfKevins = kevins.Length;
		numberOfAiDrivers = numberOfDrivers - numberOfKevins;

		//Maak alle pickups.
		pickupPool = new Transform[trackPieces.Length];
		GameObject pickupParent = new GameObject("PickupParent");
		for (int i = 0; i < pickupPool.Length; i++) {
			pickupPool[i] = Instantiate(pickupPrefab, trackPieces[i].position, Quaternion.identity, pickupParent.transform).transform;
			pickupPool[i].name = i.ToString();
			pickupPool[i].gameObject.SetActive(false);
		}

		//Instantiate alle drivers.
		driverAgents = new NavMeshAgent[numberOfAiDrivers];
		GameObject driverParent = new GameObject("DriverParent");
		for (int i = 0; i < numberOfAiDrivers; i++) {
			driverAgents[i] = Instantiate(enemyDriverPrefab, Util.PickRandom(trackPieces).position, Quaternion.identity, driverParent.transform).GetComponent<NavMeshAgent>();
			driverAgents[i].name = i.ToString();
		}

		//Geef alle kevins een naam die overeenkomt met hun driverIndex.
		for (int i = 0; i < numberOfKevins; i++) {
			kevins[i].name = (numberOfAiDrivers + i).ToString();
		}

		//Pickups for everyone.
		for (int i = 0; i < numberOfDrivers; i++) {
			AssignNewPickup(i, true);
		}

		//Setup stuff voor leaderboard.
		leaderboards = new Transform[numberOfKevins];
		leaderboardCards = new RectTransform[numberOfKevins, numberOfDrivers];
		scoreDisplays = new Text[numberOfKevins, numberOfDrivers];
		scoreHighlights = new Image[numberOfKevins, numberOfDrivers];
		ranks = new int[numberOfDrivers];
		string[] _driverNames = Util.PickRandom(numberOfAiDrivers, false, driverNames);

		//Vul het leaderboard met naampjes en stuff
		for (int kevin = 0; kevin < kevins.Length; kevin++) {
			leaderboards[kevin] = kevins[kevin].leaderboard;

			for (int driver = 0; driver < numberOfDrivers; driver++) {
				leaderboardCards[kevin, driver] = leaderboards[kevin].GetChild(driver).GetComponent<RectTransform>();
				scoreHighlights[kevin, driver] = leaderboardCards[kevin, driver].GetChild(2).GetComponent<Image>();
				scoreDisplays[kevin, driver] = leaderboardCards[kevin, driver].GetChild(3).GetComponent<Text>();

				if (driver < numberOfAiDrivers) {
					leaderboardCards[kevin, driver].GetChild(1).GetComponent<Text>().text = _driverNames[driver];
					scores[driver] = Random.Range(minStartingScore, maxStartingScore);
					scoreDisplays[kevin, driver].text = scores[driver].ToString();
				} else {
					leaderboardCards[kevin, driver].GetChild(1).GetComponent<Text>().text = "Kevin";
					scores[driver] = 0;
				}
			}
		}

		//Add 1 punt zodat het scoreboard zichzelf formateert.
		AddPointToScore(0);
	}

	void Update ()
	{
		//Check of een ai driver een pickup bereikt.
		for (int i = 0; i < numberOfAiDrivers; i++) {
			if ((driverAgents[i].transform.position - driverAgents[i].destination).sqrMagnitude < 1) {
				PickUpPickup(i, driverTargets[i]);
			}
		}
	}

	public void PickUpPickup (int driverIndex, int pickupIndex)
	{
		//Verwijder oude pickup.
		pickupPool[pickupIndex].gameObject.SetActive(false);
		GameObject feedback = Instantiate(pickupFeedbackPrefab, pickupPool[pickupIndex].position, Quaternion.identity);
		Destroy(feedback, 1);

		AddPointToScore(driverIndex);

		for (int i = 0; i < numberOfDrivers; i++) {
			if (driverTargets[i] == pickupIndex) {
				AssignNewPickup(i);
			}
		}
	}

	void AssignNewPickup (int driverIndex, bool firstTimeInitialization = false)
	{
		//Zet oude pickup op inactive en maak feedback.
		if (!firstTimeInitialization) {
			pickupPool[driverTargets[driverIndex]].gameObject.SetActive(false);
			GameObject feedback = Instantiate(pickupFeedbackPrefab, pickupPool[driverTargets[driverIndex]].position, Quaternion.identity);
			Destroy(feedback, 1);
		}

		//Vind nieuwe target in de pool.
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

		//Creeer een nieuwe lijst met gesorteerde scores.
		List<int> sortedScores = scores.ToList();
		sortedScores.Sort(new GFG());

		//Claimedranks houd bij welke ranks er al bezet zijn om meerdere drivers op dezelfde rank te voorkomen.
		List<int> claimedRanks = new List<int>();
		for (int driver = 0; driver < numberOfDrivers; driver++) {
			for (int rank = 0; rank < numberOfDrivers; rank++) {
				//Als hun score overeenkomt met een score in de gesorteerde lijst weet ik welke rank ze zouden moeten zijn. Claimedranks voorkomt duplicate ranks.
				if (scores[driver] == sortedScores[rank] && !claimedRanks.Contains(rank)) {
					ranks[driver] = rank;
					claimedRanks.Add(rank);
					break;
				}
			}
		}

		//Update leaderboardCards naar hun juiste posities en vul strings in
		float distanceBetweenCards = -80;
		for (int kevin = 0; kevin < numberOfKevins; kevin++) {
			for (int driver = 0; driver < numberOfDrivers; driver++) {
				leaderboardCards[kevin, driver].anchoredPosition = new Vector2(0, ranks[driver] * distanceBetweenCards);
				scoreDisplays[kevin, driver].text = scores[driver].ToString();
			}
		}
	}

	//Als iemand een puntje haalt licht hun score counter even op.
	IEnumerator HighlightScore (int driverIndex)
	{
		float highlightAlpha = 1, highlightTime = .7f;

		for (float t = 0; t < highlightTime * .5f; t += Time.deltaTime) {
			for (int kevin = 0; kevin < numberOfKevins; kevin++) {
				scoreHighlights[kevin, driverIndex].color = new Color(1, 1, 1, t * 2 / highlightTime * highlightAlpha);
			}
			yield return null;
		}
		for (float t = highlightTime * .5f; t > 0; t -= Time.deltaTime) {
			for (int kevin = 0; kevin < numberOfKevins; kevin++) {
				scoreHighlights[kevin, driverIndex].color = new Color(1, 1, 1, t * 2 / highlightTime * highlightAlpha);
			}
			yield return null;
		}

		for (int kevin = 0; kevin < numberOfKevins; kevin++) {
			scoreHighlights[kevin, driverIndex].color = new Color(1, 1, 1, 0);
		}
	}
}

//Gestolen van interwebs hihihihihihi.
class GFG : IComparer<int>
{
	public int Compare (int x, int y)
	{
		return y.CompareTo(x);
	}
}