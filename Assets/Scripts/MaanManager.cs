using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class MaanManager : MonoBehaviour
{
	public GameObject kattoeprefab;
	public AudioClip[] kattoeClips;

	public void Init (params Maan[] maans)
	{

	}

	void CreateKattoe ()
	{
		Kattoe newKattoe = Instantiate(kattoeprefab).GetComponent<Kattoe>();
		newKattoe.Init(Util.PickRandom(kattoeClips));
	}
}