using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreApp.DefensiveCache.Benchmark.Models;

namespace CoreApp.DefensiveCache.Benchmark.Repositories
{
    public interface ITestRepository
    {
        Task<string> GetStringAsync(int id);

        string GetString(int id);

        Task<int?> GetIntAsync(int id);

        int? GetInt(int id);

        Task<DateTime> GetDateTimeAsync(int id);

        DateTime GetDateTime(int id);

        Task<bool> GetBoolAsync(int id);

        bool GetBool(int id);

        Task<Entity> GetObjectAsync(int id);

        Entity GetObject(int id);

        Task<List<Entity>> GetListObjectAsync(ParamQuery filter);

        List<Entity> GetListObject(ParamQuery filter);
    }
}