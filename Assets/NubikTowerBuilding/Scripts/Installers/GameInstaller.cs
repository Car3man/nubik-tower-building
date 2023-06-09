using NubikTowerBuilding.Behaviours;
using NubikTowerBuilding.Factories;
using NubikTowerBuilding.Models;
using NubikTowerBuilding.Spawners;
using Zenject;

namespace NubikTowerBuilding.Installers
{
    public class GameInstaller : MonoInstaller<GameInstaller>
    {
        public override void InstallBindings()
        {
            InstallFactories();
            InstallSpawners();
        }

        private void InstallFactories()
        {
            Container
                .BindFactory<BuildingBlockType, BuildingBlock, BuildingBlockFactory>()
                .FromMethod(CreateBuildingBlock);
            BuildingBlock CreateBuildingBlock(DiContainer diContainer, BuildingBlockType buildingBlockType)
            {
                return diContainer
                    .InstantiatePrefabResourceForComponent<BuildingBlock>($"BuildingBlocks/{buildingBlockType}");
            }
        }

        private void InstallSpawners()
        {
            Container.Bind<BuildingBlockSpawner>().FromNew().AsSingle();
        }
    }
}
