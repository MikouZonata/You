using UnityEngine;

public class LookAtKevin : MonoBehaviour
{
    void Update()
    {
		transform.LookAt(StaticData.playerTransforms[1]);
    }
}
