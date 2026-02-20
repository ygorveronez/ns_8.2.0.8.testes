using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Servicos.Embarcador.Carga
{
    public class CTePorCTeParaSubcontratacao : ServicoBase
    {
        public CTePorCTeParaSubcontratacao(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        #region Métodos Públicos

        public void GerarCTePorSubcontratacaoIndividual(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, int totalDocumentos, int quantidadeDocumentosAtualizarCarga, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao> pedidosCTeParaSubContratacaoContaContabilContabilizacao, ref int totalDocumentosGerados)
        {
            CTe serCte = new CTe(unitOfWork);
            CTeSubContratacaoEmissao serCTeSubContratacaoEmissao = new CTeSubContratacaoEmissao(unitOfWork);
            Servicos.Embarcador.Carga.NFS serNFS = new NFS(unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroQuantidade repCTeParaSubContratacaoQuantidade = new Repositorio.Embarcador.CTe.CTeTerceiroQuantidade(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repPedidoCTeParaSubContratacaoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.CTe.CTeDocumentoOriginario repDocumentoOriginario = new Repositorio.Embarcador.CTe.CTeDocumentoOriginario(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.ContainerTipo repContainerTipo = new Repositorio.Embarcador.Pedidos.ContainerTipo(unitOfWork);

            CTePorPedido serCTePorPedido = new CTePorPedido(unitOfWork);
            Hubs.Carga svcHubCarga = new Hubs.Carga();
            Servicos.Embarcador.Pedido.PedidoCTeParaSubContratacao servicoPedidoCTeParaSubContratacao = new Pedido.PedidoCTeParaSubContratacao(unitOfWork);

            for (int i = 0; i < cargaPedidos.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(cargaPedidos[i].Codigo);

                if (!cargaPedido.PossuiCTe && !cargaPedido.PossuiNFS && !cargaPedido.PossuiNFSManual && !(cargaPedido.PossuiNFSManual && cargaPedido.ModeloDocumentoFiscalIntramunicipal != null))
                    continue;

                bool utilizarExpedidorRecebedorPedido = cargaPedido.Carga.TipoOperacao?.ConfiguracaoEmissaoDocumento?.UtilizarExpedidorRecebedorPedidoSubcontratacao ?? false;
                Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem = (from obj in cargasOrigem where obj.Codigo == cargaPedido.CargaOrigem.Codigo select obj).FirstOrDefault();

                bool emitirCteFilialEmissora = false;
                if (cargaPedido.Carga.EmpresaFilialEmissora != null && !cargaPedido.Carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                    emitirCteFilialEmissora = true;
                Dominio.Entidades.Empresa empresa = cargaOrigem.Empresa;
                if (emitirCteFilialEmissora)
                    empresa = cargaOrigem.EmpresaFilialEmissora;

                bool emitiu = false;

                if (!cargaPedido.CTeEmitidoNoEmbarcador && !cargaPedido.Pedido.PedidoTransbordo)
                {
                    List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> apolicesSeguro = repApoliceSeguroAverbacao.BuscarPorCargaPedido(cargaPedido.Codigo, emitirCteFilialEmissora);

                    serCte.ObterDescricoesComponentesPadrao(cargaPedido.ObterTomador(), carga, configuracaoEmbarcador, unitOfWork, out string descricaoComponenteValorFrete, out string descricaoComponenteValorICMS);

                    bool somenteFilialEmissora = false;
                    if (cargaPedido.CargaPedidoFilialEmissora && !carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                        somenteFilialEmissora = true;

                    List<int> pedidoCTesParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarCodigosPorCargaPedido(cargaPedido.Codigo, somenteFilialEmissora);

                    foreach (int codigoPedidoCTeParaSubContratacao in pedidoCTesParaSubContratacao)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarPorCodigoComFetch(codigoPedidoCTeParaSubContratacao);

                        Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalCarga = pedidoCTeParaSubContratacao.ModeloDocumentoFiscal;

                        if (modeloDocumentoFiscalCarga == null)
                            modeloDocumentoFiscalCarga = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.CTe);

                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> pedidoCTeParaSubContratacaoNotasFiscais = repPedidoCTeParaSubContratacaoNotaFiscal.BuscarPorPedidoCTeParaSubcontratacao(pedidoCTeParaSubContratacao.Codigo);
                        Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = pedidoCTeParaSubContratacaoNotasFiscais.Count > 0 ? repPedidoXMLNotaFiscal.BuscarPorCodigo(pedidoCTeParaSubContratacaoNotasFiscais.First().PedidoXMLNotaFiscal.Codigo) : null;

                        Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteParaSubcontratacao = pedidoCTeParaSubContratacao.CTeTerceiro;

                        Dominio.Entidades.Cliente expedidor = null;
                        Dominio.Entidades.Cliente recebedor = null;
                        Dominio.Entidades.Localidade inicioPrestacao = cteParaSubcontratacao.LocalidadeInicioPrestacao;
                        Dominio.Entidades.Localidade terminoPrestacao = cteParaSubcontratacao.LocalidadeTerminoPrestacao;
                        Dominio.Entidades.Cliente remetente = cteParaSubcontratacao.Remetente.Cliente ?? repCliente.BuscarPorCPFCNPJ(double.Parse(cteParaSubcontratacao.Remetente.CPF_CNPJ_SemFormato));
                        Dominio.Entidades.Cliente destinatario = cteParaSubcontratacao.Destinatario.Cliente ?? repCliente.BuscarPorCPFCNPJ(double.Parse(cteParaSubcontratacao.Destinatario.CPF_CNPJ_SemFormato));

                        //if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        //{

                        Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoDestinatario = null;

                        if ((cargaPedido.EmitirComplementarFilialEmissora && emitirCteFilialEmissora) || carga.EmitirCTeComplementar)
                        {

                            if (carga.EmitirCTeComplementar && cteParaSubcontratacao.Emitente.CPF_CNPJ_SemFormato != empresa.CNPJ_SemFormato)
                                empresa = repEmpresa.BuscarPorCNPJ(cteParaSubcontratacao.Emitente.CPF_CNPJ_SemFormato);

                            if (cteParaSubcontratacao.Expedidor != null)
                                expedidor = cteParaSubcontratacao.Expedidor.Cliente;
                            if (cteParaSubcontratacao.Recebedor != null)
                                recebedor = cteParaSubcontratacao.Recebedor.Cliente;
                        }
                        else
                        {
                            if (utilizarExpedidorRecebedorPedido || (configuracaoEmbarcador.UtilizaEmissaoMultimodal && cargaPedido.Expedidor != null))
                            {
                                expedidor = cargaPedido.Expedidor;

                                if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada &&
                                    cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada &&
                                    expedidor != null)
                                    inicioPrestacao = expedidor.Localidade;
                                else if (utilizarExpedidorRecebedorPedido && expedidor == null && remetente != null)
                                    inicioPrestacao = remetente.Localidade;
                            }
                            else if (cteParaSubcontratacao.Expedidor != null && (cteParaSubcontratacao.Expedidor.CPF_CNPJ != cteParaSubcontratacao.Remetente.CPF_CNPJ || pedidoCTeParaSubContratacao.CargaPedido.Expedidor == null) && !pedidoCTeParaSubContratacao.CargaPedido.PedidoEncaixado && (!cargaPedido.EmitirComplementarFilialEmissora || !emitirCteFilialEmissora))
                                expedidor = cteParaSubcontratacao.Expedidor.Cliente ?? repCliente.BuscarPorCPFCNPJ(double.Parse(cteParaSubcontratacao.Expedidor.CPF_CNPJ_SemFormato));
                            else if (pedidoCTeParaSubContratacao.CargaPedido.Expedidor != null)
                            {
                                expedidor = pedidoCTeParaSubContratacao.CargaPedido.Expedidor;
                                if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
                                    inicioPrestacao = expedidor.Localidade;
                            }

                            if (utilizarExpedidorRecebedorPedido || ((configuracaoEmbarcador.UtilizaEmissaoMultimodal || (carga.TipoOperacao?.UtilizarRecebedorPedidoParaSVM ?? false)) && cargaPedido.Recebedor != null))
                            {
                                recebedor = cargaPedido.Recebedor;

                                if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada &&
                                    cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada &&
                                    recebedor != null)
                                    terminoPrestacao = recebedor.Localidade;
                                else if (utilizarExpedidorRecebedorPedido && recebedor == null && destinatario != null)
                                    terminoPrestacao = destinatario.Localidade;
                            }
                            else if (cteParaSubcontratacao.Recebedor != null && (cteParaSubcontratacao.Recebedor.CPF_CNPJ != cteParaSubcontratacao.Destinatario.CPF_CNPJ || pedidoCTeParaSubContratacao.CargaPedido.Recebedor == null) && !pedidoCTeParaSubContratacao.CargaPedido.PedidoEncaixado && (!cargaPedido.EmitirComplementarFilialEmissora || !emitirCteFilialEmissora))
                            {
                                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor))
                                    recebedor = cteParaSubcontratacao.Recebedor.Cliente ?? repCliente.BuscarPorCPFCNPJ(double.Parse(cteParaSubcontratacao.Recebedor.CPF_CNPJ_SemFormato));
                                else //todo: if feito por um caso danone onde os operadores emitirar o Ct-e da transportara danone errado, nesse caso o cte de subcontratacao deve ser somente até o destinatario rever isso (09/10/2019 Rodrigo)
                                    terminoPrestacao = destinatario.Localidade;
                            }
                            else if (pedidoCTeParaSubContratacao.CargaPedido.Recebedor != null)
                            {
                                recebedor = pedidoCTeParaSubContratacao.CargaPedido.Recebedor;
                                if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
                                    terminoPrestacao = recebedor.Localidade;
                            }

                            if (cargaPedido.Pedido.UsarOutroEnderecoDestino)
                            {
                                enderecoDestinatario = cargaPedido.Pedido.EnderecoDestino;
                                if (cargaPedido.Pedido?.EnderecoDestino?.Localidade != null)
                                    terminoPrestacao = cargaPedido.Pedido.EnderecoDestino.Localidade;
                            }
                        }

                        if (!pedidoCTeParaSubContratacao.PossuiNFSManual ||
                            (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS &&
                            (inicioPrestacao.Codigo != terminoPrestacao.Codigo || (pedidoCTeParaSubContratacao.PossuiNFSManual && cargaPedido.ModeloDocumentoFiscalIntramunicipal != null && (inicioPrestacao.Codigo == terminoPrestacao.Codigo || cargaPedido.DisponibilizarDocumentoNFSManual)))))
                        {
                            bool somenteCTeSubContratacaoFilialEmissora = false;

                            carga = repCarga.BuscarPorCodigo(carga.Codigo);

                            if (cargaPedido.CargaPedidoFilialEmissora && carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                                somenteCTeSubContratacaoFilialEmissora = true;

                            if (repCargaPedidoXMLNotaFiscalCTe.ContarCTesPorPedidoCTeParaSubContratacao(pedidoCTeParaSubContratacao.Codigo, somenteCTeSubContratacaoFilialEmissora) > 0)
                            {
                                Servicos.Log.GravarInfo($"Carga Pedido: {cargaPedido.Codigo} - Pulando geração do pedidoCTeParaSubContratacao.Codigo: {pedidoCTeParaSubContratacao.Codigo}, somenteCTeSubContratacaoFilialEmissora: {somenteCTeSubContratacaoFilialEmissora}", "EmissaoCTeCargaPedidoSubcontratacao");
                                continue;
                            }
                            else
                                Servicos.Log.GravarInfo($"Carga Pedido: {cargaPedido.Codigo} - Emitindo pedidoCTeParaSubContratacao.Codigo: {pedidoCTeParaSubContratacao.Codigo}, somenteCTeSubContratacaoFilialEmissora: {somenteCTeSubContratacaoFilialEmissora}", "EmissaoCTeCargaPedidoSubcontratacao");

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

                            bool contemNFe = pedidoCTeParaSubContratacaoNotasFiscais.Any(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal.Modelo == "55");
                            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoNotaFiscal in pedidoCTeParaSubContratacaoNotasFiscais)
                            {
                                if (contemNFe && !cte.Documentos.Any(obj => obj.ChaveNFE == pedidoCTeParaSubContratacaoNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Chave))
                                {
                                    Dominio.ObjetosDeValor.CTe.Documento docNF = serCte.BuscarDocumentoCTe(pedidoCTeParaSubContratacaoNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal, empresa.TipoAmbiente, configuracaoEmbarcador);
                                    cte.Documentos.Add(docNF);
                                }
                                else if (!contemNFe)
                                {
                                    Dominio.ObjetosDeValor.CTe.Documento docNF = serCte.BuscarDocumentoCTe(pedidoCTeParaSubContratacaoNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal, empresa.TipoAmbiente, configuracaoEmbarcador);
                                    cte.Documentos.Add(docNF);
                                }
                            }

                            cte.Peso = cteParaSubcontratacao.Peso;
                            cte.Volumes = cteParaSubcontratacao.Volumes;
                            cte.MetrosCubicos = cteParaSubcontratacao.MetrosCubicos;
                            cte.PesoCubado = cteParaSubcontratacao.PesoCubado;
                            cte.FatorCubagem = cteParaSubcontratacao.FatorCubagem;
                            cte.PesoFaturado = cteParaSubcontratacao.Peso > cteParaSubcontratacao.PesoCubado ? cteParaSubcontratacao.Peso : cteParaSubcontratacao.PesoCubado;
                            cte.PercentualPagamentoAgregado = pedidoCTeParaSubContratacao.PercentualPagamentoAgregado;

                            List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> cargaPedidoComponentesFretesCliente = BuscarPedidoCTeSubcontratacaoComponentesFrete(pedidoCTeParaSubContratacao, modeloDocumentoFiscalCarga, unitOfWork);

                            cte.ValorTotalMercadoria = cteParaSubcontratacao.ValorTotalMercadoria;
                            cte.ValorAReceber = pedidoCTeParaSubContratacao.ValorFrete;
                            cte.ValorFrete = pedidoCTeParaSubContratacao.ValorFrete;
                            cte.ValorTotalPrestacaoServico = pedidoCTeParaSubContratacao.ValorFrete;
                            cte.ValorTotalMoeda = pedidoCTeParaSubContratacao.ValorTotalMoeda + cargaPedidoComponentesFretesCliente.Sum(o => o.ValorTotalMoeda ?? 0m);

                            decimal valorICMS = pedidoCTeParaSubContratacao.ValorICMS;
                            decimal aliquota = pedidoCTeParaSubContratacao.PercentualAliquota;

                            if (cargaPedido.Pedido.EmpresaSerie != null)
                                cte.Serie = cargaPedido.Pedido.EmpresaSerie.Numero;

                            //if (!string.IsNullOrWhiteSpace(cargaPedido.Pedido.ObservacaoCTe))
                            //    cte.ObservacoesGerais = cargaPedido.Pedido.ObservacaoCTe;

                            cte.ProdutoPredominante = cteParaSubcontratacao.ProdutoPredominante;

                            List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade> quantidades = repCTeParaSubContratacaoQuantidade.BuscarPorCTeParaSubContratacao(cteParaSubcontratacao.Codigo);

                            if (configuracaoEmbarcador.UtilizaEmissaoMultimodal)
                                quantidades = repCTeParaSubContratacaoQuantidade.BuscarPorCTeParaSubContratacaoSemPesoTotal(cteParaSubcontratacao.Codigo);

                            Dominio.Entidades.Cliente tomador = Servicos.Embarcador.Carga.CTeSubContratacao.ObterTomadorCTeParaSubcontratacao(carga, cargaPedido, pedidoCTeParaSubContratacao, configuracaoEmbarcador, tipoServicoMultisoftware, modeloDocumentoEmitir);

                            Dominio.Enumeradores.TipoPagamento tipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;
                            Dominio.Enumeradores.TipoTomador tipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
                            List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesParaSubcontrataca = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>();
                            ctesParaSubcontrataca.Add(cteParaSubcontratacao);

                            List<string> tiposContaineres = configuracaoEmbarcador.UtilizaEmissaoMultimodal ? repContainerTipo.BuscarDescricoes() : new List<string>();
                            bool utilizarPrimeiraUnidadeMedidaPeso = UtilizarPrimeiraUnidadeMedidaPeso(tomador, carga.TipoOperacao);
                            List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidadesCTes = ConverterQuantidadesTerceirosParaQuantidadesCTe(quantidades, configuracaoEmbarcador.AgruparUnidadesMedidasPorDescricao, utilizarPrimeiraUnidadeMedidaPeso, tiposContaineres);

                            bool emitindoCTeFilialEmissora = false;
                            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeFilialEmissora = pedidoCTeParaSubContratacao.CTeTerceiro.ObterCargaCTe(carga.Codigo);

                            if (cargaPedido.CargaPedidoFilialEmissora && !emitirCteFilialEmissora)
                                emitindoCTeFilialEmissora = true;

                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga tipoContratacaoCarga = cargaPedido.TipoContratacaoCarga;

                            if (emitindoCTeFilialEmissora)
                                tipoContratacaoCarga = cargaPedido.TipoContratacaoCargaSubContratacaoFilialEmissora;

                            Dominio.Enumeradores.TipoServico tipoServico = Dominio.Enumeradores.TipoServico.SubContratacao;

                            if (carga.CargaSVM)
                                tipoServico = Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal;
                            else if (carga.CargaSVMTerceiro)
                                tipoServico = Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal;
                            else if (tipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho)
                                tipoServico = Dominio.Enumeradores.TipoServico.Redespacho;
                            else if (tipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario)
                                tipoServico = Dominio.Enumeradores.TipoServico.RedIntermediario;

                            List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ctesAnteriores = ConverterCTesTerceirosParaAnteriores(ctesParaSubcontrataca, unitOfWork);

                            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = BuscarRegraICMS(cargaOrigem, inicioPrestacao, terminoPrestacao, pedidoCTeParaSubContratacao, unitOfWork);
                            Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = BuscarRegraISS(cargaOrigem, pedidoCTeParaSubContratacao, inicioPrestacao, tomador, unitOfWork);
                            Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = servicoPedidoCTeParaSubContratacao.ObterRetornoImpostoIBSCBS(pedidoCTeParaSubContratacao);

                            List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> seguros = serCte.ConverterSegurosDeTerceirosEmSeguroCTe(apolicesSeguro, tipoTomador, cte.ValorTotalMercadoria);

                            string observacaoCTe = null;

                            if (pedidoXMLNotaFiscal != null && !string.IsNullOrWhiteSpace(pedidoXMLNotaFiscal.CargaPedido.Pedido.ObservacaoCTe)) //essa informação por hora só existe no CT-e para subcontratação, pois não emitimos modal aéreo
                                observacaoCTe = pedidoXMLNotaFiscal.CargaPedido.Pedido.ObservacaoCTe.Replace("#NumeroOCADocumentoTransporteAnterior", cteParaSubcontratacao.NumeroOperacionalConhecimentoAereo?.ToString() ?? string.Empty);
                            else if (!string.IsNullOrWhiteSpace(cargaPedido.Pedido.ObservacaoCTe))
                                observacaoCTe = cargaPedido.Pedido.ObservacaoCTe.Replace("#NumeroOCADocumentoTransporteAnterior", cteParaSubcontratacao.NumeroOperacionalConhecimentoAereo?.ToString() ?? string.Empty);

                            Dominio.Enumeradores.TipoCTE tipoCTe = Dominio.Enumeradores.TipoCTE.Normal;

                            if (carga.EmitirCTeComplementar || (cargaPedido.EmitirComplementarFilialEmissora && emitirCteFilialEmissora))
                            {
                                if (carga.EmitirCTeComplementar && cteParaSubcontratacao.Emitente != null && !string.IsNullOrWhiteSpace(cteParaSubcontratacao.Emitente.CPF_CNPJ_SemFormato) && (empresa == null || cteParaSubcontratacao.Emitente.CPF_CNPJ_SemFormato != empresa.CNPJ_SemFormato))
                                    empresa = repEmpresa.BuscarPorCNPJ(cteParaSubcontratacao.Emitente.CPF_CNPJ_SemFormato);

                                cte.ChaveCTESubstituicaoComplementar = cteParaSubcontratacao.ChaveAcesso;
                                cte.TipoCTe = Dominio.Enumeradores.TipoCTE.Complemento;
                                cte.TipoServico = cteParaSubcontratacao.TipoServico;
                                tomador = cteParaSubcontratacao.Tomador.Cliente;
                                tipoTomador = cteParaSubcontratacao.TipoTomador;
                                tipoCTe = Dominio.Enumeradores.TipoCTE.Complemento;
                                tipoServico = cteParaSubcontratacao.TipoServico;
                            }

                            if ((cte.ValorFrete > 0m || cargaPedidoComponentesFretesCliente.Sum(obj => obj.ValorComponente) > 0m || (!configuracaoEmbarcador.NaoGerarCTesComValoresZerados && modeloDocumentoEmitir?.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe)))
                            {
                                List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil> configuracoes = Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil.ConverterPedidoCTeParaSubContratacaoContaContabilContabilizacao(pedidoCTeParaSubContratacao, pedidosCTeParaSubContratacaoContaContabilContabilizacao);
                                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTE = serCte.GerarCTe(carga, cargaPedido, cte, empresa, remetente, destinatario, tomador, expedidor, recebedor, inicioPrestacao, terminoPrestacao, null, enderecoDestinatario, tipoPagamento, tipoTomador, quantidadesCTes, cargaPedidoComponentesFretesCliente, observacaoCTe, pedidoXMLNotaFiscal?.CargaPedido.Pedido.ObservacaoCTeTerceiro ?? cargaPedido?.Pedido?.ObservacaoCTeTerceiro, pedidoXMLNotaFiscal?.CargaPedido.Pedido ?? cargaPedido?.Pedido, null, seguros, regraICMS, regraISS, impostoIBSCBS, tipoServicoMultisoftware, modeloDocumentoEmitir, tipoServico, tipoCTe, ctesAnteriores, 0, false, cargaCTeFilialEmissora, false, unitOfWork, configuracaoEmbarcador, configuracaoGeralCarga, pedidoCTeParaSubContratacao.CentroResultado, pedidoCTeParaSubContratacao.CentroResultadoDestinatario, pedidoCTeParaSubContratacao.ItemServico, pedidoCTeParaSubContratacao.CentroResultadoEscrituracao, pedidoCTeParaSubContratacao.CentroResultadoICMS, pedidoCTeParaSubContratacao.CentroResultadoPIS, pedidoCTeParaSubContratacao.CentroResultadoCOFINS, pedidoCTeParaSubContratacao.ValorMaximoCentroContabilizacao, configuracoes, pedidoCTeParaSubContratacaoNotasFiscais.Select(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal).ToList(), null, descricaoComponenteValorICMS, descricaoComponenteValorFrete);

                                if (emitindoCTeFilialEmissora && cargaCTeFilialEmissora != null)
                                {
                                    Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                                    cargaCTeFilialEmissora.CargaCTeSubContratacaoFilialEmissora = cargaCTE;
                                    repCargaCte.Atualizar(cargaCTeFilialEmissora);
                                }

                                serCte.AverbaCargaCTe(cargaCTE, apolicesSeguro, unitOfWork, configuracaoEmbarcador, cargaPedido.Pedido.FormaAverbacaoCTE);

                                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoNotaFiscal in pedidoCTeParaSubContratacaoNotasFiscais)
                                {
                                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTE = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe();
                                    cargaPedidoXMLNotaFiscalCTE.CargaCTe = cargaCTE;
                                    cargaPedidoXMLNotaFiscalCTE.PedidoXMLNotaFiscal = pedidoCTeParaSubContratacaoNotaFiscal.PedidoXMLNotaFiscal;
                                    repCargaPedidoXMLNotaFiscalCTe.Inserir(cargaPedidoXMLNotaFiscalCTE);
                                }

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
                                }

                                if (pedidoCTeParaSubContratacao.PossuiNFSManual)
                                {
                                    serNFS.CriarCargaCTePendenteEmissaoManualDeNFS(carga, cargaCTE, inicioPrestacao, unitOfWork);

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
                                unitOfWork.CommitChanges();
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


                        if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        {
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

        public void GerarCTePorSubcontratacaoAgrupadoPorDestinatarioDoPedido(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, int totalDocumentos, int quantidadeDocumentosAtualizarCarga, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao> pedidosCTeParaSubContratacaoContaContabilContabilizacao, ref int totalDocumentosGerados, bool emissaoMultimodal)
        {
            CTe serCte = new CTe(unitOfWork);
            CTeSubContratacaoEmissao serCTeSubContratacaoEmissao = new CTeSubContratacaoEmissao(unitOfWork);
            Servicos.Embarcador.Carga.NFS serNFS = new NFS(unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroQuantidade repCTeParaSubContratacaoQuantidade = new Repositorio.Embarcador.CTe.CTeTerceiroQuantidade(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repPedidoCTeParaSubContratacaoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.CTe.CTeDocumentoOriginario repDocumentoOriginario = new Repositorio.Embarcador.CTe.CTeDocumentoOriginario(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.ContainerTipo repContainerTipo = new Repositorio.Embarcador.Pedidos.ContainerTipo(unitOfWork);

            CTePorPedido serCTePorPedido = new CTePorPedido(unitOfWork);
            Hubs.Carga svcHubCarga = new Hubs.Carga();
            Servicos.Embarcador.Pedido.PedidoCTeParaSubContratacao servicoPedidoCTeParaSubContratacao = new Pedido.PedidoCTeParaSubContratacao(unitOfWork);

            if (cargaPedidos != null && cargaPedidos.Count > 0)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido primeiroCargaPedido = repCargaPedido.BuscarPorCodigo(cargaPedidos[0].Codigo);

                bool emitiu = false;

                if (!primeiroCargaPedido.CTeEmitidoNoEmbarcador && !primeiroCargaPedido.Pedido.PedidoTransbordo)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem = (from obj in cargasOrigem where obj.Codigo == primeiroCargaPedido.CargaOrigem.Codigo select obj).FirstOrDefault();

                    bool emitirCteFilialEmissora = false;
                    if (primeiroCargaPedido.Carga.EmpresaFilialEmissora != null && !primeiroCargaPedido.Carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                        emitirCteFilialEmissora = true;
                    Dominio.Entidades.Empresa empresa = cargaOrigem.Empresa;
                    if (emitirCteFilialEmissora)
                        empresa = cargaOrigem.EmpresaFilialEmissora;

                    serCte.ObterDescricoesComponentesPadrao(primeiroCargaPedido.ObterTomador(), carga, configuracaoEmbarcador, unitOfWork, out string descricaoComponenteValorFrete, out string descricaoComponenteValorICMS);

                    bool somenteFilialEmissora = false;
                    if (primeiroCargaPedido.CargaPedidoFilialEmissora && !carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                        somenteFilialEmissora = true;

                    bool utilizarExpedidorRecebedorPedido = carga.TipoOperacao?.ConfiguracaoEmissaoDocumento?.UtilizarExpedidorRecebedorPedidoSubcontratacao ?? false;

                    List<bool> indicardorGlobalizadoDestinatario = (from obj in cargaPedidos select obj.IndicadorCTeGlobalizadoDestinatario).Distinct().ToList();

                    foreach (bool indicadorGlobalizado in indicardorGlobalizadoDestinatario)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosGlobalizados = (from obj in cargaPedidos where obj.IndicadorCTeGlobalizadoDestinatario == indicadorGlobalizado select obj).ToList();

                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidosCTesParaSubcontracao = repPedidoCTeParaSubContratacao.BuscarPorCargaPedidoComFetch(cargaPedidosGlobalizados.Select(o => o.Codigo).ToList(), somenteFilialEmissora);
                        List<Dominio.Entidades.Cliente> remetentes = null;

                        if (emissaoMultimodal)
                            remetentes = (from obj in pedidosCTesParaSubcontracao select obj.CargaPedido.Pedido.Remetente).Distinct().ToList();
                        else
                            remetentes = (from obj in pedidosCTesParaSubcontracao select obj.CTeTerceiro.Remetente.Cliente).Distinct().ToList();

                        for (int r = 0; r < remetentes.Count; r++)
                        {
                            Dominio.Entidades.Cliente remetente = remetentes[r];
                            List<Dominio.Entidades.Cliente> destinatarios = null;

                            if (emissaoMultimodal)
                                destinatarios = (from obj in pedidosCTesParaSubcontracao where obj.CargaPedido.Pedido.Remetente.CPF_CNPJ == remetente.CPF_CNPJ select obj.CargaPedido.Pedido.Destinatario).Distinct().ToList();
                            else
                                destinatarios = (from obj in pedidosCTesParaSubcontracao where obj.CTeTerceiro.Remetente.Cliente.CPF_CNPJ == remetente.CPF_CNPJ select obj.CTeTerceiro.Destinatario.Cliente).Distinct().ToList();

                            if (indicadorGlobalizado)
                            {
                                Dominio.Entidades.Cliente dest = destinatarios.FirstOrDefault();
                                destinatarios = new List<Dominio.Entidades.Cliente>();
                                destinatarios.Add(dest);
                            }

                            for (int d = 0; d < destinatarios.Count; d++)
                            {
                                Dominio.Entidades.Cliente destinatario = destinatarios[d];
                                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null;
                                if (emissaoMultimodal)
                                    cargaPedido = pedidosCTesParaSubcontracao.Where(obj => obj.CargaPedido.Pedido.Remetente.CPF_CNPJ == remetente.CPF_CNPJ && obj.CargaPedido.Pedido.Destinatario.CPF_CNPJ == destinatario.CPF_CNPJ).Select(o => o.CargaPedido).FirstOrDefault();
                                else
                                    cargaPedido = pedidosCTesParaSubcontracao.Where(obj => obj.CTeTerceiro.Remetente.Cliente.CPF_CNPJ == remetente.CPF_CNPJ && obj.CTeTerceiro.Destinatario.Cliente.CPF_CNPJ == destinatario.CPF_CNPJ).Select(o => o.CargaPedido).FirstOrDefault();

                                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = null;
                                if (emissaoMultimodal)
                                    cargasPedidos = pedidosCTesParaSubcontracao.Where(obj => obj.CargaPedido.Pedido.Remetente.CPF_CNPJ == remetente.CPF_CNPJ && obj.CargaPedido.Pedido.Destinatario.CPF_CNPJ == destinatario.CPF_CNPJ).Select(o => o.CargaPedido).ToList();
                                else
                                    cargasPedidos = pedidosCTesParaSubcontracao.Where(obj => obj.CTeTerceiro.Remetente.Cliente.CPF_CNPJ == remetente.CPF_CNPJ && obj.CTeTerceiro.Destinatario.Cliente.CPF_CNPJ == destinatario.CPF_CNPJ).Select(o => o.CargaPedido).ToList();

                                if (!cargaPedido.PossuiCTe && !cargaPedido.PossuiNFS && !cargaPedido.PossuiNFSManual && !(cargaPedido.PossuiNFSManual && cargaPedido.ModeloDocumentoFiscalIntramunicipal != null))
                                    continue;

                                List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidosCTesParaSubcontracaoDestinatario = null;
                                if (!indicadorGlobalizado)
                                {
                                    if (emissaoMultimodal)
                                        pedidosCTesParaSubcontracaoDestinatario = (from obj in pedidosCTesParaSubcontracao where obj.CargaPedido.Pedido.Remetente.CPF_CNPJ == remetente.CPF_CNPJ && obj.CargaPedido.Pedido.Destinatario.CPF_CNPJ == destinatario.CPF_CNPJ select obj).ToList();
                                    else
                                        pedidosCTesParaSubcontracaoDestinatario = (from obj in pedidosCTesParaSubcontracao where obj.CTeTerceiro.Remetente.Cliente.CPF_CNPJ == remetente.CPF_CNPJ && obj.CTeTerceiro.Destinatario.Cliente.CPF_CNPJ == destinatario.CPF_CNPJ select obj).ToList();
                                }
                                else
                                    pedidosCTesParaSubcontracaoDestinatario = pedidosCTesParaSubcontracao;

                                Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacaoBase = pedidosCTesParaSubcontracaoDestinatario.FirstOrDefault();

                                Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalCarga = pedidoCTeParaSubContratacaoBase.ModeloDocumentoFiscal;

                                if (modeloDocumentoFiscalCarga == null)
                                    modeloDocumentoFiscalCarga = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.CTe);

                                List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> pedidoCTeParaSubContratacaoNotasFiscais = repPedidoCTeParaSubContratacaoNotaFiscal.BuscarPorPedidosCTeParaSubcontratacao((from obj in pedidosCTesParaSubcontracaoDestinatario select obj.Codigo).ToList());

                                Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteParaSubcontratacao = pedidoCTeParaSubContratacaoBase.CTeTerceiro;

                                Dominio.Entidades.Cliente expedidor = null;
                                Dominio.Entidades.Cliente recebedor = null;
                                Dominio.Entidades.Localidade inicioPrestacao = cteParaSubcontratacao.LocalidadeInicioPrestacao;
                                Dominio.Entidades.Localidade terminoPrestacao = cteParaSubcontratacao.LocalidadeTerminoPrestacao;


                                Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoDestinatario = null;

                                if ((cargaPedido.EmitirComplementarFilialEmissora && emitirCteFilialEmissora) || carga.EmitirCTeComplementar)
                                {
                                    if (carga.EmitirCTeComplementar && cteParaSubcontratacao.Emitente.CPF_CNPJ_SemFormato != empresa.CNPJ_SemFormato)
                                        empresa = repEmpresa.BuscarPorCNPJ(cteParaSubcontratacao.Emitente.CPF_CNPJ_SemFormato);

                                    if (cteParaSubcontratacao.Expedidor != null)
                                        expedidor = cteParaSubcontratacao.Expedidor.Cliente;
                                    if (cteParaSubcontratacao.Recebedor != null)
                                        recebedor = cteParaSubcontratacao.Recebedor.Cliente;
                                }
                                else
                                {
                                    if (utilizarExpedidorRecebedorPedido || (configuracaoEmbarcador.UtilizaEmissaoMultimodal && cargaPedido.Expedidor != null))
                                    {
                                        expedidor = cargaPedido.Expedidor;

                                        if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada &&
                                            cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada &&
                                            expedidor != null)
                                            inicioPrestacao = expedidor.Localidade;
                                        else if (utilizarExpedidorRecebedorPedido && expedidor == null && remetente != null)
                                            inicioPrestacao = remetente.Localidade;
                                    }
                                    else if (cteParaSubcontratacao.Expedidor != null && (cteParaSubcontratacao.Expedidor.CPF_CNPJ != cteParaSubcontratacao.Remetente.CPF_CNPJ || cargaPedido.Expedidor == null) && !cargaPedido.PedidoEncaixado && (!cargaPedido.EmitirComplementarFilialEmissora || !emitirCteFilialEmissora))
                                        expedidor = cteParaSubcontratacao.Expedidor.Cliente != null ? repCliente.BuscarPorCPFCNPJ(cteParaSubcontratacao.Expedidor.Cliente.CPF_CNPJ) : repCliente.BuscarPorCPFCNPJ(double.Parse(cteParaSubcontratacao.Expedidor.CPF_CNPJ_SemFormato));
                                    else if (cargaPedido.Expedidor != null)
                                    {
                                        expedidor = cargaPedido.Expedidor;
                                        if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
                                            inicioPrestacao = expedidor.Localidade;
                                    }

                                    if (utilizarExpedidorRecebedorPedido || ((configuracaoEmbarcador.UtilizaEmissaoMultimodal || (carga.TipoOperacao?.UtilizarRecebedorPedidoParaSVM ?? false)) && cargaPedido.Recebedor != null))
                                    {
                                        recebedor = cargaPedido.Recebedor;

                                        if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada &&
                                            cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada &&
                                            recebedor != null)
                                            terminoPrestacao = recebedor.Localidade;
                                        else if (utilizarExpedidorRecebedorPedido && recebedor == null && destinatario != null)
                                            terminoPrestacao = destinatario.Localidade;
                                    }
                                    else if (cteParaSubcontratacao.Recebedor != null && (cteParaSubcontratacao.Recebedor.CPF_CNPJ != cteParaSubcontratacao.Destinatario.CPF_CNPJ || cargaPedido.Recebedor == null) && !cargaPedido.PedidoEncaixado && (!cargaPedido.EmitirComplementarFilialEmissora || !emitirCteFilialEmissora))
                                    {
                                        if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor)
                                            recebedor = cteParaSubcontratacao.Recebedor.Cliente != null ? repCliente.BuscarPorCPFCNPJ(cteParaSubcontratacao.Recebedor.Cliente.CPF_CNPJ) : repCliente.BuscarPorCPFCNPJ(double.Parse(cteParaSubcontratacao.Recebedor.CPF_CNPJ_SemFormato));
                                        else //todo: if feito por um caso danone onde os operadores emitirar o Ct-e da transportara danone errado, nesse caso o cte de subcontratacao deve ser somente até o destinatario rever isso (09/10/2019 Rodrigo)
                                            terminoPrestacao = destinatario.Localidade;
                                    }
                                    else if (cargaPedido.Recebedor != null)
                                    {
                                        recebedor = cargaPedido.Recebedor;
                                        if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
                                            terminoPrestacao = recebedor.Localidade;
                                    }

                                    if (cargaPedido.Pedido.UsarOutroEnderecoDestino)
                                    {
                                        enderecoDestinatario = cargaPedido.Pedido.EnderecoDestino;
                                        if (cargaPedido.Pedido?.EnderecoDestino?.Localidade != null)
                                            terminoPrestacao = cargaPedido.Pedido.EnderecoDestino.Localidade;
                                    }
                                }

                                if (!pedidoCTeParaSubContratacaoBase.PossuiNFSManual ||
                                    (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS &&
                                    (inicioPrestacao.Codigo != terminoPrestacao.Codigo || (pedidoCTeParaSubContratacaoBase.PossuiNFSManual && cargaPedido.ModeloDocumentoFiscalIntramunicipal != null && (inicioPrestacao.Codigo == terminoPrestacao.Codigo || cargaPedido.DisponibilizarDocumentoNFSManual)))))
                                {
                                    List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> apolicesSeguro = repApoliceSeguroAverbacao.BuscarPorCargaPedido(cargaPedido.Codigo, emitirCteFilialEmissora);

                                    bool somenteCTeSubContratacaoFilialEmissora = false;

                                    if (cargaPedido.CargaPedidoFilialEmissora && carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                                        somenteCTeSubContratacaoFilialEmissora = true;

                                    if (repCargaPedidoXMLNotaFiscalCTe.ContarCTesPorPedidoCTeParaSubContratacao(pedidosCTesParaSubcontracaoDestinatario.Select(p => p.Codigo).ToList(), somenteCTeSubContratacaoFilialEmissora) > 0)
                                        continue;

                                    Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoEmitir = null;

                                    if (pedidoCTeParaSubContratacaoBase.PossuiNFSManual)
                                        modeloDocumentoEmitir = cargaPedido.ModeloDocumentoFiscalIntramunicipal;
                                    else
                                        modeloDocumentoEmitir = modeloDocumentoFiscalCarga;

                                    unitOfWork.Start();

                                    emitiu = true;
                                    totalDocumentosGerados++;
                                    Dominio.ObjetosDeValor.CTe.CTe cte = new Dominio.ObjetosDeValor.CTe.CTe();
                                    cte.Documentos = new List<Dominio.ObjetosDeValor.CTe.Documento>();

                                    bool contemNFe = pedidoCTeParaSubContratacaoNotasFiscais.Any(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal.Modelo == "55");

                                    List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = (from obj in pedidoCTeParaSubContratacaoNotasFiscais where obj.PedidoXMLNotaFiscal != null select obj.PedidoXMLNotaFiscal.XMLNotaFiscal).Distinct().ToList();

                                    foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal in notasFiscais)
                                    {
                                        Dominio.ObjetosDeValor.CTe.Documento docNF = serCte.BuscarDocumentoCTe(notaFiscal, empresa.TipoAmbiente, configuracaoEmbarcador);
                                        if (contemNFe && docNF.ModeloDocumentoFiscal == "55" && !cte.Documentos.Any(obj => obj.ChaveNFE == notaFiscal.Chave))
                                            cte.Documentos.Add(docNF);
                                        else if (!contemNFe)
                                            cte.Documentos.Add(docNF);
                                        //else
                                        //    cte.Documentos.Add(docNF);
                                    }

                                    cte.PercentualPagamentoAgregado = pedidoCTeParaSubContratacaoBase.PercentualPagamentoAgregado;

                                    List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> cargaPedidoComponentesFretesCliente = BuscarPedidosCTeSubcontratacaoComponentesFrete(pedidosCTesParaSubcontracaoDestinatario, modeloDocumentoFiscalCarga, unitOfWork);

                                    List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> cteTerceiros = (from obj in pedidosCTesParaSubcontracaoDestinatario select obj.CTeTerceiro).Distinct().ToList();

                                    cte.Volumes = cteTerceiros.Sum(o => o.Volumes);
                                    cte.MetrosCubicos = cteTerceiros.Sum(o => o.MetrosCubicos);
                                    cte.PesoCubado = cteTerceiros.Sum(o => o.PesoCubado);
                                    cte.Peso = cteTerceiros.Sum(o => o.Peso);
                                    cte.FatorCubagem = cteTerceiros.Sum(o => o.FatorCubagem);
                                    cte.PesoFaturado = cte.Peso > cte.PesoCubado ? cte.Peso : cte.PesoCubado;

                                    cte.ValorTotalMercadoria = (from obj in cteTerceiros select obj.ValorTotalMercadoria).Sum();
                                    cte.ValorAReceber = (from obj in pedidosCTesParaSubcontracaoDestinatario select obj.ValorFrete).Sum();
                                    cte.ValorFrete = (from obj in pedidosCTesParaSubcontracaoDestinatario select obj.ValorFrete).Sum();
                                    cte.ValorTotalPrestacaoServico = (from obj in pedidosCTesParaSubcontracaoDestinatario select obj.ValorFrete).Sum();
                                    decimal valorICMS = (from obj in pedidosCTesParaSubcontracaoDestinatario select obj.ValorICMS).Sum();
                                    decimal aliquota = pedidoCTeParaSubContratacaoBase.PercentualAliquota;

                                    if (cargaPedido.Pedido.EmpresaSerie != null)
                                        cte.Serie = cargaPedido.Pedido.EmpresaSerie.Numero;

                                    cte.ProdutoPredominante = cteParaSubcontratacao.ProdutoPredominante;

                                    List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade> quantidades = repCTeParaSubContratacaoQuantidade.BuscarPorCTesParaSubContratacao((from obj in cteTerceiros select obj.Codigo).ToList());

                                    Dominio.Entidades.Cliente tomador = cteParaSubcontratacao.Emitente?.Cliente;

                                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && modeloDocumentoEmitir != null && modeloDocumentoEmitir.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe && cargaPedido.Tomador != null)
                                        tomador = cargaPedido.Tomador;

                                    if (configuracaoEmbarcador.UtilizaEmissaoMultimodal && cargaPedido.Tomador != null)
                                        tomador = cargaPedido.Tomador;

                                    if (tomador == null)
                                        tomador = cargaPedido.Pedido.SubContratante != null ? cargaPedido.Pedido.SubContratante : cargaPedido.Tomador;

                                    Dominio.Enumeradores.TipoPagamento tipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros; //cargaPedido.Pedido.TipoPagamento; //Dominio.Enumeradores.TipoPagamento.Outros;//cargaPedido.Pedido.TipoPagamento;
                                    Dominio.Enumeradores.TipoTomador tipoTomador = Dominio.Enumeradores.TipoTomador.Outros; //cargaPedido.TipoTomador; //Dominio.Enumeradores.TipoTomador.Outros;

                                    List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesParaSubcontrataca = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>();
                                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacaoDestiantario in pedidosCTesParaSubcontracaoDestinatario)
                                    {
                                        if (!ctesParaSubcontrataca.Contains(pedidoCTeParaSubContratacaoDestiantario.CTeTerceiro))
                                            ctesParaSubcontrataca.Add(pedidoCTeParaSubContratacaoDestiantario.CTeTerceiro);
                                    }

                                    List<string> tiposContaineres = configuracaoEmbarcador.UtilizaEmissaoMultimodal ? repContainerTipo.BuscarDescricoes() : new List<string>();
                                    bool utilizarPrimeiraUnidadeMedidaPeso = UtilizarPrimeiraUnidadeMedidaPeso(tomador, carga.TipoOperacao);
                                    List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidadesCTes = ConverterQuantidadesTerceirosParaQuantidadesCTe(quantidades, configuracaoEmbarcador.AgruparUnidadesMedidasPorDescricao, utilizarPrimeiraUnidadeMedidaPeso, tiposContaineres);

                                    bool emitindoCTeFilialEmissora = false;
                                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeFilialEmissora = pedidoCTeParaSubContratacaoBase.CTeTerceiro.ObterCargaCTe(carga.Codigo);

                                    if (cargaPedido.CargaPedidoFilialEmissora && !emitirCteFilialEmissora)
                                        emitindoCTeFilialEmissora = true;

                                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> listaCargaCTesFilialEmissora = pedidosCTesParaSubcontracaoDestinatario.Select(o => o.CTeTerceiro.ObterCargaCTe(carga.Codigo)).Distinct().ToList();

                                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga tipoContratacaoCarga = cargaPedido.TipoContratacaoCarga;

                                    if (emitindoCTeFilialEmissora)
                                        tipoContratacaoCarga = cargaPedido.TipoContratacaoCargaSubContratacaoFilialEmissora;

                                    Dominio.Enumeradores.TipoServico tipoServico = Dominio.Enumeradores.TipoServico.SubContratacao;

                                    if (carga.CargaSVM)
                                        tipoServico = Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal;
                                    else if (carga.CargaSVMTerceiro)
                                        tipoServico = Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal;
                                    else if (tipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho)
                                        tipoServico = Dominio.Enumeradores.TipoServico.Redespacho;
                                    else if (tipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario)
                                        tipoServico = Dominio.Enumeradores.TipoServico.RedIntermediario;

                                    List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ctesAnteriores = ConverterCTesTerceirosParaAnteriores(ctesParaSubcontrataca, unitOfWork);

                                    Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = BuscarRegraICMSAgrupada(cargaOrigem, inicioPrestacao, terminoPrestacao, pedidosCTesParaSubcontracaoDestinatario, unitOfWork);
                                    Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = BuscarRegraISSAgrupada(cargaOrigem, pedidosCTesParaSubcontracaoDestinatario, inicioPrestacao, tomador, unitOfWork);
                                    Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = servicoPedidoCTeParaSubContratacao.ObterRetornoImpostoIBSCBS(pedidosCTesParaSubcontracaoDestinatario);

                                    List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> seguros = serCte.ConverterSegurosDeTerceirosEmSeguroCTe(apolicesSeguro, tipoTomador, cte.ValorTotalMercadoria);

                                    string observacaoCTe = null;

                                    if (!string.IsNullOrWhiteSpace(cargaPedido.Pedido.ObservacaoCTe)) //essa informação por hora só existe no CT-e para subcontratação, pois não emitimos modal aéreo
                                        observacaoCTe = cargaPedido.Pedido.ObservacaoCTe.Replace("#NumeroOCADocumentoTransporteAnterior", cteParaSubcontratacao.NumeroOperacionalConhecimentoAereo?.ToString() ?? string.Empty);

                                    Dominio.Enumeradores.TipoCTE tipoCTe = Dominio.Enumeradores.TipoCTE.Normal;

                                    if (carga.EmitirCTeComplementar || (cargaPedido.EmitirComplementarFilialEmissora && emitirCteFilialEmissora))
                                    {

                                        if (carga.EmitirCTeComplementar && cteParaSubcontratacao.Emitente.CPF_CNPJ_SemFormato != empresa.CNPJ_SemFormato)
                                            empresa = repEmpresa.BuscarPorCNPJ(cteParaSubcontratacao.Emitente.CPF_CNPJ_SemFormato);

                                        cte.ChaveCTESubstituicaoComplementar = cteParaSubcontratacao.ChaveAcesso;
                                        cte.TipoCTe = Dominio.Enumeradores.TipoCTE.Complemento;
                                        cte.TipoServico = cteParaSubcontratacao.TipoServico;
                                        tomador = cteParaSubcontratacao.Tomador.Cliente;
                                        tipoTomador = cteParaSubcontratacao.TipoTomador;
                                        tipoCTe = Dominio.Enumeradores.TipoCTE.Complemento;
                                        tipoServico = cteParaSubcontratacao.TipoServico;
                                    }

                                    if (contemNFe)
                                        notasFiscais = pedidoCTeParaSubContratacaoNotasFiscais.Where(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal.Modelo == "55").Select(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal).Distinct().ToList();

                                    if ((cte.ValorFrete > 0m || cargaPedidoComponentesFretesCliente.Sum(obj => obj.ValorComponente) > 0m || (!configuracaoEmbarcador.NaoGerarCTesComValoresZerados && modeloDocumentoEmitir?.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe)))
                                    {
                                        List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil> configuracoes = Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil.ConverterPedidoCTeParaSubContratacaoContaContabilContabilizacao(pedidoCTeParaSubContratacaoBase, pedidosCTeParaSubContratacaoContaContabilContabilizacao);
                                        Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTE = serCte.GerarCTe(carga, cargaPedido, cte, empresa, remetente, destinatario, tomador, expedidor, recebedor, inicioPrestacao, terminoPrestacao, null, enderecoDestinatario, tipoPagamento, tipoTomador, quantidadesCTes, cargaPedidoComponentesFretesCliente, observacaoCTe, cargaPedido.Pedido.ObservacaoCTeTerceiro, cargaPedido.Pedido, null, seguros, regraICMS, regraISS, impostoIBSCBS, tipoServicoMultisoftware, modeloDocumentoEmitir, tipoServico, tipoCTe, ctesAnteriores, 0, false, cargaCTeFilialEmissora, false, unitOfWork, configuracaoEmbarcador, configuracaoGeralCarga, pedidoCTeParaSubContratacaoBase.CentroResultado, pedidoCTeParaSubContratacaoBase.CentroResultadoDestinatario, pedidoCTeParaSubContratacaoBase.ItemServico, pedidoCTeParaSubContratacaoBase.CentroResultadoEscrituracao, pedidoCTeParaSubContratacaoBase.CentroResultadoICMS, pedidoCTeParaSubContratacaoBase.CentroResultadoPIS, pedidoCTeParaSubContratacaoBase.CentroResultadoCOFINS, pedidoCTeParaSubContratacaoBase.ValorMaximoCentroContabilizacao, configuracoes, notasFiscais, null, descricaoComponenteValorICMS, descricaoComponenteValorFrete, null, null, cargasPedidos);

                                        if (emitindoCTeFilialEmissora && cargaCTeFilialEmissora != null)
                                        {
                                            Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

                                            if (listaCargaCTesFilialEmissora != null && listaCargaCTesFilialEmissora.Count > 0)
                                            {
                                                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCteDaFilialEmissora in listaCargaCTesFilialEmissora)
                                                {
                                                    cargaCteDaFilialEmissora.CargaCTeSubContratacaoFilialEmissora = cargaCTE;
                                                    repCargaCte.Atualizar(cargaCteDaFilialEmissora);
                                                }
                                            }
                                            else
                                            {
                                                cargaCTeFilialEmissora.CargaCTeSubContratacaoFilialEmissora = cargaCTE;
                                                repCargaCte.Atualizar(cargaCTeFilialEmissora);
                                            }
                                        }

                                        serCte.AverbaCargaCTe(cargaCTE, apolicesSeguro, unitOfWork, configuracaoEmbarcador, cargaPedido.Pedido.FormaAverbacaoCTE);

                                        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoNotaFiscal in pedidoCTeParaSubContratacaoNotasFiscais)
                                        {
                                            Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTE = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe();
                                            cargaPedidoXMLNotaFiscalCTE.CargaCTe = cargaCTE;
                                            cargaPedidoXMLNotaFiscalCTE.PedidoXMLNotaFiscal = pedidoCTeParaSubContratacaoNotaFiscal.PedidoXMLNotaFiscal;
                                            repCargaPedidoXMLNotaFiscalCTe.Inserir(cargaPedidoXMLNotaFiscalCTE);
                                        }

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
                                        }

                                        if (pedidoCTeParaSubContratacaoBase.PossuiNFSManual)
                                        {
                                            serNFS.CriarCargaCTePendenteEmissaoManualDeNFS(carga, cargaCTE, inicioPrestacao, unitOfWork);

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
                                        unitOfWork.CommitChanges();
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
                                        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao in pedidosCTesParaSubcontracaoDestinatario)
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
                                }

                                if (totalDocumentosGerados == 1 || (totalDocumentosGerados % quantidadeDocumentosAtualizarCarga) == 0)
                                    svcHubCarga.InformarQuantidadeDocumentosEmitidos(carga.Codigo, totalDocumentos, totalDocumentosGerados);

                            }
                        }
                    }

                }
                else
                {
                    Servicos.Embarcador.CTe.CTEsImportados serCTEsImportados = new Servicos.Embarcador.CTe.CTEsImportados(unitOfWork);
                    serCTEsImportados.VincularCargaCTeEmitidoAnteriormente(carga, primeiroCargaPedido, unitOfWork);
                }

                if (emitiu)
                    svcHubCarga.InformarQuantidadeDocumentosEmitidos(carga.Codigo, totalDocumentos, totalDocumentosGerados);

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    unitOfWork.FlushAndClear();
                }
            }
        }

        public void GerarCTePorSubcontratacaoAgrupadoPorPedido(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento configuracaoCargaEmissaoDocumento, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, int totalDocumentos, int quantidadeDocumentosAtualizarCarga, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao> pedidosCTeParaSubContratacaoContaContabilContabilizacao, ref int totalDocumentosGerados)
        {
            CTe serCte = new CTe(unitOfWork);
            CTeSubContratacaoEmissao serCTeSubContratacaoEmissao = new CTeSubContratacaoEmissao(unitOfWork);
            Servicos.Embarcador.Carga.NFS serNFS = new NFS(unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroQuantidade repCTeParaSubContratacaoQuantidade = new Repositorio.Embarcador.CTe.CTeTerceiroQuantidade(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repPedidoCTeParaSubContratacaoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.CTe.CTeDocumentoOriginario repDocumentoOriginario = new Repositorio.Embarcador.CTe.CTeDocumentoOriginario(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.ContainerTipo repContainerTipo = new Repositorio.Embarcador.Pedidos.ContainerTipo(unitOfWork);

            CTePorPedido serCTePorPedido = new CTePorPedido(unitOfWork);
            Hubs.Carga svcHubCarga = new Hubs.Carga();
            Servicos.Embarcador.Pedido.PedidoCTeParaSubContratacao servicoPedidoCTeParaSubContratacao = new Pedido.PedidoCTeParaSubContratacao(unitOfWork);

            if (cargaPedidos != null && cargaPedidos.Count > 0)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido primeiroCargaPedido = repCargaPedido.BuscarPorCodigo(cargaPedidos[0].Codigo);

                bool emitiu = false;

                if (!primeiroCargaPedido.CTeEmitidoNoEmbarcador && !primeiroCargaPedido.Pedido.PedidoTransbordo)
                {
                    bool utilizarExpedidorRecebedorPedido = carga.TipoOperacao?.ConfiguracaoEmissaoDocumento?.UtilizarExpedidorRecebedorPedidoSubcontratacao ?? false;

                    bool emitirCteFilialEmissora = false;
                    if (primeiroCargaPedido.Carga.EmpresaFilialEmissora != null && !primeiroCargaPedido.Carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                        emitirCteFilialEmissora = true;

                    serCte.ObterDescricoesComponentesPadrao(primeiroCargaPedido.ObterTomador(), carga, configuracaoEmbarcador, unitOfWork, out string descricaoComponenteValorFrete, out string descricaoComponenteValorICMS);

                    foreach (var cargaPedidoConsulta in cargaPedidos)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(cargaPedidoConsulta.Codigo);
                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidosCTesParaSubcontracao = repPedidoCTeParaSubContratacao.BuscarPorCargaPedidoComFetch(cargaPedido.Codigo);
                        Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem = (from obj in cargasOrigem where obj.Codigo == cargaPedido.CargaOrigem.Codigo select obj).FirstOrDefault();

                        List<Dominio.Entidades.Cliente> remetentes = (from obj in pedidosCTesParaSubcontracao select obj.CTeTerceiro.Remetente.Cliente).Distinct().ToList();
                        List<Dominio.Entidades.Cliente> destinatarios = (from obj in pedidosCTesParaSubcontracao select obj.CTeTerceiro.Destinatario.Cliente).Distinct().ToList();
                        Dominio.Entidades.Cliente remetente = cargaPedidoConsulta.Pedido.Remetente != null ? cargaPedidoConsulta.Pedido.Remetente : remetentes.FirstOrDefault();
                        Dominio.Entidades.Cliente destinatario = cargaPedidoConsulta.Pedido.Destinatario != null ? cargaPedidoConsulta.Pedido.Destinatario : destinatarios.FirstOrDefault();

                        if (!cargaPedido.PossuiCTe && !cargaPedido.PossuiNFS && !cargaPedido.PossuiNFSManual && !(cargaPedido.PossuiNFSManual && cargaPedido.ModeloDocumentoFiscalIntramunicipal != null))
                            continue;

                        Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacaoBase = pedidosCTesParaSubcontracao.FirstOrDefault();

                        Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalCarga = pedidoCTeParaSubContratacaoBase.ModeloDocumentoFiscal;

                        if (modeloDocumentoFiscalCarga == null)
                            modeloDocumentoFiscalCarga = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.CTe);

                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> pedidoCTeParaSubContratacaoNotasFiscais = repPedidoCTeParaSubContratacaoNotaFiscal.BuscarPorPedidosCTeParaSubcontratacao((from obj in pedidosCTesParaSubcontracao select obj.Codigo).ToList());

                        Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteParaSubcontratacao = pedidoCTeParaSubContratacaoBase.CTeTerceiro;

                        Dominio.Entidades.Cliente expedidor = null;
                        Dominio.Entidades.Cliente recebedor = null;
                        Dominio.Entidades.Localidade inicioPrestacao = cteParaSubcontratacao.LocalidadeInicioPrestacao;
                        Dominio.Entidades.Localidade terminoPrestacao = cteParaSubcontratacao.LocalidadeTerminoPrestacao;

                        if (utilizarExpedidorRecebedorPedido || (configuracaoEmbarcador.UtilizaEmissaoMultimodal && cargaPedido.Expedidor != null))
                        {
                            expedidor = cargaPedido.Expedidor;

                            if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada &&
                                cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada &&
                                expedidor != null)
                                inicioPrestacao = expedidor.Localidade;
                            else if (utilizarExpedidorRecebedorPedido && expedidor == null && remetente != null)
                                inicioPrestacao = remetente.Localidade;
                        }
                        else if (cteParaSubcontratacao.Expedidor != null && (cteParaSubcontratacao.Expedidor.CPF_CNPJ != cteParaSubcontratacao.Remetente.CPF_CNPJ || cargaPedido.Expedidor == null) && !cargaPedido.PedidoEncaixado)
                            expedidor = cteParaSubcontratacao.Expedidor.Cliente != null ? repCliente.BuscarPorCPFCNPJ(cteParaSubcontratacao.Expedidor.Cliente.CPF_CNPJ) : repCliente.BuscarPorCPFCNPJ(double.Parse(cteParaSubcontratacao.Expedidor.CPF_CNPJ_SemFormato));
                        else if (cargaPedido.Expedidor != null)
                        {
                            expedidor = cargaPedido.Expedidor;

                            if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
                                inicioPrestacao = expedidor.Localidade;
                        }

                        if (utilizarExpedidorRecebedorPedido || ((configuracaoEmbarcador.UtilizaEmissaoMultimodal || (carga.TipoOperacao?.UtilizarRecebedorPedidoParaSVM ?? false)) && cargaPedido.Recebedor != null))
                        {
                            recebedor = cargaPedido.Recebedor;

                            if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada &&
                                cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada &&
                                recebedor != null)
                                terminoPrestacao = recebedor?.Localidade;
                            else if (utilizarExpedidorRecebedorPedido && recebedor == null && destinatario != null)
                                terminoPrestacao = destinatario.Localidade;
                        }
                        else if (cteParaSubcontratacao.Recebedor != null && (cteParaSubcontratacao.Recebedor.CPF_CNPJ != cteParaSubcontratacao.Destinatario.CPF_CNPJ || cargaPedido.Recebedor == null) && !cargaPedido.PedidoEncaixado)
                        {
                            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor))
                                recebedor = cteParaSubcontratacao.Recebedor.Cliente != null ? repCliente.BuscarPorCPFCNPJ(cteParaSubcontratacao.Recebedor.Cliente.CPF_CNPJ) : repCliente.BuscarPorCPFCNPJ(double.Parse(cteParaSubcontratacao.Recebedor.CPF_CNPJ_SemFormato));
                            else //todo: if feito por um caso danone onde os operadores emitirar o Ct-e da transportara danone errado, nesse caso o cte de subcontratacao deve ser somente até o destinatario rever isso (09/10/2019 Rodrigo)
                                terminoPrestacao = destinatario.Localidade;
                        }
                        else if (cargaPedido.Recebedor != null)
                        {
                            recebedor = cargaPedido.Recebedor;

                            if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
                                terminoPrestacao = recebedor.Localidade;
                        }

                        Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEndercoRecebedor = null;
                        if ((cargaPedido.Carga.TipoOperacao?.ConfiguracaoEmissaoDocumento?.UtilizarOutroEnderecoPedidoMesmoSePossuirRecebedor ?? false))
                        {
                            if ((cargaPedido.Pedido?.Recebedor?.CPF_CNPJ ?? 0) > 0)
                            {
                                Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repClienteOutroEndereco = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(unitOfWork);
                                Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(unitOfWork);

                                List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco> enderecos = repClienteOutroEndereco.BuscarPorPessoa(cargaPedido.Pedido.Recebedor.CPF_CNPJ);
                                Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco endereco = enderecos.Where(obj => obj.Localidade.Codigo == cargaPedido.Pedido.Destino?.Codigo)?.FirstOrDefault();

                                if (endereco != null)
                                {
                                    terminoPrestacao = endereco.Localidade;

                                    pedidoEndercoRecebedor = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco
                                    {
                                        Endereco = endereco.Endereco ?? string.Empty,
                                        Numero = endereco.Numero ?? string.Empty,
                                        Complemento = endereco.Complemento ?? string.Empty,
                                        CEP = endereco.CEP ?? string.Empty,
                                        Bairro = endereco.Bairro ?? string.Empty,
                                        Localidade = endereco.Localidade,
                                        ClienteOutroEndereco = endereco,
                                        IE_RG = endereco.IE_RG ?? string.Empty
                                    };

                                    repPedidoEndereco.Inserir(pedidoEndercoRecebedor);
                                }
                            }
                        }

                        if (!pedidoCTeParaSubContratacaoBase.PossuiNFSManual ||
                            (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS &&
                            (inicioPrestacao.Codigo != terminoPrestacao.Codigo || (pedidoCTeParaSubContratacaoBase.PossuiNFSManual && cargaPedido.ModeloDocumentoFiscalIntramunicipal != null && (inicioPrestacao.Codigo == terminoPrestacao.Codigo || cargaPedido.DisponibilizarDocumentoNFSManual)))))
                        {
                            bool somenteCTeSubContratacaoFilialEmissora = false;

                            if (cargaPedido.CargaPedidoFilialEmissora && carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                                somenteCTeSubContratacaoFilialEmissora = true;

                            if (repCargaPedidoXMLNotaFiscalCTe.ContarCTesPorPedidoCTeParaSubContratacao(pedidosCTesParaSubcontracao.Select(p => p.Codigo).ToList(), somenteCTeSubContratacaoFilialEmissora) > 0)
                                continue;

                            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoEmitir = null;

                            if (pedidoCTeParaSubContratacaoBase.PossuiNFSManual)
                                modeloDocumentoEmitir = cargaPedido.ModeloDocumentoFiscalIntramunicipal;
                            else
                                modeloDocumentoEmitir = modeloDocumentoFiscalCarga;

                            List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> apolicesSeguro = repApoliceSeguroAverbacao.BuscarPorCargaPedido(cargaPedido.Codigo, emitirCteFilialEmissora);

                            unitOfWork.Start();

                            emitiu = true;
                            totalDocumentosGerados++;
                            Dominio.ObjetosDeValor.CTe.CTe cte = new Dominio.ObjetosDeValor.CTe.CTe();
                            cte.Documentos = new List<Dominio.ObjetosDeValor.CTe.Documento>();

                            bool contemNFe = pedidoCTeParaSubContratacaoNotasFiscais.Any(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal.Modelo == "55");
                            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoNotaFiscal in pedidoCTeParaSubContratacaoNotasFiscais)
                            {
                                Dominio.ObjetosDeValor.CTe.Documento docNF = serCte.BuscarDocumentoCTe(pedidoCTeParaSubContratacaoNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal, carga.Empresa.TipoAmbiente, configuracaoEmbarcador);
                                if (contemNFe && docNF.ModeloDocumentoFiscal == "55" && !cte.Documentos.Any(obj => obj.ChaveNFE == pedidoCTeParaSubContratacaoNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Chave))
                                    cte.Documentos.Add(docNF);
                                else if (!contemNFe)
                                    cte.Documentos.Add(docNF);
                                //else
                                //cte.Documentos.Add(docNF);
                            }

                            cte.PercentualPagamentoAgregado = pedidoCTeParaSubContratacaoBase.PercentualPagamentoAgregado;

                            List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> cargaPedidoComponentesFretesCliente = BuscarPedidosCTeSubcontratacaoComponentesFrete(pedidosCTesParaSubcontracao, modeloDocumentoFiscalCarga, unitOfWork);

                            cte.Peso = pedidosCTesParaSubcontracao.Sum(o => o.CTeTerceiro.Peso);
                            cte.Volumes = pedidosCTesParaSubcontracao.Sum(o => o.CTeTerceiro.Volumes);
                            cte.MetrosCubicos = pedidosCTesParaSubcontracao.Sum(o => o.CTeTerceiro.MetrosCubicos);
                            cte.PesoCubado = pedidosCTesParaSubcontracao.Sum(o => o.CTeTerceiro.PesoCubado);
                            cte.FatorCubagem = pedidosCTesParaSubcontracao.Sum(o => o.CTeTerceiro.FatorCubagem);
                            cte.PesoFaturado = cte.Peso > cte.PesoCubado ? cte.Peso : cte.PesoCubado;

                            cte.ValorTotalMercadoria = (from obj in pedidosCTesParaSubcontracao select obj.CTeTerceiro.ValorTotalMercadoria).Sum();
                            cte.ValorAReceber = (from obj in pedidosCTesParaSubcontracao select obj.ValorFrete).Sum();
                            cte.ValorFrete = (from obj in pedidosCTesParaSubcontracao select obj.ValorFrete).Sum();
                            cte.ValorTotalPrestacaoServico = (from obj in pedidosCTesParaSubcontracao select obj.ValorFrete).Sum();
                            decimal valorICMS = (from obj in pedidosCTesParaSubcontracao select obj.ValorICMS).Sum();
                            decimal aliquota = pedidoCTeParaSubContratacaoBase.PercentualAliquota;

                            if (cargaPedido.Pedido.EmpresaSerie != null)
                                cte.Serie = cargaPedido.Pedido.EmpresaSerie.Numero;

                            cte.ProdutoPredominante = cteParaSubcontratacao.ProdutoPredominante;

                            List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade> quantidades = repCTeParaSubContratacaoQuantidade.BuscarPorCTesParaSubContratacao((from obj in pedidosCTesParaSubcontracao select obj.CTeTerceiro.Codigo).ToList());

                            Dominio.Entidades.Cliente tomador = cteParaSubcontratacao.Emitente?.Cliente;

                            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                                tomador = Servicos.Embarcador.Carga.CTeSubContratacao.ObterTomadorCTeParaSubcontratacao(carga, cargaPedido, pedidoCTeParaSubContratacaoBase, configuracaoEmbarcador, tipoServicoMultisoftware, modeloDocumentoEmitir);

                            if (configuracaoEmbarcador.UtilizaEmissaoMultimodal && cargaPedido.Tomador != null)
                                tomador = cargaPedido.Tomador;

                            if (tomador == null)
                                tomador = cargaPedido.Pedido.SubContratante != null ? cargaPedido.Pedido.SubContratante : cargaPedido.Tomador;

                            Dominio.Enumeradores.TipoPagamento tipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros; //cargaPedido.Pedido.TipoPagamento; //Dominio.Enumeradores.TipoPagamento.Outros;//cargaPedido.Pedido.TipoPagamento;
                            Dominio.Enumeradores.TipoTomador tipoTomador = Dominio.Enumeradores.TipoTomador.Outros; //cargaPedido.TipoTomador; //Dominio.Enumeradores.TipoTomador.Outros;

                            List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesParaSubcontrataca = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>();
                            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacaoDestiantario in pedidosCTesParaSubcontracao)
                            {
                                if (!ctesParaSubcontrataca.Contains(pedidoCTeParaSubContratacaoDestiantario.CTeTerceiro))
                                    ctesParaSubcontrataca.Add(pedidoCTeParaSubContratacaoDestiantario.CTeTerceiro);
                            }


                            List<string> tiposContaineres = configuracaoEmbarcador.UtilizaEmissaoMultimodal ? repContainerTipo.BuscarDescricoes() : new List<string>();
                            bool utilizarPrimeiraUnidadeMedidaPeso = UtilizarPrimeiraUnidadeMedidaPeso(tomador, carga.TipoOperacao);
                            List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidadesCTes = ConverterQuantidadesTerceirosParaQuantidadesCTe(quantidades, configuracaoEmbarcador.AgruparUnidadesMedidasPorDescricao, utilizarPrimeiraUnidadeMedidaPeso, tiposContaineres);

                            bool emitindoCTeFilialEmissora = false;
                            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeFilialEmissora = pedidoCTeParaSubContratacaoBase.CTeTerceiro.ObterCargaCTe(carga.Codigo);

                            if (cargaPedido.CargaPedidoFilialEmissora)
                                emitindoCTeFilialEmissora = true;

                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga tipoContratacaoCarga = cargaPedido.TipoContratacaoCarga;

                            if (emitindoCTeFilialEmissora)
                                tipoContratacaoCarga = cargaPedido.TipoContratacaoCargaSubContratacaoFilialEmissora;

                            Dominio.Enumeradores.TipoServico tipoServico = Dominio.Enumeradores.TipoServico.SubContratacao;

                            if (carga.CargaSVM)
                                tipoServico = Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal;
                            else if (carga.CargaSVMTerceiro)
                                tipoServico = Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal;
                            else if (tipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho)
                                tipoServico = Dominio.Enumeradores.TipoServico.Redespacho;
                            else if (tipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario)
                                tipoServico = Dominio.Enumeradores.TipoServico.RedIntermediario;

                            List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ctesAnteriores = ConverterCTesTerceirosParaAnteriores(ctesParaSubcontrataca, unitOfWork);

                            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = BuscarRegraICMSAgrupada(cargaOrigem, inicioPrestacao, terminoPrestacao, pedidosCTesParaSubcontracao, unitOfWork);
                            Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = BuscarRegraISSAgrupada(cargaOrigem, pedidosCTesParaSubcontracao, inicioPrestacao, tomador, unitOfWork);
                            Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = servicoPedidoCTeParaSubContratacao.ObterRetornoImpostoIBSCBS(pedidosCTesParaSubcontracao);

                            List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> seguros = serCte.ConverterSegurosDeTerceirosEmSeguroCTe(apolicesSeguro, tipoTomador, cte.ValorTotalMercadoria);

                            string observacaoCTe = null;

                            if (!string.IsNullOrWhiteSpace(cargaPedido.Pedido.ObservacaoCTe)) //essa informação por hora só existe no CT-e para subcontratação, pois não emitimos modal aéreo
                                observacaoCTe = cargaPedido.Pedido.ObservacaoCTe.Replace("#NumeroOCADocumentoTransporteAnterior", cteParaSubcontratacao.NumeroOperacionalConhecimentoAereo?.ToString() ?? string.Empty);

                            Dominio.Enumeradores.TipoCTE tipoCTe = Dominio.Enumeradores.TipoCTE.Normal;
                            Dominio.Entidades.Empresa empresa = cargaOrigem.Empresa;
                            if (carga.EmitirCTeComplementar)
                            {
                                if (carga.EmitirCTeComplementar && cteParaSubcontratacao.Emitente.CPF_CNPJ_SemFormato != empresa.CNPJ_SemFormato)
                                    empresa = repEmpresa.BuscarPorCNPJ(cteParaSubcontratacao.Emitente.CPF_CNPJ_SemFormato);

                                cte.ChaveCTESubstituicaoComplementar = cteParaSubcontratacao.ChaveAcesso;
                                cte.TipoCTe = Dominio.Enumeradores.TipoCTE.Complemento;
                                cte.TipoServico = cteParaSubcontratacao.TipoServico;
                                tomador = cteParaSubcontratacao.Tomador.Cliente;
                                tipoTomador = cteParaSubcontratacao.TipoTomador;
                                tipoCTe = Dominio.Enumeradores.TipoCTE.Complemento;
                                tipoServico = cteParaSubcontratacao.TipoServico;
                            }
                            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlNotas = pedidoCTeParaSubContratacaoNotasFiscais.Select(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal).ToList();
                            if (contemNFe)
                                xmlNotas = pedidoCTeParaSubContratacaoNotasFiscais.Where(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal.Modelo == "55").Select(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal).ToList();

                            List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil> configuracoes = Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil.ConverterPedidoCTeParaSubContratacaoContaContabilContabilizacao(pedidoCTeParaSubContratacaoBase, pedidosCTeParaSubContratacaoContaContabilContabilizacao);
                            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTE = serCte.GerarCTe(carga, cargaPedido, cte, empresa, remetente, destinatario, tomador, expedidor, recebedor, inicioPrestacao, terminoPrestacao, null, null, tipoPagamento, tipoTomador, quantidadesCTes, cargaPedidoComponentesFretesCliente, observacaoCTe, cargaPedido.Pedido.ObservacaoCTeTerceiro, cargaPedido.Pedido, null, seguros, regraICMS, regraISS, impostoIBSCBS, tipoServicoMultisoftware, modeloDocumentoEmitir, tipoServico, tipoCTe, ctesAnteriores, 0, false, cargaCTeFilialEmissora, false, unitOfWork, configuracaoEmbarcador, configuracaoGeralCarga, pedidoCTeParaSubContratacaoBase.CentroResultado, pedidoCTeParaSubContratacaoBase.CentroResultadoDestinatario, pedidoCTeParaSubContratacaoBase.ItemServico, pedidoCTeParaSubContratacaoBase.CentroResultadoEscrituracao, pedidoCTeParaSubContratacaoBase.CentroResultadoICMS, pedidoCTeParaSubContratacaoBase.CentroResultadoPIS, pedidoCTeParaSubContratacaoBase.CentroResultadoCOFINS, pedidoCTeParaSubContratacaoBase.ValorMaximoCentroContabilizacao, configuracoes, xmlNotas, (cargaPedido.Carga.TipoOperacao?.ConfiguracaoEmissaoDocumento?.UtilizarOutroEnderecoPedidoMesmoSePossuirRecebedor ?? false) ? pedidoEndercoRecebedor : null, descricaoComponenteValorICMS, descricaoComponenteValorFrete);

                            if (emitindoCTeFilialEmissora && cargaCTeFilialEmissora != null)
                            {
                                Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                                cargaCTeFilialEmissora.CargaCTeSubContratacaoFilialEmissora = cargaCTE;
                                repCargaCte.Atualizar(cargaCTeFilialEmissora);
                            }

                            serCte.AverbaCargaCTe(cargaCTE, apolicesSeguro, unitOfWork, configuracaoEmbarcador, cargaPedido.Pedido.FormaAverbacaoCTE);

                            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoNotaFiscal in pedidoCTeParaSubContratacaoNotasFiscais)
                            {
                                Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTE = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe();
                                cargaPedidoXMLNotaFiscalCTE.CargaCTe = cargaCTE;
                                cargaPedidoXMLNotaFiscalCTE.PedidoXMLNotaFiscal = pedidoCTeParaSubContratacaoNotaFiscal.PedidoXMLNotaFiscal;
                                repCargaPedidoXMLNotaFiscalCTe.Inserir(cargaPedidoXMLNotaFiscalCTE);
                            }

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
                            }

                            if (pedidoCTeParaSubContratacaoBase.PossuiNFSManual)
                            {
                                serNFS.CriarCargaCTePendenteEmissaoManualDeNFS(carga, cargaCTE, inicioPrestacao, unitOfWork);

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
                                if (configuracaoCargaEmissaoDocumento.GerarRegistroPorPedidoNaNFSManualPorCTeAnterior)
                                {
                                    Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaNFeParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
                                    var listaCargaDocumentoParaEmissaoNFSManual = repCargaNFeParaEmissaoNFSManual.BuscarDocPorCargaPedido(cargaPedido.Codigo);

                                    if (listaCargaDocumentoParaEmissaoNFSManual.Count() > 0)
                                        continue;

                                    unitOfWork.Start();

                                    Dominio.Entidades.Cliente tomador = cteParaSubcontratacao.Emitente?.Cliente;

                                    if (tomador == null)
                                        tomador = cargaPedido.Pedido.SubContratante != null ? cargaPedido.Pedido.SubContratante : cargaPedido.Tomador;

                                    carga.AgNFSManual = true;
                                    serNFS.CriarNFPendenteEmissaoManualPorCargaPedido(carga, cargaPedido, tomador, inicioPrestacao, unitOfWork);
                                    unitOfWork.CommitChanges();
                                }
                                else
                                {

                                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao in pedidosCTesParaSubcontracao)
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
                    serCTEsImportados.VincularCargaCTeEmitidoAnteriormente(carga, primeiroCargaPedido, unitOfWork);
                }

                if (emitiu)
                    svcHubCarga.InformarQuantidadeDocumentosEmitidos(carga.Codigo, totalDocumentos, totalDocumentosGerados);
            }
        }

        public void GerarCTePorSubcontratacaoAgrupadoPorPedidoEmbarcador(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento configuracaoCargaEmissaoDocumento, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, int totalDocumentos, int quantidadeDocumentosAtualizarCarga, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao> pedidosCTeParaSubContratacaoContaContabilContabilizacao, ref int totalDocumentosGerados)
        {
            CTe serCte = new CTe(unitOfWork);
            Servicos.Embarcador.Carga.NFS serNFS = new NFS(unitOfWork);

            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroQuantidade repCTeParaSubContratacaoQuantidade = new Repositorio.Embarcador.CTe.CTeTerceiroQuantidade(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repPedidoCTeParaSubContratacaoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.CTe.CTeDocumentoOriginario repDocumentoOriginario = new Repositorio.Embarcador.CTe.CTeDocumentoOriginario(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Pedidos.ContainerTipo repContainerTipo = new Repositorio.Embarcador.Pedidos.ContainerTipo(unitOfWork);

            Hubs.Carga svcHubCarga = new Hubs.Carga();
            Servicos.Embarcador.Carga.CargaPedido servicoCargaPedido = new CargaPedido(unitOfWork);

            if (cargaPedidos != null && cargaPedidos.Count > 0)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido primeiroCargaPedido = repCargaPedido.BuscarPorCodigo(cargaPedidos[0].Codigo);

                bool emitiu = false;

                if (!primeiroCargaPedido.CTeEmitidoNoEmbarcador && !primeiroCargaPedido.Pedido.PedidoTransbordo)
                {
                    bool utilizarExpedidorRecebedorPedido = carga.TipoOperacao?.ConfiguracaoEmissaoDocumento?.UtilizarExpedidorRecebedorPedidoSubcontratacao ?? false;

                    bool emitirCteFilialEmissora = false;
                    if (primeiroCargaPedido.Carga.EmpresaFilialEmissora != null && !primeiroCargaPedido.Carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                        emitirCteFilialEmissora = true;

                    serCte.ObterDescricoesComponentesPadrao(primeiroCargaPedido.ObterTomador(), carga, configuracaoEmbarcador, unitOfWork, out string descricaoComponenteValorFrete, out string descricaoComponenteValorICMS);

                    foreach (var cargaPedidoConsulta in cargaPedidos)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(cargaPedidoConsulta.Codigo);
                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidosCTesParaSubcontracao = repPedidoCTeParaSubContratacao.BuscarPorCargaPedidoComFetch(cargaPedido.Codigo);
                        Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem = (from obj in cargasOrigem where obj.Codigo == cargaPedido.CargaOrigem.Codigo select obj).FirstOrDefault();

                        if (pedidosCTesParaSubcontracao?.Count == 0)
                            continue;

                        List<Dominio.Entidades.Cliente> remetentes = (from obj in pedidosCTesParaSubcontracao select obj.CTeTerceiro.Remetente.Cliente).Distinct().ToList();
                        List<Dominio.Entidades.Cliente> destinatarios = (from obj in pedidosCTesParaSubcontracao select obj.CTeTerceiro.Destinatario.Cliente).Distinct().ToList();
                        Dominio.Entidades.Cliente remetente = cargaPedidoConsulta.Pedido.Remetente != null ? cargaPedidoConsulta.Pedido.Remetente : remetentes.FirstOrDefault();
                        Dominio.Entidades.Cliente destinatario = cargaPedidoConsulta.Pedido.Destinatario != null ? cargaPedidoConsulta.Pedido.Destinatario : destinatarios.FirstOrDefault();

                        if (!cargaPedido.PossuiCTe && !cargaPedido.PossuiNFS && !cargaPedido.PossuiNFSManual && !(cargaPedido.PossuiNFSManual && cargaPedido.ModeloDocumentoFiscalIntramunicipal != null))
                            continue;

                        Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacaoBase = pedidosCTesParaSubcontracao.FirstOrDefault();

                        Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalCarga = pedidoCTeParaSubContratacaoBase.ModeloDocumentoFiscal;

                        if (modeloDocumentoFiscalCarga == null)
                            modeloDocumentoFiscalCarga = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.CTe);

                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> pedidoCTeParaSubContratacaoNotasFiscais = repPedidoCTeParaSubContratacaoNotaFiscal.BuscarPorPedidosCTeParaSubcontratacao((from obj in pedidosCTesParaSubcontracao select obj.Codigo).ToList());

                        Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteParaSubcontratacao = pedidoCTeParaSubContratacaoBase.CTeTerceiro;

                        Dominio.Entidades.Cliente expedidor = null;
                        Dominio.Entidades.Cliente recebedor = null;
                        Dominio.Entidades.Localidade inicioPrestacao = cteParaSubcontratacao.LocalidadeInicioPrestacao;
                        Dominio.Entidades.Localidade terminoPrestacao = cteParaSubcontratacao.LocalidadeTerminoPrestacao;

                        if (utilizarExpedidorRecebedorPedido || (configuracaoEmbarcador.UtilizaEmissaoMultimodal && cargaPedido.Expedidor != null))
                        {
                            expedidor = cargaPedido.Expedidor;

                            if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada &&
                                cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada &&
                                expedidor != null)
                                inicioPrestacao = expedidor.Localidade;
                            else if (utilizarExpedidorRecebedorPedido && expedidor == null && remetente != null)
                                inicioPrestacao = remetente.Localidade;
                        }
                        else if (cteParaSubcontratacao.Expedidor != null && (cteParaSubcontratacao.Expedidor.CPF_CNPJ != cteParaSubcontratacao.Remetente.CPF_CNPJ || cargaPedido.Expedidor == null) && !cargaPedido.PedidoEncaixado)
                            expedidor = cteParaSubcontratacao.Expedidor.Cliente != null ? repCliente.BuscarPorCPFCNPJ(cteParaSubcontratacao.Expedidor.Cliente.CPF_CNPJ) : repCliente.BuscarPorCPFCNPJ(double.Parse(cteParaSubcontratacao.Expedidor.CPF_CNPJ_SemFormato));
                        else if (cargaPedido.Expedidor != null)
                        {
                            expedidor = cargaPedido.Expedidor;

                            if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
                                inicioPrestacao = expedidor.Localidade;
                        }

                        if (utilizarExpedidorRecebedorPedido || ((configuracaoEmbarcador.UtilizaEmissaoMultimodal || (carga.TipoOperacao?.UtilizarRecebedorPedidoParaSVM ?? false)) && cargaPedido.Recebedor != null))
                        {
                            recebedor = cargaPedido.Recebedor;

                            if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada &&
                                cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada &&
                                recebedor != null)
                                terminoPrestacao = recebedor?.Localidade;
                            else if (utilizarExpedidorRecebedorPedido && recebedor == null && destinatario != null)
                                terminoPrestacao = destinatario.Localidade;
                        }
                        else if (cteParaSubcontratacao.Recebedor != null && (cteParaSubcontratacao.Recebedor.CPF_CNPJ != cteParaSubcontratacao.Destinatario.CPF_CNPJ || cargaPedido.Recebedor == null) && !cargaPedido.PedidoEncaixado)
                        {
                            if (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor)
                                recebedor = cteParaSubcontratacao.Recebedor.Cliente != null ? repCliente.BuscarPorCPFCNPJ(cteParaSubcontratacao.Recebedor.Cliente.CPF_CNPJ) : repCliente.BuscarPorCPFCNPJ(double.Parse(cteParaSubcontratacao.Recebedor.CPF_CNPJ_SemFormato));
                            else
                                terminoPrestacao = destinatario.Localidade;
                        }
                        else if (cargaPedido.Recebedor != null)
                        {
                            recebedor = cargaPedido.Recebedor;

                            if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
                                terminoPrestacao = recebedor.Localidade;
                        }

                        Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEndercoRecebedor = null;
                        if ((cargaPedido.Carga.TipoOperacao?.ConfiguracaoEmissaoDocumento?.UtilizarOutroEnderecoPedidoMesmoSePossuirRecebedor ?? false) && (cargaPedido.Pedido?.Recebedor?.CPF_CNPJ ?? 0) > 0)
                        {
                            Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repClienteOutroEndereco = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(unitOfWork);
                            Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(unitOfWork);

                            List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco> enderecos = repClienteOutroEndereco.BuscarPorPessoa(cargaPedido.Pedido.Recebedor.CPF_CNPJ);
                            Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco endereco = enderecos.Where(obj => obj.Localidade.Codigo == cargaPedido.Pedido.Destino?.Codigo)?.FirstOrDefault();

                            if (endereco != null)
                            {
                                terminoPrestacao = endereco.Localidade;

                                pedidoEndercoRecebedor = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco
                                {
                                    Endereco = endereco.Endereco ?? string.Empty,
                                    Numero = endereco.Numero ?? string.Empty,
                                    Complemento = endereco.Complemento ?? string.Empty,
                                    CEP = endereco.CEP ?? string.Empty,
                                    Bairro = endereco.Bairro ?? string.Empty,
                                    Localidade = endereco.Localidade,
                                    ClienteOutroEndereco = endereco,
                                    IE_RG = endereco.IE_RG ?? string.Empty
                                };

                                repPedidoEndereco.Inserir(pedidoEndercoRecebedor);
                            }
                        }

                        if (!pedidoCTeParaSubContratacaoBase.PossuiNFSManual)
                        {
                            bool somenteCTeSubContratacaoFilialEmissora = false;

                            if (cargaPedido.CargaPedidoFilialEmissora && carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                                somenteCTeSubContratacaoFilialEmissora = true;

                            if (repCargaPedidoXMLNotaFiscalCTe.ContarCTesPorPedidoCTeParaSubContratacao(pedidosCTesParaSubcontracao.Select(p => p.Codigo).ToList(), somenteCTeSubContratacaoFilialEmissora) > 0)
                                continue;

                            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoEmitir = modeloDocumentoFiscalCarga;

                            List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> apolicesSeguro = repApoliceSeguroAverbacao.BuscarPorCargaPedido(cargaPedido.Codigo, emitirCteFilialEmissora);

                            if (!unitOfWork.IsActiveTransaction())
                                unitOfWork.Start();

                            emitiu = true;
                            totalDocumentosGerados++;
                            Dominio.ObjetosDeValor.CTe.CTe cte = new Dominio.ObjetosDeValor.CTe.CTe();
                            cte.Documentos = new List<Dominio.ObjetosDeValor.CTe.Documento>();

                            bool contemNFe = pedidoCTeParaSubContratacaoNotasFiscais.Any(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal.Modelo == "55");
                            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoNotaFiscal in pedidoCTeParaSubContratacaoNotasFiscais)
                            {
                                Dominio.ObjetosDeValor.CTe.Documento docNF = serCte.BuscarDocumentoCTe(pedidoCTeParaSubContratacaoNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal, carga.Empresa.TipoAmbiente, configuracaoEmbarcador);
                                if (contemNFe && docNF.ModeloDocumentoFiscal == "55" && !cte.Documentos.Any(obj => obj.ChaveNFE == pedidoCTeParaSubContratacaoNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Chave))
                                    cte.Documentos.Add(docNF);
                                else if (!contemNFe)
                                    cte.Documentos.Add(docNF);
                            }

                            cte.PercentualPagamentoAgregado = pedidoCTeParaSubContratacaoBase.PercentualPagamentoAgregado;

                            List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> cargaPedidoComponentesFretesCliente = BuscarPedidosCTeSubcontratacaoComponentesFrete(pedidosCTesParaSubcontracao, modeloDocumentoFiscalCarga, unitOfWork);

                            cte.Peso = pedidosCTesParaSubcontracao.Sum(o => o.CTeTerceiro.Peso);
                            cte.Volumes = pedidosCTesParaSubcontracao.Sum(o => o.CTeTerceiro.Volumes);
                            cte.MetrosCubicos = pedidosCTesParaSubcontracao.Sum(o => o.CTeTerceiro.MetrosCubicos);
                            cte.PesoCubado = pedidosCTesParaSubcontracao.Sum(o => o.CTeTerceiro.PesoCubado);
                            cte.FatorCubagem = pedidosCTesParaSubcontracao.Sum(o => o.CTeTerceiro.FatorCubagem);
                            cte.PesoFaturado = cte.Peso > cte.PesoCubado ? cte.Peso : cte.PesoCubado;

                            cte.ValorTotalMercadoria = (from obj in pedidosCTesParaSubcontracao select obj.CTeTerceiro.ValorTotalMercadoria).Sum();
                            cte.ValorAReceber = cargaPedido.ValorFrete;
                            cte.ValorFrete = cargaPedido.ValorFrete;
                            cte.ValorTotalPrestacaoServico = cargaPedido.ValorFrete;

                            if (cargaPedido.Pedido.EmpresaSerie != null)
                                cte.Serie = cargaPedido.Pedido.EmpresaSerie.Numero;

                            cte.ProdutoPredominante = cteParaSubcontratacao.ProdutoPredominante;

                            List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade> quantidades = repCTeParaSubContratacaoQuantidade.BuscarPorCTesParaSubContratacao((from obj in pedidosCTesParaSubcontracao select obj.CTeTerceiro.Codigo).ToList());

                            Dominio.Entidades.Cliente tomador = cteParaSubcontratacao.Emitente?.Cliente;

                            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                                tomador = Servicos.Embarcador.Carga.CTeSubContratacao.ObterTomadorCTeParaSubcontratacao(carga, cargaPedido, pedidoCTeParaSubContratacaoBase, configuracaoEmbarcador, tipoServicoMultisoftware, modeloDocumentoEmitir);

                            if (configuracaoEmbarcador.UtilizaEmissaoMultimodal && cargaPedido.Tomador != null)
                                tomador = cargaPedido.Tomador;

                            if (tomador == null)
                                tomador = cargaPedido.Pedido.SubContratante != null ? cargaPedido.Pedido.SubContratante : cargaPedido.Tomador;

                            Dominio.Enumeradores.TipoPagamento tipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;
                            Dominio.Enumeradores.TipoTomador tipoTomador = Dominio.Enumeradores.TipoTomador.Outros;

                            List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesParaSubcontrataca = pedidosCTesParaSubcontracao.Select(x => x.CTeTerceiro).Distinct().ToList();

                            List<string> tiposContaineres = configuracaoEmbarcador.UtilizaEmissaoMultimodal ? repContainerTipo.BuscarDescricoes() : new List<string>();
                            bool utilizarPrimeiraUnidadeMedidaPeso = UtilizarPrimeiraUnidadeMedidaPeso(tomador, carga.TipoOperacao);
                            List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidadesCTes = ConverterQuantidadesTerceirosParaQuantidadesCTe(quantidades, configuracaoEmbarcador.AgruparUnidadesMedidasPorDescricao, utilizarPrimeiraUnidadeMedidaPeso, tiposContaineres);

                            bool emitindoCTeFilialEmissora = false;
                            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeFilialEmissora = pedidoCTeParaSubContratacaoBase.CTeTerceiro.ObterCargaCTe(carga.Codigo);

                            if (cargaPedido.CargaPedidoFilialEmissora)
                                emitindoCTeFilialEmissora = true;

                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga tipoContratacaoCarga = cargaPedido.TipoContratacaoCarga;

                            if (emitindoCTeFilialEmissora)
                                tipoContratacaoCarga = cargaPedido.TipoContratacaoCargaSubContratacaoFilialEmissora;

                            Dominio.Enumeradores.TipoServico tipoServico = Dominio.Enumeradores.TipoServico.SubContratacao;

                            if (carga.CargaSVM)
                                tipoServico = Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal;
                            else if (carga.CargaSVMTerceiro)
                                tipoServico = Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal;
                            else if (tipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho)
                                tipoServico = Dominio.Enumeradores.TipoServico.Redespacho;
                            else if (tipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario)
                                tipoServico = Dominio.Enumeradores.TipoServico.RedIntermediario;

                            List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ctesAnteriores = ConverterCTesTerceirosParaAnteriores(ctesParaSubcontrataca, unitOfWork);

                            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = BuscarRegraICMSAgrupadaPorPedido(cargaOrigem, inicioPrestacao, terminoPrestacao, cargaPedido, unitOfWork);
                            Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = BuscarRegraISSAgrupada(cargaOrigem, pedidosCTesParaSubcontracao, inicioPrestacao, tomador, unitOfWork);
                            Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = servicoCargaPedido.ObterRetornoImpostoIBSCBS(cargaPedido);
                            servicoCargaPedido.PreencherValoresRetornoIBSCBS(impostoIBSCBS, cargaPedido.BaseCalculoIBSCBS, cargaPedido.ValorIBSEstadual, cargaPedido.ValorIBSMunicipal, cargaPedido.ValorCBS);

                            List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> seguros = serCte.ConverterSegurosDeTerceirosEmSeguroCTe(apolicesSeguro, tipoTomador, cte.ValorTotalMercadoria);

                            string observacaoCTe = null;

                            if (!string.IsNullOrWhiteSpace(cargaPedido.Pedido.ObservacaoCTe))
                                observacaoCTe = cargaPedido.Pedido.ObservacaoCTe.Replace("#NumeroOCADocumentoTransporteAnterior", cteParaSubcontratacao.NumeroOperacionalConhecimentoAereo?.ToString() ?? string.Empty);

                            Dominio.Enumeradores.TipoCTE tipoCTe = Dominio.Enumeradores.TipoCTE.Normal;
                            Dominio.Entidades.Empresa empresa = cargaOrigem.Empresa;
                            if (carga.EmitirCTeComplementar)
                            {
                                if (carga.EmitirCTeComplementar && cteParaSubcontratacao.Emitente.CPF_CNPJ_SemFormato != empresa.CNPJ_SemFormato)
                                    empresa = repEmpresa.BuscarPorCNPJ(cteParaSubcontratacao.Emitente.CPF_CNPJ_SemFormato);

                                cte.ChaveCTESubstituicaoComplementar = cteParaSubcontratacao.ChaveAcesso;
                                cte.TipoCTe = Dominio.Enumeradores.TipoCTE.Complemento;
                                cte.TipoServico = cteParaSubcontratacao.TipoServico;
                                tomador = cteParaSubcontratacao.Tomador.Cliente;
                                tipoTomador = cteParaSubcontratacao.TipoTomador;
                                tipoCTe = Dominio.Enumeradores.TipoCTE.Complemento;
                                tipoServico = cteParaSubcontratacao.TipoServico;
                            }
                            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlNotas = pedidoCTeParaSubContratacaoNotasFiscais.Select(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal).ToList();
                            if (contemNFe)
                                xmlNotas = pedidoCTeParaSubContratacaoNotasFiscais.Where(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal.Modelo == "55").Select(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal).ToList();

                            List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil> configuracoes = Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil.ConverterPedidoCTeParaSubContratacaoContaContabilContabilizacao(pedidoCTeParaSubContratacaoBase, pedidosCTeParaSubContratacaoContaContabilContabilizacao);
                            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTE = serCte.GerarCTe(carga, cargaPedido, cte, empresa, remetente, destinatario, tomador, expedidor, recebedor, inicioPrestacao, terminoPrestacao, null, null, tipoPagamento, tipoTomador, quantidadesCTes, cargaPedidoComponentesFretesCliente, observacaoCTe, cargaPedido.Pedido.ObservacaoCTeTerceiro, cargaPedido.Pedido, null, seguros, regraICMS, regraISS, impostoIBSCBS, tipoServicoMultisoftware, modeloDocumentoEmitir, tipoServico, tipoCTe, ctesAnteriores, 0, false, cargaCTeFilialEmissora, false, unitOfWork, configuracaoEmbarcador, configuracaoGeralCarga, pedidoCTeParaSubContratacaoBase.CentroResultado, pedidoCTeParaSubContratacaoBase.CentroResultadoDestinatario, pedidoCTeParaSubContratacaoBase.ItemServico, pedidoCTeParaSubContratacaoBase.CentroResultadoEscrituracao, pedidoCTeParaSubContratacaoBase.CentroResultadoICMS, pedidoCTeParaSubContratacaoBase.CentroResultadoPIS, pedidoCTeParaSubContratacaoBase.CentroResultadoCOFINS, pedidoCTeParaSubContratacaoBase.ValorMaximoCentroContabilizacao, configuracoes, xmlNotas, (cargaPedido.Carga.TipoOperacao?.ConfiguracaoEmissaoDocumento?.UtilizarOutroEnderecoPedidoMesmoSePossuirRecebedor ?? false) ? pedidoEndercoRecebedor : null, descricaoComponenteValorICMS, descricaoComponenteValorFrete);

                            if (emitindoCTeFilialEmissora && cargaCTeFilialEmissora != null)
                            {
                                Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                                cargaCTeFilialEmissora.CargaCTeSubContratacaoFilialEmissora = cargaCTE;
                                repCargaCte.Atualizar(cargaCTeFilialEmissora);
                            }

                            serCte.AverbaCargaCTe(cargaCTE, apolicesSeguro, unitOfWork, configuracaoEmbarcador, cargaPedido.Pedido.FormaAverbacaoCTE);

                            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoNotaFiscal in pedidoCTeParaSubContratacaoNotasFiscais)
                            {
                                Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTE = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe();
                                cargaPedidoXMLNotaFiscalCTE.CargaCTe = cargaCTE;
                                cargaPedidoXMLNotaFiscalCTE.PedidoXMLNotaFiscal = pedidoCTeParaSubContratacaoNotaFiscal.PedidoXMLNotaFiscal;
                                repCargaPedidoXMLNotaFiscalCTe.Inserir(cargaPedidoXMLNotaFiscalCTE);
                            }

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
                            }

                            if (pedidoCTeParaSubContratacaoBase.PossuiNFSManual)
                            {
                                serNFS.CriarCargaCTePendenteEmissaoManualDeNFS(carga, cargaCTE, inicioPrestacao, unitOfWork);

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
                                if (configuracaoCargaEmissaoDocumento.GerarRegistroPorPedidoNaNFSManualPorCTeAnterior)
                                {
                                    Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaNFeParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
                                    var listaCargaDocumentoParaEmissaoNFSManual = repCargaNFeParaEmissaoNFSManual.BuscarDocPorCargaPedido(cargaPedido.Codigo);

                                    if (listaCargaDocumentoParaEmissaoNFSManual.Count() > 0)
                                        continue;

                                    unitOfWork.Start();

                                    Dominio.Entidades.Cliente tomador = cteParaSubcontratacao.Emitente?.Cliente;

                                    if (tomador == null)
                                        tomador = cargaPedido.Pedido.SubContratante != null ? cargaPedido.Pedido.SubContratante : cargaPedido.Tomador;

                                    carga.AgNFSManual = true;
                                    serNFS.CriarNFPendenteEmissaoManualPorCargaPedido(carga, cargaPedido, tomador, inicioPrestacao, unitOfWork);
                                    unitOfWork.CommitChanges();
                                }
                                else
                                {

                                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao in pedidosCTesParaSubcontracao)
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
                    serCTEsImportados.VincularCargaCTeEmitidoAnteriormente(carga, primeiroCargaPedido, unitOfWork);
                }

                if (emitiu)
                    svcHubCarga.InformarQuantidadeDocumentosEmitidos(carga.Codigo, totalDocumentos, totalDocumentosGerados);
            }
        }

        public void GerarCTePorSubcontratacaoAgrupado(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, int totalDocumentos, int quantidadeDocumentosAtualizarCarga, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao> pedidosCTeParaSubContratacaoContaContabilContabilizacao, bool indicadorCteSimplificado, ref int totalDocumentosGerados)
        {
            CTe serCte = new CTe(unitOfWork);
            CTeSubContratacaoEmissao serCTeSubContratacaoEmissao = new CTeSubContratacaoEmissao(unitOfWork);
            Servicos.Embarcador.Carga.NFS serNFS = new NFS(unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroQuantidade repCTeParaSubContratacaoQuantidade = new Repositorio.Embarcador.CTe.CTeTerceiroQuantidade(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroSeguro repCTeParaSubContratacaoSeguro = new Repositorio.Embarcador.CTe.CTeTerceiroSeguro(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repPedidoCTeParaSubContratacaoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.CTe.CTeDocumentoOriginario repDocumentoOriginario = new Repositorio.Embarcador.CTe.CTeDocumentoOriginario(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.ContainerTipo repContainerTipo = new Repositorio.Embarcador.Pedidos.ContainerTipo(unitOfWork);

            CTePorPedido serCTePorPedido = new CTePorPedido(unitOfWork);
            Hubs.Carga svcHubCarga = new Hubs.Carga();
            Servicos.Embarcador.Pedido.PedidoCTeParaSubContratacao servicoPedidoCTeParaSubContratacao = new Pedido.PedidoCTeParaSubContratacao(unitOfWork);

            bool utilizarExpedidorRecebedorPedido = carga.TipoOperacao?.ConfiguracaoEmissaoDocumento?.UtilizarExpedidorRecebedorPedidoSubcontratacao ?? false;

            for (int i = 0; i < cargaPedidos.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(cargaPedidos[i].Codigo);
                Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem = (from obj in cargasOrigem where obj.Codigo == cargaPedido.CargaOrigem.Codigo select obj).FirstOrDefault();

                bool emitirCteFilialEmissora = false;
                if (cargaPedido.Carga.EmpresaFilialEmissora != null && !cargaPedido.Carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                    emitirCteFilialEmissora = true;

                if (!cargaPedido.PossuiCTe && !cargaPedido.PossuiNFS && !cargaPedido.PossuiNFSManual && !(cargaPedido.PossuiNFSManual && cargaPedido.ModeloDocumentoFiscalIntramunicipal != null))
                    continue;

                bool emitiu = false;
                bool emitindoCTeFilialEmissora = false;
                if (cargaPedido.CargaPedidoFilialEmissora)
                    emitindoCTeFilialEmissora = true;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga tipoContratacaoCarga = cargaPedido.TipoContratacaoCarga;

                if (emitindoCTeFilialEmissora)
                    tipoContratacaoCarga = cargaPedido.TipoContratacaoCargaSubContratacaoFilialEmissora;

                Dominio.Enumeradores.TipoServico tipoServico = Dominio.Enumeradores.TipoServico.SubContratacao;

                if (cargaPedido.Carga.CargaSVM)
                    tipoServico = Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal;
                else if (cargaPedido.Carga.CargaSVMTerceiro)
                    tipoServico = Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal;
                else if (tipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho)
                    tipoServico = Dominio.Enumeradores.TipoServico.Redespacho;
                else if (tipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario)
                    tipoServico = Dominio.Enumeradores.TipoServico.RedIntermediario;

                Dominio.Enumeradores.TipoCTE tipoCTe = indicadorCteSimplificado && !cargaPedido.Carga.EmitirCTeComplementar && (tipoServico == Dominio.Enumeradores.TipoServico.SubContratacao || tipoServico == Dominio.Enumeradores.TipoServico.Redespacho) ? Dominio.Enumeradores.TipoCTE.Simplificado : Dominio.Enumeradores.TipoCTE.Normal;

                if (!cargaPedido.CTeEmitidoNoEmbarcador && !cargaPedido.Pedido.PedidoTransbordo)
                {
                    List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> apolicesSeguro = repApoliceSeguroAverbacao.BuscarPorCargaPedido(cargaPedido.Codigo, emitirCteFilialEmissora);

                    serCte.ObterDescricoesComponentesPadrao(cargaPedido.ObterTomador(), cargaPedido.Carga, configuracaoEmbarcador, unitOfWork, out string descricaoComponenteValorFrete, out string descricaoComponenteValorICMS);

                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidosCTesParaSubcontracao = repPedidoCTeParaSubContratacao.BuscarPorCargaPedidoComFetch(cargaPedido.Codigo);

                    List<Dominio.Entidades.Cliente> remetentes = null;
                    if (tipoCTe == Dominio.Enumeradores.TipoCTE.Simplificado)
                    {
                        remetentes = new List<Dominio.Entidades.Cliente>();
                        carga = repCarga.BuscarPorCodigo(carga.Codigo);

                        Servicos.Cliente serCliente = new Servicos.Cliente(StringConexao);
                        Servicos.WebService.Pessoas.Pessoa serPessoa = new Servicos.WebService.Pessoas.Pessoa(unitOfWork);
                        Dominio.ObjetosDeValor.RetornoVerificacaoCliente retorno = serCliente.ConverterObjetoValorPessoa(serPessoa.ConverterObjetoEmpresa(carga.Empresa), "Transportador Remetente Simplificado", unitOfWork, 0, false);

                        if (retorno.Status == true)
                            remetentes.Add(retorno.cliente);
                        else
                            Servicos.Log.GravarInfo("Não foi possivel converter o transportador para cliente para emissão do CT-e Simplificado. Motivo: " + retorno.Mensagem);
                    }
                    else
                    {
                        remetentes = (from obj in pedidosCTesParaSubcontracao select obj.CTeTerceiro.Remetente.Cliente).Distinct().ToList();
                    }

                    for (int r = 0; r < remetentes.Count; r++)
                    {
                        Dominio.Entidades.Cliente remetente = remetentes[r];

                        List<Dominio.Entidades.Cliente> destinatarios = null;
                        if (tipoCTe == Dominio.Enumeradores.TipoCTE.Simplificado)
                        {
                            destinatarios = new List<Dominio.Entidades.Cliente>();
                            carga = repCarga.BuscarPorCodigo(carga.Codigo);

                            Servicos.Cliente serCliente = new Servicos.Cliente(StringConexao);
                            Servicos.WebService.Pessoas.Pessoa serPessoa = new Servicos.WebService.Pessoas.Pessoa(unitOfWork);
                            Dominio.ObjetosDeValor.RetornoVerificacaoCliente retorno = serCliente.ConverterObjetoValorPessoa(serPessoa.ConverterObjetoEmpresa(carga.Empresa), "Transportador Destinatário Simplificado", unitOfWork, 0, false);

                            if (retorno.Status == true)
                                destinatarios.Add(retorno.cliente);
                            else
                                Servicos.Log.GravarInfo("Não foi possivel converter o transportador para cliente para emissão do CT-e Simplificado. Motivo: " + retorno.Mensagem);
                        }
                        else
                        {
                            destinatarios = (from obj in pedidosCTesParaSubcontracao where obj.CTeTerceiro.Remetente.Cliente.CPF_CNPJ == remetente.CPF_CNPJ select obj.CTeTerceiro.Destinatario.Cliente).Distinct().ToList();
                        }

                        for (int d = 0; d < destinatarios.Count; d++)
                        {
                            Dominio.Entidades.Cliente destinatario = destinatarios[d];

                            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidosCTesParaSubcontracaoDestinatario = null;
                            if (tipoCTe == Dominio.Enumeradores.TipoCTE.Simplificado)
                                pedidosCTesParaSubcontracaoDestinatario = (from obj in pedidosCTesParaSubcontracao select obj).ToList();
                            else
                                pedidosCTesParaSubcontracaoDestinatario = (from obj in pedidosCTesParaSubcontracao where obj.CTeTerceiro.Remetente.Cliente.CPF_CNPJ == remetente.CPF_CNPJ && obj.CTeTerceiro.Destinatario.Cliente.CPF_CNPJ == destinatario.CPF_CNPJ select obj).ToList();


                            Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacaoBase = pedidosCTesParaSubcontracaoDestinatario.FirstOrDefault();

                            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalCarga = pedidoCTeParaSubContratacaoBase.ModeloDocumentoFiscal;

                            if (modeloDocumentoFiscalCarga == null)
                                modeloDocumentoFiscalCarga = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.CTe);

                            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> pedidoCTeParaSubContratacaoNotasFiscais = repPedidoCTeParaSubContratacaoNotaFiscal.BuscarPorPedidosCTeParaSubcontratacao((from obj in pedidosCTesParaSubcontracaoDestinatario select obj.Codigo).ToList());

                            Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteParaSubcontratacao = pedidoCTeParaSubContratacaoBase.CTeTerceiro;

                            Dominio.Entidades.Cliente expedidor = null;
                            Dominio.Entidades.Cliente recebedor = null;
                            Dominio.Entidades.Localidade inicioPrestacao = cteParaSubcontratacao.LocalidadeInicioPrestacao;
                            Dominio.Entidades.Localidade terminoPrestacao = cteParaSubcontratacao.LocalidadeTerminoPrestacao;

                            if (utilizarExpedidorRecebedorPedido || (configuracaoEmbarcador.UtilizaEmissaoMultimodal && cargaPedido.Expedidor != null))
                            {
                                expedidor = cargaPedido.Expedidor;

                                if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada &&
                                    cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada &&
                                    expedidor != null)
                                    inicioPrestacao = expedidor.Localidade;
                                else if (utilizarExpedidorRecebedorPedido && expedidor == null && remetente != null)
                                    inicioPrestacao = remetente.Localidade;
                            }
                            else if (cteParaSubcontratacao.Expedidor != null && (cteParaSubcontratacao.Expedidor.CPF_CNPJ != cteParaSubcontratacao.Remetente.CPF_CNPJ || cargaPedido.Expedidor == null) && !cargaPedido.PedidoEncaixado)
                                expedidor = cteParaSubcontratacao.Expedidor.Cliente != null ? repCliente.BuscarPorCPFCNPJ(cteParaSubcontratacao.Expedidor.Cliente.CPF_CNPJ) : repCliente.BuscarPorCPFCNPJ(double.Parse(cteParaSubcontratacao.Expedidor.CPF_CNPJ_SemFormato));
                            else if (cargaPedido.Expedidor != null)
                            {
                                expedidor = cargaPedido.Expedidor;
                                if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
                                    inicioPrestacao = expedidor.Localidade;
                            }

                            if (utilizarExpedidorRecebedorPedido || ((configuracaoEmbarcador.UtilizaEmissaoMultimodal || (carga.TipoOperacao?.UtilizarRecebedorPedidoParaSVM ?? false)) && cargaPedido.Recebedor != null))
                            {
                                recebedor = cargaPedido.Recebedor;

                                if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada &&
                                    cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada &&
                                    recebedor != null)
                                    terminoPrestacao = recebedor.Localidade;
                                else if (utilizarExpedidorRecebedorPedido && recebedor == null && destinatario != null)
                                    terminoPrestacao = destinatario.Localidade;
                            }
                            else if (cteParaSubcontratacao.Recebedor != null && (cteParaSubcontratacao.Recebedor.CPF_CNPJ != cteParaSubcontratacao.Destinatario.CPF_CNPJ || cargaPedido.Recebedor == null) && !cargaPedido.PedidoEncaixado)
                            {
                                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor))
                                    recebedor = cteParaSubcontratacao.Recebedor.Cliente != null ? repCliente.BuscarPorCPFCNPJ(cteParaSubcontratacao.Recebedor.Cliente.CPF_CNPJ) : repCliente.BuscarPorCPFCNPJ(double.Parse(cteParaSubcontratacao.Recebedor.CPF_CNPJ_SemFormato));
                                else //todo: if feito por um caso danone onde os operadores emitirar o Ct-e da transportara danone errado, nesse caso o cte de subcontratacao deve ser somente até o destinatario rever isso (09/10/2019 Rodrigo)
                                    terminoPrestacao = destinatario.Localidade;
                            }
                            else if (cargaPedido.Recebedor != null)
                            {
                                recebedor = cargaPedido.Recebedor;
                                if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
                                    terminoPrestacao = recebedor.Localidade;
                            }

                            if (!pedidoCTeParaSubContratacaoBase.PossuiNFSManual ||
                                (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS &&
                                (inicioPrestacao.Codigo != terminoPrestacao.Codigo || (pedidoCTeParaSubContratacaoBase.PossuiNFSManual && cargaPedido.ModeloDocumentoFiscalIntramunicipal != null && (inicioPrestacao.Codigo == terminoPrestacao.Codigo || cargaPedido.DisponibilizarDocumentoNFSManual)))))
                            {
                                bool somenteCTeSubContratacaoFilialEmissora = false;

                                if (cargaPedido.CargaPedidoFilialEmissora && cargaPedido.Carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                                    somenteCTeSubContratacaoFilialEmissora = true;

                                if (repCargaPedidoXMLNotaFiscalCTe.ContarCTesPorPedidoCTeParaSubContratacao(pedidosCTesParaSubcontracaoDestinatario.Select(p => p.Codigo).ToList(), somenteCTeSubContratacaoFilialEmissora) > 0)
                                    continue;

                                Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoEmitir = null;

                                if (pedidoCTeParaSubContratacaoBase.PossuiNFSManual)
                                    modeloDocumentoEmitir = cargaPedido.ModeloDocumentoFiscalIntramunicipal;
                                else
                                    modeloDocumentoEmitir = modeloDocumentoFiscalCarga;

                                unitOfWork.Start();

                                emitiu = true;
                                totalDocumentosGerados++;
                                Dominio.ObjetosDeValor.CTe.CTe cte = new Dominio.ObjetosDeValor.CTe.CTe();
                                cte.Documentos = new List<Dominio.ObjetosDeValor.CTe.Documento>();
                                cte.Entregas = new List<Dominio.ObjetosDeValor.CTe.Entrega>();

                                bool contemNFe = pedidoCTeParaSubContratacaoNotasFiscais.Any(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal.Modelo == "55");
                                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoNotaFiscal in pedidoCTeParaSubContratacaoNotasFiscais)
                                {
                                    Dominio.ObjetosDeValor.CTe.Documento docNF = serCte.BuscarDocumentoCTe(pedidoCTeParaSubContratacaoNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal, cargaPedido.Carga.Empresa.TipoAmbiente, configuracaoEmbarcador);
                                    if (contemNFe && docNF.ModeloDocumentoFiscal == "55" && !cte.Documentos.Any(obj => obj.ChaveNFE == pedidoCTeParaSubContratacaoNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Chave))
                                        cte.Documentos.Add(docNF);
                                    else if (!contemNFe)
                                        cte.Documentos.Add(docNF);
                                    //else
                                    //    cte.Documentos.Add(docNF);
                                }

                                cte.PercentualPagamentoAgregado = pedidoCTeParaSubContratacaoBase.PercentualPagamentoAgregado;

                                List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> cargaPedidoComponentesFretesCliente = BuscarPedidosCTeSubcontratacaoComponentesFrete(pedidosCTesParaSubcontracaoDestinatario, modeloDocumentoFiscalCarga, unitOfWork);

                                cte.Peso = pedidosCTesParaSubcontracaoDestinatario.Sum(o => o.CTeTerceiro.Peso);
                                cte.Volumes = pedidosCTesParaSubcontracaoDestinatario.Sum(o => o.CTeTerceiro.Volumes);
                                cte.MetrosCubicos = pedidosCTesParaSubcontracaoDestinatario.Sum(o => o.CTeTerceiro.MetrosCubicos);
                                cte.PesoCubado = pedidosCTesParaSubcontracaoDestinatario.Sum(o => o.CTeTerceiro.PesoCubado);
                                cte.FatorCubagem = pedidosCTesParaSubcontracaoDestinatario.Sum(o => o.CTeTerceiro.FatorCubagem);
                                cte.PesoFaturado = cte.Peso > cte.PesoCubado ? cte.Peso : cte.PesoCubado;

                                cte.ValorTotalMercadoria = (from obj in pedidosCTesParaSubcontracaoDestinatario select obj.CTeTerceiro.ValorTotalMercadoria).Sum();
                                cte.ValorAReceber = (from obj in pedidosCTesParaSubcontracaoDestinatario select obj.ValorFrete).Sum();
                                cte.ValorFrete = (from obj in pedidosCTesParaSubcontracaoDestinatario select obj.ValorFrete).Sum();
                                cte.ValorTotalPrestacaoServico = (from obj in pedidosCTesParaSubcontracaoDestinatario select obj.ValorFrete).Sum();
                                decimal valorICMS = (from obj in pedidosCTesParaSubcontracaoDestinatario select obj.ValorICMS).Sum();
                                decimal aliquota = pedidoCTeParaSubContratacaoBase.PercentualAliquota;

                                if (cargaPedido.Pedido.EmpresaSerie != null)
                                    cte.Serie = cargaPedido.Pedido.EmpresaSerie.Numero;

                                cte.ProdutoPredominante = cteParaSubcontratacao.ProdutoPredominante;

                                List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade> quantidades = repCTeParaSubContratacaoQuantidade.BuscarPorCTesParaSubContratacao((from obj in pedidosCTesParaSubcontracaoDestinatario select obj.CTeTerceiro.Codigo).ToList());

                                Dominio.Entidades.Cliente tomador = Servicos.Embarcador.Carga.CTeSubContratacao.ObterTomadorCTeParaSubcontratacao(carga, cargaPedido, pedidoCTeParaSubContratacaoBase, configuracaoEmbarcador, tipoServicoMultisoftware, modeloDocumentoEmitir);

                                Dominio.Enumeradores.TipoPagamento tipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;
                                Dominio.Enumeradores.TipoTomador tipoTomador = Dominio.Enumeradores.TipoTomador.Outros;

                                List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesParaSubcontrataca = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>();
                                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacaoDestinatario in pedidosCTesParaSubcontracaoDestinatario)
                                {
                                    if (!ctesParaSubcontrataca.Contains(pedidoCTeParaSubContratacaoDestinatario.CTeTerceiro))
                                        ctesParaSubcontrataca.Add(pedidoCTeParaSubContratacaoDestinatario.CTeTerceiro);
                                }

                                List<string> tiposContaineres = configuracaoEmbarcador.UtilizaEmissaoMultimodal ? repContainerTipo.BuscarDescricoes() : new List<string>();
                                bool utilizarPrimeiraUnidadeMedidaPeso = UtilizarPrimeiraUnidadeMedidaPeso(tomador, carga.TipoOperacao);
                                List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidadesCTes = ConverterQuantidadesTerceirosParaQuantidadesCTe(quantidades, configuracaoEmbarcador.AgruparUnidadesMedidasPorDescricao, utilizarPrimeiraUnidadeMedidaPeso, tiposContaineres);

                                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeFilialEmissora = pedidoCTeParaSubContratacaoBase.CTeTerceiro.ObterCargaCTe(carga.Codigo);

                                List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ctesAnteriores = ConverterCTesTerceirosParaAnteriores(ctesParaSubcontrataca, unitOfWork);
                                if (tipoCTe == Dominio.Enumeradores.TipoCTE.Simplificado)
                                {
                                    foreach (Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteAnterior in ctesAnteriores)
                                    {
                                        bool somarFrete = true;
                                        var pedidoCTeParaSubContratacaoNotaFiscal = pedidoCTeParaSubContratacaoNotasFiscais.Where(o => o.PedidoCTeParaSubContratacao.CTeTerceiro.ChaveAcesso == cteAnterior.Chave).FirstOrDefault();
                                        if (pedidoCTeParaSubContratacaoNotaFiscal == null)
                                        {
                                            somarFrete = false;
                                            pedidoCTeParaSubContratacaoNotaFiscal = pedidoCTeParaSubContratacaoNotasFiscais.FirstOrDefault();
                                        }

                                        serCte.AdicionarAtualizarEntregaCTe(ref cte, null, pedidoCTeParaSubContratacaoNotaFiscal.PedidoXMLNotaFiscal, carga.PossuiComponenteFreteComImpostoIncluso, emitirCteFilialEmissora, null, null, false, unitOfWork, cteAnterior, somarFrete);
                                    }
                                }

                                Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = BuscarRegraICMSAgrupada(cargaOrigem, inicioPrestacao, terminoPrestacao, pedidosCTesParaSubcontracaoDestinatario, unitOfWork);
                                Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = BuscarRegraISSAgrupada(cargaOrigem, pedidosCTesParaSubcontracaoDestinatario, inicioPrestacao, tomador, unitOfWork);
                                Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = servicoPedidoCTeParaSubContratacao.ObterRetornoImpostoIBSCBS(pedidosCTesParaSubcontracaoDestinatario);

                                List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> seguros = serCte.ConverterSegurosDeTerceirosEmSeguroCTe(apolicesSeguro, tipoTomador, cte.ValorTotalMercadoria);

                                string observacaoCTe = null;

                                if (!string.IsNullOrWhiteSpace(cargaPedido.Pedido.ObservacaoCTe)) //essa informação por hora só existe no CT-e para subcontratação, pois não emitimos modal aéreo
                                    observacaoCTe = cargaPedido.Pedido.ObservacaoCTe.Replace("#NumeroOCADocumentoTransporteAnterior", cteParaSubcontratacao.NumeroOperacionalConhecimentoAereo?.ToString() ?? string.Empty);

                                Dominio.Entidades.Empresa empresa = cargaOrigem.Empresa;
                                if (cargaPedido.Carga.EmitirCTeComplementar)
                                {
                                    if (carga.EmitirCTeComplementar && cteParaSubcontratacao.Emitente.CPF_CNPJ_SemFormato != empresa.CNPJ_SemFormato)
                                        empresa = repEmpresa.BuscarPorCNPJ(cteParaSubcontratacao.Emitente.CPF_CNPJ_SemFormato);

                                    cte.ChaveCTESubstituicaoComplementar = cteParaSubcontratacao.ChaveAcesso;
                                    cte.TipoCTe = Dominio.Enumeradores.TipoCTE.Complemento;
                                    cte.TipoServico = cteParaSubcontratacao.TipoServico;
                                    tomador = cteParaSubcontratacao.Tomador.Cliente;
                                    tipoTomador = cteParaSubcontratacao.TipoTomador;
                                    tipoCTe = Dominio.Enumeradores.TipoCTE.Complemento;
                                    tipoServico = cteParaSubcontratacao.TipoServico;
                                }

                                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlNotas = pedidoCTeParaSubContratacaoNotasFiscais.Select(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal).ToList();
                                if (contemNFe)
                                    xmlNotas = pedidoCTeParaSubContratacaoNotasFiscais.Where(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal.Modelo == "55").Select(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal).ToList();

                                List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil> configuracoes = Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil.ConverterPedidoCTeParaSubContratacaoContaContabilContabilizacao(pedidoCTeParaSubContratacaoBase, pedidosCTeParaSubContratacaoContaContabilContabilizacao);
                                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTE = serCte.GerarCTe(cargaPedido.Carga, cargaPedido, cte, cargaOrigem.Empresa, remetente, destinatario, tomador, expedidor, recebedor, inicioPrestacao, terminoPrestacao, null, null, tipoPagamento, tipoTomador, quantidadesCTes, cargaPedidoComponentesFretesCliente, observacaoCTe, cargaPedido.Pedido.ObservacaoCTeTerceiro, cargaPedido.Pedido, null, seguros, regraICMS, regraISS, impostoIBSCBS, tipoServicoMultisoftware, modeloDocumentoEmitir, tipoServico, tipoCTe, ctesAnteriores, 0, false, cargaCTeFilialEmissora, false, unitOfWork, configuracaoEmbarcador, configuracaoGeralCarga, pedidoCTeParaSubContratacaoBase.CentroResultado, pedidoCTeParaSubContratacaoBase.CentroResultadoDestinatario, pedidoCTeParaSubContratacaoBase.ItemServico, pedidoCTeParaSubContratacaoBase.CentroResultadoEscrituracao, pedidoCTeParaSubContratacaoBase.CentroResultadoICMS, pedidoCTeParaSubContratacaoBase.CentroResultadoPIS, pedidoCTeParaSubContratacaoBase.CentroResultadoCOFINS, pedidoCTeParaSubContratacaoBase.ValorMaximoCentroContabilizacao, configuracoes, xmlNotas, null, descricaoComponenteValorICMS, descricaoComponenteValorFrete);

                                if (emitindoCTeFilialEmissora && cargaCTeFilialEmissora != null)
                                {
                                    Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                                    cargaCTeFilialEmissora.CargaCTeSubContratacaoFilialEmissora = cargaCTE;
                                    repCargaCte.Atualizar(cargaCTeFilialEmissora);
                                }

                                serCte.AverbaCargaCTe(cargaCTE, apolicesSeguro, unitOfWork, configuracaoEmbarcador, cargaPedido.Pedido.FormaAverbacaoCTE);

                                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoNotaFiscal in pedidoCTeParaSubContratacaoNotasFiscais)
                                {
                                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTE = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe();
                                    cargaPedidoXMLNotaFiscalCTE.CargaCTe = cargaCTE;
                                    cargaPedidoXMLNotaFiscalCTE.PedidoXMLNotaFiscal = pedidoCTeParaSubContratacaoNotaFiscal.PedidoXMLNotaFiscal;
                                    repCargaPedidoXMLNotaFiscalCTe.Inserir(cargaPedidoXMLNotaFiscalCTE);
                                }

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
                                }

                                if (pedidoCTeParaSubContratacaoBase.PossuiNFSManual)
                                {
                                    serNFS.CriarCargaCTePendenteEmissaoManualDeNFS(cargaPedido.Carga, cargaCTE, inicioPrestacao, unitOfWork);

                                    cargaPedido.Carga.AgNFSManual = true;

                                    repCarga.Atualizar(carga);
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
                                if (cargaPedido.CargaPedidoFilialEmissora || (cargaPedido.CargaPedidoTrechoAnterior?.CargaPedidoFilialEmissora ?? false))
                                {
                                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoPedidoNotaFiscal in pedidoCTeParaSubContratacaoNotasFiscais)
                                    {
                                        Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscalNFS = pedidoCTeParaSubContratacaoPedidoNotaFiscal.PedidoXMLNotaFiscal;
                                        if (pedidoXMLNotaFiscalNFS.DocsParaEmissaoNFSManual.Count > 0)
                                            continue;

                                        unitOfWork.Start();

                                        cargaPedido.Carga.AgNFSManual = true;

                                        serNFS.CriarNFPendenteEmissaoManualDeNFS(cargaPedido.Carga, pedidoXMLNotaFiscalNFS, cargaPedido.ObterTomador(), terminoPrestacao, unitOfWork);

                                        repCarga.Atualizar(carga);

                                        unitOfWork.CommitChanges();
                                    }
                                }
                                else
                                {
                                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao in pedidosCTesParaSubcontracaoDestinatario)
                                    {
                                        if (pedidoCTeParaSubContratacao.DocsParaEmissaoNFSManual.Count > 0)
                                            continue;

                                        unitOfWork.Start();

                                        Dominio.Entidades.Cliente tomador = cteParaSubcontratacao.Emitente?.Cliente;

                                        if (tomador == null)
                                            tomador = cargaPedido.Pedido.SubContratante != null ? cargaPedido.Pedido.SubContratante : cargaPedido.Tomador;

                                        cargaPedido.Carga.AgNFSManual = true;

                                        serNFS.CriarCTePendenteEmissaoManualDeNFS(carga, pedidoCTeParaSubContratacao, tomador, inicioPrestacao, pedidoCTeParaSubContratacao.ValorFrete, unitOfWork);

                                        repCarga.Atualizar(carga);

                                        unitOfWork.CommitChanges();
                                    }
                                }
                            }

                            if (totalDocumentosGerados == 1 || (totalDocumentosGerados % quantidadeDocumentosAtualizarCarga) == 0)
                                svcHubCarga.InformarQuantidadeDocumentosEmitidos(cargaPedido.Carga.Codigo, totalDocumentos, totalDocumentosGerados);

                            //unitOfWork.FlushAndClear();
                        }
                    }

                }
                else
                {
                    Servicos.Embarcador.CTe.CTEsImportados serCTEsImportados = new Servicos.Embarcador.CTe.CTEsImportados(unitOfWork);
                    serCTEsImportados.VincularCargaCTeEmitidoAnteriormente(cargaPedido.Carga, cargaPedido, unitOfWork);
                }

                if (emitiu)
                    svcHubCarga.InformarQuantidadeDocumentosEmitidos(cargaPedido.Carga.Codigo, totalDocumentos, totalDocumentosGerados);
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.CTe.CTe ConverterCTeTerceiroParaObjeto(Dominio.Entidades.Embarcador.CTe.CTeTerceiro cte, bool enviarCTeApenasParaTomador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
            Servicos.Embarcador.CTe.DocumentoCTe serDocumentoCTe = new Servicos.Embarcador.CTe.DocumentoCTe(unitOfWork);
            Servicos.Embarcador.CTe.Participante serParticipante = new Servicos.Embarcador.CTe.Participante(unitOfWork);
            Servicos.Embarcador.CTe.ModalRodoviario serModalRodoviario = new Servicos.Embarcador.CTe.ModalRodoviario(unitOfWork);
            Servicos.Embarcador.CTe.Quantidades serQuantidades = new Servicos.Embarcador.CTe.Quantidades(unitOfWork);
            Servicos.Embarcador.CTe.Seguro serSeguros = new Servicos.Embarcador.CTe.Seguro(unitOfWork);
            Servicos.Embarcador.CTe.ProdutoPerigoso serProdutoPerigoso = new Servicos.Embarcador.CTe.ProdutoPerigoso(unitOfWork);
            Servicos.WebService.Empresa.Empresa serEmpresa = new WebService.Empresa.Empresa(unitOfWork);
            Servicos.Embarcador.Localidades.Localidade serLocalidade = new Localidades.Localidade(unitOfWork);
            Servicos.Embarcador.CTe.DocumentoAnterior serDocumentoAnterior = new Servicos.Embarcador.CTe.DocumentoAnterior(unitOfWork);
            Servicos.Embarcador.CTe.DocumentoTransportaAnteriorPapel serDocumentoTransportaAnteriorPapel = new Servicos.Embarcador.CTe.DocumentoTransportaAnteriorPapel(unitOfWork);
            Servicos.Embarcador.CTe.Duplicatas serDuplicatas = new Servicos.Embarcador.CTe.Duplicatas(unitOfWork);
            Servicos.Embarcador.CTe.Observacoes serObservacoes = new Servicos.Embarcador.CTe.Observacoes(unitOfWork);
            Servicos.Embarcador.CTe.ComponenteFrete serComponenteFrete = new Servicos.Embarcador.CTe.ComponenteFrete(unitOfWork);
            Servicos.Embarcador.CTe.TotalServico serTotalServicos = new Servicos.Embarcador.CTe.TotalServico(unitOfWork);
            Servicos.Embarcador.CTe.InformacaoCarga serInformacaoCarga = new Servicos.Embarcador.CTe.InformacaoCarga(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao = new Dominio.ObjetosDeValor.Embarcador.CTe.CTe();
            cteIntegracao.Codigo = cte.Codigo;
            cteIntegracao.Emitente = serEmpresa.ConverterObjetoEmpresa(cte.Emitente);
            cteIntegracao.Numero = cte.Numero;

            cteIntegracao.Serie = cte.Serie;
            cteIntegracao.LocalidadeInicioPrestacao = serLocalidade.ConverterObjetoLocalidade(cte.LocalidadeInicioPrestacao);
            cteIntegracao.LocalidadeFimPrestacao = serLocalidade.ConverterObjetoLocalidade(cte.LocalidadeTerminoPrestacao);
            cteIntegracao.TipoCTE = cte.TipoCTE;
            cteIntegracao.TipoPagamento = cte.TipoPagamento;
            cteIntegracao.TipoServico = cte.TipoServico;
            cteIntegracao.TipoImpressao = Dominio.Enumeradores.TipoImpressao.Retrato;
            cteIntegracao.TipoTomador = cte.TipoTomador;
            cteIntegracao.CFOP = cte.CFOP.CodigoCFOP;
            cteIntegracao.DataEmissao = cte.DataEmissao;
            cteIntegracao.Retira = false;
            cteIntegracao.DetalhesRetira = "";
            cteIntegracao.SituacaoCTeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Autorizada;
            cteIntegracao.Chave = cte.ChaveAcesso;
            cteIntegracao.Protocolo = cte.ProtocoloCliente;

            serDocumentoCTe.SetarEntidadeCTeParaDocumentos(cte, ref cteIntegracao, unitOfWork);
            cteIntegracao.Remetente = serParticipante.ConverterParticipanteParaParticipante(cte.Remetente, enviarCTeApenasParaTomador, cteIntegracao.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente, unitOfWork);
            cteIntegracao.Destinatario = serParticipante.ConverterParticipanteParaParticipante(cte.Destinatario, enviarCTeApenasParaTomador, cteIntegracao.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario, unitOfWork);
            cteIntegracao.Expedidor = serParticipante.ConverterParticipanteParaParticipante(cte.Expedidor, enviarCTeApenasParaTomador, cteIntegracao.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor, unitOfWork);
            cteIntegracao.Recebedor = serParticipante.ConverterParticipanteParaParticipante(cte.Recebedor, enviarCTeApenasParaTomador, cteIntegracao.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor, unitOfWork);
            cteIntegracao.Tomador = serParticipante.ConverterParticipanteParaParticipante(cte.Tomador, enviarCTeApenasParaTomador, cteIntegracao.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros, unitOfWork);
            cteIntegracao.ModalRodoviario = serModalRodoviario.ConverterModalTransporteCTeParaModalRodoviario(cte, unitOfWork);
            cteIntegracao.InformacaoCarga = serInformacaoCarga.ConverterInfomracaoCTeParaInformacaoCarga(cte, unitOfWork);

            cteIntegracao.QuantidadesCarga = serQuantidades.ConverterQuantidadesCTeParaQuantidades(cte.CTeTerceiroQuantidades?.ToList(), unitOfWork);
            cteIntegracao.Seguros = serSeguros.ConverterSegurosCTeParaSeguro(cte.CTeTerceiroSeguros?.ToList(), unitOfWork);
            cteIntegracao.ProdutosPerigosos = null;//serProdutoPerigoso.ConverterProdutoPerigosoCTeParaProdutoPerigoso(cte, unitOfWork);
            cteIntegracao.DocumentosAnteriores = null;// serDocumentoAnterior.ConverterDocumentosAnterioresCTeParaDocumentosAnteriores(cte.DocumentosTransporteAnterior.ToList(), unitOfWork);
            cteIntegracao.DocumentosAnterioresDePapel = null;// serDocumentoTransportaAnteriorPapel.ConverterDocumentoTransporteParaTransporteAnteriorPapel(cte.DocumentosTransporteAnterior.ToList(), unitOfWork);
            //cteIntegracao.Duplicata = serDuplicatas.ConverterDynamicParaDuplicata(cte.Duplicata, unitOfWork);

            cteIntegracao.ObservacoesContribuinte = null;//serObservacoes.ConverterObservacaoContribuintesCTeParaObservacoes(cte.ObservacoesContribuinte.ToList());
            cteIntegracao.ObservacoesFisco = null;//serObservacoes.ConverterObservacaoFiscoCTeParaObservacoes(cte.ObservacoesFisco.ToList());

            //if (!string.IsNullOrWhiteSpace(cte.ObservacoesGerais))
            //{
            //    cteIntegracao.ObservacoesGeral = new Dominio.ObjetosDeValor.Embarcador.CTe.Observacao();
            //    cteIntegracao.ObservacoesGeral.Campo = "Observação Geral";
            //    cteIntegracao.ObservacoesGeral.Texto = cte.ObservacoesGerais;
            //}

            cteIntegracao.ValorFrete = serTotalServicos.ConverterCTeParaFreteValor(cte, unitOfWork);

            return cteIntegracao;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ConverterCTesTerceirosParaAnteriores(List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesTerceiro, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CTe.CTeTerceiroDocumentoAdicional repCTeTerceiroDocumentoAdicional = new Repositorio.Embarcador.CTe.CTeTerceiroDocumentoAdicional(unitOfWork);

            Servicos.WebService.Empresa.Empresa serEmpresa = new Servicos.WebService.Empresa.Empresa(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ctesAnteriores = new List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe>();

            List<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa> emitentes = new List<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa>();

            foreach (Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro in ctesTerceiro)
            {
                if (cteTerceiro.Emitente != null)
                {
                    Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa emitente = (from obj in emitentes where obj.CNPJ == cteTerceiro.Emitente.CPF_CNPJ_SemFormato select obj).FirstOrDefault();

                    if (emitente == null)
                    {
                        emitente = serEmpresa.ConverterObjetoEmpresa(cteTerceiro.Emitente);
                        emitentes.Add(emitente);
                    }

                    ctesAnteriores.Add(new Dominio.ObjetosDeValor.Embarcador.CTe.CTe
                    {
                        Emitente = emitente,
                        Chave = cteTerceiro.ChaveAcesso,
                        Numero = cteTerceiro.Numero
                    });
                }
            }

            List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicional> cteTerceiroDocumentosAdicionais = repCTeTerceiroDocumentoAdicional.BuscarPorCTeTerceiro(ctesTerceiro);
            if (cteTerceiroDocumentosAdicionais.Count > 0)
            {
                for (int i = 0; i < cteTerceiroDocumentosAdicionais.Count; i++)
                {
                    Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicional cteTerceiroDocumentoAdicional = cteTerceiroDocumentosAdicionais[i];

                    if (!ctesAnteriores.Any(o => o.Chave == cteTerceiroDocumentoAdicional.Chave))
                        ctesAnteriores.Add(new Dominio.ObjetosDeValor.Embarcador.CTe.CTe() { Emitente = cteTerceiroDocumentoAdicional.Emitente != null ? serEmpresa.ConverterObjetoEmpresa(cteTerceiroDocumentoAdicional.Emitente) : emitentes.FirstOrDefault(), Chave = cteTerceiroDocumentoAdicional.Chave, Numero = cteTerceiroDocumentoAdicional.Numero });
                }
            }

            return ctesAnteriores;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> ConverterQuantidadesTerceirosParaQuantidadesCTe(List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade> quantidadesTerceiros, bool agruparUnidadesMedidasPorDescricao, bool utilizarPrimeiraUnidadeMedidaPeso, List<string> descricoesTipoContainer)
        {
            List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidades = new List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga>();

            foreach (Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade quantidadeTerceiro in quantidadesTerceiros)
            {
                Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga quantidade = null;
                if (agruparUnidadesMedidasPorDescricao)
                    quantidade = (from obj in quantidades where obj.Medida == quantidadeTerceiro.TipoMedida select obj).FirstOrDefault();
                else
                    quantidade = (from obj in quantidades where obj.Unidade == quantidadeTerceiro.Unidade select obj).FirstOrDefault();

                if (quantidade == null)
                {
                    Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga quantidadeCarga = new Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga();
                    quantidadeCarga.Unidade = quantidadeTerceiro.Unidade;
                    quantidadeCarga.Quantidade = quantidadeTerceiro.Quantidade;
                    quantidadeCarga.Medida = quantidadeTerceiro.TipoMedida;
                    quantidades.Add(quantidadeCarga);
                }
                else
                {
                    if ((descricoesTipoContainer == null || descricoesTipoContainer.Count == 0 || !descricoesTipoContainer.Contains(quantidadeTerceiro.TipoMedida)) && !utilizarPrimeiraUnidadeMedidaPeso)
                        quantidade.Quantidade += quantidadeTerceiro.Quantidade;
                }
            }

            return quantidades;
        }

        public bool UtilizarPrimeiraUnidadeMedidaPeso(Dominio.Entidades.Cliente tomador, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            if (tipoOperacao?.UsarConfiguracaoEmissao ?? false)
                return tipoOperacao.ConfiguracaoEmissao?.UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao ?? false;

            if (tomador?.NaoUsarConfiguracaoEmissaoGrupo ?? false)
                return tomador.ConfiguracaoEmissao?.UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao ?? false;

            if (tomador?.GrupoPessoas != null)
                return tomador.GrupoPessoas.ConfiguracaoEmissao?.UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao ?? false;

            return false;
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS BuscarRegraICMSAgrupada(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Localidade inicioPrestacao, Dominio.Entidades.Localidade fimPrestacao, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidosCTeParaSubContratacao, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS();
            Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = pedidosCTeParaSubContratacao.FirstOrDefault();

            regraICMS.Aliquota = pedidoCTeParaSubContratacao.PercentualAliquota;
            regraICMS.AliquotaInternaDifal = pedidoCTeParaSubContratacao.PercentualAliquotaInternaDifal;

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

            if (pedidoCTeParaSubContratacao.CST != null)
                regraICMS.CST = pedidoCTeParaSubContratacao.CST;
            else
                regraICMS.CST = "40";

            if (regraICMS.CST == "40" || regraICMS.CST == "")
                regraICMS.Aliquota = 0;

            regraICMS.PercentualReducaoBC = pedidoCTeParaSubContratacao.PercentualReducaoBC;
            regraICMS.PercentualInclusaoBC = pedidoCTeParaSubContratacao.PercentualIncluirBaseCalculo;
            regraICMS.ValorICMS = (from obj in pedidosCTeParaSubContratacao select obj.ValorICMS).Sum();
            regraICMS.ValorICMSIncluso = (from obj in pedidosCTeParaSubContratacao select obj.ValorICMSIncluso).Sum();

            regraICMS.ValorPis = (from obj in pedidosCTeParaSubContratacao select obj.ValorPis).Sum();
            regraICMS.ValorCofins = (from obj in pedidosCTeParaSubContratacao select obj.ValorCofins).Sum();
            regraICMS.AliquotaPis = pedidoCTeParaSubContratacao.AliquotaPis;
            regraICMS.AliquotaCofins = pedidoCTeParaSubContratacao.AliquotaCofins;

            if (!string.IsNullOrWhiteSpace(pedidoCTeParaSubContratacao.ObservacaoRegraICMSCTe))
                regraICMS.ObservacaoCTe = pedidoCTeParaSubContratacao.ObservacaoRegraICMSCTe;
            else
                regraICMS.ObservacaoCTe = "";

            regraICMS.IncluirICMSBC = pedidoCTeParaSubContratacao.IncluirICMSBaseCalculo;
            regraICMS.ValorBaseCalculoICMS = (from obj in pedidosCTeParaSubContratacao select obj.BaseCalculoICMS).Sum();
            regraICMS.ValorBaseCalculoPISCOFINS = (from obj in pedidosCTeParaSubContratacao select obj.BaseCalculoICMS).Sum();
            regraICMS.DescontarICMSDoValorAReceber = pedidoCTeParaSubContratacao.CTeTerceiro != null && pedidoCTeParaSubContratacao.CTeTerceiro.CST == "60" ? (pedidoCTeParaSubContratacao.CTeTerceiro.ValorAReceber + pedidoCTeParaSubContratacao.CTeTerceiro.ValorICMS == pedidoCTeParaSubContratacao.CTeTerceiro.ValorPrestacaoServico) : false;
            regraICMS.CodigoRegra = pedidoCTeParaSubContratacao.RegraICMS?.Codigo ?? 0;

            return regraICMS;
        }

        private Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS BuscarRegraICMSAgrupadaPorPedido(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Localidade inicioPrestacao, Dominio.Entidades.Localidade fimPrestacao, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS();

            regraICMS.Aliquota = cargaPedido.PercentualAliquota;
            regraICMS.AliquotaInternaDifal = cargaPedido.PercentualAliquotaInternaDifal;

            if (cargaPedido.CFOP != null)
            {
                regraICMS.CFOP = cargaPedido.CFOP.CodigoCFOP;
            }
            else
            {
                Servicos.Embarcador.Carga.ICMS svcICMS = new ICMS(unitOfWork);

                Dominio.Entidades.Aliquota aliquota = svcICMS.ObterAliquota(carga.Empresa.Localidade.Estado, inicioPrestacao.Estado, fimPrestacao.Estado, cargaPedido.ObterTomador().Atividade, cargaPedido.ObterDestinatario().Atividade, unitOfWork);

                regraICMS.CFOP = aliquota?.CFOP?.CodigoCFOP ?? cargaPedido.CFOP.CodigoCFOP;
            }

            if (cargaPedido.CST != null)
                regraICMS.CST = cargaPedido.CST;
            else
                regraICMS.CST = "40";

            if (regraICMS.CST == "40" || regraICMS.CST == "")
                regraICMS.Aliquota = 0;

            regraICMS.PercentualReducaoBC = cargaPedido.PercentualReducaoBC;
            regraICMS.PercentualInclusaoBC = cargaPedido.PercentualIncluirBaseCalculo;
            regraICMS.ValorICMS = cargaPedido.ValorICMS;
            regraICMS.ValorICMSIncluso = cargaPedido.ValorICMSIncluso;

            regraICMS.ValorPis = cargaPedido.ValorPis;
            regraICMS.ValorCofins = cargaPedido.ValorCofins;
            regraICMS.AliquotaPis = cargaPedido.AliquotaPis;
            regraICMS.AliquotaCofins = cargaPedido.AliquotaCofins;

            if (!string.IsNullOrWhiteSpace(cargaPedido.ObservacaoRegraICMSCTe))
                regraICMS.ObservacaoCTe = cargaPedido.ObservacaoRegraICMSCTe;
            else
                regraICMS.ObservacaoCTe = "";

            regraICMS.IncluirICMSBC = cargaPedido.IncluirICMSBaseCalculo;
            regraICMS.ValorBaseCalculoICMS = cargaPedido.BaseCalculoICMS;
            regraICMS.ValorBaseCalculoPISCOFINS = cargaPedido.BaseCalculoICMS;
            regraICMS.DescontarICMSDoValorAReceber = cargaPedido.CST == "60" && (cargaPedido.ValorFrete + cargaPedido.ValorICMS == cargaPedido.ValorPrestacaoServico);
            regraICMS.CodigoRegra = cargaPedido.RegraICMS?.Codigo ?? 0;

            return regraICMS;
        }

        private Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS BuscarRegraICMS(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Localidade inicioPrestacao, Dominio.Entidades.Localidade fimPrestacao, Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS();

            regraICMS.Aliquota = pedidoCTeParaSubContratacao.PercentualAliquota;
            regraICMS.AliquotaInternaDifal = pedidoCTeParaSubContratacao.PercentualAliquotaInternaDifal;

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

            if (pedidoCTeParaSubContratacao.CST != null)
                regraICMS.CST = pedidoCTeParaSubContratacao.CST;
            else
                regraICMS.CST = "40";

            if (regraICMS.CST == "40" || regraICMS.CST == "")
                regraICMS.Aliquota = 0;

            regraICMS.PercentualReducaoBC = pedidoCTeParaSubContratacao.PercentualReducaoBC;
            regraICMS.PercentualInclusaoBC = pedidoCTeParaSubContratacao.PercentualIncluirBaseCalculo;
            regraICMS.ValorICMS = pedidoCTeParaSubContratacao.ValorICMS;
            regraICMS.ValorICMSIncluso = pedidoCTeParaSubContratacao.ValorICMSIncluso;
            regraICMS.AliquotaPis = pedidoCTeParaSubContratacao.AliquotaPis;
            regraICMS.AliquotaCofins = pedidoCTeParaSubContratacao.AliquotaCofins;
            regraICMS.ValorPis = pedidoCTeParaSubContratacao.ValorPis;
            regraICMS.ValorCofins = pedidoCTeParaSubContratacao.ValorCofins;
            regraICMS.NaoImprimirImpostosDACTE = pedidoCTeParaSubContratacao.CargaPedido.NaoImprimirImpostosDACTE;
            regraICMS.NaoEnviarImpostoICMSNaEmissaoCte = pedidoCTeParaSubContratacao.CargaPedido.NaoEnviarImpostoICMSNaEmissaoCte;

            if (!string.IsNullOrWhiteSpace(pedidoCTeParaSubContratacao.ObservacaoRegraICMSCTe))
                regraICMS.ObservacaoCTe = pedidoCTeParaSubContratacao.ObservacaoRegraICMSCTe;
            else
                regraICMS.ObservacaoCTe = "";

            regraICMS.IncluirICMSBC = pedidoCTeParaSubContratacao.IncluirICMSBaseCalculo;
            regraICMS.ValorBaseCalculoICMS = pedidoCTeParaSubContratacao.BaseCalculoICMS;
            regraICMS.ValorBaseCalculoPISCOFINS = pedidoCTeParaSubContratacao.BaseCalculoICMS;
            regraICMS.DescontarICMSDoValorAReceber = pedidoCTeParaSubContratacao.CTeTerceiro != null && pedidoCTeParaSubContratacao.CTeTerceiro.CST == "60" ? (pedidoCTeParaSubContratacao.CTeTerceiro.ValorAReceber + pedidoCTeParaSubContratacao.CTeTerceiro.ValorICMS == pedidoCTeParaSubContratacao.CTeTerceiro.ValorPrestacaoServico) : false;
            regraICMS.CodigoRegra = pedidoCTeParaSubContratacao.RegraICMS?.Codigo ?? 0;

            return regraICMS;
        }

        private Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS BuscarRegraISS(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao, Dominio.Entidades.Localidade localidadePrestacao, Dominio.Entidades.Cliente tomador, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = new Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS();

            if (pedidoCTeParaSubContratacao.PossuiNFS)
            {
                Servicos.Embarcador.Carga.ISS svcISS = new Servicos.Embarcador.Carga.ISS(unitOfWork);
                regraISS = svcISS.BuscarRegraISS(carga.Empresa, pedidoCTeParaSubContratacao.ValorFrete, localidadePrestacao, carga.TipoOperacao, tomador, null, carga?.TipoDeCarga?.NBS ?? "", unitOfWork);
            }
            else
            {
                regraISS.AliquotaISS = 0;
                regraISS.IncluirISSBaseCalculo = false;
                regraISS.PercentualRetencaoISS = 0;
                regraISS.ValorRetencaoISS = 0;

                regraISS.ReterIR = false;
                regraISS.AliquotaIR = 0;
                regraISS.BaseCalculoIR = 0;
                regraISS.ValorIR = 0;
            }

            return regraISS;
        }

        private Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS BuscarRegraISSAgrupada(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoCTesParaSubContratacao, Dominio.Entidades.Localidade localidadePrestacao, Dominio.Entidades.Cliente tomador, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubcontratacao = pedidoCTesParaSubContratacao.FirstOrDefault();

            Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = new Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS();

            if (pedidoCTeParaSubcontratacao.PossuiNFS)
            {
                Servicos.Embarcador.Carga.ISS svcISS = new Servicos.Embarcador.Carga.ISS(unitOfWork);
                regraISS = svcISS.BuscarRegraISS(carga.Empresa, pedidoCTesParaSubContratacao.Sum(o => o.ValorFrete), localidadePrestacao, carga.TipoOperacao, tomador, null, carga?.TipoDeCarga?.NBS ?? "", unitOfWork);
            }
            else
            {
                regraISS.AliquotaISS = 0;
                regraISS.IncluirISSBaseCalculo = false;
                regraISS.PercentualRetencaoISS = 0;
                regraISS.ValorRetencaoISS = 0;

                regraISS.ReterIR = false;
                regraISS.AliquotaIR = 0;
                regraISS.BaseCalculoIR = 0;
                regraISS.ValorIR = 0;
            }

            return regraISS;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> BuscarPedidoCTeSubcontratacaoComponentesFrete(Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete repPedidoCteParaSubContratacaoComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete(unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> cargaPedidoComponentesFretesCliente = new List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico>();
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete> pedidoCteParaSubContratacaoComponentesFrete = repPedidoCteParaSubContratacaoComponenteFrete.BuscarPorPedidoCTeParaSubcontratacao(pedidoCTeParaSubContratacao.Codigo, modeloDocumento);

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete pedidoCteParaSubContratacaoComponenteFrete in pedidoCteParaSubContratacaoComponentesFrete)
            {
                int index = cargaPedidoComponentesFretesCliente.FindIndex(obj => obj.TipoComponenteFrete == pedidoCteParaSubContratacaoComponenteFrete.TipoComponenteFrete && pedidoCteParaSubContratacaoComponenteFrete.ComponenteFrete?.Codigo == obj.ComponenteFrete?.Codigo);

                if (index != -1)
                {
                    Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico componenteFrete = cargaPedidoComponentesFretesCliente[index];

                    componenteFrete.ValorComponente += pedidoCteParaSubContratacaoComponenteFrete.ValorComponente;
                    componenteFrete.ValorTotalMoeda += pedidoCteParaSubContratacaoComponenteFrete.ValorTotalMoeda ?? 0m;

                    cargaPedidoComponentesFretesCliente[index] = componenteFrete;
                }
                else
                {
                    cargaPedidoComponentesFretesCliente.Add(pedidoCteParaSubContratacaoComponenteFrete.ConvertarParaComponenteDinamico());
                }
            }

            return cargaPedidoComponentesFretesCliente;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> BuscarPedidosCTeSubcontratacaoComponentesFrete(List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidosCTeParaSubContratacao, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete repPedidoCteParaSubContratacaoComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete(unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> cargaPedidoComponentesFretesCliente = new List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico>();
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete> pedidoCteParaSubContratacaoComponentesFrete = repPedidoCteParaSubContratacaoComponenteFrete.BuscarPorPedidosCTeParaSubcontratacao((from obj in pedidosCTeParaSubContratacao select obj.Codigo).ToList(), modeloDocumento);

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete pedidoCteParaSubContratacaoComponenteFrete in pedidoCteParaSubContratacaoComponentesFrete)
            {
                int index = cargaPedidoComponentesFretesCliente.FindIndex(obj => obj.TipoComponenteFrete == pedidoCteParaSubContratacaoComponenteFrete.TipoComponenteFrete && pedidoCteParaSubContratacaoComponenteFrete.ComponenteFrete?.Codigo == obj.ComponenteFrete?.Codigo);

                if (index != -1)
                {
                    Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico componenteFrete = cargaPedidoComponentesFretesCliente[index];

                    componenteFrete.ValorComponente += pedidoCteParaSubContratacaoComponenteFrete.ValorComponente;
                    componenteFrete.ValorTotalMoeda += pedidoCteParaSubContratacaoComponenteFrete.ValorTotalMoeda ?? 0m;

                    cargaPedidoComponentesFretesCliente[index] = componenteFrete;
                }
                else
                {
                    cargaPedidoComponentesFretesCliente.Add(pedidoCteParaSubContratacaoComponenteFrete.ConvertarParaComponenteDinamico());
                }
            }

            return cargaPedidoComponentesFretesCliente;
        }

        #endregion
    }
}
