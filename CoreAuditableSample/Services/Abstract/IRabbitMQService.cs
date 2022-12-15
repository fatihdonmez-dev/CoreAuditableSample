using CoreAuditableSample.Models.RabbitModels;

namespace CoreAuditableSample.Services.Abstract
{
    public interface IRabbitMQService
    {
        public void SendToQueue(Rabbit.QueueList queue, string message);
    }
}
