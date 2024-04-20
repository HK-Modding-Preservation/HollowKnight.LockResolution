using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Modding;

namespace LockResolution
{
    public class LockResolution : Mod, IGlobalSettings<CustomGlobalSaveData>, IMenuMod
    {
        public static LockResolution Instance;
        public static CustomGlobalSaveData globalSaveData = new CustomGlobalSaveData();

        public bool ToggleButtonInsideMenu => true;
        private Dictionary<string, (int width, int height)> resolutions = new Dictionary<string, (int width, int height)>();
        public override string GetVersion()
        {
            var version = GetType().Assembly.GetName().Version.ToString();
#if DEBUG
            version += "-debug";
#endif
            return version;
        }
        public override int LoadPriority() => 5;
        public override void Initialize()
        {
            Instance = this;
            var ress = UnityEngine.Screen.resolutions;
            foreach (var res in ress)
            {
                var key = $"{res.width}x{res.height}";
                if (!resolutions.ContainsKey(key))
                    resolutions.Add(key,(res.width, res.height));
            }
            ModHooks.HeroUpdateHook += ModHooks_HeroUpdateHook;
            //On.HutongGames.PlayMaker.Actions.ScreenSetResolution.OnEnter += ScreenSetResolution_OnEnter;
            //On.HutongGames.PlayMaker.Actions.ScreenSetResolution.Reset += ScreenSetResolution_Reset;
            //On.GameManager.Update
            //MenuChanger.MenuChangerMod.

            VSyncChanger.Initialize();
        }
        private void ModHooks_HeroUpdateHook()
        {
            if (!globalSaveData.locked) return;
            if (UnityEngine.Screen.width != globalSaveData.width)
            {
                UnityEngine.Screen.SetResolution(globalSaveData.width, globalSaveData.height, UnityEngine.Screen.fullScreen);
            }
        }
        void IGlobalSettings<CustomGlobalSaveData>.OnLoadGlobal(CustomGlobalSaveData data)
        {
            globalSaveData = data;
        }

        CustomGlobalSaveData IGlobalSettings<CustomGlobalSaveData>.OnSaveGlobal()
        {
            return globalSaveData;
        }
        
        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            var resstrs = resolutions.Keys.ToArray();
            return new() {
            new IMenuMod.MenuEntry
            {
                Name = "Lock",
                Description = "Lock Resolution On GamePlay",
                Values = new string[] { "True", "False" },
                Saver = locked =>
                {
                    globalSaveData.locked = locked == 0;
                    /*
                    if (globalSaveData.locked)
                    {
                        globalSaveData.width = UnityEngine.Screen.width;
                        globalSaveData.height = UnityEngine.Screen.height;
                        globalSaveData.fullScreen = UnityEngine.Screen.fullScreen;
                    }*/
                    Log($"Lock Saver {globalSaveData.locked}");
                },
                Loader = () =>
                {
                    Log($"Lock Loader {globalSaveData.locked}");
                    return globalSaveData.locked == true ? 0 : 1;
                }
            },
            new IMenuMod.MenuEntry
            {
                Name = "Resolution",
                Values = resstrs,
                Saver = ressindex=>
                {
                    var res=resolutions[resstrs[ressindex]];
                    globalSaveData.width = res.width; 
                    globalSaveData.height = res.height;
                    Log($"Resolution Saver Width{globalSaveData.width} Height{globalSaveData.height}");
                },
                Loader = () =>
                {
                    Log($"Resolution Loader Width{globalSaveData.width} Height{globalSaveData.height}");
                    int i;
                    for (i = 0; i < resstrs.Length; i++)
                    {
                        var res = resolutions[resstrs[i]];
                        if (res.width == globalSaveData.width && res.height == globalSaveData.height)
                            return i;
                    };
                    var key = $"{globalSaveData.width}x{globalSaveData.height}";
                    resolutions.Add(key,(globalSaveData.width, globalSaveData.height));
                    resstrs = resolutions.Keys.ToArray();
                    return i;
                }
            },
            new IMenuMod.MenuEntry
            {
                Name="VSync LostFocus",
                Description = "VSync the game once focus is lost",
                Values = new string[] { "Disabled", "VSync", "VSync 1/2", "VSync 1/3", "VSync 1/4" },
                Saver = vSyncEnable =>
                {
                    globalSaveData.VSyncCount = vSyncEnable;
                },
                Loader = () =>
                {
                    return globalSaveData.VSyncCount;
                }
            }
            }; 
        }
    }
}