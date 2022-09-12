namespace mark.davison.finance.bff.commands.test.integration.Scenarios.CreateCategory;

[TestClass]
public class CreateCategoryTests : CommandIntegrationTestBase
{
    [TestMethod]
    public async Task SavingNewCategory_Passes()
    {
        var handler = GetRequiredService<ICommandHandler<CreateCategoryRequest, CreateCategoryResponse>>();
        var currentUserContext = GetRequiredService<ICurrentUserContext>();

        var request = new CreateCategoryRequest { Id = Guid.NewGuid(), Name = "Category B" };
        var response = await handler.Handle(request, currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);
    }

    [TestMethod]
    public async Task SavingNewCategoryWhereNameAlreadyExists_Fails()
    {
        var handler = GetRequiredService<ICommandHandler<CreateCategoryRequest, CreateCategoryResponse>>();
        var currentUserContext = GetRequiredService<ICurrentUserContext>();

        var request = new CreateCategoryRequest { Id = Guid.NewGuid(), Name = "Category B" };
        var response = await handler.Handle(request, currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);

        response = await handler.Handle(request, currentUserContext, CancellationToken.None);

        Assert.IsFalse(response.Success);
    }
}
