namespace Domain.Configuration;

public class RabbitMqConfig
{
    public required string UserName { get; set; }
    
    public required string Password { get; set; }
    
    public required string Host { get; set; }
    
    public bool UseSsl { get; set; }

    //Optional SSL settings
    
    /*
     * openssl genrsa -out ca.key 2048
       openssl req -x509 -new -nodes -key ca.key -days 365 -out ca.crt
       openssl genrsa -out server.key 2048
       openssl req -new -key server.key -out server.csr
       openssl x509 -req -in server.csr -CA ca.crt -CAkey ca.key -CAcreateserial -out server.crt -days 365
     */
    public string SslServerName { get; set; } = "";
    public string ClientCertificatePath { get; set; } = "";
    public string ClientCertificatePassword { get; set; } = "";
}