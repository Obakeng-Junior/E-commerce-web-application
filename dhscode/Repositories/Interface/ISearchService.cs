using DHSOnlineStore.Models;

namespace DHSOnlineStore.Repositories.Interface
{
    public interface ISearchService
    {
        public interface ISearchService
        {
            Task<List<SearchResult>> SearchAsync(string query);
        }
    }
}
