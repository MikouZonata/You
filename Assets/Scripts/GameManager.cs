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
	Kevin kevin;
	Maan maan;
	KevinManager kevinManager;
	MaanManager maanManager;
	MenuManager menuManager;
	Transform[] trackPieces;
	bool firstTimeInit = true;

	const int secondsBeforeDeactivation = 90;
	float _deactivationTimer = 0;

	const int secondsBeforeFirstCloud = 180;
	const int secondsBeforeFatigue = 180;
	float _pacingTimer = 0;

	//FMOD
	const string fmodLinkedPath = "event:/Linked_Up";
	FMOD.Studio.EventInstance fmodLinkedInstance;
	bool _fmodLinkedPlaying = false;

	public GameObject singleMonitorErrorDisplay;
	const float singleMonitorErrorUptime = 7;

	void Awake ()
	{
		Init();
	}

	void Init ()
	{
		if (firstTimeInit) {
			//Activate both displays if not in editor
			Display.displays[0].Activate();
#if UNITY_EDITOR
#else
			if (Display.displays.Length > 1) {
				Display.displays[1].Activate();
			} else {
				StartCoroutine(SingleMonitorErrorMessage());
			}
#endif
			Cursor.visible = false;

			//Retrieve all pieces of the level
			Transform trackPiecesParent = level.transform;
			trackPieces = new Transform[trackPiecesParent.childCount];
			for (int i = 0; i < trackPieces.Length; i++) {
				trackPieces[i] = trackPiecesParent.GetChild(i);
			}

			maanManager = GetComponent<MaanManager>();
			kevinManager = GetComponent<KevinManager>();
			menuManager = GetComponent<MenuManager>();

			firstTimeInit = false;
		}

		//Create player characters
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

		//Main menu manager
		menuManager.Init(this);
		_deactivationTimer = 0;

		maan.GetComponent<PauseScreen>().Init(this, maan, XInputDotNetPure.PlayerIndex.One);
		kevin.GetComponent<PauseScreen>().Init(this, kevin, XInputDotNetPure.PlayerIndex.Two);

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
		//If the game is running
		if (!StaticData.menuActive) {
			//Check to see whether players are close enough to be considered linked
			if (Vector3.Distance(kevin.transform.position, maan.transform.position) <= StaticData.distanceToLink) {
				StaticData.playersAreLinked = true;
			} else {
				StaticData.playersAreLinked = false;
			}

			//If any inputs are detected postpone deactivating the game
			if (XInputEX.GetAnyInput(PlayerIndex.One) || XInputEX.GetAnyInput(PlayerIndex.Two)) {
				_deactivationTimer = 0;
			}
			//Deactivate the game after a period of inactivity
			_deactivationTimer += Time.deltaTime;
			if (_deactivationTimer > secondsBeforeDeactivation) {
				_deactivationTimer = 0;
				DeactivateGame();
			}

			//Maan's Cloud and Kevin's Fatigue activate after some time has passed
			_pacingTimer += Time.deltaTime;
			if (_pacingTimer >= secondsBeforeFirstCloud) {
				maanManager.ActivateCloud();
			}
			if (_pacingTimer >= secondsBeforeFatigue) {
				kevin.ActivateFatigue();
			}
		}

		//Play global audio if players are linked
		if (_fmodLinkedPlaying && !StaticData.playersAreLinked) {
			_fmodLinkedPlaying = false;
			fmodLinkedInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
		if (!_fmodLinkedPlaying && StaticData.playersAreLinked) {
			_fmodLinkedPlaying = true;
			fmodLinkedInstance.start();
		}
	}

	IEnumerator SingleMonitorErrorMessage ()
	{
		singleMonitorErrorDisplay.SetActive(true);
		yield return new WaitForSeconds(singleMonitorErrorUptime);
		singleMonitorErrorDisplay.SetActive(false);
	}
}