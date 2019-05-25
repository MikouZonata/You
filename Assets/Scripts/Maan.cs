using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
using XInputDotNetPure;
using Utility;

public class Maan : MonoBehaviour
{
	public PlayerIndex playerIndex = PlayerIndex.One;
	GamePadState gamePadState;
	MaanManager manager;
	Transform otherPlayer;
	Rigidbody rig;
	Transform cameraTrans;
	Vector3 _velocity, _cameraRotation;

	float movementSpeed = 20;

	List<Kattoe> kattoesInRange = new List<Kattoe>();

	public GameObject pingPrefab;
	float pingBaseSize = 1, pingMaxSize = 10;
	float pingExpandTime = .3f;

	public PostProcessProfile defaultPPProfile, nearCloudPPProfile;
	PostProcessVolume[] postProcessVolumes;

	public void Init (MaanManager manager, Transform otherPlayer)
	{
		gamePadState = GamePad.GetState(playerIndex);

		rig = GetComponent<Rigidbody>();
		cameraTrans = transform.GetChild(0);
		cameraDefaultPosition = cameraTrans.localPosition;

		postProcessVolumes = GetComponentsInChildren<PostProcessVolume>();
		postProcessVolumes[0].profile = defaultPPProfile;
		postProcessVolumes[0].weight = 1;
		postProcessVolumes[1].profile = nearCloudPPProfile;
		postProcessVolumes[1].weight = 0;

		this.manager = manager;
		this.otherPlayer = otherPlayer;
	}

	private void Update ()
	{
		gamePadState = GamePad.GetState(playerIndex);

		if (GetAButtonDown()) {
			Ping();
		}

		if (gamePadState.Buttons.Back == ButtonState.Pressed) {
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
		}
	}

	private void OnTriggerExit (Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("MaanKattoe")) {
			kattoesInRange.Remove(other.GetComponent<Kattoe>());
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

	float cameraMaxZAngle = 42, cameraMinZAngle = -32;
	float _cameraZAngle = 0;
	float cameraXSensitivity = 260, cameraZSensitivity = 150;
	void CameraMovement ()
	{
		transform.Rotate(new Vector3(0, gamePadState.ThumbSticks.Right.X * cameraXSensitivity * Time.deltaTime, 0));
		_cameraZAngle = Mathf.Clamp(_cameraZAngle - gamePadState.ThumbSticks.Right.Y * cameraZSensitivity * Time.deltaTime, cameraMinZAngle, cameraMaxZAngle);
		cameraTrans.localRotation = Quaternion.Euler(new Vector3(_cameraZAngle, 0, 0));
	}

	Vector3 cameraDefaultPosition;
	float screenShakeMaxIntensity = .16f, visualMaxDistanceToCloud = 70, visualReactionMinDistanceToCloud = 15;
	float _screenShakeIntensity = 0, screenShakeIntensityGrowth = .2f;
	float _postProcessingWeight, postProcessingWeightGrowth = .3f;
	public void VisualReactionToCloud (float distanceMaanToCloud)
	{
		float _targetIntensity;
		float _targetPPWeight;
		if (distanceMaanToCloud >= visualMaxDistanceToCloud) {
			_targetIntensity = _targetPPWeight = 0;
		} else if (distanceMaanToCloud <= visualReactionMinDistanceToCloud) {
			_targetIntensity = screenShakeMaxIntensity;
			_targetPPWeight = 1;
		} else {
			_targetIntensity = screenShakeMaxIntensity * (visualMaxDistanceToCloud - distanceMaanToCloud) / visualMaxDistanceToCloud;
			_targetPPWeight = (visualMaxDistanceToCloud - distanceMaanToCloud) / visualMaxDistanceToCloud;
		}

		if (StaticData.playersAreLinked) {
			_targetIntensity *= 0.08f;
			_targetPPWeight *= .1f;
		}

		_screenShakeIntensity = Mathf.MoveTowards(_screenShakeIntensity, _targetIntensity, screenShakeIntensityGrowth * Time.deltaTime);
		_postProcessingWeight = Mathf.MoveTowards(_postProcessingWeight, _targetPPWeight, postProcessingWeightGrowth * Time.deltaTime);

		Vector3 screenShakeResult = new Vector3(Mathf.Sin(Time.time * 100), Mathf.Sin(Time.time * 120 + 1), 0);
		cameraTrans.localPosition = cameraDefaultPosition + screenShakeResult * _screenShakeIntensity;
		postProcessVolumes[1].weight = _postProcessingWeight;
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