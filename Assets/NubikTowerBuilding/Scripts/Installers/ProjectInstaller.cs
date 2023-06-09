using KlopoffGames.Core;
using KlopoffGames.Core.Ads;
using KlopoffGames.Core.Analytics;
using KlopoffGames.Core.Audio;
using KlopoffGames.Core.Interfaces;
using KlopoffGames.Core.Localization;
using KlopoffGames.Core.Saving;
using NubikTowerBuilding.Adapters;
using NubikTowerBuilding.Scenes;
using Zenject;

namespace NubikTowerBuilding.Infrastructure
{
    public class ProjectInstaller : MonoInstaller<ProjectInstaller>
    {
        public override void InstallBindings()
        {
            InstallProjectEnvironment();
            InstallAdvertisement();
            InstallAnalytics();
            InstallSaves();
            InstallLocalization();
            InstallAudio();
            InstallSceneReferences();
        }

        private void InstallProjectEnvironment()
        {
#if PRODUCTION
            Container.Bind<ProjectEnvironment>().FromInstance(new ProjectEnvironment
            {
                Name = "production"
            });
#else 
            Container.Bind<ProjectEnvironment>().FromInstance(new ProjectEnvironment
            {
                Name = "development"
            });
#endif
        }

        private void InstallAdvertisement()
        {
            Container.Bind<IAdvertisement>().To<DummyAdvertisement>().AsSingle();
        }

        private void InstallAnalytics()
        {
            Container.Bind<IAnalytics>().To<UnityAnalytics>().AsSingle().NonLazy();
        }

        private void InstallSaves()
        {
            Container.Bind<ISavingManager>().To<SimpleSavingManager>().AsSingle();
        }

        private void InstallLocalization()
        {
            Container.Bind<ICsvParser>().To<CsvParserAdapter>().AsSingle();
            Container.Bind<LocalizationManager>().AsSingle();
        }

        private void InstallAudio()
        {
            Container.Bind<AudioResourceProvider>().AsSingle();
            Container.BindMemoryPool<AudioSource, AudioSourcePool>()
                .FromComponentInNewPrefabResource("Audio/AudioSource");
            Container.BindInterfacesAndSelfTo<AudioManager>().AsSingle();
        }

        private void InstallSceneReferences()
        {
            Container.Bind<SplashSceneRef>().FromNew().AsSingle();
            Container.Bind<GameSceneRef>().FromNew().AsSingle();
        }
    }
}