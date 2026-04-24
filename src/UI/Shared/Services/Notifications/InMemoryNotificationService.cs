using Blazored.LocalStorage;
using Shared.NotificationContent;

namespace Shared.Services.Notifications;

public class InMemoryNotificationService(ILocalStorageService localStorageService) : INotificationService
{
    private const string LocalStorageKey = "__notficationTimestamp";

    private readonly List<NotificationMessage> _messages = [];

    private async Task<DateTime> GetLastReadTimestamp()
    {
        if (!await localStorageService.ContainKeyAsync(LocalStorageKey))
        {
            return DateTime.MinValue;
        }

        DateTime timestamp = await localStorageService.GetItemAsync<DateTime>(LocalStorageKey);
        return timestamp;
    }

    public async Task<bool> AreNewNotificationsAvailable()
    {
        DateTime timestamp = await GetLastReadTimestamp();
        bool entriesFound = _messages.Any(x => x.PublishDate > timestamp);

        return entriesFound;
    }

    public async Task MarkNotificationsAsRead()
    {
        await localStorageService.SetItemAsync(LocalStorageKey, DateTime.UtcNow.Date);
    }

    public async Task MarkNotificationsAsRead(string id)
    {
        NotificationMessage message = await GetMessageById(id);
        if (message == null)
        { return; }

        DateTime timestamp = await localStorageService.GetItemAsync<DateTime>(LocalStorageKey);
        if (message.PublishDate > timestamp)
        {
            await localStorageService.SetItemAsync(LocalStorageKey, message.PublishDate);
        }

    }

    public Task<NotificationMessage?> GetMessageById(string id) =>
        Task.FromResult(_messages.FirstOrDefault(x => x.Id == id));

    public async Task<IDictionary<NotificationMessage, bool>> GetNotifications()
    {
        DateTime lastReadTimestamp = await GetLastReadTimestamp();
        var items = _messages.ToDictionary(x => x, x => lastReadTimestamp > x.PublishDate);
        return items;
    }

    public Task AddNotification(NotificationMessage message)
    {
        _messages.Add(message);
        return Task.CompletedTask;
    }

    public void Preload()
    {
        _messages.Add(new NotificationMessage(
            nameof(Announcement_v9_GA),
            "MudBlazor v9.0.0 Released",
            "Major Version",
            "Announcement",
            //new DateTime(2026, 03, 01, 0, 0, 0, DateTimeKind.Local),
            DateTime.UtcNow,
            new Uri("https://github.com/MudBlazor/MudBlazor/blob/f979c2c84e3ddd5f01a20ebc1102838d32a4b01b/content/Nuget.png"),
            new List<NotificationAuthor>
            {
                    new NotificationAuthor("The MudBlazor Team", new Uri("https://mudblazor.com/_content/MudBlazor.Docs/images/logo.png"))
            },
            typeof(Announcement_v9_GA)
        ));

        _messages.Add(new NotificationMessage(
            nameof(Announcement_v8_GA),
            "MudBlazor v8.0.0 Released",
            "Major Version",
            "Announcement",
            new DateTime(2025, 01, 19, 0, 0, 0, DateTimeKind.Local),
            new Uri("https://github.com/MudBlazor/MudBlazor/blob/f979c2c84e3ddd5f01a20ebc1102838d32a4b01b/content/Nuget.png"),
            new List<NotificationAuthor>
            {
                    new NotificationAuthor("The MudBlazor Team", new Uri("https://mudblazor.com/_content/MudBlazor.Docs/images/logo.png"))
            },
            typeof(Announcement_v8_GA)
        ));

        _messages.Add(new NotificationMessage(
            nameof(Announcement_v7_GA),
            "MudBlazor v7.0.0 Released",
            "Major Version",
            "Announcement",
            new DateTime(2024, 06, 29, 0, 0, 0, DateTimeKind.Local),
            new Uri("https://github.com/MudBlazor/MudBlazor/blob/f979c2c84e3ddd5f01a20ebc1102838d32a4b01b/content/Nuget.png"),
            new List<NotificationAuthor>
            {
                    new NotificationAuthor("The MudBlazor Team", new Uri("https://mudblazor.com/_content/MudBlazor.Docs/images/logo.png"))
            },
            typeof(Announcement_v7_GA)
        ));

        _messages.Add(new NotificationMessage(
            "mudblazor-here-to-stay",
            "MudBlazor is here to stay",
            "We are paving the way for the future of Blazor",
            "Announcement",
            new DateTime(2022, 01, 13, 0, 0, 0, DateTimeKind.Local),
            new Uri("https://avatars.githubusercontent.com/u/10367109?v=4"),
            new List<NotificationAuthor>
            {
                    new NotificationAuthor("Jonny Larsson", new Uri("https://avatars.githubusercontent.com/u/10367109?v=4"))
            },
            typeof(Announcement_MudBlazorIsHereToStay)
        ));
    }
}
