using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using DV.Logic.Job;
using HarmonyLib;
using UnityEngine;

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

	[HarmonyPatch(typeof(StationProceduralJobGenerator), "GenerateOutChainJob")]
	public class StationProceduralJobGenerator_GenerateOutChainJob_Patch
	{
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
		{
			if (!HaulWorld.settings.DisableShuntingUnloadJobs)
			{
				return instructions;
			}

			var instructionList = instructions.ToList();
			var targetLabel = generator.DefineLabel();
			var foundIndex = -1;

			//we need to jump to after the startingJobType switch
			//the first set of instructions after switch calls FinalizeSetupAndGenerateFirstJob
			//it's also the only appearance of that call in the function so that's the search target
			//the set has 3 instructions(ldloc.s for chainController, ldc.i4.0, callvirt)
			for (var i = 0; i < instructionList.Count; i++)
			{
				if (instructionList[i].opcode == OpCodes.Callvirt &&
				    instructionList[i].Calls(
					    typeof(JobChainController)
						    .GetMethod(nameof(JobChainController.FinalizeSetupAndGenerateFirstJob)))
				    )
				{
					Debug.Log("Found the FinalizeSetupAndGenerateFirstJob call! Adding label");
					//since we need to jump to the start of instruction set, to the ldloc.s opcode, subtract 2
					instructionList[i-2].labels.Add(targetLabel);
					foundIndex = i - 2;
					break;
				}
			}

			if (foundIndex < 0)
			{
				Debug.Log("Cannot find the jump target for GenerateOutChainJob patch! Skipping the patch");
				return instructionList;
			}

			var populateCallOffset = 30;
			Debug.Log("Start searching for the jump location");
			// looking for the spot to insert the jump instruction
			for (var i = populateCallOffset; i < instructionList.Count; i++)
			{
				if (instructionList[i].opcode == OpCodes.Callvirt
				    && instructionList[i].Calls(
					    AccessTools.Method(
						    typeof(JobChainController),
						    nameof(JobChainController.AddJobDefinitionToChain)))
				    && instructionList[i-populateCallOffset].opcode == OpCodes.Call
				    && instructionList[i-populateCallOffset].Calls(
					    AccessTools.Method(
						    typeof(StationProceduralJobGenerator),
						    "PopulateHaulJobDefinitionWithExistingCars")))
				{
					// Debug.Log("Opcode at -30: #{instructionList[i-30].opcode}");
					// Debug.Log("Opcode at -29: #{instructionList[i-29].opcode}");
					Debug.Log("Found the end of case! Patching");
					instructionList.Insert(i+1, new CodeInstruction(OpCodes.Br, targetLabel));
					break;
				}
			}
			return instructionList;
		}
	}
}
