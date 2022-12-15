using CoreAuditableSample.Models.RabbitModels;
using CoreAuditableSample.Services.Abstract;
using CoreAuditableSample.Utils;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace CoreAuditableSample.Services.Concrete
{
    public class RabbitMQService : IRabbitMQService
    {
        private readonly RabbitSettings _rabbitSettings = null;
        private readonly IHostingEnvironment _hostingEnvironment;
        public RabbitMQService(IOptions<RabbitSettings> rabbitSettingsAccessor, IHostingEnvironment hostingEnvironment)
        {
            _rabbitSettings = rabbitSettingsAccessor.Value;
            _hostingEnvironment = hostingEnvironment;
        }
        public void SendToQueue(Rabbit.QueueList queue, string message)
        {
            try
            {
                if (!_hostingEnvironment.IsDevelopment() && IsAllowed(queue, message))
                {
                    var factory = new ConnectionFactory()
                    {
                        HostName = _rabbitSettings.HostName,
                        UserName = _rabbitSettings.UserName,
                        Password = _rabbitSettings.Password
                    };

                    using (var connection = factory.CreateConnection())
                    {
                        using (var channel = connection.CreateModel())
                        {
                            channel.QueueDeclare(queue.ToString(), true, false, false, null);
                            var properties = channel.CreateBasicProperties();
                            properties.SetPersistent(true);
                            channel.BasicPublish("", queue.ToString(), properties, Encoding.UTF8.GetBytes(message));
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        bool IsAllowed(Rabbit.QueueList queue, string message)
        {
            if (queue == Rabbit.QueueList.InProgressOperations)
            {
                try
                {
                    var item = JsonConvert.DeserializeObject<Rabbit.InProgressOperationObject>(message);
                    if (item.Type == "DeleteCacheList")
                    {
                        if (!item.ExtraObjectJson.IsNullOrEmpty())
                        {
                            var extraItem = JsonConvert.DeserializeObject<Rabbit.CacheListExtra>(item.ExtraObjectJson);
                            return !extraItem.CDNUrlList.IsNullOrEmpty() || !extraItem.DataCacheList.IsNullOrEmpty() || !extraItem.RequestUrlList.IsNullOrEmpty() || !extraItem.UrlList.IsNullOrEmpty();
                        }
                        return false;
                    }
                }
                catch { }
            }
            return true;
        }
    }
}
