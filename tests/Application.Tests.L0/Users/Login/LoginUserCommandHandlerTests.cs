using Application.Abstractions.Authentication;
using Application.Tests.L0.Infrastructure;
using Application.Users.Login;
using Domain.Users;

namespace Application.Tests.L0.Users.Login;

public class LoginUserCommandHandlerTests
{
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly ITokenProvider _tokenProvider = Substitute.For<ITokenProvider>();

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_UserDoesNotExist()
    {
        using var context = TestDbContext.Create();

        var command = new LoginUserCommand("nonexistent@test.com", "password123");
        var handler = new LoginUserCommandHandler(context, _passwordHasher, _tokenProvider);

        Result<string> result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("Users.NotFoundByEmail");
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_PasswordIsIncorrect()
    {
        using var context = TestDbContext.Create();
        const string email = "user@test.com";
        context.Users.Add(new User { Id = Guid.NewGuid(), Email = email, FirstName = "Test", LastName = "User", PasswordHash = "stored_hash" });
        await context.SaveChangesAsync();

        _passwordHasher.Verify("wrong_password", "stored_hash").Returns(false);

        var command = new LoginUserCommand(email, "wrong_password");
        var handler = new LoginUserCommandHandler(context, _passwordHasher, _tokenProvider);

        Result<string> result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("Users.NotFoundByEmail");
    }

    [Fact]
    public async Task Handle_Should_ReturnToken_When_CredentialsAreValid()
    {
        using var context = TestDbContext.Create();
        const string email = "user@test.com";
        const string password = "correctP@ss";
        const string expectedToken = "jwt.token.value";

        context.Users.Add(new User { Id = Guid.NewGuid(), Email = email, FirstName = "Test", LastName = "User", PasswordHash = "stored_hash" });
        await context.SaveChangesAsync();

        _passwordHasher.Verify(password, "stored_hash").Returns(true);
        _tokenProvider.Create(Arg.Any<User>()).Returns(expectedToken);

        var command = new LoginUserCommand(email, password);
        var handler = new LoginUserCommandHandler(context, _passwordHasher, _tokenProvider);

        Result<string> result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(expectedToken);
    }

    [Fact]
    public async Task Handle_Should_NotGenerateToken_When_PasswordIsIncorrect()
    {
        using var context = TestDbContext.Create();
        const string email = "user@test.com";
        context.Users.Add(new User { Id = Guid.NewGuid(), Email = email, FirstName = "Test", LastName = "User", PasswordHash = "stored_hash" });
        await context.SaveChangesAsync();

        _passwordHasher.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

        var command = new LoginUserCommand(email, "bad_password");
        var handler = new LoginUserCommandHandler(context, _passwordHasher, _tokenProvider);

        await handler.Handle(command, CancellationToken.None);

        _tokenProvider.DidNotReceive().Create(Arg.Any<User>());
    }
}
