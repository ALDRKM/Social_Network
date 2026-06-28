using System.Text.RegularExpressions;

namespace Social_Network.Helpers
{
    public static class SystemMessageHelper
    {
        private static readonly Regex MarkerRegex = new(@"^\[(SUB_REQ:\d+(?::(OK|DEN))?|SUB_OK|SUB_DEN|MENTION:\d+)\]", RegexOptions.Compiled);
        private static readonly Regex SubReqResolvedRegex = new(@"^\[SUB_REQ:(\d+):(OK|DEN)\]", RegexOptions.Compiled);

        public static string StripMarkers(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return MarkerRegex.Replace(text, string.Empty);
        }

        public static string GetDisplayText(string text)
        {
            var result = GetSubscriptionRequestResult(text);
            if (result != null) return result;
            return StripMarkers(text);
        }

        public static bool IsSubscriptionRequest(string text) =>
            text.StartsWith("[SUB_REQ:", StringComparison.Ordinal) && !IsSubscriptionRequestResolved(text);

        public static bool IsSubscriptionRequestResolved(string text) =>
            SubReqResolvedRegex.IsMatch(text);

        public static string? GetSubscriptionRequestResult(string text)
        {
            var match = SubReqResolvedRegex.Match(text);
            if (!match.Success) return null;
            return match.Groups[2].Value == "OK" ? "Одобрено" : "Отклонено";
        }

        public static int? GetSubscriptionRequestId(string text)
        {
            var match = Regex.Match(text, @"^\[SUB_REQ:(\d+)\]");
            if (!match.Success) return null;
            return int.TryParse(match.Groups[1].Value, out var id) ? id : null;
        }

        public static int? GetMentionPostId(string text)
        {
            var match = Regex.Match(text, @"^\[MENTION:(\d+)\]");
            if (!match.Success) return null;
            return int.TryParse(match.Groups[1].Value, out var id) ? id : null;
        }

        public static bool HasMarker(string text) => MarkerRegex.IsMatch(text);
    }
}
