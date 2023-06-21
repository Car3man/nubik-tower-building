using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using KlopoffGames.Core.Audio;
using KlopoffGames.Core.Windows;
using NubikTowerBuilding.Scenes;
using NubikTowerBuilding.Utility;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace NubikTowerBuilding.Ui.Windows
{
    public class DefaultPopUpWindow : BaseWindow
    {
        [Inject] private SceneChanger _sceneChanger;
        [Inject] private AudioManager _audio;
        
        [SerializeField] private Image shadow;
        [SerializeField] private RectTransform panel;

        private Color _shadowColor;
        
        protected const float OpenDuration = 0.3f;
        protected const float DisposeDuration = 0.2f;
        
        public override UniTask OnCreate()
        {
            _shadowColor = shadow.color;
            
            shadow
                .DOColor(_shadowColor, OpenDuration)
                .From(Color.clear);
            
            panel
                .DOAnchorPosY(0f, OpenDuration)
                .From(new Vector2(0f, 1000f))
                .SetEase(Ease.OutBack);
            
            _audio.PlaySound("PopupOpen", false,1f, 1f);
            
            return UniTask.CompletedTask;
        }

        public override async UniTask OnHide()
        {
            if (ApplicationQuitStatus.Quiting || _sceneChanger.SceneChanging)
            {
                return;
            }
            
            var tasks = new List<UniTask>
            {
                shadow
                    .DOColor(Color.clear, DisposeDuration)
                    .From(_shadowColor)
                    .AsyncWaitForCompletion()
                    .AsUniTask(),
                
                panel
                    .DOAnchorPosY(1000f, DisposeDuration)
                    .From(new Vector2(0f, 0f))
                    .SetEase(Ease.InBack)
                    .AsyncWaitForCompletion()
                    .AsUniTask()
            };
            
            _audio.PlaySound("PopupClose", false,1f, 1f);
            await UniTask.WhenAll(tasks);
        }
    }
}
