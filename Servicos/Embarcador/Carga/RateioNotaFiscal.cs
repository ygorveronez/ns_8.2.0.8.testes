using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Servicos.Embarcador.Carga
{
    public class RateioNotaFiscal : ServicoBase
    {
        public RateioNotaFiscal() : base() { }

        public RateioNotaFiscal(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }
        #region Métodos Públicos

        public void RatearFreteCargaPedidoEntreNotas(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidosComponentesFreteCarga, bool rateioFreteFilialEmissora, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            if (configuracao.NaoRatearFreteCargaPedidoEntreNotas)
                return;

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalComponente repXMLNotaFiscalComponente = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalComponente(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponentesFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.ICMS.RegraICMS repRegraICMS = new Repositorio.Embarcador.ICMS.RegraICMS(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repDocumentoEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalContaContabilContabilizacao repPedidoXMLNotaFiscalContaContabilContabilizacao = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalContaContabilContabilizacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao repCargaPedidoContaContabilContabilizacao = new Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaFechamento repCargaFechamento = new Repositorio.Embarcador.Cargas.CargaFechamento(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repCTeParaSubContratacaoPedidoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubcontratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> cargaCTesParaSubContratacao = repPedidoCTeParaSubcontratacao.BuscarPorCarga(carga.Codigo);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> cargaPedidoCTeParaSubContratacaoNotasFiscais = repCTeParaSubContratacaoPedidoNotaFiscal.BuscarPorCarga(carga.Codigo);

            Servicos.Embarcador.Carga.ICMS serCargaICMS = new Servicos.Embarcador.Carga.ICMS(unitOfWork);
            Servicos.Embarcador.Carga.ISS serCargaISS = new Servicos.Embarcador.Carga.ISS(unitOfWork);

            Servicos.Embarcador.Carga.CTe serCargaCte = new CTe(unitOfWork);
            Servicos.Embarcador.Carga.RateioFormula serRateioFormula = new Servicos.Embarcador.Carga.RateioFormula(unitOfWork);
            Servicos.Embarcador.Carga.RateioCTeParaSubcontratacao serRateioCTeParaSubcontratacao = new Servicos.Embarcador.Carga.RateioCTeParaSubcontratacao(unitOfWork);
            Servicos.Embarcador.Carga.FreteCliente serFreteCliente = new FreteCliente(unitOfWork);
            Servicos.Embarcador.Carga.RateioFrete serRateioFrete = new RateioFrete();
            Servicos.Embarcador.Carga.CargaPedido servicoCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
            Servicos.Embarcador.Pedido.PedidoXMLNotaFiscal servicoPedidoXMLNotaFiscal = new Servicos.Embarcador.Pedido.PedidoXMLNotaFiscal(unitOfWork);
            Servicos.Embarcador.Imposto.ImpostoIBSCBS servicoImpostoIBSCBS = new Servicos.Embarcador.Imposto.ImpostoIBSCBS(unitOfWork);

            //verificar se carga esta em um fechamento aguardando rateio..
            bool possuiCargaEmFechamento = false;

            if (repCargaFechamento.CargaEstaEmFechamentoAgRateio(carga.Codigo))
                possuiCargaEmFechamento = true;


            if (!rateioFreteFilialEmissora)
                repPedidoXMLNotaFiscalContaContabilContabilizacao.DeletarPorCarga(carga.Codigo);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = repCargaPedidoProduto.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosLiberados = (from obj in cargaPedidos where !obj.PedidoSemNFe select obj).ToList(); //Servicos.Embarcador.Carga.CargaPedido.ObterCargaPedidosParaRateio(carga, unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao> cargaPedidoContaContabilContabilizacaos = repCargaPedidoContaContabilContabilizacao.BuscarPorCarga(carga.Codigo);
            if (cargaPedidosLiberados.Count == 0)
                return;

            decimal valorFixoSubContratacaoParcial = 0m;
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                valorFixoSubContratacaoParcial = serFreteCliente.ObterValorFixoSubContratacaoParcial(carga, cargaPedidosLiberados);

            decimal totalICMSCarga = 0m;
            decimal totalPisCarga = 0m;
            decimal totalCofinsCarga = 0m;
            decimal totalISSCarga = 0m;
            decimal totalRetencaoISSCarga = 0m;
            decimal totalValorIBSEstadualCarga = 0m;
            decimal totalValorIBSMunicipalCarga = 0m;
            decimal totalValorCBSCarga = 0m;

            bool rateouEntreNotas = false;
            Dominio.Entidades.Embarcador.Cargas.CargaPedido ultimaCargaPedido = cargaPedidosLiberados.Last();
            bool existeEmissaoPorNota = false;

            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteICMS = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS);
            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componentePisCofins = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS);

            repPedidoXMLNotaFiscalComponenteFrete.DeletarPorCarga(carga.Codigo, rateioFreteFilialEmissora);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoPedidoXMLNotsaFiscaisCarga = repPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo);

            //aqui filtrar apenas notas que estao em pedidos com numerocontrole, caso a carga esta em fechamento rateio.
            if (possuiCargaEmFechamento)
            {
                pedidoPedidoXMLNotsaFiscaisCarga = pedidoPedidoXMLNotsaFiscaisCarga.Where(x => x.CargaPedido.Pedido.NumeroControle != null && !string.IsNullOrWhiteSpace(x.XMLNotaFiscal.NumeroControlePedido)).ToList();
            }

            //aqui filtrar para o rateio apenas notas que não sao do tipo factura (notas normais)
            if (carga.EmitindoCRT == false && (carga.TipoOperacao?.TipoOperacaoMercosul ?? false))
            {
                pedidoPedidoXMLNotsaFiscaisCarga = pedidoPedidoXMLNotsaFiscaisCarga.Where(x => ((bool?)x.XMLNotaFiscal.TipoFatura ?? false) == false).ToList();
            }

            if (carga.TipoOperacao?.ConfiguracaoCarga?.TipoOperacaoInternacional ?? false)
                pedidoPedidoXMLNotsaFiscaisCarga = pedidoPedidoXMLNotsaFiscaisCarga.FindAll(x => ((bool?)x.XMLNotaFiscal.TipoFatura ?? false));

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalComponente> componentesXMLNotaFiscal = repXMLNotaFiscalComponente.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentesICMSXMLNotaFiscalExistenteCarga = repPedidoXMLNotaFiscalComponenteFrete.BuscarPorCargaETipo(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS, null, rateioFreteFilialEmissora);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentesPisConfisXMLNotaFiscalExistenteCarga = repPedidoXMLNotaFiscalComponenteFrete.BuscarPorCargaETipo(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS, null, rateioFreteFilialEmissora);
            List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> documentosProvisaoCarga = repDocumentoProvisao.BuscarComNotaFiscalPorCargas((from obj in cargaPedidos select obj.CargaOrigem.Codigo).Distinct().ToList());
            List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> documentosEmissaoNFSCarga = repDocumentoEmissaoNFSManual.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem = Servicos.Embarcador.Carga.Carga.ObterCargasOrigem(carga, unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscaisAdicionarParaProvisao = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            bool abriuTransacao = false;

            if (!unitOfWork.IsActiveTransaction())
            {
                unitOfWork.Start();
                abriuTransacao = true;
            }

            List<Dominio.Entidades.Aliquota> aliquotasUfEmpresa = null;
            List<Dominio.Entidades.CFOP> cfops = null;
            List<Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo> pedagioEstadosBaseCalculo = null;
            if (pedidoPedidoXMLNotsaFiscaisCarga.Count() > 50 && tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                Repositorio.Aliquota repAliquota = new Repositorio.Aliquota(unitOfWork);
                Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
                Repositorio.Embarcador.ICMS.PedagioEstadoBaseCalculo repositorioPedagioEstadoBaseCalculo = new Repositorio.Embarcador.ICMS.PedagioEstadoBaseCalculo(unitOfWork);
                aliquotasUfEmpresa = repAliquota.BuscarPorUfEmpresa((rateioFreteFilialEmissora ? carga.EmpresaFilialEmissora : carga.Empresa).Localidade.Estado.Sigla);
                cfops = repCFOP.BuscarTodos();
                pedagioEstadosBaseCalculo = repositorioPedagioEstadoBaseCalculo.BuscarTodos();
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidosLiberados)
            {
                bool produtoEmbarcadorConsultar = false;
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = repCargaPedidoProduto.BuscarProdutoComRegraICMS(cargaPedido.Codigo);

                List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteRateio> componentesFreteRateados = new List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteRateio>();
                Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem = (from obj in cargasOrigem where obj.Codigo == cargaPedido.CargaOrigem.Codigo select obj).FirstOrDefault();

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos tipoEmissaoDocumentos = cargaPedido.TipoRateio;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes tipoEmissaoCTeParticipantes = cargaPedido.TipoEmissaoCTeParticipantes;
                Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaRateio = cargaPedido.FormulaRateio;

                bool emissaoSeraPorNota = serCargaCte.VerificarSeEmissaoSeraPorNota(tipoEmissaoDocumentos) && !cargaPedido.ImpostoInformadoPeloEmbarcador;

                bool tipoEmissaoNaoeUmCTeNota = tipoEmissaoDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalIndividual;

                if (emissaoSeraPorNota)
                    existeEmissaoPorNota = true;

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFrete = (from obj in cargaPedidosComponentesFreteCarga
                                                                                                                     where obj.CargaPedido.Codigo == cargaPedido.Codigo
                                                                                                                     && obj.ComponenteFilialEmissora == rateioFreteFilialEmissora
                                                                                                                     && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS
                                                                                                                     && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS
                                                                                                                     && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS
                                                                                                                     select obj).ToList();

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoPedidoXMLNotsaFiscaisModalidade = null;

                //if (cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupadaEntrePedidos)
                //    pedidoPedidoXMLNotsaFiscaisModalidade = (from obj in pedidoPedidoXMLNotsaFiscaisCarga select obj).ToList();
                //else
                pedidoPedidoXMLNotsaFiscaisModalidade = (from obj in pedidoPedidoXMLNotsaFiscaisCarga where obj.CargaPedido.Codigo == cargaPedido.Codigo select obj).ToList();

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete> modalidades = pedidoPedidoXMLNotsaFiscaisModalidade.Select(obj => obj.XMLNotaFiscal.ModalidadeFrete).Distinct().ToList();

                if (emissaoSeraPorNota)
                {
                    if (!rateioFreteFilialEmissora)
                    {
                        cargaPedido.BaseCalculoICMS = 0m;
                        cargaPedido.ValorICMS = 0m;
                        cargaPedido.ValorPis = 0m;
                        cargaPedido.ValorCofins = 0m;
                        cargaPedido.ValorICMSIncluso = 0m;
                        cargaPedido.ValorISS = 0m;
                        cargaPedido.ValorRetencaoISS = 0m;
                        cargaPedido.BaseCalculoISS = 0m;
                        cargaPedido.ValorCreditoPresumido = 0m;
                        cargaPedido.BaseCalculoIR = 0m;
                        cargaPedido.ValorIR = 0m;

                        servicoCargaPedido.ZerarCamposImpostoIBSCBS(cargaPedido, true);
                    }
                    else
                    {
                        cargaPedido.BaseCalculoICMSFilialEmissora = 0m;
                        cargaPedido.ValorICMSFilialEmissora = 0m;
                        cargaPedido.ValorCreditoPresumidoFilialEmissora = 0m;

                        servicoCargaPedido.ZerarCamposImpostoIBSCBSFilialEmissora(cargaPedido, true);
                    }
                }

                bool possuiCTe = cargaPedido.PossuiCTe;
                bool possuiNFS = cargaPedido.PossuiNFS;
                bool possuiNFSManual = cargaPedido.PossuiNFSManual;

                int numeroNotas = 0;
                int numeroNotasCTeSubContratacao = 0;
                int totalNormalCTesSubContrata = 1;
                int numeroNotasRateio = 0;
                int totalCTes = 1;
                int volumeTotalNotas = 0;
                int volumeTotal = 0;
                int volumeNotasCTeSubcontratacao = 0;

                decimal pesoTotalNotas = 0m;
                decimal pesoLiquidoTotalNotas = 0m;
                decimal metrosCubicosTotaisNotas = 0m;
                decimal pesoNotasCTeSubContratacao = 0m;
                decimal pesoLiquidoNotasCTeSubcontratacao = 0m;
                decimal metrosCubicosTotaisCTeSubcontratacao = 0m;
                decimal valorTotalNF = pedidoPedidoXMLNotsaFiscaisModalidade.Sum(o => o.XMLNotaFiscal.Valor);
                decimal pesoTotal = 0m;
                decimal pesoLiquidoTotal = 0m;
                decimal metrosCubicosTotais = 0m;
                decimal totalICMS = 0m;
                decimal totalPis = 0m;
                decimal totalCofins = 0m;
                decimal totalICMSIncluso = 0m;
                decimal totalCreditoPresumido = 0m;
                decimal totalISS = 0m;
                decimal totalRetencaoISS = 0m;
                decimal totalBaseCalculoISS = 0m;
                decimal totalBaseCalculoICMS = 0m;
                decimal totalFreteCargaPedidoNotas = 0m;
                decimal totalMoedaCargaPedidoNotas = 0m;
                decimal totalICMSFreteCargaPedidoNotas = 0m;
                decimal totalPisFreteCargaPedidoNotas = 0m;
                decimal totalCofinsFreteCargaPedidoNotas = 0m;
                decimal totalICMSInclusoFreteCargaPedidoNotas = 0m;
                decimal totalCreditoPresumidoCargaPedidoNotas = 0m;
                decimal totalBaseCalculoCargaPedidoNotas = 0m;
                decimal totalISSFreteCargaPedidoNotas = 0m;
                decimal totalRetencaoISSCargaPedidoNotas = 0m;
                decimal totalBaseCalculoISSCargaPedidoNotas = 0m;
                decimal totalICMSIncluir = 0m;
                decimal totalICMSInclusoIncluir = 0m;
                decimal totalISSIncluir = 0m;
                decimal totalBaseCalculoIR = 0m;
                decimal totalValorIR = 0m;
                decimal pesoTotalParaCalculoFatorCubagem = 0m;

                decimal totalBaseCalculoIBSCBS = 0m;
                decimal totalValorIBSEstadual = 0m;
                decimal totalValorIBSMunicipal = 0m;
                decimal totalValorCBS = 0m;
                decimal totalBaseCalculoIBSCBSCargaPedidoNotas = 0m;
                decimal totalValorIBSEstadualCargaPedidoNotas = 0m;
                decimal totalValorIBSMunicipalCargaPedidoNotas = 0m;
                decimal totalValorCBSCargaPedidoNotas = 0m;

                decimal valorFretePedido = cargaPedido.ValorFrete;

                if (rateioFreteFilialEmissora)
                    valorFretePedido = cargaPedido.ValorFreteFilialEmissora;

                if (pedidoPedidoXMLNotsaFiscaisModalidade.Count > 0)
                {
                    if (!rateioFreteFilialEmissora) //|| cargaPedido.CargaPedidoFilialEmissora)
                    {
                        cargaPedido.PossuiCTe = false;
                        cargaPedido.PossuiNFS = false;
                        cargaPedido.PossuiNFSManual = false;
                    }

                    if (cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
                    {
                        totalNormalCTesSubContrata = repPedidoCTeParaSubContratacao.ContarCTesPorCargaPedido(cargaPedido.Codigo, true);
                        numeroNotas = (from obj in pedidoPedidoXMLNotsaFiscaisModalidade where obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj).Count();
                        numeroNotasCTeSubContratacao = (from obj in pedidoPedidoXMLNotsaFiscaisModalidade where obj.TipoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj).Count();
                        pesoLiquidoTotalNotas = (from obj in pedidoPedidoXMLNotsaFiscaisModalidade where obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj.XMLNotaFiscal.PesoLiquido).Sum();
                        pesoTotalNotas = (from obj in pedidoPedidoXMLNotsaFiscaisModalidade where obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj.XMLNotaFiscal.Peso).Sum();
                        volumeTotalNotas = (from obj in pedidoPedidoXMLNotsaFiscaisModalidade where obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj.XMLNotaFiscal.Volumes).Sum();
                        volumeTotal = (from obj in pedidoPedidoXMLNotsaFiscaisModalidade where obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj.XMLNotaFiscal.Volumes).Sum();
                        pesoNotasCTeSubContratacao = (from obj in pedidoPedidoXMLNotsaFiscaisModalidade where obj.TipoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj.XMLNotaFiscal.Peso).Sum();
                        pesoLiquidoNotasCTeSubcontratacao = (from obj in pedidoPedidoXMLNotsaFiscaisModalidade where obj.TipoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj.XMLNotaFiscal.PesoLiquido).Sum();
                        volumeNotasCTeSubcontratacao = (from obj in pedidoPedidoXMLNotsaFiscaisModalidade where obj.TipoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj.XMLNotaFiscal.Volumes).Sum();

                        metrosCubicosTotaisNotas = pedidoPedidoXMLNotsaFiscaisModalidade.Where(o => o.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao).Sum(o => o.XMLNotaFiscal.MetrosCubicos);
                        metrosCubicosTotaisCTeSubcontratacao = pedidoPedidoXMLNotsaFiscaisModalidade.Where(o => o.TipoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao).Sum(o => o.XMLNotaFiscal.MetrosCubicos);
                    }
                    else
                    {
                        numeroNotas = pedidoPedidoXMLNotsaFiscaisModalidade.Count();
                        pesoTotalNotas = pedidoPedidoXMLNotsaFiscaisModalidade.Sum(obj => obj.XMLNotaFiscal.Peso);
                        pesoLiquidoTotalNotas = pedidoPedidoXMLNotsaFiscaisModalidade.Sum(obj => obj.XMLNotaFiscal.PesoLiquido);
                        volumeTotalNotas = pedidoPedidoXMLNotsaFiscaisModalidade.Sum(obj => obj.XMLNotaFiscal.Volumes);
                        metrosCubicosTotaisNotas = pedidoPedidoXMLNotsaFiscaisModalidade.Where(o => o.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao).Sum(o => o.XMLNotaFiscal.MetrosCubicos);

                    }
                    numeroNotasRateio = numeroNotas + numeroNotasCTeSubContratacao;
                    pesoTotal = pesoNotasCTeSubContratacao + pesoTotalNotas;
                    pesoLiquidoTotal = pesoLiquidoNotasCTeSubcontratacao + pesoLiquidoTotalNotas;
                    volumeTotal = volumeNotasCTeSubcontratacao + volumeTotalNotas;
                    metrosCubicosTotais = metrosCubicosTotaisCTeSubcontratacao + metrosCubicosTotaisNotas;
                    pesoTotalParaCalculoFatorCubagem = serRateioFormula.ObterPesoTotalCubadoFatorCubagem(pedidoPedidoXMLNotsaFiscaisModalidade);

                    if (valorFixoSubContratacaoParcial > 0m)
                    {
                        numeroNotasRateio = numeroNotas;
                        pesoTotal = pesoTotalNotas;
                        pesoLiquidoTotal = pesoLiquidoTotalNotas;
                        volumeTotal = volumeTotalNotas;
                        metrosCubicosTotais = metrosCubicosTotaisNotas;
                    }

                    if (numeroNotasRateio > 0)
                    {
                        bool contemNotaFiscalSemInscricao = false;
                        if (configuracao.AgruparCTesDiferentesPedidosMesmoDestinatario)
                            contemNotaFiscalSemInscricao = repPedidoXMLNotaFiscal.ContemNotaFiscalSemInscricao(cargaPedido.Codigo);

                        switch (cargaPedido.TipoRateio)
                        {
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupadaEntrePedidos:
                                {
                                    if (!contemNotaFiscalSemInscricao && configuracao.AgruparCTesDiferentesPedidosMesmoDestinatario)
                                        totalCTes = repPedidoXMLNotaFiscal.ContarAgrupadosPorRemetenteEDestinatario(cargaPedido.Codigo, configuracao.AgruparCTesDiferentesPedidosMesmoDestinatario, 0);
                                    else
                                        totalCTes = repPedidoXMLNotaFiscal.ContarAgrupadosPorRemetenteEDestinatario(cargaPedido.Codigo, false, 0);
                                    break;
                                }
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupada:
                                {
                                    if (!contemNotaFiscalSemInscricao && configuracao.AgruparCTesDiferentesPedidosMesmoDestinatario)
                                        totalCTes = repPedidoXMLNotaFiscal.ContarAgrupadosPorRemetenteEDestinatarioETomadorEModalidade(cargaPedido.Codigo, configuracao.AgruparCTesDiferentesPedidosMesmoDestinatario);
                                    else
                                        totalCTes = repPedidoXMLNotaFiscal.ContarAgrupadosPorRemetenteEDestinatarioETomadorEModalidade(cargaPedido.Codigo, false);
                                    break;
                                }
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalIndividual:
                                totalCTes = repPedidoXMLNotaFiscal.ContarPorCargaPedido(cargaPedido.Codigo);
                                break;
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoAgrupado:
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoIndividual:
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.NaoInformado:
                                totalCTes = 1;
                                break;
                            default:
                                break;
                        }
                    }
                }

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentesICMSXMLNotaFiscalExistente = (from obj in componentesICMSXMLNotaFiscalExistenteCarga where obj.PedidoXMLNotaFiscal.CargaPedido.Codigo == cargaPedido.Codigo select obj).ToList();
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentesPisCofinsXMLNotaFiscalExistente = (from obj in componentesPisConfisXMLNotaFiscalExistenteCarga where obj.PedidoXMLNotaFiscal.CargaPedido.Codigo == cargaPedido.Codigo select obj).ToList();

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete ultimaModalidade = modalidades.LastOrDefault();

                if (cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
                {
                    for (var i = modalidades.Count - 1; i >= 0; i--)
                    {
                        if (pedidoPedidoXMLNotsaFiscaisModalidade.Any(o => o.XMLNotaFiscal.ModalidadeFrete == modalidades[i] && o.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao))
                        {
                            ultimaModalidade = modalidades[i];
                            break;
                        }
                    }
                }

                foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete modalidade in modalidades)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoPedidoXMLNotasFiscais = (from obj in pedidoPedidoXMLNotsaFiscaisModalidade where obj.XMLNotaFiscal.ModalidadeFrete == modalidade orderby obj.XMLNotaFiscal.Tomador?.CPF_CNPJ select obj).ToList();

                    if (pedidoPedidoXMLNotasFiscais.Count > 0)
                    {
                        if (numeroNotasRateio > 0)
                        {
                            rateouEntreNotas = true;

                            List<Dominio.ObjetosDeValor.Embarcador.Carga.RateioNotaFiscal.participantesRateio> objRemetentesNormais = new List<Dominio.ObjetosDeValor.Embarcador.Carga.RateioNotaFiscal.participantesRateio>();
                            List<Dominio.ObjetosDeValor.Embarcador.Carga.RateioNotaFiscal.participantesRateio> objDestinatariosNormais = new List<Dominio.ObjetosDeValor.Embarcador.Carga.RateioNotaFiscal.participantesRateio>();

                            List<double> remetentes = new List<double>();
                            List<double> destinatarios = new List<double>();
                            List<double> tomadores = new List<double>();
                            List<double> remetentesNormais = new List<double>();
                            List<double> destinatariosNormais = new List<double>();
                            List<double> remetentesSubContratado = new List<double>();
                            List<double> destinatariosSubContratado = new List<double>();
                            List<string> iesRemetentes = new List<string>();
                            List<string> iesDestinatarios = new List<string>();

                            double ultimoRemetenteNormal = 0d;
                            double ultimoDestinatarioNormal = 0d;
                            double ultimoTomadorNormal = 0d;
                            double ultimoRemetenteSubContratado = 0d;
                            double ultimoDestinatarioSubContratado = 0d;
                            double ultimoTomadorSubContratado = 0d;
                            string ieUltimoRemetente = "";
                            string ieUltimoDestinatario = "";
                            bool contemNotaFiscalSemInscricao = false;
                            if (configuracao.AgruparCTesDiferentesPedidosMesmoDestinatario)
                                contemNotaFiscalSemInscricao = repPedidoXMLNotaFiscal.ContemNotaFiscalSemInscricao(cargaPedido.Codigo);

                            //se a formula de rateio for por CT-e é necessário validar a quantidade de CT-es que serão gerados caso a emissão seja por nota fiscal agrupada.
                            if ((formulaRateio != null && formulaRateio.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porCTe || cargaPedidoComponentesFrete.Any(obj => obj.RateioFormula != null && obj.RateioFormula.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porCTe))
                                || cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupada || cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupadaEntrePedidos)
                            {
                                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoPedidoXMLNotasFiscais)
                                {
                                    if (pedidoXMLNotaFiscal.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao)
                                    {
                                        double cpfCnpjTomador = pedidoXMLNotaFiscal.XMLNotaFiscal.Tomador?.CPF_CNPJ ?? 0D;

                                        if (!remetentesNormais.Contains(pedidoXMLNotaFiscal.XMLNotaFiscal.Emitente.CPF_CNPJ))
                                        {
                                            remetentesNormais.Add(pedidoXMLNotaFiscal.XMLNotaFiscal.Emitente.CPF_CNPJ);
                                            ultimoRemetenteNormal = pedidoXMLNotaFiscal.XMLNotaFiscal.Emitente.CPF_CNPJ;
                                        }

                                        if (objRemetentesNormais.FirstOrDefault(x => x.remetente == pedidoXMLNotaFiscal.XMLNotaFiscal.Emitente.CPF_CNPJ) == null)
                                            objRemetentesNormais.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.RateioNotaFiscal.participantesRateio() { tomador = cpfCnpjTomador, remetente = pedidoXMLNotaFiscal.XMLNotaFiscal.Emitente.CPF_CNPJ });

                                        if (!configuracao.AgruparCTesDiferentesPedidosMesmoDestinatario || contemNotaFiscalSemInscricao)
                                        {
                                            if (!iesRemetentes.Contains(""))
                                            {
                                                iesRemetentes.Add("");
                                                ieUltimoRemetente = "";
                                            }
                                        }
                                        else if (!iesRemetentes.Contains(pedidoXMLNotaFiscal.XMLNotaFiscal.IERemetente))
                                        {
                                            iesRemetentes.Add(pedidoXMLNotaFiscal.XMLNotaFiscal.IERemetente);
                                            ieUltimoRemetente = pedidoXMLNotaFiscal.XMLNotaFiscal.IERemetente;
                                        }

                                        if (!destinatariosNormais.Contains(pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario.CPF_CNPJ))
                                        {
                                            destinatariosNormais.Add(pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario.CPF_CNPJ);
                                            ultimoDestinatarioNormal = pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario.CPF_CNPJ;
                                        }

                                        if (objDestinatariosNormais.FirstOrDefault(x => x.destinatario == pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario.CPF_CNPJ) == null)
                                            objDestinatariosNormais.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.RateioNotaFiscal.participantesRateio() { tomador = cpfCnpjTomador, remetente = pedidoXMLNotaFiscal.XMLNotaFiscal.Emitente.CPF_CNPJ, destinatario = pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario.CPF_CNPJ });

                                        if (!configuracao.AgruparCTesDiferentesPedidosMesmoDestinatario || contemNotaFiscalSemInscricao)
                                        {
                                            if (!iesDestinatarios.Contains(""))
                                            {
                                                iesDestinatarios.Add("");
                                                ieUltimoDestinatario = "";
                                            }
                                        }
                                        else if (!iesDestinatarios.Contains(pedidoXMLNotaFiscal.XMLNotaFiscal.IEDestinatario))
                                        {
                                            iesDestinatarios.Add(pedidoXMLNotaFiscal.XMLNotaFiscal.IEDestinatario);
                                            ieUltimoDestinatario = pedidoXMLNotaFiscal.XMLNotaFiscal.IEDestinatario;
                                        }

                                        if (!tomadores.Contains(cpfCnpjTomador))
                                        {
                                            tomadores.Add(cpfCnpjTomador);
                                            ultimoTomadorNormal = cpfCnpjTomador;
                                        }

                                    }
                                    else
                                    {
                                        if (!remetentesSubContratado.Contains(pedidoXMLNotaFiscal.XMLNotaFiscal.Emitente.CPF_CNPJ))
                                        {
                                            remetentesSubContratado.Add(pedidoXMLNotaFiscal.XMLNotaFiscal.Emitente.CPF_CNPJ);
                                            ultimoRemetenteSubContratado = pedidoXMLNotaFiscal.XMLNotaFiscal.Emitente.CPF_CNPJ;
                                        }

                                        if (!destinatariosSubContratado.Contains(pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario.CPF_CNPJ))
                                        {
                                            destinatariosSubContratado.Add(pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario.CPF_CNPJ);
                                            ultimoDestinatarioSubContratado = pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario.CPF_CNPJ;
                                        }

                                        double cpfCnpjTomador = pedidoXMLNotaFiscal.XMLNotaFiscal.Tomador?.CPF_CNPJ ?? 0D;
                                        if (!tomadores.Contains(cpfCnpjTomador))
                                        {
                                            tomadores.Add(cpfCnpjTomador);
                                            ultimoTomadorSubContratado = cpfCnpjTomador;
                                        }
                                    }
                                }

                                remetentes.AddRange(remetentesNormais);
                                foreach (double sub in remetentesSubContratado)
                                {
                                    if (!remetentes.Contains(sub))
                                        remetentes.Add(sub);
                                }

                                destinatarios.AddRange(destinatariosNormais);
                                foreach (double dest in destinatariosSubContratado)
                                {
                                    if (!destinatarios.Contains(dest))
                                        destinatarios.Add(dest);
                                }
                            }
                            else
                            {
                                objRemetentesNormais.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.RateioNotaFiscal.participantesRateio() { tomador = 0d, remetente = 0d, destinatario = 0d });
                                objDestinatariosNormais.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.RateioNotaFiscal.participantesRateio() { tomador = 0d, remetente = 0d, destinatario = 0d });
                                tomadores.Add(0d);
                                remetentes.Add(0d);
                                destinatarios.Add(0d);
                                remetentesNormais.Add(0d);
                                destinatariosNormais.Add(0d);
                                remetentesSubContratado.Add(0d);
                                destinatariosSubContratado.Add(0d);
                                iesRemetentes.Add("");
                                iesDestinatarios.Add("");
                            }

                            double ultimoTomador = tomadores.LastOrDefault();
                            double ultimoRemetente = objRemetentesNormais.LastOrDefault(x => x.tomador == ultimoTomador)?.remetente ?? 0d;
                            double ultimoDestinatario = objDestinatariosNormais.LastOrDefault(x => x.tomador == ultimoTomador && x.remetente == ultimoRemetente)?.destinatario ?? 0d;

                            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal ultimoPedidoXmlNotaFiscalSubContratacao = null;
                            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal ultimoPedidoXmlNotaFiscalNormal = null;
                            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal ultimoPedidoXmlNotaFiscal = null;

                            if (cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SVMTerceiro ||
                                cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada ||
                                cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho ||
                                cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SVMProprio ||
                                cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario)
                                ultimoPedidoXmlNotaFiscalNormal = ultimoPedidoXmlNotaFiscalSubContratacao;

                            foreach (double tomador in tomadores)
                            {
                                foreach (double remetente in remetentes)
                                {
                                    foreach (string ieRemetente in iesRemetentes)
                                    {
                                        foreach (double destinatario in destinatarios)
                                        {
                                            foreach (string ieDestinatario in iesDestinatarios)
                                            {
                                                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoPedidoXMLNotsaFiscaisAgrupados = null;
                                                if (destinatario > 0d || remetente > 0d)
                                                {
                                                    if (tomador > 0d && !string.IsNullOrWhiteSpace(ieRemetente) && !string.IsNullOrWhiteSpace(ieDestinatario))
                                                        pedidoPedidoXMLNotsaFiscaisAgrupados = (from obj in pedidoPedidoXMLNotasFiscais where obj.XMLNotaFiscal.Tomador != null && obj.XMLNotaFiscal.Tomador.CPF_CNPJ == tomador && obj.XMLNotaFiscal.Emitente.CPF_CNPJ == remetente && obj.XMLNotaFiscal.Destinatario.CPF_CNPJ == destinatario && (obj.XMLNotaFiscal.IEDestinatario == ieDestinatario || obj.XMLNotaFiscal.IEDestinatario == "" || obj.XMLNotaFiscal.IEDestinatario == null) && (obj.XMLNotaFiscal.IERemetente == ieRemetente || obj.XMLNotaFiscal.IERemetente == "" || obj.XMLNotaFiscal.IERemetente == null) select obj).ToList();
                                                    else if (!string.IsNullOrWhiteSpace(ieRemetente) && !string.IsNullOrWhiteSpace(ieDestinatario))
                                                        pedidoPedidoXMLNotsaFiscaisAgrupados = (from obj in pedidoPedidoXMLNotasFiscais where obj.XMLNotaFiscal.Tomador == null && obj.XMLNotaFiscal.Emitente.CPF_CNPJ == remetente && obj.XMLNotaFiscal.Destinatario.CPF_CNPJ == destinatario && (obj.XMLNotaFiscal.IEDestinatario == ieDestinatario || obj.XMLNotaFiscal.IEDestinatario == "" || obj.XMLNotaFiscal.IEDestinatario == null) && (obj.XMLNotaFiscal.IERemetente == ieRemetente || obj.XMLNotaFiscal.IERemetente == "" || obj.XMLNotaFiscal.IERemetente == null) select obj).ToList();
                                                    else if (tomador > 0d)
                                                        pedidoPedidoXMLNotsaFiscaisAgrupados = (from obj in pedidoPedidoXMLNotasFiscais where obj.XMLNotaFiscal.Tomador != null && obj.XMLNotaFiscal.Tomador.CPF_CNPJ == tomador && obj.XMLNotaFiscal.Emitente.CPF_CNPJ == remetente && obj.XMLNotaFiscal.Destinatario.CPF_CNPJ == destinatario select obj).ToList();
                                                    else
                                                        pedidoPedidoXMLNotsaFiscaisAgrupados = (from obj in pedidoPedidoXMLNotasFiscais where obj.XMLNotaFiscal.Tomador == null && obj.XMLNotaFiscal.Emitente.CPF_CNPJ == remetente && obj.XMLNotaFiscal.Destinatario.CPF_CNPJ == destinatario select obj).ToList();
                                                }
                                                else
                                                {
                                                    pedidoPedidoXMLNotsaFiscaisAgrupados = pedidoPedidoXMLNotasFiscais;
                                                }

                                                // Tratamento para evitar valor negativo no componente de frete, pois é adicionada a diferença de valor na última nota fiscal calculada.
                                                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                                                {
                                                    if (formulaRateio == null)
                                                    {
                                                        //Peso é o parâmetro padrão quando não se tem nenhuma formula definida para o cliente.
                                                        pedidoPedidoXMLNotsaFiscaisAgrupados = pedidoPedidoXMLNotsaFiscaisAgrupados.OrderBy(o => o.XMLNotaFiscal.Peso).ToList();
                                                    }
                                                    else
                                                    {
                                                        if (formulaRateio.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.peso)
                                                            pedidoPedidoXMLNotsaFiscaisAgrupados = pedidoPedidoXMLNotsaFiscaisAgrupados.OrderBy(o => o.XMLNotaFiscal.Peso).ToList();
                                                        else if (formulaRateio.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.PesoLiquido)
                                                            pedidoPedidoXMLNotsaFiscaisAgrupados = pedidoPedidoXMLNotsaFiscaisAgrupados.OrderBy(o => o.XMLNotaFiscal.PesoLiquido).ToList();
                                                        else if (formulaRateio.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.ValorMercadoria)
                                                            pedidoPedidoXMLNotsaFiscaisAgrupados = pedidoPedidoXMLNotsaFiscaisAgrupados.OrderBy(o => o.XMLNotaFiscal.Valor).ToList();
                                                        else if (formulaRateio.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.PorPesoUtilizandoFatorCubagem)
                                                            pedidoPedidoXMLNotsaFiscaisAgrupados = pedidoPedidoXMLNotsaFiscaisAgrupados.OrderBy(o => o.XMLNotaFiscal.MetrosCubicos).ToList();
                                                    }

                                                }

                                                if (pedidoPedidoXMLNotsaFiscaisAgrupados.Count > 0)
                                                {
                                                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal ultimoPedidoXmlNotaFiscalNormalAgrupado = (from obj in pedidoPedidoXMLNotsaFiscaisAgrupados where obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj).LastOrDefault();

                                                    if (tomador == ultimoTomador && remetente == ultimoRemetente && destinatario == ultimoDestinatario && modalidade == ultimaModalidade && ieRemetente == ieUltimoRemetente && ieDestinatario == ieUltimoDestinatario)
                                                        ultimoPedidoXmlNotaFiscal = pedidoPedidoXMLNotsaFiscaisAgrupados.LastOrDefault();

                                                    if (tomador == ultimoTomadorNormal && remetente == ultimoRemetenteNormal && ultimoDestinatarioNormal == destinatario && modalidade == ultimaModalidade && ieRemetente == ieUltimoRemetente && ieDestinatario == ieUltimoDestinatario)
                                                        ultimoPedidoXmlNotaFiscalNormal = (from obj in pedidoPedidoXMLNotsaFiscaisAgrupados where obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj).LastOrDefault();

                                                    if (tomador == ultimoTomadorSubContratado && remetente == ultimoRemetenteSubContratado && ultimoDestinatarioSubContratado == destinatario)
                                                        ultimoPedidoXmlNotaFiscalSubContratacao = (from obj in pedidoPedidoXMLNotasFiscais where obj.TipoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj).LastOrDefault();

                                                    decimal SomaValorTotalPorCTe = 0m;
                                                    decimal SomaValorPisTotalPorCTe = 0m;
                                                    decimal SomaValorCofinsTotalPorCTe = 0m;
                                                    decimal SomaValorICMSTotalPorCTe = 0m;
                                                    decimal SomaValorICMSInclusoTotalPorCTe = 0m;
                                                    decimal SomaBaseCalculoTotalPorCTe = 0m;
                                                    decimal SomaValorCreditoPresumidoTotalPorCTe = 0m;
                                                    decimal SomaValorISSTotalPorCTe = 0m;
                                                    decimal SomaBaseCalculoISSTotalPorCTe = 0m;
                                                    decimal SomaValorRetencaoISSTotalPorCTe = 0m;
                                                    decimal somaValorTotalMoedaPorCTe = 0m;

                                                    decimal SomaBaseCalculoIBSCBSTotalPorCTe = 0m;
                                                    decimal SomaValorIBSEstadualTotalPorCTe = 0m;
                                                    decimal SomaValorIBSMunicipalTotalPorCTe = 0m;
                                                    decimal SomaValorCBSTotalPorCTe = 0m;

                                                    bool adicionarDocumentosParaProvisao = false;

                                                    Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoEmissao = null;

                                                    if (cargaPedidoComponentesFrete != null && cargaPedidoComponentesFrete.Any(o => o.PorQuantidadeDocumentos && o.TipoCalculoQuantidadeDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete.PorDocumentoEmitido))
                                                        modeloDocumentoEmissao = ObterModeloDocumentoEmissao(cargaPedido, pedidoPedidoXMLNotsaFiscaisAgrupados.First(), unitOfWork, tipoServicoMultisoftware);

                                                    Servicos.Embarcador.Imposto.ImpostoPisCofins servicoPisCofins = new Servicos.Embarcador.Imposto.ImpostoPisCofins();
                                                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> pedidoXMLNotaFiscalCompontesFreteAgrupados = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete>();

                                                    decimal SomaValorTotalPorCTeAgrupado = 0;
                                                    decimal valorTotalComponentesOriginal = 0;
                                                    for (int i = 0; i < pedidoPedidoXMLNotsaFiscaisAgrupados.Count; i++)
                                                    {
                                                        Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXmlNotaFiscal = pedidoPedidoXMLNotsaFiscaisAgrupados[i];

                                                        pedidoXmlNotaFiscal.PercentualPagamentoAgregado = cargaPedido.PercentualPagamentoAgregado;
                                                        pedidoXmlNotaFiscal.ModeloDocumentoFiscal = cargaPedido.ModeloDocumentoFiscal;

                                                        if (pedidoXmlNotaFiscal.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao)
                                                        {
                                                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProduto = (from obj in cargaPedidoProdutos where obj.CargaPedido.Codigo == pedidoXmlNotaFiscal.CargaPedido.Codigo select obj).ToList();
                                                            decimal densidadeProdutos = (cargaPedidoProduto.Sum(obj => obj.Produto?.MetroCubito) ?? 0m);

                                                            decimal valorTotalMoeda = cargaPedido.ValorTotalMoeda ?? 0m;
                                                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moeda = cargaPedido.Moeda ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real;
                                                            decimal cotacaoMoeda = cargaPedido.ValorCotacaoMoeda ?? 0m;
                                                            decimal valorFreteCTeOriginal = 0;

                                                            AplicarRateioNoPedidoNotaFiscal(valorFretePedido, valorFixoSubContratacaoParcial, totalCTes, numeroNotasRateio, pesoTotal, valorTotalNF, pedidoPedidoXMLNotsaFiscaisAgrupados.Count, ref SomaValorTotalPorCTe, ref totalFreteCargaPedidoNotas, pedidoXmlNotaFiscal, formulaRateio, ultimoPedidoXmlNotaFiscal, ultimoPedidoXmlNotaFiscalNormalAgrupado, ultimoPedidoXmlNotaFiscalNormal, rateioFreteFilialEmissora, metrosCubicosTotais, densidadeProdutos, pesoLiquidoTotal, volumeTotal, moeda, cotacaoMoeda, valorTotalMoeda, ref somaValorTotalMoedaPorCTe, ref totalMoedaCargaPedidoNotas, ref SomaValorTotalPorCTeAgrupado, ref valorFreteCTeOriginal, carga.TipoOperacao?.ConfiguracaoCalculoFrete?.MesclarValorEmbarcadorComTabelaFrete ?? false, tipoEmissaoNaoeUmCTeNota, pesoTotalParaCalculoFatorCubagem);

                                                            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> pedidoXMLNotaFiscalCompontesFrete = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete>();
                                                            decimal baseCalculo = pedidoXmlNotaFiscal.ValorFrete;
                                                            if (rateioFreteFilialEmissora)
                                                                baseCalculo = pedidoXmlNotaFiscal.ValorFreteFilialEmissora;
                                                            decimal baseCalculoIBSCBS = baseCalculo;

                                                            decimal valorTotalComponentes = 0m, valorTotalMoedaComponentes = 0m;
                                                            decimal valorRateioOriginal = 0;

                                                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFrete in cargaPedidoComponentesFrete)
                                                            {
                                                                Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaComponente = cargaPedidoComponenteFrete.RateioFormula ?? formulaRateio;

                                                                if (cargaPedidoComponenteFrete.ModeloDocumentoFiscalRateio == null || ((cargaPedidoComponenteFrete.ModeloDocumentoFiscalRateio.Numero == "57" && (modeloDocumentoEmissao == null || modeloDocumentoEmissao.Numero == "57")) || (modeloDocumentoEmissao != null && modeloDocumentoEmissao.Codigo == cargaPedidoComponenteFrete.ModeloDocumentoFiscalRateio.Codigo)))
                                                                {
                                                                    if (cargaPedidoComponenteFrete.ComponenteFrete == null)
                                                                        Log.GravarAdvertencia("Não existe um componente do tipo " + cargaPedidoComponenteFrete.TipoComponenteFrete.ToString() + " cadastrado, vai da exceção, precisa cadastrar");

                                                                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalComponente xmlNotaFiscalComponente = null;

                                                                    if (componentesXMLNotaFiscal.Any() && !(carga.TipoOperacao?.ConfiguracaoCalculoFrete?.MesclarValorEmbarcadorComTabelaFrete ?? false))
                                                                        xmlNotaFiscalComponente = componentesXMLNotaFiscal.Where(o => o.XMLNotaFiscal.Codigo == pedidoXmlNotaFiscal.XMLNotaFiscal.Codigo && o.ComponenteFrete.Codigo == cargaPedidoComponenteFrete.ComponenteFrete.Codigo).FirstOrDefault();

                                                                    int totalCTesUtilizar = totalCTes;

                                                                    if (cargaPedidoComponenteFrete.PorQuantidadeDocumentos && cargaPedidoComponenteFrete.TipoCalculoQuantidadeDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete.PorDocumentoEmitido)
                                                                        totalCTesUtilizar = cargaPedidoComponenteFrete.QuantidadeTotalDocumentos;

                                                                    decimal valorComponente = 0m, valorMoedaComponente = 0m;
                                                                    bool incluirIntegralmenteContratoFreteTerceiro = false;
                                                                    decimal pesoParaCalculoFatorCubagem = 0;

                                                                    if (formulaComponente?.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.PorPesoUtilizandoFatorCubagem)
                                                                        pesoParaCalculoFatorCubagem = serRateioFormula.ObterPesoCubadoFatorCubagem(formulaRateio.ParametroRateioFormula, pedidoXmlNotaFiscal.CargaPedido.TipoUsoFatorCubagemRateioFormula, pedidoXmlNotaFiscal.CargaPedido.FatorCubagemRateioFormula ?? 0, pedidoXmlNotaFiscal.XMLNotaFiscal.Volumes, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.MetrosCubicos);

                                                                    if (xmlNotaFiscalComponente == null && componentesXMLNotaFiscal.Any() && carga.TipoOperacao?.TipoIntegracao?.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercement)
                                                                        continue;

                                                                    if (xmlNotaFiscalComponente == null)
                                                                    {
                                                                        if (moeda != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real)
                                                                        {
                                                                            valorMoedaComponente = serRateioFormula.AplicarFormulaRateio(formulaComponente, cargaPedidoComponenteFrete.ValorTotalMoeda ?? 0m, numeroNotasRateio, totalCTesUtilizar, pesoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, cargaPedidoComponenteFrete.Percentual, cargaPedidoComponenteFrete.TipoValor, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXmlNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, true, pedidoXmlNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Volumes, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                                                                            valorTotalComponentesOriginal += valorRateioOriginal;

                                                                            if (formulaComponente != null && formulaComponente.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porCTe && !tipoEmissaoNaoeUmCTeNota)// quando o rateio é por cte é necessário dividir o valor por nota para chegar ao valor de cada nota fiscal do CT-e.
                                                                                valorMoedaComponente = Math.Floor(valorMoedaComponente / pedidoPedidoXMLNotsaFiscaisAgrupados.Count * 100) / 100;

                                                                            if (formulaComponente != null && formulaComponente.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porCTe && ultimoPedidoXmlNotaFiscalNormalAgrupado != null && pedidoXmlNotaFiscal.Equals(ultimoPedidoXmlNotaFiscalNormalAgrupado))
                                                                            {
                                                                                decimal valorTotalMoedaComponentePorCTe = Math.Floor(cargaPedidoComponentesFrete.Where(obj => obj.TipoComponenteFrete == cargaPedidoComponenteFrete.TipoComponenteFrete && (cargaPedidoComponenteFrete.ComponenteFrete == null || obj.ComponenteFrete.Equals(cargaPedidoComponenteFrete.ComponenteFrete))).Sum(obj => obj.ValorTotalMoeda ?? 0m) / totalCTesUtilizar * 100) / 100;

                                                                                decimal valorTotalMoedaCargaPedidoComponente1 = componentesFreteRateados.Where(o => o.CodigoCargaPedido == cargaPedido.Codigo &&
                                                                                                                                    o.TipoComponente == cargaPedidoComponenteFrete.TipoComponenteFrete &&
                                                                                                                                    o.CodigoComponenteFrete == (cargaPedidoComponenteFrete.ComponenteFrete?.Codigo ?? 0) &&
                                                                                                                                    ((o.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada &&
                                                                                                                                      o.CPFCNPJRemetente == destinatario &&
                                                                                                                                      o.CPFCNPJDestinatario == remetente) ||
                                                                                                                                     (o.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida &&
                                                                                                                                      o.CPFCNPJRemetente == remetente &&
                                                                                                                                      o.CPFCNPJDestinatario == destinatario))).Sum(o => o.ValorMoeda);

                                                                                valorTotalMoedaCargaPedidoComponente1 += valorMoedaComponente;
                                                                                valorMoedaComponente += valorTotalMoedaComponentePorCTe - valorTotalMoedaCargaPedidoComponente1;
                                                                            }

                                                                            if ((ultimoPedidoXmlNotaFiscal != null && pedidoXmlNotaFiscal.Equals(ultimoPedidoXmlNotaFiscal) && valorFixoSubContratacaoParcial == 0m) || (ultimoPedidoXmlNotaFiscalNormal != null && pedidoXmlNotaFiscal.Equals(ultimoPedidoXmlNotaFiscalNormal) && valorFixoSubContratacaoParcial > 0m))
                                                                            {
                                                                                decimal valorTotalMoedaComponente = cargaPedidoComponentesFrete.Where(obj => obj.TipoComponenteFrete == cargaPedidoComponenteFrete.TipoComponenteFrete && (cargaPedidoComponenteFrete.ComponenteFrete == null || obj.ComponenteFrete.Equals(cargaPedidoComponenteFrete.ComponenteFrete))).Sum(obj => obj.ValorTotalMoeda ?? 0m);

                                                                                decimal valorTotalMoedaCargaPedidoComponente1 = componentesFreteRateados.Where(o => o.CodigoCargaPedido == cargaPedido.Codigo &&
                                                                                                                                                                    o.TipoComponente == cargaPedidoComponenteFrete.TipoComponenteFrete &&
                                                                                                                                                                    o.CodigoComponenteFrete == (cargaPedidoComponenteFrete.ComponenteFrete?.Codigo ?? 0)).Sum(o => o.ValorMoeda);
                                                                                valorTotalMoedaCargaPedidoComponente1 += valorMoedaComponente;
                                                                                valorMoedaComponente += valorTotalMoedaComponente - valorTotalMoedaCargaPedidoComponente1;
                                                                            }

                                                                            if (formulaRateio?.RatearEmBlocoDeEmissao ?? false)
                                                                                valorComponente = Math.Round(valorMoedaComponente * cotacaoMoeda, 3, MidpointRounding.AwayFromZero);
                                                                            else
                                                                                valorComponente = Math.Round(valorMoedaComponente * cotacaoMoeda, 2, MidpointRounding.AwayFromZero);
                                                                        }
                                                                        else
                                                                        {
                                                                            decimal pesoParaCalculo = pesoTotal;
                                                                            int qtdNotasParaCalculo = numeroNotasRateio;

                                                                            valorComponente = serRateioFormula.AplicarFormulaRateio(formulaComponente, cargaPedidoComponenteFrete.ValorComponente, qtdNotasParaCalculo, totalCTesUtilizar, pesoParaCalculo, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, cargaPedidoComponenteFrete.Percentual, cargaPedidoComponenteFrete.TipoValor, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXmlNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, true, pedidoXmlNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Volumes, volumeTotal, null, cargaPedidoComponenteFrete.UtilizarFormulaRateioCarga ?? false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                                                                            valorTotalComponentesOriginal += valorRateioOriginal;

                                                                            if (formulaComponente != null && formulaComponente.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porCTe && !tipoEmissaoNaoeUmCTeNota)// quando o rateio é por cte é necessário dividir o valor por nota para chegar ao valor de cada nota fiscal do CT-e.
                                                                                valorComponente = Math.Floor((valorComponente / pedidoPedidoXMLNotsaFiscaisAgrupados.Count) * 100) / 100;

                                                                            if (formulaComponente != null && formulaComponente.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porCTe && ultimoPedidoXmlNotaFiscalNormalAgrupado != null && pedidoXmlNotaFiscal.Equals(ultimoPedidoXmlNotaFiscalNormalAgrupado) && !tipoEmissaoNaoeUmCTeNota)
                                                                            {
                                                                                decimal valorTotalComponentePorCTe = 0m;

                                                                                if (totalCTesUtilizar > 0)
                                                                                    valorTotalComponentePorCTe = Math.Floor(((from obj in cargaPedidoComponentesFrete where obj.TipoComponenteFrete == cargaPedidoComponenteFrete.TipoComponenteFrete && (cargaPedidoComponenteFrete.ComponenteFrete == null || obj.ComponenteFrete.Equals(cargaPedidoComponenteFrete.ComponenteFrete)) select obj.ValorComponente).Sum() / totalCTesUtilizar) * 100) / 100;

                                                                                decimal valorTotalCargaPedidoComponente1 = componentesFreteRateados.Where(o => o.CodigoCargaPedido == cargaPedido.Codigo &&
                                                                                                                                    o.TipoComponente == cargaPedidoComponenteFrete.TipoComponenteFrete &&
                                                                                                                                    o.CodigoComponenteFrete == (cargaPedidoComponenteFrete.ComponenteFrete?.Codigo ?? 0) &&
                                                                                                                                    ((o.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada &&
                                                                                                                                      o.CPFCNPJRemetente == destinatario &&
                                                                                                                                      o.CPFCNPJDestinatario == remetente) ||
                                                                                                                                     (o.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida &&
                                                                                                                                      o.CPFCNPJRemetente == remetente &&
                                                                                                                                      o.CPFCNPJDestinatario == destinatario))).Sum(o => o.Valor);

                                                                                valorTotalCargaPedidoComponente1 += valorComponente;
                                                                                valorComponente += valorTotalComponentePorCTe - valorTotalCargaPedidoComponente1;
                                                                            }

                                                                            if ((ultimoPedidoXmlNotaFiscal != null && pedidoXmlNotaFiscal.Equals(ultimoPedidoXmlNotaFiscal) && valorFixoSubContratacaoParcial == 0m) || (ultimoPedidoXmlNotaFiscalNormal != null && pedidoXmlNotaFiscal.Equals(ultimoPedidoXmlNotaFiscalNormal) && valorFixoSubContratacaoParcial > 0m))
                                                                            {
                                                                                decimal valorTotalComponente = (from obj in cargaPedidoComponentesFrete where obj.TipoComponenteFrete == cargaPedidoComponenteFrete.TipoComponenteFrete && (cargaPedidoComponenteFrete.ComponenteFrete == null || obj.ComponenteFrete.Equals(cargaPedidoComponenteFrete.ComponenteFrete)) select obj.ValorComponente).Sum();


                                                                                decimal valorTotalCargaPedidoComponente1 = componentesFreteRateados.Where(o => o.CodigoCargaPedido == cargaPedido.Codigo &&
                                                                                                                                                               o.TipoComponente == cargaPedidoComponenteFrete.TipoComponenteFrete &&
                                                                                                                                                               o.CodigoComponenteFrete == (cargaPedidoComponenteFrete.ComponenteFrete?.Codigo ?? 0)).Sum(o => o.Valor);
                                                                                valorTotalCargaPedidoComponente1 += valorComponente;
                                                                                valorComponente += valorTotalComponente - valorTotalCargaPedidoComponente1;
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        valorComponente = xmlNotaFiscalComponente.Valor;
                                                                        incluirIntegralmenteContratoFreteTerceiro = xmlNotaFiscalComponente.IncluirIntegralmenteContratoFreteTerceiro.HasValue ? xmlNotaFiscalComponente.IncluirIntegralmenteContratoFreteTerceiro.Value : false;
                                                                    }

                                                                    Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico cargaPedidoComponenteFreteDinamico = cargaPedidoComponenteFrete.ConvertarParaComponenteDinamico();
                                                                    cargaPedidoComponenteFreteDinamico.ValorComponente = valorComponente;
                                                                    cargaPedidoComponenteFreteDinamico.ValorTotalMoeda = valorMoedaComponente;
                                                                    cargaPedidoComponenteFreteDinamico.ValorCotacaoMoeda = cotacaoMoeda;
                                                                    cargaPedidoComponenteFreteDinamico.Moeda = moeda;
                                                                    cargaPedidoComponenteFreteDinamico.IncluirIntegralmenteContratoFreteTerceiro = incluirIntegralmenteContratoFreteTerceiro;

                                                                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete pedidoXMLNotaFiscalComponente = AplicarRateioNosComponentesPedidoNotaFiscal(cargaPedidoComponenteFreteDinamico, pedidoXmlNotaFiscal, unitOfWork);

                                                                    if (pedidoXMLNotaFiscalComponente.IncluirBaseCalculoICMS)
                                                                        baseCalculo += pedidoXMLNotaFiscalComponente.ValorComponente;

                                                                    pedidoXMLNotaFiscalCompontesFrete.Add(pedidoXMLNotaFiscalComponente);
                                                                    pedidoXMLNotaFiscalCompontesFreteAgrupados.Add(pedidoXMLNotaFiscalComponente);

                                                                    if (!rateioFreteFilialEmissora)
                                                                    {
                                                                        valorTotalComponentes += pedidoXMLNotaFiscalComponente.ValorComponente;
                                                                        valorTotalMoedaComponentes += pedidoXMLNotaFiscalComponente.ValorTotalMoeda ?? 0m;
                                                                    }

                                                                    componentesFreteRateados.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteRateio()
                                                                    {
                                                                        CodigoCargaPedido = cargaPedido.Codigo,
                                                                        CodigoComponenteFrete = cargaPedidoComponenteFrete.ComponenteFrete?.Codigo ?? 0,
                                                                        CPFCNPJDestinatario = pedidoXmlNotaFiscal.XMLNotaFiscal.Destinatario.CPF_CNPJ,
                                                                        CPFCNPJRemetente = pedidoXmlNotaFiscal.XMLNotaFiscal.Emitente.CPF_CNPJ,
                                                                        TipoComponente = cargaPedidoComponenteFrete.TipoComponenteFrete,
                                                                        TipoOperacaoNotaFiscal = pedidoXmlNotaFiscal.XMLNotaFiscal.TipoOperacaoNotaFiscal,
                                                                        Valor = pedidoXMLNotaFiscalComponente.ValorComponente,
                                                                        ValorMoeda = pedidoXMLNotaFiscalComponente.ValorTotalMoeda ?? 0m
                                                                    });
                                                                }
                                                            }

                                                            SetarValoresEmbarcador(carga, cargaPedido, ref baseCalculo, ref valorTotalComponentes, ref pedidoXmlNotaFiscal, ref pedidoXMLNotaFiscalCompontesFrete, componentesXMLNotaFiscal, unitOfWork);

                                                            if (tipoEmissaoDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalIndividual)
                                                            {
                                                                if (emissaoSeraPorNota)
                                                                {
                                                                    decimal valorFrete = pedidoXmlNotaFiscal.ValorFrete;
                                                                    bool incluirICMSBaseCalculo = cargaPedido.IncluirICMSBaseCalculo;
                                                                    decimal percentualIncluirBaseCalculo = cargaPedido.PercentualIncluirBaseCalculo;
                                                                    if (rateioFreteFilialEmissora)
                                                                    {
                                                                        valorFrete = pedidoXmlNotaFiscal.ValorFreteFilialEmissora;
                                                                        incluirICMSBaseCalculo = cargaPedido.IncluirICMSBaseCalculoFilialEmissora;
                                                                        percentualIncluirBaseCalculo = cargaPedido.PercentualIncluirBaseCalculoFilialEmissora;
                                                                    }

                                                                    CalcularImpostos(cargaPedido, carga, rateioFreteFilialEmissora, pedidoXmlNotaFiscal, valorFrete, incluirICMSBaseCalculo, percentualIncluirBaseCalculo, pedidoXMLNotaFiscalCompontesFrete, tipoServicoMultisoftware, cargaPedido.Expedidor, cargaPedido.Recebedor, unitOfWork, configuracao, aliquotasUfEmpresa, cfops, componentesICMSXMLNotaFiscalExistente, pedagioEstadosBaseCalculo, componenteICMS, produtoEmbarcadorConsultar, produtoEmbarcador);
                                                                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete pedidoXMLNotaFiscalComponente in pedidoXMLNotaFiscalCompontesFrete)
                                                                    {
                                                                        if (pedidoXMLNotaFiscalComponente.ModeloDocumentoFiscal == null || pedidoXMLNotaFiscalComponente.ModeloDocumentoFiscal.Numero == "57" || pedidoXMLNotaFiscalComponente.ModeloDocumentoFiscal.Numero == "39")
                                                                        {
                                                                            pedidoXMLNotaFiscalComponente.ModeloDocumentoFiscal = pedidoXmlNotaFiscal.ModeloDocumentoFiscal;
                                                                            repPedidoXMLNotaFiscalComponenteFrete.Atualizar(pedidoXMLNotaFiscalComponente);
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (possuiCTe)
                                                                    {
                                                                        if (!rateioFreteFilialEmissora)
                                                                        {
                                                                            pedidoXmlNotaFiscal.ICMSPagoPorST = cargaPedido.ICMSPagoPorST;
                                                                            pedidoXmlNotaFiscal.PercentualAliquota = cargaPedido.PercentualAliquota;
                                                                            pedidoXmlNotaFiscal.AliquotaCofins = cargaPedido.AliquotaCofins;
                                                                            pedidoXmlNotaFiscal.AliquotaPis = cargaPedido.AliquotaPis;
                                                                            pedidoXmlNotaFiscal.CFOP = cargaPedido.CFOP;
                                                                            pedidoXmlNotaFiscal.CST = cargaPedido.CST;
                                                                            pedidoXmlNotaFiscal.IncluirICMSBaseCalculo = cargaPedido.IncluirICMSBaseCalculo;
                                                                            pedidoXmlNotaFiscal.PercentualIncluirBaseCalculo = cargaPedido.PercentualIncluirBaseCalculo;
                                                                            pedidoXmlNotaFiscal.PercentualReducaoBC = cargaPedido.PercentualReducaoBC;
                                                                            pedidoXmlNotaFiscal.ObservacaoRegraICMSCTe = cargaPedido.ObservacaoRegraICMSCTe;
                                                                            decimal aliquotaPisCofins = pedidoXmlNotaFiscal.AliquotaCofins + pedidoXmlNotaFiscal.AliquotaPis;
                                                                            pedidoXmlNotaFiscal.ValorICMSIncluso = Math.Round(serCargaICMS.CalcularICMSInclusoNoFrete(cargaPedido.CST, ref baseCalculo, pedidoXmlNotaFiscal.PercentualAliquota, pedidoXmlNotaFiscal.PercentualIncluirBaseCalculo, pedidoXmlNotaFiscal.PercentualReducaoBC, pedidoXmlNotaFiscal.IncluirICMSBaseCalculo, pedidoXmlNotaFiscal.PercentualAliquotaInternaDifal, aliquotaPisCofins), 2, MidpointRounding.AwayFromZero);
                                                                            pedidoXmlNotaFiscal.ValorICMS = Math.Round(serCargaICMS.CalcularInclusaoICMSNoFrete(cargaPedido.CST, ref baseCalculo, pedidoXmlNotaFiscal.PercentualAliquota, pedidoXmlNotaFiscal.PercentualIncluirBaseCalculo, pedidoXmlNotaFiscal.PercentualReducaoBC, pedidoXmlNotaFiscal.IncluirICMSBaseCalculo, aliquotaPisCofins), 2, MidpointRounding.AwayFromZero);
                                                                            pedidoXmlNotaFiscal.ValorPis = Math.Round(servicoPisCofins.CalcularValorPis(cargaPedido.AliquotaPis, baseCalculo), 2, MidpointRounding.AwayFromZero);
                                                                            pedidoXmlNotaFiscal.ValorCofins = Math.Round(servicoPisCofins.CalcularValorCofins(cargaPedido.AliquotaCofins, baseCalculo), 2, MidpointRounding.AwayFromZero);
                                                                            pedidoXmlNotaFiscal.BaseCalculoICMS = baseCalculo;
                                                                            pedidoXmlNotaFiscal.PercentualCreditoPresumido = cargaPedido.PercentualCreditoPresumido;
                                                                            pedidoXmlNotaFiscal.ValorCreditoPresumido = Math.Round(pedidoXmlNotaFiscal.ValorICMS * (cargaPedido.PercentualCreditoPresumido / 100), 2, MidpointRounding.AwayFromZero);
                                                                            pedidoXmlNotaFiscal.PossuiCTe = true;
                                                                            pedidoXmlNotaFiscal.PossuiNFS = false;
                                                                            pedidoXmlNotaFiscal.DescontarICMSDoValorAReceber = cargaPedido.DescontarICMSDoValorAReceber;
                                                                            pedidoXmlNotaFiscal.NaoReduzirRetencaoICMSDoValorDaPrestacao = cargaPedido.NaoReduzirRetencaoICMSDoValorDaPrestacao;

                                                                            Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = servicoImpostoIBSCBS.ObterImpostoIBSCBSComTributacaoDefinida(cargaPedido, baseCalculoIBSCBS);
                                                                            servicoPedidoXMLNotaFiscal.PreencherCamposImpostoIBSCBS(pedidoXmlNotaFiscal, impostoIBSCBS);

                                                                            cargaPedido.PossuiCTe = true;
                                                                        }
                                                                        else
                                                                        {
                                                                            pedidoXmlNotaFiscal.ICMSPagoPorSTFilialEmissora = cargaPedido.ICMSPagoPorSTFilialEmissora;
                                                                            pedidoXmlNotaFiscal.PercentualAliquotaFilialEmissora = cargaPedido.PercentualAliquotaFilialEmissora;
                                                                            pedidoXmlNotaFiscal.CFOPFilialEmissora = cargaPedido.CFOPFilialEmissora;
                                                                            pedidoXmlNotaFiscal.CSTFilialEmissora = cargaPedido.CSTFilialEmissora;
                                                                            pedidoXmlNotaFiscal.IncluirICMSBaseCalculoFilialEmissora = cargaPedido.IncluirICMSBaseCalculoFilialEmissora;
                                                                            pedidoXmlNotaFiscal.PercentualIncluirBaseCalculoFilialEmissora = cargaPedido.PercentualIncluirBaseCalculoFilialEmissora;
                                                                            pedidoXmlNotaFiscal.PercentualReducaoBCFilialEmissora = cargaPedido.PercentualReducaoBCFilialEmissora;
                                                                            pedidoXmlNotaFiscal.ObservacaoRegraICMSCTeFilialEmissora = cargaPedido.ObservacaoRegraICMSCTeFilialEmissora;
                                                                            pedidoXmlNotaFiscal.ValorICMSFilialEmissora = Math.Round(serCargaICMS.CalcularInclusaoICMSNoFrete(cargaPedido.CSTFilialEmissora, ref baseCalculo, pedidoXmlNotaFiscal.PercentualAliquotaFilialEmissora, pedidoXmlNotaFiscal.PercentualIncluirBaseCalculoFilialEmissora, pedidoXmlNotaFiscal.PercentualReducaoBCFilialEmissora, pedidoXmlNotaFiscal.IncluirICMSBaseCalculoFilialEmissora), 2, MidpointRounding.AwayFromZero);
                                                                            pedidoXmlNotaFiscal.BaseCalculoICMSFilialEmissora = baseCalculo;
                                                                            pedidoXmlNotaFiscal.PercentualCreditoPresumidoFilialEmissora = cargaPedido.PercentualCreditoPresumidoFilialEmissora;
                                                                            pedidoXmlNotaFiscal.ValorCreditoPresumidoFilialEmissora = Math.Round(pedidoXmlNotaFiscal.ValorICMSFilialEmissora * (cargaPedido.PercentualCreditoPresumidoFilialEmissora / 100), 2, MidpointRounding.AwayFromZero);
                                                                            pedidoXmlNotaFiscal.PossuiCTe = true;
                                                                            pedidoXmlNotaFiscal.PossuiNFS = false;
                                                                            pedidoXmlNotaFiscal.DescontarICMSDoValorAReceber = cargaPedido.DescontarICMSDoValorAReceber;
                                                                            pedidoXmlNotaFiscal.NaoReduzirRetencaoICMSDoValorDaPrestacao = cargaPedido.NaoReduzirRetencaoICMSDoValorDaPrestacao;

                                                                            Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = servicoImpostoIBSCBS.ObterImpostoIBSCBSComTributacaoDefinidaFilialEmissora(cargaPedido, baseCalculoIBSCBS);
                                                                            servicoPedidoXMLNotaFiscal.PreencherCamposImpostoIBSCBSFilialEmissora(pedidoXmlNotaFiscal, impostoIBSCBS);

                                                                            cargaPedido.PossuiCTe = true;
                                                                        }

                                                                        GerarComponenteICMS(pedidoXmlNotaFiscal, rateioFreteFilialEmissora, unitOfWork, componenteICMS, componentesICMSXMLNotaFiscalExistente);
                                                                        GerarComponentePisCofins(pedidoXmlNotaFiscal, unitOfWork, componentePisCofins, componentesPisCofinsXMLNotaFiscalExistente);
                                                                    }
                                                                    else
                                                                    {
                                                                        pedidoXmlNotaFiscal.PossuiCTe = false;
                                                                    }

                                                                    bool setarImportosISS = false;
                                                                    if (!rateioFreteFilialEmissora)
                                                                    {
                                                                        if (possuiNFS)
                                                                        {
                                                                            setarImportosISS = true;
                                                                            cargaPedido.PossuiNFS = true;
                                                                            pedidoXmlNotaFiscal.PossuiCTe = false;
                                                                            pedidoXmlNotaFiscal.PossuiNFS = true;

                                                                        }
                                                                        else
                                                                        {
                                                                            pedidoXmlNotaFiscal.PossuiNFS = false;
                                                                            cargaPedido.PossuiNFS = false;
                                                                        }
                                                                    }


                                                                    if (possuiNFSManual && !rateioFreteFilialEmissora)
                                                                    {
                                                                        setarImportosISS = true;
                                                                        cargaPedido.PossuiNFSManual = true;
                                                                        pedidoXmlNotaFiscal.PossuiNFSManual = true;
                                                                        pedidoXmlNotaFiscal.PossuiCTe = false;
                                                                        pedidoXmlNotaFiscal.PossuiNFS = false;
                                                                    }
                                                                    else
                                                                        pedidoXmlNotaFiscal.PossuiNFSManual = false;

                                                                    if (setarImportosISS)
                                                                    {
                                                                        pedidoXmlNotaFiscal.PercentualAliquotaISS = cargaPedido.PercentualAliquotaISS;
                                                                        pedidoXmlNotaFiscal.PercentualRetencaoISS = cargaPedido.PercentualRetencaoISS;
                                                                        pedidoXmlNotaFiscal.IncluirISSBaseCalculo = cargaPedido.IncluirISSBaseCalculo;
                                                                        pedidoXmlNotaFiscal.ValorISS = Math.Round(serCargaISS.CalcularInclusaoISSNoFrete(ref baseCalculo, pedidoXmlNotaFiscal.PercentualAliquotaISS, pedidoXmlNotaFiscal.IncluirISSBaseCalculo), 2, MidpointRounding.AwayFromZero);
                                                                        pedidoXmlNotaFiscal.ValorRetencaoISS = Math.Round(serCargaISS.CalcularRetencaoISSNoFrete(pedidoXmlNotaFiscal.ValorISS, pedidoXmlNotaFiscal.PercentualRetencaoISS), 2, MidpointRounding.AwayFromZero);
                                                                        pedidoXmlNotaFiscal.BaseCalculoISS = baseCalculo;

                                                                        if (pedidoXmlNotaFiscal.IncluirISSBaseCalculo)
                                                                            baseCalculoIBSCBS += pedidoXmlNotaFiscal.ValorISS;

                                                                        Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = servicoImpostoIBSCBS.ObterImpostoIBSCBSComValoresArredondados(new Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBS
                                                                        {
                                                                            BaseCalculo = baseCalculoIBSCBS,
                                                                            ValoAbaterBaseCalculo = pedidoXmlNotaFiscal.ValorISS,
                                                                            CodigoLocalidade = cargaPedido.Destino.Codigo,
                                                                            SiglaUF = cargaPedido.Destino.Estado.Sigla,
                                                                            CodigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0,
                                                                            Empresa = cargaPedido.Carga.Empresa
                                                                        });

                                                                        servicoPedidoXMLNotaFiscal.PreencherCamposImpostoIBSCBS(pedidoXmlNotaFiscal, impostoIBSCBS);

                                                                        GerarComponenteISS(pedidoXmlNotaFiscal, unitOfWork);
                                                                    }
                                                                    else
                                                                    {
                                                                        pedidoXmlNotaFiscal.ValorISS = 0;
                                                                        pedidoXmlNotaFiscal.BaseCalculoISS = 0;
                                                                        pedidoXmlNotaFiscal.PercentualAliquotaISS = 0;
                                                                        pedidoXmlNotaFiscal.PercentualRetencaoISS = 0;
                                                                        pedidoXmlNotaFiscal.IncluirISSBaseCalculo = false;
                                                                        pedidoXmlNotaFiscal.ValorRetencaoISS = 0;

                                                                    }

                                                                    Servicos.Log.GravarInfo($"Atualizando RatearFreteCargaPedidoEntreNotas cargaPedido = {cargaPedido?.Codigo ?? 0}, pedidoXmlNotaFiscal = {pedidoXmlNotaFiscal.Codigo} com valorISS = {pedidoXmlNotaFiscal.ValorISS} e incluirISSBaseCalculo = {pedidoXmlNotaFiscal.IncluirISSBaseCalculo}, valorRetencaoISS = {pedidoXmlNotaFiscal.ValorRetencaoISS}", "AtualizarPedidoXMLNotaFiscal");
                                                                }

                                                                if (!rateioFreteFilialEmissora)
                                                                {
                                                                    totalBaseCalculoICMS += pedidoXmlNotaFiscal.BaseCalculoICMS;
                                                                    totalICMS += pedidoXmlNotaFiscal.ValorICMS;
                                                                    totalPis += pedidoXmlNotaFiscal.ValorPis;
                                                                    totalCofins += pedidoXmlNotaFiscal.ValorCofins;
                                                                    totalICMSIncluso += pedidoXmlNotaFiscal.ValorICMSIncluso;
                                                                    totalCreditoPresumido += pedidoXmlNotaFiscal.ValorCreditoPresumido;
                                                                    totalBaseCalculoISS += pedidoXmlNotaFiscal.BaseCalculoISS;
                                                                    totalISS += pedidoXmlNotaFiscal.ValorISS;
                                                                    totalRetencaoISS += pedidoXmlNotaFiscal.ValorRetencaoISS;

                                                                    totalBaseCalculoIBSCBS += pedidoXmlNotaFiscal.BaseCalculoIBSCBS;
                                                                    totalValorIBSEstadual += pedidoXmlNotaFiscal.ValorIBSEstadual;
                                                                    totalValorIBSMunicipal += pedidoXmlNotaFiscal.ValorIBSMunicipal;
                                                                    totalValorCBS += pedidoXmlNotaFiscal.ValorCBS;

                                                                    if (pedidoXmlNotaFiscal.IncluirICMSBaseCalculo && pedidoXmlNotaFiscal.CST != "60")
                                                                    {
                                                                        totalICMSIncluir += pedidoXmlNotaFiscal.ValorICMS;
                                                                        totalICMSInclusoIncluir += pedidoXmlNotaFiscal.ValorICMSIncluso;
                                                                    }

                                                                    if (pedidoXmlNotaFiscal.IncluirISSBaseCalculo)
                                                                        totalISSIncluir += pedidoXmlNotaFiscal.ValorISS;
                                                                }
                                                                else
                                                                {
                                                                    totalBaseCalculoICMS += pedidoXmlNotaFiscal.BaseCalculoICMSFilialEmissora;
                                                                    totalICMS += pedidoXmlNotaFiscal.ValorICMSFilialEmissora;
                                                                    totalCreditoPresumido += pedidoXmlNotaFiscal.ValorCreditoPresumidoFilialEmissora;

                                                                    totalBaseCalculoIBSCBS += pedidoXmlNotaFiscal.BaseCalculoIBSCBSFilialEmissora;
                                                                    totalValorIBSEstadual += pedidoXmlNotaFiscal.ValorIBSEstadualFilialEmissora;
                                                                    totalValorIBSMunicipal += pedidoXmlNotaFiscal.ValorIBSMunicipalFilialEmissora;
                                                                    totalValorCBS += pedidoXmlNotaFiscal.ValorCBSFilialEmissora;

                                                                    if (pedidoXmlNotaFiscal.IncluirICMSBaseCalculoFilialEmissora && pedidoXmlNotaFiscal.CSTFilialEmissora != "60")
                                                                        totalICMSIncluir += pedidoXmlNotaFiscal.ValorICMSFilialEmissora;
                                                                }

                                                                if (!rateioFreteFilialEmissora)
                                                                {
                                                                    pedidoXmlNotaFiscal.ValorTotalComponentes = valorTotalComponentes;
                                                                    pedidoXmlNotaFiscal.ValorTotalMoedaComponentes = valorTotalMoedaComponentes;
                                                                }

                                                                adicionarDocumentosParaProvisao = true;
                                                            }
                                                            else if (tipoEmissaoDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoAgrupado || tipoEmissaoDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoIndividual || (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && tipoEmissaoDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.NaoInformado))
                                                            {
                                                                if (possuiCTe)
                                                                    AplicarRateioICMSNoPedidoNotaFiscal(cargaPedido, rateioFreteFilialEmissora, totalCTes, numeroNotasRateio, pesoTotal, valorTotalNF, pedidoPedidoXMLNotsaFiscaisAgrupados.Count, ref SomaValorICMSTotalPorCTe, ref SomaValorPisTotalPorCTe, ref SomaValorCofinsTotalPorCTe, ref totalICMSFreteCargaPedidoNotas, ref totalPisFreteCargaPedidoNotas, ref totalCofinsFreteCargaPedidoNotas, ref SomaBaseCalculoTotalPorCTe, ref totalBaseCalculoCargaPedidoNotas, ref SomaValorCreditoPresumidoTotalPorCTe, ref totalCreditoPresumidoCargaPedidoNotas, pedidoXmlNotaFiscal, formulaRateio, ultimoPedidoXmlNotaFiscal, ultimoPedidoXmlNotaFiscalNormalAgrupado, ultimoPedidoXmlNotaFiscalNormal, componenteICMS, componentesICMSXMLNotaFiscalExistente, componentePisCofins, componentesPisCofinsXMLNotaFiscalExistente, unitOfWork, metrosCubicosTotais, densidadeProdutos, ref SomaValorICMSInclusoTotalPorCTe, ref totalICMSInclusoFreteCargaPedidoNotas, pesoLiquidoTotal, volumeTotal, pesoTotalParaCalculoFatorCubagem, ref SomaBaseCalculoIBSCBSTotalPorCTe, ref SomaValorIBSEstadualTotalPorCTe, ref SomaValorIBSMunicipalTotalPorCTe, ref SomaValorCBSTotalPorCTe, ref totalBaseCalculoIBSCBSCargaPedidoNotas, ref totalValorIBSEstadualCargaPedidoNotas, ref totalValorIBSMunicipalCargaPedidoNotas, ref totalValorCBSCargaPedidoNotas);

                                                                if (!rateioFreteFilialEmissora)
                                                                {
                                                                    if ((possuiNFSManual || possuiNFS))
                                                                        AplicarRateioISSNoPedidoNotaFiscal(cargaPedido, totalCTes, numeroNotasRateio, pesoTotal, valorTotalNF, pedidoPedidoXMLNotsaFiscaisAgrupados.Count, ref SomaValorISSTotalPorCTe, ref totalISSFreteCargaPedidoNotas, ref SomaBaseCalculoISSTotalPorCTe, ref totalBaseCalculoISSCargaPedidoNotas, ref SomaValorRetencaoISSTotalPorCTe, ref totalRetencaoISSCargaPedidoNotas, pedidoXmlNotaFiscal, formulaRateio, ultimoPedidoXmlNotaFiscal, ultimoPedidoXmlNotaFiscalNormalAgrupado, ultimoPedidoXmlNotaFiscalNormal, unitOfWork, metrosCubicosTotais, densidadeProdutos, pesoLiquidoTotal, volumeTotal, pesoTotalParaCalculoFatorCubagem, ref SomaBaseCalculoIBSCBSTotalPorCTe, ref SomaValorIBSEstadualTotalPorCTe, ref SomaValorIBSMunicipalTotalPorCTe, ref SomaValorCBSTotalPorCTe, ref totalBaseCalculoIBSCBSCargaPedidoNotas, ref totalValorIBSEstadualCargaPedidoNotas, ref totalValorIBSMunicipalCargaPedidoNotas, ref totalValorCBSCargaPedidoNotas);
                                                                    else
                                                                        ZerarRateioISSNoPedidoNotaFiscal(cargaPedido, pedidoXmlNotaFiscal, unitOfWork);
                                                                }

                                                                if (!rateioFreteFilialEmissora)
                                                                {
                                                                    pedidoXmlNotaFiscal.ValorTotalComponentes = valorTotalComponentes;
                                                                    pedidoXmlNotaFiscal.ValorTotalMoedaComponentes = valorTotalMoedaComponentes;
                                                                }

                                                                adicionarDocumentosParaProvisao = true;
                                                            }

                                                            if (!rateioFreteFilialEmissora)
                                                                InformarDadosContabeisPedidoNotaFiscal(cargaOrigem, pedidoXmlNotaFiscal, emissaoSeraPorNota, cargaPedido, cargaPedidoContaContabilContabilizacaos, configuracao, tipoServicoMultisoftware, unitOfWork);

                                                            repPedidoXMLNotaFiscal.Atualizar(pedidoXmlNotaFiscal);
                                                        }
                                                    }

                                                    if ((formulaRateio?.RatearEmBlocoDeEmissao ?? false) && SomaValorTotalPorCTeAgrupado > 0)
                                                    {
                                                        ultimoPedidoXmlNotaFiscal = pedidoPedidoXMLNotsaFiscaisAgrupados.LastOrDefault();
                                                        decimal totalFreteAgrupado = pedidoPedidoXMLNotsaFiscaisAgrupados.Sum(c => c.ValorFrete);
                                                        decimal diferenca = Math.Round(SomaValorTotalPorCTeAgrupado - totalFreteAgrupado, 3, MidpointRounding.ToEven);
                                                        if (ultimoPedidoXmlNotaFiscal != null && diferenca != 0)
                                                        {
                                                            if (diferenca != 0m)
                                                            {
                                                                ultimoPedidoXmlNotaFiscal.ValorFrete += diferenca;
                                                                totalFreteCargaPedidoNotas += diferenca;
                                                                repPedidoXMLNotaFiscal.Atualizar(ultimoPedidoXmlNotaFiscal);
                                                            }
                                                        }
                                                    }
                                                    if ((formulaRateio?.RatearEmBlocoDeEmissao ?? false) && valorTotalComponentesOriginal > 0)
                                                    {
                                                        ultimoPedidoXmlNotaFiscal = pedidoPedidoXMLNotsaFiscaisAgrupados.LastOrDefault();
                                                        decimal totalValorTotalComponentes = pedidoPedidoXMLNotsaFiscaisAgrupados.Sum(c => c.ValorTotalComponentes);
                                                        decimal diferenca = Math.Round(valorTotalComponentesOriginal - totalValorTotalComponentes, 3, MidpointRounding.ToEven);
                                                        if (ultimoPedidoXmlNotaFiscal != null && diferenca != 0)
                                                        {
                                                            if (diferenca != 0m)
                                                            {
                                                                ultimoPedidoXmlNotaFiscal.ValorTotalComponentes += diferenca;
                                                                repPedidoXMLNotaFiscal.Atualizar(ultimoPedidoXmlNotaFiscal);
                                                            }
                                                        }
                                                    }


                                                    if (tipoEmissaoDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupada ||
                                                        tipoEmissaoDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupadaEntrePedidos)
                                                    {
                                                        decimal valorTotal = 0m;

                                                        bool incluirICMSBaseCalculo = cargaPedido.IncluirICMSBaseCalculo;
                                                        decimal percentualIncluirBaseCalculo = cargaPedido.PercentualIncluirBaseCalculo;
                                                        if (!rateioFreteFilialEmissora)
                                                            valorTotal = pedidoPedidoXMLNotsaFiscaisAgrupados.Sum(obj => obj.ValorFrete);
                                                        else
                                                        {
                                                            valorTotal = pedidoPedidoXMLNotsaFiscaisAgrupados.Sum(obj => obj.ValorFreteFilialEmissora);
                                                            incluirICMSBaseCalculo = cargaPedido.IncluirICMSBaseCalculoFilialEmissora;
                                                            percentualIncluirBaseCalculo = cargaPedido.PercentualIncluirBaseCalculoFilialEmissora;
                                                        }

                                                        if (emissaoSeraPorNota && tipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.Normal)
                                                        {
                                                            CalcularImpostos(formulaRateio, cargaPedido, carga, rateioFreteFilialEmissora, pedidoPedidoXMLNotsaFiscaisAgrupados, valorTotal, incluirICMSBaseCalculo, percentualIncluirBaseCalculo, tipoServicoMultisoftware, null, unitOfWork, configuracao, cargaPedidoProdutos);

                                                            if (!rateioFreteFilialEmissora)
                                                            {
                                                                totalBaseCalculoISS += pedidoPedidoXMLNotsaFiscaisAgrupados.Sum(o => o.BaseCalculoISS);
                                                                totalISS += pedidoPedidoXMLNotsaFiscaisAgrupados.Sum(o => o.ValorISS);
                                                                totalRetencaoISS += pedidoPedidoXMLNotsaFiscaisAgrupados.Sum(o => o.ValorRetencaoISS);
                                                                totalBaseCalculoICMS += pedidoPedidoXMLNotsaFiscaisAgrupados.Sum(o => o.BaseCalculoICMS);
                                                                totalICMS += pedidoPedidoXMLNotsaFiscaisAgrupados.Sum(o => o.ValorICMS);
                                                                totalPis += pedidoPedidoXMLNotsaFiscaisAgrupados.Sum(o => o.ValorPis);
                                                                totalCofins += pedidoPedidoXMLNotsaFiscaisAgrupados.Sum(o => o.ValorCofins);
                                                                totalCreditoPresumido += pedidoPedidoXMLNotsaFiscaisAgrupados.Sum(o => o.ValorCreditoPresumido);
                                                                totalICMSIncluir += pedidoPedidoXMLNotsaFiscaisAgrupados.Where(o => o.IncluirICMSBaseCalculo).Sum(o => o.ValorICMSIncluso);
                                                                totalISSIncluir += pedidoPedidoXMLNotsaFiscaisAgrupados.Where(o => o.IncluirISSBaseCalculo).Sum(o => o.ValorISS);
                                                                totalBaseCalculoIR += pedidoPedidoXMLNotsaFiscaisAgrupados.Where(o => o.ReterIR).Sum(o => o.BaseCalculoIR);
                                                                totalValorIR += pedidoPedidoXMLNotsaFiscaisAgrupados.Where(o => o.ReterIR).Sum(o => o.ValorIR);

                                                                totalBaseCalculoIBSCBS += pedidoPedidoXMLNotsaFiscaisAgrupados.Sum(o => o.BaseCalculoIBSCBS);
                                                                totalValorIBSEstadual += pedidoPedidoXMLNotsaFiscaisAgrupados.Sum(o => o.ValorIBSEstadual);
                                                                totalValorIBSMunicipal += pedidoPedidoXMLNotsaFiscaisAgrupados.Sum(o => o.ValorIBSMunicipal);
                                                                totalValorCBS += pedidoPedidoXMLNotsaFiscaisAgrupados.Sum(o => o.ValorCBS);
                                                            }
                                                            else
                                                            {
                                                                totalBaseCalculoICMS += pedidoPedidoXMLNotsaFiscaisAgrupados.Sum(o => o.BaseCalculoICMSFilialEmissora);
                                                                totalICMS += pedidoPedidoXMLNotsaFiscaisAgrupados.Sum(o => o.ValorICMSFilialEmissora);
                                                                totalCreditoPresumido += pedidoPedidoXMLNotsaFiscaisAgrupados.Sum(o => o.ValorCreditoPresumidoFilialEmissora);
                                                                totalICMSIncluir += pedidoPedidoXMLNotsaFiscaisAgrupados.Where(o => o.IncluirICMSBaseCalculoFilialEmissora).Sum(o => o.ValorICMSFilialEmissora);

                                                                totalBaseCalculoIBSCBS += pedidoPedidoXMLNotsaFiscaisAgrupados.Sum(o => o.BaseCalculoIBSCBSFilialEmissora);
                                                                totalValorIBSEstadual += pedidoPedidoXMLNotsaFiscaisAgrupados.Sum(o => o.ValorIBSEstadualFilialEmissora);
                                                                totalValorIBSMunicipal += pedidoPedidoXMLNotsaFiscaisAgrupados.Sum(o => o.ValorIBSMunicipalFilialEmissora);
                                                                totalValorCBS += pedidoPedidoXMLNotsaFiscaisAgrupados.Sum(o => o.ValorCBSFilialEmissora);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Servicos.Embarcador.Carga.RateioFormula svcRateio = new RateioFormula(unitOfWork);

                                                            decimal baseCalculo = 0m;

                                                            bool zerarBaseCalculo = false;

                                                            if (cargaPedido.RegraICMS != null)
                                                            {
                                                                Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMS = repRegraICMS.BuscarPorCodigo(cargaPedido.RegraICMS.Codigo);

                                                                zerarBaseCalculo = regraICMS?.ZerarValorICMS ?? false;
                                                            }

                                                            decimal valorFreteBase;
                                                            if (!rateioFreteFilialEmissora)
                                                                valorFreteBase = pedidoPedidoXMLNotsaFiscaisAgrupados.Sum(o => o.ValorFrete);
                                                            else
                                                                valorFreteBase = pedidoPedidoXMLNotsaFiscaisAgrupados.Sum(o => o.ValorFreteFilialEmissora);

                                                            decimal baseCalculoIBSCBS = valorFreteBase;

                                                            if (!zerarBaseCalculo)
                                                            {
                                                                baseCalculo = valorFreteBase;

                                                                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXmlNotaFiscal in pedidoPedidoXMLNotsaFiscaisAgrupados)
                                                                {
                                                                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> pedidoXMLNotaFiscalComponenteFretes = (from obj in pedidoXMLNotaFiscalCompontesFreteAgrupados where obj.PedidoXMLNotaFiscal.Codigo == pedidoXmlNotaFiscal.Codigo select obj).ToList();

                                                                    ObterBaseCalculo(ref baseCalculo, rateioFreteFilialEmissora, carga, cargaPedido, pedidoXmlNotaFiscal, unitOfWork, tipoServicoMultisoftware, pedidoXMLNotaFiscalComponenteFretes);
                                                                    baseCalculoIBSCBS += Math.Round(pedidoXMLNotaFiscalComponenteFretes.Sum(obj => obj.ValorComponente), 2, MidpointRounding.AwayFromZero);
                                                                }
                                                            }

                                                            decimal valorTotalNotasAgrupadas = pedidoPedidoXMLNotsaFiscaisAgrupados.Sum(o => o.XMLNotaFiscal.Valor);
                                                            decimal pesoTotalNotasAgrupadas = pedidoPedidoXMLNotsaFiscaisAgrupados.Sum(o => o.XMLNotaFiscal.Peso);
                                                            decimal pesoLiquidoTotalNotasAgrupadas = pedidoPedidoXMLNotsaFiscaisAgrupados.Sum(o => o.XMLNotaFiscal.PesoLiquido);
                                                            int volumeTotalNotasAgrupadas = pedidoPedidoXMLNotsaFiscaisAgrupados.Sum(o => o.XMLNotaFiscal.Volumes);
                                                            decimal metrosCubicosTotaisNotasAgrupadas = pedidoPedidoXMLNotsaFiscaisAgrupados.Sum(o => o.XMLNotaFiscal.MetrosCubicos);

                                                            int totalNotasAgrupadas = pedidoPedidoXMLNotsaFiscaisAgrupados.Count;

                                                            decimal pesoParaCalculo = pesoTotalNotasAgrupadas;
                                                            int qtdNotasParaCalculo = totalNotasAgrupadas;

                                                            if (possuiCTe)
                                                            {
                                                                decimal totalBaseCalculoRateada = 0m,
                                                                        totalICMSRateado = 0m,
                                                                        totalICMSInclusoRateado = 0m,
                                                                        totalPisRateado = 0m,
                                                                        totalCofinsRateado = 0m,
                                                                        totalCreditoPresumidoRateado = 0m,
                                                                        totalBaseCalculoIBSCBSRateado = 0m,
                                                                        totalValorIBSEstadualRateado = 0m,
                                                                        totalValorIBSMunicipalRateado = 0m,
                                                                        totalValorCBSRateado = 0m;

                                                                string CST = cargaPedido.CST;
                                                                decimal percentualAliquotaInternaDifal = cargaPedido.PercentualAliquotaInternaDifal;
                                                                decimal percentualAliquota = cargaPedido.PercentualAliquota;
                                                                decimal aliquotaPis = cargaPedido.AliquotaPis;
                                                                decimal aliquotaCofins = cargaPedido.AliquotaPis;
                                                                decimal percentualReducaoBC = cargaPedido.PercentualReducaoBC;
                                                                if (rateioFreteFilialEmissora)
                                                                {
                                                                    CST = cargaPedido.CSTFilialEmissora;
                                                                    percentualAliquota = cargaPedido.PercentualAliquotaFilialEmissora;
                                                                    percentualAliquotaInternaDifal = cargaPedido.PercentualAliquotaFilialEmissoraInternaDifal;
                                                                    percentualReducaoBC = cargaPedido.PercentualReducaoBCFilialEmissora;
                                                                }

                                                                decimal aliquotaPisCofins = (cargaPedido.AliquotaPis + cargaPedido.AliquotaCofins);
                                                                decimal icmsIncluso = serCargaICMS.CalcularICMSInclusoNoFrete(CST, ref baseCalculo, percentualAliquota, percentualIncluirBaseCalculo, percentualReducaoBC, incluirICMSBaseCalculo, percentualAliquotaInternaDifal, aliquotaPisCofins);
                                                                icmsIncluso = Math.Round(icmsIncluso, 2, MidpointRounding.AwayFromZero);

                                                                decimal icms = serCargaICMS.CalcularInclusaoICMSNoFrete(CST, ref baseCalculo, percentualAliquota, percentualIncluirBaseCalculo, percentualReducaoBC, incluirICMSBaseCalculo, aliquotaPisCofins);
                                                                icms = Math.Round(icms, 2, MidpointRounding.AwayFromZero);

                                                                decimal pis = servicoPisCofins.CalcularValorPis(cargaPedido.AliquotaPis, baseCalculo);
                                                                decimal cofins = servicoPisCofins.CalcularValorCofins(cargaPedido.AliquotaCofins, baseCalculo);

                                                                decimal creditoPresumido = icms * (cargaPedido.PercentualCreditoPresumido / 100);
                                                                decimal densidadeProdutos = cargaPedido.Produtos?.Sum(o => o.Produto?.MetroCubito) ?? 0m;

                                                                if (rateioFreteFilialEmissora)
                                                                    creditoPresumido = icms * (cargaPedido.PercentualCreditoPresumidoFilialEmissora / 100);

                                                                creditoPresumido = Math.Round(creditoPresumido, 2, MidpointRounding.AwayFromZero);

                                                                Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS;
                                                                if (rateioFreteFilialEmissora)
                                                                    impostoIBSCBS = servicoImpostoIBSCBS.ObterImpostoIBSCBSComTributacaoDefinidaFilialEmissora(cargaPedido, baseCalculoIBSCBS);
                                                                else
                                                                    impostoIBSCBS = servicoImpostoIBSCBS.ObterImpostoIBSCBSComTributacaoDefinida(cargaPedido, baseCalculoIBSCBS);

                                                                baseCalculoIBSCBS = impostoIBSCBS.BaseCalculo;
                                                                decimal valorIBSEstadual = impostoIBSCBS.ValorIBSEstadual;
                                                                decimal valorIBSMunicipal = impostoIBSCBS.ValorIBSMunicipal;
                                                                decimal valorCBS = impostoIBSCBS.ValorCBS;

                                                                for (var i = 0; i < totalNotasAgrupadas; i++)
                                                                {
                                                                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = pedidoPedidoXMLNotsaFiscaisAgrupados[i];
                                                                    decimal valorRateioOriginal = 0;
                                                                    decimal pesoParaCalculoFatorCubagem = 0;

                                                                    if (formulaRateio?.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.PorPesoUtilizandoFatorCubagem)
                                                                        pesoParaCalculoFatorCubagem = serRateioFormula.ObterPesoCubadoFatorCubagem(formulaRateio.ParametroRateioFormula, pedidoXMLNotaFiscal.CargaPedido.TipoUsoFatorCubagemRateioFormula, pedidoXMLNotaFiscal.CargaPedido.FatorCubagemRateioFormula ?? 0, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos);

                                                                    decimal baseCalculoRateada = svcRateio.AplicarFormulaRateio(formulaRateio, baseCalculo, totalNotasAgrupadas, 1, pesoParaCalculo, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotalNotasAgrupadas, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotaisNotasAgrupadas, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotalNotasAgrupadas, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotalNotasAgrupadas, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                                                                    decimal baseCalculoIBSCBSRateado = svcRateio.AplicarFormulaRateio(formulaRateio, baseCalculoIBSCBS, totalNotasAgrupadas, 1, pesoParaCalculo, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotalNotasAgrupadas, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotaisNotasAgrupadas, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotalNotasAgrupadas, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotalNotasAgrupadas, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);

                                                                    if (!emissaoSeraPorNota && totalNotasAgrupadas == 1)
                                                                    {
                                                                        if (totalBaseCalculoRateada == 0 && (baseCalculoRateada != cargaPedido.BaseCalculoICMS || icms != cargaPedido.ValorICMS))
                                                                        {
                                                                            baseCalculoRateada = cargaPedido.BaseCalculoICMS;
                                                                            baseCalculo = cargaPedido.BaseCalculoICMS;
                                                                            icms = cargaPedido.ValorICMS;
                                                                            icmsIncluso = cargaPedido.ValorICMSIncluso;
                                                                        }

                                                                        if (totalBaseCalculoIBSCBSRateado == 0 && (baseCalculoIBSCBSRateado != cargaPedido.BaseCalculoIBSCBS || valorIBSEstadual != cargaPedido.ValorIBSEstadual || valorIBSMunicipal != cargaPedido.ValorIBSMunicipal || valorCBS != cargaPedido.ValorCBS))
                                                                        {
                                                                            baseCalculoIBSCBSRateado = cargaPedido.BaseCalculoIBSCBS;
                                                                            baseCalculoIBSCBS = cargaPedido.BaseCalculoIBSCBS;
                                                                            valorIBSEstadual = cargaPedido.ValorIBSEstadual;
                                                                            valorIBSMunicipal = cargaPedido.ValorIBSMunicipal;
                                                                            valorCBS = cargaPedido.ValorCBS;
                                                                        }
                                                                    }

                                                                    decimal icmsRateado = svcRateio.AplicarFormulaRateio(formulaRateio, icms, totalNotasAgrupadas, 1, pesoParaCalculo, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotalNotasAgrupadas, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotaisNotasAgrupadas, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotalNotasAgrupadas, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotalNotasAgrupadas, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                                                                    decimal icmsInclusoRateado = svcRateio.AplicarFormulaRateio(formulaRateio, icmsIncluso, totalNotasAgrupadas, 1, pesoParaCalculo, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotalNotasAgrupadas, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotaisNotasAgrupadas, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotalNotasAgrupadas, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotalNotasAgrupadas, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                                                                    decimal creditoPresumidoRateado = svcRateio.AplicarFormulaRateio(formulaRateio, creditoPresumido, totalNotasAgrupadas, 1, pesoParaCalculo, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotalNotasAgrupadas, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotaisNotasAgrupadas, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotalNotasAgrupadas, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotalNotasAgrupadas, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                                                                    decimal pisRateado = svcRateio.AplicarFormulaRateio(formulaRateio, pis, totalNotasAgrupadas, 1, pesoParaCalculo, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotalNotasAgrupadas, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotaisNotasAgrupadas, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotalNotasAgrupadas, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotalNotasAgrupadas, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                                                                    decimal cofinsRateado = svcRateio.AplicarFormulaRateio(formulaRateio, cofins, totalNotasAgrupadas, 1, pesoParaCalculo, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotalNotasAgrupadas, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotaisNotasAgrupadas, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotalNotasAgrupadas, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotalNotasAgrupadas, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);

                                                                    decimal valorIBSEstadualRateado = svcRateio.AplicarFormulaRateio(formulaRateio, valorIBSEstadual, totalNotasAgrupadas, 1, pesoParaCalculo, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotalNotasAgrupadas, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotaisNotasAgrupadas, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotalNotasAgrupadas, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotalNotasAgrupadas, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                                                                    decimal valorIBSMunicipalRateado = svcRateio.AplicarFormulaRateio(formulaRateio, valorIBSMunicipal, totalNotasAgrupadas, 1, pesoParaCalculo, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotalNotasAgrupadas, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotaisNotasAgrupadas, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotalNotasAgrupadas, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotalNotasAgrupadas, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                                                                    decimal valorCBSRateado = svcRateio.AplicarFormulaRateio(formulaRateio, valorCBS, totalNotasAgrupadas, 1, pesoParaCalculo, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotalNotasAgrupadas, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotaisNotasAgrupadas, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotalNotasAgrupadas, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotalNotasAgrupadas, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);

                                                                    if (i == totalNotasAgrupadas - 1)
                                                                    {
                                                                        baseCalculoRateada = baseCalculo - totalBaseCalculoRateada;
                                                                        icmsRateado = icms - totalICMSRateado;
                                                                        icmsInclusoRateado = icmsIncluso - totalICMSInclusoRateado;
                                                                        creditoPresumidoRateado = creditoPresumido - totalCreditoPresumidoRateado;
                                                                        pisRateado = pis - totalPisRateado;
                                                                        cofinsRateado = cofins - totalCofinsRateado;

                                                                        baseCalculoIBSCBSRateado = baseCalculoIBSCBS - totalBaseCalculoIBSCBSRateado;
                                                                        valorIBSEstadualRateado = valorIBSEstadual - totalValorIBSEstadualRateado;
                                                                        valorIBSMunicipalRateado = valorIBSMunicipal - totalValorIBSMunicipalRateado;
                                                                        valorCBSRateado = valorCBS - totalValorCBSRateado;
                                                                    }

                                                                    totalICMSRateado += icmsRateado;
                                                                    totalICMSInclusoRateado += icmsInclusoRateado;
                                                                    totalBaseCalculoRateada += baseCalculoRateada;
                                                                    totalCreditoPresumidoRateado += creditoPresumidoRateado;
                                                                    totalPisRateado += pisRateado;
                                                                    totalCofinsRateado += cofinsRateado;

                                                                    totalBaseCalculoIBSCBSRateado += baseCalculoIBSCBSRateado;
                                                                    totalValorIBSEstadualRateado += valorIBSEstadualRateado;
                                                                    totalValorIBSMunicipalRateado += valorIBSMunicipalRateado;
                                                                    totalValorCBSRateado += valorCBSRateado;

                                                                    if (!rateioFreteFilialEmissora)
                                                                    {
                                                                        pedidoXMLNotaFiscal.ValorTotalComponentes = pedidoXMLNotaFiscalCompontesFreteAgrupados.Where(obj => obj.PedidoXMLNotaFiscal.Codigo == pedidoXMLNotaFiscal.Codigo).Sum(obj => obj.ValorComponente);
                                                                        pedidoXMLNotaFiscal.ValorTotalMoedaComponentes = pedidoXMLNotaFiscalCompontesFreteAgrupados.Where(obj => obj.PedidoXMLNotaFiscal.Codigo == pedidoXMLNotaFiscal.Codigo).Sum(obj => obj.ValorTotalMoeda ?? 0m);
                                                                        pedidoXMLNotaFiscal.ICMSPagoPorST = cargaPedido.ICMSPagoPorST;
                                                                        pedidoXMLNotaFiscal.PercentualAliquota = cargaPedido.PercentualAliquota;
                                                                        pedidoXMLNotaFiscal.AliquotaPis = cargaPedido.AliquotaPis;
                                                                        pedidoXMLNotaFiscal.AliquotaCofins = cargaPedido.AliquotaCofins;
                                                                        pedidoXMLNotaFiscal.PercentualAliquotaInternaDifal = cargaPedido.PercentualAliquotaInternaDifal;
                                                                        pedidoXMLNotaFiscal.CFOP = cargaPedido.CFOP;
                                                                        pedidoXMLNotaFiscal.CST = cargaPedido.CST;
                                                                        pedidoXMLNotaFiscal.IncluirICMSBaseCalculo = cargaPedido.IncluirICMSBaseCalculo;
                                                                        pedidoXMLNotaFiscal.PercentualIncluirBaseCalculo = cargaPedido.PercentualIncluirBaseCalculo;
                                                                        pedidoXMLNotaFiscal.PercentualReducaoBC = cargaPedido.PercentualReducaoBC;
                                                                        pedidoXMLNotaFiscal.ObservacaoRegraICMSCTe = cargaPedido.ObservacaoRegraICMSCTe;
                                                                        pedidoXMLNotaFiscal.PossuiCTe = true;
                                                                        pedidoXMLNotaFiscal.PossuiNFS = false;

                                                                        pedidoXMLNotaFiscal.PossuiNFSManual = false;
                                                                        pedidoXMLNotaFiscal.IncluirISSBaseCalculo = false;
                                                                        pedidoXMLNotaFiscal.ValorISS = 0m;
                                                                        pedidoXMLNotaFiscal.BaseCalculoISS = 0m;
                                                                        pedidoXMLNotaFiscal.PercentualAliquotaISS = 0m;
                                                                        pedidoXMLNotaFiscal.PercentualRetencaoISS = 0m;

                                                                        pedidoXMLNotaFiscal.DescontarICMSDoValorAReceber = cargaPedido.DescontarICMSDoValorAReceber;
                                                                        pedidoXMLNotaFiscal.NaoReduzirRetencaoICMSDoValorDaPrestacao = cargaPedido.NaoReduzirRetencaoICMSDoValorDaPrestacao;

                                                                        pedidoXMLNotaFiscal.PercentualCreditoPresumido = cargaPedido.PercentualCreditoPresumido;
                                                                        pedidoXMLNotaFiscal.ValorCreditoPresumido = creditoPresumidoRateado;

                                                                        if (pedidoXMLNotaFiscal.ModeloDocumentoFiscal == null || pedidoXMLNotaFiscal.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                                                                            pedidoXMLNotaFiscal.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.CTe);

                                                                        pedidoXMLNotaFiscal.ValorICMS = icmsRateado;
                                                                        pedidoXMLNotaFiscal.ValorPis = pisRateado;
                                                                        pedidoXMLNotaFiscal.ValorCofins = cofinsRateado;
                                                                        pedidoXMLNotaFiscal.ValorICMSIncluso = icmsInclusoRateado;
                                                                        pedidoXMLNotaFiscal.BaseCalculoICMS = baseCalculoRateada;

                                                                        servicoPedidoXMLNotaFiscal.PreencherCamposImpostoIBSCBSComTributacaoDefinida(pedidoXMLNotaFiscal, cargaPedido, baseCalculoIBSCBSRateado, valorIBSEstadualRateado, valorIBSMunicipalRateado, valorCBSRateado);

                                                                        repPedidoXMLNotaFiscal.Atualizar(pedidoXMLNotaFiscal);

                                                                        totalBaseCalculoICMS += pedidoXMLNotaFiscal.BaseCalculoICMS;
                                                                        totalICMS += pedidoXMLNotaFiscal.ValorICMS;
                                                                        totalPis += pedidoXMLNotaFiscal.ValorPis;
                                                                        totalCofins += pedidoXMLNotaFiscal.ValorCofins;
                                                                        totalCreditoPresumido += pedidoXMLNotaFiscal.ValorCreditoPresumido;

                                                                        if (pedidoXMLNotaFiscal.IncluirICMSBaseCalculo && pedidoXMLNotaFiscal.CST != "60")
                                                                        {
                                                                            if (pedidoXMLNotaFiscal.ValorICMSIncluso > 0)
                                                                                totalICMSIncluir += pedidoXMLNotaFiscal.ValorICMSIncluso;
                                                                            else
                                                                                totalICMSIncluir += pedidoXMLNotaFiscal.ValorICMS;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        pedidoXMLNotaFiscal.ICMSPagoPorSTFilialEmissora = cargaPedido.ICMSPagoPorSTFilialEmissora;
                                                                        pedidoXMLNotaFiscal.PercentualAliquotaFilialEmissora = cargaPedido.PercentualAliquotaFilialEmissora;
                                                                        pedidoXMLNotaFiscal.CFOPFilialEmissora = cargaPedido.CFOPFilialEmissora;
                                                                        pedidoXMLNotaFiscal.CSTFilialEmissora = cargaPedido.CSTFilialEmissora;
                                                                        pedidoXMLNotaFiscal.IncluirICMSBaseCalculoFilialEmissora = cargaPedido.IncluirICMSBaseCalculoFilialEmissora;
                                                                        pedidoXMLNotaFiscal.PercentualIncluirBaseCalculoFilialEmissora = cargaPedido.PercentualIncluirBaseCalculoFilialEmissora;
                                                                        pedidoXMLNotaFiscal.PercentualReducaoBCFilialEmissora = cargaPedido.PercentualReducaoBCFilialEmissora;
                                                                        pedidoXMLNotaFiscal.ObservacaoRegraICMSCTeFilialEmissora = cargaPedido.ObservacaoRegraICMSCTeFilialEmissora;
                                                                        pedidoXMLNotaFiscal.PossuiCTe = true;
                                                                        pedidoXMLNotaFiscal.PossuiNFS = false;
                                                                        pedidoXMLNotaFiscal.DescontarICMSDoValorAReceber = cargaPedido.DescontarICMSDoValorAReceber;
                                                                        pedidoXMLNotaFiscal.NaoReduzirRetencaoICMSDoValorDaPrestacao = cargaPedido.NaoReduzirRetencaoICMSDoValorDaPrestacao;

                                                                        pedidoXMLNotaFiscal.PercentualCreditoPresumidoFilialEmissora = cargaPedido.PercentualCreditoPresumidoFilialEmissora;
                                                                        pedidoXMLNotaFiscal.ValorCreditoPresumidoFilialEmissora = creditoPresumidoRateado;

                                                                        pedidoXMLNotaFiscal.ValorICMSFilialEmissora = icmsRateado;
                                                                        pedidoXMLNotaFiscal.BaseCalculoICMSFilialEmissora = baseCalculoRateada;

                                                                        servicoPedidoXMLNotaFiscal.PreencherCamposImpostoIBSCBSComTributacaoDefinidaFilialEmissora(pedidoXMLNotaFiscal, cargaPedido, baseCalculoIBSCBSRateado, valorIBSEstadualRateado, valorIBSMunicipalRateado, valorCBSRateado);

                                                                        repPedidoXMLNotaFiscal.Atualizar(pedidoXMLNotaFiscal);

                                                                        totalBaseCalculoICMS += pedidoXMLNotaFiscal.BaseCalculoICMSFilialEmissora;
                                                                        totalICMS += pedidoXMLNotaFiscal.ValorICMSFilialEmissora;
                                                                        totalCreditoPresumido += pedidoXMLNotaFiscal.ValorCreditoPresumidoFilialEmissora;

                                                                        if (pedidoXMLNotaFiscal.IncluirICMSBaseCalculoFilialEmissora && pedidoXMLNotaFiscal.CSTFilialEmissora != "60")
                                                                            totalICMSIncluir += pedidoXMLNotaFiscal.ValorICMSFilialEmissora;
                                                                    }

                                                                    totalBaseCalculoIBSCBS += baseCalculoIBSCBSRateado;
                                                                    totalValorIBSEstadual += valorIBSEstadualRateado;
                                                                    totalValorIBSMunicipal += valorIBSMunicipalRateado;
                                                                    totalValorCBS += valorCBSRateado;

                                                                    GerarComponenteICMS(pedidoXMLNotaFiscal, rateioFreteFilialEmissora, unitOfWork, componenteICMS, componentesICMSXMLNotaFiscalExistente);
                                                                    GerarComponentePisCofins(pedidoXMLNotaFiscal, unitOfWork, componentePisCofins, componentesPisCofinsXMLNotaFiscalExistente);
                                                                    adicionarDocumentosParaProvisao = true;

                                                                    Servicos.Log.GravarInfo($"Atualizando RatearFreteCargaPedidoEntreNotas 2 cargaPedido = {cargaPedido?.Codigo ?? 0}, pedidoXmlNotaFiscal = {pedidoXMLNotaFiscal.Codigo} com valorISS = {pedidoXMLNotaFiscal.ValorISS} e incluirISSBaseCalculo = {pedidoXMLNotaFiscal.IncluirISSBaseCalculo}, valorRetencaoISS = {pedidoXMLNotaFiscal.ValorRetencaoISS}", "AtualizarPedidoXMLNotaFiscal");
                                                                }
                                                                cargaPedido.PossuiCTe = true;
                                                            }
                                                            else
                                                            {
                                                                for (var i = 0; i < totalNotasAgrupadas; i++)
                                                                {
                                                                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = pedidoPedidoXMLNotsaFiscaisAgrupados[i];

                                                                    if (pedidoXMLNotaFiscal.PossuiCTe)
                                                                    {
                                                                        if (pedidoXMLNotaFiscal.ModeloDocumentoFiscal != null && pedidoXMLNotaFiscal.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                                                                            pedidoXMLNotaFiscal.ModeloDocumentoFiscal = cargaPedido.ModeloDocumentoFiscal;

                                                                        pedidoXMLNotaFiscal.PossuiCTe = false;

                                                                        repPedidoXMLNotaFiscal.Atualizar(pedidoXMLNotaFiscal);
                                                                    }

                                                                    pedidoXMLNotaFiscal.ICMSPagoPorST = false;
                                                                    pedidoXMLNotaFiscal.PercentualAliquota = 0m;
                                                                    pedidoXMLNotaFiscal.PercentualAliquotaInternaDifal = 0m;
                                                                    pedidoXMLNotaFiscal.IncluirICMSBaseCalculo = false;
                                                                    pedidoXMLNotaFiscal.PercentualIncluirBaseCalculo = 0m;
                                                                    pedidoXMLNotaFiscal.PercentualReducaoBC = 0m;
                                                                    pedidoXMLNotaFiscal.PercentualCreditoPresumido = 0m;
                                                                    pedidoXMLNotaFiscal.ValorCreditoPresumido = 0m;
                                                                    pedidoXMLNotaFiscal.ValorICMS = 0m;
                                                                    pedidoXMLNotaFiscal.ValorICMSIncluso = 0m;
                                                                    pedidoXMLNotaFiscal.BaseCalculoICMS = 0m;
                                                                    pedidoXMLNotaFiscal.SetarRegraICMS(0);

                                                                    servicoPedidoXMLNotaFiscal.ZerarCamposImpostoIBSCBS(pedidoXMLNotaFiscal);
                                                                }
                                                            }

                                                            if (!rateioFreteFilialEmissora)
                                                            {
                                                                if (possuiNFS || possuiNFSManual)
                                                                {
                                                                    decimal totalBaseCalculoRateada = 0m,
                                                                            totalISSRateado = 0m,
                                                                            totalRetencaoISSRateado = 0m,
                                                                            totalBaseCalculoIBSCBSRateado = 0m,
                                                                            totalValorIBSEstadualRateado = 0m,
                                                                            totalValorIBSMunicipalRateado = 0m,
                                                                            totalValorCBSRateado = 0m; ;

                                                                    decimal iss = Math.Round(serCargaISS.CalcularInclusaoISSNoFrete(ref baseCalculo, cargaPedido.PercentualAliquotaISS, cargaPedido.IncluirISSBaseCalculo), 2, MidpointRounding.AwayFromZero);
                                                                    decimal retencaoISS = Math.Round(serCargaISS.CalcularRetencaoISSNoFrete(iss, cargaPedido.PercentualRetencaoISS), 2, MidpointRounding.AwayFromZero);

                                                                    baseCalculoIBSCBS = baseCalculo - iss;

                                                                    Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS;

                                                                    if (rateioFreteFilialEmissora)
                                                                        impostoIBSCBS = servicoImpostoIBSCBS.ObterImpostoIBSCBSComTributacaoDefinidaFilialEmissora(cargaPedido, baseCalculoIBSCBS);
                                                                    else
                                                                        impostoIBSCBS = servicoImpostoIBSCBS.ObterImpostoIBSCBSComTributacaoDefinida(cargaPedido, baseCalculoIBSCBS);

                                                                    baseCalculoIBSCBS = impostoIBSCBS.BaseCalculo;
                                                                    decimal valorIBSEstadual = impostoIBSCBS.ValorIBSEstadual;
                                                                    decimal valorIBSMunicipal = impostoIBSCBS.ValorIBSMunicipal;
                                                                    decimal valorCBS = impostoIBSCBS.ValorCBS;

                                                                    for (var i = 0; i < totalNotasAgrupadas; i++)
                                                                    {
                                                                        Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = pedidoPedidoXMLNotsaFiscaisAgrupados[i];
                                                                        //decimal densidadeProdutos = pedidoXMLNotaFiscal.CargaPedido?.Produtos?.Sum(o => o.Produto?.MetroCubito) ?? 0m;
                                                                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProduto = (from obj in cargaPedidoProdutos where obj.CargaPedido.Codigo == pedidoXMLNotaFiscal.CargaPedido.Codigo select obj).ToList();
                                                                        decimal densidadeProdutos = (cargaPedidoProduto.Sum(obj => obj.Produto?.MetroCubito) ?? 0m);
                                                                        decimal valorRateioOriginal = 0;
                                                                        decimal baseCalculoRateada = svcRateio.AplicarFormulaRateio(formulaRateio, baseCalculo, totalNotasAgrupadas, 1, pesoTotalNotasAgrupadas, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotalNotasAgrupadas, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotaisNotasAgrupadas, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotalNotasAgrupadas, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotalNotasAgrupadas);
                                                                        decimal baseCalculoIBSCBSRateada = svcRateio.AplicarFormulaRateio(formulaRateio, baseCalculoIBSCBS, totalNotasAgrupadas, 1, pesoTotalNotasAgrupadas, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotalNotasAgrupadas, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotaisNotasAgrupadas, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotalNotasAgrupadas, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotalNotasAgrupadas);
                                                                        decimal issRateado = svcRateio.AplicarFormulaRateio(formulaRateio, iss, totalNotasAgrupadas, 1, pesoTotalNotasAgrupadas, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotalNotasAgrupadas, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotaisNotasAgrupadas, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotalNotasAgrupadas, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotalNotasAgrupadas);
                                                                        decimal retencaoISSRateado = svcRateio.AplicarFormulaRateio(formulaRateio, retencaoISS, totalNotasAgrupadas, 1, pesoTotalNotasAgrupadas, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotalNotasAgrupadas, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotaisNotasAgrupadas, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotalNotasAgrupadas, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotalNotasAgrupadas);

                                                                        decimal valorIBSEstadualRateado = svcRateio.AplicarFormulaRateio(formulaRateio, valorIBSEstadual, totalNotasAgrupadas, 1, pesoTotalNotasAgrupadas, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotalNotasAgrupadas, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotaisNotasAgrupadas, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotalNotasAgrupadas, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotalNotasAgrupadas);
                                                                        decimal valorIBSMunicipalRateado = svcRateio.AplicarFormulaRateio(formulaRateio, valorIBSMunicipal, totalNotasAgrupadas, 1, pesoTotalNotasAgrupadas, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotalNotasAgrupadas, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotaisNotasAgrupadas, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotalNotasAgrupadas, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotalNotasAgrupadas);
                                                                        decimal valorCBSRateado = svcRateio.AplicarFormulaRateio(formulaRateio, valorCBS, totalNotasAgrupadas, 1, pesoTotalNotasAgrupadas, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotalNotasAgrupadas, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotaisNotasAgrupadas, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotalNotasAgrupadas, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotalNotasAgrupadas);

                                                                        decimal pesoParaCalculoFatorCubagem = 0;

                                                                        if (formulaRateio?.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.PorPesoUtilizandoFatorCubagem)
                                                                            pesoParaCalculoFatorCubagem = serRateioFormula.ObterPesoCubadoFatorCubagem(formulaRateio.ParametroRateioFormula, pedidoXMLNotaFiscal.CargaPedido.TipoUsoFatorCubagemRateioFormula, pedidoXMLNotaFiscal.CargaPedido.FatorCubagemRateioFormula ?? 0, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos);

                                                                        if (i == totalNotasAgrupadas - 1)
                                                                        {
                                                                            baseCalculoRateada = baseCalculo - totalBaseCalculoRateada;
                                                                            issRateado = iss - totalISSRateado;
                                                                            retencaoISSRateado = retencaoISS - totalRetencaoISSRateado;
                                                                            baseCalculoIBSCBSRateada = baseCalculoIBSCBS - totalBaseCalculoIBSCBSRateado;
                                                                            valorIBSEstadualRateado = valorIBSEstadual - totalValorIBSEstadualRateado;
                                                                            valorIBSMunicipalRateado = valorIBSMunicipal - totalValorIBSMunicipalRateado;
                                                                            valorCBSRateado = valorCBS - totalValorCBSRateado;
                                                                        }

                                                                        totalRetencaoISSRateado += retencaoISSRateado;
                                                                        totalISSRateado += issRateado;
                                                                        totalBaseCalculoRateada += baseCalculoRateada;
                                                                        totalBaseCalculoIBSCBS += baseCalculoIBSCBSRateada;
                                                                        totalValorIBSEstadualRateado += valorIBSEstadualRateado;
                                                                        totalValorIBSMunicipalRateado += valorIBSMunicipalRateado;
                                                                        totalValorCBSRateado += valorCBSRateado;

                                                                        pedidoXMLNotaFiscal.PercentualAliquotaISS = cargaPedido.PercentualAliquotaISS;
                                                                        pedidoXMLNotaFiscal.PercentualRetencaoISS = cargaPedido.PercentualRetencaoISS;
                                                                        pedidoXMLNotaFiscal.IncluirISSBaseCalculo = cargaPedido.IncluirISSBaseCalculo;
                                                                        pedidoXMLNotaFiscal.ValorISS = issRateado;
                                                                        pedidoXMLNotaFiscal.ValorRetencaoISS = pedidoXMLNotaFiscal.PercentualRetencaoISS == 100 ? pedidoXMLNotaFiscal.ValorISS : retencaoISSRateado;
                                                                        pedidoXMLNotaFiscal.BaseCalculoISS = baseCalculoRateada;

                                                                        servicoPedidoXMLNotaFiscal.PreencherCamposImpostoIBSCBSComTributacaoDefinida(pedidoXMLNotaFiscal, cargaPedido, baseCalculoIBSCBSRateada, valorIBSEstadualRateado, valorIBSEstadualRateado, valorCBSRateado);

                                                                        if (!rateioFreteFilialEmissora)
                                                                            pedidoXMLNotaFiscal.ValorTotalComponentes = pedidoXMLNotaFiscalCompontesFreteAgrupados.Where(obj => obj.PedidoXMLNotaFiscal.Codigo == pedidoXMLNotaFiscal.Codigo).Sum(obj => obj.ValorComponente);

                                                                        pedidoXMLNotaFiscal.PossuiCTe = false;
                                                                        if (possuiNFS)
                                                                        {
                                                                            if (pedidoXMLNotaFiscal.ModeloDocumentoFiscal == null || pedidoXMLNotaFiscal.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                                                                                pedidoXMLNotaFiscal.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.NFSe);

                                                                            pedidoXMLNotaFiscal.PossuiNFS = true;
                                                                            pedidoXMLNotaFiscal.PossuiNFSManual = false;
                                                                        }
                                                                        else
                                                                        {
                                                                            pedidoXMLNotaFiscal.PossuiNFS = false;
                                                                            pedidoXMLNotaFiscal.PossuiNFSManual = true;
                                                                        }

                                                                        repPedidoXMLNotaFiscal.Atualizar(pedidoXMLNotaFiscal);

                                                                        totalBaseCalculoISS += pedidoXMLNotaFiscal.BaseCalculoISS;
                                                                        totalISS += pedidoXMLNotaFiscal.ValorISS;
                                                                        totalRetencaoISS += pedidoXMLNotaFiscal.ValorRetencaoISS;
                                                                        totalBaseCalculoIBSCBS += baseCalculoIBSCBSRateada;
                                                                        totalValorIBSEstadual += valorIBSEstadualRateado;
                                                                        totalValorIBSMunicipal += valorIBSMunicipalRateado;
                                                                        totalValorCBS += valorCBSRateado;

                                                                        if (pedidoXMLNotaFiscal.IncluirISSBaseCalculo)
                                                                            totalISSIncluir += pedidoXMLNotaFiscal.ValorISS;

                                                                        GerarComponenteISS(pedidoXMLNotaFiscal, unitOfWork);
                                                                        adicionarDocumentosParaProvisao = true;
                                                                    }

                                                                    if (possuiNFS)
                                                                    {
                                                                        cargaPedido.PossuiNFS = true;
                                                                        cargaPedido.PossuiNFSManual = false;
                                                                    }
                                                                    else
                                                                    {
                                                                        cargaPedido.PossuiNFS = false;
                                                                        cargaPedido.PossuiNFSManual = true;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    for (var i = 0; i < totalNotasAgrupadas; i++)
                                                                    {
                                                                        Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = pedidoPedidoXMLNotsaFiscaisAgrupados[i];

                                                                        if (!pedidoXMLNotaFiscal.PossuiNFSManual && !pedidoXMLNotaFiscal.PossuiNFS)
                                                                        {
                                                                            pedidoXMLNotaFiscal.ValorISS = 0;
                                                                            pedidoXMLNotaFiscal.BaseCalculoISS = 0;
                                                                            pedidoXMLNotaFiscal.PercentualAliquotaISS = 0;
                                                                            pedidoXMLNotaFiscal.PercentualRetencaoISS = 0;
                                                                            pedidoXMLNotaFiscal.IncluirISSBaseCalculo = false;
                                                                            pedidoXMLNotaFiscal.ValorRetencaoISS = 0;
                                                                            pedidoXMLNotaFiscal.PossuiNFS = false;
                                                                            pedidoXMLNotaFiscal.PossuiNFSManual = false;

                                                                            repPedidoXMLNotaFiscal.Atualizar(pedidoXMLNotaFiscal);
                                                                        }
                                                                    }

                                                                    cargaPedido.PossuiNFS = false;
                                                                    cargaPedido.PossuiNFSManual = false;
                                                                }
                                                            }
                                                        }
                                                    }

                                                    if (adicionarDocumentosParaProvisao)
                                                        pedidoXMLNotasFiscaisAdicionarParaProvisao.AddRange(pedidoPedidoXMLNotsaFiscaisAgrupados);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga tipoContratacaoCarga = cargaPedido.TipoContratacaoCarga;

                if (cargaPedido.CargaPedidoFilialEmissora && !rateioFreteFilialEmissora)
                    tipoContratacaoCarga = cargaPedido.TipoContratacaoCargaSubContratacaoFilialEmissora;

                bool ratearSub = false;
                if (!cargaPedido.CargaPedidoFilialEmissora)
                    ratearSub = true;
                else if (rateioFreteFilialEmissora && tipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal)
                    ratearSub = true;

                if (ratearSub)
                {
                    //if (tipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal
                    //     && (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ||
                    //         ((carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador || rateioFreteFilialEmissora || (carga.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador && cargaPedido.PedidoEncaixado)) && cargaPedido.CargaPedidoEncaixe == null)))
                    //{

                    if (tipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal && cargaPedido.CargaPedidoEncaixe == null)
                    {
                        Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaRateioSubContratante = serRateioFormula.ObterFormulaDeRateioSubContratante(carga, unitOfWork);
                        if (formulaRateioSubContratante == null)
                            formulaRateioSubContratante = formulaRateio;

                        decimal valorFreteCTes = valorFixoSubContratacaoParcial;
                        decimal valorTotalMoedaCTes = 0m;

                        List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> componentes = new List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico>();

                        if (valorFixoSubContratacaoParcial == 0m)
                        {
                            decimal valorRestanteRateio = cargaPedido.ValorFrete;
                            if (rateioFreteFilialEmissora)
                                valorRestanteRateio = cargaPedido.ValorFreteFilialEmissora;

                            if (!cargaPedido.EmitirComplementarFilialEmissora || !rateioFreteFilialEmissora)
                                valorFreteCTes = valorRestanteRateio - totalFreteCargaPedidoNotas;
                            else
                                valorFreteCTes = valorRestanteRateio;

                            valorTotalMoedaCTes = (cargaPedido.ValorTotalMoeda ?? 0m) - totalMoedaCargaPedidoNotas;

                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFrete in cargaPedidoComponentesFrete)
                            {
                                decimal valorTotalCargaPedidoComponente = repPedidoXMLNotaFiscalComponenteFrete.BuscarTotalCargaPedidoPorCompomente(cargaPedido.Codigo, cargaPedidoComponenteFrete.TipoComponenteFrete, cargaPedidoComponenteFrete.ComponenteFrete, rateioFreteFilialEmissora);
                                decimal valorTotalMoedaCargaPedidoComponente = repPedidoXMLNotaFiscalComponenteFrete.BuscarTotalMoedaCargaPedidoPorComponente(cargaPedido.Codigo, cargaPedidoComponenteFrete.TipoComponenteFrete, cargaPedidoComponenteFrete.ComponenteFrete, rateioFreteFilialEmissora);

                                decimal valorRestante = cargaPedidoComponenteFrete.ValorComponente - valorTotalCargaPedidoComponente;
                                decimal valorRestanteMoeda = (cargaPedidoComponenteFrete.ValorTotalMoeda ?? 0m) - valorTotalMoedaCargaPedidoComponente;

                                if (valorRestante > 0m)
                                {
                                    Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico componenteFreteDinamico = cargaPedidoComponenteFrete.ConvertarParaComponenteDinamico();
                                    componenteFreteDinamico.ValorComponente = valorRestante;
                                    componenteFreteDinamico.ValorTotalMoeda = valorRestanteMoeda;
                                    componentes.Add(componenteFreteDinamico);
                                }
                            }
                        }

                        serRateioCTeParaSubcontratacao.RatearFreteCargaPedidoEntreCTesParaSubcontratacao(cargaPedido, carga, valorFreteCTes, valorTotalMoedaCTes, componentes, formulaRateioSubContratante, rateioFreteFilialEmissora, cargaPedidoContaContabilContabilizacaos, tipoServicoMultisoftware, unitOfWork, componenteICMS, componentePisCofins, cargaPedidoCTeParaSubContratacaoNotasFiscais, cargaCTesParaSubContratacao, componentesICMSXMLNotaFiscalExistenteCarga, componentesPisConfisXMLNotaFiscalExistenteCarga);
                    }
                }


                RefazerComponentesCargaComBaseNasNotasFiscais(carga, ref cargaPedidosComponentesFreteCarga, unitOfWork);

                //valida o tipo de contrataoção pois se emitir subcontratação ou redespacho já seta as informações no pedido na hora do rateio no método RatearFreteCargaPedidoEntreCTesParaSubcontratacao
                if ((carga.TipoOperacao?.ConfiguracaoCalculoFrete?.MesclarValorEmbarcadorComTabelaFrete ?? false) ||
                    cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal ||
                    cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada ||
                    (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS
                    && (carga.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador || cargaPedido.CargaPedidoEncaixe != null || (cargaPedido.CargaPedidoTrechoAnterior != null && cargaPedido.CargaPedidoTrechoAnterior.CargaPedidoFilialEmissora) || (!rateioFreteFilialEmissora && cargaPedido.EmitirComplementarFilialEmissora))))
                {
                    if (emissaoSeraPorNota)
                    {
                        totalPisCarga += totalPis;
                        totalCofinsCarga += totalCofins;
                        totalICMSCarga += totalICMS;
                        totalISSCarga += totalISS;
                        totalRetencaoISSCarga += totalRetencaoISS;

                        totalValorIBSEstadualCarga += totalValorIBSEstadual;
                        totalValorIBSMunicipalCarga += totalValorIBSMunicipal;
                        totalValorCBSCarga += totalValorCBS;

                        decimal valorTotalComponentesPedido = repCargaPedidoComponentesFrete.BuscarValorComponentes(cargaPedido.Codigo, rateioFreteFilialEmissora);

                        if (!rateioFreteFilialEmissora)
                        {
                            cargaPedido.BaseCalculoICMS = totalBaseCalculoICMS;
                            cargaPedido.ValorICMS = totalICMS;
                            cargaPedido.ValorPis = totalPis;
                            cargaPedido.ValorCofins = totalCofins;
                            cargaPedido.ValorICMSIncluso = totalICMSIncluir;
                            cargaPedido.ValorISS = totalISS;
                            cargaPedido.ValorRetencaoISS = totalRetencaoISS;
                            cargaPedido.BaseCalculoISS = totalBaseCalculoISS;
                            cargaPedido.ValorCreditoPresumido = totalCreditoPresumido;
                            cargaPedido.ValorFreteAPagar = cargaPedido.ValorFrete + totalICMSIncluir + totalISSIncluir + valorTotalComponentesPedido;

                            servicoCargaPedido.PreencherValoresImpostoIBSCBS(cargaPedido, totalBaseCalculoIBSCBS, totalValorIBSEstadual, totalValorIBSMunicipal, totalValorCBS);
                        }
                        else
                        {
                            cargaPedido.BaseCalculoICMSFilialEmissora = totalBaseCalculoICMS;
                            cargaPedido.ValorICMSFilialEmissora = totalICMS;
                            cargaPedido.ValorICMSIncluso = totalICMS;
                            cargaPedido.ValorCreditoPresumidoFilialEmissora = totalCreditoPresumido;
                            cargaPedido.ValorFreteAPagarFilialEmissora = cargaPedido.ValorFreteFilialEmissora + totalICMSIncluir + totalISSIncluir + valorTotalComponentesPedido;

                            servicoCargaPedido.PreencherValoresImpostoIBSCBSFilialEmissora(cargaPedido, totalBaseCalculoIBSCBS, totalValorIBSEstadual, totalValorIBSMunicipal, totalValorCBS);
                        }

                        repCargaPedido.Atualizar(cargaPedido);
                    }
                    else
                    {
                        cargaPedido.PossuiCTe = possuiCTe;
                        cargaPedido.PossuiNFS = possuiNFS;
                        cargaPedido.PossuiNFSManual = possuiNFSManual;
                    }
                }
                else
                {
                    if (rateioFreteFilialEmissora)
                    {
                        totalICMSCarga += cargaPedido.ValorICMSFilialEmissora;
                        totalISSCarga += cargaPedido.ValorISS;
                        totalRetencaoISSCarga += cargaPedido.ValorRetencaoISS;

                        totalValorIBSEstadualCarga += cargaPedido.ValorIBSEstadualFilialEmissora;
                        totalValorIBSMunicipalCarga += cargaPedido.ValorIBSMunicipalFilialEmissora;
                        totalValorCBSCarga += cargaPedido.ValorCBSFilialEmissora;
                    }
                    else
                    {
                        totalPisCarga += cargaPedido.ValorPis;
                        totalCofinsCarga += cargaPedido.ValorCofins;
                        totalICMSCarga += cargaPedido.ValorICMS;
                        totalISSCarga += cargaPedido.ValorISS;
                        totalRetencaoISSCarga += cargaPedido.ValorRetencaoISS;

                        totalValorIBSEstadualCarga += cargaPedido.ValorIBSEstadual;
                        totalValorIBSMunicipalCarga += cargaPedido.ValorIBSMunicipal;
                        totalValorCBSCarga += cargaPedido.ValorCBS;
                    }
                }
            }

            serRateioCTeParaSubcontratacao.AjustarValoresImpostosEntrePedidos(carga, tipoServicoMultisoftware, unitOfWork);
            Servicos.Log.GravarInfo($"Notas para gerar Documento Provisão {string.Join(", ", pedidoXMLNotasFiscaisAdicionarParaProvisao?.Select(x => x.Codigo)?.ToList())}", "GeracaoDocumentosProvisao");
            for (int i = 0; i < pedidoXMLNotasFiscaisAdicionarParaProvisao.Count; i++)
                Servicos.Embarcador.Escrituracao.DocumentoProvisao.AdicionarDocumentoParaProvisao(pedidoXMLNotasFiscaisAdicionarParaProvisao[i], rateioFreteFilialEmissora, tipoServicoMultisoftware, unitOfWork, documentosProvisaoCarga, documentosEmissaoNFSCarga);

            if (abriuTransacao)
                unitOfWork.CommitChanges();

            if (existeEmissaoPorNota && rateouEntreNotas)
            {
                if (!rateioFreteFilialEmissora)
                {
                    carga.ValorFreteAPagar = cargaPedidosLiberados.Sum(o => o.ValorFreteAPagar);
                    carga.ValorISS = totalISSCarga;
                    carga.ValorRetencaoISS = totalRetencaoISSCarga;
                    carga.ValorICMS = totalICMSCarga;
                    carga.ValorPis = totalPisCarga;
                    carga.ValorCofins = totalCofinsCarga;

                    carga.ValorIBSEstadual = totalValorIBSEstadualCarga;
                    carga.ValorIBSMunicipal = totalValorIBSMunicipalCarga;
                    carga.ValorCBS = totalValorCBSCarga;

                    if (carga.TipoOperacao?.ConfiguracaoCalculoFrete?.MesclarValorEmbarcadorComTabelaFrete ?? false)
                        carga.ValorFrete = cargaPedidosLiberados.Sum(o => o.ValorFrete);
                }
                else
                {
                    carga.ValorFreteAPagarFilialEmissora = cargaPedidosLiberados.Sum(o => o.ValorFreteAPagarFilialEmissora);
                    carga.ValorICMSFilialEmissora = totalICMSCarga;

                    carga.ValorIBSEstadualFilialEmissora = totalValorIBSEstadualCarga;
                    carga.ValorIBSMunicipalFilialEmissora = totalValorIBSMunicipalCarga;
                    carga.ValorCBSFilialEmissora = totalValorCBSCarga;
                }

                repCarga.Atualizar(carga);

                serRateioFrete.AcrescentarValoresDaCargaAgrupada(carga, rateioFreteFilialEmissora, cargaPedidosLiberados, unitOfWork);
            }
        }

        private void RefazerComponentesCargaComBaseNasNotasFiscais(Dominio.Entidades.Embarcador.Cargas.Carga carga, ref List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidosComponentesFreteCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (!(carga.TipoOperacao?.ConfiguracaoCalculoFrete?.MesclarValorEmbarcadorComTabelaFrete ?? false))
                return;

            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponentesFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponenteFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentesFretePedidoXMLNotaFiscal = repPedidoXMLNotaFiscalComponenteFrete.BuscarPorCarga(carga.Codigo);

            List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFretes = repCargaComponenteFrete.BuscarPorCarga(carga.Codigo);

            List<int> codigosCargaPedidos = componentesFretePedidoXMLNotaFiscal.Select(o => o.PedidoXMLNotaFiscal.CargaPedido.Codigo).Distinct().ToList();
            List<int> codigosComponentesFretes = componentesFretePedidoXMLNotaFiscal.Select(o => o.ComponenteFrete.Codigo).Distinct().ToList();

            foreach (int codigoCargaPedido in codigosCargaPedidos)
            {
                //List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFretes = repCargaPedidoComponentesFrete.BuscarPorCargaPedido(codigoCargaPedido);

                foreach (int codigoComponenteFrete in codigosComponentesFretes)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete pedidoXMLNotaFiscalComponenteFrete = componentesFretePedidoXMLNotaFiscal.Where(o => o.PedidoXMLNotaFiscal.CargaPedido.Codigo == codigoCargaPedido && o.ComponenteFrete.Codigo == codigoComponenteFrete).FirstOrDefault();

                    if (pedidoXMLNotaFiscalComponenteFrete == null)
                        continue;

                    int index = cargaPedidosComponentesFreteCarga.FindIndex(o => o.CargaPedido.Codigo == codigoCargaPedido && o.ComponenteFrete.Codigo == codigoComponenteFrete);

                    if (index > -1)
                    {
                        cargaPedidosComponentesFreteCarga[index].ValorComponente = componentesFretePedidoXMLNotaFiscal.Where(o => o.PedidoXMLNotaFiscal.CargaPedido.Codigo == codigoCargaPedido && o.ComponenteFrete.Codigo == codigoComponenteFrete).Sum(o => o.ValorComponente);

                        repCargaPedidoComponentesFrete.Atualizar(cargaPedidosComponentesFreteCarga[index]);
                    }
                    else
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete()
                        {
                            AcrescentaValorTotalAReceber = pedidoXMLNotaFiscalComponenteFrete.AcrescentaValorTotalAReceber,
                            CargaPedido = pedidoXMLNotaFiscalComponenteFrete.PedidoXMLNotaFiscal.CargaPedido,
                            ComponenteFrete = pedidoXMLNotaFiscalComponenteFrete.ComponenteFrete,
                            DescontarValorTotalAReceber = pedidoXMLNotaFiscalComponenteFrete.DescontarValorTotalAReceber,
                            IncluirBaseCalculoICMS = pedidoXMLNotaFiscalComponenteFrete.IncluirBaseCalculoICMS,
                            IncluirIntegralmenteContratoFreteTerceiro = pedidoXMLNotaFiscalComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro,
                            ModeloDocumentoFiscal = pedidoXMLNotaFiscalComponenteFrete.ModeloDocumentoFiscal,
                            ModeloDocumentoFiscalRateio = pedidoXMLNotaFiscalComponenteFrete.ModeloDocumentoFiscalRateio,
                            Moeda = pedidoXMLNotaFiscalComponenteFrete.Moeda,
                            NaoSomarValorTotalAReceber = pedidoXMLNotaFiscalComponenteFrete.NaoSomarValorTotalAReceber,
                            DescontarDoValorAReceberValorComponente = pedidoXMLNotaFiscalComponenteFrete.DescontarDoValorAReceberValorComponente,
                            DescontarDoValorAReceberOICMSDoComponente = pedidoXMLNotaFiscalComponenteFrete.DescontarDoValorAReceberOICMSDoComponente,
                            ValorICMSComponenteDestacado = pedidoXMLNotaFiscalComponenteFrete.ValorICMSComponenteDestacado,
                            NaoSomarValorTotalPrestacao = pedidoXMLNotaFiscalComponenteFrete.NaoSomarValorTotalPrestacao,
                            OutraDescricaoCTe = pedidoXMLNotaFiscalComponenteFrete.OutraDescricaoCTe,
                            Percentual = pedidoXMLNotaFiscalComponenteFrete.Percentual,
                            PorQuantidadeDocumentos = pedidoXMLNotaFiscalComponenteFrete.PorQuantidadeDocumentos,
                            RateioFormula = pedidoXMLNotaFiscalComponenteFrete.RateioFormula,
                            TipoCalculoQuantidadeDocumentos = pedidoXMLNotaFiscalComponenteFrete.TipoCalculoQuantidadeDocumentos,
                            TipoComponenteFrete = pedidoXMLNotaFiscalComponenteFrete.TipoComponenteFrete,
                            TipoValor = pedidoXMLNotaFiscalComponenteFrete.TipoValor,
                            ValorComponente = componentesFretePedidoXMLNotaFiscal.Where(o => o.PedidoXMLNotaFiscal.CargaPedido.Codigo == codigoCargaPedido && o.ComponenteFrete.Codigo == codigoComponenteFrete).Sum(o => o.ValorComponente),
                            ValorCotacaoMoeda = pedidoXMLNotaFiscalComponenteFrete.ValorCotacaoMoeda,
                        };

                        repCargaPedidoComponentesFrete.Inserir(cargaPedidoComponenteFrete);

                        cargaPedidosComponentesFreteCarga.Add(cargaPedidoComponenteFrete);
                    }
                }
            }
        }

        private void SetarValoresEmbarcador(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, ref decimal baseCalculo, ref decimal valorTotalComponentes, ref Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal, ref List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> pedidoXMLNotaFiscalCompontesFrete, List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalComponente> componentesXMLNotaFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga.TipoOperacao?.ConfiguracaoCalculoFrete?.MesclarValorEmbarcadorComTabelaFrete ?? false)
            {
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(unitOfWork);

                int codigoXMLNotaFiscal = pedidoXMLNotaFiscal.XMLNotaFiscal.Codigo;
                int codigoPedidoXMLNotaFiscal = pedidoXMLNotaFiscal.Codigo;

                pedidoXMLNotaFiscal.ValorFrete += pedidoXMLNotaFiscal.XMLNotaFiscal.ValorFrete;
                cargaPedido.ValorFrete += pedidoXMLNotaFiscal.XMLNotaFiscal.ValorFrete;

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalComponente> componentesMesclar = componentesXMLNotaFiscal.Where(o => o.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal).ToList();

                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalComponente componenteMesclar in componentesMesclar)
                {
                    int index = pedidoXMLNotaFiscalCompontesFrete.FindIndex(o => o.PedidoXMLNotaFiscal.Codigo == codigoPedidoXMLNotaFiscal && o.ComponenteFrete.Codigo == componenteMesclar.ComponenteFrete.Codigo);

                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete pedidoXMLNotaFiscalComponenteFrete = null;

                    if (index > -1)
                    {
                        pedidoXMLNotaFiscalComponenteFrete = pedidoXMLNotaFiscalCompontesFrete.ElementAt(index);

                        pedidoXMLNotaFiscalComponenteFrete.ValorComponente += componenteMesclar.Valor;

                        if (pedidoXMLNotaFiscalComponenteFrete.IncluirBaseCalculoICMS)
                            baseCalculo += componenteMesclar.Valor;

                        repPedidoXMLNotaFiscalComponenteFrete.Atualizar(pedidoXMLNotaFiscalComponenteFrete);
                    }
                    else
                    {
                        Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = carga.TabelaFrete;
                        bool destacarComponenteTabelaFrete = Servicos.Embarcador.Carga.Frete.DestacarComponenteTabelaFrete(tabelaFrete, componenteMesclar.ComponenteFrete);

                        pedidoXMLNotaFiscalComponenteFrete = new Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete
                        {
                            ComponenteFrete = componenteMesclar.ComponenteFrete,
                            PedidoXMLNotaFiscal = pedidoXMLNotaFiscal,
                            TipoComponenteFrete = componenteMesclar.ComponenteFrete.TipoComponenteFrete,
                            OutraDescricaoCTe = componenteMesclar.ComponenteFrete.ImprimirOutraDescricaoCTe ? componenteMesclar.ComponenteFrete.DescricaoComponente : null,
                            ValorComponente = componenteMesclar.Valor,
                            ModeloDocumentoFiscal = cargaPedido.ModeloDocumentoFiscal,
                            IncluirBaseCalculoICMS = cargaPedido.IncluirICMSBaseCalculo,
                            Percentual = 0m,
                            TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor,
                            DescontarValorTotalAReceber = componenteMesclar.ComponenteFrete?.DescontarValorTotalAReceber ?? false,
                            ComponenteFilialEmissora = false,
                            AcrescentaValorTotalAReceber = componenteMesclar.ComponenteFrete?.AcrescentaValorTotalAReceber ?? false,
                            NaoSomarValorTotalAReceber = (destacarComponenteTabelaFrete ? tabelaFrete?.NaoSomarValorTotalAReceber : componenteMesclar.ComponenteFrete.NaoSomarValorTotalAReceber) ?? false,
                            DescontarDoValorAReceberValorComponente = (destacarComponenteTabelaFrete ? tabelaFrete?.DescontarDoValorAReceberValorComponente : componenteMesclar.ComponenteFrete.DescontarValorTotalAReceber) ?? false,
                            DescontarDoValorAReceberOICMSDoComponente = tabelaFrete?.DescontarDoValorAReceberOICMSDoComponente ?? false,
                            ValorICMSComponenteDestacado = tabelaFrete?.ValorICMSComponenteDestacado ?? 0,
                            NaoSomarValorTotalPrestacao = (destacarComponenteTabelaFrete ? tabelaFrete?.NaoSomarValorTotalPrestacao : componenteMesclar.ComponenteFrete.NaoSomarValorTotalPrestacao) ?? false,
                            Moeda = null,
                            ValorCotacaoMoeda = null,
                            ValorTotalMoeda = null,
                            IncluirIntegralmenteContratoFreteTerceiro = componenteMesclar.IncluirIntegralmenteContratoFreteTerceiro.HasValue ? componenteMesclar.IncluirIntegralmenteContratoFreteTerceiro.Value : false
                        };

                        repPedidoXMLNotaFiscalComponenteFrete.Inserir(pedidoXMLNotaFiscalComponenteFrete);

                        if (pedidoXMLNotaFiscalComponenteFrete.IncluirBaseCalculoICMS)
                            baseCalculo += componenteMesclar.Valor;

                        pedidoXMLNotaFiscalCompontesFrete.Add(pedidoXMLNotaFiscalComponenteFrete);
                    }

                    valorTotalComponentes += pedidoXMLNotaFiscalComponenteFrete.ValorComponente;
                }
            }
        }

        private void AplicarRateioISSNoPedidoNotaFiscal(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, int totalCTes, int numeroNotasRateio, decimal pesoTotal, decimal valorTotalNF, int numeroNotasFiscaisCTes, ref decimal SomaISSValorTotalPorCTe, ref decimal totalISSFreteCargaPedidoNotas, ref decimal SomaBaseCalculoTotalPorCTe, ref decimal totalBaseCalculoCargaPedidoNotas, ref decimal SomaValorRetencaoISSTotalPorCTe, ref decimal totalValoRetencaoISSCargaPedidoNotas, Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXmlNotaFiscal, Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaRateio, Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal ultimoPedidoXmlNotaFiscal, Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal ultimoPedidoXmlNotaFiscalNormalAgrupado, Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal ultimoPedidoXmlNotaFiscalNormal, Repositorio.UnitOfWork unitOfWork, decimal metrosCubicosTotais, decimal densidadeProdutosCargaPedido, decimal pesoLiquidoTotal, int VolumeTotal, decimal pesoTotalParaCalculoFatorCubagem,
            ref decimal SomaBaseCalculoIBSCBSTotalPorCTe, ref decimal SomaValorIBSEstadualTotalPorCTe, ref decimal SomaValorIBSMunicipalTotalPorCTe, ref decimal SomaValorCBSTotalPorCTe, ref decimal totalBaseCalculoIBSCBSCargaPedidoNotas, ref decimal totalValorIBSEstadualCargaPedidoNotas, ref decimal totalValorIBSMunicipalCargaPedidoNotas, ref decimal totalValorCBSCargaPedidoNotas)
        {
            Servicos.Embarcador.Carga.RateioFormula serRateioFormula = new Servicos.Embarcador.Carga.RateioFormula(unitOfWork);
            Servicos.Embarcador.Pedido.PedidoXMLNotaFiscal servicoPedidoXMLNotaFiscal = new Servicos.Embarcador.Pedido.PedidoXMLNotaFiscal(unitOfWork);

            decimal valorISS = cargaPedido.ValorISS;
            decimal valorRetencaoISS = cargaPedido.ValorRetencaoISS;
            decimal baseCalculoISS = cargaPedido.BaseCalculoISS;
            decimal baseCalculoIBSCBS = cargaPedido.BaseCalculoIBSCBS;
            decimal valorCBS = cargaPedido.ValorCBS;
            decimal valorIBSMunicipal = cargaPedido.ValorIBSMunicipal;
            decimal valorIBSEstadual = cargaPedido.ValorIBSEstadual;
            decimal densidadeProdutos = densidadeProdutosCargaPedido; //cargaPedido.Produtos?.Sum(o => o.Produto?.MetroCubito) ?? 0m;

            decimal valorISSFreteAplicadoRateio = 0m, valorRetencaoISSAplicadoRateio = 0m, valorBaseCalculoAplicadoRateio = 0m, baseCalculoIBSCBSAplicadoRateio = 0m;
            decimal valorRateioOriginal = 0, valorIBSMunicipalAplicadoRateio = 0m, valorIBSEstadualAplicadoRateio = 0m, valorCBSAplicadoRateio = 0m;

            decimal pesoParaCalculoFatorCubagem = 0;

            if (formulaRateio?.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.PorPesoUtilizandoFatorCubagem)
                pesoParaCalculoFatorCubagem = serRateioFormula.ObterPesoCubadoFatorCubagem(formulaRateio.ParametroRateioFormula, pedidoXmlNotaFiscal.CargaPedido.TipoUsoFatorCubagemRateioFormula, pedidoXmlNotaFiscal.CargaPedido.FatorCubagemRateioFormula ?? 0, pedidoXmlNotaFiscal.XMLNotaFiscal.Volumes, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.MetrosCubicos);

            valorISSFreteAplicadoRateio = serRateioFormula.AplicarFormulaRateio(formulaRateio, valorISS, numeroNotasRateio, totalCTes, pesoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXmlNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXmlNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Volumes, VolumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
            valorRetencaoISSAplicadoRateio = serRateioFormula.AplicarFormulaRateio(formulaRateio, valorRetencaoISS, numeroNotasRateio, totalCTes, pesoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXmlNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXmlNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Volumes, VolumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
            valorBaseCalculoAplicadoRateio = serRateioFormula.AplicarFormulaRateio(formulaRateio, baseCalculoISS, numeroNotasRateio, totalCTes, pesoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXmlNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXmlNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Volumes, VolumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
            baseCalculoIBSCBSAplicadoRateio = serRateioFormula.AplicarFormulaRateio(formulaRateio, baseCalculoIBSCBS, numeroNotasRateio, totalCTes, pesoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXmlNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXmlNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Volumes, VolumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
            valorCBSAplicadoRateio = serRateioFormula.AplicarFormulaRateio(formulaRateio, valorCBS, numeroNotasRateio, totalCTes, pesoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXmlNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXmlNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Volumes, VolumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
            valorIBSMunicipalAplicadoRateio = serRateioFormula.AplicarFormulaRateio(formulaRateio, valorIBSMunicipal, numeroNotasRateio, totalCTes, pesoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXmlNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXmlNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Volumes, VolumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
            valorIBSEstadualAplicadoRateio = serRateioFormula.AplicarFormulaRateio(formulaRateio, valorIBSEstadual, numeroNotasRateio, totalCTes, pesoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXmlNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXmlNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Volumes, VolumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);

            if (formulaRateio != null && formulaRateio.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porCTe)// quando o rateio é por cte é necessário dividir o valor por nota para chegar ao valor de cada nota fiscal do CT-e.
            {
                valorISSFreteAplicadoRateio = Math.Floor((valorISSFreteAplicadoRateio / numeroNotasFiscaisCTes) * 100) / 100;
                valorRetencaoISSAplicadoRateio = Math.Floor((valorRetencaoISSAplicadoRateio / numeroNotasFiscaisCTes) * 100) / 100;
                valorBaseCalculoAplicadoRateio = Math.Floor((valorBaseCalculoAplicadoRateio / numeroNotasFiscaisCTes) * 100) / 100;
                baseCalculoIBSCBSAplicadoRateio = Math.Floor((baseCalculoIBSCBSAplicadoRateio / numeroNotasFiscaisCTes) * 100) / 100;
                valorCBSAplicadoRateio = Math.Floor((valorCBSAplicadoRateio / numeroNotasFiscaisCTes) * 100) / 100;
                valorIBSMunicipalAplicadoRateio = Math.Floor((valorIBSMunicipalAplicadoRateio / numeroNotasFiscaisCTes) * 100) / 100;
                valorIBSEstadualAplicadoRateio = Math.Floor((valorIBSEstadualAplicadoRateio / numeroNotasFiscaisCTes) * 100) / 100;
            }

            pedidoXmlNotaFiscal.ValorISS = valorISSFreteAplicadoRateio;
            pedidoXmlNotaFiscal.ValorRetencaoISS = valorRetencaoISSAplicadoRateio;
            pedidoXmlNotaFiscal.BaseCalculoISS = valorBaseCalculoAplicadoRateio;

            servicoPedidoXMLNotaFiscal.PreencherValoresImpostoIBSCBSRateado(pedidoXmlNotaFiscal, baseCalculoIBSCBSAplicadoRateio, valorIBSEstadualAplicadoRateio, valorIBSMunicipalAplicadoRateio, valorCBSAplicadoRateio);

            SomaISSValorTotalPorCTe += valorISSFreteAplicadoRateio;
            SomaValorRetencaoISSTotalPorCTe += valorRetencaoISSAplicadoRateio;
            SomaBaseCalculoTotalPorCTe += valorBaseCalculoAplicadoRateio;
            SomaBaseCalculoIBSCBSTotalPorCTe += baseCalculoIBSCBSAplicadoRateio;
            SomaValorCBSTotalPorCTe += valorCBSAplicadoRateio;
            SomaValorIBSMunicipalTotalPorCTe += valorIBSMunicipalAplicadoRateio;
            SomaValorIBSEstadualTotalPorCTe += valorIBSEstadualAplicadoRateio;

            if (formulaRateio != null && formulaRateio.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porCTe && ultimoPedidoXmlNotaFiscalNormalAgrupado != null && pedidoXmlNotaFiscal.Equals(ultimoPedidoXmlNotaFiscalNormalAgrupado))
            {
                pedidoXmlNotaFiscal.ValorISS += valorISS - SomaISSValorTotalPorCTe;
                pedidoXmlNotaFiscal.ValorRetencaoISS += valorRetencaoISS - SomaValorRetencaoISSTotalPorCTe;
                pedidoXmlNotaFiscal.BaseCalculoISS += baseCalculoISS - SomaBaseCalculoTotalPorCTe;
                pedidoXmlNotaFiscal.BaseCalculoIBSCBS = baseCalculoIBSCBS - SomaBaseCalculoIBSCBSTotalPorCTe;
                pedidoXmlNotaFiscal.ValorCBS = valorCBS - SomaValorCBSTotalPorCTe;
                pedidoXmlNotaFiscal.ValorIBSMunicipal = valorIBSMunicipal - SomaValorIBSMunicipalTotalPorCTe;
                pedidoXmlNotaFiscal.ValorIBSEstadual = valorIBSEstadual - SomaValorIBSEstadualTotalPorCTe;
            }

            totalISSFreteCargaPedidoNotas += pedidoXmlNotaFiscal.ValorISS;
            totalValoRetencaoISSCargaPedidoNotas += pedidoXmlNotaFiscal.ValorRetencaoISS;
            totalBaseCalculoCargaPedidoNotas += pedidoXmlNotaFiscal.BaseCalculoISS;

            totalBaseCalculoIBSCBSCargaPedidoNotas += pedidoXmlNotaFiscal.BaseCalculoIBSCBS;
            totalValorIBSEstadualCargaPedidoNotas += pedidoXmlNotaFiscal.ValorIBSEstadual;
            totalValorIBSMunicipalCargaPedidoNotas += pedidoXmlNotaFiscal.ValorIBSMunicipal;
            totalValorCBSCargaPedidoNotas += pedidoXmlNotaFiscal.ValorCBS;

            pedidoXmlNotaFiscal.PercentualAliquotaISS = cargaPedido.PercentualAliquotaISS;
            pedidoXmlNotaFiscal.PercentualRetencaoISS = cargaPedido.PercentualRetencaoISS;
            pedidoXmlNotaFiscal.IncluirISSBaseCalculo = cargaPedido.IncluirISSBaseCalculo;

            pedidoXmlNotaFiscal.SetarRegraOutraAliquota(cargaPedido.OutrasAliquotas?.Codigo ?? 0);

            servicoPedidoXMLNotaFiscal.PreencherCamposImpostoIBSCBSComTributacaoDefinida(pedidoXmlNotaFiscal, cargaPedido);

            if ((ultimoPedidoXmlNotaFiscal != null && pedidoXmlNotaFiscal.Equals(ultimoPedidoXmlNotaFiscal)) || (ultimoPedidoXmlNotaFiscalNormal != null && pedidoXmlNotaFiscal.Equals(ultimoPedidoXmlNotaFiscalNormal)))
            {
                pedidoXmlNotaFiscal.ValorISS += valorISS - totalISSFreteCargaPedidoNotas;
                pedidoXmlNotaFiscal.ValorRetencaoISS += valorRetencaoISS - totalValoRetencaoISSCargaPedidoNotas;
                pedidoXmlNotaFiscal.BaseCalculoISS += baseCalculoISS - totalBaseCalculoCargaPedidoNotas;

                servicoPedidoXMLNotaFiscal.AcrescentarValoresImpostoIBSCBS(pedidoXmlNotaFiscal, baseCalculoIBSCBS - totalBaseCalculoIBSCBSCargaPedidoNotas, valorIBSEstadual - totalValorIBSEstadualCargaPedidoNotas, valorIBSMunicipal - totalValorIBSMunicipalCargaPedidoNotas, valorCBS - totalValorCBSCargaPedidoNotas);
            }

            Servicos.Log.GravarInfo($"Atualizando AplicarRateioISSNoPedidoNotaFiscal cargaPedido = {cargaPedido?.Codigo ?? 0}, pedidoXmlNotaFiscal = {pedidoXmlNotaFiscal.Codigo} com valorISS = {pedidoXmlNotaFiscal.ValorISS} e incluirISSBaseCalculo = {pedidoXmlNotaFiscal.IncluirISSBaseCalculo}, valorRetencaoISS = {pedidoXmlNotaFiscal.ValorRetencaoISS}", "AtualizarPedidoXMLNotaFiscal");

            GerarComponenteISS(pedidoXmlNotaFiscal, unitOfWork);
        }

        private void ZerarRateioISSNoPedidoNotaFiscal(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXmlNotaFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            if (pedidoXmlNotaFiscal.ValorISS <= 0)
                return;

            Servicos.Embarcador.Carga.RateioFormula serRateioFormula = new Servicos.Embarcador.Carga.RateioFormula(unitOfWork);
            Servicos.Embarcador.Pedido.PedidoXMLNotaFiscal servicoPedidoXMLNotaFiscal = new Servicos.Embarcador.Pedido.PedidoXMLNotaFiscal(unitOfWork);

            pedidoXmlNotaFiscal.ValorISS = 0;
            pedidoXmlNotaFiscal.BaseCalculoISS = 0;
            pedidoXmlNotaFiscal.PercentualAliquotaISS = 0;
            pedidoXmlNotaFiscal.PercentualRetencaoISS = 0;
            pedidoXmlNotaFiscal.IncluirISSBaseCalculo = false;
            pedidoXmlNotaFiscal.ValorRetencaoISS = 0;

            GerarComponenteISS(pedidoXmlNotaFiscal, unitOfWork);
        }

        private void AplicarRateioICMSNoPedidoNotaFiscal(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, bool rateioFreteFilialEmissora, int totalCTes, int numeroNotasRateio, decimal pesoTotal, decimal valorTotalNF, int numeroNotasFiscaisCTes, ref decimal SomaICMSValorTotalPorCTe, ref decimal SomaPisTotalPorCTe, ref decimal SomaCofinsTotalPorCTe, ref decimal totalICMSFreteCargaPedidoNotas, ref decimal totalPisCargaPedidoNotas, ref decimal totalCofinsCargaPedidoNotas, ref decimal SomaBaseCalculoTotalPorCTe, ref decimal totalBaseCalculoCargaPedidoNotas, ref decimal SomaCreditoPresumidoTotalPorCTe, ref decimal totalCreditoPresumidoCargaPedidoNotas, Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXmlNotaFiscal, Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaRateio, Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal ultimoPedidoXmlNotaFiscal, Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal ultimoPedidoXmlNotaFiscalNormalAgrupado, Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal ultimoPedidoXmlNotaFiscalNormal, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteICMS, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentesICMSXMLNotaFiscalExistente, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componentePisCofins, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentesPisCofinsXMLNotaFiscalExistente, Repositorio.UnitOfWork unitOfWork, decimal metrosCubicosTotais, decimal densidadeProdutosCargaPedido, ref decimal SomaICMSInclusoValorTotalPorCTe, ref decimal totalICMSInclusoFreteCargaPedidoNotas, decimal pesoLiquidoTotal, int volumeTotal, decimal pesoTotalParaCalculoFatorCubagem,
            ref decimal SomaBaseCalculoIBSCBSTotalPorCTe, ref decimal SomaValorIBSEstadualTotalPorCTe, ref decimal SomaValorIBSMunicipalTotalPorCTe, ref decimal SomaValorCBSTotalPorCTe, ref decimal totalBaseCalculoIBSCBSCargaPedidoNotas, ref decimal totalValorIBSEstadualCargaPedidoNotas, ref decimal totalValorIBSMunicipalCargaPedidoNotas, ref decimal totalValorCBSCargaPedidoNotas)
        {
            Servicos.Embarcador.Carga.RateioFormula serRateioFormula = new Servicos.Embarcador.Carga.RateioFormula(unitOfWork);
            Servicos.Embarcador.Pedido.PedidoXMLNotaFiscal servicoPedidoXMLNotaFiscal = new Servicos.Embarcador.Pedido.PedidoXMLNotaFiscal(unitOfWork);

            decimal valorPis = cargaPedido.ValorPis;
            decimal valorCofins = cargaPedido.ValorCofins;
            decimal valorICMS = cargaPedido.ValorICMS;
            decimal valorICMSIncluso = cargaPedido.ValorICMSIncluso;
            decimal valorCreditoPresumido = cargaPedido.ValorCreditoPresumido;
            decimal baseCalculo = cargaPedido.BaseCalculoICMS;
            decimal baseCalculoIBSCBS = cargaPedido.BaseCalculoIBSCBS;
            decimal valorIBSEstadual = cargaPedido.ValorIBSEstadual;
            decimal valorIBSMunicipal = cargaPedido.ValorIBSMunicipal;
            decimal valorCBS = cargaPedido.ValorCBS;
            decimal valorRateioOriginal = 0;

            if (rateioFreteFilialEmissora)
            {
                valorICMS = cargaPedido.ValorICMSFilialEmissora;
                valorCreditoPresumido = cargaPedido.ValorCreditoPresumidoFilialEmissora;
                baseCalculo = cargaPedido.BaseCalculoICMSFilialEmissora;

                baseCalculoIBSCBS = cargaPedido.BaseCalculoIBSCBSFilialEmissora;
                valorIBSEstadual = cargaPedido.ValorIBSEstadualFilialEmissora;
                valorIBSMunicipal = cargaPedido.ValorIBSMunicipalFilialEmissora;
                valorCBS = cargaPedido.ValorCBSFilialEmissora;
            }

            decimal valorPisAplicadoRateio = 0m, valorCofinsAplicadoRateio = 0m, valorICMSInclusoFreteAplicadoRateio = 0m, valorICMSFreteAplicadoRateio = 0m, valorCreditoPresumidoFreteAplicadoRateio = 0m, valorBaseCalculoAplicadoRateio = 0m;
            decimal baseCalculoIBSCBSAplicadoRateio = 0m, valorIBSEstadualAplicadoRateio = 0m, valorIBSMunicipalAplicadoRateio = 0m, valorCBSAplicadoRateio = 0m;
            decimal densidadeProdutos = densidadeProdutosCargaPedido;//pedidoXmlNotaFiscal.CargaPedido?.Produtos?.Sum(o => o.Produto?.MetroCubito) ?? 0m;

            decimal pesoParaCalculoFatorCubagem = 0;

            if (formulaRateio?.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.PorPesoUtilizandoFatorCubagem)
                pesoParaCalculoFatorCubagem = serRateioFormula.ObterPesoCubadoFatorCubagem(formulaRateio.ParametroRateioFormula, pedidoXmlNotaFiscal.CargaPedido.TipoUsoFatorCubagemRateioFormula, pedidoXmlNotaFiscal.CargaPedido.FatorCubagemRateioFormula ?? 0, pedidoXmlNotaFiscal.XMLNotaFiscal.Volumes, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.MetrosCubicos);

            //todo: esse or foi feito para a danone, pois la a nota já foi usada e contem o valor quando é usada novamente rever isso, criar um padrão onde o valor da nota seja fixado na carga e não na nota
            if (pedidoXmlNotaFiscal.XMLNotaFiscal.ValorFrete <= 0m || (pedidoXmlNotaFiscal.CargaPedido.Carga.TipoOperacao?.PermitirTransbordarNotasDeOutrasCargas ?? false))// se o valor do frete for previamente especificado na nota fiscal não é necessário fazer o rateio e sim utilizar o valor do frete que está na nota.
            {
                valorPisAplicadoRateio = serRateioFormula.AplicarFormulaRateio(formulaRateio, valorPis, numeroNotasRateio, totalCTes, pesoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXmlNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXmlNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Volumes, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                valorCofinsAplicadoRateio = serRateioFormula.AplicarFormulaRateio(formulaRateio, valorCofins, numeroNotasRateio, totalCTes, pesoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXmlNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXmlNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Volumes, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                valorICMSFreteAplicadoRateio = serRateioFormula.AplicarFormulaRateio(formulaRateio, valorICMS, numeroNotasRateio, totalCTes, pesoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXmlNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXmlNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Volumes, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                valorICMSInclusoFreteAplicadoRateio = serRateioFormula.AplicarFormulaRateio(formulaRateio, valorICMSIncluso, numeroNotasRateio, totalCTes, pesoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXmlNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXmlNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Volumes, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                valorCreditoPresumidoFreteAplicadoRateio = serRateioFormula.AplicarFormulaRateio(formulaRateio, valorCreditoPresumido, numeroNotasRateio, totalCTes, pesoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXmlNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXmlNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Volumes, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                valorBaseCalculoAplicadoRateio = serRateioFormula.AplicarFormulaRateio(formulaRateio, baseCalculo, numeroNotasRateio, totalCTes, pesoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXmlNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXmlNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Volumes, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);

                baseCalculoIBSCBSAplicadoRateio = serRateioFormula.AplicarFormulaRateio(formulaRateio, baseCalculoIBSCBS, numeroNotasRateio, totalCTes, pesoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXmlNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXmlNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Volumes, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                valorIBSEstadualAplicadoRateio = serRateioFormula.AplicarFormulaRateio(formulaRateio, valorIBSEstadual, numeroNotasRateio, totalCTes, pesoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXmlNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXmlNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Volumes, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                valorIBSMunicipalAplicadoRateio = serRateioFormula.AplicarFormulaRateio(formulaRateio, valorIBSMunicipal, numeroNotasRateio, totalCTes, pesoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXmlNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXmlNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Volumes, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                valorCBSAplicadoRateio = serRateioFormula.AplicarFormulaRateio(formulaRateio, valorCBS, numeroNotasRateio, totalCTes, pesoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXmlNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXmlNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Volumes, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);

                if (formulaRateio != null && formulaRateio.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porCTe)// quando o rateio é por cte é necessário dividir o valor por nota para chegar ao valor de cada nota fiscal do CT-e.
                {
                    valorPisAplicadoRateio = Math.Floor((valorPisAplicadoRateio / numeroNotasFiscaisCTes) * 100) / 100;
                    valorCofinsAplicadoRateio = Math.Floor((valorCofinsAplicadoRateio / numeroNotasFiscaisCTes) * 100) / 100;
                    valorICMSFreteAplicadoRateio = Math.Floor((valorICMSFreteAplicadoRateio / numeroNotasFiscaisCTes) * 100) / 100;
                    valorICMSInclusoFreteAplicadoRateio = Math.Floor((valorICMSInclusoFreteAplicadoRateio / numeroNotasFiscaisCTes) * 100) / 100;
                    valorCreditoPresumidoFreteAplicadoRateio = Math.Floor((valorCreditoPresumidoFreteAplicadoRateio / numeroNotasFiscaisCTes) * 100) / 100;
                    valorBaseCalculoAplicadoRateio = Math.Floor((valorBaseCalculoAplicadoRateio / numeroNotasFiscaisCTes) * 100) / 100;

                    baseCalculoIBSCBSAplicadoRateio = Math.Floor((baseCalculoIBSCBSAplicadoRateio / numeroNotasFiscaisCTes) * 100) / 100;
                    valorIBSEstadualAplicadoRateio = Math.Floor((valorIBSEstadualAplicadoRateio / numeroNotasFiscaisCTes) * 100) / 100;
                    valorIBSMunicipalAplicadoRateio = Math.Floor((valorIBSMunicipalAplicadoRateio / numeroNotasFiscaisCTes) * 100) / 100;
                    valorCBSAplicadoRateio = Math.Floor((valorCBSAplicadoRateio / numeroNotasFiscaisCTes) * 100) / 100;
                }
            }
            else
            {
                if (!rateioFreteFilialEmissora)
                {
                    valorICMSFreteAplicadoRateio = pedidoXmlNotaFiscal.ValorICMS;
                    valorPisAplicadoRateio = pedidoXmlNotaFiscal.ValorPis;
                    valorCofins = pedidoXmlNotaFiscal.ValorCofins;
                    valorICMSInclusoFreteAplicadoRateio = pedidoXmlNotaFiscal.ValorICMSIncluso;
                    valorCreditoPresumidoFreteAplicadoRateio = pedidoXmlNotaFiscal.ValorCreditoPresumido;
                    valorBaseCalculoAplicadoRateio = pedidoXmlNotaFiscal.BaseCalculoICMS;

                    baseCalculoIBSCBSAplicadoRateio = pedidoXmlNotaFiscal.BaseCalculoIBSCBS;
                    valorIBSEstadualAplicadoRateio = pedidoXmlNotaFiscal.ValorIBSEstadual;
                    valorIBSMunicipalAplicadoRateio = pedidoXmlNotaFiscal.ValorIBSMunicipal;
                    valorCBSAplicadoRateio = pedidoXmlNotaFiscal.ValorCBS;
                }
                else
                {
                    valorICMSFreteAplicadoRateio = pedidoXmlNotaFiscal.ValorICMSFilialEmissora;
                    valorCreditoPresumidoFreteAplicadoRateio = pedidoXmlNotaFiscal.ValorCreditoPresumidoFilialEmissora;
                    valorBaseCalculoAplicadoRateio = pedidoXmlNotaFiscal.BaseCalculoICMSFilialEmissora;

                    baseCalculoIBSCBSAplicadoRateio = pedidoXmlNotaFiscal.BaseCalculoIBSCBSFilialEmissora;
                    valorIBSEstadualAplicadoRateio = pedidoXmlNotaFiscal.ValorIBSEstadualFilialEmissora;
                    valorIBSMunicipalAplicadoRateio = pedidoXmlNotaFiscal.ValorIBSMunicipalFilialEmissora;
                    valorCBSAplicadoRateio = pedidoXmlNotaFiscal.ValorCBSFilialEmissora;
                }
            }

            if (!rateioFreteFilialEmissora)
            {
                pedidoXmlNotaFiscal.ValorPis = valorPisAplicadoRateio;
                pedidoXmlNotaFiscal.ValorCofins = valorCofinsAplicadoRateio;
                pedidoXmlNotaFiscal.ValorICMS = valorICMSFreteAplicadoRateio;
                pedidoXmlNotaFiscal.ValorICMSIncluso = valorICMSInclusoFreteAplicadoRateio;
                pedidoXmlNotaFiscal.ValorCreditoPresumido = valorCreditoPresumidoFreteAplicadoRateio;
                pedidoXmlNotaFiscal.BaseCalculoICMS = valorBaseCalculoAplicadoRateio;

                servicoPedidoXMLNotaFiscal.PreencherValoresImpostoIBSCBSRateado(pedidoXmlNotaFiscal, baseCalculoIBSCBSAplicadoRateio, valorIBSEstadualAplicadoRateio, valorIBSMunicipalAplicadoRateio, valorCBSAplicadoRateio);
            }
            else
            {
                pedidoXmlNotaFiscal.ValorICMSFilialEmissora = valorICMSFreteAplicadoRateio;
                pedidoXmlNotaFiscal.ValorCreditoPresumidoFilialEmissora = valorCreditoPresumidoFreteAplicadoRateio;
                pedidoXmlNotaFiscal.BaseCalculoICMSFilialEmissora = valorBaseCalculoAplicadoRateio;

                servicoPedidoXMLNotaFiscal.PreencherValoresImpostoIBSCBSFilialEmissoraRateado(pedidoXmlNotaFiscal, baseCalculoIBSCBSAplicadoRateio, valorIBSEstadualAplicadoRateio, valorIBSMunicipalAplicadoRateio, valorCBSAplicadoRateio);
            }

            SomaPisTotalPorCTe += valorPisAplicadoRateio;
            SomaCofinsTotalPorCTe += valorCofinsAplicadoRateio;
            SomaICMSValorTotalPorCTe += valorICMSFreteAplicadoRateio;
            SomaICMSInclusoValorTotalPorCTe += valorICMSInclusoFreteAplicadoRateio;
            SomaCreditoPresumidoTotalPorCTe += valorCreditoPresumidoFreteAplicadoRateio;
            SomaBaseCalculoTotalPorCTe += valorBaseCalculoAplicadoRateio;

            SomaBaseCalculoIBSCBSTotalPorCTe += baseCalculoIBSCBSAplicadoRateio;
            SomaValorIBSEstadualTotalPorCTe += valorIBSEstadualAplicadoRateio;
            SomaValorIBSMunicipalTotalPorCTe += valorIBSMunicipalAplicadoRateio;
            SomaValorCBSTotalPorCTe += valorCBSAplicadoRateio;

            if (formulaRateio != null && formulaRateio.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porCTe && ultimoPedidoXmlNotaFiscalNormalAgrupado != null && pedidoXmlNotaFiscal.Equals(ultimoPedidoXmlNotaFiscalNormalAgrupado))
            {
                if (!rateioFreteFilialEmissora)
                {
                    pedidoXmlNotaFiscal.ValorCofins += valorCofins - SomaCofinsTotalPorCTe;
                    pedidoXmlNotaFiscal.ValorPis += valorPis - SomaPisTotalPorCTe;
                    pedidoXmlNotaFiscal.ValorICMS += valorICMS - SomaICMSValorTotalPorCTe;
                    pedidoXmlNotaFiscal.ValorICMSIncluso += valorICMSIncluso - SomaICMSInclusoValorTotalPorCTe;
                    pedidoXmlNotaFiscal.ValorCreditoPresumido += valorCreditoPresumido - SomaCreditoPresumidoTotalPorCTe;
                    pedidoXmlNotaFiscal.BaseCalculoICMS += baseCalculo - SomaBaseCalculoTotalPorCTe;

                    servicoPedidoXMLNotaFiscal.AcrescentarValoresImpostoIBSCBS(pedidoXmlNotaFiscal, baseCalculoIBSCBS - SomaBaseCalculoIBSCBSTotalPorCTe, valorIBSEstadual - SomaValorIBSEstadualTotalPorCTe, valorIBSMunicipal - SomaValorIBSMunicipalTotalPorCTe, valorCBS - SomaValorCBSTotalPorCTe);
                }
                else
                {
                    pedidoXmlNotaFiscal.ValorICMSFilialEmissora += valorICMS - SomaICMSValorTotalPorCTe;
                    pedidoXmlNotaFiscal.ValorCreditoPresumidoFilialEmissora += valorCreditoPresumido - SomaCreditoPresumidoTotalPorCTe;
                    pedidoXmlNotaFiscal.BaseCalculoICMSFilialEmissora += baseCalculo - SomaBaseCalculoTotalPorCTe;

                    servicoPedidoXMLNotaFiscal.AcrescentarValoresImpostoIBSCBSFilialEmissora(pedidoXmlNotaFiscal, baseCalculoIBSCBS - SomaBaseCalculoIBSCBSTotalPorCTe, valorIBSEstadual - SomaValorIBSEstadualTotalPorCTe, valorIBSMunicipal - SomaValorIBSMunicipalTotalPorCTe, valorCBS - SomaValorCBSTotalPorCTe);
                }
            }

            if (!rateioFreteFilialEmissora)
            {
                totalPisCargaPedidoNotas += pedidoXmlNotaFiscal.ValorPis;
                totalCofinsCargaPedidoNotas += pedidoXmlNotaFiscal.ValorCofins;
                totalICMSFreteCargaPedidoNotas += pedidoXmlNotaFiscal.ValorICMS;
                totalICMSInclusoFreteCargaPedidoNotas += pedidoXmlNotaFiscal.ValorICMSIncluso;
                totalCreditoPresumidoCargaPedidoNotas += pedidoXmlNotaFiscal.ValorCreditoPresumido;
                totalBaseCalculoCargaPedidoNotas += pedidoXmlNotaFiscal.BaseCalculoICMS;

                totalBaseCalculoIBSCBSCargaPedidoNotas += pedidoXmlNotaFiscal.BaseCalculoIBSCBS;
                totalValorIBSEstadualCargaPedidoNotas += pedidoXmlNotaFiscal.ValorIBSEstadual;
                totalValorIBSMunicipalCargaPedidoNotas += pedidoXmlNotaFiscal.ValorIBSMunicipal;
                totalValorCBSCargaPedidoNotas += pedidoXmlNotaFiscal.ValorCBS;

                pedidoXmlNotaFiscal.CFOP = cargaPedido.CFOP;
                pedidoXmlNotaFiscal.CST = cargaPedido.CST;
                pedidoXmlNotaFiscal.PercentualAliquota = cargaPedido.PercentualAliquota;
                pedidoXmlNotaFiscal.AliquotaCofins = cargaPedido.AliquotaCofins;
                pedidoXmlNotaFiscal.AliquotaPis = cargaPedido.AliquotaPis;
                pedidoXmlNotaFiscal.PercentualIncluirBaseCalculo = cargaPedido.PercentualIncluirBaseCalculo;
                pedidoXmlNotaFiscal.PercentualReducaoBC = cargaPedido.PercentualReducaoBC;
                pedidoXmlNotaFiscal.IncluirICMSBaseCalculo = cargaPedido.IncluirICMSBaseCalculo;
                pedidoXmlNotaFiscal.ICMSPagoPorST = cargaPedido.ICMSPagoPorST;
                pedidoXmlNotaFiscal.DescontarICMSDoValorAReceber = cargaPedido.DescontarICMSDoValorAReceber;
                pedidoXmlNotaFiscal.NaoReduzirRetencaoICMSDoValorDaPrestacao = cargaPedido.NaoReduzirRetencaoICMSDoValorDaPrestacao;

                servicoPedidoXMLNotaFiscal.PreencherCamposImpostoIBSCBSComTributacaoDefinida(pedidoXmlNotaFiscal, cargaPedido);
            }
            else
            {
                totalICMSFreteCargaPedidoNotas += pedidoXmlNotaFiscal.ValorICMSFilialEmissora;
                totalCreditoPresumidoCargaPedidoNotas += pedidoXmlNotaFiscal.ValorCreditoPresumidoFilialEmissora;
                totalBaseCalculoCargaPedidoNotas += pedidoXmlNotaFiscal.BaseCalculoICMSFilialEmissora;

                totalBaseCalculoIBSCBSCargaPedidoNotas += pedidoXmlNotaFiscal.BaseCalculoIBSCBSFilialEmissora;
                totalValorIBSEstadualCargaPedidoNotas += pedidoXmlNotaFiscal.ValorIBSEstadualFilialEmissora;
                totalValorIBSMunicipalCargaPedidoNotas += pedidoXmlNotaFiscal.ValorIBSMunicipalFilialEmissora;
                totalValorCBSCargaPedidoNotas += pedidoXmlNotaFiscal.ValorCBSFilialEmissora;

                pedidoXmlNotaFiscal.CFOPFilialEmissora = cargaPedido.CFOPFilialEmissora;
                pedidoXmlNotaFiscal.CSTFilialEmissora = cargaPedido.CSTFilialEmissora;
                pedidoXmlNotaFiscal.PercentualAliquotaFilialEmissora = cargaPedido.PercentualAliquotaFilialEmissora;
                pedidoXmlNotaFiscal.PercentualIncluirBaseCalculoFilialEmissora = cargaPedido.PercentualIncluirBaseCalculoFilialEmissora;
                pedidoXmlNotaFiscal.PercentualReducaoBCFilialEmissora = cargaPedido.PercentualReducaoBCFilialEmissora;
                pedidoXmlNotaFiscal.IncluirICMSBaseCalculoFilialEmissora = cargaPedido.IncluirICMSBaseCalculoFilialEmissora;
                pedidoXmlNotaFiscal.ICMSPagoPorSTFilialEmissora = cargaPedido.ICMSPagoPorSTFilialEmissora;
                pedidoXmlNotaFiscal.DescontarICMSDoValorAReceber = cargaPedido.DescontarICMSDoValorAReceber;
                pedidoXmlNotaFiscal.NaoReduzirRetencaoICMSDoValorDaPrestacao = cargaPedido.NaoReduzirRetencaoICMSDoValorDaPrestacao;

                servicoPedidoXMLNotaFiscal.PreencherCamposImpostoIBSCBSComTributacaoDefinidaFilialEmissora(pedidoXmlNotaFiscal, cargaPedido);
            }

            Servicos.Log.GravarInfo($"Atualizando AplicarRateioICMSNoPedidoNotaFiscal cargaPedido = {cargaPedido?.Codigo ?? 0}, pedidoXmlNotaFiscal = {pedidoXmlNotaFiscal.Codigo} com valorISS = {pedidoXmlNotaFiscal.ValorISS} e incluirISSBaseCalculo = {pedidoXmlNotaFiscal.IncluirISSBaseCalculo}, valorRetencaoISS = {pedidoXmlNotaFiscal.ValorRetencaoISS}", "AtualizarPedidoXMLNotaFiscal");

            if ((ultimoPedidoXmlNotaFiscal != null && pedidoXmlNotaFiscal.Equals(ultimoPedidoXmlNotaFiscal)) || (ultimoPedidoXmlNotaFiscalNormal != null && pedidoXmlNotaFiscal.Equals(ultimoPedidoXmlNotaFiscalNormal)))
            {
                if (!rateioFreteFilialEmissora)
                {

                    pedidoXmlNotaFiscal.ValorPis += valorPis - totalPisCargaPedidoNotas;
                    pedidoXmlNotaFiscal.ValorCofins += valorCofins - totalCofinsCargaPedidoNotas;
                    pedidoXmlNotaFiscal.ValorICMS += valorICMS - totalICMSFreteCargaPedidoNotas;
                    pedidoXmlNotaFiscal.ValorICMSIncluso += valorICMSIncluso - totalICMSInclusoFreteCargaPedidoNotas;
                    pedidoXmlNotaFiscal.ValorCreditoPresumido += valorCreditoPresumido - totalCreditoPresumidoCargaPedidoNotas;
                    pedidoXmlNotaFiscal.BaseCalculoICMS += baseCalculo - totalBaseCalculoCargaPedidoNotas;

                    servicoPedidoXMLNotaFiscal.AcrescentarValoresImpostoIBSCBS(pedidoXmlNotaFiscal, baseCalculoIBSCBS - totalBaseCalculoIBSCBSCargaPedidoNotas, valorIBSEstadual - totalValorIBSEstadualCargaPedidoNotas, valorIBSMunicipal - totalValorIBSMunicipalCargaPedidoNotas, valorCBS - totalValorCBSCargaPedidoNotas);
                }
                else
                {
                    pedidoXmlNotaFiscal.ValorICMSFilialEmissora += valorICMS - totalICMSFreteCargaPedidoNotas;
                    pedidoXmlNotaFiscal.ValorCreditoPresumidoFilialEmissora += valorCreditoPresumido - totalCreditoPresumidoCargaPedidoNotas;
                    pedidoXmlNotaFiscal.BaseCalculoICMSFilialEmissora += baseCalculo - totalBaseCalculoCargaPedidoNotas;

                    servicoPedidoXMLNotaFiscal.AcrescentarValoresImpostoIBSCBSFilialEmissora(pedidoXmlNotaFiscal, baseCalculoIBSCBS - totalBaseCalculoIBSCBSCargaPedidoNotas, valorIBSEstadual - totalValorIBSEstadualCargaPedidoNotas, valorIBSMunicipal - totalValorIBSMunicipalCargaPedidoNotas, valorCBS - totalValorCBSCargaPedidoNotas);
                }
            }

            GerarComponenteICMS(pedidoXmlNotaFiscal, rateioFreteFilialEmissora, unitOfWork, componenteICMS, componentesICMSXMLNotaFiscalExistente);
            GerarComponentePisCofins(pedidoXmlNotaFiscal, unitOfWork, componentePisCofins, componentesPisCofinsXMLNotaFiscalExistente);
        }

        public void AplicarRateioNoPedidoNotaFiscal(decimal valorFreteNota, decimal valorFixoSubContratacaoParcial, int totalCTes, int numeroNotasRateio, decimal pesoTotal, decimal valorTotalNF, int numeroNotasFiscaisCTes, ref decimal SomaValorTotalPorCTe, ref decimal totalFreteCargaPedidoNotas, Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXmlNotaFiscal, Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaRateio, Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal ultimoPedidoXmlNotaFiscal, Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal ultimoPedidoXmlNotaFiscalNormalAgrupado, Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal ultimoPedidoXmlNotaFiscalNormal, bool rateioFreteFilialEmissora, decimal metrosCubicosTotais, decimal dencidadeTotalCargaPedido, decimal pesoLiquidoTotal, int volumeTotal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moeda, decimal cotacaoMoeda, decimal valorTotalMoeda, ref decimal somaValorTotalMoedaPorCTe, ref decimal totalMoedaCargaPedidoNotas, ref decimal SomaValorTotalPorCTeAgrupado, ref decimal valorFreteCTeOriginal, bool mesclarValorEmbarcadorComTabelaFrete = false, bool tipoEmissaoNaoeUmCTeNota = false, decimal pesoTotalParaCalculoFatorCubagem = 0)
        {
            Servicos.Embarcador.Carga.RateioFormula serRateioFormula = new Servicos.Embarcador.Carga.RateioFormula();

            decimal valorTotalFreteCargaPedido = valorFreteNota;
            decimal valorTotalPorCTe = Math.Floor((valorTotalFreteCargaPedido / totalCTes) * 100) / 100;
            decimal valorTotalMoedaPorCTe = 0m;
            decimal densidadeProdutos = dencidadeTotalCargaPedido;//pedidoXmlNotaFiscal.CargaPedido?.Produtos?.Sum(o => o.Produto?.MetroCubito) ?? 0m;
            decimal valorFreteAplicadoRateio = 0m, valorTotalMoedaAplicadoRateio = 0m;
            decimal valorRateioOriginal = 0;
            decimal pesoParaCalculoFatorCubagem = 0;
            if (formulaRateio?.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.PorPesoUtilizandoFatorCubagem)
                pesoParaCalculoFatorCubagem = serRateioFormula.ObterPesoCubadoFatorCubagem(formulaRateio.ParametroRateioFormula, pedidoXmlNotaFiscal.CargaPedido.TipoUsoFatorCubagemRateioFormula, pedidoXmlNotaFiscal.CargaPedido.FatorCubagemRateioFormula ?? 0, pedidoXmlNotaFiscal.XMLNotaFiscal.Volumes, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.MetrosCubicos);


            if (moeda != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real)
                valorTotalMoedaPorCTe = Math.Floor(valorTotalMoeda / totalCTes * 100) / 100;

            //todo: esse or foi feito para a danone, pois la a nota já foi usada e contem o valor quando é usada novamente rever isso, criar um padrão onde o valor da nota seja fixado na carga e não na nota
            if ((mesclarValorEmbarcadorComTabelaFrete || pedidoXmlNotaFiscal.XMLNotaFiscal.ValorFrete <= 0m) || (pedidoXmlNotaFiscal.CargaPedido.Carga.TipoOperacao?.PermitirTransbordarNotasDeOutrasCargas ?? false))// se o valor do frete for previamente especificado na nota fiscal não é necessário fazer o rateio e sim utilizar o valor do frete que está na nota.
            {
                if (moeda != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real)
                {
                    valorTotalMoedaAplicadoRateio = serRateioFormula.AplicarFormulaRateio(formulaRateio, valorTotalMoeda, numeroNotasRateio, totalCTes, pesoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXmlNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXmlNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Volumes, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);

                    if (formulaRateio != null && formulaRateio.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porCTe && !tipoEmissaoNaoeUmCTeNota)// quando o rateio é por cte é necessário dividir o valor por nota para chegar ao valor de cada nota fiscal do CT-e.
                        valorTotalMoedaAplicadoRateio = Math.Floor(valorTotalMoedaAplicadoRateio / numeroNotasFiscaisCTes * 100) / 100;

                    if ((formulaRateio?.ExigirConferenciaManual ?? false) && pedidoXmlNotaFiscal.ValorFreteAjusteManual > 0)
                        valorFreteAplicadoRateio = pedidoXmlNotaFiscal.ValorFreteAjusteManual;
                    else if (formulaRateio?.RatearEmBlocoDeEmissao ?? false)
                        valorFreteAplicadoRateio = Math.Round(valorTotalMoedaAplicadoRateio * cotacaoMoeda, 3, MidpointRounding.AwayFromZero);
                    else
                        valorFreteAplicadoRateio = Math.Round(valorTotalMoedaAplicadoRateio * cotacaoMoeda, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    if ((formulaRateio?.ExigirConferenciaManual ?? false) && pedidoXmlNotaFiscal.ValorFreteAjusteManual > 0)
                        valorFreteAplicadoRateio = pedidoXmlNotaFiscal.ValorFreteAjusteManual;
                    else
                        valorFreteAplicadoRateio = serRateioFormula.AplicarFormulaRateio(formulaRateio, valorTotalFreteCargaPedido, numeroNotasRateio, totalCTes, pesoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXmlNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXmlNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXmlNotaFiscal.XMLNotaFiscal.Volumes, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);

                    SomaValorTotalPorCTeAgrupado += valorRateioOriginal;
                    valorFreteCTeOriginal = valorRateioOriginal;

                    // quando o rateio é por cte é necessário dividir o valor por nota para chegar ao valor de cada nota fiscal do CT-e.
                    if (formulaRateio != null && !tipoEmissaoNaoeUmCTeNota
                        && formulaRateio.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porCTe
                        && pedidoXmlNotaFiscal.ValorFreteAjusteManual == 0
                       )
                        valorFreteAplicadoRateio = Math.Floor((valorFreteAplicadoRateio / numeroNotasFiscaisCTes) * 100) / 100;
                }
            }
            else
            {
                valorFreteAplicadoRateio = pedidoXmlNotaFiscal.XMLNotaFiscal.ValorFrete;
            }

            if (!rateioFreteFilialEmissora)
            {
                pedidoXmlNotaFiscal.Moeda = moeda;
                pedidoXmlNotaFiscal.ValorCotacaoMoeda = cotacaoMoeda;
                pedidoXmlNotaFiscal.ValorTotalMoeda = valorTotalMoedaAplicadoRateio;
                pedidoXmlNotaFiscal.ValorFrete = valorFreteAplicadoRateio;

                SomaValorTotalPorCTe += pedidoXmlNotaFiscal.ValorFrete;
                somaValorTotalMoedaPorCTe += pedidoXmlNotaFiscal.ValorTotalMoeda ?? 0m;
            }
            else
            {
                pedidoXmlNotaFiscal.ValorFreteFilialEmissora = valorFreteAplicadoRateio;
                SomaValorTotalPorCTe += pedidoXmlNotaFiscal.ValorFreteFilialEmissora;
            }


            if (((mesclarValorEmbarcadorComTabelaFrete || pedidoXmlNotaFiscal.XMLNotaFiscal.ValorFrete <= 0m) || (pedidoXmlNotaFiscal.CargaPedido.Carga.TipoOperacao?.PermitirTransbordarNotasDeOutrasCargas ?? false)) && formulaRateio != null && formulaRateio.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porCTe && ultimoPedidoXmlNotaFiscalNormalAgrupado != null && pedidoXmlNotaFiscal.Equals(ultimoPedidoXmlNotaFiscalNormalAgrupado) && !tipoEmissaoNaoeUmCTeNota)
            {
                if (!rateioFreteFilialEmissora)
                {
                    if (moeda != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real)
                    {
                        decimal diferenca = valorTotalMoedaPorCTe - somaValorTotalMoedaPorCTe;

                        if (diferenca != 0m)
                        {
                            pedidoXmlNotaFiscal.ValorTotalMoeda += diferenca;
                            if (formulaRateio?.RatearEmBlocoDeEmissao ?? false)
                                pedidoXmlNotaFiscal.ValorFrete = Math.Round((pedidoXmlNotaFiscal.ValorTotalMoeda ?? 0m) * cotacaoMoeda, 3, MidpointRounding.AwayFromZero);
                            else
                                pedidoXmlNotaFiscal.ValorFrete = Math.Round((pedidoXmlNotaFiscal.ValorTotalMoeda ?? 0m) * cotacaoMoeda, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    else
                    {
                        pedidoXmlNotaFiscal.ValorFrete += valorTotalPorCTe - SomaValorTotalPorCTe;
                    }
                }
                else
                    pedidoXmlNotaFiscal.ValorFreteFilialEmissora += valorTotalPorCTe - SomaValorTotalPorCTe;
            }

            if (!rateioFreteFilialEmissora)
            {
                totalFreteCargaPedidoNotas += pedidoXmlNotaFiscal.ValorFrete;
                totalMoedaCargaPedidoNotas += pedidoXmlNotaFiscal.ValorTotalMoeda ?? 0m;
            }
            else
                totalFreteCargaPedidoNotas += pedidoXmlNotaFiscal.ValorFreteFilialEmissora;

            if ((ultimoPedidoXmlNotaFiscal != null && pedidoXmlNotaFiscal.Equals(ultimoPedidoXmlNotaFiscal) && valorFixoSubContratacaoParcial == 0m) || (ultimoPedidoXmlNotaFiscalNormal != null && pedidoXmlNotaFiscal.Equals(ultimoPedidoXmlNotaFiscalNormal) && valorFixoSubContratacaoParcial > 0m))
            {
                if (!rateioFreteFilialEmissora)
                {
                    if (moeda != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real)
                    {
                        decimal diferenca = valorTotalMoeda - totalMoedaCargaPedidoNotas;

                        if (diferenca != 0m)
                        {
                            pedidoXmlNotaFiscal.ValorTotalMoeda += diferenca;
                            if (formulaRateio?.RatearEmBlocoDeEmissao ?? false)
                                pedidoXmlNotaFiscal.ValorFrete = Math.Round((pedidoXmlNotaFiscal.ValorTotalMoeda ?? 0m) * cotacaoMoeda, 3, MidpointRounding.AwayFromZero);
                            else
                                pedidoXmlNotaFiscal.ValorFrete = Math.Round((pedidoXmlNotaFiscal.ValorTotalMoeda ?? 0m) * cotacaoMoeda, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    else
                    {
                        pedidoXmlNotaFiscal.ValorFrete += valorTotalFreteCargaPedido - totalFreteCargaPedidoNotas;
                        SomaValorTotalPorCTeAgrupado = 0;
                    }
                }
                else
                    pedidoXmlNotaFiscal.ValorFreteFilialEmissora += valorTotalFreteCargaPedido - totalFreteCargaPedidoNotas;
            }

            Servicos.Log.GravarInfo($"Atualizando AplicarRateioNoPedidoNotaFiscal pedidoXmlNotaFiscal = {pedidoXmlNotaFiscal.Codigo} com valorISS = {pedidoXmlNotaFiscal.ValorISS} e incluirISSBaseCalculo = {pedidoXmlNotaFiscal.IncluirISSBaseCalculo}, valorRetencaoISS = {pedidoXmlNotaFiscal.ValorRetencaoISS}", "AtualizarPedidoXMLNotaFiscal");
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete AplicarRateioNosComponentesPedidoNotaFiscal(Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico cargaPedidoComponenteFreteDinamico, Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXmlNotaFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete pedidoXMLNotaFiscalComponente = new Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete
            {
                ComponenteFrete = cargaPedidoComponenteFreteDinamico.ComponenteFrete,
                PedidoXMLNotaFiscal = pedidoXmlNotaFiscal,
                TipoComponenteFrete = cargaPedidoComponenteFreteDinamico.TipoComponenteFrete,
                ModeloDocumentoFiscal = cargaPedidoComponenteFreteDinamico.ModeloDocumentoFiscal,
                RateioFormula = cargaPedidoComponenteFreteDinamico.RateioFormula,
                OutraDescricaoCTe = cargaPedidoComponenteFreteDinamico.OutraDescricaoCTe,
                ValorComponente = (cargaPedidoComponenteFreteDinamico.ComponenteFrete?.DescontarComponenteNotaFiscalServico ?? false) ? 0 : cargaPedidoComponenteFreteDinamico.ValorComponente,
                IncluirBaseCalculoICMS = cargaPedidoComponenteFreteDinamico.IncluirBaseCalculoImposto,
                Percentual = cargaPedidoComponenteFreteDinamico.Percentual,
                TipoValor = cargaPedidoComponenteFreteDinamico.TipoValor,
                DescontarValorTotalAReceber = cargaPedidoComponenteFreteDinamico.DescontarValorTotalAReceber,
                ComponenteFilialEmissora = cargaPedidoComponenteFreteDinamico.ComponenteFilialEmissora,
                AcrescentaValorTotalAReceber = cargaPedidoComponenteFreteDinamico.AcrescentaValorTotalAReceber,
                NaoSomarValorTotalAReceber = cargaPedidoComponenteFreteDinamico.NaoSomarValorTotalAReceber,
                DescontarDoValorAReceberValorComponente = cargaPedidoComponenteFreteDinamico.DescontarDoValorAReceberValorComponente,
                DescontarDoValorAReceberOICMSDoComponente = cargaPedidoComponenteFreteDinamico.DescontarDoValorAReceberOICMSDoComponente,
                ValorICMSComponenteDestacado = cargaPedidoComponenteFreteDinamico.ValorICMSComponenteDestacado,
                NaoSomarValorTotalPrestacao = cargaPedidoComponenteFreteDinamico.NaoSomarValorTotalPrestacao,
                Moeda = cargaPedidoComponenteFreteDinamico.Moeda,
                ValorCotacaoMoeda = cargaPedidoComponenteFreteDinamico.ValorCotacaoMoeda,
                ValorTotalMoeda = cargaPedidoComponenteFreteDinamico.ValorTotalMoeda,
                IncluirIntegralmenteContratoFreteTerceiro = cargaPedidoComponenteFreteDinamico.IncluirIntegralmenteContratoFreteTerceiro
            };

            repPedidoXMLNotaFiscalComponenteFrete.Inserir(pedidoXMLNotaFiscalComponente);

            return pedidoXMLNotaFiscalComponente;
        }

        public void GerarComponentePisCofins(Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componentePISCOFINS = null, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentesPISCOFINSXMLNotaFiscalExistente = null)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete pedidoXMLNotaFiscalComponenteFrete = null;

            if (componentesPISCOFINSXMLNotaFiscalExistente != null)
                pedidoXMLNotaFiscalComponenteFrete = componentesPISCOFINSXMLNotaFiscalExistente.Where(o => o.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS && o.PedidoXMLNotaFiscal.Codigo == pedidoXMLNotaFiscal.Codigo).FirstOrDefault();
            else
                pedidoXMLNotaFiscalComponenteFrete = repPedidoXMLNotaFiscalComponenteFrete.BuscarPorPedidoXMLNotaFiscalETipo(pedidoXMLNotaFiscal.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS, null, false);

            decimal valorPisCofins = pedidoXMLNotaFiscal.ValorCofins + pedidoXMLNotaFiscal.ValorPis;

            if (valorPisCofins > 0m)
            {
                if (componentePISCOFINS == null)
                {
                    Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
                    componentePISCOFINS = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS);
                }

                if (pedidoXMLNotaFiscalComponenteFrete == null)
                    pedidoXMLNotaFiscalComponenteFrete = new Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete();

                pedidoXMLNotaFiscalComponenteFrete.AcrescentaValorTotalAReceber = false;
                pedidoXMLNotaFiscalComponenteFrete.PedidoXMLNotaFiscal = pedidoXMLNotaFiscal;
                pedidoXMLNotaFiscalComponenteFrete.ComponenteFrete = componentePISCOFINS;
                pedidoXMLNotaFiscalComponenteFrete.DescontarValorTotalAReceber = false;
                pedidoXMLNotaFiscalComponenteFrete.IncluirBaseCalculoICMS = false;
                pedidoXMLNotaFiscalComponenteFrete.TipoComponenteFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS;
                pedidoXMLNotaFiscalComponenteFrete.ValorComponente = valorPisCofins;

                if (pedidoXMLNotaFiscalComponenteFrete.Codigo > 0)
                    repPedidoXMLNotaFiscalComponenteFrete.Atualizar(pedidoXMLNotaFiscalComponenteFrete);
                else
                    repPedidoXMLNotaFiscalComponenteFrete.Inserir(pedidoXMLNotaFiscalComponenteFrete);
            }
            else if (pedidoXMLNotaFiscalComponenteFrete != null)
            {
                repPedidoXMLNotaFiscalComponenteFrete.Deletar(pedidoXMLNotaFiscalComponenteFrete);
            }
        }

        public void GerarComponenteICMS(Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal, bool componenteFilialEmissora, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteICMS = null, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentesICMSXMLNotaFiscalExistente = null)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete pedidoXMLNotaFiscalComponenteFrete = null;

            if (componentesICMSXMLNotaFiscalExistente != null)
                pedidoXMLNotaFiscalComponenteFrete = componentesICMSXMLNotaFiscalExistente.Where(o => o.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS && o.PedidoXMLNotaFiscal.Codigo == pedidoXMLNotaFiscal.Codigo).FirstOrDefault();
            else
                pedidoXMLNotaFiscalComponenteFrete = repPedidoXMLNotaFiscalComponenteFrete.BuscarPorPedidoXMLNotaFiscalETipo(pedidoXMLNotaFiscal.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS, null, componenteFilialEmissora);

            string CST = pedidoXMLNotaFiscal.CST;
            bool incluirICMSBaseCalculo = pedidoXMLNotaFiscal.IncluirICMSBaseCalculo;
            decimal valorICMS = pedidoXMLNotaFiscal.ValorICMS;
            decimal valorICMSIncluso = pedidoXMLNotaFiscal.ValorICMSIncluso;

            if (componenteFilialEmissora)
            {
                CST = pedidoXMLNotaFiscal.CSTFilialEmissora;
                incluirICMSBaseCalculo = pedidoXMLNotaFiscal.IncluirICMSBaseCalculoFilialEmissora;
                valorICMS = pedidoXMLNotaFiscal.ValorICMSFilialEmissora;
            }

            if (incluirICMSBaseCalculo && CST != "60" && (valorICMS > 0m || valorICMSIncluso > 0m))
            {
                if (componenteICMS == null)
                {
                    Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
                    componenteICMS = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS);
                }

                if (pedidoXMLNotaFiscalComponenteFrete == null)
                    pedidoXMLNotaFiscalComponenteFrete = new Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete();

                pedidoXMLNotaFiscalComponenteFrete.AcrescentaValorTotalAReceber = false;
                pedidoXMLNotaFiscalComponenteFrete.PedidoXMLNotaFiscal = pedidoXMLNotaFiscal;
                pedidoXMLNotaFiscalComponenteFrete.ComponenteFilialEmissora = componenteFilialEmissora;
                pedidoXMLNotaFiscalComponenteFrete.ComponenteFrete = componenteICMS;
                pedidoXMLNotaFiscalComponenteFrete.DescontarValorTotalAReceber = false;
                pedidoXMLNotaFiscalComponenteFrete.IncluirBaseCalculoICMS = false;
                pedidoXMLNotaFiscalComponenteFrete.TipoComponenteFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS;
                pedidoXMLNotaFiscalComponenteFrete.ValorComponente = valorICMSIncluso > 0m ? valorICMSIncluso : valorICMS;

                if (pedidoXMLNotaFiscalComponenteFrete.Codigo > 0)
                    repPedidoXMLNotaFiscalComponenteFrete.Atualizar(pedidoXMLNotaFiscalComponenteFrete);
                else
                    repPedidoXMLNotaFiscalComponenteFrete.Inserir(pedidoXMLNotaFiscalComponenteFrete);
            }
            else if (pedidoXMLNotaFiscalComponenteFrete != null)
            {
                repPedidoXMLNotaFiscalComponenteFrete.Deletar(pedidoXMLNotaFiscalComponenteFrete);
            }
        }

        public void GerarComponenteISS(Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(unitOfWork);

            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS);
            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete pedidoXMLNotaFiscalComponenteFrete = repPedidoXMLNotaFiscalComponenteFrete.BuscarPorPedidoXMLNotaFiscalETipo(pedidoXMLNotaFiscal.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS, null, false);

            Servicos.Log.GravarInfo($"Atualizando GerarComponenteISS pedidoXmlNotaFiscal = {pedidoXMLNotaFiscal.Codigo} com valorISS = {pedidoXMLNotaFiscal.ValorISS} e incluirISSBaseCalculo = {pedidoXMLNotaFiscal.IncluirISSBaseCalculo}, valorRetencaoISS = {pedidoXMLNotaFiscal.ValorRetencaoISS}", "AtualizarPedidoXMLNotaFiscal");

            if (pedidoXMLNotaFiscal.IncluirISSBaseCalculo && pedidoXMLNotaFiscal.ValorISS > 0m)
            {
                if (pedidoXMLNotaFiscalComponenteFrete == null)
                    pedidoXMLNotaFiscalComponenteFrete = new Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete();

                pedidoXMLNotaFiscalComponenteFrete.AcrescentaValorTotalAReceber = false;
                pedidoXMLNotaFiscalComponenteFrete.PedidoXMLNotaFiscal = pedidoXMLNotaFiscal;
                pedidoXMLNotaFiscalComponenteFrete.ComponenteFrete = componenteFrete;
                pedidoXMLNotaFiscalComponenteFrete.DescontarValorTotalAReceber = false;
                pedidoXMLNotaFiscalComponenteFrete.IncluirBaseCalculoICMS = false;
                pedidoXMLNotaFiscalComponenteFrete.TipoComponenteFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS;
                pedidoXMLNotaFiscalComponenteFrete.ValorComponente = pedidoXMLNotaFiscal.ValorISS;

                if (pedidoXMLNotaFiscalComponenteFrete.Codigo > 0)
                    repPedidoXMLNotaFiscalComponenteFrete.Atualizar(pedidoXMLNotaFiscalComponenteFrete);
                else
                    repPedidoXMLNotaFiscalComponenteFrete.Inserir(pedidoXMLNotaFiscalComponenteFrete);
            }
            else if (pedidoXMLNotaFiscalComponenteFrete != null)
            {
                repPedidoXMLNotaFiscalComponenteFrete.Deletar(pedidoXMLNotaFiscalComponenteFrete);
            }
        }

        public void GerarComponenteISS(Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete repPedidoCteParaSubContratacaoComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete(unitOfWork);

            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS);
            Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete pedidoCteParaSubContratacaoComponenteFrete = repPedidoCteParaSubContratacaoComponenteFrete.BuscarPorPedidoCteParaSubContratacaoETipo(pedidoCTeParaSubContratacao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS, null);

            if (pedidoCTeParaSubContratacao.IncluirISSBaseCalculo && pedidoCTeParaSubContratacao.ValorISS > 0m)
            {
                if (pedidoCteParaSubContratacaoComponenteFrete == null)
                    pedidoCteParaSubContratacaoComponenteFrete = new Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete();

                pedidoCteParaSubContratacaoComponenteFrete.NaoSomarValorTotalAReceber = false;
                pedidoCteParaSubContratacaoComponenteFrete.NaoSomarValorTotalPrestacao = false;
                pedidoCteParaSubContratacaoComponenteFrete.AcrescentaValorTotalAReceber = false;
                pedidoCteParaSubContratacaoComponenteFrete.PedidoCTeParaSubContratacao = pedidoCTeParaSubContratacao;
                pedidoCteParaSubContratacaoComponenteFrete.ComponenteFrete = componenteFrete;
                pedidoCteParaSubContratacaoComponenteFrete.DescontarValorTotalAReceber = false;
                pedidoCteParaSubContratacaoComponenteFrete.TipoComponenteFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS;
                pedidoCteParaSubContratacaoComponenteFrete.ValorComponente = pedidoCTeParaSubContratacao.ValorISS;

                if (pedidoCteParaSubContratacaoComponenteFrete.Codigo > 0)
                    repPedidoCteParaSubContratacaoComponenteFrete.Atualizar(pedidoCteParaSubContratacaoComponenteFrete);
                else
                    repPedidoCteParaSubContratacaoComponenteFrete.Inserir(pedidoCteParaSubContratacaoComponenteFrete);
            }
            else if (pedidoCteParaSubContratacaoComponenteFrete != null)
            {
                repPedidoCteParaSubContratacaoComponenteFrete.Deletar(pedidoCteParaSubContratacaoComponenteFrete);
            }
        }

        #endregion

        #region Métodos Privados

        private string InformarDadosContabeisPedidoNotaFiscal(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal, bool emissaoPorNota, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao> cargaPedidoContaContabilContabilizacaosCarga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            string retorno = "";
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return retorno;


            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalContaContabilContabilizacao repPedidoXMLNotaFiscalContaContabilContabilizacao = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalContaContabilContabilizacao(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            if (emissaoPorNota)
            {
                Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado serConfiguracaoCentroResultado = new ConfiguracaoContabil.ConfiguracaoCentroResultado();
                Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil serConfiguracaoContaContabil = new ConfiguracaoContabil.ConfiguracaoContaContabil();

                Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil configuracaoContaContabil = serConfiguracaoContaContabil.ObterConfiguracaoContaContabil(pedidoXMLNotaFiscal.XMLNotaFiscal.Emitente, pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario, cargaPedido.ObterTomador(), null, carga.Empresa, carga.TipoOperacao, carga.Rota, cargaPedido.ModeloDocumentoFiscal, null, unitOfWork);

                if ((configuracaoContaContabil != null) && (configuracaoContaContabil.ConfiguracaoContaContabilContabilizacoes?.Count > 0))
                {
                    foreach (Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilContabilizacao configuracaoContabilizacao in configuracaoContaContabil.ConfiguracaoContaContabilContabilizacoes)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalContaContabilContabilizacao pedidoXMLNotaFiscalContaContabilContabilizacao = new Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalContaContabilContabilizacao();

                        pedidoXMLNotaFiscalContaContabilContabilizacao.PedidoXMLNotaFiscal = pedidoXMLNotaFiscal;
                        if (configuracaoContabilizacao.CodigoPlanoConta > 0)
                            pedidoXMLNotaFiscalContaContabilContabilizacao.PlanoConta = new Dominio.Entidades.Embarcador.Financeiro.PlanoConta() { Codigo = configuracaoContabilizacao.CodigoPlanoConta };
                        pedidoXMLNotaFiscalContaContabilContabilizacao.TipoContabilizacao = configuracaoContabilizacao.TipoContabilizacao;
                        if (configuracaoContabilizacao.CodigoPlanoContaContraPartidaProvisao > 0)
                            pedidoXMLNotaFiscalContaContabilContabilizacao.PlanoContaContraPartida = new Dominio.Entidades.Embarcador.Financeiro.PlanoConta() { Codigo = configuracaoContabilizacao.CodigoPlanoContaContraPartidaProvisao };
                        pedidoXMLNotaFiscalContaContabilContabilizacao.TipoContaContabil = configuracaoContabilizacao.TipoContaContabil;
                        repPedidoXMLNotaFiscalContaContabilContabilizacao.Inserir(pedidoXMLNotaFiscalContaContabilContabilizacao);
                    }
                }
                else if ((carga.TipoOperacao?.InserirDadosContabeisXCampoXTextCTe ?? false) || configuracao.CentroResultadoPedidoObrigatorio)
                {
                    retorno = "Não foi localizada uma configuração de conta contábil compatível com a nota fiscal " + pedidoXMLNotaFiscal.XMLNotaFiscal.Numero.ToString();
                    cargaPedido.Carga.PossuiPendenciaConfiguracaoContabil = true;
                    repCarga.Atualizar(cargaPedido.Carga);
                }

                bool logisticaReversa = carga.TipoOperacao?.LogisticaReversa ?? false;

                Dominio.Entidades.Cliente destinatario = !logisticaReversa ? pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario : pedidoXMLNotaFiscal.XMLNotaFiscal.Emitente;
                Dominio.Entidades.Cliente remetente = !logisticaReversa ? pedidoXMLNotaFiscal.XMLNotaFiscal.Emitente : pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario;
                Dominio.Entidades.Cliente expedidor = !logisticaReversa ? pedidoXMLNotaFiscal.XMLNotaFiscal.Expedidor : pedidoXMLNotaFiscal.XMLNotaFiscal.Recebedor;
                Dominio.Entidades.Cliente recebedor = !logisticaReversa ? pedidoXMLNotaFiscal.XMLNotaFiscal.Recebedor : pedidoXMLNotaFiscal.XMLNotaFiscal.Expedidor;
                Dominio.Entidades.Localidade origem = !logisticaReversa ? pedidoXMLNotaFiscal.XMLNotaFiscal.Emitente.Localidade : pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario.Localidade;

                if (configuracao.ArmazenarCentroCustoDestinatario)
                    destinatario = null;


                Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado configuracaoCentroResultado = serConfiguracaoCentroResultado.ObterConfiguracaoCentroResultado(remetente, destinatario, expedidor, recebedor, cargaPedido.ObterTomador(), null, null, carga.Empresa, carga.TipoOperacao, null, carga.Rota, carga.Filial, origem, unitOfWork);
                if (configuracaoCentroResultado != null)
                {
                    pedidoXMLNotaFiscal.CentroResultado = configuracaoCentroResultado.CentroResultadoContabilizacao;
                    pedidoXMLNotaFiscal.ItemServico = configuracaoCentroResultado.ItemServico;
                    pedidoXMLNotaFiscal.ValorMaximoCentroContabilizacao = configuracaoCentroResultado.ValorMaximoCentroContabilizacao;
                    pedidoXMLNotaFiscal.CentroResultadoEscrituracao = configuracaoCentroResultado.CentroResultadoEscrituracao;
                    pedidoXMLNotaFiscal.CentroResultadoICMS = configuracaoCentroResultado.CentroResultadoICMS;
                    pedidoXMLNotaFiscal.CentroResultadoPIS = configuracaoCentroResultado.CentroResultadoPIS;
                    pedidoXMLNotaFiscal.CentroResultadoCOFINS = configuracaoCentroResultado.CentroResultadoCOFINS;
                }
                else
                {
                    if ((carga.TipoOperacao?.InserirDadosContabeisXCampoXTextCTe ?? false) || configuracao.CentroResultadoPedidoObrigatorio)
                    {
                        retorno = "Não foi localizada uma configuração de centro de resultado compatível com a nota fiscal " + pedidoXMLNotaFiscal.XMLNotaFiscal.Numero.ToString();
                        cargaPedido.Carga.PossuiPendenciaConfiguracaoContabil = true;
                        repCarga.Atualizar(cargaPedido.Carga);
                    }
                    pedidoXMLNotaFiscal.CentroResultado = null;
                    pedidoXMLNotaFiscal.ItemServico = "";
                    pedidoXMLNotaFiscal.ValorMaximoCentroContabilizacao = 0;
                    pedidoXMLNotaFiscal.CentroResultadoEscrituracao = null;
                    pedidoXMLNotaFiscal.CentroResultadoICMS = null;
                    pedidoXMLNotaFiscal.CentroResultadoPIS = null;
                    pedidoXMLNotaFiscal.CentroResultadoCOFINS = null;
                }


                if (configuracao.ArmazenarCentroCustoDestinatario)
                {
                    destinatario = !logisticaReversa ? pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario : pedidoXMLNotaFiscal.XMLNotaFiscal.Emitente;

                    Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado configuracaoCentroResultadoDestinatario = serConfiguracaoCentroResultado.ObterConfiguracaoCentroResultado(null, destinatario, null, null, cargaPedido.ObterTomador(), null, null, carga.Empresa, carga.TipoOperacao, null, carga.Rota, carga.Filial, origem, unitOfWork);
                    if (configuracaoCentroResultadoDestinatario != null)
                        pedidoXMLNotaFiscal.CentroResultadoDestinatario = configuracaoCentroResultadoDestinatario.CentroResultadoContabilizacao;
                    else
                    {
                        pedidoXMLNotaFiscal.CentroResultadoDestinatario = null;
                        if ((carga.TipoOperacao?.InserirDadosContabeisXCampoXTextCTe ?? false) || configuracao.CentroResultadoPedidoObrigatorio)
                        {
                            retorno = "Não foi localizada uma configuração de centro de resultado de destino compatível com a nota fiscal " + pedidoXMLNotaFiscal.XMLNotaFiscal.Numero.ToString();
                            cargaPedido.Carga.PossuiPendenciaConfiguracaoContabil = true;
                            repCarga.Atualizar(cargaPedido.Carga);
                        }
                    }
                }
            }
            else
            {
                pedidoXMLNotaFiscal.CentroResultado = cargaPedido.CentroResultado;
                pedidoXMLNotaFiscal.ItemServico = cargaPedido.ItemServico;
                pedidoXMLNotaFiscal.CentroResultadoEscrituracao = cargaPedido.CentroResultadoEscrituracao;
                pedidoXMLNotaFiscal.CentroResultadoICMS = cargaPedido.CentroResultadoICMS;
                pedidoXMLNotaFiscal.CentroResultadoPIS = cargaPedido.CentroResultadoPIS;
                pedidoXMLNotaFiscal.CentroResultadoCOFINS = cargaPedido.CentroResultadoCOFINS;

                pedidoXMLNotaFiscal.ValorMaximoCentroContabilizacao = cargaPedido.ValorMaximoCentroContabilizacao;
                pedidoXMLNotaFiscal.CentroResultadoDestinatario = cargaPedido.CentroResultadoDestinatario;
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao> cargaPedidoContaContabilContabilizacaos = (from obj in cargaPedidoContaContabilContabilizacaosCarga where obj.CargaPedido.Codigo == cargaPedido.Codigo select obj).ToList();
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao cargaPedidoContaContabilContabilizacao in cargaPedidoContaContabilContabilizacaos)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalContaContabilContabilizacao pedidoXMLNotaFiscalContaContabilContabilizacao = new Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalContaContabilContabilizacao();

                    pedidoXMLNotaFiscalContaContabilContabilizacao.PedidoXMLNotaFiscal = pedidoXMLNotaFiscal;
                    pedidoXMLNotaFiscalContaContabilContabilizacao.PlanoConta = cargaPedidoContaContabilContabilizacao.PlanoConta;
                    pedidoXMLNotaFiscalContaContabilContabilizacao.TipoContabilizacao = cargaPedidoContaContabilContabilizacao.TipoContabilizacao;
                    pedidoXMLNotaFiscalContaContabilContabilizacao.PlanoContaContraPartida = cargaPedidoContaContabilContabilizacao.PlanoContaContraPartida;
                    pedidoXMLNotaFiscalContaContabilContabilizacao.TipoContaContabil = cargaPedidoContaContabilContabilizacao.TipoContaContabil;
                    repPedidoXMLNotaFiscalContaContabilContabilizacao.Inserir(pedidoXMLNotaFiscalContaContabilContabilizacao);
                }
            }

            Servicos.Log.GravarInfo($"Atualizando InformarDadosContabeisPedidoNotaFiscal pedidoXmlNotaFiscal = {pedidoXMLNotaFiscal.Codigo} com valorISS = {pedidoXMLNotaFiscal.ValorISS} e incluirISSBaseCalculo = {pedidoXMLNotaFiscal.IncluirISSBaseCalculo}, valorRetencaoISS = {pedidoXMLNotaFiscal.ValorRetencaoISS}", "AtualizarPedidoXMLNotaFiscal");

            return retorno;
        }

        public void CalcularImpostos(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool calculoImpostosFilialEmissora, Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXmlNotaFiscal, decimal valorBaseCalculo, bool incluirICMSBase, decimal percentualIncluiFrete, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> pedidoXMLNotaFiscalCompontesFrete, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Cliente expedidor, Dominio.Entidades.Cliente recebedor, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, List<Dominio.Entidades.Aliquota> aliquotasUfEmpresa = null, List<Dominio.Entidades.CFOP> cfops = null, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentesICMSXMLNotaFiscalExistente = null, List<Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo> pedagioEstadosBaseCalculo = null, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteICMS = null, bool produtoEmbarcadorConsultar = true, Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = null)
        {
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);

            Servicos.Embarcador.Carga.ICMS serCargaICMS = new Servicos.Embarcador.Carga.ICMS(unitOfWork);
            Servicos.Embarcador.Carga.ISS serCargaISS = new Servicos.Embarcador.Carga.ISS(unitOfWork);
            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
            Servicos.Embarcador.Pedido.PedidoXMLNotaFiscal servicoPedidoXMLNotaFiscal = new Servicos.Embarcador.Pedido.PedidoXMLNotaFiscal(unitOfWork);
            Servicos.Embarcador.Imposto.ImpostoIBSCBS servicoImpostoIBSCBS = new Servicos.Embarcador.Imposto.ImpostoIBSCBS(unitOfWork);

            ObterBaseCalculo(ref valorBaseCalculo, calculoImpostosFilialEmissora, carga, cargaPedido, pedidoXmlNotaFiscal, unitOfWork, tipoServicoMultisoftware, pedidoXMLNotaFiscalCompontesFrete, pedagioEstadosBaseCalculo);

            decimal baseCalculoIBSCBS = valorBaseCalculo;

            if (!calculoImpostosFilialEmissora)
            {
                pedidoXmlNotaFiscal.ValorTotalComponentes = pedidoXMLNotaFiscalCompontesFrete.Sum(o => o.ValorComponente);
                pedidoXmlNotaFiscal.ValorTotalMoedaComponentes = pedidoXMLNotaFiscalCompontesFrete.Sum(o => o.ValorTotalMoeda ?? 0m);
            }

            Dominio.Entidades.Cliente rementete = pedidoXmlNotaFiscal.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida ? pedidoXmlNotaFiscal.XMLNotaFiscal.Emitente : pedidoXmlNotaFiscal.XMLNotaFiscal.Destinatario;
            Dominio.Entidades.Cliente destintario = pedidoXmlNotaFiscal.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida ? pedidoXmlNotaFiscal.XMLNotaFiscal.Destinatario : pedidoXmlNotaFiscal.XMLNotaFiscal.Emitente;

            Dominio.Entidades.Localidade origem = expedidor?.Localidade ?? rementete.Localidade;
            Dominio.Entidades.Localidade destino = recebedor?.Localidade ?? destintario.Localidade;

            if (carga.TipoOperacao?.ConfiguracaoEmissaoDocumento?.EmitirDocumentoSempreOrigemDestinoPedido ?? false)
            {
                origem = rementete.Localidade;
                destino = destintario.Localidade;
            }
            else if (recebedor != null)
                destintario = recebedor;

            Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();

            if (configuracao.UtilizarLocalidadePrestacaoPedido)
            {
                if (cargaPedido.Pedido.LocalidadeInicioPrestacao != null)
                    origem = cargaPedido.Pedido.LocalidadeInicioPrestacao;

                if (cargaPedido.Pedido.LocalidadeTerminoPrestacao != null)
                    destino = cargaPedido.Pedido.LocalidadeTerminoPrestacao;
            }

            if (tomador != null)
            {
                bool possuiCTe = false;
                bool possuiNFS = false;
                bool possuiNFSManual = false;
                Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalIntramunicipal = null;

                serCargaPedido.VerificarQuaisDocumentosDeveEmitir(carga, cargaPedido, origem, destino, tipoServicoMultisoftware, unitOfWork, out possuiCTe, out possuiNFS, out possuiNFSManual, out modeloDocumentoFiscalIntramunicipal, configuracao, out bool sempreDisponibilizarDocumentoNFSManual);

                cargaPedido.DisponibilizarDocumentoNFSManual = sempreDisponibilizarDocumentoNFSManual;

                Dominio.Entidades.Empresa empresa = carga.Empresa;

                if (calculoImpostosFilialEmissora)
                    empresa = carga.EmpresaFilialEmissora;

                if (possuiCTe)
                {
                    if (cargaPedido.ModeloDocumentoFiscal?.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                        cargaPedido.ModeloDocumentoFiscal = null;

                    if (pedidoXmlNotaFiscal.ModeloDocumentoFiscal == null || pedidoXmlNotaFiscal.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                        pedidoXmlNotaFiscal.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.CTe);


                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga tipoContratacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal;

                    if (pedidoXmlNotaFiscal.TipoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao)
                        tipoContratacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada;

                    Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = serCargaICMS.BuscarRegraICMS(carga, cargaPedido, empresa, rementete, destintario, tomador, origem, destino, ref incluirICMSBase, ref percentualIncluiFrete, valorBaseCalculo, produtoEmbarcador, unitOfWork, tipoServicoMultisoftware, configuracao, tipoContratacao, null, false, Dominio.Enumeradores.TipoCTE.Normal, aliquotasUfEmpresa, produtoEmbarcadorConsultar);

                    if (!regraICMS.IncluiICMSBaseCalculo)
                        baseCalculoIBSCBS -= Math.Round(regraICMS.ValorICMS, 2, MidpointRounding.AwayFromZero);

                    Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = servicoImpostoIBSCBS.ObterImpostoIBSCBS(new Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBS
                    {
                        BaseCalculo = baseCalculoIBSCBS,
                        CodigoLocalidade = pedidoXmlNotaFiscal.XMLNotaFiscal.Destinatario?.Localidade?.Codigo ?? cargaPedido.Pedido.Destino?.Codigo ?? 0,
                        SiglaUF = pedidoXmlNotaFiscal.XMLNotaFiscal.Destinatario?.Localidade?.Estado?.Sigla ?? cargaPedido.Pedido.Destino?.Estado?.Sigla ?? "",
                        CodigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0,
                        Empresa = empresa
                    });

                    if (!calculoImpostosFilialEmissora)
                    {
                        if (cfops != null)
                            pedidoXmlNotaFiscal.CFOP = cfops.Where(o => o.CodigoCFOP == regraICMS.CFOP).FirstOrDefault();
                        else
                            pedidoXmlNotaFiscal.CFOP = repCFOP.BuscarPorNumero(regraICMS.CFOP);
                        pedidoXmlNotaFiscal.CST = regraICMS.CST;
                        pedidoXmlNotaFiscal.IncluirICMSBaseCalculo = incluirICMSBase;
                        pedidoXmlNotaFiscal.PercentualIncluirBaseCalculo = percentualIncluiFrete;
                        pedidoXmlNotaFiscal.PercentualReducaoBC = regraICMS.PercentualReducaoBC;
                        pedidoXmlNotaFiscal.ObservacaoRegraICMSCTe = regraICMS.ObservacaoCTe;
                        pedidoXmlNotaFiscal.PercentualAliquota = regraICMS.Aliquota;
                        pedidoXmlNotaFiscal.AliquotaPis = regraICMS.AliquotaPis;
                        pedidoXmlNotaFiscal.AliquotaCofins = regraICMS.AliquotaCofins;
                        pedidoXmlNotaFiscal.PercentualAliquotaInternaDifal = regraICMS.AliquotaInternaDifal;
                        pedidoXmlNotaFiscal.BaseCalculoICMS = regraICMS.ValorBaseCalculoICMS;
                        pedidoXmlNotaFiscal.PossuiCTe = true;
                        pedidoXmlNotaFiscal.PossuiNFS = false;
                        pedidoXmlNotaFiscal.DescontarICMSDoValorAReceber = regraICMS.DescontarICMSDoValorAReceber;
                        pedidoXmlNotaFiscal.NaoReduzirRetencaoICMSDoValorDaPrestacao = regraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao;
                        pedidoXmlNotaFiscal.NaoImprimirImpostosDACTE = regraICMS.NaoImprimirImpostosDACTE;
                        pedidoXmlNotaFiscal.NaoEnviarImpostoICMSNaEmissaoCte = regraICMS.NaoEnviarImpostoICMSNaEmissaoCte;

                        pedidoXmlNotaFiscal.SetarRegraICMS(regraICMS.CodigoRegra);

                        if (pedidoXmlNotaFiscal.CST == "60")
                            pedidoXmlNotaFiscal.ICMSPagoPorST = true;

                        pedidoXmlNotaFiscal.ValorPis = Math.Round(regraICMS.ValorPis, 2, MidpointRounding.AwayFromZero);
                        pedidoXmlNotaFiscal.ValorCofins = Math.Round(regraICMS.ValorCofins, 2, MidpointRounding.AwayFromZero);
                        pedidoXmlNotaFiscal.ValorICMS = Math.Round(regraICMS.ValorICMS, 2, MidpointRounding.AwayFromZero);
                        pedidoXmlNotaFiscal.ValorICMSIncluso = Math.Round(regraICMS.ValorICMSIncluso, 2, MidpointRounding.AwayFromZero);

                        pedidoXmlNotaFiscal.PercentualCreditoPresumido = regraICMS.PercentualCreditoPresumido;
                        pedidoXmlNotaFiscal.ValorCreditoPresumido = Math.Round(regraICMS.ValorCreditoPresumido, 2, MidpointRounding.AwayFromZero);

                        servicoPedidoXMLNotaFiscal.PreencherCamposImpostoIBSCBS(pedidoXmlNotaFiscal, impostoIBSCBS);

                        GerarComponenteICMS(pedidoXmlNotaFiscal, calculoImpostosFilialEmissora, unitOfWork, componenteICMS, componentesICMSXMLNotaFiscalExistente);
                        if (!calculoImpostosFilialEmissora)
                            GerarComponentePisCofins(pedidoXmlNotaFiscal, unitOfWork);

                        cargaPedido.PercentualAliquota = regraICMS.Aliquota;
                        cargaPedido.PercentualAliquotaInternaDifal = regraICMS.AliquotaInternaDifal;
                        cargaPedido.CST = regraICMS.CST;
                        cargaPedido.PercentualReducaoBC = regraICMS.PercentualReducaoBC;
                        cargaPedido.ObservacaoRegraICMSCTe = regraICMS.ObservacaoCTe;
                        cargaPedido.AliquotaCofins = regraICMS.AliquotaCofins;
                        cargaPedido.AliquotaPis = regraICMS.AliquotaPis;
                        cargaPedido.BaseCalculoICMS = regraICMS.ValorBaseCalculoICMS;
                        cargaPedido.DescontarICMSDoValorAReceber = regraICMS.DescontarICMSDoValorAReceber;
                        cargaPedido.NaoReduzirRetencaoICMSDoValorDaPrestacao = regraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao;
                        cargaPedido.NaoEnviarImpostoICMSNaEmissaoCte = regraICMS.NaoEnviarImpostoICMSNaEmissaoCte;
                        cargaPedido.NaoImprimirImpostosDACTE = regraICMS.NaoImprimirImpostosDACTE;
                        cargaPedido.SetarRegraICMS(regraICMS.CodigoRegra);

                        serCargaPedido.PreencherCamposImpostoIBSCBS(cargaPedido, impostoIBSCBS, true);

                        Servicos.Log.GravarInfo($"7 - CargaPedido: {cargaPedido.Codigo} -> Aliquota: {cargaPedido.PercentualAliquota}", "ProcessarAliquota");

                        cargaPedido.PossuiCTe = true;
                    }
                    else
                    {
                        if (cfops != null)
                            pedidoXmlNotaFiscal.CFOPFilialEmissora = cfops.Where(o => o.CodigoCFOP == regraICMS.CFOP).FirstOrDefault();
                        else
                            pedidoXmlNotaFiscal.CFOPFilialEmissora = repCFOP.BuscarPorNumero(regraICMS.CFOP);

                        pedidoXmlNotaFiscal.CSTFilialEmissora = regraICMS.CST;
                        pedidoXmlNotaFiscal.IncluirICMSBaseCalculoFilialEmissora = incluirICMSBase;
                        pedidoXmlNotaFiscal.PercentualIncluirBaseCalculoFilialEmissora = percentualIncluiFrete;
                        pedidoXmlNotaFiscal.PercentualReducaoBCFilialEmissora = regraICMS.PercentualReducaoBC;
                        pedidoXmlNotaFiscal.ObservacaoRegraICMSCTeFilialEmissora = regraICMS.ObservacaoCTe;
                        pedidoXmlNotaFiscal.PercentualAliquotaFilialEmissora = regraICMS.Aliquota;
                        pedidoXmlNotaFiscal.PercentualAliquotaFilialEmissoraInternaDifal = 0;
                        pedidoXmlNotaFiscal.BaseCalculoICMSFilialEmissora = regraICMS.ValorBaseCalculoICMS;
                        pedidoXmlNotaFiscal.PossuiCTe = true;
                        pedidoXmlNotaFiscal.PossuiNFS = false;
                        pedidoXmlNotaFiscal.DescontarICMSDoValorAReceber = regraICMS.DescontarICMSDoValorAReceber;
                        pedidoXmlNotaFiscal.NaoEnviarImpostoICMSNaEmissaoCte = regraICMS.NaoEnviarImpostoICMSNaEmissaoCte;
                        pedidoXmlNotaFiscal.NaoReduzirRetencaoICMSDoValorDaPrestacao = regraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao;
                        pedidoXmlNotaFiscal.NaoImprimirImpostosDACTE = regraICMS.NaoImprimirImpostosDACTE;

                        if (pedidoXmlNotaFiscal.CSTFilialEmissora == "60")
                            pedidoXmlNotaFiscal.ICMSPagoPorSTFilialEmissora = true;

                        pedidoXmlNotaFiscal.ValorICMSFilialEmissora = Math.Round(regraICMS.ValorICMS, 2, MidpointRounding.AwayFromZero);

                        pedidoXmlNotaFiscal.PercentualCreditoPresumidoFilialEmissora = regraICMS.PercentualCreditoPresumido;
                        pedidoXmlNotaFiscal.ValorCreditoPresumidoFilialEmissora = Math.Round(regraICMS.ValorCreditoPresumido, 2, MidpointRounding.AwayFromZero);

                        servicoPedidoXMLNotaFiscal.PreencherCamposImpostoIBSCBSFilialEmissora(pedidoXmlNotaFiscal, impostoIBSCBS);

                        GerarComponenteICMS(pedidoXmlNotaFiscal, calculoImpostosFilialEmissora, unitOfWork);
                        if (!calculoImpostosFilialEmissora)
                            GerarComponentePisCofins(pedidoXmlNotaFiscal, unitOfWork);

                        cargaPedido.PercentualAliquotaFilialEmissora = regraICMS.Aliquota;
                        cargaPedido.PercentualAliquotaFilialEmissoraInternaDifal = 0;
                        cargaPedido.CSTFilialEmissora = regraICMS.CST;
                        cargaPedido.PercentualReducaoBCFilialEmissora = regraICMS.PercentualReducaoBC;
                        cargaPedido.ObservacaoRegraICMSCTeFilialEmissora = regraICMS.ObservacaoCTe;
                        cargaPedido.PercentualAliquotaFilialEmissora = regraICMS.Aliquota;
                        cargaPedido.BaseCalculoICMSFilialEmissora = regraICMS.ValorBaseCalculoICMS;
                        cargaPedido.PossuiCTe = true;
                        cargaPedido.DescontarICMSDoValorAReceber = regraICMS.DescontarICMSDoValorAReceber;
                        cargaPedido.NaoReduzirRetencaoICMSDoValorDaPrestacao = regraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao;
                        cargaPedido.NaoImprimirImpostosDACTE = regraICMS.NaoImprimirImpostosDACTE;
                        cargaPedido.NaoEnviarImpostoICMSNaEmissaoCte = regraICMS.NaoEnviarImpostoICMSNaEmissaoCte;

                        serCargaPedido.PreencherCamposImpostoIBSCBSFilialEmissora(cargaPedido, impostoIBSCBS, true);
                    }
                }
                else
                {
                    if (!calculoImpostosFilialEmissora)
                    {
                        pedidoXmlNotaFiscal.IncluirICMSBaseCalculo = false;
                        pedidoXmlNotaFiscal.PercentualIncluirBaseCalculo = 0m;
                        pedidoXmlNotaFiscal.ObservacaoRegraICMSCTe = "";
                        pedidoXmlNotaFiscal.BaseCalculoICMS = 0m;
                        pedidoXmlNotaFiscal.PercentualReducaoBC = 0m;
                        pedidoXmlNotaFiscal.PercentualAliquota = 0m;
                        pedidoXmlNotaFiscal.AliquotaPis = 0m;
                        pedidoXmlNotaFiscal.AliquotaCofins = 0m;
                        pedidoXmlNotaFiscal.ValorICMS = 0m;
                        pedidoXmlNotaFiscal.ValorICMSIncluso = 0m;
                        pedidoXmlNotaFiscal.PercentualCreditoPresumido = 0m;
                        pedidoXmlNotaFiscal.ValorCreditoPresumido = 0m;
                        pedidoXmlNotaFiscal.PossuiCTe = false;
                        pedidoXmlNotaFiscal.DescontarICMSDoValorAReceber = false;
                        pedidoXmlNotaFiscal.NaoReduzirRetencaoICMSDoValorDaPrestacao = false;
                        pedidoXmlNotaFiscal.NaoImprimirImpostosDACTE = false;
                        pedidoXmlNotaFiscal.NaoEnviarImpostoICMSNaEmissaoCte = false;
                        pedidoXmlNotaFiscal.SetarRegraICMS(0);

                        servicoPedidoXMLNotaFiscal.ZerarCamposImpostoIBSCBS(pedidoXmlNotaFiscal);

                        if (pedidoXmlNotaFiscal.ModeloDocumentoFiscal != null && pedidoXmlNotaFiscal.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                            pedidoXmlNotaFiscal.ModeloDocumentoFiscal = cargaPedido.ModeloDocumentoFiscal;
                    }
                    else
                    {
                        pedidoXmlNotaFiscal.IncluirICMSBaseCalculoFilialEmissora = false;
                        pedidoXmlNotaFiscal.PercentualIncluirBaseCalculoFilialEmissora = 0m;
                        pedidoXmlNotaFiscal.ObservacaoRegraICMSCTeFilialEmissora = "";
                        pedidoXmlNotaFiscal.BaseCalculoICMSFilialEmissora = 0m;
                        pedidoXmlNotaFiscal.PercentualReducaoBCFilialEmissora = 0m;
                        pedidoXmlNotaFiscal.PercentualAliquotaFilialEmissora = 0m;
                        pedidoXmlNotaFiscal.ValorICMSFilialEmissora = 0m;
                        pedidoXmlNotaFiscal.PercentualCreditoPresumidoFilialEmissora = 0m;
                        pedidoXmlNotaFiscal.ValorCreditoPresumidoFilialEmissora = 0m;
                        pedidoXmlNotaFiscal.DescontarICMSDoValorAReceber = false;
                        pedidoXmlNotaFiscal.NaoReduzirRetencaoICMSDoValorDaPrestacao = false;
                        pedidoXmlNotaFiscal.NaoEnviarImpostoICMSNaEmissaoCte = false;
                        pedidoXmlNotaFiscal.NaoImprimirImpostosDACTE = false;

                        servicoPedidoXMLNotaFiscal.ZerarCamposImpostoIBSCBSFilialEmissora(pedidoXmlNotaFiscal);
                    }
                }

                if (!calculoImpostosFilialEmissora)
                {
                    bool setarImportosISS = false;

                    if (possuiNFS)
                    {
                        setarImportosISS = true;

                        if (pedidoXmlNotaFiscal.ModeloDocumentoFiscal == null || pedidoXmlNotaFiscal.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                            pedidoXmlNotaFiscal.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.NFSe);

                        pedidoXmlNotaFiscal.PossuiCTe = false;
                        pedidoXmlNotaFiscal.PossuiNFS = true;
                        cargaPedido.PossuiNFS = true;
                    }
                    else
                    {
                        pedidoXmlNotaFiscal.PossuiNFS = false;

                        if (pedidoXmlNotaFiscal.ModeloDocumentoFiscal != null && pedidoXmlNotaFiscal.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                            pedidoXmlNotaFiscal.ModeloDocumentoFiscal = cargaPedido.ModeloDocumentoFiscal;
                    }

                    if (possuiNFSManual)
                    {
                        setarImportosISS = true;
                        pedidoXmlNotaFiscal.ModeloDocumentoFiscal = null;
                        pedidoXmlNotaFiscal.PossuiNFSManual = true;
                        cargaPedido.PossuiNFSManual = true;
                        cargaPedido.ModeloDocumentoFiscalIntramunicipal = modeloDocumentoFiscalIntramunicipal;
                    }
                    else
                    {
                        pedidoXmlNotaFiscal.PossuiNFSManual = false;
                        //cargaPedido.PossuiNFSManual = false;
                        //cargaPedido.ModeloDocumentoFiscalIntramunicipal = null;
                    }

                    if (setarImportosISS)
                    {
                        Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = serCargaISS.BuscarRegraISS(carga.Empresa, valorBaseCalculo, destino, carga.TipoOperacao, tomador, null, carga?.TipoDeCarga?.NBS ?? "", unitOfWork);
                        if (regraISS != null)
                        {
                            pedidoXmlNotaFiscal.ValorISS = regraISS.ValorISS;
                            pedidoXmlNotaFiscal.BaseCalculoISS = regraISS.ValorBaseCalculoISS;
                            pedidoXmlNotaFiscal.PercentualAliquotaISS = regraISS.AliquotaISS;
                            pedidoXmlNotaFiscal.PercentualRetencaoISS = regraISS.PercentualRetencaoISS;
                            pedidoXmlNotaFiscal.IncluirISSBaseCalculo = regraISS.IncluirISSBaseCalculo;
                            pedidoXmlNotaFiscal.ValorRetencaoISS = regraISS.ValorRetencaoISS;

                            pedidoXmlNotaFiscal.ReterIR = regraISS.ReterIR;
                            pedidoXmlNotaFiscal.AliquotaIR = regraISS.AliquotaIR;
                            pedidoXmlNotaFiscal.BaseCalculoIR = regraISS.BaseCalculoIR;
                            pedidoXmlNotaFiscal.ValorIR = regraISS.ValorIR;

                            if (regraISS.IncluirISSBaseCalculo)
                                baseCalculoIBSCBS += regraISS.ValorISS;

                            Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = servicoImpostoIBSCBS.ObterImpostoIBSCBSComValoresArredondados(new Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBS
                            {
                                BaseCalculo = baseCalculoIBSCBS,
                                ValoAbaterBaseCalculo = regraISS.ValorISS,
                                CodigoLocalidade = destino.Codigo,
                                SiglaUF = destino.Estado.Sigla,
                                CodigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0,
                                Empresa = empresa
                            });

                            servicoPedidoXMLNotaFiscal.PreencherCamposImpostoIBSCBS(pedidoXmlNotaFiscal, impostoIBSCBS);

                            GerarComponenteISS(pedidoXmlNotaFiscal, unitOfWork);

                            cargaPedido.PercentualAliquotaISS = regraISS.AliquotaISS;
                            cargaPedido.PercentualRetencaoISS = regraISS.PercentualRetencaoISS;
                            cargaPedido.IncluirISSBaseCalculo = regraISS.IncluirISSBaseCalculo;

                            cargaPedido.AliquotaIR = regraISS.AliquotaIR;
                            cargaPedido.BaseCalculoIR = regraISS.BaseCalculoIR;
                            cargaPedido.ReterIR = regraISS.ReterIR;

                            serCargaPedido.PreencherCamposImpostoIBSCBS(cargaPedido, impostoIBSCBS, true);

                        }
                    }
                    else
                    {
                        pedidoXmlNotaFiscal.ValorISS = 0;
                        pedidoXmlNotaFiscal.BaseCalculoISS = 0;
                        pedidoXmlNotaFiscal.PercentualAliquotaISS = 0;
                        pedidoXmlNotaFiscal.PercentualRetencaoISS = 0;
                        pedidoXmlNotaFiscal.IncluirISSBaseCalculo = false;
                        pedidoXmlNotaFiscal.ValorRetencaoISS = 0;

                        pedidoXmlNotaFiscal.ReterIR = false;
                        pedidoXmlNotaFiscal.AliquotaIR = 0;
                        pedidoXmlNotaFiscal.BaseCalculoIR = 0;
                        pedidoXmlNotaFiscal.ValorIR = 0;
                    }
                }

                Servicos.Log.GravarInfo($"Atualizando CalcularImpostos cargaPedido = {cargaPedido?.Codigo ?? 0}, pedidoXmlNotaFiscal = {pedidoXmlNotaFiscal.Codigo} com valorISS = {pedidoXmlNotaFiscal.ValorISS} e incluirISSBaseCalculo = {pedidoXmlNotaFiscal.IncluirISSBaseCalculo}, valorRetencaoISS = {pedidoXmlNotaFiscal.ValorRetencaoISS}", "AtualizarPedidoXMLNotaFiscal");
                Servicos.Embarcador.Escrituracao.DocumentoProvisao.AdicionarDocumentoParaProvisao(pedidoXmlNotaFiscal, calculoImpostosFilialEmissora, tipoServicoMultisoftware, unitOfWork);
            }
            else
            {
                throw new Exception("Tomador não encontrado na base da multisoftware");
            }
        }

        public void CalcularImpostos(Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaRateio, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool calculoImpostosFilialEmissora, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXmlsNotasFiscais, decimal valorBaseCalculo, bool incluirICMSBase, decimal percentualIncluiFrete, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Cliente recebedor, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProduto, Dominio.Entidades.Cliente expedidor = null, decimal pesoTotalParaCalculoFatorCubagem = 0)
        {
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);

            Servicos.Embarcador.Carga.ICMS serCargaICMS = new Servicos.Embarcador.Carga.ICMS(unitOfWork);
            Servicos.Embarcador.Carga.ISS serCargaISS = new Servicos.Embarcador.Carga.ISS(unitOfWork);
            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
            Servicos.Embarcador.Pedido.PedidoXMLNotaFiscal servicoPedidoXMLNotaFiscal = new Servicos.Embarcador.Pedido.PedidoXMLNotaFiscal(unitOfWork);
            Servicos.Embarcador.Imposto.ImpostoIBSCBS servicoImpostoIBSCBS = new Servicos.Embarcador.Imposto.ImpostoIBSCBS(unitOfWork);

            //if (formulaRateio != null)
            //    formulaRateio = repRateioFormula.BuscarPorCodigo(formulaRateio.Codigo);

            decimal baseCalculoIBSCBS = valorBaseCalculo;

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> pedidoXMLNotaFiscalCompontesFreteAgrupada = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete>();
            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidosXmlsNotasFiscais)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> pedidoXMLNotaFiscalCompontesFrete = repPedidoXMLNotaFiscalComponenteFrete.BuscarPorXMLnotaFiscal(pedidoXMLNotaFiscal.Codigo, null, calculoImpostosFilialEmissora);

                ObterBaseCalculo(ref valorBaseCalculo, calculoImpostosFilialEmissora, carga, cargaPedido, pedidoXMLNotaFiscal, unitOfWork, tipoServicoMultisoftware, pedidoXMLNotaFiscalCompontesFrete);
                baseCalculoIBSCBS += Math.Round(pedidoXMLNotaFiscalCompontesFrete.Sum(obj => obj.ValorComponente), 2, MidpointRounding.AwayFromZero);

                pedidoXMLNotaFiscalCompontesFreteAgrupada.AddRange(pedidoXMLNotaFiscalCompontesFrete);
            }

            Servicos.Log.GravarInfo($"2 Notas para gerar Documento Provisão {string.Join(", ", pedidosXmlsNotasFiscais?.Select(x => x.Codigo)?.ToList())}", "GeracaoDocumentosProvisao");
            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal primeiraNota = pedidosXmlsNotasFiscais[0];

            Dominio.Entidades.Cliente remetente = primeiraNota.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida ? primeiraNota.XMLNotaFiscal.Emitente : primeiraNota.XMLNotaFiscal.Destinatario;
            Dominio.Entidades.Cliente destinatario = primeiraNota.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida ? primeiraNota.XMLNotaFiscal.Destinatario : primeiraNota.XMLNotaFiscal.Emitente;


            Dominio.Entidades.Cliente tomador = null;
            if (cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupada || cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalIndividual || cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoAgrupado)
            {
                if (cargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente || cargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
                {
                    if (cargaPedido.Pedido.UsarTipoPagamentoNF)
                    {
                        if (primeiraNota.XMLNotaFiscal.ModalidadeFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago)
                            tomador = remetente;
                        else if (primeiraNota.XMLNotaFiscal.ModalidadeFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar)
                            tomador = destinatario;
                        else
                            tomador = cargaPedido.ObterTomador();
                    }
                    else
                        tomador = cargaPedido.ObterTomador();
                }
                else
                    tomador = cargaPedido.ObterTomador();
            }
            else
                tomador = cargaPedido.ObterTomador();

            if (recebedor != null)
                destinatario = recebedor;

            if (expedidor != null)
                remetente = expedidor;


            if (tomador != null)
            {
                bool possuiCTe = false;
                bool possuiNFS = false;
                bool possuiNFSManual = false;
                Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalIntramunicipal = null;

                Dominio.Entidades.Localidade origem = remetente.Localidade;
                Dominio.Entidades.Localidade destino = destinatario.Localidade;

                if (configuracao.UtilizarLocalidadePrestacaoPedido)
                {
                    if (cargaPedido.Pedido.LocalidadeInicioPrestacao != null)
                        origem = cargaPedido.Pedido.LocalidadeInicioPrestacao;

                    if (cargaPedido.Pedido.LocalidadeTerminoPrestacao != null)
                        destino = cargaPedido.Pedido.LocalidadeTerminoPrestacao;
                }

                serCargaPedido.VerificarQuaisDocumentosDeveEmitir(carga, cargaPedido, origem, destino, tipoServicoMultisoftware, unitOfWork, out possuiCTe, out possuiNFS, out possuiNFSManual, out modeloDocumentoFiscalIntramunicipal, configuracao, out bool sempreDisponibilizarDocumentoNFSManual);

                cargaPedido.DisponibilizarDocumentoNFSManual = sempreDisponibilizarDocumentoNFSManual;

                Servicos.Embarcador.Carga.RateioFormula svcRateio = new RateioFormula(unitOfWork);

                decimal valorTotal = pedidosXmlsNotasFiscais.Sum(o => o.XMLNotaFiscal.Valor);
                decimal pesoTotal = pedidosXmlsNotasFiscais.Sum(o => o.XMLNotaFiscal.Peso);
                decimal pesoLiquidoTotal = pedidosXmlsNotasFiscais.Sum(o => o.XMLNotaFiscal.PesoLiquido);
                int volumeTotal = pedidosXmlsNotasFiscais.Sum(o => o.XMLNotaFiscal.Volumes);
                decimal metrosCubicosTotais = pedidosXmlsNotasFiscais.Sum(o => o.XMLNotaFiscal.MetrosCubicos);
                decimal valorRateioOriginal = 0;

                int totalNotas = pedidosXmlsNotasFiscais.Count;

                decimal totalBaseCalculoICMSRateada = 0m,
                        totalCreditoPresumidoRateado = 0m,
                        totalBaseCalculoISSRateada = 0m,
                        totalICMSRateado = 0m,
                        totalPisRateado = 0m,
                        totalCofinsRateado = 0m,
                        totalICMSInclusoRateado = 0m,
                        totalISSRateado = 0m,
                        totalRetencaoISSRateado = 0m,
                        totalIRRateado = 0m,
                        totalAliquotaIRRateado = 0m,
                        totalBaseCalculoIRRateada = 0m,
                        totalBaseCalculoIBSCBSRateado = 0m,
                        totalValorIBSEstadualRateado = 0m,
                        totalValorIBSMunicipalRateado = 0m,
                        totalValorCBSRateado = 0m;

                Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = null;
                Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = null;
                Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = null;

                Dominio.Entidades.Empresa empresa = carga.Empresa;
                if (calculoImpostosFilialEmissora)
                    empresa = carga.EmpresaFilialEmissora;

                if ((possuiNFS || possuiNFSManual) && !calculoImpostosFilialEmissora)
                {
                    regraISS = serCargaISS.BuscarRegraISS(carga.Empresa, valorBaseCalculo, destino, carga.TipoOperacao, tomador, null, carga?.TipoDeCarga?.NBS ?? "", unitOfWork);
                    if (regraISS != null)
                    {
                        regraISS.ValorISS = Math.Round(regraISS.ValorISS, 2, MidpointRounding.AwayFromZero);
                        regraISS.ValorRetencaoISS = Math.Round(regraISS.ValorRetencaoISS, 2, MidpointRounding.AwayFromZero);
                        regraISS.ValorIR = Math.Round(regraISS.ValorIR, 2, MidpointRounding.AwayFromZero);

                        if (regraISS.IncluirISSBaseCalculo)
                            baseCalculoIBSCBS += regraISS.ValorISS;

                        impostoIBSCBS = servicoImpostoIBSCBS.ObterImpostoIBSCBSComValoresArredondados(new Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBS
                        {
                            BaseCalculo = baseCalculoIBSCBS,
                            ValoAbaterBaseCalculo = regraISS.ValorISS,
                            CodigoLocalidade = destino.Codigo,
                            SiglaUF = destino.Estado.Sigla,
                            CodigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0,
                            Empresa = empresa
                        });
                    }
                }

                if (possuiCTe)
                {
                    if (cargaPedido.ModeloDocumentoFiscal?.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                        cargaPedido.ModeloDocumentoFiscal = null;

                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga tipoContratacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal;

                    if (primeiraNota.TipoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao)
                        tipoContratacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada;

                    regraICMS = serCargaICMS.BuscarRegraICMS(carga, cargaPedido, empresa, remetente, destinatario, tomador, origem, destino, ref incluirICMSBase, ref percentualIncluiFrete, valorBaseCalculo, null, unitOfWork, tipoServicoMultisoftware, configuracao, tipoContratacao);
                    regraICMS.ValorPis = Math.Round(regraICMS.ValorPis, 2, MidpointRounding.AwayFromZero);
                    regraICMS.ValorCofins = Math.Round(regraICMS.ValorCofins, 2, MidpointRounding.AwayFromZero);
                    regraICMS.ValorICMS = Math.Round(regraICMS.ValorICMS, 2, MidpointRounding.AwayFromZero);
                    regraICMS.ValorICMSIncluso = Math.Round(regraICMS.ValorICMSIncluso, 2, MidpointRounding.AwayFromZero);
                    regraICMS.ValorCreditoPresumido = Math.Round(regraICMS.ValorCreditoPresumido, 2, MidpointRounding.AwayFromZero);

                    bool incluirICMS = !calculoImpostosFilialEmissora ? cargaPedido.IncluirICMSBaseCalculo : cargaPedido.IncluirICMSBaseCalculoFilialEmissora;

                    if (!incluirICMS)
                        baseCalculoIBSCBS -= Math.Round(regraICMS.ValorICMS, 2, MidpointRounding.AwayFromZero);

                    impostoIBSCBS = servicoImpostoIBSCBS.ObterImpostoIBSCBSComValoresArredondados(new Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBS { BaseCalculo = baseCalculoIBSCBS, CodigoLocalidade = destino.Codigo, SiglaUF = destino.Estado.Sigla, CodigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0, Empresa = empresa });
                }

                for (var i = 0; i < totalNotas; i++)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = pedidosXmlsNotasFiscais[i];
                    decimal densidadeProdutos = (cargaPedidoProduto.Sum(obj => obj.Produto?.MetroCubito) ?? 0m); //pedidoXMLNotaFiscal.CargaPedido?.Produtos?.Sum(o => o.Produto?.MetroCubito) ?? 0m;
                    decimal pesoParaCalculoFatorCubagem = 0;

                    if (formulaRateio?.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.PorPesoUtilizandoFatorCubagem)
                        pesoParaCalculoFatorCubagem = svcRateio.ObterPesoCubadoFatorCubagem(formulaRateio.ParametroRateioFormula, cargaPedido.TipoUsoFatorCubagemRateioFormula, cargaPedido.FatorCubagemRateioFormula ?? 0, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos);

                    if (possuiCTe)
                    {
                        decimal valorBaseCalculoICMS = regraICMS.ValorBaseCalculoICMS;
                        decimal valorICMS = regraICMS.ValorICMS;
                        decimal valorPis = regraICMS.ValorPis;
                        decimal valorCofins = regraICMS.ValorCofins;
                        decimal valorICMSIncluso = regraICMS.ValorICMSIncluso;
                        decimal valorCreditoPresumido = regraICMS.ValorCreditoPresumido;

                        decimal valorbaseCalculoIBSCBS = impostoIBSCBS.BaseCalculo;
                        decimal valorIBSEstadual = impostoIBSCBS.ValorIBSEstadual;
                        decimal valorIBSMunicipal = impostoIBSCBS.ValorIBSMunicipal;
                        decimal valorCBS = impostoIBSCBS.ValorCBS;

                        if (formulaRateio != null && formulaRateio.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.porCTe)
                        {
                            valorBaseCalculoICMS = regraICMS.ValorBaseCalculoICMS / totalNotas;
                            valorICMS = regraICMS.ValorICMS / totalNotas;
                            valorICMSIncluso = regraICMS.ValorICMSIncluso / totalNotas;
                            valorCreditoPresumido = regraICMS.ValorCreditoPresumido / totalNotas;
                            valorPis = regraICMS.ValorPis / totalNotas;
                            valorCofins = regraICMS.ValorCofins / totalNotas;

                            valorbaseCalculoIBSCBS = impostoIBSCBS.BaseCalculo / totalNotas;
                            valorIBSEstadual = impostoIBSCBS.ValorIBSEstadual / totalNotas;
                            valorIBSMunicipal = impostoIBSCBS.ValorIBSMunicipal / totalNotas;
                            valorCBS = impostoIBSCBS.ValorCBS / totalNotas;
                        }

                        decimal baseCalculoRateada = svcRateio.AplicarFormulaRateio(formulaRateio, valorBaseCalculoICMS, totalNotas, 1, pesoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotal, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                        decimal icmsRateado = svcRateio.AplicarFormulaRateio(formulaRateio, valorICMS, totalNotas, 1, pesoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotal, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                        decimal icmsInclusoRateado = svcRateio.AplicarFormulaRateio(formulaRateio, valorICMSIncluso, totalNotas, 1, pesoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotal, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                        decimal valorCreditoPresumidoRateado = svcRateio.AplicarFormulaRateio(formulaRateio, valorCreditoPresumido, totalNotas, 1, pesoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotal, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                        decimal pisRateado = svcRateio.AplicarFormulaRateio(formulaRateio, valorPis, totalNotas, 1, pesoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotal, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                        decimal cofinsRateado = svcRateio.AplicarFormulaRateio(formulaRateio, valorCofins, totalNotas, 1, pesoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotal, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);

                        decimal valorbaseCalculoIBSCBSRateado = svcRateio.AplicarFormulaRateio(formulaRateio, valorbaseCalculoIBSCBS, totalNotas, 1, pesoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotal, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                        decimal valorIBSEstadualRateado = svcRateio.AplicarFormulaRateio(formulaRateio, valorIBSEstadual, totalNotas, 1, pesoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotal, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                        decimal valorIBSMunicipalRateado = svcRateio.AplicarFormulaRateio(formulaRateio, valorIBSMunicipal, totalNotas, 1, pesoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotal, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                        decimal valorCBSRateado = svcRateio.AplicarFormulaRateio(formulaRateio, valorCBS, totalNotas, 1, pesoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotal, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);

                        if (i == totalNotas - 1)
                        {
                            pisRateado = regraICMS.ValorPis - totalPisRateado;
                            cofinsRateado = regraICMS.ValorCofins - totalCofinsRateado;
                            icmsRateado = regraICMS.ValorICMS - totalICMSRateado;
                            icmsInclusoRateado = regraICMS.ValorICMSIncluso - totalICMSInclusoRateado;
                            baseCalculoRateada = regraICMS.ValorBaseCalculoICMS - totalBaseCalculoICMSRateada;
                            valorCreditoPresumidoRateado = regraICMS.ValorCreditoPresumido - totalCreditoPresumidoRateado;

                            if (!configuracao.DesconsiderarSobraRateioParaBaseCalculoIBSCBS)
                            {
                                valorbaseCalculoIBSCBSRateado = impostoIBSCBS.BaseCalculo - totalBaseCalculoIBSCBSRateado;
                                valorIBSEstadualRateado = impostoIBSCBS.ValorIBSEstadual - totalValorIBSEstadualRateado;
                                valorIBSMunicipalRateado = impostoIBSCBS.ValorIBSMunicipal - totalValorIBSMunicipalRateado;
                                valorCBSRateado = impostoIBSCBS.ValorCBS - totalValorCBSRateado;
                            }
                        }

                        totalPisRateado += pisRateado;
                        totalCofinsRateado += cofinsRateado;
                        totalICMSRateado += icmsRateado;
                        totalICMSInclusoRateado += icmsInclusoRateado;
                        totalBaseCalculoICMSRateada += baseCalculoRateada;
                        totalCreditoPresumidoRateado += valorCreditoPresumidoRateado;

                        totalBaseCalculoIBSCBSRateado += valorbaseCalculoIBSCBSRateado;
                        totalValorIBSEstadualRateado += valorIBSEstadualRateado;
                        totalValorIBSMunicipalRateado += valorIBSMunicipalRateado;
                        totalValorCBSRateado += valorCBSRateado;

                        if (!calculoImpostosFilialEmissora)
                        {
                            pedidoXMLNotaFiscal.ValorTotalComponentes = pedidoXMLNotaFiscalCompontesFreteAgrupada.Where(obj => obj.PedidoXMLNotaFiscal.Codigo == pedidoXMLNotaFiscal.Codigo).Sum(obj => obj.ValorComponente);
                            pedidoXMLNotaFiscal.ValorTotalMoedaComponentes = pedidoXMLNotaFiscalCompontesFreteAgrupada.Where(obj => obj.PedidoXMLNotaFiscal.Codigo == pedidoXMLNotaFiscal.Codigo).Sum(obj => obj.ValorTotalMoeda ?? 0m);
                            pedidoXMLNotaFiscal.CFOP = repCFOP.BuscarPorNumero(regraICMS.CFOP);
                            pedidoXMLNotaFiscal.CST = regraICMS.CST;
                            pedidoXMLNotaFiscal.IncluirICMSBaseCalculo = incluirICMSBase;
                            pedidoXMLNotaFiscal.PercentualIncluirBaseCalculo = percentualIncluiFrete;
                            pedidoXMLNotaFiscal.PercentualReducaoBC = regraICMS.PercentualReducaoBC;
                            pedidoXMLNotaFiscal.ObservacaoRegraICMSCTe = regraICMS.ObservacaoCTe;
                            pedidoXMLNotaFiscal.PercentualAliquota = regraICMS.Aliquota;
                            pedidoXMLNotaFiscal.AliquotaPis = regraICMS.AliquotaPis;
                            pedidoXMLNotaFiscal.AliquotaCofins = regraICMS.AliquotaCofins;
                            pedidoXMLNotaFiscal.PercentualAliquotaInternaDifal = regraICMS.AliquotaInternaDifal;
                            pedidoXMLNotaFiscal.BaseCalculoICMS = baseCalculoRateada;
                            pedidoXMLNotaFiscal.PercentualCreditoPresumido = regraICMS.PercentualCreditoPresumido;
                            pedidoXMLNotaFiscal.ValorCreditoPresumido = valorCreditoPresumidoRateado;
                            pedidoXMLNotaFiscal.PossuiCTe = true;
                            pedidoXMLNotaFiscal.PossuiNFS = false;
                            pedidoXMLNotaFiscal.DescontarICMSDoValorAReceber = regraICMS.DescontarICMSDoValorAReceber;
                            pedidoXMLNotaFiscal.NaoReduzirRetencaoICMSDoValorDaPrestacao = regraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao;
                            pedidoXMLNotaFiscal.NaoImprimirImpostosDACTE = regraICMS.NaoImprimirImpostosDACTE;
                            pedidoXMLNotaFiscal.NaoEnviarImpostoICMSNaEmissaoCte = regraICMS.NaoEnviarImpostoICMSNaEmissaoCte;
                            pedidoXMLNotaFiscal.SetarRegraICMS(regraICMS.CodigoRegra);

                            if (pedidoXMLNotaFiscal.ModeloDocumentoFiscal == null || pedidoXMLNotaFiscal.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                                pedidoXMLNotaFiscal.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.CTe);

                            if (pedidoXMLNotaFiscal.CST == "60")
                                pedidoXMLNotaFiscal.ICMSPagoPorST = true;

                            pedidoXMLNotaFiscal.ValorPis = pisRateado;
                            pedidoXMLNotaFiscal.ValorCofins = cofinsRateado;
                            pedidoXMLNotaFiscal.ValorICMS = icmsRateado;
                            pedidoXMLNotaFiscal.ValorICMSIncluso = icmsInclusoRateado;

                            servicoPedidoXMLNotaFiscal.PreencherCamposImpostoIBSCBS(pedidoXMLNotaFiscal, impostoIBSCBS);
                            servicoPedidoXMLNotaFiscal.PreencherValoresImpostoIBSCBSRateado(pedidoXMLNotaFiscal, valorbaseCalculoIBSCBSRateado, valorIBSEstadualRateado, valorIBSMunicipalRateado, valorCBSRateado);
                        }
                        else
                        {
                            pedidoXMLNotaFiscal.CFOPFilialEmissora = repCFOP.BuscarPorNumero(regraICMS.CFOP);
                            pedidoXMLNotaFiscal.CSTFilialEmissora = regraICMS.CST;
                            pedidoXMLNotaFiscal.IncluirICMSBaseCalculoFilialEmissora = incluirICMSBase;
                            pedidoXMLNotaFiscal.PercentualIncluirBaseCalculoFilialEmissora = percentualIncluiFrete;
                            pedidoXMLNotaFiscal.PercentualReducaoBCFilialEmissora = regraICMS.PercentualReducaoBC;
                            pedidoXMLNotaFiscal.ObservacaoRegraICMSCTeFilialEmissora = regraICMS.ObservacaoCTe;
                            pedidoXMLNotaFiscal.PercentualAliquotaFilialEmissora = regraICMS.Aliquota;
                            pedidoXMLNotaFiscal.PercentualAliquotaFilialEmissoraInternaDifal = 0;
                            pedidoXMLNotaFiscal.BaseCalculoICMSFilialEmissora = baseCalculoRateada;
                            pedidoXMLNotaFiscal.PercentualCreditoPresumidoFilialEmissora = regraICMS.PercentualCreditoPresumido;
                            pedidoXMLNotaFiscal.ValorCreditoPresumidoFilialEmissora = valorCreditoPresumidoRateado;
                            pedidoXMLNotaFiscal.DescontarICMSDoValorAReceber = regraICMS.DescontarICMSDoValorAReceber;
                            pedidoXMLNotaFiscal.NaoReduzirRetencaoICMSDoValorDaPrestacao = regraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao;
                            pedidoXMLNotaFiscal.NaoImprimirImpostosDACTE = regraICMS.NaoImprimirImpostosDACTE;
                            pedidoXMLNotaFiscal.NaoEnviarImpostoICMSNaEmissaoCte = regraICMS.NaoEnviarImpostoICMSNaEmissaoCte;

                            if (pedidoXMLNotaFiscal.CSTFilialEmissora == "60")
                                pedidoXMLNotaFiscal.ICMSPagoPorSTFilialEmissora = true;

                            pedidoXMLNotaFiscal.ValorICMSFilialEmissora = icmsRateado;

                            servicoPedidoXMLNotaFiscal.PreencherCamposImpostoIBSCBSFilialEmissora(pedidoXMLNotaFiscal, impostoIBSCBS);
                            servicoPedidoXMLNotaFiscal.PreencherValoresImpostoIBSCBSFilialEmissoraRateado(pedidoXMLNotaFiscal, valorbaseCalculoIBSCBSRateado, valorIBSEstadualRateado, valorIBSMunicipalRateado, valorCBSRateado);
                        }

                        GerarComponenteICMS(pedidoXMLNotaFiscal, calculoImpostosFilialEmissora, unitOfWork);
                        if (!calculoImpostosFilialEmissora)
                            GerarComponentePisCofins(pedidoXMLNotaFiscal, unitOfWork);
                    }
                    else
                    {
                        if (!calculoImpostosFilialEmissora)
                        {
                            pedidoXMLNotaFiscal.IncluirICMSBaseCalculo = false;
                            pedidoXMLNotaFiscal.PercentualIncluirBaseCalculo = 0m;
                            pedidoXMLNotaFiscal.ObservacaoRegraICMSCTe = "";
                            pedidoXMLNotaFiscal.BaseCalculoICMS = 0m;
                            pedidoXMLNotaFiscal.PercentualReducaoBC = 0m;
                            pedidoXMLNotaFiscal.PercentualAliquota = 0m;
                            pedidoXMLNotaFiscal.AliquotaCofins = 0m;
                            pedidoXMLNotaFiscal.AliquotaPis = 0m;
                            pedidoXMLNotaFiscal.ValorICMS = 0m;
                            pedidoXMLNotaFiscal.ValorPis = 0m;
                            pedidoXMLNotaFiscal.ValorCofins = 0m;
                            pedidoXMLNotaFiscal.ValorICMSIncluso = 0m;
                            pedidoXMLNotaFiscal.ValorCreditoPresumido = 0m;
                            pedidoXMLNotaFiscal.PercentualCreditoPresumido = 0m;
                            pedidoXMLNotaFiscal.PossuiCTe = false;
                            pedidoXMLNotaFiscal.DescontarICMSDoValorAReceber = false;
                            pedidoXMLNotaFiscal.NaoEnviarImpostoICMSNaEmissaoCte = false;
                            pedidoXMLNotaFiscal.NaoReduzirRetencaoICMSDoValorDaPrestacao = false;
                            pedidoXMLNotaFiscal.NaoImprimirImpostosDACTE = false;
                            pedidoXMLNotaFiscal.SetarRegraICMS(0);

                            servicoPedidoXMLNotaFiscal.ZerarCamposImpostoIBSCBS(pedidoXMLNotaFiscal);

                            if (pedidoXMLNotaFiscal.ModeloDocumentoFiscal != null && pedidoXMLNotaFiscal.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                                pedidoXMLNotaFiscal.ModeloDocumentoFiscal = cargaPedido.ModeloDocumentoFiscal;
                        }
                        else
                        {
                            pedidoXMLNotaFiscal.IncluirICMSBaseCalculoFilialEmissora = false;
                            pedidoXMLNotaFiscal.PercentualIncluirBaseCalculoFilialEmissora = 0m;
                            pedidoXMLNotaFiscal.ObservacaoRegraICMSCTeFilialEmissora = "";
                            pedidoXMLNotaFiscal.BaseCalculoICMSFilialEmissora = 0m;
                            pedidoXMLNotaFiscal.PercentualReducaoBCFilialEmissora = 0m;
                            pedidoXMLNotaFiscal.PercentualAliquotaFilialEmissora = 0m;
                            pedidoXMLNotaFiscal.ValorICMSFilialEmissora = 0m;
                            pedidoXMLNotaFiscal.ValorCreditoPresumidoFilialEmissora = 0m;
                            pedidoXMLNotaFiscal.PercentualCreditoPresumidoFilialEmissora = 0m;
                            pedidoXMLNotaFiscal.DescontarICMSDoValorAReceber = false;
                            pedidoXMLNotaFiscal.NaoReduzirRetencaoICMSDoValorDaPrestacao = false;
                            pedidoXMLNotaFiscal.NaoImprimirImpostosDACTE = false;
                            pedidoXMLNotaFiscal.NaoEnviarImpostoICMSNaEmissaoCte = false;

                            servicoPedidoXMLNotaFiscal.ZerarCamposImpostoIBSCBSFilialEmissora(pedidoXMLNotaFiscal);
                        }
                    }

                    if (!calculoImpostosFilialEmissora)
                    {
                        if (possuiNFS || possuiNFSManual)
                        {
                            if (regraISS != null)
                            {
                                decimal baseCalculoRateada = Math.Round(svcRateio.AplicarFormulaRateio(formulaRateio, regraISS.ValorBaseCalculoISS, totalNotas, 1, pesoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotal, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem), 2, MidpointRounding.ToEven);
                                decimal issRateado = Math.Round(svcRateio.AplicarFormulaRateio(formulaRateio, regraISS.ValorISS, totalNotas, 1, pesoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotal, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem), 2, MidpointRounding.ToEven);
                                decimal retencaoISSRateado = Math.Round(svcRateio.AplicarFormulaRateio(formulaRateio, regraISS.ValorRetencaoISS, totalNotas, 1, pesoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotal, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem), 2, MidpointRounding.ToEven);

                                decimal baseCalculoIRRateada = Math.Round(svcRateio.AplicarFormulaRateio(formulaRateio, regraISS.BaseCalculoIR, totalNotas, 1, pesoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotal, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem), 2, MidpointRounding.ToEven);
                                decimal irRateado = Math.Round(svcRateio.AplicarFormulaRateio(formulaRateio, regraISS.ValorIR, totalNotas, 1, pesoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotal, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem), 2, MidpointRounding.ToEven);
                                decimal aliquotaIRRateado = Math.Round(svcRateio.AplicarFormulaRateio(formulaRateio, regraISS.AliquotaIR, totalNotas, 1, pesoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotal, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem), 2, MidpointRounding.ToEven);

                                decimal valorbaseCalculoIBSCBSRateado = Math.Round(svcRateio.AplicarFormulaRateio(formulaRateio, impostoIBSCBS.BaseCalculo, totalNotas, 1, pesoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotal, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem), 2, MidpointRounding.ToEven);
                                decimal valorIBSEstadualRateado = Math.Round(svcRateio.AplicarFormulaRateio(formulaRateio, impostoIBSCBS.ValorIBSEstadual, totalNotas, 1, pesoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotal, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem), 2, MidpointRounding.ToEven);
                                decimal valorIBSMunicipalRateado = Math.Round(svcRateio.AplicarFormulaRateio(formulaRateio, impostoIBSCBS.ValorIBSMunicipal, totalNotas, 1, pesoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotal, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem), 2, MidpointRounding.ToEven);
                                decimal valorCBSRateado = Math.Round(svcRateio.AplicarFormulaRateio(formulaRateio, impostoIBSCBS.ValorCBS, totalNotas, 1, pesoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotal, 0m, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotais, densidadeProdutos, false, pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotal, pedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumeTotal, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem), 2, MidpointRounding.ToEven);

                                if (i == totalNotas - 1)
                                {
                                    issRateado = regraISS.ValorISS - totalISSRateado;
                                    retencaoISSRateado = regraISS.ValorRetencaoISS - totalRetencaoISSRateado;
                                    baseCalculoRateada = regraISS.ValorBaseCalculoISS - totalBaseCalculoISSRateada;

                                    irRateado = regraISS.ValorIR - totalIRRateado;
                                    aliquotaIRRateado = regraISS.AliquotaIR - totalAliquotaIRRateado;
                                    baseCalculoIRRateada = regraISS.BaseCalculoIR - totalBaseCalculoIRRateada;

                                    valorbaseCalculoIBSCBSRateado = impostoIBSCBS.BaseCalculo - totalBaseCalculoIBSCBSRateado;
                                    valorIBSEstadualRateado = impostoIBSCBS.ValorIBSEstadual - totalValorIBSEstadualRateado;
                                    valorIBSMunicipalRateado = impostoIBSCBS.ValorIBSMunicipal - totalValorIBSMunicipalRateado;
                                    valorCBSRateado = impostoIBSCBS.ValorCBS - totalValorCBSRateado;
                                }

                                totalISSRateado += issRateado;
                                totalRetencaoISSRateado += retencaoISSRateado;
                                totalBaseCalculoISSRateada += baseCalculoRateada;

                                totalIRRateado += irRateado;
                                totalAliquotaIRRateado += aliquotaIRRateado;
                                totalBaseCalculoIRRateada += baseCalculoIRRateada;

                                totalBaseCalculoIBSCBSRateado += valorbaseCalculoIBSCBSRateado;
                                totalValorIBSEstadualRateado += valorIBSEstadualRateado;
                                totalValorIBSMunicipalRateado += valorIBSMunicipalRateado;
                                totalValorCBSRateado += valorCBSRateado;

                                pedidoXMLNotaFiscal.ValorISS = issRateado;
                                pedidoXMLNotaFiscal.BaseCalculoISS = baseCalculoRateada;
                                pedidoXMLNotaFiscal.PercentualAliquotaISS = regraISS.AliquotaISS;
                                pedidoXMLNotaFiscal.PercentualRetencaoISS = regraISS.PercentualRetencaoISS;
                                pedidoXMLNotaFiscal.IncluirISSBaseCalculo = regraISS.IncluirISSBaseCalculo;
                                pedidoXMLNotaFiscal.ValorRetencaoISS = retencaoISSRateado;
                                pedidoXMLNotaFiscal.AliquotaIR = regraISS.AliquotaIR;
                                pedidoXMLNotaFiscal.ReterIR = regraISS.ReterIR;
                                pedidoXMLNotaFiscal.BaseCalculoIR = baseCalculoIRRateada;
                                pedidoXMLNotaFiscal.ValorIR = irRateado;

                                servicoPedidoXMLNotaFiscal.PreencherCamposImpostoIBSCBS(pedidoXMLNotaFiscal, impostoIBSCBS);
                                servicoPedidoXMLNotaFiscal.PreencherValoresImpostoIBSCBSRateado(pedidoXMLNotaFiscal, valorbaseCalculoIBSCBSRateado, valorIBSEstadualRateado, valorIBSMunicipalRateado, valorCBSRateado);
                            }

                            pedidoXMLNotaFiscal.PossuiCTe = false;

                            if (possuiNFS)
                            {
                                pedidoXMLNotaFiscal.PossuiNFS = true;
                                pedidoXMLNotaFiscal.PossuiNFSManual = false;

                                if (pedidoXMLNotaFiscal.ModeloDocumentoFiscal == null || pedidoXMLNotaFiscal.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                                    pedidoXMLNotaFiscal.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.NFSe);
                            }
                            else
                            {
                                pedidoXMLNotaFiscal.PossuiNFS = false;
                                pedidoXMLNotaFiscal.PossuiNFSManual = true;
                                pedidoXMLNotaFiscal.ModeloDocumentoFiscal = null;
                            }

                            GerarComponenteISS(pedidoXMLNotaFiscal, unitOfWork);
                        }
                        else
                        {
                            pedidoXMLNotaFiscal.ValorISS = 0;
                            pedidoXMLNotaFiscal.BaseCalculoISS = 0;
                            pedidoXMLNotaFiscal.PercentualAliquotaISS = 0;
                            pedidoXMLNotaFiscal.PercentualRetencaoISS = 0;
                            pedidoXMLNotaFiscal.IncluirISSBaseCalculo = false;
                            pedidoXMLNotaFiscal.ValorRetencaoISS = 0;
                            pedidoXMLNotaFiscal.PossuiNFS = false;
                            pedidoXMLNotaFiscal.PossuiNFSManual = false;

                            pedidoXMLNotaFiscal.ValorIR = 0;
                            pedidoXMLNotaFiscal.ReterIR = false;
                            pedidoXMLNotaFiscal.AliquotaIR = 0;
                            pedidoXMLNotaFiscal.BaseCalculoIR = 0;

                            if (pedidoXMLNotaFiscal.ModeloDocumentoFiscal != null && pedidoXMLNotaFiscal.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                                pedidoXMLNotaFiscal.ModeloDocumentoFiscal = cargaPedido.ModeloDocumentoFiscal;

                        }
                    }

                    repPedidoXMLNotaFiscal.Atualizar(pedidoXMLNotaFiscal);

                    if (!calculoImpostosFilialEmissora)
                    {
                        pedidoXMLNotaFiscal.ValorTotalComponentes = 0m;
                        pedidoXMLNotaFiscal.ValorTotalMoedaComponentes = 0m;
                    }

                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> pedidoXMLNotaFiscalCompontesFrete = (from obj in pedidoXMLNotaFiscalCompontesFreteAgrupada where obj.PedidoXMLNotaFiscal.Codigo == pedidoXMLNotaFiscal.Codigo select obj).ToList();
                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete pedidoXMLNotaFiscalComponente in pedidoXMLNotaFiscalCompontesFrete)
                    {
                        if (pedidoXMLNotaFiscalComponente.ModeloDocumentoFiscal == null || pedidoXMLNotaFiscalComponente.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe || pedidoXMLNotaFiscalComponente.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                        {
                            pedidoXMLNotaFiscalComponente.ModeloDocumentoFiscal = pedidoXMLNotaFiscal.ModeloDocumentoFiscal;
                            repPedidoXMLNotaFiscalComponenteFrete.Atualizar(pedidoXMLNotaFiscalComponente);
                        }

                        if (!calculoImpostosFilialEmissora)
                        {
                            pedidoXMLNotaFiscal.ValorTotalComponentes += pedidoXMLNotaFiscalComponente.ValorComponente;
                            pedidoXMLNotaFiscal.ValorTotalMoedaComponentes += pedidoXMLNotaFiscalComponente.ValorTotalMoeda ?? 0m;
                        }
                    }

                    Servicos.Embarcador.Escrituracao.DocumentoProvisao.AdicionarDocumentoParaProvisao(pedidoXMLNotaFiscal, calculoImpostosFilialEmissora, tipoServicoMultisoftware, unitOfWork);

                    pedidosXmlsNotasFiscais[i] = pedidoXMLNotaFiscal;
                }

                if (possuiCTe)
                {
                    cargaPedido.PercentualAliquota = regraICMS.Aliquota;
                    cargaPedido.PercentualAliquotaInternaDifal = regraICMS.AliquotaInternaDifal;
                    cargaPedido.CST = regraICMS.CST;
                    cargaPedido.PercentualReducaoBC = regraICMS.PercentualReducaoBC;
                    cargaPedido.ObservacaoRegraICMSCTe = regraICMS.ObservacaoCTe;
                    cargaPedido.AliquotaPis = regraICMS.AliquotaPis;
                    cargaPedido.AliquotaCofins = regraICMS.AliquotaCofins;
                    cargaPedido.BaseCalculoICMS = regraICMS.ValorBaseCalculoICMS;
                    cargaPedido.ValorCreditoPresumido = regraICMS.ValorCreditoPresumido;
                    cargaPedido.PercentualCreditoPresumido = regraICMS.PercentualCreditoPresumido;
                    cargaPedido.DescontarICMSDoValorAReceber = regraICMS.DescontarICMSDoValorAReceber;
                    cargaPedido.NaoReduzirRetencaoICMSDoValorDaPrestacao = regraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao;
                    cargaPedido.NaoImprimirImpostosDACTE = regraICMS.NaoImprimirImpostosDACTE;
                    cargaPedido.NaoEnviarImpostoICMSNaEmissaoCte = regraICMS.NaoEnviarImpostoICMSNaEmissaoCte;
                    cargaPedido.SetarRegraICMS(regraICMS.CodigoRegra);
                    cargaPedido.PossuiCTe = true;

                    serCargaPedido.PreencherCamposImpostoIBSCBS(cargaPedido, impostoIBSCBS, true);

                    Servicos.Log.GravarInfo($"8 - CargaPedido: {cargaPedido.Codigo} -> Aliquota: {cargaPedido.PercentualAliquota}", "ProcessarAliquota");
                }

                if (possuiNFS && !calculoImpostosFilialEmissora)
                    cargaPedido.PossuiNFS = true;

                if (possuiNFSManual && !calculoImpostosFilialEmissora)
                {
                    cargaPedido.PossuiNFSManual = true;
                    cargaPedido.ModeloDocumentoFiscalIntramunicipal = modeloDocumentoFiscalIntramunicipal;
                }

                if ((possuiNFS || possuiNFSManual) && !calculoImpostosFilialEmissora)
                {
                    if (regraISS != null)
                    {
                        cargaPedido.PercentualAliquotaISS = regraISS.AliquotaISS;
                        cargaPedido.PercentualRetencaoISS = regraISS.PercentualRetencaoISS;
                        cargaPedido.IncluirISSBaseCalculo = regraISS.IncluirISSBaseCalculo;
                        cargaPedido.AliquotaIR = regraISS.AliquotaIR;
                        cargaPedido.BaseCalculoIR = regraISS.BaseCalculoIR;
                        cargaPedido.ReterIR = regraISS.ReterIR;
                        cargaPedido.ValorIR = regraISS.ValorIR;
                        serCargaPedido.PreencherCamposImpostoIBSCBS(cargaPedido, impostoIBSCBS, true);
                    }
                }
            }
            else
            {
                throw new Exception("Tomador não encontrado na base da multisoftware");
            }
        }

        private void ObterBaseCalculo(ref decimal valorBaseCalculo, bool calculoImpostosFilialEmissora, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> pedidoXMLNotaFiscalCompontesFrete, List<Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo> pedagioEstadosBaseCalculo = null)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(unidadeTrabalho);

            if (pedidoXMLNotaFiscalCompontesFrete == null)
                pedidoXMLNotaFiscalCompontesFrete = repPedidoXMLNotaFiscalComponenteFrete.BuscarPorXMLnotaFiscal(pedidoXMLNotaFiscal.Codigo, null, calculoImpostosFilialEmissora);

            Dominio.Entidades.Empresa empresa = calculoImpostosFilialEmissora ? carga.EmpresaFilialEmissora : carga.Empresa;
            Servicos.Embarcador.Carga.ICMS servicoIcms = new Servicos.Embarcador.Carga.ICMS();

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete pedidoXMLNotaFiscalComponenteFrete in pedidoXMLNotaFiscalCompontesFrete)
                valorBaseCalculo += servicoIcms.ObterValorIcmsComponenteFrete(pedidoXMLNotaFiscalComponenteFrete, empresa, cargaPedido.Origem.Estado.Sigla, unidadeTrabalho, tipoServicoMultisoftware, pedagioEstadosBaseCalculo);
        }

        private Dominio.Entidades.ModeloDocumentoFiscal ObterModeloDocumentoEmissao(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaPreviaDocumento repCargaPreviaDocumento = new Repositorio.Embarcador.Cargas.CargaPreviaDocumento(unitOfWork);

            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = repCargaPreviaDocumento.ObterModeloDocumentoPorNotaFiscal(pedidoXMLNotaFiscal.Codigo);

            return modeloDocumentoFiscal;
        }

        #endregion
    }
}
