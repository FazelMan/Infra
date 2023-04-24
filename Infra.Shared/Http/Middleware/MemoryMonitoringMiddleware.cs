using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Infra.Shared.Http.Middleware
{
    public class MemoryMonitoringMiddleware
    {
        private readonly RequestDelegate _next;
        private int _maxMemoryLimit = 0;

        public MemoryMonitoringMiddleware(
            RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (_maxMemoryLimit == 0)
            {
                if (Shared.Helpers.Host.Config["EndpointConfigs:MaxMemoryLimit"] != null)
                {
                    if (int.TryParse(Shared.Helpers.Host.Config["EndpointConfigs:MaxMemoryLimit"], out int limit))
                    {
                        _maxMemoryLimit = limit;
                    }
                }
            }

            // if it is still 0, then default to 200
            if (_maxMemoryLimit == 0)
                _maxMemoryLimit = 200;

            var memoryConsumption =
                Process.GetCurrentProcess().PrivateMemorySize64 / (1024 * 1024);

            if ((memoryConsumption / _maxMemoryLimit) == 1)
                GC.Collect();

            await _next(context);
        }
    }
}