using Moq;
using StudyRoomBooking.Models;
using StudyRoomBooking.Repositories;
using StudyRoomBooking.Services;
using Xunit;

namespace Subapp1.Tests;

public class FacilitiesServiceTests
{
    [Fact]
    public async Task BuildingCreate_TrimsAndSavesName()
    {
        var repository = new Mock<IBuildingRepository>();
        var building = new Building { Name = "  Pilestredet 52  ", Address = "  Oslo  " };

        var error = await new BuildingService(repository.Object).CreateAsync(building);

        Assert.Null(error);
        repository.Verify(r => r.AddAsync(It.Is<Building>(b =>
            b.Name == "Pilestredet 52" && b.Address == "Oslo")), Times.Once);
    }

    [Fact]
    public async Task BuildingWithRooms_CannotBeDeleted()
    {
        var repository = new Mock<IBuildingRepository>();
        repository.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(new Building { Id = 4, Name = "P50" });
        repository.Setup(r => r.HasRoomsAsync(4)).ReturnsAsync(true);

        var error = await new BuildingService(repository.Object).DeleteAsync(4);

        Assert.Contains("Move or delete", error);
        repository.Verify(r => r.DeleteAsync(It.IsAny<Building>()), Times.Never);
    }

    [Fact]
    public async Task BuildingName_DuplicateIsRejected()
    {
        var repository = new Mock<IBuildingRepository>();
        repository.Setup(r => r.NameExistsAsync("P50", null)).ReturnsAsync(true);

        var error = await new BuildingService(repository.Object)
            .CreateAsync(new Building { Name = " P50 " });

        Assert.Contains("already exists", error);
        repository.Verify(r => r.AddAsync(It.IsAny<Building>()), Times.Never);
    }

    [Fact]
    public async Task AmenityName_DuplicateIsRejected()
    {
        var repository = new Mock<IAmenityRepository>();
        repository.Setup(r => r.NameExistsAsync("Whiteboard", null)).ReturnsAsync(true);

        var error = await new AmenityService(repository.Object)
            .CreateAsync(new Amenity { Name = " Whiteboard " });

        Assert.Contains("already exists", error);
        repository.Verify(r => r.AddAsync(It.IsAny<Amenity>()), Times.Never);
    }

    [Fact]
    public async Task AmenityUpdate_ChangesExistingItem()
    {
        var repository = new Mock<IAmenityRepository>();
        var stored = new Amenity { Id = 3, Name = "Old" };
        repository.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(stored);

        var error = await new AmenityService(repository.Object).UpdateAsync(3,
            new Amenity { Name = "  Whiteboard  ", Description = "  Large  " });

        Assert.Null(error);
        Assert.Equal("Whiteboard", stored.Name);
        Assert.Equal("Large", stored.Description);
        repository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RoomWithUnknownBuilding_IsRejected()
    {
        var repository = new Mock<IRoomRepository>();
        repository.Setup(r => r.BuildingExistsAsync(99)).ReturnsAsync(false);

        var error = await new RoomService(repository.Object)
            .SaveAsync(new Room { RoomNumber = "101", BuildingId = 99, Capacity = 6 }, new List<int>());

        Assert.Equal("Select an existing building.", error);
        repository.Verify(r => r.SaveWithAmenitiesAsync(It.IsAny<Room>(), It.IsAny<List<int>>()), Times.Never);
    }

    [Fact]
    public async Task RoomWithBookings_CannotBeDeletedFromAdmin()
    {
        var repository = new Mock<IRoomRepository>();
        repository.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(new Room { Id = 7, RoomNumber = "101" });
        repository.Setup(r => r.HasBookingsAsync(7)).ReturnsAsync(true);

        var error = await new RoomService(repository.Object).DeleteForAdminAsync(7);

        Assert.Contains("has bookings", error);
        repository.Verify(r => r.DeleteAsync(It.IsAny<Room>()), Times.Never);
    }
}
