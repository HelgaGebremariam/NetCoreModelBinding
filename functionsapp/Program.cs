using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.Functions.Worker.Configuration;
using functionsapp.Middleware;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;

namespace functionsapp
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults(worker =>
                {
                    worker.UseNewtonsoftJson().UseMiddleware<ModelBindingMiddleware>(); 
                })
                .ConfigureServices(s => {
                    
                })
                .Build();

            host.Run();
        }
    }
}