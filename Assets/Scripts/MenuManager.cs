using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class MenuManager : MonoBehaviour
{
	GameManager _gameManager;
	public GameObject kevinCanvas, maanCanvas;

	GamePadState[] gamePadStates = new GamePadState[2];

	const float timeBeforeTimeOut = 3;
	float _timer = 0;

	private void Awake ()
	{
		_gameManager = GetComponent<GameManager>();
	}

	void Update ()
	{
		if (!StaticData.menuActive) {
			bool anyInputDetected = false;
			for (int i = 0; i < 2; i++) {
				gamePadStates[i] = GamePad.GetState((PlayerIndex) i);

				if (AnyInput(gamePadStates[i]))
					anyInputDetected = true;
			}

			if (anyInputDetected) {
				_timer = 0;
			} else {
				_timer += Time.deltaTime;
				if (_timer >= timeBeforeTimeOut) {
					ActivateMenu();
				}
			}
		} else {
			for (int i = 0; i < 2; i++) {
				gamePadStates[i] = GamePad.GetState((PlayerIndex) i);

				if (AnyInput(gamePadStates[i])) {
					DeactivateMenu();
					return;
				}
			}
		}
	}

	bool AnyInput (GamePadState state)
	{
		if (state.Buttons.A == ButtonState.Pressed)
			return true;
		//if (state.Buttons.B == ButtonState.Pressed)
		//	return true;
		//if (state.Buttons.X == ButtonState.Pressed)
		//	return true;
		//if (state.Buttons.Y == ButtonState.Pressed)
		//	return true;
		//if (state.Buttons.Start == ButtonState.Pressed)
		//	return true;
		//if (state.Buttons.Back == ButtonState.Pressed)
		//	return true;
		if (state.ThumbSticks.Left.X != 0)
			return true;
		if (state.ThumbSticks.Left.Y != 0)
			return true;
		if (state.ThumbSticks.Right.X != 0)
			return true;
		if (state.ThumbSticks.Right.Y != 0)
			return true;
		if (state.Triggers.Left != 0)
			return true;
		if (state.Triggers.Right != 0)
			return true;

		return false;
	}


	void ActivateMenu ()
	{
		_gameManager.ActivateMenu();
		StaticData.menuActive = true;
	}

	void DeactivateMenu ()
	{
		_gameManager.DeactivateMenu();
		StaticData.menuActive = false;
	}
}