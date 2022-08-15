using EvenTransit.Domain.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Serilog.Core;
using Serilog.Events;

namespace EvenTransit.Logging.Serilog;

public class HttpEnricher : ILogEventEnricher
{
    private const string LogItemsKey = "LogItems";

    private IHttpContextAccessor? _httpContextAccessor;

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        try
        {
            var context = _httpContextAccessor?.HttpContext;
            var request = context?.Request;
            if (request == null) return;
            if (context == null) return;
            
            if (!context.Items.TryGetValue(LogItemsKey, out var items))
            {
                var config =
                    context.RequestServices.GetRequiredService<EvenTransitConfig>();

                items = GetLogItems(config, request.Headers);

                FilterCookiesToBeLogged(config, request.Cookies, (Dictionary<string, string?>)items);

                context.Items.Add(LogItemsKey, items);
            }

            foreach (var item in (Dictionary<string, string>)items!)
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(item.Key, item.Value));
            }
        }
        catch (Exception e)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(nameof(HttpEnricher), $"Failed. {e.Message}"));
        }
    }

    public void SetHttpContextAccessor(IHttpContextAccessor? httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    private static Dictionary<string, string> GetLogItems(EvenTransitConfig config, IHeaderDictionary headers)
    {
        if (config.Logging?.Headers == null || !config.Logging.Headers.Any())
        {
            return headers.ToDictionary(x => x.Key, x => x.Value.ToString());
        }

        var result = new Dictionary<string, string>();

        foreach (var name in config.Logging.Headers)
        {
            if (headers.TryGetValue(name, out var header))
            {
                result.Add(name, header);
            }
        }

        return result;
    }

    private static void FilterCookiesToBeLogged(EvenTransitConfig config, IRequestCookieCollection? cookies,
        Dictionary<string, string?> items)
    {
        if (cookies == null || !cookies.Any()) return;

        if (config.Logging?.Cookies == null || !config.Logging.Cookies.Any()) return;

        items.Remove(HeaderNames.Cookie);

        foreach (var name in config.Logging.Cookies)
        {
            if (cookies.TryGetValue(name, out var cookie))
            {
                items.Add(name, cookie);
            }
        }
    }
}
