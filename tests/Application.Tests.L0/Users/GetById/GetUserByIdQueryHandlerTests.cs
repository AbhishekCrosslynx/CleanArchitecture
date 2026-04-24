using Application.Abstractions.Authentication;
using Application.Tests.L0.Infrastructure;
using Application.Users.GetById;
using Domain.Users;

namespace Application.Tests.L0.Users.GetById;

public class GetUserByIdQueryHandlerTests
{
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();

    [Fact]
    public async Task Handle_Should_ReturnUnauthorized_When_UserIdDoesNotMatchContext()
    {
        using var context = TestDbContext.Create();
        _userContext.UserId.Returns(Guid.NewGuid());

        var query = new GetUserByIdQuery(Guid.NewGuid());
        var handler = new GetUserByIdQueryHandler(context, _userContext);

        Result<UserResponse> result = await handler.Handle(query, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("Users.Unauthorized");
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_UserDoesNotExist()
    {
        using var context = TestDbContext.Create();
        var userId = Guid.NewGuid();
        _userContext.UserId.Returns(userId);

        var query = new GetUserByIdQuery(userId);
        var handler = new GetUserByIdQueryHandler(context, _userContext);

        Result<UserResponse> result = await handler.Handle(query, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("Users.NotFound");
    }

    [Fact]
    public async Task Handle_Should_ReturnUserResponse_When_UserExists()
    {
        using var context = TestDbContext.Create();
        var userId = Guid.NewGuid();
        _userContext.UserId.Returns(userId);

        context.Users.Add(new User { Id = userId, Email = "alice@example.com", FirstName = "Alice", LastName = "Smith", PasswordHash = "hash" });
        await context.SaveChangesAsync();

        var query = new GetUserByIdQuery(userId);
        var handler = new GetUserByIdQueryHandler(context, _userContext);

        Result<UserResponse> result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldBe(userId);
        result.Value.Email.ShouldBe("alice@example.com");
        result.Value.FirstName.ShouldBe("Alice");
        result.Value.LastName.ShouldBe("Smith");
    }
}
