using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class PauseScreen : MonoBehaviour
{
	GameManager gameManager;
	ICharacter character;
	PlayerIndex playerIndex;
	GamePadState _gamePadState;

	public enum Options { Resume, Quit };
	Options selectedOption = Options.Resume;

	public GameObject canvasParent;
	public Image resumeIndicator, quitIndicator;

	public void Init (GameManager gameManager, ICharacter character, PlayerIndex playerIndex)
	{
		this.gameManager = gameManager;
		this.character = character;
		this.playerIndex = playerIndex;
	}

	void Update ()
	{
		_gamePadState = GamePad.GetState(playerIndex);

		//Select an option
		if (XInputDotNetExtender.instance.GetButtonDown(XInputDotNetExtender.Buttons.A, playerIndex) ||
			XInputDotNetExtender.instance.GetButtonDown(XInputDotNetExtender.Buttons.Start, playerIndex)){
			if (selectedOption == Options.Resume) {
				Deactivate();
			} else {
				gameManager.DeactivateGame();
			}
		}

		//Change option
		if (XInputDotNetExtender.instance.GetButtonDown(XInputDotNetExtender.Buttons.DPadUp, playerIndex) ||
			XInputDotNetExtender.instance.GetButtonDown(XInputDotNetExtender.Buttons.DPadDown, playerIndex) ||
			XInputDotNetExtender.instance.GetDirectionDown(XInputDotNetExtender.DirectionalInputs.LeftStick, XInputDotNetExtender.Directions.Up, playerIndex) ||
			XInputDotNetExtender.instance.GetDirectionDown(XInputDotNetExtender.DirectionalInputs.LeftStick, XInputDotNetExtender.Directions.Down, playerIndex) ||
			XInputDotNetExtender.instance.GetDirectionDown(XInputDotNetExtender.DirectionalInputs.LeftStick, XInputDotNetExtender.Directions.Left, playerIndex) ||
			XInputDotNetExtender.instance.GetDirectionDown(XInputDotNetExtender.DirectionalInputs.LeftStick, XInputDotNetExtender.Directions.Right, playerIndex)
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