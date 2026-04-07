using CoreWCF;
using KT3.BookingPrototype.Core.Contracts;
using System;

namespace KT3.BookingPrototype.Service.Services;

public static class AuthGuard
{
    public static void EnsureAuthorized()
    {
        var context = OperationContext.Current;
        if (context is null)
        {
            throw new FaultException("Authentication failed: no operation context.");
        }

        var headers = context.IncomingMessageHeaders;
        var index = headers.FindHeader(AuthHeader.Name, AuthHeader.Namespace);
        if (index < 0)
        {
            throw new FaultException("Authentication failed: missing X-Api-Key header.");
        }

        var apiKey = headers.GetHeader<string>(index);
        if (!string.Equals(apiKey, AuthHeader.DemoApiKey, StringComparison.Ordinal))
        {
            throw new FaultException("Authentication failed: invalid API key.");
        }
    }
}
