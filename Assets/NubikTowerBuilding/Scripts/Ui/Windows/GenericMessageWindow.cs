using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NubikTowerBuilding.Ui.Windows;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BubbleBlast.Common.Ui.Windows
{
    public class GenericMessageWindow : DefaultPopUpWindow
    {
        [SerializeField] private Button buttonClose;
        [SerializeField] private Button buttonNegative;
        [SerializeField] private TextMeshProUGUI buttonNegativeText;
        [SerializeField] private Button buttonPositive;
        [SerializeField] private TextMeshProUGUI buttonPositiveText;
        [SerializeField] private TextMeshProUGUI primaryTextTemplate;
        [SerializeField] private TextMeshProUGUI secondaryTextTemplate;
        [SerializeField] private RectTransform content;

        private readonly List<TextMeshProUGUI> _primaryTexts = new();
        private readonly List<TextMeshProUGUI> _secondaryTexts = new();
        private System.Action _closeCallback;
        private System.Action _negativeCallback;
        private System.Action _positiveCallback;

        public override UniTask OnCreate()
        {
            primaryTextTemplate.gameObject.SetActive(false);
            secondaryTextTemplate.gameObject.SetActive(false);
            
            buttonClose.onClick.AddListener(OnButtonCloseClick);
            buttonNegative.onClick.AddListener(OnButtonNegativeClick);
            buttonPositive.onClick.AddListener(OnButtonPositiveClick);
            
            return base.OnCreate();
        }

        public override async UniTask OnHide()
        {
            buttonClose.onClick.RemoveListener(OnButtonCloseClick);
            buttonNegative.onClick.RemoveListener(OnButtonNegativeClick);
            buttonPositive.onClick.RemoveListener(OnButtonPositiveClick);

            await base.OnHide();
        }
        
        private void OnButtonCloseClick()
        {
            Hide();
            _closeCallback?.Invoke();
        }

        private void OnButtonNegativeClick()
        {
            Hide();
            _negativeCallback?.Invoke();
        }

        private void OnButtonPositiveClick()
        {
            Hide();
            _positiveCallback?.Invoke();
        }
        
        private void ApplyBuilder(
            List<string> primaryTexts,
            List<string> secondaryTexts,
            System.Action closeCallback,
            (string, System.Action) negativeCallback,
            (string, System.Action) positiveCallback
            )
        {
            foreach (var primaryText in _primaryTexts)
            {
                Destroy(primaryText.gameObject);
            }
            _primaryTexts.Clear();

            foreach (var secondaryText in _secondaryTexts)
            {
                Destroy(secondaryText.gameObject);
            }
            _secondaryTexts.Clear();
            
            foreach (var primaryText in primaryTexts)
            {
                if (string.IsNullOrEmpty(primaryText))
                {
                    continue;
                }
                
                var primaryTextInstance = Instantiate(primaryTextTemplate, content);
                primaryTextInstance.text = primaryText;
                primaryTextInstance.gameObject.SetActive(true);
                _primaryTexts.Add(primaryTextInstance);
            }
            
            foreach (var secondaryText in secondaryTexts)
            {
                if (string.IsNullOrEmpty(secondaryText))
                {
                    continue;
                }
                
                var secondaryTextInstance = Instantiate(secondaryTextTemplate, content);
                secondaryTextInstance.text = secondaryText;
                secondaryTextInstance.gameObject.SetActive(true);
                _secondaryTexts.Add(secondaryTextInstance);
            }

            _closeCallback = closeCallback;
            _negativeCallback = negativeCallback.Item2;
            _positiveCallback = positiveCallback.Item2;

            buttonClose.gameObject.SetActive(_closeCallback != null);
            buttonNegative.gameObject.SetActive(_negativeCallback != null);
            buttonPositive.gameObject.SetActive(_positiveCallback != null);
            
            buttonNegativeText.text = negativeCallback.Item1;
            buttonPositiveText.text = positiveCallback.Item1;
        }
        
        public Builder Build()
        {
            return new Builder(this);
        }

        public class Builder
        {
            private readonly GenericMessageWindow _window;
            private readonly List<string> _primaryTexts = new();
            private readonly List<string> _secondaryTexts = new();
            private System.Action _closeCallback;
            private (string, System.Action) _negativeCallback;
            private (string, System.Action) _positiveCallback;

            public Builder(GenericMessageWindow window)
            {
                _window = window;
            }
            
            public Builder AddPrimaryText(string text)
            {
                _primaryTexts.Add(text);
                return this;
            }
            
            public Builder AddSecondaryText(string text)
            {
                _secondaryTexts.Add(text);
                return this;
            }

            public Builder SetCloseButton(System.Action callback)
            {
                _closeCallback = callback;
                return this;
            }
            
            public Builder SetNegativeButton(string text, System.Action callback)
            {
                _negativeCallback = (text, callback);
                return this;
            }
            
            public Builder SetPositiveButton(string text, System.Action callback)
            {
                _positiveCallback = (text, callback);
                return this;
            }

            public void Commit()
            {
                _window.ApplyBuilder(
                    _primaryTexts,
                    _secondaryTexts,
                    _closeCallback,
                    _negativeCallback,
                    _positiveCallback
                );
            }
        }
    }
}
