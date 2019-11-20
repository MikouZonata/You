using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
	public Transform badCloudTrans;
	float badCloudDefaultSize = 1, badCloudSmallSize = .2f;
	float scaleSpeed = .9f;

	void Update ()
	{
		float _size = badCloudTrans.localScale.x;
		if (!StaticData.playersAreLinked) {
			_size = Mathf.MoveTowards(_size, badCloudDefaultSize, scaleSpeed * Time.deltaTime);
		} else {
			_size = Mathf.MoveTowards(_size, badCloudSmallSize, scaleSpeed * Time.deltaTime);
		}
		badCloudTrans.localScale = new Vector3(_size, _size, _size);
	}
}