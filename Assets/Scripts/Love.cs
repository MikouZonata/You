using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class Love : MonoBehaviour
{
	public GameObject feedbackPrefab;
	Transform targetTrans;
	
	const float loveFadeInTime = .2f;
	const float loveMovementSpeed = 24;

	public void Init (Transform targetTrans)
	{
		this.targetTrans = targetTrans;
		StartCoroutine(LoveRoutine());
	}

	IEnumerator LoveRoutine ()
	{
		StartCoroutine(LoveFadeIn(GetComponent<SpriteRenderer>()));

		while (true) {
			transform.position = Vector3.MoveTowards(transform.position, targetTrans.position + Vector3.up, loveMovementSpeed * Time.deltaTime);
			transform.rotation = Quaternion.Lerp(transform.rotation, targetTrans.rotation, 3 * Time.deltaTime);

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
		float fadeTimeInverse = 1 / loveFadeInTime;
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

		Destroy(feedbackTrans.gameObject);
		Destroy(gameObject);
	}
}