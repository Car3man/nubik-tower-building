using System.Collections.Generic;

namespace KlopoffGames.Core.Interfaces
{
    public interface ICsvParser
    {
        List<List<string>> Parse(string content);
    }
}