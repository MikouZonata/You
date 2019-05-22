using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using XInputDotNetPure;
using Utility;

public class Kevin : MonoBehaviour
{
	public PlayerIndex playerIndex = PlayerIndex.Two;
	GamePadState gamePadState;
	KevinManager manager;
	Transform otherPlayer;
	Rigidbody rig;

	Camera mainCamera;
	float FoVBaseDegrees = 60, FoVDegreesPerVelocity = .3f;

	ParticleSystem sideDriftParticles;
	ParticleSystem.EmissionModule sideDriftEmissionModule;
	float sideDriftDefaultEmission;

	public LineRenderer linkRenderer;

	public TrailRenderer throttleTrail;
	Vector3 _velocity = Vector3.zero;
	float triggerValue;
	float throttleMaxForwardSpeed = 26, throttleMaxBackwardsSpeed = 10, _throttleSpeed = 0;
	float throttleAcceleration = 2, throttleDecceleration = .8f, throttleNaturalDecceleration = 18;
	float _speedPoint = 0;
	float throttleTrailMaxTime = .08f;

	float _fatigueFactor = 0;
	float fatigueFactorMin = .55f, fatigueTimeUntilMin = 40, fatigueTimeUntilRecharged = 5;
	float fatigueGrowRate, fatigueShrinkRate;
	float fatigueRechargePerPickup = 0.04f;

	float maxTurnRate = 170, minTurnRate = 72;
	float turnRateLossPerVelocity = 4.2f;
	float _steeringSideDrift = 0, sideDriftPerVelocity = .42f, sideDriftMaxVelocity = 22, sideDriftMinVelocity = 12;
	float driftingMaxSideFactor = 5.8f, _driftingSideFactor = 1, driftingMaxTurnFactor = 1.24f, _driftingTurnFactor = 1, driftingTimeToMax = .12f;
	float driftingSideAcceleration, driftingTurnAcceleration;

	//public TrailRenderer boostTrail;
	//float _boostSpeed = 0, boostMaxSpeed = 12, boostTimeTillMax = .7f;
	//float boostTrailMaxTime = 0.12f;

	public Transform leaderboard;

	public void Init (KevinManager manager, Transform otherPlayer)
	{
		gamePadState = GamePad.GetState(playerIndex);

		rig = GetComponent<Rigidbody>();
		mainCamera = transform.GetChild(0).GetComponent<Camera>();

		fatigueShrinkRate = (1 - fatigueFactorMin) / fatigueTimeUntilMin;
		fatigueGrowRate = (1 - fatigueFactorMin) / fatigueTimeUntilRecharged;

		sideDriftParticles = GetComponentInChildren<ParticleSystem>();
		sideDriftEmissionModule = sideDriftParticles.emission;
		sideDriftDefaultEmission = sideDriftEmissionModule.rateOverTime.constant;
		sideDriftEmissionModule.rateOverTime = 0;
		var emission = sideDriftParticles.emission;
		emission.rateOverTime = 0;

		driftingSideAcceleration = (driftingMaxSideFactor - 1) / driftingTimeToMax;
		driftingTurnAcceleration = (driftingMaxTurnFactor - 1) / driftingTimeToMax;

		this.manager = manager;
		this.otherPlayer = otherPlayer;
	}

	private void Update ()
	{
		gamePadState = GamePad.GetState(playerIndex);

		CameraFoV();
		ShowLink();

		if (gamePadState.Buttons.Back == ButtonState.Pressed || gamePadState.Buttons.Start == ButtonState.Pressed) {
			SceneManager.LoadScene(0);
		}

		//if (gamePadState.Buttons.Back == ButtonState.Pressed) {
		//	StaticData.playerOptions[(int) playerIndex] = StaticData.PlayerOptions.Maan;
		//	SceneManager.LoadScene(1);
		//}
	}

	void FixedUpdate ()
	{
		triggerValue = gamePadState.Triggers.Right;

		Throttle();
		Fatigue();
		//LinkBoost();

		_velocity.z = _throttleSpeed * _fatigueFactor;  //_boostSpeed;
		_velocity.x = _steeringSideDrift;

		transform.Rotate(new Vector3(0, Steering(), 0));

		rig.velocity = transform.rotation * _velocity;
	}

	private void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Pickup") {
			manager.PickUpPickup(Util.ToInt(transform.name), Util.ToInt(other.name));
			_fatigueFactor = Mathf.Clamp(_fatigueFactor + fatigueRechargePerPickup, 0, 1);
		}
	}

	void CameraFoV ()
	{
		mainCamera.fieldOfView = FoVBaseDegrees + _velocity.z * FoVDegreesPerVelocity;
	}

	void ShowLink ()
	{
		Vector3[] positions;
		if (StaticData.playersAreLinked) {
			positions = new Vector3[] { transform.position, otherPlayer.position };
		} else {
			positions = new Vector3[] { transform.position, transform.position };
		}
		linkRenderer.SetPositions(positions);
	}

	void Throttle ()
	{
		if (triggerValue > 0) {
			if (_speedPoint >= 0) {
				_speedPoint = Mathf.MoveTowards(_speedPoint, triggerValue, throttleAcceleration * Time.deltaTime);
				_throttleSpeed = Mathf.Pow(_speedPoint, .5f) * throttleMaxForwardSpeed;
			} else {
				_speedPoint = Mathf.MoveTowards(_speedPoint, triggerValue, (throttleAcceleration + throttleDecceleration) * Time.deltaTime);
				if (_speedPoint == 0)
					_speedPoint += 0.01f;
				_throttleSpeed = Mathf.Pow(Mathf.Abs(_speedPoint), .5f) * -throttleMaxBackwardsSpeed;
			}
		} else if (triggerValue < 0) {
			if (_speedPoint >= 0) {
				_speedPoint = Mathf.MoveTowards(_speedPoint, triggerValue, (throttleAcceleration + throttleDecceleration) * Time.deltaTime);
				if (_speedPoint == 0)
					_speedPoint -= 0.01f;
				_throttleSpeed = Mathf.Pow(Mathf.Abs(_speedPoint), .5f) * throttleMaxForwardSpeed;
			} else {
				_speedPoint = Mathf.MoveTowards(_speedPoint, triggerValue, throttleAcceleration * Time.deltaTime);
				_throttleSpeed = Mathf.Pow(Mathf.Abs(_speedPoint), .5f) * -throttleMaxBackwardsSpeed;
			}
		} else {
			_throttleSpeed = Mathf.MoveTowards(_throttleSpeed, 0, throttleNaturalDecceleration * Time.deltaTime);
			if (_throttleSpeed >= 0) {
				_speedPoint = Mathf.Pow(_throttleSpeed / throttleMaxForwardSpeed, 2);
			} else {
				_speedPoint = Mathf.Pow(_throttleSpeed / throttleMaxBackwardsSpeed, 2) * -1;
			}
		}

		ThrottleTrail();
	}

	void ThrottleTrail ()
	{
		float actualTriggerValue = triggerValue;
		if (triggerValue < .1f)
			actualTriggerValue = 0;
		throttleTrail.time = Mathf.Lerp(0, throttleTrailMaxTime, actualTriggerValue);
	}

	void Fatigue ()
	{
		if (StaticData.playersAreLinked) {
			_fatigueFactor = Mathf.MoveTowards(_fatigueFactor, 1, fatigueGrowRate * Time.deltaTime);
		} else {
			_fatigueFactor = Mathf.MoveTowards(_fatigueFactor, fatigueFactorMin, fatigueShrinkRate * Time.deltaTime);
		}
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



		if (Mathf.Abs(_steeringSideDrift) >= 2.5f) {
			sideDriftParticles.Play();
			sideDriftEmissionModule.rateOverTime = sideDriftDefaultEmission;
		} else {
			sideDriftParticles.Stop();
			sideDriftEmissionModule.rateOverTime = 0;
		}

		return result;
	}

	//void LinkBoost ()
	//{
	//	if (StaticData.playersAreLinked) {
	//		_boostSpeed = Mathf.MoveTowards(_boostSpeed, boostMaxSpeed * gamePadState.Triggers.Right, boostMaxSpeed / boostTimeTillMax * Time.deltaTime);
	//	} else {
	//		_boostSpeed = Mathf.MoveTowards(_boostSpeed, 0, boostMaxSpeed / boostTimeTillMax * Time.deltaTime);
	//	}

	//	BoostTrail();
	//}

	//void BoostTrail ()
	//{
	//	boostTrail.time = Mathf.Lerp(0, boostTrailMaxTime, _boostSpeed / boostMaxSpeed);
	//}

	public Transform GetTransform ()
	{
		return transform;
	}
}