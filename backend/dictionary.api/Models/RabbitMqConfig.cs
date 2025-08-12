namespace Dictionary.Api.Models;

public class RabbitMqConfig
{
  public string Host { get; set; } = "localhost";
  public int Port { get; set; } = 5672;
  public string UserName { get; set; } = "user";
  public string Password { get; set; } = "password";
}
