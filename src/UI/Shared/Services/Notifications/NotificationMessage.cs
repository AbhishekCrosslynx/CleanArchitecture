using System;
using System.Collections.Generic;

namespace Shared.Services.Notifications;

public record NotificationMessage(string Id, string Title, string Except, string Category, DateTime PublishDate, Uri ImgUrl, IEnumerable<NotificationAuthor> Authors, Type ContentComponent);
