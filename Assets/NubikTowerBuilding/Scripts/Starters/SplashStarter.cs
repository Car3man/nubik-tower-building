using NubikTowerBuilding.Scenes;
using UnityEngine;
using Zenject;

namespace NubikTowerBuilding.Starters
{
    public class SplashStarter : MonoBehaviour
    {
        [Inject] private GameSceneRef _gameSceneRef;
        
        private void Start()
        {
            _gameSceneRef.Load();
        }
    }
}
