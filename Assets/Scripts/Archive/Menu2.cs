using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XInputDotNetPure;
using Utility;

namespace Archive
{
	public class Menu2 : MonoBehaviour
	{
		GamePadState[] gamePadStates = new GamePadState[2];
		bool[] controllerWaitingForReset = new bool[] { false, false };
		float timeToReset = 0.2f;

		enum Options { Start, Controls };
		Options selectedOption = Options.Start;
		bool controlsActive = false;
		public GameObject[] startIndicators, controlsIndicators;
		public GameObject controlsScreen;

		private void Awake ()
		{
			selectedOption = Options.Start;
			foreach (GameObject go in startIndicators)
				go.SetActive(true);
			foreach (GameObject go in controlsIndicators)
				go.SetActive(false);
			controlsScreen.SetActive(false);
			controlsActive = false;
		}

		void Update ()
		{
			for (int i = 0; i < 2; i++) {
				gamePadStates[i] = GamePad.GetState((PlayerIndex) i);

				if (selectedOption == Options.Start) {
					if (!controllerWaitingForReset[i]) {
						if (gamePadStates[i].DPad.Up == ButtonState.Pressed || gamePadStates[i].DPad.Down == ButtonState.Pressed || gamePadStates[i].ThumbSticks.Left.Y > 0.3f || gamePadStates[i].ThumbSticks.Left.Y < -.3f) {
							selectedOption = Options.Controls;

							foreach (GameObject go in startIndicators)
								go.SetActive(false);
							foreach (GameObject go in controlsIndicators)
								go.SetActive(true);

							controllerWaitingForReset[i] = true;
							StartCoroutine(ResetController(i));
						}
					}

					if (gamePadStates[i].Buttons.A == ButtonState.Pressed || gamePadStates[i].Buttons.Start == ButtonState.Pressed) {
						SceneManager.LoadScene(1);
					}
				} else {
					if (!controllerWaitingForReset[i]) {
						if (!controlsActive) {
							if (gamePadStates[i].DPad.Up == ButtonState.Pressed || gamePadStates[i].DPad.Down == ButtonState.Pressed || gamePadStates[i].ThumbSticks.Left.Y > 0.3f || gamePadStates[i].ThumbSticks.Left.Y < -.3f) {
								selectedOption = Options.Start;

								foreach (GameObject go in startIndicators)
									go.SetActive(true);
								foreach (GameObject go in controlsIndicators)
									go.SetActive(false);

								controllerWaitingForReset[i] = true;
								StartCoroutine(ResetController(i));
							}
						}

						if (gamePadStates[i].Buttons.A == ButtonState.Pressed || gamePadStates[i].Buttons.Start == ButtonState.Pressed || gamePadStates[i].Buttons.B == ButtonState.Pressed) {
							controlsActive = !controlsActive;
							controlsScreen.SetActive(controlsActive);
							controllerWaitingForReset[i] = true;
							StartCoroutine(ResetController(i));
						}
					}
				}
			}


		}

		IEnumerator ResetController (int index)
		{
			yield return new WaitForSeconds(timeToReset);
			controllerWaitingForReset[index] = false;
		}
	}
}