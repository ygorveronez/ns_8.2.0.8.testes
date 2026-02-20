using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao
{
    public class IntegracaoCancelamentoPagamento
    {

        public static void IniciarIntegracoesComDocumentos(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento cancelamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao repCancelamentoPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao(unitOfWork);
            Repositorio.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao repCancelamentoPagamentoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao> cancelamentoIntegracoes = repCancelamentoPagamentoIntegracao.BuscarPorCancelamentoPagamento(cancelamento.Codigo);
            List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao> cancelamentoIntegracoesEDIs = repCancelamentoPagamentoEDIIntegracao.BuscarPorCancelamentoPagamento(cancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao);

            foreach (Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao integracao in cancelamentoIntegracoes)
            {
                if (integracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao)
                {
                    //cada tipo de integração deve adicionar os documentos necessários nas filas
                    switch (integracao.TipoIntegracao.Tipo)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP:
                            // Integrar;
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada:
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao:
                            integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Camil:
                            new Servicos.Embarcador.Integracao.Camil.IntegracaoCamil(unitOfWork).IntegrarCancelamentoPagamento(integracao);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever:
                            new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(unitOfWork).IntegrarCancelamentoPagamento(integracao);
                            break;
                        default:
                            break;
                    }
                }
                repCancelamentoPagamentoIntegracao.Atualizar(integracao);
            }

            foreach (Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao integracao in cancelamentoIntegracoesEDIs)
            {
                if (integracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao)
                {
                    //cada tipo de integração deve adicionar os documentos necessários nas filas
                    switch (integracao.TipoIntegracao.Tipo)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP:
                            //FTP.IntegracaoFTP.EnviarEDI(integracao, unitOfWork);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada:
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao:
                            integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                            break;
                        default:
                            break;
                    }
                }
                repCancelamentoPagamentoEDIIntegracao.Atualizar(integracao);
            }
        }

        public static void VerificarProvisoesIntegradas(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.CancelamentoPagamento repCancelamentoPagamento = new Repositorio.Embarcador.Escrituracao.CancelamentoPagamento(unitOfWork);
            Repositorio.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao repCancelamentoPagamentoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao(unitOfWork);
            Hubs.Pagamento servicoNotificacaoPagamento = new Hubs.Pagamento();

            List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento> cancelamentos = repCancelamentoPagamento.BuscarCancelamentoPagamentoAgIntegracao();

            foreach (Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento cancelamento in cancelamentos)
            {
                if (cancelamento.Integracoes.Any(o => (o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno && o.TipoIntegracao != null && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Camil) 
                    || o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao) 
                    || repCancelamentoPagamentoEDIIntegracao.ContarPorCancelamentoPagamento(cancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao) > 0)
                    continue;
                else if (cancelamento.Integracoes.Any(o => o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) || repCancelamentoPagamentoEDIIntegracao.ContarPorCancelamentoPagamento(cancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) > 0)
                {
                    cancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoPagamento.FalhaIntegracao;
                    repCancelamentoPagamento.Atualizar(cancelamento);
                    servicoNotificacaoPagamento.InformarCancelamentoPagamentoAtualizada(cancelamento.Codigo);
                    continue;
                }
                else
                {
                    cancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoPagamento.Cancelado;
                    repCancelamentoPagamento.Atualizar(cancelamento);
                    servicoNotificacaoPagamento.InformarCancelamentoPagamentoAtualizada(cancelamento.Codigo);
                }
            }
        }
    }
}
