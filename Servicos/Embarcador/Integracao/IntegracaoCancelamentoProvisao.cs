using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Entidades.Embarcador.Escrituracao;
using Repositorio;

namespace Servicos.Embarcador.Integracao
{
    public class IntegracaoCancelamentoProvisao: ServicoBase
    {
        public IntegracaoCancelamentoProvisao(UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, CancellationToken cancelationToken = default) : base(unitOfWork, tipoServicoMultisoftware, cancelationToken)
        {
        }

        public static void VerificarCancelamentoProvisoesIntegradas(UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.CancelamentoProvisao repCancelamentoProvisao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisao(unitOfWork);
            Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao repCancelamentoProvisaoIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao(unitOfWork);
            Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao repCancelamentoProvisaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao(unitOfWork);
            Hubs.CancelamentoProvisao servicoNotificacaoCancelamentoProvisao = new Hubs.CancelamentoProvisao();

            List<int> cancelamentos = repCancelamentoProvisao.BuscarCancelamentoProvisaoAgIntegracao();
            List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao> integracoesCancelamento = repCancelamentoProvisaoIntegracao.BuscarPorCancelamentoProvisao(cancelamentos);


            foreach (int cancelamentoProvisao in cancelamentos)
            {

                List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao> integracoesCancelamentoProvisaoAtual = integracoesCancelamento.Where(x => x.CancelamentoProvisao.Codigo == cancelamentoProvisao).ToList();
                int integracoesPendetesEDI = repCancelamentoProvisaoEDIIntegracao.ContarPorCancelamentoProvisao(cancelamentoProvisao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao);
                int integracoesProblemaEDI = repCancelamentoProvisaoEDIIntegracao.ContarPorCancelamentoProvisao(cancelamentoProvisao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);

                if (integracoesCancelamentoProvisaoAtual.Any(x => x.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno || x.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao) || integracoesPendetesEDI > 0)
                    continue;

                var cancelamento = repCancelamentoProvisao.BuscarPorCodigo(cancelamentoProvisao, false);

                if (integracoesCancelamentoProvisaoAtual.Any(x => x.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) || integracoesProblemaEDI > 0)
                {

                    cancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.FalhaIntegracao;
                    repCancelamentoProvisao.Atualizar(cancelamento);
                    servicoNotificacaoCancelamentoProvisao.InformarCancelamentoProvisaoAtualizada(cancelamentoProvisao);
                    continue;
                }

                bool todosDocumentosCancelados = !integracoesCancelamentoProvisaoAtual.Any(x => x.DocumentoProvisao.Cancelado == false);

                cancelamento.Situacao = todosDocumentosCancelados ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.Cancelado : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.PendenciaCancelamento;
                repCancelamentoProvisao.Atualizar(cancelamento);
                servicoNotificacaoCancelamentoProvisao.InformarCancelamentoProvisaoAtualizada(cancelamentoProvisao);
            }
        }

        public async Task IniciarIntegracoesComDocumentosAsync(CancelamentoProvisao cancelamentoProvisao)
        {
            Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao repositorioCancelamentoProvisaoIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao repositorioCancelamentoProvisaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao(_unitOfWork, _cancellationToken);

            List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao> cancelamentoProvisaoIntegracao = await repositorioCancelamentoProvisaoIntegracao.BuscarPorCancelamentoProvisaoAsync(cancelamentoProvisao.Codigo, _cancellationToken);
            List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao> cancelamentoProvisaoIntegracaoEDI = await repositorioCancelamentoProvisaoEDIIntegracao.BuscarPorCancelamentoProvisaoAsync(cancelamentoProvisao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao, _cancellationToken);

            FTP.IntegracaoFTP servicoIntegracaoFTP = new FTP.IntegracaoFTP(_unitOfWork, _tipoServicoMultisoftware, _cancellationToken);

            foreach (Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao integracao in cancelamentoProvisaoIntegracao)
            {
                switch (integracao.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Carrefour:
                        IntegrarGZIPCarrefour(integracao, _unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever:
                        new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(_unitOfWork).IntegrarCancelamentoDocumentoProvisao(integracao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Camil:
                        new Servicos.Embarcador.Integracao.Camil.IntegracaoCamil(_unitOfWork).IntegrarCancelamentoProvisao(integracao);
                        break;
                    default:
                        break;
                }
            }

            foreach (Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao integracao in cancelamentoProvisaoIntegracaoEDI)
            {
                if (integracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao)
                {
                    //cada tipo de integração deve adicionar os documentos necessários nas filas
                    switch (integracao.TipoIntegracao.Tipo)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP:
                            await servicoIntegracaoFTP.EnviarEDIAsync(integracao);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada:
                            integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public static void IntegrarGZIPCarrefour(CancelamentoProvisaoIntegracao integracao, Repositorio.UnitOfWork unitOfWork)
        {
            // Obtem EDI
            byte[] gz = GerarGZIP(integracao, unitOfWork, out string nomeArquivo, false);

            if (gz != null)
            {
                // Envia pro ws da carrefour
                //Servicos.Embarcador.Integracao.Carrefour.IntegracaoCarrefour.GetInstance().IntegrarProvisao(integracao, gz, nomeArquivo, unitOfWork);
            }
        }

        public static byte[] GerarGZIP(CancelamentoProvisaoIntegracao integracao, Repositorio.UnitOfWork unitOfWork, out string nomeArquivo, bool incrementarSequencia)
        {
            Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao repCancelamentoProvisaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao(unitOfWork);

            // Obtem EDI
            Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao cancelamentoProvisaoEDI = repCancelamentoProvisaoEDIIntegracao.BuscarUltimoPorCancelamentoProvisao(integracao.CancelamentoProvisao.Codigo);
            System.IO.MemoryStream edi = Servicos.Embarcador.Integracao.IntegracaoEDI.GerarEDI(cancelamentoProvisaoEDI, unitOfWork);

            if (edi != null)
            {
                nomeArquivo = Servicos.Embarcador.Integracao.IntegracaoEDI.ObterNomeArquivoEDI(cancelamentoProvisaoEDI, incrementarSequencia, unitOfWork);
                repCancelamentoProvisaoEDIIntegracao.Atualizar(cancelamentoProvisaoEDI);
                return Utilidades.File.GerarGZIP(edi.ToArray());
            }
            else
            {
                nomeArquivo = "";
                return null;
            }

        }
    }
}
