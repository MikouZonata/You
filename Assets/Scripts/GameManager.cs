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
	public GameObject level;
	public GameObject kevinPrefab, maanPrefab;
	bool _firstTimeInit = true;
	Kevin _kevin;
	Maan _maan;
	KevinManager _kevinManager;
	MaanManager _maanManager;
	Transform[] trackPieces;

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

			_maanManager = GetComponent<MaanManager>();
			_kevinManager = GetComponent<KevinManager>();

			_firstTimeInit = false;
		}

		_maan = (Instantiate(maanPrefab, new Vector3(-1, .05f, 0), Quaternion.identity).GetComponent<Maan>());
		_maan.playerIndex = (PlayerIndex) 0;
		StaticData.playerTransforms[0] = _maan.transform;
		_kevin = (Instantiate(kevinPrefab, new Vector3(1, .05f, 0), Quaternion.identity).GetComponent<Kevin>());
		_kevin.playerIndex = (PlayerIndex) 1;
		StaticData.playerTransforms[1] = _kevin.transform;

		_maan.Init(_maanManager, _kevin.transform);
		_kevin.Init(_kevinManager, _maan.transform);

		_maanManager.Init(trackPieces, _maan);
		_kevinManager.Init(trackPieces, _kevin);

		fmodLinkedInstance = RuntimeManager.CreateInstance(fmodLinkedPath);
	}

	private void DeactivateGame ()
	{
		_kevinManager.Deactivate();
		_maanManager.Deactivate();
	}

	void Update ()
	{
		if (!StaticData.menuActive) {
			if (Vector3.Distance(_kevin.transform.position, _maan.transform.position) <= StaticData.distanceToLink) {
				StaticData.playersAreLinked = true;
			} else {
				StaticData.playersAreLinked = false;
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

	public void ActivateMenu ()
	{
		DeactivateGame();
	}

	public void DeactivateMenu ()
	{
		Init();
	}
}