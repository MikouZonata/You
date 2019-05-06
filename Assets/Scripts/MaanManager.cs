using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class MaanManager : MonoBehaviour
{
	Transform[] trackPieces;

	public GameObject kattoeprefab;
	public AudioClip[] kattoeClips;

	int activeAnimals = 20;

	public void Init (Transform[] trackPieces, params Maan[] maans)
	{
		this.trackPieces = trackPieces;

		Transform[] spawnPositions = Util.PickRandom(activeAnimals, false, trackPieces);
		for (int i = 0; i < activeAnimals; i++) {
			CreateKattoe(spawnPositions[i].position);
		}
	}

	void CreateKattoe (Vector3 position)
	{
		Kattoe newKattoe = Instantiate(kattoeprefab, position, Quaternion.identity).GetComponent<Kattoe>();
		newKattoe.Init(Util.PickRandom(kattoeClips));
	}
}