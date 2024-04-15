using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modding;

namespace LockResolution
{
    public class LockResolution:Mod
    {

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
            ModHooks.HeroUpdateHook += ModHooks_HeroUpdateHook;
        }
        private void ModHooks_HeroUpdateHook()
        {
            if (UnityEngine.Screen.width != 1920)
            {
                UnityEngine.Screen.SetResolution(1920, 1080, fullscreen: true);
            }
        }
    }
}
