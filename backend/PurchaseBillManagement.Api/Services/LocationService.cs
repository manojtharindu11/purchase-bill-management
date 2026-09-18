using Microsoft.EntityFrameworkCore;
using PurchaseBillManagement.Api.Data;
using PurchaseBillManagement.Api.DTOs.Locations;

namespace PurchaseBillManagement.Api.Services
{
    public interface ILocationService
    {
        Task<List<LocationDto>> GetAllAsync(CancellationToken cancellationToken = default);
    }

    public class LocationService : ILocationService
    {
        private readonly AppDbContext _dbContext;

        public LocationService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<LocationDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.LocationDetails
                .AsNoTracking()
                .OrderBy(l => l.LocationName)
                .Select(l => new LocationDto
                {
                    LocationCode = l.LocationCode,
                    LocationName = l.LocationName
                })
                .ToListAsync(cancellationToken);
        }
    }
}
