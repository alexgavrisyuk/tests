using System;
using System.Net.Http;
using AccountService.Api;
using AccountService.Domain.Repositories;
using AccountService.Infrastructure;
using AccountService.IntegrationsTests.Handlers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace AccountService.IntegrationsTests.Infrastructure
{
    public class IntegrationTestBase
    {
        protected readonly HttpClient TestClient;

        protected Mock<IAccountRepository> AccountRepositoryMock;

        public IntegrationTestBase()
        {
            AccountRepositoryMock = new Mock<IAccountRepository>();
            
            var appFactory = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll(typeof(ApplicationDbContext));
                        services.AddDbContext<ApplicationDbContext>(options =>
                        {
                            options.UseInMemoryDatabase("AccountDb");
                        });
                        services.AddSingleton(AccountRepositoryMock.Object);
                        services.AddAuthentication("Test")
                            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                                "Test", options => {});
                        
                        var sp = services.BuildServiceProvider();


                        using var scope = sp.CreateScope();
                        var scopedServices = scope.ServiceProvider;
                        var db = scopedServices.GetRequiredService<ApplicationDbContext>();

                        db.Database.Migrate();

                        try
                        {
                            // Seed the database with test data.
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("An error occurred seeding the " +
                                              "database with test messages. Error: {Message}", ex.Message);
                        }
                    });
                });
            TestClient = appFactory.CreateClient();
        }
    }
}