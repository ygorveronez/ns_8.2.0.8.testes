using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga
{
    public class CTePorPedido : ServicoBase
    {
        public CTePorPedido(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #region Métodos Públicos

        public void GerarOutrosDocumentosPorPedido(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Cliente expedidor, Dominio.Entidades.Cliente recebedor, Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoOrigem, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoDestino, Dominio.Enumeradores.TipoPagamento tipoPagamento, Dominio.Enumeradores.TipoTomador tipoTomador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalCarga, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoRecebedor, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao> cargaPedidosContaContabilContabilizacao, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoExpedidor, bool documentoOperacaoContainer = false)
        {
            CTe serCte = new CTe(unitOfWork);
            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            List<Dominio.Entidades.ModeloDocumentoFiscal> modelosDocumentosFiscais = new List<Dominio.Entidades.ModeloDocumentoFiscal>();

            remetente = repCliente.BuscarPorCPFCNPJ(remetente.CPF_CNPJ);

            if (enderecoOrigem != null)
                enderecoOrigem = repPedidoEndereco.BuscarPorCodigo(enderecoOrigem.Codigo);

            destinatario = repCliente.BuscarPorCPFCNPJ(destinatario.CPF_CNPJ);
            recebedor = recebedor != null ? repCliente.BuscarPorCPFCNPJ(recebedor.CPF_CNPJ) : null;
            expedidor = expedidor != null ? repCliente.BuscarPorCPFCNPJ(expedidor.CPF_CNPJ) : null;

            if (enderecoDestino != null)
                enderecoDestino = repPedidoEndereco.BuscarPorCodigo(enderecoDestino.Codigo);

            cargaPedidos = repCargaPedido.BuscarPorCodigo(cargaPedidos.Select(o => o.Codigo).ToArray());
            carga = repCarga.BuscarPorCodigo(carga.Codigo);
            origem = repLocalidade.BuscarPorCodigo(origem.Codigo);
            destino = repLocalidade.BuscarPorCodigo(destino.Codigo);

            if (tomador != null)
                tomador = repCliente.BuscarPorCPFCNPJ(tomador.CPF_CNPJ);

            bool emitirCteFilialEmissora = false;
            if (carga.EmpresaFilialEmissora != null && !carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                emitirCteFilialEmissora = true;

            List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> cargaPedidoComponentesFretesOutrosDocumentos = BuscarCargaPedidoComponentesFreteOutrosDocumentos(cargaPedidos, modeloDocumentoFiscalCarga, unitOfWork, emitirCteFilialEmissora);

            if (cargaPedidoComponentesFretesOutrosDocumentos.Count > 0)
                modelosDocumentosFiscais = (from obj in cargaPedidoComponentesFretesOutrosDocumentos select obj.ModeloDocumentoFiscal).Distinct().ToList();

            if (carga.EmitindoCRT)
                modelosDocumentosFiscais.Add(modeloDocumentoFiscalCarga);

            if (carga.RealizandoOperacaoContainer)
                modelosDocumentosFiscais.Add(modeloDocumentoFiscalCarga);

            foreach (Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal in modelosDocumentosFiscais)
            {
                unitOfWork.Start();

                List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> cargaPedidoComponentesFretesOutrosDocumentosAgrupados = cargaPedidoComponentesFretesOutrosDocumentos.Where(obj => obj.ModeloDocumentoFiscal.Codigo == modeloDocumentoFiscal.Codigo).ToList();

                Dominio.ObjetosDeValor.CTe.CTe cte = new Dominio.ObjetosDeValor.CTe.CTe();
                cte.Documentos = new List<Dominio.ObjetosDeValor.CTe.Documento>();

                List<string> rotas = new List<string>();

                Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = BuscarRegraICMS(cargaPedidos[0], emitirCteFilialEmissora);
                Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = BuscarRegraISS(cargaPedidos[0]);
                Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = serCargaPedido.ObterRetornoImpostoIBSCBS(cargaPedidos[0], emitirCteFilialEmissora);

                if (configuracaoEmbarcador.Pais == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPais.Exterior && regraICMS.CFOP == 0)
                {
                    Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
                    regraICMS.CFOP = repCFOP.BuscarPrimeiroPorTipo(Dominio.Enumeradores.TipoCFOP.Saida)?.CodigoCFOP ?? 0;
                }

                decimal peso = 0m, metrosCubicos = 0m, pesoCubado = 0m, fatorCubagem = 0m;
                int volumes = 0;

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlNotasFiscais = null;

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);

                    if (pedidoXMLNotasFiscais.Any(o => o.CTes.Count > 0 && o.CTes.Any(obj => obj.CargaCTe.CTe.ModeloDocumentoFiscal.Codigo == modeloDocumentoFiscal.Codigo)))
                        continue;

                    xmlNotasFiscais = pedidoXMLNotasFiscais.Select(o => o.XMLNotaFiscal).ToList();

                    if (carga.EmitindoCRT)
                        xmlNotasFiscais = pedidoXMLNotasFiscais.Where(x => x.XMLNotaFiscal.TipoFatura == true).Select(o => o.XMLNotaFiscal).ToList();

                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotasFiscais)
                    {
                        if ((carga?.TipoOperacao?.ConfiguracaoDocumentoEmissao?.PossuiNotaOrdemVenda ?? false) &&
                                    (carga?.TipoOperacao?.ConfiguracaoDocumentoEmissao?.NaoUtilizaNotaVendaObjetoCTE ?? false)
                                    && pedidoXMLNotaFiscal.XMLNotaFiscal.TipoNotaFiscalIntegrada == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscalIntegrada.OrdemVenda)
                        {
                            cargaPedido.Pedido.ObservacaoCTe += "NFe Venda: " + pedidoXMLNotaFiscal.XMLNotaFiscal.Chave;
                            repPedido.Atualizar(cargaPedido.Pedido);

                            Servicos.Log.GravarInfo("9 - Atualizando Obs pedido" + cargaPedido.Pedido.Codigo + " chave: " + pedidoXMLNotaFiscal.XMLNotaFiscal.Chave, "SolicitarEmissaoDocumentosAutorizados");
                            continue;
                        }

                        if (!string.IsNullOrWhiteSpace(pedidoXMLNotaFiscal.XMLNotaFiscal.Rota))
                        {
                            if (!rotas.Contains(pedidoXMLNotaFiscal.XMLNotaFiscal.Rota))
                                rotas.Add(pedidoXMLNotaFiscal.XMLNotaFiscal.Rota);
                        }

                        Dominio.ObjetosDeValor.CTe.Documento docNF = serCte.BuscarDocumentoCTe(pedidoXMLNotaFiscal.XMLNotaFiscal, carga.Empresa.TipoAmbiente, configuracaoEmbarcador);
                        cte.Documentos.Add(docNF);

                        peso += docNF.Peso;
                        volumes += docNF.Volume;

                        metrosCubicos += pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos;
                        pesoCubado += pedidoXMLNotaFiscal.XMLNotaFiscal.PesoCubado;

                        if (pedidoXMLNotaFiscal.XMLNotaFiscal.FatorCubagem > 0m)
                            fatorCubagem = pedidoXMLNotaFiscal.XMLNotaFiscal.FatorCubagem;


                        cte.ValorTotalMercadoria += docNF.Valor;
                        cte.ValorImpostoSuspenso += pedidoXMLNotaFiscal?.XMLNotaFiscal?.ValorImpostoSuspenso ?? 0;
                    }

                    if (rotas.Count == 0)
                    {
                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotas cargaPedidoRota in cargaPedido.CargaPedidoRotas)
                            rotas.Add(cargaPedidoRota.IdenticacaoRota);
                    }

                    if (cargaPedido.Pedido.EmpresaSerie != null)
                        cte.Serie = cargaPedido.Pedido.EmpresaSerie.Numero;

                    if (modeloDocumentoFiscal.GerarISSAutomaticamente)
                    {
                        regraISS.AliquotaISS = cargaPedido.PercentualAliquotaISS;
                        regraISS.ValorISS += cargaPedido.ValorISS;
                        regraISS.ValorBaseCalculoISS += cargaPedido.BaseCalculoISS;
                        regraISS.ValorRetencaoISS += cargaPedido.ValorRetencaoISS;
                    }
                    else
                    {
                        regraISS.ValorISS = 0m;
                        regraISS.ValorBaseCalculoISS = 0m;
                        regraISS.ValorRetencaoISS = 0m;
                        regraISS.AliquotaISS = 0m;
                    }
                }

                decimal valorComponentes = 0;
                if (cargaPedidoComponentesFretesOutrosDocumentosAgrupados.Count > 0)
                {
                    valorComponentes = cargaPedidoComponentesFretesOutrosDocumentosAgrupados.Sum(obj => obj.ValorComponente);
                }

                //cte.ValorAReceber += valorComponentes;
                cte.ValorFrete += valorComponentes;
                //cte.ValorTotalPrestacaoServico += valorComponentes;

                regraICMS.ValorCreditoPresumido = 0m;
                regraICMS.ValorICMS = 0m;
                regraICMS.ValorPis = 0m;
                regraICMS.ValorCofins = 0m;
                regraICMS.ValorICMSIncluso = 0m;
                regraICMS.ValorBaseCalculoICMS = 0m;
                regraICMS.ValorBaseCalculoPISCOFINS = 0m;

                serCargaPedido.PreencherValoresRetornoIBSCBS(impostoIBSCBS, cargaPedidos[0].BaseCalculoIBSCBS, cargaPedidos[0].ValorIBSEstadual, cargaPedidos[0].ValorIBSMunicipal, cargaPedidos[0].ValorCBS);

                if (metrosCubicos == 0 && configuracaoEmbarcador.UsarCubagemPedidoSeNFeSemCubagem)
                    metrosCubicos = cargaPedidos.Sum(obj => obj.Pedido.CubagemTotal);

                List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidades = serCte.RetornaQuantidades(peso, volumes, fatorCubagem, metrosCubicos, pesoCubado, carga?.TipoOperacao?.ConfiguracaoEmissaoDocumento?.RatearPesoModeloVeicularEntreCTes ?? false, carga?.TipoOperacao?.ConfiguracaoEmissaoDocumento?.DescricaoUnidadeMedidaPesoModeloVeicularRateado, serCte.ObterPesoModeloVeicularRateado(cte.Peso, carga, unitOfWork));

                string observacaoCTe = cargaPedidos[0].Pedido.ObservacaoCTe; //como aqui a emissão é agrupada por pedidos, pode ser que o mesmo CT-e use varios pedidos, dessa forma a observação e o número do pedido serão sempre o do primeiro pedido do agrupamento de pedidos para o cte.
                string observacaoCTeTerceiro = cargaPedidos[0].Pedido.ObservacaoCTeTerceiro;

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoObservacaoCTe = cargaPedidos[0].Pedido;

                cte.ProdutoPredominante = serCte.BuscarProdutoPredominante(cargaPedidos, configuracaoEmbarcador, unitOfWork);

                List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> apoliceSeguroAverbacaos = repApoliceSeguroAverbacao.BuscarPorCargaPedido(cargaPedidos.FirstOrDefault().Codigo, emitirCteFilialEmissora);
                List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> seguros = serCte.ConverterSegurosDeTerceirosEmSeguroCTe(apoliceSeguroAverbacaos, tipoTomador, cte.ValorTotalMercadoria);
                List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ctesAnteriores = new List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe>();
                Dominio.Enumeradores.TipoCTE tipoCTe = Dominio.Enumeradores.TipoCTE.Normal;
                Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem = (from obj in cargasOrigem where obj.Codigo == cargaPedidos[0].CargaOrigem.Codigo select obj).FirstOrDefault();
                Dominio.Entidades.Empresa empresa = cargaOrigem.Empresa;
                if (emitirCteFilialEmissora)
                {
                    if (cargaPedidos[0].CargaPedidoProximoTrecho != null || cargaPedidos[0].ProximoTrechoComplementaFilialEmissora)
                    {
                        destino = destinatario.Localidade;
                        recebedor = null;
                    }

                    if (cargaPedidos[0].CargaPedidoTrechoAnterior != null)
                    {
                        origem = remetente.Localidade;
                        expedidor = null;
                    }
                    empresa = cargaOrigem.EmpresaFilialEmissora;
                }

                if (carga?.TipoOperacao?.ConfiguracaoEmissaoDocumento?.EmitirDocumentoSempreOrigemDestinoPedido ?? false)
                {
                    destino = destinatario.Localidade;
                    origem = remetente.Localidade;
                    recebedor = null;
                    expedidor = null;
                }

                if (documentoOperacaoContainer)
                {
                    cte.DocumentoOperacaoContainer = true;
                }

                serCte.ObterDescricoesComponentesPadrao(tomador, carga, configuracaoEmbarcador, unitOfWork, out string descricaoComponenteValorFrete, out string descricaoComponenteValorICMS);

                List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil> configuracoes = Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil.ConverterCargaPedidoContaContabilContabilizacao(cargaPedidos.FirstOrDefault(), cargaPedidosContaContabilContabilizacao);
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTE = serCte.GerarCTe(carga, cargaPedidos[0], cte, empresa, remetente, destinatario, tomador, expedidor, recebedor, origem, destino, enderecoOrigem, enderecoDestino, tipoPagamento, tipoTomador, quantidades, cargaPedidoComponentesFretesOutrosDocumentosAgrupados, observacaoCTe, observacaoCTeTerceiro, pedidoObservacaoCTe, rotas, seguros, regraICMS, regraISS, impostoIBSCBS, tipoServicoMultisoftware, modeloDocumentoFiscal, Dominio.Enumeradores.TipoServico.Normal, tipoCTe, ctesAnteriores, 0, false, null, false, unitOfWork, configuracaoEmbarcador, configuracaoGeralCarga, cargaPedidos.FirstOrDefault().CentroResultado, cargaPedidos.FirstOrDefault().CentroResultadoDestinatario, cargaPedidos.FirstOrDefault().ItemServico, cargaPedidos.FirstOrDefault().CentroResultadoEscrituracao, cargaPedidos.FirstOrDefault().CentroResultadoICMS, cargaPedidos.FirstOrDefault().CentroResultadoPIS, cargaPedidos.FirstOrDefault().CentroResultadoCOFINS, cargaPedidos.FirstOrDefault().ValorMaximoCentroContabilizacao, configuracoes, xmlNotasFiscais, enderecoRecebedor, descricaoComponenteValorICMS, descricaoComponenteValorFrete, enderecoExpedidor, null, cargaPedidos);

                if (documentoOperacaoContainer)
                {
                    if ((carga.TipoOperacao?.ConfiguracaoEmissaoDocumento?.AverbarContainerComAverbacaoCarga ?? false))
                    {
                        serCte.AverbaCargaCTe(cargaCTE, apoliceSeguroAverbacaos, unitOfWork, configuracaoEmbarcador, cargaPedidos.FirstOrDefault()?.Pedido?.FormaAverbacaoCTE ?? Dominio.Enumeradores.FormaAverbacaoCTE.Definitiva);
                    }
                }
                else
                {
                    serCte.AverbaCargaCTe(cargaCTE, apoliceSeguroAverbacaos, unitOfWork, configuracaoEmbarcador, cargaPedidos.FirstOrDefault()?.Pedido?.FormaAverbacaoCTE ?? Dominio.Enumeradores.FormaAverbacaoCTE.Definitiva);
                }

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscais = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotaFiscais)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTE = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe();
                        cargaPedidoXMLNotaFiscalCTE.CargaCTe = cargaCTE;
                        cargaPedidoXMLNotaFiscalCTE.PedidoXMLNotaFiscal = pedidoXMLNotaFiscal;
                        repCargaPedidoXMLNotaFiscalCTe.Inserir(cargaPedidoXMLNotaFiscalCTE);
                    }
                }
                unitOfWork.CommitChanges();
            }

        }

        public void GerarCTePorPedidoIndividual(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Cliente expedidor, Dominio.Entidades.Cliente recebedor, Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoOrigem, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoDestino, Dominio.Enumeradores.TipoPagamento tipoPagamento, Dominio.Enumeradores.TipoTomador tipoTomador, ref List<Dominio.Entidades.NFSe> NFSesParaEmissao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, int totalDocumentos, ref int totalDocumentosGerados, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal, Dominio.Entidades.Embarcador.Documentos.CIOT ciot, bool redespacho, int tipoEnvio, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscaisCarga, List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoXMLNotaFiscalCTe> cargaPedidoXMLNotaFiscalCTeEmitidos, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotas> cargaPedidoRotasCarga, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoRecebedor, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao> cargaPedidosContaContabilContabilizacao, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoExpedidor)
        {
            unitOfWork.FlushAndClear();

            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Hubs.Carga svcHubCarga = new Hubs.Carga();
            Repositorio.Embarcador.Cargas.CargaNFSComponentesFrete repCargaNFSComponentesFrete = new Repositorio.Embarcador.Cargas.CargaNFSComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            CTe serCte = new CTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOTCTe repCIOTCTe = new Repositorio.Embarcador.Documentos.CIOTCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
            Servicos.Embarcador.Carga.NFS serNFS = new NFS(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            if (!cargaPedido.PossuiCTe && !cargaPedido.PossuiNFS && !cargaPedido.PossuiNFSManual)
                return;

            totalDocumentosGerados++;
            Servicos.Log.GravarInfo("2 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTePorPedidoIndividual");
            unitOfWork.Start();

            bool emitirCte = false;

            Dominio.ObjetosDeValor.CTe.CTe cte = new Dominio.ObjetosDeValor.CTe.CTe();
            cte.Documentos = new List<Dominio.ObjetosDeValor.CTe.Documento>();

            decimal peso = 0m, pesoLiquido = 0m, metrosCubicos = 0m, pesoCubado = 0m, fatorCubagem = 0m, percentualPagamentoAgregado = 0m, pallets = 0m, valorTotalMoeda = 0m;
            int volumes = 0;

            carga = repCarga.BuscarPorCodigoFetch(carga.Codigo);
            cargaPedido = repCargaPedido.BuscarPorCodigoComFetch(cargaPedido.Codigo);

            if (cargaPedido.Pedido.TipoOperacao != null && cargaPedido.Pedido.TipoOperacao.ColetaEmProdutorRural && cargaPedido.Pedido.TipoOperacao.RemetenteDoCTeSeraODestinatarioDoPedido)
                remetente = destinatario;

            Servicos.Log.GravarInfo("3 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTePorPedidoIndividual");
            remetente = repCliente.BuscarPorCPFCNPJ(remetente.CPF_CNPJ);
            destinatario = repCliente.BuscarPorCPFCNPJ(destinatario.CPF_CNPJ);
            if (expedidor != null)
                expedidor = repCliente.BuscarPorCPFCNPJ(expedidor.CPF_CNPJ);

            if (recebedor != null)
                recebedor = repCliente.BuscarPorCPFCNPJ(recebedor.CPF_CNPJ);

            if (tomador != null)
                tomador = repCliente.BuscarPorCPFCNPJ(tomador.CPF_CNPJ);

            if (enderecoDestino != null)
                enderecoDestino = repPedidoEndereco.BuscarPorCodigo(enderecoDestino.Codigo);

            if (enderecoOrigem != null)
                enderecoOrigem = repPedidoEndereco.BuscarPorCodigo(enderecoOrigem.Codigo);
            Servicos.Log.GravarInfo("4 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTePorPedidoIndividual");

            bool emitirCteFilialEmissora = false;

            if (cargaPedido.Carga.EmpresaFilialEmissora != null && !cargaPedido.Carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                emitirCteFilialEmissora = true;
            Servicos.Log.GravarInfo("5 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTePorPedidoIndividual");
            List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> cargaPedidoComponentesFretesCliente = serCargaPedido.BuscarCargaPedidoComponentesFrete(cargaPedido, unitOfWork, emitirCteFilialEmissora);

            List<string> rotas = new List<string>();
            Servicos.Log.GravarInfo("6 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTePorPedidoIndividual");
            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = BuscarRegraICMS(cargaPedido, emitirCteFilialEmissora);

            if (configuracaoEmbarcador.Pais == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPais.Exterior && regraICMS.CFOP == 0)
            {
                Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
                regraICMS.CFOP = repCFOP.BuscarPrimeiroPorTipo(Dominio.Enumeradores.TipoCFOP.Saida)?.CodigoCFOP ?? 0;
            }

            Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = BuscarRegraISS(cargaPedido);
            Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = serCargaPedido.ObterRetornoImpostoIBSCBS(cargaPedido, emitirCteFilialEmissora);

            Servicos.Log.GravarInfo("7 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTePorPedidoIndividual");
            Dominio.Enumeradores.TipoServico tipoServico = Dominio.Enumeradores.TipoServico.Normal;

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesAnterioresTodasCargasPedido = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais = (from obj in pedidoXMLNotasFiscaisCarga where obj.CargaPedido.Codigo == cargaPedido.Codigo select obj).ToList(); //repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);
            Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem = (from obj in cargasOrigem where obj.Codigo == cargaPedido.CargaOrigem.Codigo select obj).FirstOrDefault();
            Dominio.Entidades.Empresa empresa = cargaOrigem.Empresa; //repEmpresa.BuscarPorCodigo(cargaPedido.CargaOrigem.Empresa.Codigo);

            if (emitirCteFilialEmissora)
            {
                if (cargaPedido.CargaPedidoProximoTrecho != null || cargaPedido.ProximoTrechoComplementaFilialEmissora)
                {
                    destino = destinatario.Localidade;
                    recebedor = null;
                }

                if (cargaPedido.CargaPedidoTrechoAnterior != null)
                {
                    origem = remetente.Localidade;
                    expedidor = null;
                }

                empresa = cargaOrigem.EmpresaFilialEmissora; //repEmpresa.BuscarPorCodigo(cargaPedido.CargaOrigem.EmpresaFilialEmissora.Codigo);
            }

            if (carga?.TipoOperacao?.ConfiguracaoEmissaoDocumento?.EmitirDocumentoSempreOrigemDestinoPedido ?? false)
            {
                destino = destinatario.Localidade;
                origem = remetente.Localidade;
                recebedor = null;
                expedidor = null;
            }

            if (cargaPedido.NaoConsiderarRecebedorParaEmitirDocumentos)
            {
                destino = destinatario.Localidade;
                recebedor = null;
            }

            Dominio.Enumeradores.TipoAmbiente tipoAmbiente = empresa.TipoAmbiente;

            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoEmitir = null;
            List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> apoliceSeguroAverbacaos = repApoliceSeguroAverbacao.BuscarPorCargaPedido(cargaPedido.Codigo, emitirCteFilialEmissora);


            if (!cargaPedido.CTeEmitidoNoEmbarcador && !cargaPedido.Pedido.PedidoTransbordo)
            {
                if (!cargaPedido.PossuiNFSManual || (origem.Codigo != destino.Codigo && (!cargaPedido.PossuiNFSManual || !(carga.Empresa?.SempreEmitirNFS ?? false))) || (cargaPedido.PossuiNFSManual && cargaPedido.ModeloDocumentoFiscalIntramunicipal != null && (origem.Codigo == destino.Codigo || cargaPedido.DisponibilizarDocumentoNFSManual)))
                {
                    //if (repCargaPedidoXMLNotaFiscalCTe.ContarCTEsPorCargaPedido(cargaPedido.Codigo) > 0)
                    //    return;

                    if ((from obj in cargaPedidoXMLNotaFiscalCTeEmitidos where obj.CargaPedido == cargaPedido.Codigo select obj).Count() > 0)
                        return;

                    emitirCte = true;

                    if (cargaPedido.PossuiNFSManual)
                        modeloDocumentoEmitir = cargaPedido.ModeloDocumentoFiscalIntramunicipal;
                    else
                    {
                        if (modeloDocumentoFiscal == null)
                            modeloDocumentoEmitir = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.CTe);
                        else
                            modeloDocumentoEmitir = modeloDocumentoFiscal;
                    }

                    Servicos.Log.GravarInfo("8 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTePorPedidoIndividual");

                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotasFiscais)
                    {
                        if ((carga?.TipoOperacao?.ConfiguracaoDocumentoEmissao?.PossuiNotaOrdemVenda ?? false) &&
                                    (carga?.TipoOperacao?.ConfiguracaoDocumentoEmissao?.NaoUtilizaNotaVendaObjetoCTE ?? false)
                                    && pedidoXMLNotaFiscal.XMLNotaFiscal.TipoNotaFiscalIntegrada == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscalIntegrada.OrdemVenda)
                        {
                            cargaPedido.Pedido.ObservacaoCTe += "NFe Venda: " + pedidoXMLNotaFiscal.XMLNotaFiscal.Chave;
                            repPedido.Atualizar(cargaPedido.Pedido);

                            Servicos.Log.GravarInfo("9 - Atualizando Obs pedido" + cargaPedido.Pedido.Codigo + " chave: " + pedidoXMLNotaFiscal.XMLNotaFiscal.Chave, "SolicitarEmissaoDocumentosAutorizados");
                            continue;
                        }

                        if (!string.IsNullOrWhiteSpace(pedidoXMLNotaFiscal.XMLNotaFiscal.Rota))
                        {
                            if (!rotas.Contains(pedidoXMLNotaFiscal.XMLNotaFiscal.Rota))
                                rotas.Add(pedidoXMLNotaFiscal.XMLNotaFiscal.Rota);
                        }

                        Dominio.ObjetosDeValor.CTe.Documento docNF = serCte.BuscarDocumentoCTe(pedidoXMLNotaFiscal.XMLNotaFiscal, tipoAmbiente, configuracaoEmbarcador);
                        cte.Documentos.Add(docNF);

                        percentualPagamentoAgregado = pedidoXMLNotaFiscal.PercentualPagamentoAgregado;
                        peso += docNF.Peso;
                        pesoLiquido += pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido;
                        volumes += docNF.Volume;

                        pallets += pedidoXMLNotaFiscal.XMLNotaFiscal.QuantidadePallets;
                        metrosCubicos += pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos;
                        pesoCubado += pedidoXMLNotaFiscal.XMLNotaFiscal.PesoCubado;
                        valorTotalMoeda += pedidoXMLNotaFiscal.ValorTotalMoeda ?? 0m;

                        if (pedidoXMLNotaFiscal.XMLNotaFiscal.FatorCubagem > 0m)
                            fatorCubagem = pedidoXMLNotaFiscal.XMLNotaFiscal.FatorCubagem;

                        cte.ValorTotalMercadoria += docNF.Valor;
                        cte.ValorImpostoSuspenso += pedidoXMLNotaFiscal?.XMLNotaFiscal?.ValorImpostoSuspenso ?? 0;
                    }
                    Servicos.Log.GravarInfo("9 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTePorPedidoIndividual");

                    if (metrosCubicos == 0 && configuracaoEmbarcador.UsarCubagemPedidoSeNFeSemCubagem)
                        metrosCubicos = cargaPedido.Pedido.CubagemTotal;

                    cte.PercentualPagamentoAgregado = percentualPagamentoAgregado;
                    cte.Peso = peso;
                    cte.PesoLiquido = pesoLiquido;
                    cte.Volumes = volumes;
                    cte.MetrosCubicos = metrosCubicos;
                    cte.Pallets = pallets;
                    cte.PesoCubado = pesoCubado;
                    cte.FatorCubagem = fatorCubagem;
                    cte.PesoFaturado = (peso > pesoCubado ? peso : pesoCubado);
                    cte.ValorTotalMoeda = valorTotalMoeda + cargaPedidoComponentesFretesCliente.Sum(o => o.ValorTotalMoeda ?? 0m);
                    cte.Moeda = cargaPedido.Moeda;
                    cte.ValorCotacaoMoeda = cargaPedido.ValorCotacaoMoeda;

                    if (rotas.Count == 0)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotas> cargaPedidoRotas = (from obj in cargaPedidoRotasCarga where obj.CargaPedido.Codigo == cargaPedido.Codigo select obj).ToList();
                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotas cargaPedidoRota in cargaPedidoRotas)
                            rotas.Add(cargaPedidoRota.IdenticacaoRota);
                    }

                    if (cargaPedido.Pedido.EmpresaSerie != null)
                        cte.Serie = cargaPedido.Pedido.EmpresaSerie.Numero;

                    if (!emitirCteFilialEmissora)
                    {
                        if (carga.PossuiComponenteFreteComImpostoIncluso)
                        {
                            cte.ValorAReceber += cargaPedido.ValorFreteComICMSIncluso;
                            cte.ValorFrete += cargaPedido.ValorFreteComICMSIncluso;
                            cte.ValorTotalPrestacaoServico += cargaPedido.ValorFreteComICMSIncluso;

                            regraICMS.IncluirICMSBC = false;
                            regraICMS.PercentualInclusaoBC = 0m;
                            regraICMS.ValorICMS += cargaPedido.ValorFreteComICMSIncluso - cargaPedido.ValorFrete + cargaPedidoComponentesFretesCliente.Sum(o => o.ValorComponenteComICMSIncluso) - cargaPedidoComponentesFretesCliente.Sum(o => o.ValorComponente);
                            regraICMS.ValorICMSIncluso += regraICMS.ValorICMS;
                            regraICMS.ValorBaseCalculoICMS += cargaPedido.ValorFreteComICMSIncluso + cargaPedidoComponentesFretesCliente.Sum(o => o.ValorComponenteComICMSIncluso);
                            regraICMS.ValorBaseCalculoPISCOFINS += cargaPedido.ValorFreteComICMSIncluso - cargaPedido.BaseCalculoICMS;
                        }
                        else
                        {
                            cte.ValorAReceber += cargaPedido.ValorFrete;
                            cte.ValorFrete += cargaPedido.ValorFrete;
                            cte.ValorTotalPrestacaoServico += cargaPedido.ValorFrete;

                            regraICMS.ValorICMS += cargaPedido.ValorICMS;
                            regraICMS.ValorICMSIncluso += cargaPedido.ValorICMSIncluso;
                            regraICMS.ValorBaseCalculoICMS += cargaPedido.BaseCalculoICMS;
                            regraICMS.ValorBaseCalculoPISCOFINS += cargaPedido.BaseCalculoICMS;
                        }

                        regraICMS.ValorCreditoPresumido += cargaPedido.ValorCreditoPresumido;
                        regraICMS.ValorPis += cargaPedido.ValorPis;
                        regraICMS.ValorCofins += cargaPedido.ValorCofins;

                        serCargaPedido.PreencherValoresRetornoIBSCBS(impostoIBSCBS, cargaPedido.BaseCalculoIBSCBS, cargaPedido.ValorIBSEstadual, cargaPedido.ValorIBSMunicipal, cargaPedido.ValorCBS);
                    }
                    else
                    {
                        cte.ValorAReceber += cargaPedido.ValorFreteFilialEmissora;
                        cte.ValorFrete += cargaPedido.ValorFreteFilialEmissora;
                        cte.ValorTotalPrestacaoServico += cargaPedido.ValorFreteFilialEmissora;

                        regraICMS.ValorCreditoPresumido += cargaPedido.ValorCreditoPresumidoFilialEmissora;
                        regraICMS.ValorICMS += cargaPedido.ValorICMSFilialEmissora;
                        regraICMS.ValorICMSIncluso += cargaPedido.ValorICMSFilialEmissora;
                        regraICMS.ValorBaseCalculoICMS += cargaPedido.BaseCalculoICMSFilialEmissora;
                        regraICMS.ValorBaseCalculoPISCOFINS += cargaPedido.BaseCalculoICMSFilialEmissora;

                        serCargaPedido.PreencherValoresRetornoIBSCBS(impostoIBSCBS, cargaPedido.BaseCalculoIBSCBSFilialEmissora, cargaPedido.ValorIBSEstadualFilialEmissora, cargaPedido.ValorIBSMunicipalFilialEmissora, cargaPedido.ValorCBSFilialEmissora);
                    }

                    regraISS.ValorISS += cargaPedido.ValorISS;
                    regraISS.ValorBaseCalculoISS += cargaPedido.BaseCalculoISS;
                    regraISS.ValorRetencaoISS += cargaPedido.ValorRetencaoISS;

                    regraISS.BaseCalculoIR += cargaPedido.BaseCalculoIR;
                    regraISS.AliquotaIR += cargaPedido.AliquotaIR;
                    regraISS.ValorIR += cargaPedido.ValorIR;

                    if (redespacho)
                    {
                        string raizEmpresa = cargaPedido.CargaPedidoTrechoAnterior != null ? Utilidades.String.OnlyNumbers(cargaPedido.CargaPedidoTrechoAnterior.CargaOrigem.Empresa.CNPJ_SemFormato).Remove(8, 6) : "";
                        string raizTomador = "";

                        if (cargaPedido.ObterTomador().Tipo == "J")
                            raizTomador = Utilidades.String.OnlyNumbers(cargaPedido.ObterTomador().CPF_CNPJ_SemFormato).Remove(8, 6);
                        if (raizEmpresa == raizTomador)
                        {
                            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesAnteiroresCargaPedido = repCargaPedidoXMLNotaFiscalCTe.BuscarCTesPorCargaPedido(cargaPedido.CargaPedidoTrechoAnterior?.Codigo ?? 0);
                            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cteAnteriorCargapedido in ctesAnteiroresCargaPedido)
                            {
                                if (!ctesAnterioresTodasCargasPedido.Contains(cteAnteriorCargapedido))
                                    ctesAnterioresTodasCargasPedido.Add(cteAnteriorCargapedido);
                            }
                            if (recebedor != null && expedidor != null)
                                tipoServico = Dominio.Enumeradores.TipoServico.RedIntermediario;
                            else
                                tipoServico = Dominio.Enumeradores.TipoServico.Redespacho;
                        }
                    }
                }
                else
                {
                    if (repCargaDocumentoParaEmissaoNFSManual.ContarPorPedidoXMLNotasFiscais((from obj in pedidoXMLNotasFiscais select obj.Codigo).ToList()) > 0)
                        return;

                    //if (pedidoXMLNotasFiscais.Any(o => o.DocsParaEmissaoNFSManual.Count > 0))
                    //    return;

                    carga.AgNFSManual = true;

                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotasFiscais)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual cargaDocumentoParaEmissaoNFSManual = serNFS.CriarNFPendenteEmissaoManualDeNFS(carga, pedidoXMLNotaFiscal, cargaPedido.ObterTomador(), cargaPedido.Origem, unitOfWork);
                        serNFS.AverbaCargaNFe(cargaDocumentoParaEmissaoNFSManual, apoliceSeguroAverbacaos, unitOfWork, configuracaoEmbarcador, cargaPedido.Pedido.FormaAverbacaoCTE);
                    }
                }
            }
            else
            {
                Servicos.Embarcador.CTe.CTEsImportados serCTEsImportados = new Servicos.Embarcador.CTe.CTEsImportados(unitOfWork);
                serCTEsImportados.VincularCargaCTeEmitidoAnteriormente(carga, cargaPedido, unitOfWork);
            }


            if (cargaPedido.ValorFreteResidual > 0)
            {
                if (repCargaDocumentoParaEmissaoNFSManual.ContarPorPedidoXMLNotasFiscaisResiduais((from obj in pedidoXMLNotasFiscais select obj.Codigo).ToList()) > 0)
                    return;

                serNFS.CriarNFPendenteEmissaoManualDeNFS(carga, pedidoXMLNotasFiscais.FirstOrDefault(), cargaPedido.Pedido.Remetente, cargaPedido.Origem, unitOfWork, cargaPedido.ValorFreteResidual);
            }

            Servicos.Log.GravarInfo("10 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTePorPedidoIndividual");
            if (emitirCte && (cte.ValorFrete > 0m || cargaPedidoComponentesFretesCliente.Sum(obj => obj.ValorComponente) > 0m || (!configuracaoEmbarcador.NaoGerarCTesComValoresZerados && modeloDocumentoEmitir?.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe))) //&& (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || cte.ValorFrete > 0m || cargaPedidoComponentesFretesCliente.Sum(obj => obj.ValorComponente) > 0m))
            {
                List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidades = serCte.RetornaQuantidades(peso, volumes, fatorCubagem, metrosCubicos, pesoCubado, carga?.TipoOperacao?.ConfiguracaoEmissaoDocumento?.RatearPesoModeloVeicularEntreCTes ?? false, carga?.TipoOperacao?.ConfiguracaoEmissaoDocumento?.DescricaoUnidadeMedidaPesoModeloVeicularRateado, serCte.ObterPesoModeloVeicularRateado(cte.Peso, carga, unitOfWork));
                Servicos.Log.GravarInfo("11 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTePorPedidoIndividual");
                cte.ProdutoPredominante = serCte.BuscarProdutoPredominante(cargaPedido, configuracaoEmbarcador, unitOfWork);

                if (ciot != null)
                    cte.CIOT = ciot.Numero;

                Servicos.Log.GravarInfo("12 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTePorPedidoIndividual");


                List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> seguros = serCte.ConverterSegurosDeTerceirosEmSeguroCTe(apoliceSeguroAverbacaos, tipoTomador, cte.ValorTotalMercadoria);
                Servicos.Log.GravarInfo("13 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTePorPedidoIndividual");
                List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ctesAnteriores = serCte.ConverterCTesTerceirosParaAnteriores(ctesAnterioresTodasCargasPedido);
                Servicos.Log.GravarInfo("14 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTePorPedidoIndividual");
                Dominio.Enumeradores.TipoCTE tipoCTe = Dominio.Enumeradores.TipoCTE.Normal;

                serCte.ObterDescricoesComponentesPadrao(cargaPedido.ObterTomador(), carga, configuracaoEmbarcador, unitOfWork, out string descricaoComponenteValorFrete, out string descricaoComponenteValorICMS);
                Servicos.Log.GravarInfo("15 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTePorPedidoIndividual");

                List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil> configuracoes = Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil.ConverterCargaPedidoContaContabilContabilizacao(cargaPedido, cargaPedidosContaContabilContabilizacao);
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTE = serCte.GerarCTe(carga, cargaPedido, cte, empresa, remetente, destinatario, tomador, expedidor, recebedor, origem, destino, enderecoOrigem, enderecoDestino, tipoPagamento, tipoTomador, quantidades, cargaPedidoComponentesFretesCliente, cargaPedido.Pedido.ObservacaoCTe, cargaPedido.Pedido.ObservacaoCTeTerceiro, cargaPedido.Pedido, rotas, seguros, regraICMS, regraISS, impostoIBSCBS, tipoServicoMultisoftware, modeloDocumentoEmitir, tipoServico, tipoCTe, ctesAnteriores, tipoEnvio, false, null, false, unitOfWork, configuracaoEmbarcador, configuracaoGeralCarga, cargaPedido.CentroResultado, cargaPedido.CentroResultadoDestinatario, cargaPedido.ItemServico, cargaPedido.CentroResultadoEscrituracao, cargaPedido.CentroResultadoICMS, cargaPedido.CentroResultadoPIS, cargaPedido.CentroResultadoCOFINS, cargaPedido.ValorMaximoCentroContabilizacao, configuracoes, pedidoXMLNotasFiscais.Select(o => o.XMLNotaFiscal).ToList(), enderecoRecebedor, descricaoComponenteValorICMS, descricaoComponenteValorFrete, enderecoExpedidor);
                Servicos.Log.GravarInfo("16 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTePorPedidoIndividual");
                serCte.AverbaCargaCTe(cargaCTE, apoliceSeguroAverbacaos, unitOfWork, configuracaoEmbarcador, cargaPedido.Pedido.FormaAverbacaoCTE);
                Servicos.Log.GravarInfo("17 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTePorPedidoIndividual");
                if (ciot != null)
                {
                    Dominio.Entidades.Embarcador.Documentos.CIOTCTe ciotCTe = new Dominio.Entidades.Embarcador.Documentos.CIOTCTe()
                    {
                        CargaCTe = cargaCTE,
                        CIOT = ciot
                    };

                    repCIOTCTe.Inserir(ciotCTe);
                }

                Servicos.Log.GravarInfo("18 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTePorPedidoIndividual");
                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotasFiscais)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTE = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe();
                    cargaPedidoXMLNotaFiscalCTE.CargaCTe = cargaCTE;
                    cargaPedidoXMLNotaFiscalCTE.PedidoXMLNotaFiscal = pedidoXMLNotaFiscal;
                    repCargaPedidoXMLNotaFiscalCTe.Inserir(cargaPedidoXMLNotaFiscalCTE);
                }
                Servicos.Log.GravarInfo("19 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTePorPedidoIndividual");

                Servicos.Log.GravarInfo("20 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTePorPedidoIndividual");
                if (cargaPedido.PossuiNFSManual)
                {
                    serNFS.CriarCargaCTePendenteEmissaoManualDeNFS(carga, cargaCTE, cargaPedido.Origem, unitOfWork);

                    carga.AgNFSManual = true;
                }

                Servicos.Log.GravarInfo("21 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTePorPedidoIndividual");
                unitOfWork.CommitChanges();
                Servicos.Log.GravarInfo("22 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTePorPedidoIndividual");

                Servicos.Log.GravarInfo("23 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTePorPedidoIndividual");
                if (cargaCTE.CTe != null && cargaCTE.CTe.Status == "E")
                {
                    if (cargaCTE.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                    {
                        Servicos.CTe servicoCte = new Servicos.CTe(unitOfWork);
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cteIntegrado = cargaCTE.CTe;

                        servicoCte.Emitir(ref cteIntegrado, unitOfWork);
                    }
                    else if (cargaCTE.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                    {
                        Servicos.NFSe servicoNFSe = new Servicos.NFSe(unitOfWork);
                        servicoNFSe.EmitirNFSe(cargaCTE.CTe.Codigo, unitOfWork);
                    }
                }
                Servicos.Log.GravarInfo("24 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTePorPedidoIndividual");
            }

            Servicos.Log.GravarInfo("25 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTePorPedidoIndividual");
            svcHubCarga.InformarQuantidadeDocumentosEmitidos(carga.Codigo, totalDocumentos, totalDocumentosGerados);
            Servicos.Log.GravarInfo("26 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "GerarCTePorPedidoIndividual");
        }

        public void GerarCTePorPedidoAgrupado(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosCarga, Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Cliente expedidor, Dominio.Entidades.Cliente recebedor, Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoOrigem, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoDestino, Dominio.Enumeradores.TipoPagamento tipoPagamento, Dominio.Enumeradores.TipoTomador tipoTomador, ref List<Dominio.Entidades.NFSe> NFSesParaEmissao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, int totalDocumentos, ref int totalDocumentosGerados, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal, Dominio.Entidades.Embarcador.Documentos.CIOT ciot, bool redespacho, int tipoEnvio, bool indicadorGlobalizadoDestinatario, bool indicadorCteSimplificado, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscaisCarga, List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoXMLNotaFiscalCTe> cargaPedidoXMLNotaFiscalCTeEmitidos, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotas> cargaPedidoRotasCarga, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoRecebedor, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao> cargaPedidosContaContabilContabilizacao, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoExpedidor, bool usaClienteOutroEnderecoDestino, bool usaClienteOutroEnderecoOrigem, bool ajusteSiniefNro8 = false)
        {
            unitOfWork.FlushAndClear();

            Hubs.Carga svcHubCarga = new Hubs.Carga();

            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            carga = repCarga.BuscarPorCodigoFetch(carga.Codigo);
            CTe serCte = new CTe(unitOfWork);

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOTCTe repCIOTCTe = new Repositorio.Embarcador.Documentos.CIOTCTe(unitOfWork);
            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
            Servicos.Embarcador.Carga.NFS serNFS = new NFS(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            List<int> codigosCargaPedido = (from obj in cargaPedidosCarga orderby obj.PedidoSemNFe select obj.Codigo).ToList();
            Dominio.Entidades.Embarcador.Cargas.CargaPedido primeiraCargaPedido = repCargaPedido.BuscarPorCodigoComFetch(codigosCargaPedido.FirstOrDefault());

            if (!primeiraCargaPedido.PossuiCTe && !primeiraCargaPedido.PossuiNFS && !primeiraCargaPedido.PossuiNFSManual)
                return;

            if (primeiraCargaPedido.Pedido.TipoOperacao != null && primeiraCargaPedido.Pedido.TipoOperacao.ColetaEmProdutorRural && primeiraCargaPedido.Pedido.TipoOperacao.RemetenteDoCTeSeraODestinatarioDoPedido)
                remetente = destinatario;

            totalDocumentosGerados++;

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            remetente = repCliente.BuscarPorCPFCNPJ(remetente.CPF_CNPJ);
            destinatario = repCliente.BuscarPorCPFCNPJ(destinatario.CPF_CNPJ);

            if (expedidor != null)
                expedidor = repCliente.BuscarPorCPFCNPJ(expedidor.CPF_CNPJ);

            if (recebedor != null)
                recebedor = repCliente.BuscarPorCPFCNPJ(recebedor.CPF_CNPJ);

            if (tomador != null)
                tomador = repCliente.BuscarPorCPFCNPJ(tomador.CPF_CNPJ);

            if (enderecoDestino != null)
                enderecoDestino = repPedidoEndereco.BuscarPorCodigo(enderecoDestino.Codigo);

            if (enderecoOrigem != null)
                enderecoOrigem = repPedidoEndereco.BuscarPorCodigo(enderecoOrigem.Codigo);

            unitOfWork.Start();

            bool emitirCte = false;
            bool pedidoComNotaCobertura = !string.IsNullOrWhiteSpace(primeiraCargaPedido.Pedido.NumeroControle) && tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS;
            Dominio.Enumeradores.TipoCTE tipoCte = indicadorCteSimplificado ? Dominio.Enumeradores.TipoCTE.Simplificado : Dominio.Enumeradores.TipoCTE.Normal;

            Dominio.ObjetosDeValor.CTe.CTe cte = new Dominio.ObjetosDeValor.CTe.CTe();
            cte.indicadorGlobalizado = indicadorGlobalizadoDestinatario && !pedidoComNotaCobertura ? Dominio.Enumeradores.OpcaoSimNao.Sim : Dominio.Enumeradores.OpcaoSimNao.Nao;

            cte.Documentos = new List<Dominio.ObjetosDeValor.CTe.Documento>();
            cte.Entregas = new List<Dominio.ObjetosDeValor.CTe.Entrega>();

            decimal peso = 0m, pesoLiquido = 0m, metrosCubicos = 0m, pesoCubado = 0m, fatorCubagem = 0m, percentualPagamentoAgregado = 0m, pallets = 0m, valorTotalMoeda = 0m;
            int volumes = 0;
            bool emitirCteFilialEmissora = false;
            if (primeiraCargaPedido.Carga.EmpresaFilialEmissora != null && !primeiraCargaPedido.Carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                emitirCteFilialEmissora = true;

            List<string> rotas = new List<string>();

            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = BuscarRegraICMS(primeiraCargaPedido, emitirCteFilialEmissora);

            Servicos.Log.GravarInfo($"_________| Carga: {carga.Codigo}, CargaPedido: {primeiraCargaPedido.Codigo}, Pedido {primeiraCargaPedido.Pedido.Codigo} |_________", "ProcessarAliquota");
            Servicos.Log.GravarInfo($"CargaPedido: {primeiraCargaPedido.Codigo} -> Aliquota: {regraICMS.Aliquota}", "ProcessarAliquota");

            if (configuracaoEmbarcador.Pais == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPais.Exterior && regraICMS.CFOP == 0)
            {
                Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
                regraICMS.CFOP = repCFOP.BuscarPrimeiroPorTipo(Dominio.Enumeradores.TipoCFOP.Saida)?.CodigoCFOP ?? 0;
            }

            Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = BuscarRegraISS(primeiraCargaPedido);
            Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = serCargaPedido.ObterRetornoImpostoIBSCBS(primeiraCargaPedido, emitirCteFilialEmissora);

            Dominio.Enumeradores.TipoServico tipoServico = Dominio.Enumeradores.TipoServico.Normal;

            Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem = (from obj in cargasOrigem where obj.Codigo == primeiraCargaPedido.CargaOrigem.Codigo select obj).FirstOrDefault();
            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(cargaOrigem.Empresa.Codigo);

            if (emitirCteFilialEmissora)
            {
                if (primeiraCargaPedido.CargaPedidoProximoTrecho != null)
                {
                    destino = destinatario.Localidade;
                    recebedor = null;
                }

                if (primeiraCargaPedido.CargaPedidoTrechoAnterior != null)
                {
                    origem = remetente.Localidade;
                    expedidor = null;
                }

                empresa = cargaOrigem.EmpresaFilialEmissora;
            }

            if ((carga?.TipoOperacao?.ConfiguracaoEmissaoDocumento?.EmitirDocumentoSempreOrigemDestinoPedido ?? false) || (carga?.TipoOperacao?.TipoConsolidacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTipoConsolidacao.PreCheckIn))
            {
                if (!usaClienteOutroEnderecoDestino || (carga?.TipoOperacao?.TipoConsolidacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTipoConsolidacao.PreCheckIn))
                    destino = destinatario.Localidade;
                if (!usaClienteOutroEnderecoOrigem || (carga?.TipoOperacao?.TipoConsolidacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTipoConsolidacao.PreCheckIn))
                    origem = remetente.Localidade;

                recebedor = null;
                expedidor = null;
            }

            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoEmitir = null;

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesAnterioresTodasCargasPedido = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlNotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            if (codigosCargaPedido.Count < 2500)
                cargaPedidos = repCargaPedido.BuscarPorCodigoComFetch(codigosCargaPedido);
            else
            {
                Servicos.Log.GravarInfo("Mais de 2500 pedidos agrupados na carga " + carga.Codigo, "Mais2500Pedido");
                foreach (int codigo in codigosCargaPedido)
                    cargaPedidos.Add(repCargaPedido.BuscarPorCodigoComFetch(codigo));
            }

            List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> apoliceSeguroAverbacaos = repApoliceSeguroAverbacao.BuscarPorCargaPedido(primeiraCargaPedido.Codigo, emitirCteFilialEmissora);
            List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> cargaPedidoComponentesFretesCliente = serCargaPedido.BuscarCargaPedidoComponentesFrete(cargaPedidos, emitirCteFilialEmissora, unitOfWork);
            string observacaoCteAdicional = string.Empty;

            bool cargaInternacional = carga.TipoOperacao?.ConfiguracaoCarga?.TipoOperacaoInternacional ?? false;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                if (!cargaPedido.CTeEmitidoNoEmbarcador && !cargaPedido.Pedido.PedidoTransbordo)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais = (from obj in pedidoXMLNotasFiscaisCarga where obj.CargaPedido.Codigo == cargaPedido.Codigo select obj).ToList(); //repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);

                    if (!cargaPedido.PossuiNFSManual || (origem.Codigo != destino.Codigo && (!cargaPedido.PossuiNFSManual || !(carga.Empresa?.SempreEmitirNFS ?? false))) || (cargaPedido.PossuiNFSManual && cargaPedido.ModeloDocumentoFiscalIntramunicipal != null && (origem.Codigo == destino.Codigo || cargaPedido.DisponibilizarDocumentoNFSManual)))
                    {
                        //if ( repCargaPedidoXMLNotaFiscalCTe.ContarCTEsPorCargaPedido(cargaPedido.Codigo) > 0)
                        //    return;
                        if ((from obj in cargaPedidoXMLNotaFiscalCTeEmitidos where obj.CargaPedido == cargaPedido.Codigo select obj).Count() > 0)
                            return;

                        emitirCte = true;

                        //xmlNotasFiscais.AddRange(pedidoXMLNotasFiscais.Select(o => o.XMLNotaFiscal).ToList());

                        if (cargaPedido.PossuiNFSManual)
                            modeloDocumentoEmitir = cargaPedido.ModeloDocumentoFiscalIntramunicipal;
                        else
                            modeloDocumentoEmitir = modeloDocumentoFiscal;

                        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotasFiscais)
                        {
                            if (!xmlNotasFiscais.Contains(pedidoXMLNotaFiscal.XMLNotaFiscal))
                            {
                                Servicos.Log.GravarInfo("5 - Validando Nota de venda", "SolicitarEmissaoDocumentosAutorizados");

                                if ((carga?.TipoOperacao?.ConfiguracaoDocumentoEmissao?.PossuiNotaOrdemVenda ?? false) &&
                                    (carga?.TipoOperacao?.ConfiguracaoDocumentoEmissao?.NaoUtilizaNotaVendaObjetoCTE ?? false)
                                    && pedidoXMLNotaFiscal.XMLNotaFiscal.TipoNotaFiscalIntegrada == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscalIntegrada.OrdemVenda)
                                {
                                    cargaPedido.Pedido.ObservacaoCTe += "NFe Venda: " + pedidoXMLNotaFiscal.XMLNotaFiscal.Chave;
                                    repPedido.Atualizar(cargaPedido.Pedido);

                                    Servicos.Log.GravarInfo("7 - Atualizando Obs pedido" + cargaPedido.Pedido.Codigo + " chave: " + pedidoXMLNotaFiscal.XMLNotaFiscal.Chave, "SolicitarEmissaoDocumentosAutorizados");
                                    continue;
                                }

                                if ((carga?.TipoOperacao?.ConfiguracaoDocumentoEmissao?.PossuiNotaOrdemVenda ?? false) &&
                                    (carga?.TipoOperacao?.ConfiguracaoDocumentoEmissao?.EmitirCTENotaRemessa ?? false)
                                    && pedidoXMLNotaFiscal.XMLNotaFiscal.TipoNotaFiscalIntegrada != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscalIntegrada.RemessaVenda)
                                {
                                    cargaPedido.Pedido.ObservacaoCTe += "NFe Venda: " + pedidoXMLNotaFiscal.XMLNotaFiscal.Chave;
                                    repPedido.Atualizar(cargaPedido.Pedido);

                                    Servicos.Log.GravarInfo("7 - Atualizando Obs pedido" + cargaPedido.Pedido.Codigo + " chave: " + pedidoXMLNotaFiscal.XMLNotaFiscal.Chave, "SolicitarEmissaoDocumentosAutorizados");
                                    continue;
                                }

                                if ((carga?.TipoOperacao?.ConfiguracaoEmissaoDocumento?.ClassificacaoNFeRemessaVenda ?? false)
                                    && (((carga.TipoOperacao.ConfiguracaoEmissaoDocumento?.EnviarParaObservacaoCTeNFeRemessa ?? false)
                                       && pedidoXMLNotaFiscal.XMLNotaFiscal.ClassificacaoNFe == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClassificacaoNFe.Remessa)
                                    || ((carga.TipoOperacao.ConfiguracaoEmissaoDocumento?.EnviarParaObservacaoCTeNFeVenda ?? false)
                                       && pedidoXMLNotaFiscal.XMLNotaFiscal.ClassificacaoNFe == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClassificacaoNFe.Venda)))
                                {
                                    observacaoCteAdicional = $" NFe {Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClassificacaoNFeHelper.ObterDescricao(pedidoXMLNotaFiscal.XMLNotaFiscal.ClassificacaoNFe.Value)}: {pedidoXMLNotaFiscal.XMLNotaFiscal.Chave}";
                                    continue;
                                }

                                if (cargaInternacional && modeloDocumentoFiscal.DocumentoTipoCRT && !pedidoXMLNotaFiscal.XMLNotaFiscal.TipoFatura)
                                    continue;

                                xmlNotasFiscais.Add(pedidoXMLNotaFiscal.XMLNotaFiscal);
                                if (!string.IsNullOrWhiteSpace(pedidoXMLNotaFiscal.XMLNotaFiscal.Rota))
                                {
                                    if (!rotas.Contains(pedidoXMLNotaFiscal.XMLNotaFiscal.Rota))
                                        rotas.Add(pedidoXMLNotaFiscal.XMLNotaFiscal.Rota);
                                }

                                Dominio.ObjetosDeValor.CTe.Documento docNF = serCte.BuscarDocumentoCTe(pedidoXMLNotaFiscal.XMLNotaFiscal, empresa.TipoAmbiente, configuracaoEmbarcador);
                                cte.Documentos.Add(docNF);

                                if (indicadorCteSimplificado)
                                    serCte.AdicionarAtualizarEntregaCTe(ref cte, docNF, pedidoXMLNotaFiscal, carga.PossuiComponenteFreteComImpostoIncluso, emitirCteFilialEmissora, enderecoOrigem, enderecoDestino, ajusteSiniefNro8, unitOfWork);

                                percentualPagamentoAgregado = pedidoXMLNotaFiscal.PercentualPagamentoAgregado;
                                pesoLiquido += pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido;
                                peso += docNF.Peso;
                                volumes += docNF.Volume;

                                pallets = pedidoXMLNotaFiscal.XMLNotaFiscal.QuantidadePallets;
                                metrosCubicos += pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos;
                                pesoCubado += pedidoXMLNotaFiscal.XMLNotaFiscal.PesoCubado;
                                valorTotalMoeda += pedidoXMLNotaFiscal.ValorTotalMoeda ?? 0m;

                                if (pedidoXMLNotaFiscal.XMLNotaFiscal.FatorCubagem > 0m)
                                    fatorCubagem = pedidoXMLNotaFiscal.XMLNotaFiscal.FatorCubagem;

                                cte.ValorTotalMercadoria += docNF.Valor;
                                cte.ValorImpostoSuspenso += pedidoXMLNotaFiscal?.XMLNotaFiscal?.ValorImpostoSuspenso ?? 0;
                            }
                        }

                        if (rotas.Count == 0)
                        {
                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotas> cargaPedidoRotas = (from obj in cargaPedidoRotasCarga where obj.CargaPedido.Codigo == cargaPedido.Codigo select obj).ToList();
                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotas cargaPedidoRota in cargaPedidoRotas)
                                rotas.Add(cargaPedidoRota.IdenticacaoRota);
                        }

                        if (cargaPedido.Pedido.EmpresaSerie != null)
                            cte.Serie = cargaPedido.Pedido.EmpresaSerie.Numero;

                        if (emitirCteFilialEmissora)
                        {
                            cte.ValorAReceber += cargaPedido.ValorFreteFilialEmissora;
                            cte.ValorFrete += cargaPedido.ValorFreteFilialEmissora;
                            cte.ValorTotalPrestacaoServico += cargaPedido.ValorFreteFilialEmissora;

                            regraICMS.ValorICMS += cargaPedido.ValorICMSFilialEmissora;
                            regraICMS.ValorICMSIncluso += cargaPedido.ValorICMSFilialEmissora;
                            regraICMS.ValorBaseCalculoICMS += cargaPedido.BaseCalculoICMSFilialEmissora;
                            regraICMS.ValorBaseCalculoPISCOFINS += cargaPedido.BaseCalculoICMSFilialEmissora;
                            regraICMS.ValorCreditoPresumido += cargaPedido.ValorCreditoPresumidoFilialEmissora;

                            serCargaPedido.PreencherValoresRetornoIBSCBS(impostoIBSCBS, cargaPedido.BaseCalculoIBSCBSFilialEmissora, cargaPedido.ValorIBSEstadualFilialEmissora, cargaPedido.ValorIBSMunicipalFilialEmissora, cargaPedido.ValorCBSFilialEmissora);
                        }
                        else
                        {
                            if (carga.PossuiComponenteFreteComImpostoIncluso)
                            {
                                cte.ValorAReceber += cargaPedido.ValorFreteComICMSIncluso;
                                cte.ValorFrete += cargaPedido.ValorFreteComICMSIncluso;
                                cte.ValorTotalPrestacaoServico += cargaPedido.ValorFreteComICMSIncluso;

                                regraICMS.IncluirICMSBC = false;
                                regraICMS.PercentualInclusaoBC = 0m;
                                regraICMS.ValorICMS += cargaPedido.ValorFreteComICMSIncluso - cargaPedido.ValorFrete + cargaPedidoComponentesFretesCliente.Sum(o => o.ValorComponenteComICMSIncluso) - cargaPedidoComponentesFretesCliente.Sum(o => o.ValorComponente);
                                regraICMS.ValorICMSIncluso += regraICMS.ValorICMS;
                                regraICMS.ValorBaseCalculoICMS += cargaPedido.ValorFreteComICMSIncluso + cargaPedidoComponentesFretesCliente.Sum(o => o.ValorComponenteComICMSIncluso);
                                regraICMS.ValorBaseCalculoPISCOFINS += cargaPedido.ValorFreteComICMSIncluso - cargaPedido.BaseCalculoICMS;
                            }
                            else
                            {
                                cte.ValorAReceber += cargaPedido.ValorFrete;
                                cte.ValorFrete += cargaPedido.ValorFrete;
                                cte.ValorTotalPrestacaoServico += cargaPedido.ValorFrete;

                                regraICMS.ValorICMS += cargaPedido.ValorICMS;
                                regraICMS.ValorICMSIncluso += cargaPedido.ValorICMSIncluso;
                                regraICMS.ValorBaseCalculoICMS += cargaPedido.BaseCalculoICMS;
                                regraICMS.ValorBaseCalculoPISCOFINS += cargaPedido.BaseCalculoICMS;
                            }

                            regraICMS.ValorPis += cargaPedido.ValorPis;
                            regraICMS.ValorCofins += cargaPedido.ValorCofins;
                            regraICMS.ValorCreditoPresumido += cargaPedido.ValorCreditoPresumido;

                            serCargaPedido.PreencherValoresRetornoIBSCBS(impostoIBSCBS, cargaPedido.BaseCalculoIBSCBS, cargaPedido.ValorIBSEstadual, cargaPedido.ValorIBSMunicipal, cargaPedido.ValorCBS);
                        }

                        regraISS.ValorISS += cargaPedido.ValorISS;
                        regraISS.ValorBaseCalculoISS += cargaPedido.BaseCalculoISS;
                        regraISS.ValorRetencaoISS += cargaPedido.ValorRetencaoISS;

                        regraISS.BaseCalculoIR += cargaPedido.BaseCalculoIR;
                        regraISS.AliquotaIR += cargaPedido.AliquotaIR;
                        regraISS.ValorIR += cargaPedido.ValorIR;

                        if (redespacho)
                        {
                            string raizEmpresa = ""; //Utilidades.String.OnlyNumbers(cargaPedido.CargaPedidoTrechoAnterior.CargaOrigem.Empresa.CNPJ_SemFormato).Remove(8, 6);
                            string raizTomador = "";

                            if (cargaPedido.ObterTomador().Tipo == "J")
                                raizTomador = Utilidades.String.OnlyNumbers(cargaPedido.ObterTomador().CPF_CNPJ_SemFormato).Remove(8, 6);
                            if (raizEmpresa == raizTomador)
                            {
                                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesAnteiroresCargaPedido = repCargaPedidoXMLNotaFiscalCTe.BuscarCTesPorCargaPedido(cargaPedido.CargaPedidoTrechoAnterior?.Codigo ?? 0);
                                foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cteAnteriorCargapedido in ctesAnteiroresCargaPedido)
                                {
                                    if (!ctesAnterioresTodasCargasPedido.Contains(cteAnteriorCargapedido))
                                        ctesAnterioresTodasCargasPedido.Add(cteAnteriorCargapedido);
                                }
                                if (recebedor != null && expedidor != null)
                                    tipoServico = Dominio.Enumeradores.TipoServico.RedIntermediario;
                                else
                                    tipoServico = Dominio.Enumeradores.TipoServico.Redespacho;
                            }
                        }
                    }
                    else
                    {
                        if (repCargaDocumentoParaEmissaoNFSManual.ContarPorPedidoXMLNotasFiscais((from obj in pedidoXMLNotasFiscais select obj.Codigo).ToList()) > 0)
                            return;

                        //if (pedidoXMLNotasFiscais.Any(o => o.DocsParaEmissaoNFSManual.Count > 0))
                        //    continue;

                        carga.AgNFSManual = true;

                        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotasFiscais)
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual cargaDocumentoParaEmissaoNFSManual = serNFS.CriarNFPendenteEmissaoManualDeNFS(carga, pedidoXMLNotaFiscal, cargaPedido.ObterTomador(), cargaPedido.Origem, unitOfWork);
                            serNFS.AverbaCargaNFe(cargaDocumentoParaEmissaoNFSManual, apoliceSeguroAverbacaos, unitOfWork, configuracaoEmbarcador, cargaPedido.Pedido.FormaAverbacaoCTE);
                        }
                    }
                }
                else
                {
                    Servicos.Embarcador.CTe.CTEsImportados serCTEsImportados = new Servicos.Embarcador.CTe.CTEsImportados(unitOfWork);
                    serCTEsImportados.VincularCargaCTeEmitidoAnteriormente(carga, cargaPedido, unitOfWork);
                }
            }

            decimal freteResidual = cargaPedidos.Sum(obj => obj.ValorFreteResidual);
            if (freteResidual > 0)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais = (from obj in pedidoXMLNotasFiscaisCarga where obj.CargaPedido.Codigo == primeiraCargaPedido.Codigo select obj).ToList(); //repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);

                (from obj in pedidoXMLNotasFiscaisCarga where obj.CargaPedido.Codigo == primeiraCargaPedido.Codigo select obj).ToList(); //repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);

                if (repCargaDocumentoParaEmissaoNFSManual.ContarPorPedidoXMLNotasFiscaisResiduais((from obj in pedidoXMLNotasFiscais select obj.Codigo).ToList()) > 0)
                    return;

                serNFS.CriarNFPendenteEmissaoManualDeNFS(carga, pedidoXMLNotasFiscais.FirstOrDefault(), primeiraCargaPedido.Pedido.Remetente, primeiraCargaPedido.Origem, unitOfWork, freteResidual);

            }

            Servicos.Log.GravarInfo("8 - EmitirCte " + carga.Codigo, "SolicitarEmissaoDocumentosAutorizados");

            if (emitirCte && (cte.ValorFrete > 0m || cargaPedidoComponentesFretesCliente.Sum(obj => obj.ValorComponente) > 0m || (!configuracaoEmbarcador.NaoGerarCTesComValoresZerados && modeloDocumentoEmitir?.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe))) //&& (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || cte.ValorFrete > 0m || cargaPedidoComponentesFretesCliente.Sum(obj => obj.ValorComponente) > 0m))
            {

                if (metrosCubicos == 0 && configuracaoEmbarcador.UsarCubagemPedidoSeNFeSemCubagem)
                    metrosCubicos = cargaPedidos.Sum(obj => obj.Pedido.CubagemTotal);

                cte.PercentualPagamentoAgregado = percentualPagamentoAgregado;
                cte.Peso = peso;
                cte.PesoLiquido = pesoLiquido;
                cte.Volumes = volumes;
                cte.MetrosCubicos = metrosCubicos;
                cte.Pallets = pallets;
                cte.PesoCubado = pesoCubado;
                cte.FatorCubagem = fatorCubagem;
                cte.PesoFaturado = (peso > pesoCubado ? peso : pesoCubado);
                cte.ValorTotalMoeda = valorTotalMoeda + cargaPedidoComponentesFretesCliente.Sum(o => o.ValorTotalMoeda ?? 0m);
                cte.Moeda = carga.Moeda;
                cte.ValorCotacaoMoeda = carga.ValorCotacaoMoeda;

                List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidades = serCte.RetornaQuantidades(peso, volumes, fatorCubagem, metrosCubicos, pesoCubado, carga?.TipoOperacao?.ConfiguracaoEmissaoDocumento?.RatearPesoModeloVeicularEntreCTes ?? false, carga?.TipoOperacao?.ConfiguracaoEmissaoDocumento?.DescricaoUnidadeMedidaPesoModeloVeicularRateado, serCte.ObterPesoModeloVeicularRateado(cte.Peso, carga, unitOfWork));

                string observacaoCTe = primeiraCargaPedido.Pedido.ObservacaoCTe; //como aqui a emissão é agrupada por pedidos, pode ser que o mesmo CT-e use varios pedidos, dessa forma a observação e o número do pedido serão sempre o do primeiro pedido do agrupamento de pedidos para o cte.
                string observacaoCTeTerceiro = primeiraCargaPedido.Pedido.ObservacaoCTeTerceiro;
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoObservacaoCTe = primeiraCargaPedido.Pedido;

                cte.ProdutoPredominante = serCte.BuscarProdutoPredominante(cargaPedidos, configuracaoEmbarcador, unitOfWork);

                if (ciot != null)
                    cte.CIOT = ciot.Numero;

                if (!string.IsNullOrEmpty(observacaoCteAdicional))
                    observacaoCTe += observacaoCteAdicional;

                List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> seguros = serCte.ConverterSegurosDeTerceirosEmSeguroCTe(apoliceSeguroAverbacaos, tipoTomador, cte.ValorTotalMercadoria);
                List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ctesAnteriores = serCte.ConverterCTesTerceirosParaAnteriores(ctesAnterioresTodasCargasPedido);

                serCte.ObterDescricoesComponentesPadrao(tomador, carga, configuracaoEmbarcador, unitOfWork, out string descricaoComponenteValorFrete, out string descricaoComponenteValorICMS);

                List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil> configuracoes = Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil.ConverterCargaPedidoContaContabilContabilizacao(primeiraCargaPedido, cargaPedidosContaContabilContabilizacao);

                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTE = serCte.GerarCTe(carga, primeiraCargaPedido, cte, empresa, remetente, destinatario, tomador, expedidor, recebedor, origem, destino, enderecoOrigem, enderecoDestino, tipoPagamento, tipoTomador, quantidades, cargaPedidoComponentesFretesCliente, observacaoCTe, observacaoCTeTerceiro, pedidoObservacaoCTe, rotas, seguros, regraICMS, regraISS, impostoIBSCBS, tipoServicoMultisoftware, modeloDocumentoEmitir, tipoServico, tipoCte, ctesAnteriores, tipoEnvio, false, null, false, unitOfWork, configuracaoEmbarcador, configuracaoGeralCarga, primeiraCargaPedido.CentroResultado, primeiraCargaPedido.CentroResultadoDestinatario, primeiraCargaPedido.ItemServico, primeiraCargaPedido.CentroResultadoEscrituracao, primeiraCargaPedido.CentroResultadoICMS, primeiraCargaPedido.CentroResultadoPIS, primeiraCargaPedido.CentroResultadoCOFINS, primeiraCargaPedido.ValorMaximoCentroContabilizacao, configuracoes, xmlNotasFiscais, enderecoRecebedor, descricaoComponenteValorICMS, descricaoComponenteValorFrete, enderecoExpedidor, null, cargaPedidos);

                Servicos.Log.GravarInfo("9 - Gerou CTE CARGA_CTE codigo: " + cargaCTE?.Codigo, "SolicitarEmissaoDocumentosAutorizados");

                serCte.AverbaCargaCTe(cargaCTE, apoliceSeguroAverbacaos, unitOfWork, configuracaoEmbarcador, primeiraCargaPedido.Pedido.FormaAverbacaoCTE);

                if (ciot != null)
                {
                    Dominio.Entidades.Embarcador.Documentos.CIOTCTe ciotCTe = new Dominio.Entidades.Embarcador.Documentos.CIOTCTe()
                    {
                        CargaCTe = cargaCTE,
                        CIOT = ciot
                    };

                    repCIOTCTe.Inserir(ciotCTe);
                }

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscais = (from obj in pedidoXMLNotasFiscaisCarga where obj.CargaPedido.Codigo == cargaPedido.Codigo select obj).ToList();//repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotaFiscais)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTE = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe();
                        cargaPedidoXMLNotaFiscalCTE.CargaCTe = cargaCTE;
                        cargaPedidoXMLNotaFiscalCTE.PedidoXMLNotaFiscal = pedidoXMLNotaFiscal;
                        repCargaPedidoXMLNotaFiscalCTe.Inserir(cargaPedidoXMLNotaFiscalCTE);
                    }
                }

                if (primeiraCargaPedido.PossuiNFSManual)
                {
                    serNFS.CriarCargaCTePendenteEmissaoManualDeNFS(carga, cargaCTE, origem, unitOfWork);

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

            svcHubCarga.InformarQuantidadeDocumentosEmitidos(carga.Codigo, totalDocumentos, totalDocumentosGerados);
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS BuscarRegraICMS(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, bool emitirCTeFilialEmissora)
        {
            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS();

            if (!emitirCTeFilialEmissora)
            {
                if (cargaPedido.ModeloDocumentoFiscal == null || cargaPedido.ModeloDocumentoFiscal.Numero == "57")
                {
                    regraICMS.CFOP = cargaPedido.CFOP?.CodigoCFOP ?? 0;
                    regraICMS.CST = cargaPedido.CST;
                }
                regraICMS.Aliquota = cargaPedido.PercentualAliquota;
                regraICMS.AliquotaInternaDifal = cargaPedido.PercentualAliquotaInternaDifal;
                regraICMS.PercentualReducaoBC = cargaPedido.PercentualReducaoBC;
                regraICMS.ObservacaoCTe = cargaPedido.ObservacaoRegraICMSCTe;
                regraICMS.PercentualInclusaoBC = cargaPedido.PercentualIncluirBaseCalculo;
                regraICMS.IncluirICMSBC = cargaPedido.IncluirICMSBaseCalculo;
                regraICMS.PercentualCreditoPresumido = cargaPedido.PercentualCreditoPresumido;
                regraICMS.DescontarICMSDoValorAReceber = cargaPedido.DescontarICMSDoValorAReceber;
                regraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao = cargaPedido.NaoReduzirRetencaoICMSDoValorDaPrestacao;
                regraICMS.NaoImprimirImpostosDACTE = cargaPedido.NaoImprimirImpostosDACTE;
                regraICMS.NaoEnviarImpostoICMSNaEmissaoCte = cargaPedido.NaoEnviarImpostoICMSNaEmissaoCte;
                regraICMS.CodigoRegra = cargaPedido.RegraICMS?.Codigo ?? 0;
                regraICMS.AliquotaPis = cargaPedido.AliquotaPis;
                regraICMS.AliquotaCofins = cargaPedido.AliquotaCofins;
                regraICMS.ValorPis = cargaPedido.ValorPis;
                regraICMS.ValorCofins = cargaPedido.ValorCofins;
            }
            else
            {
                regraICMS.CFOP = cargaPedido.CFOPFilialEmissora?.CodigoCFOP ?? 0;
                regraICMS.CST = cargaPedido.CSTFilialEmissora;
                regraICMS.Aliquota = cargaPedido.PercentualAliquotaFilialEmissora;
                regraICMS.AliquotaInternaDifal = cargaPedido.PercentualAliquotaFilialEmissoraInternaDifal;
                regraICMS.PercentualReducaoBC = cargaPedido.PercentualReducaoBCFilialEmissora;
                regraICMS.ObservacaoCTe = cargaPedido.ObservacaoRegraICMSCTeFilialEmissora;
                regraICMS.PercentualInclusaoBC = cargaPedido.PercentualIncluirBaseCalculoFilialEmissora;
                regraICMS.IncluirICMSBC = cargaPedido.IncluirICMSBaseCalculoFilialEmissora;
                regraICMS.PercentualCreditoPresumido = cargaPedido.PercentualCreditoPresumidoFilialEmissora;
                regraICMS.DescontarICMSDoValorAReceber = cargaPedido.DescontarICMSDoValorAReceber;
                regraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao = cargaPedido.NaoReduzirRetencaoICMSDoValorDaPrestacao;
                regraICMS.NaoImprimirImpostosDACTE = cargaPedido.NaoImprimirImpostosDACTE;
                regraICMS.NaoEnviarImpostoICMSNaEmissaoCte = cargaPedido.NaoEnviarImpostoICMSNaEmissaoCte;
            }

            return regraICMS;
        }

        private Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS BuscarRegraISS(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = new Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS();

            regraISS.AliquotaISS = cargaPedido.PercentualAliquotaISS;
            regraISS.IncluirISSBaseCalculo = cargaPedido.IncluirISSBaseCalculo;
            regraISS.PercentualRetencaoISS = cargaPedido.PercentualRetencaoISS;

            regraISS.ReterIR = cargaPedido.ReterIR;
            regraISS.AliquotaIR = cargaPedido.AliquotaIR;
            regraISS.BaseCalculoIR = cargaPedido.BaseCalculoIR;
            regraISS.ValorIR = cargaPedido.ValorIR;

            return regraISS;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> BuscarCargaPedidoComponentesFreteOutrosDocumentos(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalCarga, Repositorio.UnitOfWork unitOfWork, bool componenteFilialEmissora)
        {
            //todo: Os componentes atualmente são montados por tipo no CT-e ver regra, se necessário para agrupar por componente de Frete (apenas exibição)
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> cargaPedidoComponentesFretesCliente = new List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFretes = repComponenteFrete.BuscarPorCargaPedidoOutrosDocumentos(cargaPedido.Codigo, modeloDocumentoFiscalCarga.Codigo, componenteFilialEmissora);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFrete in cargaPedidoComponentesFretes)
                {
                    if (cargaPedidoComponentesFretesCliente.Exists(obj => obj.TipoComponenteFrete == cargaPedidoComponenteFrete.TipoComponenteFrete && cargaPedidoComponenteFrete.ComponenteFrete == obj.ComponenteFrete))
                    {
                        cargaPedidoComponentesFretesCliente.Find(obj => obj.TipoComponenteFrete == cargaPedidoComponenteFrete.TipoComponenteFrete && cargaPedidoComponenteFrete.ComponenteFrete == obj.ComponenteFrete).ValorComponente += cargaPedidoComponenteFrete.ValorComponente;
                    }
                    else
                    {
                        Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico componente = cargaPedidoComponenteFrete.ConvertarParaComponenteDinamico();
                        cargaPedidoComponentesFretesCliente.Add(componente);
                    }
                }
            }
            return cargaPedidoComponentesFretesCliente;
        }

        #endregion
    }
}
