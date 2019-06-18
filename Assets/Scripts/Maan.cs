using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
using XInputDotNetPure;
using Utility;
using MultiAudioListener;

public class Maan : MonoBehaviour
{
	public PlayerIndex playerIndex = PlayerIndex.One;
	GamePadState gamePadState;
	Vector2 _leftStickInput;
	MaanManager manager;
	Transform otherPlayer;
	Rigidbody rig;
	Transform cameraAnchorTrans, cameraTrans;
	Vector3 _velocity, _cameraRotation;

	float movementSpeed = 17;

	float cameraMaxZAngle = 42, cameraMinZAngle = -32;
	float _cameraZAngle = 0;
	float cameraXSensitivity = 260, cameraZSensitivity = 150;

	public LineRenderer linkRenderer;

	Vector3 cameraDefaultPosition;
	float screenShakeMaxIntensity = .14f, visualMaxDistanceToCloud = 55, visualReactionMinDistanceToCloud = 12;
	float _screenShakeIntensity = 0, screenShakeIntensityGrowth = .18f;
	float _postProcessingWeight, postProcessingWeightGrowth = .3f;

	List<Kattoe> kattoesInRange = new List<Kattoe>();

	public GameObject pingExclamation, pingRoseFeedback;
	public AudioClip[] pingClips;
	Transform pingParent;
	List<GameObject> pingFeedbackPool = new List<GameObject>();
	SpriteRenderer pingRenderer;
	float pingActiveTime = .3f;

	public Transform[] kattoeAnchors;
	List<Transform> occupiedKattoeAnchors = new List<Transform>();

	public PostProcessProfile defaultPPProfile, nearCloudPPProfile;
	PostProcessVolume[] postProcessVolumes;

	public Image fadeToBlackDisplay;

	public void Init (MaanManager manager, Transform otherPlayer)
	{
		gamePadState = GamePad.GetState(playerIndex);

		rig = GetComponent<Rigidbody>();
		cameraAnchorTrans = transform.GetChild(0);
		cameraDefaultPosition = cameraAnchorTrans.localPosition;
		cameraTrans = cameraAnchorTrans.GetChild(0);

		postProcessVolumes = GetComponentsInChildren<PostProcessVolume>();
		postProcessVolumes[0].profile = defaultPPProfile;
		postProcessVolumes[0].weight = 1;
		postProcessVolumes[1].profile = nearCloudPPProfile;
		postProcessVolumes[1].weight = 0;

		fadeToBlackDisplay.enabled = false;

		pingParent = new GameObject("PingFeedbackParent").transform;
		pingRenderer = pingExclamation.GetComponent<SpriteRenderer>();
		pingRenderer.color = new Color(1, 1, 1, 0);

		this.manager = manager;
		this.otherPlayer = otherPlayer;
	}

	private void Update ()
	{
		gamePadState = GamePad.GetState(playerIndex);
		_leftStickInput = new Vector2(gamePadState.ThumbSticks.Left.X, gamePadState.ThumbSticks.Left.Y);

		ShowLink();

		if (GetAButtonDown()) {
			Ping();
		}

		if (gamePadState.Buttons.Back == ButtonState.Pressed) {
			SceneManager.LoadScene(1);
		}
	}

	void FixedUpdate ()
	{
		//_velocity = transform.rotation * CharacterMovement();
		_velocity = transform.forward * movementSpeed * _leftStickInput.magnitude;
		CameraMovement();
		rig.velocity = _velocity;
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

	float _cameraYAngle = 0;
	void CameraMovement ()
	{
		_cameraYAngle += gamePadState.ThumbSticks.Right.X * cameraXSensitivity * Time.deltaTime;
		if (_cameraYAngle < -180) {
			_cameraYAngle += 360;
		} else if (_cameraYAngle > 180) {
			_cameraYAngle -= 360;
		}
		_cameraZAngle = Mathf.Clamp(_cameraZAngle - gamePadState.ThumbSticks.Right.Y * cameraZSensitivity * Time.deltaTime,
			cameraMinZAngle, cameraMaxZAngle);
		cameraAnchorTrans.localRotation = Quaternion.Euler(new Vector3(_cameraZAngle, _cameraYAngle, 0));
		cameraTrans.LookAt(transform.position + Quaternion.Euler(_cameraZAngle, _cameraYAngle, 0) * Vector3.forward * 2.5f);
		
		float desiredAngle = Vector2.Angle(Vector2.up, _leftStickInput);
		if (_leftStickInput.x < 0)
			desiredAngle *= -1;
		desiredAngle = _cameraYAngle + desiredAngle;

		if (Util.Distance(transform.eulerAngles.y, desiredAngle) < 1) {
			return;
		}

		float rotation = desiredAngle < transform.eulerAngles.y ? -360 : 360;
		transform.Rotate(0, rotation * Time.deltaTime, 0);
		_cameraYAngle -= rotation * Time.deltaTime;

		//transform.Rotate(new Vector3(0, gamePadState.ThumbSticks.Right.X * cameraXSensitivity * Time.deltaTime, 0));
	}

	void ShowLink ()
	{
		Vector3[] positions;
		if (StaticData.playersAreLinked) {
			positions = new Vector3[] { transform.position + Vector3.up, otherPlayer.position + Vector3.up };
		} else {
			positions = new Vector3[] { transform.position, transform.position };
		}
		linkRenderer.SetPositions(positions);
	}

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
			_targetPPWeight *= .11f;
		}

		_screenShakeIntensity = Mathf.MoveTowards(_screenShakeIntensity, _targetIntensity, screenShakeIntensityGrowth * Time.deltaTime);
		_postProcessingWeight = Mathf.MoveTowards(_postProcessingWeight, _targetPPWeight, postProcessingWeightGrowth * Time.deltaTime);

		Vector3 screenShakeResult = new Vector3(Mathf.Sin(Time.time * 100), Mathf.Sin(Time.time * 120 + 1), 0);
		cameraAnchorTrans.localPosition = cameraDefaultPosition + screenShakeResult * _screenShakeIntensity;
		postProcessVolumes[1].weight = _postProcessingWeight;
	}

	public void EngagedByKattoe (Kattoe kattoe, bool engageOrDisengage)
	{
		if (engageOrDisengage) {
			kattoesInRange.Add(kattoe);
		} else {
			kattoesInRange.Remove(kattoe);
		}
	}
	void Ping ()
	{
		StopCoroutine(PingExclamationRoutine());
		StartCoroutine(PingExclamationRoutine());

		for (int i = 0; i < pingFeedbackPool.Count; i++) {
			if (!pingFeedbackPool[i].activeSelf) {
				StartCoroutine(PingRoseRoutine(pingFeedbackPool[i]));
				goto Finish;
			}
		}

		GameObject temp = Instantiate(pingRoseFeedback, transform.position, Quaternion.identity);
		temp.transform.parent = pingParent;
		StartCoroutine(PingRoseRoutine(temp));

		Finish:
		for (int i = 0; i < kattoesInRange.Count; i++) {
			kattoesInRange[i].ReceiveLure();
		}
	}
	IEnumerator PingExclamationRoutine ()
	{
		pingRenderer.color = Color.white;
		float colorFactor = 1 / pingActiveTime;

		for (float t = pingActiveTime; t > 0; t -= Time.deltaTime) {
			pingRenderer.color = new Color(1, 1, 1, t * colorFactor);
			yield return null;
		}

		pingRenderer.color = new Color(1,1,1,0);
	}
	IEnumerator PingRoseRoutine (GameObject go)
	{
		go.transform.position = transform.position + Vector3.up * .05f;
		go.GetComponent<MultiAudioSource>().AudioClip = Util.PickRandom(pingClips);
		go.GetComponent<MultiAudioSource>().Play();
		Animator[] temp = go.GetComponentsInChildren<Animator>();
		foreach (Animator a in temp) {
			a.Play("Swirl");
		}

		yield return new WaitForSeconds(2);
		go.SetActive(false);
	}

	public Transform KattoeRequestFlockAnchor ()
	{
		Transform attemptedAnchor = Util.PickRandom(kattoeAnchors);
		while (occupiedKattoeAnchors.Contains(attemptedAnchor)) {
			attemptedAnchor = Util.PickRandom(kattoeAnchors);
		}
		occupiedKattoeAnchors.Add(attemptedAnchor);
		StaticData.kattoesBondedToMaan++;
		return attemptedAnchor;
	}
	public void KattoeLeaveFlock (Transform anchor)
	{
		occupiedKattoeAnchors.Remove(anchor);
		StaticData.kattoesBondedToMaan--;
	}

	public IEnumerator FadeToBlack (float totalTime, float fadeTime = 1)
	{
		fadeToBlackDisplay.enabled = true;
		Color _color = fadeToBlackDisplay.color = Color.black;

		yield return new WaitForSeconds(totalTime - fadeTime);

		float transformationValue = 1 / fadeTime;
		for (float t = fadeTime; t > 0; t -= Time.deltaTime) {
			_color.a = t * transformationValue;
			fadeToBlackDisplay.color = _color;
			yield return null;
		}
		fadeToBlackDisplay.enabled = false;
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