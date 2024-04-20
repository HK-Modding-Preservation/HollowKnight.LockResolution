using UnityEngine;

namespace LockResolution
{
    public class CustomGlobalSaveData
    {
        public bool locked = true;
        public int width = UnityEngine.Screen.width;
        public int height = UnityEngine.Screen.height;
        //public bool fullScreen = UnityEngine.Screen.fullScreen;
        public int VSyncCount = 0;
    }
}