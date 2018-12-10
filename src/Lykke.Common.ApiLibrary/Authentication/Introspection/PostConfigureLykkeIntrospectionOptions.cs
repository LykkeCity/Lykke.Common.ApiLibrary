using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using IdentityModel.Client;
using Lykke.Common.ApiLibrary.Authentication.Introspection.Infrastructure;
using Lykke.Common.ApiLibrary.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace Lykke.Common.ApiLibrary.Authentication.Introspection
{
    public class PostConfigureLykkeIntrospectionOptions : IPostConfigureOptions<LykkeIntrospectionOptions>
    {
        private readonly IDistributedCache _cache;

        public PostConfigureLykkeIntrospectionOptions(IDistributedCache cache = null)
        {
            _cache = cache;
        }

        public void PostConfigure(string name, LykkeIntrospectionOptions options)
        {
            options.Validate();

            if (options.EnableCaching && _cache == null)
            {
                throw new ArgumentException("Caching is enabled, but no IDistributedCache is found in the services collection", nameof(_cache));
            }

            options.IntrospectionClient = new AsyncLazy<IntrospectionClient>(() => InitializeIntrospectionClient(options));
            options.LazyIntrospections = new ConcurrentDictionary<string, AsyncLazy<IntrospectionResponse>>();
        }

        private async Task<string> GetIntrospectionEndpointFromDiscoveryDocument(LykkeIntrospectionOptions Options)
        {
            DiscoveryClient client;

            if (Options.DiscoveryHttpHandler != null)
            {
                client = new DiscoveryClient(Options.Authority, Options.DiscoveryHttpHandler);
            }
            else
            {
                client = new DiscoveryClient(Options.Authority);
            }

            client.Timeout = Options.DiscoveryTimeout;
            client.Policy = Options?.DiscoveryPolicy ?? new DiscoveryPolicy();
            
            var disco = await client.GetAsync().ConfigureAwait(false);
            if (disco.IsError)
            {
                if (disco.ErrorType == ResponseErrorType.Http)
                {
                    throw new InvalidOperationException($"Discovery endpoint {client.Url} is unavailable: {disco.Error}");
                }
                if (disco.ErrorType == ResponseErrorType.PolicyViolation)
                {
                    throw new InvalidOperationException($"Policy error while contacting the discovery endpoint {client.Url}: {disco.Error}");
                }
                if (disco.ErrorType == ResponseErrorType.Exception)
                {
                    throw new InvalidOperationException($"Error parsing discovery document from {client.Url}: {disco.Error}");
                }
            }

            return disco.IntrospectionEndpoint;
        }

        private async Task<IntrospectionClient> InitializeIntrospectionClient(LykkeIntrospectionOptions Options)
        {
            string endpoint;

            if (Options.IntrospectionEndpoint.IsPresent())
            {
                endpoint = Options.IntrospectionEndpoint;
            }
            else
            {
                endpoint = await GetIntrospectionEndpointFromDiscoveryDocument(Options).ConfigureAwait(false);
                Options.IntrospectionEndpoint = endpoint;
            }

            IntrospectionClient client;
            if (Options.IntrospectionHttpHandler != null)
            {
                client = new IntrospectionClient(
                    endpoint,
                    headerStyle: Options.BasicAuthenticationHeaderStyle,
                    innerHttpMessageHandler: Options.IntrospectionHttpHandler);
            }
            else
            {
                client = new IntrospectionClient(endpoint);
            }

            client.Timeout = Options.DiscoveryTimeout;
            return client;
        }
    }
}