using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

// Assuming your DbContext, entities, and repository classes are in the appropriate namespace
// using YourNamespace;

var services = new ServiceCollection();
ConfigureServices(services);

var serviceProvider = services.BuildServiceProvider();
using var scope = serviceProvider.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();
var userRepository = scope.ServiceProvider.GetRequiredService<GenericRepository<UserEntity, UserId>>();
var orderRepository = scope.ServiceProvider.GetRequiredService<GenericRepository<OrderEntity, OrderId>>();

// Example usage
await dbContext.Database.EnsureCreatedAsync();
await RunExampleAsync(userRepository, orderRepository);

static void ConfigureServices(IServiceCollection services)
{
    // Add DbContext with InMemory Database
    services.AddDbContext<MyDbContext>(options =>
        options.UseInMemoryDatabase("MyInMemoryDb"));

    // Add repositories
    services.AddScoped(typeof(GenericRepository<,>));
    services.AddScoped(typeof(GenericService<,>));
}

static async Task RunExampleAsync(GenericRepository<UserEntity, UserId> userRepo, GenericRepository<OrderEntity, OrderId> orderRepo)
{
    // Example: Add a new user
    var newUser = new UserEntity { Id = new UserId(Guid.NewGuid()) };
    await userRepo.AddAsync(newUser);

    // Example: Retrieve the user
    var retrievedUser = await userRepo.GetByIdAsync(newUser.Id);
    Console.WriteLine($"Retrieved User ID: {retrievedUser?.Id}");

    // Add more examples as needed...
}
