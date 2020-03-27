using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AccountService.Core.Dtos;
using AccountService.Domain.Models;
using AccountService.IntegrationsTests.Infrastructure;
using Newtonsoft.Json;
using Xunit;

namespace AccountService.IntegrationsTests.ControllerTests
{
    public class AccountControllerTests : IntegrationTestBase
    {
        private List<Account> Accounts = new List<Account>()
        {
            new Account(Guid.NewGuid())
        };

        [Fact]
        public async Task ShouldReturnAllAsync()
        {
            AccountRepositoryMock.Setup(x => x.GetAllAsync())
                .Returns(Task.FromResult(Accounts));

            var response = await TestClient.GetAsync("Account");
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<AccountDto>>(content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, result.Count);
        }
    }
}