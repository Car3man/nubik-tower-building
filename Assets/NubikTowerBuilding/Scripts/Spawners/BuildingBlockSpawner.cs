﻿using NubikTowerBuilding.Behaviours;
using NubikTowerBuilding.Factories;
using NubikTowerBuilding.Models;
using UnityEngine;

namespace NubikTowerBuilding.Spawners
{
    public class BuildingBlockSpawner
    {
        private readonly BuildingBlockFactory _factory;

        public BuildingBlockSpawner(
            BuildingBlockFactory factory
            )
        {
            _factory = factory;
        }

        public BuildingBlock Spawn(BuildingBlockType type)
        {
            var instance = _factory.Create(type);
            return instance;
        }

        public void Despawn(BuildingBlock buildingBlock)
        {
            Object.Destroy(buildingBlock.gameObject);
        }
    }
}