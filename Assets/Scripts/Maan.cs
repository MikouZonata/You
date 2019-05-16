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
	Transform cameraTrans;
	Vector3 _velocity, _cameraRotation;

	float triggerValue;
	float rotationSpeed = 220, movementSpeed = 20;

	List<Kattoe> kattoesInRange = new List<Kattoe>();
	List<Barrier> barriersInRange = new List<Barrier>();

	public GameObject pingPrefab;
	float pingBaseSize = 1, pingMaxSize = 10;
	float pingExpandTime = .3f;

	public void Init (MaanManager manager, Transform otherPlayer)
	{
		gamePadState = GamePad.GetState(playerIndex);

		rig = GetComponent<Rigidbody>();
		cameraTrans = transform.GetChild(0);

		this.manager = manager;
		this.otherPlayer = otherPlayer;
	}

	private void Update ()
	{
		gamePadState = GamePad.GetState(playerIndex);

		bool linked = false;
		if (Vector3.Distance(transform.position, otherPlayer.position) <= StaticData.distanceToLink) {
			linked = true;
		}

		if (GetAButtonDown()) {
			Ping();
		}

		for (int i = 0; i < barriersInRange.Count; i++) {
			barriersInRange[i].Disintegrate(linked);
			if (barriersInRange[i].destroyed) {
				barriersInRange.Remove(barriersInRange[i]);
				i--;
			}
		}

		if (gamePadState.Buttons.Back == ButtonState.Pressed) {
			StaticData.playerOptions[(int) playerIndex] = StaticData.PlayerOptions.Kevin;
			SceneManager.LoadScene(1);
		}
	}

	void FixedUpdate ()
	{
		_velocity = transform.rotation * CharacterMovement();
		CameraMovement();
		rig.velocity = _velocity;
	}

	private void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("MaanKattoe")) {
			kattoesInRange.Add(other.GetComponent<Kattoe>());
		} else if (other.gameObject.layer == LayerMask.NameToLayer("MaanBarrier")) {
			barriersInRange.Add(other.GetComponent<Barrier>());
			other.GetComponent<Barrier>().maanInRange = true;
		}
	}

	private void OnTriggerExit (Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("MaanKattoe")) {
			kattoesInRange.Remove(other.GetComponent<Kattoe>());
		} else if (other.gameObject.layer == LayerMask.NameToLayer("MaanBarrier")) {
			other.GetComponent<Barrier>().maanInRange = false;
			barriersInRange.Remove(other.GetComponent<Barrier>());
		}
	}

	Vector3 CharacterMovement ()
	{
		Vector3 result = Vector3.zero;
		result.x = gamePadState.ThumbSticks.Left.X * movementSpeed;
		result.z = gamePadState.ThumbSticks.Left.Y * movementSpeed;
		if (result.sqrMagnitude > movementSpeed * movementSpeed) {
			result = result.normalized * movementSpeed;
		}
		return result;
	}

	float cameraMaxZAngle = 45, cameraMinZAngle = -32;
	float _cameraZAngle = 0;
	float cameraXSensitivity = 380, cameraZSensitivity = 170;
	void CameraMovement ()
	{
		transform.Rotate(new Vector3(0, gamePadState.ThumbSticks.Right.X * cameraXSensitivity * Time.deltaTime, 0));
		_cameraZAngle = Mathf.Clamp(_cameraZAngle - gamePadState.ThumbSticks.Right.Y * cameraZSensitivity * Time.deltaTime, cameraMinZAngle, cameraMaxZAngle);
		cameraTrans.localRotation = Quaternion.Euler(new Vector3(_cameraZAngle, 0, 0));
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
		for (float t = 0; t < pingExpandTime; t += Time.deltaTime) {
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