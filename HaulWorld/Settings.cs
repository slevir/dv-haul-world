using UnityModManagerNet;

namespace HaulWorld;

public class Settings : UnityModManager.ModSettings, IDrawable
{
	[Draw("Minimum job train length (needs reload)", Min = 1, Max = 10)] public int MinimumJobTrainLength = 3;
	[Draw("Allow multiple freight hauls per track")] public bool AllowMultipleJobsPerTrack = true;
	[Draw("Generate jobs on input tracks too! (needs reload)")] public bool GenerateOnInputTracksToo = false;

	public override void Save(UnityModManager.ModEntry modEntry)
	{
		Save(this, modEntry);
	}

	public void OnChange()
	{

	}
}
