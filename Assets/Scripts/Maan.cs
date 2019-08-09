using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using XInputDotNetPure;
using Utility;
using FMODUnity;

public class Maan : MonoBehaviour, ICharacter
{
	[HideInInspector]
	public PlayerIndex playerIndex = PlayerIndex.One;
	GamePadState gamePadState;
	Vector2 _leftStickInput;
	MaanManager manager;
	Transform kevin;
	Rigidbody rig;
	Transform cameraAnchorTrans, cameraTrans;
	Vector3 _velocity, _cameraRotation;

	PauseScreen pauseScreen;
	bool pauseActive = true;

	float movementSpeed = 17;

	public GameObject cameraPrefab;
	float cameraMaxZAngle = 42, cameraMinZAngle = -40;
	float _cameraZAngle = 0;
	float cameraXSensitivity = 260, cameraZSensitivity = 150;

	public LineRenderer linkRenderer;

	Vector3 cameraDefaultPosition;
	const float screenShakeIntensityFactor = .17f;

	Transform modelTrans;
	float _modelYAngle = 0;

	List<Kattoe> kattoesInRange = new List<Kattoe>();

	public GameObject pingExclamation, pingRoseFeedback;
	Transform pingParent;
	List<GameObject> pingFeedbackPool = new List<GameObject>();
	SpriteRenderer pingRenderer;
	float pingActiveTime = .3f;

	public Transform[] kattoeAnchors;
	List<Transform> occupiedKattoeAnchors = new List<Transform>();
	public int KattoesBonded
	{
		get {
			return occupiedKattoeAnchors.Count;
		}
	}

	public Image fadeToBlackDisplay;

	public GameObject lovePrefab;
	const float loveMinTimebetweenLoveLinked = 2, loveMaxTimeBetweenLoveLinked = 4;
	const float loveMinTimebetweenLoveUnlinked = 8, loveMaxTimeBetweenLoveUnlinked = 11;
	float _loveTimer = 0, _loveTime = 0;
	bool _wasLinked = false;

	//FMOD
	string fmodWhistlePath = "event:/Maan/Calling_Cats_Maan";
	const int fmodWhistlePoolSize = 8;
	int _fmodWhistlePoolIndex = 0;
	FMOD.Studio.EventInstance[] fmodWhistleInstances = new FMOD.Studio.EventInstance[fmodWhistlePoolSize];

	public void Init (MaanManager manager, Transform kevin)
	{
		gamePadState = GamePad.GetState(playerIndex);

		rig = GetComponent<Rigidbody>();

		pauseScreen = GetComponent<PauseScreen>();
		ActivatePause();

		cameraAnchorTrans = Instantiate(cameraPrefab, transform.position, transform.rotation).transform;
		cameraTrans = cameraAnchorTrans.GetChild(0);
		cameraDefaultPosition = cameraTrans.localPosition;

		modelTrans = transform.GetChild(0);

		fadeToBlackDisplay.enabled = false;

		pingParent = new GameObject("PingFeedbackParent").transform;
		pingRenderer = pingExclamation.GetComponent<SpriteRenderer>();
		pingRenderer.color = new Color(1, 1, 1, 0);

		for (int i = 0; i < fmodWhistlePoolSize; i++) {
			fmodWhistleInstances[i] = RuntimeManager.CreateInstance(fmodWhistlePath);
		}

		this.manager = manager;
		this.kevin = kevin;
	}

	public void Destroy ()
	{
		StopAllCoroutines();

		Destroy(cameraAnchorTrans.gameObject);
		Destroy(pingParent.gameObject);

		foreach (FMOD.Studio.EventInstance instance in fmodWhistleInstances) {
			instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		}

		Destroy(gameObject);
	}

	void ActivatePause ()
	{
		pauseScreen.Activate();
		pauseActive = true;
	}
	public void DeactivatePause ()
	{
		pauseActive = false;
	}

	private void Update ()
	{
		if (!pauseActive) {
			gamePadState = GamePad.GetState(playerIndex);
			if (XInputDotNetExtender.instance.GetButtonDown(XInputDotNetExtender.Buttons.Start, playerIndex)) {
				ActivatePause();
			}
		} else
			gamePadState = new GamePadState();
		_leftStickInput = new Vector2(gamePadState.ThumbSticks.Left.X, gamePadState.ThumbSticks.Left.Y);

		CameraMovement();
		ModelRotation();
		ShowLink();
		LoveClock();

		if (XInputDotNetExtender.instance.GetButtonDown(XInputDotNetExtender.Buttons.A, playerIndex)) {
			Ping();
		}
	}

	void FixedUpdate ()
	{
		_velocity = transform.rotation * CharacterMovement();
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
		result = Quaternion.Euler(0, _cameraYAngle, 0) * result;
		return result;
	}

	float _cameraYAngle = 0;
	void CameraMovement ()
	{
		cameraAnchorTrans.position = transform.position;

		_cameraYAngle += gamePadState.ThumbSticks.Right.X * cameraXSensitivity * Time.deltaTime;
		_modelYAngle -= gamePadState.ThumbSticks.Right.X * cameraXSensitivity * Time.deltaTime;
		if (_cameraYAngle < -180) {
			_cameraYAngle += 360;
		} else if (_cameraYAngle > 180) {
			_cameraYAngle -= 360;
		}
		_cameraZAngle = Mathf.Clamp(_cameraZAngle - gamePadState.ThumbSticks.Right.Y * cameraZSensitivity * Time.deltaTime,
			cameraMinZAngle, cameraMaxZAngle);
		cameraAnchorTrans.rotation = Quaternion.Euler(new Vector3(_cameraZAngle, _cameraYAngle, 0));
		cameraTrans.LookAt(transform.position + Quaternion.Euler(_cameraZAngle, _cameraYAngle, 0) * Vector3.forward * 2.5f);
	}

	void ModelRotation ()
	{
		if (_leftStickInput.magnitude > 0.1f) {
			_modelYAngle = Vector2.Angle(_leftStickInput, Vector2.up);
			if (_leftStickInput.x < 0) {
				_modelYAngle *= -1;
			}
		}
		modelTrans.rotation = Quaternion.Euler(0, _modelYAngle + _cameraYAngle, 0);
	}

	void ShowLink ()
	{
		Vector3[] positions;
		if (StaticData.playersAreLinked) {
			positions = new Vector3[] { transform.position + Vector3.up, kevin.position + Vector3.up };
		} else {
			positions = new Vector3[] { transform.position, transform.position };
		}
		linkRenderer.SetPositions(positions);
	}

	void LoveClock ()
	{
		if (_wasLinked && !StaticData.playersAreLinked) {
			_loveTime = Random.Range(loveMinTimebetweenLoveUnlinked, loveMaxTimeBetweenLoveUnlinked);
		} else if (!_wasLinked && StaticData.playersAreLinked) {
			_loveTime = Random.Range(loveMinTimebetweenLoveLinked, loveMaxTimeBetweenLoveLinked);
			ShowLove();
		}
		_wasLinked = StaticData.playersAreLinked;

		_loveTimer += Time.deltaTime;
		if (_loveTimer > _loveTime) {
			ShowLove();
			if (StaticData.playersAreLinked)
				_loveTime = Random.Range(loveMinTimebetweenLoveLinked, loveMaxTimeBetweenLoveLinked);
			else
				_loveTime = Random.Range(loveMinTimebetweenLoveUnlinked, loveMaxTimeBetweenLoveUnlinked);
			_loveTimer = 0;
		}
	}

	public void ScreenShake (float intensity)
	{
		Vector3 screenShakeResult = new Vector3(Mathf.Sin(Time.time * 100), Mathf.Sin(Time.time * 120 + 1), 0);
		cameraTrans.localPosition = cameraDefaultPosition + screenShakeResult * intensity * screenShakeIntensityFactor;
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

		fmodWhistleInstances[_fmodWhistlePoolIndex].start();
		_fmodWhistlePoolIndex++;
		if (_fmodWhistlePoolIndex >= fmodWhistlePoolSize)
			_fmodWhistlePoolIndex = 0;

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

		pingRenderer.color = new Color(1, 1, 1, 0);
	}
	IEnumerator PingRoseRoutine (GameObject go)
	{
		go.transform.position = transform.position + Vector3.up * .05f;
		Animator[] temp = go.GetComponentsInChildren<Animator>();
		foreach (Animator a in temp) {
			a.Play("Swirl");
		}

		yield return new WaitForSeconds(2);
		go.SetActive(false);
	}

	void ShowLove ()
	{
		Love love = Instantiate(lovePrefab, transform.position + new Vector3(Random.Range(-1.0f, 1.0f), 2.5f, Random.Range(-1.0f, 1.0f)), cameraTrans.rotation).GetComponent<Love>();
		love.Init(kevin);
	}

	public Transform KattoeRequestFlockAnchor ()
	{
		Transform attemptedAnchor = Util.PickRandom(kattoeAnchors);
		while (occupiedKattoeAnchors.Contains(attemptedAnchor)) {
			attemptedAnchor = Util.PickRandom(kattoeAnchors);
		}
		occupiedKattoeAnchors.Add(attemptedAnchor);
		return attemptedAnchor;
	}
	public void KattoeLeaveFlock (Transform anchor)
	{
		occupiedKattoeAnchors.Remove(anchor);
	}

	public IEnumerator FadeToBlack (float totalTime, Color fadeColor, float fadeTime = 1)
	{
		fadeToBlackDisplay.enabled = true;
		Color _color = fadeToBlackDisplay.color = fadeColor;

		yield return new WaitForSeconds(totalTime - fadeTime);

		float transformationValue = 1 / fadeTime;
		for (float t = fadeTime; t > 0; t -= Time.deltaTime) {
			_color.a = t * transformationValue;
			fadeToBlackDisplay.color = _color;
			yield return null;
		}
		fadeToBlackDisplay.enabled = false;
	}
}