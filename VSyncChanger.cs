using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LockResolution
{
    internal static class VSyncChanger
    {
        static GameObject go;
        public static void Initialize()
        {
            if (go == null)
            {
                go = new("VSyncChangerLostFocusModObject", typeof(Behaviour));
                GameObject.DontDestroyOnLoad(go);
            }
        }
        public static void Unload()
        {
            if (go != null)
            {
                GameObject.Destroy(go);
                go = null;
            }
        }
        private sealed class Behaviour : MonoBehaviour
        {
            private int lastVSync;
            public void OnApplicationFocus(bool hasFocus)
            {
                if (LockResolution.globalSaveData.VSyncCount is 0) return;
                if (hasFocus)
                {
                    UnityEngine.QualitySettings.vSyncCount = lastVSync;
                }
                else
                {
                    lastVSync = UnityEngine.QualitySettings.vSyncCount;
                    UnityEngine.QualitySettings.vSyncCount = LockResolution.globalSaveData.VSyncCount;
                }
            }
        }
    }
}
