namespace Quellbrook.Notifier.Messaging;

/// <summary>
/// The notifier's copy of <c>orders.order-placed.v1</c>: only the consignee's name and contact details
/// (contracts/consumed/orders/order-placed.v1.schema.json). Unknown fields are ignored.
/// </summary>
public sealed record OrderPlacedMessage(Guid OrderId, OrderPlacedMessage.ConsigneePart Consignee)
{
    public const string EventType = "orders.order-placed.v1";

    public sealed record ConsigneePart(string Name, ContactPart Contact);

    public sealed record ContactPart(string? Email, string? Phone);
}
