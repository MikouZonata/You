using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XInputDotNetPure
{
	public class XInputDotNetExtender : MonoBehaviour
	{
		public static XInputDotNetExtender instance;

		GamePadState[] prevStates = new GamePadState[4], currentState = new GamePadState[4];
		public enum Buttons { A, B, X, Y, Start, Back, RB, LB, LS, RS, Guide, DPadUp, DPadRight, DPadDown, DPadLeft };
		public enum DirectionalInputs { Left, Right };
		public enum Directions { Up, Right, Down, Left };

		private void Awake ()
		{
			instance = this;
		}

		private void Update ()
		{
			for (int i = 0; i < 4; i++) {
				prevStates[i] = currentState[i];
				currentState[i] = GamePad.GetState((PlayerIndex) i);
			}
		}

		public bool GetButtonDown (Buttons button, PlayerIndex playerIndex)
		{
			int index = (int) playerIndex;
			switch (button) {
				default:
					Debug.Log("XInputDotNetExtender GetButtonDown() has been sent invalid arguments?");
					return false;
				case Buttons.A:
					if (prevStates[index].Buttons.A == ButtonState.Released && currentState[index].Buttons.A == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.B:
					if (prevStates[index].Buttons.B == ButtonState.Released && currentState[index].Buttons.B == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.X:
					if (prevStates[index].Buttons.X == ButtonState.Released && currentState[index].Buttons.X == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.Y:
					if (prevStates[index].Buttons.Y == ButtonState.Released && currentState[index].Buttons.Y == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.Start:
					if (prevStates[index].Buttons.Start == ButtonState.Released && currentState[index].Buttons.Start == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.Back:
					if (prevStates[index].Buttons.Back == ButtonState.Released && currentState[index].Buttons.Back == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.LB:
					if (prevStates[index].Buttons.LeftShoulder == ButtonState.Released && currentState[index].Buttons.LeftShoulder == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.RB:
					if (prevStates[index].Buttons.RightShoulder == ButtonState.Released && currentState[index].Buttons.RightShoulder == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.LS:
					if (prevStates[index].Buttons.LeftStick == ButtonState.Released && currentState[index].Buttons.LeftStick == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.RS:
					if (prevStates[index].Buttons.RightStick == ButtonState.Released && currentState[index].Buttons.RightStick == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.Guide:
					if (prevStates[index].Buttons.Guide == ButtonState.Released && currentState[index].Buttons.Guide == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.DPadUp:
					if (prevStates[index].DPad.Up == ButtonState.Released && currentState[index].DPad.Up == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.DPadRight:
					if (prevStates[index].DPad.Right == ButtonState.Released && currentState[index].DPad.Right == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.DPadDown:
					if (prevStates[index].DPad.Down == ButtonState.Released && currentState[index].DPad.Down == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.DPadLeft:
					if (prevStates[index].DPad.Left == ButtonState.Released && currentState[index].DPad.Left == ButtonState.Pressed)
						return true;
					else
						return false;

			}
		}

		public bool GetDirectionDown (DirectionalInputs input, Directions direction, PlayerIndex playerIndex)
		{
			int index = (int) playerIndex;
			switch (input) {
				default:
					Debug.Log("XInputDotNetExtender GetDirectionDown() has been sent invalid arguments?");
					return false;
				case DirectionalInputs.Left:
					switch (direction) {
						default:
							Debug.Log("XInputDotNetExtender GetDirectionDown() has been sent invalid arguments?");
							return false;
						case Directions.Up:
							if (prevStates[index].ThumbSticks.Left.Y <= 0 && currentState[index].ThumbSticks.Left.Y > 0)
								return true;
							else
								return false;
						case Directions.Right:
							if (prevStates[index].ThumbSticks.Left.X <= 0 && currentState[index].ThumbSticks.Left.X > 0)
								return true;
							else
								return false;
						case Directions.Down:
							if (prevStates[index].ThumbSticks.Left.Y >= 0 && currentState[index].ThumbSticks.Left.Y < 0)
								return true;
							else
								return false;
						case Directions.Left:
							if (prevStates[index].ThumbSticks.Left.X >= 0 && currentState[index].ThumbSticks.Left.X < 0)
								return true;
							else
								return false;
					}
				case DirectionalInputs.Right:
					switch (direction) {
						default:
							Debug.Log("XInputDotNetExtender GetDirectionDown() has been sent invalid arguments?");
							return false;
						case Directions.Up:
							if (prevStates[index].ThumbSticks.Right.Y <= 0 && currentState[index].ThumbSticks.Right.Y > 0)
								return true;
							else
								return false;
						case Directions.Right:
							if (prevStates[index].ThumbSticks.Right.X <= 0 && currentState[index].ThumbSticks.Right.X > 0)
								return true;
							else
								return false;
						case Directions.Down:
							if (prevStates[index].ThumbSticks.Right.Y >= 0 && currentState[index].ThumbSticks.Right.Y < 0)
								return true;
							else
								return false;
						case Directions.Left:
							if (prevStates[index].ThumbSticks.Right.X >= 0 && currentState[index].ThumbSticks.Right.X < 0)
								return true;
							else
								return false;
					}
			}
		}

		/// <summary>
		/// Checks for A, B, X, Y, Start, Back, Bumpers, Stick presses and the Guide buttons
		/// </summary>
		/// <param name="playerIndex"></param>
		/// <returns></returns>
		public bool GetAnyButton (PlayerIndex playerIndex)
		{
			int index = (int) playerIndex;

			if (currentState[index].Buttons.A == ButtonState.Pressed)
				return true;
			if (currentState[index].Buttons.B == ButtonState.Pressed)
				return true;
			if (currentState[index].Buttons.X == ButtonState.Pressed)
				return true;
			if (currentState[index].Buttons.Y == ButtonState.Pressed)
				return true;
			if (currentState[index].Buttons.Start == ButtonState.Pressed)
				return true;
			if (currentState[index].Buttons.Back == ButtonState.Pressed)
				return true;
			if (currentState[index].Buttons.RightShoulder == ButtonState.Pressed)
				return true;
			if (currentState[index].Buttons.LeftShoulder == ButtonState.Pressed)
				return true;
			if (currentState[index].Buttons.LeftStick == ButtonState.Pressed)
				return true;
			if (currentState[index].Buttons.RightStick == ButtonState.Pressed)
				return true;
			if (currentState[index].Buttons.Guide == ButtonState.Pressed)
				return true;

			return false;
		}
	}
}