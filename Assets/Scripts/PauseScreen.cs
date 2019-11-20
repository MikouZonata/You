using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetExtended;

public class PauseScreen : MonoBehaviour
{
	GameManager gameManager;
	ICharacter character;
	XInputDotNetPure.PlayerIndex playerIndex;

	public enum Options { Resume, Quit };
	Options selectedOption = Options.Resume;

	public GameObject canvasParent;
	public Image resumeIndicator, quitIndicator;

	public void Init (GameManager gameManager, ICharacter character, XInputDotNetPure.PlayerIndex playerIndex)
	{
		this.gameManager = gameManager;
		this.character = character;
		this.playerIndex = playerIndex;
	}

	void Update ()
	{
		//Select an option
		if (XInputEX.GetButtonDown(playerIndex, XInputEX.Buttons.A) ||
			XInputEX.GetButtonDown(playerIndex, XInputEX.Buttons.Start)){
			if (selectedOption == Options.Resume) {
				Deactivate();
			} else {
				gameManager.DeactivateGame();
			}
		}

		//Change option
		if (XInputEX.GetButtonDown(playerIndex, XInputEX.Buttons.DPadUp) ||
			XInputEX.GetButtonDown(playerIndex, XInputEX.Buttons.DPadDown) ||
			XInputEX.GetDirectionDown(playerIndex, XInputEX.DirectionalInputs.LeftStick, XInputEX.Directions.Up) ||
			XInputEX.GetDirectionDown(playerIndex, XInputEX.DirectionalInputs.LeftStick, XInputEX.Directions.Down) ||
			XInputEX.GetDirectionDown(playerIndex, XInputEX.DirectionalInputs.LeftStick, XInputEX.Directions.Left) ||
			XInputEX.GetDirectionDown(playerIndex, XInputEX.DirectionalInputs.LeftStick, XInputEX.Directions.Right)
			) {
			if (selectedOption == Options.Resume) {
				selectedOption = Options.Quit;
				resumeIndicator.enabled = false;
				quitIndicator.enabled = true;
			} else {
				selectedOption = Options.Resume;
				quitIndicator.enabled = false;
				resumeIndicator.enabled = true;
			}
		}
	}

	public void Activate ()
	{
		selectedOption = Options.Resume;
		canvasParent.SetActive(true);
		resumeIndicator.enabled = true;
		quitIndicator.enabled = false;
		enabled = true;
	}

	void Deactivate ()
	{
		canvasParent.SetActive(false);
		character.DeactivatePause();
		enabled = false;
	}
}