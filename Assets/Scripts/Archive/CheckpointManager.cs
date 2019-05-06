using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Archive
{
	public class CheckpointManager : MonoBehaviour
	{
		List<GameObject> checkpoints = new List<GameObject>();
		GameManager manager;

		GameObject[] players;
		Kevin[] playerScripts;
		LayerMask[] defaultCullingMasks;
		Text[] timerDisplays;
		float[] timers;
		int[] playerCheckpointIndex;

		public float startingTime = 30, secondsPerPassedCheckpoint = 3;

		[HideInInspector]
		public bool started = false;
		bool anyoneAlive = true;
		int[] checkpointsCleared;

		public void Init (GameManager manager, Kevin[] playerScripts)
		{
			this.manager = manager;
			this.playerScripts = playerScripts;
			players = new GameObject[2];
			for (int i = 0; i < 2; i++) {
				players[i] = playerScripts[i].gameObject;
			}

			//Haal alle checkpoints op
			for (int i = 0; i < transform.childCount; i++) {
				checkpoints.Add(transform.GetChild(i).gameObject);
				Checkpoint temp = checkpoints[i].AddComponent<Checkpoint>();
				temp.Init(this, i);
			}

			//Maak je arraytjes
			int numberOfPlayers = players.Length;
			playerCheckpointIndex = new int[numberOfPlayers];
			timerDisplays = new Text[numberOfPlayers];
			defaultCullingMasks = new LayerMask[numberOfPlayers];
			timers = new float[numberOfPlayers];
			checkpointsCleared = new int[numberOfPlayers];
			for (int i = 0; i < players.Length; i++) {
				playerCheckpointIndex[i] = 1;
				timerDisplays[i] = players[i].GetComponentInChildren<Text>();
				timers[i] = startingTime;
				checkpointsCleared[i] = 0;
			}
		}

		void Update ()
		{
			if (started && anyoneAlive) {
				for (int i = 0; i < players.Length; i++) {
					if (timers[i] > 0) {
						timers[i] -= Time.deltaTime;
						//playerScripts[i].alive = true;
					} else {
						//playerScripts[i].alive = false;
					}

					timerDisplays[i].text = Mathf.RoundToInt(timers[i]).ToString();
				}

				bool check = false;
				foreach (Kevin ps in playerScripts) {
					//if (ps.alive)
						check = true;
				}
				if (!check) {
					anyoneAlive = false;
					//manager.GameOver(true);
				}
			} else {
				bool check = false;
				foreach (Kevin ps in playerScripts) {
					//if (ps.alive)
						check = true;
				}
				if (check) {
					anyoneAlive = true;
					//manager.GameOver(false);
				}
			}
		}

		public void PlayerPassedCheckpoint (GameObject player, int checkpointIndex)
		{
			//Als de speler en de checkpointindex goeie zijn voeg tijd toe, ga naar volgende checkpointindex
			for (int i = 0; i < players.Length; i++) {
				if (player.name == players[i].name && checkpointIndex == playerCheckpointIndex[i]) {
					timers[i] += secondsPerPassedCheckpoint;
					checkpointsCleared[i]++;
					playerCheckpointIndex[i]++;
					//StartCoroutine(playerScripts[i].PassedCheckpoint(secondsPerPassedCheckpoint));
					if (playerCheckpointIndex[i] > checkpoints.Count - 1)
						playerCheckpointIndex[i] = 0;
				}
			}
		}
	}
}