using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class MenuManager : MonoBehaviour
{
	GamePadState[] gamePadStates = new GamePadState[2];

	GameManager gameManager;
	GameObject menuParent;
	public Text[] titleDisplays, creditsDisplays, linesDisplays;
	public Image[] faceDisplays;

	bool _menuOpen = false;
	bool _cutscenePlaying = false;
	string[][] cutsceneLines = new string[2][];
	string[] kevinLines = new string[] {
		"Yo, ik ben Kevin.",
		"Maan en ik zijn ruim 4 jaar samen.",
		"Soms ben ik ervan overtuigd dat Maan het beste is dat mij ooit is overkomen.",
		"Soms denk ik dat wij zo veel van elkaar verschillen dat wij gelukkiger zouden zijn met andere partners.",
		"In deze game kunnen jullie ervaren hoe het is om ons te zijn.",
		"Denken jullie dat wij bij elkaar passen?"
	};
	string[] maanLines = new string[] {
		"Hoi! Ik ben Maan :D",
		"Kevin en ik zijn al 4 jaar samen (vet lang!)",
		"Hij is vet leuk en lief ook al denkt hij zelf van niet.",
		"Maar soms lijkt hij mij niet nodig te hebben en weet ik niet of hij wel van mij houdt...",
		"In deze game kunnen jullie ervaren hoe het is om ons te zijn.",
		"Denken jullie dat wij bij elkaar passen?"
	};
	float[] timesPerLine = new float[] { 2, 2.5f, 3.5f, 3.5f, 3, 4 };
	const float lineFadeTime = .32f;

	public void Init (GameManager gameManager)
	{
		this.gameManager = gameManager;
		menuParent = transform.GetChild(0).gameObject;
		menuParent.SetActive(false);
		cutsceneLines[0] = kevinLines;
		cutsceneLines[1] = maanLines;

		menuParent.SetActive(false);
		enabled = false;
	}

	void Update ()
	{
		if (_menuOpen) {
			for (int i = 0; i < 2; i++) {
				if (!_cutscenePlaying &&
					(XInputDotNetExtender.instance.GetAnyButton(PlayerIndex.One) ||
					XInputDotNetExtender.instance.GetAnyButton(PlayerIndex.Two))) {
					_cutscenePlaying = true;
					StartCoroutine(RunCutscene());
					return;
				}
			}
		}
	}

	IEnumerator OpenMenu ()
	{
		for (int i = 0; i < 2; i++) {
			linesDisplays[i].color = new Color(0, 0, 0, 0);
			faceDisplays[i].color = new Color(1, 1, 1, 0);
			titleDisplays[i].color = new Color(0, 0, 0, 0);
			creditsDisplays[i].color = new Color(0, 0, 0, 0);
		}

		float fadeTimeInverse = 1 / lineFadeTime;
		for (float a = 0; a < 1; a += Time.deltaTime * fadeTimeInverse) {
			for (int i = 0; i < 2; i++) {
				faceDisplays[i].color = new Color(1, 1, 1, a);
				titleDisplays[i].color = new Color(0, 0, 0, a);
				creditsDisplays[i].color = new Color(0, 0, 0, a);
			}
			yield return null;
		}

		for (int i = 0; i < 2; i++) {
			faceDisplays[i].color = new Color(1, 1, 1, 1);
			titleDisplays[i].color = new Color(0, 0, 0, 1);
			creditsDisplays[i].color = new Color(0, 0, 0, 1);
		}

		_menuOpen = true;
	}

	IEnumerator RunCutscene ()
	{
		float fadeTimeInverse = 1 / lineFadeTime;

		for (float a = 1; a > 0; a -= Time.deltaTime * fadeTimeInverse) {
			for (int i = 0; i < 2; i++) {
				faceDisplays[i].color = new Color(1, 1, 1, a);
				titleDisplays[i].color = new Color(0, 0, 0, a);
				creditsDisplays[i].color = new Color(0, 0, 0, a);
			}
			yield return null;
		}
		for (int i = 0; i < 2; i++) {
			faceDisplays[i].color = new Color(1, 1, 1, 0);
			titleDisplays[i].color = new Color(0, 0, 0, 0);
			creditsDisplays[i].color = new Color(0, 0, 0, 0);
		}

		for (int i = 0; i < 2; i++) {
			titleDisplays[i].enabled = false;
			creditsDisplays[i].enabled = false;
			linesDisplays[i].enabled = true;
			linesDisplays[i].color = new Color(0, 0, 0, 0);
		}



		for (int lineIndex = 0; lineIndex < cutsceneLines[0].Length; lineIndex++) {
			for (int i = 0; i < linesDisplays.Length; i++) {
				linesDisplays[i].text = cutsceneLines[i][lineIndex];
			}
			for (float a = 0; a < 1; a += Time.deltaTime * fadeTimeInverse) {
				foreach (Text t in linesDisplays) {
					t.color = new Color(0, 0, 0, a);
				}
				yield return null;
			}
			yield return new WaitForSeconds(timesPerLine[lineIndex]);
			for (float a = 1; a > 0; a -= Time.deltaTime * fadeTimeInverse) {
				foreach (Text t in linesDisplays) {
					t.color = new Color(0, 0, 0, a);
				}
				yield return null;
			}
		}

		yield return new WaitForSeconds(1);

		DeactivateMenu();
		_cutscenePlaying = false;
	}

	public void ActivateMenu ()
	{
		Debug.Log("Menu Activated");
		menuParent.SetActive(true);

		for (int i = 0; i < 2; i++) {
			titleDisplays[i].enabled = true;
			titleDisplays[i].color = Color.black;
			creditsDisplays[i].enabled = true;
			creditsDisplays[i].color = Color.black;
			faceDisplays[i].color = Color.white;
			linesDisplays[i].enabled = false;
		}
		_cutscenePlaying = false;
		_menuOpen = false;
		StartCoroutine(OpenMenu());

		enabled = true;
	}

	void DeactivateMenu ()
	{
		Debug.Log("Menu Deactivated");
		_menuOpen = false;
		menuParent.SetActive(false);

		gameManager.ActivateGame();

		enabled = false;
	}
}