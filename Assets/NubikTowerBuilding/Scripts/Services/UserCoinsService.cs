using Zenject;

namespace NubikTowerBuilding.Services
{
    public class UserCoinsService
    {
        [Inject] private SavingService _saving;
        
        public delegate void CoinsChangeDelegate(int coins);
        private CoinsChangeDelegate _onCoinsChange;
        public event CoinsChangeDelegate OnCoinsChange
        {
            add
            {
                _onCoinsChange += value;
                _onCoinsChange?.Invoke(GetCoins());
            }
            remove => _onCoinsChange -= value;
        }

        public int GetCoins()
        {
            return _saving.GetCoins();
        }
        
        public void SetCoins(int coins)
        {
            if (GetCoins() == coins)
            {
                return;
            }
            
            _saving.SetCoins(coins);
            _onCoinsChange?.Invoke(coins);
        }

        public void AddCoins(int coins)
        {
            SetCoins(GetCoins() + coins);
        }

        public void SubtractCoins(int coins)
        {
            SetCoins(GetCoins() - coins);
        }
    }
}