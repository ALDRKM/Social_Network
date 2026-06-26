namespace Social_Network.Helpers
{
    // Глобальное состояние для индикаторов в меню навигации (например, непрочитанные сообщения)
    public static class AppState
    {
        private static bool _hasUnreadChats;

        public static bool HasUnreadChats
        {
            get => _hasUnreadChats;
            set
            {
                if (_hasUnreadChats == value) return;
                _hasUnreadChats = value;
                UnreadChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        public static event EventHandler? UnreadChanged;
    }
}
