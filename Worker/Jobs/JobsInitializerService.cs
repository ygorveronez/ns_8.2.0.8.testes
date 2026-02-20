using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGT.Hangfire.Threads.Jobs
{
    public class JobsInitializerService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public JobsInitializerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                var connectionString = Servicos.Database.ConnectionString.Instance.GetDatabaseConnectionString("AdminMultisoftware");

                await Task.Run(() => JobsInitialConfiguration.FirstRun(connectionString, configuration, scope.ServiceProvider), cancellationToken);

                await Task.Run(JobsInitialConfiguration.StartJobManagement, cancellationToken);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"Erro ao iniciar Jobs: {ex.Message}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
