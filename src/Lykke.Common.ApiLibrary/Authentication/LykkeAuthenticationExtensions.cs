using System;
using Lykke.Common.ApiLibrary.Authentication.Introspection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Lykke.Common.ApiLibrary.Authentication
{
    /// <summary>
    /// Extensions for registering the OAuth 2.0 introspection authentication handler
    /// </summary>
    public static class LykkeAuthenticationExtensions
    {
        /// <summary>
        /// Adds the OAuth 2.0 introspection handler.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configureOptions">The configure options.</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddLykkeAuthentication(this AuthenticationBuilder services, Action<LykkeIntrospectionOptions> configureOptions)
            => services.AddLykkeAuthentication(AuthentcationSchemes.Bearer, configureOptions: configureOptions);


        /// <summary>
        /// Adds the OAuth 2.0 introspection handler.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="authenticationScheme">The authentication scheme.</param>
        /// <param name="configureOptions">The configure options.</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddLykkeAuthentication(this AuthenticationBuilder builder, string authenticationScheme, Action<LykkeIntrospectionOptions> configureOptions)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<LykkeIntrospectionOptions>, PostConfigureLykkeIntrospectionOptions>());
            return builder.AddScheme<LykkeIntrospectionOptions, LykkeAuthenticationHandler>(authenticationScheme, configureOptions);
        }
    }
}