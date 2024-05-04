using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace SalesOrderConfirmation.EventsProcessor
{
    public class Worker
    {
        private readonly ILogger _logger;

        public Worker(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Worker>();
        }

        [Function("SalesOrderConfirmation.EventsProcessor.Worker")]
        public void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            
            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }
    }
}
