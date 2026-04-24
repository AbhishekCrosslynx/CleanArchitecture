using Application.Abstractions.Authentication;
using Application.Tests.L0.Infrastructure;
using Application.Users.GetByEmail;
using Domain.Users;

namespace Application.Tests.L0.Users.GetByEmail;

public class GetUserByEmailQueryHandlerTests
{
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_UserDoesNotExist()
    {
        using var context = TestDbContext.Create();
        _userContext.UserId.Returns(Guid.NewGuid());

        var query = new GetUserByEmailQuery("nonexistent@test.com");
        var handler = new GetUserByEmailQueryHandler(context, _userContext);

        Result<UserResponse> result = await handler.Handle(query, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("Users.NotFoundByEmail");
    }

    [Fact]
    public async Task Handle_Should_ReturnUnauthorized_When_EmailBelongsToAnotherUser()
    {
        using var context = TestDbContext.Create();
        var emailOwnerUserId = Guid.NewGuid();
        var requestingUserId = Guid.NewGuid();
        _userContext.UserId.Returns(requestingUserId);

        context.Users.Add(new User { Id = emailOwnerUserId, Email = "owner@test.com", FirstName = "Owner", LastName = "User", PasswordHash = "hash" });
        await context.SaveChangesAsync();

        var query = new GetUserByEmailQuery("owner@test.com");
        var handler = new GetUserByEmailQueryHandler(context, _userContext);

        Result<UserResponse> result = await handler.Handle(query, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("Users.Unauthorized");
    }

    [Fact]
    public async Task Handle_Should_ReturnUserResponse_When_QueryIsValid()
    {
        using var context = TestDbContext.Create();
        var userId = Guid.NewGuid();
        _userContext.UserId.Returns(userId);

        context.Users.Add(new User { Id = userId, Email = "me@test.com", FirstName = "My", LastName = "Name", PasswordHash = "hash" });
        await context.SaveChangesAsync();

        var query = new GetUserByEmailQuery("me@test.com");
        var handler = new GetUserByEmailQueryHandler(context, _userContext);

        Result<UserResponse> result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Email.ShouldBe("me@test.com");
        result.Value.FirstName.ShouldBe("My");
        result.Value.LastName.ShouldBe("Name");
    }
}
