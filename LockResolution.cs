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
        public LockResolution Instance;
        public static CustomGlobalSaveData globalSaveData = new CustomGlobalSaveData();

        public bool ToggleButtonInsideMenu => true;

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
            ModHooks.HeroUpdateHook += ModHooks_HeroUpdateHook;
            //On.HutongGames.PlayMaker.Actions.ScreenSetResolution.OnEnter += ScreenSetResolution_OnEnter;
            //On.HutongGames.PlayMaker.Actions.ScreenSetResolution.Reset += ScreenSetResolution_Reset;
            //On.GameManager.Update
            //MenuChanger.MenuChangerMod.
        }
        private void ModHooks_HeroUpdateHook()
        {
            if (!globalSaveData.locked) return;
            if (UnityEngine.Screen.width != globalSaveData.width)
            {
                UnityEngine.Screen.SetResolution(globalSaveData.width, globalSaveData.height, globalSaveData.fullScreen);
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
        IMenuMod.MenuEntry menuEntry;
        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            menuEntry = new IMenuMod.MenuEntry
            {
                Name = "Lock",
                Description = "Lock Resolution On GamePlay",
                Values = new string[] { "True", "False" },
                Saver = locked =>
                {
                    globalSaveData.locked = locked == 0;
                    if (globalSaveData.locked)
                    {
                        globalSaveData.width = UnityEngine.Screen.width;
                        globalSaveData.height = UnityEngine.Screen.height;
                        globalSaveData.fullScreen = UnityEngine.Screen.fullScreen;
                    }
                    Log($"Saver {globalSaveData.locked} Width{globalSaveData.width} Height{globalSaveData.height}");
                },
                Loader = () =>
                {
                    Log($"Loader {globalSaveData.locked} Width{globalSaveData.width} Height{globalSaveData.height}");
                    return globalSaveData.locked == true ? 0 : 1;
                }
            };
            return new() { menuEntry };
        }
        /*
        private void updateMenuName()
        {
            menuEntry.Name = globalSaveData.locked
                ? $"Locked Width{globalSaveData.width} Height{globalSaveData.height}"
                : $"Lock Current Width{UnityEngine.Screen.width} Height{UnityEngine.Screen.height}";

            Log($"updateMenuName {globalSaveData.locked}");
            Log($"updateMenuName {menuEntry.Name}");
        }*/
    }
}