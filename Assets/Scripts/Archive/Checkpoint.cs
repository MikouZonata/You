using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archive
{
	public class Checkpoint : MonoBehaviour
	{
		CheckpointManager manager;
		int checkpointIndex = 0;

		public void Init (CheckpointManager manager, int checkpointIndex)
		{
			this.manager = manager;
			this.checkpointIndex = checkpointIndex;
		}

		private void OnTriggerEnter (Collider other)
		{
			if (manager != null)
				manager.PlayerPassedCheckpoint(other.gameObject, checkpointIndex);
		}
	}
}