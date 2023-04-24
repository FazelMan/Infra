using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Infra.Shared.Http.Filters
{
    public class TtfbCalculator : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            await next();

            stopWatch.Stop();

            context.HttpContext.Response.Headers.Add(
                "x-time-elapsed",
                stopWatch.Elapsed.ToString());
        }
    }
}
