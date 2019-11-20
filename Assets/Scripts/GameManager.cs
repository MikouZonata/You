using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using XInputDotNetExtended;
using Utility;
using FMODUnity;

public class GameManager : MonoBehaviour
{
	public GameObject level;
	public GameObject kevinPrefab, maanPrefab;
	bool _firstTimeInit = true;
	Kevin kevin;
	Maan maan;
	KevinManager kevinManager;
	MaanManager maanManager;
	MenuManager menuManager;
	Transform[] trackPieces;

	const int secondsBeforeDeactivation = 90;
	float _deactivationTimer = 0;

	const int secondsBeforeFirstCloud = 180;
	const int secondsBeforeFatigue = 180;
	float _pacingTimer = 0;

	//FMOD
	string fmodLinkedPath = "event:/Linked_Up";
	FMOD.Studio.EventInstance fmodLinkedInstance;
	bool fmodLinkedPlaying = false;

	void Awake ()
	{
		Init();
	}

	void Init ()
	{
		if (_firstTimeInit) {
			Display.displays[0].Activate();
#if UNITY_EDITOR
#else
			if (Display.displays.Length > 0) {
				Display.displays[1].Activate();
			}
#endif
			Cursor.visible = false;

			Transform trackPiecesParent = level.transform;
			trackPieces = new Transform[trackPiecesParent.childCount];
			for (int i = 0; i < trackPieces.Length; i++) {
				trackPieces[i] = trackPiecesParent.GetChild(i);
			}

			maanManager = GetComponent<MaanManager>();
			kevinManager = GetComponent<KevinManager>();
			menuManager = GetComponent<MenuManager>();

			_firstTimeInit = false;
		}

		maan = (Instantiate(maanPrefab, new Vector3(-1, .05f, 0), Quaternion.identity).GetComponent<Maan>());
		maan.playerIndex = (XInputDotNetPure.PlayerIndex) 0;
		StaticData.playerTransforms[0] = maan.transform;
		kevin = (Instantiate(kevinPrefab, new Vector3(1, .05f, 0), Quaternion.identity).GetComponent<Kevin>());
		kevin.playerIndex = (XInputDotNetPure.PlayerIndex) 1;
		StaticData.playerTransforms[1] = kevin.transform;

		maan.Init(maanManager, kevin.transform);
		kevin.Init(kevinManager, maan.transform);

		maanManager.Init(trackPieces, maan);
		kevinManager.Init(trackPieces, kevin);

		menuManager.Init(this);
		_deactivationTimer = 0;

		maan.GetComponent<PauseScreen>().Init(this, maan, PlayerIndex.One);
		kevin.GetComponent<PauseScreen>().Init(this, kevin, PlayerIndex.Two);

		fmodLinkedInstance = RuntimeManager.CreateInstance(fmodLinkedPath);
	}

	public void DeactivateGame ()
	{
		kevinManager.Deactivate();
		maanManager.Deactivate();

		menuManager.ActivateMenu();
		StaticData.menuActive = true;
	}

	public void ActivateGame ()
	{
		Init();
		_pacingTimer = 0;
		_deactivationTimer = 0;
		StaticData.menuActive = false;
	}

	void Update ()
	{
		if (!StaticData.menuActive) {
			if (Vector3.Distance(kevin.transform.position, maan.transform.position) <= StaticData.distanceToLink) {
				StaticData.playersAreLinked = true;
			} else {
				StaticData.playersAreLinked = false;
			}

			if (XInputEX.GetAnyInput(PlayerIndex.One) || XInputEX.GetAnyInput(PlayerIndex.Two)) {
				_deactivationTimer = 0;
			}
			_deactivationTimer += Time.deltaTime;
			if (_deactivationTimer > secondsBeforeDeactivation) {
				_deactivationTimer = 0;
				DeactivateGame();
			}

			_pacingTimer += Time.deltaTime;
			if (_pacingTimer >= secondsBeforeFirstCloud) {
				maanManager.ActivateCloud();
			}
			if (_pacingTimer >= secondsBeforeFatigue) {
				kevin.ActivateFatigue();
			}
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