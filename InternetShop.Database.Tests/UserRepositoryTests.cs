using InternetShop.Database.Models;
using Microsoft.EntityFrameworkCore;


namespace InternetShop.Database.Tests
{
    public class UserRepositoryTests
    {
        [Fact]
        public async Task GetByEmailAsync_ReturnsUser_WhenUserExists()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<UserDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new InMemoryDbContext(options))
            {
                context.Users.Add(new UserEntity
                {
                    Id = new Guid(),
                    Email = "test@example.com",
                    Firstname = "af",
                    Lastname = "af",
                    PasswordHash = "hashedpassword"
                });
                context.SaveChanges();

                var repository = new UserRepository(context);

                // Act
                var result = await repository.GetByEmailAsync("test@example.com", CancellationToken.None);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(new Guid(), result.Id);
                Assert.Equal("test@example.com", result.Email);
                Assert.Equal("af", result.Firstname);
                Assert.Equal("af", result.Lastname);
                Assert.Equal("hashedpassword", result.PasswordHash);
            }
        }

        [Fact]
        public async Task GetByEmailAsync_ReturnsNull_WhenUserDoesNotExist()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<UserDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new InMemoryDbContext(options))
            {
                var repository = new UserRepository(context);

                // Act
                var result = await repository.GetByEmailAsync("nonexistent@example.com", CancellationToken.None);

                // Assert
                Assert.Null(result);
            }
        }
    }
}