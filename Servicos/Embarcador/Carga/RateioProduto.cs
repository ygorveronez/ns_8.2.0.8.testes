using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga
{
    public class RateioProduto : ServicoBase
    {
        public RateioProduto(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public void RatearFretePorProduto(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork = null)
        {
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                bool controlaUnit = false;
                if (unitOfWork == null)
                {
                    unitOfWork = new Repositorio.UnitOfWork(StringConexao);
                    controlaUnit = true;
                }
                try
                {
                    if (controlaUnit)
                        unitOfWork.Start();

                    Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
                    Repositorio.Embarcador.Cargas.CargaCTe repCargaCTE = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                    Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);

                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTeComplementoInfos = repCargaCTeComplementoInfo.BuscarPorOcorrencia(cargaOcorrencia.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo in cargaCTeComplementoInfos)
                    {
                        if (cargaCTeComplementoInfo.CargaDocumentoParaEmissaoNFSManualGerado != null)
                        {
                            RatearProdutoPorCargaCTe(cargaOcorrencia.Carga, cargaCTeComplementoInfo, cargaCTeComplementoInfo.CargaDocumentoParaEmissaoNFSManualGerado, unitOfWork, tipoServicoMultisoftware);
                        }
                        else
                        {
                            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTE.BuscarPorCargaCTeComplementoInfo(cargaCTeComplementoInfo.Codigo);
                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
                                RatearProdutoPorCargaCTe(cargaOcorrencia.Carga, cargaCTeComplementoInfo, cargaCTe, unitOfWork, tipoServicoMultisoftware);
                        }
                    }

                    if (controlaUnit)
                        unitOfWork.CommitChanges();
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    if (controlaUnit)
                        unitOfWork.Rollback();
                }
            }
        }


        public void RatearFretePorProduto(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork = null)
        {
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                bool controlaUnit = false;
                if (unitOfWork == null)
                {
                    unitOfWork = new Repositorio.UnitOfWork(StringConexao);
                    controlaUnit = true;
                }
                try
                {
                    if (controlaUnit)
                        unitOfWork.Start();

                    Repositorio.Embarcador.Cargas.CargaCTe repCargaCTE = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                    Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);

                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs = repCargaCTE.BuscarPorCarga(carga.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTEs)
                        RatearProdutoPorCargaCTe(carga, null, cargaCTe, unitOfWork, tipoServicoMultisoftware);

                    List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> cargaDocumentoParaEmissaoNFSManuals = repCargaDocumentoParaEmissaoNFSManual.BuscarPorCarga(carga.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual cargaDocumentoParaEmissaoNFSManual in cargaDocumentoParaEmissaoNFSManuals)
                        RatearProdutoPorCargaCTe(carga, null, cargaDocumentoParaEmissaoNFSManual, unitOfWork, tipoServicoMultisoftware);

                    if (controlaUnit)
                        unitOfWork.CommitChanges();
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    if (controlaUnit)
                        unitOfWork.Rollback();
                }
            }
        }

        public void RatearProdutoPorCargaCTe(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo, Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual cargaDocumentoParaEmissaoNFSManual, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
                Repositorio.Embarcador.Rateio.RateioCargaPedidoProduto repRateioCargaPedidoCTeProduto = new Repositorio.Embarcador.Rateio.RateioCargaPedidoProduto(unitOfWork);
                Repositorio.InformacaoCargaCTE repInformacaoCargaCTE = new Repositorio.InformacaoCargaCTE(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscaCte = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
                Repositorio.Embarcador.Rateio.RateioProdutoComponenteFrete repRateioProdutoComponenteFrete = new Repositorio.Embarcador.Rateio.RateioProdutoComponenteFrete(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                Servicos.Embarcador.Carga.ComponetesFrete serComponetesFrete = new ComponetesFrete(unitOfWork);

                decimal valorSomadoRateioProduto = 0;
                decimal valorSomadoRateioFreteProduto = 0;
                decimal valorSomaICMS = 0;
                decimal valorApagarCTe = cargaDocumentoParaEmissaoNFSManual.ValorFrete;
                decimal valorFreteCTe = cargaDocumentoParaEmissaoNFSManual.ValorFrete;
                decimal valorISSCTe = 0; //cargaDocumentoParaEmissaoNFSManual.ValorISS;

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null;
                if (cargaDocumentoParaEmissaoNFSManual.PedidoXMLNotaFiscal != null)
                    cargaPedido = cargaDocumentoParaEmissaoNFSManual.PedidoXMLNotaFiscal.CargaPedido;
                else
                    cargaPedido = cargaDocumentoParaEmissaoNFSManual.PedidoCTeParaSubContratacao.CargaPedido;

                decimal pesoTotalCte = cargaDocumentoParaEmissaoNFSManual.PedidoXMLNotaFiscal?.XMLNotaFiscal.Peso ?? cargaDocumentoParaEmissaoNFSManual.PedidoCTeParaSubContratacao.CTeTerceiro.Peso;
                decimal valorMercadoriaCTe = cargaDocumentoParaEmissaoNFSManual.PedidoXMLNotaFiscal?.XMLNotaFiscal.ValorTotalProdutos ?? cargaDocumentoParaEmissaoNFSManual.PedidoCTeParaSubContratacao.CTeTerceiro.ValorTotalMercadoria;

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = repCargaPedidoProduto.BuscarPorCargaPedido(cargaPedido.Codigo);

                if (cargaPedido.Carga.TipoRateioProdutos == Dominio.Enumeradores.TipoRateioProdutos.Valor)
                {
                    decimal valorTotalPedido = (from obj in cargaPedidoProdutos select (obj.ValorUnitarioProduto * obj.Quantidade)).Sum();
                    if (valorMercadoriaCTe > 0 && valorTotalPedido > 0)
                    {
                        decimal valorPagarPedido = (valorApagarCTe / valorMercadoriaCTe) * valorTotalPedido;
                        decimal valorFretePedido = Math.Round((valorFreteCTe / valorMercadoriaCTe) * valorTotalPedido, 2, MidpointRounding.AwayFromZero);
                        decimal valorICMSPedido = (valorISSCTe / valorMercadoriaCTe) * valorTotalPedido;

                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto ultimoProduto = cargaPedidoProdutos.Last();
                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto in cargaPedidoProdutos)
                        {
                            Dominio.Entidades.Embarcador.Rateio.RateioCargaPedidoProduto rateioNotaCteProduto = new Dominio.Entidades.Embarcador.Rateio.RateioCargaPedidoProduto();
                            rateioNotaCteProduto.CargaPedido = cargaPedido;
                            rateioNotaCteProduto.CargaDocumentoParaEmissaoNFSManual = cargaDocumentoParaEmissaoNFSManual;
                            rateioNotaCteProduto.Empresa = cargaPedido.Carga.Empresa;
                            rateioNotaCteProduto.Cliente = cargaPedido.Pedido.Destinatario;
                            rateioNotaCteProduto.CargaCTeComplementoInfo = cargaCTeComplementoInfo;

                            rateioNotaCteProduto.Quantidade = cargaPedidoProduto.Quantidade;
                            if (rateioNotaCteProduto.Quantidade == 0)
                                rateioNotaCteProduto.Quantidade = 1;
                            //rateioNotaCteProduto.Peso = ((rateioNotaCteProduto.Quantidade * cargaPedidoProduto.PesoUnitario) + cargaPedidoProduto.PesoTotalEmbalagem);
                            rateioNotaCteProduto.ValorUnitarioProduto = cargaPedidoProduto.ValorUnitarioProduto * cargaPedidoProduto.Quantidade;
                            rateioNotaCteProduto.ProdutoEmbarcador = cargaPedidoProduto.Produto;

                            rateioNotaCteProduto.ValorTotalRateio = Math.Round((valorPagarPedido / valorTotalPedido) * rateioNotaCteProduto.ValorUnitarioProduto, 2, MidpointRounding.AwayFromZero);
                            valorSomadoRateioProduto += rateioNotaCteProduto.ValorTotalRateio;

                            rateioNotaCteProduto.ValorICMS = Math.Round((valorICMSPedido / valorTotalPedido) * rateioNotaCteProduto.ValorUnitarioProduto, 2, MidpointRounding.AwayFromZero);
                            valorSomaICMS += rateioNotaCteProduto.ValorICMS;

                            rateioNotaCteProduto.ValorFreteRateio = Math.Round((valorFretePedido / valorTotalPedido) * rateioNotaCteProduto.ValorUnitarioProduto, 2, MidpointRounding.AwayFromZero);
                            valorSomadoRateioFreteProduto += rateioNotaCteProduto.ValorFreteRateio;

                            if (cargaPedidoProduto.Equals(ultimoProduto))
                            {
                                decimal diferenca = valorApagarCTe - valorSomadoRateioProduto;
                                rateioNotaCteProduto.ValorTotalRateio += diferenca;
                                decimal diferencaICMS = valorISSCTe - valorSomaICMS;
                                rateioNotaCteProduto.ValorICMS += diferencaICMS;
                                decimal diferencaFreteLiquido = valorFreteCTe - valorSomadoRateioFreteProduto;
                                rateioNotaCteProduto.ValorFreteRateio += diferencaFreteLiquido;
                            }

                            if (cargaPedido.ICMSPagoPorST)
                                rateioNotaCteProduto.ValorICMSST = rateioNotaCteProduto.ValorICMS;
                            else
                                rateioNotaCteProduto.ValorICMSST = 0;


                            rateioNotaCteProduto.ValorTotalFrete = valorApagarCTe;

                            rateioNotaCteProduto.ValorUnitarioRateio = Math.Round(rateioNotaCteProduto.ValorTotalRateio / rateioNotaCteProduto.ValorUnitarioProduto, 6, MidpointRounding.AwayFromZero);
                            rateioNotaCteProduto.ValorUnitarioProduto = cargaPedidoProduto.ValorUnitarioProduto;
                            repRateioCargaPedidoCTeProduto.Inserir(rateioNotaCteProduto);
                        }
                    }
                }
                else
                {
                    decimal pesoTotalPedido = 0;
                    if (configuracao.UtilizarPesoEmbalagemProdutoParaRateio)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaProdutos = repCargaPedidoProduto.BuscarPorCarga(carga.Codigo);

                        pesoTotalPedido = (from obj in cargaProdutos select obj.PesoTotalEmbalagem).Sum();
                        pesoTotalCte = (from obj in cargaPedidoProdutos select obj.PesoTotalEmbalagem).Sum();
                    }
                    else
                        pesoTotalPedido = (from obj in cargaPedidoProdutos select ((obj.PesoUnitario * obj.Quantidade) + obj.PesoTotalEmbalagem)).Sum();

                    if (pesoTotalCte > 0 && pesoTotalPedido > 0)
                    {
                        decimal valorPagarPedido = (valorApagarCTe / pesoTotalCte) * pesoTotalPedido;
                        decimal valorFretePedido = (valorFreteCTe / pesoTotalCte) * pesoTotalPedido;
                        decimal valorICMSPedido = (valorISSCTe / pesoTotalCte) * pesoTotalPedido;

                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto ultimoProduto = cargaPedidoProdutos.Last();
                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto in cargaPedidoProdutos)
                        {
                            Dominio.Entidades.Embarcador.Rateio.RateioCargaPedidoProduto rateioNotaCteProduto = new Dominio.Entidades.Embarcador.Rateio.RateioCargaPedidoProduto();
                            rateioNotaCteProduto.CargaPedido = cargaPedido;
                            rateioNotaCteProduto.CargaDocumentoParaEmissaoNFSManual = cargaDocumentoParaEmissaoNFSManual;
                            rateioNotaCteProduto.Empresa = cargaPedido.Carga.Empresa;
                            rateioNotaCteProduto.Cliente = cargaPedido.Pedido.Destinatario;

                            rateioNotaCteProduto.Quantidade = cargaPedidoProduto.Quantidade;
                            if (rateioNotaCteProduto.Quantidade == 0)
                                rateioNotaCteProduto.Quantidade = 1;

                            if (configuracao.UtilizarPesoEmbalagemProdutoParaRateio)
                                rateioNotaCteProduto.Peso = cargaPedidoProduto.PesoTotalEmbalagem;
                            else
                                rateioNotaCteProduto.Peso = ((rateioNotaCteProduto.Quantidade * cargaPedidoProduto.PesoUnitario) + cargaPedidoProduto.PesoTotalEmbalagem);
                            rateioNotaCteProduto.ProdutoEmbarcador = cargaPedidoProduto.Produto;

                            rateioNotaCteProduto.ValorTotalRateio = Math.Round((valorPagarPedido / pesoTotalPedido) * rateioNotaCteProduto.Peso, 2, MidpointRounding.AwayFromZero);
                            valorSomadoRateioProduto += rateioNotaCteProduto.ValorTotalRateio;

                            rateioNotaCteProduto.ValorICMS = Math.Round((valorICMSPedido / pesoTotalPedido) * rateioNotaCteProduto.Peso, 2, MidpointRounding.AwayFromZero);
                            valorSomaICMS += rateioNotaCteProduto.ValorICMS;

                            rateioNotaCteProduto.ValorFreteRateio = Math.Round((valorFretePedido / pesoTotalPedido) * rateioNotaCteProduto.Peso, 2, MidpointRounding.AwayFromZero);
                            valorSomadoRateioFreteProduto += rateioNotaCteProduto.ValorFreteRateio;

                            if (cargaPedidoProduto.Equals(ultimoProduto))
                            {
                                decimal diferenca = valorApagarCTe - valorSomadoRateioProduto;
                                rateioNotaCteProduto.ValorTotalRateio += diferenca;
                                decimal diferencaICMS = valorISSCTe - valorSomaICMS;
                                rateioNotaCteProduto.ValorICMS += diferencaICMS;
                                decimal diferencaFreteLiquido = valorFreteCTe - valorSomadoRateioFreteProduto;
                                rateioNotaCteProduto.ValorFreteRateio += diferencaFreteLiquido;
                            }

                            if (cargaPedido.ICMSPagoPorST)
                                rateioNotaCteProduto.ValorICMSST = rateioNotaCteProduto.ValorICMS;
                            else
                                rateioNotaCteProduto.ValorICMSST = 0;


                            rateioNotaCteProduto.ValorTotalFrete = valorApagarCTe;

                            rateioNotaCteProduto.ValorUnitarioRateio = Math.Round(rateioNotaCteProduto.ValorTotalRateio / rateioNotaCteProduto.Peso, 6, MidpointRounding.AwayFromZero);
                            rateioNotaCteProduto.ValorUnitarioProduto = cargaPedidoProduto.ValorUnitarioProduto;
                            repRateioCargaPedidoCTeProduto.Inserir(rateioNotaCteProduto);

                        }
                    }
                }
            }

        }

        public void RatearProdutoPorCargaCTe(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
                Repositorio.Embarcador.Rateio.RateioCargaPedidoProduto repRateioCargaPedidoCTeProduto = new Repositorio.Embarcador.Rateio.RateioCargaPedidoProduto(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponentesFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unitOfWork);
                Repositorio.InformacaoCargaCTE repInformacaoCargaCTE = new Repositorio.InformacaoCargaCTE(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscaCte = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
                Repositorio.Embarcador.Rateio.RateioProdutoComponenteFrete repRateioProdutoComponenteFrete = new Repositorio.Embarcador.Rateio.RateioProdutoComponenteFrete(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                Servicos.Embarcador.Carga.ComponetesFrete serComponetesFrete = new ComponetesFrete(unitOfWork);

                decimal valorSomadoRateioProduto = 0;
                decimal valorSomadoRateioFreteProduto = 0;
                decimal valorSomaICMS = 0;
                decimal valorApagarCTe = cargaCTe.CTe?.ValorAReceber ?? cargaCTe.PreCTe.ValorAReceber;
                decimal valorFreteCTe = cargaCTe.CTe?.ValorFrete ?? cargaCTe.PreCTe.ValorFrete;
                decimal valorICMSCTe = cargaCTe.CTe?.ValorICMS ?? cargaCTe.PreCTe.ValorICMS;

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete> cargaCTeComponentesFrete = repCargaCTeComponentesFrete.BuscarPorCargaCTe(cargaCTe.Codigo);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosCTe = repCargaPedidoXMLNotaFiscaCte.BuscarCargaPedidoPorCargaCTe(cargaCTe.Codigo);

                decimal pesoTotalCte = 0;
                decimal valorMercadoriaCTe = 0;
                if (cargaCTe.CTe != null)
                {
                    pesoTotalCte = repInformacaoCargaCTE.ObterPesoTotal(cargaCTe.CTe.Codigo) - (from obj in cargaPedidosCTe select obj.Pedido.PesoTotalPaletes).Sum();
                    valorMercadoriaCTe = cargaCTe.CTe.ValorTotalMercadoria;
                }

                if (cargaCTe.CTe != null && cargaCTe.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento)
                { // se for complementar o CT-e não possui o peso, por isso se busca o peso dos produtos dos pedidos
                    pesoTotalCte = 0;
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidosCTe)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = repCargaPedidoProduto.BuscarPorCargaPedido(cargaPedido.Codigo);
                        if (configuracao.UtilizarPesoEmbalagemProdutoParaRateio)
                            pesoTotalCte += (from obj in cargaPedidoProdutos select obj.PesoTotalEmbalagem).Sum();
                        else
                            pesoTotalCte += (from obj in cargaPedidoProdutos select ((obj.PesoUnitario * obj.Quantidade) + obj.PesoTotalEmbalagem)).Sum();
                    }
                }

                if (cargaPedidosCTe.Count <= 0)
                    return;

                Dominio.Entidades.Embarcador.Cargas.CargaPedido ultimaCargaPedido = cargaPedidosCTe.Last();
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidosCTe)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = repCargaPedidoProduto.BuscarPorCargaPedido(cargaPedido.Codigo);

                    if (cargaPedido.Carga.TipoRateioProdutos == Dominio.Enumeradores.TipoRateioProdutos.Valor)
                    {
                        decimal valorTotalPedido = (from obj in cargaPedidoProdutos select (obj.ValorUnitarioProduto * obj.Quantidade)).Sum();
                        if (valorMercadoriaCTe > 0 && valorTotalPedido > 0)
                        {
                            decimal valorPagarPedido = (valorApagarCTe / valorMercadoriaCTe) * valorTotalPedido;
                            decimal valorFretePedido = Math.Round((valorFreteCTe / valorMercadoriaCTe) * valorTotalPedido, 2, MidpointRounding.AwayFromZero);
                            decimal valorICMSPedido = (valorICMSCTe / valorMercadoriaCTe) * valorTotalPedido;

                            Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto ultimoProduto = cargaPedidoProdutos.Last();
                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto in cargaPedidoProdutos)
                            {
                                Dominio.Entidades.Embarcador.Rateio.RateioCargaPedidoProduto rateioNotaCteProduto = new Dominio.Entidades.Embarcador.Rateio.RateioCargaPedidoProduto();
                                rateioNotaCteProduto.CargaPedido = cargaPedido;
                                rateioNotaCteProduto.CargaCTe = cargaCTe;
                                rateioNotaCteProduto.Empresa = cargaPedido.Carga.Empresa;
                                rateioNotaCteProduto.Cliente = cargaPedido.Pedido.Destinatario;


                                decimal valorPedagio = (from obj in cargaCTeComponentesFrete where obj.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO select obj.ValorComponente).Sum();
                                rateioNotaCteProduto.ValorPedagio = Math.Round(valorPedagio, 3, MidpointRounding.AwayFromZero);

                                rateioNotaCteProduto.Quantidade = cargaPedidoProduto.Quantidade;
                                if (rateioNotaCteProduto.Quantidade == 0)
                                    rateioNotaCteProduto.Quantidade = 1;
                                //rateioNotaCteProduto.Peso = ((rateioNotaCteProduto.Quantidade * cargaPedidoProduto.PesoUnitario) + cargaPedidoProduto.PesoTotalEmbalagem);
                                rateioNotaCteProduto.ValorUnitarioProduto = cargaPedidoProduto.ValorUnitarioProduto * cargaPedidoProduto.Quantidade;
                                rateioNotaCteProduto.ProdutoEmbarcador = cargaPedidoProduto.Produto;

                                rateioNotaCteProduto.ValorTotalRateio = Math.Round((valorPagarPedido / valorTotalPedido) * rateioNotaCteProduto.ValorUnitarioProduto, 2, MidpointRounding.AwayFromZero);
                                valorSomadoRateioProduto += rateioNotaCteProduto.ValorTotalRateio;

                                rateioNotaCteProduto.ValorICMS = Math.Round((valorICMSPedido / valorTotalPedido) * rateioNotaCteProduto.ValorUnitarioProduto, 2, MidpointRounding.AwayFromZero);
                                valorSomaICMS += rateioNotaCteProduto.ValorICMS;

                                rateioNotaCteProduto.ValorFreteRateio = Math.Round((valorFretePedido / valorTotalPedido) * rateioNotaCteProduto.ValorUnitarioProduto, 2, MidpointRounding.AwayFromZero);
                                valorSomadoRateioFreteProduto += rateioNotaCteProduto.ValorFreteRateio;

                                if (cargaPedido.Equals(ultimaCargaPedido) && cargaPedidoProduto.Equals(ultimoProduto))
                                {
                                    decimal diferenca = valorApagarCTe - valorSomadoRateioProduto;
                                    rateioNotaCteProduto.ValorTotalRateio += diferenca;
                                    decimal diferencaICMS = valorICMSCTe - valorSomaICMS;
                                    rateioNotaCteProduto.ValorICMS += diferencaICMS;
                                    decimal diferencaFreteLiquido = valorFreteCTe - valorSomadoRateioFreteProduto;
                                    rateioNotaCteProduto.ValorFreteRateio += diferencaFreteLiquido;
                                }

                                if (cargaPedido.ICMSPagoPorST)
                                    rateioNotaCteProduto.ValorICMSST = rateioNotaCteProduto.ValorICMS;
                                else
                                    rateioNotaCteProduto.ValorICMSST = 0;


                                rateioNotaCteProduto.ValorTotalFrete = valorApagarCTe;

                                rateioNotaCteProduto.ValorUnitarioRateio = Math.Round(rateioNotaCteProduto.ValorTotalRateio / rateioNotaCteProduto.ValorUnitarioProduto, 6, MidpointRounding.AwayFromZero);
                                rateioNotaCteProduto.ValorUnitarioProduto = cargaPedidoProduto.ValorUnitarioProduto;
                                repRateioCargaPedidoCTeProduto.Inserir(rateioNotaCteProduto);

                                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete cargaCTeComponenteFrete in cargaCTeComponentesFrete)
                                {
                                    decimal valorComponentePorPedido = (cargaCTeComponenteFrete.ValorComponente / valorMercadoriaCTe) * valorTotalPedido;
                                    decimal valorComponente = Math.Round((valorComponentePorPedido / valorTotalPedido) * rateioNotaCteProduto.ValorUnitarioProduto, 2, MidpointRounding.AwayFromZero);
                                    if (cargaPedido.Equals(ultimaCargaPedido) && cargaPedidoProduto.Equals(ultimoProduto))
                                    {
                                        decimal valorTotalComponente = (from obj in cargaCTeComponentesFrete where obj.TipoComponenteFrete == cargaCTeComponenteFrete.TipoComponenteFrete && (cargaCTeComponenteFrete.ComponenteFrete == null || obj.ComponenteFrete.Equals(cargaCTeComponenteFrete.ComponenteFrete)) select obj.ValorComponente).Sum();
                                        decimal valorTotalCargaPedidoComponente = repRateioProdutoComponenteFrete.BuscarTotalPorCargaCTeCompomente(cargaCTe.Codigo, cargaCTeComponenteFrete.TipoComponenteFrete, cargaCTeComponenteFrete.ComponenteFrete) + valorComponente;
                                        valorComponente += valorTotalComponente - valorTotalCargaPedidoComponente;
                                    }
                                    serComponetesFrete.AdicionarRateoProdutoComplementoFrete(rateioNotaCteProduto, valorComponente, cargaCTeComponenteFrete.TipoComponenteFrete, cargaCTeComponenteFrete.ComponenteFrete, cargaCTeComponenteFrete.DescontarValorTotalAReceber, unitOfWork);
                                }
                            }
                        }
                    }
                    else
                    {
                        decimal pesoTotalPedido = 0;
                        if (configuracao.UtilizarPesoEmbalagemProdutoParaRateio)
                        {
                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaProdutos = repCargaPedidoProduto.BuscarPorCarga(carga.Codigo);

                            pesoTotalPedido = (from obj in cargaProdutos select obj.PesoTotalEmbalagem).Sum();
                            pesoTotalCte = (from obj in cargaPedidoProdutos select obj.PesoTotalEmbalagem).Sum();
                        }
                        else
                            pesoTotalPedido = (from obj in cargaPedidoProdutos select ((obj.PesoUnitario * obj.Quantidade) + obj.PesoTotalEmbalagem)).Sum();

                        if (pesoTotalCte > 0 && pesoTotalPedido > 0)
                        {
                            decimal valorPagarPedido = configuracao.UtilizarPesoEmbalagemProdutoParaRateio ? carga.ValorFreteAPagar : (valorApagarCTe / pesoTotalCte) * pesoTotalPedido;
                            decimal valorFretePedido = configuracao.UtilizarPesoEmbalagemProdutoParaRateio ? carga.ValorFrete : (valorFreteCTe / pesoTotalCte) * pesoTotalPedido;
                            decimal valorICMSPedido = configuracao.UtilizarPesoEmbalagemProdutoParaRateio ? carga.ValorICMS : (valorICMSCTe / pesoTotalCte) * pesoTotalPedido;

                            if (cargaCTeComplementoInfo?.CargaOcorrencia != null && configuracao.UtilizarPesoEmbalagemProdutoParaRateio)
                            {
                                valorPagarPedido = (cargaCTeComplementoInfo.CTe.ValorAReceber / pesoTotalCte) * pesoTotalPedido;
                                valorFretePedido = valorPagarPedido;
                            }

                            Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto ultimoProduto = cargaPedidoProdutos.Last();
                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto in cargaPedidoProdutos)
                            {
                                Dominio.Entidades.Embarcador.Rateio.RateioCargaPedidoProduto rateioNotaCteProduto = new Dominio.Entidades.Embarcador.Rateio.RateioCargaPedidoProduto();
                                rateioNotaCteProduto.CargaPedido = cargaPedido;
                                rateioNotaCteProduto.CargaCTe = cargaCTe;
                                rateioNotaCteProduto.Empresa = cargaPedido.Carga.Empresa;
                                rateioNotaCteProduto.CargaCTeComplementoInfo = cargaCTeComplementoInfo;
                                rateioNotaCteProduto.Cliente = cargaPedido.Pedido.Destinatario;


                                decimal valorPedagio = (from obj in cargaCTeComponentesFrete where obj.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO select obj.ValorComponente).Sum();
                                rateioNotaCteProduto.ValorPedagio = Math.Round(valorPedagio, 3, MidpointRounding.AwayFromZero);

                                rateioNotaCteProduto.Quantidade = cargaPedidoProduto.Quantidade;
                                if (rateioNotaCteProduto.Quantidade == 0)
                                    rateioNotaCteProduto.Quantidade = 1;


                                if (configuracao.UtilizarPesoEmbalagemProdutoParaRateio)
                                    rateioNotaCteProduto.Peso = cargaPedidoProduto.PesoTotalEmbalagem;
                                else
                                    rateioNotaCteProduto.Peso = ((rateioNotaCteProduto.Quantidade * cargaPedidoProduto.PesoUnitario) + cargaPedidoProduto.PesoTotalEmbalagem);
                                rateioNotaCteProduto.ProdutoEmbarcador = cargaPedidoProduto.Produto;

                                rateioNotaCteProduto.ValorTotalRateio = Math.Round((valorPagarPedido / pesoTotalPedido) * rateioNotaCteProduto.Peso, 2, MidpointRounding.AwayFromZero);
                                valorSomadoRateioProduto += rateioNotaCteProduto.ValorTotalRateio;

                                rateioNotaCteProduto.ValorICMS = Math.Round((valorICMSPedido / pesoTotalPedido) * rateioNotaCteProduto.Peso, 2, MidpointRounding.AwayFromZero);
                                valorSomaICMS += rateioNotaCteProduto.ValorICMS;

                                rateioNotaCteProduto.ValorFreteRateio = Math.Round((valorFretePedido / pesoTotalPedido) * rateioNotaCteProduto.Peso, 2, MidpointRounding.AwayFromZero);
                                valorSomadoRateioFreteProduto += rateioNotaCteProduto.ValorFreteRateio;

                                if (cargaPedido.Equals(ultimaCargaPedido) && cargaPedidoProduto.Equals(ultimoProduto) && !configuracao.UtilizarPesoEmbalagemProdutoParaRateio)
                                {
                                    decimal diferenca = valorApagarCTe - valorSomadoRateioProduto;
                                    rateioNotaCteProduto.ValorTotalRateio += diferenca;
                                    decimal diferencaICMS = valorICMSCTe - valorSomaICMS;
                                    rateioNotaCteProduto.ValorICMS += diferencaICMS;
                                    decimal diferencaFreteLiquido = valorFreteCTe - valorSomadoRateioFreteProduto;
                                    rateioNotaCteProduto.ValorFreteRateio += diferencaFreteLiquido;
                                }

                                if (cargaPedido.ICMSPagoPorST)
                                    rateioNotaCteProduto.ValorICMSST = rateioNotaCteProduto.ValorICMS;
                                else
                                    rateioNotaCteProduto.ValorICMSST = 0;


                                rateioNotaCteProduto.ValorTotalFrete = valorApagarCTe;
                                if (rateioNotaCteProduto.Peso > 0)
                                    rateioNotaCteProduto.ValorUnitarioRateio = Math.Round(rateioNotaCteProduto.ValorTotalRateio / rateioNotaCteProduto.Peso, 6, MidpointRounding.AwayFromZero);

                                rateioNotaCteProduto.ValorUnitarioProduto = cargaPedidoProduto.ValorUnitarioProduto;
                                repRateioCargaPedidoCTeProduto.Inserir(rateioNotaCteProduto);

                                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete cargaCTeComponenteFrete in cargaCTeComponentesFrete)
                                {
                                    decimal valorComponentePorPedido = (cargaCTeComponenteFrete.ValorComponente / pesoTotalCte) * pesoTotalPedido;

                                    if (configuracao.UtilizarPesoEmbalagemProdutoParaRateio)
                                    {
                                        List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> componenteCarga = repCargaComponentesFrete.BuscarPorCargaPorCompomente(carga.Codigo, cargaCTeComponenteFrete.TipoComponenteFrete, cargaCTeComponenteFrete.ComponenteFrete);
                                        if (componenteCarga != null && componenteCarga.Count > 0)
                                            valorComponentePorPedido = componenteCarga.Sum(o => o.ValorComponente);
                                    }

                                    decimal valorComponente = Math.Round((valorComponentePorPedido / pesoTotalPedido) * rateioNotaCteProduto.Peso, 2, MidpointRounding.AwayFromZero);
                                    if (cargaPedido.Equals(ultimaCargaPedido) && cargaPedidoProduto.Equals(ultimoProduto) && !configuracao.UtilizarPesoEmbalagemProdutoParaRateio)
                                    {
                                        decimal valorTotalComponente = (from obj in cargaCTeComponentesFrete where obj.TipoComponenteFrete == cargaCTeComponenteFrete.TipoComponenteFrete && (cargaCTeComponenteFrete.ComponenteFrete == null || obj.ComponenteFrete.Equals(cargaCTeComponenteFrete.ComponenteFrete)) select obj.ValorComponente).Sum();
                                        decimal valorTotalCargaPedidoComponente = repRateioProdutoComponenteFrete.BuscarTotalPorCargaCTeCompomente(cargaCTe.Codigo, cargaCTeComponenteFrete.TipoComponenteFrete, cargaCTeComponenteFrete.ComponenteFrete) + valorComponente;
                                        valorComponente += valorTotalComponente - valorTotalCargaPedidoComponente;
                                    }
                                    serComponetesFrete.AdicionarRateoProdutoComplementoFrete(rateioNotaCteProduto, valorComponente, cargaCTeComponenteFrete.TipoComponenteFrete, cargaCTeComponenteFrete.ComponenteFrete, cargaCTeComponenteFrete.DescontarValorTotalAReceber, unitOfWork);
                                }
                            }
                        }
                    }
                }
            }

        }

        public void RatearValePedagioPorCarga(int codigoCarga, decimal valorValePedagio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
            Repositorio.Embarcador.Rateio.RateioCargaPedidoProduto repRateioCargaPedidoCTeProduto = new Repositorio.Embarcador.Rateio.RateioCargaPedidoProduto(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedidos = repCargaPedido.BuscarPorCarga(codigoCarga);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = repCargaPedidoProduto.BuscarPorCarga(codigoCarga);
            decimal valorTotalItens = (from obj in cargaPedidoProdutos select (obj.ValorUnitarioProduto * obj.Quantidade)).Sum();
            decimal pesoTotalItens = 0;
            if (configuracao.UtilizarPesoEmbalagemProdutoParaRateio)
                pesoTotalItens = (from obj in cargaPedidoProdutos select obj.PesoTotalEmbalagem).Sum();
            else
                pesoTotalItens = (from obj in cargaPedidoProdutos select ((obj.PesoUnitario * obj.Quantidade) + obj.PesoTotalEmbalagem)).Sum();

            decimal somaRateioValePedagio = 0;
            int countRegistro = 0;
            int totaisRegistros = repRateioCargaPedidoCTeProduto.ContarPorCarga(codigoCarga);
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in listaCargaPedidos)
            {
                List<Dominio.Entidades.Embarcador.Rateio.RateioCargaPedidoProduto> listaRateioNotaCteProduto = repRateioCargaPedidoCTeProduto.BuscarPorProtocoloPedido(cargaPedido.Pedido.Codigo);

                foreach (Dominio.Entidades.Embarcador.Rateio.RateioCargaPedidoProduto rateio in listaRateioNotaCteProduto)
                {
                    countRegistro += 1;

                    rateio.ValorValePedagioRateio = RatearValorValePedagio(valorValePedagio, valorTotalItens, pesoTotalItens, rateio.ValorUnitarioProduto * rateio.Quantidade, rateio.Peso, rateio.CargaPedido.Carga.TipoRateioProdutos);
                    somaRateioValePedagio += rateio.ValorValePedagioRateio;
                    if (countRegistro >= totaisRegistros)
                    {
                        decimal diferenca = valorValePedagio - somaRateioValePedagio;
                        rateio.ValorValePedagioRateio += diferenca;
                    }
                    repRateioCargaPedidoCTeProduto.Atualizar(rateio);
                }
            }
        }

        private decimal RatearValorValePedagio(decimal valorValePedagio, decimal valorTotal, decimal pesoTotal, decimal valorProduto, decimal pesoProduto, Dominio.Enumeradores.TipoRateioProdutos tipoRateio)
        {
            decimal valorRateado = 0;

            if (valorValePedagio == 0)
                return 0;

            if (tipoRateio == Dominio.Enumeradores.TipoRateioProdutos.Peso)
            {
                if (pesoTotal > 0 && pesoProduto > 0)
                {
                    decimal fatorCalculo = pesoProduto / pesoTotal;

                    valorRateado = Math.Round(valorValePedagio * fatorCalculo, 2, MidpointRounding.AwayFromZero);
                }
            }
            else if (valorTotal > 0 && valorProduto > 0)
            {
                decimal fatorCalculo = valorProduto / valorTotal;

                valorRateado = Math.Round(valorValePedagio * fatorCalculo, 2, MidpointRounding.AwayFromZero);
            }

            return valorRateado;
        }

        //public void RatearProdutoPorCargaNFEs(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaNFS cargaNFS, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        //{
        //    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
        //    {
        //        Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalNFS repCargaPedidoXMLNotaFiscalNFS = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalNFS(unitOfWork);
        //        Repositorio.Embarcador.Cargas.CargaNFSComponentesFrete repCargaNFSComponentesFrete = new Repositorio.Embarcador.Cargas.CargaNFSComponentesFrete(unitOfWork);
        //        Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
        //        Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
        //        Repositorio.Embarcador.Rateio.RateioCargaPedidoProduto repRateioCargaPedidoCTeProduto = new Repositorio.Embarcador.Rateio.RateioCargaPedidoProduto(unitOfWork);
        //        Repositorio.Embarcador.Rateio.RateioProdutoComponenteFrete repRateioProdutoComponenteFrete = new Repositorio.Embarcador.Rateio.RateioProdutoComponenteFrete(unitOfWork);

        //        Servicos.Embarcador.Carga.ComponetesFrete serComponetesFrete = new ComponetesFrete(unitOfWork);

        //        decimal valorSomadoRateioProduto = 0;
        //        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosNFS = repCargaPedidoXMLNotaFiscalNFS.BuscarCargaPedidosPorCargaNFS(cargaNFS.Codigo);
        //        List<Dominio.Entidades.Embarcador.Cargas.CargaNFSComponentesFrete> cargaNFSComponentesFrete = repCargaNFSComponentesFrete.BuscarPorCargaNFS(cargaNFS.Codigo);

        //        decimal valorApagarNFS = cargaNFS.ValorFrete;
        //        decimal pesoTotalNFS = cargaNFS.Peso - (from obj in cargaPedidosNFS select obj.Pedido.PesoTotalPaletes).Sum();
        //        Dominio.Entidades.Embarcador.Cargas.CargaPedido ultimaCargaPedido = cargaPedidosNFS.Last();

        //        foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidosNFS)
        //        {
        //            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = repCargaPedidoProduto.BuscarPorCargaPedido(cargaPedido.Codigo);
        //            decimal pesoTotalPedido = (from obj in cargaPedidoProdutos select ((obj.PesoUnitario * obj.Quantidade) + obj.PesoTotalEmbalagem)).Sum();
        //            decimal valorPagarPedido = (valorApagarNFS / pesoTotalNFS) * pesoTotalPedido;

        //            Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto ultimoProduto = cargaPedidoProdutos.Last();
        //            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto in cargaPedidoProdutos)
        //            {
        //                Dominio.Entidades.Embarcador.Rateio.RateioCargaPedidoProduto rateioNotaCteProduto = new Dominio.Entidades.Embarcador.Rateio.RateioCargaPedidoProduto();
        //                rateioNotaCteProduto.CargaPedido = cargaPedido;
        //                rateioNotaCteProduto.CargaNFS = cargaNFS;
        //                rateioNotaCteProduto.Empresa = cargaPedido.Carga.Empresa;
        //                rateioNotaCteProduto.Cliente = cargaPedido.Pedido.Destinatario;
        //                decimal valorPedagio = (from obj in cargaNFSComponentesFrete where obj.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO select obj.ValorComponente).Sum();
        //                rateioNotaCteProduto.ValorPedagio = Math.Round(valorPedagio, 3, MidpointRounding.AwayFromZero);
        //                rateioNotaCteProduto.Peso = ((cargaPedidoProduto.Quantidade * cargaPedidoProduto.PesoUnitario) + cargaPedidoProduto.PesoTotalEmbalagem);
        //                rateioNotaCteProduto.Quantidade = cargaPedidoProduto.Quantidade;
        //                rateioNotaCteProduto.ProdutoEmbarcador = cargaPedidoProduto.Produto;
        //                rateioNotaCteProduto.CargaCTeComplementoInfo = cargaCTeComplementoInfo;

        //                rateioNotaCteProduto.ValorTotalRateio = Math.Round((valorPagarPedido / pesoTotalPedido) * rateioNotaCteProduto.Peso, 2, MidpointRounding.AwayFromZero);
        //                valorSomadoRateioProduto += rateioNotaCteProduto.ValorTotalRateio;

        //                rateioNotaCteProduto.ValorTotalFrete = valorApagarNFS;
        //                if (rateioNotaCteProduto.Peso > 0)
        //                    rateioNotaCteProduto.ValorUnitarioRateio = Math.Round(rateioNotaCteProduto.ValorTotalRateio / rateioNotaCteProduto.Peso, 6, MidpointRounding.AwayFromZero);

        //                rateioNotaCteProduto.ValorUnitarioProduto = cargaPedidoProduto.ValorUnitarioProduto;


        //                if (cargaPedido.Equals(ultimaCargaPedido) && cargaPedidoProduto.Equals(ultimoProduto))
        //                {
        //                    decimal diferenca = valorApagarNFS - valorSomadoRateioProduto;
        //                    rateioNotaCteProduto.ValorTotalRateio += diferenca;
        //                }

        //                if (cargaPedido.ICMSPagoPorST)
        //                    rateioNotaCteProduto.ValorICMSST = rateioNotaCteProduto.ValorICMS;
        //                else
        //                    rateioNotaCteProduto.ValorICMSST = 0;


        //                repRateioCargaPedidoCTeProduto.Inserir(rateioNotaCteProduto);

        //                foreach (Dominio.Entidades.Embarcador.Cargas.CargaNFSComponentesFrete cargaCTeComponenteFrete in cargaNFSComponentesFrete)
        //                {
        //                    decimal valorComponente = Math.Round((cargaCTeComponenteFrete.ValorComponente / pesoTotalPedido) * rateioNotaCteProduto.Peso, 2, MidpointRounding.AwayFromZero);
        //                    if (cargaPedido.Equals(ultimaCargaPedido) && cargaPedidoProduto.Equals(ultimoProduto))
        //                    {
        //                        decimal valorTotalComponente = (from obj in cargaNFSComponentesFrete where obj.TipoComponenteFrete == cargaCTeComponenteFrete.TipoComponenteFrete && (cargaCTeComponenteFrete.ComponenteFrete == null || obj.ComponenteFrete.Equals(cargaCTeComponenteFrete.ComponenteFrete)) select obj.ValorComponente).Sum();
        //                        decimal valorTotalCargaPedidoComponente = repRateioProdutoComponenteFrete.BuscarTotalPorCargaNFsCompomente(cargaNFS.Codigo, cargaCTeComponenteFrete.TipoComponenteFrete, cargaCTeComponenteFrete.ComponenteFrete) + valorComponente;
        //                        valorComponente += valorTotalComponente - valorTotalCargaPedidoComponente;
        //                    }
        //                    serComponetesFrete.AdicionarRateoProdutoComplementoFrete(rateioNotaCteProduto, valorComponente, cargaCTeComponenteFrete.TipoComponenteFrete, cargaCTeComponenteFrete.ComponenteFrete, false, unitOfWork);
        //                }
        //            }

        //        }
        //    }
        //}

    }
}
