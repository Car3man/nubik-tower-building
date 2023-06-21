using KlopoffGames.Core.Windows;
using NubikTowerBuilding.Ui.Windows;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace NubikTowerBuilding.Ui
{
    public class InGameCanvas : MonoBehaviour
    {
        [Inject] private WindowManager _windows;
        [SerializeField] private Button buttonSettings;
        
        private void OnEnable()
        {
            buttonSettings.onClick.AddListener(OnButtonSettingsClick);
        }

        private void OnDisable()
        {
            buttonSettings.onClick.RemoveAllListeners();
        }

        private void OnButtonSettingsClick()
        {
            _windows.CreateWindow<SettingsWindow>("Windows/Game");
        }
    }
}
