using XInputDotNetPure;
using UnityEngine;

namespace XInputDotNetExtended
{
	public static class XInputEX
	{
		private static GamePadState[] prevStates = new GamePadState[4], currentStates = new GamePadState[4];
		public enum Buttons { A, B, X, Y, Start, Back, RB, LB, LS, RS, Guide, DPadUp, DPadRight, DPadDown, DPadLeft };
		public enum DirectionalInputs { LeftStick, RightStick, DPad };
		public enum Triggers { Left, Right };
		public enum Directions { Up, Right, Down, Left };
		public enum Axis { LeftStickHorizontal, LeftStickVertical, RightStickHorizontal, RightStickVertical, DPadHorizontal, DPadVertical, Triggers };

		static XInputEX ()
		{
			GameObject updaterGO = new GameObject("XInputDotNetUpdater", typeof(XInputDotNetUpdater));
			updaterGO.hideFlags = HideFlags.HideInHierarchy;
			XInputDotNetUpdater.OnUpdate += Update;
		}

		private static void Update ()
		{
			for (int i = 0; i < 4; i++) {
				prevStates[i] = currentStates[i];
				currentStates[i] = GamePad.GetState((XInputDotNetPure.PlayerIndex) i);
			}
		}

		/// <summary>
		/// Returns true if the given button is pressed, false if it's not.
		/// </summary>
		/// <param name="playerIndex">Which controller would you like to check?</param>
		/// <param name="button">Which button should be checked?</param>
		public static bool GetButton (XInputDotNetPure.PlayerIndex playerIndex, Buttons button)
		{
			int index = (int) playerIndex;
			switch (button) {
				default:
					return false;
				case Buttons.A:
					if (currentStates[index].Buttons.A == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.B:
					if (currentStates[index].Buttons.B == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.X:
					if (currentStates[index].Buttons.X == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.Y:
					if (currentStates[index].Buttons.Y == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.Start:
					if (currentStates[index].Buttons.Start == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.Back:
					if (currentStates[index].Buttons.Back == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.LB:
					if (currentStates[index].Buttons.LeftShoulder == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.RB:
					if (currentStates[index].Buttons.RightShoulder == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.LS:
					if (currentStates[index].Buttons.LeftStick == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.RS:
					if (currentStates[index].Buttons.RightStick == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.Guide:
					if (currentStates[index].Buttons.Guide == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.DPadUp:
					if (currentStates[index].DPad.Up == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.DPadRight:
					if (currentStates[index].DPad.Right == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.DPadDown:
					if (currentStates[index].DPad.Down == ButtonState.Pressed)
						return true;
					else
						return false;
				case Buttons.DPadLeft:
					if (currentStates[index].DPad.Left == ButtonState.Pressed)
						return true;
					else
						return false;

			}
		}
		/// <summary>
		/// Returns true if the given button is pressed and wasn't pressed the previous frame.
		/// </summary>
		/// <param name="playerIndex">Which controller would you like to check?</param>
		/// <param name="button">Which button should be checked?</param>
		public static bool GetButtonDown (XInputDotNetPure.PlayerIndex playerIndex, Buttons button)
		{
			int index = (int) playerIndex;
			switch (button) {
				default:
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
		/// <summary>
		/// Returns true if the given button is released and was pressed the previous frame.
		/// </summary>
		/// <param name="playerIndex">Which controller would you like to check?</param>
		/// <param name="button">Which button should be checked?</param>
		public static bool GetButtonUp (XInputDotNetPure.PlayerIndex playerIndex, Buttons button)
		{
			int index = (int) playerIndex;
			switch (button) {
				default:
					return false;
				case Buttons.A:
					if (prevStates[index].Buttons.A == ButtonState.Pressed && currentStates[index].Buttons.A == ButtonState.Released)
						return true;
					else
						return false;
				case Buttons.B:
					if (prevStates[index].Buttons.B == ButtonState.Pressed && currentStates[index].Buttons.B == ButtonState.Released)
						return true;
					else
						return false;
				case Buttons.X:
					if (prevStates[index].Buttons.X == ButtonState.Pressed && currentStates[index].Buttons.X == ButtonState.Released)
						return true;
					else
						return false;
				case Buttons.Y:
					if (prevStates[index].Buttons.Y == ButtonState.Pressed && currentStates[index].Buttons.Y == ButtonState.Released)
						return true;
					else
						return false;
				case Buttons.Start:
					if (prevStates[index].Buttons.Start == ButtonState.Pressed && currentStates[index].Buttons.Start == ButtonState.Released)
						return true;
					else
						return false;
				case Buttons.Back:
					if (prevStates[index].Buttons.Back == ButtonState.Pressed && currentStates[index].Buttons.Back == ButtonState.Released)
						return true;
					else
						return false;
				case Buttons.LB:
					if (prevStates[index].Buttons.LeftShoulder == ButtonState.Pressed && currentStates[index].Buttons.LeftShoulder == ButtonState.Released)
						return true;
					else
						return false;
				case Buttons.RB:
					if (prevStates[index].Buttons.RightShoulder == ButtonState.Pressed && currentStates[index].Buttons.RightShoulder == ButtonState.Released)
						return true;
					else
						return false;
				case Buttons.LS:
					if (prevStates[index].Buttons.LeftStick == ButtonState.Pressed && currentStates[index].Buttons.LeftStick == ButtonState.Released)
						return true;
					else
						return false;
				case Buttons.RS:
					if (prevStates[index].Buttons.RightStick == ButtonState.Pressed && currentStates[index].Buttons.RightStick == ButtonState.Released)
						return true;
					else
						return false;
				case Buttons.Guide:
					if (prevStates[index].Buttons.Guide == ButtonState.Pressed && currentStates[index].Buttons.Guide == ButtonState.Released)
						return true;
					else
						return false;
				case Buttons.DPadUp:
					if (prevStates[index].DPad.Up == ButtonState.Pressed && currentStates[index].DPad.Up == ButtonState.Released)
						return true;
					else
						return false;
				case Buttons.DPadRight:
					if (prevStates[index].DPad.Right == ButtonState.Pressed && currentStates[index].DPad.Right == ButtonState.Released)
						return true;
					else
						return false;
				case Buttons.DPadDown:
					if (prevStates[index].DPad.Down == ButtonState.Pressed && currentStates[index].DPad.Down == ButtonState.Released)
						return true;
					else
						return false;
				case Buttons.DPadLeft:
					if (prevStates[index].DPad.Left == ButtonState.Pressed && currentStates[index].DPad.Left == ButtonState.Released)
						return true;
					else
						return false;

			}
		}

		/// <summary>
		/// Returns true if the given directional input is held.
		/// </summary>
		/// <param name="playerIndex">Which controller would you like to check?</param>
		/// <param name="input">Which input would you like to check?</param>
		/// <param name="direction">Which direction would you like to check?</param>
		/// <param name="stickDeadzone">How much of a deadzone should be applied? (sticks only)</param>
		public static bool GetDirection (XInputDotNetPure.PlayerIndex playerIndex, DirectionalInputs input, Directions direction, float stickDeadzone = .12f)
		{
			int index = (int) playerIndex;
			switch (input) {
				default:
					return false;
				case DirectionalInputs.LeftStick:
					switch (direction) {
						default:
							return false;
						case Directions.Up:
							if (currentStates[index].ThumbSticks.Left.Y >= stickDeadzone)
								return true;
							else
								return false;
						case Directions.Right:
							if (currentStates[index].ThumbSticks.Left.X >= stickDeadzone)
								return true;
							else
								return false;
						case Directions.Down:
							if (currentStates[index].ThumbSticks.Left.Y <= -stickDeadzone)
								return true;
							else
								return false;
						case Directions.Left:
							if (currentStates[index].ThumbSticks.Left.X <= -stickDeadzone)
								return true;
							else
								return false;
					}
				case DirectionalInputs.RightStick:
					switch (direction) {
						default:
							return false;
						case Directions.Up:
							if (currentStates[index].ThumbSticks.Right.Y >= stickDeadzone)
								return true;
							else
								return false;
						case Directions.Right:
							if (currentStates[index].ThumbSticks.Right.X >= stickDeadzone)
								return true;
							else
								return false;
						case Directions.Down:
							if (currentStates[index].ThumbSticks.Right.Y <= -stickDeadzone)
								return true;
							else
								return false;
						case Directions.Left:
							if (currentStates[index].ThumbSticks.Right.X <= -stickDeadzone)
								return true;
							else
								return false;
					}
				case DirectionalInputs.DPad:
					switch (direction) {
						default:
							return false;
						case Directions.Up:
							if (currentStates[index].DPad.Up == ButtonState.Pressed)
								return true;
							else
								return false;
						case Directions.Right:
							if (currentStates[index].DPad.Right == ButtonState.Pressed)
								return true;
							else
								return false;
						case Directions.Down:
							if (currentStates[index].DPad.Down == ButtonState.Pressed)
								return true;
							else
								return false;
						case Directions.Left:
							if (currentStates[index].DPad.Left == ButtonState.Pressed)
								return true;
							else
								return false;
					}
			}
		}
		/// <summary>
		/// Returns true if the given directional input is held and wasn't held the previous frame.
		/// </summary>
		/// <param name="playerIndex">Which controller would you like to check?</param>
		/// <param name="input">Which input would you like to check?</param>
		/// <param name="direction">Which direction would you like to check?</param>
		/// <param name="stickDeadzone">How much of a deadzone should be applied? (sticks only)</param>
		public static bool GetDirectionDown (XInputDotNetPure.PlayerIndex playerIndex, DirectionalInputs input, Directions direction, float stickDeadzone = .12f)
		{
			int index = (int) playerIndex;
			switch (input) {
				default:
					return false;
				case DirectionalInputs.LeftStick:
					switch (direction) {
						default:
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
		/// Returns true if the given directional input is released and was held the previous frame.
		/// </summary>
		/// <param name="playerIndex">Which controller would you like to check?</param>
		/// <param name="input">Which input would you like to check?</param>
		/// <param name="direction">Which direction would you like to check?</param>
		/// <param name="stickDeadzone">How much of a deadzone should be applied? (sticks only)</param>
		public static bool GetDirectionUp (XInputDotNetPure.PlayerIndex playerIndex, DirectionalInputs input, Directions direction, float stickDeadzone = .12f)
		{
			int index = (int) playerIndex;
			switch (input) {
				default:
					return false;
				case DirectionalInputs.LeftStick:
					switch (direction) {
						default:
							return false;
						case Directions.Up:
							if (prevStates[index].ThumbSticks.Left.Y >= stickDeadzone && currentStates[index].ThumbSticks.Left.Y < stickDeadzone)
								return true;
							else
								return false;
						case Directions.Right:
							if (prevStates[index].ThumbSticks.Left.X >= stickDeadzone && currentStates[index].ThumbSticks.Left.X < stickDeadzone)
								return true;
							else
								return false;
						case Directions.Down:
							if (prevStates[index].ThumbSticks.Left.Y <= -stickDeadzone && currentStates[index].ThumbSticks.Left.Y > -stickDeadzone)
								return true;
							else
								return false;
						case Directions.Left:
							if (prevStates[index].ThumbSticks.Left.X <= -stickDeadzone && currentStates[index].ThumbSticks.Left.X > -stickDeadzone)
								return true;
							else
								return false;
					}
				case DirectionalInputs.RightStick:
					switch (direction) {
						default:
							return false;
						case Directions.Up:
							if (prevStates[index].ThumbSticks.Right.Y >= stickDeadzone && currentStates[index].ThumbSticks.Right.Y < stickDeadzone)
								return true;
							else
								return false;
						case Directions.Right:
							if (prevStates[index].ThumbSticks.Right.X >= stickDeadzone && currentStates[index].ThumbSticks.Right.X < stickDeadzone)
								return true;
							else
								return false;
						case Directions.Down:
							if (prevStates[index].ThumbSticks.Right.Y <= -stickDeadzone && currentStates[index].ThumbSticks.Right.Y > -stickDeadzone)
								return true;
							else
								return false;
						case Directions.Left:
							if (prevStates[index].ThumbSticks.Right.X <= -stickDeadzone && currentStates[index].ThumbSticks.Right.X > -stickDeadzone)
								return true;
							else
								return false;
					}
				case DirectionalInputs.DPad:
					switch (direction) {
						default:
							return false;
						case Directions.Up:
							if (prevStates[index].DPad.Up == ButtonState.Pressed && currentStates[index].DPad.Up == ButtonState.Released)
								return true;
							else
								return false;
						case Directions.Right:
							if (prevStates[index].DPad.Right == ButtonState.Pressed && currentStates[index].DPad.Right == ButtonState.Released)
								return true;
							else
								return false;
						case Directions.Down:
							if (prevStates[index].DPad.Down == ButtonState.Pressed && currentStates[index].DPad.Down == ButtonState.Released)
								return true;
							else
								return false;
						case Directions.Left:
							if (prevStates[index].DPad.Left == ButtonState.Pressed && currentStates[index].DPad.Left == ButtonState.Released)
								return true;
							else
								return false;
					}
			}
		}

		/// <summary>
		/// Returns a value fro 0 through 1 depending on how far the given trigger is pressed.
		/// </summary>
		/// <param name="playerIndex">Which controller would you like to check?</param>
		/// <param name="trigger">Which trigger would you like to check?</param>
		/// <param name="deadzone">How much of a deadzone should be applied?</param>
		public static float GetTrigger (XInputDotNetPure.PlayerIndex playerIndex, Triggers trigger, float deadzone = .08f)
		{
			int index = (int) playerIndex;
			float triggerValue = 0;

			switch (trigger) {
				default:
					return 0;
				case Triggers.Left:
					triggerValue = currentStates[index].Triggers.Left;
					if (triggerValue >= deadzone)
						return triggerValue;
					else
						return 0;
				case Triggers.Right:
					triggerValue = currentStates[index].Triggers.Right;
					if (triggerValue >= deadzone)
						return triggerValue;
					else
						return 0;
			}
		}
		/// <summary>
		/// Returns true if the given trigger is held and wasn't held the previous frame.
		/// </summary>
		/// <param name="playerIndex">Which controller would you like to check?</param>
		/// <param name="trigger">Which trigger would you like to check?</param>
		/// <param name="deadzone">How much of a deadzone should be applied?</param>
		public static bool GetTriggerDown (XInputDotNetPure.PlayerIndex playerIndex, Triggers trigger, float deadzone = .08f)
		{
			int index = (int) playerIndex;

			switch (trigger) {
				default:
					return false;
				case Triggers.Left:
					if (prevStates[index].Triggers.Left < deadzone && currentStates[index].Triggers.Left >= deadzone)
						return true;
					else
						return false;
				case Triggers.Right:
					if (prevStates[index].Triggers.Right < deadzone && currentStates[index].Triggers.Right >= deadzone)
						return true;
					else
						return false;
			}
		}
		/// <summary>
		/// Returns true if the given trigger is released and was held the previous frame.
		/// </summary>
		/// <param name="playerIndex">Which controller would you like to check?</param>
		/// <param name="trigger">Which trigger would you like to check?</param>
		/// <param name="deadzone">How much of a deadzone should be applied?</param>
		public static bool GetTriggerUp (XInputDotNetPure.PlayerIndex playerIndex, Triggers trigger, float deadzone = .08f)
		{
			int index = (int) playerIndex;

			switch (trigger) {
				default:
					return false;
				case Triggers.Left:
					if (prevStates[index].Triggers.Left >= deadzone && currentStates[index].Triggers.Left < deadzone)
						return true;
					else
						return false;
				case Triggers.Right:
					if (prevStates[index].Triggers.Right >= deadzone && currentStates[index].Triggers.Right < deadzone)
						return true;
					else
						return false;
			}
		}

		/// <summary>
		/// Returns a value from -1.0 through 1.0 for the given axis.
		/// </summary>
		/// <param name="playerIndex">Which controller would you like to check?</param>
		/// <param name="axis">Which axis would you like to check?</param>
		/// <returns></returns>
		public static float GetAxis (XInputDotNetPure.PlayerIndex playerIndex, Axis axis)
		{
			int index = (int) playerIndex;
			float result = 0;

			switch (axis) {
				default:
					return 0;
				case Axis.LeftStickHorizontal:
					return currentStates[index].ThumbSticks.Left.X;
				case Axis.LeftStickVertical:
					return currentStates[index].ThumbSticks.Left.Y;
				case Axis.RightStickHorizontal:
					return currentStates[index].ThumbSticks.Right.X;
				case Axis.RightStickVertical:
					return currentStates[index].ThumbSticks.Right.Y;
				case Axis.DPadHorizontal:
					if (currentStates[index].DPad.Left == ButtonState.Pressed)
						result += -1;
					if (currentStates[index].DPad.Right == ButtonState.Pressed)
						result += 1;
					return result;
				case Axis.DPadVertical:
					if (currentStates[index].DPad.Down == ButtonState.Pressed)
						result += 1;
					if (currentStates[index].DPad.Up == ButtonState.Pressed)
						result += 1;
					return result;
				case Axis.Triggers:
					result += currentStates[index].Triggers.Right;
					result -= currentStates[index].Triggers.Left;
					return result;
			}
		}

		/// <summary>
		/// Returns true if A, B, X, Y, Start, Back, Bumpers, Stick presses or the Guide button is pressed.
		/// </summary>
		/// <param name="playerIndex">Which controller would you like to check?</param>
		public static bool GetAnyButton (XInputDotNetPure.PlayerIndex playerIndex)
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
		/// <summary>
		/// Returns true if A, B, X, Y, Start, Back, Bumpers, Stick presses or the Guide button is pressed while non of them were pressed the previous frame.
		/// </summary>
		/// <param name="playerIndex">Which controller would you like to check?</param>
		public static bool GetAnyButtonDown (XInputDotNetPure.PlayerIndex playerIndex)
		{
			int index = (int) playerIndex;

			if (prevStates[index].Buttons.A == ButtonState.Released && currentStates[index].Buttons.A == ButtonState.Pressed)
				return true;
			if (prevStates[index].Buttons.B == ButtonState.Released && currentStates[index].Buttons.B == ButtonState.Pressed)
				return true;
			if (prevStates[index].Buttons.X == ButtonState.Released && currentStates[index].Buttons.X == ButtonState.Pressed)
				return true;
			if (prevStates[index].Buttons.Y == ButtonState.Released && currentStates[index].Buttons.Y == ButtonState.Pressed)
				return true;
			if (prevStates[index].Buttons.Start == ButtonState.Released && currentStates[index].Buttons.Start == ButtonState.Pressed)
				return true;
			if (prevStates[index].Buttons.Back == ButtonState.Released && currentStates[index].Buttons.Back == ButtonState.Pressed)
				return true;
			if (prevStates[index].Buttons.RightShoulder == ButtonState.Released && currentStates[index].Buttons.RightShoulder == ButtonState.Pressed)
				return true;
			if (prevStates[index].Buttons.LeftShoulder == ButtonState.Released && currentStates[index].Buttons.LeftShoulder == ButtonState.Pressed)
				return true;
			if (prevStates[index].Buttons.LeftStick == ButtonState.Released && currentStates[index].Buttons.LeftStick == ButtonState.Pressed)
				return true;
			if (prevStates[index].Buttons.RightStick == ButtonState.Released && currentStates[index].Buttons.RightStick == ButtonState.Pressed)
				return true;
			if (prevStates[index].Buttons.Guide == ButtonState.Released && currentStates[index].Buttons.Guide == ButtonState.Pressed)
				return true;

			return false;
		}
		/// <summary>
		/// Returns true if A, B, X, Y, Start, Back, Bumpers, Stick presses or the Guide button are released while any of them were pressed the previous frame.
		/// </summary>
		/// <param name="playerIndex">Which controller would you like to check?</param>
		public static bool GetAnyButtonUp (XInputDotNetPure.PlayerIndex playerIndex)
		{
			int index = (int) playerIndex;

			if (prevStates[index].Buttons.A == ButtonState.Pressed && currentStates[index].Buttons.A == ButtonState.Released)
				return true;
			if (prevStates[index].Buttons.B == ButtonState.Pressed && currentStates[index].Buttons.B == ButtonState.Released)
				return true;
			if (prevStates[index].Buttons.X == ButtonState.Pressed && currentStates[index].Buttons.X == ButtonState.Released)
				return true;
			if (prevStates[index].Buttons.Y == ButtonState.Pressed && currentStates[index].Buttons.Y == ButtonState.Released)
				return true;
			if (prevStates[index].Buttons.Start == ButtonState.Pressed && currentStates[index].Buttons.Start == ButtonState.Released)
				return true;
			if (prevStates[index].Buttons.Back == ButtonState.Pressed && currentStates[index].Buttons.Back == ButtonState.Released)
				return true;
			if (prevStates[index].Buttons.RightShoulder == ButtonState.Pressed && currentStates[index].Buttons.RightShoulder == ButtonState.Released)
				return true;
			if (prevStates[index].Buttons.LeftShoulder == ButtonState.Pressed && currentStates[index].Buttons.LeftShoulder == ButtonState.Released)
				return true;
			if (prevStates[index].Buttons.LeftStick == ButtonState.Pressed && currentStates[index].Buttons.LeftStick == ButtonState.Released)
				return true;
			if (prevStates[index].Buttons.RightStick == ButtonState.Pressed && currentStates[index].Buttons.RightStick == ButtonState.Released)
				return true;
			if (prevStates[index].Buttons.Guide == ButtonState.Pressed && currentStates[index].Buttons.Guide == ButtonState.Released)
				return true;

			return false;
		}

		/// <summary>
		/// Returns true if any button, trigger or directional input is pressed or held in any direction.
		/// </summary>
		/// <param name="playerIndex">Which controller would you like to check?</param>
		/// <param name="stickDeadzone">How much of a deadzone should be applied? (sticks only)</param>
		public static bool GetAnyInput (XInputDotNetPure.PlayerIndex playerIndex, float stickDeadzone = .12f)
		{
			if (GetAnyButtonDown(playerIndex))
				return true;
			if (Mathf.Abs(GetAxis(playerIndex, Axis.LeftStickHorizontal)) >= stickDeadzone)
				return true;
			if (Mathf.Abs(GetAxis(playerIndex, Axis.LeftStickVertical)) >= stickDeadzone)
				return true;
			if (Mathf.Abs(GetAxis(playerIndex, Axis.RightStickHorizontal)) >= stickDeadzone)
				return true;
			if (Mathf.Abs(GetAxis(playerIndex, Axis.RightStickVertical)) >= stickDeadzone)
				return true;
			if (GetAxis(playerIndex, Axis.DPadHorizontal) != 0)
				return true;
			if (GetAxis(playerIndex, Axis.DPadVertical) != 0)
				return true;
			if (GetAxis(playerIndex, Axis.Triggers) != 0) {
				return true;
			}

			return false;
		}
		/// <summary>
		/// Returns true if any button, trigger or directional input is pressed or held in any direction while none were pressed or held the previous frame.
		/// </summary>
		/// <param name="playerIndex">Which controller would you like to check?</param>
		/// <param name="stickDeadzone">How much of a deadzone should be applied? (sticks only)</param>
		public static bool GetAnyInputDown (XInputDotNetPure.PlayerIndex playerIndex, float stickDeadzone = .12f)
		{
			if (GetAnyButtonDown(playerIndex))
				return true;
			if (GetDirectionDown(playerIndex, DirectionalInputs.LeftStick, Directions.Up))
				return true;
			if (GetDirectionDown(playerIndex, DirectionalInputs.LeftStick, Directions.Right))
				return true;
			if (GetDirectionDown(playerIndex, DirectionalInputs.LeftStick, Directions.Down))
				return true;
			if (GetDirectionDown(playerIndex, DirectionalInputs.LeftStick, Directions.Left))
				return true;
			if (GetDirectionDown(playerIndex, DirectionalInputs.RightStick, Directions.Up))
				return true;
			if (GetDirectionDown(playerIndex, DirectionalInputs.RightStick, Directions.Right))
				return true;
			if (GetDirectionDown(playerIndex, DirectionalInputs.RightStick, Directions.Down))
				return true;
			if (GetDirectionDown(playerIndex, DirectionalInputs.RightStick, Directions.Left))
				return true;
			if (GetDirectionDown(playerIndex, DirectionalInputs.DPad, Directions.Up))
				return true;
			if (GetDirectionDown(playerIndex, DirectionalInputs.DPad, Directions.Right))
				return true;
			if (GetDirectionDown(playerIndex, DirectionalInputs.DPad, Directions.Down))
				return true;
			if (GetDirectionDown(playerIndex, DirectionalInputs.DPad, Directions.Left))
				return true;
			if (GetTriggerDown(playerIndex, Triggers.Left))
				return true;
			if (GetTriggerDown(playerIndex, Triggers.Right))
				return true;

			return false;
		}
		/// <summary>
		/// Returns true if all button, trigger or directional input are released while any were pressed or held the previous frame.
		/// </summary>
		/// <param name="playerIndex">Which controller would you like to check?</param>
		/// <param name="stickDeadzone">How much of a deadzone should be applied? (sticks only)</param>
		public static bool GetAnyInputUp (XInputDotNetPure.PlayerIndex playerIndex, float stickDeadzone = .12f)
		{
			if (GetAnyButtonUp(playerIndex))
				return true;
			if (GetDirectionUp(playerIndex, DirectionalInputs.LeftStick, Directions.Up, stickDeadzone))
				return true;
			if (GetDirectionUp(playerIndex, DirectionalInputs.LeftStick, Directions.Right, stickDeadzone))
				return true;
			if (GetDirectionUp(playerIndex, DirectionalInputs.LeftStick, Directions.Down, stickDeadzone))
				return true;
			if (GetDirectionUp(playerIndex, DirectionalInputs.LeftStick, Directions.Left, stickDeadzone))
				return true;
			if (GetDirectionUp(playerIndex, DirectionalInputs.RightStick, Directions.Up, stickDeadzone))
				return true;
			if (GetDirectionUp(playerIndex, DirectionalInputs.RightStick, Directions.Right, stickDeadzone))
				return true;
			if (GetDirectionUp(playerIndex, DirectionalInputs.RightStick, Directions.Down, stickDeadzone))
				return true;
			if (GetDirectionUp(playerIndex, DirectionalInputs.RightStick, Directions.Left, stickDeadzone))
				return true;
			if (GetDirectionUp(playerIndex, DirectionalInputs.DPad, Directions.Up))
				return true;
			if (GetDirectionUp(playerIndex, DirectionalInputs.DPad, Directions.Right))
				return true;
			if (GetDirectionUp(playerIndex, DirectionalInputs.DPad, Directions.Down))
				return true;
			if (GetDirectionUp(playerIndex, DirectionalInputs.DPad, Directions.Left))
				return true;
			if (GetTriggerUp(playerIndex, Triggers.Left))
				return true;
			if (GetTriggerUp(playerIndex, Triggers.Right))
				return true;

			return false;
		}
	}

	public static class PlayerIndex
	{
		public static XInputDotNetPure.PlayerIndex One
		{
			get {
				return XInputDotNetPure.PlayerIndex.One;
			}
		}
		public static XInputDotNetPure.PlayerIndex Two
		{
			get {
				return XInputDotNetPure.PlayerIndex.Two;
			}
		}
		public static XInputDotNetPure.PlayerIndex Three
		{
			get {
				return XInputDotNetPure.PlayerIndex.Three;
			}
		}
		public static XInputDotNetPure.PlayerIndex Four
		{
			get {
				return XInputDotNetPure.PlayerIndex.Four;
			}
		}
	}
}