using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace HangfireTemplate.Services
{
    public class TestService
    {
        private readonly ILogger<TestService> _logger;

        public TestService(ILogger<TestService> logger)
        {
            _logger = logger;
        }

        public async Task HelloAsync()
        {
            await Task.Delay(1);
            _logger.LogInformation("Hello");
        }
    }
}