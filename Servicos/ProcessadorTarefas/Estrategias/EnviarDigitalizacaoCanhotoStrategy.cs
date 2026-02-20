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

public class EnviarDigitalizacaoCanhotoStrategy : IStrategyQuebraRequest, IStrategyRetornoIntegracao
{
    private readonly ITarefaIntegracao _repositorioTarefaIntegracao;
    private readonly IRequestSubtarefaRepository _repositorioSubtarefa;
    private readonly Repositorio.UnitOfWork _unitOfWork;

    public EnviarDigitalizacaoCanhotoStrategy(IRequestSubtarefaRepository repositorioSubtarefa, ITarefaIntegracao repositorioTarefaIntegracao, ITenantService tenantService)
    {
        _repositorioTarefaIntegracao = repositorioTarefaIntegracao;
        _repositorioSubtarefa = repositorioSubtarefa;
        _unitOfWork = new Repositorio.UnitOfWork(tenantService.StringConexao(), Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova);
    }

    #region Métodos Públicos

    public List<RequestSubtarefa> QuebrarRequestTarefa(ContextoEtapa contexto)
    {
        List<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto> canhotosIntegracao =
            contexto.RequestDoc.Dados.FromBsonDocument<List<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>>();

        return PreencherSubtarefas(canhotosIntegracao, contexto.TarefaId);
    }

    public async Task RetornoIntegracaoTarefa(ContextoEtapa contexto, TarefaIntegracao tarefaIntegracao, CancellationToken cancellationToken)
    {
        switch (tarefaIntegracao.TipoIntegracao)
        {
            case TipoIntegracao.Comprovei:
                await new Embarcador.Integracao.Comprovei.IntegracaoComprovei(_unitOfWork, _repositorioSubtarefa, _repositorioTarefaIntegracao)
                    .IntegrarRetornoEnviarDigitalizacaoCanhotoAsync(contexto, tarefaIntegracao, cancellationToken);
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

    private List<RequestSubtarefa> PreencherSubtarefas(List<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto> canhotosIntegracao, string tarefaId)
    {
        List<RequestSubtarefa> retorno = new List<RequestSubtarefa>();
        int ordem = 0;

        foreach (var canhotoIntegracao in canhotosIntegracao)
        {
            retorno.Add(new RequestSubtarefa
            {
                TarefaId = tarefaId,
                Dados = canhotoIntegracao.ToBsonDocument(),
                Ordem = ordem++
            });
        }

        return retorno;
    }

    #endregion Métodos Privados
}
