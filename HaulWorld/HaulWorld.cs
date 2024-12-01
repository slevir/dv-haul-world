using System;
using System.Linq;
using System.Reflection;
using CommandTerminal;
using DV.Logic.Job;
using DV.Utils;
using HarmonyLib;
using UnityModManagerNet;

namespace HaulWorld
{
	public static class HaulWorld
	{
		public static Settings settings = new();

		// Unity Mod Manage Wiki: https://wiki.nexusmods.com/index.php/Category:Unity_Mod_Manager
		private static bool Load(UnityModManager.ModEntry modEntry)
		{
			Harmony? harmony = null;

			try
			{
				settings = Settings.Load<Settings>(modEntry);
				harmony = new Harmony(modEntry.Info.Id);
				harmony.PatchAll(Assembly.GetExecutingAssembly());

				modEntry.OnGUI = OnGui;
				modEntry.OnSaveGUI = OnSaveGUI;
			}
			catch (Exception ex)
			{
				modEntry.Logger.LogException($"Failed to load {modEntry.Info.DisplayName}:", ex);
				harmony?.UnpatchAll(modEntry.Info.Id);
				return false;
			}

			return true;
		}

		static void OnGui(UnityModManager.ModEntry modEntry)
		{
			settings.Draw(modEntry);
		}

		static void OnSaveGUI(UnityModManager.ModEntry modEntry)
		{
			settings.Save(modEntry);
		}

		#if DEBUG
		[RegisterCommand("FinishJobs", Help = "Finishes all active jobs", MaxArgCount = 0, MinArgCount = 0)]
		public static void FinishJobs(CommandArg[] args)
		{
			var jobs = SingletonBehaviour<JobsManager>.Instance.currentJobs.ToList();
			foreach(var job in jobs)
				SingletonBehaviour<JobsManager>.Instance.CompleteTheJob(job);
		}
		#endif
	}
}
