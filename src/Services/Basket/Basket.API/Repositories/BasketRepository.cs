using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _redisCache;
        public BasketRepository(IDistributedCache distributedCache)
        {
            this._redisCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        }
        public async Task DeleteBasket(string userName)
        {
            await _redisCache.RemoveAsync(userName);
        }

        public async Task<ShoppingCart> GetBasket(string userName)
        {
            var basket  = await _redisCache.GetStringAsync(userName);
            if (string.IsNullOrEmpty(basket)) return null;

            return JsonConvert.DeserializeObject<ShoppingCart>(basket);
        }

        public async Task<ShoppingCart> UpdateBasket(ShoppingCart basket)
        {
            var basketJson = JsonConvert.SerializeObject(basket);
            await _redisCache.SetStringAsync(basket.UserName, basketJson);
            return await GetBasket(basket.UserName);
        }
    }
}
