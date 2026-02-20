using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Servicos.Embarcador.Carga
{
    public class Cancelamento
    {
        #region Métodos Públicos

        public static Dominio.Entidades.Embarcador.Cargas.CargaCancelamento GerarCargaCancelamento(Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar cargaCancelamentoAdicionar, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamento repositorioCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repositorioCargaCancelamento.BuscarPorCarga(cargaCancelamentoAdicionar.Carga.Codigo);

            if (cargaCancelamento != null)
            {
                cargaCancelamento.DataCancelamento = DateTime.Now;
                cargaCancelamento.MensagemRejeicaoCancelamento = "";
                cargaCancelamento.MotivoCancelamento = cargaCancelamentoAdicionar.MotivoCancelamento;
                cargaCancelamento.Usuario = cargaCancelamentoAdicionar.Usuario;
                cargaCancelamento.UsuarioERPSolicitouCancelamento = cargaCancelamentoAdicionar.UsuarioERPSolicitouCancelamento;
                cargaCancelamento.DuplicarCarga = cargaCancelamentoAdicionar.DuplicarCarga;
                cargaCancelamento.ControleIntegracaoEmbarcador = cargaCancelamentoAdicionar?.ControleIntegracaoEmbarcador ?? string.Empty;

                if (cargaCancelamentoAdicionar.LiberarPedidosParaMontagemCarga || (configuracao.LiberarPedidosParaMontagemCargaCancelada && (cargaCancelamento.Carga.Carregamento != null || cargaCancelamento.Carga.CargaGeradaViaDocumentoTransporte) && !cargaCancelamento.Carga.CargaDePreCargaFechada && !cargaCancelamento.Carga.CargaDePreCargaEmFechamento))
                    cargaCancelamento.LiberarPedidosParaMontagemCarga = true;

                if (cargaCancelamentoAdicionar.DefinirSituacaoEmCancelamento)
                {
                    cargaCancelamento.Situacao = SituacaoCancelamentoCarga.EmCancelamento;
                    cargaCancelamento.EnviouCTesParaCancelamento = false;
                    cargaCancelamento.EnviouMDFesParaCancelamento = false;

                }

                if (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.RejeicaoCancelamento)
                    cargaCancelamento.Situacao = SituacaoCancelamentoCarga.AgConfirmacao;

                repositorioCargaCancelamento.Atualizar(cargaCancelamento);
            }
            else
            {
                cargaCancelamento = new Dominio.Entidades.Embarcador.Cargas.CargaCancelamento();
                cargaCancelamento.Carga = cargaCancelamentoAdicionar.Carga;
                cargaCancelamento.DataCancelamento = DateTime.Now;
                cargaCancelamento.MensagemRejeicaoCancelamento = "";
                cargaCancelamento.MotivoCancelamento = cargaCancelamentoAdicionar.MotivoCancelamento;
                cargaCancelamento.UsuarioERPSolicitouCancelamento = cargaCancelamentoAdicionar.UsuarioERPSolicitouCancelamento;
                cargaCancelamento.EnviouAverbacoesCTesParaCancelamento = true;
                cargaCancelamento.SituacaoCargaNoCancelamento = cargaCancelamentoAdicionar.Carga.SituacaoCarga;
                cargaCancelamento.Situacao = SituacaoCancelamentoCarga.AgConfirmacao;
                cargaCancelamento.Tipo = TipoCancelamentoCarga.Cancelamento;
                cargaCancelamento.Usuario = cargaCancelamentoAdicionar.Usuario;
                cargaCancelamento.DuplicarCarga = cargaCancelamentoAdicionar.DuplicarCarga;
                cargaCancelamento.ControleIntegracaoEmbarcador = cargaCancelamentoAdicionar?.ControleIntegracaoEmbarcador ?? string.Empty;
                cargaCancelamento.confirmacaoERP = (cargaCancelamento.Usuario == null);

                if (cargaCancelamentoAdicionar.LiberarPedidosParaMontagemCarga || (configuracao.LiberarPedidosParaMontagemCargaCancelada && (cargaCancelamento.Carga.Carregamento != null || cargaCancelamento.Carga.CargaGeradaViaDocumentoTransporte) && !cargaCancelamento.Carga.CargaDePreCargaFechada && !cargaCancelamento.Carga.CargaDePreCargaEmFechamento))
                    cargaCancelamento.LiberarPedidosParaMontagemCarga = true;

                if (cargaCancelamentoAdicionar.DefinirSituacaoEmCancelamento)
                    cargaCancelamento.Situacao = SituacaoCancelamentoCarga.EmCancelamento;

                repositorioCargaCancelamento.Inserir(cargaCancelamento);

                if (cargaCancelamentoAdicionar.GerarIntegracoes)
                    Cancelamento.GerarIntegracoesCancelamento(ref cargaCancelamento, cargaCancelamentoAdicionar.TipoServicoMultisoftware, unitOfWork);
            }

            return cargaCancelamento;
        }

        public static void SolicitarCancelamentoCarga(ref Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool enviarNotificacaoHUB = true)
        {
            SolicitarCancelamentoCarga(ref cargaCancelamento, unitOfWork, stringConexao, tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Cancelado, enviarNotificacaoHUB);
        }

        public static void SolicitarCancelamentoCarga(ref Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento situacaoCarregamento, bool enviarNotificacaoHUB = true)
        {
            Repositorio.AverbacaoMDFe repAverbacaoMDFe = new Repositorio.AverbacaoMDFe(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiro repCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotasFiscais = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComplementoFrete repCargaComplementoFrete = new Repositorio.Embarcador.Cargas.CargaComplementoFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repConhecimentoDeTransporteEletronico = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoMDFe repCargaPedidoDocumentoMDFe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoMDFe(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial repCargaPedidoXMLNotaFiscalParcial = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido repSessaoRoteirizadorPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscalCarga = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);

            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
            Servicos.NFSe svcNFSe = new Servicos.NFSe(unitOfWork);
            Servicos.MDFe svcMDFe = new Servicos.MDFe(unitOfWork);
            Carga svcCarga = new Carga(unitOfWork);
            Servicos.Embarcador.Frete.ContratoFreteCliente.ContratoFreteCliente svcContratoFreteCliente = new Servicos.Embarcador.Frete.ContratoFreteCliente.ContratoFreteCliente(unitOfWork);
            Carga serCarga = new Carga(unitOfWork);
            ComplementoFrete serCargaComplementoFrete = new ComplementoFrete(unitOfWork);
            Canhotos.Canhoto serCanhoto = new Canhotos.Canhoto(unitOfWork);
            Credito.CreditoMovimentacao serCreditoMovimentacao = new Credito.CreditoMovimentacao(unitOfWork);
            Hubs.Carga svcHubCarga = new Hubs.Carga();
            Logistica.CargaJanelaCarregamentoPrioridade servicoCargaJanelaCarregamentoPrioridade = new Logistica.CargaJanelaCarregamentoPrioridade(unitOfWork);
            Pedido.ColetaContainer servicoColetaContainer = new Pedido.ColetaContainer(unitOfWork);
            MontagemCarga.MontagemCarga servicoMontagemCarga = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

            cargaCancelamento.SituacaoCargaNoCancelamento = cargaCancelamento.Carga.SituacaoCarga;
            int codigoCargaCancelamento = cargaCancelamento.Carga.Codigo;
            bool cargaComQuebraContainer = cargaCancelamento.Carga?.CargaComQuebraDeContainer ?? false;

            if (!cargaComQuebraContainer && Carga.IsCargaBloqueada(cargaCancelamento.Carga, unitOfWork))
            {
                cargaCancelamento.MensagemRejeicaoCancelamento = "A carga está bloqueada e não permite alteração.";
                cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;

                repCargaCancelamento.Atualizar(cargaCancelamento);

                if (enviarNotificacaoHUB)
                    svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                return;
            }

            if (!cargaComQuebraContainer && cargaCancelamento.Carga.CargaMDFes != null && cargaCancelamento.Carga.CargaMDFes.Count > 0 && cargaCancelamento.Carga.CargaMDFes.Any(cargaMDFe => cargaMDFe.MDFe != null && cargaMDFe.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe && cargaMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado && cargaMDFe.MDFe.DataAutorizacao < DateTime.Now.AddDays(-1)))
            {
                cargaCancelamento.MensagemRejeicaoCancelamento = "Não é possível cancelar MDF-e(s) que foram emitidos a mais de 1 dia.";
                cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;

                repCargaCancelamento.Atualizar(cargaCancelamento);

                if (enviarNotificacaoHUB)
                    svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                return;
            }

            if (!cargaComQuebraContainer && cargaCancelamento.Carga.CargaAgrupamento != null)
            {
                cargaCancelamento.MensagemRejeicaoCancelamento = "A carga " + cargaCancelamento.Carga.CodigoCargaEmbarcador + " está agrupada na carga " + cargaCancelamento.Carga.CargaAgrupamento.CodigoCargaEmbarcador + ", para adicioná-la novamente é necessário cancelar toda a carga de agrupamento.";
                cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;

                repCargaCancelamento.Atualizar(cargaCancelamento);

                if (enviarNotificacaoHUB)
                    svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                return;
            }

            if (!cargaComQuebraContainer && cargaCancelamento.Carga.Ocorrencias != null &&
                cargaCancelamento.Carga.Ocorrencias.Count > 0 &&
                cargaCancelamento.Carga.Ocorrencias.Any(o => o.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Cancelada &&
                                                    o.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.RejeitadaEtapaEmissao &&
                                                    o.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Rejeitada &&
                                                    !o.TipoOcorrencia.TipoOcorrenciaControleEntrega &&
                                                    o.ValorOcorrencia != 0))
            {
                cargaCancelamento.MensagemRejeicaoCancelamento = "Existem ocorrências não canceladas para esta carga, não sendo possível cancelar a mesma.";
                cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;

                repCargaCancelamento.Atualizar(cargaCancelamento);

                if (enviarNotificacaoHUB)
                    svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                return;
            }

            Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao = null;
            if (!cargaCancelamento.Carga.CargaAgrupada)
                documentoProvisao = repDocumentoProvisao.BuscarDocumentoCargaEmFechamento(cargaCancelamento.Carga.Codigo);
            else
                documentoProvisao = repDocumentoProvisao.BuscarDocumentoCargaAgrupamentoEmFechamento(cargaCancelamento.Carga.Codigo);

            if (!cargaComQuebraContainer && documentoProvisao != null)
            {
                cargaCancelamento.MensagemRejeicaoCancelamento = "Antes de cancelar a carga é necessário finalizar a provisão de seus documento, a carga está sendo provisionada na provisão de número " + documentoProvisao.Provisao.Numero + ".";
                cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;

                repCargaCancelamento.Atualizar(cargaCancelamento);

                if (enviarNotificacaoHUB)
                    svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                return;
            }

            Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = null;
            if (!cargaCancelamento.Carga.CargaAgrupada)
                documentoFaturamento = repDocumentoFaturamento.BuscarDocumentoCargaEmFechamento(cargaCancelamento.Carga.Codigo);
            else
                documentoFaturamento = repDocumentoFaturamento.BuscarDocumentoCargaAgrupamentoEmFechamento(cargaCancelamento.Carga.Codigo);

            if (!cargaComQuebraContainer && documentoFaturamento != null)
            {
                cargaCancelamento.MensagemRejeicaoCancelamento = "Antes de cancelar a carga é necessário finalizar a pagamento, a carga está sendo finalizada no pagamento de número " + documentoFaturamento.Pagamento.Numero + ".";
                cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;

                repCargaCancelamento.Atualizar(cargaCancelamento);

                if (enviarNotificacaoHUB)
                    svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                return;
            }

            bool liquidada = false;
            if (!cargaCancelamento.Carga.CargaAgrupada)
                liquidada = repDocumentoFaturamento.ExisteDocumentoPagoPorCarga(cargaCancelamento.Carga.Codigo);
            else
                liquidada = repDocumentoFaturamento.ExisteDocumentoPagoPorCargaAgrupada(cargaCancelamento.Carga.Codigo);

            if (!cargaComQuebraContainer && liquidada)
            {
                cargaCancelamento.MensagemRejeicaoCancelamento = "Não é possível cancelar a carga pois seus documentos já foram pagos.";
                cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;

                repCargaCancelamento.Atualizar(cargaCancelamento);

                if (enviarNotificacaoHUB)
                    svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                return;
            }

            string erro = string.Empty;

            if (!cargaComQuebraContainer && !ValidarFechamentoDiarioDocumentosCarga(out erro, cargaCancelamento, unitOfWork, tipoServicoMultisoftware))
            {
                cargaCancelamento.MensagemRejeicaoCancelamento = erro;
                cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;

                repCargaCancelamento.Atualizar(cargaCancelamento);

                if (enviarNotificacaoHUB)
                    svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                return;
            }

            if (!ValidarSeCargaEstaVinculadaAAlgumDocumento(cargaCancelamento.Carga, unitOfWork, out erro, out List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulosParaCancelamento, tipoServicoMultisoftware) && !cargaComQuebraContainer)
            {
                cargaCancelamento.MensagemRejeicaoCancelamento = erro;
                cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;

                repCargaCancelamento.Atualizar(cargaCancelamento);

                if (enviarNotificacaoHUB)
                    svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                return;
            }

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCtes;

            if (cargaCancelamento.CTe != null && (cargaCancelamento.Carga.TipoOperacao?.ConfiguracaoCarga?.TipoCancelamentoCargaDocumento ?? TipoCancelamentoCargaDocumento.Carga) == TipoCancelamentoCargaDocumento.Documentos)
                cargaCtes = repCargaCTe.BuscarPorCarga(cargaCancelamento.Carga.Codigo, false, false, false, false, false, 0, 0, false, false, 0, cargaCancelamento.CTe.Codigo);
            else
                cargaCtes = repCargaCTe.BuscarPorCarga(cargaCancelamento.Carga.Codigo);

            if (!cargaComQuebraContainer && !cargaCancelamento.Carga.CargaTransbordo)// se é uma carga de transbordo os CT-es não podem ser cancelados nesta carga, e sim na carga original.
            {
                if (cargaCtes.Any(cargaCTe => cargaCTe.CTe != null && cargaCTe.SistemaEmissor == SistemaEmissor.MultiCTe && cargaCTe.CTe.ModeloDocumentoFiscal.Numero == "57" && cargaCTe.CTe.Status == "A"
                    && cargaCTe.CTe.DataRetornoSefaz < DateTime.Now.AddDays(-7) && cargaCTe.CargaCTeTrechoAnterior == null))
                {
                    cargaCancelamento.MensagemRejeicaoCancelamento = "Não é possível cancelar CT-e(s) que foram emitidos a mais de 7 dias.";
                    cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;

                    repCargaCancelamento.Atualizar(cargaCancelamento);

                    if (enviarNotificacaoHUB)
                        svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                    return;
                }

                if (cargaCtes.Any(cargaCTe => cargaCTe.CTe != null && cargaCTe.SistemaEmissor != SistemaEmissor.MultiCTe && cargaCTe.CTe.SituacaoCTeSefaz == SituacaoCTeSefaz.Autorizada)
                    && ((!cargaCancelamento.CancelarDocumentosEmitidosNoEmbarcador && tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                       || cargaCancelamento.CancelarDocumentosEmitidosNoEmbarcador && tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS))
                {
                    cargaCancelamento.MensagemRejeicaoCancelamento = "É necessário importar o XML de cancelamento do embarcador para finalizar o cancelamento da carga.";
                    cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;

                    repCargaCancelamento.Atualizar(cargaCancelamento);

                    if (enviarNotificacaoHUB)
                        svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                    return;
                }
            }

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

            Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitorametno = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = repConfiguracaoMonitorametno.BuscarConfiguracaoPadrao();

            int segundos = -configuracao.TempoSegundosParaInicioEmissaoDocumentos;

            if (!cargaComQuebraContainer && (cargaCancelamento.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete
                && cargaCancelamento.Carga.ExigeNotaFiscalParaCalcularFrete
                            && (cargaCancelamento.Carga.DataEnvioUltimaNFe.HasValue && cargaCancelamento.Carga.DataEnvioUltimaNFe.Value <= DateTime.Now.AddSeconds(segundos))
                            && !cargaCancelamento.Carga.AguardandoEmissaoDocumentoAnterior && !cargaCancelamento.Carga.CalculandoFrete && cargaCancelamento.Carga.CargaFechada
                            && !cargaCancelamento.Carga.PendenteGerarCargaDistribuidor
                            && !cargaCancelamento.Carga.AgValorRedespacho)
                            || (cargaCancelamento.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe
                            && !cargaCancelamento.Carga.ExigeNotaFiscalParaCalcularFrete && (cargaCancelamento.Carga.DataEnvioUltimaNFe.HasValue
                            && cargaCancelamento.Carga.DataEnvioUltimaNFe.Value <= DateTime.Now.AddSeconds(segundos))
                            && !cargaCancelamento.Carga.AguardandoEmissaoDocumentoAnterior
                            && !cargaCancelamento.Carga.AgValorRedespacho
                            && !cargaCancelamento.Carga.PendenteGerarCargaDistribuidor
                            && !cargaCancelamento.Carga.AguardarIntegracaoEtapaTransportador)
                            || (cargaCancelamento.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos
                             && cargaCancelamento.Carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora
                             && cargaCancelamento.Carga.EmEmissaoCTeSubContratacaoFilialEmissora
                             && !cargaCancelamento.Carga.PossuiPendencia
                             && !cargaCancelamento.Carga.AgValorRedespacho)
                            )
            {
                cargaCancelamento.MensagemRejeicaoCancelamento = "Não é possível cancelar a carga os documentos estão sendo gerados, tente novamente em instantes.";
                cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;

                repCargaCancelamento.Atualizar(cargaCancelamento);

                if (enviarNotificacaoHUB)
                    svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                return;
            }

            if (!cargaComQuebraContainer && cargaCancelamento.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao
                && cargaCancelamento.Carga.GerandoIntegracoes)
            {
                cargaCancelamento.MensagemRejeicaoCancelamento = "Não é possível cancelar a carga enquanto a integração está sendo realizada, tente novamente em instantes.";
                cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;

                repCargaCancelamento.Atualizar(cargaCancelamento);

                if (enviarNotificacaoHUB)
                    svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                return;
            }

            if (!cargaComQuebraContainer && cargaCancelamento.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos &&
                                       !cargaCancelamento.Carga.PossuiPendencia &&
                                       !cargaCancelamento.Carga.AgImportacaoCTe &&
                                       !cargaCancelamento.Carga.EmEmissaoCTeSubContratacaoFilialEmissora &&
                                       !cargaCancelamento.Carga.AgGeracaoCTesAnteriorFilialEmissora &&
                                       !cargaCancelamento.Carga.AgImportacaoMDFe &&
                                       !cargaCancelamento.Carga.CTesEmDigitacao &&
                                       !cargaCancelamento.Carga.IntegrandoValePedagio &&
                                       !cargaCancelamento.Carga.EmitindoCTes &&
                                       !cargaCancelamento.Carga.FinalizandoProcessoEmissao)
            {
                cargaCancelamento.MensagemRejeicaoCancelamento = "Não é possível cancelar a carga os documentos estão sendo emitidos, tente novamente em instantes.";
                cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;

                repCargaCancelamento.Atualizar(cargaCancelamento);

                if (enviarNotificacaoHUB)
                    svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                return;
            }

            if (!cargaComQuebraContainer && cargaCancelamento.Carga.CalculandoFrete && !repCargaPedido.VerificarPorCargaSePendenteDadosRecebedor(cargaCancelamento.Carga.Codigo))
            {
                if (!configuracao.ExigirCargaRoteirizada ||
                    ((configuracao.ExigirCargaRoteirizada || (cargaCancelamento.Carga.TipoOperacao?.ExigirCargaRoteirizada ?? false)) && (cargaCancelamento.Carga.SituacaoRoteirizacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Concluido || cargaCancelamento.Carga.SituacaoRoteirizacaoCarga == SituacaoRoteirizacao.Erro || cargaCancelamento.Carga.SituacaoRoteirizacaoCarga == SituacaoRoteirizacao.SemDefinicao)))
                {
                    cargaCancelamento.MensagemRejeicaoCancelamento = "Não é possível cancelar a carga enquanto o seu frete está sendo calculado, tente novamente em instantes.";
                    cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;

                    repCargaCancelamento.Atualizar(cargaCancelamento);

                    if (enviarNotificacaoHUB)
                        svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                    return;
                }
            }

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(cargaCancelamento.Carga.Codigo);

            if (!cargaComQuebraContainer && cargaCancelamento.Tipo == TipoCancelamentoCarga.Cancelamento)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    if (cargaPedido.CargaPedidoProximoTrecho != null && !cargaPedido.CargaPedidoFilialEmissora)
                    {
                        if (cargaPedido.CargaPedidoProximoTrecho.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && cargaPedido.CargaPedidoProximoTrecho.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                        {
                            //processo da Arcelor onde a carga de redespacho é gerada automaticamente e ao cancelar deve ser cancelada junto a carga original
                            if (cargaCancelamento.Carga.TipoOperacao?.GerarRedespachoParaOutrasEtapasCarregamento ?? false)
                            {
                                cargaPedido.CargaPedidoProximoTrecho.Carga.SituacaoCarga = SituacaoCarga.Cancelada;
                                cargaPedido.CargaPedidoProximoTrecho.Carga.CargaFechada = false;
                                repCarga.Atualizar(cargaPedido.CargaPedidoProximoTrecho.Carga);
                                return;
                            }
                            else
                            {
                                cargaCancelamento.MensagemRejeicaoCancelamento = "Esta carga possui uma carga de redespacho não cancelada, para poder cancelar essa carga primeiro é necessário cancelar a carga de redespacho número " + cargaPedido.CargaPedidoProximoTrecho.Carga.CodigoCargaEmbarcador + ". ";
                                cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;
                                repCargaCancelamento.Atualizar(cargaCancelamento);

                                if (enviarNotificacaoHUB)
                                    svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                                return;
                            }
                        }
                    }
                }
            }

            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessaoRoteirizador = null;
            bool montagemCarregamentoPedidoProduto = false;

            if (cargaCancelamento.Carga.Carregamento != null && cargaCancelamento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento)
            {
                montagemCarregamentoPedidoProduto = (cargaCancelamento.Carga?.Carregamento?.SessaoRoteirizador?.MontagemCarregamentoPedidoProduto ?? false);

                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoCarga repCarregamentoCarga = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoCarga(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoCarga> carregamentoCargas = repCarregamentoCarga.BuscarPorCarregamento(cargaCancelamento.Carga.Carregamento.Codigo);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoCarga carregamentoCargaExcluir = null;
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoCarga> carregamentoCargasRecalcuar = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoCarga>();

                foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoCarga carregamentoCarga in carregamentoCargas)
                {
                    if (carregamentoCarga.Carga.Codigo != cargaCancelamento.Carga.Codigo)
                    {
                        if (serCarga.VerificarSeCargaEstaNaLogistica(carregamentoCarga.Carga, tipoServicoMultisoftware))
                        {
                            if (carregamentoCarga.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                                carregamentoCargasRecalcuar.Add(carregamentoCarga);
                        }
                        else
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamentoMontagem = repCargaCancelamento.BuscarPorCarga(carregamentoCarga.Carga.Codigo);
                            if (cargaCancelamentoMontagem == null)
                            {
                                cargaCancelamento.MensagemRejeicaoCancelamento = "Não é possível solicitar o cancelamento desta carga pois a mesma está atrelada ao carregamento " + carregamentoCarga.Carregamento.NumeroCarregamento + " que está com a carga " + carregamentoCarga.Carga.CodigoCargaEmbarcador + " em processo de emissão dos documentos, para poder cancelar essa carga é necessário cancelar a carga " + cargaCancelamento.Carga.CodigoCargaEmbarcador + " primeiro. ";
                                cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;
                                repCargaCancelamento.Atualizar(cargaCancelamento);

                                if (enviarNotificacaoHUB)
                                    svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                                return;
                            }
                            else
                            {
                                if (cargaCancelamentoMontagem.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento)
                                {
                                    cargaCancelamentoMontagem.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.EmCancelamento;
                                    repCargaCancelamento.Atualizar(cargaCancelamentoMontagem);
                                }
                            }
                        }
                    }
                    else
                        carregamentoCargaExcluir = carregamentoCarga;
                }

                // #16252 - Solicitação de retirada da regra, não de acordo .. pois é um controle que foi implementado para que
                // pedidos contidos em uma sessão "INICIADA" a mesma não pode ser cancelada. No cenário apresentado todos os pedidos estavam com cargas geradas
                // sendo necessário apenas o usuário "FINALIZAR A SESSÃO", onde os pedidos/cargas seria liberados para cancelamentos. conforme descrito na tarefa e anexado imagem.

                // Se a carga pertence a um carregamento de origem de sessão, a mesma não pode estar com situação INICIADA. 
                //if (cargaCancelamento.Carga.Carregamento.SessaoRoteirizador != null)
                //{
                //    if (cargaCancelamento.Carga.Carregamento.SessaoRoteirizador.SituacaoSessaoRoteirizador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizador.Iniciada)
                //    {
                //        cargaCancelamento.MensagemRejeicaoCancelamento = "Não é possível solicitar o cancelamento desta carga pois a mesma pertence a sessão de roteirização " + cargaCancelamento.Carga.Carregamento.SessaoRoteirizador.Codigo.ToString() + " e a mesma encontra-se com a situação INICIADA.";
                //        cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;
                //        repCargaCancelamento.Atualizar(cargaCancelamento);
                //        svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);
                //        return;
                //    }
                //}

                sessaoRoteirizador = cargaCancelamento.Carga.Carregamento.SessaoRoteirizador;

                foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoCarga carregamentoRecalcular in carregamentoCargasRecalcuar)
                {
                    carregamentoRecalcular.Carga.DataInicioCalculoFrete = DateTime.Now;
                    carregamentoRecalcular.Carga.CalculandoFrete = true;
                    repCarga.Atualizar(carregamentoRecalcular.Carga);
                }

                int codigoCarregamento = cargaCancelamento.Carga.Carregamento.Codigo;
                cargaCancelamento.Carga.Carregamento = null;
                repCarga.Atualizar(cargaCancelamento.Carga);
                if (carregamentoCargaExcluir != null)
                    repCarregamentoCarga.Deletar(carregamentoCargaExcluir);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = repCarregamento.BuscarPorCodigo(codigoCarregamento);
                if (carregamento != null)
                    servicoMontagemCarga.AjustarCarregamento(carregamento, situacaoCarregamento);
            }

            if (cargaCancelamento.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento)
            {
                if (enviarNotificacaoHUB)
                    svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                return;
            }

            CancelarCIOTCargaCancelamento(cargaCancelamento, unitOfWork, tipoServicoMultisoftware, new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado());
            if (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.RejeicaoCancelamento)
            {
                if (enviarNotificacaoHUB)
                    svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);
                return;
            }

            bool mdfeLiberados = true,
                 cteLiberados = true,
                 averbacoesMDFeLiberadas = true;

            if (!cargaComQuebraContainer)
            {
                if (cargaCancelamento.EnviouMDFesParaCancelamento)
                {
                    if (cargaCancelamento.Carga.CargaMDFes.Any(o => o.MDFe != null && o.MDFe.Status == Dominio.Enumeradores.StatusMDFe.EmCancelamento))
                        mdfeLiberados = false;
                    else if (cargaCancelamento.Carga.CargaMDFes.Any(o => o.MDFe != null && o.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe && o.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado && o.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado && o.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Rejeicao))
                    {
                        cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;
                        cargaCancelamento.MensagemRejeicaoCancelamento = "Não foi possível cancelar todos os MDF-es.";

                        repCargaCancelamento.Atualizar(cargaCancelamento);

                        if (enviarNotificacaoHUB)
                            svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                        return;
                    }
                }
                else
                {
                    if (cargaCancelamento.Carga.CargaMDFes != null && cargaCancelamento.Carga.CargaMDFes.Count > 0)
                    {
                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe in cargaCancelamento.Carga.CargaMDFes)
                        {
                            if (cargaMDFe.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe)
                            {
                                if (cargaMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado)
                                {
                                    if (!Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(cargaMDFe.MDFe.SistemaEmissor).CancelarMdfe(cargaMDFe.MDFe.Codigo, cargaMDFe.MDFe.Empresa.Codigo, cargaCancelamento.MotivoCancelamento, unitOfWork))
                                    {
                                        cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;
                                        cargaCancelamento.MensagemRejeicaoCancelamento = "Não foi possível cancelar o MDF-e " + cargaMDFe.MDFe.Numero + ".";

                                        repCargaCancelamento.Atualizar(cargaCancelamento);

                                        if (enviarNotificacaoHUB)
                                            svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                                        return;
                                    }

                                    mdfeLiberados = false;
                                }
                                else if (cargaMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.EmCancelamento)
                                {
                                    mdfeLiberados = false;
                                }
                                else if (cargaMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Enviado || cargaMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Pendente || cargaMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.EmEncerramento)
                                {
                                    cargaCancelamento.MensagemRejeicaoCancelamento = "Não é possível cancelar a carga enquanto existirem MDF-e(s) na situação de Enviado, Pendente ou Em Encerramento.";
                                    cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;

                                    repCargaCancelamento.Atualizar(cargaCancelamento);

                                    if (enviarNotificacaoHUB)
                                        svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                                    return;
                                }
                            }
                        }
                    }

                    cargaCancelamento.EnviouMDFesParaCancelamento = true;
                    repCargaCancelamento.Atualizar(cargaCancelamento);

                    if (enviarNotificacaoHUB)
                        svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);
                }
            }

            if (!mdfeLiberados)
                return;

            if (!cargaComQuebraContainer && !cargaCancelamento.LiberarCancelamentoComAverbacaoMDFeRejeitada && repAverbacaoMDFe.ExistePorCargaEStatus(cargaCancelamento.Carga.Codigo, Dominio.Enumeradores.StatusAverbacaoMDFe.Sucesso))
            {
                cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;
                cargaCancelamento.MensagemRejeicaoCancelamento = "Não foi possível cancelar todas as averbações dos MDF-es.";

                repCargaCancelamento.Atualizar(cargaCancelamento);

                if (enviarNotificacaoHUB)
                    svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                return;
            }

            if (repAverbacaoMDFe.ExistePorCargaEStatus(cargaCancelamento.Carga.Codigo, Dominio.Enumeradores.StatusAverbacaoMDFe.AgCancelamento))
                averbacoesMDFeLiberadas = false;

            if (!averbacoesMDFeLiberadas)
                return;

            if (!cargaComQuebraContainer && !cargaCancelamento.Carga.CargaTransbordo)// se é uma carga de transbordo os CT-es não podem ser cancelados nesta carga, e sim na carga original.
            {
                if (cargaCancelamento.EnviouCTesParaCancelamento)
                {
                    if (cargaCtes.Any(o => o.CTe != null && (o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe || o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe) && (o.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmCancelamento || o.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmInutilizacao)))
                        cteLiberados = false;
                    else
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesValidar = cargaCtes;
                        if (!cargaCancelamento.CancelarDocumentosEmitidosNoEmbarcador)
                            cargaCTesValidar = cargaCtes.Where(obj => obj.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe).ToList();


                        if (cargaCTesValidar.Any(o => o.CTe != null && (o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe || o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                        && o.CTe.SituacaoCTeSefaz != SituacaoCTeSefaz.Cancelada && o.CTe.SituacaoCTeSefaz != SituacaoCTeSefaz.Inutilizada && o.CTe.SituacaoCTeSefaz != SituacaoCTeSefaz.Denegada && o.CargaCTeTrechoAnterior == null))
                        {
                            cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;
                            cargaCancelamento.MensagemRejeicaoCancelamento = "Não foi possível cancelar/inutilizar todos os CT-es.";

                            repCargaCancelamento.Atualizar(cargaCancelamento);

                            if (enviarNotificacaoHUB)
                                svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                            return;
                        }

                    }
                }
                else
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCtes)
                    {

                        if (cargaCTe.SistemaEmissor != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe || cargaCTe.CargaCTeComplementoInfo != null)
                        {
                            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                                continue;
                            else
                            {

                                cargaCancelamento.EnviouCTesParaCancelamento = true;
                                cargaCancelamento.DataEnvioCancelamento = DateTime.Now;
                                repCargaCancelamento.Atualizar(cargaCancelamento);
                                cteLiberados = false;

                                if (enviarNotificacaoHUB)
                                    svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                                continue;
                            }
                        }

                        if (cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe && cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe)
                        {
                            DateTime dataCancelamento = DateTime.Now;
                            if (cargaCTe.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Rejeitada ||
                                cargaCTe.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmDigitacao)
                                cargaCTe.CTe.Status = "I";
                            else
                                cargaCTe.CTe.Status = "C";

                            cargaCTe.CTe.DataRetornoSefaz = dataCancelamento;
                            cargaCTe.CTe.DataCancelamento = dataCancelamento;
                            cargaCTe.CTe.ObservacaoCancelamento = cargaCancelamento.MotivoCancelamento;

                            repConhecimentoDeTransporteEletronico.Atualizar(cargaCTe.CTe);

                            svcCTe.AjustarAverbacoesParaCancelamento(cargaCTe.CTe.Codigo, unitOfWork);

                            continue;
                        }

                        if (cargaCTe.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Autorizada && cargaCTe.CargaCTeTrechoAnterior == null)
                        {
                            if (cargaCTe.CTe.ModeloDocumentoFiscal.Numero == "57")
                            {
                                if (!Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoCTe(cargaCTe.CTe.SistemaEmissor).CancelarCte(cargaCTe.CTe.Codigo, cargaCTe.CTe.Empresa.Codigo, cargaCancelamento.MotivoCancelamento, unitOfWork))
                                {
                                    cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;
                                    cargaCancelamento.MensagemRejeicaoCancelamento = "Não foi possível cancelar o CT-e " + cargaCTe.CTe.Numero + ".";

                                    repCargaCancelamento.Atualizar(cargaCancelamento);

                                    if (enviarNotificacaoHUB)
                                        svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                                    return;
                                }
                            }
                            else if (cargaCTe.CTe.ModeloDocumentoFiscal.Numero == "39")
                            {
                                if (!svcNFSe.CancelarNFSe(cargaCTe.CTe.Codigo, unitOfWork))
                                {
                                    cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;
                                    cargaCancelamento.MensagemRejeicaoCancelamento = "Não foi possível cancelar a NFS-e " + cargaCTe.CTe.Numero + ".";

                                    repCargaCancelamento.Atualizar(cargaCancelamento);

                                    if (enviarNotificacaoHUB)
                                        svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                                    return;
                                }
                            }

                            cteLiberados = false;
                        }
                        else if (cargaCTe.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmCancelamento ||
                                 cargaCTe.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmInutilizacao)
                        {
                            cteLiberados = false;
                        }
                        else if (cargaCTe.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Rejeitada ||
                                 cargaCTe.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmDigitacao)
                        {

                            if (cargaCTe.CTe.ModeloDocumentoFiscal.Numero == "57")
                            {
                                if (!svcCTe.Inutilizar(cargaCTe.CTe.Codigo, cargaCTe.CTe.Empresa.Codigo, cargaCancelamento.MotivoCancelamento, tipoServicoMultisoftware, unitOfWork))
                                {
                                    cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;
                                    cargaCancelamento.MensagemRejeicaoCancelamento = "Não foi possível inutilizar o CT-e " + cargaCTe.CTe.Numero + ".";

                                    repCargaCancelamento.Atualizar(cargaCancelamento);

                                    if (enviarNotificacaoHUB)
                                        svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                                    return;
                                }
                            }
                            else if (cargaCTe.CTe.ModeloDocumentoFiscal.Numero == "39")
                            {
                                cargaCTe.CTe.Status = "I";
                                cargaCTe.CTe.DataRetornoSefaz = DateTime.Now;
                                cargaCTe.CTe.ObservacaoCancelamento = cargaCancelamento.MotivoCancelamento;
                                repConhecimentoDeTransporteEletronico.Atualizar(cargaCTe.CTe);
                            }

                            cteLiberados = false;
                        }
                        else if (cargaCTe.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Pendente ||
                                 cargaCTe.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Enviada)
                        {
                            cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;
                            cargaCancelamento.MensagemRejeicaoCancelamento = "Não é possível cancelar a carga enquanto existirem CT-e(s) na situação de Enviado ou Pendente.";

                            repCargaCancelamento.Atualizar(cargaCancelamento);

                            if (enviarNotificacaoHUB)
                                svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                            return;
                        }
                    }

                    cargaCancelamento.EnviouCTesParaCancelamento = true;
                    cargaCancelamento.DataEnvioCancelamento = DateTime.Now;
                    cargaCancelamento.TentativasEnvioCancelamento += 1;

                    repCargaCancelamento.Atualizar(cargaCancelamento);

                    if (enviarNotificacaoHUB)
                        svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);
                }
            }

            if (cteLiberados && mdfeLiberados && cargaCancelamento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento)
            {
                bool averbacoesLiberadas = true;
                bool integracoesLiberadas = true;
                bool valePegadiosLiberados = true;

                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);

                if (!cargaComQuebraContainer)
                {
                    if (cargaCancelamento.EnviouValePedagiosParaCancelamento)
                    {
                        if (!cargaCancelamento.LiberarCancelamentoComValePedagioRejeitado && repCargaIntegracaoValePedagio.VerificarVPnaoCanceladoPorCarga(cargaCancelamento.Carga.Codigo, true))
                        {
                            cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;
                            cargaCancelamento.MensagemRejeicaoCancelamento = "Não foi possível cancelar o(s) Vale(s) Pedágio(s) da Carga.";

                            repCargaCancelamento.Atualizar(cargaCancelamento);

                            if (enviarNotificacaoHUB)
                                svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                            return;
                        }

                        if (repCargaIntegracaoValePedagio.VerificarEmCanceladoPorCarga(cargaCancelamento.Carga.Codigo))
                            valePegadiosLiberados = false;
                    }
                    else
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> cargaIntegracoesValePedagio = repCargaIntegracaoValePedagio.BuscarPorCarga(cargaCancelamento.Carga.Codigo, true);
                        bool enviouCancelamento = false;

                        foreach (Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagio in cargaIntegracoesValePedagio)
                        {
                            if ((cargaIntegracaoValePedagio.SituacaoValePedagio == SituacaoValePedagio.Confirmada) ||
                                (cargaIntegracaoValePedagio.SituacaoValePedagio == SituacaoValePedagio.Comprada && cargaIntegracaoValePedagio.TipoIntegracao.Tipo.IntegraCancelamentoValePedagio()))
                            {
                                enviouCancelamento = true;
                                cargaIntegracaoValePedagio.SituacaoValePedagio = SituacaoValePedagio.EmCancelamento;
                                repCargaIntegracaoValePedagio.Atualizar(cargaIntegracaoValePedagio);
                            }
                        }

                        if (enviouCancelamento)
                        {
                            cargaCancelamento.EnviouValePedagiosParaCancelamento = true;
                            repCargaCancelamento.Atualizar(cargaCancelamento);
                            valePegadiosLiberados = false;
                        }
                        else
                            valePegadiosLiberados = true;
                    }

                    if (cargaCancelamento.EnviouAverbacoesCTesParaCancelamento)
                    {
                        Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);

                        if (!cargaCancelamento.LiberarCancelamentoComAverbacaoCTeRejeitada && repAverbacaoCTe.ExistePorCargaEStatus(cargaCancelamento.Carga.Codigo, Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso))
                        {
                            cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;
                            cargaCancelamento.MensagemRejeicaoCancelamento = "Não foi possível cancelar todas as averbações dos CT-es.";

                            repCargaCancelamento.Atualizar(cargaCancelamento);

                            if (enviarNotificacaoHUB)
                                svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                            return;
                        }

                        if (repAverbacaoCTe.ExistePorCargaEStatus(cargaCancelamento.Carga.Codigo, new Dominio.Enumeradores.StatusAverbacaoCTe[] { Dominio.Enumeradores.StatusAverbacaoCTe.Enviado, Dominio.Enumeradores.StatusAverbacaoCTe.AgCancelamento }))
                            averbacoesLiberadas = false;
                    }
                    else
                    {
                        cargaCancelamento.EnviouAverbacoesCTesParaCancelamento = true;
                        repCargaCancelamento.Atualizar(cargaCancelamento);
                        averbacoesLiberadas = false;
                    }

                    if (cargaCancelamento.GerouIntegracao && cargaCancelamento.EnviouIntegracoesDeCancelamento)
                    {
                        Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(unitOfWork);
                        Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI repCargaCancelamentoIntegracaoEDI = new Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI(unitOfWork);
                        Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao repCargaCancelamentoCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao(unitOfWork);

                        List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao> situacoesIntegracaoPendente = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao>()
                    {
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao
                    };

                        if (!cargaCancelamento.LiberarCancelamentoComIntegracaoRejeitada)
                        {
                            if (repCargaCancelamentoIntegracaoEDI.ContarPorCancelamento(cargaCancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) > 0 ||
                                repCargaCancelamentoCargaIntegracao.ContarPorCargaCancelamento(cargaCancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) > 0 ||
                                repCargaCancelamentoCargaCTeIntegracao.ContarPorCargaCancelamento(cargaCancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) > 0)
                            {
                                cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;
                                cargaCancelamento.MensagemRejeicaoCancelamento = "Não foi possível enviar todas as integrações.";

                                repCargaCancelamento.Atualizar(cargaCancelamento);

                                if (enviarNotificacaoHUB)
                                    svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                                return;
                            }
                        }

                        if (repCargaCancelamentoCargaIntegracao.ContarPorCargaCancelamento(cargaCancelamento.Codigo, situacoesIntegracaoPendente.ToArray()) > 0 ||
                            repCargaCancelamentoIntegracaoEDI.ContarPorCargaCancelamento(cargaCancelamento.Codigo, situacoesIntegracaoPendente.ToArray()) > 0 ||
                            repCargaCancelamentoCargaCTeIntegracao.ContarPorCargaCancelamento(cargaCancelamento.Codigo, situacoesIntegracaoPendente.ToArray()) > 0)
                            integracoesLiberadas = false;
                    }
                    else if (cargaCancelamento.GerouIntegracao)
                    {
                        Servicos.Embarcador.Integracao.IntegracaoCargaCancelamento svcIntegracaoCargaCancelamento = new Servicos.Embarcador.Integracao.IntegracaoCargaCancelamento(unitOfWork);

                        CriarIntegracoesCargaCancelamentoCarga(cargaCancelamento, unitOfWork);
                        svcIntegracaoCargaCancelamento.IniciarIntegracoesComEDI(cargaCancelamento, unitOfWork, tipoServicoMultisoftware);
                        svcIntegracaoCargaCancelamento.IniciarIntegracoesComDocumentos(cargaCancelamento, cargaPedidos, unitOfWork);

                        cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.FinalizandoCancelamento;
                        cargaCancelamento.EnviouIntegracoesDeCancelamento = true;

                        repCargaCancelamento.Atualizar(cargaCancelamento);

                        integracoesLiberadas = false;
                    }
                }

                if (averbacoesLiberadas && integracoesLiberadas && valePegadiosLiberados)
                {
                    bool abrirTransacao = false;

                    if (!unitOfWork.IsActiveTransaction())
                    {
                        abrirTransacao = true;
                        unitOfWork.Start();
                    }

                    Servicos.Embarcador.Financeiro.Titulo.GerarCancelamentoAutomaticoTitulosEmAberto(titulosParaCancelamento, "Cancelamento do título gerado automaticamente à partir do cancelamento da carga " + cargaCancelamento.Carga.CodigoCargaEmbarcador + ".", tipoServicoMultisoftware, unitOfWork);

                    if (cargaCancelamento.DuplicarCarga)
                    {
                        Dominio.Entidades.Embarcador.Cargas.Carga cargaDuplicada = svcCarga.DuplicarCarga(cargaCancelamento.Carga.Codigo, cargaCancelamento.LiberarPedidosParaMontagemCarga, tipoServicoMultisoftware, unitOfWork);
                        if (cargaDuplicada == null)
                            throw new Exception("Não foi possível duplicar a carga.");

                        if (configuracaoGeralCarga.RecalcularFreteAoDuplicarCargaCancelamentoDocumento ?? false)
                        {
                            Servicos.Embarcador.Carga.Frete servicoCargaFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork);
                            servicoCargaFrete.RecalcularFreteTabelaFrete(cargaDuplicada, cargaDuplicada.TabelaFreteRota?.Codigo ?? 0, unitOfWork, configuracao, new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork).BuscarConfiguracaoPadrao());
                            repCarga.Atualizar(cargaDuplicada);
                        }

                        cargaCancelamento = repCargaCancelamento.BuscarPorCodigo(cargaCancelamento.Codigo);
                        cargaPedidos = repCargaPedido.BuscarPorCarga(cargaCancelamento.Carga.Codigo);
                    }


                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidoMontagemCargaAtualizar = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                    List<int> cargaPedidoXMLNotaFiscalParcials = repCargaPedidoXMLNotaFiscalParcial.BuscarCodigoNotasPorCarga(cargaCancelamento.Carga.Codigo);
                    Retornos.RetornoCarga servicoRetornoCarga = new Retornos.RetornoCarga(unitOfWork);

                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> sessaoRoteirizadorPedidos = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido>();
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidosSessaoEmMontagem = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();
                    if (sessaoRoteirizador != null)
                    {
                        List<int> codigosPedidos = (from ped in cargaPedidos select ped.Pedido.Codigo).ToList();
                        sessaoRoteirizadorPedidos = repSessaoRoteirizadorPedido.BuscarSessaoRoteirizadorPedidos(sessaoRoteirizador.Codigo, codigosPedidos);
                        carregamentoPedidosSessaoEmMontagem = repCarregamentoPedido.BuscarPorPedidosEmMontagem(sessaoRoteirizador.Codigo, codigosPedidos);
                    }

                    // Lista de produtos dos pedidos para validar o saldo ao cancelar.... #32122
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidosProdutos = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
                    if (!cargaCancelamento.LiberarPedidosParaMontagemCarga)
                    {
                        List<int> codigosPedidos = (from ped in cargaPedidos select ped.Pedido.Codigo).ToList();
                        pedidosProdutos = repPedidoProduto.BuscarPorPedidos(codigosPedidos);
                    }

                    List<int> codigosPedidoCancelarRoteirizacao = new List<int>();

                    if (!cargaComQuebraContainer)
                    {
                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                        {
                            //se for redespacho e o trecho anterior ainda não foi emitido, ajusta para buscar novamente os dados do proximo trecho
                            if (cargaPedido.CargaPedidoTrechoAnterior != null && cargaPedido.CargaPedidoTrechoAnterior.CargaPedidoFilialEmissora &&
                                  (cargaPedido.CargaPedidoTrechoAnterior.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte
                                   && cargaPedido.CargaPedidoTrechoAnterior.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada
                                   && cargaPedido.CargaPedidoTrechoAnterior.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao
                                   && cargaPedido.CargaPedidoTrechoAnterior.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos
                                   && cargaPedido.CargaPedidoTrechoAnterior.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.LiberadoPagamento))
                            {
                                if (cargaPedido.CargaPedidoTrechoAnterior.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete ||
                                    !cargaPedido.CargaPedidoTrechoAnterior.Carga.CalculandoFrete)
                                {
                                    string retornoMontagem = servicoMontagemCarga.CalcularFreteTodoCarregamento(cargaPedido.CargaPedidoTrechoAnterior.Carga);
                                    if (string.IsNullOrWhiteSpace(retornoMontagem))
                                    {
                                        cargaPedidoMontagemCargaAtualizar.Add(cargaPedido);
                                    }
                                    else
                                    {
                                        cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;
                                        cargaCancelamento.MensagemRejeicaoCancelamento = "Não é possível cancelar pois a carregamento da carga não está apto a recalcular o frete.";

                                        repCargaCancelamento.Atualizar(cargaCancelamento);

                                        if (enviarNotificacaoHUB)
                                            svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                                        return;
                                    }
                                }
                                else
                                {
                                    cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;
                                    cargaCancelamento.MensagemRejeicaoCancelamento = "Não é possível cancelar pois a carga do trecho anterior ainda está em processo de cálculo de frete.";

                                    repCargaCancelamento.Atualizar(cargaCancelamento);

                                    if (enviarNotificacaoHUB)
                                        svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                                    return;
                                }
                            }


                            string retorno = servicoRetornoCarga.ValidarCargaCanceladaParaNovoRetorno(cargaCancelamento.Carga);

                            if (!string.IsNullOrWhiteSpace(retorno))
                            {
                                cargaCancelamento.MensagemRejeicaoCancelamento = retorno;
                                cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;

                                repCargaCancelamento.Atualizar(cargaCancelamento);

                                if (enviarNotificacaoHUB)
                                    svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                                return;
                            }

                            if (cargaPedido.ReentregaSolicitada)
                            {
                                cargaPedido.Pedido.ReentregaSolicitada = true;
                                cargaPedido.Pedido.DataSolicitacaoReentrega = DateTime.Now;
                                repPedido.Atualizar(cargaPedido.Pedido);
                            }

                            //quando possuir um trecho anterior o pedido não pode ser cancelado, só deve ser cancelado na carga do trecho anterior.
                            if (cargaPedido.CargaPedidoTrechoAnterior == null && !cargaPedido.Carga.CargaColeta) //|| cargaPedido.CargaPedidoTrechoAnterior.CargaPedidoFilialEmissora)
                            {
                                //#32122 - Se não estiver passando liberar para montagem, validar se o pedido não está me mais de uma carga.
                                // Se estiver, não devemos cancelar o pedido.
                                bool pedidoOriundoSaldo = CargaPedidoOriundoSaldo(cargaCancelamento, cargaPedido, pedidosProdutos, montagemCarregamentoPedidoProduto);

                                if (!cargaCancelamento.LiberarPedidosParaMontagemCarga && !pedidoOriundoSaldo)
                                {
                                    cargaPedido.Pedido.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado;

                                    if (cargaPedido.Pedido.SituacaoRoteirizadorIntegracao == SituacaoRoteirizadorIntegracao.Integrado)
                                        codigosPedidoCancelarRoteirizacao.Add(cargaPedido.Pedido.Codigo);
                                }
                                else
                                {
                                    if (pedidoOriundoSaldo)
                                        AtualizarQuantidadePedidoProdutoCancelamentoCargaPedido(cargaPedido, unitOfWork);
                                    else
                                    {
                                        cargaPedido.Pedido.PesoSaldoRestante += cargaPedido.Peso;
                                        cargaPedido.Pedido.PedidoTotalmenteCarregado = false;
                                        if (cargaPedido.Expedidor != null)
                                            cargaPedido.Pedido.PedidoRedespachoTotalmenteCarregado = false;

                                        //TODO: PPC - Adicionado log temporário para identificar problema de retorno de saldo de pedido.
                                        Servicos.Log.TratarErro($"Pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} - Liberou saldo pedido {cargaPedido.Pedido.PesoSaldoRestante} - Peso Total.: {cargaPedido.Pedido.PesoTotal} - Totalmente carregado.: {cargaPedido.Pedido.PedidoTotalmenteCarregado}. Cancelamento.SolicitarCancelamentoCarga", "SaldoPedido");
                                    }
                                    cargaPedido.Pedido.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto;
                                }

                                if (configuracaoGeralCarga.AoCancelarCargaManterPedidosEmAberto)
                                {
                                    cargaPedido.Pedido.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto;

                                    if (configuracaoGeralCarga?.RemoverVinculoNotaPedidoAbertoAoCancelarCarga ?? false)
                                    {
                                        repCargaPedidoXMLNotaFiscalCTe.ExcluirCargaPedidoNotaFiscalCTePorCargaPedido(cargaPedido.Codigo);
                                        repCargaEntregaNotaFiscalCarga.ExcluirCargaEntregaNotaFiscalPorCargaPedido(cargaPedido.Codigo);
                                        repPedidoXMLNotaFiscal.DeletarPorCargaPedido(cargaPedido.Codigo);
                                        cargaPedido.Pedido.NotasFiscais.Clear();
                                    }
                                }

                                cargaPedido.Pedido.ControleNumeracao = cargaPedido.Pedido.Codigo;
                                repPedido.Atualizar(cargaPedido.Pedido);

                                //Agora precisamos ver se o pedido estava em uma sessão e o mesmo não está em outro carregamento... da mesma sessão.. vamos remover o pedido da sessão.
                                if (sessaoRoteirizador != null && cargaPedido.Pedido.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto)
                                {
                                    bool existeCarregamentoEmMontagemSessao = carregamentoPedidosSessaoEmMontagem.Exists(x => x.Pedido.Codigo == cargaPedido.Pedido.Codigo);
                                    if (!existeCarregamentoEmMontagemSessao)
                                    {
                                        List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> sessaoRoteirizadorPedido = (from srp in sessaoRoteirizadorPedidos
                                                                                                                                                     where srp.Pedido.Codigo == cargaPedido.Pedido.Codigo
                                                                                                                                                     select srp).ToList();
                                        foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido srp in sessaoRoteirizadorPedido)
                                        {
                                            srp.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao;
                                            repSessaoRoteirizadorPedido.Atualizar(srp);
                                        }
                                    }
                                }

                                if (!cargaPedido.Pedido.PedidoTransbordo)
                                {
                                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLsNotasFiscais = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);
                                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscaisAjustarCanhoto = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
                                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLsNotasFiscais)
                                    {
                                        if (cargaPedidoXMLNotaFiscalParcials.Any(obj => obj == pedidoXMLNotaFiscal.XMLNotaFiscal.Codigo))
                                            continue;

                                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFicaisDaNota = pedidoXMLsNotasFiscais.Count() < 300 ? repPedidoXMLNotaFiscal.BuscarPorXMLNotaFiscal(pedidoXMLNotaFiscal.XMLNotaFiscal.Codigo) : null;


                                        if (pedidoXMLNotasFicaisDaNota != null && pedidoXMLNotasFicaisDaNota.Count() > 1)//todo: fiz essa regra de menor que 300 pela avon, verificar uma forma melhor depois.
                                        {
                                            if (!pedidoXMLNotasFicaisDaNota.Any(obj => obj.CargaPedido.Codigo != pedidoXMLNotaFiscal.CargaPedido.Codigo && obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada))
                                            {
                                                if (!configuracao.NaoInativarNotasAoCancelarCarga)
                                                {
                                                    pedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva = false;
                                                    repXMLNotasFiscais.Atualizar(pedidoXMLNotaFiscal.XMLNotaFiscal);
                                                }
                                            }
                                            else
                                                pedidoXMLNotaFiscaisAjustarCanhoto.AddRange((from obj in pedidoXMLNotasFicaisDaNota where obj.CargaPedido.Codigo != pedidoXMLNotaFiscal.CargaPedido.Codigo && obj.XMLNotaFiscal.Canhoto?.Carga.Codigo == codigoCargaCancelamento && obj.CargaPedido.Recebedor == null && obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada select obj).ToList());

                                        }
                                        else if (!configuracao.NaoInativarNotasAoCancelarCarga)
                                        {
                                            pedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva = false;
                                            repXMLNotasFiscais.Atualizar(pedidoXMLNotaFiscal.XMLNotaFiscal);
                                        }
                                    }


                                    if (!configuracao.NaoInativarNotasAoCancelarCarga)
                                    {
                                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoXMLsParaSubcontratacao = repPedidoCTeParaSubContratacao.BuscarPorCargaPedido(cargaPedido.Codigo);
                                        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoXMLParaSubcontratacao in pedidoXMLsParaSubcontratacao)
                                        {
                                            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidosCTeParaSubContratacaoDoCTe = pedidoXMLsParaSubcontratacao.Count() < 300 ? repPedidoCTeParaSubContratacao.BuscarTodosPorCTeSubContratacao(pedidoXMLParaSubcontratacao.CTeTerceiro.Codigo) : null;
                                            if (pedidosCTeParaSubContratacaoDoCTe != null && pedidosCTeParaSubContratacaoDoCTe.Count() > 1)//todo: fiz essa regra de menor que 300 pela avon, verificar uma forma melhor depois.
                                            {
                                                if (!pedidosCTeParaSubContratacaoDoCTe.Any(obj => obj.CargaPedido.Codigo != pedidoXMLParaSubcontratacao.CargaPedido.Codigo && obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada))
                                                {
                                                    pedidoXMLParaSubcontratacao.CTeTerceiro.Ativo = false;
                                                    repCTeTerceiro.Atualizar(pedidoXMLParaSubcontratacao.CTeTerceiro);
                                                    Servicos.Log.TratarErro("Inativou o CTeTerceiro de código " + pedidoXMLParaSubcontratacao.CTeTerceiro.Codigo, "CTeTerceiro");
                                                }
                                            }
                                            else
                                            {
                                                pedidoXMLParaSubcontratacao.CTeTerceiro.Ativo = false;
                                                repCTeTerceiro.Atualizar(pedidoXMLParaSubcontratacao.CTeTerceiro);
                                                Servicos.Log.TratarErro("Inativou o CTeTerceiro de código " + pedidoXMLParaSubcontratacao.CTeTerceiro.Codigo, "CTeTerceiro");
                                            }
                                        }
                                    }

                                    if (!cargaCancelamento.DuplicarCarga)
                                    {
                                        Servicos.Embarcador.Canhotos.Canhoto svcCanhoto = new Canhotos.Canhoto(unitOfWork);
                                        svcCanhoto.AjustarCanhotosCargaCancelada(pedidoXMLNotaFiscaisAjustarCanhoto, tipoServicoMultisoftware, configuracao, unitOfWork);
                                    }

                                }

                                if (cargaPedido.CTeEmitidoNoEmbarcador && !cargaCancelamento.DuplicarCarga)
                                {
                                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> cargaPedidoDocumentoCTes = repCargaPedidoDocumentoCTe.BuscarPorCargaPedido(cargaPedido.Codigo);
                                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe in cargaPedidoDocumentoCTes)
                                    {
                                        if (cargaPedidoDocumentoCTe.CTe.Status == "A")
                                        {
                                            cargaPedidoDocumentoCTe.CTe.CTeSemCarga = true;
                                            repConhecimentoDeTransporteEletronico.Atualizar(cargaPedidoDocumentoCTe.CTe);
                                        }
                                    }

                                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe> cargaPedidoDocumentoMDFes = repCargaPedidoDocumentoMDFe.BuscarPorCargaPedido(cargaPedido.Codigo);
                                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe cargaPedidoDocumentoMDFe in cargaPedidoDocumentoMDFes)
                                    {
                                        if (cargaPedidoDocumentoMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado)
                                        {
                                            cargaPedidoDocumentoMDFe.MDFe.MDFeSemCarga = true;
                                            repMDFe.Atualizar(cargaPedidoDocumentoMDFe.MDFe);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                    {
                        if (cargaPedido.CargaPedidoFilialEmissora)
                        {
                            if (cargaPedido.CargaPedidoProximoTrecho != null && (cargaPedido.CargaPedidoProximoTrecho.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && cargaPedido.CargaPedidoProximoTrecho.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada))
                            {
                                List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoCTesParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarPorCargaPedido(cargaPedido.CargaPedidoProximoTrecho.Codigo);

                                if (!configuracao.NaoInativarNotasAoCancelarCarga)
                                {
                                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao in pedidoCTesParaSubContratacao)
                                    {
                                        pedidoCTeParaSubContratacao.CTeTerceiro.Ativo = false;
                                        repCTeTerceiro.Atualizar(pedidoCTeParaSubContratacao.CTeTerceiro);
                                        Servicos.Log.TratarErro("Inativou o CTeTerceiro de código " + pedidoCTeParaSubContratacao.CTeTerceiro.Codigo, "CTeTerceiro");
                                    }
                                }

                                cargaPedido.CargaPedidoProximoTrecho.CargaPedidoTrechoAnterior = null;

                                repCargaPedido.Atualizar(cargaPedido.CargaPedidoProximoTrecho);
                                cargaPedido.CargaPedidoProximoTrecho.Carga.AguardandoEmissaoDocumentoAnterior = true;
                                cargaPedido.CargaPedidoProximoTrecho.Carga.DataEnvioUltimaNFe = null;
                                cargaPedido.CargaPedidoProximoTrecho.Carga.DataInicioEmissaoDocumentos = null;
                                repCarga.Atualizar(cargaPedido.CargaPedidoProximoTrecho.Carga);
                            }
                        }

                        cargaPedido.Ativo = false;
                        repCargaPedido.Atualizar(cargaPedido);
                    }

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidoMontagemCargaAtualizar)
                    {
                        cargaPedido.CargaPedidoTrechoAnterior.CargaPedidoProximoTrecho = null;
                        repCargaPedido.Atualizar(cargaPedido.CargaPedidoTrechoAnterior);
                        repCarga.Atualizar(cargaPedido.CargaPedidoTrechoAnterior.Carga);
                    }

                    Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = repPreCarga.BuscarPorCarga(cargaCancelamento.Carga.Codigo);
                    if (preCarga != null)
                    {
                        preCarga.SituacaoPreCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreCarga.Cancelada;
                    }

                    new Pallets.DevolucaoPallets(unitOfWork).CancelarPallets(cargaCancelamento.Carga, tipoServicoMultisoftware);

                    cargaCancelamento.Carga.DataAtualizacaoCarga = DateTime.Now;
                    cargaCancelamento.Carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada;

                    if (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiEmbarcador)
                        cargaCancelamento.Carga.ControleNumeracao = cargaCancelamento.Carga.Codigo;

                    repCarga.CancelarCargasVinculadas(cargaCancelamento.Carga.Codigo, cargaCancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada);
                    servicoCargaJanelaCarregamentoPrioridade.RemoverPrioridadesPorCarga(cargaCancelamento.Carga);

                    if (cargaCancelamento.Carga.CargaAgrupada)
                        repCarga.AtualizarSituacaoCargasAgrupadas(cargaCancelamento.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada);

                    Servicos.Embarcador.Integracao.Ortec.IntegracaoOrtec.RemoverVinculosCargaCancelada(cargaCancelamento.Carga, unitOfWork);

                    cargaCancelamento.Carga.DataInicioCalculoFrete = null;
                    cargaCancelamento.Carga.CalculandoFrete = false;
                    cargaCancelamento.Carga.CalcularFreteSemEstornarComplemento = false;

                    servicoRetornoCarga.DisponibilizarCargaCanceladaParaNovoRetorno(cargaCancelamento.Carga);
                    CancelarEDIIntegracao(cargaCancelamento.Carga, unitOfWork);

                    repCarga.Atualizar(cargaCancelamento.Carga);

                    Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorCarga(cargaCancelamento.Carga.Codigo);

                    if (cargaCIOT != null && cargaCIOT.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia)
                    {
                        cargaCIOT.CIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Cancelado;
                        repCIOT.Atualizar(cargaCIOT.CIOT);
                    }

                    serCanhoto.InativarCanhotosAvulsosCarga(cargaCancelamento.Carga, unitOfWork);

                    serCreditoMovimentacao.ExtornarCreditoObtidoNaCarga(cargaCancelamento.Carga, unitOfWork);
                    svcCarga.CancelarConsolidacao(cargaCancelamento.Carga, tipoServicoMultisoftware, unitOfWork);
                    cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.Cancelada;
                    repCargaCancelamento.Atualizar(cargaCancelamento);
                    new Servicos.Embarcador.Integracao.IntegracaoPedidoRoterizador(unitOfWork).AdicionarParaIntegracaoAutomaticamente(codigosPedidoCancelarRoteirizacao, TipoRoteirizadorIntegracao.CancelarPedido);


                    List<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete> cargaComplementosFrete = repCargaComplementoFrete.BuscarPorCarga(cargaCancelamento.Carga.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete cargaComplemento in cargaComplementosFrete)
                        serCargaComplementoFrete.ExtornarComplementoDeFrete(cargaComplemento, tipoServicoMultisoftware, unitOfWork);

                    //List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> cargaOcorrencias = repCargaOcorrencias.BuscarPorCarga(cargaCancelamento.Carga.Codigo);
                    //foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia in cargaOcorrencias)
                    //    serOcorrencia.CancelarOcorrencia(cargaOcorrencia, unitOfWork, tipoServicoMultisoftware);

                    if (!cargaComQuebraContainer && !cargaCancelamento.CancelarDocumentosEmitidosNoEmbarcador)
                    {
                        if (!RemoverEGerarMovimentacaoDosCTesEmitidosPorOutroSistema(out erro, cargaCancelamento, unitOfWork, tipoServicoMultisoftware, stringConexao))
                        {
                            unitOfWork.Rollback();

                            cargaCancelamento.MensagemRejeicaoCancelamento = erro;
                            cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;

                            repCargaCancelamento.Atualizar(cargaCancelamento);

                            if (enviarNotificacaoHUB)
                                svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                            return;
                        }
                    }

                    GerarControleFinanceiroCancelamentoCarga(cargaCancelamento.Carga, unitOfWork, tipoServicoMultisoftware, stringConexao);
                    Servicos.Embarcador.Escrituracao.CancelamentoProvisao.CancelarProvisaoDocumentosCarga(cargaCancelamento.Carga, unitOfWork);

                    if (!(configuracaoMonitoramento?.ManterMonitoramentosDeCargasCanceladasAoReceberNovaCarga ?? false))
                        Servicos.Embarcador.Monitoramento.Monitoramento.ExcluirMonitoriaPorCarga(cargaCancelamento.Carga.Codigo, configuracao, unitOfWork);
                    else
                        Servicos.Embarcador.Monitoramento.Monitoramento.CancelarMonitoramento(cargaCancelamento.Carga, configuracao, new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado(), "", !cargaCancelamento.DuplicarCarga, unitOfWork);

                    Servicos.Embarcador.Pedido.Pedido.AtualizarSituacaoPlanejamentoPedidoTMS(cargaCancelamento.Carga, cargaPedidos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPlanejamentoPedidoTMS.CargaCanceladaAnulada, unitOfWork);
                    servicoColetaContainer.CancelarColetaContainer(cargaCancelamento.Carga);

                    if (!GerarMovimentosCancelamentoCarga(out erro, cargaCancelamento.Carga, unitOfWork, tipoServicoMultisoftware, stringConexao))
                    {
                        unitOfWork.Rollback();

                        cargaCancelamento.MensagemRejeicaoCancelamento = erro;
                        cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;

                        repCargaCancelamento.Atualizar(cargaCancelamento);

                        if (enviarNotificacaoHUB)
                            svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                        return;
                    }

                    if (!CancelarContratoFreteTerceiro(out erro, cargaCancelamento.Carga, unitOfWork, tipoServicoMultisoftware))
                    {
                        unitOfWork.Rollback();
                        return;
                    }

                    List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> filasCarregamentoVeiculoDisponibilizadas = DisponibilizarFilasCarregamentoVeiculo(cargaCancelamento.Carga.Codigo, unitOfWork, tipoServicoMultisoftware);

                    ReverterSaldoContratoPrestacaoServico(cargaCancelamento.Carga, unitOfWork);

                    if (abrirTransacao)
                        unitOfWork.CommitChanges();

                    NotificarAlteracoesFilaCarregamentoVeiculo(filasCarregamentoVeiculoDisponibilizadas, unitOfWork);

                    if (enviarNotificacaoHUB)
                        svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);
                }

                svcCarga.CancelarAgendamento(cargaCancelamento.Carga, tipoServicoMultisoftware, unitOfWork);
                svcCarga.CancelarAgendamentoPallet(cargaCancelamento.Carga, unitOfWork);

                svcContratoFreteCliente.ExtornaSaldo(cargaCancelamento.Carga);

                // Disponibilidade veiculo
                if (cargaCancelamento.Carga.Veiculo != null)
                    Servicos.Embarcador.GestaoPatio.DisponibilidadeVeiculo.SetaVeiculoDisponivel(cargaCancelamento.Carga.Veiculo.Codigo, unitOfWork);
            }
        }

        public static bool ValidarSePossivelCancelar(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string erro, Repositorio.UnitOfWork unitOfWork)
        {
            erro = "";

            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfigEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
            Repositorio.Embarcador.Financeiro.PagamentoProvedorCarga repPagamentoProvedorCarga = new Repositorio.Embarcador.Financeiro.PagamentoProvedorCarga(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config = repConfigEmbarcador.BuscarConfiguracaoPadrao();

            if (carga.CargaMDFes != null && carga.CargaMDFes.Any(o => o.MDFe.Status == Dominio.Enumeradores.StatusMDFe.EmEncerramento || o.MDFe.Status == Dominio.Enumeradores.StatusMDFe.EmCancelamento))
            {
                erro = "Não é possível adicionar um cancelamento/anulação para esta carga pois há um MDF-e em encerramento/cancelamento para a mesma.";
                return false;
            }

            if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada)
            {
                erro = "A carga selecionada já está em cancelamento ou cancelada.";
                return false;
            }

            if (Carga.IsCargaBloqueada(carga, unitOfWork))
            {
                erro = "A carga está bloqueada e não permite alteração.";
                return false;
            }

            if (carga.Ocorrencias != null && carga.Ocorrencias.Any(o => o.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Cancelada && o.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Rejeitada && o.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.RejeitadaEtapaEmissao))
            {
                erro = "Existem ocorrências para esta carga que não estão canceladas, não sendo possível realizar o cancelamento da carga.";
                return false;
            }

            int quantidade = repCargaDocumentoParaEmissaoNFSManual.ContarPorCargaVinculadasNFSManual(carga.Codigo);
            if (quantidade > 0)
            {
                erro = "Exitem NFSs Manuais para está carga que não estão canceladas, não sendo possível realizar o cancelamento/anulação da carga.";
                return false;
            }

            if (carga.CargaMDFes != null)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe in carga.CargaMDFes.ToList())
                {
                    if (cargaMDFe.MDFe != null && cargaMDFe.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual = repCargaMDFeManual.BuscarPorMDFe(cargaMDFe.MDFe.Codigo);
                        if (cargaMDFeManual != null)
                        {
                            erro = "Existem MDF-es manuais para esta carga que não estão canceladas, não sendo possível realizar o cancelamento da carga.";
                            return false;
                        }
                    }
                }
            }

            Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManualCarga = repCargaMDFeManual.BuscarPorCarga(carga.Codigo);
            if (cargaMDFeManualCarga != null)
            {
                erro = "Existem MDF-es manuais para esta carga que não estão canceladas, não sendo possível realizar o cancelamento da carga.";
                return false;
            }

            if (repCargaIntegracaoValePedagio.VerificarSeExisteValePedagioPorStatus(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Encerrada))
            {
                erro = "Já existem vales pedágios encerrados para a carga, não sendo possível solicitar o cancelamento.";
                return false;
            }

            if (repPagamentoProvedorCarga.VerificarSeExisteCargaEmPagamentoProvedor(carga.Codigo))
            {
                erro = "Existe um pagamento ao fornecedor em andamento, não sendo possível solicitar o cancelamento.";
                return false;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCarga(carga.Codigo);
            if (cargaCancelamento != null)
            {
                erro = "Já existe uma solicitação de cancelamento para esta carga.";
                return false;
            }

            if (!(serCarga.VerificarSeCargaEstaNaLogistica(carga, tipoServicoMultisoftware) || config.PermitirCancelamentoTotalCarga || (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe && (operadorLogistica?.SupervisorLogistica ?? false))))
            {
                erro = "Não é possível solicitar o cancelamento de uma carga após liberá-la para faturamento, se necessário solicite ao responsável pelo faturamento que faça o cancelamento da mesma no ERP.";
                return false;
            }

            if (carga.FreteDeTerceiro && carga.CargaCTes != null)
            {
                if (carga.CargaCTes.Any(o => o.CIOTs.Any(c => c.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto || c.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgLiberarViagem)))
                {
                    erro = "A carga possui um CIOT em aberto, não sendo possível o cancelamento. Cancele o CIOT para efetuar o cancelamento da carga.";
                    return false;
                }

                if (carga.CargaCTes.Any(o => o.CIOTs.Any(c => c.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado)))
                {
                    erro = "A carga possui um CIOT encerrado, não sendo possível o cancelamento.";
                    return false;
                }
            }

            if (carga.EmpresaFilialEmissora != null && carga.Pedidos != null)
            {
                if (carga.Pedidos.Any(obj => obj.CargaPedidoProximoTrecho != null
                && (obj.CargaPedidoProximoTrecho.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos
                || obj.CargaPedidoProximoTrecho.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte
                || obj.CargaPedidoProximoTrecho.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada
                || obj.CargaPedidoProximoTrecho.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao
                || obj.CargaPedidoProximoTrecho.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos
                || obj.CargaPedidoProximoTrecho.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.LiberadoPagamento
                )))
                {
                    {
                        erro = "A carga possui um redespacho já autorizado, para cancelar essa carga é necessário cancelar primeiro as cargas de redespacho.";
                        return false;
                    }
                }
                else
                {
                    //se faltam apenas 5 minutos para emissão não permite cancelar sem cancelar o proximo trecho
                    if (carga.Pedidos.Any(obj => obj.CargaPedidoProximoTrecho != null && obj.CargaPedidoProximoTrecho.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe
                        && (obj.CargaPedidoProximoTrecho.Carga.DataEnvioUltimaNFe.HasValue && obj.CargaPedidoProximoTrecho.Carga.DataEnvioUltimaNFe.Value >= DateTime.Now.AddMinutes(-5))))
                    {
                        erro = "O redespacho desta viagem será emitido dentro de minutos, desta forma é necessário cancelar o próximo trecho primeiro para cancelar essa viagem.";
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool VerificarSeJaIntegradoComERP(Dominio.Entidades.Embarcador.Cargas.Carga carga, out string erro, string tipo, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            erro = "";
            if (configuracao.BloquearCancelamentoCargasComDataCarregamentoEDadosTransporteInformados)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoesLiberadas = new List<TipoIntegracao>();
                tiposIntegracoesLiberadas.Add(TipoIntegracao.SaintGobain);
                Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
                if ((carga.DataCarregamentoCarga.HasValue && carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova) || repCargaDadosTransporteIntegracao.VerificarSeIntegrouPorCarga(carga.Codigo, tiposIntegracoesLiberadas) || carga.CarregamentoIntegradoERP)
                {
                    erro = "A carga já foi integrada ao ERP, desta forma, ela não pode mais ser " + tipo + " diretamente pelo portal, somente por solicitação do ERP.";
                    return false;
                }
            }

            return true;
        }

        public static void GeraRegistrosCancelamento(Dominio.Entidades.Embarcador.Cargas.Carga carga, string motivo, Dominio.Entidades.Usuario usuario, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfigEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config = repConfigEmbarcador.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = new Dominio.Entidades.Embarcador.Cargas.CargaCancelamento
            {
                Carga = carga,
                DataCancelamento = DateTime.Now,
                MotivoCancelamento = motivo,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Cancelamento,
                Usuario = usuario,
                EnviouAverbacoesCTesParaCancelamento = true,
                SituacaoCargaNoCancelamento = carga.SituacaoCarga,
                CancelarDocumentosEmitidosNoEmbarcador = false, //cancelarDocumentosEmitidosNoEmbarcador,
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.EmCancelamento,
                DuplicarCarga = false
            };

            //if (!naoDuplicarCarga && (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || config.SempreDuplicarCargaCancelada)) //|| (carga.TipoOperacao != null && carga.TipoOperacao.PermiteImportarDocumentosManualmente))
            //    cargaCancelamento.DuplicarCarga = true;
            //else
            //    cargaCancelamento.DuplicarCarga = false;

            repCargaCancelamento.Inserir(cargaCancelamento);

            GerarLogCancelamento(cargaCancelamento, usuario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogCargaCancelamento.Emissao, unitOfWork);
            GerarIntegracoesCancelamento(ref cargaCancelamento, tipoServicoMultisoftware, unitOfWork);

            repCargaCancelamento.Atualizar(cargaCancelamento);

            Servicos.Auditoria.Auditoria.Auditar(auditado, cargaCancelamento, null, "Adicionou Cancelamento da Carga", unitOfWork);
            Servicos.Auditoria.Auditoria.Auditar(auditado, cargaCancelamento.Carga, null, "Adicionou Cancelamento da Carga", unitOfWork);
        }

        public static void RemoverDocumentoFaturamentoCancelamentoCarga(List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentosFaturamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto repFaturaDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto(unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento in documentosFaturamento)
            {
                List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> faturaDocumentos = repFaturaDocumento.BuscarPorDocumentoFaturamento(documentoFaturamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Cancelado);

                foreach (Dominio.Entidades.Embarcador.Fatura.FaturaDocumento faturaDocumento in faturaDocumentos)
                {
                    repFaturaDocumentoAcrescimoDesconto.DeletarPorFaturaDocumento(faturaDocumento.Codigo);

                    repTitulo.AjustarTitulosCancelamentoCarga(faturaDocumento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado);
                    repTituloDocumento.AjustarTitulosDocumentosCancelamentoCarga(faturaDocumento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado);

                    repFaturaDocumento.Deletar(faturaDocumento);
                }

                documentoFaturamento.Motoristas = null;
                documentoFaturamento.Veiculos = null;

                repDocumentoFaturamento.Deletar(documentoFaturamento);
            }
        }

        public static bool ReverterItensEmAbertoAposCancelamentoCTe(out string erro, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeTrabalho)
        {
            if (cargaCTe.CTe.Status != "C" && cargaCTe.CTe.Status != "Z")
            {
                erro = string.Empty;
                return true;
            }

            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentosFaturamento = repDocumentoFaturamento.BuscarTodosPorCTe(cargaCTe.CTe.Codigo);
            foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento in documentosFaturamento)
            {
                if (cargaCTe.CTe.Status == "Z")
                    documentoFaturamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Anulado;
                else
                    documentoFaturamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Cancelado;

                documentoFaturamento.DataLiberacaoPagamento = DateTime.Now;
                repDocumentoFaturamento.Atualizar(documentoFaturamento);
            }

            List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulosParaCancelamentoCTe = repTituloDocumento.BuscarTitulosEmAbertoPorCTe(cargaCTe.CTe.Codigo);
            Servicos.Embarcador.Financeiro.Titulo.GerarCancelamentoAutomaticoTitulosEmAberto(titulosParaCancelamentoCTe, "Cancelamento do título gerado automaticamente à partir do cancelamento do CT-e " + cargaCTe.CTe.Numero + "-" + cargaCTe.CTe.Serie.Numero + ".", tipoServicoMultisoftware, unidadeTrabalho);
            List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> cargaDocumentoParaEmissaoNFSManuais = repCargaDocumentoParaEmissaoNFSManual.BuscarPorCargaCTe(cargaCTe.Codigo);
            cargaDocumentoParaEmissaoNFSManuais.AddRange(repCargaDocumentoParaEmissaoNFSManual.BuscarPorPedidoXMLNotaFiscalPorCarga(cargaCTe.Codigo));//cargaCTe.NotasFiscais.Select(o => o.PedidoXMLNotaFiscal.Codigo).ToArray()););

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual cargaDocumentoParaEmissaoNFSManual in cargaDocumentoParaEmissaoNFSManuais)
                if (cargaDocumentoParaEmissaoNFSManual.LancamentoNFSManual == null)
                    repCargaDocumentoParaEmissaoNFSManual.Deletar(cargaDocumentoParaEmissaoNFSManual);

            erro = string.Empty;
            return true;
        }

        public static void GerarIntegracoesCancelamento(ref Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeTrabalho)
        {
            if (Integracao.IntegracaoCargaCancelamento.AdicionarIntegracoesCarga(cargaCancelamento, unidadeTrabalho, tipoServicoMultisoftware))
                return;

            if (cargaCancelamento.SituacaoCargaNoCancelamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos && cargaCancelamento.Carga.EmpresaFilialEmissora != null && cargaCancelamento.Carga.EmpresaFilialEmissora.TransportadorLayoutsEDI.Any(obj => obj.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC_CANCELAMENTO))
                cargaCancelamento.GerouIntegracao = true;
            else if (cargaCancelamento.SituacaoCargaNoCancelamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos && cargaCancelamento.Carga.Empresa != null && cargaCancelamento.Carga.Empresa.TransportadorLayoutsEDI.Any(obj => obj.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC_CANCELAMENTO))
                cargaCancelamento.GerouIntegracao = true;
            else
                cargaCancelamento.GerouIntegracao = false;
        }

        public static bool GerarMovimentoCancelamentoCTe(out string erro, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, string stringConexao, bool gerarMovimentoRemocaoCTeEmitidoOutroSistema = false, bool anulacaoComCTeAnulacao = false)
        {
            if (cargaCTe.CTe.Status != "C" && cargaCTe.CTe.Status != "Z")
            {
                if (!(gerarMovimentoRemocaoCTeEmitidoOutroSistema && cargaCTe.SistemaEmissor != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe && cargaCTe.CTe.Status == "A"))
                {
                    erro = string.Empty;
                    return true;
                }
            }

            if (cargaCTe.Carga.CargaTransbordo)
            {
                erro = string.Empty;
                return true;
            }

            Servicos.Embarcador.Financeiro.ProcessoMovimento svcMovimento = new Financeiro.ProcessoMovimento();

            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponentes = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            DateTime dataMovimentacao = DateTime.Now;
            string observacaoMovimentacao = string.Empty;

            Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimento = null,
                                                                  tipoMovimentoImpostos = null,
                                                                  tipoMovimentoValorLiquido = null,
                                                                  tipoMovimentoPIS = null,
                                                                  tipoMovimentoCOFINS = null,
                                                                  tipoMovimentoIR = null,
                                                                  tipoMovimentoCSLL = null;

            if (cargaCTe.CTe.Status == "Z")
            {
                observacaoMovimentacao = "Movimento gerado à partir da anulação do ";

                if (anulacaoComCTeAnulacao)
                {
                    tipoMovimento = cargaCTe.CTe.ModeloDocumentoFiscal?.TipoMovimentoAnulacaoNotaAnulacaoEmbarcador != null ? cargaCTe.CTe.ModeloDocumentoFiscal?.TipoMovimentoAnulacaoNotaAnulacaoEmbarcador : cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoAnulacao;
                    tipoMovimentoImpostos = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoImpostoAnulacaoNotaAnulacaoEmbarcador;
                    tipoMovimentoValorLiquido = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoAnulacaoNotaAnulacaoEmbarcador;
                    tipoMovimentoPIS = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoPISAnulacaoNotaAnulacaoEmbarcador;
                    tipoMovimentoCOFINS = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoCOFINSAnulacaoNotaAnulacaoEmbarcador;
                    tipoMovimentoIR = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoIRAnulacaoNotaAnulacaoEmbarcador;
                    tipoMovimentoCSLL = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoCSLLAnulacaoNotaAnulacaoEmbarcador;
                }
                else
                {
                    tipoMovimento = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoAnulacao;
                    tipoMovimentoImpostos = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoImpostoAnulacao;
                    tipoMovimentoValorLiquido = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoAnulacao;
                    tipoMovimentoPIS = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoPISAnulacao;
                    tipoMovimentoCOFINS = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoCOFINSAnulacao;
                    tipoMovimentoIR = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoIRAnulacao;
                    tipoMovimentoCSLL = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoCSLLAnulacao;
                }

                if (cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoPropriaNacional != null && cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoAgregadoNacional != null && cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoTerceiroNacional != null &&
                    cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoPropriaInternacional != null && cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoAgregadoInternacional != null && cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoTerceiroInternacional != null)
                {
                    bool cteNacional = cargaCTe.CTe.LocalidadeInicioPrestacao.Pais == null || cargaCTe.CTe.LocalidadeTerminoPrestacao.Pais == null && (cargaCTe.CTe.LocalidadeInicioPrestacao.Pais.Sigla == "01058" && cargaCTe.CTe.LocalidadeTerminoPrestacao.Pais.Sigla == "01058");
                    bool cteProprio = cargaCTe.Carga.Terceiro == null;
                    bool cteAgregado = false;
                    bool cteTerceiro = false;
                    if (!cteProprio)
                    {
                        Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = ObterModalidadeTransportador(cargaCTe.Carga.Terceiro, unitOfWork);

                        cteAgregado = modalidadeTerceiro != null && modalidadeTerceiro.TipoTransportador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo.TACAgregado;
                        cteTerceiro = modalidadeTerceiro == null || modalidadeTerceiro.TipoTransportador != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo.TACAgregado;
                    }
                    if (cteNacional && cteProprio)
                        tipoMovimentoValorLiquido = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoPropriaNacional;
                    else if (!cteNacional && cteProprio)
                        tipoMovimentoValorLiquido = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoPropriaInternacional;
                    else if (cteNacional && cteAgregado)
                        tipoMovimentoValorLiquido = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoAgregadoNacional;
                    else if (!cteNacional && cteAgregado)
                        tipoMovimentoValorLiquido = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoAgregadoInternacional;
                    else if (cteNacional && cteTerceiro)
                        tipoMovimentoValorLiquido = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoTerceiroNacional;
                    else if (!cteNacional && cteTerceiro)
                        tipoMovimentoValorLiquido = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoTerceiroInternacional;
                }

                dataMovimentacao = cargaCTe.CTe.DataAnulacao.HasValue ? cargaCTe.CTe.DataAnulacao.Value : cargaCTe.CTe.DataCancelamento.HasValue ? cargaCTe.CTe.DataCancelamento.Value : cargaCTe.CTe.DataEmissao.Value;
            }
            else if (cargaCTe.CTe.Status == "C")
            {
                observacaoMovimentacao = "Movimento gerado à partir do cancelamento do ";
                tipoMovimento = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoCancelamento;
                tipoMovimentoImpostos = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoImpostoCancelamento;
                tipoMovimentoValorLiquido = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamento;
                tipoMovimentoPIS = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoPISCancelamento;
                tipoMovimentoCOFINS = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoCOFINSCancelamento;
                tipoMovimentoIR = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoIRCancelamento;
                tipoMovimentoCSLL = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoCSLLCancelamento;
                dataMovimentacao = cargaCTe.CTe.DataCancelamento.Value;

                if (cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoPropriaNacional != null && cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoAgregadoNacional != null && cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoTerceiroNacional != null &&
                    cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoPropriaInternacional != null && cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoAgregadoInternacional != null && cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoTerceiroInternacional != null)
                {
                    bool cteNacional = cargaCTe.CTe.LocalidadeInicioPrestacao.Pais == null || cargaCTe.CTe.LocalidadeTerminoPrestacao.Pais == null && (cargaCTe.CTe.LocalidadeInicioPrestacao.Pais.Sigla == "01058" && cargaCTe.CTe.LocalidadeTerminoPrestacao.Pais.Sigla == "01058");
                    bool cteProprio = cargaCTe.Carga.Terceiro == null;
                    bool cteAgregado = false;
                    bool cteTerceiro = false;
                    if (!cteProprio)
                    {
                        Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = ObterModalidadeTransportador(cargaCTe.Carga.Terceiro, unitOfWork);

                        cteAgregado = modalidadeTerceiro != null && modalidadeTerceiro.TipoTransportador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo.TACAgregado;
                        cteTerceiro = modalidadeTerceiro == null || modalidadeTerceiro.TipoTransportador != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo.TACAgregado;
                    }
                    if (cteNacional && cteProprio)
                        tipoMovimentoValorLiquido = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoPropriaNacional;
                    else if (!cteNacional && cteProprio)
                        tipoMovimentoValorLiquido = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoPropriaInternacional;
                    else if (cteNacional && cteAgregado)
                        tipoMovimentoValorLiquido = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoAgregadoNacional;
                    else if (!cteNacional && cteAgregado)
                        tipoMovimentoValorLiquido = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoAgregadoInternacional;
                    else if (cteNacional && cteTerceiro)
                        tipoMovimentoValorLiquido = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoTerceiroNacional;
                    else if (!cteNacional && cteTerceiro)
                        tipoMovimentoValorLiquido = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoTerceiroInternacional;
                }
            }
            else if (gerarMovimentoRemocaoCTeEmitidoOutroSistema && cargaCTe.CTe.Status == "A" && cargaCTe.SistemaEmissor != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe)
            {
                observacaoMovimentacao = "Movimento gerado à partir da remoção do documento emitido por outro sistema da carga " + cargaCTe.Carga.CodigoCargaEmbarcador + " - ";
                tipoMovimento = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoCancelamento;
                tipoMovimentoImpostos = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoImpostoCancelamento;
                tipoMovimentoValorLiquido = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamento;
                tipoMovimentoPIS = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoPISCancelamento;
                tipoMovimentoCOFINS = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoCOFINSCancelamento;
                tipoMovimentoIR = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoIRCancelamento;
                tipoMovimentoCSLL = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoCSLLCancelamento;
                dataMovimentacao = cargaCTe.CTe.DataAutorizacao.Value;

                if (cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoPropriaNacional != null && cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoAgregadoNacional != null && cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoTerceiroNacional != null &&
                    cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoPropriaInternacional != null && cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoAgregadoInternacional != null && cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoTerceiroInternacional != null)
                {
                    bool cteNacional = cargaCTe.CTe.LocalidadeInicioPrestacao.Pais == null || cargaCTe.CTe.LocalidadeTerminoPrestacao.Pais == null && (cargaCTe.CTe.LocalidadeInicioPrestacao.Pais.Sigla == "01058" && cargaCTe.CTe.LocalidadeTerminoPrestacao.Pais.Sigla == "01058");
                    bool cteProprio = cargaCTe.Carga.Terceiro == null;
                    bool cteAgregado = false;
                    bool cteTerceiro = false;
                    if (!cteProprio)
                    {
                        Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = ObterModalidadeTransportador(cargaCTe.Carga.Terceiro, unitOfWork);

                        cteAgregado = modalidadeTerceiro != null && modalidadeTerceiro.TipoTransportador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo.TACAgregado;
                        cteTerceiro = modalidadeTerceiro == null || modalidadeTerceiro.TipoTransportador != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo.TACAgregado;
                    }
                    if (cteNacional && cteProprio)
                        tipoMovimentoValorLiquido = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoPropriaNacional;
                    else if (!cteNacional && cteProprio)
                        tipoMovimentoValorLiquido = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoPropriaInternacional;
                    else if (cteNacional && cteAgregado)
                        tipoMovimentoValorLiquido = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoAgregadoNacional;
                    else if (!cteNacional && cteAgregado)
                        tipoMovimentoValorLiquido = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoAgregadoInternacional;
                    else if (cteNacional && cteTerceiro)
                        tipoMovimentoValorLiquido = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoTerceiroNacional;
                    else if (!cteNacional && cteTerceiro)
                        tipoMovimentoValorLiquido = cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoTerceiroInternacional;
                }
            }

            decimal valorImposto = cargaCTe.CTe.ValorICMS;
            string tipoImposto = "ICMS.";
            if (cargaCTe.CTe.ModeloDocumentoFiscal != null && (cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe || cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS))
            {
                valorImposto = cargaCTe.CTe.ValorISS;
                tipoImposto = "ISS.";
            }

            observacaoMovimentacao += cargaCTe.CTe.ModeloDocumentoFiscal.Abreviacao + " " + cargaCTe.CTe.Numero + "-" + cargaCTe.CTe.Serie.Numero + ".";

            if (tipoMovimento == null)
            {
                erro = "Tipo de movimento para anulação/cancelamento do CT-e não configurado.";
                return false;
            }

            if (dataMovimentacao == DateTime.MinValue)
                dataMovimentacao = DateTime.Now;

            if (!svcMovimento.GerarMovimentacao(out erro, tipoMovimento, dataMovimentacao, cargaCTe.CTe.ValorAReceber, cargaCTe.CTe.Numero.ToString() + "-" + cargaCTe.CTe.Serie.Numero.ToString(), observacaoMovimentacao, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTe.CTe?.TomadorPagador?.Cliente, cargaCTe.CTe?.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTe.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Cancelamento))
                return false;

            if (cargaCTe.CTe.ModeloDocumentoFiscal.DiferenciarMovimentosParaValorLiquido)
            {
                if (!svcMovimento.GerarMovimentacao(out erro, tipoMovimentoValorLiquido, dataMovimentacao, cargaCTe.CTe.ValorFrete, cargaCTe.CTe.Numero.ToString() + "-" + cargaCTe.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Valor do frete líquido.", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTe.CTe?.TomadorPagador?.Cliente, cargaCTe.CTe?.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTe.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Cancelamento))
                    return false;
            }

            if (cargaCTe.CTe.ModeloDocumentoFiscal.DiferenciarMovimentosParaImpostos)
                if (!svcMovimento.GerarMovimentacao(out erro, tipoMovimentoImpostos, dataMovimentacao, valorImposto, cargaCTe.CTe.Numero.ToString() + "-" + cargaCTe.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: " + tipoImposto, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTe.CTe?.TomadorPagador?.Cliente, cargaCTe.CTe?.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTe.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Cancelamento))
                    return false;

            if (cargaCTe.CTe.ModeloDocumentoFiscal.DiferenciarMovimentosParaPIS)
                if (!svcMovimento.GerarMovimentacao(out erro, tipoMovimentoPIS, dataMovimentacao, cargaCTe.CTe.ValorPIS, cargaCTe.CTe.Numero.ToString() + "-" + cargaCTe.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: PIS.", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTe.CTe?.TomadorPagador?.Cliente, cargaCTe.CTe?.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTe.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Cancelamento))
                    return false;

            if (cargaCTe.CTe.ModeloDocumentoFiscal.DiferenciarMovimentosParaCOFINS)
                if (!svcMovimento.GerarMovimentacao(out erro, tipoMovimentoCOFINS, dataMovimentacao, cargaCTe.CTe.ValorCOFINS, cargaCTe.CTe.Numero.ToString() + "-" + cargaCTe.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: COFINS.", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTe.CTe?.TomadorPagador?.Cliente, cargaCTe.CTe?.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTe.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Cancelamento))
                    return false;

            if (cargaCTe.CTe.ModeloDocumentoFiscal.DiferenciarMovimentosParaIR)
                if (!svcMovimento.GerarMovimentacao(out erro, tipoMovimentoIR, dataMovimentacao, cargaCTe.CTe.ValorIR, cargaCTe.CTe.Numero.ToString() + "-" + cargaCTe.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: IR.", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTe.CTe?.TomadorPagador?.Cliente, cargaCTe.CTe?.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTe.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Cancelamento))
                    return false;

            if (cargaCTe.CTe.ModeloDocumentoFiscal.DiferenciarMovimentosParaCSLL)
                if (!svcMovimento.GerarMovimentacao(out erro, tipoMovimentoCSLL, dataMovimentacao, cargaCTe.CTe.ValorCSLL, cargaCTe.CTe.Numero.ToString() + "-" + cargaCTe.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: CSLL.", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTe.CTe?.TomadorPagador?.Cliente, cargaCTe.CTe?.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTe.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Cancelamento))
                    return false;

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete> componentesFrete = repCargaCTeComponentes.BuscarPorCargaCTeQueGeraMovimentacao(cargaCTe.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete componente in componentesFrete)
            {
                if (componente.ComponenteFrete.GerarMovimentoAutomatico)
                {
                    Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimentoComponente = null;

                    if (cargaCTe.CTe.Status == "Z")
                        tipoMovimentoComponente = componente.ComponenteFrete.TipoMovimentoAnulacao;
                    else
                        tipoMovimentoComponente = componente.ComponenteFrete.TipoMovimentoCancelamento;

                    decimal valorMovimentacaoComponente = Math.Abs(componente.ValorComponente);

                    if (!svcMovimento.GerarMovimentacao(out erro, tipoMovimentoComponente, dataMovimentacao, valorMovimentacaoComponente, cargaCTe.CTe.Numero.ToString() + "-" + cargaCTe.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Componente: " + componente.ComponenteFrete.Descricao + ".", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTe.CTe?.TomadorPagador?.Cliente, cargaCTe.CTe?.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTe.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Cancelamento))
                        return false;
                }
            }

            cargaCTe.GerouMovimentacaoCancelamento = true;

            repCargaCTe.Atualizar(cargaCTe);

            erro = string.Empty;
            return true;
        }

        public static bool GerarMovimentoCancelamentoCTe(out string erro, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeTrabalho)
        {
            if (cte.Status != "C" && cte.Status != "Z")
            {
                erro = string.Empty;
                return true;
            }

            Servicos.Embarcador.Financeiro.ProcessoMovimento svcMovimento = new Financeiro.ProcessoMovimento();
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unidadeTrabalho);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);

            DateTime dataMovimentacao = DateTime.Now;
            string observacaoMovimentacao = string.Empty;

            Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimento = null,
                                                                  tipoMovimentoImpostos = null,
                                                                  tipoMovimentoValorLiquido = null,
                                                                  tipoMovimentoPIS = null,
                                                                  tipoMovimentoCOFINS = null,
                                                                  tipoMovimentoIR = null,
                                                                  tipoMovimentoCSLL = null;

            if (cte.Status == "Z")
            {
                observacaoMovimentacao = "Movimento gerado à partir da anulação do ";
                tipoMovimento = cte.ModeloDocumentoFiscal.TipoMovimentoAnulacao;
                tipoMovimentoImpostos = cte.ModeloDocumentoFiscal.TipoMovimentoImpostoAnulacao;
                tipoMovimentoValorLiquido = cte.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoAnulacao;
                tipoMovimentoPIS = cte.ModeloDocumentoFiscal.TipoMovimentoPISAnulacao;
                tipoMovimentoCOFINS = cte.ModeloDocumentoFiscal.TipoMovimentoCOFINSAnulacao;
                tipoMovimentoIR = cte.ModeloDocumentoFiscal.TipoMovimentoIRAnulacao;
                tipoMovimentoCSLL = cte.ModeloDocumentoFiscal.TipoMovimentoCSLLAnulacao;
                dataMovimentacao = cte.DataAnulacao.Value;
            }
            else if (cte.Status == "C")
            {
                observacaoMovimentacao = "Movimento gerado à partir do cancelamento do ";
                tipoMovimento = cte.ModeloDocumentoFiscal.TipoMovimentoCancelamento;
                tipoMovimentoImpostos = cte.ModeloDocumentoFiscal.TipoMovimentoImpostoCancelamento;
                tipoMovimentoValorLiquido = cte.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamento;
                tipoMovimentoPIS = cte.ModeloDocumentoFiscal.TipoMovimentoPISCancelamento;
                tipoMovimentoCOFINS = cte.ModeloDocumentoFiscal.TipoMovimentoCOFINSCancelamento;
                tipoMovimentoIR = cte.ModeloDocumentoFiscal.TipoMovimentoIRCancelamento;
                tipoMovimentoCSLL = cte.ModeloDocumentoFiscal.TipoMovimentoCSLLCancelamento;
                dataMovimentacao = cte.DataCancelamento.Value;
            }

            if (cte.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoPropriaNacional != null && cte.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoAgregadoNacional != null && cte.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoTerceiroNacional != null &&
                    cte.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoPropriaInternacional != null && cte.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoAgregadoInternacional != null && cte.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoTerceiroInternacional != null)
            {
                Dominio.Entidades.Veiculo veiculoPrincipal = repCTe.BuscarPrimeiroVeiculo(cte.Codigo);
                bool cteNacional = cte.LocalidadeInicioPrestacao.Pais == null || cte.LocalidadeTerminoPrestacao.Pais == null && (cte.LocalidadeInicioPrestacao.Pais.Sigla == "01058" && cte.LocalidadeTerminoPrestacao.Pais.Sigla == "01058");
                bool cteProprio = veiculoPrincipal == null || veiculoPrincipal.Tipo == "P";
                bool cteAgregado = false;
                bool cteTerceiro = false;
                if (!cteProprio)
                {
                    Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = ObterModalidadeTransportador(veiculoPrincipal?.Proprietario ?? null, unidadeTrabalho);

                    cteAgregado = modalidadeTerceiro != null && modalidadeTerceiro.TipoTransportador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo.TACAgregado;
                    cteTerceiro = modalidadeTerceiro == null || modalidadeTerceiro.TipoTransportador != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo.TACAgregado;
                }
                if (cteNacional && cteProprio)
                    tipoMovimentoValorLiquido = cte.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoPropriaNacional;
                else if (!cteNacional && cteProprio)
                    tipoMovimentoValorLiquido = cte.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoPropriaInternacional;
                else if (cteNacional && cteAgregado)
                    tipoMovimentoValorLiquido = cte.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoAgregadoNacional;
                else if (!cteNacional && cteAgregado)
                    tipoMovimentoValorLiquido = cte.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoAgregadoInternacional;
                else if (cteNacional && cteTerceiro)
                    tipoMovimentoValorLiquido = cte.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoTerceiroNacional;
                else if (!cteNacional && cteTerceiro)
                    tipoMovimentoValorLiquido = cte.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamentoTerceiroInternacional;
            }

            observacaoMovimentacao += cte.ModeloDocumentoFiscal.Abreviacao + " " + cte.Numero + "-" + cte.Serie.Numero + ".";

            if (tipoMovimento == null)
            {
                erro = "Tipo de movimento para anulação/cancelamento do CT-e não configurado.";
                return false;
            }

            if (!svcMovimento.GerarMovimentacao(out erro, tipoMovimento, dataMovimentacao, cte.ValorAReceber, cte.Numero.ToString() + "-" + cte.Serie.Numero.ToString(), observacaoMovimentacao, unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cte.TomadorPagador?.Cliente, cte.TomadorPagador?.GrupoPessoas, null, null, null, cte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Cancelamento))
                return false;

            if (cte.ModeloDocumentoFiscal.DiferenciarMovimentosParaValorLiquido)
            {
                if (!svcMovimento.GerarMovimentacao(out erro, tipoMovimentoValorLiquido, dataMovimentacao, cte.ValorFrete, cte.Numero.ToString() + "-" + cte.Serie.Numero.ToString(), observacaoMovimentacao + " Valor do frete líquido.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cte.TomadorPagador?.Cliente, cte.TomadorPagador?.GrupoPessoas, null, null, null, cte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Cancelamento))
                    return false;
            }

            if (cte.ModeloDocumentoFiscal.DiferenciarMovimentosParaImpostos)
            {
                decimal valorImposto = 0m;
                string tipoImposto = string.Empty;

                if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS || cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                {
                    valorImposto = cte.ValorISS;
                    tipoImposto = "ISS";
                }
                else
                {
                    valorImposto = cte.ValorICMS;
                    tipoImposto = "ICMS";
                }

                if (!svcMovimento.GerarMovimentacao(out erro, tipoMovimentoImpostos, dataMovimentacao, valorImposto, cte.Numero.ToString() + "-" + cte.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: " + tipoImposto + ".", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cte.TomadorPagador?.Cliente, cte.TomadorPagador?.GrupoPessoas, null, null, null, cte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Cancelamento))
                    return false;
            }

            if (cte.ModeloDocumentoFiscal.DiferenciarMovimentosParaPIS)
                if (!svcMovimento.GerarMovimentacao(out erro, tipoMovimentoPIS, dataMovimentacao, cte.ValorPIS, cte.Numero.ToString() + "-" + cte.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: PIS.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cte.TomadorPagador?.Cliente, cte.TomadorPagador?.GrupoPessoas, null, null, null, cte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Cancelamento))
                    return false;

            if (cte.ModeloDocumentoFiscal.DiferenciarMovimentosParaCOFINS)
                if (!svcMovimento.GerarMovimentacao(out erro, tipoMovimentoCOFINS, dataMovimentacao, cte.ValorCOFINS, cte.Numero.ToString() + "-" + cte.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: COFINS.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cte.TomadorPagador?.Cliente, cte.TomadorPagador?.GrupoPessoas, null, null, null, cte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Cancelamento))
                    return false;

            if (cte.ModeloDocumentoFiscal.DiferenciarMovimentosParaIR)
                if (!svcMovimento.GerarMovimentacao(out erro, tipoMovimentoIR, dataMovimentacao, cte.ValorIR, cte.Numero.ToString() + "-" + cte.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: IR.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cte.TomadorPagador?.Cliente, cte.TomadorPagador?.GrupoPessoas, null, null, null, cte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Cancelamento))
                    return false;

            if (cte.ModeloDocumentoFiscal.DiferenciarMovimentosParaCSLL)
                if (!svcMovimento.GerarMovimentacao(out erro, tipoMovimentoCSLL, dataMovimentacao, cte.ValorCSLL, cte.Numero.ToString() + "-" + cte.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: CSLL.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cte.TomadorPagador?.Cliente, cte.TomadorPagador?.GrupoPessoas, null, null, null, cte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Cancelamento))
                    return false;

            if (cte.IncluirISSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim && cte.ValorISS > 0m)
            {
                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS);

                if (componenteFrete != null && componenteFrete.GerarMovimentoAutomatico)
                {
                    Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimentoComponente = null;

                    if (cte.Status == "Z")
                        tipoMovimentoComponente = componenteFrete.TipoMovimentoAnulacao;
                    else
                        tipoMovimentoComponente = componenteFrete.TipoMovimentoCancelamento;

                    if (!svcMovimento.GerarMovimentacao(out erro, tipoMovimentoComponente, dataMovimentacao, Math.Abs(cte.ValorISS), cte.Numero.ToString() + "-" + cte.Serie.Numero.ToString(), observacaoMovimentacao + " Componente: " + componenteFrete.Descricao + ".", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cte.TomadorPagador?.Cliente, cte.TomadorPagador?.GrupoPessoas, null, null, null, cte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Cancelamento))
                        return false;
                }
            }
            else if (cte.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim && cte.ValorICMS > 0m && cte.CST != "60")
            {
                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS);

                if (componenteFrete != null && componenteFrete.GerarMovimentoAutomatico)
                {
                    Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimentoComponente = null;

                    if (cte.Status == "Z")
                        tipoMovimentoComponente = componenteFrete.TipoMovimentoAnulacao;
                    else
                        tipoMovimentoComponente = componenteFrete.TipoMovimentoCancelamento;

                    if (!svcMovimento.GerarMovimentacao(out erro, tipoMovimentoComponente, dataMovimentacao, Math.Abs(cte.ValorICMS), cte.Numero.ToString() + "-" + cte.Serie.Numero.ToString(), observacaoMovimentacao + " Componente: " + componenteFrete.Descricao + ".", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cte.TomadorPagador?.Cliente, cte.TomadorPagador?.GrupoPessoas, null, null, null, cte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Cancelamento))
                        return false;
                }
            }

            erro = string.Empty;
            return true;
        }

        public static void RemoverDocumentosCargaComDesacordo(Dominio.Entidades.Embarcador.Cargas.Carga carga, int codigoCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Documentos.ControleDocumento repControleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Documentos.HistoricoIrregularidade repHistoricoIrregularidade = new Repositorio.Embarcador.Documentos.HistoricoIrregularidade(unitOfWork);


            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarPorCarga(carga.Codigo);
            List<int> codigosCTes;

            if (codigoCTe > 0)
                cargaCTes = cargaCTes.Where(x => x.CTe.Codigo == codigoCTe).ToList();

            if (cargaCTes != null)
            {
                codigosCTes = cargaCTes.Select(x => x.CTe.Codigo).ToList();

                List<Dominio.Entidades.Embarcador.Documentos.ControleDocumento> controleDocumentos = repControleDocumento.BuscarPorCodigosCTes(codigosCTes);

                if (controleDocumentos != null)
                {
                    if (controleDocumentos.All(o => o.MotivoDesacordo != null))
                    {
                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
                        {
                            if (cargaCTe.SistemaEmissor == SistemaEmissor.MultiCTe)
                            {
                                cargaCTe.CTe.Status = "Z";
                                repCTe.Atualizar(cargaCTe.CTe);
                            }
                            else
                            {
                                cargaCTe.CTe = null;
                                repCargaCTe.Atualizar(cargaCTe);
                            }

                            Dominio.Entidades.Embarcador.Documentos.ControleDocumento controleDocumento = controleDocumentos.Where(o => o.CTe.Codigo == cargaCTe.CTe.Codigo).FirstOrDefault();
                            List<Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade> historico = repHistoricoIrregularidade.BuscarPendentesPorControleDocumento(controleDocumento.Codigo);

                            foreach (Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade item in historico)
                            {
                                item.SituacaoIrregularidade = SituacaoIrregularidade.CTECancelado;
                                repHistoricoIrregularidade.Atualizar(item);
                            }
                        }

                        carga.SituacaoCarga = SituacaoCarga.AgNFe;
                    }
                    else
                    {
                        throw new ControllerException(Localization.Resources.Cargas.CancelamentoCarga.ParaRealizarAAnulacaoENecessarioQueTodosOsDocumentosASeremCanceladosEstejamEmDesacordo);
                    }
                }
            }
        }

        public static bool ValidarDocumentosCargaCancelamento(Dominio.Entidades.Embarcador.Cargas.Carga carga, out bool ctePermite, out bool mdfePermite, out bool averbacaoPermite, out bool ciotPermite, Repositorio.UnitOfWork unitOfWork)
        {

            ctePermite = true;
            mdfePermite = true;
            ciotPermite = true;
            averbacaoPermite = true;
            bool emitiuTodosCte = true;
            bool problemaEmissao = false;

            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesCarga = repCTe.BuscarPorCargaEStatus(carga.Codigo, "E");
            List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> mdfesCarga = repMDFe.BuscarPorCargaMDFeEStatus(carga.Codigo, Dominio.Enumeradores.StatusMDFe.Enviado);
            List<Dominio.Entidades.AverbacaoCTe> averbacaoCTe = new List<Dominio.Entidades.AverbacaoCTe>();
            List<Dominio.Entidades.Embarcador.Documentos.CIOT> ciotsCarga = new List<Dominio.Entidades.Embarcador.Documentos.CIOT>();

            if (ctesCarga.Count() > 0)
                ctePermite = false;

            if (mdfesCarga.Count() > 0)
                mdfePermite = false;

            bool ctesSubContratacaoFilialEmissora = false;
            if (carga.EmpresaFilialEmissora != null && carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                ctesSubContratacaoFilialEmissora = true;

            if (repCargaCte.ContarCTePorSituacaoDiff(carga.Codigo, ctesSubContratacaoFilialEmissora, new string[] { "A", "C", "K", "N", "F", "Z" }) > 0)
                emitiuTodosCte = false;

            if (repCargaCte.ContarCTePorListaSituacao(carga.Codigo, ctesSubContratacaoFilialEmissora, new string[] { "R", "D", "I", "L" }) > 0)
            {
                problemaEmissao = true;
            }

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesExcederamLimiteTempo = null;
            if (carga.ControlaTempoParaEmissao)
            {
                ctesExcederamLimiteTempo = repCargaCte.BuscarCTesPorCargaETempoLimiteEmissao(carga.Codigo, ctesSubContratacaoFilialEmissora, 25);
                if (ctesExcederamLimiteTempo.Count > 0)
                {
                    problemaEmissao = true;
                }
            }

            if (!problemaEmissao)
            {
                if (emitiuTodosCte)
                {
                    averbacaoCTe = repAverbacaoCTe.BuscarPorCargaESituacao(carga.Codigo, Dominio.Enumeradores.StatusAverbacaoCTe.Enviado);

                    if (averbacaoCTe.Count() > 0)
                        averbacaoPermite = false;

                    ciotsCarga = repCIOT.BuscarPorCargaEStatus(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao);
                    if (ciotsCarga.Count() > 0)
                        ciotPermite = false;
                }
            }

            if (!ctePermite || !mdfePermite || !ciotPermite || !averbacaoPermite)
                return false;
            else
                return true;
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private static Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas ObterModalidadeTransportador(Dominio.Entidades.Cliente terceiro, Repositorio.UnitOfWork unitOfWork)
        {
            if (terceiro == null)
                return null;

            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTerceiro = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unitOfWork);
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = repModalidadeTerceiro.BuscarPorPessoa(terceiro.CPF_CNPJ);
            return modalidadeTerceiro;
        }

        /// <summary>
        /// #33420 - ASSAI, esse procedimento é executado apenas quando o cancelamento de um cargaPedido for oriundo de um saldo do produto, ou seja,
        /// parte do produto do pedido está na carga a ser cancelada e parte não. Alem disso o parametro LiberarPedidosParaMontagemCarga deve ser FALSE, caso contrario, 
        /// libera todo o saldo do produto do pedido para nova montagem.
        /// </summary>
        /// <param name="cargaPedido"></param>
        /// <param name="unitOfWork"></param>
        private static void AtualizarQuantidadePedidoProdutoCancelamentoCargaPedido(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
            decimal pesoDescartado = 0;
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto in cargaPedido.Produtos)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto = repPedidoProduto.BuscarPorPedidoProduto(cargaPedido.Pedido.Codigo, cargaPedidoProduto.Produto.Codigo);

                if (pedidoProduto != null)
                {
                    decimal m3 = pedidoProduto.MetroCubico / (pedidoProduto.Quantidade > 0 ? pedidoProduto.Quantidade : 1);
                    decimal pallet = pedidoProduto.QuantidadePalet / (pedidoProduto.Quantidade > 0 ? pedidoProduto.Quantidade : 1);
                    pesoDescartado += pedidoProduto.PesoUnitario * cargaPedidoProduto.Quantidade; // (pedidoProduto.Quantidade > 0 ? pedidoProduto.Quantidade : 1);

                    pedidoProduto.Quantidade -= cargaPedidoProduto.Quantidade;
                    pedidoProduto.MetroCubico = m3 * (pedidoProduto.Quantidade > 0 ? pedidoProduto.Quantidade : 1);
                    pedidoProduto.QuantidadePalet = pallet * (pedidoProduto.Quantidade > 0 ? pedidoProduto.Quantidade : 1);

                    repPedidoProduto.Atualizar(pedidoProduto);
                }
            }

            cargaPedido.Pedido.PesoTotal -= pesoDescartado;
        }

        private static bool CancelarContratoFreteTerceiro(out string erro, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (carga == null)
            {
                erro = string.Empty;
                return true;
            }

            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCarga(carga.Codigo);

            if (contratoFrete != null)
            {
                if ((contratoFrete.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Finalizada ||
                    contratoFrete.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aprovado)
                    && tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS
                    )
                {
                    erro = "Não é possível cancelar um contrato de frete aprovado/finalizado.";
                    return false;
                }

                string mensagemErro = "";
                Servicos.Embarcador.Terceiros.ContratoFrete.RealizarCancelamentoTotvs(contratoFrete, unidadeTrabalho, out mensagemErro);
                if (!string.IsNullOrWhiteSpace(mensagemErro) && tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    erro = mensagemErro;
                    return false;
                }

                contratoFrete.SituacaoContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Cancelado;

                repContratoFrete.Atualizar(contratoFrete);
            }

            erro = string.Empty;
            return true;
        }

        private static void CancelarEDIIntegracao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Integracao.ControleIntegracaoCargaEDI.AtualizarSituacaoCargaControleEDI(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEDI.Cancelada, unitOfWork);
        }

        /// <summary>
        /// Procedimento para validar se o pedido da carga pedido está totalmente carregado na carga pedido produto.
        /// </summary>
        /// <param name="cargaCancelamento">Dados para cancelamento da carga.</param>
        /// <param name="cargaPedido">Carga pedido que está validando o cancelamento.</param>
        /// <param name="pedidosProdutos">Todos os produtos dos pedidos contidos na carga.</param>
        /// <param name="montagemCarregamentoPedidoProduto">Se o carregamento é de uma sessão de roteirização por montagem pedido produto... (ASSAI)</param>
        /// <returns>False se o pedido está totalmente carregado ou True quando o pedido pode estar em mais de uma carga.</returns>
        private static bool CargaPedidoOriundoSaldo(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidosProdutos, bool montagemCarregamentoPedidoProduto)
        {
            //#32122 - Se não estiver passando liberar para montagem, validar se o pedido não está me mais de uma carga.
            // Se estiver, não devemos cancelar o pedido.
            bool pedidoOriundoSaldo = false;

            // Problema na TelhaNorte aonde não estava cancelando o pedido, adicionado condição de montagem por medido produto.
            if (!cargaCancelamento.LiberarPedidosParaMontagemCarga && montagemCarregamentoPedidoProduto) // (cargaPedido.Carga?.Carregamento?.SessaoRoteirizador?.MontagemCarregamentoPedidoProduto ?? false))
            {
                // Contem os produtos e suas quantidades do pedido
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidoProdutos = (from obj in pedidosProdutos
                                                                                           where obj.Pedido.Codigo == cargaPedido.Pedido.Codigo
                                                                                           select obj).ToList();

                if (pedidoProdutos != null && pedidoProdutos.Count > 0 && cargaPedido.Produtos != null && cargaPedido.Produtos.Count > 0)
                {
                    // Se existe algum produto que a quantidade do produto do pedido é superior a quantidade do produto na carga pedido.
                    // pedidoOriundoSaldo = pedidoProdutos.Any(x => x.Quantidade > (cargaPedido.Produtos.FirstOrDefault(cp => cp.Produto.Codigo == x.Produto.Codigo)?.Quantidade ?? 0) + 0.5m);

                    // Vamos somar as quantidades do pedido produto e a quantidade do produto na carga pedido produto.. 
                    //  Pois na Danone, o mesmo produto apresentou-se mais de 1x no mesmo pedido e sendo assim o pedido não foi cancelado.
                    var pedidoProdutoQtde = pedidoProdutos.GroupBy(x => x.Produto.Codigo)
                                                          .Select(s => new
                                                          {
                                                              CodigoProduto = s.First().Produto.Codigo,
                                                              Quantidade = s.Sum(c => c.Quantidade)
                                                          }).ToList();

                    var cargaPedidoProdutoQtde = cargaPedido.Produtos.GroupBy(x => x.Produto.Codigo)
                                                                     .Select(s => new
                                                                     {
                                                                         CodigoProduto = s.First().Produto.Codigo,
                                                                         Quantidade = s.Sum(c => c.Quantidade)
                                                                     }).ToList();

                    pedidoOriundoSaldo = pedidoProdutoQtde.Any(x => x.Quantidade > (cargaPedidoProdutoQtde.FirstOrDefault(cp => cp.CodigoProduto == x.CodigoProduto)?.Quantidade ?? 0) + 0.5m);
                }

            }

            return pedidoOriundoSaldo;
        }

        private static List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> DisponibilizarFilasCarregamentoVeiculo(int codigoCarga, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Logistica.FilaCarregamentoVeiculo(unitOfWork, OrigemAlteracaoFilaCarregamento.Sistema);
            List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> filasCarregamentoVeiculoDisponibilizadas = servicoFilaCarregamentoVeiculo.DisponibilizarPorCargaCancelada(codigoCarga, tipoServicoMultisoftware);

            return filasCarregamentoVeiculoDisponibilizadas;
        }

        private static void GerarLogCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Dominio.Entidades.Usuario usuario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogCargaCancelamento tipo, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoLog repCargaCancelamentoLog = new Repositorio.Embarcador.Cargas.CargaCancelamentoLog(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoLog log = new Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoLog()
            {
                Acao = tipo,
                CargaCancelamento = cargaCancelamento,
                Data = DateTime.Now,
                Usuario = usuario
            };

            repCargaCancelamentoLog.Inserir(log);
        }

        private static bool ValidarFechamentoDiarioDocumentosCarga(out string erro, Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                Repositorio.Embarcador.Cargas.CargaNFS repCargaNFSe = new Repositorio.Embarcador.Cargas.CargaNFS(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaNFS> cargaNFSes = repCargaNFSe.BuscarPorCarga(cargaCancelamento.Carga.Codigo);

                if (cargaNFSes != null && (cargaNFSes.Count > 0 || cargaCancelamento.Carga.CargaCTes?.Count > 0))
                {
                    List<DateTime> datasDeEmissao = (from obj in cargaNFSes select obj.NotaFiscalServico.NFSe.DataEmissao.Date).ToList();
                    if (cargaCancelamento.Carga.CargaCTes != null && cargaCancelamento.Carga.CargaCTes.Count > 0)
                        datasDeEmissao.AddRange((from obj in cargaCancelamento.Carga.CargaCTes where obj.CTe != null select obj.CTe.DataEmissao.Value.Date));

                    datasDeEmissao = datasDeEmissao.Distinct().ToList();

                    foreach (DateTime dataEmissao in datasDeEmissao)
                    {
                        if (Servicos.Embarcador.Financeiro.FechamentoDiario.VerificarSeExisteFechamento(cargaCancelamento.Carga?.Empresa?.Codigo ?? 0, dataEmissao, unitOfWork))
                        {
                            erro = "Já existe um fechamento diário igual ou posterior à data " + dataEmissao.ToString("dd/MM/yyyy") + ", não sendo possível realizar o cancelamento/anulação da carga.";
                            return false;
                        }
                    }
                }
            }

            erro = string.Empty;
            return true;
        }

        private static bool ValidarSeCargaEstaVinculadaAAlgumDocumento(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unidadeTrabalho, out string erro, out List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulosParaCancelamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            erro = "";
            Repositorio.Embarcador.Fatura.FaturaCarga repFaturaCarga = new Repositorio.Embarcador.Fatura.FaturaCarga(unidadeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoCarga repAcertoCarga = new Repositorio.Embarcador.Acerto.AcertoCarga(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeTrabalho);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unidadeTrabalho);

            titulosParaCancelamento = new List<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCarga(carga.Codigo);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && !configuracaoGeralCarga.CancelarCIOTAutomaticamenteFluxoCancelamentoCarga &&
                contratoFrete != null && (contratoFrete.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Finalizada ||
                                          contratoFrete.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aprovado))
            {
                erro = "A carga está vinculada ao contrato de frete nº " + contratoFrete.NumeroContrato.ToString() + ", que já está aprovado/finalizado, não sendo possível realizar o cancelamento/anulação.";
                return false;
            }

            List<int> numerosFaturas = repFaturaCarga.BuscarNumeroFaturaPorCarga(carga.Codigo);

            if (numerosFaturas.Count > 0)
            {
                erro = "A carga está vinculada à(s) fatura(s) nº " + string.Join(", ", numerosFaturas) + ", não sendo possível realizar o cancelamento/anulação.";
                return false;
            }

            List<int> numeroAcertosViagem = repAcertoCarga.BuscarNumeroAcertoPorCarga(carga.Codigo);

            if (numeroAcertosViagem.Count > 0)
            {
                erro = "A carga está vinculada ao(s) acerto(s) de viagem nº " + string.Join(", ", numeroAcertosViagem) + ", não sendo possível realizar o cancelamento/anulação.";
                return false;
            }

            if (carga.CargaTransbordo)
                return true;

            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeFirst = carga.CargaCTes != null && carga.CargaCTes.Count > 0 ? carga.CargaCTes.FirstOrDefault() : null;
            bool documentoFinanceiroPorCarga = repDocumentoFaturamento.ExistePorCarga(carga.Codigo);

            bool documentoFinanceiroPorCTe = false;
            if (cargaCTeFirst != null && cargaCTeFirst.CTe != null)
                documentoFinanceiroPorCTe = repDocumentoFaturamento.ExistePorCTe(carga.Codigo);

            if (documentoFinanceiroPorCarga)
            {
                List<int> numerosFaturasNova = repFaturaDocumento.BuscarNumeroFaturaPorCarga(carga.Codigo);
                if (numerosFaturasNova.Count > 0)
                {
                    erro = "A Carga (" + carga.CodigoCargaEmbarcador + ") está vinculada à(s) fatura(s) nº " + string.Join(", ", numerosFaturasNova) + ", não sendo possível realizar o cancelamento/anulação.";
                    return false;
                }

                List<int> numerosTitulos = repTituloDocumento.BuscarNumeroTituloPorCarga(carga.Codigo);
                if (numerosTitulos.Count > 0)
                {
                    erro = "A Carga (" + carga.CodigoCargaEmbarcador + ") está vinculada ao(s) título(s) nº " + string.Join(", ", numerosTitulos) + ", não sendo possível realizar o cancelamento/anulação.";
                    return false;
                }

                List<int> nossoNumeroBoletoTitulos = repTituloDocumento.BuscarNumeroBoletoTituloPorCarga(carga.Codigo);
                if (nossoNumeroBoletoTitulos.Count > 0)
                {
                    erro = "A Carga (" + carga.CodigoCargaEmbarcador + ") está vinculada a boleto(s) no(s) título(s) nº " + string.Join(", ", nossoNumeroBoletoTitulos) + ", não sendo possível realizar o cancelamento/anulação.";
                    return false;
                }

                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulosParaCancelamentoCTe = repTituloDocumento.BuscarTitulosEmAbertoPorCarga(carga.Codigo);
                if (titulosParaCancelamentoCTe.Count > 0)
                    titulosParaCancelamento.AddRange(titulosParaCancelamentoCTe);
            }
            else if (documentoFinanceiroPorCTe)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCtes = repCargaCte.BuscarPorCarga(carga.Codigo);

                for (int i = 0; i < cargaCtes.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = cargaCtes[i];

                    if (cargaCTe.CTe != null)
                    {
                        List<int> numerosFaturasNova = repFaturaDocumento.BuscarNumeroFaturaPorCTe(cargaCTe.CTe.Codigo);
                        if (numerosFaturasNova.Count > 0)
                        {
                            erro = "O CT-e (" + cargaCTe.CTe.Numero + ") está vinculado à(s) fatura(s) nº " + string.Join(", ", numerosFaturasNova) + ", não sendo possível realizar o cancelamento/anulação.";
                            return false;
                        }

                        List<int> numerosTitulos = repTituloDocumento.BuscarNumeroTituloPorCTe(cargaCTe.CTe.Codigo);
                        if (numerosTitulos.Count > 0)
                        {
                            erro = "O CT-e (" + cargaCTe.CTe.Numero + ") está vinculado ao(s) título(s) nº " + string.Join(", ", numerosTitulos) + ", não sendo possível realizar o cancelamento/anulação.";
                            return false;
                        }

                        List<int> nossoNumeroBoletoTitulos = repTituloDocumento.BuscarNumeroBoletoTituloPorCTe(cargaCTe.CTe.Codigo);
                        if (nossoNumeroBoletoTitulos.Count > 0)
                        {
                            erro = "O CT-e (" + cargaCTe.CTe.Numero + ") está vinculado a boleto(s) no(s) título(s) nº " + string.Join(", ", nossoNumeroBoletoTitulos) + ", não sendo possível realizar o cancelamento/anulação.";
                            return false;
                        }

                        List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulosParaCancelamentoCTe = repTituloDocumento.BuscarTitulosEmAbertoPorCTe(cargaCTe.CTe.Codigo);

                        if (titulosParaCancelamentoCTe.Count > 0)
                            titulosParaCancelamento.AddRange(titulosParaCancelamentoCTe);
                    }
                }
            }

            return true;
        }

        private static bool GerarMovimentosCancelamentoCarga(out string erro, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                erro = string.Empty;
                return true;
            }

            if (carga.CargaTransbordo)
            {
                erro = string.Empty;
                return true;
            }

            if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada && carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada)
            {
                erro = string.Empty;
                return true;
            }

            if (!carga.DataFinalizacaoEmissao.HasValue) //não gerou movimentação de autorização, gera as movimentações de autorização antes de gerar as de cancelamento
            {
                Servicos.Embarcador.Carga.Documentos svcDocumentos = new Documentos(unidadeTrabalho);
                svcDocumentos.GerarMovimentosAutorizacaoCarga(ref carga, unidadeTrabalho, tipoServicoMultisoftware, true, false);
            }

            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarPorCargaSemComplementares(carga.Codigo);

            if (cargaCTes.Count > 0)
            {
                if (Servicos.Embarcador.Carga.Carga.VerificarSeGeraMovimentacaoAgrupadaPorPedido(carga, unidadeTrabalho))
                {
                    if (!GerarMovimentoCancelamentoCarga(out erro, carga, tipoServicoMultisoftware, unidadeTrabalho, stringConexao))
                        return false;
                }
                else
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
                    {
                        if (!GerarMovimentoCancelamentoCTe(out erro, cargaCTe, tipoServicoMultisoftware, unidadeTrabalho, stringConexao))
                            return false;
                    }
                }
            }

            erro = string.Empty;
            return true;
        }

        private static bool GerarMovimentoCancelamentoCarga(out string erro, Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao)
        {
            if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada && carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada)
            {
                erro = string.Empty;
                return true;
            }

            if (carga.CargaTransbordo)
            {
                erro = string.Empty;
                return true;
            }

            Servicos.Embarcador.Financeiro.ProcessoMovimento svcMovimento = new Financeiro.ProcessoMovimento(stringConexao);
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponentes = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
            Repositorio.ModeloDocumentoFiscal repModeloDocumento = new Repositorio.ModeloDocumentoFiscal(unidadeTrabalho);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unidadeTrabalho);

            string statusCTe = null;
            List<DateTime> datasMovimentacoes = null;

            if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
            {
                statusCTe = "Z";
                datasMovimentacoes = repCargaCTe.BuscarDatasAnulacaoPorCarga(carga.Codigo, statusCTe);
            }
            else
            {
                statusCTe = "C";
                datasMovimentacoes = repCargaCTe.BuscarDatasCancelamentoPorCarga(carga.Codigo, statusCTe);
            }

            List<int> codigosComponentesFrete = repCargaCTeComponentes.BuscarCodigoComponenteFreteQueGeraMovimentacaoPorCarga(carga.Codigo, statusCTe);
            List<int> codigosModelosDocumento = repCargaCTe.BuscarCodigoModeloDocumentoPorCarga(carga.Codigo, statusCTe);

            foreach (int codigoModeloDocumento in codigosModelosDocumento)
            {
                Dominio.Entidades.ModeloDocumentoFiscal modeloDocumento = repModeloDocumento.BuscarPorId(codigoModeloDocumento);

                Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimento = null,
                                                                      tipoMovimentoImposto = null,
                                                                      tipoMovimentoValorLiquido = null,
                                                                      tipoMovimentoPIS = null,
                                                                      tipoMovimentoCOFINS = null,
                                                                      tipoMovimentoIR = null,
                                                                      tipoMovimentoCSLL = null;

                string observacaoMovimentacao = "dos documentos " + modeloDocumento.Abreviacao + " da carga " + carga.CodigoCargaEmbarcador + ".";

                if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                {
                    observacaoMovimentacao = "Movimento gerado à partir da anulação " + observacaoMovimentacao;
                    tipoMovimento = modeloDocumento.TipoMovimentoAnulacao;
                    tipoMovimentoImposto = modeloDocumento.TipoMovimentoImpostoAnulacao;
                    tipoMovimentoValorLiquido = modeloDocumento.TipoMovimentoValorLiquidoAnulacao;
                    tipoMovimentoCOFINS = modeloDocumento.TipoMovimentoCOFINSAnulacao;
                    tipoMovimentoPIS = modeloDocumento.TipoMovimentoPISAnulacao;
                    tipoMovimentoIR = modeloDocumento.TipoMovimentoIRAnulacao;
                    tipoMovimentoCSLL = modeloDocumento.TipoMovimentoCSLLAnulacao;
                }
                else
                {
                    observacaoMovimentacao = "Movimento gerado à partir do cancelamento " + observacaoMovimentacao;
                    tipoMovimento = modeloDocumento.TipoMovimentoCancelamento;
                    tipoMovimentoImposto = modeloDocumento.TipoMovimentoImpostoCancelamento;
                    tipoMovimentoValorLiquido = modeloDocumento.TipoMovimentoValorLiquidoCancelamento;
                    tipoMovimentoCOFINS = modeloDocumento.TipoMovimentoCOFINSCancelamento;
                    tipoMovimentoPIS = modeloDocumento.TipoMovimentoPISCancelamento;
                    tipoMovimentoIR = modeloDocumento.TipoMovimentoIRCancelamento;
                    tipoMovimentoCSLL = modeloDocumento.TipoMovimentoCSLLCancelamento;
                }

                if (modeloDocumento.TipoMovimentoValorLiquidoCancelamentoPropriaNacional != null && modeloDocumento.TipoMovimentoValorLiquidoCancelamentoAgregadoNacional != null && modeloDocumento.TipoMovimentoValorLiquidoCancelamentoTerceiroNacional != null &&
                    modeloDocumento.TipoMovimentoValorLiquidoCancelamentoPropriaInternacional != null && modeloDocumento.TipoMovimentoValorLiquidoCancelamentoAgregadoInternacional != null && modeloDocumento.TipoMovimentoValorLiquidoCancelamentoTerceiroInternacional != null)
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico primeiroCTe = repCargaCTe.BuscarPrimeiroCTePorCarga(carga.Codigo);
                    bool cteNacional = primeiroCTe != null && primeiroCTe.LocalidadeInicioPrestacao.Pais == null || primeiroCTe.LocalidadeTerminoPrestacao.Pais == null && (primeiroCTe.LocalidadeInicioPrestacao.Pais.Sigla == "01058" && primeiroCTe.LocalidadeTerminoPrestacao.Pais.Sigla == "01058");
                    bool cteProprio = carga.Terceiro == null;
                    bool cteAgregado = false;
                    bool cteTerceiro = false;
                    if (!cteProprio)
                    {
                        Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = ObterModalidadeTransportador(carga.Terceiro, unidadeTrabalho);

                        cteAgregado = modalidadeTerceiro != null && modalidadeTerceiro.TipoTransportador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo.TACAgregado;
                        cteTerceiro = modalidadeTerceiro == null || modalidadeTerceiro.TipoTransportador != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo.TACAgregado;
                    }
                    if (cteNacional && cteProprio)
                        tipoMovimentoValorLiquido = modeloDocumento.TipoMovimentoValorLiquidoCancelamentoPropriaNacional;
                    else if (!cteNacional && cteProprio)
                        tipoMovimentoValorLiquido = modeloDocumento.TipoMovimentoValorLiquidoCancelamentoPropriaInternacional;
                    else if (cteNacional && cteAgregado)
                        tipoMovimentoValorLiquido = modeloDocumento.TipoMovimentoValorLiquidoCancelamentoAgregadoNacional;
                    else if (!cteNacional && cteAgregado)
                        tipoMovimentoValorLiquido = modeloDocumento.TipoMovimentoValorLiquidoCancelamentoAgregadoInternacional;
                    else if (cteNacional && cteTerceiro)
                        tipoMovimentoValorLiquido = modeloDocumento.TipoMovimentoValorLiquidoCancelamentoTerceiroNacional;
                    else if (!cteNacional && cteTerceiro)
                        tipoMovimentoValorLiquido = modeloDocumento.TipoMovimentoValorLiquidoCancelamentoTerceiroInternacional;
                }

                foreach (DateTime dataMovimentacao in datasMovimentacoes)
                {
                    decimal valorMovimentacao = 0m;

                    if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                        valorMovimentacao = repCargaCTe.BuscarValorTotalReceberPorCargaEModeloDocumento(carga.Codigo, codigoModeloDocumento, statusCTe, null, null, dataMovimentacao);
                    else
                        valorMovimentacao = repCargaCTe.BuscarValorTotalReceberPorCargaEModeloDocumento(carga.Codigo, codigoModeloDocumento, statusCTe, null, dataMovimentacao, null);

                    svcMovimento.GerarMovimentacao(tipoMovimento, dataMovimentacao, valorMovimentacao, carga.CodigoCargaEmbarcador, observacaoMovimentacao, unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Carga, tipoServicoMultisoftware);

                    if (modeloDocumento.DiferenciarMovimentosParaValorLiquido)
                    {
                        decimal valorFreteLiquido = 0m;

                        if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                            valorFreteLiquido = repCargaCTe.BuscarValorFreteLiquidoPorCargaEModeloDocumento(carga.Codigo, codigoModeloDocumento, statusCTe, null, null, dataMovimentacao);
                        else
                            valorFreteLiquido = repCargaCTe.BuscarValorFreteLiquidoPorCargaEModeloDocumento(carga.Codigo, codigoModeloDocumento, statusCTe, null, dataMovimentacao, null);

                        if (!svcMovimento.GerarMovimentacao(out erro, tipoMovimentoValorLiquido, dataMovimentacao, valorFreteLiquido, carga.CodigoCargaEmbarcador, observacaoMovimentacao + " Valor do frete líquido.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Carga, tipoServicoMultisoftware))
                            return false;
                    }

                    if (modeloDocumento.DiferenciarMovimentosParaImpostos)
                    {
                        decimal valorMovimentacaoImpostos = 0m;

                        if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                            valorMovimentacaoImpostos = repCargaCTe.BuscarValorICMSPorCargaEModeloDocumento(carga.Codigo, codigoModeloDocumento, statusCTe, null, null, dataMovimentacao);
                        else
                            valorMovimentacaoImpostos = repCargaCTe.BuscarValorICMSPorCargaEModeloDocumento(carga.Codigo, codigoModeloDocumento, statusCTe, null, dataMovimentacao, null);

                        if (!svcMovimento.GerarMovimentacao(out erro, tipoMovimentoImposto, dataMovimentacao, valorMovimentacaoImpostos, carga.CodigoCargaEmbarcador, observacaoMovimentacao + " Imposto: ICMS.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Carga, tipoServicoMultisoftware))
                            return false;
                    }

                    if (modeloDocumento.DiferenciarMovimentosParaPIS)
                    {
                        decimal valorMovimentacaoPIS = 0m;

                        if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                            valorMovimentacaoPIS = repCargaCTe.BuscarValorPISPorCargaEModeloDocumento(carga.Codigo, codigoModeloDocumento, statusCTe, null, null, dataMovimentacao);
                        else
                            valorMovimentacaoPIS = repCargaCTe.BuscarValorPISPorCargaEModeloDocumento(carga.Codigo, codigoModeloDocumento, statusCTe, null, dataMovimentacao, null);

                        if (!svcMovimento.GerarMovimentacao(out erro, tipoMovimentoPIS, dataMovimentacao, valorMovimentacaoPIS, carga.CodigoCargaEmbarcador, observacaoMovimentacao + " Imposto: PIS.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Carga, tipoServicoMultisoftware))
                            return false;
                    }

                    if (modeloDocumento.DiferenciarMovimentosParaCOFINS)
                    {
                        decimal valorMovimentacaoCOFINS = 0m;

                        if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                            valorMovimentacaoCOFINS = repCargaCTe.BuscarValorCOFINSPorCargaEModeloDocumento(carga.Codigo, codigoModeloDocumento, statusCTe, null, null, dataMovimentacao);
                        else
                            valorMovimentacaoCOFINS = repCargaCTe.BuscarValorCOFINSPorCargaEModeloDocumento(carga.Codigo, codigoModeloDocumento, statusCTe, null, dataMovimentacao, null);

                        if (!svcMovimento.GerarMovimentacao(out erro, tipoMovimentoCOFINS, dataMovimentacao, valorMovimentacaoCOFINS, carga.CodigoCargaEmbarcador, observacaoMovimentacao + " Imposto: COFINS.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Carga, tipoServicoMultisoftware))
                            return false;
                    }

                    if (modeloDocumento.DiferenciarMovimentosParaIR)
                    {
                        decimal valorMovimentacaoIR = 0m;

                        if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                            valorMovimentacaoIR = repCargaCTe.BuscarValorIRPorCargaEModeloDocumento(carga.Codigo, codigoModeloDocumento, statusCTe, null, null, dataMovimentacao);
                        else
                            valorMovimentacaoIR = repCargaCTe.BuscarValorIRPorCargaEModeloDocumento(carga.Codigo, codigoModeloDocumento, statusCTe, null, dataMovimentacao, null);

                        if (!svcMovimento.GerarMovimentacao(out erro, tipoMovimentoIR, dataMovimentacao, valorMovimentacaoIR, carga.CodigoCargaEmbarcador, observacaoMovimentacao + " Imposto: IR.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Carga, tipoServicoMultisoftware))
                            return false;
                    }

                    if (modeloDocumento.DiferenciarMovimentosParaCSLL)
                    {
                        decimal valorMovimentacaoCSLL = 0m;

                        if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                            valorMovimentacaoCSLL = repCargaCTe.BuscarValorCSLLPorCargaEModeloDocumento(carga.Codigo, codigoModeloDocumento, statusCTe, null, null, dataMovimentacao);
                        else
                            valorMovimentacaoCSLL = repCargaCTe.BuscarValorCSLLPorCargaEModeloDocumento(carga.Codigo, codigoModeloDocumento, statusCTe, null, dataMovimentacao, null);

                        if (!svcMovimento.GerarMovimentacao(out erro, tipoMovimentoCSLL, dataMovimentacao, valorMovimentacaoCSLL, carga.CodigoCargaEmbarcador, observacaoMovimentacao + " Imposto: CSLL.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Carga, tipoServicoMultisoftware))
                            return false;
                    }

                    foreach (int codigoComponenteFrete in codigosComponentesFrete)
                    {
                        Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorCodigo(codigoComponenteFrete);
                        Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimentoComponente = null;

                        if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                            tipoMovimentoComponente = componenteFrete.TipoMovimentoAnulacao;
                        else
                            tipoMovimentoComponente = componenteFrete.TipoMovimentoCancelamento;

                        decimal valorMovimentacaoComponente = 0m;

                        if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                            valorMovimentacaoComponente = Math.Abs(repCargaCTeComponentes.BuscarValorComponentePorCargaEModeloDocumento(carga.Codigo, codigoComponenteFrete, codigoModeloDocumento, statusCTe, null, null, dataMovimentacao));
                        else
                            valorMovimentacaoComponente = Math.Abs(repCargaCTeComponentes.BuscarValorComponentePorCargaEModeloDocumento(carga.Codigo, codigoComponenteFrete, codigoModeloDocumento, statusCTe, null, dataMovimentacao, null));

                        if (!svcMovimento.GerarMovimentacao(out erro, tipoMovimentoComponente, dataMovimentacao, valorMovimentacaoComponente, carga.CodigoCargaEmbarcador, observacaoMovimentacao + " Componente: " + componenteFrete.Descricao + ".", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Carga, tipoServicoMultisoftware))
                            return false;
                    }
                }
            }

            erro = string.Empty;
            return true;
        }

        private static bool GerarMovimentoCancelamentoCarga(out string erro, Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao)
        {
            //if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada && carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada)
            //{
            //    erro = string.Empty;
            //    return true;
            //}

            if (cargaCancelamento.Carga.CargaTransbordo)
            {
                erro = string.Empty;
                return true;
            }

            Servicos.Embarcador.Financeiro.ProcessoMovimento svcMovimento = new Financeiro.ProcessoMovimento(stringConexao);
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponentes = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
            Repositorio.ModeloDocumentoFiscal repModeloDocumento = new Repositorio.ModeloDocumentoFiscal(unidadeTrabalho);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unidadeTrabalho);

            string statusCTe = null;
            List<DateTime> datasMovimentacoes = null;


            if (cargaCancelamento.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Anulacao)
            {
                statusCTe = "Z";
                datasMovimentacoes = repCargaCTe.BuscarDatasAnulacaoPorCarga(cargaCancelamento.Carga.Codigo, statusCTe);
            }
            else
            {
                statusCTe = "C";
                datasMovimentacoes = repCargaCTe.BuscarDatasCancelamentoPorCarga(cargaCancelamento.Carga.Codigo, statusCTe);
            }

            List<int> codigosComponentesFrete = repCargaCTeComponentes.BuscarCodigoComponenteFreteQueGeraMovimentacaoPorCarga(cargaCancelamento.Carga.Codigo, statusCTe);
            List<int> codigosModelosDocumento = repCargaCTe.BuscarCodigoModeloDocumentoPorCarga(cargaCancelamento.Carga.Codigo, statusCTe);

            foreach (int codigoModeloDocumento in codigosModelosDocumento)
            {
                Dominio.Entidades.ModeloDocumentoFiscal modeloDocumento = repModeloDocumento.BuscarPorId(codigoModeloDocumento);

                Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimento = null,
                                                                      tipoMovimentoImposto = null,
                                                                      tipoMovimentoValorLiquido = null,
                                                                      tipoMovimentoPIS = null,
                                                                      tipoMovimentoCOFINS = null,
                                                                      tipoMovimentoIR = null,
                                                                      tipoMovimentoCSLL = null;

                string observacaoMovimentacao = "dos documentos " + modeloDocumento.Abreviacao + " da carga " + cargaCancelamento.Carga.CodigoCargaEmbarcador + ".";

                if (cargaCancelamento.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Anulacao)
                {
                    observacaoMovimentacao = "Movimento gerado à partir da anulação " + observacaoMovimentacao;
                    tipoMovimento = modeloDocumento.TipoMovimentoAnulacao;
                    tipoMovimentoImposto = modeloDocumento.TipoMovimentoImpostoAnulacao;
                    tipoMovimentoValorLiquido = modeloDocumento.TipoMovimentoValorLiquidoAnulacao;
                    tipoMovimentoCOFINS = modeloDocumento.TipoMovimentoCOFINSAnulacao;
                    tipoMovimentoPIS = modeloDocumento.TipoMovimentoPISAnulacao;
                    tipoMovimentoIR = modeloDocumento.TipoMovimentoIRAnulacao;
                    tipoMovimentoCSLL = modeloDocumento.TipoMovimentoCSLLAnulacao;
                }
                else
                {
                    observacaoMovimentacao = "Movimento gerado à partir do cancelamento " + observacaoMovimentacao;
                    tipoMovimento = modeloDocumento.TipoMovimentoCancelamento;
                    tipoMovimentoImposto = modeloDocumento.TipoMovimentoImpostoCancelamento;
                    tipoMovimentoValorLiquido = modeloDocumento.TipoMovimentoValorLiquidoCancelamento;
                    tipoMovimentoCOFINS = modeloDocumento.TipoMovimentoCOFINSCancelamento;
                    tipoMovimentoPIS = modeloDocumento.TipoMovimentoPISCancelamento;
                    tipoMovimentoIR = modeloDocumento.TipoMovimentoIRCancelamento;
                    tipoMovimentoCSLL = modeloDocumento.TipoMovimentoCSLLCancelamento;
                }

                if (modeloDocumento.TipoMovimentoValorLiquidoCancelamentoPropriaNacional != null && modeloDocumento.TipoMovimentoValorLiquidoCancelamentoAgregadoNacional != null && modeloDocumento.TipoMovimentoValorLiquidoCancelamentoTerceiroNacional != null &&
                    modeloDocumento.TipoMovimentoValorLiquidoCancelamentoPropriaInternacional != null && modeloDocumento.TipoMovimentoValorLiquidoCancelamentoAgregadoInternacional != null && modeloDocumento.TipoMovimentoValorLiquidoCancelamentoTerceiroInternacional != null)
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico primeiroCTe = repCargaCTe.BuscarPrimeiroCTePorCarga(cargaCancelamento.Carga.Codigo);
                    bool cteNacional = primeiroCTe != null && primeiroCTe.LocalidadeInicioPrestacao.Pais == null || primeiroCTe.LocalidadeTerminoPrestacao.Pais == null && (primeiroCTe.LocalidadeInicioPrestacao.Pais.Sigla == "01058" && primeiroCTe.LocalidadeTerminoPrestacao.Pais.Sigla == "01058");
                    bool cteProprio = cargaCancelamento.Carga.Terceiro == null;
                    bool cteAgregado = false;
                    bool cteTerceiro = false;
                    if (!cteProprio)
                    {
                        Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = ObterModalidadeTransportador(cargaCancelamento.Carga.Terceiro, unidadeTrabalho);

                        cteAgregado = modalidadeTerceiro != null && modalidadeTerceiro.TipoTransportador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo.TACAgregado;
                        cteTerceiro = modalidadeTerceiro == null || modalidadeTerceiro.TipoTransportador != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo.TACAgregado;
                    }
                    if (cteNacional && cteProprio)
                        tipoMovimentoValorLiquido = modeloDocumento.TipoMovimentoValorLiquidoCancelamentoPropriaNacional;
                    else if (!cteNacional && cteProprio)
                        tipoMovimentoValorLiquido = modeloDocumento.TipoMovimentoValorLiquidoCancelamentoPropriaInternacional;
                    else if (cteNacional && cteAgregado)
                        tipoMovimentoValorLiquido = modeloDocumento.TipoMovimentoValorLiquidoCancelamentoAgregadoNacional;
                    else if (!cteNacional && cteAgregado)
                        tipoMovimentoValorLiquido = modeloDocumento.TipoMovimentoValorLiquidoCancelamentoAgregadoInternacional;
                    else if (cteNacional && cteTerceiro)
                        tipoMovimentoValorLiquido = modeloDocumento.TipoMovimentoValorLiquidoCancelamentoTerceiroNacional;
                    else if (!cteNacional && cteTerceiro)
                        tipoMovimentoValorLiquido = modeloDocumento.TipoMovimentoValorLiquidoCancelamentoTerceiroInternacional;
                }

                foreach (DateTime dataMovimentacao in datasMovimentacoes)
                {
                    decimal valorMovimentacao = 0m;

                    if (cargaCancelamento.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Anulacao)
                        valorMovimentacao = repCargaCTe.BuscarValorTotalReceberPorCargaEModeloDocumento(cargaCancelamento.Carga.Codigo, codigoModeloDocumento, statusCTe, null, null, dataMovimentacao);
                    else
                        valorMovimentacao = repCargaCTe.BuscarValorTotalReceberPorCargaEModeloDocumento(cargaCancelamento.Carga.Codigo, codigoModeloDocumento, statusCTe, null, dataMovimentacao, null);

                    svcMovimento.GerarMovimentacao(tipoMovimento, dataMovimentacao, valorMovimentacao, cargaCancelamento.Carga.CodigoCargaEmbarcador, observacaoMovimentacao, unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Carga, tipoServicoMultisoftware);

                    if (modeloDocumento.DiferenciarMovimentosParaValorLiquido)
                    {
                        decimal valorFreteLiquido = 0m;

                        if (cargaCancelamento.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Anulacao)
                            valorFreteLiquido = repCargaCTe.BuscarValorFreteLiquidoPorCargaEModeloDocumento(cargaCancelamento.Carga.Codigo, codigoModeloDocumento, statusCTe, null, null, dataMovimentacao);
                        else
                            valorFreteLiquido = repCargaCTe.BuscarValorFreteLiquidoPorCargaEModeloDocumento(cargaCancelamento.Carga.Codigo, codigoModeloDocumento, statusCTe, null, dataMovimentacao, null);

                        if (!svcMovimento.GerarMovimentacao(out erro, tipoMovimentoValorLiquido, dataMovimentacao, valorFreteLiquido, cargaCancelamento.Carga.CodigoCargaEmbarcador, observacaoMovimentacao + " Valor do frete líquido.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Carga, tipoServicoMultisoftware))
                            return false;
                    }

                    if (modeloDocumento.DiferenciarMovimentosParaImpostos)
                    {
                        decimal valorMovimentacaoImpostos = 0m;

                        if (cargaCancelamento.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Anulacao)
                            valorMovimentacaoImpostos = repCargaCTe.BuscarValorICMSPorCargaEModeloDocumento(cargaCancelamento.Carga.Codigo, codigoModeloDocumento, statusCTe, null, null, dataMovimentacao);
                        else
                            valorMovimentacaoImpostos = repCargaCTe.BuscarValorICMSPorCargaEModeloDocumento(cargaCancelamento.Carga.Codigo, codigoModeloDocumento, statusCTe, null, dataMovimentacao, null);

                        if (!svcMovimento.GerarMovimentacao(out erro, tipoMovimentoImposto, dataMovimentacao, valorMovimentacaoImpostos, cargaCancelamento.Carga.CodigoCargaEmbarcador, observacaoMovimentacao + " Imposto: ICMS.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Carga, tipoServicoMultisoftware))
                            return false;
                    }

                    if (modeloDocumento.DiferenciarMovimentosParaPIS)
                    {
                        decimal valorMovimentacaoPIS = 0m;

                        if (cargaCancelamento.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Anulacao)
                            valorMovimentacaoPIS = repCargaCTe.BuscarValorPISPorCargaEModeloDocumento(cargaCancelamento.Carga.Codigo, codigoModeloDocumento, statusCTe, null, null, dataMovimentacao);
                        else
                            valorMovimentacaoPIS = repCargaCTe.BuscarValorPISPorCargaEModeloDocumento(cargaCancelamento.Carga.Codigo, codigoModeloDocumento, statusCTe, null, dataMovimentacao, null);

                        if (!svcMovimento.GerarMovimentacao(out erro, tipoMovimentoPIS, dataMovimentacao, valorMovimentacaoPIS, cargaCancelamento.Carga.CodigoCargaEmbarcador, observacaoMovimentacao + " Imposto: PIS.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Carga, tipoServicoMultisoftware))
                            return false;
                    }

                    if (modeloDocumento.DiferenciarMovimentosParaCOFINS)
                    {
                        decimal valorMovimentacaoCOFINS = 0m;

                        if (cargaCancelamento.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Anulacao)
                            valorMovimentacaoCOFINS = repCargaCTe.BuscarValorCOFINSPorCargaEModeloDocumento(cargaCancelamento.Carga.Codigo, codigoModeloDocumento, statusCTe, null, null, dataMovimentacao);
                        else
                            valorMovimentacaoCOFINS = repCargaCTe.BuscarValorCOFINSPorCargaEModeloDocumento(cargaCancelamento.Carga.Codigo, codigoModeloDocumento, statusCTe, null, dataMovimentacao, null);

                        if (!svcMovimento.GerarMovimentacao(out erro, tipoMovimentoCOFINS, dataMovimentacao, valorMovimentacaoCOFINS, cargaCancelamento.Carga.CodigoCargaEmbarcador, observacaoMovimentacao + " Imposto: COFINS.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Carga, tipoServicoMultisoftware))
                            return false;
                    }

                    if (modeloDocumento.DiferenciarMovimentosParaIR)
                    {
                        decimal valorMovimentacaoIR = 0m;

                        if (cargaCancelamento.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Anulacao)
                            valorMovimentacaoIR = repCargaCTe.BuscarValorIRPorCargaEModeloDocumento(cargaCancelamento.Carga.Codigo, codigoModeloDocumento, statusCTe, null, null, dataMovimentacao);
                        else
                            valorMovimentacaoIR = repCargaCTe.BuscarValorIRPorCargaEModeloDocumento(cargaCancelamento.Carga.Codigo, codigoModeloDocumento, statusCTe, null, dataMovimentacao, null);

                        if (!svcMovimento.GerarMovimentacao(out erro, tipoMovimentoIR, dataMovimentacao, valorMovimentacaoIR, cargaCancelamento.Carga.CodigoCargaEmbarcador, observacaoMovimentacao + " Imposto: IR.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Carga, tipoServicoMultisoftware))
                            return false;
                    }

                    if (modeloDocumento.DiferenciarMovimentosParaCSLL)
                    {
                        decimal valorMovimentacaoCSLL = 0m;

                        if (cargaCancelamento.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Anulacao)
                            valorMovimentacaoCSLL = repCargaCTe.BuscarValorCSLLPorCargaEModeloDocumento(cargaCancelamento.Carga.Codigo, codigoModeloDocumento, statusCTe, null, null, dataMovimentacao);
                        else
                            valorMovimentacaoCSLL = repCargaCTe.BuscarValorCSLLPorCargaEModeloDocumento(cargaCancelamento.Carga.Codigo, codigoModeloDocumento, statusCTe, null, dataMovimentacao, null);

                        if (!svcMovimento.GerarMovimentacao(out erro, tipoMovimentoCSLL, dataMovimentacao, valorMovimentacaoCSLL, cargaCancelamento.Carga.CodigoCargaEmbarcador, observacaoMovimentacao + " Imposto: CSLL.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Carga, tipoServicoMultisoftware))
                            return false;
                    }

                    foreach (int codigoComponenteFrete in codigosComponentesFrete)
                    {
                        Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorCodigo(codigoComponenteFrete);
                        Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimentoComponente = null;

                        if (cargaCancelamento.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Anulacao)
                            tipoMovimentoComponente = componenteFrete.TipoMovimentoAnulacao;
                        else
                            tipoMovimentoComponente = componenteFrete.TipoMovimentoCancelamento;

                        decimal valorMovimentacaoComponente = 0m;

                        if (cargaCancelamento.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Anulacao)
                            valorMovimentacaoComponente = Math.Abs(repCargaCTeComponentes.BuscarValorComponentePorCargaEModeloDocumento(cargaCancelamento.Carga.Codigo, codigoComponenteFrete, codigoModeloDocumento, statusCTe, null, null, dataMovimentacao));
                        else
                            valorMovimentacaoComponente = Math.Abs(repCargaCTeComponentes.BuscarValorComponentePorCargaEModeloDocumento(cargaCancelamento.Carga.Codigo, codigoComponenteFrete, codigoModeloDocumento, statusCTe, null, dataMovimentacao, null));

                        if (!svcMovimento.GerarMovimentacao(out erro, tipoMovimentoComponente, dataMovimentacao, valorMovimentacaoComponente, cargaCancelamento.Carga.CodigoCargaEmbarcador, observacaoMovimentacao + " Componente: " + componenteFrete.Descricao + ".", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Carga, tipoServicoMultisoftware))
                            return false;
                    }
                }
            }

            erro = string.Empty;
            return true;
        }

        private static void GerarControleFinanceiroCancelamentoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao)
        {
            //if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            //    return;

            if (carga.CargaTransbordo)
                return;

            if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada && carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada)
                return;

            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarPorCargaSemComplementares(carga.Codigo);

            if (cargaCTes.Count > 0)
            {
                if (Servicos.Embarcador.Carga.Carga.VerificarSeGeraMovimentacaoAgrupadaPorPedido(carga, unidadeTrabalho))
                {
                    Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = repDocumentoFaturamento.BuscarPorCarga(carga.Codigo);

                    if (documentoFaturamento != null)
                    {
                        if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada)
                        {
                            documentoFaturamento.DataCancelamento = repCargaCTe.BuscarUltimaDataCancelamentoPorCarga(carga.Codigo, "C");
                            documentoFaturamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Cancelado;
                        }
                        else if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                        {
                            documentoFaturamento.DataAnulacao = repCargaCTe.BuscarUltimaDataAnulacaoPorCarga(carga.Codigo, "Z");
                            documentoFaturamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Anulado;
                        }

                        repDocumentoFaturamento.Atualizar(documentoFaturamento);
                    }
                }
                else
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
                    {
                        if (cargaCTe.CTe != null)
                        {
                            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentoFaturamentos = repDocumentoFaturamento.BuscarDocumentoAtivoPorCTe(cargaCTe.CTe.Codigo);

                            foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento in documentoFaturamentos)
                            {
                                if (cargaCTe.CTe.Status == "C")
                                {
                                    documentoFaturamento.DataCancelamento = cargaCTe.CTe.DataCancelamento;
                                    documentoFaturamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Cancelado;
                                }
                                else if (cargaCTe.CTe.Status == "Z")
                                {
                                    documentoFaturamento.DataAnulacao = cargaCTe.CTe.DataAnulacao;
                                    documentoFaturamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Anulado;
                                }

                                repDocumentoFaturamento.Atualizar(documentoFaturamento);

                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Quando uma carga é anulada e existem MDFes não encerrados o sistema faz a solicitação para encerramento dos mesmos.
        /// </summary>
        private static void EnviarMDFesParaEncerramentoViaAnulacao(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            Servicos.Embarcador.Carga.MDFe serCargaMDFe = new Servicos.Embarcador.Carga.MDFe(unidadeTrabalho);

            Dominio.ObjetosDeValor.Embarcador.Carga.DadosEncerramentoMDFe dadosEncerramento = serCargaMDFe.ObterDadosEncerramento(cargaMDFe.Codigo, unidadeTrabalho);

            string retornoEncerramento = serCargaMDFe.EncerrarMDFe(dadosEncerramento.Codigo, carga.Codigo, dadosEncerramento.Localidades[0].Codigo, dadosEncerramento.DataEncerramento, null, cargaCancelamento.Usuario, tipoServicoMultisoftware, unidadeTrabalho, Auditado);

            if (Auditado != null)
            {
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaMDFe.MDFe, "Encerrado pela anulação da carga.", unidadeTrabalho);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaMDFe, "Encerrado pela anulação da carga.", unidadeTrabalho);
            }
        }

        private static void CriarIntegracoesCargaCancelamentoCarga(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Integracao.Marfrig.IntegracaoOrdemEmbarqueMarfrig servicoIntegracaoOrdemEmbarqueMarfrig = new Servicos.Embarcador.Integracao.Marfrig.IntegracaoOrdemEmbarqueMarfrig(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracao repositorioCargaCancelamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao> cargaCargasIntegracao = repositorioCargaCargaIntegracao.BuscarPorCarga(cargaCancelamento.Carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> listaCargaDadosTransporteIntegracao = repositorioCargaDadosTransporteIntegracao.BuscarPorCarga(cargaCancelamento.Carga.Codigo);
            bool gerouPorCargaCargaIntegracao = false;
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao in cargaCargasIntegracao)
            {
                if (cargaCargaIntegracao.SituacaoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado)
                    continue;

                if (repositorioCargaCancelamentoIntegracao.ContarPorCancelamentoETipoIntegracao(cargaCargaIntegracao.Codigo, cargaCargaIntegracao.TipoIntegracao.Tipo) > 0)
                    continue;

                switch (cargaCargaIntegracao.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AngelLira:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MundialRisk:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Logiun:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Boticario:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TelhaNorte:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Trizy:
                        Integracao.IntegracaoCargaCancelamento.AdicionarIntegracaoCancelamentoPorTipoIntegracao(cargaCancelamento, cargaCargaIntegracao.TipoIntegracao.Tipo, unitOfWork);
                        gerouPorCargaCargaIntegracao = true;
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TrizyContrato:
                        if (cargaCargaIntegracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado || !string.IsNullOrEmpty(cargaCargaIntegracao.Protocolo) || !string.IsNullOrEmpty(cargaCargaIntegracao.PreProtocolo))
                        {
                            Integracao.IntegracaoCargaCancelamento.AdicionarIntegracaoCancelamentoPorTipoIntegracao(cargaCancelamento, cargaCargaIntegracao.TipoIntegracao.Tipo, unitOfWork);
                            gerouPorCargaCargaIntegracao = true;
                        }
                        break;
                }
            }

            if (gerouPorCargaCargaIntegracao)
                return;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao in listaCargaDadosTransporteIntegracao)
            {
                if (cargaDadosTransporteIntegracao.SituacaoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado)
                    continue;

                if (repositorioCargaCancelamentoIntegracao.ContarPorCancelamentoETipoIntegracao(cargaDadosTransporteIntegracao.Carga?.CargaCancelamento?.Codigo ?? 0, cargaDadosTransporteIntegracao.TipoIntegracao.Tipo) > 0)
                    continue;

                switch (cargaDadosTransporteIntegracao.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AngelLira:
                        Integracao.IntegracaoCargaCancelamento.AdicionarIntegracaoCancelamentoPorTipoIntegracao(cargaCancelamento, cargaDadosTransporteIntegracao.TipoIntegracao.Tipo, unitOfWork);
                        break;

                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TrizyContrato:
                        if (cargaDadosTransporteIntegracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado || !string.IsNullOrEmpty(cargaDadosTransporteIntegracao.Protocolo) || !string.IsNullOrEmpty(cargaDadosTransporteIntegracao.PreProtocolo))
                            Integracao.IntegracaoCargaCancelamento.AdicionarIntegracaoCancelamentoPorTipoIntegracao(cargaCancelamento, cargaDadosTransporteIntegracao.TipoIntegracao.Tipo, unitOfWork);
                        break;
                }
            }
        }

        private static void NotificarAlteracoesFilaCarregamentoVeiculo(List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> listaFilaCarregamentoVeiculoAlterada, Repositorio.UnitOfWork unitOfWork)
        {
            if (listaFilaCarregamentoVeiculoAlterada?.Count > 0)
                new Logistica.FilaCarregamentoVeiculo(unitOfWork, OrigemAlteracaoFilaCarregamento.Sistema).NotificarAlteracoes(listaFilaCarregamentoVeiculoAlterada);
        }

        private static bool RemoverEGerarMovimentacaoDosCTesEmitidosPorOutroSistema(out string erro, Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ||
                cargaCancelamento.Carga.CargaTransbordo)
            {
                erro = string.Empty;
                return true;
            }

            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unidadeTrabalho);
            Repositorio.Embarcador.Canhotos.CanhotoHistorico repCanhotoHistorico = new Repositorio.Embarcador.Canhotos.CanhotoHistorico(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeTrabalho);
            Repositorio.VeiculoCTE repVeiculoCTe = new Repositorio.VeiculoCTE(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracao repCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
            Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao repDocumentoEscrituracao = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao(unidadeTrabalho);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unidadeTrabalho);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao repCargaCancelamentoCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> mdfesEmitidosEmOutroSistema = repCargaMDFe.BuscarPorCargaEmitidosEmOutroSistema(cargaCancelamento.Carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ctesEmitidosEmOutroSistema = repCargaCTe.BuscarPorCargaEmitidosEmOutroSistema(cargaCancelamento.Carga.Codigo);

            Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamentoCarga = repDocumentoFaturamento.BuscarPorCarga(cargaCancelamento.Carga.Codigo);

            if (documentoFaturamentoCarga != null)
                RemoverDocumentoFaturamentoCancelamentoCarga(new List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>() { documentoFaturamentoCarga }, unidadeTrabalho);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in ctesEmitidosEmOutroSistema)
            {
                //if (cargaCTe.Carga.DataFinalizacaoEmissao.HasValue) //não gerou movimentação de autorização, gera as movimentações de autorização antes de gerar as de cancelamento
                //{
                if (!GerarMovimentoCancelamentoCTe(out erro, cargaCTe, tipoServicoMultisoftware, unidadeTrabalho, stringConexao, true))
                    return false;
                //}

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTeComplementoInfos = repCargaCTeComplementoInfo.BuscarPorCargaCTeComplementado(cargaCTe.Codigo);
                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento> cargaOcorrenciaDocumentos = repCargaOcorrenciaDocumento.BuscarPorCargaCTe(cargaCTe.Codigo);
                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCanhoto.BuscarPorCargaCTe(cargaCTe.Codigo);
                Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao documentoEscrituracao = repDocumentoEscrituracao.BuscarPorCTeECarga(cargaCancelamento.Carga.Codigo, cargaCTe.CTe.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao> integracoes = repCargaCTeIntegracao.BuscarPorCargaCTe(cargaCTe.Codigo);
                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentoFaturamentos = repDocumentoFaturamento.BuscarTodosPorCTe(cargaCTe.CTe.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao> integracoesCancelamento = repCargaCancelamentoCargaCTeIntegracao.BuscarPorCargaCTe(cargaCTe.Codigo);

                repCargaMDFeManual.RemoverCargaCTe(cargaCTe.Codigo);

                if (!cargaCancelamento.DuplicarCarga)
                {
                    cargaCTe.CTe.CTeSemCarga = true;

                    repCTe.Atualizar(cargaCTe.CTe);
                }

                if (canhoto != null)
                {
                    List<Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico> historicos = repCanhotoHistorico.BuscarPorCanhoto(canhoto.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico historico in historicos)
                        repCanhotoHistorico.Deletar(historico);

                    canhoto.MotoristasResponsaveis = null;

                    repCanhoto.Deletar(canhoto);
                }

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo in cargaCTeComplementoInfos)
                {
                    repCargaCTe.RemoverVinculoCargaCTeComplemento(cargaCTeComplementoInfo.Codigo);
                    repCargaCTeComplementoInfo.Deletar(cargaCTeComplementoInfo);
                }

                foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento cargaOcorrenciaDocumento in cargaOcorrenciaDocumentos)
                    repCargaOcorrenciaDocumento.Deletar(cargaOcorrenciaDocumento);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao integracao in integracoes)
                    repCargaCTeIntegracao.Deletar(integracao);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao integracaoCancelamento in integracoesCancelamento)
                    repCargaCancelamentoCargaCTeIntegracao.Deletar(integracaoCancelamento);

                RemoverDocumentoFaturamentoCancelamentoCarga(documentoFaturamentos, unidadeTrabalho);

                //remove os veículos importados para o CT-e da carga, (CT-e 3.0 não tem veículo aí o sistema usa os da carga ao importar o CT-e para a mesma)
                foreach (Dominio.Entidades.VeiculoCTE veiculoCTe in cargaCTe.CTe.Veiculos)
                    if (veiculoCTe.ImportadoCarga)
                        repVeiculoCTe.Deletar(veiculoCTe);

                if (documentoEscrituracao != null)
                    repDocumentoEscrituracao.Deletar(documentoEscrituracao);

                if (!VerificarVinculoContratoFreteTerceiro(cargaCTe, unidadeTrabalho, tipoServicoMultisoftware, out erro))
                    return false;

                repCargaCTe.Deletar(cargaCTe);
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe in mdfesEmitidosEmOutroSistema)
            {
                if (!cargaCancelamento.DuplicarCarga)
                {
                    cargaMDFe.MDFe.MDFeSemCarga = true;

                    repMDFe.Atualizar(cargaMDFe.MDFe);
                }

                repCargaMDFe.Deletar(cargaMDFe);
            }

            erro = string.Empty;
            return true;
        }
        private static bool VerificarVinculoContratoFreteTerceiro(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string mensagemErro)
        {
            mensagemErro = string.Empty;
            Repositorio.Embarcador.Terceiros.ContratoFreteCTe repContratoFreteCTe = new Repositorio.Embarcador.Terceiros.ContratoFreteCTe(unitOfWork);
            Servicos.Embarcador.Terceiros.ContratoFrete servicoContratoFrete = new Servicos.Embarcador.Terceiros.ContratoFrete(unitOfWork);

            List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteCTe> listaContratoFreteCTe = repContratoFreteCTe.BuscarPorCargaCTe(cargaCTe.Codigo);

            if (listaContratoFreteCTe != null && listaContratoFreteCTe.Count() > 0)
            {
                foreach (var contratoFreteCTe in listaContratoFreteCTe)
                {
                    if (!servicoContratoFrete.ReabrirContratoFrete(contratoFreteCTe.ContratoFrete, unitOfWork, tipoServicoMultisoftware, null, new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado(), out mensagemErro))
                    {
                        return false;
                    }
                }

            }
            return true;
        }
        private static void ReverterSaldoContratoPrestacaoServico(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Embarcador.Frete.ContratoPrestacaoServicoSaldo servicoSaldo = new Embarcador.Frete.ContratoPrestacaoServicoSaldo(unitOfWork);

            if (servicoSaldo.IsUtilizaContratoPrestacaoServico())
            {
                Embarcador.Carga.Carga servicoCarga = new Embarcador.Carga.Carga(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Frete.ContratoPrestacaoServicoSaldoDados dados = new Dominio.ObjetosDeValor.Embarcador.Frete.ContratoPrestacaoServicoSaldoDados()
                {
                    CodigoFilial = carga.Filial?.Codigo ?? 0,
                    CodigoTransportador = carga.Empresa?.Codigo ?? 0,
                    Descricao = $"Saldo referente ao cancelamento da carga {servicoCarga.ObterNumeroCarga(carga, unitOfWork)}",
                    TipoLancamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLancamento.Automatico,
                    TipoMovimentacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentacaoContratoPrestacaoServico.Entrada,
                    Valor = carga.ValorFrete + carga.ValorFreteContratoFreteTotal
                };

                servicoSaldo.AdicionarSemValidacao(dados);
            }
        }

        #endregion Métodos Privados

        #region Métodos Públicos (Cancelamento Novo)

        public static void VerificarCargasAgCancelamentoMDFes(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);

            List<int> cargasEmCancelamento = repCargaCancelamento.BuscarCodigosPorSituacaoETipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.AgCancelamentoMDFe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Cancelamento);

            foreach (int codigoCargaCancelamento in cargasEmCancelamento)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCodigo(codigoCargaCancelamento);

                CancelarMDFesCargaEmCancelamento(cargaCancelamento, unitOfWork, tipoServicoMultisoftware, Auditado);
            }
        }

        public void VerificarCargasAgCancelamentoCIOT(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
            List<int> cargasEmCancelamento = repCargaCancelamento.BuscarCodigosPorSituacaoETipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.AgIntegracaoCancelamentoCIOT, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Cancelamento);

            foreach (int codigoCargaCancelamento in cargasEmCancelamento)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCodigo(codigoCargaCancelamento);

                CancelarCIOTCargaCancelamento(cargaCancelamento, unitOfWork, tipoServicoMultisoftware, auditado);
            }
        }

        public static void VerificarCargasAgCancelamentoAverbacoesMDFes(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);

            List<int> cargasEmCancelamento = repCargaCancelamento.BuscarCodigosPorSituacaoETipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.AgCancelamentoAverbacaoMDFe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Cancelamento);

            foreach (int codigoCargaCancelamento in cargasEmCancelamento)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCodigo(codigoCargaCancelamento);

                CancelarAverbacoesMDFesCargaEmCancelamento(cargaCancelamento, unitOfWork, tipoServicoMultisoftware);
            }
        }

        public static void VerificarCargasAgCancelamentoCTes(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);

            List<int> cargasEmCancelamento = repCargaCancelamento.BuscarCodigosPorSituacaoETipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.AgCancelamentoCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Cancelamento);

            foreach (int codigoCargaCancelamento in cargasEmCancelamento)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCodigo(codigoCargaCancelamento);

                CancelarCTesCargaEmCancelamento(cargaCancelamento, unitOfWork, tipoServicoMultisoftware);
            }
        }

        public static void VerificarCargasAgCancelamentoAverbacoesCTes(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);

            List<int> cargasEmCancelamento = repCargaCancelamento.BuscarCodigosPorSituacaoETipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.AgCancelamentoAverbacaoCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Cancelamento);

            foreach (int codigoCargaCancelamento in cargasEmCancelamento)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCodigo(codigoCargaCancelamento);

                CancelarAverbacoesCTesCargaEmCancelamento(cargaCancelamento, unitOfWork, tipoServicoMultisoftware);
            }
        }

        public static void VerificarCargasAgIntegracoesCancelamento(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);

            List<int> cargasEmCancelamento = repCargaCancelamento.BuscarCodigosPorSituacaoETipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.AgIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Cancelamento);

            foreach (int codigoCargaCancelamento in cargasEmCancelamento)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCodigo(codigoCargaCancelamento);

                VerificarIntegracoesCargaEmCancelamento(cargaCancelamento, unitOfWork, tipoServicoMultisoftware);
            }
        }

        public void VerificarCargasAgIntegracoesDadosCancelamento(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, List<int> cargasEmCancelamento = null)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
            if (cargasEmCancelamento == null)
                cargasEmCancelamento = repCargaCancelamento.BuscarCodigosPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.AgIntegracaoDadosCancelamento);

            foreach (int codigoCargaCancelamento in cargasEmCancelamento)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCodigo(codigoCargaCancelamento);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao> cargaCargaIntegracao = repCargaCargaIntegracao.BuscarPorCarga(cargaCancelamento.Carga.Codigo);

                if (cargaCargaIntegracao.Count > 0)
                {
                    VerificarIntegracoesCargaDadosCancelamento(cargaCancelamento, unitOfWork, tipoServicoMultisoftware);
                }

                if (configuracaoGeralCarga.CancelarCIOTAutomaticamenteFluxoCancelamentoCarga)
                {
                    cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.AgCancelamentoMDFe;
                    repCargaCancelamento.Atualizar(cargaCancelamento);
                }
                else
                {
                    repCargaCancelamento.AtulizarStatusCargaSemIntegracaoCancelamento(cargaCancelamento.Codigo);
                }
            }
        }

        public static void VerificarCargasEmFinalizacaoCancelamento(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente ClienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Servicos.Global.OrquestradorFila servicoOrquestradorFila = new Servicos.Global.OrquestradorFila(unitOfWork, IdentificadorControlePosicaoThread.VerificarCargasEmFinalizacaoCancelamento);

            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);

            List<int> cargasEmCancelamento = servicoOrquestradorFila.Ordenar((limiteRegistros) => repCargaCancelamento.BuscarCodigosVerificarCargasEmFinalizacaoCancelamento(limiteRegistros));

            foreach (int codigoCargaCancelamento in cargasEmCancelamento)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCodigo(codigoCargaCancelamento);

                FinalizarCancelamentoCargaEmCancelamento(cargaCancelamento, unitOfWork, tipoServicoMultisoftware, ClienteMultisoftware, auditado);
            }
        }

        public static void VerificarCargasReenvioCancelamentoCTe(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

            if (configuracao != null && configuracao.TempoMinutosParaReenviarCancelamento > 0)
            {
                int numeroTentativas = (60 / configuracao.TempoMinutosParaReenviarCancelamento) * 24;

                DateTime dataParametro = DateTime.Now.AddMinutes(configuracao.TempoMinutosParaReenviarCancelamento * -1);

                List<int> cargasRejeicaoCancelamento = repCargaCancelamento.BuscarCodigosReenvioCancelamento(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento, numeroTentativas, dataParametro, 50);

                foreach (int codigoCargaCancelamento in cargasRejeicaoCancelamento)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCodigo(codigoCargaCancelamento);
                    if (cargaCancelamento != null)
                        ReenviarCancelamentoCTe(cargaCancelamento, unitOfWork, tipoServicoMultisoftware);
                }
            }
        }

        public static void SolicitarCancelamentoCargaNovo(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

            Servicos.Embarcador.Hubs.Carga svcHubCarga = new Servicos.Embarcador.Hubs.Carga();

            string erro = string.Empty;

            if (!ValidarCancelamentoCarga(out erro, cargaCancelamento, unitOfWork, tipoServicoMultisoftware) ||
                !AjustarCarregamentos(out erro, cargaCancelamento, unitOfWork, tipoServicoMultisoftware))
            {
                cargaCancelamento.MensagemRejeicaoCancelamento = erro;
                cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;
                cargaCancelamento.TentativasEnvioCancelamento += 1;
                cargaCancelamento.DataEnvioCancelamento = DateTime.Now;

                repCargaCancelamento.Atualizar(cargaCancelamento);

                svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                unitOfWork.FlushAndClear();

                return;
            }

            unitOfWork.Start();

            if (configuracaoGeralCarga.CancelarCIOTAutomaticamenteFluxoCancelamentoCarga)
                cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.AgCancelamentoMDFe;
            else
                cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.AgIntegracaoCancelamentoCIOT;

            repCargaCancelamento.Atualizar(cargaCancelamento);

            unitOfWork.CommitChanges();

            svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

            unitOfWork.FlushAndClear();
        }

        public Dominio.Entidades.Embarcador.CTe.CancelamentoCTeSemCarga AdicionarCancelamentoCTeSemCarga(List<int> codigoCtesSemCarga, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Usuario usuario)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.CTe.CancelamentoCTeSemCarga repCTeSemCarga = new Repositorio.Embarcador.CTe.CancelamentoCTeSemCarga(unitOfWork);
            Repositorio.Embarcador.CTe.CancelamentoCTe repCancelamentoCTe = new Repositorio.Embarcador.CTe.CancelamentoCTe(unitOfWork);
            Dominio.Entidades.Embarcador.CTe.CancelamentoCTeSemCarga cancelamentoCTeSemCarga = new Dominio.Entidades.Embarcador.CTe.CancelamentoCTeSemCarga();


            cancelamentoCTeSemCarga.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCTeSemCarga.AgCancelamentoCTe;
            cancelamentoCTeSemCarga.DataInclusao = DateTime.Now;
            cancelamentoCTeSemCarga.MotivoCancelamento = @$"Cancelamento de CTes sem carga solicitado por {usuario.Nome}";
            cancelamentoCTeSemCarga.Usuario = usuario;
            repCTeSemCarga.Inserir(cancelamentoCTeSemCarga);

            foreach (var cteSemCarga in codigoCtesSemCarga)
            {
                var cte = repCTe.BuscarPorCodigo(cteSemCarga);

                if (cte.Status == "A")
                {
                    var cancelamentoCTe = new Dominio.Entidades.Embarcador.CTe.CancelamentoCTe();
                    cancelamentoCTe.CancelamentoCTeSemCarga = cancelamentoCTeSemCarga;
                    cancelamentoCTe.CTe = cte;
                    repCancelamentoCTe.Inserir(cancelamentoCTe);

                }
            }
            repCTeSemCarga.Atualizar(cancelamentoCTeSemCarga);

            return cancelamentoCTeSemCarga;
        }

        public void VerificarCTeSemCargaAgCancelamento(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CTe.CancelamentoCTeSemCarga repCTeSemCarga = new Repositorio.Embarcador.CTe.CancelamentoCTeSemCarga(unitOfWork);
            var listaCtesPendentesCancelamento = repCTeSemCarga.BuscarCteSemCargaPorStatus(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCTeSemCarga.AgCancelamentoCTe);
            Repositorio.Embarcador.CTe.CancelamentoCTe repCTeCancelamento = new Repositorio.Embarcador.CTe.CancelamentoCTe(unitOfWork);
            List<Dominio.Entidades.Embarcador.CTe.CancelamentoCTe> cancelamentoCTes = new List<Dominio.Entidades.Embarcador.CTe.CancelamentoCTe>();
            foreach (var codigo in listaCtesPendentesCancelamento)
            {
                Dominio.Entidades.Embarcador.CTe.CancelamentoCTeSemCarga cancelamentoCTeSemCarga = repCTeSemCarga.BuscarPorCodigo(codigo);
                cancelamentoCTes = repCTeCancelamento.BuscarPorCodigoCancelamento(codigo);

                CancelarCTesSemCarga(cancelamentoCTeSemCarga, cancelamentoCTes, unitOfWork);

            }
        }

        public bool ValidaCargaEmProcessamento(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                return false;

            List<SituacaoCarga> situacoesPermiteCancelamento = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga>
            {
                SituacaoCarga.EmTransporte,
                SituacaoCarga.Encerrada,
                SituacaoCarga.AgImpressaoDocumentos,
                SituacaoCarga.Nova,
            };

            if (situacoesPermiteCancelamento.Contains(carga.SituacaoCarga))
                return false;

            if (carga.ExigeNotaFiscalParaCalcularFrete && carga.SituacaoCarga == SituacaoCarga.AgNFe)
                return false;

            if ((configuracaoEmbarcador.ExigirCargaRoteirizada || (carga.TipoOperacao?.ExigirCargaRoteirizada ?? false)) && carga.CalculandoFrete && carga.SituacaoRoteirizacaoCarga == SituacaoRoteirizacao.Erro && carga.NumeroTentativasRoteirizacaoCarga >= configuracaoEmbarcador.NumeroTentativasConsultarCargasErroRoteirizacao)
                return false;

            if (carga.AgValorRedespacho || carga.PendenteGerarCargaDistribuidor || (carga.CalculandoFrete && carga.TipoFreteEscolhido == TipoFreteEscolhido.Tabela))
                return true;

            if ((configuracaoEmbarcador.ExigirCargaRoteirizada || (carga.TipoOperacao?.ExigirCargaRoteirizada ?? false)) && (carga.SituacaoRoteirizacaoCarga == SituacaoRoteirizacao.Aguardando || carga.SituacaoRoteirizacaoCarga == SituacaoRoteirizacao.Erro) && carga.SituacaoCarga != SituacaoCarga.PendeciaDocumentos)
                return true;

            Repositorio.Embarcador.Cargas.CargaCTe repCte = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);

            if (carga.ProcessandoDocumentosFiscais && repCte.BuscarPorCargaValidaEmProcessamentoCte(carga))
                return true;

            return false;
        }

        #endregion Métodos Públicos (Cancelamento Novo)

        #region Métodos Privados (Cancelamento Novo)

        public static void CancelarCIOTCargaCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Servicos.Embarcador.CIOT.CIOT svcCIOT = new Servicos.Embarcador.CIOT.CIOT();
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
            Dominio.Entidades.Embarcador.Documentos.CIOT ciot = repCIOT.BuscarCIOTEmCancelamentoPorCarga(cargaCancelamento.Carga.Codigo);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

            try
            {
                bool possuiIDViagemInformado = false;
                if (ciot != null && ciot.Operadora == OperadoraCIOT.RepomFrete && !string.IsNullOrEmpty(ciot.ProtocoloAutorizacao))
                    possuiIDViagemInformado = true;

                if (!configuracaoGeralCarga.CancelarCIOTAutomaticamenteFluxoCancelamentoCarga || (ciot == null && !cargaCancelamento.EnviouMDFesParaCancelamento) || (ciot != null && ciot.Situacao == SituacaoCIOT.Cancelado && !cargaCancelamento.EnviouMDFesParaCancelamento) || (ciot?.CIOTPorPeriodo ?? false))
                {
                    cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.AgCancelamentoMDFe;
                }
                else if ((ciot != null && !possuiIDViagemInformado && ciot?.Situacao == SituacaoCIOT.Pendencia) || (ciot?.Situacao == SituacaoCIOT.AgIntegracao && tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)) //na situação AgIntegracao pode que em processos paralelos o CIOT seja cancelado e enviado ao mesmo tempo neste caso o cliente tera que cancelar o CIOT direto na operadora. 
                {
                    cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.AgCancelamentoMDFe;
                    if (svcCIOT.CancelarCIOTEmSituacaoDePendencia(ciot, tipoServicoMultisoftware, unitOfWork, out string mensagemErro))
                    {
                        Servicos.Auditoria.Auditoria.Auditar(auditado, ciot, null, "CIOT Cancelado automaticamente pelo fluxo de cancelamento.", unitOfWork);

                        if (configuracaoGeralCarga.CancelarCIOTAutomaticamenteFluxoCancelamentoCarga)
                            cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.AgIntegracao;
                        else
                            cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.AgCancelamentoMDFe;

                        ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Cancelado;
                        repCIOT.Atualizar(ciot);
                    }
                    else
                    {
                        cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;
                        cargaCancelamento.MensagemRejeicaoCancelamento = mensagemErro;
                    }
                }
                else if (cargaCancelamento.EnviouCIOTCancelamento)
                {
                    cargaCancelamento.Situacao = SituacaoCancelamentoCarga.RejeicaoCancelamento;
                    cargaCancelamento.MensagemRejeicaoCancelamento = "Ocorreu um erro durante o cancelamento do CIOT. Tente novamente";
                }
                else if (ciot?.Situacao == SituacaoCIOT.Aberto || ciot?.Situacao == SituacaoCIOT.PagamentoAutorizado || ciot?.Situacao == SituacaoCIOT.AgLiberarViagem || possuiIDViagemInformado)
                {
                    cargaCancelamento.EnviouCIOTCancelamento = true;
                    repCargaCancelamento.Atualizar(cargaCancelamento);

                    if (svcCIOT.CancelarCIOT(ciot, auditado.Usuario, tipoServicoMultisoftware, unitOfWork, out string mensagemErro))
                    {
                        Servicos.Auditoria.Auditoria.Auditar(auditado, ciot, null, "CIOT Cancelado automaticamente.", unitOfWork);

                        if (configuracaoGeralCarga.CancelarCIOTAutomaticamenteFluxoCancelamentoCarga)
                            cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.AgIntegracao;
                        else
                            cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.AgCancelamentoMDFe;

                        ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Cancelado;
                        repCIOT.Atualizar(ciot);
                    }
                    else
                    {
                        cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;
                        cargaCancelamento.MensagemRejeicaoCancelamento = mensagemErro;
                    }
                }
                else if (ciot?.Situacao == SituacaoCIOT.Encerrado)
                {
                    cargaCancelamento.Situacao = SituacaoCancelamentoCarga.RejeicaoCancelamento;
                    cargaCancelamento.MensagemRejeicaoCancelamento = "O CIOT já está encerrado, não sendo possível cancelar";
                }
                else if (ciot?.Situacao == SituacaoCIOT.Cancelado && cargaCancelamento.EnviouMDFesParaCancelamento && tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS
                    || ciot == null && cargaCancelamento.EnviouMDFesParaCancelamento || (ciot?.Situacao == SituacaoCIOT.AgIntegracao && ciot?.ArquivosTransacao?.Count() == 0)) //cargas em loop/travadas de cancelamento no fluxo novo. 
                {
                    cargaCancelamento.Situacao = SituacaoCancelamentoCarga.AgIntegracao;
                }

                repCargaCancelamento.Atualizar(cargaCancelamento);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
            }
        }

        private static void ReenviarCancelamentoCTe(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);

            unitOfWork.Start();

            cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.EmCancelamento;
            cargaCancelamento.EnviouCTesParaCancelamento = false;
            repCargaCancelamento.Atualizar(cargaCancelamento);

            unitOfWork.CommitChanges();

            unitOfWork.FlushAndClear();

            return;
        }

        private static void CancelarMDFesCargaEmCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);

            Servicos.Embarcador.Hubs.Carga svcHubCarga = new Servicos.Embarcador.Hubs.Carga();

            bool mdfesLiberados = true;

            if (cargaCancelamento.EnviouMDFesParaCancelamento)
            {
                if (!ValidarMDFesEmCancelamento(out string erro, ref mdfesLiberados, cargaCancelamento, unitOfWork))
                {
                    cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;
                    cargaCancelamento.MensagemRejeicaoCancelamento = erro;

                    repCargaCancelamento.Atualizar(cargaCancelamento);

                    svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                    return;
                }
            }
            else
            {
                bool cancelouComSucesso = CancelarMDFesCarga(out string erro, ref mdfesLiberados, cargaCancelamento, unitOfWork, tipoServicoMultisoftware, Auditado);

                cargaCancelamento = repCargaCancelamento.BuscarPorCodigo(cargaCancelamento.Codigo);

                if (!cancelouComSucesso)
                {
                    cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;
                    cargaCancelamento.MensagemRejeicaoCancelamento = erro;

                    repCargaCancelamento.Atualizar(cargaCancelamento);

                    svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                    return;
                }

                cargaCancelamento.EnviouMDFesParaCancelamento = true;

                repCargaCancelamento.Atualizar(cargaCancelamento);

                svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);
            }

            if (!mdfesLiberados)
                return;

            unitOfWork.Start();

            cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.AgCancelamentoAverbacaoMDFe;

            repCargaCancelamento.Atualizar(cargaCancelamento);

            unitOfWork.CommitChanges();

            unitOfWork.FlushAndClear();
        }

        private static void CancelarAverbacoesMDFesCargaEmCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.AverbacaoMDFe repAverbacaoMDFe = new Repositorio.AverbacaoMDFe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);

            Servicos.Embarcador.Hubs.Carga svcHubCarga = new Servicos.Embarcador.Hubs.Carga();

            bool averbacoesMDFeLiberadas = true;

            if (cargaCancelamento.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Cancelamento)
            {
                if (!cargaCancelamento.LiberarCancelamentoComAverbacaoMDFeRejeitada &&
                    repAverbacaoMDFe.ExistePorCargaEStatus(cargaCancelamento.Carga.Codigo, Dominio.Enumeradores.StatusAverbacaoMDFe.Sucesso))
                {
                    unitOfWork.Start();

                    cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;
                    cargaCancelamento.MensagemRejeicaoCancelamento = "Não foi possível cancelar todas as averbações dos MDF-es.";

                    repCargaCancelamento.Atualizar(cargaCancelamento);

                    unitOfWork.CommitChanges();

                    svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                    unitOfWork.FlushAndClear();

                    return;
                }

                if (repAverbacaoMDFe.ExistePorCargaEStatus(cargaCancelamento.Carga.Codigo, Dominio.Enumeradores.StatusAverbacaoMDFe.AgCancelamento))
                    averbacoesMDFeLiberadas = false;
                else
                {
                    List<Dominio.Entidades.AverbacaoMDFe> averbacaoMDFes = repAverbacaoMDFe.BuscarPorCargaCancelamento(cargaCancelamento.Carga.Codigo, cargaCancelamento.Codigo);
                    foreach (Dominio.Entidades.AverbacaoMDFe averbacaoMDFe in averbacaoMDFes)
                    {
                        averbacaoMDFe.CargaCancelamento = cargaCancelamento;
                        repAverbacaoMDFe.Atualizar(averbacaoMDFe);
                    }
                }
            }

            if (!averbacoesMDFeLiberadas)
                return;

            unitOfWork.Start();

            cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.AgCancelamentoCTe;

            repCargaCancelamento.Atualizar(cargaCancelamento);

            unitOfWork.CommitChanges();

            unitOfWork.FlushAndClear();
        }

        private static void CancelarCTesCargaEmCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);

            Servicos.Embarcador.Hubs.Carga svcHubCarga = new Servicos.Embarcador.Hubs.Carga();

            bool ctesLiberados = true;

            if (!cargaCancelamento.Carga.CargaTransbordo)// se é uma carga de transbordo os CT-es não podem ser cancelados nesta carga, e sim na carga original.
            {
                if (cargaCancelamento.EnviouCTesParaCancelamento)
                {
                    if (!cargaCancelamento.LiberarCancelamentoComIntegracaoRejeitada && !ValidarCTesEmCancelamento(out string erro, ref ctesLiberados, cargaCancelamento, unitOfWork, tipoServicoMultisoftware))
                    {
                        cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;
                        cargaCancelamento.MensagemRejeicaoCancelamento = "Não foi possível cancelar/inutilizar todos os CT-es.";

                        repCargaCancelamento.Atualizar(cargaCancelamento);

                        svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                        unitOfWork.FlushAndClear();

                        return;
                    }
                }
                else
                {
                    bool cancelouComSucesso = CancelarCTesCarga(out string erro, ref ctesLiberados, cargaCancelamento, unitOfWork, tipoServicoMultisoftware);

                    cargaCancelamento = repCargaCancelamento.BuscarPorCodigo(cargaCancelamento.Codigo);

                    if (!cancelouComSucesso)
                    {
                        cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;
                        cargaCancelamento.MensagemRejeicaoCancelamento = erro;

                        repCargaCancelamento.Atualizar(cargaCancelamento);

                        svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                        unitOfWork.FlushAndClear();

                        return;
                    }

                    cargaCancelamento.EnviouCTesParaCancelamento = true;
                    cargaCancelamento.DataEnvioCancelamento = DateTime.Now;
                    cargaCancelamento.TentativasEnvioCancelamento += 1;

                    repCargaCancelamento.Atualizar(cargaCancelamento);

                    svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);
                }
            }

            if (ctesLiberados)
            {
                unitOfWork.Start();

                cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.AgCancelamentoAverbacaoCTe;

                repCargaCancelamento.Atualizar(cargaCancelamento);

                unitOfWork.CommitChanges();
            }

            unitOfWork.FlushAndClear();

            return;
        }

        private static void VerificarIntegracoesCargaEmCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(cargaCancelamento.Carga.Codigo);

            Servicos.Embarcador.Hubs.Carga svcHubCarga = new Servicos.Embarcador.Hubs.Carga();

            bool integracoesLiberadas = true;

            if (cargaCancelamento.GerouIntegracao && cargaCancelamento.EnviouIntegracoesDeCancelamento)
            {
                Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI repCargaCancelamentoIntegracaoEDI = new Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao repCargaCancelamentoCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao(unitOfWork);

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao> situacoesIntegracaoPendente = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao>()
                {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao
                };

                if (!cargaCancelamento.LiberarCancelamentoComIntegracaoRejeitada)
                {
                    if (repCargaCancelamentoIntegracaoEDI.ContarPorCancelamento(cargaCancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) > 0 ||
                        repCargaCancelamentoCargaIntegracao.ContarPorCargaCancelamento(cargaCancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) > 0 ||
                        repCargaCancelamentoCargaCTeIntegracao.ContarPorCargaCancelamento(cargaCancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) > 0)
                    {
                        cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;
                        cargaCancelamento.MensagemRejeicaoCancelamento = "Não foi possível enviar todas as integrações.";

                        repCargaCancelamento.Atualizar(cargaCancelamento);

                        svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                        return;
                    }
                }

                if (repCargaCancelamentoCargaIntegracao.ContarPorCargaCancelamento(cargaCancelamento.Codigo, situacoesIntegracaoPendente.ToArray()) > 0 ||
                    repCargaCancelamentoIntegracaoEDI.ContarPorCargaCancelamento(cargaCancelamento.Codigo, situacoesIntegracaoPendente.ToArray()) > 0 ||
                    repCargaCancelamentoCargaCTeIntegracao.ContarPorCargaCancelamento(cargaCancelamento.Codigo, situacoesIntegracaoPendente.ToArray()) > 0)
                    integracoesLiberadas = false;
            }
            else if (cargaCancelamento.GerouIntegracao)
            {
                Servicos.Embarcador.Integracao.IntegracaoCargaCancelamento svcIntegracaoCargaCancelamento = new Servicos.Embarcador.Integracao.IntegracaoCargaCancelamento();

                CriarIntegracoesCargaCancelamentoCarga(cargaCancelamento, unitOfWork);
                svcIntegracaoCargaCancelamento.IniciarIntegracoesComEDI(cargaCancelamento, unitOfWork, tipoServicoMultisoftware);
                svcIntegracaoCargaCancelamento.IniciarIntegracoesComDocumentos(cargaCancelamento, cargaPedidos, unitOfWork);

                cargaCancelamento.EnviouIntegracoesDeCancelamento = true;

                repCargaCancelamento.Atualizar(cargaCancelamento);

                integracoesLiberadas = false;
            }

            if (integracoesLiberadas)
            {
                unitOfWork.Start();

                cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.FinalizandoCancelamento;

                repCargaCancelamento.Atualizar(cargaCancelamento);

                unitOfWork.CommitChanges();
            }

            unitOfWork.FlushAndClear();
        }

        private void VerificarIntegracoesCargaDadosCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            new Servicos.Embarcador.Integracao.IntegracaoCargaCancelamentoDadosCancelamento().IniciarIntegracoesDadosCancelamento(cargaCancelamento, unitOfWork);
        }

        private static void AjustarCargaParaNovaEmissao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repositoriCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracao repositoriCargaCteIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Frete.ContratoSaldoMes repositorioSaldoMes = new Repositorio.Embarcador.Frete.ContratoSaldoMes(unitOfWork);
            Repositorio.Embarcador.Frete.ContratoSaldoMesCTe repositorioSaldoMesCTe = new Repositorio.Embarcador.Frete.ContratoSaldoMesCTe(unitOfWork);
            Servicos.Embarcador.Escrituracao.Provisao servicoProvisao = new Servicos.Embarcador.Escrituracao.Provisao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos;

            if (cargaCancelamento.TipoCancelamentoCargaDocumento == TipoCancelamentoCargaDocumento.Documentos)
                cargaPedidos = repositorioCargaPedido.BuscarPorCargaECte(carga.Codigo, cargaCancelamento.CTe.Codigo);
            else
                cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);

            if (cargaPedidos.Count == 0)
                return;

            List<Dominio.Entidades.Embarcador.Frete.ContratoSaldoMes> contratoSaldoMes = repositorioSaldoMes.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Frete.ContratoSaldoMesCTe> contratoSaldoMesCTe = repositorioSaldoMesCTe.BuscarPorCargaCte(carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Frete.ContratoSaldoMesCTe contratoCTeRemover in contratoSaldoMesCTe)
                repositorioSaldoMesCTe.Deletar(contratoCTeRemover);

            foreach (Dominio.Entidades.Embarcador.Frete.ContratoSaldoMes contratoRemover in contratoSaldoMes)
                repositorioSaldoMes.Deletar(contratoRemover);

            carga.CargaEmitidaParcialmente = (cargaCancelamento.TipoCancelamentoCargaDocumento == TipoCancelamentoCargaDocumento.Documentos) && repositoriCargaCte.ExisteNaoCanceladoPorCarga(carga.Codigo);
            carga.DataFinalizacaoEmissao = null;

            //VALIDAR TIPO OPERACAO. PERMITE INFORMAR NOTAS MANUALMENTE..(SE MARCADO VOLTA PRA NF, SENAO FICA EM FRETE)
            if ((carga.TipoOperacao?.TipoConsolidacao != EnumTipoConsolidacao.PreCheckIn) || !carga.CargaEmitidaParcialmente)
            {
                carga.DataEnvioUltimaNFe = null;
                carga.DataInicioEmissaoDocumentos = null;
                carga.DataInicioGeracaoCTes = null;
                carga.SituacaoCarga = SituacaoCarga.AgNFe;

                carga.ContratoFreteTransportador = null;
                carga.ProcessandoDocumentosFiscais = false;
            }

            repCarga.Atualizar(carga);

            if (cargaCancelamento.TipoCancelamentoCargaDocumento == TipoCancelamentoCargaDocumento.Documentos)
                Servicos.Auditoria.Auditoria.Auditar(auditado, carga, null, $"Carga retornada para a situação {carga.SituacaoCarga.ObterDescricao()} a partir do cancelamento de carga do tipo Documentos", unitOfWork);
            if (cargaCancelamento.TipoCancelamentoCargaDocumento == TipoCancelamentoCargaDocumento.TodosDocumentos)
                Servicos.Auditoria.Auditoria.Auditar(auditado, carga, null, $"Carga retornada para a situação {carga.SituacaoCarga.ObterDescricao()} a partir do cancelamento de carga do tipo Todos os Documentos", unitOfWork);

            if ((cargaCancelamento.TipoCancelamentoCargaDocumento == TipoCancelamentoCargaDocumento.TodosDocumentos) && repositorioTipoIntegracao.ExistePorTipo(TipoIntegracao.Unilever))
            {
                //Processo realizado para unilever, Cargas de tipo reverso #65431
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCtes = repositoriCargaCte.BuscarPreCTePorCarga(carga.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao> integracoesCTe = repositoriCargaCteIntegracao.BuscarPorCarga(carga.Codigo);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao integracao in integracoesCTe)
                    repositoriCargaCteIntegracao.Deletar(integracao);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte in cargaCtes)
                    repositoriCargaCte.Deletar(cargaCte);

                carga.ProblemaIntegracaoValePedagio = false;
                repCarga.Atualizar(carga);
            }

            servicoProvisao.RemoverDocumentosCanceladosDaProvisao(cargaCancelamento, carga);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                cargaPedido.CTesEmitidos = false;
                repositorioCargaPedido.Atualizar(cargaPedido);
            }
        }

        private static void FinalizarCancelamentoCargaEmCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente ClienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);

            Servicos.Embarcador.Hubs.Carga svcHubCarga = new Servicos.Embarcador.Hubs.Carga();
            Servicos.Embarcador.Carga.Documentos svcDocumentos = new Servicos.Embarcador.Carga.Documentos(unitOfWork);

            try
            {
                string erro = null;

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = repConfiguracaoMonitoramento.BuscarConfiguracaoPadrao();

                CancelarTitulosEmAbertoCarga(cargaCancelamento, unitOfWork, tipoServicoMultisoftware);
                GerarControleFinanceiroCancelamentoCargaComTransacao(cargaCancelamento, unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigoFetch(cargaCancelamento.Carga.Codigo);

                svcDocumentos.AtualizarSumarizacaoViagem(carga, unitOfWork, true);

                if (!GerarMovimentosCancelamentoCargaComTransacao(out erro, cargaCancelamento, unitOfWork, tipoServicoMultisoftware))
                {
                    cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;
                    cargaCancelamento.MensagemRejeicaoCancelamento = erro;

                    repCargaCancelamento.Atualizar(cargaCancelamento);

                    return;
                }

                unitOfWork.Start();

                if ((cargaCancelamento.TipoCancelamentoCargaDocumento == TipoCancelamentoCargaDocumento.Documentos) || (cargaCancelamento.TipoCancelamentoCargaDocumento == TipoCancelamentoCargaDocumento.TodosDocumentos))
                {
                    AjustarCargaParaNovaEmissao(carga, cargaCancelamento, unitOfWork, auditado);

                    if (carga.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn)
                    {
                        Repositorio.Embarcador.Pedidos.StageAgrupamento repStageAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(unitOfWork);
                        List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> stageAgrupamentos = repStageAgrupamento.BuscarPorCargaDt(carga.Codigo);
                        List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasFilho = (from agrupamento in stageAgrupamentos where agrupamento.CargaGerada != null select agrupamento.CargaGerada).Distinct().ToList();

                        foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaFilho in cargasFilho)
                            AjustarCargaParaNovaEmissao(cargaFilho, cargaCancelamento, unitOfWork, auditado);
                    }

                    cargaCancelamento.Situacao = cargaCancelamento.Tipo == TipoCancelamentoCarga.Anulacao ? SituacaoCancelamentoCarga.Anulada : SituacaoCancelamentoCarga.Cancelada;

                    repCargaCancelamento.Atualizar(cargaCancelamento);

                    unitOfWork.CommitChanges();

                    return;
                }

                if (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiEmbarcador && (carga.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn) && (carga.DadosSumarizados?.CargaTrecho == CargaTrechoSumarizada.Agrupadora))
                    new Servicos.Embarcador.Carga.Carga(unitOfWork).FinalizarCargasConsolidadoAoCancelarCargaDT(carga, unitOfWork, auditado, tipoServicoMultisoftware);

                if (cargaCancelamento.DuplicarCarga)
                    DuplicarCargaCancelada(cargaCancelamento, unitOfWork, tipoServicoMultisoftware, ClienteMultisoftware, auditado);

                if (!AjustarItensCargaEmCancelamento(out erro, cargaCancelamento, unitOfWork, tipoServicoMultisoftware, configuracao, auditado))
                {
                    unitOfWork.Rollback();

                    repCargaCancelamento.SetarSituacaoEMensagem(cargaCancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento, erro);

                    return;
                }

                AjustarDadosTransporteCargaDuplicada(cargaCancelamento, carga, unitOfWork, tipoServicoMultisoftware, configuracao);

                if (!(configuracaoMonitoramento?.ManterMonitoramentosDeCargasCanceladasAoReceberNovaCarga ?? false))
                    Servicos.Embarcador.Monitoramento.Monitoramento.ExcluirMonitoriaPorCarga(carga.Codigo, configuracao, unitOfWork);
                else
                    Servicos.Embarcador.Monitoramento.Monitoramento.CancelarMonitoramento(carga, configuracao, new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado(), "", !cargaCancelamento.DuplicarCarga, unitOfWork);

                new Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao(unitOfWork, auditado, ClienteMultisoftware).CancelarPorCargaDevolucao(carga.Codigo, cargaCancelamento.MotivoCancelamento);

                Servicos.Embarcador.Escrituracao.CancelamentoProvisao.CancelarProvisaoDocumentosCarga(carga, unitOfWork);
                Servicos.Embarcador.Pedido.Pedido.AtualizarSituacaoPlanejamentoPedidoTMS(carga, null, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPlanejamentoPedidoTMS.CargaCanceladaAnulada, unitOfWork);

                if (!CancelarContratoFreteTerceiro(out erro, carga, unitOfWork, tipoServicoMultisoftware))
                {
                    unitOfWork.Rollback();

                    repCargaCancelamento.SetarSituacaoEMensagem(cargaCancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento, erro);

                    return;
                }

                List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> filasCarregamentoVeiculoDisponibilizadas = DisponibilizarFilasCarregamentoVeiculo(carga.Codigo, unitOfWork, tipoServicoMultisoftware);

                if (carga.Veiculo != null)
                    Servicos.Embarcador.GestaoPatio.DisponibilidadeVeiculo.SetaVeiculoDisponivel(carga.Veiculo.Codigo, unitOfWork);

                unitOfWork.CommitChanges();

                NotificarAlteracoesFilaCarregamentoVeiculo(filasCarregamentoVeiculoDisponibilizadas, unitOfWork);
                new Servicos.Embarcador.Pedido.ColetaContainer(unitOfWork).EnviarEmaiCancelamentoCargaComContainer(carga);
                Servicos.Embarcador.Cargas.CargaOferta servicoCargaOferta = new(unitOfWork);
                servicoCargaOferta.CancelarOfertaAposCancelamentoCarga(cargaCancelamento.Carga.Codigo, auditado);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);

                repCargaCancelamento.SetarSituacaoEMensagem(cargaCancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento, "Ocorreu uma falha ao finalizar o Cancelamento.");
            }
            finally
            {
                unitOfWork.FlushAndClear();
                svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);
            }
        }

        private static void CancelarTitulosEmAbertoCarga(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (cargaCancelamento.Carga.CargaTransbordo)
                return;

            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

            List<int> codigosTitulosEmAberto = repTituloDocumento.BuscarCodigosTitulosEmAbertoPorCargaCTe(cargaCancelamento.Carga.Codigo);
            codigosTitulosEmAberto.AddRange(repTituloDocumento.BuscarCodigosTitulosEmAbertoPorCarga(cargaCancelamento.Carga.Codigo));

            foreach (int codigoTituloEmAberto in codigosTitulosEmAberto)
            {
                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(codigoTituloEmAberto);

                unitOfWork.Start();

                Servicos.Embarcador.Financeiro.Titulo.GerarCancelamentoAutomaticoTituloEmAberto(titulo, "Cancelamento do título gerado automaticamente à partir do cancelamento da carga " + cargaCancelamento.Carga.CodigoCargaEmbarcador + ".", tipoServicoMultisoftware, unitOfWork);

                unitOfWork.CommitChanges();

                unitOfWork.FlushAndClear();
            }
        }

        private static void DuplicarCargaCancelada(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente ClienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork).BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork).BuscarPrimeiroRegistro();

            Dominio.Entidades.Embarcador.Cargas.Carga cargaDuplicada = svcCarga.DuplicarCarga(cargaCancelamento.Carga, cargaCancelamento, cargaCancelamento.LiberarPedidosParaMontagemCarga, tipoServicoMultisoftware, ClienteMultisoftware, unitOfWork, auditado);

            if (configuracaoGeralCarga.RecalcularFreteAoDuplicarCargaCancelamentoDocumento ?? false)
            {
                Servicos.Embarcador.Carga.Frete servicoCargaFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork);
                servicoCargaFrete.RecalcularFreteTabelaFrete(cargaDuplicada, cargaDuplicada.TabelaFreteRota?.Codigo ?? 0, unitOfWork, configuracaoTMS, new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork).BuscarConfiguracaoPadrao());
                repCarga.Atualizar(cargaDuplicada);
            }

            if (cargaDuplicada == null)
            {
                unitOfWork.Rollback();
                throw new Exception("Não foi possível duplicar a carga.");
            }

            cargaCancelamento.CargaDuplicada = cargaDuplicada;
        }

        private static void GerarControleFinanceiroCancelamentoCargaComTransacao(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaCancelamento.Carga.CargaTransbordo)
                return;

            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);

            List<int> codigosDocumentosFaturamento = repDocumentoFaturamento.BuscarCodigosAtivosPorCTeSemComplementares(cargaCancelamento.Carga.Codigo); //repDocumentoFaturamento.BuscarCodigosAtivosPorCargaECTeSemComplementares(cargaCancelamento.Carga.Codigo);
            codigosDocumentosFaturamento.AddRange(repDocumentoFaturamento.BuscarCodigosAtivosPorCargaSemComplementares(cargaCancelamento.Carga.Codigo));

            foreach (int codigoDocumentoFaturamento in codigosDocumentosFaturamento)
            {
                Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = repDocumentoFaturamento.BuscarPorCodigoComFetch(codigoDocumentoFaturamento);

                if (documentoFaturamento.TipoDocumento == TipoDocumentoFaturamento.Carga && cargaCancelamento.TipoCancelamentoCargaDocumento != TipoCancelamentoCargaDocumento.Carga)
                    continue;

                unitOfWork.Start();

                if (documentoFaturamento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento.Carga)
                {
                    if (cargaCancelamento.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Cancelamento)
                    {
                        DateTime dataCancelamento = repCargaCTe.BuscarUltimaDataCancelamentoPorCarga(documentoFaturamento.Carga.Codigo, "C");

                        if (dataCancelamento == DateTime.MinValue)
                            dataCancelamento = DateTime.Now;

                        documentoFaturamento.DataCancelamento = dataCancelamento;
                        documentoFaturamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Cancelado;
                    }
                    else if (cargaCancelamento.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Anulacao)
                    {
                        documentoFaturamento.DataAnulacao = repCargaCTe.BuscarUltimaDataAnulacaoPorCarga(documentoFaturamento.Carga.Codigo, "Z");
                        documentoFaturamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Anulado;
                    }
                }
                else
                {
                    if (documentoFaturamento.CTe.Status == "C")
                    {
                        documentoFaturamento.DataCancelamento = documentoFaturamento.CTe.DataCancelamento;
                        documentoFaturamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Cancelado;
                    }
                    else if (documentoFaturamento.CTe.Status == "Z")
                    {
                        documentoFaturamento.DataAnulacao = documentoFaturamento.CTe.DataAnulacao;
                        documentoFaturamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Anulado;
                    }
                }

                repDocumentoFaturamento.Atualizar(documentoFaturamento);

                unitOfWork.CommitChanges();

                unitOfWork.FlushAndClear();
            }
        }

        private static bool GerarMovimentosCancelamentoCargaComTransacao(out string erro, Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                erro = string.Empty;
                return true;
            }

            if (cargaCancelamento.Carga.CargaTransbordo)
            {
                erro = string.Empty;
                return true;
            }

            //if (cargaCancelamento.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada && 
            //    cargaCancelamento.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada)
            //{
            //    erro = string.Empty;
            //    return true;
            //}

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(cargaCancelamento.Carga.Codigo);

            if (!carga.DataFinalizacaoEmissao.HasValue) //não gerou movimentação de autorização, gera as movimentações de autorização antes de gerar as de cancelamento
            {
                Servicos.Embarcador.Carga.Documentos svcDocumentos = new Documentos(unitOfWork);
                svcDocumentos.GerarMovimentosAutorizacaoCarga(ref carga, unitOfWork, tipoServicoMultisoftware, true, true);
            }

            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            List<int> codigosCargaCTes = repCargaCTe.BuscarCodigosPorCargaSemComplementaresESemMovimentoCancelamento(carga.Codigo);

            if (codigosCargaCTes.Count > 0)
            {
                carga = repCarga.BuscarPorCodigo(carga.Codigo);

                if (Servicos.Embarcador.Carga.Carga.VerificarSeGeraMovimentacaoAgrupadaPorPedido(carga, unitOfWork))
                {
                    if (!carga.GerouMovimentacaoCancelamento)
                    {
                        unitOfWork.Start();

                        if (!GerarMovimentoCancelamentoCarga(out erro, cargaCancelamento, tipoServicoMultisoftware, unitOfWork, null))
                        {
                            unitOfWork.Rollback();
                            return false;
                        }

                        carga.GerouMovimentacaoCancelamento = true;

                        repCarga.Atualizar(carga);

                        unitOfWork.CommitChanges();

                        unitOfWork.FlushAndClear();
                    }
                }
                else
                {

                    foreach (int codigoCargaCTe in codigosCargaCTes)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCodigo(codigoCargaCTe);

                        if (!cargaCTe.GerouMovimentacaoCancelamento)
                        {
                            unitOfWork.Start();

                            if (!GerarMovimentoCancelamentoCTe(out erro, cargaCTe, tipoServicoMultisoftware, unitOfWork, null))
                            {
                                unitOfWork.Rollback();
                                return false;
                            }

                            unitOfWork.CommitChanges();

                            unitOfWork.FlushAndClear();
                        }
                    }
                }
            }

            erro = string.Empty;
            return true;
        }

        private static void AjustarDadosTransporteCargaDuplicada(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            if (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS)
                return;

            if (!cargaCancelamento.DuplicarCarga)
                return;

            if (carga.CargaDePreCargaFechada || carga.CargaDePreCargaEmFechamento)
                return;

            bool dadosTransporteInformados = (
                (cargaCancelamento.CargaDuplicada.ExigeNotaFiscalParaCalcularFrete && (cargaCancelamento.CargaDuplicada.SituacaoCarga != SituacaoCarga.Nova)) ||
                (!cargaCancelamento.CargaDuplicada.ExigeNotaFiscalParaCalcularFrete && (cargaCancelamento.CargaDuplicada.SituacaoCarga == SituacaoCarga.AgNFe))
            );

            if (!dadosTransporteInformados)
                return;

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Carga.PropriedadesGeracaoCarga propriedades = new Dominio.ObjetosDeValor.Embarcador.Carga.PropriedadesGeracaoCarga()
            {
                PermitirHorarioCarregamentoComLimiteAtingido = true,
                PermitirHorarioCarregamentoInferiorAoAtual = true
            };
            Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDescarregamento configuracoesDescarregamento = new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDescarregamento()
            {
                PermitirHorarioDescarregamentoComLimiteAtingido = true,
                PermitirHorarioDescarregamentoInferiorAoAtual = true
            };
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = servicoCarga.AdicionarCargaJanelaCarregamento(cargaCancelamento.CargaDuplicada, configuracao, tipoServicoMultisoftware, unitOfWork, propriedades);

            servicoCarga.AdicionarCargaJanelaDescarregamento(cargaCancelamento.CargaDuplicada, cargaJanelaCarregamento, configuracao, unitOfWork, tipoServicoMultisoftware, configuracoesDescarregamento);
            servicoCarga.AdicionarFluxoGestaoPatio(cargaCancelamento.CargaDuplicada, cargaJanelaCarregamento, unitOfWork, tipoServicoMultisoftware, validarDadosTransporteInformados: false);
        }

        private static bool AjustarItensCargaEmCancelamento(out string erro, Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            erro = null;

            Canhotos.Canhoto svcCanhoto = new Canhotos.Canhoto(unitOfWork);
            Carga svcCarga = new Carga(unitOfWork);
            ComplementoFrete svcCargaComplementoFrete = new ComplementoFrete(unitOfWork);
            Credito.CreditoMovimentacao svcCreditoMovimentacao = new Credito.CreditoMovimentacao(unitOfWork);
            Logistica.CargaJanelaCarregamentoPrioridade servicoCargaJanelaCarregamentoPrioridade = new Logistica.CargaJanelaCarregamentoPrioridade(unitOfWork);
            Pedido.ColetaContainer servicoColetaContainer = new Pedido.ColetaContainer(unitOfWork);
            Retornos.RetornoCarga servicoRetornoCarga = new Retornos.RetornoCarga(unitOfWork);
            Servicos.Embarcador.GestaoPallet.MovimentacaoPallet servicoMovimentacaoPallet = new Servicos.Embarcador.GestaoPallet.MovimentacaoPallet(unitOfWork, auditado);
            MontagemCarga.MontagemCarga servicoMontagemCarga = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubcontratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiro repCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoMDFe repCargaPedidoDocumentoMDFe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoMDFe(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComplementoFrete repCargaComplementoFrete = new Repositorio.Embarcador.Cargas.CargaComplementoFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial repCargaPedidoXMLNotaFiscalParcial = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOTCTe repCIOTCTe = new Repositorio.Embarcador.Documentos.CIOTCTe(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscalCarga = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

            cargaCancelamento = repCargaCancelamento.BuscarPorCodigo(cargaCancelamento.Codigo);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(cargaCancelamento.Carga.Codigo);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidoMontagemCargaAtualizar = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            List<int> cargaPedidoXMLNotaFiscalParcials = repCargaPedidoXMLNotaFiscalParcial.BuscarCodigoNotasPorCarga(cargaCancelamento.Carga.Codigo);

            // Lista de produtos dos pedidos para validar o saldo ao cancelar.... #32122
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidosProdutos = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
            if (!cargaCancelamento.LiberarPedidosParaMontagemCarga)
            {
                List<int> codigosPedidos = (from ped in cargaPedidos select ped.Pedido.Codigo).ToList();
                pedidosProdutos = repPedidoProduto.BuscarPorPedidos(codigosPedidos);
            }

            List<int> codigosPedidoCancelarRoteirizacao = new List<int>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                if (cargaPedido.ReentregaSolicitada)
                {
                    cargaPedido.Pedido.ReentregaSolicitada = true;
                    cargaPedido.Pedido.DataSolicitacaoReentrega = DateTime.Now;
                    repPedido.Atualizar(cargaPedido.Pedido);
                }

                //se for redespacho e o trecho anterior ainda não foi emitido, ajusta para buscar novamente os dados do proximo trecho
                if (cargaPedido.CargaPedidoTrechoAnterior != null && cargaPedido.CargaPedidoTrechoAnterior.CargaPedidoFilialEmissora &&
                    (cargaPedido.CargaPedidoTrechoAnterior.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte
                    && cargaPedido.CargaPedidoTrechoAnterior.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada
                    && cargaPedido.CargaPedidoTrechoAnterior.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao
                    && cargaPedido.CargaPedidoTrechoAnterior.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos
                    && cargaPedido.CargaPedidoTrechoAnterior.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.LiberadoPagamento))
                {
                    if (cargaPedido.CargaPedidoTrechoAnterior.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete || !cargaPedido.CargaPedidoTrechoAnterior.Carga.CalculandoFrete)
                    {
                        string retornoMontagem = servicoMontagemCarga.CalcularFreteTodoCarregamento(cargaPedido.CargaPedidoTrechoAnterior.Carga);
                        if (string.IsNullOrWhiteSpace(retornoMontagem))
                        {
                            cargaPedidoMontagemCargaAtualizar.Add(cargaPedido);
                        }
                        else
                        {
                            erro = "Não é possível cancelar pois a carregamento da carga não está apto a recalcular o frete.";
                            return false;
                        }
                    }
                    else
                    {
                        erro = "Não é possível cancelar pois a carga do trecho anterior ainda está em processo de cálculo de frete.";
                        return false;
                    }
                }

                //quando possuir um trecho anterior o pedido não pode ser cancelado, só deve ser cancelado na carga do trecho anterior.
                if (cargaPedido.CargaPedidoTrechoAnterior == null)
                {
                    //#32122 - Se não estiver passando liberar para montagem, validar se o pedido não está me mais de uma carga.
                    // Se estiver, não devemos cancelar o pedido.
                    bool pedidoOriundoSaldo = CargaPedidoOriundoSaldo(cargaCancelamento, cargaPedido, pedidosProdutos, (cargaPedido.Carga?.Carregamento?.SessaoRoteirizador?.MontagemCarregamentoPedidoProduto ?? false));

                    if (!cargaCancelamento.LiberarPedidosParaMontagemCarga && !configuracaoGeralCarga.AoCancelarCargaManterPedidosEmAberto && !pedidoOriundoSaldo)
                    {
                        cargaPedido.Pedido.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado;

                        if (cargaPedido.Pedido.SituacaoRoteirizadorIntegracao == SituacaoRoteirizadorIntegracao.Integrado)
                            codigosPedidoCancelarRoteirizacao.Add(cargaPedido.Pedido.Codigo);
                    }
                    else
                    {
                        if (pedidoOriundoSaldo)
                            AtualizarQuantidadePedidoProdutoCancelamentoCargaPedido(cargaPedido, unitOfWork);
                        else
                        {
                            cargaPedido.Pedido.PedidoTotalmenteCarregado = false;
                            if (cargaPedido.Expedidor != null)
                                cargaPedido.Pedido.PedidoRedespachoTotalmenteCarregado = false;
                            else
                                cargaPedido.Pedido.PesoSaldoRestante += cargaPedido.Peso;

                            //TODO: PPC - Adicionado log temporário para identificar problema de retorno de saldo de pedido.
                            Servicos.Log.TratarErro($"Pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} - Liberou saldo pedido {cargaPedido.Pedido.PesoSaldoRestante} - Peso Total.: {cargaPedido.Pedido.PesoTotal} - Totalmente carregado.: {cargaPedido.Pedido.PedidoTotalmenteCarregado}. Cancelamento.AjustarItensCargaEmCancelamento", "SaldoPedido");
                        }

                        cargaPedido.Pedido.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto;

                        if (configuracaoGeralCarga?.RemoverVinculoNotaPedidoAbertoAoCancelarCarga ?? false)
                        {
                            repCargaPedidoXMLNotaFiscalCTe.ExcluirCargaPedidoNotaFiscalCTePorCargaPedido(cargaPedido.Codigo);
                            repCargaEntregaNotaFiscalCarga.ExcluirCargaEntregaNotaFiscalPorCargaPedido(cargaPedido.Codigo);
                            repPedidoXMLNotaFiscal.DeletarPorCargaPedido(cargaPedido.Codigo);
                            cargaPedido.Pedido.NotasFiscais.Clear();
                        }

                    }

                    cargaPedido.Pedido.ControleNumeracao = cargaPedido.Pedido.Codigo;
                    repPedido.Atualizar(cargaPedido.Pedido);

                    if (!cargaPedido.Pedido.PedidoTransbordo && !cargaCancelamento.DuplicarCarga)
                    {
                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLsNotasFiscais = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);

                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscaisAjustarCanhoto = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
                        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLsNotasFiscais)
                        {
                            if (cargaPedidoXMLNotaFiscalParcials.Any(obj => obj == pedidoXMLNotaFiscal.XMLNotaFiscal.Codigo))
                                continue;

                            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFicaisDaNota = pedidoXMLsNotasFiscais.Count() < 300 ? repPedidoXMLNotaFiscal.BuscarPorXMLNotaFiscal(pedidoXMLNotaFiscal.XMLNotaFiscal.Codigo) : null;

                            if (pedidoXMLNotasFicaisDaNota != null && pedidoXMLNotasFicaisDaNota.Count() > 1)//todo: fiz essa regra de menor que 300 pela avon, verificar uma forma melhor depois.
                            {
                                if (!pedidoXMLNotasFicaisDaNota.Any(obj => obj.CargaPedido.Codigo != pedidoXMLNotaFiscal.CargaPedido.Codigo && obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada))
                                {
                                    if (!configuracao.NaoInativarNotasAoCancelarCarga)
                                    {
                                        pedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva = false;
                                        repXMLNotaFiscal.Atualizar(pedidoXMLNotaFiscal.XMLNotaFiscal);
                                    }
                                }
                                else
                                    pedidoXMLNotaFiscaisAjustarCanhoto.AddRange((from obj in pedidoXMLNotasFicaisDaNota where obj.CargaPedido.Codigo != pedidoXMLNotaFiscal.CargaPedido.Codigo && obj.XMLNotaFiscal.Canhoto?.Carga?.Codigo == cargaCancelamento.Carga.Codigo && obj.CargaPedido.Recebedor == null && obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada select obj).ToList());

                            }
                            else if (!configuracao.NaoInativarNotasAoCancelarCarga)
                            {
                                pedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva = false;
                                repXMLNotaFiscal.Atualizar(pedidoXMLNotaFiscal.XMLNotaFiscal);
                            }
                        }

                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoXMLsParaSubcontratacao = repPedidoCTeParaSubcontratacao.BuscarPorCargaPedido(cargaPedido.Codigo);
                        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoXMLParaSubcontratacao in pedidoXMLsParaSubcontratacao)
                        {
                            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidosCTeParaSubContratacaoDoCTe = pedidoXMLsParaSubcontratacao.Count() < 300 ? repPedidoCTeParaSubcontratacao.BuscarTodosPorCTeSubContratacao(pedidoXMLParaSubcontratacao.CTeTerceiro.Codigo) : null;
                            if (pedidosCTeParaSubContratacaoDoCTe != null && pedidosCTeParaSubContratacaoDoCTe.Count() > 1)//todo: fiz essa regra de menor que 300 pela avon, verificar uma forma melhor depois.
                            {
                                if (!pedidosCTeParaSubContratacaoDoCTe.Any(obj => obj.CargaPedido.Codigo != pedidoXMLParaSubcontratacao.CargaPedido.Codigo && obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada))
                                {
                                    pedidoXMLParaSubcontratacao.CTeTerceiro.Ativo = false;
                                    repCTeTerceiro.Atualizar(pedidoXMLParaSubcontratacao.CTeTerceiro);
                                    Servicos.Log.TratarErro("Inativou o CTeTerceiro de código " + pedidoXMLParaSubcontratacao.CTeTerceiro.Codigo, "CTeTerceiro");
                                }
                            }
                            else
                            {
                                pedidoXMLParaSubcontratacao.CTeTerceiro.Ativo = false;
                                repCTeTerceiro.Atualizar(pedidoXMLParaSubcontratacao.CTeTerceiro);
                                Servicos.Log.TratarErro("Inativou o CTeTerceiro de código " + pedidoXMLParaSubcontratacao.CTeTerceiro.Codigo, "CTeTerceiro");
                            }
                        }

                        if (!cargaCancelamento.DuplicarCarga)
                            svcCanhoto.AjustarCanhotosCargaCancelada(pedidoXMLNotaFiscaisAjustarCanhoto, tipoServicoMultisoftware, configuracao, unitOfWork);
                    }

                    if (cargaPedido.CTeEmitidoNoEmbarcador && !cargaCancelamento.DuplicarCarga)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> cargaPedidoDocumentoCTes = repCargaPedidoDocumentoCTe.BuscarPorCargaPedido(cargaPedido.Codigo);
                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe in cargaPedidoDocumentoCTes)
                        {
                            if (cargaPedidoDocumentoCTe.CTe.Status == "A")
                            {
                                cargaPedidoDocumentoCTe.CTe.CTeSemCarga = true;
                                repCTe.Atualizar(cargaPedidoDocumentoCTe.CTe);
                            }
                        }

                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe> cargaPedidoDocumentoMDFes = repCargaPedidoDocumentoMDFe.BuscarPorCargaPedido(cargaPedido.Codigo);
                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe cargaPedidoDocumentoMDFe in cargaPedidoDocumentoMDFes)
                        {
                            if (cargaPedidoDocumentoMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado)
                            {
                                cargaPedidoDocumentoMDFe.MDFe.MDFeSemCarga = true;
                                repMDFe.Atualizar(cargaPedidoDocumentoMDFe.MDFe);
                            }
                        }
                    }
                }
                else
                {
                    if (repCargaPedido.FoiDisponibilizadoPedidoParaNovaMontagemCarga(cargaPedido.CargaPedidoTrechoAnterior.Codigo))
                    {
                        cargaPedido.Pedido.PedidoTotalmenteCarregado = false;
                        repPedido.Atualizar(cargaPedido.Pedido);
                    }
                }
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                if (cargaPedido.CargaPedidoFilialEmissora)
                {
                    if (cargaPedido.CargaPedidoProximoTrecho != null && (cargaPedido.CargaPedidoProximoTrecho.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && cargaPedido.CargaPedidoProximoTrecho.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada))
                    {
                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoCTesParaSubContratacao = repPedidoCTeParaSubcontratacao.BuscarPorCargaPedido(cargaPedido.CargaPedidoProximoTrecho.Codigo);

                        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao in pedidoCTesParaSubContratacao)
                        {
                            pedidoCTeParaSubContratacao.CTeTerceiro.Ativo = false;
                            repCTeTerceiro.Atualizar(pedidoCTeParaSubContratacao.CTeTerceiro);
                            Servicos.Log.TratarErro("Inativou o CTeTerceiro de código " + pedidoCTeParaSubContratacao.CTeTerceiro.Codigo, "CTeTerceiro");
                        }

                        cargaPedido.CargaPedidoProximoTrecho.CargaPedidoTrechoAnterior = null;

                        repCargaPedido.Atualizar(cargaPedido.CargaPedidoProximoTrecho);
                        cargaPedido.CargaPedidoProximoTrecho.Carga.AguardandoEmissaoDocumentoAnterior = true;
                        cargaPedido.CargaPedidoProximoTrecho.Carga.DataInicioEmissaoDocumentos = null;
                        cargaPedido.CargaPedidoProximoTrecho.Carga.DataEnvioUltimaNFe = null;
                        repCarga.Atualizar(cargaPedido.CargaPedidoProximoTrecho.Carga);
                    }
                }

                cargaPedido.Ativo = false;
                repCargaPedido.Atualizar(cargaPedido);
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidoMontagemCargaAtualizar)
            {
                if (cargaPedido.CargaPedidoTrechoAnterior != null)
                {
                    cargaPedido.CargaPedidoTrechoAnterior.CargaPedidoProximoTrecho = null;
                    repCargaPedido.Atualizar(cargaPedido.CargaPedidoTrechoAnterior);
                    repCarga.Atualizar(cargaPedido.CargaPedidoTrechoAnterior.Carga);
                }
            }

            Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = repPreCarga.BuscarPorCarga(cargaCancelamento.Carga.Codigo);
            if (preCarga != null)
            {
                preCarga.SituacaoPreCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreCarga.Cancelada;

                repPreCarga.Atualizar(preCarga);
            }

            new Pallets.DevolucaoPallets(unitOfWork).CancelarPallets(cargaCancelamento.Carga, tipoServicoMultisoftware);

            if (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiEmbarcador)
                cargaCancelamento.Carga.ControleNumeracao = cargaCancelamento.Carga.Codigo;

            if (cargaCancelamento.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Anulacao)
            {
                cargaCancelamento.Carga.DataAtualizacaoCarga = DateTime.Now;
                cargaCancelamento.Carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada;
                repCarga.CancelarCargasVinculadas(cargaCancelamento.Carga.Codigo, cargaCancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada);

                if (cargaCancelamento.Carga.CargaAgrupada)
                    repCarga.CancelarCargasAgrupadas(cargaCancelamento.Carga.Codigo, cargaCancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada);
            }
            else
            {
                cargaCancelamento.Carga.DataAtualizacaoCarga = DateTime.Now;
                cargaCancelamento.Carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada;
                repCarga.CancelarCargasVinculadas(cargaCancelamento.Carga.Codigo, cargaCancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada);

                if (cargaCancelamento.Carga.CargaAgrupada)
                    repCarga.CancelarCargasAgrupadas(cargaCancelamento.Carga.Codigo, cargaCancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada);
            }

            Servicos.Embarcador.Integracao.Ortec.IntegracaoOrtec.RemoverVinculosCargaCancelada(cargaCancelamento.Carga, unitOfWork);
            servicoCargaJanelaCarregamentoPrioridade.RemoverPrioridadesPorCarga(cargaCancelamento.Carga);

            cargaCancelamento.Carga.DataInicioCalculoFrete = null;
            cargaCancelamento.Carga.CalculandoFrete = false;
            cargaCancelamento.Carga.CalcularFreteSemEstornarComplemento = false;

            erro = servicoRetornoCarga.ValidarCargaCanceladaParaNovoRetorno(cargaCancelamento.Carga);
            if (!string.IsNullOrWhiteSpace(erro))
                return false;

            servicoRetornoCarga.DisponibilizarCargaCanceladaParaNovoRetorno(cargaCancelamento.Carga);
            CancelarEDIIntegracao(cargaCancelamento.Carga, unitOfWork);

            repCarga.Atualizar(cargaCancelamento.Carga);

            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorCarga(cargaCancelamento.Carga.Codigo);

            if (cargaCIOT != null)
            {
                if (cargaCIOT.CIOT.CIOTPorPeriodo)
                {
                    repCargaCIOT.Deletar(cargaCIOT);

                    List<Dominio.Entidades.Embarcador.Documentos.CIOTCTe> ciotCTes = cargaCancelamento.Carga.CargaCTes.Select(o => o.CIOTs.Where(ciotCTe => ciotCTe.CIOT.Codigo == cargaCIOT.CIOT.Codigo)).SelectMany(o => o).ToList();

                    foreach (Dominio.Entidades.Embarcador.Documentos.CIOTCTe ciotCTe in ciotCTes)
                        repCIOTCTe.Deletar(ciotCTe);
                }
                else
                {
                    if (cargaCIOT.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia ||
                         cargaCIOT.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao)
                    {
                        cargaCIOT.CIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Cancelado;
                        repCIOT.Atualizar(cargaCIOT.CIOT);
                    }
                }
            }

            svcCanhoto.InativarCanhotosAvulsosCarga(cargaCancelamento.Carga, unitOfWork);

            if (cargaCancelamento.Carga.CargaTransbordo)
                svcCanhoto.AjustarCanhotosCargaTransbordo(cargaCancelamento.Carga, unitOfWork);

            svcCreditoMovimentacao.ExtornarCreditoObtidoNaCarga(cargaCancelamento.Carga, unitOfWork);
            svcCarga.CancelarAgendamento(cargaCancelamento.Carga, tipoServicoMultisoftware, unitOfWork, true);
            svcCarga.CancelarAgendamentoPallet(cargaCancelamento.Carga, unitOfWork);
            servicoMovimentacaoPallet.CancelarMovimentacaoPalletPorCarga(cargaCancelamento.Carga);
            svcCarga.CancelarConsolidacao(cargaCancelamento.Carga, tipoServicoMultisoftware, unitOfWork);
            servicoColetaContainer.CancelarColetaContainer(cargaCancelamento.Carga);
            new Servicos.Embarcador.Integracao.IntegracaoPedidoRoterizador(unitOfWork).AdicionarParaIntegracaoAutomaticamente(codigosPedidoCancelarRoteirizacao, TipoRoteirizadorIntegracao.CancelarPedido);

            List<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete> cargaComplementosFrete = repCargaComplementoFrete.BuscarPorCarga(cargaCancelamento.Carga.Codigo);
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete cargaComplemento in cargaComplementosFrete)
                svcCargaComplementoFrete.ExtornarComplementoDeFrete(cargaComplemento, tipoServicoMultisoftware, unitOfWork);

            if (!cargaCancelamento.CancelarDocumentosEmitidosNoEmbarcador && !RemoverEGerarMovimentacaoDosCTesEmitidosPorOutroSistema(out erro, cargaCancelamento, unitOfWork, tipoServicoMultisoftware, ""))
                return false;

            if (cargaCancelamento.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Anulacao)
                cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.Anulada;
            else
                cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.Cancelada;

            repCargaCancelamento.Atualizar(cargaCancelamento);

            ReverterSaldoContratoPrestacaoServico(cargaCancelamento.Carga, unitOfWork);

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && !cargaCancelamento.Carga.DataFinalizacaoEmissao.HasValue) //caso não tenha gerado os lotes de documentos autorizados, gera os mesmos
                Servicos.Embarcador.Escrituracao.DocumentoEscrituracao.AdicionarDocumentoParaEscrituracao(cargaCancelamento.Carga, tipoServicoMultisoftware, unitOfWork);

            Servicos.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento.AdicionarDocumentoParaEscrituracao(cargaCancelamento.Carga, unitOfWork);

            return true;
        }

        private static bool ValidarMDFesEmCancelamento(out string erro, ref bool mdfesLiberados, Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork)
        {
            erro = null;

            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
            int codigoCargaCancelamentoFiltrar = 0;

            if ((cargaCancelamento.TipoCancelamentoCargaDocumento == TipoCancelamentoCargaDocumento.Documentos) || (cargaCancelamento.TipoCancelamentoCargaDocumento == TipoCancelamentoCargaDocumento.TodosDocumentos))
                codigoCargaCancelamentoFiltrar = cargaCancelamento.Codigo;

            if (repCargaMDFe.ExisteMDFeEmCancelamento(cargaCancelamento.Carga.Codigo, codigoCargaCancelamentoFiltrar))
                mdfesLiberados = false;
            else if (repCargaMDFe.ExisteMDFeComRejeicaoNoCancelamento(cargaCancelamento.Carga.Codigo, codigoCargaCancelamentoFiltrar))
            {
                erro = "Não foi possível cancelar todos os MDF-es, verifique o retorno do SEFAZ.";
                return false;
            }

            return true;
        }

        private static bool CancelarMDFesCarga(out string erro, ref bool mdfesLiberados, Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            erro = null;

            Repositorio.Embarcador.Cargas.CargaMDFe repositorioCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
            Servicos.MDFe servicoMDFe = new Servicos.MDFe();
            List<int> codigosCargasMDFe;

            if (cargaCancelamento.TipoCancelamentoCargaDocumento == TipoCancelamentoCargaDocumento.Documentos)
            {
                int codigoCargaMDFe = repositorioCargaMDFe.BuscarCodigoPorCargaECTe(cargaCancelamento.Carga.Codigo, cargaCancelamento.CTe.Codigo);

                codigosCargasMDFe = (codigoCargaMDFe > 0) ? new List<int>() { codigoCargaMDFe } : new List<int>();
            }
            else if (cargaCancelamento.TipoCancelamentoCargaDocumento == TipoCancelamentoCargaDocumento.TodosDocumentos)
                codigosCargasMDFe = repositorioCargaMDFe.BuscarCodigosPorCargaSemCancelamento(cargaCancelamento.Carga.Codigo);
            else
                codigosCargasMDFe = repositorioCargaMDFe.BuscarCodigosPorCarga(cargaCancelamento.Carga.Codigo);

            foreach (int codigoCargaMDFe in codigosCargasMDFe)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe = repositorioCargaMDFe.BuscarPorCodigoComFetch(codigoCargaMDFe);

                if (cargaCancelamento.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Anulacao)
                {
                    if (cargaMDFe.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe && cargaMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado)
                        EnviarMDFesParaEncerramentoViaAnulacao(cargaCancelamento, cargaMDFe, tipoServicoMultisoftware, cargaCancelamento.Carga, unitOfWork, "", Auditado);
                }
                else
                {
                    if (cargaMDFe.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe)
                    {
                        if (cargaMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado)
                        {
                            if (!Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(cargaMDFe.MDFe.SistemaEmissor).CancelarMdfe(cargaMDFe.MDFe.Codigo, cargaMDFe.MDFe.Empresa.Codigo, cargaCancelamento.MotivoCancelamento, unitOfWork))
                            {
                                erro = "Não foi possível cancelar o MDF-e " + cargaMDFe.MDFe.Numero + ".";
                                return false;
                            }
                            else
                                mdfesLiberados = false;
                        }
                        else if (cargaMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.EmCancelamento)
                        {
                            mdfesLiberados = false;
                        }
                        else if (cargaMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Enviado || cargaMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Pendente || cargaMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.EmEncerramento)
                        {
                            erro = "Não é possível cancelar a carga enquanto existir(em) MDF-e(s) na situação de Enviado, Pendente ou Em Encerramento.";
                            return false;
                        }
                    }
                }

                cargaMDFe.CargaCancelamento = cargaCancelamento;
                repositorioCargaMDFe.Atualizar(cargaMDFe);
            }

            unitOfWork.FlushAndClear();

            return true;
        }

        private static bool ValidarCTesEmCancelamento(out string erro, ref bool ctesLiberados, Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            erro = null;

            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            if (repCargaCTe.ExisteCTeEmCancelamento(cargaCancelamento.Carga.Codigo))
                ctesLiberados = false;
            else if ((cargaCancelamento.CTe == null || (cargaCancelamento.CTe != null && cargaCancelamento.CTe.Cancelado == "N")) &&
                repCargaCTe.ExisteCTeNaoCancelado(cargaCancelamento.Carga.Codigo, tipoServicoMultisoftware, cargaCancelamento.LiberarCancelamentoComCTeNaoInutilizado, !cargaCancelamento.CancelarDocumentosEmitidosNoEmbarcador))
            {
                erro = "Não foi possível cancelar/inutilizar todos os CT-es.";
                return false;
            }

            return true;
        }

        private static bool CancelarCTesCarga(out string erro, ref bool ctesLiberados, Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            erro = null;

            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();

            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
            Servicos.NFSe svcNFSe = new Servicos.NFSe();

            bool deveAnularOsCTesEmitidosNoEmbarcador = (cargaCancelamento.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Anulacao /*&& !cargaCancelamento.DuplicarCarga*/);

            int codigoCTeCancelamentoUnitario = (cargaCancelamento.Carga.TipoOperacao?.ConfiguracaoCarga?.TipoCancelamentoCargaDocumento ?? TipoCancelamentoCargaDocumento.Carga) == TipoCancelamentoCargaDocumento.Documentos ? cargaCancelamento.CTe?.Codigo ?? 0 : 0;

            List<int> codigosCargaCTe = repCargaCTe.BuscarCodigosPorCargaParaCancelamento(cargaCancelamento.Carga.Codigo, tipoServicoMultisoftware, deveAnularOsCTesEmitidosNoEmbarcador, codigoCTeCancelamentoUnitario);

            foreach (int codigoCargaCTe in codigosCargaCTe)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCodigoComFetch(codigoCargaCTe);

                if (cargaCTe.CTe == null)
                    continue;

                if (cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe &&
                    cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe)
                {
                    DateTime dataCancelamento = DateTime.Now;

                    unitOfWork.Start();

                    if (cargaCTe.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Rejeitada ||
                        cargaCTe.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmDigitacao)
                        cargaCTe.CTe.Status = "I";
                    else
                        cargaCTe.CTe.Status = "C";

                    cargaCTe.CTe.DataRetornoSefaz = dataCancelamento;
                    cargaCTe.CTe.DataCancelamento = dataCancelamento;
                    cargaCTe.CTe.ObservacaoCancelamento = cargaCancelamento.MotivoCancelamento;

                    repCTe.Atualizar(cargaCTe.CTe);

                    svcCTe.AjustarAverbacoesParaCancelamento(cargaCTe.CTe.Codigo, unitOfWork);

                    unitOfWork.CommitChanges();

                    unitOfWork.FlushAndClear();

                    continue;
                }

                //Problemas com CT-es inutilizado gerencialmente e autorizados na Sefaz
                if (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS && cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe && cargaCTe.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Rejeitada)
                {
                    var cteRetorno = svcCTe.ConsultaEAtualizaStatusDocumento(cargaCTe.CTe, tipoServicoMultisoftware, unitOfWork);

                    if (cteRetorno.SituacaoCTeSefaz == SituacaoCTeSefaz.Autorizada)
                    {
                        cargaCTe.CTe = cteRetorno;
                        repCargaCTe.Atualizar(cargaCTe);
                        unitOfWork.CommitChanges();
                    }
                }

                if (cargaCTe.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Autorizada)
                {
                    if (cargaCancelamento.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Anulacao)
                    {
                        DateTime dataAnulacao = DateTime.Now;

                        unitOfWork.Start();

                        cargaCTe.CTe.Status = "Z";
                        cargaCTe.CTe.DataRetornoSefaz = dataAnulacao;
                        cargaCTe.CTe.DataAnulacao = dataAnulacao;
                        cargaCTe.CTe.ObservacaoCancelamento = cargaCancelamento.MotivoCancelamento;

                        repCTe.Atualizar(cargaCTe.CTe);

                        unitOfWork.CommitChanges();

                        unitOfWork.FlushAndClear();

                        continue;
                    }
                    else
                    {
                        if (cargaCTe.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe)
                        {
                            if (cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                            {
                                if (!Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoCTe(cargaCTe.CTe.SistemaEmissor).CancelarCte(cargaCTe.CTe.Codigo, cargaCTe.CTe.Empresa.Codigo, cargaCancelamento.MotivoCancelamento, unitOfWork))
                                {
                                    erro = "Não foi possível cancelar o CT-e " + cargaCTe.CTe.Numero + ".";
                                    return false;
                                }
                            }
                            else if (cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                            {
                                if (!svcNFSe.CancelarNFSe(cargaCTe.CTe.Codigo, unitOfWork))
                                {
                                    erro = "Não foi possível cancelar a NFS-e " + cargaCTe.CTe.Numero + ".";
                                    return false;
                                }
                            }
                        }
                    }
                    ctesLiberados = false;
                }
                else if (cargaCTe.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmCancelamento ||
                         cargaCTe.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmInutilizacao)
                {
                    ctesLiberados = false;
                }
                else if (cargaCTe.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Rejeitada ||
                         cargaCTe.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmDigitacao)
                {
                    if (cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                    {
                        if (!svcCTe.Inutilizar(cargaCTe.CTe.Codigo, cargaCTe.CTe.Empresa.Codigo, Utilidades.String.Left(cargaCancelamento.MotivoCancelamento, 200), tipoServicoMultisoftware, unitOfWork))
                        {
                            erro = "Não foi possível inutilizar o CT-e " + cargaCTe.CTe.Numero + ".";
                            return false;
                        }
                    }
                    else if (cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                    {
                        DateTime dataCancelamento = DateTime.Now;

                        unitOfWork.Start();

                        cargaCTe.CTe.Status = "I";
                        cargaCTe.CTe.DataRetornoSefaz = dataCancelamento;
                        cargaCTe.CTe.DataCancelamento = dataCancelamento;

                        cargaCTe.CTe.ObservacaoCancelamento = cargaCancelamento.MotivoCancelamento;

                        repCTe.Atualizar(cargaCTe.CTe);

                        unitOfWork.CommitChanges();
                    }

                    ctesLiberados = false;
                }
                else if (cargaCTe.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Pendente ||
                         cargaCTe.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Enviada)
                {
                    erro = "Não é possível cancelar a carga enquanto existirem CT-e(s) na situação de Enviado ou Pendente.";
                    return false;
                }

                cargaCTe.CargaCancelamento = cargaCancelamento;
                repCargaCTe.Atualizar(cargaCTe);

                unitOfWork.FlushAndClear();
            }

            return true;
        }

        private static void CancelarAverbacoesCTesCargaEmCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

            Servicos.Embarcador.Hubs.Carga svcHubCarga = new Servicos.Embarcador.Hubs.Carga();

            bool averbacoesLiberadas = true;
            bool valePedagiosLiberados = true;

            string erro = null;

            if (cargaCancelamento.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Cancelamento)
            {
                if (!CancelarValePedagios(out erro, ref valePedagiosLiberados, cargaCancelamento, unitOfWork, tipoServicoMultisoftware) ||
                    !CancelarAverbacoesCTe(out erro, ref averbacoesLiberadas, cargaCancelamento, unitOfWork, tipoServicoMultisoftware))
                {
                    cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;
                    cargaCancelamento.MensagemRejeicaoCancelamento = erro;

                    repCargaCancelamento.Atualizar(cargaCancelamento);

                    svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                    return;
                }
            }
            else if (cargaCancelamento.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Anulacao)
            {
                if (!CancelarValePedagios(out erro, ref valePedagiosLiberados, cargaCancelamento, unitOfWork, tipoServicoMultisoftware))
                {
                    cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento;
                    cargaCancelamento.MensagemRejeicaoCancelamento = erro;

                    repCargaCancelamento.Atualizar(cargaCancelamento);

                    svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);

                    return;
                }
            }

            if (averbacoesLiberadas && valePedagiosLiberados)
            {
                unitOfWork.Start();

                if (configuracaoGeralCarga.CancelarCIOTAutomaticamenteFluxoCancelamentoCarga)
                    cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.AgIntegracaoCancelamentoCIOT;
                else
                    cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.AgIntegracao;

                repCargaCancelamento.Atualizar(cargaCancelamento);

                unitOfWork.CommitChanges();

                svcHubCarga.InformarCancelamentoAtualizado(cargaCancelamento.Codigo);
            }

            unitOfWork.FlushAndClear();
        }

        private static bool CancelarValePedagios(out string erro, ref bool valePegadiosLiberados, Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            erro = null;

            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);

            if (cargaCancelamento.EnviouValePedagiosParaCancelamento)
            {
                if (!cargaCancelamento.LiberarCancelamentoComValePedagioRejeitado && repCargaIntegracaoValePedagio.VerificarVPnaoCanceladoPorCarga(cargaCancelamento.Carga.Codigo, true))
                {
                    erro = "Não foi possível cancelar os vale pedágios da carga.";
                    return false;
                }

                if (repCargaIntegracaoValePedagio.VerificarEmCanceladoPorCarga(cargaCancelamento.Carga.Codigo))
                    valePegadiosLiberados = false;
            }
            else
            {
                List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> cargaIntegracoesValePedagio = repCargaIntegracaoValePedagio.BuscarPorCarga(cargaCancelamento.Carga.Codigo, true);
                bool enviouCancelamento = false;
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tipoIntegracao = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>(){
                           Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Repom
                           , Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.PagBem
                           , Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DBTrans
                           , Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Pamcard
                           , Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EFrete
                           , Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Extratta
                           , Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DigitalCom
                           , Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Ambipar
                           , Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NDDCargo
                        };

                foreach (Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagio in cargaIntegracoesValePedagio)
                {
                    if ((cargaIntegracaoValePedagio.SituacaoValePedagio == SituacaoValePedagio.Confirmada) ||
                        (cargaIntegracaoValePedagio.SituacaoValePedagio == SituacaoValePedagio.Comprada && tipoIntegracao.Contains(cargaIntegracaoValePedagio.TipoIntegracao.Tipo)))
                    {
                        enviouCancelamento = true;
                        cargaIntegracaoValePedagio.SituacaoValePedagio = SituacaoValePedagio.EmCancelamento;
                        cargaIntegracaoValePedagio.CargaCancelamento = cargaCancelamento;
                        repCargaIntegracaoValePedagio.Atualizar(cargaIntegracaoValePedagio);
                    }
                }

                if (enviouCancelamento)
                {
                    cargaCancelamento.EnviouValePedagiosParaCancelamento = true;

                    repCargaCancelamento.Atualizar(cargaCancelamento);

                    valePegadiosLiberados = false;
                }
                else
                    valePegadiosLiberados = true;
            }

            return true;
        }

        private static bool CancelarAverbacoesCTe(out string erro, ref bool averbacoesLiberadas, Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            erro = null;

            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);

            if (cargaCancelamento.EnviouAverbacoesCTesParaCancelamento)
            {
                Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);

                if (!cargaCancelamento.LiberarCancelamentoComAverbacaoCTeRejeitada && repAverbacaoCTe.ExistePorCargaEStatus(cargaCancelamento.Carga.Codigo, Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso))
                {
                    erro = "Não foi possível cancelar todas as averbações dos CT-es.";
                    return false;
                }

                if (repAverbacaoCTe.ExistePorCargaEStatus(cargaCancelamento.Carga.Codigo, new Dominio.Enumeradores.StatusAverbacaoCTe[] { Dominio.Enumeradores.StatusAverbacaoCTe.Enviado, Dominio.Enumeradores.StatusAverbacaoCTe.AgCancelamento }))
                    averbacoesLiberadas = false;
                else
                {
                    List<Dominio.Entidades.AverbacaoCTe> averbacaoCTes = repAverbacaoCTe.BuscarPorCargaCancelamento(cargaCancelamento.Carga.Codigo, cargaCancelamento.Codigo);
                    foreach (Dominio.Entidades.AverbacaoCTe averbacaoCTe in averbacaoCTes)
                    {
                        averbacaoCTe.CargaCancelamento = cargaCancelamento;
                        repAverbacaoCTe.Atualizar(averbacaoCTe);
                    }
                }
            }
            else
            {
                cargaCancelamento.EnviouAverbacoesCTesParaCancelamento = true;

                repCargaCancelamento.Atualizar(cargaCancelamento);

                averbacoesLiberadas = false;
            }

            return true;
        }

        private static bool ValidarCancelamentoCarga(out string erro, Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);

            erro = string.Empty;


            if (cargaCancelamento.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Cancelamento)
            {
                if (repCargaMDFe.ExisteMDFeInvalidoParaCancelamento(cargaCancelamento.Carga.Codigo))
                {
                    erro = "Não é possível cancelar MDF-e(s) que foram emitidos a mais de 1 dia.";
                    return false;
                }

                if (repCargaOcorrencia.ExisteOcorrenciaInvalidaParaCancelamento(cargaCancelamento.Carga.Codigo))
                {
                    erro = "Existem ocorrências não canceladas para esta carga, não sendo possível cancelar a mesma.";
                    return false;
                }

                if (!cargaCancelamento.Carga.CargaTransbordo) // se é uma carga de transbordo os CT-es não podem ser cancelados nesta carga, e sim na carga original.
                {
                    if (repCargaCTe.PossuiCTesAutorizadosEmitidosPeloEmbarcador(cargaCancelamento.Carga.Codigo) && cargaCancelamento.CancelarDocumentosEmitidosNoEmbarcador)
                    {
                        erro = "É necessário importar o XML de cancelamento do embarcador para finalizar o cancelamento da carga.";
                        return false;
                    }
                }

                if (cargaCancelamento.Carga.CalculandoFrete && cargaCancelamento.Carga.SituacaoRoteirizacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Erro &
                    (cargaCancelamento.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador ||
                     cargaCancelamento.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                    && !repCargaPedido.VerificarPorCargaSePendenteDadosRecebedor(cargaCancelamento.Carga.Codigo))
                {
                    erro = "Não é possível cancelar a carga enquanto o frete está sendo calculado, tente novamente em alguns minutos.";
                    return false;
                }
            }

            List<int> provisoes = repDocumentoProvisao.BuscarNumeroDocumentoInvalidoParaCancelamento(cargaCancelamento.Carga.Codigo);
            if (provisoes.Count > 0)
            {
                erro = "Antes de cancelar a carga é necessário finalizar a provisão de seus documento, a carga está sendo provisionada na provisão de número " + string.Join(", ", provisoes) + ".";
                return false;
            }

            List<int> numeroDocumento = null;
            if (!cargaCancelamento.Carga.CargaAgrupada)
                numeroDocumento = repDocumentoFaturamento.BuscarNumeroDocumentoInvalidoParaCancelamento(cargaCancelamento.Carga.Codigo);
            else
                numeroDocumento = repDocumentoFaturamento.BuscarNumeroDocumentoInvalidoParaCancelamentoCargaAgrupamento(cargaCancelamento.Carga.Codigo);

            if (numeroDocumento.Count > 0)
            {
                erro = "Antes de cancelar a carga é necessário finalizar a pagamento, a carga está sendo finalizada no pagamento de número " + string.Join(", ", numeroDocumento) + ".";
                return false;
            }

            bool liquidada = false;
            if (!cargaCancelamento.Carga.CargaAgrupada)
                liquidada = repDocumentoFaturamento.ExisteDocumentoPagoPorCarga(cargaCancelamento.Carga.Codigo);
            else
                liquidada = repDocumentoFaturamento.ExisteDocumentoPagoPorCargaAgrupada(cargaCancelamento.Carga.Codigo);

            if (liquidada)
            {
                erro = "Não é possível cancelar a carga pois os documentos já foram pagos.";
                return false;
            }
            if (cargaCancelamento.Tipo == TipoCancelamentoCarga.Cancelamento)
            {
                List<string> codigosCargasRedespachoNaoCanceladas = repCargaPedido.BuscarNumeroCargaRedespachoNaoCancelada(cargaCancelamento.Carga.Codigo);
                if (codigosCargasRedespachoNaoCanceladas.Count > 0)
                {
                    if (cargaCancelamento.Carga.TipoOperacao?.GerarRedespachoParaOutrasEtapasCarregamento ?? false)
                    {
                        //processo da Arcelor, cargas de redespacho sao geradas pelo Multi nao sao visualizadas pelo SAP entao devem ser canceladas automaticamente
                        List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasRedespacho = repCargaPedido.BuscarCargasRedespachoNaoCancelada(cargaCancelamento.Carga.Codigo);
                        foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaRedespacho in cargasRedespacho)
                        {
                            cargaRedespacho.CargaFechada = false;
                            cargaRedespacho.SituacaoCarga = SituacaoCarga.Cancelada;
                            repCarga.Atualizar(cargaRedespacho);
                        }
                    }
                    else
                    {

                        erro = "Esta carga possui uma carga de redespacho não cancelada, para poder cancelar essa carga primeiro é necessário cancelar a carga de redespacho número " + string.Join(", ", codigosCargasRedespachoNaoCanceladas) + ". ";
                        return false;
                    }
                }
            }


            if (!ValidarFechamentoDiarioDocumentos(out erro, cargaCancelamento, unitOfWork, tipoServicoMultisoftware))
                return false;

            if (!ValidarSeCargaEstaVinculadaAAlgumDocumento(out erro, cargaCancelamento.Carga, unitOfWork, tipoServicoMultisoftware))
                return false;

            List<SituacaoCarga> situacoesPermiteCancelamento = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga>
            {
                SituacaoCarga.EmTransporte,
                SituacaoCarga.Encerrada,
                SituacaoCarga.AgImpressaoDocumentos,
                SituacaoCarga.Nova,
            };

            if (situacoesPermiteCancelamento.Contains(cargaCancelamento.Carga.SituacaoCarga))
                return true;

            if (cargaCancelamento.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Anulacao)
            {
                if (repCargaCTe.PossuiCTesInvalidosParaAnulacao(cargaCancelamento.Carga.Codigo))
                {
                    erro = "Não é possível anular a carga enquanto existirem documentos na situação de Enviado, Pendente, Em Cancelamento ou Em Inutilização.";
                    return false;
                }
            }
            else
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                int segundos = -configuracao.TempoSegundosParaInicioEmissaoDocumentos;

                if ((cargaCancelamento.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete
                    && cargaCancelamento.Carga.ExigeNotaFiscalParaCalcularFrete
                                && (cargaCancelamento.Carga.DataEnvioUltimaNFe.HasValue && cargaCancelamento.Carga.DataEnvioUltimaNFe.Value <= DateTime.Now.AddSeconds(segundos))
                                && !cargaCancelamento.Carga.AguardandoEmissaoDocumentoAnterior && !cargaCancelamento.Carga.CalculandoFrete && cargaCancelamento.Carga.CargaFechada
                                && !cargaCancelamento.Carga.PendenteGerarCargaDistribuidor
                                && !cargaCancelamento.Carga.AgValorRedespacho)
                                || (cargaCancelamento.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe
                                && !cargaCancelamento.Carga.ExigeNotaFiscalParaCalcularFrete && (cargaCancelamento.Carga.DataEnvioUltimaNFe.HasValue
                                && cargaCancelamento.Carga.DataEnvioUltimaNFe.Value <= DateTime.Now.AddSeconds(segundos))
                                && !cargaCancelamento.Carga.AguardandoEmissaoDocumentoAnterior
                                && !cargaCancelamento.Carga.AgValorRedespacho
                                && !cargaCancelamento.Carga.PendenteGerarCargaDistribuidor
                                && !cargaCancelamento.Carga.AguardarIntegracaoEtapaTransportador) ||
                                (cargaCancelamento.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos
                                 && cargaCancelamento.Carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora
                                 && cargaCancelamento.Carga.EmEmissaoCTeSubContratacaoFilialEmissora
                                 && !cargaCancelamento.Carga.PossuiPendencia
                                 && !cargaCancelamento.Carga.AgValorRedespacho)
                                )
                {
                    erro = "Não é possível cancelar a carga os documentos estão sendo gerados, tente novamente em instantes.";
                    return false;
                }

                if (cargaCancelamento.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos &&
                                           !cargaCancelamento.Carga.PossuiPendencia &&
                                           !cargaCancelamento.Carga.AgImportacaoCTe &&
                                           !cargaCancelamento.Carga.EmEmissaoCTeSubContratacaoFilialEmissora &&
                                           !cargaCancelamento.Carga.AgGeracaoCTesAnteriorFilialEmissora &&
                                           !cargaCancelamento.Carga.AgImportacaoMDFe &&
                                           !cargaCancelamento.Carga.CTesEmDigitacao &&
                                           !cargaCancelamento.Carga.IntegrandoValePedagio &&
                                           !cargaCancelamento.Carga.EmitindoCTes &&
                                           !cargaCancelamento.Carga.FinalizandoProcessoEmissao)
                {
                    erro = "Não é possível cancelar a carga os documentos estão sendo emitidos, tente novamente em instantes.";
                    return false;
                }

                if (cargaCancelamento.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos &&
                   cargaCancelamento.Carga.FinalizandoProcessoEmissao &&
                   cargaCancelamento.Carga.CargaFechada)
                {
                    erro = "A carga está em processo de finalização de emissão dos documentos, tente novamente em instantes.";
                    return false;
                }

                if (cargaCancelamento.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao
                    && cargaCancelamento.Carga.GerandoIntegracoes == true)
                {
                    erro = "Não é possível cancelar a carga enquanto a integração está sendo realizada, tente novamente em instantes.";
                    return false;
                }

            }

            return true;
        }

        private static bool ValidarFechamentoDiarioDocumentos(out string erro, Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            erro = null;

            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return true;

            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            List<DateTime> datasEmissao = repCargaCTe.BuscarDatasEmissaoPorCarga(cargaCancelamento.Carga.Codigo);

            if (datasEmissao.Count <= 0)
                return true;

            foreach (DateTime dataEmissao in datasEmissao)
            {
                if (Servicos.Embarcador.Financeiro.FechamentoDiario.VerificarSeExisteFechamento(cargaCancelamento?.Carga?.Empresa?.Codigo ?? 0, dataEmissao, unitOfWork))
                {
                    erro = "Já existe um fechamento diário igual ou posterior à data " + dataEmissao.ToString("dd/MM/yyyy") + ", não sendo possível realizar o cancelamento/anulação da carga.";
                    return false;
                }
            }

            return true;
        }

        private static bool ValidarSeCargaEstaVinculadaAAlgumDocumento(out string erro, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            erro = null;

            Repositorio.Embarcador.Fatura.FaturaCarga repFaturaCarga = new Repositorio.Embarcador.Fatura.FaturaCarga(unidadeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoCarga repAcertoCarga = new Repositorio.Embarcador.Acerto.AcertoCarga(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeTrabalho);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

            List<int> numeroContratos = repContratoFrete.BuscarNumeroContratoInvalidoParaCancelamento(carga.Codigo);
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && !configuracaoGeralCarga.CancelarCIOTAutomaticamenteFluxoCancelamentoCarga && numeroContratos.Count > 0)
            {
                erro = "A carga está vinculada ao contrato de frete nº " + string.Join(", ", numeroContratos) + ", que já está aprovado/finalizado, não sendo possível realizar o cancelamento/anulação.";
                return false;
            }

            List<int> numerosFaturas = repFaturaCarga.BuscarNumeroFaturaPorCarga(carga.Codigo);
            if (numerosFaturas.Count > 0)
            {
                erro = "A carga está vinculada à(s) fatura(s) nº " + string.Join(", ", numerosFaturas) + ", não sendo possível realizar o cancelamento/anulação.";
                return false;
            }

            List<int> numeroAcertosViagem = repAcertoCarga.BuscarNumeroAcertoPorCarga(carga.Codigo);
            if (numeroAcertosViagem.Count > 0)
            {
                erro = "A carga está vinculada ao(s) acerto(s) de viagem nº " + string.Join(", ", numeroAcertosViagem) + ", não sendo possível realizar o cancelamento/anulação.";
                return false;
            }

            if (carga.CargaTransbordo)
                return true;

            bool documentoFinanceiroPorCarga = repDocumentoFaturamento.ExistePorCarga(carga.Codigo);
            bool documentoFinanceiroPorCTe = repDocumentoFaturamento.ExistePorCTe(carga.Codigo);

            if (documentoFinanceiroPorCarga)
            {
                List<int> numerosFaturasNova = repFaturaDocumento.BuscarNumeroFaturaPorCarga(carga.Codigo);
                if (numerosFaturasNova.Count > 0 && !carga.CargaRecebidaDeIntegracao)
                {
                    erro = "A Carga (" + carga.CodigoCargaEmbarcador + ") está vinculada à(s) fatura(s) nº " + string.Join(", ", numerosFaturasNova) + ", não sendo possível realizar o cancelamento/anulação.";
                    return false;
                }

                List<int> numerosTitulos = repTituloDocumento.BuscarNumeroTituloPorCarga(carga.Codigo);
                if (numerosTitulos.Count > 0 && !carga.CargaRecebidaDeIntegracao)
                {
                    erro = "A Carga (" + carga.CodigoCargaEmbarcador + ") está vinculada ao(s) título(s) nº " + string.Join(", ", numerosTitulos) + ", não sendo possível realizar o cancelamento/anulação.";
                    return false;
                }

                List<int> nossoNumeroBoletoTitulos = repTituloDocumento.BuscarNumeroBoletoTituloPorCarga(carga.Codigo);
                if (nossoNumeroBoletoTitulos.Count > 0 && !carga.CargaRecebidaDeIntegracao)
                {
                    erro = "A carga está vinculada a boleto(s) no(s) título(s) nº " + string.Join(", ", nossoNumeroBoletoTitulos) + ", não sendo possível realizar o cancelamento/anulação.";
                    return false;
                }
            }

            if (documentoFinanceiroPorCTe)
            {
                List<int> numerosFaturasNova = repFaturaDocumento.BuscarNumeroFaturaPorCargaCTe(carga.Codigo);
                if (numerosFaturasNova.Count > 0 && !carga.CargaRecebidaDeIntegracao && !configuracaoGeralCarga.AtivarCancelamentoDeFaturaETituloAoFluxoDeCancelamentoNaCarga)
                {
                    erro = "Os documentos da carga estão vinculados à(s) fatura(s) nº " + string.Join(", ", numerosFaturasNova) + ", não sendo possível realizar o cancelamento/anulação.";
                    return false;
                }

                List<int> numerosTitulos = repTituloDocumento.BuscarNumeroTituloPorCargaCTe(carga.Codigo);
                if (numerosTitulos.Count > 0 && !carga.CargaRecebidaDeIntegracao && !configuracaoGeralCarga.AtivarCancelamentoDeFaturaETituloAoFluxoDeCancelamentoNaCarga)
                {
                    erro = "Os documentos da carga estão vinculados ao(s) título(s) nº " + string.Join(", ", numerosTitulos) + ", não sendo possível realizar o cancelamento/anulação.";
                    return false;
                }

                List<int> nossoNumeroBoletoTitulos = repTituloDocumento.BuscarNumeroBoletoTituloPorCargaCTe(carga.Codigo);
                if (nossoNumeroBoletoTitulos.Count > 0 && !carga.CargaRecebidaDeIntegracao && !configuracaoGeralCarga.AtivarCancelamentoDeFaturaETituloAoFluxoDeCancelamentoNaCarga)
                {
                    erro = "Os documentos da carga estão vinculados a boleto(s) no(s) título(s) nº " + string.Join(", ", nossoNumeroBoletoTitulos) + ", não sendo possível realizar o cancelamento/anulação.";
                    return false;
                }
            }

            return true;
        }

        private static bool AjustarCarregamentos(out string erro, Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            erro = null;

            if (cargaCancelamento.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Anulacao)
                return true;

            if (cargaCancelamento.Carga.Carregamento == null || cargaCancelamento.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento)
                return true;

            if (cargaCancelamento.DuplicarCarga)
                return true;

            Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoCarga repCarregamentoCarga = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoCarga> carregamentoCargas = repCarregamentoCarga.BuscarPorCarregamento(cargaCancelamento.Carga.Carregamento.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento> cargasCancelamentoAtualizar = new List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento>();

            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoCarga carregamentoCargaExcluir = null;
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoCarga> carregamentoCargasRecalcuar = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoCarga>();

            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoCarga carregamentoCarga in carregamentoCargas)
            {
                if (carregamentoCarga.Carga.Codigo != cargaCancelamento.Carga.Codigo)
                {
                    if (svcCarga.VerificarSeCargaEstaNaLogistica(carregamentoCarga.Carga, tipoServicoMultisoftware))
                    {
                        if (carregamentoCarga.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                            carregamentoCargasRecalcuar.Add(carregamentoCarga);
                    }
                    else
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamentoMontagem = repCargaCancelamento.BuscarPorCarga(carregamentoCarga.Carga.Codigo);

                        if (cargaCancelamentoMontagem == null)
                        {
                            erro = "Não é possível solicitar o cancelamento desta carga pois a mesma está atrelada ao carregamento " + carregamentoCarga.Carregamento.NumeroCarregamento + " que está com a carga " + carregamentoCarga.Carga.CodigoCargaEmbarcador + " em processo de emissão dos documentos. Para poder cancelar essa carga é necessário cancelar a carga " + carregamentoCarga.Carga.CodigoCargaEmbarcador + ". ";
                            return false;
                        }
                        else
                        {
                            if (cargaCancelamentoMontagem.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento)
                                cargasCancelamentoAtualizar.Add(cargaCancelamentoMontagem);
                        }
                    }
                }
                else
                    carregamentoCargaExcluir = carregamentoCarga;
            }

            unitOfWork.Start();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamentoAtualizar in cargasCancelamentoAtualizar)
            {
                cargaCancelamentoAtualizar.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.EmCancelamento;
                repCargaCancelamento.Atualizar(cargaCancelamentoAtualizar);
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoCarga carregamentoRecalcular in carregamentoCargasRecalcuar)
            {
                carregamentoRecalcular.Carga.DataInicioCalculoFrete = DateTime.Now;
                carregamentoRecalcular.Carga.CalculandoFrete = true;
                repCarga.Atualizar(carregamentoRecalcular.Carga);
            }

            // Problema no cancelamento de carga, ede pedidos com carregamento, aonde os carregamentos seguem como FINALIZADOS
            //  e os pedidos relacionados a uma sessão de roteirização #22275 e #22450
            int codigoCarregamento = AjustarCarregamentoPedidosSessaoRoteirizador(cargaCancelamento, unitOfWork);

            if (carregamentoCargaExcluir != null)
            {
                //int codigoCarregamento = cargaCancelamento.Carga.Carregamento.Codigo;
                cargaCancelamento.Carga.Carregamento = null;

                repCarga.Atualizar(cargaCancelamento.Carga);
                repCarregamentoCarga.Deletar(carregamentoCargaExcluir);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = repCarregamento.BuscarPorCodigo(codigoCarregamento);

                if (carregamento != null)
                    new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork).AjustarCarregamento(carregamento);
            }

            unitOfWork.CommitChanges();

            return true;
        }

        private static int AjustarCarregamentoPedidosSessaoRoteirizador(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido repSessaoRoteirizadorPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);

            int codigoCarregamento = cargaCancelamento.Carga.Carregamento?.Codigo ?? 0;
            if (codigoCarregamento > 0)
            {
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessaoRoteirizador = cargaCancelamento.Carga?.Carregamento?.SessaoRoteirizador;
                if (sessaoRoteirizador != null)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> sessaoRoteirizadorPedidos = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido>();
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidosSessaoEmMontagem = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(cargaCancelamento.Carga.Codigo);
                    List<int> codigosPedidos = (from ped in cargaPedidos select ped.Pedido.Codigo).ToList();
                    sessaoRoteirizadorPedidos = repSessaoRoteirizadorPedido.BuscarSessaoRoteirizadorPedidos(sessaoRoteirizador.Codigo, codigosPedidos);
                    carregamentoPedidosSessaoEmMontagem = repCarregamentoPedido.BuscarPorPedidosEmMontagem(sessaoRoteirizador.Codigo, codigosPedidos);

                    //Agora precisamos ver se o pedido estava em uma sessão e o mesmo não está em outro carregamento... da mesma sessão.. vamos remover o pedido da sessão.
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                    {
                        if (cargaPedido.Pedido.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto)
                        {
                            bool existeCarregamentoEmMontagemSessao = carregamentoPedidosSessaoEmMontagem.Exists(x => x.Pedido.Codigo == cargaPedido.Pedido.Codigo);
                            if (!existeCarregamentoEmMontagemSessao)
                            {
                                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> sessaoRoteirizadorPedido = (from srp in sessaoRoteirizadorPedidos
                                                                                                                                             where srp.Pedido.Codigo == cargaPedido.Pedido.Codigo
                                                                                                                                             select srp).ToList();
                                foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido srp in sessaoRoteirizadorPedido)
                                {
                                    srp.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao;
                                    repSessaoRoteirizadorPedido.Atualizar(srp);
                                }
                            }

                            if (cargaCancelamento.LiberarPedidosParaMontagemCarga)
                            {
                                if (cargaPedido.Expedidor != null)
                                    cargaPedido.Pedido.PedidoRedespachoTotalmenteCarregado = false;
                                else
                                {
                                    cargaPedido.Pedido.PesoSaldoRestante += cargaPedido.Peso;
                                    cargaPedido.Pedido.PedidoTotalmenteCarregado = false;
                                }
                                repPedido.Atualizar(cargaPedido.Pedido);
                            }
                        }
                    }
                }

                cargaCancelamento.Carga.Carregamento.SituacaoCarregamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Cancelado;
                repCarregamento.Atualizar(cargaCancelamento.Carga.Carregamento);
            }

            return codigoCarregamento;
        }

        private static bool CancelarCTesSemCarga(Dominio.Entidades.Embarcador.CTe.CancelamentoCTeSemCarga cancelamentoCTeSemCarga, List<Dominio.Entidades.Embarcador.CTe.CancelamentoCTe> cancelamentoCTes, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                unitOfWork.Start();
                Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.CTe.CancelamentoCTeSemCarga repCTeSemCarga = new Repositorio.Embarcador.CTe.CancelamentoCTeSemCarga(unitOfWork);
                Repositorio.Embarcador.CTe.CancelamentoCTe repCTeCancelamento = new Repositorio.Embarcador.CTe.CancelamentoCTe(unitOfWork);

                if (!cancelamentoCTes.Any())
                    return false;

                foreach (var cancelamentoCTe in cancelamentoCTes)
                {
                    if (cancelamentoCTe.CTe == null)
                        return false;

                    if (cancelamentoCTe.CTe.Status == "A")
                    {
                        if (!Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoCTe(cancelamentoCTe.CTe.SistemaEmissor).CancelarCte(cancelamentoCTe.CTe.Codigo, cancelamentoCTe.CTe.Empresa.Codigo, "Cancelamento de CTe sem Carga", unitOfWork))
                        {
                            cancelamentoCTeSemCarga.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCTeSemCarga.RejeicaoCancelamento;
                            cancelamentoCTeSemCarga.MotivoRejeicao += "Não foi possível cancelar o CT-e " + cancelamentoCTe.CTe.Numero + ".";

                            repCTeSemCarga.Atualizar(cancelamentoCTeSemCarga);
                        }
                    }
                }

                if (string.IsNullOrEmpty(cancelamentoCTeSemCarga.MotivoRejeicao))
                {
                    cancelamentoCTeSemCarga.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCTeSemCarga.Cancelado;
                    repCTeSemCarga.Atualizar(cancelamentoCTeSemCarga);
                }

                unitOfWork.CommitChanges();
                unitOfWork.FlushAndClear();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"Ocorreu uma falha ao cancelar os Ctes - {ex}", "CancelamentoCTesSemCarga");
            }

            return false;
        }

        #endregion Métodos Privados (Cancelamento Novo)
    }
}
