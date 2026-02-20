using Dominio.Entidades.ProcessadorTarefas;
using Dominio.Interfaces.Database;
using Dominio.Interfaces.ProcessadorTarefas;
using Dominio.Interfaces.Repositorios.ProcessadorTarefas;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.ProcessadorTarefas;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Utilidades.Extensions;

namespace Servicos.ProcessadorTarefas.Estrategias;

public class GerarCarregamentoRoteirizacaoEmLoteStrategy : IStrategyQuebraRequest, IStrategyRetornoIntegracao
{
    private readonly ITarefaIntegracao _repositorioTarefaIntegracao;
    private readonly IRequestSubtarefaRepository _repositorioSubtarefa;
    private readonly IProcessamentoTarefaRepository _repositorioTarefa;
    private readonly Repositorio.UnitOfWork _unitOfWork;

    public GerarCarregamentoRoteirizacaoEmLoteStrategy(
        IRequestSubtarefaRepository repositorioSubtarefa,
        ITarefaIntegracao repositorioTarefaIntegracao,
        IProcessamentoTarefaRepository repositorioTarefa,
        ITenantService tenantService)
    {
        _repositorioTarefaIntegracao = repositorioTarefaIntegracao;
        _repositorioSubtarefa = repositorioSubtarefa;
        _repositorioTarefa = repositorioTarefa;
        _unitOfWork = new Repositorio.UnitOfWork(tenantService.StringConexao(), Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova);
    }

    #region Métodos Públicos

    public List<RequestSubtarefa> QuebrarRequestTarefa(ContextoEtapa contexto)
    {
        List<Dominio.ObjetosDeValor.Embarcador.Carga.CarregamentoRoteirizacao> carregamentosRoteirizacao =
            contexto.RequestDoc.Dados.FromBsonDocument<List<Dominio.ObjetosDeValor.Embarcador.Carga.CarregamentoRoteirizacao>>();

        return PreencherSubtarefas(carregamentosRoteirizacao, contexto.TarefaId);
    }

    public async Task RetornoIntegracaoTarefa(ContextoEtapa contexto, TarefaIntegracao tarefaIntegracao, CancellationToken cancellationToken)
    {
        switch (tarefaIntegracao.TipoIntegracao)
        {
            case TipoIntegracao.Fusion:
                await new Embarcador.Integracao.Fusion.IntegracaoFusion(_unitOfWork, _repositorioSubtarefa, _repositorioTarefaIntegracao)
                    .IntegrarRetornoGerarCarregamentoRoteirizacaoEmLoteAsync(contexto, tarefaIntegracao, cancellationToken);
                break;

            default:
                {
                    tarefaIntegracao.Tentativas++;
                    tarefaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    tarefaIntegracao.ProblemaIntegracao = "Integração não implementada.";

                    await _repositorioTarefaIntegracao.AtualizarAsync(tarefaIntegracao, cancellationToken);

                    break;
                }
        }
    }

    #endregion Métodos Públicos

    #region Métodos Privados

    private List<RequestSubtarefa> PreencherSubtarefas(List<Dominio.ObjetosDeValor.Embarcador.Carga.CarregamentoRoteirizacao> carregamentosRoteirizacao, string tarefaId)
    {
        List<RequestSubtarefa> retorno = new List<RequestSubtarefa>();
        int ordem = 0;

        foreach (var carregamentoRoteirizacao in carregamentosRoteirizacao)
        {
            retorno.Add(new RequestSubtarefa
            {
                TarefaId = tarefaId,
                Dados = carregamentoRoteirizacao.ToBsonDocument(),
                Ordem = ordem++
            });
        }

        return retorno;
    }

    #endregion Métodos Privados
}
