using Hangfire;
using Repositorio;
using SGT.BackgroundWorkers;
using SGT.BackgroundWorkers.Utils;
using System.Diagnostics;
using System.Reflection;

namespace SGT.Hangfire.Threads.Jobs;

public class JobsManagement
{
    private readonly ILogger<JobsManagement> _logger;
    private readonly IConfiguration _configuration;

    public JobsManagement(ILogger<JobsManagement> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task ExecuteAsync(CancellationToken stoppingToken)
    {
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

        bool homologacao = bool.Parse(_configuration["ConfiguracoesAmbiente:Homologacao"]);
        var unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Servicos.Database.ConnectionString.Instance.GetDatabaseConnectionString("AdminMultisoftware"));
        AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWorkAdmin);
        var clientesHangfire = repClienteURLAcesso.BuscarPorClientesPorTipoExecucao(AdminMultisoftware.Dominio.Enumeradores.TipoExecucao.Hangfire, homologacao);

        foreach (var clienteUrlAcesso in clientesHangfire)
        {
            using (var unitOfWork = new Repositorio.UnitOfWork(GerarStringConexao(clienteUrlAcesso.Cliente.ClienteConfiguracao), Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                Repositorio.ControleThread repositorioControleThread = new Repositorio.ControleThread(unitOfWork);
                foreach (var threadType in types)
                {
                    var threads = await repositorioControleThread.BuscarTodosAsync();
                    var codigoCliente = clienteUrlAcesso.Cliente.Codigo;
                    foreach (var controleThread in threads)
                    {
                        var typeFound = types.Where(x => x.Name == controleThread.Thread).FirstOrDefault();
                        if (typeFound != null)
                        {

                            var key = $"job:{typeFound.Name}:{codigoCliente}";

                            if (controleThread.Ativo)
                            {
                                if (HangfireExtensions.JobNaoEstaAgendado(key))
                                {
                                    HangfireExtensions.AgendarJob(key);
                                    var jobRunnerType = typeof(JobRunner<>).MakeGenericType(typeFound);
                                    var runMethod = jobRunnerType.GetMethod("Run");

                                    var jobRunnerInstance = Activator.CreateInstance(jobRunnerType);

                                    runMethod.Invoke(jobRunnerInstance, new object[] { codigoCliente, controleThread.Tempo });
                                }
                                else
                                {
                                    Servicos.Log.TratarErro($"A thread {typeFound.Name} para o cliente {codigoCliente} esta ativa e esta em execucao");
                                }
                            }
                            else
                            {
                                if (HangfireExtensions.JobEstaAgendado(key))
                                {
                                    HangfireExtensions.RemoverAgendamentoDoJob(key);
                                }
                                else
                                {
                                    Servicos.Log.TratarErro($"A thread {typeFound.Name} para o cliente {codigoCliente} esta inativa no banco e não esta em execução");
                                }
                            }
                        }
                        else
                        {
                            Servicos.Log.TratarErro($"Registrado um tipo de thread que não foi encontrado no código, Tipo: {controleThread.Thread}, Cliente: {codigoCliente}");
                        }
                    }
                }
            }
        }
        unitOfWorkAdmin.Dispose();
    }

    private string GerarStringConexao(AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteConfiguracao configuracao)
    {
        if (configuracao.LoginPorAD)
            return $"Data Source={configuracao.DBServidor};database={configuracao.DBBase};Integrated Security=SSPI;persist security info=True;Max Pool Size=1000;";
        else
            return $"Data Source={configuracao.DBServidor};Initial Catalog={configuracao.DBBase};User Id={configuracao.DBUsuario};Password={configuracao.DBSenha};Max Pool Size=1000;";
    }
}