using Amazon.SQS;
using Amazon.SQS.Model;
using Azure;
using LigaLibre.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LigaLibre.Infrastructure.Services;

public class SqsService(IAmazonSQS sqsClient, ILogger<SqsService> logger) : ISqsService
{
    public async Task deleteMessageAsync(string queueName, string receiptHandle)
    {
        var queueUrl = await sqsClient.GetQueueUrlAsync(queueName);
        await sqsClient.DeleteMessageAsync(queueUrl.QueueUrl, receiptHandle);
        logger.LogInformation($"Mensaje eliminado de la queue {queueName}.");
    }

    public async Task<IEnumerable<QueueMessage>> ReceiveMessageAsync(string queueName, int maxMessages = 10)
    {
        try
        {
            var queueUrl = await sqsClient.GetQueueUrlAsync(queueName);
            var request = new ReceiveMessageRequest
            {
                QueueUrl = queueUrl.QueueUrl,
                MaxNumberOfMessages = maxMessages,
                WaitTimeSeconds = 2,
            };

            var response = await sqsClient.ReceiveMessageAsync(request);
            if(response.Messages == null)
            {
                return new List<QueueMessage>();
            }
            return response.Messages.Select(m => new QueueMessage
            {
                MessageId = m.MessageId,
                Body = m.Body,
                ReceiptHandle = m.ReceiptHandle
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error al recibir mensajes de la queue {queueName}.");
            throw;
        }
      
    }

    public async Task SendMessageAsync<T>(T message, string queueName, int delaySeconds = 0)
    {
        var queueUrl = await sqsClient.GetQueueUrlAsync(queueName);
        var messageBody = JsonSerializer.Serialize(message);

        var request = new SendMessageRequest
        {
            QueueUrl = queueUrl.QueueUrl,
            MessageBody = messageBody,
            DelaySeconds = delaySeconds
        };

        var response = await sqsClient.SendMessageAsync(request);

        logger.LogInformation($"Mensaje enviado a la queue {queueName}. MessageId: {response.MessageId}");

    }

}

