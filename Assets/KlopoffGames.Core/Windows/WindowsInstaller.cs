using UnityEngine;
using Zenject;

namespace KlopoffGames.Core.Windows
{
    public class WindowsInstaller : MonoInstaller
    {
        [SerializeField] private RectTransform parent;

        public override void InstallBindings()
        {
            Container.BindFactory<string, BaseWindow, BaseWindow.Factory>()
                .FromMethod(CreateWindow);

            Container.BindInterfacesAndSelfTo<WindowManager>()
                .AsSingle();
        }

        private BaseWindow CreateWindow(DiContainer diContainer, string resourcePath)
        {
            return diContainer.InstantiatePrefabResourceForComponent<BaseWindow>(resourcePath, parent);
        }
    }
}