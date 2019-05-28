namespace Utility
{
	using UnityEngine;
	using System.Collections.Generic;
	using System.Linq;

	public static class Util
	{
		//UTILITY METHODS
		public static T PickRandom<T> (params T[] values)
		{
			return values[Random.Range(0, values.Length)];
		}
		public static T[] PickRandom<T> (int howMany = 2, bool canContainDuplicates = true, params T[] values)
		{
			T[] result = new T[howMany];

			if (canContainDuplicates) {
				for (int i = 0; i < howMany; i++) {
					result[i] = values[Random.Range(0, values.Length)];
				}
			} else {
				List<int> indices = new List<int>();
				for (int i = 0; i < howMany; i++) {
					int attempt = Random.Range(0, values.Length);
					if (!indices.Contains(attempt)) {
						indices.Add(attempt);
					} else {
						i--;
					}
				}

				for (int i = 0; i < howMany; i++) {
					result[i] = values[indices[i]];
				}
			}

			return result;
		}

		public static List<T> Shuffle<T> (List<T> collection)
		{
			T[] resultArray = PickRandom(collection.Count, false, collection.ToArray());
			return resultArray.ToList();
		}
		public static T[] Shuffle<T> (params T[] collection)
		{
			T[] result = PickRandom(collection.Length, false, collection.ToArray());
			return result;
		}

		public static int ToInt (bool b)
		{
			if (b)
				return 1;
			else
				return 0;
		}

		public static bool ToBool (int i)
		{
			if (i == 1)
				return true;
			else
				return false;
		}

		public static int InvertZeroAndOne (int i)
		{
			if (i == 0)
				return 1;
			else
				return 0;
		}

		public static string ConvertSecondsToString (float time)
		{
			string result = "";

			int minutes = Mathf.FloorToInt(time / 60);
			int seconds = Mathf.FloorToInt(time % 60);
			int milliSeconds = Mathf.FloorToInt(time * 1000 % 1000);

			if (minutes < 10)
				result += "0";
			result += minutes.ToString() + ":";
			if (seconds < 10)
				result += "0";
			result += seconds.ToString() + ":" + milliSeconds.ToString();

			return result;
		}

		//EXTIONSION METHODS
		public static float SqrDistanceTo (this Vector3 lhs, Vector3 rhs)
		{
			return (lhs - rhs).sqrMagnitude;
		}

		public static int ToInt (this string str)
		{
			char[] letters = str.ToCharArray();
			int result = 0;

			for (int i = letters.Length - 1; i >= 0; i--) {
				switch (letters[i]) {
					case '0':
						break;
					case '1':
						result += (int) (1 * Mathf.Pow(10, letters.Length - 1 - i));
						break;
					case '2':
						result += (int) (2 * Mathf.Pow(10, letters.Length - 1 - i));
						break;
					case '3':
						result += (int) (3 * Mathf.Pow(10, letters.Length - 1 - i));
						break;
					case '4':
						result += (int) (4 * Mathf.Pow(10, letters.Length - 1 - i));
						break;
					case '5':
						result += (int) (5 * Mathf.Pow(10, letters.Length - 1 - i));
						break;
					case '6':
						result += (int) (6 * Mathf.Pow(10, letters.Length - 1 - i));
						break;
					case '7':
						result += (int) (7 * Mathf.Pow(10, letters.Length - 1 - i));
						break;
					case '8':
						result += (int) (8 * Mathf.Pow(10, letters.Length - 1 - i));
						break;
					case '9':
						result += (int) (9 * Mathf.Pow(10, letters.Length - 1 - i));
						break;
					default:
						Debug.LogWarning("Trying to convert non-number to int");
						break;
				}
			}

			return result;
		}
	}

	public class WaitForFrames : CustomYieldInstruction
	{
		float _timer = 0;
		float timeToWait;

		public WaitForFrames (int framesToWait, int frameRate = 60)
		{
			timeToWait = framesToWait / frameRate;
		}

		public override bool keepWaiting
		{
			get {
				if (_timer >= timeToWait) {
					return false;
				} else {
					_timer += Time.deltaTime;
					return true;
				}
			}
		}
	}
}