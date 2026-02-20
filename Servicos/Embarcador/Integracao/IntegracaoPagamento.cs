using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Entidades.Embarcador.Escrituracao;
using Repositorio;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao
{
    public class IntegracaoPagamento : ServicoBase
    {
        public IntegracaoPagamento(UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, CancellationToken cancelationToken = default) : base(unitOfWork, tipoServicoMultisoftware, cancelationToken)
        {
        }

        public static void ValidarRetornoPagamentos(List<Dominio.Entidades.Embarcador.Escrituracao.Pagamento> pagamentos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.Pagamento repPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(unitOfWork);
            Repositorio.Embarcador.Escrituracao.PagamentoEDIIntegracao repPagamentoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoEDIIntegracao(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
            Hubs.Pagamento servicoNotificacaoPagamento = new Hubs.Pagamento();

            foreach (Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento in pagamentos)
            {
                if (pagamento.Integracoes.Any(o => o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao) || repPagamentoEDIIntegracao.ContarPorPagamento(pagamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao) > 0)
                {
                    continue;
                }
                else if (pagamento.Integracoes.Any(o => o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) || repPagamentoEDIIntegracao.ContarPorPagamento(pagamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) > 0)
                {
                    pagamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamento.FalhaIntegracao;
                    repPagamento.Atualizar(pagamento);
                    servicoNotificacaoPagamento.InformarPagamentoAtualizada(pagamento.Codigo);
                    continue;
                }
                else if (pagamento.Integracoes.Any(o => o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno) || repPagamentoEDIIntegracao.ContarPorPagamento(pagamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno) > 0)
                {
                    pagamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamento.EmIntegracao;
                    repPagamento.Atualizar(pagamento);
                    servicoNotificacaoPagamento.InformarPagamentoAtualizada(pagamento.Codigo);
                    continue;
                }
                else
                {
                    repDocumentoFaturamento.LiberarPagamentosAutomaticamentePorPagamento(pagamento.Codigo);
                    pagamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamento.Finalizado;
                    repPagamento.Atualizar(pagamento);
                    servicoNotificacaoPagamento.InformarPagamentoAtualizada(pagamento.Codigo);
                }
            }
        }


        public async Task VerificarPagamentosIntegradosAsync()
        {
            Repositorio.Embarcador.Escrituracao.Pagamento repositorioPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(_unitOfWork, _cancellationToken);

            List<Dominio.Entidades.Embarcador.Escrituracao.Pagamento> pagamentos = await repositorioPagamento.BuscarPagamentoAgIntegracaoAsync(_cancellationToken);

            ValidarRetornoPagamentos(pagamentos, _unitOfWork);
        }

        public async Task IniciarIntegracoesComDocumentosAsync(Pagamento pagamento, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso)
        {
            Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repositorioPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.PagamentoEDIIntegracao repositorioPagamentoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoEDIIntegracao(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao> pagamentoIntegracao = repositorioPagamentoIntegracao.BuscarPorPagamentosPendentesDeIntegracao(pagamento.Codigo);
            List<Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao> pagamentoIntegracaoEDI = repositorioPagamentoEDIIntegracao.BuscarPorPagamento(pagamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao);

            foreach (Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao integracao in pagamentoIntegracao)
            {
                switch (integracao.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Digibee:
                        Servicos.Embarcador.Integracao.Digibee.IntegracaoDigibee.IntegrarDocumentosContabilizacao(integracao, _unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Ultragaz:
                        var servicoUltragaz = new Servicos.Embarcador.Integracao.Ultragaz.IntegracaoUltragaz(_unitOfWork, _tipoServicoMultisoftware);
                        servicoUltragaz.IntegrarDocumentosContabilizacao(integracao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.YPE:
                        new Servicos.Embarcador.Integracao.YPE.IntegracaoYPE(_unitOfWork).IntegrarPagamentos(integracao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EFretePagamento:
                        new Servicos.Embarcador.Integracao.EFrete.Recebivel(_unitOfWork, clienteURLAcesso).IntegrarPagamentos(integracao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever:
                        new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(_unitOfWork).IntegrarPagamento(integracao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Camil:
                        await new Servicos.Embarcador.Integracao.Camil.IntegracaoCamil(_unitOfWork).IntegrarPagamento(integracao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.JDEFaturas:
                        new Servicos.Embarcador.Integracao.Vedacit.IntegracaoJDEFaturas(_unitOfWork).IntegrarPagamento(integracao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.GrupoSC:
                        new Servicos.Embarcador.Integracao.GrupoSC.IntegracaoGrupoSC(_unitOfWork).IntegrarPagamento(integracao);
                        break;
                    default:
                        break;
                }
            }

            FTP.IntegracaoFTP servcoIntegracaoFTP = new FTP.IntegracaoFTP(_unitOfWork, _tipoServicoMultisoftware, _cancellationToken);

            foreach (Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao integracao in pagamentoIntegracaoEDI)
            {
                if (integracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao)
                {
                    //cada tipo de integração deve adicionar os documentos necessários nas filas
                    switch (integracao.TipoIntegracao.Tipo)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP:
                            await servcoIntegracaoFTP.EnviarEDIAsync(integracao);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada:
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao:
                            integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                            break;
                        default:
                            break;
                    }
                }

                await repositorioPagamentoEDIIntegracao.AtualizarAsync(integracao);
            }
        }
    }
}
