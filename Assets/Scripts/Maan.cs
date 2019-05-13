using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using XInputDotNetPure;
using Utility;

public class Maan : MonoBehaviour
{
	public PlayerIndex playerIndex = PlayerIndex.Two;
	GamePadState gamePadState;
	MaanManager manager;
	Transform otherPlayer;
	Rigidbody rig;

	float triggerValue;
	float rotationSpeed = 220, movementSpeed = 20;

	List<Kattoe> kattoesInRange = new List<Kattoe>();
	List<Barrier> barriersInRange = new List<Barrier>();

	public GameObject pingPrefab;
	float pingBaseSize = 1, pingMaxSize = 10;
	float pingExpandTime = .3f;

	float distanceToLink = 9f;

	public void Init (MaanManager manager, Transform otherPlayer)
	{
		gamePadState = GamePad.GetState(playerIndex);

		rig = GetComponent<Rigidbody>();

		this.manager = manager;
		this.otherPlayer = otherPlayer;
	}

	private void Update ()
	{
		gamePadState = GamePad.GetState(playerIndex);


		if (Vector3.Distance(transform.position, otherPlayer.position) <= distanceToLink) {
			//WE ZIJN LINKED
		}

		if (GetAButtonDown()) {
			Ping();
		}

		if (gamePadState.Buttons.Back == ButtonState.Pressed) {
			StaticData.playerOptions[(int) playerIndex] = StaticData.PlayerOptions.Kevin;
			SceneManager.LoadScene(1);
		}
	}

	void FixedUpdate ()
	{
		transform.Rotate(0, gamePadState.ThumbSticks.Left.X * rotationSpeed * Time.fixedDeltaTime, 0);
		rig.velocity = transform.rotation * Vector3.forward * gamePadState.ThumbSticks.Left.Y * movementSpeed;
	}

	private void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("MaanKattoe")) {
			kattoesInRange.Add(other.GetComponent<Kattoe>());
		} else if (other.gameObject.layer == LayerMask.NameToLayer("MaanBarrier")) {
			barriersInRange.Add(other.GetComponent<Barrier>());
		}
	}

	private void OnTriggerExit (Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("MaanKattoe")) {
			kattoesInRange.Remove(other.GetComponent<Kattoe>());
		} else if (other.gameObject.layer == LayerMask.NameToLayer("MaanBarrier")) {
			barriersInRange.Remove(other.GetComponent<Barrier>());
		}
	}

	void Ping ()
	{
		StartCoroutine(PingRoutine(Instantiate(pingPrefab, transform.position, Quaternion.identity)));

		for (int i = 0; i < kattoesInRange.Count; i++) {
			if (kattoesInRange[i].Tempt()) {
				kattoesInRange[i].Attach(transform);
				kattoesInRange.RemoveAt(i);
				i--;
			}
		}
	}
	IEnumerator PingRoutine (GameObject pingGO)
	{
		float pingSize = pingBaseSize;
		for (float t = 0; t < pingExpandTime; t+=Time.deltaTime) {
			pingSize = Mathf.Lerp(pingBaseSize, pingMaxSize, t / pingExpandTime);
			pingGO.transform.localScale = new Vector3(pingSize, pingSize, pingSize);
			pingGO.transform.position = transform.position;
			yield return null;
		}
		Destroy(pingGO);
	}

	bool AButtonReleased = true;
	bool GetAButtonDown ()
	{
		if (AButtonReleased && gamePadState.Buttons.A == ButtonState.Pressed) {
			AButtonReleased = false;
			StartCoroutine(WaitForAButtonRelease());
			return true;
		} else
			return false;
	}
	IEnumerator WaitForAButtonRelease ()
	{
		while (true) {
			if (gamePadState.Buttons.A == ButtonState.Released) {
				AButtonReleased = true;
				break;
			} else
				yield return null;
		}
	}
}