namespace KlopoffGames.Core.Saving
{
    public interface ISavingManager
    {
        public delegate void LoadDelegate();
        public delegate void SaveDelegate();

        void SetAvailableKeys(string[] availableKeys);
        
        string GetString(string key, string defaultValue);
        int GetInt(string key, int defaultValue);
        float GetFloat(string key, float defaultValue);
        bool GetBool(string key, bool defaultValue);

        void SetString(string key, string value);
        void SetInt(string key, int value);
        void SetFloat(string key, float value);
        void SetBool(string key, bool value);

        void Load(LoadDelegate onEnd);
        void Save(SaveDelegate onEnd);
    }
}
