using Azure.Messaging.ServiceBus;
using Mango.service.EmailAPI.Services;
using Mango.Services.RewardAPI;
using Mango.Services.RewardAPI.Message;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.RewardAPI.Messaging
{
    public class AzureServiceBusConsumer
        : IAzureServiceBusConsumer
    {
        private readonly string servicebusconnectionstring;
        private readonly string orderedCreatedTopic;
        private readonly string orderCreatedSubscription;
        private readonly IConfiguration _configuration;
        private readonly ServiceBusProcessor _rewardprocessor;
        private readonly RewardService _rewardService;
        public AzureServiceBusConsumer(IConfiguration configuration,    RewardService rewardService)
        {_configuration = configuration;
            _rewardService = rewardService ;
            servicebusconnectionstring = _configuration.GetValue<string>("ServiceBusConnectionString");

            orderedCreatedTopic = _configuration.GetValue<string>("TopicsAndQueueNames:OrderCreatedTopic");
            orderCreatedSubscription = _configuration.GetValue<string>("TopicsAndQueueNames:OrderCreated_Rewards_Subscription");

            var client = new ServiceBusClient(servicebusconnectionstring);
            _rewardprocessor = client.CreateProcessor(orderedCreatedTopic,orderCreatedSubscription);

        }

        public async Task Start()
        {
            _rewardprocessor.ProcessMessageAsync += OnNewOrderRequestReceived;
            _rewardprocessor.ProcessErrorAsync += ErrorHandler;
            await _rewardprocessor.StartProcessingAsync();
        }
        public async Task Stop()
        {
            await _rewardprocessor.StopProcessingAsync();
            await _rewardprocessor.DisposeAsync();
        }
        private async Task OnNewOrderRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            RewardsMessage objMessage = JsonConvert.DeserializeObject<RewardsMessage>(body);
            try
            {
                //try to log email 
                await _rewardService.UpdateRewards(objMessage);

                await args.CompleteMessageAsync(args.Message);

            }catch (Exception ex)
            {

            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

      
    }
}
