using Repositorio;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga
{
    public class NFSManualPorNota : ServicoBase
    {        
        public NFSManualPorNota(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public void GerarNFSManualPorNota(int codigoNFSManual, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Enumeradores.TipoPagamento tipoPagamento, Dominio.Enumeradores.TipoTomador tipoTomador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
            Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repPedidoCTeParaSubContratacaoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroQuantidade repCTeParaSubContratacaoQuantidade = new Repositorio.Embarcador.CTe.CTeTerceiroQuantidade(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unitOfWork);
            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.ContainerTipo repContainerTipo = new Repositorio.Embarcador.Pedidos.ContainerTipo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao repCargaPedidoContaContabilContabilizacao = new Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao(unitOfWork);

            Servicos.Embarcador.Carga.CTe serCte = new CTe(unitOfWork);
            Servicos.Embarcador.Carga.RateioNFSManual serRateioNFManual = new RateioNFSManual();
            Servicos.Embarcador.Carga.CTePorCTeParaSubcontratacao serCTePorCTeParaSubcontratacao = new CTePorCTeParaSubcontratacao(unitOfWork);
            Servicos.Embarcador.Hubs.NFSManual svcNFSManual = new Servicos.Embarcador.Hubs.NFSManual();
            Hubs.Carga svcHubCarga = new Hubs.Carga();

            Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual = repLancamentoNFSManual.BuscarPorCodigo(codigoNFSManual);
            List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> cargaDocumentoParaEmissaoNFSManuals = repCargaDocumentoParaEmissaoNFSManual.BuscarPorLancamentoNFsManual(lancamentoNFSManual.Codigo);

            List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> notasVinculadas = (from obj in cargaDocumentoParaEmissaoNFSManuals where obj.DocumentosNFSe != null select obj).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> documentosSemNFSeVinculada = repCargaDocumentoParaEmissaoNFSManual.BuscarPorLancamentoNFsManualSemDocumentos(lancamentoNFSManual.Codigo);

            Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultadoFaturamento = cargaDocumentoParaEmissaoNFSManuals.Where(o => o.CargaCTe?.CTe != null).Select(o => o.CargaCTe.CTe.CentroResultadoFaturamento).FirstOrDefault();

            Dominio.ObjetosDeValor.CTe.CTe cte = new Dominio.ObjetosDeValor.CTe.CTe
            {
                Documentos = new List<Dominio.ObjetosDeValor.CTe.Documento>()
            };

            List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> cargaPedidoComponentesFretesCliente = new List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico>();
            List<string> rotas = new List<string>();

            decimal peso = 0;
            int volumes = 0;

            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS(); //BuscarRegraICMS(cargaPedidos[0]);
            Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = BuscarRegraISS(lancamentoNFSManual.DadosNFS);
            Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = BuscarIBSCBS(lancamentoNFSManual.DadosNFS);

            Dominio.Enumeradores.TipoServico tipoServico = Dominio.Enumeradores.TipoServico.Normal;

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesAnterioresTodasCargasPedido = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = (from obj in documentosSemNFSeVinculada where obj.PedidoXMLNotaFiscal != null && obj.PedidoCTeParaSubContratacao == null && obj.CargaCTe == null select obj.PedidoXMLNotaFiscal.CargaPedido).Distinct().ToList();
            cargaPedidos.AddRange((from obj in documentosSemNFSeVinculada where obj.PedidoCTeParaSubContratacao != null && obj.PedidoXMLNotaFiscal == null && obj.CargaCTe == null select obj.PedidoCTeParaSubContratacao.CargaPedido).Distinct().ToList());
            cargaPedidos.AddRange((from obj in documentosSemNFSeVinculada where obj.CargaCTe != null && obj.PedidoXMLNotaFiscal == null && obj.PedidoCTeParaSubContratacao == null select obj.CargaCTe.NotasFiscais.Select(o => o.PedidoXMLNotaFiscal.CargaPedido)).SelectMany(o => o).Distinct().ToList());

            if (centroResultadoFaturamento == null)
                centroResultadoFaturamento = cargaPedidos.Where(o => o.Pedido.CentroResultado != null).Select(o => o.Pedido.CentroResultado).FirstOrDefault();

            if (cargaPedidos.Count == 0)//não sei se isso é certo, mas estava dando problema pois não localizava nenhuma cargaPedido para a nota
                cargaPedidos.AddRange((from obj in documentosSemNFSeVinculada select obj.Carga.Pedidos).SelectMany(o => o).Distinct().ToList());

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = (from obj in cargaPedidos select obj.Carga).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesParaSubcontrataca = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>();

            //se não rateou nenhum documento o sistema zera os valores da carga e cargapedido para somar novamente 
            if (!documentosSemNFSeVinculada.Any(obj => obj.RateouValorFrete))
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    cargaPedido.ValorISS = 0m;
                    cargaPedido.ValorRetencaoISS = 0m;
                    cargaPedido.BaseCalculoISS = 0m;
                    repCargaPedido.Atualizar(cargaPedido);
                }

                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                {
                    carga.ValorISS = 0m;
                    carga.ValorRetencaoISS = 0m;
                    repCarga.Atualizar(carga);
                }
            }

            serRateioNFManual.AplicarRateioNaEstruturaDasCargasNoLancamentoManual(lancamentoNFSManual, unitOfWork);


            unitOfWork.Start();

            lancamentoNFSManual = repLancamentoNFSManual.BuscarPorCodigo(codigoNFSManual);

            List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidades = new List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga>();
            List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> seguros = new List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro>();

            documentosSemNFSeVinculada = repCargaDocumentoParaEmissaoNFSManual.BuscarPorLancamentoNFsManualSemDocumentos(lancamentoNFSManual.Codigo);

            // Itera Documentos

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscals = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            cte.Veiculos = new List<Dominio.ObjetosDeValor.CTe.Veiculo>();

            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = (from obj in cargaDocumentoParaEmissaoNFSManuals where obj.CargaOcorrencia != null select obj.CargaOcorrencia).FirstOrDefault();
            Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo = null;

            if (cargaOcorrencia != null)
                cargaCTeComplementoInfo = repCargaCTeComplementoInfo.BuscarNFSManualASerComplementadaPorOcorrencia(cargaOcorrencia.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual documento in documentosSemNFSeVinculada)
            {
                if (documento.PedidoXMLNotaFiscal != null)
                {
                    pedidoXMLNotaFiscals.Add(documento.PedidoXMLNotaFiscal);
                    Dominio.ObjetosDeValor.CTe.Documento docNF = serCte.BuscarDocumentoCTe(documento.PedidoXMLNotaFiscal.XMLNotaFiscal, lancamentoNFSManual.Transportador.TipoAmbiente, configuracaoEmbarcador);
                    if (!cte.Documentos.Any(obj => obj.ChaveNFE == docNF.ChaveNFE && obj.Tipo == docNF.Tipo && obj.Numero == docNF.Numero))
                    {
                        cte.Documentos.Add(docNF);
                        peso += docNF.Peso;
                        volumes += docNF.Volume;
                        cte.ValorTotalMercadoria += docNF.Valor;
                        cte.ValorImpostoSuspenso += documento?.PedidoXMLNotaFiscal?.XMLNotaFiscal?.ValorImpostoSuspenso ?? 0;
                    }
                }
                else if (documento.PedidoCTeParaSubContratacao != null)
                {
                    Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteParaSubcontratacao = documento.PedidoCTeParaSubContratacao.CTeTerceiro;

                    ctesParaSubcontrataca.Add(cteParaSubcontratacao);
                    cte.ValorTotalMercadoria = cteParaSubcontratacao.ValorTotalMercadoria;

                    List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade> quantidadesCte = repCTeParaSubContratacaoQuantidade.BuscarPorCTeParaSubContratacao(cteParaSubcontratacao.Codigo);
                    List<string> tiposContaineres = configuracaoEmbarcador.UtilizaEmissaoMultimodal ? repContainerTipo.BuscarDescricoes() : new List<string>();
                    bool utilizarPrimeiraUnidadeMedidaPeso = serCTePorCTeParaSubcontratacao.UtilizarPrimeiraUnidadeMedidaPeso(lancamentoNFSManual.Tomador, null);
                    List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidadesCTes = serCTePorCTeParaSubcontratacao.ConverterQuantidadesTerceirosParaQuantidadesCTe(quantidadesCte, configuracaoEmbarcador.AgruparUnidadesMedidasPorDescricao, utilizarPrimeiraUnidadeMedidaPeso, tiposContaineres);

                    PreecherQuantidadesTotais(quantidadesCTes, ref quantidades);

                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> pedidoCTeParaSubContratacaoNotasFiscais = repPedidoCTeParaSubContratacaoNotaFiscal.BuscarPorPedidoCTeParaSubcontratacao(documento.PedidoCTeParaSubContratacao.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoPedidoNotaFiscal in pedidoCTeParaSubContratacaoNotasFiscais)
                    {
                        pedidoXMLNotaFiscals.Add(pedidoCTeParaSubContratacaoPedidoNotaFiscal.PedidoXMLNotaFiscal);
                        Dominio.ObjetosDeValor.CTe.Documento docNF = serCte.BuscarDocumentoCTe(pedidoCTeParaSubContratacaoPedidoNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal, lancamentoNFSManual.Transportador.TipoAmbiente, configuracaoEmbarcador);
                        if (!cte.Documentos.Any(obj => obj.ChaveNFE == docNF.ChaveNFE && obj.Tipo == docNF.Tipo && obj.Numero == docNF.Numero))
                            cte.Documentos.Add(docNF);
                    }
                }
                else if (documento.CargaCTe != null)
                {
                    if (documento.CargaCTe.NotasFiscais != null)
                    {
                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTe in documento.CargaCTe.NotasFiscais.ToList())
                            pedidoXMLNotaFiscals.Add(cargaPedidoXMLNotaFiscalCTe.PedidoXMLNotaFiscal);
                    }

                    foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal documentoCTe in documento.CargaCTe.CTe.XMLNotaFiscais)
                    {
                        Dominio.ObjetosDeValor.CTe.Documento docNF = serCte.BuscarDocumentoCTe(documentoCTe, lancamentoNFSManual.Transportador.TipoAmbiente, configuracaoEmbarcador);
                        if (!cte.Documentos.Any(obj => obj.ChaveNFE == docNF.ChaveNFE && obj.Tipo == docNF.Tipo && obj.Numero == docNF.Numero))
                            cte.Documentos.Add(docNF);

                        peso += documentoCTe.Peso;
                        volumes += documentoCTe.Volumes;
                        cte.ValorTotalMercadoria += documentoCTe.Valor;
                    }
                }

                if (documento.CargaOrigem != null)
                {
                    if (documento.CargaOrigem.Veiculo != null)
                    {
                        if (!cte.Veiculos.Any(c => c.Placa == documento.CargaOrigem.Veiculo.Placa))
                            cte.Veiculos.Add(new Dominio.ObjetosDeValor.CTe.Veiculo() { Placa = documento.CargaOrigem.Veiculo.Placa, CodigoVeiculo = documento.CargaOrigem.Veiculo.Codigo });
                    }

                    if (documento.CargaOrigem.VeiculosVinculados?.Count > 0)
                    {
                        foreach (Dominio.Entidades.Veiculo veiculo in documento.CargaOrigem.VeiculosVinculados)
                        {
                            if (!cte.Veiculos.Any(c => c.Placa == veiculo.Placa))
                                cte.Veiculos.Add(new Dominio.ObjetosDeValor.CTe.Veiculo() { Placa = veiculo.Placa, CodigoVeiculo = veiculo.Codigo });
                        }
                    }
                }
            }

            List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ctesAnteriores = serCTePorCTeParaSubcontratacao.ConverterCTesTerceirosParaAnteriores(ctesParaSubcontrataca, unitOfWork);

            cte.ObservacoesGerais = lancamentoNFSManual.DadosNFS.Observacoes;
            cte.Serie = lancamentoNFSManual.DadosNFS.Serie.Numero;
            cte.ValorAReceber += lancamentoNFSManual.DadosNFS.ValorFrete;
            cte.ValorFrete += lancamentoNFSManual.DadosNFS.ValorFrete;
            cte.ValorTotalPrestacaoServico += lancamentoNFSManual.DadosNFS.ValorFrete;
            cte.ValorTotalMoeda = lancamentoNFSManual.DadosNFS.ValorTotalMoeda;
            cte.Moeda = lancamentoNFSManual.DadosNFS.Moeda;

            if (lancamentoNFSManual.DadosNFS.ValorCOFINS > 0m)
                cte.COFINS = new Dominio.ObjetosDeValor.CTe.ImpostoCOFINS() { Valor = lancamentoNFSManual.DadosNFS.ValorCOFINS };

            if (lancamentoNFSManual.DadosNFS.ValorPIS > 0m)
                cte.PIS = new Dominio.ObjetosDeValor.CTe.ImpostoPIS() { Valor = lancamentoNFSManual.DadosNFS.ValorPIS };

            if (lancamentoNFSManual.DadosNFS.ValorIR > 0m)
                cte.IR = new Dominio.ObjetosDeValor.CTe.ImpostoIR() { Valor = lancamentoNFSManual.DadosNFS.ValorIR };

            if (lancamentoNFSManual.DadosNFS.ValorCSLL > 0m)
                cte.CSLL = new Dominio.ObjetosDeValor.CTe.ImpostoCSLL() { Valor = lancamentoNFSManual.DadosNFS.ValorCSLL };

            List<Dominio.Entidades.NFSe> listaNFSe = (from obj in notasVinculadas where obj.DocumentosNFSe != null select obj.NFSe).Distinct().ToList();// nota.DocumentosNFSe.NFSe
            foreach (Dominio.Entidades.NFSe nfse in listaNFSe)
            {
                nfse.Status = Dominio.Enumeradores.StatusNFSe.NFSeManualGerada;
                nfse.Numero = lancamentoNFSManual.DadosNFS.Numero;
                nfse.Serie = lancamentoNFSManual.DadosNFS.Serie;
                repNFSe.Atualizar(nfse);
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual nota in notasVinculadas)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in nota.Carga.CargaCTes)
                {
                    if (cargaCTe.CTe != null)
                    {
                        cargaCTe.CTe.Numero = lancamentoNFSManual.DadosNFS.Numero;
                        cargaCTe.CTe.Serie = lancamentoNFSManual.DadosNFS.Serie;
                        cargaCTe.CTe.TipoControle = repCTe.BuscarUltimoTipoControle() + 1;
                        repCTe.Atualizar(cargaCTe.CTe);
                    }
                }
            }

            if (cte.ValorFrete > 0m && documentosSemNFSeVinculada.Count > 0)
            {
                if (quantidades.Count <= 0)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidadesNFe = serCte.RetornaQuantidades(peso, volumes, 0m, 0m, 0m);
                    PreecherQuantidadesTotais(quantidadesNFe, ref quantidades);
                }

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosProduto = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoProduto = repCargaPedido.BuscarPorCodigo(cargaPedidos[0]?.Codigo ?? 0);
                if (cargaPedidoProduto != null)
                {
                    cargaPedidosProduto.Add(cargaPedidoProduto);
                    cte.ProdutoPredominante = serCte.BuscarProdutoPredominante(cargaPedidosProduto, configuracaoEmbarcador, unitOfWork);
                }

                Dominio.Enumeradores.TipoCTE tipoCTe = Dominio.Enumeradores.TipoCTE.Normal;

                serCte.ObterDescricoesComponentesPadrao(lancamentoNFSManual.Tomador, null, configuracaoEmbarcador, unitOfWork, out string descricaoComponenteValorFrete, out string descricaoComponenteValorICMS);
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xMLNotaFiscals = (from obj in pedidoXMLNotaFiscals select obj.XMLNotaFiscal).ToList();
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidos.FirstOrDefault();

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao> cargaPedidosContaContabilContabilizacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao>();
                List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil>();

                Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = null;
                Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultadoEscrituracao = null;
                Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultadoDestinatario = null;

                Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultadoICMS = null;
                Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultadoPIS = null;
                Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultadoCOFINS = null;
                decimal valorMaximoCentroContabilizacao = 0;
                string itemServico = "";
                if (cargaOcorrencia != null)
                {
                    Repositorio.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao repCargaCTeComplementoInfoContaContabilContabilizacao = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao(unitOfWork);

                    if (cargaCTeComplementoInfo != null)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao> cargaCTeComplementoInfoContaContabilContabilizacaos = repCargaCTeComplementoInfoContaContabilContabilizacao.BuscarPorCargaCTeComplementoInfo(cargaCTeComplementoInfo.Codigo);
                        configuracoes = Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil.ConverterCargaCTeComplementoInfoContaContabilContabilizacao(cargaCTeComplementoInfo, cargaCTeComplementoInfoContaContabilContabilizacaos);
                        centroResultado = cargaCTeComplementoInfo.CentroResultado;
                        centroResultadoEscrituracao = cargaCTeComplementoInfo.CentroResultadoEscrituracao;
                        centroResultadoDestinatario = cargaCTeComplementoInfo.CentroResultadoDestinatario;

                        centroResultadoICMS = cargaCTeComplementoInfo.CentroResultadoICMS;
                        centroResultadoPIS = cargaCTeComplementoInfo.CentroResultadoPIS;
                        centroResultadoCOFINS = cargaCTeComplementoInfo.CentroResultadoCOFINS;
                        itemServico = cargaCTeComplementoInfo.ItemServico;
                        valorMaximoCentroContabilizacao = cargaCTeComplementoInfo.ValorMaximoCentroContabilizacao;
                    }
                }
                else if (cargaPedido != null)
                {
                    cargaPedidosContaContabilContabilizacao = repCargaPedidoContaContabilContabilizacao.BuscarPorCargaPedido(cargaPedido.Codigo);
                    configuracoes = Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil.ConverterCargaPedidoContaContabilContabilizacao(cargaPedido, cargaPedidosContaContabilContabilizacao);
                    centroResultado = cargaPedido.CentroResultado;
                    centroResultadoEscrituracao = cargaPedido.CentroResultadoEscrituracao;
                    centroResultadoDestinatario = cargaPedido.CentroResultadoDestinatario;
                    centroResultadoICMS = cargaPedido.CentroResultadoICMS;
                    centroResultadoPIS = cargaPedido.CentroResultadoPIS;
                    centroResultadoCOFINS = cargaPedido.CentroResultadoCOFINS;

                    itemServico = cargaPedido.ItemServico;
                    valorMaximoCentroContabilizacao = cargaPedido.ValorMaximoCentroContabilizacao;
                }

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteIntegrado = serCte.GerarCTe(lancamentoNFSManual, cargas, cargaPedidos, pedidoXMLNotaFiscals, lancamentoNFSManual.Transportador, cte, lancamentoNFSManual.Tomador, lancamentoNFSManual.Tomador, lancamentoNFSManual.Tomador, null, null, lancamentoNFSManual.LocalidadePrestacao, lancamentoNFSManual.LocalidadePrestacao, null, null, tipoPagamento, tipoTomador, quantidades, cargaPedidoComponentesFretesCliente, "", null, rotas, seguros, regraICMS, regraISS, impostoIBSCBS, tipoServicoMultisoftware, lancamentoNFSManual.DadosNFS.ModeloDocumentoFiscal, tipoServico, tipoCTe, ctesAnteriores, 0, false, lancamentoNFSManual.DadosNFS.Numero, unitOfWork, xMLNotaFiscals, descricaoComponenteValorFrete, descricaoComponenteValorICMS, configuracaoEmbarcador, centroResultado, centroResultadoDestinatario, itemServico, centroResultadoEscrituracao, centroResultadoICMS, centroResultadoPIS, centroResultadoCOFINS, valorMaximoCentroContabilizacao, configuracoes, cargaDocumentoParaEmissaoNFSManuals);

                cteIntegrado.CentroResultadoFaturamento = centroResultadoFaturamento;
                cteIntegrado.PossuiPedidoSubstituicao = cargaPedidos.Where(o => o.Pedido.Substituicao == true).Select(o => o.Pedido.Substituicao).Any();

                if (cargaOcorrencia != null && cargaCTeComplementoInfo != null)
                {
                    List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> cargasOcorrencia = (from obj in cargaDocumentoParaEmissaoNFSManuals where obj.CargaOcorrencia != null select obj.CargaOcorrencia).Distinct().ToList();
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargasCTeComplementoInfo = (from obj in cargaDocumentoParaEmissaoNFSManuals where obj.CargaCTeComplementoInfo != null select obj.CargaCTeComplementoInfo).ToList();

                    foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrenciaAtualizar in cargasOcorrencia)
                    {
                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaComplementosInfo in cargasCTeComplementoInfo.Where(obj => obj.CargaOcorrencia.Codigo == cargaOcorrenciaAtualizar.Codigo).ToList())
                        {
                            cargaComplementosInfo.CTe = cteIntegrado;
                            repCargaCTeComplementoInfo.Atualizar(cargaComplementosInfo);
                        }

                        cargaOcorrenciaAtualizar.NFSManualPendenteGeracao = repCargaOcorrencia.VerificarSeExisteNFSManualPendenteGeracao(cargaOcorrenciaAtualizar.Codigo);
                        repCargaOcorrencia.Atualizar(cargaOcorrenciaAtualizar);
                    }
                }

                if (lancamentoNFSManual.DadosNFS.DataEmissao != null)
                {
                    cteIntegrado.DataEmissao = lancamentoNFSManual.DadosNFS.DataEmissao.Value;

                    if (cteIntegrado.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe &&
                        cteIntegrado.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe &&
                        cteIntegrado.Status == "A")
                        cteIntegrado.DataAutorizacao = cteIntegrado.DataEmissao;
                }

                if (!string.IsNullOrWhiteSpace(lancamentoNFSManual.DadosNFS.XML))
                {
                    Dominio.Entidades.XMLCTe xmlCTe = new Dominio.Entidades.XMLCTe();
                    xmlCTe.XML = lancamentoNFSManual.DadosNFS.XML;
                    xmlCTe.Tipo = Dominio.Enumeradores.TipoXMLCTe.Autorizacao;
                    xmlCTe.CTe = cteIntegrado;
                    repXMLCTe.Inserir(xmlCTe);
                }

                lancamentoNFSManual.CTe = cteIntegrado;

                repCTe.Atualizar(cteIntegrado);
                repLancamentoNFSManual.Atualizar(lancamentoNFSManual);
                repCargaDocumentoParaEmissaoNFSManual.SetarCTesPorLancamentoManual(lancamentoNFSManual.Codigo, cteIntegrado.Codigo);

                lancamentoNFSManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.EmEmissao;

                repLancamentoNFSManual.Atualizar(lancamentoNFSManual);

                unitOfWork.CommitChanges();

                if (cteIntegrado != null && cteIntegrado.Status == "E")
                {
                    if (cteIntegrado.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                    {
                        Servicos.CTe servicoCte = new Servicos.CTe(unitOfWork);
                        servicoCte.Emitir(cteIntegrado.Codigo, 0, unitOfWork);
                    }
                    else if (cteIntegrado.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                    {
                        Servicos.NFSe servicoNFSe = new Servicos.NFSe(unitOfWork);
                        servicoNFSe.EmitirNFSe(cteIntegrado.Codigo, unitOfWork);
                    }
                }
            }
            else
            {
                lancamentoNFSManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.AgIntegracao;
                repLancamentoNFSManual.Atualizar(lancamentoNFSManual);
                unitOfWork.CommitChanges();
            }

            // Integracao com SignalR
            svcNFSManual.InformarLancamentoNFSManualAtualizada(lancamentoNFSManual.Codigo);
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS BuscarIBSCBS(Dominio.Entidades.Embarcador.NFS.DadosNFSManual nFSManual)
        {
            Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS
            {
                AliquotaCBS = nFSManual.AliquotaCBS,
                AliquotaIBSEstadual = nFSManual.AliquotaIBSEstadual,
                AliquotaIBSMunicipal = nFSManual.AliquotaIBSMunicipal,
                BaseCalculo = nFSManual.BaseCalculoIBSCBS,
                ClassificacaoTributaria = nFSManual.ClassificacaoTributariaIBSCBS,
                CodigoIndicadorOperacao = nFSManual.IndicadorOperacao,
                CodigoOutraAliquota = nFSManual?.OutrasAliquotas?.Codigo ?? 0,
                CST = nFSManual.CSTIBSCBS,
                NBS = nFSManual.NBS,
                PercentualReducaoCBS = nFSManual.PercentualReducaoCBS,
                PercentualReducaoIBSEstadual = nFSManual.PercentualReducaoIBSEstadual,
                PercentualReducaoIBSMunicipal = nFSManual.PercentualReducaoIBSMunicipal,
                ValorCBS = nFSManual.ValorCBS,
                ValorIBSEstadual = nFSManual.ValorIBSEstadual,
                ValorIBSMunicipal = nFSManual.ValorIBSMunicipal
            };

            return impostoIBSCBS;
        }

        private Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS BuscarRegraISS(Dominio.Entidades.Embarcador.NFS.DadosNFSManual nFSManual)
        {
            Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = new Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS
            {
                AliquotaISS = nFSManual.AliquotaISS,
                IncluirISSBaseCalculo = nFSManual.IncluirISSBC,
                PercentualRetencaoISS = nFSManual.PercentualRetencao
            };
            regraISS.ValorBaseCalculoISS = nFSManual.ValorBaseCalculo;
            regraISS.ValorRetencaoISS = nFSManual.ValorRetido;
            regraISS.ValorISS = nFSManual.ValorISS;

            return regraISS;
        }

        private void PreecherQuantidadesTotais(List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidadesCTes, ref List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidadesGlobal)
        {
            foreach (Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga quantidadeCTe in quantidadesCTes)
            {
                Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga quantidadeGlobal = (from obj in quantidadesGlobal where obj != null && obj.Unidade == quantidadeCTe.Unidade select obj).FirstOrDefault();
                if (quantidadeGlobal == null)
                    quantidadesGlobal.Add(quantidadeCTe);
                else
                    quantidadeGlobal.Quantidade += quantidadeCTe.Quantidade;
            }
        }

        #endregion
    }
}
