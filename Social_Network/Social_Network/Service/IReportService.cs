namespace Social_Network.Service
{
    public record TopPostDto(int PostId, string Author, string Content, int Likes, DateTime CreatedAt);
    public record TopTagDto(string Tag, int Count);
    public record StatsDto(int Users, int Posts, int Subscriptions, int Messages, int Comments, int Likes);

    public interface IReportService
    {
        Task<List<TopPostDto>> TopPostsByLikesAsync(DateTime? from = null, DateTime? to = null);
        Task<List<TopTagDto>> TopTagsAsync();
        Task<StatsDto?> GetStatsAsync();
        Task<string?> ExportUserDataAsync(int userId);
    }
}
