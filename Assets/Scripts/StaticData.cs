using UnityEngine;

public static class StaticData
{
	public enum PlayerOptions { Kevin, Maan };
	public static PlayerOptions[] playerOptions = new PlayerOptions[] { PlayerOptions.Maan, PlayerOptions.Maan };
	public static int levelNumber = 1;

	public static Transform[] playerTransforms = new Transform[2];
}