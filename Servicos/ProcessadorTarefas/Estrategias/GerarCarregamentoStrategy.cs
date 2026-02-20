using Dominio.Entidades.ProcessadorTarefas;
using Dominio.Interfaces.Database;
using Dominio.Interfaces.ProcessadorTarefas;
using Dominio.Interfaces.Repositorios.ProcessadorTarefas;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.ProcessadorTarefas;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.ProcessadorTarefas.Estrategias;

public class GerarCarregamentoStrategy : IStrategyRetornoIntegracao
{
    private readonly ITarefaIntegracao _repositorioTarefaIntegracao;
    private readonly IRequestDocumentoRepository _repositorioRequestDocumento;
    private readonly Repositorio.UnitOfWork _unitOfWork;

    public GerarCarregamentoStrategy(IRequestDocumentoRepository repositorioRequestDocumento, ITarefaIntegracao repositorioTarefaIntegracao, ITenantService tenantService)
    {
        _repositorioTarefaIntegracao = repositorioTarefaIntegracao;
        _repositorioRequestDocumento = repositorioRequestDocumento;
        _unitOfWork = new Repositorio.UnitOfWork(tenantService.StringConexao(), Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova);
    }

    #region Métodos Públicos

    public async Task RetornoIntegracaoTarefa(ContextoEtapa contexto, TarefaIntegracao tarefaIntegracao, CancellationToken cancellationToken)
    {
        switch (tarefaIntegracao.TipoIntegracao)
        {
            case TipoIntegracao.GrupoSC:
                await new Embarcador.Integracao.GrupoSC.IntegracaoGrupoSC(_unitOfWork, _repositorioRequestDocumento, _repositorioTarefaIntegracao).IntegrarRetornoGerarCarregamentoAsync(contexto, tarefaIntegracao, cancellationToken);
                break;
            case TipoIntegracao.Comprovei:
                await new Embarcador.Integracao.Comprovei.IntegracaoComprovei(_unitOfWork, _repositorioRequestDocumento, _repositorioTarefaIntegracao).IntegrarRetornoGerarCarregamentoAsync(contexto, tarefaIntegracao, cancellationToken);
                break;
            case TipoIntegracao.Fusion:
                await new Embarcador.Integracao.Fusion.IntegracaoFusion(_unitOfWork, _repositorioRequestDocumento, _repositorioTarefaIntegracao).IntegrarRetornoGerarCarregamentoAsync(contexto, tarefaIntegracao, cancellationToken);
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

    #endregion Métodos Privados
}
