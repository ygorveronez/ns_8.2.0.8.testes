using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ReportApi.options;
using ReportApi.ReportService;

namespace ReportApi.Worker;

public class ConsultarRelatoriosPendentesWorker : BackgroundReportWorker
{
    #region Atributos

    private List<int> _relatoriosEmProcessamento = new List<int>();
    private List<Thread> _threadsGeracaoRelatorio = new List<Thread>();
    private readonly string _arquivoLogGeracaoRelatorio = "Geração";

    #endregion Atributos

    #region Construtores

    public ConsultarRelatoriosPendentesWorker(IOptions<DatabaseOptions> option, IServiceProvider serviceProvider, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRelatorio configuracaoRelatorio) : base(option, serviceProvider, configuracaoRelatorio) { }

    #endregion Construtores

    #region Métodos Protegidos

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        Servicos.Log.TratarErro("ConsultarRelatoriosPendentesWorker Parando.", "StartAPI");

        return base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Servicos.Log.TratarErro("ConsultarRelatoriosPendentesWorker Iniciado.", "StartAPI");

        while (!cancellationToken.IsCancellationRequested)
        {
            GC.Collect();

            try
            {
                await using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();
                Repositorio.UnitOfWork unitOfWork = scope.ServiceProvider.GetService<Repositorio.UnitOfWork>();

                for (int i = _threadsGeracaoRelatorio.Count - 1; i >= 0; i--)
                {
                    if (_threadsGeracaoRelatorio[i].IsAlive)
                        continue;

                    _threadsGeracaoRelatorio.RemoveAt(i);
                }

                int quantidadeThreadsDisponiveis = _configuracaoRelatorio.QuantidadeRelatoriosParalelo - _threadsGeracaoRelatorio.Count;

                if (quantidadeThreadsDisponiveis <= 0)
                {
                    Servicos.Log.TratarErro($"Já existem {_threadsGeracaoRelatorio.Count} threads ativas. Aguardando 10 segundos para nova verificação.", _arquivoLogGeracaoRelatorio);
                    await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                    continue;
                }

                List<int> relatoriosPendentesGeracao = ObterRelatoriosPendentes(unitOfWork);

                if (relatoriosPendentesGeracao.Count <= 0)
                {
                    Servicos.Log.TratarErro($"Não existem relatórios pendentes para geração. Aguardando 10 segundos para nova verificação. {_threadsGeracaoRelatorio.Count} de {_configuracaoRelatorio.QuantidadeRelatoriosParalelo} threads em execução.", _arquivoLogGeracaoRelatorio);
                    await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                    continue;
                }

                for (int i = 0; i < quantidadeThreadsDisponiveis && i < relatoriosPendentesGeracao.Count; i++)
                {
                    int codigoRelatorioPendenteGeracao = relatoriosPendentesGeracao[i];
                    System.Threading.Thread thread = new System.Threading.Thread(() => GerarRelatorio(codigoRelatorioPendenteGeracao));

                    thread.Start();
                    _threadsGeracaoRelatorio.Add(thread);
                }

                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, _arquivoLogGeracaoRelatorio);
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }
    }

    #endregion Métodos Protegidos

    #region Métodos Privados

    private void GerarRelatorio(int codigoRelatorioControleGeracao)
    {
        try
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            RelatorioFactory relatorioFactory = scope.ServiceProvider.GetService<RelatorioFactory>();

            lock (_relatoriosEmProcessamento)
            {
                if (_relatoriosEmProcessamento.Any(o => o == codigoRelatorioControleGeracao))
                    return;

                _relatoriosEmProcessamento.Add(codigoRelatorioControleGeracao);
            }

            Servicos.Log.TratarErro($"Iniciou thread para geração do relatório {codigoRelatorioControleGeracao}.", _arquivoLogGeracaoRelatorio);

            relatorioFactory.GerarRelatorio(codigoRelatorioControleGeracao);

            Servicos.Log.TratarErro($"Finalizou thread para geração do relatório {codigoRelatorioControleGeracao}.", _arquivoLogGeracaoRelatorio);
        }
        catch (Exception ex)
        {
            Servicos.Log.TratarErro(ex, _arquivoLogGeracaoRelatorio);
        }
        finally
        {
            lock (_relatoriosEmProcessamento)
            {
                if (_relatoriosEmProcessamento.Any(o => o == codigoRelatorioControleGeracao))
                    _relatoriosEmProcessamento.Remove(codigoRelatorioControleGeracao);
            }
        }
    }

    private List<int> ObterRelatoriosPendentes(Repositorio.UnitOfWork unitOfWork)
    {
        Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repositorioRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);

#if DEBUG
        return repositorioRelatorioControleGeracao.BuscarCodigosRelatoriosPendentesGeracaoLocal(_relatoriosEmProcessamento);
#endif
        return repositorioRelatorioControleGeracao.BuscarCodigosRelatoriosPendentesGeracao(_relatoriosEmProcessamento);
    }

    #endregion Métodos Privados
}
