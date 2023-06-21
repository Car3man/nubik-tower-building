using NubikTowerBuilding.Behaviours;
using NubikTowerBuilding.Effects;
using NubikTowerBuilding.Factories;
using NubikTowerBuilding.Managers;
using NubikTowerBuilding.Models;
using NubikTowerBuilding.Pools;
using NubikTowerBuilding.Spawners;
using UnityEngine;
using Zenject;

namespace NubikTowerBuilding.Installers
{
    public class GameInstaller : MonoInstaller<GameInstaller>
    {
        [Header("Static Behaviours")]
        [SerializeField] private GameCamera gameCamera;
        [SerializeField] private BuildPlatform buildPlatform;
        [SerializeField] private Crane crane;
        [SerializeField] private Tower tower;

        [Header("Managers")]
        [SerializeField] private GameManager gameManager;
        [SerializeField] private BuildManager buildManager;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private HeightManager heightManager;
        [SerializeField] private SwingManager swingManager;
        [SerializeField] private HealthManager healthManager;
        [SerializeField] private GameOverManager gameOverManager;
        [SerializeField] private BuildEffectManager buildEffectManager;
        
        public override void InstallBindings()
        {
            InstallMainBehaviours();
            InstallManagers();
            InstallFactories();
            InstallPools();
            InstallSpawners();
        }

        private void InstallMainBehaviours()
        {
            Container.Bind<GameCamera>()
                .FromInstance(gameCamera).AsSingle().NonLazy();
            Container.Bind<BuildPlatform>()
                .FromInstance(buildPlatform).AsSingle().NonLazy();
            Container.Bind<Crane>()
                .FromInstance(crane).AsSingle().NonLazy();
            Container.Bind<Tower>()
                .FromInstance(tower).AsSingle().NonLazy();
        }

        private void InstallManagers()
        {
            Container.Bind<GameManager>()
                .FromInstance(gameManager).AsSingle().NonLazy();
            Container.Bind<BuildManager>()
                .FromInstance(buildManager).AsSingle().NonLazy();
            Container.Bind<ScoreManager>()
                .FromInstance(scoreManager).AsSingle().NonLazy();
            Container.Bind<HeightManager>()
                .FromInstance(heightManager).AsSingle().NonLazy();
            Container.Bind<SwingManager>()
                .FromInstance(swingManager).AsSingle().NonLazy();
            Container.Bind<HealthManager>()
                .FromInstance(healthManager).AsSingle().NonLazy();
            Container.Bind<GameOverManager>()
                .FromInstance(gameOverManager).AsSingle().NonLazy();
            Container.Bind<BuildEffectManager>()
                .FromInstance(buildEffectManager).AsSingle().NonLazy();
            Container.Bind<BuildingPurchaseManager>().FromNew().AsSingle().NonLazy();
        }
        
        private void InstallFactories()
        {
            Container
                .BindFactory<bool, BuildingBlockType, BuildingBlock, BuildingBlockFactory>()
                .FromMethod(CreateBuildingBlock);
            BuildingBlock CreateBuildingBlock(DiContainer diContainer, bool isFirst, BuildingBlockType buildingBlockType)
            {
                var resourcePath = $"BuildingBlocks/{buildingBlockType}";
                if (isFirst)
                {
                    resourcePath += "First";
                }
                var position = Vector3.one * 1000f;
                var rotation = Quaternion.identity;
                return diContainer
                    .InstantiatePrefabResourceForComponent<BuildingBlock>(resourcePath, position, rotation, null);
            }
        }

        private void InstallPools()
        {
            Container.BindMemoryPool<BuildPuffEffect, BuildPuffEffectPool>()
                .WithInitialSize(2)
                .FromComponentInNewPrefabResource("Effects/BuildPuffEffect");
            Container.BindMemoryPool<PerfectBuildSplashEffect, PerfectBuildSplashEffectPool>()
                .WithInitialSize(1)
                .FromComponentInNewPrefabResource("Effects/PerfectBuildSplashEffect");
            Container.BindMemoryPool<PerfectBuildIdleEffect, PerfectBuildIdleEffectPool>()
                .WithInitialSize(4)
                .FromComponentInNewPrefabResource("Effects/PerfectBuildIdleEffect");
            Container.BindMemoryPool<SettleInEffect, SettleInEffectPool>()
                .WithInitialSize(4)
                .FromComponentInNewPrefabResource("Effects/SettleInEffect");
        }

        private void InstallSpawners()
        {
            Container.Bind<BuildingBlockSpawner>().FromNew().AsSingle();
            Container.Bind<BuildPuffEffectSpawner>().FromNew().AsSingle();
            Container.Bind<PerfectBuildSplashEffectSpawner>().FromNew().AsSingle();
            Container.Bind<PerfectBuildIdleEffectSpawner>().FromNew().AsSingle();
            Container.Bind<SettleInEffectSpawner>().FromNew().AsSingle();
        }
    }
}
