using System;
using TMPro;
using UnityEngine;
using Zenject;

namespace KlopoffGames.Core.Localization
{
	public class LocalizationTextMeshProUGUI : MonoBehaviour 
	{
		[SerializeField] private string id;

		private LocalizationManager _localizationManager;
		private TextMeshProUGUI _text;

		[Inject]
		public void Construct(LocalizationManager localizationManager)
		{
			_localizationManager = localizationManager;
		}

		private void OnValidate()
		{
			if (GetComponent<TextMeshProUGUI>() == null)
			{
				throw new NullReferenceException($"No {nameof(TextMeshProUGUI)} attached to gameObject");
			}
		}

		private void OnEnable()
		{
			_text = GetComponent<TextMeshProUGUI>();
			
			if (_localizationManager != null)
			{
				_localizationManager.OnLocalizationLoad += OnLocalizationLoad;
				UpdateText();
			}
		}

		private void OnDisable()
		{
			if (_localizationManager != null)
			{
				_localizationManager.OnLocalizationLoad -= OnLocalizationLoad;
			}
		}

		public void UpdateText () 
		{
			if (_text != null)
			{
				_text.text = _localizationManager.GetString(id);
			}
		}
	
		private void OnLocalizationLoad(string language)
		{
			UpdateText();
		}
	}
}
