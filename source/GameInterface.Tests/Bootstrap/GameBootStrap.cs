﻿using HarmonyLib;
using SandBox;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;
using Xunit;

namespace GameInterface.Tests.Bootstrap
{
    public class GameBootStrap
    {
        private static object @lock = new object();

        private const string HarmonyID = "Coop.Testing";
        private static Harmony harmony;

        private static Campaign current;

        /// <summary>
        /// Initializes any game functionality used for testing
        /// Currently initializes the following:
        /// - MBObjectManager
        /// </summary>
        public static void Initialize()
        {
            lock (@lock)
            {
                if (Harmony.HasAnyPatches(HarmonyID) == false)
                {
                    harmony = new Harmony(HarmonyID);
                    harmony.PatchAll();

                    // MBObjectManager is the default object managment system used by Bannerlord
                    InitializeMBObjectManager();
                    // Game provides saving and loading functionality, as well as some default values used in the game
                    InitializeGame();
                    // Campaign stores most of the data for the campaign map
                    //InitializeCampaign();

                    Assert.NotNull(Campaign.Current);
                }
            }
        }

        private static void InitializeMBObjectManager()
        {
            if (MBObjectManager.Instance != null) return;

            MBObjectManager.Init();
            RegisterType<ItemObject>();
            RegisterType<Settlement>();
            RegisterType<Hero>();
            RegisterType<MobileParty>();
            RegisterType<ItemObject>();
            RegisterType<TraitObject>();
            RegisterType<SkillObject>();
            RegisterType<PerkObject>();
            RegisterType<BannerEffect>();
            RegisterType<CharacterAttribute>();
        }

        private static uint itemCounter = 0;
        private static void RegisterType<T>() where T : MBObjectBase
        {
            MBObjectManager.Instance.RegisterType<T>($"{typeof(T).Name}", $"{typeof(T).Name}s", itemCounter++, true, false);
        }

        private static readonly PropertyInfo Campaign_Current = typeof(Campaign).GetProperty(nameof(Campaign.Current))!;

        private static void InitializeGame()
        {
            var ctor_Module = AccessTools.Constructor(typeof(TaleWorlds.MountAndBlade.Module));
            var currentModule = AccessTools.Property(typeof(TaleWorlds.MountAndBlade.Module), "CurrentModule");

            currentModule.SetValue(null, ctor_Module.Invoke(null));

            SandBoxGameManager gameManager = new SandBoxGameManager();
            
            Campaign campaign = new Campaign(CampaignGameMode.Campaign);
            Campaign_Current.SetValue(null, campaign);
            Game game = Game.CreateGame(campaign, gameManager);

            game.Initialize();

            AccessTools.Field(typeof(Campaign), "_mapSceneWrapper").SetValue(campaign, new MapScene());
        }
    }
}
