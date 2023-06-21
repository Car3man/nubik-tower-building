using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace NubikTowerBuilding.Scenes
{
    public class SceneChanger
    {
        public bool SceneChanging { get; private set; }
        
        public async void LoadScene(string name)
        {
            if (SceneChanging)
            {
                throw new System.Exception("Cannot load scene at another scene load process");
            }

            SceneChanging = true;
            await SceneManager.LoadSceneAsync(name);
            SceneChanging = false;
        }
    }
}