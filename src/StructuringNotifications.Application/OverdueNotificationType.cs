namespace StructuringNotifications.Application;

internal class OverdueNotificationType
{
    private OverdueNotificationType()
    {
    }

    public int DelayDays { get; private set; }
    public bool SendToSignatureAddress { get; private set; }
    public bool SendToSupport { get; private set; }
    public bool SendToSupplier { get; private set; }

    public static readonly OverdueNotificationType[] AvailableTypes =
    {
        new()
        {
            DelayDays = -3,
            SendToSignatureAddress = false,
            SendToSupport = false,
            SendToSupplier = false,
        },
        new()
        {
            DelayDays = 0,
            SendToSignatureAddress = false,
            SendToSupport = true,
            SendToSupplier = false
        },
        new()
        {
            DelayDays = 3,
            SendToSignatureAddress = false,
            SendToSupport = false,
            SendToSupplier = false
        },
        new()
        {
            DelayDays = 7,
            SendToSignatureAddress = true,
            SendToSupport = true,
            SendToSupplier = false
        },
        new()
        {
            DelayDays = 15,
            SendToSignatureAddress = true,
            SendToSupport = false,
            SendToSupplier = true
        },
        new()
        {
            DelayDays = 30,
            SendToSignatureAddress = true,
            SendToSupport = true,
            SendToSupplier = true
        },
        new()
        {
            DelayDays = 40,
            SendToSignatureAddress = true,
            SendToSupport = false,
            SendToSupplier = true,
        }
    };
}

