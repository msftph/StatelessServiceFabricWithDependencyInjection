using System.Fabric;
using Microsoft.Extensions.Hosting;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace StatelessServiceWithDI
{
    internal class CustomCommunicationListener : ICommunicationListener
    {
        private readonly ServiceContext serviceContext;
        private readonly HostApplicationBuilder builder;

        private IHost host;

        public CustomCommunicationListener(
            ServiceContext serviceContext, 
            Func<HostApplicationBuilder> createBuilderFunc)
        {
            this.builder = createBuilderFunc();            
            this.serviceContext = serviceContext;
        }

        public void Abort()
        {
            if (this.host is null)
                return;
            this.host.Dispose();
            this.host = null;
        }

        public async Task CloseAsync(CancellationToken cancellationToken)
        {
            if (this.host is null)
                return;

            await this.host.StopAsync(cancellationToken);
            this.host.Dispose();
            this.host = null;
        }

        public async Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            this.host = this.builder.Build();
            if (this.host is null)
            {
                throw new InvalidOperationException("host is null");
            }
            await this.host.StartAsync(cancellationToken);
            return this.serviceContext.PublishAddress;
        }
    }
}
