﻿using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Verse;
using Object = UnityEngine.Object;
using RimWorld;

namespace AlienRace
{
    public class ModInitializer : ITab
    {
        protected GameObject modInitializerControllerObject;

        public ModInitializer()
        {
            LongEventHandler.QueueLongEvent(delegate
            {
                this.modInitializerControllerObject = new GameObject("AlienRacer");
                this.modInitializerControllerObject.AddComponent<ModInitializerBehaviour>();
                this.modInitializerControllerObject.AddComponent<DoOnMainThread>();
                Object.DontDestroyOnLoad(this.modInitializerControllerObject);
            }, "queueInject", false, null);
        }

        protected override void FillTab()
        { }
    }

    public class DoOnMainThread : MonoBehaviour
    {

        public static readonly Queue<Action> ExecuteOnMainThread = new Queue<Action>();

        public void Update()
        {
            while (ExecuteOnMainThread.Count > 0)
            {
                ExecuteOnMainThread.Dequeue().Invoke();
            }
        }
    }

    class ModInitializerBehaviour : MonoBehaviour
    {
        public void FixedUpdate()
        {
        }

        public void OnLevelLoaded()
        {

        }

        public void Start()
        {
            Log.Message("Initiated Alien Pawn Detours.");
            MethodInfo method1a = typeof(Verse.GenSpawn).GetMethod("Spawn", new Type[] { typeof(Thing), typeof(IntVec3), typeof(Map), typeof(Rot4) });
            MethodInfo method1b = typeof(GenSpawnAlien).GetMethod("SpawnModded", new Type[] { typeof(Thing), typeof(IntVec3), typeof(Map), typeof(Rot4) });

            MethodInfo method2a = typeof(RimWorld.InteractionWorker_RecruitAttempt).GetMethod("DoRecruit", new Type[] { typeof(Pawn), typeof(Pawn), typeof(float), typeof(bool) });
            MethodInfo method2b = typeof(AlienRace.AlienRaceUtilities).GetMethod("DoRecruitAlien", new Type[] { typeof(Pawn), typeof(Pawn), typeof(float), typeof(bool) });

            MethodInfo method3a = typeof(RimWorld.FloatMenuMakerMap).GetMethod("AddHumanlikeOrders", BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo method3b = typeof(AlienRace.MenuMakerMapRestricted).GetMethod("AddHumanlikeOrders", BindingFlags.Static | BindingFlags.NonPublic);

            MethodInfo method4a = typeof(Verse.StartingPawnUtility).GetMethod("NewGeneratedStartingPawn", BindingFlags.Static | BindingFlags.Public);
            MethodInfo method4b = typeof(AlienRace.AlienRaceUtilities).GetMethod("NewGeneratedStartingPawnModded", BindingFlags.Static | BindingFlags.Public);

            MethodInfo method5a = typeof(Verse.PawnGenerator).GetMethod("GeneratePawn", new Type[] { typeof(PawnKindDef), typeof(Faction) });
            MethodInfo method5b = typeof(AlienRace.AlienPawnGenerator).GetMethod("GeneratePawn", new Type[] { typeof(PawnKindDef), typeof(Faction) });

            MethodInfo method6a = typeof(Verse.PawnGenerator).GetMethod("GeneratePawn", new Type[] { typeof(PawnGenerationRequest)});
            MethodInfo method6b = typeof(AlienRace.AlienPawnGenerator).GetMethod("GeneratePawn", new Type[] { typeof(PawnGenerationRequest) });

            try
            {
                Detours.TryDetourFromTo(method1a, method1b);
                Detours.TryDetourFromTo(method2a, method2b);
                Detours.TryDetourFromTo(method3a, method3b);
                Detours.TryDetourFromTo(method4a, method4b);
                Detours.TryDetourFromTo(method5a, method5b);
                Detours.TryDetourFromTo(method6a, method6b);
                Log.Message("Spawn method detoured!");
            }
            catch (Exception)
            {
                Log.Error("Could not detour Aliens");
                throw;
            }
        }
    }
}