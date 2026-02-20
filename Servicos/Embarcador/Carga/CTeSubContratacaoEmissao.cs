using Repositorio;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga
{
    public class CTeSubContratacaoEmissao : ServicoBase
    {        
        public CTeSubContratacaoEmissao(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public string EmitirCTE(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos tipoEmissaoDocumentos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga tipoContratacaoCarga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento configuracaoCargaEmissaoDocumento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao> pedidosCTeParaSubContratacaoContaContabilContabilizacao, string webServiceConsultaCTe, int totalDocumentos, int quantidadeDocumentosAtualizarCarga, bool indicadorCteSimplificado, ref int totalDocumentosGerados)
        {
            //todo: iniicalmente gera 1 cte por cte de subcontratação, futuramente ver a necessidade de outros tipos.
            string mensagem = "";
            Servicos.Embarcador.Carga.CTePorCTeParaSubcontratacao serCTePorCTeParaSubcontratacao = new CTePorCTeParaSubcontratacao(unitOfWork);
            Servicos.Embarcador.Carga.CTePorCTeParaSVM serCTePorCTeParaSVM = new CTePorCTeParaSVM(unitOfWork);
            Servicos.Embarcador.Carga.RateioCTe serRateioCTe = new Servicos.Embarcador.Carga.RateioCTe(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            if (carga.CargaSVM && carga.PortoOrigem != null && carga.PortoDestino != null && carga.TerminalDestino != null && carga.TerminalOrigem != null)
            {
                bool ratearMesmoQueMultiModal = carga.Empresa.SVMMesmoQueMultimodal.HasValue ? carga.Empresa.SVMMesmoQueMultimodal.Value : false;
                bool ratearPorTerminais = carga.Empresa.SVMTerminaisPortuarioOrigemDestino.HasValue ? carga.Empresa.SVMTerminaisPortuarioOrigemDestino.Value : false;
                bool ratearPorBUS = carga.Empresa.SVMBUSPortoOrigemDestino.HasValue ? carga.Empresa.SVMBUSPortoOrigemDestino.Value : false;

                if (carga.PortoOrigem.FormaEmissaoSVM == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaEmissaoSVM.PortoOrigem || carga.PortoDestino.FormaEmissaoSVM == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaEmissaoSVM.PortoDestino)
                {
                    //solicitado por e-mail e chamados abertos pela Roberta em 20/01/2020
                    //if (carga.Pedidos != null && carga.Pedidos.Any(p => p.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada))
                    serCTePorCTeParaSVM.GerarCTePorCTe(carga, configuracaoEmbarcador, configuracaoGeralCarga, cargaPedidos, tipoServicoMultisoftware, unitOfWork, totalDocumentos, quantidadeDocumentosAtualizarCarga, pedidosCTeParaSubContratacaoContaContabilContabilizacao, ref totalDocumentosGerados);
                    //else
                    //serCTePorCTeParaSVM.GerarCTePorContainer(carga, configuracaoEmbarcador, cargaPedidos, tipoServicoMultisoftware, unitOfWork, totalDocumentos, quantidadeDocumentosAtualizarCarga, ref totalDocumentosGerados);
                }
                else if (carga.Empresa.FormaRateioSVM == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaRateioSVM.UmCTeMultimodalParaUmCTeAquaviario)
                    serCTePorCTeParaSVM.GerarCTePorCTe(carga, configuracaoEmbarcador, configuracaoGeralCarga, cargaPedidos, tipoServicoMultisoftware, unitOfWork, totalDocumentos, quantidadeDocumentosAtualizarCarga, pedidosCTeParaSubContratacaoContaContabilContabilizacao, ref totalDocumentosGerados);
                else if (carga.Empresa.FormaRateioSVM == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaRateioSVM.AgruparPorTerminalOrigemDestino)
                    serCTePorCTeParaSVM.GerarCTePorTerminal(carga, configuracaoEmbarcador, configuracaoGeralCarga, cargaPedidos, tipoServicoMultisoftware, unitOfWork, totalDocumentos, quantidadeDocumentosAtualizarCarga, ref totalDocumentosGerados, ratearPorTerminais, ratearPorBUS, pedidosCTeParaSubContratacaoContaContabilContabilizacao);
                else if (carga.Empresa.FormaRateioSVM == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaRateioSVM.AgruparPorRemetenteDestinatario)
                    serCTePorCTeParaSVM.GerarCTePorRemetenteDestinatario(carga, configuracaoEmbarcador, configuracaoGeralCarga, cargaPedidos, tipoServicoMultisoftware, unitOfWork, totalDocumentos, quantidadeDocumentosAtualizarCarga, ref totalDocumentosGerados, ratearMesmoQueMultiModal, ratearPorTerminais, ratearPorBUS, pedidosCTeParaSubContratacaoContaContabilContabilizacao);
                else if (carga.Empresa.FormaRateioSVM == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaRateioSVM.AgruparPorBookingRemetenteDestinatarioExpedidorRecebedor)
                    serCTePorCTeParaSVM.GerarCTePorBooking(carga, configuracaoEmbarcador, configuracaoGeralCarga, cargaPedidos, tipoServicoMultisoftware, unitOfWork, totalDocumentos, quantidadeDocumentosAtualizarCarga, ref totalDocumentosGerados, ratearMesmoQueMultiModal, ratearPorTerminais, ratearPorBUS, pedidosCTeParaSubContratacaoContaContabilContabilizacao);
                else if (carga.Empresa.FormaRateioSVM == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaRateioSVM.AgruparPorSacado)
                    serCTePorCTeParaSVM.GerarCTePorSacado(carga, configuracaoEmbarcador, configuracaoGeralCarga, cargaPedidos, tipoServicoMultisoftware, unitOfWork, totalDocumentos, quantidadeDocumentosAtualizarCarga, ref totalDocumentosGerados, ratearPorTerminais, ratearPorBUS, pedidosCTeParaSubContratacaoContaContabilContabilizacao);
                else if (carga.Empresa.FormaRateioSVM == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaRateioSVM.AgruparPorTerminalOrigemDestinoSacado)
                    serCTePorCTeParaSVM.GerarCTePorTerminalSacado(carga, configuracaoEmbarcador, configuracaoGeralCarga, cargaPedidos, tipoServicoMultisoftware, unitOfWork, totalDocumentos, quantidadeDocumentosAtualizarCarga, ref totalDocumentosGerados, ratearPorTerminais, ratearPorBUS, pedidosCTeParaSubContratacaoContaContabilContabilizacao);
                else if (carga.Empresa.FormaRateioSVM == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaRateioSVM.AgruparPorRemetenteDestinatarioSacado)
                    serCTePorCTeParaSVM.GerarCTePorRemetenteDestinatarioSacado(carga, configuracaoEmbarcador, configuracaoGeralCarga, cargaPedidos, tipoServicoMultisoftware, unitOfWork, totalDocumentos, quantidadeDocumentosAtualizarCarga, ref totalDocumentosGerados, ratearMesmoQueMultiModal, ratearPorTerminais, ratearPorBUS, pedidosCTeParaSubContratacaoContaContabilContabilizacao);
                else if (carga.Empresa.FormaRateioSVM == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaRateioSVM.AgruparPorBookingRemetenteDestinatarioExpedidorRecebedorSacado)
                {
                    serCTePorCTeParaSVM.GerarCTePorSacado(carga, configuracaoEmbarcador, configuracaoGeralCarga, cargaPedidos, tipoServicoMultisoftware, unitOfWork, totalDocumentos, quantidadeDocumentosAtualizarCarga, ref totalDocumentosGerados, ratearPorTerminais, ratearPorBUS, pedidosCTeParaSubContratacaoContaContabilContabilizacao);
                    //Mudado por solicitaçao da roberta em 17/12/2019
                    //serCTePorCTeParaSVM.GerarCTePorBookingRemetenteDestinatarioExpedidorRecebedorSacado(carga, configuracaoEmbarcador, cargaPedidos, tipoServicoMultisoftware, unitOfWork, totalDocumentos, quantidadeDocumentosAtualizarCarga, ref totalDocumentosGerados, ratearMesmoQueMultiModal, ratearPorTerminais, ratearPorBUS);
                }
                else
                    serCTePorCTeParaSVM.GerarCTePorCTe(carga, configuracaoEmbarcador, configuracaoGeralCarga, cargaPedidos, tipoServicoMultisoftware, unitOfWork, totalDocumentos, quantidadeDocumentosAtualizarCarga, pedidosCTeParaSubContratacaoContaContabilContabilizacao, ref totalDocumentosGerados);

                //Ajustar valor da carga por aqui        
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidoAtualizar = repCargaPedido.BuscarPorCarga(carga.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarPorCarga(carga.Codigo);
                serRateioCTe.AjustarFretePorCTes(cargaPedidoAtualizar[0], cargaCTes, tipoServicoMultisoftware, unitOfWork);
            }
            else
            {
                if ((configuracaoEmbarcador.AtivarEmissaoSubcontratacaoAgrupado || configuracaoEmbarcador.UtilizaEmissaoMultimodal) && (tipoEmissaoDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupada || tipoEmissaoDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupadaEntrePedidos))
                    serCTePorCTeParaSubcontratacao.GerarCTePorSubcontratacaoAgrupado(carga, cargasOrigem, configuracaoEmbarcador, configuracaoGeralCarga, cargaPedidos, tipoServicoMultisoftware, unitOfWork, totalDocumentos, quantidadeDocumentosAtualizarCarga, pedidosCTeParaSubContratacaoContaContabilContabilizacao, indicadorCteSimplificado, ref totalDocumentosGerados);
                else if ((configuracaoEmbarcador.AtivarEmissaoSubcontratacaoAgrupado || configuracaoEmbarcador.UtilizaEmissaoMultimodal) && tipoEmissaoDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoAgrupado)
                    serCTePorCTeParaSubcontratacao.GerarCTePorSubcontratacaoAgrupadoPorDestinatarioDoPedido(carga, cargasOrigem, configuracaoEmbarcador, configuracaoGeralCarga, cargaPedidos, tipoServicoMultisoftware, unitOfWork, totalDocumentos, quantidadeDocumentosAtualizarCarga, pedidosCTeParaSubContratacaoContaContabilContabilizacao, ref totalDocumentosGerados, configuracaoEmbarcador.UtilizaEmissaoMultimodal);
                else if ((configuracaoEmbarcador.AtivarEmissaoSubcontratacaoAgrupado || configuracaoEmbarcador.UtilizaEmissaoMultimodal) && tipoEmissaoDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoIndividual && tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    serCTePorCTeParaSubcontratacao.GerarCTePorSubcontratacaoAgrupadoPorPedido(carga, cargasOrigem, configuracaoEmbarcador, configuracaoGeralCarga, configuracaoCargaEmissaoDocumento, cargaPedidos, tipoServicoMultisoftware, unitOfWork, totalDocumentos, quantidadeDocumentosAtualizarCarga, pedidosCTeParaSubContratacaoContaContabilContabilizacao, ref totalDocumentosGerados);
                else if ((configuracaoEmbarcador.AtivarEmissaoSubcontratacaoAgrupado || configuracaoEmbarcador.UtilizaEmissaoMultimodal) && tipoEmissaoDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoIndividual && tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    serCTePorCTeParaSubcontratacao.GerarCTePorSubcontratacaoAgrupadoPorPedidoEmbarcador(carga, cargasOrigem, configuracaoEmbarcador, configuracaoGeralCarga, configuracaoCargaEmissaoDocumento, cargaPedidos, tipoServicoMultisoftware, unitOfWork, totalDocumentos, quantidadeDocumentosAtualizarCarga, pedidosCTeParaSubContratacaoContaContabilContabilizacao, ref totalDocumentosGerados);
                else
                {
                    //if ((tipoEmissaoDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoAgrupado && cargaPedidos.Any(obj => obj.PedidoEncaixado)) || cargaPedidos.Any(obj => obj.IndicadorCTeGlobalizadoDestinatario))
                    if ((tipoEmissaoDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoAgrupado || cargaPedidos.Any(obj => obj.IndicadorCTeGlobalizadoDestinatario)))
                        serCTePorCTeParaSubcontratacao.GerarCTePorSubcontratacaoAgrupadoPorDestinatarioDoPedido(carga, cargasOrigem, configuracaoEmbarcador, configuracaoGeralCarga, cargaPedidos, tipoServicoMultisoftware, unitOfWork, totalDocumentos, quantidadeDocumentosAtualizarCarga, pedidosCTeParaSubContratacaoContaContabilContabilizacao, ref totalDocumentosGerados, configuracaoEmbarcador.UtilizaEmissaoMultimodal);
                    else
                        serCTePorCTeParaSubcontratacao.GerarCTePorSubcontratacaoIndividual(carga, cargasOrigem, configuracaoEmbarcador, configuracaoGeralCarga, cargaPedidos, tipoServicoMultisoftware, unitOfWork, totalDocumentos, quantidadeDocumentosAtualizarCarga, pedidosCTeParaSubContratacaoContaContabilContabilizacao, ref totalDocumentosGerados);

                }

            }


            return mensagem;
        }
    }
}