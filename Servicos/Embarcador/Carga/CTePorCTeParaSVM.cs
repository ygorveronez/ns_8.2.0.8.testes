using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Servicos.Embarcador.Carga
{
    public class CTePorCTeParaSVM : ServicoBase
    {
        public CTePorCTeParaSVM(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        #region Métodos Públicos

        public void GerarCTePorCTe(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, int totalDocumentos, int quantidadeDocumentosAtualizarCarga, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao> pedidosCTeParaSubContratacaoContaContabilContabilizacao, ref int totalDocumentosGerados)
        {
            CTe serCte = new CTe(unitOfWork);
            CTeSubContratacaoEmissao serCTeSubContratacaoEmissao = new CTeSubContratacaoEmissao(unitOfWork);
            Servicos.Embarcador.Carga.NFS serNFS = new NFS(unitOfWork);

            Repositorio.Embarcador.CTe.CTeTerceiroQuantidade repCTeParaSubContratacaoQuantidade = new Repositorio.Embarcador.CTe.CTeTerceiroQuantidade(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroSeguro repCTeParaSubContratacaoSeguro = new Repositorio.Embarcador.CTe.CTeTerceiroSeguro(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repPedidoCTeParaSubContratacaoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.CTe.CTeDocumentoOriginario repDocumentoOriginario = new Repositorio.Embarcador.CTe.CTeDocumentoOriginario(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            CTePorPedido serCTePorPedido = new CTePorPedido(unitOfWork);
            Hubs.Carga svcHubCarga = new Hubs.Carga();

            for (int i = 0; i < cargaPedidos.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigoComFetchEmpresa(cargaPedidos[i].Codigo);

                if (!cargaPedido.PossuiCTe && !cargaPedido.PossuiNFS && !cargaPedido.PossuiNFSManual && !(cargaPedido.PossuiNFSManual && cargaPedido.ModeloDocumentoFiscalIntramunicipal != null))
                    continue;

                bool emitiu = false;

                if (!cargaPedido.CTeEmitidoNoEmbarcador && !cargaPedido.Pedido.PedidoTransbordo)
                {
                    serCte.ObterDescricoesComponentesPadrao(cargaPedido.ObterTomador(), carga, configuracaoEmbarcador, unitOfWork, out string descricaoComponenteValorFrete, out string descricaoComponenteValorICMS);

                    List<int> pedidoCTesParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarCodigosPorCargaPedido(cargaPedido.Codigo);

                    foreach (int codigoPedidoCTeParaSubContratacao in pedidoCTesParaSubContratacao)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarPorCodigoComFetch(codigoPedidoCTeParaSubContratacao);

                        Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalCarga = pedidoCTeParaSubContratacao.ModeloDocumentoFiscal;

                        if (modeloDocumentoFiscalCarga == null)
                            modeloDocumentoFiscalCarga = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.CTe);

                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> pedidoCTeParaSubContratacaoNotasFiscais = repPedidoCTeParaSubContratacaoNotaFiscal.BuscarPorPedidoCTeParaSubcontratacao(pedidoCTeParaSubContratacao.Codigo);
                        Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorCodigo(pedidoCTeParaSubContratacaoNotasFiscais.First().PedidoXMLNotaFiscal.Codigo);

                        Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteParaSubcontratacao = pedidoCTeParaSubContratacao.CTeTerceiro;

                        Dominio.Entidades.Cliente expedidor = null;
                        Dominio.Entidades.Cliente recebedor = null;
                        Dominio.Entidades.Localidade inicioPrestacao = cteParaSubcontratacao.LocalidadeInicioPrestacao;
                        Dominio.Entidades.Localidade terminoPrestacao = cteParaSubcontratacao.LocalidadeTerminoPrestacao;
                        Dominio.Entidades.Cliente remetente = repCliente.BuscarPorCPFCNPJ((cteParaSubcontratacao.Remetente.Cliente.CPF_CNPJ));
                        Dominio.Entidades.Cliente destinatario = repCliente.BuscarPorCPFCNPJ((cteParaSubcontratacao.Destinatario.Cliente.CPF_CNPJ));

                        //if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        //{
                        if (pedidoCTeParaSubContratacao.CargaPedido.Expedidor != null)
                        {
                            expedidor = pedidoCTeParaSubContratacao.CargaPedido.Expedidor;
                            if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
                                inicioPrestacao = expedidor.Localidade;
                        }

                        if (pedidoCTeParaSubContratacao.CargaPedido.Recebedor != null)
                        {
                            recebedor = pedidoCTeParaSubContratacao.CargaPedido.Recebedor;
                            if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
                                terminoPrestacao = recebedor.Localidade;
                        }
                        //}

                        if (!pedidoCTeParaSubContratacao.PossuiNFSManual ||
                            (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS &&
                            (inicioPrestacao.Codigo != terminoPrestacao.Codigo || (pedidoCTeParaSubContratacao.PossuiNFSManual && cargaPedido.ModeloDocumentoFiscalIntramunicipal != null && inicioPrestacao.Codigo == terminoPrestacao.Codigo))))
                        {
                            bool somenteCTeSubContratacaoFilialEmissora = false;

                            carga = repCarga.BuscarPorCodigo(carga.Codigo);

                            if (cargaPedido.CargaPedidoFilialEmissora && carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                                somenteCTeSubContratacaoFilialEmissora = true;

                            //Adicionado a validação do código da carga, pois estão gerando dois SVM's com as mesmas notas fiscais (em caso de reiniciar a fila, pode gerar dois CT-es iguais)
                            if (repCargaPedidoXMLNotaFiscalCTe.ContarCTesPorPedidoCTeParaSubContratacao(pedidoCTeParaSubContratacao.Codigo, somenteCTeSubContratacaoFilialEmissora) > 0)
                                continue;

                            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoEmitir = null;

                            if (pedidoCTeParaSubContratacao.PossuiNFSManual)
                                modeloDocumentoEmitir = cargaPedido.ModeloDocumentoFiscalIntramunicipal;
                            else
                                modeloDocumentoEmitir = modeloDocumentoFiscalCarga;

                            unitOfWork.Start();

                            emitiu = true;
                            totalDocumentosGerados++;
                            Dominio.ObjetosDeValor.CTe.CTe cte = new Dominio.ObjetosDeValor.CTe.CTe();
                            cte.Documentos = new List<Dominio.ObjetosDeValor.CTe.Documento>();

                            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoNotaFiscal in pedidoCTeParaSubContratacaoNotasFiscais)
                            {
                                Dominio.ObjetosDeValor.CTe.Documento docNF = serCte.BuscarDocumentoCTe(pedidoCTeParaSubContratacaoNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal, carga.Empresa.TipoAmbiente, configuracaoEmbarcador);
                                cte.Documentos.Add(docNF);
                            }

                            cte.PercentualPagamentoAgregado = pedidoCTeParaSubContratacao.PercentualPagamentoAgregado;

                            List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> cargaPedidoComponentesFretesCliente = BuscarPedidoCTeSubcontratacaoComponentesFrete(pedidoCTeParaSubContratacao, modeloDocumentoFiscalCarga, unitOfWork);

                            cte.ValorTotalMercadoria = cteParaSubcontratacao.ValorTotalMercadoria;

                            decimal valorFreteNegociado = repPedidoCTeParaSubContratacao.ObterValorFreteNegociavelPedido(pedidoCTeParaSubContratacao.CTeTerceiro.ChaveAcesso);
                            decimal valorCusteioSVM = repPedidoCTeParaSubContratacao.ObterValorCusteioSVMPedido(pedidoCTeParaSubContratacao.CTeTerceiro.ChaveAcesso);
                            decimal valorICMS = pedidoCTeParaSubContratacao.CTeTerceiro.ValorICMS;
                            decimal aliquota = repPedidoCTeParaSubContratacao.ObterAliquota(pedidoCTeParaSubContratacao.CTeTerceiro.ChaveAcesso);//pedidoCTeParaSubContratacao.CTeTerceiro.AliquotaICMS;

                            cargaPedido.Pedido.NumeroBooking = repPedidoCTeParaSubContratacao.ObterNumeroBookingPedido(pedidoCTeParaSubContratacao.CTeTerceiro.ChaveAcesso);
                            cargaPedido.Pedido.DescricaoCarrierNavioViagem = repPedidoCTeParaSubContratacao.ObterDescricaoCarrierPedido(pedidoCTeParaSubContratacao.CTeTerceiro.ChaveAcesso);
                            cargaPedido.Pedido.TipoPropostaFeeder = repPedidoCTeParaSubContratacao.ObterTipoPropostaFeederPedido(pedidoCTeParaSubContratacao.CTeTerceiro.ChaveAcesso);
                            cargaPedido.Pedido.Container = repPedidoCTeParaSubContratacao.ObterContainerPedido(pedidoCTeParaSubContratacao.CTeTerceiro.ChaveAcesso);
                            cargaPedido.Pedido.LacreContainerUm = repPedidoCTeParaSubContratacao.ObterLacrePedido(pedidoCTeParaSubContratacao.CTeTerceiro.ChaveAcesso, 1);
                            cargaPedido.Pedido.LacreContainerDois = repPedidoCTeParaSubContratacao.ObterLacrePedido(pedidoCTeParaSubContratacao.CTeTerceiro.ChaveAcesso, 2);
                            cargaPedido.Pedido.LacreContainerTres = repPedidoCTeParaSubContratacao.ObterLacrePedido(pedidoCTeParaSubContratacao.CTeTerceiro.ChaveAcesso, 3);

                            Repositorio.Embarcador.Pedidos.PedidoTransbordo repPedidoTransbordo = new Repositorio.Embarcador.Pedidos.PedidoTransbordo(unitOfWork);
                            List<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo> transbordosAnteriores = repPedidoTransbordo.BuscarPorPedido(cargaPedido.Pedido.Codigo);
                            List<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo> transbordos = repPedidoCTeParaSubContratacao.ObterTransbordosPedido(pedidoCTeParaSubContratacao.CTeTerceiro.ChaveAcesso);
                            if (transbordos != null && transbordos.Count > 0)
                            {
                                if (transbordosAnteriores != null && transbordosAnteriores.Count > 0)
                                {
                                    foreach (var transbordo in transbordosAnteriores)
                                    {
                                        repPedidoTransbordo.Deletar(transbordo);
                                    }
                                }
                                foreach (var transbordo in transbordos)
                                {
                                    Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo trans = new Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo()
                                    {
                                        Navio = transbordo.Navio,
                                        Pedido = cargaPedido.Pedido,
                                        PedidoViagemNavio = transbordo.PedidoViagemNavio,
                                        Porto = transbordo.Porto,
                                        Sequencia = transbordo.Sequencia,
                                        Terminal = transbordo.Terminal
                                    };
                                    repPedidoTransbordo.Inserir(trans);
                                }
                            }

                            repPedido.Atualizar(cargaPedido.Pedido);


                            int qtdContainer = 1;
                            decimal valorFrete = 1;
                            decimal baseICMS = ((100 - aliquota) / 100);
                            List<string> listaChave = new List<string>();
                            listaChave.Add(pedidoCTeParaSubContratacao.CTeTerceiro.ChaveAcesso);
                            List<int> codigosContainer = new List<int>();
                            codigosContainer.Add(cargaPedido.Pedido.Container.Codigo);

                            valorFrete = CalcularValorSVM(cargaPedido.TipoPropostaMultimodal, pedidoCTeParaSubContratacao.CTeTerceiro.ChaveAcesso, listaChave,
                                1, valorCusteioSVM, valorFreteNegociado, baseICMS, "", cargaPedido.Pedido.NumeroBooking, unitOfWork, qtdContainer, codigosContainer);

                            cte.ValorAReceber = valorFrete;
                            cte.ValorFrete = valorFrete;
                            cte.ValorTotalPrestacaoServico = valorFrete;

                            if (cargaPedido.Pedido.EmpresaSerie != null)
                                cte.Serie = cargaPedido.Pedido.EmpresaSerie.Numero;

                            //if (!string.IsNullOrWhiteSpace(cargaPedido.Pedido.ObservacaoCTe))
                            //    cte.ObservacoesGerais = cargaPedido.Pedido.ObservacaoCTe;

                            cte.ProdutoPredominante = cteParaSubcontratacao.ProdutoPredominante;

                            List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade> quantidades = repCTeParaSubContratacaoQuantidade.BuscarPorCTeParaSubContratacao(cteParaSubcontratacao.Codigo);
                            List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroSeguro> seguros = repCTeParaSubContratacaoSeguro.BuscarPorCTeParaSubContratacao(cteParaSubcontratacao.Codigo);

                            Dominio.Entidades.Cliente tomador = cteParaSubcontratacao.Emitente?.Cliente;

                            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && modeloDocumentoEmitir != null && modeloDocumentoEmitir.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe && cargaPedido.Tomador != null)
                                tomador = cargaPedido.Tomador;

                            if (tomador == null)
                                tomador = cargaPedido.Pedido.SubContratante != null ? cargaPedido.Pedido.SubContratante : cargaPedido.Tomador;

                            Dominio.Enumeradores.TipoPagamento tipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros; //cargaPedido.Pedido.TipoPagamento; //Dominio.Enumeradores.TipoPagamento.Outros;//cargaPedido.Pedido.TipoPagamento;
                            Dominio.Enumeradores.TipoTomador tipoTomador = Dominio.Enumeradores.TipoTomador.Outros; //cargaPedido.TipoTomador; //Dominio.Enumeradores.TipoTomador.Outros;
                            List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesParaSubcontrataca = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>();
                            if (!ctesParaSubcontrataca.Contains(cteParaSubcontratacao))
                                ctesParaSubcontrataca.Add(cteParaSubcontratacao);

                            List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidadesCTes = ConverterQuantidadesTerceirosParaQuantidadesCTe(quantidades);

                            bool emitindoCTeFilialEmissora = false;
                            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeFilialEmissora = pedidoCTeParaSubContratacao.CTeTerceiro.ObterCargaCTe(carga.Codigo);

                            if (cargaPedido.CargaPedidoFilialEmissora)
                                emitindoCTeFilialEmissora = true;

                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga tipoContratacaoCarga = cargaPedido.TipoContratacaoCarga;

                            if (emitindoCTeFilialEmissora)
                                tipoContratacaoCarga = cargaPedido.TipoContratacaoCargaSubContratacaoFilialEmissora;

                            Dominio.Enumeradores.TipoServico tipoServico = Dominio.Enumeradores.TipoServico.SubContratacao;

                            if (carga.CargaSVM)
                                tipoServico = Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal;
                            else if (tipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho)
                                tipoServico = Dominio.Enumeradores.TipoServico.Redespacho;
                            else if (tipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario)
                                tipoServico = Dominio.Enumeradores.TipoServico.RedIntermediario;

                            List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ctesAnteriores = ConverterCTesTerceirosParaAnteriores(ctesParaSubcontrataca);

                            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = BuscarRegraICMS(cargaPedido.CargaOrigem, inicioPrestacao, terminoPrestacao, pedidoCTeParaSubContratacao, unitOfWork);
                            Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = BuscarRegraISS(cargaPedido.CargaOrigem, pedidoCTeParaSubContratacao, unitOfWork);
                            Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS();

                            cargaPedido = repCargaPedido.BuscarPorCodigo(cargaPedido.Codigo);

                            cargaPedido.ValorFrete = valorFrete;
                            cargaPedido.ValorFreteAPagar = valorFrete;
                            cargaPedido.ValorICMS = 0;
                            cargaPedido.PercentualAliquota = 0;
                            cargaPedido.BaseCalculoICMS = 0;
                            cargaPedido.CST = "41";
                            cargaPedido.CFOP = repCFOP.BuscarPorNumero(regraICMS.CFOP);

                            repCargaPedido.Atualizar(cargaPedido);

                            List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> apoliceSeguro = ConverterSegurosDeTerceirosEmSeguroCTe(seguros);

                            string observacaoCTe = null;

                            if (!string.IsNullOrWhiteSpace(pedidoXMLNotaFiscal.CargaPedido.Pedido.ObservacaoCTe)) //essa informação por hora só existe no CT-e para subcontratação, pois não emitimos modal aéreo
                                observacaoCTe = pedidoXMLNotaFiscal.CargaPedido.Pedido.ObservacaoCTe.Replace("#NumeroOCADocumentoTransporteAnterior", cteParaSubcontratacao.NumeroOperacionalConhecimentoAereo?.ToString() ?? string.Empty);

                            Dominio.Enumeradores.TipoCTE tipoCTe = Dominio.Enumeradores.TipoCTE.Normal;

                            Dominio.Entidades.Empresa empresa = cargaPedido.CargaOrigem.Empresa;
                            if (carga.EmitirCTeComplementar)
                            {
                                if (cteParaSubcontratacao.Emitente.CPF_CNPJ_SemFormato != empresa.CNPJ_SemFormato)
                                    empresa = repEmpresa.BuscarPorCNPJ(cteParaSubcontratacao.Emitente.CPF_CNPJ_SemFormato);

                                cte.ChaveCTESubstituicaoComplementar = cteParaSubcontratacao.ChaveAcesso;
                                cte.TipoCTe = Dominio.Enumeradores.TipoCTE.Complemento;
                                cte.TipoServico = cteParaSubcontratacao.TipoServico;
                                tomador = cteParaSubcontratacao.Tomador.Cliente;
                                tipoTomador = cteParaSubcontratacao.TipoTomador;
                                tipoCTe = Dominio.Enumeradores.TipoCTE.Complemento;
                                tipoServico = cteParaSubcontratacao.TipoServico;
                            }

                            List<string> chavesCTes = new List<string>();
                            if (ctesAnteriores != null && ctesAnteriores.Count > 0)
                                chavesCTes = ctesAnteriores.Select(o => o.Chave).ToList();
                            List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil> configuracoes = Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil.ConverterPedidoCTeParaSubContratacaoContaContabilContabilizacao(pedidoCTeParaSubContratacao, pedidosCTeParaSubContratacaoContaContabilContabilizacao);
                            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTE = serCte.GerarCTe(carga, cargaPedido, cte, empresa, remetente, destinatario, tomador, expedidor, recebedor, inicioPrestacao, terminoPrestacao, null, null, tipoPagamento, tipoTomador, quantidadesCTes, cargaPedidoComponentesFretesCliente, observacaoCTe, pedidoXMLNotaFiscal.CargaPedido.Pedido.ObservacaoCTeTerceiro, pedidoXMLNotaFiscal.CargaPedido.Pedido, null, apoliceSeguro, regraICMS, regraISS, impostoIBSCBS, tipoServicoMultisoftware, modeloDocumentoEmitir, tipoServico, tipoCTe, ctesAnteriores, 0, false, cargaCTeFilialEmissora, false, unitOfWork, configuracaoEmbarcador, configuracaoGeralCarga, pedidoCTeParaSubContratacao.CentroResultado, pedidoCTeParaSubContratacao.CentroResultadoDestinatario, pedidoCTeParaSubContratacao.ItemServico, pedidoCTeParaSubContratacao.CentroResultadoEscrituracao, pedidoCTeParaSubContratacao.CentroResultadoICMS, pedidoCTeParaSubContratacao.CentroResultadoPIS, pedidoCTeParaSubContratacao.CentroResultadoCOFINS, pedidoCTeParaSubContratacao.ValorMaximoCentroContabilizacao, configuracoes, pedidoCTeParaSubContratacaoNotasFiscais.Select(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal).ToList(), null, descricaoComponenteValorICMS, descricaoComponenteValorFrete, null, null, null, 0, chavesCTes, null, null, null, null, true);
                            SalvarNumeroControleSVM(cargaPedido.Codigo, cargaCTE.CTe?.NumeroControle ?? "", unitOfWork, cargaCTE.CTe?.Codigo ?? 0);

                            if (emitindoCTeFilialEmissora && cargaCTeFilialEmissora != null)
                            {
                                Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                                cargaCTeFilialEmissora.CargaCTeSubContratacaoFilialEmissora = cargaCTE;
                                repCargaCte.Atualizar(cargaCTeFilialEmissora);
                            }
                            List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> apoliceSeguroAverbacaos = repApoliceSeguroAverbacao.BuscarPorCargaPedido(cargaPedido.Codigo, emitindoCTeFilialEmissora);
                            serCte.AverbaCargaCTe(cargaCTE, apoliceSeguroAverbacaos, unitOfWork, configuracaoEmbarcador, cargaPedido.Pedido.FormaAverbacaoCTE);
                            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoNotaFiscal in pedidoCTeParaSubContratacaoNotasFiscais)
                            {
                                Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTE = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe();
                                cargaPedidoXMLNotaFiscalCTE.CargaCTe = cargaCTE;
                                cargaPedidoXMLNotaFiscalCTE.PedidoXMLNotaFiscal = pedidoCTeParaSubContratacaoNotaFiscal.PedidoXMLNotaFiscal;
                                repCargaPedidoXMLNotaFiscalCTe.Inserir(cargaPedidoXMLNotaFiscalCTE);
                            }

                            bool inseriuOBS = false;
                            if (cargaCTE.CTe != null)
                            {
                                Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario documentoOriginario = new Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario()
                                {
                                    CTe = cargaCTE.CTe,
                                    Chave = cteParaSubcontratacao.ChaveAcesso,
                                    Numero = cteParaSubcontratacao.Numero,
                                    Serie = cteParaSubcontratacao.Serie,
                                    DataEmissao = cteParaSubcontratacao.DataEmissao,
                                    NumeroMinuta = cteParaSubcontratacao.NumeroMinuta,
                                    NumeroOperacionalConhecimentoAereo = cteParaSubcontratacao.NumeroOperacionalConhecimentoAereo
                                };

                                repDocumentoOriginario.Inserir(documentoOriginario);
                                if (!inseriuOBS && cargaCTE.CTe.SVMProprio && cargaCTE.CTe.PortoOrigem != null && cargaCTE.CTe.PortoOrigem != null && cargaCTE.CTe.PortoOrigem.AtivarDespachanteComoConsignatario && cargaCTE.CTe.PortoDestino.AtivarDespachanteComoConsignatario)
                                {
                                    string chaveCTeMultimodal = documentoOriginario.Chave;
                                    if (!string.IsNullOrWhiteSpace(chaveCTeMultimodal))
                                    {
                                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cteMultimodal = repCTe.BuscarPorChave(chaveCTeMultimodal);
                                        if (cteMultimodal != null && cteMultimodal.TomadorPagador != null && cteMultimodal.TomadorPagador.GrupoPessoas != null && cteMultimodal.TomadorPagador.GrupoPessoas.AdicionarDespachanteComoConsignatario && cteMultimodal.TomadorPagador.GrupoPessoas.Despachante != null)
                                        {
                                            cargaCTE.CTe.ObservacoesGerais += " A " + cteMultimodal.TomadorPagador.GrupoPessoas.Despachante.Nome + ", CNPJ " + cteMultimodal.TomadorPagador.GrupoPessoas.Despachante.CPF_CNPJ_Formatado + ", irá atuar como despachante para a liberação dessa carga no porto de descarga.";
                                            cargaCTE.CTe.ObservacoesGerais = cargaCTE.CTe.ObservacoesGerais.Trim();
                                            cargaCTE.CTe.EmailDespachanteSVM = cteMultimodal.TomadorPagador.GrupoPessoas.EmailDespachante;
                                            repCTe.Atualizar(cargaCTE.CTe);
                                        }
                                    }
                                    inseriuOBS = true;
                                }
                            }

                            if (pedidoCTeParaSubContratacao.PossuiNFSManual)
                            {
                                serNFS.CriarCargaCTePendenteEmissaoManualDeNFS(carga, cargaCTE, inicioPrestacao, unitOfWork);

                                carga.AgNFSManual = true;
                            }

                            unitOfWork.CommitChanges();
                            if (cte != null && cte.DocumentosTransporteAnteriores != null && cargaCTE.CTe.TipoServico == Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal && cargaCTE.CTe.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario)
                            {
                                Repositorio.DocumentoDeTransporteAnteriorCTe repDocumento = new Repositorio.DocumentoDeTransporteAnteriorCTe(unitOfWork);
                                Servicos.Log.TratarErro(cargaCTE.CTe.Codigo.ToString() + " - Iniciou SalvarInformacoesDocumentosAnteriores SVM - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "TempoEmissaoCTe");
                                foreach (var doc in cte.DocumentosTransporteAnteriores)
                                {
                                    unitOfWork.Start();

                                    Dominio.Entidades.DocumentoDeTransporteAnteriorCTe documento = new Dominio.Entidades.DocumentoDeTransporteAnteriorCTe();

                                    documento.Chave = doc.Chave;
                                    documento.CTe = cargaCTE.CTe;

                                    DateTime dataEmissao;
                                    DateTime.TryParseExact(doc.DataEmissao, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
                                    double cpfCnpj = 0;
                                    if (doc.Emissor != null)
                                        double.TryParse(Utilidades.String.OnlyNumbers(doc.Emissor.CPFCNPJ), out cpfCnpj);

                                    if (dataEmissao != DateTime.MinValue)
                                        documento.DataEmissao = dataEmissao;

                                    documento.Emissor = cpfCnpj > 0 ? repCliente.BuscarPorCPFCNPJ(cpfCnpj) : null;
                                    documento.Numero = doc.Numero;
                                    documento.Serie = doc.Serie;
                                    documento.Tipo = doc.Tipo;

                                    repDocumento.Inserir(documento);

                                    unitOfWork.CommitChanges();

                                    unitOfWork.FlushAndClear();
                                }
                            }
                            cargaCTE = repCargaCTe.BuscarPorCodigo(cargaCTE.Codigo);
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
                            if (cargaPedido.CargaPedidoFilialEmissora || (cargaPedido.CargaPedidoTrechoAnterior?.CargaPedidoFilialEmissora ?? false))
                            {
                                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoPedidoNotaFiscal in pedidoCTeParaSubContratacaoNotasFiscais)
                                {
                                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscalNFS = pedidoCTeParaSubContratacaoPedidoNotaFiscal.PedidoXMLNotaFiscal;
                                    if (pedidoXMLNotaFiscalNFS.DocsParaEmissaoNFSManual.Count > 0)
                                        continue;

                                    unitOfWork.Start();
                                    carga.AgNFSManual = true;

                                    serNFS.CriarNFPendenteEmissaoManualDeNFS(carga, pedidoXMLNotaFiscalNFS, cargaPedido.ObterTomador(), terminoPrestacao, unitOfWork);

                                    unitOfWork.CommitChanges();
                                }
                            }
                            else
                            {
                                if (pedidoCTeParaSubContratacao.DocsParaEmissaoNFSManual.Count > 0)
                                    continue;

                                unitOfWork.Start();

                                Dominio.Entidades.Cliente tomador = cteParaSubcontratacao.Emitente?.Cliente;

                                if (tomador == null)
                                    tomador = cargaPedido.Pedido.SubContratante != null ? cargaPedido.Pedido.SubContratante : cargaPedido.Tomador;

                                carga.AgNFSManual = true;
                                serNFS.CriarCTePendenteEmissaoManualDeNFS(carga, pedidoCTeParaSubContratacao, tomador, inicioPrestacao, pedidoCTeParaSubContratacao.ValorFrete, unitOfWork);
                                unitOfWork.CommitChanges();
                            }
                        }

                        if (totalDocumentosGerados == 1 || (totalDocumentosGerados % quantidadeDocumentosAtualizarCarga) == 0)
                            svcHubCarga.InformarQuantidadeDocumentosEmitidos(carga.Codigo, totalDocumentos, totalDocumentosGerados);

                        unitOfWork.FlushAndClear();
                    }
                }
                else
                {
                    Servicos.Embarcador.CTe.CTEsImportados serCTEsImportados = new Servicos.Embarcador.CTe.CTEsImportados(unitOfWork);
                    serCTEsImportados.VincularCargaCTeEmitidoAnteriormente(carga, cargaPedido, unitOfWork);
                }

                if (emitiu)
                    svcHubCarga.InformarQuantidadeDocumentosEmitidos(carga.Codigo, totalDocumentos, totalDocumentosGerados);
            }
        }

        public void GerarCTePorTerminal(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, int totalDocumentos, int quantidadeDocumentosAtualizarCarga, ref int totalDocumentosGerados, bool ratearPorTerminais, bool ratearPorBUS, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao> pedidosCTeParaSubContratacaoContaContabilContabilizacao)
        {
            CTe serCte = new CTe(unitOfWork);
            CTeSubContratacaoEmissao serCTeSubContratacaoEmissao = new CTeSubContratacaoEmissao(unitOfWork);
            Servicos.Embarcador.Carga.NFS serNFS = new NFS(unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroQuantidade repCTeParaSubContratacaoQuantidade = new Repositorio.Embarcador.CTe.CTeTerceiroQuantidade(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroSeguro repCTeParaSubContratacaoSeguro = new Repositorio.Embarcador.CTe.CTeTerceiroSeguro(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repPedidoCTeParaSubContratacaoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.CTe.CTeDocumentoOriginario repDocumentoOriginario = new Repositorio.Embarcador.CTe.CTeDocumentoOriginario(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            CTePorPedido serCTePorPedido = new CTePorPedido(unitOfWork);
            Hubs.Carga svcHubCarga = new Hubs.Carga();

            for (int i = 0; i < cargaPedidos.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigoComFetchEmpresa(cargaPedidos[i].Codigo);

                if (!cargaPedido.PossuiCTe && !cargaPedido.PossuiNFS && !cargaPedido.PossuiNFSManual && !(cargaPedido.PossuiNFSManual && cargaPedido.ModeloDocumentoFiscalIntramunicipal != null))
                    continue;

                bool emitiu = false;

                if (!cargaPedido.CTeEmitidoNoEmbarcador && !cargaPedido.Pedido.PedidoTransbordo)
                {
                    serCte.ObterDescricoesComponentesPadrao(cargaPedido.ObterTomador(), carga, configuracaoEmbarcador, unitOfWork, out string descricaoComponenteValorFrete, out string descricaoComponenteValorICMS);

                    List<int> codigosTerminalOrigem = repPedidoCTeParaSubContratacao.BuscarCodigosTerminaisOrigemPorCargaPedido(cargaPedido.Codigo);
                    List<int> codigosTerminalDestino = repPedidoCTeParaSubContratacao.BuscarCodigosTerminaisDestinoPorCargaPedido(cargaPedido.Codigo);

                    foreach (var codigoTerminalOrigem in codigosTerminalOrigem)
                    {
                        foreach (var codigoTerminalDestino in codigosTerminalDestino)
                        {
                            List<int> pedidoCTesParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarCodigosPorCargaPedidoTerminalOrigemTerminalDestino(cargaPedido.Codigo, codigoTerminalOrigem, codigoTerminalDestino);

                            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidosCTeParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarPorCodigosComFetch(pedidoCTesParaSubContratacao);
                            if (pedidosCTeParaSubContratacao != null && pedidosCTeParaSubContratacao.Count > 0)
                            {
                                Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalCarga = pedidosCTeParaSubContratacao.FirstOrDefault().ModeloDocumentoFiscal;

                                if (modeloDocumentoFiscalCarga == null)
                                    modeloDocumentoFiscalCarga = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.CTe);

                                List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> pedidoCTeParaSubContratacaoNotasFiscais = repPedidoCTeParaSubContratacaoNotaFiscal.BuscarPorPedidoCTeParaSubcontratacao(pedidoCTesParaSubContratacao);
                                Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorCodigo(pedidoCTeParaSubContratacaoNotasFiscais.First().PedidoXMLNotaFiscal.Codigo);

                                List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesParaSubcontratacao = pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro).ToList();

                                Dominio.Entidades.Cliente expedidor = null;
                                Dominio.Entidades.Cliente recebedor = null;
                                Dominio.Entidades.Localidade inicioPrestacao = ctesParaSubcontratacao.FirstOrDefault().LocalidadeInicioPrestacao;
                                Dominio.Entidades.Localidade terminoPrestacao = ctesParaSubcontratacao.FirstOrDefault().LocalidadeTerminoPrestacao;
                                Dominio.Entidades.Cliente remetente = repCliente.BuscarPorCPFCNPJ((ctesParaSubcontratacao.FirstOrDefault().Remetente.Cliente.CPF_CNPJ));
                                Dominio.Entidades.Cliente destinatario = repCliente.BuscarPorCPFCNPJ((ctesParaSubcontratacao.FirstOrDefault().Destinatario.Cliente.CPF_CNPJ));

                                if (pedidosCTeParaSubContratacao.FirstOrDefault().CargaPedido.Expedidor != null)
                                {
                                    expedidor = pedidosCTeParaSubContratacao.FirstOrDefault().CargaPedido.Expedidor;
                                    if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
                                        inicioPrestacao = expedidor.Localidade;
                                }

                                if (pedidosCTeParaSubContratacao.FirstOrDefault().CargaPedido.Recebedor != null)
                                {
                                    recebedor = pedidosCTeParaSubContratacao.FirstOrDefault().CargaPedido.Recebedor;
                                    if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
                                        terminoPrestacao = recebedor.Localidade;
                                }

                                if (!pedidosCTeParaSubContratacao.FirstOrDefault().PossuiNFSManual ||
                                    (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS &&
                                    (inicioPrestacao.Codigo != terminoPrestacao.Codigo || (pedidosCTeParaSubContratacao.FirstOrDefault().PossuiNFSManual && cargaPedido.ModeloDocumentoFiscalIntramunicipal != null && inicioPrestacao.Codigo == terminoPrestacao.Codigo))))
                                {
                                    bool somenteCTeSubContratacaoFilialEmissora = false;

                                    carga = repCarga.BuscarPorCodigo(carga.Codigo);

                                    if (cargaPedido.CargaPedidoFilialEmissora && carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                                        somenteCTeSubContratacaoFilialEmissora = true;

                                    //if (repCargaPedidoXMLNotaFiscalCTe.ContarCTesPorNotasFiscais(pedidoCTeParaSubContratacaoNotasFiscais.Select(o => o.PedidoXMLNotaFiscal.Codigo).ToArray(), somenteCTeSubContratacaoFilialEmissora) > 0)  //pedidoXMLNotaFiscal.CTes.Count > 0) //já foi gerado um ct-e para esta nota
                                    if (repCargaPedidoXMLNotaFiscalCTe.ContarCTesPorPedidoCTeParaSubContratacao(pedidoCTesParaSubContratacao, somenteCTeSubContratacaoFilialEmissora) > 0)
                                        continue;

                                    Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoEmitir = null;

                                    if (pedidosCTeParaSubContratacao.FirstOrDefault().PossuiNFSManual)
                                        modeloDocumentoEmitir = cargaPedido.ModeloDocumentoFiscalIntramunicipal;
                                    else
                                        modeloDocumentoEmitir = modeloDocumentoFiscalCarga;

                                    unitOfWork.Start();

                                    emitiu = true;
                                    totalDocumentosGerados++;
                                    Dominio.ObjetosDeValor.CTe.CTe cte = new Dominio.ObjetosDeValor.CTe.CTe();
                                    cte.Documentos = new List<Dominio.ObjetosDeValor.CTe.Documento>();

                                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoNotaFiscal in pedidoCTeParaSubContratacaoNotasFiscais)
                                    {
                                        Dominio.ObjetosDeValor.CTe.Documento docNF = serCte.BuscarDocumentoCTe(pedidoCTeParaSubContratacaoNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal, carga.Empresa.TipoAmbiente, configuracaoEmbarcador);
                                        cte.Documentos.Add(docNF);
                                    }

                                    cte.PercentualPagamentoAgregado = pedidosCTeParaSubContratacao.FirstOrDefault().PercentualPagamentoAgregado;

                                    List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> cargaPedidoComponentesFretesCliente = BuscarPedidoCTeSubcontratacaoComponentesFrete(pedidosCTeParaSubContratacao, modeloDocumentoFiscalCarga, unitOfWork);

                                    cte.ValorTotalMercadoria = ctesParaSubcontratacao.Sum(o => o.ValorTotalMercadoria);

                                    decimal valorFreteNegociado = repPedidoCTeParaSubContratacao.ObterValorFreteNegociavelPedido(cargaPedido.Codigo);
                                    decimal valorCusteioSVM = repPedidoCTeParaSubContratacao.ObterValorCusteioSVMPedido(cargaPedido.Codigo);
                                    decimal valorICMS = pedidosCTeParaSubContratacao.Sum(o => o.ValorICMS);
                                    decimal aliquota = repPedidoCTeParaSubContratacao.ObterAliquota(cargaPedido.Codigo);

                                    cargaPedido.Pedido.NumeroBooking = repPedidoCTeParaSubContratacao.ObterNumeroBookingPedido(pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).FirstOrDefault());
                                    cargaPedido.Pedido.DescricaoCarrierNavioViagem = repPedidoCTeParaSubContratacao.ObterDescricaoCarrierPedido(pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).FirstOrDefault());
                                    cargaPedido.Pedido.TipoPropostaFeeder = repPedidoCTeParaSubContratacao.ObterTipoPropostaFeederPedido(pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).FirstOrDefault());

                                    Repositorio.Embarcador.Pedidos.PedidoTransbordo repPedidoTransbordo = new Repositorio.Embarcador.Pedidos.PedidoTransbordo(unitOfWork);
                                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo> transbordosAnteriores = repPedidoTransbordo.BuscarPorPedido(cargaPedido.Pedido.Codigo);
                                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo> transbordos = repPedidoCTeParaSubContratacao.ObterTransbordosPedido(pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).FirstOrDefault());
                                    if (transbordos != null && transbordos.Count > 0)
                                    {
                                        if (transbordosAnteriores != null && transbordosAnteriores.Count > 0)
                                        {
                                            foreach (var transbordo in transbordosAnteriores)
                                            {
                                                repPedidoTransbordo.Deletar(transbordo);
                                            }
                                        }
                                        foreach (var transbordo in transbordos)
                                        {
                                            Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo trans = new Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo()
                                            {
                                                Navio = transbordo.Navio,
                                                Pedido = cargaPedido.Pedido,
                                                PedidoViagemNavio = transbordo.PedidoViagemNavio,
                                                Porto = transbordo.Porto,
                                                Sequencia = transbordo.Sequencia,
                                                Terminal = transbordo.Terminal
                                            };
                                            repPedidoTransbordo.Inserir(trans);
                                        }
                                    }

                                    repPedido.Atualizar(cargaPedido.Pedido);

                                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = repPedidoCTeParaSubContratacao.ObterCargaPedido(cargaPedido.Codigo);
                                    int qtdContainer = cargasPedidos != null && cargasPedidos.Count > 0 ? cargasPedidos.Select(p => p.Pedido.Container.Codigo).Distinct().Count() : 1;
                                    List<int> codigosContainer = new List<int>();
                                    codigosContainer.AddRange(cargasPedidos.Select(p => p.Pedido.Container.Codigo).Distinct().ToList());

                                    decimal valorFrete = 1;
                                    decimal baseICMS = ((100 - aliquota) / 100);
                                    valorFrete = CalcularValorSVM(cargaPedido.TipoPropostaMultimodal, pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).FirstOrDefault(), pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).ToList(),
                                        pedidosCTeParaSubContratacao.Count(), valorCusteioSVM, valorFreteNegociado, baseICMS, "", cargaPedido.Pedido.NumeroBooking, unitOfWork, qtdContainer, codigosContainer);


                                    cte.ValorAReceber = valorFrete;
                                    cte.ValorFrete = valorFrete;
                                    cte.ValorTotalPrestacaoServico = valorFrete;


                                    if (cargaPedido.Pedido.EmpresaSerie != null)
                                        cte.Serie = cargaPedido.Pedido.EmpresaSerie.Numero;

                                    cte.ProdutoPredominante = ctesParaSubcontratacao.FirstOrDefault().ProdutoPredominante;

                                    List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade> quantidades = repCTeParaSubContratacaoQuantidade.BuscarPorCTeParaSubContratacao(ctesParaSubcontratacao.Select(o => o.Codigo).ToList());
                                    List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroSeguro> seguros = repCTeParaSubContratacaoSeguro.BuscarPorCTeParaSubContratacao(ctesParaSubcontratacao.Select(o => o.Codigo).ToList());

                                    Dominio.Entidades.Cliente tomador = ctesParaSubcontratacao.FirstOrDefault().Emitente?.Cliente;

                                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && modeloDocumentoEmitir != null && modeloDocumentoEmitir.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe && cargaPedido.Tomador != null)
                                        tomador = cargaPedido.Tomador;

                                    if (tomador == null)
                                        tomador = cargaPedido.Pedido.SubContratante != null ? cargaPedido.Pedido.SubContratante : cargaPedido.Tomador;

                                    Dominio.Enumeradores.TipoPagamento tipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros; //cargaPedido.Pedido.TipoPagamento; //Dominio.Enumeradores.TipoPagamento.Outros;//cargaPedido.Pedido.TipoPagamento;
                                    Dominio.Enumeradores.TipoTomador tipoTomador = Dominio.Enumeradores.TipoTomador.Outros; //cargaPedido.TipoTomador; //Dominio.Enumeradores.TipoTomador.Outros;
                                    List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesParaSubcontrataca = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>();
                                    foreach (var cteParaSubcontratacao in ctesParaSubcontratacao)
                                    {
                                        if (!ctesParaSubcontrataca.Contains(cteParaSubcontratacao))
                                            ctesParaSubcontrataca.Add(cteParaSubcontratacao);
                                    }

                                    List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidadesCTes = ConverterQuantidadesTerceirosParaQuantidadesCTe(quantidades);

                                    bool emitindoCTeFilialEmissora = false;
                                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeFilialEmissora = pedidosCTeParaSubContratacao.FirstOrDefault().CTeTerceiro.ObterCargaCTe(carga.Codigo);

                                    if (cargaPedido.CargaPedidoFilialEmissora)
                                        emitindoCTeFilialEmissora = true;

                                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga tipoContratacaoCarga = cargaPedido.TipoContratacaoCarga;

                                    if (emitindoCTeFilialEmissora)
                                        tipoContratacaoCarga = cargaPedido.TipoContratacaoCargaSubContratacaoFilialEmissora;

                                    Dominio.Enumeradores.TipoServico tipoServico = Dominio.Enumeradores.TipoServico.SubContratacao;

                                    if (carga.CargaSVM)
                                        tipoServico = Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal;
                                    else if (tipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho)
                                        tipoServico = Dominio.Enumeradores.TipoServico.Redespacho;
                                    else if (tipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario)
                                        tipoServico = Dominio.Enumeradores.TipoServico.RedIntermediario;

                                    List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ctesAnteriores = ConverterCTesTerceirosParaAnteriores(ctesParaSubcontrataca);
                                    List<string> chavesCTes = new List<string>();
                                    if (ctesAnteriores != null && ctesAnteriores.Count > 0)
                                        chavesCTes = ctesAnteriores.Select(o => o.Chave).ToList();

                                    Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = BuscarRegraICMS(cargaPedido.CargaOrigem, inicioPrestacao, terminoPrestacao, pedidosCTeParaSubContratacao.FirstOrDefault(), unitOfWork);
                                    Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = BuscarRegraISS(cargaPedido.CargaOrigem, pedidosCTeParaSubContratacao.FirstOrDefault(), unitOfWork);
                                    Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS();

                                    cargaPedido = repCargaPedido.BuscarPorCodigo(cargaPedido.Codigo);

                                    cargaPedido.ValorFrete = valorFrete;
                                    cargaPedido.ValorFreteAPagar = valorFrete;
                                    cargaPedido.ValorICMS = 0;
                                    cargaPedido.PercentualAliquota = 0;
                                    cargaPedido.BaseCalculoICMS = 0;
                                    cargaPedido.CST = "41";
                                    cargaPedido.CFOP = repCFOP.BuscarPorNumero(regraICMS.CFOP);

                                    repCargaPedido.Atualizar(cargaPedido);

                                    List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> apoliceSeguro = ConverterSegurosDeTerceirosEmSeguroCTe(seguros);

                                    string observacaoCTe = null;
                                    Dominio.Enumeradores.TipoCTE tipoCTe = Dominio.Enumeradores.TipoCTE.Normal;

                                    List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil> configuracoes = Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil.ConverterPedidoCTeParaSubContratacaoContaContabilContabilizacao(pedidosCTeParaSubContratacao.FirstOrDefault(), pedidosCTeParaSubContratacaoContaContabilContabilizacao);
                                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTE = serCte.GerarCTe(carga, cargaPedido, cte, cargaPedido.CargaOrigem.Empresa, remetente, destinatario, tomador, expedidor, recebedor, inicioPrestacao, terminoPrestacao, null, null, tipoPagamento, tipoTomador, quantidadesCTes, cargaPedidoComponentesFretesCliente, observacaoCTe, pedidoXMLNotaFiscal.CargaPedido.Pedido.ObservacaoCTeTerceiro, pedidoXMLNotaFiscal.CargaPedido.Pedido, null, apoliceSeguro, regraICMS, regraISS, impostoIBSCBS, tipoServicoMultisoftware, modeloDocumentoEmitir, tipoServico, tipoCTe, ctesAnteriores, 0, false, cargaCTeFilialEmissora, false, unitOfWork, configuracaoEmbarcador, configuracaoGeralCarga, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultado, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultadoDestinatario, pedidosCTeParaSubContratacao.FirstOrDefault().ItemServico, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultadoEscrituracao, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultadoICMS, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultadoPIS, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultadoCOFINS, pedidosCTeParaSubContratacao.FirstOrDefault().ValorMaximoCentroContabilizacao, configuracoes, pedidoCTeParaSubContratacaoNotasFiscais.Select(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal).ToList(), null, descricaoComponenteValorICMS, descricaoComponenteValorFrete, null, null, cargasPedidos, 0, chavesCTes, null, null, null, null, true);
                                    SalvarNumeroControleSVM(cargaPedido.Codigo, cargaCTE.CTe?.NumeroControle ?? "", unitOfWork, cargaCTE.CTe?.Codigo ?? 0);

                                    if (emitindoCTeFilialEmissora && cargaCTeFilialEmissora != null)
                                    {
                                        Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                                        cargaCTeFilialEmissora.CargaCTeSubContratacaoFilialEmissora = cargaCTE;
                                        repCargaCte.Atualizar(cargaCTeFilialEmissora);
                                    }
                                    List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> apoliceSeguroAverbacaos = repApoliceSeguroAverbacao.BuscarPorCargaPedido(cargaPedido.Codigo, emitindoCTeFilialEmissora);
                                    serCte.AverbaCargaCTe(cargaCTE, apoliceSeguroAverbacaos, unitOfWork, configuracaoEmbarcador, cargaPedido.Pedido.FormaAverbacaoCTE);
                                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoNotaFiscal in pedidoCTeParaSubContratacaoNotasFiscais)
                                    {
                                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTE = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe();
                                        cargaPedidoXMLNotaFiscalCTE.CargaCTe = cargaCTE;
                                        cargaPedidoXMLNotaFiscalCTE.PedidoXMLNotaFiscal = pedidoCTeParaSubContratacaoNotaFiscal.PedidoXMLNotaFiscal;
                                        repCargaPedidoXMLNotaFiscalCTe.Inserir(cargaPedidoXMLNotaFiscalCTE);
                                    }

                                    bool inseriuOBS = false;
                                    if (cargaCTE.CTe != null)
                                    {
                                        foreach (var cteParaSubcontratacao in ctesParaSubcontratacao)
                                        {
                                            Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario documentoOriginario = new Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario()
                                            {
                                                CTe = cargaCTE.CTe,
                                                Chave = cteParaSubcontratacao.ChaveAcesso,
                                                Numero = cteParaSubcontratacao.Numero,
                                                Serie = cteParaSubcontratacao.Serie,
                                                DataEmissao = cteParaSubcontratacao.DataEmissao,
                                                NumeroMinuta = cteParaSubcontratacao.NumeroMinuta,
                                                NumeroOperacionalConhecimentoAereo = cteParaSubcontratacao.NumeroOperacionalConhecimentoAereo
                                            };
                                            repDocumentoOriginario.Inserir(documentoOriginario);

                                            if (!inseriuOBS && cargaCTE.CTe.SVMProprio && cargaCTE.CTe.PortoOrigem != null && cargaCTE.CTe.PortoOrigem != null && cargaCTE.CTe.PortoOrigem.AtivarDespachanteComoConsignatario && cargaCTE.CTe.PortoDestino.AtivarDespachanteComoConsignatario)
                                            {
                                                string chaveCTeMultimodal = documentoOriginario.Chave;
                                                if (!string.IsNullOrWhiteSpace(chaveCTeMultimodal))
                                                {
                                                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cteMultimodal = repCTe.BuscarPorChave(chaveCTeMultimodal);
                                                    if (cteMultimodal != null && cteMultimodal.TomadorPagador != null && cteMultimodal.TomadorPagador.GrupoPessoas != null && cteMultimodal.TomadorPagador.GrupoPessoas.AdicionarDespachanteComoConsignatario && cteMultimodal.TomadorPagador.GrupoPessoas.Despachante != null)
                                                    {
                                                        cargaCTE.CTe.ObservacoesGerais += " A " + cteMultimodal.TomadorPagador.GrupoPessoas.Despachante.Nome + ", CNPJ " + cteMultimodal.TomadorPagador.GrupoPessoas.Despachante.CPF_CNPJ_Formatado + ", irá atuar como despachante para a liberação dessa carga no porto de descarga.";
                                                        cargaCTE.CTe.ObservacoesGerais = cargaCTE.CTe.ObservacoesGerais.Trim();
                                                        cargaCTE.CTe.EmailDespachanteSVM = cteMultimodal.TomadorPagador.GrupoPessoas.EmailDespachante;
                                                        repCTe.Atualizar(cargaCTE.CTe);
                                                    }
                                                }
                                                inseriuOBS = true;
                                            }
                                        }
                                    }

                                    if (pedidosCTeParaSubContratacao.FirstOrDefault().PossuiNFSManual)
                                    {
                                        serNFS.CriarCargaCTePendenteEmissaoManualDeNFS(carga, cargaCTE, inicioPrestacao, unitOfWork);

                                        carga.AgNFSManual = true;
                                    }

                                    unitOfWork.CommitChanges();
                                    if (cte != null && cte.DocumentosTransporteAnteriores != null && cargaCTE.CTe.TipoServico == Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal && cargaCTE.CTe.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario)
                                    {
                                        Repositorio.DocumentoDeTransporteAnteriorCTe repDocumento = new Repositorio.DocumentoDeTransporteAnteriorCTe(unitOfWork);
                                        Servicos.Log.TratarErro(cargaCTE.CTe.Codigo.ToString() + " - Iniciou SalvarInformacoesDocumentosAnteriores SVM - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "TempoEmissaoCTe");
                                        foreach (var doc in cte.DocumentosTransporteAnteriores)
                                        {
                                            unitOfWork.Start();

                                            Dominio.Entidades.DocumentoDeTransporteAnteriorCTe documento = new Dominio.Entidades.DocumentoDeTransporteAnteriorCTe();

                                            documento.Chave = doc.Chave;
                                            documento.CTe = cargaCTE.CTe;

                                            DateTime dataEmissao;
                                            DateTime.TryParseExact(doc.DataEmissao, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
                                            double cpfCnpj = 0;
                                            if (doc.Emissor != null)
                                                double.TryParse(Utilidades.String.OnlyNumbers(doc.Emissor.CPFCNPJ), out cpfCnpj);

                                            if (dataEmissao != DateTime.MinValue)
                                                documento.DataEmissao = dataEmissao;

                                            documento.Emissor = cpfCnpj > 0 ? repCliente.BuscarPorCPFCNPJ(cpfCnpj) : null;
                                            documento.Numero = doc.Numero;
                                            documento.Serie = doc.Serie;
                                            documento.Tipo = doc.Tipo;

                                            repDocumento.Inserir(documento);

                                            unitOfWork.CommitChanges();

                                            unitOfWork.FlushAndClear();
                                        }
                                    }
                                    cargaCTE = repCargaCTe.BuscarPorCodigo(cargaCTE.Codigo);
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
                                    if (cargaPedido.CargaPedidoFilialEmissora || (cargaPedido.CargaPedidoTrechoAnterior?.CargaPedidoFilialEmissora ?? false))
                                    {
                                        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoPedidoNotaFiscal in pedidoCTeParaSubContratacaoNotasFiscais)
                                        {
                                            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscalNFS = pedidoCTeParaSubContratacaoPedidoNotaFiscal.PedidoXMLNotaFiscal;
                                            if (pedidoXMLNotaFiscalNFS.DocsParaEmissaoNFSManual.Count > 0)
                                                continue;

                                            unitOfWork.Start();
                                            carga.AgNFSManual = true;

                                            serNFS.CriarNFPendenteEmissaoManualDeNFS(carga, pedidoXMLNotaFiscalNFS, cargaPedido.ObterTomador(), terminoPrestacao, unitOfWork);

                                            unitOfWork.CommitChanges();
                                        }
                                    }
                                    else
                                    {
                                        if (pedidosCTeParaSubContratacao.FirstOrDefault().DocsParaEmissaoNFSManual.Count > 0)
                                            continue;

                                        unitOfWork.Start();

                                        Dominio.Entidades.Cliente tomador = ctesParaSubcontratacao.FirstOrDefault().Emitente?.Cliente;

                                        if (tomador == null)
                                            tomador = cargaPedido.Pedido.SubContratante != null ? cargaPedido.Pedido.SubContratante : cargaPedido.Tomador;

                                        carga.AgNFSManual = true;
                                        serNFS.CriarCTePendenteEmissaoManualDeNFS(carga, pedidosCTeParaSubContratacao.FirstOrDefault(), tomador, inicioPrestacao, pedidosCTeParaSubContratacao.FirstOrDefault().ValorFrete, unitOfWork);
                                        unitOfWork.CommitChanges();
                                    }
                                }

                                if (totalDocumentosGerados == 1 || (totalDocumentosGerados % quantidadeDocumentosAtualizarCarga) == 0)
                                    svcHubCarga.InformarQuantidadeDocumentosEmitidos(carga.Codigo, totalDocumentos, totalDocumentosGerados);

                                unitOfWork.FlushAndClear();
                            }

                        }
                    }

                }
                else
                {
                    Servicos.Embarcador.CTe.CTEsImportados serCTEsImportados = new Servicos.Embarcador.CTe.CTEsImportados(unitOfWork);
                    serCTEsImportados.VincularCargaCTeEmitidoAnteriormente(carga, cargaPedido, unitOfWork);
                }

                if (emitiu)
                    svcHubCarga.InformarQuantidadeDocumentosEmitidos(carga.Codigo, totalDocumentos, totalDocumentosGerados);
            }
        }

        public void GerarCTePorBooking(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, int totalDocumentos, int quantidadeDocumentosAtualizarCarga, ref int totalDocumentosGerados, bool ratearMesmoQueMultiModal, bool ratearPorTerminais, bool ratearPorBUS, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao> pedidosCTeParaSubContratacaoContaContabilContabilizacao)
        {
            CTe serCte = new CTe(unitOfWork);
            CTeSubContratacaoEmissao serCTeSubContratacaoEmissao = new CTeSubContratacaoEmissao(unitOfWork);
            Servicos.Embarcador.Carga.NFS serNFS = new NFS(unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroQuantidade repCTeParaSubContratacaoQuantidade = new Repositorio.Embarcador.CTe.CTeTerceiroQuantidade(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroSeguro repCTeParaSubContratacaoSeguro = new Repositorio.Embarcador.CTe.CTeTerceiroSeguro(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repPedidoCTeParaSubContratacaoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.CTe.CTeDocumentoOriginario repDocumentoOriginario = new Repositorio.Embarcador.CTe.CTeDocumentoOriginario(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            CTePorPedido serCTePorPedido = new CTePorPedido(unitOfWork);
            Hubs.Carga svcHubCarga = new Hubs.Carga();

            for (int i = 0; i < cargaPedidos.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigoComFetchEmpresa(cargaPedidos[i].Codigo);

                if (!cargaPedido.PossuiCTe && !cargaPedido.PossuiNFS && !cargaPedido.PossuiNFSManual && !(cargaPedido.PossuiNFSManual && cargaPedido.ModeloDocumentoFiscalIntramunicipal != null))
                    continue;

                bool emitiu = false;

                if (!cargaPedido.CTeEmitidoNoEmbarcador && !cargaPedido.Pedido.PedidoTransbordo)
                {
                    serCte.ObterDescricoesComponentesPadrao(cargaPedido.ObterTomador(), carga, configuracaoEmbarcador, unitOfWork, out string descricaoComponenteValorFrete, out string descricaoComponenteValorICMS);

                    List<string> numerosBookings = repPedidoCTeParaSubContratacao.BuscarBookingsPorCargaPedido(cargaPedido.Codigo);

                    foreach (var numeroBooking in numerosBookings)
                    {
                        List<int> pedidoCTesParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarCodigosPorBooking(cargaPedido.Codigo, numeroBooking);

                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidosCTeParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarPorCodigosComFetch(pedidoCTesParaSubContratacao);
                        if (pedidosCTeParaSubContratacao != null && pedidosCTeParaSubContratacao.Count > 0)
                        {
                            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalCarga = pedidosCTeParaSubContratacao.FirstOrDefault().ModeloDocumentoFiscal;

                            if (modeloDocumentoFiscalCarga == null)
                                modeloDocumentoFiscalCarga = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.CTe);

                            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> pedidoCTeParaSubContratacaoNotasFiscais = repPedidoCTeParaSubContratacaoNotaFiscal.BuscarPorPedidoCTeParaSubcontratacao(pedidoCTesParaSubContratacao);
                            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorCodigo(pedidoCTeParaSubContratacaoNotasFiscais.First().PedidoXMLNotaFiscal.Codigo);

                            List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesParaSubcontratacao = pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro).ToList();

                            Dominio.Entidades.Cliente expedidor = null;
                            Dominio.Entidades.Cliente recebedor = null;
                            Dominio.Entidades.Localidade inicioPrestacao = ctesParaSubcontratacao.FirstOrDefault().LocalidadeInicioPrestacao;
                            Dominio.Entidades.Localidade terminoPrestacao = ctesParaSubcontratacao.FirstOrDefault().LocalidadeTerminoPrestacao;
                            Dominio.Entidades.Cliente remetente = repCliente.BuscarPorCPFCNPJ((ctesParaSubcontratacao.FirstOrDefault().Remetente.Cliente.CPF_CNPJ));
                            Dominio.Entidades.Cliente destinatario = repCliente.BuscarPorCPFCNPJ((ctesParaSubcontratacao.FirstOrDefault().Destinatario.Cliente.CPF_CNPJ));

                            if (pedidosCTeParaSubContratacao.FirstOrDefault().CargaPedido.Expedidor != null)
                            {
                                expedidor = pedidosCTeParaSubContratacao.FirstOrDefault().CargaPedido.Expedidor;
                                if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
                                    inicioPrestacao = expedidor.Localidade;
                            }

                            if (pedidosCTeParaSubContratacao.FirstOrDefault().CargaPedido.Recebedor != null)
                            {
                                recebedor = pedidosCTeParaSubContratacao.FirstOrDefault().CargaPedido.Recebedor;
                                if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
                                    terminoPrestacao = recebedor.Localidade;
                            }

                            if (!pedidosCTeParaSubContratacao.FirstOrDefault().PossuiNFSManual ||
                                (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS &&
                                (inicioPrestacao.Codigo != terminoPrestacao.Codigo || (pedidosCTeParaSubContratacao.FirstOrDefault().PossuiNFSManual && cargaPedido.ModeloDocumentoFiscalIntramunicipal != null && inicioPrestacao.Codigo == terminoPrestacao.Codigo))))
                            {
                                bool somenteCTeSubContratacaoFilialEmissora = false;

                                carga = repCarga.BuscarPorCodigo(carga.Codigo);

                                if (cargaPedido.CargaPedidoFilialEmissora && carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                                    somenteCTeSubContratacaoFilialEmissora = true;

                                //if (repCargaPedidoXMLNotaFiscalCTe.ContarCTesPorNotasFiscais(pedidoCTeParaSubContratacaoNotasFiscais.Select(o => o.PedidoXMLNotaFiscal.Codigo).ToArray(), somenteCTeSubContratacaoFilialEmissora) > 0)  //pedidoXMLNotaFiscal.CTes.Count > 0) //já foi gerado um ct-e para esta nota
                                if (repCargaPedidoXMLNotaFiscalCTe.ContarCTesPorPedidoCTeParaSubContratacao(pedidoCTesParaSubContratacao, somenteCTeSubContratacaoFilialEmissora) > 0)
                                    continue;

                                Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoEmitir = null;

                                if (pedidosCTeParaSubContratacao.FirstOrDefault().PossuiNFSManual)
                                    modeloDocumentoEmitir = cargaPedido.ModeloDocumentoFiscalIntramunicipal;
                                else
                                    modeloDocumentoEmitir = modeloDocumentoFiscalCarga;

                                unitOfWork.Start();

                                emitiu = true;
                                totalDocumentosGerados++;
                                Dominio.ObjetosDeValor.CTe.CTe cte = new Dominio.ObjetosDeValor.CTe.CTe();
                                cte.Documentos = new List<Dominio.ObjetosDeValor.CTe.Documento>();

                                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoNotaFiscal in pedidoCTeParaSubContratacaoNotasFiscais)
                                {
                                    Dominio.ObjetosDeValor.CTe.Documento docNF = serCte.BuscarDocumentoCTe(pedidoCTeParaSubContratacaoNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal, carga.Empresa.TipoAmbiente, configuracaoEmbarcador);
                                    cte.Documentos.Add(docNF);
                                }

                                cte.PercentualPagamentoAgregado = pedidosCTeParaSubContratacao.FirstOrDefault().PercentualPagamentoAgregado;

                                List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> cargaPedidoComponentesFretesCliente = BuscarPedidoCTeSubcontratacaoComponentesFrete(pedidosCTeParaSubContratacao, modeloDocumentoFiscalCarga, unitOfWork);

                                cte.ValorTotalMercadoria = ctesParaSubcontratacao.Sum(o => o.ValorTotalMercadoria);

                                decimal valorFreteNegociado = repPedidoCTeParaSubContratacao.ObterValorFreteNegociavelPedido(cargaPedido.Codigo);
                                decimal valorCusteioSVM = repPedidoCTeParaSubContratacao.ObterValorCusteioSVMPedido(cargaPedido.Codigo);
                                decimal valorICMS = pedidosCTeParaSubContratacao.Sum(o => o.ValorICMS);
                                decimal aliquota = repPedidoCTeParaSubContratacao.ObterAliquota(cargaPedido.Codigo);

                                cargaPedido.Pedido.NumeroBooking = repPedidoCTeParaSubContratacao.ObterNumeroBookingPedido(pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).FirstOrDefault());
                                cargaPedido.Pedido.DescricaoCarrierNavioViagem = repPedidoCTeParaSubContratacao.ObterDescricaoCarrierPedido(pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).FirstOrDefault());
                                cargaPedido.Pedido.TipoPropostaFeeder = repPedidoCTeParaSubContratacao.ObterTipoPropostaFeederPedido(pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).FirstOrDefault());

                                Repositorio.Embarcador.Pedidos.PedidoTransbordo repPedidoTransbordo = new Repositorio.Embarcador.Pedidos.PedidoTransbordo(unitOfWork);
                                List<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo> transbordosAnteriores = repPedidoTransbordo.BuscarPorPedido(cargaPedido.Pedido.Codigo);
                                List<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo> transbordos = repPedidoCTeParaSubContratacao.ObterTransbordosPedido(pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).FirstOrDefault());
                                if (transbordos != null && transbordos.Count > 0)
                                {
                                    if (transbordosAnteriores != null && transbordosAnteriores.Count > 0)
                                    {
                                        foreach (var transbordo in transbordosAnteriores)
                                        {
                                            repPedidoTransbordo.Deletar(transbordo);
                                        }
                                    }
                                    foreach (var transbordo in transbordos)
                                    {
                                        Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo trans = new Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo()
                                        {
                                            Navio = transbordo.Navio,
                                            Pedido = cargaPedido.Pedido,
                                            PedidoViagemNavio = transbordo.PedidoViagemNavio,
                                            Porto = transbordo.Porto,
                                            Sequencia = transbordo.Sequencia,
                                            Terminal = transbordo.Terminal
                                        };
                                        repPedidoTransbordo.Inserir(trans);
                                    }
                                }

                                repPedido.Atualizar(cargaPedido.Pedido);

                                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = repPedidoCTeParaSubContratacao.ObterCargaPedido(cargaPedido.Codigo);
                                int qtdContainer = cargasPedidos != null && cargasPedidos.Count > 0 ? cargasPedidos.Select(p => p.Pedido.Container.Codigo).Distinct().Count() : 1;
                                List<int> codigosContainer = new List<int>();
                                codigosContainer.AddRange(cargasPedidos.Select(p => p.Pedido.Container.Codigo).Distinct().ToList());
                                decimal valorFrete = 1;
                                decimal baseICMS = ((100 - aliquota) / 100);
                                valorFrete = CalcularValorSVM(cargaPedido.TipoPropostaMultimodal, pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).FirstOrDefault(), pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).ToList(),
                                    pedidosCTeParaSubContratacao.Count(), valorCusteioSVM, valorFreteNegociado, baseICMS, "", numeroBooking, unitOfWork, qtdContainer, codigosContainer);

                                cte.ValorAReceber = valorFrete;
                                cte.ValorFrete = valorFrete;
                                cte.ValorTotalPrestacaoServico = valorFrete;

                                if (cargaPedido.Pedido.EmpresaSerie != null)
                                    cte.Serie = cargaPedido.Pedido.EmpresaSerie.Numero;

                                cte.ProdutoPredominante = ctesParaSubcontratacao.FirstOrDefault().ProdutoPredominante;

                                List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade> quantidades = repCTeParaSubContratacaoQuantidade.BuscarPorCTeParaSubContratacao(ctesParaSubcontratacao.Select(o => o.Codigo).ToList());
                                List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroSeguro> seguros = repCTeParaSubContratacaoSeguro.BuscarPorCTeParaSubContratacao(ctesParaSubcontratacao.Select(o => o.Codigo).ToList());

                                Dominio.Entidades.Cliente tomador = ctesParaSubcontratacao.FirstOrDefault().Emitente?.Cliente;

                                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && modeloDocumentoEmitir != null && modeloDocumentoEmitir.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe && cargaPedido.Tomador != null)
                                    tomador = cargaPedido.Tomador;

                                if (tomador == null)
                                    tomador = cargaPedido.Pedido.SubContratante != null ? cargaPedido.Pedido.SubContratante : cargaPedido.Tomador;

                                Dominio.Enumeradores.TipoPagamento tipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros; //cargaPedido.Pedido.TipoPagamento; //Dominio.Enumeradores.TipoPagamento.Outros;//cargaPedido.Pedido.TipoPagamento;
                                Dominio.Enumeradores.TipoTomador tipoTomador = Dominio.Enumeradores.TipoTomador.Outros; //cargaPedido.TipoTomador; //Dominio.Enumeradores.TipoTomador.Outros;
                                List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesParaSubcontrataca = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>();
                                foreach (var cteParaSubcontratacao in ctesParaSubcontratacao)
                                {
                                    if (!ctesParaSubcontrataca.Contains(cteParaSubcontratacao))
                                        ctesParaSubcontrataca.Add(cteParaSubcontratacao);
                                }

                                List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidadesCTes = ConverterQuantidadesTerceirosParaQuantidadesCTe(quantidades);

                                bool emitindoCTeFilialEmissora = false;
                                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeFilialEmissora = pedidosCTeParaSubContratacao.FirstOrDefault().CTeTerceiro.ObterCargaCTe(carga.Codigo);

                                if (cargaPedido.CargaPedidoFilialEmissora)
                                    emitindoCTeFilialEmissora = true;

                                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga tipoContratacaoCarga = cargaPedido.TipoContratacaoCarga;

                                if (emitindoCTeFilialEmissora)
                                    tipoContratacaoCarga = cargaPedido.TipoContratacaoCargaSubContratacaoFilialEmissora;

                                Dominio.Enumeradores.TipoServico tipoServico = Dominio.Enumeradores.TipoServico.SubContratacao;

                                if (carga.CargaSVM)
                                    tipoServico = Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal;
                                else if (tipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho)
                                    tipoServico = Dominio.Enumeradores.TipoServico.Redespacho;
                                else if (tipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario)
                                    tipoServico = Dominio.Enumeradores.TipoServico.RedIntermediario;

                                List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ctesAnteriores = ConverterCTesTerceirosParaAnteriores(ctesParaSubcontrataca);
                                List<string> chavesCTes = new List<string>();
                                if (ctesAnteriores != null && ctesAnteriores.Count > 0)
                                    chavesCTes = ctesAnteriores.Select(o => o.Chave).ToList();

                                Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = BuscarRegraICMS(cargaPedido.CargaOrigem, inicioPrestacao, terminoPrestacao, pedidosCTeParaSubContratacao.FirstOrDefault(), unitOfWork);
                                Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = BuscarRegraISS(cargaPedido.CargaOrigem, pedidosCTeParaSubContratacao.FirstOrDefault(), unitOfWork);
                                Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS();

                                cargaPedido = repCargaPedido.BuscarPorCodigo(cargaPedido.Codigo);

                                cargaPedido.ValorFrete = valorFrete;
                                cargaPedido.ValorFreteAPagar = valorFrete;
                                cargaPedido.ValorICMS = 0;
                                cargaPedido.PercentualAliquota = 0;
                                cargaPedido.BaseCalculoICMS = 0;
                                cargaPedido.CST = "41";
                                cargaPedido.CFOP = repCFOP.BuscarPorNumero(regraICMS.CFOP);

                                repCargaPedido.Atualizar(cargaPedido);

                                List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> apoliceSeguro = ConverterSegurosDeTerceirosEmSeguroCTe(seguros);

                                string observacaoCTe = null;
                                Dominio.Enumeradores.TipoCTE tipoCTe = Dominio.Enumeradores.TipoCTE.Normal;

                                List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil> configuracoes = Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil.ConverterPedidoCTeParaSubContratacaoContaContabilContabilizacao(pedidosCTeParaSubContratacao.FirstOrDefault(), pedidosCTeParaSubContratacaoContaContabilContabilizacao);
                                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTE = serCte.GerarCTe(carga, cargaPedido, cte, cargaPedido.CargaOrigem.Empresa, remetente, destinatario, tomador, expedidor, recebedor, inicioPrestacao, terminoPrestacao, null, null, tipoPagamento, tipoTomador, quantidadesCTes, cargaPedidoComponentesFretesCliente, observacaoCTe, pedidoXMLNotaFiscal.CargaPedido.Pedido.ObservacaoCTeTerceiro, pedidoXMLNotaFiscal.CargaPedido.Pedido, null, apoliceSeguro, regraICMS, regraISS, impostoIBSCBS, tipoServicoMultisoftware, modeloDocumentoEmitir, tipoServico, tipoCTe, ctesAnteriores, 0, false, cargaCTeFilialEmissora, false, unitOfWork, configuracaoEmbarcador, configuracaoGeralCarga, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultado, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultadoDestinatario, pedidosCTeParaSubContratacao.FirstOrDefault().ItemServico, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultadoEscrituracao, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultadoICMS, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultadoPIS, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultadoCOFINS, pedidosCTeParaSubContratacao.FirstOrDefault().ValorMaximoCentroContabilizacao, configuracoes, pedidoCTeParaSubContratacaoNotasFiscais.Select(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal).ToList(), null, descricaoComponenteValorICMS, descricaoComponenteValorFrete, null, null, cargasPedidos, 0, chavesCTes, null, null, null, null, true);
                                SalvarNumeroControleSVM(cargaPedido.Codigo, cargaCTE.CTe?.NumeroControle ?? "", unitOfWork, cargaCTE.CTe?.Codigo ?? 0);

                                if (emitindoCTeFilialEmissora && cargaCTeFilialEmissora != null)
                                {
                                    Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                                    cargaCTeFilialEmissora.CargaCTeSubContratacaoFilialEmissora = cargaCTE;
                                    repCargaCte.Atualizar(cargaCTeFilialEmissora);
                                }
                                List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> apoliceSeguroAverbacaos = repApoliceSeguroAverbacao.BuscarPorCargaPedido(cargaPedido.Codigo, emitindoCTeFilialEmissora);
                                serCte.AverbaCargaCTe(cargaCTE, apoliceSeguroAverbacaos, unitOfWork, configuracaoEmbarcador, cargaPedido.Pedido.FormaAverbacaoCTE);
                                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoNotaFiscal in pedidoCTeParaSubContratacaoNotasFiscais)
                                {
                                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTE = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe();
                                    cargaPedidoXMLNotaFiscalCTE.CargaCTe = cargaCTE;
                                    cargaPedidoXMLNotaFiscalCTE.PedidoXMLNotaFiscal = pedidoCTeParaSubContratacaoNotaFiscal.PedidoXMLNotaFiscal;
                                    repCargaPedidoXMLNotaFiscalCTe.Inserir(cargaPedidoXMLNotaFiscalCTE);
                                }

                                bool inseriuOBS = false;
                                if (cargaCTE.CTe != null)
                                {
                                    foreach (var cteParaSubcontratacao in ctesParaSubcontratacao)
                                    {
                                        Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario documentoOriginario = new Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario()
                                        {
                                            CTe = cargaCTE.CTe,
                                            Chave = cteParaSubcontratacao.ChaveAcesso,
                                            Numero = cteParaSubcontratacao.Numero,
                                            Serie = cteParaSubcontratacao.Serie,
                                            DataEmissao = cteParaSubcontratacao.DataEmissao,
                                            NumeroMinuta = cteParaSubcontratacao.NumeroMinuta,
                                            NumeroOperacionalConhecimentoAereo = cteParaSubcontratacao.NumeroOperacionalConhecimentoAereo
                                        };
                                        repDocumentoOriginario.Inserir(documentoOriginario);

                                        if (!inseriuOBS && cargaCTE.CTe.SVMProprio && cargaCTE.CTe.PortoOrigem != null && cargaCTE.CTe.PortoOrigem != null && cargaCTE.CTe.PortoOrigem.AtivarDespachanteComoConsignatario && cargaCTE.CTe.PortoDestino.AtivarDespachanteComoConsignatario)
                                        {
                                            string chaveCTeMultimodal = documentoOriginario.Chave;
                                            if (!string.IsNullOrWhiteSpace(chaveCTeMultimodal))
                                            {
                                                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteMultimodal = repCTe.BuscarPorChave(chaveCTeMultimodal);
                                                if (cteMultimodal != null && cteMultimodal.TomadorPagador != null && cteMultimodal.TomadorPagador.GrupoPessoas != null && cteMultimodal.TomadorPagador.GrupoPessoas.AdicionarDespachanteComoConsignatario && cteMultimodal.TomadorPagador.GrupoPessoas.Despachante != null)
                                                {
                                                    cargaCTE.CTe.ObservacoesGerais += " A " + cteMultimodal.TomadorPagador.GrupoPessoas.Despachante.Nome + ", CNPJ " + cteMultimodal.TomadorPagador.GrupoPessoas.Despachante.CPF_CNPJ_Formatado + ", irá atuar como despachante para a liberação dessa carga no porto de descarga.";
                                                    cargaCTE.CTe.ObservacoesGerais = cargaCTE.CTe.ObservacoesGerais.Trim();
                                                    cargaCTE.CTe.EmailDespachanteSVM = cteMultimodal.TomadorPagador.GrupoPessoas.EmailDespachante;
                                                    repCTe.Atualizar(cargaCTE.CTe);
                                                }
                                            }
                                            inseriuOBS = true;
                                        }
                                    }
                                }

                                if (pedidosCTeParaSubContratacao.FirstOrDefault().PossuiNFSManual)
                                {
                                    serNFS.CriarCargaCTePendenteEmissaoManualDeNFS(carga, cargaCTE, inicioPrestacao, unitOfWork);

                                    carga.AgNFSManual = true;
                                }

                                unitOfWork.CommitChanges();
                                if (cte != null && cte.DocumentosTransporteAnteriores != null && cargaCTE.CTe.TipoServico == Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal && cargaCTE.CTe.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario)
                                {
                                    Repositorio.DocumentoDeTransporteAnteriorCTe repDocumento = new Repositorio.DocumentoDeTransporteAnteriorCTe(unitOfWork);
                                    Servicos.Log.TratarErro(cargaCTE.CTe.Codigo.ToString() + " - Iniciou SalvarInformacoesDocumentosAnteriores SVM - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "TempoEmissaoCTe");
                                    foreach (var doc in cte.DocumentosTransporteAnteriores)
                                    {
                                        unitOfWork.Start();

                                        Dominio.Entidades.DocumentoDeTransporteAnteriorCTe documento = new Dominio.Entidades.DocumentoDeTransporteAnteriorCTe();

                                        documento.Chave = doc.Chave;
                                        documento.CTe = cargaCTE.CTe;

                                        DateTime dataEmissao;
                                        DateTime.TryParseExact(doc.DataEmissao, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
                                        double cpfCnpj = 0;
                                        if (doc.Emissor != null)
                                            double.TryParse(Utilidades.String.OnlyNumbers(doc.Emissor.CPFCNPJ), out cpfCnpj);

                                        if (dataEmissao != DateTime.MinValue)
                                            documento.DataEmissao = dataEmissao;

                                        documento.Emissor = cpfCnpj > 0 ? repCliente.BuscarPorCPFCNPJ(cpfCnpj) : null;
                                        documento.Numero = doc.Numero;
                                        documento.Serie = doc.Serie;
                                        documento.Tipo = doc.Tipo;

                                        repDocumento.Inserir(documento);

                                        unitOfWork.CommitChanges();

                                        unitOfWork.FlushAndClear();
                                    }
                                }
                                cargaCTE = repCargaCTe.BuscarPorCodigo(cargaCTE.Codigo);
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
                                if (cargaPedido.CargaPedidoFilialEmissora || (cargaPedido.CargaPedidoTrechoAnterior?.CargaPedidoFilialEmissora ?? false))
                                {
                                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoPedidoNotaFiscal in pedidoCTeParaSubContratacaoNotasFiscais)
                                    {
                                        Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscalNFS = pedidoCTeParaSubContratacaoPedidoNotaFiscal.PedidoXMLNotaFiscal;
                                        if (pedidoXMLNotaFiscalNFS.DocsParaEmissaoNFSManual.Count > 0)
                                            continue;

                                        unitOfWork.Start();
                                        carga.AgNFSManual = true;

                                        serNFS.CriarNFPendenteEmissaoManualDeNFS(carga, pedidoXMLNotaFiscalNFS, cargaPedido.ObterTomador(), terminoPrestacao, unitOfWork);

                                        unitOfWork.CommitChanges();
                                    }
                                }
                                else
                                {
                                    if (pedidosCTeParaSubContratacao.FirstOrDefault().DocsParaEmissaoNFSManual.Count > 0)
                                        continue;

                                    unitOfWork.Start();

                                    Dominio.Entidades.Cliente tomador = ctesParaSubcontratacao.FirstOrDefault().Emitente?.Cliente;

                                    if (tomador == null)
                                        tomador = cargaPedido.Pedido.SubContratante != null ? cargaPedido.Pedido.SubContratante : cargaPedido.Tomador;

                                    carga.AgNFSManual = true;
                                    serNFS.CriarCTePendenteEmissaoManualDeNFS(carga, pedidosCTeParaSubContratacao.FirstOrDefault(), tomador, inicioPrestacao, pedidosCTeParaSubContratacao.FirstOrDefault().ValorFrete, unitOfWork);
                                    unitOfWork.CommitChanges();
                                }
                            }

                            if (totalDocumentosGerados == 1 || (totalDocumentosGerados % quantidadeDocumentosAtualizarCarga) == 0)
                                svcHubCarga.InformarQuantidadeDocumentosEmitidos(carga.Codigo, totalDocumentos, totalDocumentosGerados);

                            unitOfWork.FlushAndClear();
                        }
                    }

                }
                else
                {
                    Servicos.Embarcador.CTe.CTEsImportados serCTEsImportados = new Servicos.Embarcador.CTe.CTEsImportados(unitOfWork);
                    serCTEsImportados.VincularCargaCTeEmitidoAnteriormente(carga, cargaPedido, unitOfWork);
                }

                if (emitiu)
                    svcHubCarga.InformarQuantidadeDocumentosEmitidos(carga.Codigo, totalDocumentos, totalDocumentosGerados);
            }
        }

        public void GerarCTePorRemetenteDestinatario(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, int totalDocumentos, int quantidadeDocumentosAtualizarCarga, ref int totalDocumentosGerados, bool ratearMesmoQueMultiModal, bool ratearPorTerminais, bool ratearPorBUS, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao> pedidosCTeParaSubContratacaoContaContabilContabilizacao)
        {
            CTe serCte = new CTe(unitOfWork);
            CTeSubContratacaoEmissao serCTeSubContratacaoEmissao = new CTeSubContratacaoEmissao(unitOfWork);
            Servicos.Embarcador.Carga.NFS serNFS = new NFS(unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroQuantidade repCTeParaSubContratacaoQuantidade = new Repositorio.Embarcador.CTe.CTeTerceiroQuantidade(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroSeguro repCTeParaSubContratacaoSeguro = new Repositorio.Embarcador.CTe.CTeTerceiroSeguro(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repPedidoCTeParaSubContratacaoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.CTe.CTeDocumentoOriginario repDocumentoOriginario = new Repositorio.Embarcador.CTe.CTeDocumentoOriginario(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            CTePorPedido serCTePorPedido = new CTePorPedido(unitOfWork);
            Hubs.Carga svcHubCarga = new Hubs.Carga();

            for (int i = 0; i < cargaPedidos.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigoComFetchEmpresa(cargaPedidos[i].Codigo);

                if (!cargaPedido.PossuiCTe && !cargaPedido.PossuiNFS && !cargaPedido.PossuiNFSManual && !(cargaPedido.PossuiNFSManual && cargaPedido.ModeloDocumentoFiscalIntramunicipal != null))
                    continue;

                bool emitiu = false;

                if (!cargaPedido.CTeEmitidoNoEmbarcador && !cargaPedido.Pedido.PedidoTransbordo)
                {
                    serCte.ObterDescricoesComponentesPadrao(cargaPedido.ObterTomador(), carga, configuracaoEmbarcador, unitOfWork, out string descricaoComponenteValorFrete, out string descricaoComponenteValorICMS);

                    List<string> cnpjsDestinatario = repPedidoCTeParaSubContratacao.BuscarCodigosDestinatarioPorCargaPedido(cargaPedido.Codigo);
                    List<string> cnpjsRemetente = repPedidoCTeParaSubContratacao.BuscarCodigosRemetentePorCargaPedido(cargaPedido.Codigo);

                    foreach (var cnpjRemetente in cnpjsRemetente)
                    {
                        foreach (var cnpjDestinatario in cnpjsDestinatario)
                        {
                            List<int> pedidoCTesParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarCodigosPorDestinatarioRemetente(cargaPedido.Codigo, cnpjDestinatario, cnpjRemetente);

                            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidosCTeParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarPorCodigosComFetch(pedidoCTesParaSubContratacao);
                            if (pedidosCTeParaSubContratacao != null && pedidosCTeParaSubContratacao.Count > 0)
                            {
                                Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalCarga = pedidosCTeParaSubContratacao.FirstOrDefault().ModeloDocumentoFiscal;

                                if (modeloDocumentoFiscalCarga == null)
                                    modeloDocumentoFiscalCarga = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.CTe);

                                List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> pedidoCTeParaSubContratacaoNotasFiscais = repPedidoCTeParaSubContratacaoNotaFiscal.BuscarPorPedidoCTeParaSubcontratacao(pedidoCTesParaSubContratacao);
                                Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorCodigo(pedidoCTeParaSubContratacaoNotasFiscais.First().PedidoXMLNotaFiscal.Codigo);

                                List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesParaSubcontratacao = pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro).ToList();

                                Dominio.Entidades.Cliente expedidor = null;
                                Dominio.Entidades.Cliente recebedor = null;
                                Dominio.Entidades.Localidade inicioPrestacao = ctesParaSubcontratacao.FirstOrDefault().LocalidadeInicioPrestacao;
                                Dominio.Entidades.Localidade terminoPrestacao = ctesParaSubcontratacao.FirstOrDefault().LocalidadeTerminoPrestacao;
                                Dominio.Entidades.Cliente remetente = repCliente.BuscarPorCPFCNPJ((ctesParaSubcontratacao.FirstOrDefault().Remetente.Cliente.CPF_CNPJ));
                                Dominio.Entidades.Cliente destinatario = repCliente.BuscarPorCPFCNPJ((ctesParaSubcontratacao.FirstOrDefault().Destinatario.Cliente.CPF_CNPJ));

                                if (pedidosCTeParaSubContratacao.FirstOrDefault().CargaPedido.Expedidor != null)
                                {
                                    expedidor = pedidosCTeParaSubContratacao.FirstOrDefault().CargaPedido.Expedidor;
                                    if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
                                        inicioPrestacao = expedidor.Localidade;
                                }

                                if (pedidosCTeParaSubContratacao.FirstOrDefault().CargaPedido.Recebedor != null)
                                {
                                    recebedor = pedidosCTeParaSubContratacao.FirstOrDefault().CargaPedido.Recebedor;
                                    if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
                                        terminoPrestacao = recebedor.Localidade;
                                }

                                if (!pedidosCTeParaSubContratacao.FirstOrDefault().PossuiNFSManual ||
                                    (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS &&
                                    (inicioPrestacao.Codigo != terminoPrestacao.Codigo || (pedidosCTeParaSubContratacao.FirstOrDefault().PossuiNFSManual && cargaPedido.ModeloDocumentoFiscalIntramunicipal != null && inicioPrestacao.Codigo == terminoPrestacao.Codigo))))
                                {
                                    bool somenteCTeSubContratacaoFilialEmissora = false;

                                    carga = repCarga.BuscarPorCodigo(carga.Codigo);

                                    if (cargaPedido.CargaPedidoFilialEmissora && carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                                        somenteCTeSubContratacaoFilialEmissora = true;

                                    //if (repCargaPedidoXMLNotaFiscalCTe.ContarCTesPorNotasFiscais(pedidoCTeParaSubContratacaoNotasFiscais.Select(o => o.PedidoXMLNotaFiscal.Codigo).ToArray(), somenteCTeSubContratacaoFilialEmissora) > 0)  //pedidoXMLNotaFiscal.CTes.Count > 0) //já foi gerado um ct-e para esta nota
                                    if (repCargaPedidoXMLNotaFiscalCTe.ContarCTesPorPedidoCTeParaSubContratacao(pedidoCTesParaSubContratacao, somenteCTeSubContratacaoFilialEmissora) > 0)
                                        continue;

                                    Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoEmitir = null;

                                    if (pedidosCTeParaSubContratacao.FirstOrDefault().PossuiNFSManual)
                                        modeloDocumentoEmitir = cargaPedido.ModeloDocumentoFiscalIntramunicipal;
                                    else
                                        modeloDocumentoEmitir = modeloDocumentoFiscalCarga;

                                    unitOfWork.Start();

                                    emitiu = true;
                                    totalDocumentosGerados++;
                                    Dominio.ObjetosDeValor.CTe.CTe cte = new Dominio.ObjetosDeValor.CTe.CTe();
                                    cte.Documentos = new List<Dominio.ObjetosDeValor.CTe.Documento>();

                                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoNotaFiscal in pedidoCTeParaSubContratacaoNotasFiscais)
                                    {
                                        Dominio.ObjetosDeValor.CTe.Documento docNF = serCte.BuscarDocumentoCTe(pedidoCTeParaSubContratacaoNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal, carga.Empresa.TipoAmbiente, configuracaoEmbarcador);
                                        cte.Documentos.Add(docNF);
                                    }

                                    cte.PercentualPagamentoAgregado = pedidosCTeParaSubContratacao.FirstOrDefault().PercentualPagamentoAgregado;

                                    List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> cargaPedidoComponentesFretesCliente = BuscarPedidoCTeSubcontratacaoComponentesFrete(pedidosCTeParaSubContratacao, modeloDocumentoFiscalCarga, unitOfWork);

                                    cte.ValorTotalMercadoria = ctesParaSubcontratacao.Sum(o => o.ValorTotalMercadoria);

                                    decimal valorFreteNegociado = repPedidoCTeParaSubContratacao.ObterValorFreteNegociavelPedido(cargaPedido.Codigo);
                                    decimal valorCusteioSVM = repPedidoCTeParaSubContratacao.ObterValorCusteioSVMPedido(cargaPedido.Codigo);
                                    decimal valorICMS = pedidosCTeParaSubContratacao.Sum(o => o.ValorICMS);
                                    decimal aliquota = repPedidoCTeParaSubContratacao.ObterAliquota(cargaPedido.Codigo);

                                    cargaPedido.Pedido.NumeroBooking = repPedidoCTeParaSubContratacao.ObterNumeroBookingPedido(pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).FirstOrDefault());
                                    cargaPedido.Pedido.DescricaoCarrierNavioViagem = repPedidoCTeParaSubContratacao.ObterDescricaoCarrierPedido(pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).FirstOrDefault());
                                    cargaPedido.Pedido.TipoPropostaFeeder = repPedidoCTeParaSubContratacao.ObterTipoPropostaFeederPedido(pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).FirstOrDefault());

                                    Repositorio.Embarcador.Pedidos.PedidoTransbordo repPedidoTransbordo = new Repositorio.Embarcador.Pedidos.PedidoTransbordo(unitOfWork);
                                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo> transbordosAnteriores = repPedidoTransbordo.BuscarPorPedido(cargaPedido.Pedido.Codigo);
                                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo> transbordos = repPedidoCTeParaSubContratacao.ObterTransbordosPedido(pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).FirstOrDefault());
                                    if (transbordos != null && transbordos.Count > 0)
                                    {
                                        if (transbordosAnteriores != null && transbordosAnteriores.Count > 0)
                                        {
                                            foreach (var transbordo in transbordosAnteriores)
                                            {
                                                repPedidoTransbordo.Deletar(transbordo);
                                            }
                                        }
                                        foreach (var transbordo in transbordos)
                                        {
                                            Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo trans = new Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo()
                                            {
                                                Navio = transbordo.Navio,
                                                Pedido = cargaPedido.Pedido,
                                                PedidoViagemNavio = transbordo.PedidoViagemNavio,
                                                Porto = transbordo.Porto,
                                                Sequencia = transbordo.Sequencia,
                                                Terminal = transbordo.Terminal
                                            };
                                            repPedidoTransbordo.Inserir(trans);
                                        }
                                    }

                                    repPedido.Atualizar(cargaPedido.Pedido);

                                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = repPedidoCTeParaSubContratacao.ObterCargaPedido(cargaPedido.Codigo);
                                    int qtdContainer = cargasPedidos != null && cargasPedidos.Count > 0 ? cargasPedidos.Select(p => p.Pedido.Container.Codigo).Distinct().Count() : 1;
                                    List<int> codigosContainer = new List<int>();
                                    codigosContainer.AddRange(cargasPedidos.Select(p => p.Pedido.Container.Codigo).Distinct().ToList());
                                    decimal valorFrete = 1;
                                    decimal baseICMS = ((100 - aliquota) / 100);
                                    valorFrete = CalcularValorSVM(cargaPedido.TipoPropostaMultimodal, pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).FirstOrDefault(), pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).ToList(),
                                        pedidosCTeParaSubContratacao.Count(), valorCusteioSVM, valorFreteNegociado, baseICMS, "", cargaPedido.Pedido.NumeroBooking, unitOfWork, qtdContainer, codigosContainer);


                                    cte.ValorAReceber = valorFrete;//pedidoCTeParaSubContratacao.ValorFrete;
                                    cte.ValorFrete = valorFrete;// pedidoCTeParaSubContratacao.ValorFrete;
                                    cte.ValorTotalPrestacaoServico = valorFrete;// pedidoCTeParaSubContratacao.ValorFrete;

                                    if (cargaPedido.Pedido.EmpresaSerie != null)
                                        cte.Serie = cargaPedido.Pedido.EmpresaSerie.Numero;

                                    cte.ProdutoPredominante = ctesParaSubcontratacao.FirstOrDefault().ProdutoPredominante;

                                    List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade> quantidades = repCTeParaSubContratacaoQuantidade.BuscarPorCTeParaSubContratacao(ctesParaSubcontratacao.Select(o => o.Codigo).ToList());
                                    List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroSeguro> seguros = repCTeParaSubContratacaoSeguro.BuscarPorCTeParaSubContratacao(ctesParaSubcontratacao.Select(o => o.Codigo).ToList());

                                    Dominio.Entidades.Cliente tomador = ctesParaSubcontratacao.FirstOrDefault().Emitente?.Cliente;

                                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && modeloDocumentoEmitir != null && modeloDocumentoEmitir.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe && cargaPedido.Tomador != null)
                                        tomador = cargaPedido.Tomador;

                                    if (tomador == null)
                                        tomador = cargaPedido.Pedido.SubContratante != null ? cargaPedido.Pedido.SubContratante : cargaPedido.Tomador;

                                    Dominio.Enumeradores.TipoPagamento tipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros; //cargaPedido.Pedido.TipoPagamento; //Dominio.Enumeradores.TipoPagamento.Outros;//cargaPedido.Pedido.TipoPagamento;
                                    Dominio.Enumeradores.TipoTomador tipoTomador = Dominio.Enumeradores.TipoTomador.Outros; //cargaPedido.TipoTomador; //Dominio.Enumeradores.TipoTomador.Outros;
                                    List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesParaSubcontrataca = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>();
                                    foreach (var cteParaSubcontratacao in ctesParaSubcontratacao)
                                    {
                                        if (!ctesParaSubcontrataca.Contains(cteParaSubcontratacao))
                                            ctesParaSubcontrataca.Add(cteParaSubcontratacao);
                                    }

                                    List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidadesCTes = ConverterQuantidadesTerceirosParaQuantidadesCTe(quantidades);

                                    bool emitindoCTeFilialEmissora = false;
                                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeFilialEmissora = pedidosCTeParaSubContratacao.FirstOrDefault().CTeTerceiro.ObterCargaCTe(carga.Codigo);

                                    if (cargaPedido.CargaPedidoFilialEmissora)
                                        emitindoCTeFilialEmissora = true;

                                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga tipoContratacaoCarga = cargaPedido.TipoContratacaoCarga;

                                    if (emitindoCTeFilialEmissora)
                                        tipoContratacaoCarga = cargaPedido.TipoContratacaoCargaSubContratacaoFilialEmissora;

                                    Dominio.Enumeradores.TipoServico tipoServico = Dominio.Enumeradores.TipoServico.SubContratacao;

                                    if (carga.CargaSVM)
                                        tipoServico = Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal;
                                    else if (tipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho)
                                        tipoServico = Dominio.Enumeradores.TipoServico.Redespacho;
                                    else if (tipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario)
                                        tipoServico = Dominio.Enumeradores.TipoServico.RedIntermediario;

                                    List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ctesAnteriores = ConverterCTesTerceirosParaAnteriores(ctesParaSubcontrataca);
                                    List<string> chavesCTes = new List<string>();
                                    if (ctesAnteriores != null && ctesAnteriores.Count > 0)
                                        chavesCTes = ctesAnteriores.Select(o => o.Chave).ToList();

                                    Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = BuscarRegraICMS(cargaPedido.CargaOrigem, inicioPrestacao, terminoPrestacao, pedidosCTeParaSubContratacao.FirstOrDefault(), unitOfWork);
                                    Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = BuscarRegraISS(cargaPedido.CargaOrigem, pedidosCTeParaSubContratacao.FirstOrDefault(), unitOfWork);
                                    Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS();

                                    cargaPedido = repCargaPedido.BuscarPorCodigo(cargaPedido.Codigo);

                                    cargaPedido.ValorFrete = valorFrete;
                                    cargaPedido.ValorFreteAPagar = valorFrete;
                                    cargaPedido.ValorICMS = 0;
                                    cargaPedido.PercentualAliquota = 0;
                                    cargaPedido.BaseCalculoICMS = 0;
                                    cargaPedido.CST = "41";
                                    cargaPedido.CFOP = repCFOP.BuscarPorNumero(regraICMS.CFOP);

                                    repCargaPedido.Atualizar(cargaPedido);

                                    List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> apoliceSeguro = ConverterSegurosDeTerceirosEmSeguroCTe(seguros);

                                    string observacaoCTe = null;
                                    Dominio.Enumeradores.TipoCTE tipoCTe = Dominio.Enumeradores.TipoCTE.Normal;

                                    double.TryParse(cnpjDestinatario, out double cnpjDestinatarioContainer);
                                    List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil> configuracoes = Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil.ConverterPedidoCTeParaSubContratacaoContaContabilContabilizacao(pedidosCTeParaSubContratacao.FirstOrDefault(), pedidosCTeParaSubContratacaoContaContabilContabilizacao);
                                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTE = serCte.GerarCTe(carga, cargaPedido, cte, cargaPedido.CargaOrigem.Empresa, remetente, destinatario, tomador, expedidor, recebedor, inicioPrestacao, terminoPrestacao, null, null, tipoPagamento, tipoTomador, quantidadesCTes, cargaPedidoComponentesFretesCliente, observacaoCTe, pedidoXMLNotaFiscal.CargaPedido.Pedido.ObservacaoCTeTerceiro, pedidoXMLNotaFiscal.CargaPedido.Pedido, null, apoliceSeguro, regraICMS, regraISS, impostoIBSCBS, tipoServicoMultisoftware, modeloDocumentoEmitir, tipoServico, tipoCTe, ctesAnteriores, 0, false, cargaCTeFilialEmissora, false, unitOfWork, configuracaoEmbarcador, configuracaoGeralCarga, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultado, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultadoDestinatario, pedidosCTeParaSubContratacao.FirstOrDefault().ItemServico, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultadoEscrituracao, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultadoICMS, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultadoPIS, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultadoCOFINS, pedidosCTeParaSubContratacao.FirstOrDefault().ValorMaximoCentroContabilizacao, configuracoes, pedidoCTeParaSubContratacaoNotasFiscais.Select(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal).ToList(), null, descricaoComponenteValorICMS, descricaoComponenteValorFrete, null, null, cargasPedidos, cnpjDestinatarioContainer, chavesCTes, null, null, null, null, true);
                                    SalvarNumeroControleSVM(cargaPedido.Codigo, cargaCTE.CTe?.NumeroControle ?? "", unitOfWork, cargaCTE.CTe?.Codigo ?? 0);

                                    if (emitindoCTeFilialEmissora && cargaCTeFilialEmissora != null)
                                    {
                                        Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                                        cargaCTeFilialEmissora.CargaCTeSubContratacaoFilialEmissora = cargaCTE;
                                        repCargaCte.Atualizar(cargaCTeFilialEmissora);
                                    }
                                    List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> apoliceSeguroAverbacaos = repApoliceSeguroAverbacao.BuscarPorCargaPedido(cargaPedido.Codigo, emitindoCTeFilialEmissora);
                                    serCte.AverbaCargaCTe(cargaCTE, apoliceSeguroAverbacaos, unitOfWork, configuracaoEmbarcador, cargaPedido.Pedido.FormaAverbacaoCTE);
                                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoNotaFiscal in pedidoCTeParaSubContratacaoNotasFiscais)
                                    {
                                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTE = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe();
                                        cargaPedidoXMLNotaFiscalCTE.CargaCTe = cargaCTE;
                                        cargaPedidoXMLNotaFiscalCTE.PedidoXMLNotaFiscal = pedidoCTeParaSubContratacaoNotaFiscal.PedidoXMLNotaFiscal;
                                        repCargaPedidoXMLNotaFiscalCTe.Inserir(cargaPedidoXMLNotaFiscalCTE);
                                    }

                                    bool inseriuOBS = false;
                                    if (cargaCTE.CTe != null)
                                    {
                                        foreach (var cteParaSubcontratacao in ctesParaSubcontratacao)
                                        {
                                            Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario documentoOriginario = new Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario()
                                            {
                                                CTe = cargaCTE.CTe,
                                                Chave = cteParaSubcontratacao.ChaveAcesso,
                                                Numero = cteParaSubcontratacao.Numero,
                                                Serie = cteParaSubcontratacao.Serie,
                                                DataEmissao = cteParaSubcontratacao.DataEmissao,
                                                NumeroMinuta = cteParaSubcontratacao.NumeroMinuta,
                                                NumeroOperacionalConhecimentoAereo = cteParaSubcontratacao.NumeroOperacionalConhecimentoAereo
                                            };
                                            repDocumentoOriginario.Inserir(documentoOriginario);

                                            if (!inseriuOBS && cargaCTE.CTe.SVMProprio && cargaCTE.CTe.PortoOrigem != null && cargaCTE.CTe.PortoOrigem != null && cargaCTE.CTe.PortoOrigem.AtivarDespachanteComoConsignatario && cargaCTE.CTe.PortoDestino.AtivarDespachanteComoConsignatario)
                                            {
                                                string chaveCTeMultimodal = documentoOriginario.Chave;
                                                if (!string.IsNullOrWhiteSpace(chaveCTeMultimodal))
                                                {
                                                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cteMultimodal = repCTe.BuscarPorChave(chaveCTeMultimodal);
                                                    if (cteMultimodal != null && cteMultimodal.TomadorPagador != null && cteMultimodal.TomadorPagador.GrupoPessoas != null && cteMultimodal.TomadorPagador.GrupoPessoas.AdicionarDespachanteComoConsignatario && cteMultimodal.TomadorPagador.GrupoPessoas.Despachante != null)
                                                    {
                                                        cargaCTE.CTe.ObservacoesGerais += " A " + cteMultimodal.TomadorPagador.GrupoPessoas.Despachante.Nome + ", CNPJ " + cteMultimodal.TomadorPagador.GrupoPessoas.Despachante.CPF_CNPJ_Formatado + ", irá atuar como despachante para a liberação dessa carga no porto de descarga.";
                                                        cargaCTE.CTe.ObservacoesGerais = cargaCTE.CTe.ObservacoesGerais.Trim();
                                                        cargaCTE.CTe.EmailDespachanteSVM = cteMultimodal.TomadorPagador.GrupoPessoas.EmailDespachante;
                                                        repCTe.Atualizar(cargaCTE.CTe);
                                                    }
                                                }
                                                inseriuOBS = true;
                                            }
                                        }
                                    }

                                    if (pedidosCTeParaSubContratacao.FirstOrDefault().PossuiNFSManual)
                                    {
                                        serNFS.CriarCargaCTePendenteEmissaoManualDeNFS(carga, cargaCTE, inicioPrestacao, unitOfWork);

                                        carga.AgNFSManual = true;
                                    }

                                    unitOfWork.CommitChanges();
                                    if (cte != null && cte.DocumentosTransporteAnteriores != null && cargaCTE.CTe.TipoServico == Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal && cargaCTE.CTe.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario)
                                    {
                                        Repositorio.DocumentoDeTransporteAnteriorCTe repDocumento = new Repositorio.DocumentoDeTransporteAnteriorCTe(unitOfWork);
                                        Servicos.Log.TratarErro(cargaCTE.CTe.Codigo.ToString() + " - Iniciou SalvarInformacoesDocumentosAnteriores SVM - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "TempoEmissaoCTe");
                                        foreach (var doc in cte.DocumentosTransporteAnteriores)
                                        {
                                            unitOfWork.Start();

                                            Dominio.Entidades.DocumentoDeTransporteAnteriorCTe documento = new Dominio.Entidades.DocumentoDeTransporteAnteriorCTe();

                                            documento.Chave = doc.Chave;
                                            documento.CTe = cargaCTE.CTe;

                                            DateTime dataEmissao;
                                            DateTime.TryParseExact(doc.DataEmissao, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
                                            double cpfCnpj = 0;
                                            if (doc.Emissor != null)
                                                double.TryParse(Utilidades.String.OnlyNumbers(doc.Emissor.CPFCNPJ), out cpfCnpj);

                                            if (dataEmissao != DateTime.MinValue)
                                                documento.DataEmissao = dataEmissao;

                                            documento.Emissor = cpfCnpj > 0 ? repCliente.BuscarPorCPFCNPJ(cpfCnpj) : null;
                                            documento.Numero = doc.Numero;
                                            documento.Serie = doc.Serie;
                                            documento.Tipo = doc.Tipo;

                                            repDocumento.Inserir(documento);

                                            unitOfWork.CommitChanges();

                                            unitOfWork.FlushAndClear();
                                        }
                                    }
                                    cargaCTE = repCargaCTe.BuscarPorCodigo(cargaCTE.Codigo);
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
                                    if (cargaPedido.CargaPedidoFilialEmissora || (cargaPedido.CargaPedidoTrechoAnterior?.CargaPedidoFilialEmissora ?? false))
                                    {
                                        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoPedidoNotaFiscal in pedidoCTeParaSubContratacaoNotasFiscais)
                                        {
                                            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscalNFS = pedidoCTeParaSubContratacaoPedidoNotaFiscal.PedidoXMLNotaFiscal;
                                            if (pedidoXMLNotaFiscalNFS.DocsParaEmissaoNFSManual.Count > 0)
                                                continue;

                                            unitOfWork.Start();
                                            carga.AgNFSManual = true;

                                            serNFS.CriarNFPendenteEmissaoManualDeNFS(carga, pedidoXMLNotaFiscalNFS, cargaPedido.ObterTomador(), terminoPrestacao, unitOfWork);

                                            unitOfWork.CommitChanges();
                                        }
                                    }
                                    else
                                    {
                                        if (pedidosCTeParaSubContratacao.FirstOrDefault().DocsParaEmissaoNFSManual.Count > 0)
                                            continue;

                                        unitOfWork.Start();

                                        Dominio.Entidades.Cliente tomador = ctesParaSubcontratacao.FirstOrDefault().Emitente?.Cliente;

                                        if (tomador == null)
                                            tomador = cargaPedido.Pedido.SubContratante != null ? cargaPedido.Pedido.SubContratante : cargaPedido.Tomador;

                                        carga.AgNFSManual = true;
                                        serNFS.CriarCTePendenteEmissaoManualDeNFS(carga, pedidosCTeParaSubContratacao.FirstOrDefault(), tomador, inicioPrestacao, pedidosCTeParaSubContratacao.FirstOrDefault().ValorFrete, unitOfWork);
                                        unitOfWork.CommitChanges();
                                    }
                                }

                                if (totalDocumentosGerados == 1 || (totalDocumentosGerados % quantidadeDocumentosAtualizarCarga) == 0)
                                    svcHubCarga.InformarQuantidadeDocumentosEmitidos(carga.Codigo, totalDocumentos, totalDocumentosGerados);

                                unitOfWork.FlushAndClear();
                            }
                        }
                    }

                }
                else
                {
                    Servicos.Embarcador.CTe.CTEsImportados serCTEsImportados = new Servicos.Embarcador.CTe.CTEsImportados(unitOfWork);
                    serCTEsImportados.VincularCargaCTeEmitidoAnteriormente(carga, cargaPedido, unitOfWork);
                }

                if (emitiu)
                    svcHubCarga.InformarQuantidadeDocumentosEmitidos(carga.Codigo, totalDocumentos, totalDocumentosGerados);
            }
        }

        public void GerarCTePorTerminalSacado(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, int totalDocumentos, int quantidadeDocumentosAtualizarCarga, ref int totalDocumentosGerados, bool ratearPorTerminais, bool ratearPorBUS, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao> pedidosCTeParaSubContratacaoContaContabilContabilizacao)
        {
            CTe serCte = new CTe(unitOfWork);
            CTeSubContratacaoEmissao serCTeSubContratacaoEmissao = new CTeSubContratacaoEmissao(unitOfWork);
            Servicos.Embarcador.Carga.NFS serNFS = new NFS(unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroQuantidade repCTeParaSubContratacaoQuantidade = new Repositorio.Embarcador.CTe.CTeTerceiroQuantidade(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroSeguro repCTeParaSubContratacaoSeguro = new Repositorio.Embarcador.CTe.CTeTerceiroSeguro(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repPedidoCTeParaSubContratacaoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.CTe.CTeDocumentoOriginario repDocumentoOriginario = new Repositorio.Embarcador.CTe.CTeDocumentoOriginario(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            CTePorPedido serCTePorPedido = new CTePorPedido(unitOfWork);
            Hubs.Carga svcHubCarga = new Hubs.Carga();

            for (int i = 0; i < cargaPedidos.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigoComFetchEmpresa(cargaPedidos[i].Codigo);

                if (!cargaPedido.PossuiCTe && !cargaPedido.PossuiNFS && !cargaPedido.PossuiNFSManual && !(cargaPedido.PossuiNFSManual && cargaPedido.ModeloDocumentoFiscalIntramunicipal != null))
                    continue;

                bool emitiu = false;

                if (!cargaPedido.CTeEmitidoNoEmbarcador && !cargaPedido.Pedido.PedidoTransbordo)
                {
                    serCte.ObterDescricoesComponentesPadrao(cargaPedido.ObterTomador(), carga, configuracaoEmbarcador, unitOfWork, out string descricaoComponenteValorFrete, out string descricaoComponenteValorICMS);

                    List<int> codigosTerminalOrigem = repPedidoCTeParaSubContratacao.BuscarCodigosTerminaisOrigemPorCargaPedido(cargaPedido.Codigo);
                    List<int> codigosTerminalDestino = repPedidoCTeParaSubContratacao.BuscarCodigosTerminaisDestinoPorCargaPedido(cargaPedido.Codigo);
                    List<string> cnpjsTomador = repPedidoCTeParaSubContratacao.BuscarCodigosTomadorPorCargaPedido(cargaPedido.Codigo);
                    cnpjsTomador = cnpjsTomador.Distinct().ToList();

                    foreach (var codigoTerminalOrigem in codigosTerminalOrigem)
                    {
                        foreach (var codigoTerminalDestino in codigosTerminalDestino)
                        {
                            foreach (var cnpjTomador in cnpjsTomador)
                            {
                                List<int> pedidoCTesParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarCodigosPorCargaPedidoTerminalOrigemTerminalDestinoSacado(cargaPedido.Codigo, codigoTerminalOrigem, codigoTerminalDestino, cnpjTomador);

                                List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidosCTeParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarPorCodigosComFetch(pedidoCTesParaSubContratacao);
                                if (pedidosCTeParaSubContratacao != null && pedidosCTeParaSubContratacao.Count > 0)
                                {
                                    Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalCarga = pedidosCTeParaSubContratacao.FirstOrDefault().ModeloDocumentoFiscal;

                                    if (modeloDocumentoFiscalCarga == null)
                                        modeloDocumentoFiscalCarga = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.CTe);

                                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> pedidoCTeParaSubContratacaoNotasFiscais = repPedidoCTeParaSubContratacaoNotaFiscal.BuscarPorPedidoCTeParaSubcontratacao(pedidoCTesParaSubContratacao);
                                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorCodigo(pedidoCTeParaSubContratacaoNotasFiscais.First().PedidoXMLNotaFiscal.Codigo);

                                    List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesParaSubcontratacao = pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro).ToList();

                                    Dominio.Entidades.Cliente expedidor = null;
                                    Dominio.Entidades.Cliente recebedor = null;
                                    Dominio.Entidades.Localidade inicioPrestacao = ctesParaSubcontratacao.FirstOrDefault().LocalidadeInicioPrestacao;
                                    Dominio.Entidades.Localidade terminoPrestacao = ctesParaSubcontratacao.FirstOrDefault().LocalidadeTerminoPrestacao;
                                    Dominio.Entidades.Cliente remetente = repCliente.BuscarPorCPFCNPJ((ctesParaSubcontratacao.FirstOrDefault().Remetente.Cliente.CPF_CNPJ));
                                    Dominio.Entidades.Cliente destinatario = repCliente.BuscarPorCPFCNPJ((ctesParaSubcontratacao.FirstOrDefault().Destinatario.Cliente.CPF_CNPJ));

                                    if (pedidosCTeParaSubContratacao.FirstOrDefault().CargaPedido.Expedidor != null)
                                    {
                                        expedidor = pedidosCTeParaSubContratacao.FirstOrDefault().CargaPedido.Expedidor;
                                        if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
                                            inicioPrestacao = expedidor.Localidade;
                                    }

                                    if (pedidosCTeParaSubContratacao.FirstOrDefault().CargaPedido.Recebedor != null)
                                    {
                                        recebedor = pedidosCTeParaSubContratacao.FirstOrDefault().CargaPedido.Recebedor;
                                        if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
                                            terminoPrestacao = recebedor.Localidade;
                                    }

                                    if (!pedidosCTeParaSubContratacao.FirstOrDefault().PossuiNFSManual ||
                                        (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS &&
                                        (inicioPrestacao.Codigo != terminoPrestacao.Codigo || (pedidosCTeParaSubContratacao.FirstOrDefault().PossuiNFSManual && cargaPedido.ModeloDocumentoFiscalIntramunicipal != null && inicioPrestacao.Codigo == terminoPrestacao.Codigo))))
                                    {
                                        bool somenteCTeSubContratacaoFilialEmissora = false;

                                        carga = repCarga.BuscarPorCodigo(carga.Codigo);

                                        if (cargaPedido.CargaPedidoFilialEmissora && carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                                            somenteCTeSubContratacaoFilialEmissora = true;

                                        //if (repCargaPedidoXMLNotaFiscalCTe.ContarCTesPorNotasFiscais(pedidoCTeParaSubContratacaoNotasFiscais.Select(o => o.PedidoXMLNotaFiscal.Codigo).ToArray(), somenteCTeSubContratacaoFilialEmissora) > 0)  //pedidoXMLNotaFiscal.CTes.Count > 0) //já foi gerado um ct-e para esta nota
                                        if (repCargaPedidoXMLNotaFiscalCTe.ContarCTesPorPedidoCTeParaSubContratacao(pedidoCTesParaSubContratacao, somenteCTeSubContratacaoFilialEmissora) > 0)
                                            continue;

                                        Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoEmitir = null;

                                        if (pedidosCTeParaSubContratacao.FirstOrDefault().PossuiNFSManual)
                                            modeloDocumentoEmitir = cargaPedido.ModeloDocumentoFiscalIntramunicipal;
                                        else
                                            modeloDocumentoEmitir = modeloDocumentoFiscalCarga;

                                        unitOfWork.Start();

                                        emitiu = true;
                                        totalDocumentosGerados++;
                                        Dominio.ObjetosDeValor.CTe.CTe cte = new Dominio.ObjetosDeValor.CTe.CTe();
                                        cte.Documentos = new List<Dominio.ObjetosDeValor.CTe.Documento>();

                                        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoNotaFiscal in pedidoCTeParaSubContratacaoNotasFiscais)
                                        {
                                            Dominio.ObjetosDeValor.CTe.Documento docNF = serCte.BuscarDocumentoCTe(pedidoCTeParaSubContratacaoNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal, carga.Empresa.TipoAmbiente, configuracaoEmbarcador);
                                            cte.Documentos.Add(docNF);
                                        }

                                        cte.PercentualPagamentoAgregado = pedidosCTeParaSubContratacao.FirstOrDefault().PercentualPagamentoAgregado;

                                        List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> cargaPedidoComponentesFretesCliente = BuscarPedidoCTeSubcontratacaoComponentesFrete(pedidosCTeParaSubContratacao, modeloDocumentoFiscalCarga, unitOfWork);

                                        cte.ValorTotalMercadoria = ctesParaSubcontratacao.Sum(o => o.ValorTotalMercadoria);

                                        decimal valorFreteNegociado = repPedidoCTeParaSubContratacao.ObterValorFreteNegociavelPedido(cargaPedido.Codigo);
                                        decimal valorCusteioSVM = repPedidoCTeParaSubContratacao.ObterValorCusteioSVMPedido(cargaPedido.Codigo);
                                        decimal valorICMS = pedidosCTeParaSubContratacao.Sum(o => o.ValorICMS);
                                        decimal aliquota = repPedidoCTeParaSubContratacao.ObterAliquota(cargaPedido.Codigo);

                                        cargaPedido.Pedido.NumeroBooking = repPedidoCTeParaSubContratacao.ObterNumeroBookingPedido(pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).FirstOrDefault());
                                        cargaPedido.Pedido.DescricaoCarrierNavioViagem = repPedidoCTeParaSubContratacao.ObterDescricaoCarrierPedido(pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).FirstOrDefault());
                                        cargaPedido.Pedido.TipoPropostaFeeder = repPedidoCTeParaSubContratacao.ObterTipoPropostaFeederPedido(pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).FirstOrDefault());

                                        Repositorio.Embarcador.Pedidos.PedidoTransbordo repPedidoTransbordo = new Repositorio.Embarcador.Pedidos.PedidoTransbordo(unitOfWork);
                                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo> transbordosAnteriores = repPedidoTransbordo.BuscarPorPedido(cargaPedido.Pedido.Codigo);
                                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo> transbordos = repPedidoCTeParaSubContratacao.ObterTransbordosPedido(pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).FirstOrDefault());
                                        if (transbordos != null && transbordos.Count > 0)
                                        {
                                            if (transbordosAnteriores != null && transbordosAnteriores.Count > 0)
                                            {
                                                foreach (var transbordo in transbordosAnteriores)
                                                {
                                                    repPedidoTransbordo.Deletar(transbordo);
                                                }
                                            }
                                            foreach (var transbordo in transbordos)
                                            {
                                                Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo trans = new Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo()
                                                {
                                                    Navio = transbordo.Navio,
                                                    Pedido = cargaPedido.Pedido,
                                                    PedidoViagemNavio = transbordo.PedidoViagemNavio,
                                                    Porto = transbordo.Porto,
                                                    Sequencia = transbordo.Sequencia,
                                                    Terminal = transbordo.Terminal
                                                };
                                                repPedidoTransbordo.Inserir(trans);
                                            }
                                        }

                                        repPedido.Atualizar(cargaPedido.Pedido);

                                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = repPedidoCTeParaSubContratacao.ObterCargaPedido(cargaPedido.Codigo);
                                        int qtdContainer = cargasPedidos != null && cargasPedidos.Count > 0 ? cargasPedidos.Select(p => p.Pedido.Container.Codigo).Distinct().Count() : 1;
                                        List<int> codigosContainer = new List<int>();
                                        codigosContainer.AddRange(cargasPedidos.Select(p => p.Pedido.Container.Codigo).Distinct().ToList());
                                        decimal valorFrete = 1;
                                        decimal baseICMS = ((100 - aliquota) / 100);
                                        valorFrete = CalcularValorSVM(cargaPedido.TipoPropostaMultimodal, pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).FirstOrDefault(), pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).ToList(),
                                            pedidosCTeParaSubContratacao.Count(), valorCusteioSVM, valorFreteNegociado, baseICMS, "", cargaPedido.Pedido.NumeroBooking, unitOfWork, qtdContainer, codigosContainer);

                                        cte.ValorAReceber = valorFrete;//pedidoCTeParaSubContratacao.ValorFrete;
                                        cte.ValorFrete = valorFrete;// pedidoCTeParaSubContratacao.ValorFrete;
                                        cte.ValorTotalPrestacaoServico = valorFrete;// pedidoCTeParaSubContratacao.ValorFrete;

                                        if (cargaPedido.Pedido.EmpresaSerie != null)
                                            cte.Serie = cargaPedido.Pedido.EmpresaSerie.Numero;

                                        cte.ProdutoPredominante = ctesParaSubcontratacao.FirstOrDefault().ProdutoPredominante;

                                        List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade> quantidades = repCTeParaSubContratacaoQuantidade.BuscarPorCTeParaSubContratacao(ctesParaSubcontratacao.Select(o => o.Codigo).ToList());
                                        List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroSeguro> seguros = repCTeParaSubContratacaoSeguro.BuscarPorCTeParaSubContratacao(ctesParaSubcontratacao.Select(o => o.Codigo).ToList());

                                        Dominio.Entidades.Cliente tomador = ctesParaSubcontratacao.FirstOrDefault().Emitente?.Cliente;

                                        if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && modeloDocumentoEmitir != null && modeloDocumentoEmitir.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe && cargaPedido.Tomador != null)
                                            tomador = cargaPedido.Tomador;

                                        if (tomador == null)
                                            tomador = cargaPedido.Pedido.SubContratante != null ? cargaPedido.Pedido.SubContratante : cargaPedido.Tomador;

                                        Dominio.Enumeradores.TipoPagamento tipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros; //cargaPedido.Pedido.TipoPagamento; //Dominio.Enumeradores.TipoPagamento.Outros;//cargaPedido.Pedido.TipoPagamento;
                                        Dominio.Enumeradores.TipoTomador tipoTomador = Dominio.Enumeradores.TipoTomador.Outros; //cargaPedido.TipoTomador; //Dominio.Enumeradores.TipoTomador.Outros;
                                        List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesParaSubcontrataca = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>();
                                        foreach (var cteParaSubcontratacao in ctesParaSubcontratacao)
                                        {
                                            if (!ctesParaSubcontrataca.Contains(cteParaSubcontratacao))
                                                ctesParaSubcontrataca.Add(cteParaSubcontratacao);
                                        }

                                        List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidadesCTes = ConverterQuantidadesTerceirosParaQuantidadesCTe(quantidades);

                                        bool emitindoCTeFilialEmissora = false;
                                        Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeFilialEmissora = pedidosCTeParaSubContratacao.FirstOrDefault().CTeTerceiro.ObterCargaCTe(carga.Codigo);

                                        if (cargaPedido.CargaPedidoFilialEmissora)
                                            emitindoCTeFilialEmissora = true;

                                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga tipoContratacaoCarga = cargaPedido.TipoContratacaoCarga;

                                        if (emitindoCTeFilialEmissora)
                                            tipoContratacaoCarga = cargaPedido.TipoContratacaoCargaSubContratacaoFilialEmissora;

                                        Dominio.Enumeradores.TipoServico tipoServico = Dominio.Enumeradores.TipoServico.SubContratacao;

                                        if (carga.CargaSVM)
                                            tipoServico = Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal;
                                        else if (tipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho)
                                            tipoServico = Dominio.Enumeradores.TipoServico.Redespacho;
                                        else if (tipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario)
                                            tipoServico = Dominio.Enumeradores.TipoServico.RedIntermediario;

                                        List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ctesAnteriores = ConverterCTesTerceirosParaAnteriores(ctesParaSubcontrataca);
                                        List<string> chavesCTes = new List<string>();
                                        if (ctesAnteriores != null && ctesAnteriores.Count > 0)
                                            chavesCTes = ctesAnteriores.Select(o => o.Chave).ToList();

                                        Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = BuscarRegraICMS(cargaPedido.CargaOrigem, inicioPrestacao, terminoPrestacao, pedidosCTeParaSubContratacao.FirstOrDefault(), unitOfWork);
                                        Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = BuscarRegraISS(cargaPedido.CargaOrigem, pedidosCTeParaSubContratacao.FirstOrDefault(), unitOfWork);
                                        Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS();

                                        cargaPedido = repCargaPedido.BuscarPorCodigo(cargaPedido.Codigo);

                                        cargaPedido.ValorFrete = valorFrete;
                                        cargaPedido.ValorFreteAPagar = valorFrete;
                                        cargaPedido.ValorICMS = 0;
                                        cargaPedido.PercentualAliquota = 0;
                                        cargaPedido.BaseCalculoICMS = 0;
                                        cargaPedido.CST = "41";
                                        cargaPedido.CFOP = repCFOP.BuscarPorNumero(regraICMS.CFOP);

                                        repCargaPedido.Atualizar(cargaPedido);

                                        List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> apoliceSeguro = ConverterSegurosDeTerceirosEmSeguroCTe(seguros);

                                        string observacaoCTe = null;
                                        Dominio.Enumeradores.TipoCTE tipoCTe = Dominio.Enumeradores.TipoCTE.Normal;
                                        List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil> configuracoes = Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil.ConverterPedidoCTeParaSubContratacaoContaContabilContabilizacao(pedidosCTeParaSubContratacao.FirstOrDefault(), pedidosCTeParaSubContratacaoContaContabilContabilizacao);
                                        Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTE = serCte.GerarCTe(carga, cargaPedido, cte, cargaPedido.CargaOrigem.Empresa, remetente, destinatario, tomador, expedidor, recebedor, inicioPrestacao, terminoPrestacao, null, null, tipoPagamento, tipoTomador, quantidadesCTes, cargaPedidoComponentesFretesCliente, observacaoCTe, pedidoXMLNotaFiscal.CargaPedido.Pedido.ObservacaoCTeTerceiro, pedidoXMLNotaFiscal.CargaPedido.Pedido, null, apoliceSeguro, regraICMS, regraISS, impostoIBSCBS, tipoServicoMultisoftware, modeloDocumentoEmitir, tipoServico, tipoCTe, ctesAnteriores, 0, false, cargaCTeFilialEmissora, false, unitOfWork, configuracaoEmbarcador, configuracaoGeralCarga, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultado, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultadoDestinatario, pedidosCTeParaSubContratacao.FirstOrDefault().ItemServico, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultadoEscrituracao, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultadoICMS, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultadoPIS, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultadoCOFINS, pedidosCTeParaSubContratacao.FirstOrDefault().ValorMaximoCentroContabilizacao, configuracoes, pedidoCTeParaSubContratacaoNotasFiscais.Select(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal).ToList(), null, descricaoComponenteValorICMS, descricaoComponenteValorFrete, null, null, cargasPedidos, 0, chavesCTes, null, null, null, null, true);
                                        SalvarNumeroControleSVM(cargaPedido.Codigo, cargaCTE.CTe?.NumeroControle ?? "", unitOfWork, cargaCTE.CTe?.Codigo ?? 0);

                                        if (emitindoCTeFilialEmissora && cargaCTeFilialEmissora != null)
                                        {
                                            Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                                            cargaCTeFilialEmissora.CargaCTeSubContratacaoFilialEmissora = cargaCTE;
                                            repCargaCte.Atualizar(cargaCTeFilialEmissora);
                                        }
                                        List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> apoliceSeguroAverbacaos = repApoliceSeguroAverbacao.BuscarPorCargaPedido(cargaPedido.Codigo, emitindoCTeFilialEmissora);
                                        serCte.AverbaCargaCTe(cargaCTE, apoliceSeguroAverbacaos, unitOfWork, configuracaoEmbarcador, cargaPedido.Pedido.FormaAverbacaoCTE);
                                        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoNotaFiscal in pedidoCTeParaSubContratacaoNotasFiscais)
                                        {
                                            Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTE = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe();
                                            cargaPedidoXMLNotaFiscalCTE.CargaCTe = cargaCTE;
                                            cargaPedidoXMLNotaFiscalCTE.PedidoXMLNotaFiscal = pedidoCTeParaSubContratacaoNotaFiscal.PedidoXMLNotaFiscal;
                                            repCargaPedidoXMLNotaFiscalCTe.Inserir(cargaPedidoXMLNotaFiscalCTE);
                                        }

                                        bool inseriuOBS = false;
                                        if (cargaCTE.CTe != null)
                                        {
                                            foreach (var cteParaSubcontratacao in ctesParaSubcontratacao)
                                            {
                                                Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario documentoOriginario = new Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario()
                                                {
                                                    CTe = cargaCTE.CTe,
                                                    Chave = cteParaSubcontratacao.ChaveAcesso,
                                                    Numero = cteParaSubcontratacao.Numero,
                                                    Serie = cteParaSubcontratacao.Serie,
                                                    DataEmissao = cteParaSubcontratacao.DataEmissao,
                                                    NumeroMinuta = cteParaSubcontratacao.NumeroMinuta,
                                                    NumeroOperacionalConhecimentoAereo = cteParaSubcontratacao.NumeroOperacionalConhecimentoAereo
                                                };
                                                repDocumentoOriginario.Inserir(documentoOriginario);

                                                if (!inseriuOBS && cargaCTE.CTe.SVMProprio && cargaCTE.CTe.PortoOrigem != null && cargaCTE.CTe.PortoOrigem != null && cargaCTE.CTe.PortoOrigem.AtivarDespachanteComoConsignatario && cargaCTE.CTe.PortoDestino.AtivarDespachanteComoConsignatario)
                                                {
                                                    string chaveCTeMultimodal = documentoOriginario.Chave;
                                                    if (!string.IsNullOrWhiteSpace(chaveCTeMultimodal))
                                                    {
                                                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cteMultimodal = repCTe.BuscarPorChave(chaveCTeMultimodal);
                                                        if (cteMultimodal != null && cteMultimodal.TomadorPagador != null && cteMultimodal.TomadorPagador.GrupoPessoas != null && cteMultimodal.TomadorPagador.GrupoPessoas.AdicionarDespachanteComoConsignatario && cteMultimodal.TomadorPagador.GrupoPessoas.Despachante != null)
                                                        {
                                                            cargaCTE.CTe.ObservacoesGerais += " A " + cteMultimodal.TomadorPagador.GrupoPessoas.Despachante.Nome + ", CNPJ " + cteMultimodal.TomadorPagador.GrupoPessoas.Despachante.CPF_CNPJ_Formatado + ", irá atuar como despachante para a liberação dessa carga no porto de descarga.";
                                                            cargaCTE.CTe.ObservacoesGerais = cargaCTE.CTe.ObservacoesGerais.Trim();
                                                            cargaCTE.CTe.EmailDespachanteSVM = cteMultimodal.TomadorPagador.GrupoPessoas.EmailDespachante;
                                                            repCTe.Atualizar(cargaCTE.CTe);
                                                        }
                                                    }
                                                    inseriuOBS = true;
                                                }
                                            }
                                        }

                                        if (pedidosCTeParaSubContratacao.FirstOrDefault().PossuiNFSManual)
                                        {
                                            serNFS.CriarCargaCTePendenteEmissaoManualDeNFS(carga, cargaCTE, inicioPrestacao, unitOfWork);

                                            carga.AgNFSManual = true;
                                        }

                                        if (cte != null && cte.DocumentosTransporteAnteriores != null && cargaCTE.CTe.TipoServico == Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal && cargaCTE.CTe.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario)
                                        {
                                            Repositorio.DocumentoDeTransporteAnteriorCTe repDocumento = new Repositorio.DocumentoDeTransporteAnteriorCTe(unitOfWork);
                                            Servicos.Log.TratarErro(cargaCTE.CTe.Codigo.ToString() + " - Iniciou SalvarInformacoesDocumentosAnteriores SVM - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "TempoEmissaoCTe");
                                            foreach (var doc in cte.DocumentosTransporteAnteriores)
                                            {
                                                unitOfWork.Start();

                                                Dominio.Entidades.DocumentoDeTransporteAnteriorCTe documento = new Dominio.Entidades.DocumentoDeTransporteAnteriorCTe();

                                                documento.Chave = doc.Chave;
                                                documento.CTe = cargaCTE.CTe;

                                                DateTime dataEmissao;
                                                DateTime.TryParseExact(doc.DataEmissao, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
                                                double cpfCnpj = 0;
                                                if (doc.Emissor != null)
                                                    double.TryParse(Utilidades.String.OnlyNumbers(doc.Emissor.CPFCNPJ), out cpfCnpj);

                                                if (dataEmissao != DateTime.MinValue)
                                                    documento.DataEmissao = dataEmissao;

                                                documento.Emissor = cpfCnpj > 0 ? repCliente.BuscarPorCPFCNPJ(cpfCnpj) : null;
                                                documento.Numero = doc.Numero;
                                                documento.Serie = doc.Serie;
                                                documento.Tipo = doc.Tipo;

                                                repDocumento.Inserir(documento);

                                                unitOfWork.CommitChanges();

                                                unitOfWork.FlushAndClear();
                                            }
                                        }
                                        cargaCTE = repCargaCTe.BuscarPorCodigo(cargaCTE.Codigo);
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
                                        if (cargaPedido.CargaPedidoFilialEmissora || (cargaPedido.CargaPedidoTrechoAnterior?.CargaPedidoFilialEmissora ?? false))
                                        {
                                            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoPedidoNotaFiscal in pedidoCTeParaSubContratacaoNotasFiscais)
                                            {
                                                Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscalNFS = pedidoCTeParaSubContratacaoPedidoNotaFiscal.PedidoXMLNotaFiscal;
                                                if (pedidoXMLNotaFiscalNFS.DocsParaEmissaoNFSManual.Count > 0)
                                                    continue;

                                                unitOfWork.Start();
                                                carga.AgNFSManual = true;

                                                serNFS.CriarNFPendenteEmissaoManualDeNFS(carga, pedidoXMLNotaFiscalNFS, cargaPedido.ObterTomador(), terminoPrestacao, unitOfWork);

                                                unitOfWork.CommitChanges();
                                            }
                                        }
                                        else
                                        {
                                            if (pedidosCTeParaSubContratacao.FirstOrDefault().DocsParaEmissaoNFSManual.Count > 0)
                                                continue;

                                            unitOfWork.Start();

                                            Dominio.Entidades.Cliente tomador = ctesParaSubcontratacao.FirstOrDefault().Emitente?.Cliente;

                                            if (tomador == null)
                                                tomador = cargaPedido.Pedido.SubContratante != null ? cargaPedido.Pedido.SubContratante : cargaPedido.Tomador;

                                            carga.AgNFSManual = true;
                                            serNFS.CriarCTePendenteEmissaoManualDeNFS(carga, pedidosCTeParaSubContratacao.FirstOrDefault(), tomador, inicioPrestacao, pedidosCTeParaSubContratacao.FirstOrDefault().ValorFrete, unitOfWork);
                                            unitOfWork.CommitChanges();
                                        }
                                    }

                                    if (totalDocumentosGerados == 1 || (totalDocumentosGerados % quantidadeDocumentosAtualizarCarga) == 0)
                                        svcHubCarga.InformarQuantidadeDocumentosEmitidos(carga.Codigo, totalDocumentos, totalDocumentosGerados);

                                    unitOfWork.FlushAndClear();
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

                if (emitiu)
                    svcHubCarga.InformarQuantidadeDocumentosEmitidos(carga.Codigo, totalDocumentos, totalDocumentosGerados);
            }
        }

        public void GerarCTePorSacado(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, int totalDocumentos, int quantidadeDocumentosAtualizarCarga, ref int totalDocumentosGerados, bool ratearPorTerminais, bool ratearPorBUS, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao> pedidosCTeParaSubContratacaoContaContabilContabilizacao)
        {
            CTe serCte = new CTe(unitOfWork);
            CTeSubContratacaoEmissao serCTeSubContratacaoEmissao = new CTeSubContratacaoEmissao(unitOfWork);
            Servicos.Embarcador.Carga.NFS serNFS = new NFS(unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroQuantidade repCTeParaSubContratacaoQuantidade = new Repositorio.Embarcador.CTe.CTeTerceiroQuantidade(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroSeguro repCTeParaSubContratacaoSeguro = new Repositorio.Embarcador.CTe.CTeTerceiroSeguro(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repPedidoCTeParaSubContratacaoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.CTe.CTeDocumentoOriginario repDocumentoOriginario = new Repositorio.Embarcador.CTe.CTeDocumentoOriginario(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);

            CTePorPedido serCTePorPedido = new CTePorPedido(unitOfWork);
            Hubs.Carga svcHubCarga = new Hubs.Carga();

            for (int i = 0; i < cargaPedidos.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigoComFetchEmpresa(cargaPedidos[i].Codigo);

                if (!cargaPedido.PossuiCTe && !cargaPedido.PossuiNFS && !cargaPedido.PossuiNFSManual && !(cargaPedido.PossuiNFSManual && cargaPedido.ModeloDocumentoFiscalIntramunicipal != null))
                    continue;

                bool emitiu = false;

                if (!cargaPedido.CTeEmitidoNoEmbarcador && !cargaPedido.Pedido.PedidoTransbordo)
                {
                    serCte.ObterDescricoesComponentesPadrao(cargaPedido.ObterTomador(), carga, configuracaoEmbarcador, unitOfWork, out string descricaoComponenteValorFrete, out string descricaoComponenteValorICMS);

                    List<string> cnpjsTomador = repPedidoCTeParaSubContratacao.BuscarCodigosTomadorPorCargaPedido(cargaPedido.Codigo);
                    cnpjsTomador = cnpjsTomador.Distinct().ToList();
                    totalDocumentos = cnpjsTomador.Count();

                    foreach (var cnpjTomador in cnpjsTomador)
                    {
                        List<int> pedidoCTesParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarCodigosPorTomador(cargaPedido.Codigo, cnpjTomador);

                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidosCTeParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarPorCodigosComFetch(pedidoCTesParaSubContratacao);
                        if (pedidosCTeParaSubContratacao != null && pedidosCTeParaSubContratacao.Count > 0)
                        {
                            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalCarga = pedidosCTeParaSubContratacao.FirstOrDefault().ModeloDocumentoFiscal;

                            if (modeloDocumentoFiscalCarga == null)
                                modeloDocumentoFiscalCarga = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.CTe);

                            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> pedidoCTeParaSubContratacaoNotasFiscais = repPedidoCTeParaSubContratacaoNotaFiscal.BuscarPorPedidoCTeParaSubcontratacao(pedidoCTesParaSubContratacao);
                            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorCodigo(pedidoCTeParaSubContratacaoNotasFiscais.First().PedidoXMLNotaFiscal.Codigo);

                            List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesParaSubcontratacao = pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro).ToList();

                            Dominio.Entidades.Cliente expedidor = null;
                            Dominio.Entidades.Cliente recebedor = null;
                            Dominio.Entidades.Localidade inicioPrestacao = ctesParaSubcontratacao.FirstOrDefault().LocalidadeInicioPrestacao;
                            Dominio.Entidades.Localidade terminoPrestacao = ctesParaSubcontratacao.FirstOrDefault().LocalidadeTerminoPrestacao;
                            Dominio.Entidades.Cliente remetente = repCliente.BuscarPorCPFCNPJ((ctesParaSubcontratacao.FirstOrDefault().Remetente.Cliente.CPF_CNPJ));
                            Dominio.Entidades.Cliente destinatario = repCliente.BuscarPorCPFCNPJ((ctesParaSubcontratacao.FirstOrDefault().Destinatario.Cliente.CPF_CNPJ));

                            if (pedidosCTeParaSubContratacao.FirstOrDefault().CargaPedido.Expedidor != null)
                            {
                                expedidor = pedidosCTeParaSubContratacao.FirstOrDefault().CargaPedido.Expedidor;
                                if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
                                    inicioPrestacao = expedidor.Localidade;
                            }

                            if (pedidosCTeParaSubContratacao.FirstOrDefault().CargaPedido.Recebedor != null)
                            {
                                recebedor = pedidosCTeParaSubContratacao.FirstOrDefault().CargaPedido.Recebedor;
                                if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
                                    terminoPrestacao = recebedor.Localidade;
                            }

                            if (!pedidosCTeParaSubContratacao.FirstOrDefault().PossuiNFSManual ||
                                (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS &&
                                (inicioPrestacao.Codigo != terminoPrestacao.Codigo || (pedidosCTeParaSubContratacao.FirstOrDefault().PossuiNFSManual && cargaPedido.ModeloDocumentoFiscalIntramunicipal != null && inicioPrestacao.Codigo == terminoPrestacao.Codigo))))
                            {
                                bool somenteCTeSubContratacaoFilialEmissora = false;

                                carga = repCarga.BuscarPorCodigo(carga.Codigo);

                                if (cargaPedido.CargaPedidoFilialEmissora && carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                                    somenteCTeSubContratacaoFilialEmissora = true;

                                //if (repCargaPedidoXMLNotaFiscalCTe.ContarCTesPorNotasFiscais(pedidoCTeParaSubContratacaoNotasFiscais.Select(o => o.PedidoXMLNotaFiscal.Codigo).ToArray(), somenteCTeSubContratacaoFilialEmissora) > 0)  //pedidoXMLNotaFiscal.CTes.Count > 0) //já foi gerado um ct-e para esta nota
                                if (repCargaPedidoXMLNotaFiscalCTe.ContarCTesPorPedidoCTeParaSubContratacao(pedidoCTesParaSubContratacao, somenteCTeSubContratacaoFilialEmissora) > 0)
                                    continue;

                                Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoEmitir = null;

                                if (pedidosCTeParaSubContratacao.FirstOrDefault().PossuiNFSManual)
                                    modeloDocumentoEmitir = cargaPedido.ModeloDocumentoFiscalIntramunicipal;
                                else
                                    modeloDocumentoEmitir = modeloDocumentoFiscalCarga;

                                unitOfWork.Start();

                                emitiu = true;
                                totalDocumentosGerados++;
                                Dominio.ObjetosDeValor.CTe.CTe cte = new Dominio.ObjetosDeValor.CTe.CTe();
                                cte.Documentos = new List<Dominio.ObjetosDeValor.CTe.Documento>();

                                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoNotaFiscal in pedidoCTeParaSubContratacaoNotasFiscais)
                                {
                                    Dominio.ObjetosDeValor.CTe.Documento docNF = serCte.BuscarDocumentoCTe(pedidoCTeParaSubContratacaoNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal, carga.Empresa.TipoAmbiente, configuracaoEmbarcador);
                                    cte.Documentos.Add(docNF);
                                }

                                cte.PercentualPagamentoAgregado = pedidosCTeParaSubContratacao.FirstOrDefault().PercentualPagamentoAgregado;

                                List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> cargaPedidoComponentesFretesCliente = BuscarPedidoCTeSubcontratacaoComponentesFrete(pedidosCTeParaSubContratacao, modeloDocumentoFiscalCarga, unitOfWork);

                                cte.ValorTotalMercadoria = ctesParaSubcontratacao.Sum(o => o.ValorTotalMercadoria);

                                decimal valorFreteNegociado = repPedidoCTeParaSubContratacao.ObterValorFreteNegociavelPedido(cargaPedido.Codigo);
                                decimal valorCusteioSVM = repPedidoCTeParaSubContratacao.ObterValorCusteioSVMPedido(cargaPedido.Codigo);
                                decimal valorICMS = pedidosCTeParaSubContratacao.Sum(o => o.ValorICMS);
                                decimal aliquota = repPedidoCTeParaSubContratacao.ObterAliquota(cargaPedido.Codigo);//pedidosCTeParaSubContratacao.Average(o => o.PercentualAliquota);

                                cargaPedido.Pedido.NumeroBooking = repPedidoCTeParaSubContratacao.ObterNumeroBookingPedido(pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).FirstOrDefault());
                                cargaPedido.Pedido.DescricaoCarrierNavioViagem = repPedidoCTeParaSubContratacao.ObterDescricaoCarrierPedido(pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).FirstOrDefault());
                                cargaPedido.Pedido.TipoPropostaFeeder = repPedidoCTeParaSubContratacao.ObterTipoPropostaFeederPedido(pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).FirstOrDefault());

                                Repositorio.Embarcador.Pedidos.PedidoTransbordo repPedidoTransbordo = new Repositorio.Embarcador.Pedidos.PedidoTransbordo(unitOfWork);
                                List<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo> transbordosAnteriores = repPedidoTransbordo.BuscarPorPedido(cargaPedido.Pedido.Codigo);
                                List<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo> transbordos = repPedidoCTeParaSubContratacao.ObterTransbordosPedido(pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).FirstOrDefault());
                                if (transbordos != null && transbordos.Count > 0)
                                {
                                    if (transbordosAnteriores != null && transbordosAnteriores.Count > 0)
                                    {
                                        foreach (var transbordo in transbordosAnteriores)
                                        {
                                            repPedidoTransbordo.Deletar(transbordo);
                                        }
                                    }
                                    foreach (var transbordo in transbordos)
                                    {
                                        Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo trans = new Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo()
                                        {
                                            Navio = transbordo.Navio,
                                            Pedido = cargaPedido.Pedido,
                                            PedidoViagemNavio = transbordo.PedidoViagemNavio,
                                            Porto = transbordo.Porto,
                                            Sequencia = transbordo.Sequencia,
                                            Terminal = transbordo.Terminal
                                        };
                                        repPedidoTransbordo.Inserir(trans);
                                    }
                                }

                                repPedido.Atualizar(cargaPedido.Pedido);

                                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = repPedidoCTeParaSubContratacao.ObterCargaPedido(cargaPedido.Codigo);
                                int qtdContainer = cargasPedidos != null && cargasPedidos.Count > 0 ? cargasPedidos.Select(p => p.Pedido.Container.Codigo).Distinct().Count() : 1;
                                List<int> codigosContainer = new List<int>();
                                codigosContainer.AddRange(cargasPedidos.Select(p => p.Pedido.Container.Codigo).Distinct().ToList());

                                decimal valorFrete = 1;
                                decimal baseICMS = ((100 - aliquota) / 100);
                                valorFrete = CalcularValorSVM(cargaPedido.TipoPropostaMultimodal, pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).FirstOrDefault(), pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).ToList(),
                                    pedidosCTeParaSubContratacao.Count(), valorCusteioSVM, valorFreteNegociado, baseICMS, "", cargaPedido.Pedido.NumeroBooking, unitOfWork, qtdContainer, codigosContainer);

                                cte.ValorAReceber = valorFrete;
                                cte.ValorFrete = valorFrete;
                                cte.ValorTotalPrestacaoServico = valorFrete;

                                if (cargaPedido.Pedido.EmpresaSerie != null)
                                    cte.Serie = cargaPedido.Pedido.EmpresaSerie.Numero;

                                cte.ProdutoPredominante = ctesParaSubcontratacao.FirstOrDefault().ProdutoPredominante;

                                List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade> quantidades = repCTeParaSubContratacaoQuantidade.BuscarPorCTeParaSubContratacao(ctesParaSubcontratacao.Select(o => o.Codigo).ToList());
                                List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroSeguro> seguros = repCTeParaSubContratacaoSeguro.BuscarPorCTeParaSubContratacao(ctesParaSubcontratacao.Select(o => o.Codigo).ToList());

                                Dominio.Entidades.Cliente tomador = ctesParaSubcontratacao.FirstOrDefault().Emitente?.Cliente;

                                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && modeloDocumentoEmitir != null && modeloDocumentoEmitir.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe && cargaPedido.Tomador != null)
                                    tomador = cargaPedido.Tomador;

                                if (tomador == null)
                                    tomador = cargaPedido.Pedido.SubContratante != null ? cargaPedido.Pedido.SubContratante : cargaPedido.Tomador;

                                Dominio.Enumeradores.TipoPagamento tipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros; //cargaPedido.Pedido.TipoPagamento; //Dominio.Enumeradores.TipoPagamento.Outros;//cargaPedido.Pedido.TipoPagamento;
                                Dominio.Enumeradores.TipoTomador tipoTomador = Dominio.Enumeradores.TipoTomador.Outros; //cargaPedido.TipoTomador; //Dominio.Enumeradores.TipoTomador.Outros;
                                List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesParaSubcontrataca = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>();
                                foreach (var cteParaSubcontratacao in ctesParaSubcontratacao)
                                {
                                    if (!ctesParaSubcontrataca.Contains(cteParaSubcontratacao))
                                        ctesParaSubcontrataca.Add(cteParaSubcontratacao);
                                }

                                List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidadesCTes = ConverterQuantidadesTerceirosParaQuantidadesCTe(quantidades);

                                bool emitindoCTeFilialEmissora = false;
                                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeFilialEmissora = pedidosCTeParaSubContratacao.FirstOrDefault().CTeTerceiro.ObterCargaCTe(carga.Codigo);

                                if (cargaPedido.CargaPedidoFilialEmissora)
                                    emitindoCTeFilialEmissora = true;

                                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga tipoContratacaoCarga = cargaPedido.TipoContratacaoCarga;

                                if (emitindoCTeFilialEmissora)
                                    tipoContratacaoCarga = cargaPedido.TipoContratacaoCargaSubContratacaoFilialEmissora;

                                Dominio.Enumeradores.TipoServico tipoServico = Dominio.Enumeradores.TipoServico.SubContratacao;

                                if (carga.CargaSVM)
                                    tipoServico = Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal;
                                else if (tipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho)
                                    tipoServico = Dominio.Enumeradores.TipoServico.Redespacho;
                                else if (tipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario)
                                    tipoServico = Dominio.Enumeradores.TipoServico.RedIntermediario;

                                List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ctesAnteriores = ConverterCTesTerceirosParaAnteriores(ctesParaSubcontrataca);
                                List<string> chavesCTes = new List<string>();
                                if (ctesAnteriores != null && ctesAnteriores.Count > 0)
                                    chavesCTes = ctesAnteriores.Select(o => o.Chave).ToList();

                                Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = BuscarRegraICMS(cargaPedido.CargaOrigem, inicioPrestacao, terminoPrestacao, pedidosCTeParaSubContratacao.FirstOrDefault(), unitOfWork);
                                Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = BuscarRegraISS(cargaPedido.CargaOrigem, pedidosCTeParaSubContratacao.FirstOrDefault(), unitOfWork);
                                Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS();

                                cargaPedido = repCargaPedido.BuscarPorCodigo(cargaPedido.Codigo);

                                cargaPedido.ValorFrete = valorFrete;
                                cargaPedido.ValorFreteAPagar = valorFrete;
                                cargaPedido.ValorICMS = 0;
                                cargaPedido.PercentualAliquota = 0;
                                cargaPedido.BaseCalculoICMS = 0;
                                cargaPedido.CST = "41";
                                cargaPedido.CFOP = repCFOP.BuscarPorNumero(regraICMS.CFOP);

                                repCargaPedido.Atualizar(cargaPedido);

                                List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> apoliceSeguro = ConverterSegurosDeTerceirosEmSeguroCTe(seguros);

                                string observacaoCTe = null;
                                Dominio.Enumeradores.TipoCTE tipoCTe = Dominio.Enumeradores.TipoCTE.Normal;
                                List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil> configuracoes = Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil.ConverterPedidoCTeParaSubContratacaoContaContabilContabilizacao(pedidosCTeParaSubContratacao.FirstOrDefault(), pedidosCTeParaSubContratacaoContaContabilContabilizacao);
                                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTE = serCte.GerarCTe(carga, cargaPedido, cte, cargaPedido.CargaOrigem.Empresa, remetente, destinatario, tomador, expedidor, recebedor, inicioPrestacao, terminoPrestacao, null, null, tipoPagamento, tipoTomador, quantidadesCTes, cargaPedidoComponentesFretesCliente, observacaoCTe, pedidoXMLNotaFiscal.CargaPedido.Pedido.ObservacaoCTeTerceiro, pedidoXMLNotaFiscal.CargaPedido.Pedido, null, apoliceSeguro, regraICMS, regraISS, impostoIBSCBS, tipoServicoMultisoftware, modeloDocumentoEmitir, tipoServico, tipoCTe, ctesAnteriores, 0, false, cargaCTeFilialEmissora, false, unitOfWork, configuracaoEmbarcador, configuracaoGeralCarga, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultado, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultadoDestinatario, pedidosCTeParaSubContratacao.FirstOrDefault().ItemServico, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultadoEscrituracao, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultadoICMS, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultadoPIS, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultadoCOFINS, pedidosCTeParaSubContratacao.FirstOrDefault().ValorMaximoCentroContabilizacao, configuracoes, pedidoCTeParaSubContratacaoNotasFiscais.Select(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal).ToList(), null, descricaoComponenteValorICMS, descricaoComponenteValorFrete, null, null, cargasPedidos, 0, null, null, null, null, null, true);
                                SalvarNumeroControleSVM(cargaPedido.Codigo, cargaCTE.CTe?.NumeroControle ?? "", unitOfWork, cargaCTE.CTe?.Codigo ?? 0);

                                if (emitindoCTeFilialEmissora && cargaCTeFilialEmissora != null)
                                {
                                    Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                                    cargaCTeFilialEmissora.CargaCTeSubContratacaoFilialEmissora = cargaCTE;
                                    repCargaCte.Atualizar(cargaCTeFilialEmissora);
                                }
                                List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> apoliceSeguroAverbacaos = repApoliceSeguroAverbacao.BuscarPorCargaPedido(cargaPedido.Codigo, emitindoCTeFilialEmissora);
                                serCte.AverbaCargaCTe(cargaCTE, apoliceSeguroAverbacaos, unitOfWork, configuracaoEmbarcador, cargaPedido.Pedido.FormaAverbacaoCTE);
                                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoNotaFiscal in pedidoCTeParaSubContratacaoNotasFiscais)
                                {
                                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTE = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe();
                                    cargaPedidoXMLNotaFiscalCTE.CargaCTe = cargaCTE;
                                    cargaPedidoXMLNotaFiscalCTE.PedidoXMLNotaFiscal = pedidoCTeParaSubContratacaoNotaFiscal.PedidoXMLNotaFiscal;
                                    repCargaPedidoXMLNotaFiscalCTe.Inserir(cargaPedidoXMLNotaFiscalCTE);
                                }

                                bool inseriuOBS = false;
                                if (cargaCTE.CTe != null)
                                {
                                    foreach (var cteParaSubcontratacao in ctesParaSubcontratacao)
                                    {
                                        Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario documentoOriginario = new Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario()
                                        {
                                            CTe = cargaCTE.CTe,
                                            Chave = cteParaSubcontratacao.ChaveAcesso,
                                            Numero = cteParaSubcontratacao.Numero,
                                            Serie = cteParaSubcontratacao.Serie,
                                            DataEmissao = cteParaSubcontratacao.DataEmissao,
                                            NumeroMinuta = cteParaSubcontratacao.NumeroMinuta,
                                            NumeroOperacionalConhecimentoAereo = cteParaSubcontratacao.NumeroOperacionalConhecimentoAereo
                                        };
                                        repDocumentoOriginario.Inserir(documentoOriginario);

                                        if (!inseriuOBS && cargaCTE.CTe.SVMProprio && cargaCTE.CTe.PortoOrigem != null && cargaCTE.CTe.PortoOrigem != null && cargaCTE.CTe.PortoOrigem.AtivarDespachanteComoConsignatario && cargaCTE.CTe.PortoDestino.AtivarDespachanteComoConsignatario)
                                        {
                                            string chaveCTeMultimodal = documentoOriginario.Chave;
                                            if (!string.IsNullOrWhiteSpace(chaveCTeMultimodal))
                                            {
                                                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteMultimodal = repCTe.BuscarPorChave(chaveCTeMultimodal);
                                                if (cteMultimodal != null && cteMultimodal.TomadorPagador != null && cteMultimodal.TomadorPagador.GrupoPessoas != null && cteMultimodal.TomadorPagador.GrupoPessoas.AdicionarDespachanteComoConsignatario && cteMultimodal.TomadorPagador.GrupoPessoas.Despachante != null)
                                                {
                                                    cargaCTE.CTe.ObservacoesGerais += " A " + cteMultimodal.TomadorPagador.GrupoPessoas.Despachante.Nome + ", CNPJ " + cteMultimodal.TomadorPagador.GrupoPessoas.Despachante.CPF_CNPJ_Formatado + ", irá atuar como despachante para a liberação dessa carga no porto de descarga.";
                                                    cargaCTE.CTe.ObservacoesGerais = cargaCTE.CTe.ObservacoesGerais.Trim();
                                                    cargaCTE.CTe.EmailDespachanteSVM = cteMultimodal.TomadorPagador.GrupoPessoas.EmailDespachante;
                                                    repCTe.Atualizar(cargaCTE.CTe);
                                                }
                                            }
                                            inseriuOBS = true;
                                        }
                                    }
                                }

                                if (pedidosCTeParaSubContratacao.FirstOrDefault().PossuiNFSManual)
                                {
                                    serNFS.CriarCargaCTePendenteEmissaoManualDeNFS(carga, cargaCTE, inicioPrestacao, unitOfWork);

                                    carga.AgNFSManual = true;
                                }

                                unitOfWork.CommitChanges();

                                if (cte != null && cte.DocumentosTransporteAnteriores != null && cargaCTE.CTe.TipoServico == Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal && cargaCTE.CTe.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario)
                                {
                                    Repositorio.DocumentoDeTransporteAnteriorCTe repDocumento = new Repositorio.DocumentoDeTransporteAnteriorCTe(unitOfWork);
                                    Servicos.Log.TratarErro(cargaCTE.CTe.Codigo.ToString() + " - Iniciou SalvarInformacoesDocumentosAnteriores SVM - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "TempoEmissaoCTe");
                                    foreach (var doc in cte.DocumentosTransporteAnteriores)
                                    {
                                        unitOfWork.Start();

                                        Dominio.Entidades.DocumentoDeTransporteAnteriorCTe documento = new Dominio.Entidades.DocumentoDeTransporteAnteriorCTe();

                                        documento.Chave = doc.Chave;
                                        documento.CTe = cargaCTE.CTe;

                                        DateTime dataEmissao;
                                        DateTime.TryParseExact(doc.DataEmissao, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
                                        double cpfCnpj = 0;
                                        if (doc.Emissor != null)
                                            double.TryParse(Utilidades.String.OnlyNumbers(doc.Emissor.CPFCNPJ), out cpfCnpj);

                                        if (dataEmissao != DateTime.MinValue)
                                            documento.DataEmissao = dataEmissao;

                                        documento.Emissor = cpfCnpj > 0 ? repCliente.BuscarPorCPFCNPJ(cpfCnpj) : null;
                                        documento.Numero = doc.Numero;
                                        documento.Serie = doc.Serie;
                                        documento.Tipo = doc.Tipo;

                                        repDocumento.Inserir(documento);

                                        unitOfWork.CommitChanges();

                                        unitOfWork.FlushAndClear();
                                    }
                                }
                                cargaCTE = repCargaCTe.BuscarPorCodigo(cargaCTE.Codigo);
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
                                if (cargaPedido.CargaPedidoFilialEmissora || (cargaPedido.CargaPedidoTrechoAnterior?.CargaPedidoFilialEmissora ?? false))
                                {
                                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoPedidoNotaFiscal in pedidoCTeParaSubContratacaoNotasFiscais)
                                    {
                                        Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscalNFS = pedidoCTeParaSubContratacaoPedidoNotaFiscal.PedidoXMLNotaFiscal;
                                        if (pedidoXMLNotaFiscalNFS.DocsParaEmissaoNFSManual.Count > 0)
                                            continue;

                                        unitOfWork.Start();
                                        carga.AgNFSManual = true;

                                        serNFS.CriarNFPendenteEmissaoManualDeNFS(carga, pedidoXMLNotaFiscalNFS, cargaPedido.ObterTomador(), terminoPrestacao, unitOfWork);

                                        unitOfWork.CommitChanges();
                                    }
                                }
                                else
                                {
                                    if (pedidosCTeParaSubContratacao.FirstOrDefault().DocsParaEmissaoNFSManual.Count > 0)
                                        continue;

                                    unitOfWork.Start();

                                    Dominio.Entidades.Cliente tomador = ctesParaSubcontratacao.FirstOrDefault().Emitente?.Cliente;

                                    if (tomador == null)
                                        tomador = cargaPedido.Pedido.SubContratante != null ? cargaPedido.Pedido.SubContratante : cargaPedido.Tomador;

                                    carga.AgNFSManual = true;
                                    serNFS.CriarCTePendenteEmissaoManualDeNFS(carga, pedidosCTeParaSubContratacao.FirstOrDefault(), tomador, inicioPrestacao, pedidosCTeParaSubContratacao.FirstOrDefault().ValorFrete, unitOfWork);
                                    unitOfWork.CommitChanges();
                                }
                            }

                            if (totalDocumentosGerados == 1 || (totalDocumentosGerados % quantidadeDocumentosAtualizarCarga) == 0)
                                svcHubCarga.InformarQuantidadeDocumentosEmitidos(carga.Codigo, totalDocumentos, totalDocumentosGerados);

                            unitOfWork.FlushAndClear();
                        }
                    }

                }
                else
                {
                    Servicos.Embarcador.CTe.CTEsImportados serCTEsImportados = new Servicos.Embarcador.CTe.CTEsImportados(unitOfWork);
                    serCTEsImportados.VincularCargaCTeEmitidoAnteriormente(carga, cargaPedido, unitOfWork);
                }

                if (emitiu)
                    svcHubCarga.InformarQuantidadeDocumentosEmitidos(carga.Codigo, totalDocumentos, totalDocumentosGerados);
            }
        }

        public void GerarCTePorRemetenteDestinatarioSacado(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, int totalDocumentos, int quantidadeDocumentosAtualizarCarga, ref int totalDocumentosGerados, bool ratearMesmoQueMultiModal, bool ratearPorTerminais, bool ratearPorBUS, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao> pedidosCTeParaSubContratacaoContaContabilContabilizacao)
        {
            CTe serCte = new CTe(unitOfWork);
            CTeSubContratacaoEmissao serCTeSubContratacaoEmissao = new CTeSubContratacaoEmissao(unitOfWork);
            Servicos.Embarcador.Carga.NFS serNFS = new NFS(unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroQuantidade repCTeParaSubContratacaoQuantidade = new Repositorio.Embarcador.CTe.CTeTerceiroQuantidade(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroSeguro repCTeParaSubContratacaoSeguro = new Repositorio.Embarcador.CTe.CTeTerceiroSeguro(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repPedidoCTeParaSubContratacaoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.CTe.CTeDocumentoOriginario repDocumentoOriginario = new Repositorio.Embarcador.CTe.CTeDocumentoOriginario(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            CTePorPedido serCTePorPedido = new CTePorPedido(unitOfWork);
            Hubs.Carga svcHubCarga = new Hubs.Carga();

            for (int i = 0; i < cargaPedidos.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigoComFetchEmpresa(cargaPedidos[i].Codigo);

                if (!cargaPedido.PossuiCTe && !cargaPedido.PossuiNFS && !cargaPedido.PossuiNFSManual && !(cargaPedido.PossuiNFSManual && cargaPedido.ModeloDocumentoFiscalIntramunicipal != null))
                    continue;

                bool emitiu = false;

                if (!cargaPedido.CTeEmitidoNoEmbarcador && !cargaPedido.Pedido.PedidoTransbordo)
                {
                    serCte.ObterDescricoesComponentesPadrao(cargaPedido.ObterTomador(), carga, configuracaoEmbarcador, unitOfWork, out string descricaoComponenteValorFrete, out string descricaoComponenteValorICMS);

                    List<string> cnpjsDestinatario = repPedidoCTeParaSubContratacao.BuscarCodigosDestinatarioPorCargaPedido(cargaPedido.Codigo);
                    List<string> cnpjsRemetente = repPedidoCTeParaSubContratacao.BuscarCodigosRemetentePorCargaPedido(cargaPedido.Codigo);
                    List<string> cnpjsTomador = repPedidoCTeParaSubContratacao.BuscarCodigosTomadorPorCargaPedido(cargaPedido.Codigo);
                    cnpjsTomador = cnpjsTomador.Distinct().ToList();

                    foreach (var cnpjRemetente in cnpjsRemetente)
                    {
                        foreach (var cnpjDestinatario in cnpjsDestinatario)
                        {
                            foreach (var cnpjTomador in cnpjsTomador)
                            {
                                List<int> pedidoCTesParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarCodigosPorDestinatarioRemetenteSacado(cargaPedido.Codigo, cnpjDestinatario, cnpjRemetente, cnpjTomador);

                                List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidosCTeParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarPorCodigosComFetch(pedidoCTesParaSubContratacao);
                                if (pedidosCTeParaSubContratacao != null && pedidosCTeParaSubContratacao.Count > 0)
                                {
                                    Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalCarga = pedidosCTeParaSubContratacao.FirstOrDefault().ModeloDocumentoFiscal;

                                    if (modeloDocumentoFiscalCarga == null)
                                        modeloDocumentoFiscalCarga = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.CTe);

                                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> pedidoCTeParaSubContratacaoNotasFiscais = repPedidoCTeParaSubContratacaoNotaFiscal.BuscarPorPedidoCTeParaSubcontratacao(pedidoCTesParaSubContratacao);
                                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorCodigo(pedidoCTeParaSubContratacaoNotasFiscais.First().PedidoXMLNotaFiscal.Codigo);

                                    List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesParaSubcontratacao = pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro).ToList();

                                    Dominio.Entidades.Cliente expedidor = null;
                                    Dominio.Entidades.Cliente recebedor = null;
                                    Dominio.Entidades.Localidade inicioPrestacao = ctesParaSubcontratacao.FirstOrDefault().LocalidadeInicioPrestacao;
                                    Dominio.Entidades.Localidade terminoPrestacao = ctesParaSubcontratacao.FirstOrDefault().LocalidadeTerminoPrestacao;
                                    Dominio.Entidades.Cliente remetente = repCliente.BuscarPorCPFCNPJ((ctesParaSubcontratacao.FirstOrDefault().Remetente.Cliente.CPF_CNPJ));
                                    Dominio.Entidades.Cliente destinatario = repCliente.BuscarPorCPFCNPJ((ctesParaSubcontratacao.FirstOrDefault().Destinatario.Cliente.CPF_CNPJ));

                                    if (pedidosCTeParaSubContratacao.FirstOrDefault().CargaPedido.Expedidor != null)
                                    {
                                        expedidor = pedidosCTeParaSubContratacao.FirstOrDefault().CargaPedido.Expedidor;
                                        if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
                                            inicioPrestacao = expedidor.Localidade;
                                    }

                                    if (pedidosCTeParaSubContratacao.FirstOrDefault().CargaPedido.Recebedor != null)
                                    {
                                        recebedor = pedidosCTeParaSubContratacao.FirstOrDefault().CargaPedido.Recebedor;
                                        if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
                                            terminoPrestacao = recebedor.Localidade;
                                    }

                                    if (!pedidosCTeParaSubContratacao.FirstOrDefault().PossuiNFSManual ||
                                        (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS &&
                                        (inicioPrestacao.Codigo != terminoPrestacao.Codigo || (pedidosCTeParaSubContratacao.FirstOrDefault().PossuiNFSManual && cargaPedido.ModeloDocumentoFiscalIntramunicipal != null && inicioPrestacao.Codigo == terminoPrestacao.Codigo))))
                                    {
                                        bool somenteCTeSubContratacaoFilialEmissora = false;

                                        carga = repCarga.BuscarPorCodigo(carga.Codigo);

                                        if (cargaPedido.CargaPedidoFilialEmissora && carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                                            somenteCTeSubContratacaoFilialEmissora = true;

                                        //if (repCargaPedidoXMLNotaFiscalCTe.ContarCTesPorNotasFiscais(pedidoCTeParaSubContratacaoNotasFiscais.Select(o => o.PedidoXMLNotaFiscal.Codigo).ToArray(), somenteCTeSubContratacaoFilialEmissora) > 0)  //pedidoXMLNotaFiscal.CTes.Count > 0) //já foi gerado um ct-e para esta nota
                                        if (repCargaPedidoXMLNotaFiscalCTe.ContarCTesPorPedidoCTeParaSubContratacao(pedidoCTesParaSubContratacao, somenteCTeSubContratacaoFilialEmissora) > 0)
                                            continue;

                                        Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoEmitir = null;

                                        if (pedidosCTeParaSubContratacao.FirstOrDefault().PossuiNFSManual)
                                            modeloDocumentoEmitir = cargaPedido.ModeloDocumentoFiscalIntramunicipal;
                                        else
                                            modeloDocumentoEmitir = modeloDocumentoFiscalCarga;

                                        unitOfWork.Start();

                                        emitiu = true;
                                        totalDocumentosGerados++;
                                        Dominio.ObjetosDeValor.CTe.CTe cte = new Dominio.ObjetosDeValor.CTe.CTe();
                                        cte.Documentos = new List<Dominio.ObjetosDeValor.CTe.Documento>();

                                        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoNotaFiscal in pedidoCTeParaSubContratacaoNotasFiscais)
                                        {
                                            Dominio.ObjetosDeValor.CTe.Documento docNF = serCte.BuscarDocumentoCTe(pedidoCTeParaSubContratacaoNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal, carga.Empresa.TipoAmbiente, configuracaoEmbarcador);
                                            cte.Documentos.Add(docNF);
                                        }

                                        cte.PercentualPagamentoAgregado = pedidosCTeParaSubContratacao.FirstOrDefault().PercentualPagamentoAgregado;

                                        List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> cargaPedidoComponentesFretesCliente = BuscarPedidoCTeSubcontratacaoComponentesFrete(pedidosCTeParaSubContratacao, modeloDocumentoFiscalCarga, unitOfWork);

                                        cte.ValorTotalMercadoria = ctesParaSubcontratacao.Sum(o => o.ValorTotalMercadoria);

                                        decimal valorFreteNegociado = repPedidoCTeParaSubContratacao.ObterValorFreteNegociavelPedido(cargaPedido.Codigo);
                                        decimal valorCusteioSVM = repPedidoCTeParaSubContratacao.ObterValorCusteioSVMPedido(cargaPedido.Codigo);
                                        decimal valorICMS = pedidosCTeParaSubContratacao.Sum(o => o.ValorICMS);
                                        decimal aliquota = repPedidoCTeParaSubContratacao.ObterAliquota(cargaPedido.Codigo);

                                        cargaPedido.Pedido.NumeroBooking = repPedidoCTeParaSubContratacao.ObterNumeroBookingPedido(pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).FirstOrDefault());
                                        cargaPedido.Pedido.DescricaoCarrierNavioViagem = repPedidoCTeParaSubContratacao.ObterDescricaoCarrierPedido(pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).FirstOrDefault());
                                        cargaPedido.Pedido.TipoPropostaFeeder = repPedidoCTeParaSubContratacao.ObterTipoPropostaFeederPedido(pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).FirstOrDefault());

                                        Repositorio.Embarcador.Pedidos.PedidoTransbordo repPedidoTransbordo = new Repositorio.Embarcador.Pedidos.PedidoTransbordo(unitOfWork);
                                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo> transbordosAnteriores = repPedidoTransbordo.BuscarPorPedido(cargaPedido.Pedido.Codigo);
                                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo> transbordos = repPedidoCTeParaSubContratacao.ObterTransbordosPedido(pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).FirstOrDefault());
                                        if (transbordos != null && transbordos.Count > 0)
                                        {
                                            if (transbordosAnteriores != null && transbordosAnteriores.Count > 0)
                                            {
                                                foreach (var transbordo in transbordosAnteriores)
                                                {
                                                    repPedidoTransbordo.Deletar(transbordo);
                                                }
                                            }
                                            foreach (var transbordo in transbordos)
                                            {
                                                Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo trans = new Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo()
                                                {
                                                    Navio = transbordo.Navio,
                                                    Pedido = cargaPedido.Pedido,
                                                    PedidoViagemNavio = transbordo.PedidoViagemNavio,
                                                    Porto = transbordo.Porto,
                                                    Sequencia = transbordo.Sequencia,
                                                    Terminal = transbordo.Terminal
                                                };
                                                repPedidoTransbordo.Inserir(trans);
                                            }
                                        }

                                        repPedido.Atualizar(cargaPedido.Pedido);

                                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = repPedidoCTeParaSubContratacao.ObterCargaPedido(cargaPedido.Codigo);
                                        int qtdContainer = cargasPedidos != null && cargasPedidos.Count > 0 ? cargasPedidos.Select(p => p.Pedido.Container.Codigo).Distinct().Count() : 1;
                                        List<int> codigosContainer = new List<int>();
                                        codigosContainer.AddRange(cargasPedidos.Select(p => p.Pedido.Container.Codigo).Distinct().ToList());
                                        decimal valorFrete = 1;
                                        decimal baseICMS = ((100 - aliquota) / 100);
                                        valorFrete = CalcularValorSVM(cargaPedido.TipoPropostaMultimodal, pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).FirstOrDefault(), pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).ToList(),
                                            pedidosCTeParaSubContratacao.Count(), valorCusteioSVM, valorFreteNegociado, baseICMS, "", cargaPedido.Pedido.NumeroBooking, unitOfWork, qtdContainer, codigosContainer);

                                        cte.ValorAReceber = valorFrete;//pedidoCTeParaSubContratacao.ValorFrete;
                                        cte.ValorFrete = valorFrete;// pedidoCTeParaSubContratacao.ValorFrete;
                                        cte.ValorTotalPrestacaoServico = valorFrete;// pedidoCTeParaSubContratacao.ValorFrete;

                                        if (cargaPedido.Pedido.EmpresaSerie != null)
                                            cte.Serie = cargaPedido.Pedido.EmpresaSerie.Numero;

                                        cte.ProdutoPredominante = ctesParaSubcontratacao.FirstOrDefault().ProdutoPredominante;

                                        List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade> quantidades = repCTeParaSubContratacaoQuantidade.BuscarPorCTeParaSubContratacao(ctesParaSubcontratacao.Select(o => o.Codigo).ToList());
                                        List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroSeguro> seguros = repCTeParaSubContratacaoSeguro.BuscarPorCTeParaSubContratacao(ctesParaSubcontratacao.Select(o => o.Codigo).ToList());

                                        Dominio.Entidades.Cliente tomador = ctesParaSubcontratacao.FirstOrDefault().Emitente?.Cliente;

                                        if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && modeloDocumentoEmitir != null && modeloDocumentoEmitir.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe && cargaPedido.Tomador != null)
                                            tomador = cargaPedido.Tomador;

                                        if (tomador == null)
                                            tomador = cargaPedido.Pedido.SubContratante != null ? cargaPedido.Pedido.SubContratante : cargaPedido.Tomador;

                                        Dominio.Enumeradores.TipoPagamento tipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros; //cargaPedido.Pedido.TipoPagamento; //Dominio.Enumeradores.TipoPagamento.Outros;//cargaPedido.Pedido.TipoPagamento;
                                        Dominio.Enumeradores.TipoTomador tipoTomador = Dominio.Enumeradores.TipoTomador.Outros; //cargaPedido.TipoTomador; //Dominio.Enumeradores.TipoTomador.Outros;
                                        List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesParaSubcontrataca = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>();
                                        foreach (var cteParaSubcontratacao in ctesParaSubcontratacao)
                                        {
                                            if (!ctesParaSubcontrataca.Contains(cteParaSubcontratacao))
                                                ctesParaSubcontrataca.Add(cteParaSubcontratacao);
                                        }

                                        List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidadesCTes = ConverterQuantidadesTerceirosParaQuantidadesCTe(quantidades);

                                        bool emitindoCTeFilialEmissora = false;
                                        Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeFilialEmissora = pedidosCTeParaSubContratacao.FirstOrDefault().CTeTerceiro.ObterCargaCTe(carga.Codigo);

                                        if (cargaPedido.CargaPedidoFilialEmissora)
                                            emitindoCTeFilialEmissora = true;

                                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga tipoContratacaoCarga = cargaPedido.TipoContratacaoCarga;

                                        if (emitindoCTeFilialEmissora)
                                            tipoContratacaoCarga = cargaPedido.TipoContratacaoCargaSubContratacaoFilialEmissora;

                                        Dominio.Enumeradores.TipoServico tipoServico = Dominio.Enumeradores.TipoServico.SubContratacao;

                                        if (carga.CargaSVM)
                                            tipoServico = Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal;
                                        else if (tipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho)
                                            tipoServico = Dominio.Enumeradores.TipoServico.Redespacho;
                                        else if (tipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario)
                                            tipoServico = Dominio.Enumeradores.TipoServico.RedIntermediario;

                                        List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ctesAnteriores = ConverterCTesTerceirosParaAnteriores(ctesParaSubcontrataca);
                                        List<string> chavesCTes = new List<string>();
                                        if (ctesAnteriores != null && ctesAnteriores.Count > 0)
                                            chavesCTes = ctesAnteriores.Select(o => o.Chave).ToList();

                                        Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = BuscarRegraICMS(cargaPedido.CargaOrigem, inicioPrestacao, terminoPrestacao, pedidosCTeParaSubContratacao.FirstOrDefault(), unitOfWork);
                                        Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = BuscarRegraISS(cargaPedido.CargaOrigem, pedidosCTeParaSubContratacao.FirstOrDefault(), unitOfWork);
                                        Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS();

                                        cargaPedido = repCargaPedido.BuscarPorCodigo(cargaPedido.Codigo);

                                        cargaPedido.ValorFrete = valorFrete;
                                        cargaPedido.ValorFreteAPagar = valorFrete;
                                        cargaPedido.ValorICMS = 0;
                                        cargaPedido.PercentualAliquota = 0;
                                        cargaPedido.BaseCalculoICMS = 0;
                                        cargaPedido.CST = "41";
                                        cargaPedido.CFOP = repCFOP.BuscarPorNumero(regraICMS.CFOP);

                                        repCargaPedido.Atualizar(cargaPedido);

                                        List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> apoliceSeguro = ConverterSegurosDeTerceirosEmSeguroCTe(seguros);

                                        string observacaoCTe = null;
                                        Dominio.Enumeradores.TipoCTE tipoCTe = Dominio.Enumeradores.TipoCTE.Normal;

                                        double.TryParse(cnpjDestinatario, out double cnpjDestinatarioContainer);
                                        List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil> configuracoes = Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil.ConverterPedidoCTeParaSubContratacaoContaContabilContabilizacao(pedidosCTeParaSubContratacao.FirstOrDefault(), pedidosCTeParaSubContratacaoContaContabilContabilizacao);
                                        Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTE = serCte.GerarCTe(carga, cargaPedido, cte, cargaPedido.CargaOrigem.Empresa, remetente, destinatario, tomador, expedidor, recebedor, inicioPrestacao, terminoPrestacao, null, null, tipoPagamento, tipoTomador, quantidadesCTes, cargaPedidoComponentesFretesCliente, observacaoCTe, pedidoXMLNotaFiscal.CargaPedido.Pedido.ObservacaoCTeTerceiro, pedidoXMLNotaFiscal.CargaPedido.Pedido, null, apoliceSeguro, regraICMS, regraISS, impostoIBSCBS, tipoServicoMultisoftware, modeloDocumentoEmitir, tipoServico, tipoCTe, ctesAnteriores, 0, false, cargaCTeFilialEmissora, false, unitOfWork, configuracaoEmbarcador, configuracaoGeralCarga, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultado, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultadoDestinatario, pedidosCTeParaSubContratacao.FirstOrDefault().ItemServico, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultadoEscrituracao, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultadoICMS, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultadoPIS, pedidosCTeParaSubContratacao.FirstOrDefault().CentroResultadoCOFINS, pedidosCTeParaSubContratacao.FirstOrDefault().ValorMaximoCentroContabilizacao, configuracoes, pedidoCTeParaSubContratacaoNotasFiscais.Select(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal).ToList(), null, descricaoComponenteValorICMS, descricaoComponenteValorFrete, null, null, cargasPedidos, cnpjDestinatarioContainer, chavesCTes, null, null, null, null, true);
                                        SalvarNumeroControleSVM(cargaPedido.Codigo, cargaCTE.CTe?.NumeroControle ?? "", unitOfWork, cargaCTE.CTe?.Codigo ?? 0);

                                        if (emitindoCTeFilialEmissora && cargaCTeFilialEmissora != null)
                                        {
                                            Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                                            cargaCTeFilialEmissora.CargaCTeSubContratacaoFilialEmissora = cargaCTE;
                                            repCargaCte.Atualizar(cargaCTeFilialEmissora);
                                        }
                                        List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> apoliceSeguroAverbacaos = repApoliceSeguroAverbacao.BuscarPorCargaPedido(cargaPedido.Codigo, emitindoCTeFilialEmissora);
                                        serCte.AverbaCargaCTe(cargaCTE, apoliceSeguroAverbacaos, unitOfWork, configuracaoEmbarcador, cargaPedido.Pedido.FormaAverbacaoCTE);
                                        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoNotaFiscal in pedidoCTeParaSubContratacaoNotasFiscais)
                                        {
                                            Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTE = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe();
                                            cargaPedidoXMLNotaFiscalCTE.CargaCTe = cargaCTE;
                                            cargaPedidoXMLNotaFiscalCTE.PedidoXMLNotaFiscal = pedidoCTeParaSubContratacaoNotaFiscal.PedidoXMLNotaFiscal;
                                            repCargaPedidoXMLNotaFiscalCTe.Inserir(cargaPedidoXMLNotaFiscalCTE);
                                        }

                                        bool inseriuOBS = false;
                                        if (cargaCTE.CTe != null)
                                        {
                                            foreach (var cteParaSubcontratacao in ctesParaSubcontratacao)
                                            {
                                                Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario documentoOriginario = new Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario()
                                                {
                                                    CTe = cargaCTE.CTe,
                                                    Chave = cteParaSubcontratacao.ChaveAcesso,
                                                    Numero = cteParaSubcontratacao.Numero,
                                                    Serie = cteParaSubcontratacao.Serie,
                                                    DataEmissao = cteParaSubcontratacao.DataEmissao,
                                                    NumeroMinuta = cteParaSubcontratacao.NumeroMinuta,
                                                    NumeroOperacionalConhecimentoAereo = cteParaSubcontratacao.NumeroOperacionalConhecimentoAereo
                                                };
                                                repDocumentoOriginario.Inserir(documentoOriginario);

                                                if (!inseriuOBS && cargaCTE.CTe.SVMProprio && cargaCTE.CTe.PortoOrigem != null && cargaCTE.CTe.PortoOrigem != null && cargaCTE.CTe.PortoOrigem.AtivarDespachanteComoConsignatario && cargaCTE.CTe.PortoDestino.AtivarDespachanteComoConsignatario)
                                                {
                                                    string chaveCTeMultimodal = documentoOriginario.Chave;
                                                    if (!string.IsNullOrWhiteSpace(chaveCTeMultimodal))
                                                    {
                                                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cteMultimodal = repCTe.BuscarPorChave(chaveCTeMultimodal);
                                                        if (cteMultimodal != null && cteMultimodal.TomadorPagador != null && cteMultimodal.TomadorPagador.GrupoPessoas != null && cteMultimodal.TomadorPagador.GrupoPessoas.AdicionarDespachanteComoConsignatario && cteMultimodal.TomadorPagador.GrupoPessoas.Despachante != null)
                                                        {
                                                            cargaCTE.CTe.ObservacoesGerais += " A " + cteMultimodal.TomadorPagador.GrupoPessoas.Despachante.Nome + ", CNPJ " + cteMultimodal.TomadorPagador.GrupoPessoas.Despachante.CPF_CNPJ_Formatado + ", irá atuar como despachante para a liberação dessa carga no porto de descarga.";
                                                            cargaCTE.CTe.ObservacoesGerais = cargaCTE.CTe.ObservacoesGerais.Trim();
                                                            cargaCTE.CTe.EmailDespachanteSVM = cteMultimodal.TomadorPagador.GrupoPessoas.EmailDespachante;
                                                            repCTe.Atualizar(cargaCTE.CTe);
                                                        }
                                                    }
                                                    inseriuOBS = true;
                                                }
                                            }
                                        }

                                        if (pedidosCTeParaSubContratacao.FirstOrDefault().PossuiNFSManual)
                                        {
                                            serNFS.CriarCargaCTePendenteEmissaoManualDeNFS(carga, cargaCTE, inicioPrestacao, unitOfWork);

                                            carga.AgNFSManual = true;
                                        }

                                        unitOfWork.CommitChanges();
                                        if (cte != null && cte.DocumentosTransporteAnteriores != null && cargaCTE.CTe.TipoServico == Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal && cargaCTE.CTe.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario)
                                        {
                                            Repositorio.DocumentoDeTransporteAnteriorCTe repDocumento = new Repositorio.DocumentoDeTransporteAnteriorCTe(unitOfWork);
                                            Servicos.Log.TratarErro(cargaCTE.CTe.Codigo.ToString() + " - Iniciou SalvarInformacoesDocumentosAnteriores SVM - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "TempoEmissaoCTe");
                                            foreach (var doc in cte.DocumentosTransporteAnteriores)
                                            {
                                                unitOfWork.Start();

                                                Dominio.Entidades.DocumentoDeTransporteAnteriorCTe documento = new Dominio.Entidades.DocumentoDeTransporteAnteriorCTe();

                                                documento.Chave = doc.Chave;
                                                documento.CTe = cargaCTE.CTe;

                                                DateTime dataEmissao;
                                                DateTime.TryParseExact(doc.DataEmissao, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
                                                double cpfCnpj = 0;
                                                if (doc.Emissor != null)
                                                    double.TryParse(Utilidades.String.OnlyNumbers(doc.Emissor.CPFCNPJ), out cpfCnpj);

                                                if (dataEmissao != DateTime.MinValue)
                                                    documento.DataEmissao = dataEmissao;

                                                documento.Emissor = cpfCnpj > 0 ? repCliente.BuscarPorCPFCNPJ(cpfCnpj) : null;
                                                documento.Numero = doc.Numero;
                                                documento.Serie = doc.Serie;
                                                documento.Tipo = doc.Tipo;

                                                repDocumento.Inserir(documento);

                                                unitOfWork.CommitChanges();

                                                unitOfWork.FlushAndClear();
                                            }
                                        }
                                        cargaCTE = repCargaCTe.BuscarPorCodigo(cargaCTE.Codigo);
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
                                        if (cargaPedido.CargaPedidoFilialEmissora || (cargaPedido.CargaPedidoTrechoAnterior?.CargaPedidoFilialEmissora ?? false))
                                        {
                                            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoPedidoNotaFiscal in pedidoCTeParaSubContratacaoNotasFiscais)
                                            {
                                                Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscalNFS = pedidoCTeParaSubContratacaoPedidoNotaFiscal.PedidoXMLNotaFiscal;
                                                if (pedidoXMLNotaFiscalNFS.DocsParaEmissaoNFSManual.Count > 0)
                                                    continue;

                                                unitOfWork.Start();
                                                carga.AgNFSManual = true;

                                                serNFS.CriarNFPendenteEmissaoManualDeNFS(carga, pedidoXMLNotaFiscalNFS, cargaPedido.ObterTomador(), terminoPrestacao, unitOfWork);

                                                unitOfWork.CommitChanges();
                                            }
                                        }
                                        else
                                        {
                                            if (pedidosCTeParaSubContratacao.FirstOrDefault().DocsParaEmissaoNFSManual.Count > 0)
                                                continue;

                                            unitOfWork.Start();

                                            Dominio.Entidades.Cliente tomador = ctesParaSubcontratacao.FirstOrDefault().Emitente?.Cliente;

                                            if (tomador == null)
                                                tomador = cargaPedido.Pedido.SubContratante != null ? cargaPedido.Pedido.SubContratante : cargaPedido.Tomador;

                                            carga.AgNFSManual = true;
                                            serNFS.CriarCTePendenteEmissaoManualDeNFS(carga, pedidosCTeParaSubContratacao.FirstOrDefault(), tomador, inicioPrestacao, pedidosCTeParaSubContratacao.FirstOrDefault().ValorFrete, unitOfWork);
                                            unitOfWork.CommitChanges();
                                        }
                                    }

                                    if (totalDocumentosGerados == 1 || (totalDocumentosGerados % quantidadeDocumentosAtualizarCarga) == 0)
                                        svcHubCarga.InformarQuantidadeDocumentosEmitidos(carga.Codigo, totalDocumentos, totalDocumentosGerados);

                                    unitOfWork.FlushAndClear();
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

                if (emitiu)
                    svcHubCarga.InformarQuantidadeDocumentosEmitidos(carga.Codigo, totalDocumentos, totalDocumentosGerados);
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS BuscarRegraICMS(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Localidade inicioPrestacao, Dominio.Entidades.Localidade fimPrestacao, Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS();
            regraICMS.Aliquota = 0;
            if (pedidoCTeParaSubContratacao.CFOP != null)
            {
                regraICMS.CFOP = pedidoCTeParaSubContratacao.CFOP.CodigoCFOP;
            }
            else
            {
                Servicos.Embarcador.Carga.ICMS svcICMS = new ICMS(unitOfWork);

                Dominio.Entidades.Aliquota aliquota = svcICMS.ObterAliquota(carga.Empresa.Localidade.Estado, inicioPrestacao.Estado, fimPrestacao.Estado, pedidoCTeParaSubContratacao.CTeTerceiro.Emitente.Atividade, pedidoCTeParaSubContratacao.CTeTerceiro.Destinatario.Atividade, unitOfWork);

                regraICMS.CFOP = aliquota?.CFOP?.CodigoCFOP ?? pedidoCTeParaSubContratacao.CTeTerceiro.CFOP.CodigoCFOP;
            }

            regraICMS.CFOP = 6351;
            regraICMS.CST = "41";
            regraICMS.PercentualReducaoBC = 0;
            regraICMS.PercentualInclusaoBC = 0;
            regraICMS.ValorICMS = 0;
            regraICMS.ValorICMSIncluso = 0;
            regraICMS.ObservacaoCTe = "Não tributado conforme AJUSTE SINEF 10, DE 8 DE JULHO DE 2016";
            regraICMS.IncluirICMSBC = false;
            regraICMS.ValorBaseCalculoICMS = 0;
            regraICMS.ValorBaseCalculoPISCOFINS = 0;
            regraICMS.DescontarICMSDoValorAReceber = false;
            regraICMS.CodigoRegra = pedidoCTeParaSubContratacao.RegraICMS?.Codigo ?? 0;

            return regraICMS;
        }

        private Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS BuscarRegraISS(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = new Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS();

            regraISS.AliquotaISS = 0;
            regraISS.IncluirISSBaseCalculo = false;
            regraISS.PercentualRetencaoISS = 0;
            regraISS.ValorRetencaoISS = 0;

            regraISS.ReterIR = false;
            regraISS.AliquotaIR = 0;
            regraISS.BaseCalculoIR = 0;
            regraISS.ValorIR = 0;

            return regraISS;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> BuscarPedidoCTeSubcontratacaoComponentesFrete(Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete repPedidoCteParaSubContratacaoComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete(unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> cargaPedidoComponentesFretesCliente = new List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico>();
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete> pedidoCteParaSubContratacaoComponentesFrete = repPedidoCteParaSubContratacaoComponenteFrete.BuscarPorPedidoCTeParaSubcontratacao(pedidoCTeParaSubContratacao.Codigo, modeloDocumento);

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete pedidoCteParaSubContratacaoComponenteFrete in pedidoCteParaSubContratacaoComponentesFrete)
            {
                if (cargaPedidoComponentesFretesCliente.Exists(obj => obj.TipoComponenteFrete == pedidoCteParaSubContratacaoComponenteFrete.TipoComponenteFrete && obj.ComponenteFrete?.Codigo == pedidoCteParaSubContratacaoComponenteFrete.ComponenteFrete?.Codigo))
                {
                    cargaPedidoComponentesFretesCliente.Find(obj => obj.TipoComponenteFrete == pedidoCteParaSubContratacaoComponenteFrete.TipoComponenteFrete && obj.ComponenteFrete?.Codigo == pedidoCteParaSubContratacaoComponenteFrete.ComponenteFrete?.Codigo).ValorComponente += pedidoCteParaSubContratacaoComponenteFrete.ValorComponente;
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico componente = pedidoCteParaSubContratacaoComponenteFrete.ConvertarParaComponenteDinamico();
                    cargaPedidoComponentesFretesCliente.Add(componente);
                }
            }
            return cargaPedidoComponentesFretesCliente;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> BuscarPedidoCTeSubcontratacaoComponentesFrete(List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoCTeParaSubContratacao, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete repPedidoCteParaSubContratacaoComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete(unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> cargaPedidoComponentesFretesCliente = new List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico>();
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete> pedidoCteParaSubContratacaoComponentesFrete = repPedidoCteParaSubContratacaoComponenteFrete.BuscarPorPedidoCTeParaSubcontratacao(pedidoCTeParaSubContratacao.Select(o => o.Codigo).ToList(), modeloDocumento);

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete pedidoCteParaSubContratacaoComponenteFrete in pedidoCteParaSubContratacaoComponentesFrete)
            {
                if (cargaPedidoComponentesFretesCliente.Exists(obj => obj.TipoComponenteFrete == pedidoCteParaSubContratacaoComponenteFrete.TipoComponenteFrete && obj.ComponenteFrete?.Codigo == pedidoCteParaSubContratacaoComponenteFrete.ComponenteFrete?.Codigo))
                {
                    cargaPedidoComponentesFretesCliente.Find(obj => obj.TipoComponenteFrete == pedidoCteParaSubContratacaoComponenteFrete.TipoComponenteFrete && obj.ComponenteFrete?.Codigo == pedidoCteParaSubContratacaoComponenteFrete.ComponenteFrete?.Codigo).ValorComponente += pedidoCteParaSubContratacaoComponenteFrete.ValorComponente;
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico componente = pedidoCteParaSubContratacaoComponenteFrete.ConvertarParaComponenteDinamico();
                    cargaPedidoComponentesFretesCliente.Add(componente);
                }
            }
            return cargaPedidoComponentesFretesCliente;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ConverterCTesTerceirosParaAnteriores(List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesTerceiro)
        {
            Servicos.WebService.Empresa.Empresa serEmpresa = new Servicos.WebService.Empresa.Empresa(_unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ctesAnteriores = new List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe>();
            foreach (Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro in ctesTerceiro)
            {
                Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteAnterior = new Dominio.ObjetosDeValor.Embarcador.CTe.CTe();
                cteAnterior.Emitente = serEmpresa.ConverterObjetoEmpresa(cteTerceiro.Emitente);
                cteAnterior.Chave = cteTerceiro.ChaveAcesso;
                cteAnterior.Numero = cteTerceiro.Numero;
                ctesAnteriores.Add(cteAnterior);
            }

            return ctesAnteriores;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ConverterCTesTerceirosParaAnteriores(Dominio.Entidades.ConhecimentoDeTransporteEletronico cteTerceiro, Dominio.Entidades.Cliente tomador)
        {
            Servicos.WebService.Empresa.Empresa serEmpresa = new Servicos.WebService.Empresa.Empresa(_unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ctesAnteriores = new List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe>();

            Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteAnterior = new Dominio.ObjetosDeValor.Embarcador.CTe.CTe();
            cteAnterior.Emitente = serEmpresa.ConverterObjetoEmpresa(tomador);
            cteAnterior.Chave = cteTerceiro.Chave;
            cteAnterior.Numero = cteTerceiro.Numero;
            ctesAnteriores.Add(cteAnterior);

            return ctesAnteriores;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> ConverterQuantidadesTerceirosParaQuantidadesCTe(List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade> quantidadesTerceiros)
        {
            List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidades = new List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga>();

            foreach (Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade quantidadeTerceiro in quantidadesTerceiros)
            {
                Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga quantidadeCarga = new Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga();
                quantidadeCarga.Unidade = quantidadeTerceiro.Unidade;
                quantidadeCarga.Quantidade = quantidadeTerceiro.Quantidade;
                quantidadeCarga.Medida = quantidadeTerceiro.TipoMedida;
                quantidades.Add(quantidadeCarga);
            }

            return quantidades;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> ConverterSegurosDeTerceirosEmSeguroCTe(List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroSeguro> segurosTerceiro)
        {
            List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> apolicesSeguro = new List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro>();

            foreach (Dominio.Entidades.Embarcador.CTe.CTeTerceiroSeguro seguroTerceiro in segurosTerceiro)
            {
                Dominio.ObjetosDeValor.Embarcador.CTe.Seguro seguro = new Dominio.ObjetosDeValor.Embarcador.CTe.Seguro();
                seguro.Apolice = seguroTerceiro.NumeroApolice;
                seguro.Averbacao = seguroTerceiro.NumeroAverbacao;
                seguro.ResponsavelSeguro = seguroTerceiro.Tipo;
                seguro.Seguradora = seguroTerceiro.NomeSeguradora;
                seguro.Valor = seguroTerceiro.Valor;
                apolicesSeguro.Add(seguro);
            }

            return apolicesSeguro;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> ConverterQuantidadesTerceirosParaQuantidadesCTe(List<Dominio.Entidades.InformacaoCargaCTE> quantidadesTerceiros)
        {
            List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidades = new List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga>();

            foreach (Dominio.Entidades.InformacaoCargaCTE quantidadeTerceiro in quantidadesTerceiros)
            {
                Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga quantidadeCarga = new Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga();
                quantidadeCarga.Unidade = (Dominio.Enumeradores.UnidadeMedida)int.Parse(quantidadeTerceiro.UnidadeMedida);
                quantidadeCarga.Quantidade = quantidadeTerceiro.Quantidade;
                quantidadeCarga.Medida = quantidadeTerceiro.DescricaoUnidadeMedida;
                quantidades.Add(quantidadeCarga);
            }

            return quantidades;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> ConverterSegurosDeTerceirosEmSeguroCTe(List<Dominio.Entidades.SeguroCTE> segurosTerceiro)
        {
            List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> apolicesSeguro = new List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro>();

            foreach (Dominio.Entidades.SeguroCTE seguroTerceiro in segurosTerceiro)
            {
                Dominio.ObjetosDeValor.Embarcador.CTe.Seguro seguro = new Dominio.ObjetosDeValor.Embarcador.CTe.Seguro();
                seguro.Apolice = seguroTerceiro.NumeroApolice;
                seguro.Averbacao = seguroTerceiro.NumeroAverbacao;
                seguro.ResponsavelSeguro = seguroTerceiro.Tipo;
                seguro.Seguradora = seguroTerceiro.NomeSeguradora;
                seguro.Valor = seguroTerceiro.Valor;
                apolicesSeguro.Add(seguro);
            }

            return apolicesSeguro;
        }

        private decimal CalcularValorSVM(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal tipoPropostaMultimodal, string chaveCTe, List<string> listaChavesCTe, int qtdDocumentos, decimal valorCusteioSVM, decimal valorFreteNegociado, decimal baseICMS, string numeroContainer, string numeroBooking, Repositorio.UnitOfWork unitOfWork, int qtdContainer, List<int> codigosContainere)
        {
            Repositorio.ContainerCTE repContainerCTE = new Repositorio.ContainerCTE(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.InformacaoCargaCTE informacaoCargaCTE = new Repositorio.InformacaoCargaCTE(unitOfWork);

            decimal valorFrete = 1;
            if (tipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada)
            {
                decimal pesoTotalNotas = repContainerCTE.BuscarPesoNotasPorCTe(listaChavesCTe, numeroContainer);
                decimal volumeM3Notas = repContainerCTE.BuscarMetrosCubicosNotasPorCTe(listaChavesCTe, numeroContainer);
                decimal dencidadeProduto = repPedidoCTeParaSubContratacao.ObterDencidadeProdutoPedido(chaveCTe);
                decimal dencidadeParametroFixo = 300;
                decimal valorBaseCalculoSVM = 0;
                if (pesoTotalNotas >= (volumeM3Notas * dencidadeParametroFixo))
                    valorBaseCalculoSVM = (valorFreteNegociado * (pesoTotalNotas / 1000));
                else
                    valorBaseCalculoSVM = (valorFreteNegociado * ((volumeM3Notas * dencidadeProduto) / 100));

                valorFrete = (valorFreteNegociado - valorCusteioSVM - valorBaseCalculoSVM);
                valorFrete = (valorFrete / baseICMS);
                if (valorFrete <= 0)
                    valorFrete = 1;
                else
                {
                    //alterado foruma de rateio de container para peso do CT-e                    
                    decimal totalFreteMTL = 0;
                    foreach (var chaveCTeMTL in listaChavesCTe)
                    {
                        pesoTotalNotas = repContainerCTE.BuscarPesoCTeContainer(chaveCTeMTL, numeroBooking);
                        decimal pesoMTL = informacaoCargaCTE.BuscarPorChaveCTeUnidade(chaveCTeMTL);
                        if (pesoTotalNotas <= 0)
                            pesoTotalNotas = repContainerCTE.BuscarPesoCTeContainer(chaveCTeMTL);
                        if (pesoTotalNotas <= 0)
                            pesoTotalNotas = 1;
                        if (pesoMTL <= 0)
                            pesoMTL = 1;
                        decimal percentual = pesoMTL / pesoTotalNotas;
                        totalFreteMTL += (percentual * valorFrete);
                    }
                    valorFrete = totalFreteMTL;
                }
            }
            else
            {
                valorFrete = (valorFreteNegociado - valorCusteioSVM);
                valorFrete = (valorFrete / baseICMS);
                if (valorFrete <= 0)
                    valorFrete = 1;
                else
                {
                    //alterado foruma de rateio de container para peso do CT-e                    
                    decimal totalFreteMTL = 0;
                    foreach (var chaveCTeMTL in listaChavesCTe)
                    {
                        decimal pesoTotalNotas = repContainerCTE.BuscarPesoCTeContainer(chaveCTeMTL, numeroBooking);
                        decimal pesoMTL = informacaoCargaCTE.BuscarPorChaveCTeUnidade(chaveCTeMTL);
                        if (pesoTotalNotas <= 0)
                            pesoTotalNotas = repContainerCTE.BuscarPesoCTeContainer(chaveCTeMTL);
                        if (pesoTotalNotas <= 0)
                            pesoTotalNotas = 1;
                        if (pesoMTL <= 0)
                            pesoMTL = 1;
                        decimal percentual = pesoMTL / pesoTotalNotas;
                        totalFreteMTL += (percentual * valorFrete);
                    }
                    valorFrete = totalFreteMTL;
                }
            }
            return valorFrete;
        }

        private void SalvarNumeroControleSVM(int codigoCargaPedido, string numeroControleSVM, Repositorio.UnitOfWork unitOfWork, int codigoCTeSVM)
        {
            if (!string.IsNullOrWhiteSpace(numeroControleSVM))
            {
                try
                {
                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                    Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                    Repositorio.Embarcador.CTe.CTeSVMMultimodal repCTeSVMMultimodal = new Repositorio.Embarcador.CTe.CTeSVMMultimodal(unitOfWork);

                    List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCTe.BuscarPorCargaPedidoDocumentoCTe(codigoCargaPedido);
                    foreach (var cte in ctes)
                    {
                        cte.NumeroControleSVM = numeroControleSVM;
                        repCTe.Atualizar(cte);

                        Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCTe(cte.Codigo);
                        if (cargaCTe != null)
                        {
                            Dominio.Entidades.Embarcador.CTe.CTeSVMMultimodal multimodal = new Dominio.Entidades.Embarcador.CTe.CTeSVMMultimodal()
                            {
                                CargaMultimodal = cargaCTe.Carga,
                                CTeMultimodal = cte,
                                CTeSVM = repCTe.BuscarPorCodigo(codigoCTeSVM)
                            };

                            repCTeSVMMultimodal.Inserir(multimodal);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro("Erro ao salvar o numero de controle gerado do SVM : " + ex);
                }
            }
        }

        #endregion
    }
}
