using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class Love : MonoBehaviour
{
	public GameObject feedbackPrefab;
	Transform targetTrans;
	
	const float fadeInTime = .2f;
	const float acceleration = 5;
	float _speed = 5;

	public void Init (Transform targetTrans)
	{
		this.targetTrans = targetTrans;
		StartCoroutine(LoveRoutine());
	}

	private void Update ()
	{
		_speed += acceleration * Time.deltaTime;
	}

	IEnumerator LoveRoutine ()
	{
		StartCoroutine(LoveFadeIn(GetComponent<SpriteRenderer>()));

		while (true) {
			transform.position = Vector3.MoveTowards(transform.position, targetTrans.position + Vector3.up, _speed * Time.deltaTime);
			transform.rotation = Quaternion.Lerp(transform.rotation, targetTrans.rotation, Time.deltaTime);

			if (Util.SqrDistanceTo(transform.position, targetTrans.position + Vector3.up) < .5f) {
				break;
			}

			yield return null;
		}

		Transform feedbackTrans = Instantiate(feedbackPrefab, targetTrans.position + Vector3.up, targetTrans.rotation).transform;
		StartCoroutine(FeedbackFollowPlayer(feedbackTrans));
		GetComponent<SpriteRenderer>().enabled = false;
	}

	IEnumerator LoveFadeIn (SpriteRenderer renderer)
	{
		float fadeTimeInverse = 1 / fadeInTime;
		renderer.color = new Color(1, 1, 1, 0);

		for (float a = 0; a < 1; a += Time.deltaTime * fadeTimeInverse) {
			renderer.color = new Color(1, 1, 1, a);
			yield return null;
		}

		renderer.color = new Color(1, 1, 1, 1);
	}

	IEnumerator FeedbackFollowPlayer (Transform feedbackTrans)
	{
		for (float t = 0; t < 2.5f; t += Time.deltaTime) {
			feedbackTrans.position = targetTrans.position + Vector3.up;
			yield return null;
		}

		yield return null;
		Destroy(feedbackTrans.gameObject);
		Destroy(gameObject);
	}
}