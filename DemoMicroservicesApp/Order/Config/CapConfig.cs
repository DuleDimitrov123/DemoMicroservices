namespace Order.Config;

public class CapConfig
{
    public int FailedRetryIntervalSeconds { get; set; }

    public int FailedRetryCount { get; set; }

    public int SucceedMessageExpirationSeconds { get; set; }

    public int FailedMessageExpirationSeconds { get; set; }
}
