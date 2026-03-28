using AppCore.Repositories;
using AppCore.Models;
using Infrastructure.Memory;

namespace UnitTest;

public class MemoryGenericRepositoryTest
{
    private IGenericRepositoryAsync<Vehicle>  _repo = new MemoryGenericRepository<Vehicle>();

    [Fact]
    public async Task AddVehicleToRepositoryTestAsync()
    {
        // Arrange
        var expected = new Vehicle()
        {
            LicensePlate = "TK 8434Y"
        };
        // Act
        await _repo.AddAsync(expected);
        // Assert
        var actual = await _repo.FindByIdAsync(expected.Id);
        Assert.NotNull(actual);
        Assert.Equal(expected, actual);
        Assert.Equal(expected.Id, actual?.Id);
    }
    
    [Fact]
    public async Task AddAsync_ThrowsInvalidOperationException_WhenIdAlreadyExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var firstVehicle = new Vehicle { Id = id, LicensePlate = "TK 123" };
        var duplicateVehicle = new Vehicle { Id = id, LicensePlate = "TK 456" };
        await _repo.AddAsync(firstVehicle);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _repo.AddAsync(duplicateVehicle));
    }

    // --- FIND BY ID ASYNC ---

    [Fact]
    public async Task FindByIdAsync_ReturnsNull_WhenEntityDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repo.FindByIdAsync(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    // --- FIND ALL ASYNC ---

    [Fact]
    public async Task FindAllAsync_ReturnsAllEntities()
    {
        // Arrange
        await _repo.AddAsync(new Vehicle { LicensePlate = "W1 111" });
        await _repo.AddAsync(new Vehicle { LicensePlate = "W2 222" });

        // Act
        var results = await _repo.FindAllAsync();

        // Assert
        Assert.NotNull(results);
        Assert.Equal(2, results.Count());
    }

    // --- FIND PAGED ASYNC ---

    [Fact]
    public async Task FindPagedAsync_ReturnsCorrectPageAndCount()
    {
        // Arrange
        for (int i = 1; i <= 5; i++)
        {
            await _repo.AddAsync(new Vehicle { LicensePlate = $"TEST {i}" });
        }

        
        var pagedResult = await _repo.FindPagedAsync(page: 2, pageSize: 2);

        // Assert
        Assert.NotNull(pagedResult);
        Assert.Equal(5, pagedResult.TotalCount);
        Assert.Equal(2, pagedResult.Page);
        Assert.Equal(2, pagedResult.PageSize);
        Assert.Equal(2, pagedResult.Items.Count);
    }

    // --- UPDATE ASYNC ---

    [Fact]
    public async Task UpdateAsync_UpdatesExistingEntity()
    {
        // Arrange
        var vehicle = new Vehicle { LicensePlate = "OLD PLATE" };
        await _repo.AddAsync(vehicle);

        var updatedVehicle = new Vehicle { LicensePlate = "NEW PLATE" };

        // Act
        await _repo.UpdateAsync(vehicle.Id, updatedVehicle);

        // Assert
        var actual = await _repo.FindByIdAsync(vehicle.Id);
        Assert.NotNull(actual);
        Assert.Equal("NEW PLATE", actual.LicensePlate);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsKeyNotFoundException_WhenEntityDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var vehicleToUpdate = new Vehicle { LicensePlate = "TEST" };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _repo.UpdateAsync(nonExistentId, vehicleToUpdate));
    }

    // --- REMOVE BY ID ASYNC ---

    [Fact]
    public async Task RemoveByIdAsync_RemovesEntity_WhenItExists()
    {
        // Arrange
        var vehicle = new Vehicle { LicensePlate = "TO DELETE" };
        await _repo.AddAsync(vehicle);

        // Act
        await _repo.RemoveByIdAsync(vehicle.Id);

        // Assert
        var result = await _repo.FindByIdAsync(vehicle.Id);
        Assert.Null(result);
    }

    [Fact]
    public async Task RemoveByIdAsync_ThrowsKeyNotFoundException_WhenEntityDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _repo.RemoveByIdAsync(nonExistentId));
    }
}