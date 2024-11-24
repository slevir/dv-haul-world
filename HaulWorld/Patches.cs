using System.Collections.Generic;
using System.Linq;
using DV.Logic.Job;
using HarmonyLib;
using DV.ThingTypes;

namespace HaulWorld
{
	[HarmonyPatch(typeof(StationProceduralJobsRuleset), "Awake")]
	public class StationProceduarlJobsRuleset_Awake_Patch
	{
		private static void Postfix(StationProceduralJobsRuleset __instance)
		{
			__instance.unloadStartingJobSupported = false;
			__instance.loadStartingJobSupported = false;
			__instance.minCarsPerJob = HaulWorld.settings.MinimumJobTrainLength;
		}
	}

	[HarmonyPatch(typeof(Yard), MethodType.Constructor,
		typeof(List<Track>), typeof(List<Track>), typeof(List<Track>), typeof(List<WarehouseMachine>), typeof(string))]
	public class Yard_Yard_Patch
	{
		private static void Prefix(
			ref List<Track> storageTracks,
			ref List<Track> transferInTracks,
			ref List<Track> transferOutTracks)
		{
			if (HaulWorld.settings.GenerateOnInputTracksToo)
			{
				var allTracks = storageTracks.Concat(transferInTracks).Concat(transferOutTracks).ToList();
				transferInTracks = storageTracks.Concat(transferInTracks).ToList();
				storageTracks = allTracks;
				transferOutTracks = allTracks;

			}
			else
			{
				transferOutTracks.AddRange(storageTracks);
				if (HaulWorld.settings.AllowMultipleJobsPerTrack)
				{
					transferInTracks.AddRange(storageTracks);
				}
			}
		}
	}

	[HarmonyPatch(typeof(YardTracksOrganizer), nameof(YardTracksOrganizer.FilterOutOccupiedTracks))]
	public class YardTracksOrganiser_FilterOutOccupiedTracks_Patch
	{
		static bool Prefix(List<Track> tracks, ref List<Track> __result)
		{
			if (HaulWorld.settings.AllowMultipleJobsPerTrack)
			{
				__result = new List<Track>(tracks);
				return false;
			}
			return true;
		}
	}
}
