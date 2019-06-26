using FMODUnity;

public static class FMODCollabPatch
{
	public static bool fmodAvailable = true;

	static FMODCollabPatch ()
	{
		try {
			FMOD.Studio.EventInstance temp = RuntimeManager.CreateInstance("event:/Linked_Up");
			temp.start();
			temp.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		} catch {
			fmodAvailable = false;
		}
	}
}