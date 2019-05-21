using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;
using UnityEngine.SceneManagement;

namespace Archive
{
	public class Menu : MonoBehaviour
	{
		public GameObject[] playerArrowDisplays = new GameObject[2], levelArrowDisplays = new GameObject[2], startArrowDisplays = new GameObject[2];
		public Text playersDisplay, levelDisplay;

		enum MenuOptions { Players = 0, Level = 1, Start = 2 };
		MenuOptions selectedOption = MenuOptions.Players;
		enum PlayersOptions { Two = 2, Four = 4 };
		PlayersOptions playersOption = PlayersOptions.Two;
		enum LevelOptions { One = 0, Two = 1, Three = 2 };
		LevelOptions levelOption = LevelOptions.One;
		int numberOfLevels = 3;

		PlayerIndex[] playerIndices = new PlayerIndex[] { PlayerIndex.One, PlayerIndex.Two, PlayerIndex.Three, PlayerIndex.Four };
		GamePadState[] gamePadStates = new GamePadState[4];
		bool[] gamePadWaitingForReset = new bool[] { false, false, false, false };

		void Start ()
		{
			SwitchSelectedOption(MenuOptions.Players);
			//StaticData.levelNumber = 0;
			//LevelInformation.numberOfPlayers = 2;
		}

		void Update ()
		{
			for (int i = 0; i < playerIndices.Length; i++) {
				gamePadStates[i] = GamePad.GetState(playerIndices[i]);
			}

			switch (selectedOption) {
				case MenuOptions.Players:
					for (int i = 0; i < gamePadStates.Length; i++) {
						if ((gamePadStates[i].ThumbSticks.Left.Y > 0 || gamePadStates[i].DPad.Up == ButtonState.Pressed) && !gamePadWaitingForReset[i]) {
							SwitchSelectedOption(MenuOptions.Start);
							gamePadWaitingForReset[i] = true;
						} else if ((gamePadStates[i].ThumbSticks.Left.Y < 0 || gamePadStates[i].DPad.Down == ButtonState.Pressed) && !gamePadWaitingForReset[i]) {
							SwitchSelectedOption(MenuOptions.Level);
							gamePadWaitingForReset[i] = true;
						} else if (!gamePadWaitingForReset[i] && (gamePadStates[i].Buttons.A == ButtonState.Pressed || gamePadStates[i].Buttons.Start == ButtonState.Pressed ||
							  gamePadStates[i].ThumbSticks.Left.X != 0 || gamePadStates[i].DPad.Right == ButtonState.Pressed || gamePadStates[i].DPad.Left == ButtonState.Pressed))
							if (playersOption == PlayersOptions.Two) {
								playersOption = PlayersOptions.Four;
								playersDisplay.text = "4";
								//LevelInformation.numberOfPlayers = (int) playersOption;
								gamePadWaitingForReset[i] = true;
							} else {
								playersOption = PlayersOptions.Two;
								playersDisplay.text = "2";
								//LevelInformation.numberOfPlayers = (int) playersOption;
								gamePadWaitingForReset[i] = true;
							}
					}
					break;
				case MenuOptions.Level:
					for (int i = 0; i < gamePadStates.Length; i++) {
						//Up and down
						if ((gamePadStates[i].ThumbSticks.Left.Y > 0 || gamePadStates[i].DPad.Up == ButtonState.Pressed) && !gamePadWaitingForReset[i]) {
							SwitchSelectedOption(MenuOptions.Players);
							gamePadWaitingForReset[i] = true;
						} else if ((gamePadStates[i].ThumbSticks.Left.Y < 0 || gamePadStates[i].DPad.Down == ButtonState.Pressed) && !gamePadWaitingForReset[i]) {
							SwitchSelectedOption(MenuOptions.Start);
							gamePadWaitingForReset[i] = true;
						} else
						//Selecting a level
						if (!gamePadWaitingForReset[i] && (gamePadStates[i].Buttons.A == ButtonState.Pressed || gamePadStates[i].Buttons.Start == ButtonState.Pressed ||
							  gamePadStates[i].ThumbSticks.Left.X > 0 || gamePadStates[i].DPad.Right == ButtonState.Pressed)) {
							int temp = (int) levelOption + 1;
							if (temp > numberOfLevels - 1)
								temp = 0;
							levelOption = (LevelOptions) temp;
							levelDisplay.text = ((int) levelOption + 1).ToString();
							//StaticData.levelNumber = (int) levelOption;
							gamePadWaitingForReset[i] = true;
						} else if (!gamePadWaitingForReset[i] && (gamePadStates[i].ThumbSticks.Left.X < 0 || gamePadStates[i].DPad.Left == ButtonState.Pressed)) {
							int temp = (int) levelOption - 1;
							if (temp < 0)
								temp = numberOfLevels - 1;
							levelOption = (LevelOptions) temp;
							levelDisplay.text = ((int) levelOption + 1).ToString();
							//StaticData.levelNumber = (int) levelOption;
							gamePadWaitingForReset[i] = true;
						}
					}
					break;
				case MenuOptions.Start:
					for (int i = 0; i < gamePadStates.Length; i++) {
						if ((gamePadStates[i].ThumbSticks.Left.Y > 0 || gamePadStates[i].DPad.Up == ButtonState.Pressed) && !gamePadWaitingForReset[i]) {
							SwitchSelectedOption(MenuOptions.Level);
							gamePadWaitingForReset[i] = true;
						} else if ((gamePadStates[i].ThumbSticks.Left.Y < 0 || gamePadStates[i].DPad.Down == ButtonState.Pressed) && !gamePadWaitingForReset[i]) {
							SwitchSelectedOption(MenuOptions.Players);
							gamePadWaitingForReset[i] = true;
						} else if (!gamePadWaitingForReset[i] && (gamePadStates[i].Buttons.A == ButtonState.Pressed || gamePadStates[i].Buttons.Start == ButtonState.Pressed))
							StartGame();
					}
					break;
			}

			//Check whether controller has reset to neutral before allowing them to input again
			for (int i = 0; i < gamePadWaitingForReset.Length; i++) {
				if (gamePadStates[i].ThumbSticks.Left.Y == 0 &&
					gamePadStates[i].ThumbSticks.Left.X == 0 &&
					gamePadStates[i].DPad.Up == ButtonState.Released &&
					gamePadStates[i].DPad.Right == ButtonState.Released &&
					gamePadStates[i].DPad.Down == ButtonState.Released &&
					gamePadStates[i].DPad.Left == ButtonState.Released &&
					gamePadStates[i].Buttons.A == ButtonState.Released &&
					gamePadStates[i].Buttons.Start == ButtonState.Released)
					gamePadWaitingForReset[i] = false;
			}
		}

		void SwitchSelectedOption (MenuOptions nextOption)
		{
			foreach (GameObject g in levelArrowDisplays)
				g.SetActive(false);
			foreach (GameObject g in playerArrowDisplays)
				g.SetActive(false);
			foreach (GameObject g in startArrowDisplays)
				g.SetActive(false);
			switch (nextOption) {
				case MenuOptions.Level:
					foreach (GameObject g in levelArrowDisplays)
						g.SetActive(true);
					break;
				case MenuOptions.Players:
					foreach (GameObject g in playerArrowDisplays)
						g.SetActive(true);
					break;
				case MenuOptions.Start:
					foreach (GameObject g in startArrowDisplays)
						g.SetActive(true);
					break;
			}
			selectedOption = nextOption;
		}

		void StartGame ()
		{
			Debug.Log("Starting level " + (int) levelOption + " with " + (int) playersOption + " players.");
			SceneManager.LoadScene(1);
		}
	}
}