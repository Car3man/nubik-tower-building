using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using KlopoffGames.Core.Saving;
using UnityEngine;

namespace KlopoffGames.WebPlatforms.Yandex
{
    public class YandexSavingManager : ISavingManager
    {
        private readonly YandexManager _yandex;
        private readonly Dictionary<string, object> _data = new();
        private string[] _availableKeys;

        public YandexSavingManager(
            YandexManager yandex
            )
        {
            _yandex = yandex;
        }

        public void SetAvailableKeys(string[] availableKeys)
        {
            _availableKeys = availableKeys;
        }

        private bool IsKeyAvailable(string key)
        {
            if (_availableKeys != null)
            {
                return _availableKeys.Contains(key);
            }
            return true;
        }

        public string GetString(string key, string defaultValue)
        {
            if (_data.TryGetValue(key, out var value))
            {
                if (value is string stringVal)
                {
                    return stringVal;
                }
            }
            return defaultValue;
        }

        public int GetInt(string key, int defaultValue)
        {
            if (_data.TryGetValue(key, out var value))
            {
                if (int.TryParse(value.ToString(), out var intVal))
                {
                    return intVal;
                }
            }
            return defaultValue;
        }

        public float GetFloat(string key, float defaultValue)
        {
            if (_data.TryGetValue(key, out var value))
            {
                if (float.TryParse(value.ToString(), out var floatVal))
                {
                    return floatVal;
                }
            }
            return defaultValue;
        }

        public bool GetBool(string key, bool defaultValue)
        {
            if (_data.TryGetValue(key, out var value))
            {
                if (bool.TryParse(value.ToString(), out var boolVal))
                {
                    return boolVal;
                }
            }
            return defaultValue;
        }

        public void SetString(string key, string value)
        {
            SetValue(key, value);
        }

        public void SetInt(string key, int value)
        {
            SetValue(key, value);
        }

        public void SetFloat(string key, float value)
        {
            SetValue(key, value);
        }

        public void SetBool(string key, bool value)
        {
            SetValue(key, value);
        }

        private void SetValue(string key, object value)
        {
            if (IsKeyAvailable(key))
            {
                _data[key] = value;
                _yandex.SetPlayerData(_data, false);
            }
            else
            {
                Debug.LogWarning($"Trying to set value on unavailable key: {key}");
            }
        }

        public async void Load(ISavingManager.LoadDelegate onEnd)
        {
            await UniTask.WaitUntil(() => _yandex.IsSdkReady);

            _yandex.OnPlayerDataReceive += PlayerDataReceived;
            _yandex.GetPlayerData(_availableKeys);
            
            void PlayerDataReceived(Dictionary<string, object> playerData)
            {
                _yandex.OnPlayerDataReceive -= PlayerDataReceived;

                foreach (var (key, value) in playerData)
                {
                    if (!IsKeyAvailable(key))
                    {
                        continue;
                    }
                    
                    _data[key] = value;
                }
                
                onEnd?.Invoke();
            }
        }

        public void Save(ISavingManager.SaveDelegate onEnd)
        {
            _yandex.SetPlayerData(_data, false);
            onEnd?.Invoke();
        }
    }
}