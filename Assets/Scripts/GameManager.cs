using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using XInputDotNetPure;
using Utility;
using FMODUnity;

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

	//FMOD
	string fmodLinkedPath = "event:/Linked_Up";
	FMOD.Studio.EventInstance fmodLinkedInstance;
	bool fmodLinkedPlaying = false;

	private void Awake ()
	{
		Cursor.visible = false;

		Transform trackPiecesParent = level.transform;
		trackPieces = new Transform[trackPiecesParent.childCount];
		for (int i = 0; i < trackPieces.Length; i++) {
			trackPieces[i] = trackPiecesParent.GetChild(i);
		}

		maan = (Instantiate(maanPrefab, new Vector3(-1, .05f, 0), Quaternion.identity).GetComponent<Maan>());
		maan.playerIndex = (PlayerIndex) 0;
		StaticData.playerTransforms[0] = maan.transform;
		kevin = (Instantiate(kevinPrefab, new Vector3(1, .05f, 0), Quaternion.identity).GetComponent<Kevin>());
		kevin.playerIndex = (PlayerIndex) 1;
		StaticData.playerTransforms[1] = kevin.transform;

		Camera[] maansCameras = maan.GetComponentsInChildren<Camera>();
		Camera[] kevinsCameras = kevin.GetComponentsInChildren<Camera>();
		Display.displays[0].Activate();
		switch (displayMode) {
			case DisplayModes.TwoMonitors:
				if (Display.displays.Length > 1) {
					Display.displays[1].Activate();
				}
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

		fmodLinkedInstance = RuntimeManager.CreateInstance(fmodLinkedPath);
	}

	private void Update ()
	{
		GamePadState[] gamePadStates = new GamePadState[4];
		for (int i = 0; i < gamePadStates.Length; i++) {
			gamePadStates[i] = GamePad.GetState((PlayerIndex) i);
		}

		if (Vector3.Distance(kevin.transform.position, maan.transform.position) <= StaticData.distanceToLink) {
			StaticData.playersAreLinked = true;
		} else {
			StaticData.playersAreLinked = false;
		}

		if (fmodLinkedPlaying && !StaticData.playersAreLinked) {
			fmodLinkedPlaying = false;
			fmodLinkedInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
		if (!fmodLinkedPlaying && StaticData.playersAreLinked) {
			fmodLinkedPlaying = true;
			fmodLinkedInstance.start();
		}
	}
}