using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain_layer.Entities;
using Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IMapper _mapper;

        public FavoriteService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public Task<FavoriteDto> AddFavoriteAsync(int customerId, int workerId)
        {
            if (MockDatabase.Favorites.Any(f => f.CustomerId == customerId && f.WorkerId == workerId))
                return Task.FromResult<FavoriteDto>(null!);

            int newId = MockDatabase.Favorites.Any() ? MockDatabase.Favorites.Max(f => f.Id) + 1 : 1;
            var favorite = new Favorite 
            { 
                Id = newId, 
                CustomerId = customerId, 
                WorkerId = workerId, 
                Worker = MockDatabase.Workers.FirstOrDefault(w => w.Id == workerId),
                AddedAt = DateTime.UtcNow 
            };
            MockDatabase.Favorites.Add(favorite);

            return Task.FromResult(_mapper.Map<FavoriteDto>(favorite));
        }

        public Task<bool> RemoveFavoriteAsync(int customerId, int workerId)
        {
            var favorite = MockDatabase.Favorites.FirstOrDefault(f => f.CustomerId == customerId && f.WorkerId == workerId);
            if (favorite != null)
            {
                MockDatabase.Favorites.Remove(favorite);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<IEnumerable<FavoriteDto>> GetCustomerFavoritesAsync(int customerId)
        {
            var items = MockDatabase.Favorites.Where(f => f.CustomerId == customerId).ToList();
            var mappedItems = _mapper.Map<IEnumerable<FavoriteDto>>(items);
            return Task.FromResult(mappedItems);
        }
    }
}
