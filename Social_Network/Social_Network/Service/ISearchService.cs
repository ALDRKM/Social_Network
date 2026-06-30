using Social_Network.Core.Models;


namespace Social_Network.Service
{
    public interface ISearchService
    {
        Task<List<User>> SearchUsersAsync(string query);
        Task<List<Post>> SearchPostsAsync(string query);
        Task<List<Post>> SearchByTagAsync(string tag);
        Task<List<Post>> GetRecentAsync();
    }
}
