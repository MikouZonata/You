using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archive
{
	public class FinishBox : MonoBehaviour
	{
		bool playerOneIn = false, playerTwoIn = false;
		bool lapComplete = true;

		List<string> lapTimes = new List<string>();
		public UnityEngine.UI.Text timeDisplay;

		private void OnTriggerEnter (Collider other)
		{
			if (other.tag == "PlayerOne") {
				playerOneIn = true;
				if (playerTwoIn)
					lapComplete = true;
			}
			if (other.tag == "PlayerTwo") {
				playerTwoIn = true;
				if (playerOneIn)
					lapComplete = true;
			}
		}

		private void OnTriggerExit (Collider other)
		{
			if (other.tag == "PlayerOne") {
				playerOneIn = false;
				if (!playerTwoIn && lapComplete) {
					lapComplete = false;
					StartCoroutine(TimeLap());
				}
			}
			if (other.tag == "PlayerTwo") {
				playerTwoIn = false;
				if (!playerOneIn && lapComplete) {
					lapComplete = false;
					StartCoroutine(TimeLap());
				}
			}
		}

		IEnumerator TimeLap ()
		{
			float time = 0;
			while (!lapComplete) {
				time += Time.deltaTime;
				yield return null;
			}

			int minutes = Mathf.FloorToInt(time / 60);
			int seconds = Mathf.FloorToInt(time % 60);
			int milliSeconds = Mathf.FloorToInt(time * 1000 % 1000);
			string displayTime = "";
			if (minutes < 10)
				displayTime += "0";
			displayTime += minutes.ToString() + ":";
			if (seconds < 10)
				displayTime += "0";
			displayTime += seconds.ToString() + ":" + milliSeconds.ToString();

			//lapTimes.Add(displayTime);
			timeDisplay.text = displayTime;
		}
	}
}