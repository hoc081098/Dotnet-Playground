using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using WebAppShared.RabbitMq.Shared;

namespace WebAppPlayground.RabbitMq.TopicWithDlx;

public class TopicWithDlxRabbitMqConsumerBackgroundService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory();
        await using var connection = await factory.CreateConnectionAsync(stoppingToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        // 0. Setup dead-letter exchange and queue
        await channel.SetupForDlxAsync();

        // 1. Declare a topic exchange.
        // This ensures that the exchange exists (creates it if not already existing).
        await channel.ExchangeDeclareAsync(
            exchange: TopicWithDlxConfig.ExchangeName,
            type: ExchangeType.Topic,
            durable: true, // durable exchange
            autoDelete: false, // don’t delete when the last consumer disconnects
            cancellationToken: stoppingToken);

        Console.WriteLine($"[<<<] Exchange declared: {TopicWithDlxConfig.ExchangeName}");

        // 2. Declare the queue and bind it.
        // https://www.rabbitmq.com/docs/dlx#overview
        var arguments = new Dictionary<string, object?>
        {
            { "x-dead-letter-exchange", TopicWithDlxConfig.MyDeadletterExchange },
            { "x-dead-letter-routing-key", "" },
        };

        await channel.QueueDeclareAsync(
            queue: TopicWithDlxConfig.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: arguments,
            cancellationToken: stoppingToken);

        await channel.QueueBindAsync(
            queue: TopicWithDlxConfig.QueueName,
            exchange: TopicWithDlxConfig.ExchangeName,
            routingKey: TopicWithDlxConfig.Binding,
            cancellationToken: stoppingToken);

        Console.WriteLine($"[<<<] Queue declared and bound: {TopicWithDlxConfig.QueueName}");

        // 3. Set prefetch count to 1 to process one message at a time
        await channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false,
            cancellationToken: stoppingToken);

        // 4. Declare a consumer
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (sender, eventArgs) =>
            HandleMessageAsync((AsyncEventingBasicConsumer)sender, eventArgs, stoppingToken);

        // 5. Start consuming messages from the queue
        // this consumer tag identifies the subscription when it has to be cancelled
        var consumerTag = await channel.BasicConsumeAsync(
            queue: TopicWithDlxConfig.QueueName,
            // false <=> manual acknowledgements.
            // We only acknowledge messages when processed successfully.
            // If processing fails, we could use `BasicNack` to requeue or route to a dead-letter queue
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        Console.WriteLine($"[<<<] Consumer tagged: {consumerTag}");

        // 6. Wait until stopping is requested
        // If we do not wait here, the channel and connection will be disposed immediately -> cannot acknowledge messages
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private static async Task HandleMessageAsync(
        AsyncEventingBasicConsumer sender,
        BasicDeliverEventArgs eventArgs,
        CancellationToken cancellationToken)
    {
        try
        {
            // 4.1. Copy the body to a new array to make it safe to use outside this event,
            // and then parse it to an OrderPlaced instance
            var body = eventArgs.Body.ToArray();

            if (eventArgs.RoutingKey == "orders.placed")
            {
                var orderPlaced = (OrderPlaced)JsonSerializer.Deserialize(
                    body,
                    OrderPlacedReferences.GetOrderPlacedType())!;

                Console.WriteLine($"[<<<] Received OrderPlaced: {orderPlaced}");
                await Task.Delay(5_000, cancellationToken); // Simulate processing time
            }
            else
            {
                Console.WriteLine($"[<<<] Handled message with routing key: {eventArgs.RoutingKey}");
                // Handle other routing keys as needed
            }

            // 4.2. Acknowledge the message as processed
            await sender.Channel.BasicAckAsync(
                eventArgs.DeliveryTag,
                multiple: false,
                cancellationToken: cancellationToken);

            Console.WriteLine($"[<<<] Acknowledged deliveryTag={eventArgs.DeliveryTag}");
        }
        catch (OperationCanceledException)
        {
            // Ignore cancellation exceptions
            throw;
        }
        catch (Exception ex) when (ex is JsonException or NotSupportedException)
        {
            Console.WriteLine($"[<<<] Rejecting RabbitMQ message due to invalid format: {ex.Message}");

            // 4.3. Reject the message (nack) and send it to the DLX - it's an invalid/poison message
            await sender.Channel.BasicNackAsync(
                eventArgs.DeliveryTag,
                multiple: false,
                // INVALID/POISON MESSAGE - do not requeue
                requeue: false,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            // 4.4. On any other error, nack the message with requeue = true
            Console.WriteLine($"[<<<] Requeue RabbitMQ message due to processing error: {ex.Message}");

            await sender.Channel.BasicNackAsync(
                eventArgs.DeliveryTag,
                multiple: false,
                // Requeue the message for another processing attempt
                requeue: true,
                cancellationToken: cancellationToken);
        }
    }
}