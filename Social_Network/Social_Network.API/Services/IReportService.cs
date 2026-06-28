namespace Social_Network.API.Services
{
    public record TopPostDto(int PostId, string Author, string Content, int Likes, DateTime CreatedAt);
    public record TopTagDto(string Tag, int Count);
    public record StatsDto(int Users, int Posts, int Subscriptions, int Messages, int Comments, int Likes);
    public record UserReportDto(
        string Login, string? Email,
        int Posts,
        int MessagesSent,
        int LikesReceived, int LikesGiven,
        int CommentsReceived, int CommentsGiven,
        int Following, int Followers,
        List<TopTagDto> TopTags);

    public interface IReportService
    {
        Task<List<TopPostDto>> TopPostsByLikesAsync(DateTime? from, DateTime? to, int take = 20);
        Task<List<TopTagDto>> TopTagsAsync(int take = 100);
        Task<StatsDto> GetStatsAsync();
        Task<UserReportDto?> GetUserReportAsync(int userId, DateTime? from = null, DateTime? to = null);
        Task<object> ExportUserDataAsync(int userId);
    }
}
