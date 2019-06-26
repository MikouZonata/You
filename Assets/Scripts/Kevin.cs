using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using XInputDotNetPure;
using Utility;
using MultiAudioListener;
using FMODUnity;

public class Kevin : MonoBehaviour
{
	public PlayerIndex playerIndex = PlayerIndex.Two;
	GamePadState gamePadState;
	KevinManager manager;
	Transform otherPlayer;
	Rigidbody rig;

	Camera mainCam;
	Transform mainCamTrans;
	const float FoVBaseDegrees = 60, FoVDegreesPerVelocity = .3f;
	Quaternion mainCamDefaultRot;

	ParticleSystem sideDriftParticles;
	ParticleSystem.EmissionModule sideDriftEmissionModule;
	float sideDriftDefaultEmission;

	public LineRenderer linkRenderer;

	public Transform modelAnchor;
	const float modelMaxForwardsAngle = 12, modelMaxSidewaysAngle = 9;
	Vector3 _velocity = Vector3.zero;
	float _triggerValue;
	const float throttleMaxForwardSpeed = 28;
	const float throttleAcceleration = 2, throttleNaturalDecceleration = 18;
	float _speedPoint = 0, _throttleSpeed = 0;
	float throttleTrailMaxTime = .08f;

	MultiAudioSource throttleAudioSource;
	float throttleAudioMaxVolume = .4f;

	float _fatigue = .5f;
	const float fatigueRecoverRate = .167f, fatigueIncreaseRate = .011f;
	const float fatigueRechargePerPickup = 0.04f;
	const float fatigueSlowFactorMin = .4f;

	const float maxTurnRate = 192, minTurnRate = 76;
	const float turnRateLossPerVelocity = 4.22f;
	float _steeringSideDrift = 0, sideDriftPerVelocity = .42f, sideDriftMaxVelocity = 22, sideDriftMinVelocity = 12;
	float driftingMaxSideFactor = 5.2f, _driftingSideFactor = 1, driftingMaxTurnFactor = 1.24f, driftingTimeToMax = .12f;
	float _driftingTurnFactor = 1;
	float driftingSideAcceleration, driftingTurnAcceleration;

	float _struggleTimer = 0, struggleTime = 10;
	const float struggleMinTime = 1.2f, struggleMaxTime = 3.0f;
	Vector3 _struggleVelocity = Vector3.zero;

	bool selectButtonReleased = false;

	public Transform leaderboard;

	//FMOD
	string fmodHoverPath = "event:/Kevin/Hover_Engine";
	FMOD.Studio.EventInstance fmodHoverInstance;
	FMOD.Studio.ParameterInstance fmodHoverPitch;
	float fmodHoverPitchThreshold = .65f;
	bool fmodHoverPlaying = false;

	public void Init (KevinManager manager, Transform otherPlayer)
	{
		gamePadState = GamePad.GetState(playerIndex);

		rig = GetComponent<Rigidbody>();
		mainCam = transform.GetChild(0).GetComponent<Camera>();
		mainCamTrans = mainCam.transform;
		mainCamDefaultRot = mainCamTrans.rotation;

		throttleAudioSource = GetComponentInChildren<MultiAudioSource>();

		sideDriftParticles = GetComponentInChildren<ParticleSystem>();
		sideDriftEmissionModule = sideDriftParticles.emission;
		sideDriftDefaultEmission = sideDriftEmissionModule.rateOverTime.constant;
		sideDriftEmissionModule.rateOverTime = 0;
		var emission = sideDriftParticles.emission;
		emission.rateOverTime = 0;

		driftingSideAcceleration = (driftingMaxSideFactor - 1) / driftingTimeToMax;
		driftingTurnAcceleration = (driftingMaxTurnFactor - 1) / driftingTimeToMax;

		struggleTime = Random.Range(struggleMinTime, struggleMaxTime);

		fmodHoverInstance = RuntimeManager.CreateInstance(fmodHoverPath);
		fmodHoverInstance.getParameter("Engine_Pitch", out fmodHoverPitch);

		this.manager = manager;
		this.otherPlayer = otherPlayer;
	}

	private void Update ()
	{
		gamePadState = GamePad.GetState(playerIndex);

		CameraFoV();
		ShowLink();
		ModelRotation();

		if (!StaticData.playersAreLinked && rig.velocity.sqrMagnitude < 1) {
			_struggleTimer += Time.deltaTime;
			if (_struggleTimer > struggleTime) {
				StartCoroutine(Struggle());
				struggleTime = Random.Range(struggleMinTime, struggleMaxTime);
				_struggleTimer = 0;
			}
		} else {
			_struggleTimer = 0;
			struggleTime = Random.Range(struggleMinTime, struggleMaxTime);
		}
	}

	void FixedUpdate ()
	{
		_triggerValue = gamePadState.Triggers.Right;

		Throttle();
		Fatigue();

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

	void CameraFoV ()
	{
		mainCam.fieldOfView = FoVBaseDegrees + _velocity.z * FoVDegreesPerVelocity;
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

	void ModelRotation ()
	{
		float xRot = modelMaxForwardsAngle * gamePadState.Triggers.Right;
		float yRot = modelMaxSidewaysAngle * gamePadState.ThumbSticks.Left.X;
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
		if (gamePadState.Triggers.Right == 0 && fmodHoverPlaying) {
			fmodHoverPlaying = false;
			fmodHoverInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
		if (gamePadState.Triggers.Right > 0 && !fmodHoverPlaying) {
			fmodHoverPlaying = true;
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
		if (StaticData.playersAreLinked) {
			_fatigue = Mathf.MoveTowards(_fatigue, 0, fatigueRecoverRate * Time.deltaTime);
		} else {
			_fatigue = Mathf.MoveTowards(_fatigue, 1, fatigueIncreaseRate * Time.deltaTime);
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
		result = gamePadState.ThumbSticks.Left.X * turnRate * Time.deltaTime;

		float trimmedVelocity = Mathf.MoveTowards(_velocity.z, 0, sideDriftMinVelocity);
		trimmedVelocity = Mathf.Clamp(trimmedVelocity, -sideDriftMaxVelocity + sideDriftMinVelocity, sideDriftMaxVelocity - sideDriftMinVelocity);
		_steeringSideDrift = -gamePadState.ThumbSticks.Left.X * trimmedVelocity * sideDriftPerVelocity;

		if (gamePadState.Triggers.Left > 0.1f) {
			_driftingTurnFactor = Mathf.MoveTowards(_driftingTurnFactor, driftingMaxTurnFactor, driftingTurnAcceleration * Time.deltaTime);
			_driftingSideFactor = Mathf.MoveTowards(_driftingSideFactor, driftingMaxSideFactor, driftingSideAcceleration * Time.deltaTime);
		} else {
			_driftingTurnFactor = Mathf.MoveTowards(_driftingTurnFactor, 1, driftingTurnAcceleration * Time.deltaTime);
			_driftingSideFactor = Mathf.MoveTowards(_driftingSideFactor, 1, driftingSideAcceleration * Time.deltaTime);
		}
		result *= _driftingTurnFactor;
		_steeringSideDrift *= _driftingSideFactor;

		//if (Mathf.Abs(_steeringSideDrift) >= 2.5f) {
		//	sideDriftParticles.Play();
		//	sideDriftEmissionModule.rateOverTime = sideDriftDefaultEmission;
		//} else {
		//	sideDriftParticles.Stop();
		//	sideDriftEmissionModule.rateOverTime = 0;
		//}

		return result;
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

			if (gamePadState.ThumbSticks.Left.X != 0 || gamePadState.ThumbSticks.Left.Y != 0 || gamePadState.Triggers.Right != 0 || gamePadState.Triggers.Left != 0) {
				break;
			}

			yield return null;
		}

		_struggleVelocity = Vector3.zero;
	}

	public Transform GetTransform ()
	{
		return transform;
	}
}