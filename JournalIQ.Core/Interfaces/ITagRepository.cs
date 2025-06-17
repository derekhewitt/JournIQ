
namespace JournalIQ.Core
{
    public interface ITagRepository
    {
        Task<Tag> GetOrCreateAsync(string name);
    }

}
