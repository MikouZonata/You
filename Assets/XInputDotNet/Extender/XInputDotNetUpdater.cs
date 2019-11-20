using UnityEngine;

namespace XInputDotNetPure
{
	public class XInputDotNetUpdater : MonoBehaviour
	{
		public delegate void Updater ();
		public static event Updater OnUpdate;

		private void Awake ()
		{
			DontDestroyOnLoad(gameObject);
		}

		void Update ()
		{
			if (OnUpdate != null) {
				OnUpdate();
			}
		}
	}
}