using Microsoft.EntityFrameworkCore;
using Quellbrook.Notifier.Channels;
using Quellbrook.Notifier.Persistence;

namespace Quellbrook.Notifier.Notifications;

/// <summary>
/// Sends one kind of notification for an order through every channel the recipient can be reached on, at most once
/// per order, kind and channel: the notification log is checked before sending and written after.
/// </summary>
public sealed class NotificationService(NotifierDbContext db, IEmailSender email, TimeProvider time)
{
    public async Task NotifyAsync(Recipient recipient, NotificationKind kind, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(recipient);
        if (recipient.Email is { } address && !await AlreadySentAsync(recipient.OrderId, kind, "email", cancellationToken).ConfigureAwait(false))
        {
            var (subject, body) = NotificationTemplates.Email(kind, recipient.Name, recipient.OrderId);
            await email.SendAsync(new EmailMessage(recipient.OrderId, address, recipient.Name, subject, body), cancellationToken).ConfigureAwait(false);
            Record(recipient.OrderId, kind, "email", ContactMask.Email(address));
        }
    }

    private Task<bool> AlreadySentAsync(Guid orderId, NotificationKind kind, string channel, CancellationToken cancellationToken)
    {
        var kindName = kind.ToString();
        return db.NotificationLog.AnyAsync(entry => entry.OrderId == orderId && entry.Kind == kindName && entry.Channel == channel, cancellationToken);
    }

    private void Record(Guid orderId, NotificationKind kind, string channel, string maskedRecipient) =>
        db.NotificationLog.Add(new NotificationLogEntry
        {
            Id = Guid.CreateVersion7(time.GetUtcNow()),
            OrderId = orderId,
            Kind = kind.ToString(),
            Channel = channel,
            MaskedRecipient = maskedRecipient,
            SentAt = time.GetUtcNow(),
        });
}
