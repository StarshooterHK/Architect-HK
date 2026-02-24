using System;
using System.Collections.Generic;
using Architect.Behaviour.Fixers;
using Architect.Content;
using Architect.Content.Custom;
using Architect.Content.Preloads;
using Architect.Editor;
using Architect.Events;
using Architect.Events.Blocks.Operators;
using Architect.Multiplayer;
using Architect.Objects.Categories;
using Architect.Placements;
using Architect.Prefabs;
using Architect.Sharer;
using Architect.Storage;
using Architect.Utils;
using Modding;
using UnityEngine;

namespace Architect;

public class ArchitectPlugin : Mod
{
    internal static ArchitectPlugin ModInstance;
    internal static ArchitectManager Instance;

    public static readonly Sprite BlankSprite = ResourceUtils.LoadSpriteResource("blank", ppu:300);
    
    public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        ModInstance = this;
        
        var am = new GameObject("ArchitectManager");
        Instance = am.AddComponent<ArchitectManager>();
        UnityEngine.Object.DontDestroyOnLoad(am);

        SceneUtils.Init();
        PrefabManager.Init();
        
        HookUtils.Init();
        TitleUtils.Init();
        
        StorageManager.Init();
        Settings.Init(Config);
        
        MiscFixers.Init();
        EnemyFixers.Init();
        HazardFixers.Init();
        InteractableFixers.Init();
        
        Categories.Init();
        
        ActionManager.Init();
        CoopManager.Init();
        
        EditManager.Init();
        CursorManager.Init();
        EditorUI.Init();
        
        VanillaObjects.Init();
        SplineObjects.Init();
        UtilityObjects.Init();
        AbilityObjects.Init();
        MiscObjects.Init();
        
        RespawnMarkerManager.Init();
        
        PlacementManager.Init();
        
        BroadcasterHooks.Init();

        SharerManager.Init();
        
        PreloadManager.Init();

        StorageManager.MakeBackup();

        typeof(GameManager).Hook(nameof(GameManager.ResetSemiPersistentItems),
            (Action<GameManager> orig, GameManager self) =>
            {
                BoolVarBlock.SemiVars.Clear();
                NumVarBlock.SemiVars.Clear();
                StringVarBlock.SemiVars.Clear();
                orig(self);
            });
    }

    public class ArchitectManager : MonoBehaviour
    {
        private void Update()
        {
            if (HeroController.instance)
            {
                EditManager.Update();
                HazardFixers.UpdateLanterns();
            }

            CursorManager.Update();
            SharerManager.Update();
            AbilityObjects.Update();
        }
    }
    
    public static readonly LoggerHandler Logger = new();

    public class LoggerHandler
    {
        public void LogInfo(string message)
        {
            ModInstance.Log(message);
        }
        
        public void LogError(object message)
        {
            ModInstance.LogError(message);
        }
    }

    public ArchitectData SaveData { get; set; }
}
