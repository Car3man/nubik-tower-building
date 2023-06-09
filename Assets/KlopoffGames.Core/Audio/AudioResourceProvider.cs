using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace KlopoffGames.Core.Audio
{
    public class AudioResourceProvider
    {
        private readonly Dictionary<string, AudioClip> _cache = new();
        
        public async Task<AudioClip> Get(string name, string category)
        {
            if (_cache.TryGetValue(name, out var cachedClip))
            {
                return cachedClip;
            }
            
            string filePath = GetPath(name, category);
            using var request = UnityWebRequestMultimedia.GetAudioClip(filePath, AudioType.MPEG);
            await request.SendWebRequest().ToUniTask();
            if (request.result == UnityWebRequest.Result.Success)
            {
                var clip = DownloadHandlerAudioClip.GetContent(request);
                _cache[name] = clip;
                return clip;
            }

            Debug.LogWarning($"Get audio clip error, path: {filePath}, error: {request.error}");
            return null;
        }

        private string GetPath(string name, string category)
        {
            return System.IO.Path.Combine(Application.streamingAssetsPath, $"Audio/{category}/{name}.mp3");
        }
    }
}
