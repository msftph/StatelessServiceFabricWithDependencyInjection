using System.Fabric;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Hosting;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace StatelessServiceWithDI
{
    internal class TcpCommunicationListener : ICommunicationListener
    {
        private readonly ServiceContext serviceContext;
        private readonly HostApplicationBuilder builder;

        private IHost host;

        private TcpListener tcpListener;


        public TcpCommunicationListener(
            ServiceContext serviceContext, 
            Func<HostApplicationBuilder> createBuilderFunc)
        {
            this.builder = createBuilderFunc();            
            this.serviceContext = serviceContext;
        }

        public void Abort()
        {
            this.tcpListener.Stop();
            if (this.host is null)
                return;
            this.host.Dispose();
            this.host = null;
        }

        public async Task CloseAsync(CancellationToken cancellationToken)
        {
            this.tcpListener.Stop();
            await CloseHostAsync(cancellationToken);            
        }

        private async Task CloseHostAsync(CancellationToken cancellationToken)
        {
            if (this.host is null)
                return;

            await this.host.StopAsync(cancellationToken);
            this.host.Dispose();
            this.host = null;
        }


        public async Task<string> OpenAsync(CancellationToken cancellationToken)
        {            
            string publishAddress = this.serviceContext.PublishAddress;
            if (string.IsNullOrEmpty(publishAddress))
            {
                throw new InvalidOperationException("publishAddress is null or empty");
            }

            bool valid = IPEndPoint.TryParse(publishAddress, out IPEndPoint? ipEndPoint); 
            if (!valid || ipEndPoint is null)
            {
                throw new InvalidOperationException($"publishAddress is not a valid endpoint: {publishAddress}");
            }

            this.tcpListener = new TcpListener(ipEndPoint);

            this.host = this.builder.Build();
            if (this.host is null)
            {
                throw new InvalidOperationException("host is null");
            }
                        
            // start the host after the listener is created
            await this.host.StartAsync(cancellationToken);
            this.tcpListener.Start();

            return this.serviceContext.PublishAddress;
        }
    }
}
