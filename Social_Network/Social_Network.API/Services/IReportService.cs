namespace Social_Network.API.Services
{
    public record TopPostDto(int PostId, string Author, string Content, int Likes, DateTime CreatedAt);
    public record TopTagDto(string Tag, int Count);
    public record StatsDto(int Users, int Posts, int Subscriptions, int Messages, int Comments, int Likes);

    public interface IReportService
    {
        Task<List<TopPostDto>> TopPostsByLikesAsync(DateTime? from, DateTime? to, int take = 20);
        Task<List<TopTagDto>> TopTagsAsync(int take = 20);
        Task<StatsDto> GetStatsAsync();
        Task<object> ExportUserDataAsync(int userId);
    }
}
