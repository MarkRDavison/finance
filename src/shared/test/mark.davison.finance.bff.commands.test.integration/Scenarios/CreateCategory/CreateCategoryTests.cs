namespace mark.davison.finance.bff.commands.test.integration.Scenarios.CreateCategory;

[TestClass]
public class CreateCategoryTests : CQRSIntegrationTestBase
{
    [TestMethod]
    public async Task SavingNewCategory_Passes()
    {
        var handler = GetRequiredService<ICommandHandler<CreateCategoryCommandRequest, CreateCategoryCommandResponse>>();
        var repo = GetRequiredService<IRepository>();
        var users = await repo.GetEntitiesAsync<User>();
        var currentUserContext = GetRequiredService<ICurrentUserContext>();

        var request = new CreateCategoryCommandRequest { Id = Guid.NewGuid(), Name = "Category B" };
        var response = await handler.Handle(request, currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);
    }

    [TestMethod]
    public async Task SavingNewCategoryWhereNameAlreadyExists_Fails()
    {
        var handler = GetRequiredService<ICommandHandler<CreateCategoryCommandRequest, CreateCategoryCommandResponse>>();
        var currentUserContext = GetRequiredService<ICurrentUserContext>();

        var request = new CreateCategoryCommandRequest { Id = Guid.NewGuid(), Name = "Category B" };
        var response = await handler.Handle(request, currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);

        response = await handler.Handle(request, currentUserContext, CancellationToken.None);

        Assert.IsFalse(response.Success);
    }
}
