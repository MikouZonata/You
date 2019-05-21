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
	public enum DisplayModes { TwoMonitors, SplitScreen };
	public DisplayModes displayMode = DisplayModes.TwoMonitors;
	public GameObject level;
	public GameObject kevinPrefab, maanPrefab;
	Kevin kevin;
	Maan maan;
	KevinManager kevinManager;
	MaanManager maanManager;
	Transform[] trackPieces;

	private void Awake ()
	{
		Transform trackPiecesParent = level.transform;
		trackPieces = new Transform[trackPiecesParent.childCount - 1];
		for (int i = 0; i < trackPieces.Length; i++) {
			trackPieces[i] = trackPiecesParent.GetChild(i + 1);
		}

		maan = (Instantiate(maanPrefab, Vector3.left, Quaternion.identity).GetComponent<Maan>());
		maan.playerIndex = (PlayerIndex) 0;
		kevin = (Instantiate(kevinPrefab, Vector3.right, Quaternion.identity).GetComponent<Kevin>());
		kevin.playerIndex = (PlayerIndex) 1;

		Camera[] maansCameras = maan.GetComponentsInChildren<Camera>();
		Camera[] kevinsCameras = kevin.GetComponentsInChildren<Camera>();
		switch (displayMode) {
			case DisplayModes.TwoMonitors:
				foreach (Camera c in maansCameras) {
					c.rect = new Rect(0, 0, 1, 1);
					c.targetDisplay = 0;
				}
				foreach (Camera c in kevinsCameras) {
					c.rect = new Rect(0, 0, 1, 1);
					c.targetDisplay = 1;
				}
				break;
			case DisplayModes.SplitScreen:
				foreach (Camera c in maansCameras) {
					c.rect = new Rect(0, 0.5f, 1, .5f);
					c.targetDisplay = 0;
				}
				foreach (Camera c in kevinsCameras) {
					c.rect = new Rect(0, 0, 1, .5f);
					c.targetDisplay = 0;
				}
				break;
		}

		maanManager = GetComponent<MaanManager>();
		kevinManager = GetComponent<KevinManager>();

		maan.Init(maanManager, kevin.transform);
		kevin.Init(kevinManager, maan.transform);

		maanManager.Init(trackPieces, maan);
		kevinManager.Init(trackPieces, kevin);
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