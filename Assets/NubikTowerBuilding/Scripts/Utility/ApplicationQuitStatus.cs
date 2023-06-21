using UnityEngine;

namespace NubikTowerBuilding.Utility
{
    public static class ApplicationQuitStatus
    {
        public static bool Quiting { get; private set; }
        
        [RuntimeInitializeOnLoadMethod]
        private static void OnRuntimeMethodLoad()
        {
            Application.quitting += OnQuiting;
        }

        private static void OnQuiting()
        {
            Quiting = true;
        }
    }
}
