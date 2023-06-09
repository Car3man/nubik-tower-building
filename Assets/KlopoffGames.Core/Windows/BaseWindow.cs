using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace KlopoffGames.Core.Windows
{
    public class BaseWindow : MonoBehaviour
    {
        [Inject] protected WindowManager Windows;

        public bool Destroyed { get; private set; }

        private void OnDestroy()
        {
            OnDispose();
            Destroyed = true;
        }

        public void Hide()
        {
            Windows.OnWindowHide(this);
        }
        
        public virtual UniTask OnCreate()
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnActive()
        {
            return UniTask.CompletedTask;
        }

        public virtual void OnInactive()
        {
        }

        /**
         * Be careful while using this method
         * Because this method could be called while application quit/scene change
         * Handling exceptions is your problem
         */
        public virtual UniTask OnHide()
        {
            return UniTask.CompletedTask;
        }

        public virtual void OnDispose()
        {
            
        }

        public class Factory : PlaceholderFactory<string, BaseWindow>
        {
        }
    }
}