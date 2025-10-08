
namespace LigaLibre.Application.Interfaces;

public interface ISqsService
{
    Task SendMessageAsync<T>(T message, string queueName, int delaySeconds = 0);

    Task<IEnumerable<QueueMessage>> ReceiveMessageAsync(string queueName, int maxMessages = 10);

    Task deleteMessageAsync(string queueName, string receiptHandle);

}

public class QueueMessage
{
    public string MessageId { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string ReceiptHandle { get; set; } = string.Empty;

}

public static class QueuNames
{
    public const string ClubEvent = "Club-events";
}