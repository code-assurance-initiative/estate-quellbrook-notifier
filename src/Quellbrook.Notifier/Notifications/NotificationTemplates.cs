namespace Quellbrook.Notifier.Notifications;

/// <summary>The plain-text messages the consignee receives. Order references are the first eight characters of the id.</summary>
public static class NotificationTemplates
{
    public static string Reference(Guid orderId) => orderId.ToString("N")[..8].ToUpperInvariant();

    public static (string Subject, string Body) Email(NotificationKind kind, string name, Guid orderId)
    {
        var reference = Reference(orderId);
        return kind switch
        {
            NotificationKind.OrderConfirmed => (
                $"Your Quellbrook delivery {reference} is booked",
                $"Hello {name},\n\nA parcel delivery to you has been booked with Quellbrook Freight (reference {reference}). " +
                "We will tell you when it is on its way.\n\nQuellbrook Freight"),
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
        };
    }
}
