using UnityEngine;

namespace KlopoffGames.WebPlatforms.LocalWebGL
{
    public class UnityLocalWebGLBridge : MonoBehaviour
    {
        public delegate void ViewHideDelegate();
        public event ViewHideDelegate OnViewHide;
        
        public delegate void ViewRestoreDelegate();
        public event ViewRestoreDelegate OnViewRestore;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void LocalWebGL2Unity_OnViewHide()
        {
            OnViewHide?.Invoke();
        }
        
        private void LocalWebGL2Unity_OnViewRestore()
        {
            OnViewRestore?.Invoke();
        }
    }
}
