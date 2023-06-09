using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace KlopoffGames.Core.Windows
{
    public class WindowManager : IInitializable, System.IDisposable
    {
        private readonly BaseWindow.Factory _factory;
        private readonly List<BaseWindow> _stack;

        public WindowManager(
            BaseWindow.Factory factory
            )
        {
            _factory = factory;
            _stack = new List<BaseWindow>();
        }

        public void Initialize()
        {
            GrabWindowsFromScene();
        }

        public bool IsWindowActive<T>() where T : BaseWindow
        {
            if (_stack.Count == 0)
            {
                return false;
            }
            return _stack[^1].GetType() == typeof(T);
        }
        
        public T CreateWindow<T>(string prefix = "") where T : BaseWindow
        {
            var path = $"{typeof(T).Name}";
            if (!string.IsNullOrEmpty(prefix))
            {
                path = $"{prefix}/{path}";
            }
            var window = _factory.Create(path);
            OnWindowCreate(window);
            return (T)window;
        }

        public void Dispose()
        {
            while (_stack.Count > 0)
            {
                var window = _stack[^1];
                window.Hide();
            }
        }

        private async void OnWindowCreate(BaseWindow window)
        {
            if (_stack.Count > 0)
            {
                _stack[^1].OnInactive();
            }
            _stack.Add(window);
            
            await window.OnCreate();
            await window.OnActive();
        }

        public async void OnWindowHide(BaseWindow window)
        {
            _stack.Remove(window);

            await window.OnHide();
            
            if (!window.Destroyed)
            {
                Object.Destroy(window.gameObject);
            }
            
            if (_stack.Count > 0)
            {
                await _stack[^1].OnActive();
            }
        }

        private void GrabWindowsFromScene()
        {
            var sceneWindows = Object.FindObjectsOfType<BaseWindow>();
            foreach (var sceneWindow in sceneWindows)
            {
                OnWindowCreate(sceneWindow);
            }
        }
    }
}