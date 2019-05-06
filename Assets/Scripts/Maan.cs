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
	bool AButtonDown = false;
	float rotationSpeed = 220, movementSpeed = 20;

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
			//PING FOR ANIMALS
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