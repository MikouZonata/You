using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using XInputDotNetPure;
using Utility;

public class GameManager : MonoBehaviour
{
	public GameObject[] levels;
	public GameObject kevinPrefab, maanPrefab;
	Kevin[] kevins = new Kevin[2];
	Maan[] maans = new Maan[2];
	KevinManager kevinManager;
	MaanManager maanManager;
	Transform[] trackPieces;

	private void Awake ()
	{
		//find the correct set of players and initialize them
		for (int i = 0; i < 2; i++) {
			if (StaticData.playerOptions[i] == StaticData.PlayerOptions.Kevin) {
				kevins[i] = (Instantiate(kevinPrefab, new Vector3(-2 + 4 * i, 0.1f, 0), Quaternion.identity).GetComponent<Kevin>());
				kevins[i].playerIndex = (PlayerIndex) i;
				foreach (Camera c in kevins[i].transform.GetComponentsInChildren<Camera>()) {
					c.rect = new Rect(0, 0.5f - .5f * i, 1, .5f);
				}
			} else {
				maans[i] = (Instantiate(maanPrefab, new Vector3(-2 + 4 * i, 0.1f, 0), Quaternion.identity).GetComponent<Maan>());
				maans[i].playerIndex = (PlayerIndex) i;
				foreach (Camera c in maans[i].transform.GetComponentsInChildren<Camera>()) {
					c.rect = new Rect(0, 0.5f - .5f * i, 1, .5f);
				}
			}
		}

		kevinManager = GetComponent<KevinManager>();
		maanManager = GetComponent<MaanManager>();
		for (int i = 0; i < 2; i++) {
			if (StaticData.playerOptions[i] == StaticData.PlayerOptions.Kevin) {
				if (StaticData.playerOptions[Util.InvertZeroAndOne(i)] == StaticData.PlayerOptions.Kevin) {
					kevins[i].Init(kevinManager, kevins[Util.InvertZeroAndOne(i)].transform);
				} else {
					kevins[i].Init(kevinManager, maans[Util.InvertZeroAndOne(i)].transform);
				}
			} else {
				if (StaticData.playerOptions[Util.InvertZeroAndOne(i)] == StaticData.PlayerOptions.Kevin) {
					maans[i].Init(maanManager, kevins[Util.InvertZeroAndOne(i)].transform);
				} else {
					maans[i].Init(maanManager, maans[Util.InvertZeroAndOne(i)].transform);
				}
			}
		}

		//Enable correct level
		for (int i = 0; i < levels.Length; i++) {
			if (StaticData.levelNumber != i) {
				levels[i].SetActive(false);
			} else {
				levels[i].SetActive(true);
			}
		}

		Transform trackPiecesParent = levels[StaticData.levelNumber].transform;
		trackPieces = new Transform[trackPiecesParent.childCount - 1];
		for (int i = 0; i < trackPieces.Length; i++) {
			trackPieces[i] = trackPiecesParent.GetChild(i + 1);
		}

		List<Kevin> kl = new List<Kevin>();
		for (int i = 0; i < 2; i++) {
			if (kevins[i] != null) {
				kl.Add(kevins[i]);
			}
		}
		kevinManager.Init(trackPieces, kl.ToArray());

		List<Maan> ml = new List<Maan>();
		for (int i = 0; i < 2; i++) {
			if (maans[i] != null) {
				ml.Add(maans[i]);
			}
		}
		maanManager.Init(trackPieces, ml.ToArray());
	}

	private void Update ()
	{
		GamePadState[] gamePadStates = new GamePadState[4];
		for (int i = 0; i < gamePadStates.Length; i++) {
			gamePadStates[i] = GamePad.GetState((PlayerIndex) i);
		}

		//foreach (GamePadState gps in gamePadStates) {
		//	if (gps.Buttons.Back == ButtonState.Pressed) {
		//		SceneManager.LoadScene(0);
		//	}
		//}
	}
}