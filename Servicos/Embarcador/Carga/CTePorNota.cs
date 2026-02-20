using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga
{
    public class CTePorNota : ServicoBase
    {
        public CTePorNota(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #region Métodos Públicos

        public void GerarCTePorNotaAgrupada(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Cliente expedidor, Dominio.Entidades.Cliente recebedor, Dominio.Enumeradores.TipoPagamento tipoPagamento, Dominio.Enumeradores.TipoTomador tipoTomador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, int totalDocumentos, int quantidadeDocumentosAtualizarCarga, ref int totalDocumentosGerados, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal, Dominio.Entidades.Embarcador.Documentos.CIOT ciot, int tipoEnvio, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoRecebedor, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalContaContabilContabilizacao> pedidosXMLNotaFiscalContaContabilContabilizacao, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoExpedidor)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOTCTe repCIOTCTe = new Repositorio.Embarcador.Documentos.CIOTCTe(unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
            Repositorio.Embarcador.Rateio.RateioFormula repRateioFormula = new Repositorio.Embarcador.Rateio.RateioFormula(unitOfWork);

            Servicos.Embarcador.Carga.NFS serNFS = new NFS(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
            CTe serCte = new CTe(unitOfWork);
            ICMS serICMS = new ICMS(unitOfWork);
            Hubs.Carga svcHubCarga = new Hubs.Carga();
            Servicos.Embarcador.Pedido.PedidoXMLNotaFiscal servicoPedidoXMLNotaFiscal = new Pedido.PedidoXMLNotaFiscal(unitOfWork);

            List<int> codigosCargaPedidos = (from obj in cargaPedidos select obj.Codigo).ToList();

            //string produtoPredominanteEntreOsPedidos = string.Empty;

            //if ((from obj in cargaPedidos where string.IsNullOrWhiteSpace(obj.Pedido.ProdutoPredominante) select obj).Any())
            //    produtoPredominanteEntreOsPedidos = serCte.BuscarProdutoPredominante(cargaPedidos, configuracaoEmbarcador, unitOfWork);

            foreach (int codigoCargaPedido in codigosCargaPedidos)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigoComFetch(codigoCargaPedido);
                Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaRateio = cargaPedido.FormulaRateio != null ? repRateioFormula.BuscarPorCodigo(cargaPedido.FormulaRateio.Codigo) : null;
                Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem = (from obj in cargasOrigem where obj.Codigo == cargaPedido.CargaOrigem.Codigo select obj).FirstOrDefault();

                if (!cargaPedido.PossuiCTe && !cargaPedido.PossuiNFS && !cargaPedido.PossuiNFSManual)
                    continue;

                bool emitirCteFilialEmissora = false;
                if (cargaPedido.Carga.EmpresaFilialEmissora != null && !cargaPedido.Carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                    emitirCteFilialEmissora = true;

                List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> apoliceSeguroAverbacaos = repApoliceSeguroAverbacao.BuscarPorCargaPedido(cargaPedido.Codigo, emitirCteFilialEmissora);

                serCte.ObterDescricoesComponentesPadrao(cargaPedido.ObterTomador(), carga, configuracaoEmbarcador, unitOfWork, out string descricaoComponenteValorFrete, out string descricaoComponenteValorICMS);

                bool compararTomador = false;
                List<double> tomadores = new List<double>();

                if (tipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                {
                    tomadores = repPedidoXMLNotaFiscal.BuscarTomadoresPorCargaPedido(codigoCargaPedido);
                    compararTomador = true;
                }

                if (!tomadores.Contains(0D))
                    tomadores.Add(0D); //adiciona para pegar os registros sem tomador

                List<double> remetentes = repPedidoXMLNotaFiscal.BuscarRemetentesPorCargaPedido(codigoCargaPedido);
                List<double> destinatarios = repPedidoXMLNotaFiscal.BuscarDestinatariosPorCargaPedido(codigoCargaPedido);

                if (!cargaPedido.CTeEmitidoNoEmbarcador && !cargaPedido.Pedido.PedidoTransbordo)
                {
                    List<string> rotasPedido = (from obj in cargaPedido.CargaPedidoRotas select obj.IdenticacaoRota).ToList();
                    int serie = cargaPedido.Pedido.EmpresaSerie?.Numero ?? 0;
                    string observacoesGerais = cargaPedido.Pedido.ObservacaoCTe;
                    string produtoPredominante = cargaPedido.Pedido.ProdutoPredominante;

                    if (string.IsNullOrWhiteSpace(produtoPredominante))
                        produtoPredominante = serCte.BuscarProdutoPredominante(cargaPedido, configuracaoEmbarcador, unitOfWork);

                    List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete> modalidadesNotas = null;
                    bool comparaModalidadeNF = false;
                    if (cargaPedido.Pedido.UsarTipoPagamentoNF)
                    {
                        modalidadesNotas = repPedidoXMLNotaFiscal.BuscarModalidadesPagamentoPorCargaPedido(codigoCargaPedido);
                        comparaModalidadeNF = true;
                    }
                    else
                    {
                        modalidadesNotas = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete>();
                        modalidadesNotas.Add((Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete)tipoPagamento);
                    }

                    bool contemNotaFiscalSemInscricao = false;
                    if (configuracaoEmbarcador.AgruparCTesDiferentesPedidosMesmoDestinatario)
                        contemNotaFiscalSemInscricao = repPedidoXMLNotaFiscal.ContemNotaFiscalSemInscricao(codigosCargaPedidos);

                    foreach (double tomadorNotaFiscal in tomadores)
                    {
                        foreach (double remetente in remetentes)
                        {
                            List<string> iesRemetente = new List<string>();
                            if (!contemNotaFiscalSemInscricao && configuracaoEmbarcador.AgruparCTesDiferentesPedidosMesmoDestinatario)
                                iesRemetente = repPedidoXMLNotaFiscal.BuscarIERemetentePorCargaPedido(codigosCargaPedidos, remetente);
                            else
                                iesRemetente.Add("");

                            foreach (var ieRemetente in iesRemetente)
                            {
                                foreach (double destinatario in destinatarios)
                                {
                                    List<string> iesDestinatario = new List<string>();
                                    if (!contemNotaFiscalSemInscricao && configuracaoEmbarcador.AgruparCTesDiferentesPedidosMesmoDestinatario)
                                        iesDestinatario = repPedidoXMLNotaFiscal.BuscarIEDestinatarioPorCargaPedido(codigosCargaPedidos, destinatario);
                                    else
                                        iesDestinatario.Add("");

                                    foreach (var ieDestinatario in iesDestinatario)
                                    {
                                        foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete? modalidade in modalidadesNotas)
                                        {
                                            unitOfWork.FlushAndClear();

                                            cargaPedido = repCargaPedido.BuscarPorCodigo(codigoCargaPedido);
                                            carga = repCarga.BuscarPorCodigo(carga.Codigo);
                                            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasRemetentes = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(codigoCargaPedido, remetente, destinatario, compararTomador ? (double?)tomadorNotaFiscal : null, comparaModalidadeNF ? modalidade : null, ieRemetente, ieDestinatario);

                                            if (pedidoXMLNotasRemetentes.Count <= 0)
                                                continue;

                                            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscalFirst = pedidoXMLNotasRemetentes.FirstOrDefault();

                                            if (!pedidoXMLNotaFiscalFirst.PossuiCTe && !pedidoXMLNotaFiscalFirst.PossuiNFS && !pedidoXMLNotaFiscalFirst.PossuiNFSManual)
                                                continue;

                                            totalDocumentosGerados++;

                                            Dominio.Entidades.Cliente tomadorUtilizar = null;
                                            Dominio.Entidades.Cliente remetentePedido = null;
                                            Dominio.Entidades.Cliente destinatarioPedido = null;

                                            if (tomadorNotaFiscal > 0D)
                                                tomadorUtilizar = repCliente.BuscarPorCPFCNPJ(tomadorNotaFiscal);
                                            else if (tomador != null)
                                                tomadorUtilizar = repCliente.BuscarPorCPFCNPJ(tomador.CPF_CNPJ);

                                            if (expedidor != null)
                                                expedidor = repCliente.BuscarPorCPFCNPJ(expedidor.CPF_CNPJ);

                                            if (recebedor != null)
                                                recebedor = repCliente.BuscarPorCPFCNPJ(recebedor.CPF_CNPJ);

                                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal tipoOperacaoNota = pedidoXMLNotaFiscalFirst.XMLNotaFiscal.TipoOperacaoNotaFiscal;

                                            bool possuiNFSManual = false;

                                            Dominio.Entidades.Localidade localidadePrestacao = null;

                                            //todo:foi modificado para armazenar o tipo de domento na nota, após um tempo remover o código abaixo e usar apenas oque está no pedidoxmlnotafiscal (28/11/2018)
                                            if (cargaPedido.PossuiNFSManual)
                                            {
                                                if (tipoOperacaoNota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada) // esse caso é utilizado para notas de importação
                                                {
                                                    remetentePedido = repCliente.BuscarPorCPFCNPJ(destinatario);
                                                    destinatarioPedido = repCliente.BuscarPorCPFCNPJ(remetente);
                                                }
                                                else
                                                {
                                                    remetentePedido = repCliente.BuscarPorCPFCNPJ(remetente);
                                                    destinatarioPedido = repCliente.BuscarPorCPFCNPJ(destinatario);
                                                }

                                                if (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.Normal)
                                                {
                                                    if (remetentePedido != null && destinatarioPedido != null && (destinatarioPedido.Localidade.Codigo == remetentePedido.Localidade.Codigo || cargaPedido.DisponibilizarDocumentoNFSManual))
                                                    {
                                                        localidadePrestacao = destinatarioPedido.Localidade;
                                                        possuiNFSManual = true;
                                                    }
                                                }
                                                else if (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor)
                                                {
                                                    if (expedidor != null && destinatarioPedido != null && (destinatarioPedido.Localidade.Codigo == expedidor.Localidade.Codigo || cargaPedido.DisponibilizarDocumentoNFSManual))
                                                    {
                                                        localidadePrestacao = destinatarioPedido.Localidade;
                                                        possuiNFSManual = true;
                                                    }
                                                }
                                                else if (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor)
                                                {
                                                    if (recebedor != null && remetentePedido != null && (remetentePedido.Localidade.Codigo == recebedor.Localidade.Codigo || cargaPedido.DisponibilizarDocumentoNFSManual))
                                                    {
                                                        localidadePrestacao = remetentePedido.Localidade;
                                                        possuiNFSManual = true;
                                                    }
                                                }
                                                else if (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor)
                                                {
                                                    if (recebedor != null && expedidor != null && (expedidor.Localidade.Codigo == recebedor.Localidade.Codigo || cargaPedido.DisponibilizarDocumentoNFSManual))
                                                    {
                                                        localidadePrestacao = expedidor.Localidade;
                                                        possuiNFSManual = true;
                                                    }
                                                }

                                                if ((carga.Empresa?.SempreEmitirNFS ?? false) && remetentePedido != null)
                                                {
                                                    localidadePrestacao = remetentePedido.Localidade;
                                                    possuiNFSManual = true;
                                                }
                                            }

                                            if (!possuiNFSManual || (possuiNFSManual && cargaPedido.ModeloDocumentoFiscalIntramunicipal != null))
                                            {
                                                if ((from obj in pedidoXMLNotasRemetentes where obj.CTes.Count > 0 select obj).Any())
                                                    continue;

                                                Dominio.Entidades.ModeloDocumentoFiscal modeloNotaFiscalAgrupado = null;

                                                if (possuiNFSManual)
                                                    modeloNotaFiscalAgrupado = cargaPedido.ModeloDocumentoFiscalIntramunicipal;
                                                else
                                                    modeloNotaFiscalAgrupado = pedidoXMLNotaFiscalFirst.ModeloDocumentoFiscal;

                                                if (modeloNotaFiscalAgrupado == null)
                                                {
                                                    modeloNotaFiscalAgrupado = modeloDocumentoFiscal;
                                                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotasRemetentes)
                                                    {
                                                        pedidoXMLNotaFiscal.ModeloDocumentoFiscal = modeloDocumentoFiscal;
                                                        repPedidoXMLNotaFiscal.Atualizar(pedidoXMLNotaFiscal);
                                                    }
                                                }

                                                List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> cargaPedidoComponentesFretesCliente = BuscarXMLPedidoComponentesFreteAgrupados(pedidoXMLNotasRemetentes, emitirCteFilialEmissora, unitOfWork);

                                                unitOfWork.Start();

                                                Dominio.ObjetosDeValor.CTe.CTe cte = new Dominio.ObjetosDeValor.CTe.CTe();
                                                cte.Documentos = new List<Dominio.ObjetosDeValor.CTe.Documento>();

                                                List<string> rotas = new List<string>();
                                                decimal peso = 0m, pesoLiquido = 0m, metrosCubicos = 0m, pesoCubado = 0m, fatorCubagem = 0m, percentualPagamentoAgregado = 0m, pallets = 0m, valorTotalMoeda = 0m;
                                                int volumes = 0;

                                                Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = BuscarRegraICMS(pedidoXMLNotaFiscalFirst, emitirCteFilialEmissora, modeloNotaFiscalAgrupado);
                                                Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = BuscarRegraISS(pedidoXMLNotaFiscalFirst);
                                                Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = servicoPedidoXMLNotaFiscal.ObterRetornoImpostoIBSCBS(pedidoXMLNotaFiscalFirst, emitirCteFilialEmissora);

                                                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotasRemetentes)
                                                {

                                                    if ((carga?.TipoOperacao?.ConfiguracaoDocumentoEmissao?.PossuiNotaOrdemVenda ?? false) &&
                                                        (carga?.TipoOperacao?.ConfiguracaoDocumentoEmissao?.NaoUtilizaNotaVendaObjetoCTE ?? false)
                                                        && pedidoXMLNotaFiscal.XMLNotaFiscal.TipoNotaFiscalIntegrada == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscalIntegrada.OrdemVenda)
                                                    {
                                                        cargaPedido.Pedido.ObservacaoCTe += "NFe Venda: " + pedidoXMLNotaFiscal.XMLNotaFiscal.Chave;
                                                        repPedido.Atualizar(cargaPedido.Pedido);

                                                        Servicos.Log.TratarErro("9 - Atualizando Obs pedido" + cargaPedido.Pedido.Codigo + " chave: " + pedidoXMLNotaFiscal.XMLNotaFiscal.Chave, "SolicitarEmissaoDocumentosAutorizados");
                                                        continue;
                                                    }


                                                    if (!string.IsNullOrWhiteSpace(pedidoXMLNotaFiscal.XMLNotaFiscal.Rota))
                                                    {
                                                        if (!rotas.Contains(pedidoXMLNotaFiscal.XMLNotaFiscal.Rota))
                                                            rotas.Add(pedidoXMLNotaFiscal.XMLNotaFiscal.Rota);
                                                    }

                                                    Dominio.ObjetosDeValor.CTe.Documento docNF = serCte.BuscarDocumentoCTe(pedidoXMLNotaFiscal.XMLNotaFiscal, carga.Empresa.TipoAmbiente, configuracaoEmbarcador);
                                                    cte.Documentos.Add(docNF);

                                                    percentualPagamentoAgregado = pedidoXMLNotaFiscal.PercentualPagamentoAgregado;

                                                    pesoLiquido += pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido;
                                                    peso += docNF.Peso;
                                                    volumes += docNF.Volume;

                                                    pallets += pedidoXMLNotaFiscal.XMLNotaFiscal.QuantidadePallets;
                                                    metrosCubicos += pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos;
                                                    pesoCubado += pedidoXMLNotaFiscal.XMLNotaFiscal.PesoCubado;
                                                    valorTotalMoeda += pedidoXMLNotaFiscal.ValorTotalMoeda ?? 0m;

                                                    if (pedidoXMLNotaFiscal.XMLNotaFiscal.FatorCubagem > 0m)
                                                        fatorCubagem = pedidoXMLNotaFiscal.XMLNotaFiscal.FatorCubagem;

                                                    cte.ValorTotalMercadoria += docNF.Valor;
                                                    cte.ValorImpostoSuspenso += pedidoXMLNotaFiscal?.XMLNotaFiscal?.ValorImpostoSuspenso ?? 0;

                                                    if (!emitirCteFilialEmissora)
                                                    {
                                                        if (carga.PossuiComponenteFreteComImpostoIncluso)
                                                        {
                                                            regraICMS.IncluirICMSBC = false;
                                                            regraICMS.PercentualInclusaoBC = 0m;
                                                            regraICMS.ValorICMS += pedidoXMLNotaFiscal.ValorFreteComICMSIncluso - pedidoXMLNotaFiscal.ValorFrete + cargaPedidoComponentesFretesCliente.Sum(o => o.ValorComponenteComICMSIncluso) - cargaPedidoComponentesFretesCliente.Sum(o => o.ValorComponente);
                                                            regraICMS.ValorICMSIncluso += regraICMS.ValorICMS;
                                                            regraICMS.ValorBaseCalculoICMS += pedidoXMLNotaFiscal.ValorFreteComICMSIncluso + cargaPedidoComponentesFretesCliente.Sum(o => o.ValorComponenteComICMSIncluso);
                                                            regraICMS.ValorBaseCalculoPISCOFINS += pedidoXMLNotaFiscal.ValorFreteComICMSIncluso - pedidoXMLNotaFiscal.BaseCalculoICMS;
                                                        }
                                                        else
                                                        {
                                                            regraICMS.ValorBaseCalculoICMS += pedidoXMLNotaFiscal.BaseCalculoICMS;
                                                            regraICMS.ValorBaseCalculoPISCOFINS += pedidoXMLNotaFiscal.BaseCalculoICMS;
                                                            regraICMS.ValorICMSIncluso += pedidoXMLNotaFiscal.ValorICMSIncluso;
                                                            regraICMS.ValorICMS += pedidoXMLNotaFiscal.ValorICMS;
                                                        }

                                                        regraICMS.ValorPis += pedidoXMLNotaFiscal.ValorPis;
                                                        regraICMS.ValorCofins += pedidoXMLNotaFiscal.ValorCofins;
                                                        regraICMS.ValorCreditoPresumido += pedidoXMLNotaFiscal.ValorCreditoPresumido;

                                                        servicoPedidoXMLNotaFiscal.PreencherValoresRetornoIBSCBS(impostoIBSCBS, pedidoXMLNotaFiscal.BaseCalculoIBSCBS, pedidoXMLNotaFiscal.ValorIBSEstadual, pedidoXMLNotaFiscal.ValorIBSMunicipal, pedidoXMLNotaFiscal.ValorCBS);
                                                    }
                                                    else
                                                    {
                                                        regraICMS.ValorBaseCalculoICMS += pedidoXMLNotaFiscal.BaseCalculoICMSFilialEmissora;
                                                        regraICMS.ValorBaseCalculoPISCOFINS += pedidoXMLNotaFiscal.BaseCalculoICMS;
                                                        regraICMS.ValorICMS += pedidoXMLNotaFiscal.ValorICMSFilialEmissora;
                                                        regraICMS.ValorICMSIncluso += pedidoXMLNotaFiscal.ValorICMSFilialEmissora;
                                                        regraICMS.ValorCreditoPresumido += pedidoXMLNotaFiscal.ValorCreditoPresumidoFilialEmissora;

                                                        servicoPedidoXMLNotaFiscal.PreencherValoresRetornoIBSCBS(impostoIBSCBS, pedidoXMLNotaFiscal.BaseCalculoIBSCBSFilialEmissora, pedidoXMLNotaFiscal.ValorIBSEstadualFilialEmissora, pedidoXMLNotaFiscal.ValorIBSMunicipalFilialEmissora, pedidoXMLNotaFiscal.ValorCBSFilialEmissora);
                                                    }

                                                    regraISS.ValorBaseCalculoISS += pedidoXMLNotaFiscal.BaseCalculoISS;
                                                    regraISS.ValorISS += pedidoXMLNotaFiscal.ValorISS;
                                                    regraISS.ValorRetencaoISS += pedidoXMLNotaFiscal.ValorRetencaoISS;

                                                    //regraISS.BaseCalculoIR += pedidoXMLNotaFiscal.BaseCalculoIR;
                                                    //regraISS.AliquotaIR += pedidoXMLNotaFiscal.AliquotaIR;
                                                    //regraISS.ValorIR += pedidoXMLNotaFiscal.ValorIR;
                                                }

                                                cte.Peso = peso;
                                                cte.PesoLiquido = pesoLiquido;
                                                cte.Volumes = volumes;
                                                cte.MetrosCubicos = metrosCubicos;
                                                cte.Pallets = pallets;
                                                cte.PesoCubado = pesoCubado;
                                                cte.FatorCubagem = fatorCubagem;
                                                cte.PesoFaturado = (peso > pesoCubado ? peso : pesoCubado);
                                                cte.PercentualPagamentoAgregado = percentualPagamentoAgregado;
                                                cte.Moeda = pedidoXMLNotaFiscalFirst.Moeda;
                                                cte.ValorTotalMoeda = valorTotalMoeda + cargaPedidoComponentesFretesCliente.Sum(o => o.ValorTotalMoeda ?? 0m);
                                                cte.Moeda = pedidoXMLNotaFiscalFirst.Moeda;
                                                cte.ValorCotacaoMoeda = pedidoXMLNotaFiscalFirst.ValorCotacaoMoeda;

                                                List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidades = serCte.RetornaQuantidades(peso, volumes, fatorCubagem, metrosCubicos, pesoCubado, carga?.TipoOperacao?.ConfiguracaoEmissaoDocumento?.RatearPesoModeloVeicularEntreCTes ?? false, carga?.TipoOperacao?.ConfiguracaoEmissaoDocumento?.DescricaoUnidadeMedidaPesoModeloVeicularRateado, serCte.ObterPesoModeloVeicularRateado(cte.Peso, carga, unitOfWork));

                                                if (rotas.Count == 0 && rotasPedido.Count > 0)
                                                    rotas = rotasPedido;

                                                if (carga.PossuiComponenteFreteComImpostoIncluso)
                                                {
                                                    cte.ValorAReceber += pedidoXMLNotasRemetentes.Sum(obj => obj.ValorFreteComICMSIncluso);
                                                    cte.ValorFrete += cte.ValorAReceber;
                                                    cte.ValorTotalPrestacaoServico += cte.ValorAReceber;
                                                }
                                                else
                                                {
                                                    if (!emitirCteFilialEmissora)
                                                    {
                                                        if (formulaRateio?.RatearEmBlocoDeEmissao ?? false)
                                                            cte.ValorAReceber = Math.Round(pedidoXMLNotasRemetentes.Sum(obj => obj.ValorFrete), 2, MidpointRounding.ToEven);
                                                        else
                                                            cte.ValorAReceber = pedidoXMLNotasRemetentes.Sum(obj => obj.ValorFrete);
                                                    }
                                                    else
                                                        cte.ValorAReceber = pedidoXMLNotasRemetentes.Sum(obj => obj.ValorFreteFilialEmissora);

                                                    cte.ValorFrete = cte.ValorAReceber;
                                                    cte.ValorTotalPrestacaoServico = cte.ValorAReceber;
                                                }

                                                cte.Serie = serie;
                                                //cte.ObservacoesGerais = observacoesGerais;
                                                cte.ProdutoPredominante = produtoPredominante;

                                                if (remetentePedido == null || destinatarioPedido == null)
                                                {
                                                    if (tipoOperacaoNota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada) // esse caso é utilizado para notas de importação
                                                    {
                                                        remetentePedido = repCliente.BuscarPorCPFCNPJ(destinatario);
                                                        destinatarioPedido = repCliente.BuscarPorCPFCNPJ(remetente);
                                                    }
                                                    else
                                                    {
                                                        remetentePedido = repCliente.BuscarPorCPFCNPJ(remetente);
                                                        destinatarioPedido = repCliente.BuscarPorCPFCNPJ(destinatario);
                                                    }
                                                }

                                                if (ciot != null)
                                                    cte.CIOT = ciot.Numero;

                                                Dominio.Enumeradores.TipoServico tipoServico = Dominio.Enumeradores.TipoServico.Normal;

                                                if (cargaPedido.Redespacho)
                                                {
                                                    string raizEmpresa = Utilidades.String.OnlyNumbers(cargaPedido.CargaPedidoTrechoAnterior.CargaOrigem.Empresa.CNPJ_SemFormato).Remove(8, 6);
                                                    string raizTomador = "";

                                                    if (cargaPedido.ObterTomador().Tipo == "J")
                                                        raizTomador = Utilidades.String.OnlyNumbers(cargaPedido.ObterTomador().CPF_CNPJ_SemFormato).Remove(8, 6);

                                                    if (raizEmpresa == raizTomador)
                                                    {
                                                        if (recebedor != null && expedidor != null)
                                                            tipoServico = Dominio.Enumeradores.TipoServico.RedIntermediario;
                                                        else
                                                            tipoServico = Dominio.Enumeradores.TipoServico.Redespacho;
                                                    }
                                                }

                                                if (expedidor != null)
                                                    expedidor = repCliente.BuscarPorCPFCNPJ(expedidor.CPF_CNPJ);

                                                if (recebedor != null)
                                                    recebedor = repCliente.BuscarPorCPFCNPJ(recebedor.CPF_CNPJ);

                                                Dominio.Enumeradores.TipoPagamento tipoPagamentoNFe = (Dominio.Enumeradores.TipoPagamento)modalidade;

                                                if (comparaModalidadeNF)
                                                {
                                                    if (tipoPagamentoNFe == Dominio.Enumeradores.TipoPagamento.Pago)
                                                        tipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
                                                    else if (tipoPagamentoNFe == Dominio.Enumeradores.TipoPagamento.A_Pagar)
                                                        tipoTomador = Dominio.Enumeradores.TipoTomador.Destinatario;
                                                    else if (tipoPagamentoNFe == Dominio.Enumeradores.TipoPagamento.Outros)
                                                        tipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
                                                }

                                                List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> seguros = serCte.ConverterSegurosDeTerceirosEmSeguroCTe(apoliceSeguroAverbacaos, tipoTomador, cte.ValorTotalMercadoria);
                                                List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ctesAnteriores = new List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe>();

                                                Dominio.Enumeradores.TipoCTE tipoCTe = Dominio.Enumeradores.TipoCTE.Normal;

                                                Dominio.Entidades.Empresa empresa = cargaOrigem.Empresa;

                                                if (emitirCteFilialEmissora)
                                                {
                                                    if (cargaPedido.CargaPedidoProximoTrecho != null)
                                                        recebedor = null;

                                                    if (cargaPedido.CargaPedidoTrechoAnterior != null)
                                                        expedidor = null;

                                                    empresa = cargaOrigem.EmpresaFilialEmissora;
                                                }

                                                if (cargaPedido.Pedido.TipoOperacao != null && cargaPedido.Pedido.TipoOperacao.ColetaEmProdutorRural && cargaPedido.Pedido.TipoOperacao.RemetenteDoCTeSeraODestinatarioDoPedido)
                                                    remetentePedido = repCliente.BuscarPorCPFCNPJ(cargaPedido.Pedido.Destinatario.CPF_CNPJ);
                                                List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil> configuracoes = Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil.ConverterPedidoXMLNotaFiscalContaContabilContabilizacao(pedidoXMLNotaFiscalFirst, pedidosXMLNotaFiscalContaContabilContabilizacao);
                                                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTE = serCte.GerarCTe(carga, cargaPedido, cte, empresa, remetentePedido, destinatarioPedido, tomadorUtilizar, expedidor, recebedor, cargaPedido.Pedido.UsarOutroEnderecoOrigem && cargaPedido.Pedido.EnderecoOrigem?.Localidade != null ? cargaPedido.Pedido.EnderecoOrigem.Localidade : remetentePedido.Localidade, cargaPedido.Pedido.UsarOutroEnderecoDestino && cargaPedido.Pedido.EnderecoDestino?.Localidade != null ? cargaPedido.Pedido.EnderecoDestino.Localidade : destinatarioPedido.Localidade, cargaPedido.Pedido.UsarOutroEnderecoOrigem ? cargaPedido.Pedido.EnderecoOrigem : null, cargaPedido.Pedido.UsarOutroEnderecoDestino ? cargaPedido.Pedido.EnderecoDestino : null, tipoPagamentoNFe, tipoTomador, quantidades, cargaPedidoComponentesFretesCliente, cargaPedido.Pedido.ObservacaoCTe, cargaPedido.Pedido.ObservacaoCTeTerceiro, cargaPedido.Pedido, rotas, seguros, regraICMS, regraISS, impostoIBSCBS, tipoServicoMultisoftware, modeloNotaFiscalAgrupado, tipoServico, tipoCTe, ctesAnteriores, tipoEnvio, false, null, false, unitOfWork, configuracaoEmbarcador, configuracaoGeralCarga, pedidoXMLNotaFiscalFirst.CentroResultado, pedidoXMLNotaFiscalFirst.CentroResultadoDestinatario, pedidoXMLNotaFiscalFirst.ItemServico, pedidoXMLNotaFiscalFirst.CentroResultadoEscrituracao, pedidoXMLNotaFiscalFirst.CentroResultadoICMS, pedidoXMLNotaFiscalFirst.CentroResultadoPIS, pedidoXMLNotaFiscalFirst.CentroResultadoCOFINS, pedidoXMLNotaFiscalFirst.ValorMaximoCentroContabilizacao, configuracoes, pedidoXMLNotasRemetentes.Select(o => o.XMLNotaFiscal).ToList(), enderecoRecebedor, descricaoComponenteValorICMS, descricaoComponenteValorFrete, enderecoExpedidor);
                                                serCte.AverbaCargaCTe(cargaCTE, apoliceSeguroAverbacaos, unitOfWork, configuracaoEmbarcador, cargaPedido.Pedido.FormaAverbacaoCTE);

                                                if (ciot != null)
                                                {
                                                    Dominio.Entidades.Embarcador.Documentos.CIOTCTe ciotCTe = new Dominio.Entidades.Embarcador.Documentos.CIOTCTe()
                                                    {
                                                        CargaCTe = cargaCTE,
                                                        CIOT = ciot
                                                    };

                                                    repCIOTCTe.Inserir(ciotCTe);
                                                }

                                                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotasRemetentes)
                                                {
                                                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTE = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe();

                                                    cargaPedidoXMLNotaFiscalCTE.CargaCTe = cargaCTE;
                                                    cargaPedidoXMLNotaFiscalCTE.PedidoXMLNotaFiscal = pedidoXMLNotaFiscal;

                                                    repCargaPedidoXMLNotaFiscalCTe.Inserir(cargaPedidoXMLNotaFiscalCTE);
                                                }

                                                if (possuiNFSManual)
                                                {
                                                    serNFS.CriarCargaCTePendenteEmissaoManualDeNFS(carga, cargaCTE, localidadePrestacao, unitOfWork);

                                                    carga.AgNFSManual = true;
                                                }

                                                unitOfWork.CommitChanges();

                                                if (cargaCTE.CTe != null && cargaCTE.CTe.Status == "E")
                                                {
                                                    if (cargaCTE.CTe.ModeloDocumentoFiscal.Numero == "57")
                                                    {
                                                        Servicos.CTe servicoCte = new Servicos.CTe(unitOfWork);
                                                        servicoCte.Emitir(cargaCTE.CTe.Codigo, 0, unitOfWork);
                                                    }
                                                    else if (cargaCTE.CTe.ModeloDocumentoFiscal.Numero == "39")
                                                    {
                                                        Servicos.NFSe servicoNFSe = new Servicos.NFSe(unitOfWork);
                                                        servicoNFSe.EmitirNFSe(cargaCTE.CTe.Codigo, unitOfWork);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (pedidoXMLNotasRemetentes.Any(o => o.DocsParaEmissaoNFSManual.Count > 0))
                                                    continue;

                                                if (!carga.AgNFSManual)
                                                {
                                                    carga.AgNFSManual = true;
                                                    Log.TratarErro("Carga Ag NF" + " Carga " + carga.CodigoCargaEmbarcador + " ag = " + carga.AgNFSManual.ToString(), "AgNFSManual");
                                                    repCarga.Atualizar(carga);
                                                }

                                                unitOfWork.Start();

                                                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotasRemetentes)
                                                {
                                                    Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual cargaDocumentoParaEmissaoNFSManual = serNFS.CriarNFPendenteEmissaoManualDeNFS(carga, pedidoXMLNotaFiscal, cargaPedido.ObterTomador(), localidadePrestacao, unitOfWork);
                                                    serNFS.AverbaCargaNFe(cargaDocumentoParaEmissaoNFSManual, apoliceSeguroAverbacaos, unitOfWork, configuracaoEmbarcador, cargaPedido.Pedido.FormaAverbacaoCTE);
                                                }

                                                unitOfWork.CommitChanges();

                                            }

                                            if (totalDocumentosGerados == 1 || (totalDocumentosGerados % quantidadeDocumentosAtualizarCarga) == 0)
                                                svcHubCarga.InformarQuantidadeDocumentosEmitidos(carga.Codigo, totalDocumentos, totalDocumentosGerados);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Servicos.Embarcador.CTe.CTEsImportados serCTEsImportados = new Servicos.Embarcador.CTe.CTEsImportados(unitOfWork);
                    serCTEsImportados.VincularCargaCTeEmitidoAnteriormente(carga, cargaPedido, unitOfWork);
                }
            }

            svcHubCarga.InformarQuantidadeDocumentosEmitidos(carga.Codigo, totalDocumentos, totalDocumentosGerados);
        }

        public void GerarCTePorNotaIndividual(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Cliente expedidor, Dominio.Entidades.Cliente recebedor, Dominio.Enumeradores.TipoPagamento tipoPagamento, Dominio.Enumeradores.TipoTomador tipoTomador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, int totalDocumentos, int quantidadeDocumentosAtualizarCarga, ref int totalDocumentosGerados, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal, Dominio.Entidades.Embarcador.Documentos.CIOT ciot, int tipoEnvio, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoRecebedor, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalContaContabilContabilizacao> pedidosXMLNotaFiscalContaContabilContabilizacao, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoExpedidor)
        {
            Hubs.Carga svcHubCarga = new Hubs.Carga();
            CTe serCte = new CTe(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOTCTe repCIOTCTe = new Repositorio.Embarcador.Documentos.CIOTCTe(unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            Servicos.Embarcador.Carga.NFS serNFS = new NFS(unitOfWork);
            Servicos.Embarcador.Pedido.PedidoXMLNotaFiscal servicoPedidoXMLNotaFiscal = new Pedido.PedidoXMLNotaFiscal(unitOfWork);

            int count = 0;

            carga = repCarga.BuscarPorCodigo(carga.Codigo);

            if (carga.TipoOperacao?.ConfiguracaoEmissaoDocumento?.EmitirDocumentoSempreOrigemDestinoPedido ?? false)
            {
                recebedor = null;
                expedidor = null;
            }

            List<int> codigosCargaPedidos = (from obj in cargaPedidos orderby obj.Peso descending select obj.Codigo).ToList();
            Dominio.Enumeradores.TipoAmbiente tipoAmbienteEmpresa = carga.Empresa.TipoAmbiente;

            if (recebedor != null)
                recebedor = repCliente.BuscarPorCPFCNPJ(recebedor.CPF_CNPJ);

            if (expedidor != null)
                expedidor = repCliente.BuscarPorCPFCNPJ(expedidor.CPF_CNPJ);

            foreach (int codigoCargaPedido in codigosCargaPedidos)
            {
                if (count > 0)
                {
                    count = 0;
                    unitOfWork.FlushAndClear();
                }

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigoComFetch(codigoCargaPedido);
                Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem = (from obj in cargasOrigem where obj.Codigo == cargaPedido.CargaOrigem.Codigo select obj).FirstOrDefault();

                if (!cargaPedido.PossuiCTe && !cargaPedido.PossuiNFS && !cargaPedido.PossuiNFSManual)
                    continue;

                bool emitirCteFilialEmissora = false;
                if (cargaPedido.Carga.EmpresaFilialEmissora != null && !cargaPedido.Carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                    emitirCteFilialEmissora = true;

                List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> apolicesSeguro = repApoliceSeguroAverbacao.BuscarPorCargaPedido(cargaPedido.Codigo, emitirCteFilialEmissora);

                if (!cargaPedido.CTeEmitidoNoEmbarcador && !cargaPedido.Pedido.PedidoTransbordo)
                {
                    serCte.ObterDescricoesComponentesPadrao(cargaPedido.ObterTomador(), carga, configuracaoEmbarcador, unitOfWork, out string descricaoComponenteValorFrete, out string descricaoComponenteValorICMS);

                    List<string> rotasPedido = (from obj in cargaPedido.CargaPedidoRotas select obj.IdenticacaoRota).ToList();
                    int serie = cargaPedido.Pedido.EmpresaSerie?.Numero ?? 0;
                    //string observacoesGerais = cargaPedido.Pedido.ObservacaoCTe;
                    string produtoPredominante = cargaPedido.Pedido.ProdutoPredominante;

                    if (string.IsNullOrWhiteSpace(produtoPredominante))
                        produtoPredominante = serCte.BuscarProdutoPredominante(cargaPedido, configuracaoEmbarcador, unitOfWork);

                    bool usarTipoPagamentoNF = cargaPedido.Pedido.UsarTipoPagamentoNF;

                    List<int> pedidoXMLNotasFiscais = repPedidoXMLNotaFiscal.BuscarCodigosPorCargaPedido(cargaPedido.Codigo);

                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscalPallet = null;

                    Log.GravarInfo($"Carga: {carga.Codigo}. DesconsiderarNotaPalletEmissaoCTE: {carga.TipoOperacao?.ConfiguracaoDocumentoEmissao?.DesconsiderarNotaPalletEmissaoCTE ?? false}", "GerarCTePorNotaIndividual");

                    if (carga.TipoOperacao?.ConfiguracaoDocumentoEmissao?.DesconsiderarNotaPalletEmissaoCTE ?? false)
                        pedidoXMLNotaFiscalPallet = repPedidoXMLNotaFiscal.BuscarNotaPalletCompativel(cargaPedido.Pedido.Destinatario.CPF_CNPJ, cargaPedido.Pedido.Remetente.CPF_CNPJ, carga.Codigo);

                    for (var i = 0; i < pedidoXMLNotasFiscais.Count; i++)
                    {
                        totalDocumentosGerados++;

                        if (count > 0)
                        {
                            unitOfWork.FlushAndClear();
                        }

                        Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorCodigo(pedidoXMLNotasFiscais[i]);

                        if (!pedidoXMLNotaFiscal.PossuiCTe && !pedidoXMLNotaFiscal.PossuiNFS && !pedidoXMLNotaFiscal.PossuiNFSManual)
                            continue;

                        Dominio.Entidades.Cliente remetentePedido = null;
                        Dominio.Entidades.Cliente destinatarioPedido = null;
                        Dominio.Entidades.Cliente tomadorUtilizar = tomador;
                        Dominio.Entidades.Cliente recebedorUtilizar = recebedor;

                        if (pedidoXMLNotaFiscal.XMLNotaFiscal.Recebedor != null &&
                            (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor ||
                             cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor))
                        {
                            if (recebedorUtilizar == null)
                                recebedorUtilizar = pedidoXMLNotaFiscal.XMLNotaFiscal.Recebedor;
                            else if (cargaPedido.Pedido.TipoOperacao == null || !cargaPedido.Pedido.TipoOperacao.NaoUtilizarRecebedorDaNotaFiscal)
                                recebedorUtilizar = pedidoXMLNotaFiscal.XMLNotaFiscal.Recebedor;
                        }

                        if (tipoTomador == Dominio.Enumeradores.TipoTomador.Outros && pedidoXMLNotaFiscal.XMLNotaFiscal.Tomador != null)
                            tomadorUtilizar = pedidoXMLNotaFiscal.XMLNotaFiscal.Tomador;

                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal tipoOperacaoNota = pedidoXMLNotaFiscal.XMLNotaFiscal.TipoOperacaoNotaFiscal;

                        if (tipoOperacaoNota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada) // esse caso é utilizado para notas de importação
                        {
                            remetentePedido = pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario;
                            destinatarioPedido = pedidoXMLNotaFiscal.XMLNotaFiscal.Emitente;
                        }
                        else
                        {
                            remetentePedido = pedidoXMLNotaFiscal.XMLNotaFiscal.Emitente;
                            destinatarioPedido = pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario;
                        }

                        //if(destinatarioPedido?.CPF_CNPJ == remetentePedido?.CPF_CNPJ)
                        //{
                        //    destinatarioPedido = cargaPedido.Pedido.Destinatario;
                        //}

                        //todo:foi modificado para armazenar o tipo de domento na nota, após um tempo remover o código abaixo e usar apenas oque está no pedidoxmlnotafiscal (28/11/2018)
                        bool possuiNFSManual = false;
                        Dominio.Entidades.Localidade localidadePrestacao = null;
                        if (cargaPedido.PossuiNFSManual)
                        {
                            if (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.Normal)
                            {
                                if (remetentePedido != null && destinatarioPedido != null && (destinatarioPedido.Localidade.Codigo == remetentePedido.Localidade.Codigo || cargaPedido.DisponibilizarDocumentoNFSManual))
                                {
                                    localidadePrestacao = destinatarioPedido.Localidade;
                                    possuiNFSManual = true;
                                }
                            }
                            else if (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor)
                            {
                                if (expedidor != null && destinatarioPedido != null && (destinatarioPedido.Localidade.Codigo == expedidor.Localidade.Codigo || cargaPedido.DisponibilizarDocumentoNFSManual))
                                {
                                    localidadePrestacao = destinatarioPedido.Localidade;
                                    possuiNFSManual = true;
                                }
                            }
                            else if (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor)
                            {
                                if (recebedorUtilizar != null && remetentePedido != null && (remetentePedido.Localidade.Codigo == recebedorUtilizar.Localidade.Codigo || cargaPedido.DisponibilizarDocumentoNFSManual))
                                {
                                    localidadePrestacao = remetentePedido.Localidade;
                                    possuiNFSManual = true;
                                }
                            }
                            else if (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor)
                            {
                                if (recebedorUtilizar != null && expedidor != null && (expedidor.Localidade.Codigo == recebedorUtilizar.Localidade.Codigo || cargaPedido.DisponibilizarDocumentoNFSManual))
                                {
                                    localidadePrestacao = expedidor.Localidade;
                                    possuiNFSManual = true;
                                }
                            }


                            if ((carga.Empresa?.SempreEmitirNFS ?? false) && remetentePedido != null)
                            {
                                localidadePrestacao = remetentePedido.Localidade;
                                possuiNFSManual = true;
                            }
                        }

                        if (!possuiNFSManual || (remetentePedido.Localidade.Codigo != destinatarioPedido.Localidade.Codigo && (!possuiNFSManual || (carga.Empresa?.SempreEmitirNFS ?? false))) || (possuiNFSManual && cargaPedido.ModeloDocumentoFiscalIntramunicipal != null))
                        {
                            if (pedidoXMLNotaFiscal.CTes.Count > 0) //já foi gerado um ct-e para esta nota
                                continue;

                            if ((carga?.TipoOperacao?.ConfiguracaoDocumentoEmissao?.PossuiNotaOrdemVenda ?? false) &&
                                                       (carga?.TipoOperacao?.ConfiguracaoDocumentoEmissao?.NaoUtilizaNotaVendaObjetoCTE ?? false)
                                                       && pedidoXMLNotaFiscal.XMLNotaFiscal.TipoNotaFiscalIntegrada == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscalIntegrada.OrdemVenda)
                            {
                                cargaPedido.Pedido.ObservacaoCTe += "NFe Venda: " + pedidoXMLNotaFiscal.XMLNotaFiscal.Chave;
                                repPedido.Atualizar(cargaPedido.Pedido);

                                Servicos.Log.GravarInfo("9 - Atualizando Obs pedido" + cargaPedido.Pedido.Codigo + " chave: " + pedidoXMLNotaFiscal.XMLNotaFiscal.Chave, "SolicitarEmissaoDocumentosAutorizados");
                                continue;
                            }

                            unitOfWork.Start();

                            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlNotasFiscaisCTe = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>() { pedidoXMLNotaFiscal.XMLNotaFiscal };

                            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoEmitir = null;

                            if (possuiNFSManual)
                                modeloDocumentoEmitir = cargaPedido.ModeloDocumentoFiscalIntramunicipal;
                            else
                                modeloDocumentoEmitir = pedidoXMLNotaFiscal.ModeloDocumentoFiscal;

                            if (modeloDocumentoEmitir == null)
                            {
                                modeloDocumentoEmitir = modeloDocumentoFiscal;
                                pedidoXMLNotaFiscal.ModeloDocumentoFiscal = modeloDocumentoFiscal;
                                repPedidoXMLNotaFiscal.Atualizar(pedidoXMLNotaFiscal);
                            }

                            List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> cargaPedidoComponentesFretesCliente = BuscarXMLPedidoComponentesFrete(pedidoXMLNotaFiscal, emitirCteFilialEmissora, unitOfWork);

                            Dominio.ObjetosDeValor.CTe.CTe cte = new Dominio.ObjetosDeValor.CTe.CTe();

                            cte.Documentos = new List<Dominio.ObjetosDeValor.CTe.Documento>();

                            Dominio.ObjetosDeValor.CTe.Documento docNF = serCte.BuscarDocumentoCTe(pedidoXMLNotaFiscal.XMLNotaFiscal, tipoAmbienteEmpresa, configuracaoEmbarcador);

                            cte.Documentos.Add(docNF);

                            if (pedidoXMLNotaFiscalPallet != null)
                            {
                                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xMLNotaFiscalPallet = pedidoXMLNotaFiscalPallet.XMLNotaFiscal;

                                Dominio.ObjetosDeValor.CTe.Documento docNFPallet = serCte.BuscarDocumentoCTe(xMLNotaFiscalPallet, tipoAmbienteEmpresa, configuracaoEmbarcador);

                                cte.Documentos.Add(docNFPallet);

                                xmlNotasFiscaisCTe.Add(xMLNotaFiscalPallet);

                                pedidoXMLNotaFiscalPallet = null;
                            }

                            cte.ValorTotalMercadoria = docNF.Valor;
                            cte.ValorImpostoSuspenso += pedidoXMLNotaFiscal?.XMLNotaFiscal?.ValorImpostoSuspenso ?? 0;

                            if (!emitirCteFilialEmissora)
                            {
                                if (carga.PossuiComponenteFreteComImpostoIncluso)
                                {
                                    cte.ValorAReceber = pedidoXMLNotaFiscal.ValorFreteComICMSIncluso;
                                    cte.ValorFrete = pedidoXMLNotaFiscal.ValorFreteComICMSIncluso;
                                    cte.ValorTotalPrestacaoServico = pedidoXMLNotaFiscal.ValorFreteComICMSIncluso;
                                }
                                else
                                {
                                    cte.ValorAReceber = pedidoXMLNotaFiscal.ValorFrete;
                                    cte.ValorFrete = pedidoXMLNotaFiscal.ValorFrete;
                                    cte.ValorTotalPrestacaoServico = pedidoXMLNotaFiscal.ValorFrete;
                                }
                            }
                            else
                            {
                                cte.ValorAReceber = pedidoXMLNotaFiscal.ValorFreteFilialEmissora;
                                cte.ValorFrete = pedidoXMLNotaFiscal.ValorFreteFilialEmissora;
                                cte.ValorTotalPrestacaoServico = pedidoXMLNotaFiscal.ValorFreteFilialEmissora;
                            }

                            cte.Serie = serie;
                            //cte.ObservacoesGerais = observacoesGerais;
                            cte.ProdutoPredominante = produtoPredominante;

                            Dominio.Enumeradores.TipoPagamento tipoPagamentoParaEmissao = tipoPagamento;
                            if (usarTipoPagamentoNF)
                            {
                                tipoPagamentoParaEmissao = (Dominio.Enumeradores.TipoPagamento)pedidoXMLNotaFiscal.XMLNotaFiscal.ModalidadeFrete;
                                if (tipoPagamentoParaEmissao == Dominio.Enumeradores.TipoPagamento.Pago)
                                    tipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
                                else if (tipoPagamentoParaEmissao == Dominio.Enumeradores.TipoPagamento.A_Pagar)
                                    tipoTomador = Dominio.Enumeradores.TipoTomador.Destinatario;
                                else if (tipoPagamentoParaEmissao == Dominio.Enumeradores.TipoPagamento.Outros)
                                    tipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
                            }

                            cte.PercentualPagamentoAgregado = pedidoXMLNotaFiscal.PercentualPagamentoAgregado;
                            cte.PesoLiquido = pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido;
                            cte.Peso = docNF.Peso;
                            cte.Volumes = docNF.Volume;
                            cte.MetrosCubicos = pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos;
                            cte.Pallets = pedidoXMLNotaFiscal.XMLNotaFiscal.QuantidadePallets;
                            cte.PesoCubado = pedidoXMLNotaFiscal.XMLNotaFiscal.PesoCubado;
                            cte.FatorCubagem = pedidoXMLNotaFiscal.XMLNotaFiscal.FatorCubagem;
                            cte.PesoFaturado = (docNF.Peso > pedidoXMLNotaFiscal.XMLNotaFiscal.PesoCubado ? docNF.Peso : pedidoXMLNotaFiscal.XMLNotaFiscal.PesoCubado);
                            cte.ValorTotalMoeda = pedidoXMLNotaFiscal.ValorTotalMoeda + cargaPedidoComponentesFretesCliente.Sum(o => o.ValorTotalMoeda ?? 0m);
                            cte.Moeda = pedidoXMLNotaFiscal.Moeda;
                            cte.ValorCotacaoMoeda = pedidoXMLNotaFiscal.ValorCotacaoMoeda;

                            List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidades = serCte.RetornaQuantidades(docNF.Peso, docNF.Volume, pedidoXMLNotaFiscal.XMLNotaFiscal.FatorCubagem, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoCubado, carga?.TipoOperacao?.ConfiguracaoEmissaoDocumento?.RatearPesoModeloVeicularEntreCTes ?? false, carga?.TipoOperacao?.ConfiguracaoEmissaoDocumento?.DescricaoUnidadeMedidaPesoModeloVeicularRateado, serCte.ObterPesoModeloVeicularRateado(cte.Peso, carga, unitOfWork));

                            List<string> rotas = new List<string>();

                            if (!string.IsNullOrWhiteSpace(pedidoXMLNotaFiscal.XMLNotaFiscal.Rota))
                                rotas.Add(pedidoXMLNotaFiscal.XMLNotaFiscal.Rota);
                            else
                                rotas = rotasPedido;

                            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = BuscarRegraICMS(pedidoXMLNotaFiscal, emitirCteFilialEmissora, modeloDocumentoEmitir);
                            Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = BuscarRegraISS(pedidoXMLNotaFiscal);
                            Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = servicoPedidoXMLNotaFiscal.ObterRetornoImpostoIBSCBS(pedidoXMLNotaFiscal, emitirCteFilialEmissora);

                            if (!emitirCteFilialEmissora)
                            {
                                if (carga.PossuiComponenteFreteComImpostoIncluso)
                                {
                                    regraICMS.IncluirICMSBC = false;
                                    regraICMS.PercentualInclusaoBC = 0m;
                                    regraICMS.ValorICMS += pedidoXMLNotaFiscal.ValorFreteComICMSIncluso - pedidoXMLNotaFiscal.ValorFrete + cargaPedidoComponentesFretesCliente.Sum(o => o.ValorComponenteComICMSIncluso) - cargaPedidoComponentesFretesCliente.Sum(o => o.ValorComponente);
                                    regraICMS.ValorICMSIncluso += regraICMS.ValorICMS;
                                    regraICMS.ValorBaseCalculoICMS += pedidoXMLNotaFiscal.ValorFreteComICMSIncluso + cargaPedidoComponentesFretesCliente.Sum(o => o.ValorComponenteComICMSIncluso);
                                    regraICMS.ValorBaseCalculoPISCOFINS += pedidoXMLNotaFiscal.ValorFreteComICMSIncluso - pedidoXMLNotaFiscal.BaseCalculoICMS;
                                }
                                else
                                {
                                    regraICMS.ValorBaseCalculoICMS = pedidoXMLNotaFiscal.BaseCalculoICMS;
                                    regraICMS.ValorBaseCalculoPISCOFINS = pedidoXMLNotaFiscal.BaseCalculoICMS;
                                    regraICMS.ValorICMS = pedidoXMLNotaFiscal.ValorICMS;
                                    regraICMS.ValorICMSIncluso = pedidoXMLNotaFiscal.ValorICMSIncluso;
                                }

                                regraICMS.ValorPis = pedidoXMLNotaFiscal.ValorPis;
                                regraICMS.ValorCofins = pedidoXMLNotaFiscal.ValorCofins;
                                regraICMS.ValorCreditoPresumido = pedidoXMLNotaFiscal.ValorCreditoPresumido;

                                servicoPedidoXMLNotaFiscal.PreencherValoresRetornoIBSCBS(impostoIBSCBS, pedidoXMLNotaFiscal.BaseCalculoIBSCBS, pedidoXMLNotaFiscal.ValorIBSEstadual, pedidoXMLNotaFiscal.ValorIBSMunicipal, pedidoXMLNotaFiscal.ValorCBS);
                            }
                            else
                            {
                                regraICMS.ValorBaseCalculoICMS = pedidoXMLNotaFiscal.BaseCalculoICMSFilialEmissora;
                                regraICMS.ValorBaseCalculoPISCOFINS = pedidoXMLNotaFiscal.BaseCalculoICMSFilialEmissora;
                                regraICMS.ValorICMS = pedidoXMLNotaFiscal.ValorICMSFilialEmissora;
                                regraICMS.ValorICMSIncluso = pedidoXMLNotaFiscal.ValorICMSFilialEmissora;
                                regraICMS.ValorCreditoPresumido = pedidoXMLNotaFiscal.ValorCreditoPresumidoFilialEmissora;

                                servicoPedidoXMLNotaFiscal.PreencherValoresRetornoIBSCBS(impostoIBSCBS, pedidoXMLNotaFiscal.BaseCalculoIBSCBSFilialEmissora, pedidoXMLNotaFiscal.ValorIBSEstadualFilialEmissora, pedidoXMLNotaFiscal.ValorIBSMunicipalFilialEmissora, pedidoXMLNotaFiscal.ValorCBSFilialEmissora);
                            }

                            regraISS.ValorBaseCalculoISS += pedidoXMLNotaFiscal.BaseCalculoISS;
                            regraISS.ValorISS += pedidoXMLNotaFiscal.ValorISS;
                            regraISS.ValorRetencaoISS += pedidoXMLNotaFiscal.ValorRetencaoISS;

                            //regraISS.BaseCalculoIR += pedidoXMLNotaFiscal.BaseCalculoIR;
                            //regraISS.AliquotaIR += pedidoXMLNotaFiscal.AliquotaIR;
                            //regraISS.ValorIR += pedidoXMLNotaFiscal.ValorIR;

                            if (ciot != null)
                                cte.CIOT = ciot.Numero;

                            Dominio.Enumeradores.TipoServico tipoServico = Dominio.Enumeradores.TipoServico.Normal;
                            if (cargaPedido.Redespacho)
                            {
                                string raizEmpresa = Utilidades.String.OnlyNumbers(cargaPedido.CargaPedidoTrechoAnterior.CargaOrigem.Empresa.CNPJ_SemFormato).Remove(8, 6);
                                string raizTomador = "";

                                if (cargaPedido.ObterTomador().Tipo == "J")
                                    raizTomador = Utilidades.String.OnlyNumbers(cargaPedido.ObterTomador().CPF_CNPJ_SemFormato).Remove(8, 6);

                                if (raizEmpresa == raizTomador)
                                {
                                    if (recebedorUtilizar != null && expedidor != null)
                                        tipoServico = Dominio.Enumeradores.TipoServico.RedIntermediario;
                                    else
                                        tipoServico = Dominio.Enumeradores.TipoServico.Redespacho;
                                }
                            }

                            List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> seguros = serCte.ConverterSegurosDeTerceirosEmSeguroCTe(apolicesSeguro, tipoTomador, cte.ValorTotalMercadoria);
                            List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ctesAnteriores = new List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe>();

                            Dominio.Enumeradores.TipoCTE tipoCTe = Dominio.Enumeradores.TipoCTE.Normal;

                            Dominio.Entidades.Empresa empresa = cargaOrigem.Empresa;
                            if (emitirCteFilialEmissora)
                            {
                                if (cargaPedido.CargaPedidoProximoTrecho != null)
                                    recebedorUtilizar = null;

                                if (cargaPedido.CargaPedidoTrechoAnterior != null)
                                    expedidor = null;

                                empresa = cargaOrigem.EmpresaFilialEmissora;
                            }

                            if (cargaPedido.Pedido.TipoOperacao != null && cargaPedido.Pedido.TipoOperacao.ColetaEmProdutorRural && cargaPedido.Pedido.TipoOperacao.RemetenteDoCTeSeraODestinatarioDoPedido)
                                remetentePedido = repCliente.BuscarPorCPFCNPJ(cargaPedido.Pedido.Destinatario.CPF_CNPJ);

                            List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil> configuracoes = Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil.ConverterPedidoXMLNotaFiscalContaContabilContabilizacao(pedidoXMLNotaFiscal, pedidosXMLNotaFiscalContaContabilContabilizacao);
                            bool sempreEmitirAutomaticamente = false;
                            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTE = serCte.GerarCTe(carga, cargaPedido, cte, empresa, remetentePedido, destinatarioPedido, tomadorUtilizar, expedidor, recebedorUtilizar, cargaPedido.Pedido.UsarOutroEnderecoOrigem && cargaPedido.Pedido.EnderecoOrigem?.Localidade != null ? cargaPedido.Pedido.EnderecoOrigem.Localidade : remetentePedido.Localidade, cargaPedido.Pedido.UsarOutroEnderecoDestino && cargaPedido.Pedido.EnderecoDestino?.Localidade != null ? cargaPedido.Pedido.EnderecoDestino.Localidade : destinatarioPedido.Localidade, cargaPedido.Pedido.UsarOutroEnderecoOrigem ? cargaPedido.Pedido.EnderecoOrigem : null, cargaPedido.Pedido.UsarOutroEnderecoDestino ? cargaPedido.Pedido.EnderecoDestino : null, tipoPagamentoParaEmissao, tipoTomador, quantidades, cargaPedidoComponentesFretesCliente, pedidoXMLNotaFiscal.CargaPedido.Pedido.ObservacaoCTe, pedidoXMLNotaFiscal.CargaPedido.Pedido.ObservacaoCTeTerceiro, pedidoXMLNotaFiscal.CargaPedido.Pedido, rotas, seguros, regraICMS, regraISS, impostoIBSCBS, tipoServicoMultisoftware, modeloDocumentoEmitir, tipoServico, tipoCTe, ctesAnteriores, tipoEnvio, sempreEmitirAutomaticamente, null, false, unitOfWork, configuracaoEmbarcador, configuracaoGeralCarga, pedidoXMLNotaFiscal.CentroResultado, pedidoXMLNotaFiscal.CentroResultadoDestinatario, pedidoXMLNotaFiscal.ItemServico, pedidoXMLNotaFiscal.CentroResultadoEscrituracao, pedidoXMLNotaFiscal.CentroResultadoICMS, pedidoXMLNotaFiscal.CentroResultadoPIS, pedidoXMLNotaFiscal.CentroResultadoCOFINS, pedidoXMLNotaFiscal.ValorMaximoCentroContabilizacao, configuracoes, xmlNotasFiscaisCTe, enderecoRecebedor, descricaoComponenteValorICMS, descricaoComponenteValorFrete, enderecoExpedidor);

                            serCte.AverbaCargaCTe(cargaCTE, apolicesSeguro, unitOfWork, configuracaoEmbarcador, cargaPedido.Pedido.FormaAverbacaoCTE);

                            if (ciot != null)
                            {
                                Dominio.Entidades.Embarcador.Documentos.CIOTCTe ciotCTe = new Dominio.Entidades.Embarcador.Documentos.CIOTCTe()
                                {
                                    CargaCTe = cargaCTE,
                                    CIOT = ciot
                                };

                                repCIOTCTe.Inserir(ciotCTe);
                            }

                            Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTE = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe();

                            cargaPedidoXMLNotaFiscalCTE.CargaCTe = cargaCTE;
                            cargaPedidoXMLNotaFiscalCTE.PedidoXMLNotaFiscal = pedidoXMLNotaFiscal;

                            repCargaPedidoXMLNotaFiscalCTe.Inserir(cargaPedidoXMLNotaFiscalCTE);

                            if (possuiNFSManual)
                            {
                                serNFS.CriarCargaCTePendenteEmissaoManualDeNFS(carga, cargaCTE, localidadePrestacao, unitOfWork);

                                carga.AgNFSManual = true;
                            }

                            unitOfWork.CommitChanges();

                            if (cargaCTE.CTe != null && cargaCTE.CTe.Status == "E")
                            {
                                if (cargaCTE.CTe.ModeloDocumentoFiscal.Numero == "57")
                                {
                                    Servicos.CTe servicoCte = new Servicos.CTe(unitOfWork);
                                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cteIntegrado = cargaCTE.CTe;
                                    servicoCte.Emitir(ref cteIntegrado, unitOfWork);
                                }
                                else if (cargaCTE.CTe.ModeloDocumentoFiscal.Numero == "39")
                                {
                                    Servicos.NFSe servicoNFSe = new Servicos.NFSe(unitOfWork);
                                    servicoNFSe.EmitirNFSe(cargaCTE.CTe.Codigo, unitOfWork);
                                }
                            }
                        }
                        else
                        {
                            if (pedidoXMLNotaFiscal.DocsParaEmissaoNFSManual.Count > 0)
                                continue;
                            carga.AgNFSManual = true;

                            unitOfWork.Start();
                            Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual cargaDocumentoParaEmissaoNFSManual = serNFS.CriarNFPendenteEmissaoManualDeNFS(carga, pedidoXMLNotaFiscal, cargaPedido.ObterTomador(), localidadePrestacao, unitOfWork);
                            serNFS.AverbaCargaNFe(cargaDocumentoParaEmissaoNFSManual, apolicesSeguro, unitOfWork, configuracaoEmbarcador, cargaPedido.Pedido.FormaAverbacaoCTE);

                            unitOfWork.CommitChanges();
                        }

                        count++;

                        if (totalDocumentosGerados == 1 || (totalDocumentosGerados % quantidadeDocumentosAtualizarCarga) == 0)
                            svcHubCarga.InformarQuantidadeDocumentosEmitidos(carga.Codigo, totalDocumentos, totalDocumentosGerados);
                    }
                }
                else
                {
                    Servicos.Embarcador.CTe.CTEsImportados serCTEsImportados = new Servicos.Embarcador.CTe.CTEsImportados(unitOfWork);
                    serCTEsImportados.VincularCargaCTeEmitidoAnteriormente(carga, cargaPedido, unitOfWork);
                }

            }

            svcHubCarga.InformarQuantidadeDocumentosEmitidos(carga.Codigo, totalDocumentos, totalDocumentosGerados);
        }

        public void GerarCTePorNotaAgrupadaEntrePedidos(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Cliente expedidor, Dominio.Entidades.Cliente recebedor, Dominio.Enumeradores.TipoPagamento tipoPagamento, Dominio.Enumeradores.TipoTomador tipoTomador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, int totalDocumentos, int quantidadeDocumentosAtualizarCarga, ref int totalDocumentosGerados, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal, Dominio.Entidades.Embarcador.Documentos.CIOT ciot, int tipoEnvio, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoRecebedor, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalContaContabilContabilizacao> pedidosXMLNotaFiscalContaContabilContabilizacao, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoExpedidor, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoRemetente, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoDestinatario)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOTCTe repCIOTCTe = new Repositorio.Embarcador.Documentos.CIOTCTe(unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEnderedo = new Repositorio.Embarcador.Pedidos.PedidoEndereco(unitOfWork);
            Repositorio.Embarcador.Rateio.RateioFormula repRateioFormula = new Repositorio.Embarcador.Rateio.RateioFormula(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            Servicos.Embarcador.Carga.NFS serNFS = new NFS(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
            CTe serCte = new CTe(unitOfWork);
            ICMS serICMS = new ICMS(unitOfWork);
            Hubs.Carga svcHubCarga = new Hubs.Carga();
            Servicos.Embarcador.Pedido.PedidoXMLNotaFiscal servicoPedidoXMLNotaFiscal = new Pedido.PedidoXMLNotaFiscal(unitOfWork);

            List<int> codigosCargaPedidos = (from obj in cargaPedidos select obj.Codigo).ToList();

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = repCargaPedido.BuscarPorCodigoComFetch(codigosCargaPedidos);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido primeiroCargaPedido = cargasPedidos.FirstOrDefault();
            Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaRateio = primeiroCargaPedido != null && primeiroCargaPedido.FormulaRateio != null ? repRateioFormula.BuscarPorCodigo(primeiroCargaPedido.FormulaRateio.Codigo) : null;

            List<double> tomadores = new List<double>();

            if (tipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
            {
                tomadores = repPedidoXMLNotaFiscal.BuscarTomadoresPorCargaPedido(codigosCargaPedidos);
            }

            if (!tomadores.Contains(0D))
                tomadores.Add(0D); //adiciona para pegar os registros sem tomador

            List<double> remetentes = repPedidoXMLNotaFiscal.BuscarRemetentesPorCargaPedido(codigosCargaPedidos);
            List<double> destinatarios = repPedidoXMLNotaFiscal.BuscarDestinatariosPorCargaPedido(codigosCargaPedidos);


            if (enderecoRemetente != null)
                enderecoRemetente = repPedidoEnderedo.BuscarPorCodigo(enderecoRemetente.Codigo);

            if (enderecoDestinatario != null)
                enderecoDestinatario = repPedidoEnderedo.BuscarPorCodigo(enderecoDestinatario.Codigo);

            if (enderecoRecebedor != null)
                enderecoRecebedor = repPedidoEnderedo.BuscarPorCodigo(enderecoRecebedor.Codigo);

            if (enderecoExpedidor != null)
                enderecoExpedidor = repPedidoEnderedo.BuscarPorCodigo(enderecoExpedidor.Codigo);


            if (!primeiroCargaPedido.CTeEmitidoNoEmbarcador && !primeiroCargaPedido.Pedido.PedidoTransbordo)
            {
                List<string> rotasPedido = (from obj in primeiroCargaPedido.CargaPedidoRotas select obj.IdenticacaoRota).ToList();
                int serie = primeiroCargaPedido.Pedido.EmpresaSerie?.Numero ?? 0;
                string observacoesGerais = string.Join(", ", cargasPedidos.Select(c => c.Pedido.ObservacaoCTe).Distinct().ToList());
                string produtoPredominante = string.Join(", ", cargasPedidos.Select(c => c.Pedido.ProdutoPredominante).Distinct().ToList());

                if (string.IsNullOrWhiteSpace(produtoPredominante))
                    produtoPredominante = serCte.BuscarProdutoPredominante(cargasPedidos, configuracaoEmbarcador, unitOfWork);

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete> modalidadesNotas = null;
                bool comparaModalidadeNF = false;
                if (cargasPedidos.Any(c => c.Pedido.UsarTipoPagamentoNF))
                {
                    modalidadesNotas = repPedidoXMLNotaFiscal.BuscarModalidadesPagamentoPorCargaPedido(codigosCargaPedidos);
                    comparaModalidadeNF = true;
                }
                else
                {
                    modalidadesNotas = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete>();
                    modalidadesNotas.Add((Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete)tipoPagamento);
                }



                bool contemNotaFiscalSemInscricao = false;
                if (configuracaoEmbarcador.AgruparCTesDiferentesPedidosMesmoDestinatario)
                    contemNotaFiscalSemInscricao = repPedidoXMLNotaFiscal.ContemNotaFiscalSemInscricao(codigosCargaPedidos);

                foreach (double tomadorNotaFiscal in tomadores)
                {
                    foreach (double remetente in remetentes)
                    {
                        List<string> iesRemetente = new List<string>();
                        if (!contemNotaFiscalSemInscricao && configuracaoEmbarcador.AgruparCTesDiferentesPedidosMesmoDestinatario)
                            iesRemetente = repPedidoXMLNotaFiscal.BuscarIERemetentePorCargaPedido(codigosCargaPedidos, remetente);
                        else
                            iesRemetente.Add("");

                        foreach (var ieRemetente in iesRemetente)
                        {
                            foreach (double destinatario in destinatarios)
                            {
                                foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete? modalidade in modalidadesNotas)
                                {
                                    List<string> iesDestinatario = new List<string>();
                                    if (!contemNotaFiscalSemInscricao && configuracaoEmbarcador.AgruparCTesDiferentesPedidosMesmoDestinatario)
                                        iesDestinatario = repPedidoXMLNotaFiscal.BuscarIEDestinatarioPorCargaPedido(codigosCargaPedidos, destinatario);
                                    else
                                        iesDestinatario.Add("");

                                    foreach (var ieDestinatario in iesDestinatario)
                                    {
                                        unitOfWork.FlushAndClear();

                                        cargasPedidos = repPedidoXMLNotaFiscal.BuscarCargasPedidosPorCodigosRemetenteDestinatario(codigosCargaPedidos, remetente, destinatario, ieRemetente, ieDestinatario);
                                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasRemetentes = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(codigosCargaPedidos, remetente, destinatario, null, null, ieRemetente, ieDestinatario);

                                        if (pedidoXMLNotasRemetentes.Count <= 0)
                                            continue;

                                        primeiroCargaPedido = repPedidoXMLNotaFiscal.BuscarPorCodigosRemetenteDestinatario(codigosCargaPedidos, remetente, destinatario, ieRemetente, ieDestinatario);
                                        if (primeiroCargaPedido == null)
                                            continue;

                                        Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem = (from obj in cargasOrigem where obj.Codigo == primeiroCargaPedido.CargaOrigem.Codigo select obj).FirstOrDefault();

                                        serCte.ObterDescricoesComponentesPadrao(primeiroCargaPedido.ObterTomador(), carga, configuracaoEmbarcador, unitOfWork, out string descricaoComponenteValorFrete, out string descricaoComponenteValorICMS);

                                        Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscalFirst = pedidoXMLNotasRemetentes.FirstOrDefault();

                                        if (!pedidoXMLNotaFiscalFirst.PossuiCTe && !pedidoXMLNotaFiscalFirst.PossuiNFS && !pedidoXMLNotaFiscalFirst.PossuiNFSManual)
                                            continue;

                                        Dominio.Entidades.Cliente tomadorUtilizar = null;
                                        Dominio.Entidades.Cliente remetentePedido = null;
                                        Dominio.Entidades.Cliente destinatarioPedido = null;



                                        if (tomadorNotaFiscal > 0D)
                                            tomadorUtilizar = repCliente.BuscarPorCPFCNPJ(tomadorNotaFiscal);
                                        else if (tomador != null)
                                            tomadorUtilizar = repCliente.BuscarPorCPFCNPJ(tomador.CPF_CNPJ);

                                        if (expedidor != null)
                                            expedidor = repCliente.BuscarPorCPFCNPJ(expedidor.CPF_CNPJ);

                                        if (recebedor != null)
                                            recebedor = repCliente.BuscarPorCPFCNPJ(recebedor.CPF_CNPJ);

                                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal tipoOperacaoNota = pedidoXMLNotaFiscalFirst.XMLNotaFiscal.TipoOperacaoNotaFiscal;

                                        bool possuiNFSManual = false;

                                        Dominio.Entidades.Localidade localidadePrestacao = null;

                                        //todo:foi modificado para armazenar o tipo de domento na nota, após um tempo remover o código abaixo e usar apenas oque está no pedidoxmlnotafiscal (28/11/2018)
                                        if (cargasPedidos.Any(c => c.PossuiNFSManual))
                                        {
                                            if (tipoOperacaoNota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada) // esse caso é utilizado para notas de importação
                                            {
                                                remetentePedido = repCliente.BuscarPorCPFCNPJ(destinatario);
                                                destinatarioPedido = repCliente.BuscarPorCPFCNPJ(remetente);
                                            }
                                            else
                                            {
                                                remetentePedido = repCliente.BuscarPorCPFCNPJ(remetente);
                                                destinatarioPedido = repCliente.BuscarPorCPFCNPJ(destinatario);
                                            }

                                            if (cargasPedidos.Any(c => c.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.Normal))
                                            {
                                                if (remetentePedido != null && destinatarioPedido != null && (destinatarioPedido.Localidade.Codigo == remetentePedido.Localidade.Codigo || cargaPedidos.Any(o => o.DisponibilizarDocumentoNFSManual)))
                                                {
                                                    localidadePrestacao = destinatarioPedido.Localidade;
                                                    possuiNFSManual = true;
                                                }
                                            }
                                            else if (cargasPedidos.Any(c => c.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor))
                                            {
                                                if (expedidor != null && destinatarioPedido != null && (destinatarioPedido.Localidade.Codigo == expedidor.Localidade.Codigo || cargaPedidos.Any(o => o.DisponibilizarDocumentoNFSManual)))
                                                {
                                                    localidadePrestacao = destinatarioPedido.Localidade;
                                                    possuiNFSManual = true;
                                                }
                                            }
                                            else if (cargasPedidos.Any(c => c.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor))
                                            {
                                                if (recebedor != null && remetentePedido != null && (remetentePedido.Localidade.Codigo == recebedor.Localidade.Codigo || cargaPedidos.Any(o => o.DisponibilizarDocumentoNFSManual)))
                                                {
                                                    localidadePrestacao = remetentePedido.Localidade;
                                                    possuiNFSManual = true;
                                                }
                                            }
                                            else if (cargasPedidos.Any(c => c.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor))
                                            {
                                                if (recebedor != null && expedidor != null && (expedidor.Localidade.Codigo == recebedor.Localidade.Codigo || cargaPedidos.Any(o => o.DisponibilizarDocumentoNFSManual)))
                                                {
                                                    localidadePrestacao = expedidor.Localidade;
                                                    possuiNFSManual = true;
                                                }
                                            }

                                            if ((carga.Empresa?.SempreEmitirNFS ?? false) && remetentePedido != null)
                                            {
                                                localidadePrestacao = remetentePedido.Localidade;
                                                possuiNFSManual = true;
                                            }
                                        }

                                        bool emitirCteFilialEmissora = false;
                                        if (primeiroCargaPedido.Carga.EmpresaFilialEmissora != null && !primeiroCargaPedido.Carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                                            emitirCteFilialEmissora = true;
                                        List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> apolicesSeguro = repApoliceSeguroAverbacao.BuscarPorCargaPedido(codigosCargaPedidos, emitirCteFilialEmissora);

                                        if (!possuiNFSManual || (possuiNFSManual && primeiroCargaPedido.ModeloDocumentoFiscalIntramunicipal != null))
                                        {
                                            if ((from obj in pedidoXMLNotasRemetentes where obj.CTes.Count > 0 select obj).Any())
                                                continue;

                                            totalDocumentosGerados++;

                                            Dominio.Entidades.ModeloDocumentoFiscal modeloNotaFiscalAgrupado = null;

                                            if (possuiNFSManual)
                                                modeloNotaFiscalAgrupado = primeiroCargaPedido.ModeloDocumentoFiscalIntramunicipal;
                                            else
                                                modeloNotaFiscalAgrupado = pedidoXMLNotaFiscalFirst.ModeloDocumentoFiscal;

                                            if (modeloNotaFiscalAgrupado == null)
                                            {
                                                modeloNotaFiscalAgrupado = modeloDocumentoFiscal;
                                                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotasRemetentes)
                                                {
                                                    pedidoXMLNotaFiscal.ModeloDocumentoFiscal = modeloDocumentoFiscal;
                                                    repPedidoXMLNotaFiscal.Atualizar(pedidoXMLNotaFiscal);
                                                }
                                            }

                                            List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> cargaPedidoComponentesFretesCliente = BuscarXMLPedidoComponentesFreteAgrupados(pedidoXMLNotasRemetentes, emitirCteFilialEmissora, unitOfWork);

                                            unitOfWork.Start();

                                            Dominio.ObjetosDeValor.CTe.CTe cte = new Dominio.ObjetosDeValor.CTe.CTe();
                                            cte.Documentos = new List<Dominio.ObjetosDeValor.CTe.Documento>();

                                            List<string> rotas = new List<string>();
                                            decimal peso = 0m, pesoLiquido = 0m, metrosCubicos = 0m, pesoCubado = 0m, fatorCubagem = 0m, percentualPagamentoAgregado = 0m, pallets = 0m, valorTotalMoeda = 0m;
                                            int volumes = 0;

                                            cargasPedidos = repPedidoXMLNotaFiscal.BuscarCargasPedidosPorCodigosRemetenteDestinatario(codigosCargaPedidos, remetente, destinatario, ieRemetente, ieDestinatario);
                                            primeiroCargaPedido = repPedidoXMLNotaFiscal.BuscarPorCodigosRemetenteDestinatario(codigosCargaPedidos, remetente, destinatario, ieRemetente, ieDestinatario);
                                            if (primeiroCargaPedido == null)
                                                continue;

                                            carga = repCarga.BuscarPorCodigo(carga.Codigo);

                                            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = BuscarRegraICMS(pedidoXMLNotaFiscalFirst, emitirCteFilialEmissora, modeloNotaFiscalAgrupado);
                                            Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = BuscarRegraISS(pedidoXMLNotaFiscalFirst);
                                            Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = servicoPedidoXMLNotaFiscal.ObterRetornoImpostoIBSCBS(pedidoXMLNotaFiscalFirst, emitirCteFilialEmissora);

                                            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotasRemetentes)
                                            {
                                                if ((carga?.TipoOperacao?.ConfiguracaoDocumentoEmissao?.PossuiNotaOrdemVenda ?? false) &&
                                                       (carga?.TipoOperacao?.ConfiguracaoDocumentoEmissao?.NaoUtilizaNotaVendaObjetoCTE ?? false)
                                                       && pedidoXMLNotaFiscal.XMLNotaFiscal.TipoNotaFiscalIntegrada == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscalIntegrada.OrdemVenda)
                                                {
                                                    pedidoXMLNotaFiscal.CargaPedido.Pedido.ObservacaoCTe += " NFe Venda: " + pedidoXMLNotaFiscal.XMLNotaFiscal.Chave;
                                                    repPedido.Atualizar(pedidoXMLNotaFiscal.CargaPedido.Pedido);

                                                    Servicos.Log.TratarErro("9 - Atualizando Obs pedido" + pedidoXMLNotaFiscal.CargaPedido.Pedido.Codigo + " chave: " + pedidoXMLNotaFiscal.XMLNotaFiscal.Chave, "SolicitarEmissaoDocumentosAutorizados");
                                                    continue;
                                                }


                                                if (!string.IsNullOrWhiteSpace(pedidoXMLNotaFiscal.XMLNotaFiscal.Rota))
                                                {
                                                    if (!rotas.Contains(pedidoXMLNotaFiscal.XMLNotaFiscal.Rota))
                                                        rotas.Add(pedidoXMLNotaFiscal.XMLNotaFiscal.Rota);
                                                }

                                                Dominio.ObjetosDeValor.CTe.Documento docNF = serCte.BuscarDocumentoCTe(pedidoXMLNotaFiscal.XMLNotaFiscal, carga.Empresa.TipoAmbiente, configuracaoEmbarcador);
                                                cte.Documentos.Add(docNF);

                                                percentualPagamentoAgregado = pedidoXMLNotaFiscal.PercentualPagamentoAgregado;
                                                pesoLiquido += pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido;
                                                peso += docNF.Peso;
                                                volumes += docNF.Volume;

                                                pallets += pedidoXMLNotaFiscal.XMLNotaFiscal.QuantidadePallets;
                                                metrosCubicos += pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos;
                                                pesoCubado += pedidoXMLNotaFiscal.XMLNotaFiscal.PesoCubado;
                                                valorTotalMoeda += pedidoXMLNotaFiscal.ValorTotalMoeda ?? 0m;

                                                if (pedidoXMLNotaFiscal.XMLNotaFiscal.FatorCubagem > 0m)
                                                    fatorCubagem = pedidoXMLNotaFiscal.XMLNotaFiscal.FatorCubagem;

                                                cte.ValorTotalMercadoria += docNF.Valor;
                                                cte.ValorImpostoSuspenso += pedidoXMLNotaFiscal?.XMLNotaFiscal?.ValorImpostoSuspenso ?? 0;

                                                if (!emitirCteFilialEmissora)
                                                {
                                                    if (carga.PossuiComponenteFreteComImpostoIncluso)
                                                    {
                                                        regraICMS.IncluirICMSBC = false;
                                                        regraICMS.PercentualInclusaoBC = 0m;
                                                        regraICMS.ValorICMS += pedidoXMLNotaFiscal.ValorFreteComICMSIncluso - pedidoXMLNotaFiscal.ValorFrete + cargaPedidoComponentesFretesCliente.Sum(o => o.ValorComponenteComICMSIncluso) - cargaPedidoComponentesFretesCliente.Sum(o => o.ValorComponente);
                                                        regraICMS.ValorICMSIncluso += regraICMS.ValorICMS;
                                                        regraICMS.ValorBaseCalculoICMS += pedidoXMLNotaFiscal.ValorFreteComICMSIncluso + cargaPedidoComponentesFretesCliente.Sum(o => o.ValorComponenteComICMSIncluso);
                                                        regraICMS.ValorBaseCalculoPISCOFINS += pedidoXMLNotaFiscal.ValorFreteComICMSIncluso - pedidoXMLNotaFiscal.BaseCalculoICMS;
                                                    }
                                                    else
                                                    {
                                                        regraICMS.ValorBaseCalculoICMS += pedidoXMLNotaFiscal.BaseCalculoICMS;
                                                        regraICMS.ValorBaseCalculoPISCOFINS += pedidoXMLNotaFiscal.BaseCalculoICMS;
                                                        regraICMS.ValorICMS += pedidoXMLNotaFiscal.ValorICMS;
                                                        regraICMS.ValorICMSIncluso += pedidoXMLNotaFiscal.ValorICMSIncluso;
                                                    }

                                                    regraICMS.ValorCofins += pedidoXMLNotaFiscal.ValorCofins;
                                                    regraICMS.ValorPis += pedidoXMLNotaFiscal.ValorPis;
                                                    regraICMS.ValorCreditoPresumido += pedidoXMLNotaFiscal.ValorCreditoPresumido;

                                                    servicoPedidoXMLNotaFiscal.PreencherValoresRetornoIBSCBS(impostoIBSCBS, pedidoXMLNotaFiscal.BaseCalculoIBSCBS, pedidoXMLNotaFiscal.ValorIBSEstadual, pedidoXMLNotaFiscal.ValorIBSMunicipal, pedidoXMLNotaFiscal.ValorCBS);
                                                }
                                                else
                                                {
                                                    regraICMS.ValorBaseCalculoICMS += pedidoXMLNotaFiscal.BaseCalculoICMSFilialEmissora;
                                                    regraICMS.ValorICMS += pedidoXMLNotaFiscal.ValorICMSFilialEmissora;
                                                    regraICMS.ValorICMSIncluso += pedidoXMLNotaFiscal.ValorICMSFilialEmissora;
                                                    regraICMS.ValorCreditoPresumido += pedidoXMLNotaFiscal.ValorCreditoPresumidoFilialEmissora;

                                                    servicoPedidoXMLNotaFiscal.PreencherValoresRetornoIBSCBS(impostoIBSCBS, pedidoXMLNotaFiscal.BaseCalculoIBSCBSFilialEmissora, pedidoXMLNotaFiscal.ValorIBSEstadualFilialEmissora, pedidoXMLNotaFiscal.ValorIBSMunicipalFilialEmissora, pedidoXMLNotaFiscal.ValorCBSFilialEmissora);
                                                }

                                                regraISS.ValorBaseCalculoISS += pedidoXMLNotaFiscal.BaseCalculoISS;
                                                regraISS.ValorISS += pedidoXMLNotaFiscal.ValorISS;
                                                regraISS.ValorRetencaoISS += pedidoXMLNotaFiscal.ValorRetencaoISS;

                                                //regraISS.BaseCalculoIR += pedidoXMLNotaFiscal.BaseCalculoIR;
                                                //regraISS.AliquotaIR += pedidoXMLNotaFiscal.AliquotaIR;
                                                //regraISS.ValorIR += pedidoXMLNotaFiscal.ValorIR;
                                            }

                                            cte.Peso = peso;
                                            cte.PesoLiquido = pesoLiquido;
                                            cte.Volumes = volumes;
                                            cte.MetrosCubicos = metrosCubicos;
                                            cte.Pallets = pallets;
                                            cte.PesoCubado = pesoCubado;
                                            cte.FatorCubagem = fatorCubagem;
                                            cte.PesoFaturado = (peso > pesoCubado ? peso : pesoCubado);
                                            cte.PercentualPagamentoAgregado = percentualPagamentoAgregado;
                                            cte.Moeda = pedidoXMLNotaFiscalFirst.Moeda;
                                            cte.ValorTotalMoeda = valorTotalMoeda + cargaPedidoComponentesFretesCliente.Sum(o => o.ValorTotalMoeda ?? 0m);
                                            cte.Moeda = pedidoXMLNotaFiscalFirst.Moeda;
                                            cte.ValorCotacaoMoeda = pedidoXMLNotaFiscalFirst.ValorCotacaoMoeda;

                                            List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidades = serCte.RetornaQuantidades(peso, volumes, fatorCubagem, metrosCubicos, pesoCubado, carga?.TipoOperacao?.ConfiguracaoEmissaoDocumento?.RatearPesoModeloVeicularEntreCTes ?? false, carga?.TipoOperacao?.ConfiguracaoEmissaoDocumento?.DescricaoUnidadeMedidaPesoModeloVeicularRateado, serCte.ObterPesoModeloVeicularRateado(cte.Peso, carga, unitOfWork));

                                            if (rotas.Count == 0 && rotasPedido.Count > 0)
                                                rotas = rotasPedido;

                                            if (carga.PossuiComponenteFreteComImpostoIncluso)
                                                cte.ValorAReceber = pedidoXMLNotasRemetentes.Sum(o => o.ValorFreteComICMSIncluso);
                                            else
                                            {
                                                if (!emitirCteFilialEmissora)
                                                {
                                                    if (formulaRateio?.RatearEmBlocoDeEmissao ?? false)
                                                        cte.ValorAReceber = Math.Round(pedidoXMLNotasRemetentes.Sum(obj => obj.ValorFrete), 2, MidpointRounding.ToEven);
                                                    else
                                                        cte.ValorAReceber = pedidoXMLNotasRemetentes.Sum(obj => obj.ValorFrete);
                                                }
                                                else
                                                    cte.ValorAReceber = pedidoXMLNotasRemetentes.Sum(obj => obj.ValorFreteFilialEmissora);
                                            }

                                            cte.ValorFrete = cte.ValorAReceber;
                                            cte.ValorTotalPrestacaoServico = cte.ValorAReceber;
                                            cte.Serie = serie;
                                            //cte.ObservacoesGerais = observacoesGerais;
                                            cte.ProdutoPredominante = produtoPredominante;

                                            if (remetentePedido == null || destinatarioPedido == null)
                                            {
                                                if (tipoOperacaoNota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada) // esse caso é utilizado para notas de importação
                                                {
                                                    remetentePedido = repCliente.BuscarPorCPFCNPJ(destinatario);
                                                    destinatarioPedido = repCliente.BuscarPorCPFCNPJ(remetente);
                                                }
                                                else
                                                {
                                                    remetentePedido = repCliente.BuscarPorCPFCNPJ(remetente);
                                                    destinatarioPedido = repCliente.BuscarPorCPFCNPJ(destinatario);
                                                }
                                            }

                                            if (ciot != null)
                                                cte.CIOT = ciot.Numero;

                                            Dominio.Enumeradores.TipoServico tipoServico = Dominio.Enumeradores.TipoServico.Normal;

                                            if (primeiroCargaPedido.Redespacho)
                                            {
                                                string raizEmpresa = Utilidades.String.OnlyNumbers(primeiroCargaPedido.CargaPedidoTrechoAnterior.CargaOrigem.Empresa.CNPJ_SemFormato).Remove(8, 6);
                                                string raizTomador = "";

                                                if (primeiroCargaPedido.ObterTomador().Tipo == "J")
                                                    raizTomador = Utilidades.String.OnlyNumbers(primeiroCargaPedido.ObterTomador().CPF_CNPJ_SemFormato).Remove(8, 6);

                                                if (raizEmpresa == raizTomador)
                                                {
                                                    if (recebedor != null && expedidor != null)
                                                        tipoServico = Dominio.Enumeradores.TipoServico.RedIntermediario;
                                                    else
                                                        tipoServico = Dominio.Enumeradores.TipoServico.Redespacho;
                                                }
                                            }

                                            if (expedidor != null)
                                                expedidor = repCliente.BuscarPorCPFCNPJ(expedidor.CPF_CNPJ);

                                            if (recebedor != null)
                                                recebedor = repCliente.BuscarPorCPFCNPJ(recebedor.CPF_CNPJ);

                                            Dominio.Enumeradores.TipoPagamento tipoPagamentoNFe = (Dominio.Enumeradores.TipoPagamento)modalidade;

                                            if (comparaModalidadeNF)
                                            {
                                                if (tipoPagamentoNFe == Dominio.Enumeradores.TipoPagamento.Pago)
                                                    tipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
                                                else if (tipoPagamentoNFe == Dominio.Enumeradores.TipoPagamento.A_Pagar)
                                                    tipoTomador = Dominio.Enumeradores.TipoTomador.Destinatario;
                                                else if (tipoPagamentoNFe == Dominio.Enumeradores.TipoPagamento.Outros)
                                                    tipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
                                            }

                                            List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> seguros = serCte.ConverterSegurosDeTerceirosEmSeguroCTe(apolicesSeguro, tipoTomador, cte.ValorTotalMercadoria);
                                            List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ctesAnteriores = new List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe>();

                                            Dominio.Enumeradores.TipoCTE tipoCTe = Dominio.Enumeradores.TipoCTE.Normal;

                                            Dominio.Entidades.Empresa empresa = cargaOrigem.Empresa;

                                            if (emitirCteFilialEmissora)
                                            {
                                                if (primeiroCargaPedido.CargaPedidoProximoTrecho != null)
                                                    recebedor = null;

                                                if (primeiroCargaPedido.CargaPedidoTrechoAnterior != null)
                                                    expedidor = null;

                                                empresa = cargaOrigem.EmpresaFilialEmissora;
                                            }

                                            if (primeiroCargaPedido.Pedido.TipoOperacao != null && primeiroCargaPedido.Pedido.TipoOperacao.ColetaEmProdutorRural && primeiroCargaPedido.Pedido.TipoOperacao.RemetenteDoCTeSeraODestinatarioDoPedido)
                                                remetentePedido = repCliente.BuscarPorCPFCNPJ(primeiroCargaPedido.Pedido.Destinatario.CPF_CNPJ);

                                            string observacaoCTe = string.Join(", ", cargasPedidos.Select(c => c.Pedido.ObservacaoCTe).Distinct().ToList());
                                            string observacaoCTeTerceiro = string.Join(", ", cargasPedidos.Select(c => c.Pedido.ObservacaoCTeTerceiro).Distinct().ToList());

                                            List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil> configuracoes = Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil.ConverterPedidoXMLNotaFiscalContaContabilContabilizacao(pedidoXMLNotaFiscalFirst, pedidosXMLNotaFiscalContaContabilContabilizacao);
                                            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTE = serCte.GerarCTe(carga, primeiroCargaPedido, cte, empresa, remetentePedido, destinatarioPedido, tomadorUtilizar, expedidor, recebedor, remetentePedido.Localidade, enderecoDestinatario != null && enderecoDestinatario.Localidade != null ? enderecoDestinatario.Localidade : destinatarioPedido.Localidade, enderecoRemetente, enderecoDestinatario, tipoPagamentoNFe, tipoTomador, quantidades, cargaPedidoComponentesFretesCliente, observacaoCTe, observacaoCTeTerceiro, primeiroCargaPedido.Pedido, rotas, seguros, regraICMS, regraISS, impostoIBSCBS, tipoServicoMultisoftware, modeloNotaFiscalAgrupado, tipoServico, tipoCTe, ctesAnteriores, tipoEnvio, false, null, false, unitOfWork, configuracaoEmbarcador, configuracaoGeralCarga, pedidoXMLNotaFiscalFirst.CentroResultado, pedidoXMLNotaFiscalFirst.CentroResultadoDestinatario, pedidoXMLNotaFiscalFirst.ItemServico, pedidoXMLNotaFiscalFirst.CentroResultadoEscrituracao, pedidoXMLNotaFiscalFirst.CentroResultadoICMS, pedidoXMLNotaFiscalFirst.CentroResultadoPIS, pedidoXMLNotaFiscalFirst.CentroResultadoCOFINS, pedidoXMLNotaFiscalFirst.ValorMaximoCentroContabilizacao, configuracoes, pedidoXMLNotasRemetentes.Select(o => o.XMLNotaFiscal).ToList(), enderecoRecebedor, descricaoComponenteValorICMS, descricaoComponenteValorFrete, enderecoExpedidor, null, cargasPedidos);
                                            serCte.AverbaCargaCTe(cargaCTE, apolicesSeguro, unitOfWork, configuracaoEmbarcador, primeiroCargaPedido.Pedido.FormaAverbacaoCTE);

                                            if (ciot != null)
                                            {
                                                Dominio.Entidades.Embarcador.Documentos.CIOTCTe ciotCTe = new Dominio.Entidades.Embarcador.Documentos.CIOTCTe()
                                                {
                                                    CargaCTe = cargaCTE,
                                                    CIOT = ciot
                                                };

                                                repCIOTCTe.Inserir(ciotCTe);
                                            }

                                            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotasRemetentes)
                                            {
                                                Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTE = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe();

                                                cargaPedidoXMLNotaFiscalCTE.CargaCTe = cargaCTE;
                                                cargaPedidoXMLNotaFiscalCTE.PedidoXMLNotaFiscal = pedidoXMLNotaFiscal;

                                                repCargaPedidoXMLNotaFiscalCTe.Inserir(cargaPedidoXMLNotaFiscalCTE);
                                            }

                                            if (possuiNFSManual)
                                            {
                                                serNFS.CriarCargaCTePendenteEmissaoManualDeNFS(carga, cargaCTE, localidadePrestacao, unitOfWork);

                                                carga.AgNFSManual = true;
                                            }

                                            unitOfWork.CommitChanges();

                                            if (cargaCTE.CTe != null && cargaCTE.CTe.Status == "E")
                                            {
                                                if (cargaCTE.CTe.ModeloDocumentoFiscal.Numero == "57")
                                                {
                                                    Servicos.CTe servicoCte = new Servicos.CTe(unitOfWork);
                                                    servicoCte.Emitir(cargaCTE.CTe.Codigo, 0, unitOfWork);
                                                }
                                                else if (cargaCTE.CTe.ModeloDocumentoFiscal.Numero == "39")
                                                {
                                                    Servicos.NFSe servicoNFSe = new Servicos.NFSe(unitOfWork);
                                                    servicoNFSe.EmitirNFSe(cargaCTE.CTe.Codigo, unitOfWork);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (pedidoXMLNotasRemetentes.Any(o => o.DocsParaEmissaoNFSManual.Count > 0))
                                                continue;

                                            carga.AgNFSManual = true;

                                            unitOfWork.Start();

                                            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotasRemetentes)
                                            {
                                                Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual cargaDocumentoParaEmissaoNFSManual = serNFS.CriarNFPendenteEmissaoManualDeNFS(carga, pedidoXMLNotaFiscal, pedidoXMLNotaFiscal.CargaPedido.ObterTomador(), localidadePrestacao, unitOfWork);

                                                serNFS.AverbaCargaNFe(cargaDocumentoParaEmissaoNFSManual, apolicesSeguro, unitOfWork, configuracaoEmbarcador, pedidoXMLNotaFiscal.CargaPedido.Pedido.FormaAverbacaoCTE);
                                            }

                                            unitOfWork.CommitChanges();
                                        }

                                        if (totalDocumentosGerados == 1 || (totalDocumentosGerados % quantidadeDocumentosAtualizarCarga) == 0)
                                            svcHubCarga.InformarQuantidadeDocumentosEmitidos(carga.Codigo, totalDocumentos, totalDocumentosGerados);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Servicos.Embarcador.CTe.CTEsImportados serCTEsImportados = new Servicos.Embarcador.CTe.CTEsImportados(unitOfWork);
                foreach (var cargaPedido in cargasPedidos)
                {
                    serCTEsImportados.VincularCargaCTeEmitidoAnteriormente(carga, cargaPedido, unitOfWork);
                }
            }


            svcHubCarga.InformarQuantidadeDocumentosEmitidos(carga.Codigo, totalDocumentos, totalDocumentosGerados);
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS BuscarRegraICMS(Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal, bool emitirCteFilialEmissora, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal)
        {
            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS();

            if (!emitirCteFilialEmissora)
            {
                if (modeloDocumentoFiscal == null || modeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe || modeloDocumentoFiscal.CalcularImpostos)
                {
                    if (pedidoXMLNotaFiscal.CFOP != null)
                    {
                        regraICMS.CFOP = pedidoXMLNotaFiscal.CFOP?.CodigoCFOP ?? 0;
                        regraICMS.CST = pedidoXMLNotaFiscal.CST;
                    }
                }
                regraICMS.AliquotaPis = pedidoXMLNotaFiscal.AliquotaPis;
                regraICMS.AliquotaCofins = pedidoXMLNotaFiscal.AliquotaCofins;
                regraICMS.ValorPis = pedidoXMLNotaFiscal.ValorPis;
                regraICMS.ValorCofins = pedidoXMLNotaFiscal.ValorCofins;
                regraICMS.Aliquota = pedidoXMLNotaFiscal.PercentualAliquota;
                regraICMS.AliquotaInternaDifal = pedidoXMLNotaFiscal.PercentualAliquotaInternaDifal;
                regraICMS.PercentualReducaoBC = pedidoXMLNotaFiscal.PercentualReducaoBC;
                regraICMS.ObservacaoCTe = pedidoXMLNotaFiscal.ObservacaoRegraICMSCTe;
                regraICMS.PercentualInclusaoBC = pedidoXMLNotaFiscal.PercentualIncluirBaseCalculo;
                regraICMS.IncluirICMSBC = pedidoXMLNotaFiscal.IncluirICMSBaseCalculo;
                regraICMS.DescontarICMSDoValorAReceber = pedidoXMLNotaFiscal.DescontarICMSDoValorAReceber;
                regraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao = pedidoXMLNotaFiscal.NaoReduzirRetencaoICMSDoValorDaPrestacao;
                regraICMS.NaoImprimirImpostosDACTE = pedidoXMLNotaFiscal.NaoImprimirImpostosDACTE;
                regraICMS.NaoEnviarImpostoICMSNaEmissaoCte = pedidoXMLNotaFiscal.NaoEnviarImpostoICMSNaEmissaoCte;
                regraICMS.CodigoRegra = pedidoXMLNotaFiscal.RegraICMS?.Codigo ?? 0;
            }
            else
            {
                if (pedidoXMLNotaFiscal.ModeloDocumentoFiscal == null || pedidoXMLNotaFiscal.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                {
                    regraICMS.CFOP = pedidoXMLNotaFiscal.CFOPFilialEmissora?.CodigoCFOP ?? 0;
                    regraICMS.CST = pedidoXMLNotaFiscal.CSTFilialEmissora;
                }

                regraICMS.Aliquota = pedidoXMLNotaFiscal.PercentualAliquotaFilialEmissora;
                regraICMS.AliquotaInternaDifal = pedidoXMLNotaFiscal.PercentualAliquotaFilialEmissoraInternaDifal;
                regraICMS.PercentualReducaoBC = pedidoXMLNotaFiscal.PercentualReducaoBCFilialEmissora;
                regraICMS.ObservacaoCTe = pedidoXMLNotaFiscal.ObservacaoRegraICMSCTeFilialEmissora;
                regraICMS.PercentualInclusaoBC = pedidoXMLNotaFiscal.PercentualIncluirBaseCalculoFilialEmissora;
                regraICMS.IncluirICMSBC = pedidoXMLNotaFiscal.IncluirICMSBaseCalculoFilialEmissora;
                regraICMS.DescontarICMSDoValorAReceber = pedidoXMLNotaFiscal.DescontarICMSDoValorAReceber;
                regraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao = pedidoXMLNotaFiscal.NaoReduzirRetencaoICMSDoValorDaPrestacao;
                regraICMS.NaoImprimirImpostosDACTE = pedidoXMLNotaFiscal.NaoImprimirImpostosDACTE;
                regraICMS.NaoEnviarImpostoICMSNaEmissaoCte = pedidoXMLNotaFiscal.NaoEnviarImpostoICMSNaEmissaoCte;
            }
            return regraICMS;
        }

        private Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS BuscarRegraISS(Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal)
        {
            Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = new Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS();
            regraISS.AliquotaISS = pedidoXMLNotaFiscal.PercentualAliquotaISS;
            regraISS.IncluirISSBaseCalculo = pedidoXMLNotaFiscal.IncluirISSBaseCalculo;
            regraISS.PercentualRetencaoISS = pedidoXMLNotaFiscal.PercentualRetencaoISS;

            regraISS.ReterIR = pedidoXMLNotaFiscal.ReterIR;
            regraISS.AliquotaIR = pedidoXMLNotaFiscal.AliquotaIR;
            regraISS.BaseCalculoIR = pedidoXMLNotaFiscal.BaseCalculoIR;
            regraISS.ValorIR = pedidoXMLNotaFiscal.ValorIR;

            return regraISS;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> BuscarXMLPedidoComponentesFrete(Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal, bool componenteFilialEmissora, Repositorio.UnitOfWork unitOfWork)
        {
            //todo: Os componentes atualmente são montados por tipo no CT-e ver regra, se necessário para agrupar por componente de Frete (apenas exibição)
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> cargaPedidoComponentesFretesCliente = new List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico>();
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> pedidoXmlNFCompontentesFrete = repPedidoXMLNotaFiscalComponenteFrete.BuscarPorXMLnotaFiscalComPisCofins(pedidoXMLNotaFiscal.Codigo, pedidoXMLNotaFiscal.ModeloDocumentoFiscal, componenteFilialEmissora);

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete pedidoXmlNFCompontenteFrete in pedidoXmlNFCompontentesFrete)
            {
                int index = cargaPedidoComponentesFretesCliente.FindIndex(obj => obj.TipoComponenteFrete == pedidoXmlNFCompontenteFrete.TipoComponenteFrete && pedidoXmlNFCompontenteFrete.ComponenteFrete == obj.ComponenteFrete);

                if (index != -1)
                {
                    Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico componenteFrete = cargaPedidoComponentesFretesCliente[index];

                    componenteFrete.ValorComponente += pedidoXmlNFCompontenteFrete.ValorComponente;
                    componenteFrete.ValorTotalMoeda += pedidoXmlNFCompontenteFrete.ValorTotalMoeda ?? 0m;
                    componenteFrete.ValorComponenteComICMSIncluso += pedidoXmlNFCompontenteFrete.ValorComponenteComICMSIncluso;

                    cargaPedidoComponentesFretesCliente[index] = componenteFrete;
                }
                else
                {
                    cargaPedidoComponentesFretesCliente.Add(pedidoXmlNFCompontenteFrete.ConvertarParaComponenteDinamico());
                }
            }

            return cargaPedidoComponentesFretesCliente;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> BuscarXMLPedidoComponentesFreteAgrupados(List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais, bool componenteFilialEmissora, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> cargaPedidoComponentesFretesCliente = new List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico>();
            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotasFiscais)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> pedidoXmlNFCompontentesFrete = repPedidoXMLNotaFiscalComponenteFrete.BuscarPorXMLnotaFiscalComPisCofins(pedidoXMLNotaFiscal.Codigo, pedidoXMLNotaFiscal.ModeloDocumentoFiscal, componenteFilialEmissora);
                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete pedidoXmlNFCompontenteFrete in pedidoXmlNFCompontentesFrete)
                {
                    int index = cargaPedidoComponentesFretesCliente.FindIndex(obj => obj.TipoComponenteFrete == pedidoXmlNFCompontenteFrete.TipoComponenteFrete && pedidoXmlNFCompontenteFrete.ComponenteFrete == obj.ComponenteFrete);

                    if (index != -1)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico componenteFrete = cargaPedidoComponentesFretesCliente[index];

                        componenteFrete.ValorComponente += pedidoXmlNFCompontenteFrete.ValorComponente;
                        componenteFrete.ValorTotalMoeda += pedidoXmlNFCompontenteFrete.ValorTotalMoeda ?? 0m;
                        componenteFrete.ValorComponenteComICMSIncluso += pedidoXmlNFCompontenteFrete.ValorComponenteComICMSIncluso;

                        cargaPedidoComponentesFretesCliente[index] = componenteFrete;
                    }
                    else
                    {
                        cargaPedidoComponentesFretesCliente.Add(pedidoXmlNFCompontenteFrete.ConvertarParaComponenteDinamico());
                    }
                }
            }
            return cargaPedidoComponentesFretesCliente;
        }

        #endregion
    }
}
