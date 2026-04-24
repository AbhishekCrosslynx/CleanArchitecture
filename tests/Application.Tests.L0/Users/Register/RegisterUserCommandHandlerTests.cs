using Application.Abstractions.Authentication;
using Application.Tests.L0.Infrastructure;
using Application.Users.Register;
using Domain.Users;

namespace Application.Tests.L0.Users.Register;

public class RegisterUserCommandHandlerTests
{
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();

    [Fact]
    public async Task Handle_Should_ReturnConflict_When_EmailAlreadyExists()
    {
        using var context = TestDbContext.Create();
        const string email = "existing@test.com";
        context.Users.Add(new User { Id = Guid.NewGuid(), Email = email, FirstName = "Existing", LastName = "User", PasswordHash = "hash" });
        await context.SaveChangesAsync();

        var command = new RegisterUserCommand(email, "John", "Doe", "password123");
        var handler = new RegisterUserCommandHandler(context, _passwordHasher);

        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("Users.EmailNotUnique");
    }

    [Fact]
    public async Task Handle_Should_ReturnUserId_When_CommandIsValid()
    {
        using var context = TestDbContext.Create();
        _passwordHasher.Hash(Arg.Any<string>()).Returns("hashed_password");

        var command = new RegisterUserCommand("newuser@test.com", "Jane", "Smith", "secureP@ss1");
        var handler = new RegisterUserCommandHandler(context, _passwordHasher);

        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task Handle_Should_PersistUserWithHashedPassword_When_CommandIsValid()
    {
        using var context = TestDbContext.Create();
        const string hashedPassword = "bcrypt_hashed_value";
        _passwordHasher.Hash("secureP@ss1").Returns(hashedPassword);

        var command = new RegisterUserCommand("user@test.com", "Alice", "Wonder", "secureP@ss1");
        var handler = new RegisterUserCommandHandler(context, _passwordHasher);

        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        User? user = await context.Users.FindAsync(result.Value);
        user.ShouldNotBeNull();
        user!.Email.ShouldBe("user@test.com");
        user.FirstName.ShouldBe("Alice");
        user.LastName.ShouldBe("Wonder");
        user.PasswordHash.ShouldBe(hashedPassword);
    }

    [Fact]
    public async Task Handle_Should_RaiseUserRegisteredDomainEvent_When_CommandIsValid()
    {
        using var context = TestDbContext.Create();
        _passwordHasher.Hash(Arg.Any<string>()).Returns("hashed");

        var command = new RegisterUserCommand("new@test.com", "Bob", "Builder", "password123");
        var handler = new RegisterUserCommandHandler(context, _passwordHasher);

        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        User? user = await context.Users.FindAsync(result.Value);
        user.ShouldNotBeNull();
        user!.DomainEvents.ShouldContain(e => e is UserRegisteredDomainEvent);
    }

    [Fact]
    public async Task Handle_Should_HashPasswordBeforePersisting_When_CommandIsValid()
    {
        using var context = TestDbContext.Create();
        const string plainPassword = "MyPassword1!";
        _passwordHasher.Hash(plainPassword).Returns("secure_hash");

        var command = new RegisterUserCommand("test@test.com", "Test", "User", plainPassword);
        var handler = new RegisterUserCommandHandler(context, _passwordHasher);

        await handler.Handle(command, CancellationToken.None);

        _passwordHasher.Received(1).Hash(plainPassword);
    }
}
