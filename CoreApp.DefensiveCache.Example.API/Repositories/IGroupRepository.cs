using System.Threading.Tasks;

namespace CoreApp.DefensiveCache.Tests.Contracts
{
    public interface IGroupRepository
    {
        Group GetGroup(int id);
        Task<Group> GetGroupAsync(int id);
    }
}
