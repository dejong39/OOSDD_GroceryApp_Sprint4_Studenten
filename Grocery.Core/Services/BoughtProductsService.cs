
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class BoughtProductsService : IBoughtProductsService
    {
        private readonly IGroceryListItemsRepository _groceryListItemsRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IProductRepository _productRepository;
        private readonly IGroceryListRepository _groceryListRepository;
        public BoughtProductsService(IGroceryListItemsRepository groceryListItemsRepository, IGroceryListRepository groceryListRepository, IClientRepository clientRepository, IProductRepository productRepository)
        {
            _groceryListItemsRepository=groceryListItemsRepository;
            _groceryListRepository=groceryListRepository;
            _clientRepository=clientRepository;
            _productRepository=productRepository;
        }
        public List<BoughtProducts> Get(int? productId)
        {
            if (!productId.HasValue)
            {
                return new List<BoughtProducts>();
            }

            var items = _groceryListItemsRepository.GetAll()
                .Where(item => item.ProductId == productId.Value)
                .ToList();

            var groceryListIds = items.Select(item => item.GroceryListId).Distinct();

            var groceryLists = _groceryListRepository.GetAll()
                .Where(gl => groceryListIds.Contains(gl.Id))
                .ToList();

            var clientIds = groceryLists.Select(gl => gl.ClientId).Distinct();

            var clients = _clientRepository.GetAll()
                .Where(c => clientIds.Contains(c.Id))
                .ToList();

            var result = new List<BoughtProducts>();
            var product = _productRepository.Get(productId.Value);

            foreach (var client in clients)
            {
                var clientGroceryLists = groceryLists.Where(gl => gl.ClientId == client.Id).ToList();
                foreach (var groceryList in clientGroceryLists)
                {
                    result.Add(new BoughtProducts(
                        client: client,
                        groceryList: groceryList,
                        product: product
                    ));
                }
            }

            return result;
        }
    }
}
