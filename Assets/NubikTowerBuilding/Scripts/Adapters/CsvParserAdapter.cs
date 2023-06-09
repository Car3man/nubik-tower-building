using System.Collections.Generic;
using KlopoffGames.Core.Interfaces;
using yutokun;

namespace NubikTowerBuilding.Adapters
{
    public class CsvParserAdapter : ICsvParser
    {
        public List<List<string>> Parse(string content)
        {
            return CSVParser.LoadFromString(content);
        }
    }
}