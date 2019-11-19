using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XInputDotNetPure
{
	public class XInputDotNetExtender : MonoBehaviour
	{
		public static XInputDotNetExtender instance;

		GamePadState[] prevStates = new GamePadState[4], currentStates = new GamePadState[4];
		public enum Buttons { A, B, X, Y, Start, Back, RB, LB, LS, RS, Guide, DPadUp, DPadRight, DPadDown, DPadLeft };
		public enum DirectionalInputs { LeftStick, RightStick, DPad };
		public enum Directions { Up, Right, Down, Left };

		private void Awake ()
		{
			instance = this;
		}

		private void Update ()
		{
			for (int i = 0; i < 4; i++) {
				prevStates[i] = currentStates[i];
				currentStates[i] = GamePad.GetState((PlayerIndex) i);
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
					if (prevStates[index].Buttons.A == ButtonState.Released && currentStates[index].Buttons.A == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.B:
					if (prevStates[index].Buttons.B == ButtonState.Released && currentStates[index].Buttons.B == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.X:
					if (prevStates[index].Buttons.X == ButtonState.Released && currentStates[index].Buttons.X == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.Y:
					if (prevStates[index].Buttons.Y == ButtonState.Released && currentStates[index].Buttons.Y == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.Start:
					if (prevStates[index].Buttons.Start == ButtonState.Released && currentStates[index].Buttons.Start == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.Back:
					if (prevStates[index].Buttons.Back == ButtonState.Released && currentStates[index].Buttons.Back == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.LB:
					if (prevStates[index].Buttons.LeftShoulder == ButtonState.Released && currentStates[index].Buttons.LeftShoulder == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.RB:
					if (prevStates[index].Buttons.RightShoulder == ButtonState.Released && currentStates[index].Buttons.RightShoulder == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.LS:
					if (prevStates[index].Buttons.LeftStick == ButtonState.Released && currentStates[index].Buttons.LeftStick == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.RS:
					if (prevStates[index].Buttons.RightStick == ButtonState.Released && currentStates[index].Buttons.RightStick == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.Guide:
					if (prevStates[index].Buttons.Guide == ButtonState.Released && currentStates[index].Buttons.Guide == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.DPadUp:
					if (prevStates[index].DPad.Up == ButtonState.Released && currentStates[index].DPad.Up == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.DPadRight:
					if (prevStates[index].DPad.Right == ButtonState.Released && currentStates[index].DPad.Right == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.DPadDown:
					if (prevStates[index].DPad.Down == ButtonState.Released && currentStates[index].DPad.Down == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.DPadLeft:
					if (prevStates[index].DPad.Left == ButtonState.Released && currentStates[index].DPad.Left == ButtonState.Pressed)
						return true;
					else
						return false;

			}
		}

		public bool GetDirectionDown (DirectionalInputs input, Directions direction, PlayerIndex playerIndex, float stickDeadzone = .12f)
		{
			int index = (int) playerIndex;
			switch (input) {
				default:
					Debug.Log("XInputDotNetExtender GetDirectionDown() has been sent invalid arguments?");
					return false;
				case DirectionalInputs.LeftStick:
					switch (direction) {
						default:
							Debug.Log("XInputDotNetExtender GetDirectionDown() has been sent invalid arguments?");
							return false;
						case Directions.Up:
							if (prevStates[index].ThumbSticks.Left.Y < stickDeadzone && currentStates[index].ThumbSticks.Left.Y >= stickDeadzone)
								return true;
							else
								return false;
						case Directions.Right:
							if (prevStates[index].ThumbSticks.Left.X < stickDeadzone && currentStates[index].ThumbSticks.Left.X >= stickDeadzone)
								return true;
							else
								return false;
						case Directions.Down:
							if (prevStates[index].ThumbSticks.Left.Y > -stickDeadzone && currentStates[index].ThumbSticks.Left.Y <= -stickDeadzone)
								return true;
							else
								return false;
						case Directions.Left:
							if (prevStates[index].ThumbSticks.Left.X > -stickDeadzone && currentStates[index].ThumbSticks.Left.X <= -stickDeadzone)
								return true;
							else
								return false;
					}
				case DirectionalInputs.RightStick:
					switch (direction) {
						default:
							Debug.Log("XInputDotNetExtender GetDirectionDown() has been sent invalid arguments?");
							return false;
						case Directions.Up:
							if (prevStates[index].ThumbSticks.Right.Y < stickDeadzone && currentStates[index].ThumbSticks.Right.Y >= stickDeadzone)
								return true;
							else
								return false;
						case Directions.Right:
							if (prevStates[index].ThumbSticks.Right.X < stickDeadzone && currentStates[index].ThumbSticks.Right.X >= stickDeadzone)
								return true;
							else
								return false;
						case Directions.Down:
							if (prevStates[index].ThumbSticks.Right.Y > -stickDeadzone && currentStates[index].ThumbSticks.Right.Y <= -stickDeadzone)
								return true;
							else
								return false;
						case Directions.Left:
							if (prevStates[index].ThumbSticks.Right.X > -stickDeadzone && currentStates[index].ThumbSticks.Right.X <= -stickDeadzone)
								return true;
							else
								return false;
					}
				case DirectionalInputs.DPad:
					switch (direction) {
						default:
							Debug.Log("XInputDotNetExtender GetDirectionDown() has been sent invalid arguments?");
							return false;
						case Directions.Up:
							if (prevStates[index].DPad.Up == ButtonState.Released && currentStates[index].DPad.Up == ButtonState.Pressed)
								return true;
							else
								return false;
						case Directions.Right:
							if (prevStates[index].DPad.Right == ButtonState.Released && currentStates[index].DPad.Right == ButtonState.Pressed)
								return true;
							else
								return false;
						case Directions.Down:
							if (prevStates[index].DPad.Down == ButtonState.Released && currentStates[index].DPad.Down == ButtonState.Pressed)
								return true;
							else
								return false;
						case Directions.Left:
							if (prevStates[index].DPad.Left == ButtonState.Released && currentStates[index].DPad.Left == ButtonState.Pressed)
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

			if (currentStates[index].Buttons.A == ButtonState.Pressed)
				return true;
			if (currentStates[index].Buttons.B == ButtonState.Pressed)
				return true;
			if (currentStates[index].Buttons.X == ButtonState.Pressed)
				return true;
			if (currentStates[index].Buttons.Y == ButtonState.Pressed)
				return true;
			if (currentStates[index].Buttons.Start == ButtonState.Pressed)
				return true;
			if (currentStates[index].Buttons.Back == ButtonState.Pressed)
				return true;
			if (currentStates[index].Buttons.RightShoulder == ButtonState.Pressed)
				return true;
			if (currentStates[index].Buttons.LeftShoulder == ButtonState.Pressed)
				return true;
			if (currentStates[index].Buttons.LeftStick == ButtonState.Pressed)
				return true;
			if (currentStates[index].Buttons.RightStick == ButtonState.Pressed)
				return true;
			if (currentStates[index].Buttons.Guide == ButtonState.Pressed)
				return true;

			return false;
		}

		public bool GetAnyInput (PlayerIndex playerIndex, float stickDeadzone = .12f)
		{
			int index = (int) playerIndex;

			if (currentStates[index].Buttons.A == ButtonState.Pressed)
				return true;
			if (currentStates[index].Buttons.B == ButtonState.Pressed)
				return true;
			if (currentStates[index].Buttons.X == ButtonState.Pressed)
				return true;
			if (currentStates[index].Buttons.Y == ButtonState.Pressed)
				return true;
			if (currentStates[index].Buttons.Start == ButtonState.Pressed)
				return true;
			if (currentStates[index].Buttons.Back == ButtonState.Pressed)
				return true;
			if (currentStates[index].Buttons.RightShoulder == ButtonState.Pressed)
				return true;
			if (currentStates[index].Buttons.LeftShoulder == ButtonState.Pressed)
				return true;
			if (currentStates[index].Buttons.LeftStick == ButtonState.Pressed)
				return true;
			if (currentStates[index].Buttons.RightStick == ButtonState.Pressed)
				return true;
			if (currentStates[index].Buttons.Guide == ButtonState.Pressed)
				return true;
			if (Mathf.Abs(currentStates[index].ThumbSticks.Left.X) >= stickDeadzone)
				return true;
			if (Mathf.Abs(currentStates[index].ThumbSticks.Left.Y) >= stickDeadzone)
				return true;
			if (Mathf.Abs(currentStates[index].ThumbSticks.Right.X) >= stickDeadzone)
				return true;
			if (Mathf.Abs(currentStates[index].ThumbSticks.Right.Y) >= stickDeadzone)
				return true;
			if (currentStates[index].Triggers.Left != 0)
				return true;
			if (currentStates[index].Triggers.Right != 0)
				return true;

			return false;
		}
	}
}