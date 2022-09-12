using mark.davison.finance.models.dtos.Commands.CreateCategory;

namespace mark.davison.finance.web.features.test.Category.Create;

[TestClass]
public class CreateCategoryListCommandHandlerTests
{
    private readonly Mock<IClientHttpRepository> _repository;
    private readonly CreateCategoryListCommandHandler _handler;

    public CreateCategoryListCommandHandlerTests()
    {
        _repository = new(MockBehavior.Strict);
        _handler = new(_repository.Object);
    }


    [TestMethod]
    public async Task Handle_InvokesRepostory()
    {
        var accountListItems = new List<CategoryListItemDto> {
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() }
        };

        _repository
            .Setup(_ => _
                .Post<CreateCategoryResponse, CreateCategoryRequest>(
                    It.IsAny<CreateCategoryRequest>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((CreateCategoryRequest req, CancellationToken cancellationToken) => new CreateCategoryResponse()
            {
                Success = true
            })
            .Verifiable();

        var response = await _handler.Handle(new CreateCategoryListCommand(), CancellationToken.None);

        Assert.AreNotEqual(Guid.Empty, response.ItemId);

        _repository
            .Verify(_ => _
                .Post<CreateCategoryResponse, CreateCategoryRequest>(
                    It.IsAny<CreateCategoryRequest>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }
}
