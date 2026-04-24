using Application.Users.Register;
using Domain.Users;

namespace Application.Tests.L0.Users.Register;

public class UserRegisteredDomainEventHandlerTests
{
    [Fact]
    public async Task Handle_Should_CompleteSuccessfully_When_EventIsRaised()
    {
        var handler = new UserRegisteredDomainEventHandler();
        var domainEvent = new UserRegisteredDomainEvent(Guid.NewGuid());

        Func<Task> act = () => handler.Handle(domainEvent, CancellationToken.None);

        await act.ShouldNotThrowAsync();
    }

    [Fact]
    public async Task Handle_Should_CompleteSuccessfully_When_CancellationIsRequested()
    {
        var handler = new UserRegisteredDomainEventHandler();
        var domainEvent = new UserRegisteredDomainEvent(Guid.NewGuid());
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // The handler is a no-op (TODO), so it should complete even with a cancelled token
        Func<Task> act = () => handler.Handle(domainEvent, cts.Token);

        await act.ShouldNotThrowAsync();
    }
}
