using Cysharp.Threading.Tasks;
using NubikTowerBuilding.Services;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
#if VK_GAMES && !UNITY_EDITOR
using KlopoffGames.WebPlatforms.VK;
#endif

namespace NubikTowerBuilding.Ui.Windows
{
    public class AppShareWindow : DefaultPopUpWindow
    {
        [Inject] private SavingService _saving;
#if VK_GAMES && !UNITY_EDITOR
        [Inject] private VKManager _vk;
#endif
        
        [SerializeField] private Toggle toggleDontShowAnymore;
        [SerializeField] private Button buttonNo;
        [SerializeField] private Button buttonYes;

        public override UniTask OnCreate()
        {
            buttonNo.onClick.AddListener(OnButtonNoClick);
            buttonYes.onClick.AddListener(OnButtonYesClick);
            
            return base.OnCreate();
        }

        public override async UniTask OnHide()
        {
            buttonNo.onClick.RemoveListener(OnButtonNoClick);
            buttonYes.onClick.RemoveListener(OnButtonYesClick);

            if (toggleDontShowAnymore.isOn)
            {
                _saving.SetIsAppShareDisabled(true);
            }
            
            await base.OnHide();
        }
        
        private void OnButtonNoClick()
        {
            Hide();
        }

        private void OnButtonYesClick()
        {
#if VK_GAMES && !UNITY_EDITOR
            _vk.ShowInviteBox();
#endif
            Hide();
        }
    }
}
