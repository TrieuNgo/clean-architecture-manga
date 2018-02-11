namespace Acerola.Domain.UnitTests
{
    using Xunit;
    using Acerola.Domain.Accounts;
    using Acerola.Domain.Customers;
    using NSubstitute;
    using Acerola.Application;
    using Acerola.Infrastructure.Mappings;
    using Acerola.UseCaseTests;
    using System;

    public class CustomerTests
    {
        public ICustomerReadOnlyRepository customerReadOnlyRepository;
        public ICustomerWriteOnlyRepository customerWriteOnlyRepository;

        public IAccountReadOnlyRepository accountReadOnlyRepository;
        public IAccountWriteOnlyRepository accountWriteOnlyRepository;

        public IResponseConverter converter;

        public CustomerTests()
        {
            customerReadOnlyRepository = Substitute.For<ICustomerReadOnlyRepository>();
            customerWriteOnlyRepository = Substitute.For<ICustomerWriteOnlyRepository>();
            accountReadOnlyRepository = Substitute.For<IAccountReadOnlyRepository>();
            accountWriteOnlyRepository = Substitute.For<IAccountWriteOnlyRepository>();
            converter = new ResponseConverter();
        }

        [Theory]
        [InlineData("08724050601", "Ivan Paulovich", 300)]
        [InlineData("08724050601", "Ivan Paulovich Pinheiro Gomes", 100)]
        [InlineData("444", "Ivan Paulovich", 500)]
        [InlineData("08724050", "Ivan Paulovich", 300)]
        public async void Register_Valid_User_Account(string personnummer, string name, double amount)
        {
            var output = Substitute.For<CustomPresenter<Application.UseCases.Register.Response>>();

            var registerUseCase = new Application.UseCases.Register.Interactor(
                customerWriteOnlyRepository,
                accountWriteOnlyRepository,
                output,
                converter
            );

            var request = new Application.UseCases.Register.Request(
                personnummer,
                name,
                amount
            );

            await registerUseCase.Handle(request);

            Assert.Equal(request.PIN, output.Response.Customer.Personnummer);
            Assert.Equal(request.Name, output.Response.Customer.Name);
            Assert.True(output.Response.Customer.CustomerId != Guid.Empty);
            Assert.True(output.Response.Account.AccountId != Guid.Empty);
        }
    }
}
