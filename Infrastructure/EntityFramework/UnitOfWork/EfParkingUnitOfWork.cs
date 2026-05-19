using AppCore.Repositories;
using Infrastructure.EntityFramework.Context;

namespace Infrastructure.EntityFramework.UnitOfWork;

public class EfParkingUnitOfWork(
    IVehicleRepository vehicleRepository,
    IParkingGateRepository gatesRepository,
    IParkingSessionRepository sessionRepository,
    ParkingDbContext context) : IParkingUnitOfWork
{
    public IVehicleRepository Vehicles => vehicleRepository;
    public IParkingGateRepository Gates => gatesRepository;
    public IParkingSessionRepository Sessions => sessionRepository;
    
    public Task<int> SaveChangesAsync() => context.SaveChangesAsync();
    public Task BeginTransactionAsync() => context.Database.BeginTransactionAsync();
    public Task CommitTransactionAsync() => context.Database.CommitTransactionAsync();
    public Task RollbackTransactionAsync() => context.Database.RollbackTransactionAsync();
}