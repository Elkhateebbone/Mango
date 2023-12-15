using Azure.Messaging.ServiceBus;
using Mango.service.EmailAPI.Services;
using Mango.Services.RewardAPI.Message;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;

namespace Mango.service.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer
        : IAzureServiceBusConsumer
    {
        private readonly string servicebusconnectionstring;
        private readonly string emailcartqueue;
        private readonly IConfiguration _configuration;
        private readonly ServiceBusProcessor _emailcartprocessor;

        private readonly ServiceBusProcessor _emailprocessor;
        private readonly ServiceBusProcessor _emailOrderPlacedprocessor;

        private readonly EmailService _emailService;
        private readonly string order_Created_Topic;
        private readonly string orderCreated_Email_Subscription;
        private ServiceBusProcessor _registerUserProcessor; 
        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {_configuration = configuration;
            _emailService = emailService;
            servicebusconnectionstring = _configuration.GetValue<string>("ServiceBusConnectionString");

            emailcartqueue = _configuration.GetValue<string>("TopicsAndQueueNames:EmailShoppingCartQueue");
            order_Created_Topic = _configuration.GetValue<string>("TopicsAndQueueNames:OrderCreatedTopic");
            orderCreated_Email_Subscription = _configuration.GetValue<string>("TopicsAndQueueNames:OrderCreated_Rewards_Subscription");

            var client = new ServiceBusClient(servicebusconnectionstring);
            _emailcartprocessor = client.CreateProcessor(emailcartqueue);
            _emailOrderPlacedprocessor = client.CreateProcessor(order_Created_Topic,orderCreated_Email_Subscription);

        }

        public async Task Start()
        {
            _emailcartprocessor.ProcessMessageAsync += OnEmailCartRequestReceived;
            _emailcartprocessor.ProcessErrorAsync += ErrorHandler;
            await _emailcartprocessor.StartProcessingAsync();


            _registerUserProcessor.ProcessMessageAsync += OnUserRegisterRequestReceived;
            _registerUserProcessor.ProcessErrorAsync += ErrorHandler;
            await _registerUserProcessor.StartProcessingAsync();



            _emailOrderPlacedprocessor.ProcessMessageAsync += OnUserRegisterRequestReceived;
            _emailOrderPlacedprocessor.ProcessErrorAsync += ErrorHandler;
            await _emailOrderPlacedprocessor.StartProcessingAsync();
        }
        public async Task Stop()
        {
            await _emailcartprocessor.StopProcessingAsync();
            await _emailcartprocessor.DisposeAsync();

            await _registerUserProcessor.StopProcessingAsync();
            await _registerUserProcessor.DisposeAsync();

            await _emailOrderPlacedprocessor.StopProcessingAsync();
            await _emailOrderPlacedprocessor.DisposeAsync();
        }
        private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            RewardsMessage objMessage = JsonConvert.DeserializeObject<RewardsMessage>(body);
            try
            {
                //try to log email 
                await _emailService.LogOrderPlaced(objMessage);

                await args.CompleteMessageAsync(args.Message);

            }catch (Exception ex)
            {

            }
        }

        private async Task OnUserRegisterRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            CartDTO objMessage = JsonConvert.DeserializeObject<CartDTO>(body);
            try
            {
                //try to log email 
                await _emailService.EmailCartAndLog(objMessage);

                await args.CompleteMessageAsync(args.Message);

            }
            catch (Exception ex)
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
