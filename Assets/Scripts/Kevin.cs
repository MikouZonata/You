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
	public Transform modelAnchor;
	float modelMaxForwardsAngle = 12, modelMaxSidewaysAngle = 9;
	Vector3 _velocity = Vector3.zero;
	float triggerValue;
	float throttleMaxForwardSpeed = 26, throttleMaxBackwardsSpeed = 11;
	float throttleAcceleration = 2, throttleDecceleration = .8f, throttleNaturalDecceleration = 18;
	float _speedPoint = 0, _throttleSpeed = 0;
	float throttleTrailMaxTime = .08f;

	float _fatigueFactor = 1;
	float fatigueFactorMin = .55f, fatigueTimeUntilMin = 40, fatigueTimeUntilRecharged = 5;
	float fatigueGrowRate, fatigueShrinkRate;
	float fatigueRechargePerPickup = 0.04f;

	float _pickupFatigueFactor = 1;
	float pickupFatigueTimeToDeplete = 30, pickupFatigueBoostPerPickup = 0.08f;

	float maxTurnRate = 178, minTurnRate = 76;
	float turnRateLossPerVelocity = 4.2f;
	float _steeringSideDrift = 0, sideDriftPerVelocity = .42f, sideDriftMaxVelocity = 22, sideDriftMinVelocity = 12;
	float driftingMaxSideFactor = 5.2f, _driftingSideFactor = 1, driftingMaxTurnFactor = 1.24f, driftingTimeToMax = .12f;
	float _driftingTurnFactor = 1;
	float driftingSideAcceleration, driftingTurnAcceleration;

	float _struggleTimer = 0, struggleTime = 10;
	float struggleMinTime = 2, struggleMaxTime = 5.0f;
	Vector3 _struggleVelocity = Vector3.zero;

	bool selectButtonReleased = false;

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

		struggleTime = Random.Range(struggleMinTime, struggleMaxTime);

		this.manager = manager;
		this.otherPlayer = otherPlayer;
	}

	private void Update ()
	{
		gamePadState = GamePad.GetState(playerIndex);

		CameraFoV();
		ShowLink();
		ModelRotation();

		if (gamePadState.ThumbSticks.Left.X == 0 || gamePadState.ThumbSticks.Left.Y == 0 || gamePadState.Triggers.Right == 0 || gamePadState.Triggers.Left == 0) {
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

		if (gamePadState.Buttons.Back == ButtonState.Pressed && selectButtonReleased) {
			manager.Reset();
			_velocity = Vector3.zero;
			selectButtonReleased = false;
		} else if (gamePadState.Buttons.Back == ButtonState.Released) {
			selectButtonReleased = true;
		}
	}

	void FixedUpdate ()
	{
		triggerValue = gamePadState.Triggers.Right;

		Throttle();
		Fatigue();

		_velocity.z = _throttleSpeed * _fatigueFactor;  //_boostSpeed;
		_velocity.x = _steeringSideDrift;
		_velocity += _struggleVelocity;

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

	void ModelRotation ()
	{
		float xRot = modelMaxForwardsAngle * gamePadState.Triggers.Right;
		float yRot = modelMaxSidewaysAngle * gamePadState.ThumbSticks.Left.X;
		modelAnchor.rotation = transform.rotation * Quaternion.Euler(xRot, yRot, 0);
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

	IEnumerator Struggle ()
	{
		float struggleTime = Random.Range(0.08f, 0.18f);
		Vector2 radial = Random.insideUnitCircle;
		Vector3 velocity = new Vector3(radial.x, 0, Mathf.Abs(radial.y)).normalized * Random.Range(3, 7);
		float rotation = Random.Range(-80, 80);

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