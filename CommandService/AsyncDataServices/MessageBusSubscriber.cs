using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandService.AsyncDataServices
{
  public class MessageBusSubscriber : BackgroundService
  {
    private readonly IConfiguration _configuration;
    private readonly IEventProcessor _eventProcessor;
    private IConnection _connection;
    private IChannel _channel;
    private string _queueName;

    public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
    {
      _configuration = configuration;
      _eventProcessor = eventProcessor;

      InitializeRabbitMQ();
    }

    private void InitializeRabbitMQ()
    {
      var factory = new ConnectionFactory()
      {
        HostName = _configuration["RabbitMQHost"] ?? "",
        Port = int.Parse(_configuration["RabbitMQPort"] ?? "")
      };

      Console.WriteLine(factory);

      try
      {
        _connection = factory.CreateConnectionAsync().Result;
        _channel = _connection.CreateChannelAsync().Result;

        _channel.ExchangeDeclareAsync(exchange: "trigger", type: ExchangeType.Fanout);


        _queueName = _channel.QueueDeclareAsync().Result.QueueName;
        _channel.QueueBindAsync(queue: _queueName, exchange: "trigger", routingKey: "");

        Console.WriteLine("--> Listening to the Message Bus");

        _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdown;


        // Console.WriteLine("--> Connected to the Message Bus");

      }
      catch (Exception ex)
      {
        Console.WriteLine($"--> Could not connect to the Message Bus: {ex.Message}");
        // throw;
      }

    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
      stoppingToken.ThrowIfCancellationRequested();

      var consumer = new AsyncEventingBasicConsumer((IChannel)_channel);

      consumer.ReceivedAsync += (ModuleHandle, ea) =>
      {
        Console.WriteLine("--> Event Received");

        var body = ea.Body;
        var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

        _eventProcessor.ProcessEvent(notificationMessage);

        return Task.CompletedTask;
      };

      _channel.BasicConsumeAsync(queue: _queueName, autoAck: true, consumer: consumer);

      return Task.CompletedTask;
    }

    private async Task RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
      Console.WriteLine("--> Connection Shutdown");
    }

    public override void Dispose()
    {
      if (_channel.IsOpen)
      {
        _channel.CloseAsync();
        _connection.CloseAsync();
      }

      base.Dispose();
    }
  }
}