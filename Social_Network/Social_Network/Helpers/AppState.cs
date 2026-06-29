namespace Social_Network.Helpers
{
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

        public static bool SkipFeedReload { get; set; }
        public static int FeedScrollIndex { get; set; } = -1;
    }
}
