using Homework.Gateway.Attributes;
using Homework.Gateway.Configurations;
using Microsoft.Extensions.Options;
using System.ServiceModel;

namespace OldCustomerService
{
    /// <summary>
    /// Factory for old customer service
    /// - with this factory is code more testable
    /// </summary>
    [Scoped] 
    public class OldCustomerServiceFactory : IOldCustomerServiceFactory
    {
        private readonly OldCustomerServiceConfig _oldServiceConfig;

        public OldCustomerServiceFactory(IOptions<OldCustomerServiceConfig> oldServiceConfig)
        {
            _oldServiceConfig = oldServiceConfig.Value;
        }

        public ICustomerService CreateService()
        {
            var client = new CustomerServiceClient();
            return client.ChannelFactory.CreateChannel(new EndpointAddress(_oldServiceConfig.Url));
        }
    }
}
