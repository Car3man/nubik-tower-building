using System.Collections.Generic;
using KlopoffGames.Core.Interfaces;
using UnityEngine;

namespace KlopoffGames.Core.Localization
{
    public class LocalizationManager
    {
        private readonly ICsvParser _csvParser;
        private readonly Dictionary<string, string> _localization;
        private string _currentLanguage;

        private const string LocalizationResourceFilesPath = "Localization";
        private const string DefaultLanguage = "ru";

        public delegate void LocalizationLoadDelegate(string language);
        public event LocalizationLoadDelegate OnLocalizationLoad;

        public LocalizationManager(
            ICsvParser csvParser
        )
        {
            _csvParser = csvParser;
            _localization = new Dictionary<string, string>();
            
            LoadLocalization(DefaultLanguage);
        }

        private void LoadLocalization(string language)
        {
            var localizationStringsPath = GetLocalizationStringsPath(language);
            Debug.Log($"Localization loaded, path: {localizationStringsPath}");
            
            var textAsset = Resources.Load<TextAsset>(localizationStringsPath);
            if (textAsset == null)
            {
                Debug.LogWarning($"Localization text asset didn't found, path: {localizationStringsPath}");
                
                if (language != DefaultLanguage && _currentLanguage != DefaultLanguage) 
                {
                    Debug.LogWarning($"Trying switch language to fallback value ({DefaultLanguage})");
                    LoadLocalization(language);
                }
                return;
            }
            
            _localization.Clear();
            var rawLocalization = _csvParser.Parse(textAsset.text);
            for (int row = 1; row < rawLocalization.Count; row++)
            {
                if (string.IsNullOrEmpty(rawLocalization[row][0]))
                {
                    continue;
                }
                _localization.Add(rawLocalization[row][0], rawLocalization[row][1]);
            }
            _currentLanguage = language;
            
            OnLocalizationLoad?.Invoke(language);
        }

        public void SetLanguage(string language)
        {
            LoadLocalization(language);
        }
        
        public string GetString(string id)
        {
            if (!_localization.ContainsKey(id))
            {
                Debug.LogWarning($"Warning, localization string not found by provided id: {id}");
                return id;
            }
            return _localization[id];
        }

        private string GetLocalizationStringsPath(string language)
        {
            return $"{LocalizationResourceFilesPath}/{language}/strings";
        }
    }
}