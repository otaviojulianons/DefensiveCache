using CoreApp.DefensiveCache.Tests.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace CoreApp.DefensiveCache.Tests.Implementations
{
    public class GroupRepository : IGroupRepository
    {
        public Group GetGroup(int id)
        {
            Thread.Sleep(1000);
            return new Group() { Id = 1, Name = "Informática" };
        }

        public async Task<Group> GetGroupAsync(int id)
        {
            await Task.Delay(1000);
            return await Task.FromResult( new Group() { Id = 2, Name = "Smartphones" });
        }
    }
}
