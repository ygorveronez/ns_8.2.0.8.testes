using CoreWCF;

namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class Atendimento(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), IAtendimento
    {
        #region Métodos Públicos

        public Retorno<bool> AtualizarStatusDevolucao(Dominio.ObjetosDeValor.WebService.Atendimento.AtualizarStatusDevolucao atendimentoStatusDevolucao)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                ValidarToken();

                Repositorio.Embarcador.Chamados.ChamadoIntegracao repChamado = new Repositorio.Embarcador.Chamados.ChamadoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao chamadoIntegracao = repChamado.BuscarPorProtocolo(atendimentoStatusDevolucao.Protocolo);

                if (chamadoIntegracao == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Não foi localizada integração para o protocolo informado.");

                chamadoIntegracao.StatusDevolucao = atendimentoStatusDevolucao.Status;
                chamadoIntegracao.SenhaDevolucao = atendimentoStatusDevolucao.SenhaSAP;
                chamadoIntegracao.ObservacaoDevolucao = atendimentoStatusDevolucao.Observacao;

                repChamado.Atualizar(chamadoIntegracao);

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao atualizar status da devolução!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> AdicionarAtendimento(Dominio.ObjetosDeValor.WebService.Atendimento.AdicionarAtendimento adicionarAtendimento)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Dominio.ObjetosDeValor.WebService.Retorno<int> retorno = new Servicos.WebService.Atendimento.Atendimento(unitOfWork, TipoServicoMultisoftware, Auditado, Conexao.createInstance(_serviceProvider).AdminStringConexao).AdicionarAtendimento(adicionarAtendimento, default).GetAwaiter().GetResult();

                return Retorno<bool>.CreateFrom(new Dominio.ObjetosDeValor.WebService.Retorno<bool>
                {
                    Status = retorno.Status,
                    Mensagem = retorno.Mensagem,
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Objeto = retorno.Objeto > 0
                });
            });
        }

        #endregion Métodos Públicos

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceAtendimento;
        }

        #endregion Métodos Protegidos Sobrescritos
    }
}
