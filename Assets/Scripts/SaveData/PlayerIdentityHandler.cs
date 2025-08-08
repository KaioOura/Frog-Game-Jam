namespace SaveData
{
    public class PlayerIdentityHandler 
    {
        private PlayerData _playerData;
    
        public PlayerIdentityHandler(PlayerData playerData)
        {
            _playerData = playerData;
        }
    
        public string GetUsername()
        {
            return _playerData?.Username ?? "Unknown";
        }

        public void SetUsername(string newUsername)
        {
            _playerData.Username = newUsername;
        }
        
        public string GetUserID()
        {
            return _playerData?.UserID ?? "Unknown";
        }

        public void SetUserID(string newUserID)
        {
            _playerData.UserID = newUserID;
        }
    }
}
