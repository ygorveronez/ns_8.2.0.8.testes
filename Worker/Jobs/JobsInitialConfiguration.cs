using AdminMultisoftware.Repositorio;
using Dominio.Entidades;
using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.Repositorios;
using Hangfire;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.Storage;
using MSMQ.Messaging;
using Repositorio;
using SGT.BackgroundWorkers;
using SGT.BackgroundWorkers.Utils;
using System.ComponentModel;
using System.Reflection;

namespace SGT.Hangfire.Threads.Jobs;

public class JobsInitialConfiguration
{
    public static void FirstRun(string adminDatabaseConnectionString, IConfiguration configuration, IServiceProvider serviceProvider)
    {
        bool homologacao = bool.Parse(configuration["ConfiguracoesAmbiente:Homologacao"]);

        AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(new AdminMultisoftware.Repositorio.UnitOfWork(adminDatabaseConnectionString));

        var clientesHangfire = repClienteURLAcesso.BuscarPorClientesPorTipoExecucao(AdminMultisoftware.Dominio.Enumeradores.TipoExecucao.Hangfire, homologacao);

        var clientesExecutaraoHangfire = configuration["ConfiguracoesAmbiente:ClientesExecutaraoHangfire"];

        if (!string.IsNullOrWhiteSpace(clientesExecutaraoHangfire))
        {
            var codigosPermitidos = clientesExecutaraoHangfire
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToHashSet();

            clientesHangfire = clientesHangfire
                .Where(c => codigosPermitidos.Contains(c.Cliente.Codigo))
                .ToList();
        }

        foreach (var clienteHangfire in clientesHangfire)
        {

            using (var scope = serviceProvider.CreateScope())
            {

                var backgroundJobClient = scope.ServiceProvider.GetRequiredService<IBackgroundJobClient>();

                var types = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => !a.IsDynamic)
                    .SelectMany(a =>
                    {
                        try
                        {
                            return a.GetTypes();
                        }
                        catch (ReflectionTypeLoadException ex)
                        {
                            return ex.Types.Where(t => t != null);
                        }
                    })
                    .Where(x => x is not null && x.IsClass && !x.IsAbstract)
                    .Where(x => x.IsAssignableTo(typeof(ILongRunningProcessBase)))
                    .ToList();

                foreach (var type in types)
                {
                    var runningConfigAttribute = type.GetCustomAttribute<RunningConfigAttribute>();

                    if (runningConfigAttribute != null)
                    {

                        var jobRunnerType = typeof(JobRunner<>).MakeGenericType(type);

                        var runMethod = jobRunnerType.GetMethod("Run");
                        var jobRunnerInstance = Activator.CreateInstance(jobRunnerType);

                        runMethod.Invoke(jobRunnerInstance, new object[] { clienteHangfire.Codigo, runningConfigAttribute.DuracaoPadrao });
                    }
                }
            }
        }
    }

    public static void StartJobManagement()
    {
        var recurringJobManager = new RecurringJobManager();

        string jobId = "jobs-management";

        using (var connection = JobStorage.Current.GetConnection())
        {
            if (connection.GetRecurringJobs().All(job => job.Id != jobId))
            {
                RecurringJob.AddOrUpdate<JobsManagement>(
                    jobId,
                    job => job.ExecuteAsync(CancellationToken.None),
                    "*/5 * * * *",
                    new RecurringJobOptions
                    {
                        MisfireHandling = MisfireHandlingMode.Relaxed,
                        TimeZone = TimeZoneInfo.Utc
                    }
                );
            }
            else
            {
                Servicos.Log.TratarErro($"O job '{jobId}' já existe e não será adicionado novamente.");
            }
        }
    }
}

