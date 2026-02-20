using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ReportApi.options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ReportApi.Worker;

public class GerarAutomatizacoesPendentesWorker : BackgroundReportWorker
{
    #region Atributos

    private readonly string _arquivoLogAutomatizacaoRelatorio = "Automatização";

    #endregion Atributos

    #region Construtores

    public GerarAutomatizacoesPendentesWorker(IOptions<DatabaseOptions> option, IServiceProvider serviceProvider, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRelatorio configuracaoRelatorio) : base(option, serviceProvider, configuracaoRelatorio) { }

    #endregion Construtores

    #region Métodos Protegidos

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        Servicos.Log.TratarErro("GerarAutomatizacoesPendentesWorker Parando.", "StartAPI");

        return base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Servicos.Log.TratarErro("GerarAutomatizacoesPendentesWorker Iniciado.", "StartAPI");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();
                Repositorio.UnitOfWork unitOfWork = scope.ServiceProvider.GetService<Repositorio.UnitOfWork>();

                List<int> automatizacoesPendentesGeracao = ObterAutomatizacoesPendentesGeracao(unitOfWork);

                if (automatizacoesPendentesGeracao.Count <= 0)
                {
                    Servicos.Log.TratarErro("Não existem automatizações pendentes para geração. Aguardando 60 segundos para nova verificação.", _arquivoLogAutomatizacaoRelatorio);
                    await Task.Delay(TimeSpan.FromSeconds(60), cancellationToken);
                    continue;
                }

                Servicos.Embarcador.Relatorios.Automatizacao servicoAutomatizacao = new Servicos.Embarcador.Relatorios.Automatizacao(unitOfWork, TipoServicoMultisoftware, Cliente);

                foreach (int codigoAutomatizacaoPendenteGeracao in automatizacoesPendentesGeracao)
                {
                    Servicos.Log.TratarErro($"Iniciando geração da automatização {codigoAutomatizacaoPendenteGeracao}.", _arquivoLogAutomatizacaoRelatorio);

                    servicoAutomatizacao.GerarRelatorio(codigoAutomatizacaoPendenteGeracao);

                    Servicos.Log.TratarErro($"Finalizou a geração da automatização {codigoAutomatizacaoPendenteGeracao}.", _arquivoLogAutomatizacaoRelatorio);
                }

                await Task.Delay(TimeSpan.FromSeconds(60), cancellationToken);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, _arquivoLogAutomatizacaoRelatorio);
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }
    }

    #endregion Métodos Protegidos

    #region Métodos Privados

    private List<int> ObterAutomatizacoesPendentesGeracao(Repositorio.UnitOfWork unitOfWork)
    {
        Repositorio.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio repositorioAutomatizacaoGeracaoRelatorio = new Repositorio.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio(unitOfWork);

        return repositorioAutomatizacaoGeracaoRelatorio.BuscarCodigosAutomatizacoesPendentesGeracao();
    }

    #endregion Métodos Privados
}
