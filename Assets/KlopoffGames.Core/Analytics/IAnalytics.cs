using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace KlopoffGames.Core.Analytics
{
    public interface IAnalytics
    {
        UniTask Initialize();
        void LogEvent(string name, Dictionary<string, object> parameters);
    }
}