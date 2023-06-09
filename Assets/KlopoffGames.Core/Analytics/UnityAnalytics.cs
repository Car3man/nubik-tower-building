using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Services.Analytics;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

namespace KlopoffGames.Core.Analytics
{
    public class UnityAnalytics : IAnalytics
    {
        private readonly ProjectEnvironment _projectEnvironment;

        public UnityAnalytics(
            ProjectEnvironment projectEnvironment
            )
        {
            _projectEnvironment = projectEnvironment;
        }

        public async UniTask Initialize()
        {
            var options = new InitializationOptions();
            options.SetEnvironmentName(_projectEnvironment.Name);
            
            try
            {
                await UnityServices.InitializeAsync(options);
                
                var consentIdentifiers = await AnalyticsService.Instance.CheckForRequiredConsents();
                foreach (var consentIdentifier in consentIdentifiers)
                {
                    Debug.Log($"[UnityAnalytics] Required consent: {consentIdentifier}");
                }
            }
            catch (ConsentCheckException e)
            {
                Debug.LogWarning("[UnityAnalytics] Initialization error: " + e.Reason);
            }
        }

        public void LogEvent(string name, Dictionary<string, object> parameters)
        {
            AnalyticsService.Instance.CustomData(name, parameters);
        }
    }
}
