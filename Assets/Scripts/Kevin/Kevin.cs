using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using XInputDotNetPure;
using Utility;
using FMODUnity;
using XInputDotNetExtended;

public class Kevin : MonoBehaviour, ICharacter
{
	public XInputDotNetPure.PlayerIndex playerIndex = XInputDotNetPure.PlayerIndex.One;
	KevinManager manager;
	Transform maan;
	Rigidbody rig;

	const float FoVBaseDegrees = 60, FoVDegreesPerVelocity = .4f;
	Camera mainCam;
	Transform mainCamTrans;
	Quaternion mainCamDefaultRot;

	ParticleSystem sideDriftParticles;
	ParticleSystem.EmissionModule sideDriftEmissionModule;
	float _sideDriftDefaultEmission;

	PauseScreen pauseScreen;
	bool pauseActive = true;

	public LineRenderer linkRenderer;

	public Transform modelAnchor;
	const float modelFatigueForwardsFactor = 18;
	const float modelMaxForwardsAngle = 9, modelMaxSidewaysAngle = 9;
	const float throttleMaxForwardSpeed = 28;
	const float throttleAcceleration = 2, throttleNaturalDecceleration = 18;
	Vector3 _velocity = Vector3.zero;
	float _triggerValue = 0;
	float _speedPoint = 0, _throttleSpeed = 0;

	public GameObject fatigueSmokeGO;
	const float fatigueRecoverRate = .167f, fatigueIncreaseRate = .011f;
	const float fatigueFirstPlacePenalty = .006f;
	const float fatigueRechargePerPickup = 0.04f;
	const float fatigueSlowFactorMin = .5f;
	const float fatigueSmokeThreshold = .68f;
	bool overrideFatigue = false;
	bool fatigueActive = false;
	float _fatigue = 0f;
	bool _fatigueSmokePlaying = false;

	const float maxTurnRate = 192, minTurnRate = 76;
	const float turnRateLossPerVelocity = 4.22f;
	const float sideDriftPerVelocity = .42f, sideDriftMaxVelocity = 22, sideDriftMinVelocity = 12;
	const float driftingMaxSideFactor = 5.2f, driftingMaxTurnFactor = 1.24f, driftingTimeToMax = .12f;
	float driftingSideAcceleration, driftingTurnAcceleration;
	float _steeringSideDrift = 0;
	float _driftingTurnFactor = 1, _driftingSideFactor = 1;

	const float struggleMinTime = 1.2f, struggleMaxTime = 3.0f;
	float _struggleTimer = 0, _struggleTime = 10;
	Vector3 _struggleVelocity = Vector3.zero;

	public Transform leaderboardFrame;

	//Loving is currently disabled
	public GameObject lovePrefab;
	bool _goingToLove = false;
	bool _lovedOnPass = false;

	//FMOD
	const string fmodHoverPath = "event:/Kevin/Hover_Engine";
	const float fmodHoverPitchThreshold = .65f;
	FMOD.Studio.EventInstance fmodHoverInstance;
	FMOD.Studio.ParameterInstance fmodHoverPitch;
	bool _fmodHoverPlaying = false;

	public void Init (KevinManager manager, Transform maan)
	{
		rig = GetComponent<Rigidbody>();
		mainCam = transform.GetChild(0).GetComponent<Camera>();
		mainCamTrans = mainCam.transform;
		mainCamDefaultRot = mainCamTrans.rotation;

		sideDriftParticles = GetComponentInChildren<ParticleSystem>();
		sideDriftEmissionModule = sideDriftParticles.emission;
		_sideDriftDefaultEmission = sideDriftEmissionModule.rateOverTime.constant;
		sideDriftEmissionModule.rateOverTime = 0;
		var emission = sideDriftParticles.emission;
		emission.rateOverTime = 0;

		driftingSideAcceleration = (driftingMaxSideFactor - 1) / driftingTimeToMax;
		driftingTurnAcceleration = (driftingMaxTurnFactor - 1) / driftingTimeToMax;

		pauseScreen = GetComponent<PauseScreen>();
		ActivatePause();

		_struggleTime = Random.Range(struggleMinTime, struggleMaxTime);

		fmodHoverInstance = RuntimeManager.CreateInstance(fmodHoverPath);
		fmodHoverInstance.getParameter("Engine_Pitch", out fmodHoverPitch);

		fatigueSmokeGO.SetActive(false);

		this.manager = manager;
		this.maan = maan;
	}

	public void Destroy ()
	{
		StopAllCoroutines();

		fmodHoverInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		Destroy(gameObject);
	}

	private void Update ()
	{
		if (!pauseActive) {
			if (XInputEX.GetButtonDown(playerIndex, XInputEX.Buttons.Start)) {
				ActivatePause();
			}
		}

		CameraFoV();
		ShowLink();
		ModelRotation();
		Fatigue();
		//Loving();

		if (!StaticData.playersAreLinked && rig.velocity.sqrMagnitude < 1) {
			_struggleTimer += Time.deltaTime;
			if (_struggleTimer > _struggleTime) {
				StartCoroutine(Struggle());
				_struggleTime = Random.Range(struggleMinTime, struggleMaxTime);
				_struggleTimer = 0;
			}
		} else {
			_struggleTimer = 0;
			_struggleTime = Random.Range(struggleMinTime, struggleMaxTime);
		}
	}

	void FixedUpdate ()
	{
		_triggerValue = XInputEX.GetTrigger(playerIndex, XInputEX.Triggers.Right);

		Throttle();

		_velocity.z = _throttleSpeed * FatigueSlowFactor();  //_boostSpeed;
		_velocity.x = _steeringSideDrift;
		_velocity += _struggleVelocity;

		transform.Rotate(new Vector3(0, Steering(), 0));

		rig.velocity = transform.rotation * _velocity;
	}

	private void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Pickup") {
			manager.PickUpPickup(Util.ToInt(transform.name), Util.ToInt(other.name));
			_fatigue = Mathf.Clamp(_fatigue - fatigueRechargePerPickup, 0, 1);
		}
	}

	void ActivatePause ()
	{
		pauseActive = true;
		pauseScreen.Activate();
	}

	public void DeactivatePause ()
	{
		pauseActive = false;
	}

	void CameraFoV ()
	{
		mainCam.fieldOfView = FoVBaseDegrees + _velocity.z * FoVDegreesPerVelocity;
	}

	void ShowLink ()
	{
		Vector3[] positions;
		if (StaticData.playersAreLinked) {
			positions = new Vector3[] { transform.position + Vector3.up, maan.position + Vector3.up };
		} else {
			positions = new Vector3[] { transform.position, transform.position };
		}
		linkRenderer.SetPositions(positions);
	}

	void ModelRotation ()
	{
		float xRot = _fatigue * modelFatigueForwardsFactor + modelMaxForwardsAngle * XInputEX.GetTrigger(playerIndex, XInputEX.Triggers.Right);
		float yRot = modelMaxSidewaysAngle * XInputEX.GetAxis(playerIndex, XInputEX.Axis.LeftStickHorizontal);
		modelAnchor.rotation = transform.rotation * Quaternion.Euler(xRot, yRot, 0);
	}

	void Throttle ()
	{
		if (_triggerValue > 0) {
			_speedPoint = Mathf.MoveTowards(_speedPoint, _triggerValue, throttleAcceleration * Time.deltaTime);
			_throttleSpeed = Mathf.Pow(_speedPoint, .5f) * throttleMaxForwardSpeed;

		} else {
			_throttleSpeed = Mathf.MoveTowards(_throttleSpeed, 0, throttleNaturalDecceleration * Time.deltaTime);
			_speedPoint = Mathf.Pow(_throttleSpeed / throttleMaxForwardSpeed, 2);
		}

		ThrottleAudio();
	}

	void ThrottleAudio ()
	{
		if (XInputEX.GetTrigger(playerIndex, XInputEX.Triggers.Right) == 0 && _fmodHoverPlaying) {
			_fmodHoverPlaying = false;
			fmodHoverInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		} else if (XInputEX.GetTrigger(playerIndex, XInputEX.Triggers.Right) > 0 && !_fmodHoverPlaying) {
			_fmodHoverPlaying = true;
			fmodHoverInstance.start();
		}

		float _hoverPitch = 1;
		if (_fatigue > fmodHoverPitchThreshold) {
			_hoverPitch = 1 - (_fatigue - fmodHoverPitchThreshold) * 1 / fmodHoverPitchThreshold;
		}
		fmodHoverPitch.setValue(_hoverPitch);
	}

	void Fatigue ()
	{
		if (Input.GetKeyDown(KeyCode.F)) {
			overrideFatigue = !overrideFatigue;
		}

		if (!overrideFatigue && fatigueActive) {
			if (StaticData.playersAreLinked) {
				_fatigue = Mathf.MoveTowards(_fatigue, 0, fatigueRecoverRate * Time.deltaTime);
			} else {
				_fatigue = Mathf.MoveTowards(_fatigue, 1, (fatigueIncreaseRate + (manager.GetKevinRank() == 0 ? fatigueFirstPlacePenalty : 0)) * Time.deltaTime);
			}
		} else {
			//Debug Fatigue options
			if (Input.GetKey(KeyCode.LeftArrow)) {
				_fatigue -= Time.deltaTime * .5f;
			} else if (Input.GetKey(KeyCode.RightArrow)) {
				_fatigue += Time.deltaTime * .5f;
			}
			if (XInputEX.GetButtonDown(playerIndex, XInputEX.Buttons.X)) {
				_fatigue -= Time.deltaTime * .5f;
			}
			_fatigue = Mathf.Clamp(_fatigue, 0, 1);
		}

		if (!_fatigueSmokePlaying && _fatigue > fatigueSmokeThreshold) {
			fatigueSmokeGO.SetActive(true);
			_fatigueSmokePlaying = true;
		}
		if (_fatigueSmokePlaying && _fatigue < fatigueSmokeThreshold) {
			fatigueSmokeGO.SetActive(false);
			_fatigueSmokePlaying = false;
		}
	}
	float FatigueSlowFactor ()
	{
		float result;
		result = 1 - (1 - fatigueSlowFactorMin) * _fatigue;
		return result;
	}

	float Steering ()
	{
		float result = 0;

		float turnRate = Mathf.Clamp(maxTurnRate - Mathf.Abs(turnRateLossPerVelocity * _velocity.z), minTurnRate, maxTurnRate);
		result = XInputEX.GetAxis(playerIndex, XInputEX.Axis.LeftStickHorizontal) * turnRate * Time.deltaTime;

		float trimmedVelocity = Mathf.MoveTowards(_velocity.z, 0, sideDriftMinVelocity);
		trimmedVelocity = Mathf.Clamp(trimmedVelocity, -sideDriftMaxVelocity + sideDriftMinVelocity, sideDriftMaxVelocity - sideDriftMinVelocity);
		_steeringSideDrift = -XInputEX.GetAxis(playerIndex, XInputEX.Axis.LeftStickHorizontal) * trimmedVelocity * sideDriftPerVelocity;

		if (XInputEX.GetTrigger(playerIndex, XInputEX.Triggers.Left) != 0) {
			_driftingTurnFactor = Mathf.MoveTowards(_driftingTurnFactor, driftingMaxTurnFactor, driftingTurnAcceleration * Time.deltaTime);
			_driftingSideFactor = Mathf.MoveTowards(_driftingSideFactor, driftingMaxSideFactor, driftingSideAcceleration * Time.deltaTime);
		} else {
			_driftingTurnFactor = Mathf.MoveTowards(_driftingTurnFactor, 1, driftingTurnAcceleration * Time.deltaTime);
			_driftingSideFactor = Mathf.MoveTowards(_driftingSideFactor, 1, driftingSideAcceleration * Time.deltaTime);
		}
		result *= _driftingTurnFactor;
		_steeringSideDrift *= _driftingSideFactor;

		//To-Do: SideDriftParticles turn themselves off/on seemingly randomly
		//if (Mathf.Abs(_steeringSideDrift) >= 2.5f) {
		//	sideDriftParticles.Play();
		//	sideDriftEmissionModule.rateOverTime = sideDriftDefaultEmission;
		//} else {
		//	sideDriftParticles.Stop();
		//	sideDriftEmissionModule.rateOverTime = 0;
		//}

		return result;
	}

	//Loving is disabled and needs to be redesigned
	void Loving ()
	{
		if (!_lovedOnPass && _fatigue > .3f && StaticData.playersAreLinked) {
			_lovedOnPass = true;
			ShowLove();
		}
		if (_lovedOnPass && !StaticData.playersAreLinked)
			_lovedOnPass = false;

		if (!_goingToLove && _fatigue > .7f && StaticData.playersAreLinked)
			_goingToLove = true;
		if (_goingToLove && _fatigue < .2f && !StaticData.playersAreLinked) {
			_goingToLove = false;
			ShowLove();
			ShowLove();
			ShowLove();
		}
	}
	void ShowLove ()
	{
		Love love = Instantiate(lovePrefab, transform.position + new Vector3(Random.Range(-1.0f, 1.0f), 1.5f, Random.Range(-1.0f, 1.0f)), transform.rotation).GetComponent<Love>();
		love.Init(maan);
	}

	IEnumerator Struggle ()
	{
		float struggleTime = Random.Range(0.08f, 0.16f);
		Vector2 radial = Random.insideUnitCircle;
		Vector3 velocity = new Vector3(radial.x, 0, Mathf.Abs(radial.y)).normalized * Random.Range(3, 7);
		float rotation = Random.Range(-88f, 88f);

		_struggleVelocity = velocity;

		for (float t = 0; t < struggleTime; t += Time.deltaTime) {
			transform.Rotate(0, rotation * Time.deltaTime, 0);

			if (XInputEX.GetAxis(playerIndex, XInputEX.Axis.LeftStickHorizontal) != 0
				|| XInputEX.GetAxis(playerIndex, XInputEX.Axis.LeftStickVertical) != 0
				|| XInputEX.GetTrigger(playerIndex, XInputEX.Triggers.Left) != 0
				|| XInputEX.GetTrigger(playerIndex, XInputEX.Triggers.Right) != 0) {
				break;
			}

			yield return null;
		}

		_struggleVelocity = Vector3.zero;
	}

	public void ActivateFatigue ()
	{
		fatigueActive = true;
	}
	public Transform GetTransform ()
	{
		return transform;
	}
	public float GetFatigue ()
	{
		return _fatigue;
	}
}