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

public class AdicionarAtendimentoStrategy : IStrategyQuebraRequest, IStrategyRetornoIntegracao
{
    private readonly ITarefaIntegracao _repositorioTarefaIntegracao;
    private readonly IRequestSubtarefaRepository _repositorioSubtarefa;
    private readonly IProcessamentoTarefaRepository _repositorioTarefa;
    private readonly Repositorio.UnitOfWork _unitOfWork;

    public AdicionarAtendimentoStrategy(IRequestSubtarefaRepository repositorioSubtarefa, ITarefaIntegracao repositorioTarefaIntegracao, IProcessamentoTarefaRepository repositorioTarefa, ITenantService tenantService)
    {
        _repositorioTarefaIntegracao = repositorioTarefaIntegracao;
        _repositorioSubtarefa = repositorioSubtarefa;
        _repositorioTarefa = repositorioTarefa;
        _unitOfWork = new Repositorio.UnitOfWork(tenantService.StringConexao(), Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova);
    }

    #region Métodos Públicos

    public List<RequestSubtarefa> QuebrarRequestTarefa(ContextoEtapa contexto)
    {
        List<Dominio.ObjetosDeValor.WebService.Atendimento.AdicionarAtendimento> atendimentosIntegracao =
            contexto.RequestDoc.Dados.FromBsonDocument<List<Dominio.ObjetosDeValor.WebService.Atendimento.AdicionarAtendimento>>();

        return PreencherSubtarefas(atendimentosIntegracao, contexto.TarefaId);
    }

    public async Task RetornoIntegracaoTarefa(ContextoEtapa contexto, TarefaIntegracao tarefaIntegracao, CancellationToken cancellationToken)
    {
        switch (tarefaIntegracao.TipoIntegracao)
        {
            case TipoIntegracao.Comprovei:
                await new Embarcador.Integracao.Comprovei.IntegracaoComprovei(_unitOfWork, _repositorioSubtarefa, _repositorioTarefaIntegracao)
                    .IntegrarRetornoAdicionarAtendimentoAsync(contexto, tarefaIntegracao, cancellationToken);
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

    private List<RequestSubtarefa> PreencherSubtarefas(
        List<Dominio.ObjetosDeValor.WebService.Atendimento.AdicionarAtendimento> atendimentosIntegracao, string tarefaId)
    {
        List<RequestSubtarefa> retorno = new List<RequestSubtarefa>();
        int ordem = 0;

        foreach (var atendimentoIntegracao in atendimentosIntegracao)
        {
            retorno.Add(new RequestSubtarefa
            {
                TarefaId = tarefaId,
                Dados = atendimentoIntegracao.ToBsonDocument(),
                Ordem = ordem++
            });
        }

        return retorno;
    }

    #endregion Métodos Privados
}
