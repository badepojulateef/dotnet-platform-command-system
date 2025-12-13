using System;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using PlatformService.Dtos;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PlatformService.AsyncDataServices
{
  public class MessageBusClient : IMessageBusClient
  {
    private readonly IConfiguration _configuration;
    private readonly Task<IConnection> _connection;
    private readonly Task<IChannel> _channel;

    public MessageBusClient(IConfiguration configuration)
    {
      _configuration = configuration;
      var factory = new ConnectionFactory()
      {
        HostName = _configuration["RabbitMQHost"] ?? "",
        Port = int.Parse(_configuration["RabbitMQPort"] ?? "")
      };

      try
      {
        _connection = factory.CreateConnectionAsync();
        _channel = _connection.Result.CreateChannelAsync();

        _channel.Result.ExchangeDeclareAsync(exchange: "trigger", type: ExchangeType.Fanout);

        _connection.Result.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdown;

        Console.WriteLine("--> Connected to the Message Bus");

      }
      catch (Exception ex)
      {
        Console.WriteLine($"--> Could not connect to the Message Bus: {ex.Message}");
        // throw;
      }
    }
    public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
    {
      var message = JsonSerializer.Serialize(platformPublishedDto);

      if (_connection.Result.IsOpen)
      {
        Console.WriteLine("--> RabbitMQ Connection Open, sending message...");
        SendMessage(message);
      }
    }

    private void SendMessage(string message)
    {
      var props = new BasicProperties();
      var body = Encoding.UTF8.GetBytes(message);
      _channel.Result.BasicPublishAsync(exchange: "trigger", routingKey: "", mandatory: true, basicProperties: props, body: body);

      Console.WriteLine($"--> We have sent {message}");
    }

    private void Dispose()
    {
      Console.WriteLine("MessageBus Disposed");

      if (_channel.Result.IsOpen)
      {
        _channel.Result.CloseAsync();
        _connection.Result.CloseAsync();
      }
    }
    private async Task RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
      Console.WriteLine("--> RabbitMQ Connection Shutdown");
    }
  }
}