using CoreApp.DefensiveCache.Benchmark.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreApp.DefensiveCache.Benchmark.Repositories
{
    public class TestRepository : ITestRepository
    {
        public Task<Entity> GetObjectAsync(int id)
        {
            return Task.FromResult(new Entity()
            {
                Id = 10,
                Name = "Otávio Juliano"
            });
        }

        public Entity GetObject(int id)
        {
            return new Entity()
            {
                Id = 20,
                Name = "Otávio Nielsen"
            };
        }

        public Task<List<Entity>> GetListObjectAsync(ParamQuery filter)
        {
            return Task.FromResult(new List<Entity>()
            {
                new Entity()
                {
                    Id = 10,
                    Name = "Otávio Juliano"
                },
                new Entity()
                {
                    Id = 20,
                    Name = "Otávio Nielsen"
                }
            });
        }

        public Task<string> GetStringAsync(int id)
        {
            return Task.FromResult("Success");
        }

        public string GetString(int id)
        {
            return "sucess";
        }

        public Task<int?> GetIntAsync(int id)
        {
            int? valor = 5;
            return Task.FromResult<int?>(valor);
        }

        public int? GetInt(int id)
        {
            return null;
        }

        public Task<DateTime> GetDateTimeAsync(int id)
        {
            return Task.FromResult(new DateTime(2020, 11, 16));
        }

        public DateTime GetDateTime(int id)
        {
            return new DateTime(1991, 11, 16);
        }

        public Task<bool> GetBoolAsync(int id)
        {
            return Task.FromResult(true);
        }

        public bool GetBool(int id)
        {
            return true;
        }

        public List<Entity> GetListObject(ParamQuery filter)
        {
            return new List<Entity>()
            {
                new Entity()
                {
                    Id = 10,
                    Name = "Otávio Juliano"
                },
                new Entity()
                {
                    Id = 20,
                    Name = "Otávio Nielsen"
                }
            };
        }
    }
}