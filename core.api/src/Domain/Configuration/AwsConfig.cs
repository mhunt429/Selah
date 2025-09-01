namespace Domain.Configuration;

public class AwsConfig
{
    public required string AccessKey { get; set; }

    public required string SecretAccessKey { get; set; }

    public required string Region { get; set; }
}