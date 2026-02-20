using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Linq;
using System.Collections.Generic;
using SGT.BackgroundWorkers.Utils;
using System.Threading.Tasks;
using System.Threading;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 6000)]

    public class EmissaoDocumentosIntegracao : LongRunningProcessBase<EmissaoDocumentosIntegracao>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            VerificarCargasFilialEmissoraAgGerarCTeAnterior(unitOfWork, _tipoServicoMultisoftware, _stringConexao);
            VerificarCargasGerarCTeAnteriorEmitirDocumentoSempreOrigemDestinoPedido(unitOfWork, _tipoServicoMultisoftware, _stringConexao);

            SolicitarEmissaoNFSManualPententes(unitOfWork, _stringConexao, _tipoServicoMultisoftware);
            VerificarNFSManualPententes(unitOfWork, _tipoServicoMultisoftware, _stringConexao);

            GerarIntegracoesNotaFiscalEletronica(unitOfWork, _tipoServicoMultisoftware, _stringConexao);
            GerarIntegracoesNFSManualCancelamento(unitOfWork, _tipoServicoMultisoftware, _stringConexao);

        }

        private void VerificarNFSManualPententes(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao)
        {
            Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);

            List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> lancamentos = repLancamentoNFSManual.BuscarLancamentosEmEmissao();
            foreach (Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamento in lancamentos)
            {
                Servicos.Embarcador.NFSe.NFSManual serNFSManual = new Servicos.Embarcador.NFSe.NFSManual(unitOfWork);
                serNFSManual.VerificarPendenciasEmissaoLancamento(lancamento, tipoServicoMultisoftware, unitOfWork);
            }
        }

        private void SolicitarEmissaoNFSManualPententes(Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);

            List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> lancamentos = repLancamentoNFSManual.BuscarLancamentosAgEmissao(1);
            if (lancamentos.Count > 0)
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Servicos.Embarcador.Carga.NFSManualPorNota srvNFSManualPorNota = new Servicos.Embarcador.Carga.NFSManualPorNota(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                foreach (Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamento in lancamentos)
                {
                    srvNFSManualPorNota.GerarNFSManualPorNota(lancamento.Codigo, configuracaoEmbarcador, Dominio.Enumeradores.TipoPagamento.Pago, Dominio.Enumeradores.TipoTomador.Outros, tipoServicoMultisoftware, unitOfWork);
                }
            }
        }

        private void VerificarCargasFilialEmissoraAgGerarCTeAnterior(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Servicos.Embarcador.Carga.FilialEmissora serFilialEmissora = new Servicos.Embarcador.Carga.FilialEmissora();
            Servicos.Global.OrquestradorFila servicoOrquestradorFila = new Servicos.Global.OrquestradorFila(unitOfWork, IdentificadorControlePosicaoThread.VerificarCargasFilialEmissoraAgGerarCTeAnterior);
            List<int> cargaAgGerarCTeAnteriorFilialEmissora = servicoOrquestradorFila.Ordenar((limiteRegistros) => repCarga.BuscaCargasFilialEmissoraGerarCTeAnterior(limiteRegistros));

            for (var i = 0; i < cargaAgGerarCTeAnteriorFilialEmissora.Count; i++)
            {
                int codigoCarga = cargaAgGerarCTeAnteriorFilialEmissora[i];

                try
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                    serFilialEmissora.GerarCTesAnterioresDaFilialEmissora(carga, tipoServicoMultisoftware, configuracao, unitOfWork);
                    servicoOrquestradorFila.RegistroLiberadoComSucesso(codigoCarga);
                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao);
                    servicoOrquestradorFila.RegistroComFalha(codigoCarga, excecao.Message);
                }
            }

        }

        private void VerificarCargasGerarCTeAnteriorEmitirDocumentoSempreOrigemDestinoPedido(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Servicos.Embarcador.Carga.FilialEmissora serFilialEmissora = new Servicos.Embarcador.Carga.FilialEmissora();
            Servicos.Global.OrquestradorFila servicoOrquestradorFila = new Servicos.Global.OrquestradorFila(unitOfWork, IdentificadorControlePosicaoThread.VerificarCargasGerarCTeAnteriorEmitirDocumentoSempreOrigemDestinoPedido);
            List<int> cargaAgGerarCTeAnterior = servicoOrquestradorFila.Ordenar((limiteRegistros) => repCarga.BuscaCargasGerarCTeAnteriorEmitirDocumentoSempreOrigemDestinoPedido(limiteRegistros));

            for (var i = 0; i < cargaAgGerarCTeAnterior.Count; i++)
            {
                int codigoCarga = cargaAgGerarCTeAnterior[i];

                try
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                    serFilialEmissora.GerarCTesAnterioresDaFilialEmissora(carga, tipoServicoMultisoftware, configuracao, unitOfWork);
                    servicoOrquestradorFila.RegistroLiberadoComSucesso(codigoCarga);
                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao);
                    servicoOrquestradorFila.RegistroComFalha(codigoCarga, excecao.Message);
                }
            }
        }

        private void GerarIntegracoesNotaFiscalEletronica(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao)
        {
            Servicos.Embarcador.Integracao.IntegracaoNotaFiscalEletronica servicoIntegracaoNotaFiscalEletronica = new Servicos.Embarcador.Integracao.IntegracaoNotaFiscalEletronica(unitOfWork);

            Repositorio.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao repNotaFiscalEletronicaIntegracao = new Repositorio.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao(unitOfWork);
            List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal> notasFiscaisParaIntegracao = repNotaFiscalEletronicaIntegracao.BuscarNotasFiscaisProntasParaIntegracao();

            foreach (Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal notaFiscal in notasFiscaisParaIntegracao)
            {
                servicoIntegracaoNotaFiscalEletronica.IniciarIntegracoesNotaFiscalEletronica(notaFiscal);
            }


            servicoIntegracaoNotaFiscalEletronica.VerificarIntegracoesPendentesNotaFiscalEletronica();
        }

        private void GerarIntegracoesNFSManualCancelamento(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao)
        {
            Servicos.Embarcador.Integracao.IntegracaoNFSManualCancelamento servicoIntegracaoNFSManualCancelamento = new Servicos.Embarcador.Integracao.IntegracaoNFSManualCancelamento();

            Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracao repNFSManualCancelamentoIntegracao = new Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracao(unitOfWork);
            Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe repNFSManualCancelamentoIntegracaoCTe = new Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe(unitOfWork);
            List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento> nfsManualCancelamentoParaIntegracao = repNFSManualCancelamentoIntegracao.BuscarNFSManuaisCanceladasProntasParaIntegracao();
            List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe> nfsManualCancelamentoJaIntegradas = repNFSManualCancelamentoIntegracaoCTe.BuscarTodos();
            List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento> nfsManualCancelamentoQueSeraoEnviadasParaIntegracao = nfsManualCancelamentoParaIntegracao.
                                                                                                                               Where(o => !nfsManualCancelamentoJaIntegradas.
                                                                                                                               Any(p => p.NFSManualCancelamento.Codigo == o.Codigo)).
                                                                                                                               ToList();

            foreach (Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento nfsManualCancelada in nfsManualCancelamentoQueSeraoEnviadasParaIntegracao)
            {
                servicoIntegracaoNFSManualCancelamento.AdicionarNFSManualCancelamentoParaIntegracao(nfsManualCancelada, unitOfWork);
            }
        }
    }
}