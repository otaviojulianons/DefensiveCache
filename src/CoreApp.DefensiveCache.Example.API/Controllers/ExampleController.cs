using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApp.DefensiveCache.Tests.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoreApp.DefensiveCache.Example.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExampleController : ControllerBase
    {

        private readonly ILogger<ExampleController> _logger;
        private readonly IProductRepository _productRepository;
        private readonly IGroupRepository _groupRepository;

        public ExampleController(
            ILogger<ExampleController> logger, 
            IProductRepository productRepository,
            IGroupRepository groupRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
            _groupRepository = groupRepository;
        }

        [HttpGet("/product")]
        public ProductViewModel GetProduct()
        {
            var product = _productRepository.GetProduct(1);
            var group = _groupRepository.GetGroup(product.GroupId);
            
            return new ProductViewModel()
            {
                Id = product.Id,
                Name = product.Name,
                Group = group.Name,
            };
        }

        [HttpGet("/product-async")]
        public async Task<ProductViewModel> GetProductAsync()
        {
            var product = await _productRepository.GetProductAsync(1);
            var group = await _groupRepository.GetGroupAsync(product.GroupId);

            return new ProductViewModel()
            {
                Id = product.Id,
                Name = product.Name,
                Group = group.Name,
            };
        }

        [HttpGet("/group")]
        public GroupViewModel GetGroup()
        {
            var group = _groupRepository.GetGroup(10);

            return new GroupViewModel()
            {
                Id = group.Id,
                Name = group.Name,
            };
        }

        [HttpGet("/group-async")]
        public async Task<GroupViewModel> GetGroupAsync()
        {
            var group = await _groupRepository.GetGroupAsync(20);
            
            return new GroupViewModel()
            {
                Id = group.Id,
                Name = group.Name,
            };
        }
    }
}
