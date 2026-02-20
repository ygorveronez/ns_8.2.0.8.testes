using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Servicos.Embarcador.Carga
{
    public class FreteFilialEmissora
    {
        #region Métodos Públicos

        public static Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete CalcularFreteFilialEmissora(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool apenasVerificar, bool adicionarComponentesCarga, bool atualizarInformacoesPagamentoPedido, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido)
        {
            Servicos.Embarcador.Carga.Frete serFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork, tipoServicoMultisoftware);

            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

            Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete() { };

            bool freteFilialEmissoraOperador = carga.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Operador && (configuracaoGeralCarga.PermiteInformarFreteOperadorFilialEmissora ?? false);

            if ((carga.EmpresaFilialEmissora != null || carga.CalcularFreteCliente)
                && (configuracaoTMS.CalcularFreteFilialEmissoraPorTabelaDeFrete || (carga.TipoOperacao?.CalculaFretePorTabelaFreteFilialEmissora ?? false))
                && ((carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Operador || carga.CalcularFreteCliente) || freteFilialEmissoraOperador))
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFreteExistente = carga.TabelaFreteFilialEmissora;

                StringBuilder mensagemRetorno = new StringBuilder();
                List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasFreteFilialEmissora = new List<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();
                if (!apenasVerificar)
                    tabelasFreteFilialEmissora = serFrete.ObterTabelasFrete(carga, unitOfWork, tipoServicoMultisoftware, configuracaoTMS, ref mensagemRetorno, true);
                else if (tabelaFreteExistente != null)
                    tabelasFreteFilialEmissora.Add(tabelaFreteExistente);

                if (!freteFilialEmissoraOperador && !serFrete.ValidarQuantidadeTabelasFreteDisponivel(ref retorno, ref carga, tabelasFreteFilialEmissora, mensagemRetorno, apenasVerificar, unitOfWork, configuracaoTMS))
                    return retorno;

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);

                new Servicos.Embarcador.Carga.RateioFormula().DefinirFormulaRateio(carga, cargaPedidos, unitOfWork);

                if (serFrete.VerificarTipoFreteOperador(ref retorno, ref carga, cargaPedidos, configuracaoTMS, null, apenasVerificar, adicionarComponentesCarga, true, unitOfWork) ||
                    serFrete.VerificarTipoFreteEmbarcador(ref retorno, carga, apenasVerificar, true, unitOfWork) ||
                    serFrete.VerificarTabelaFreteExistente(ref retorno, carga, cargaPedidos, configuracaoTMS, null, tabelaFreteExistente, apenasVerificar, true, unitOfWork))
                    return retorno;

                Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = tabelasFreteFilialEmissora[0];

                if (tabelaFrete != null)
                {

                    switch (tabelaFrete.TipoTabelaFrete)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente:

                            if (tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorCarga)
                            {
                                if (serFrete.CalcularFretePorCliente(ref retorno, ref carga, cargaPedidos, tabelaFrete, apenasVerificar, unitOfWork, atualizarInformacoesPagamentoPedido, adicionarComponentesCarga, true, configuracaoTMS))
                                    return retorno;
                            }
                            else if (tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorPedido)
                            {
                                if (serFrete.CalcularFretePorClientePedido(ref retorno, ref carga, cargaPedidos, tabelaFrete, apenasVerificar, unitOfWork, atualizarInformacoesPagamentoPedido, adicionarComponentesCarga, true, configuracaoTMS))
                                    return retorno;
                            }
                            else if (tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorDocumentoEmitido)
                            {
                                if (serFrete.CalcularFretePorDocumentoEmitido(ref retorno, ref carga, tabelaFrete, apenasVerificar, unitOfWork, atualizarInformacoesPagamentoPedido, adicionarComponentesCarga, true, configuracaoTMS))
                                    return retorno;
                            }
                            else if (tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorMaiorValorPedido)
                            {
                                if (serFrete.CalcularFretePorClienteMaiorValorPedido(ref retorno, ref carga, cargaPedidos, tabelaFrete, apenasVerificar, unitOfWork, atualizarInformacoesPagamentoPedido, adicionarComponentesCarga, true, configuracaoTMS))
                                    return retorno;
                            }
                            else if (tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorPedidosAgrupados)
                            {
                                Servicos.Embarcador.Carga.FretePedidoAgrupado svcFretePedidoAgrupado = new FretePedidoAgrupado(configuracaoTMS, unitOfWork, tipoServicoMultisoftware);
                                svcFretePedidoAgrupado.CalcularFrete(ref retorno, ref carga, cargaPedidos, tabelaFrete, apenasVerificar, atualizarInformacoesPagamentoPedido, adicionarComponentesCarga, true);
                            }
                            else if (tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorMaiorDistanciaPedidoAgrupados)
                            {
                                serFrete.CalcularFretePorClienteMaiorDistanciaPedido(ref retorno, ref carga, cargaPedidos, tabelaFrete, apenasVerificar, unitOfWork, atualizarInformacoesPagamentoPedido, adicionarComponentesCarga, true, configuracaoTMS);
                            }
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaRota:
                            Dominio.ObjetosDeValor.Embarcador.Carga.RetornoRotasCarga dynRota = serFrete.ObterDadosRotas(carga, unitOfWork, configuracaoPedido);
                            serFrete.CalcularFretePorRota(ref retorno, ref carga, cargaPedidos, configuracaoTMS, dynRota, tabelaFrete, apenasVerificar, adicionarComponentesCarga, true, unitOfWork);
                            break;

                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaComissaoProduto:
                            serFrete.CalcularFretePorComissaoProduto(ref retorno, ref carga, cargaPedidos, configuracaoTMS, tabelaFrete, apenasVerificar, true, unitOfWork);
                            break;
                    }
                    serFrete.SetarDadosGeraisRetornoFrete(ref retorno, carga, unitOfWork, apenasVerificar, true, carga.TipoFreteEscolhido); //Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Tabela);
                    return retorno;

                }
            }
            else
            {
                Servicos.Embarcador.Carga.RateioFrete serFreteRateio = new RateioFrete(unitOfWork);
                Servicos.Embarcador.Carga.ComponetesFrete serCargaComponetesFrete = new Servicos.Embarcador.Carga.ComponetesFrete(unitOfWork);
                RateioNotaFiscal serRateioNotaFiscal = new RateioNotaFiscal(unitOfWork);
                Servicos.Embarcador.Carga.ICMS serCargaICMS = new Servicos.Embarcador.Carga.ICMS(unitOfWork);

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
                bool possuiTrechoAnterior = false;

                if (!apenasVerificar)
                {
                    serFreteRateio.ZerarValoresDaCarga(carga, true, unitOfWork);

                    Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
                    Repositorio.Embarcador.ICMS.PedagioEstadoBaseCalculo repPedagioEstadoBaseCalculo = new Repositorio.Embarcador.ICMS.PedagioEstadoBaseCalculo(unitOfWork);
                    Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidosComponentesFreteCarga = repCargaPedidoComponenteFrete.BuscarPorCarga(carga.Codigo, true);
                    List<Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo> pedagioEstadosBaseCalculo = repPedagioEstadoBaseCalculo.BuscarPorEstados((from obj in cargaPedidos select obj.Origem.Estado.Sigla).Distinct().ToList());
                    List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota> tabelaAliquotas = new List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota>();
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = serCargaICMS.ObterProdutosCargaContidosEmRegras(carga, unitOfWork);
                    List<Dominio.Entidades.Cliente> tomadoresFilialEmissora = Servicos.Embarcador.Carga.FilialEmissora.ObterTomadoresFilialEmissora((from obj in cargaPedidos where obj.CargaPedidoFilialEmissora && obj.CargaOrigem.EmpresaFilialEmissora != null select obj.CargaOrigem.EmpresaFilialEmissora.CNPJ_SemFormato).Distinct().ToList(), unitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem = Servicos.Embarcador.Carga.Carga.ObterCargasOrigem(carga, unitOfWork);

                    Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFreteICMS = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS);

                    bool abriuTransacao = false;
                    if (!unitOfWork.IsActiveTransaction())
                    {
                        unitOfWork.Start();
                        abriuTransacao = true;
                    }

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                    {
                        Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem = (from obj in cargasOrigem where obj.Codigo == cargaPedido.CargaOrigem.Codigo select obj).FirstOrDefault();
                        if (cargaPedido.CargaPedidoFilialEmissora && cargaOrigem.EmpresaFilialEmissora == null && cargaOrigem.CargaAgrupamento != null)
                        {
                            cargaOrigem.EmpresaFilialEmissora = cargaOrigem.CargaAgrupamento.EmpresaFilialEmissora;
                            repCarga.Atualizar(cargaOrigem);
                        }

                        if (cargaPedido.ValorFreteAPagar > 0)
                            Servicos.Embarcador.Carga.FreteFilialEmissora.SetarFreteEmbarcadorFilialEmissora(ref carga, cargaOrigem, cargaPedido, tipoServicoMultisoftware, true, unitOfWork, configuracaoTMS, cargaPedidosComponentesFreteCarga, pedagioEstadosBaseCalculo, cargaPedidoProdutos, tabelaAliquotas, tomadoresFilialEmissora, cargaPedidosComponentesFreteCarga, componenteFreteICMS, out possuiTrechoAnterior, configuracaoGeralCarga);
                    }

                    serFreteRateio.GerarComponenteICMS(carga, cargaPedidos, false, true, unitOfWork);

                    if (abriuTransacao)
                        unitOfWork.CommitChanges();

                    if (carga.ExigeNotaFiscalParaCalcularFrete)
                        serRateioNotaFiscal.RatearFreteCargaPedidoEntreNotas(carga, cargaPedidos, cargaPedidosComponentesFreteCarga, true, tipoServicoMultisoftware, unitOfWork, configuracaoTMS);

                    serCargaComponetesFrete.AdicionarComponentesCargaAgrupada(carga, true, cargaPedidosComponentesFreteCarga, unitOfWork);
                    serFreteRateio.AcrescentarValoresDaCargaAgrupada(carga, true, cargaPedidos, unitOfWork);

                    //serFreteRateio.RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, configuracaoTMS, true, unitOfWork, tipoServicoMultisoftware);
                }

                Servicos.Embarcador.Carga.FreteFilialEmissora.VerificarCargaAguardaValorRedespacho(ref carga, unitOfWork);
                retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido;
                serFrete.SetarDadosGeraisRetornoFrete(ref retorno, carga, unitOfWork, apenasVerificar, true, carga.TipoFreteEscolhido); //Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Tabela);
                return retorno;
            }
            return null;
        }

        public static void SetarValorFreteFilialTrechoAnterior(ref Dominio.Entidades.Embarcador.Cargas.Carga carga, bool apenasVerificar, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Servicos.Embarcador.Carga.RateioFrete serFreteRateio = new RateioFrete(unitOfWork);
            Servicos.Embarcador.Carga.ComponetesFrete serCargaComponetesFrete = new Servicos.Embarcador.Carga.ComponetesFrete(unitOfWork);
            Servicos.Embarcador.Carga.ICMS serCargaICMS = new Servicos.Embarcador.Carga.ICMS(unitOfWork);
            RateioNotaFiscal serRateioNotaFiscal = new RateioNotaFiscal(unitOfWork);

            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            if (!apenasVerificar)
            {
                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFreteICMS = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork).BuscarPrimeiroRegistro();

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasAnteriores = repCargaPedido.BuscarCargasTrechoAnterior(carga.Codigo);
                bool possuiTrechoAnterior = false;
                foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaAnteriorF in cargasAnteriores)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga cargaAnterior = cargaAnteriorF;
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(cargaAnterior.Codigo);

                    serFreteRateio.ZerarValoresDaCarga(cargaAnterior, true, unitOfWork);

                    Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
                    Repositorio.Embarcador.ICMS.PedagioEstadoBaseCalculo repPedagioEstadoBaseCalculo = new Repositorio.Embarcador.ICMS.PedagioEstadoBaseCalculo(unitOfWork);

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidosComponentesFreteCarga = repCargaPedidoComponenteFrete.BuscarPorCarga(cargaAnterior.Codigo, true);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFretesDiretos = new List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();
                    List<Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo> pedagioEstadosBaseCalculo = repPedagioEstadoBaseCalculo.BuscarPorEstados((from obj in cargaPedidos select obj.Origem.Estado.Sigla).Distinct().ToList());
                    List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota> tabelaAliquotas = new List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota>();
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = serCargaICMS.ObterProdutosCargaContidosEmRegras(cargaAnterior, unitOfWork);
                    List<Dominio.Entidades.Cliente> tomadoresFilialEmissora = Servicos.Embarcador.Carga.FilialEmissora.ObterTomadoresFilialEmissora((from obj in cargaPedidos where obj.CargaPedidoFilialEmissora select obj.CargaOrigem.EmpresaFilialEmissora.CNPJ_SemFormato).Distinct().ToList(), unitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem = Servicos.Embarcador.Carga.Carga.ObterCargasOrigem(cargaAnterior, unitOfWork);

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosTrechosAnteriores = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidosComponentesFreteCargaImpostos = repCargaPedidoComponenteFrete.BuscarPorCargaComponentesImpostos(cargaAnterior.Codigo, true);

                    bool abriuTransacao = false;
                    if (!unitOfWork.IsActiveTransaction())
                    {
                        unitOfWork.Start();
                        abriuTransacao = true;
                    }

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                    {
                        Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem = (from obj in cargasOrigem where obj.Codigo == cargaPedido.CargaOrigem.Codigo select obj).FirstOrDefault();

                        if (cargaPedido.ValorFreteAPagar > 0)
                            Servicos.Embarcador.Carga.FreteFilialEmissora.SetarFreteEmbarcadorFilialEmissora(ref cargaAnterior, cargaOrigem, cargaPedido, tipoServicoMultisoftware, true, unitOfWork, configuracao, cargaPedidosComponentesFreteCarga, pedagioEstadosBaseCalculo, cargaPedidoProdutos, tabelaAliquotas, tomadoresFilialEmissora, cargaPedidosComponentesFreteCargaImpostos, componenteFreteICMS, out possuiTrechoAnterior, configuracaoGeralCarga);
                    }

                    Servicos.Embarcador.Carga.FreteFilialEmissora.VerificarCargaAguardaValorRedespacho(ref cargaAnterior, unitOfWork);

                    if (abriuTransacao)
                        unitOfWork.CommitChanges();

                    if (cargaAnterior.ExigeNotaFiscalParaCalcularFrete)
                        serRateioNotaFiscal.RatearFreteCargaPedidoEntreNotas(cargaAnterior, cargaPedidos, cargaPedidosComponentesFreteCarga, true, tipoServicoMultisoftware, unitOfWork, configuracao);

                    serCargaComponetesFrete.AdicionarComponentesCargaAgrupada(cargaAnterior, true, cargaPedidosComponentesFreteCarga, unitOfWork);
                    serFreteRateio.AcrescentarValoresDaCargaAgrupada(cargaAnterior, true, cargaPedidos, unitOfWork);
                }
            }
        }

        public static void SetarFreteEmbarcadorNotaFiscalFilialEmissora(ref Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            if (!cargaPedido.CargaPedidoFilialEmissora)
                return;

            Servicos.Embarcador.Carga.RateioFrete serRateioFrete = new RateioFrete(unitOfWork);

            Repositorio.Embarcador.Filiais.EstadoDestinoEmpresaEmissora repositorioEstadoDestinoEmpresaEmissora = new Repositorio.Embarcador.Filiais.EstadoDestinoEmpresaEmissora(unitOfWork);
            List<Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora> estadosDestinoEmpresaEmissora = cargaPedido.Carga.Filial != null ? repositorioEstadoDestinoEmpresaEmissora.BuscarPorFilial(cargaPedido.Carga.Filial.Codigo) : new List<Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora>();
            Dominio.Entidades.Localidade destino = cargaPedido.Destino;

            Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora estadoDestino = estadosDestinoEmpresaEmissora.Find(e => e.Estado.Codigo == destino?.Estado?.Codigo);

            if ((estadosDestinoEmpresaEmissora.Count > 0) && estadoDestino != null)
                cargaPedido.Carga.EmpresaFilialEmissora = estadoDestino.Empresa;
            else
                cargaPedido.Carga.EmpresaFilialEmissora = cargaPedido.Carga.Filial?.EmpresaEmissora ?? null;

            Servicos.Embarcador.Carga.RateioNotaFiscal serRateioNotaFiscal = new Servicos.Embarcador.Carga.RateioNotaFiscal(unitOfWork);

            pedidoXMLNotaFiscal.ValorFreteFilialEmissora = pedidoXMLNotaFiscal.ValorFrete;
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> pedidoXMLNotaFiscalCompontesFrete = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete>();
            serRateioNotaFiscal.CalcularImpostos(cargaPedido, cargaPedido.Carga, true, pedidoXMLNotaFiscal, pedidoXMLNotaFiscal.ValorFreteFilialEmissora, cargaPedido.IncluirICMSBaseCalculo, cargaPedido.PercentualIncluirBaseCalculo, pedidoXMLNotaFiscalCompontesFrete, tipoServicoMultisoftware, null, null, unitOfWork, configuracao);
            serRateioNotaFiscal.GerarComponenteICMS(pedidoXMLNotaFiscal, true, unitOfWork);

            cargaPedido.ValorFreteFilialEmissora += pedidoXMLNotaFiscal.ValorFreteFilialEmissora;
            cargaPedido.ValorFreteAPagarFilialEmissora += pedidoXMLNotaFiscal.ValorFreteFilialEmissora;
            cargaPedido.ValorICMSFilialEmissora += pedidoXMLNotaFiscal.ValorICMSFilialEmissora;
            cargaPedido.BaseCalculoICMSFilialEmissora += pedidoXMLNotaFiscal.BaseCalculoICMSFilialEmissora;
            cargaPedido.PercentualAliquotaFilialEmissora = pedidoXMLNotaFiscal.PercentualAliquotaFilialEmissora;
            cargaPedido.PercentualReducaoBCFilialEmissora = pedidoXMLNotaFiscal.PercentualReducaoBCFilialEmissora;

            Dominio.ObjetosDeValor.Embarcador.Carga.CargaValoresAcrescentar cargaValoresAcrescentar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaValoresAcrescentar()
            {
                ValorFrete = pedidoXMLNotaFiscal.ValorFreteFilialEmissora,
                ValorFreteAPagar = pedidoXMLNotaFiscal.ValorFreteFilialEmissora,
                ValorICMS = pedidoXMLNotaFiscal.ValorICMSFilialEmissora,
                ValorIBSEstadual = pedidoXMLNotaFiscal.ValorIBSEstadualFilialEmissora,
                ValorIBSMunicipal = pedidoXMLNotaFiscal.ValorIBSMunicipalFilialEmissora,
                ValorCBS = pedidoXMLNotaFiscal.ValorCBSFilialEmissora,
            };

            serRateioFrete.AcrescentarValoresFilialEmissoraDaCarga(cargaPedido.Carga, cargaValoresAcrescentar);
        }

        public static void SetarFreteEmbarcadorFilialEmissora(ref Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool calculoFreteFilialEmissora, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFretes, List<Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo> pedagioEstadosBaseCalculo, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos, List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota> tabelaAliquotas, List<Dominio.Entidades.Cliente> tomadoresFilialEmissora, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidosComponentesFreteCargaImpostos, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFreteICMS, out bool possuiTrechoAnterior, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {
            possuiTrechoAnterior = false;
            if (cargaPedido.CargaPedidoFilialEmissora && calculoFreteFilialEmissora)
            {
                Servicos.Embarcador.Carga.RateioFrete serRateioFrete = new Embarcador.Carga.RateioFrete(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                if (carga.Filial?.EmpresaEmissora != null)
                {
                    if (carga.EmpresaFilialEmissora == null)
                    {
                        Repositorio.Embarcador.Filiais.EstadoDestinoEmpresaEmissora repositorioEstadoDestinoEmpresaEmissora = new Repositorio.Embarcador.Filiais.EstadoDestinoEmpresaEmissora(unitOfWork);
                        List<Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora> estadosDestinoEmpresaEmissora = carga.Filial != null ? repositorioEstadoDestinoEmpresaEmissora.BuscarPorFilial(carga.Filial.Codigo) : new List<Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora>();
                        Dominio.Entidades.Localidade destino = cargaPedido.Destino;
                        Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora estadoDestino = estadosDestinoEmpresaEmissora.Find(e => e.Estado.Codigo == destino?.Estado.Codigo);

                        if ((estadosDestinoEmpresaEmissora.Count > 0) && estadoDestino != null)
                            cargaPedido.Carga.EmpresaFilialEmissora = estadoDestino.Empresa;
                        else
                            carga.EmpresaFilialEmissora = carga.Filial.EmpresaEmissora;
                    }
                }
                else
                    carga.EmpresaFilialEmissora = null;

                if (cargaPedido.TipoContratacaoCargaSubContratacaoFilialEmissora == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada
                    || ((cargaPedido.TipoContratacaoCargaSubContratacaoFilialEmissora == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho || cargaPedido.TipoContratacaoCargaSubContratacaoFilialEmissora == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario)
                    && cargaPedido.CargaPedidoProximoTrecho == null))
                {
                    if (carga.TabelaFrete == null || !configuracao.CalcularFreteFilialEmissoraPorTabelaDeFrete)
                    {
                        if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
                        {
                            bool utilizarFreteFilialEmissoraEmbarcador = (configuracao.UtilizarFreteFilialEmissoraEmbarcador && carga.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador);

                            decimal valorISS = cargaPedido.ValorISS - cargaPedido.ValorRetencaoISS;
                            if (!cargaPedido.IncluirISSBaseCalculo)
                                valorISS = 0;

                            decimal valorICMS = !utilizarFreteFilialEmissoraEmbarcador ? cargaPedido.ValorICMS : cargaPedido.ValorICMSFilialEmissora;
                            if (!utilizarFreteFilialEmissoraEmbarcador && (!cargaPedido.IncluirICMSBaseCalculo || cargaPedido.CST == "60"))
                                valorICMS = 0;
                            else if (utilizarFreteFilialEmissoraEmbarcador && (!cargaPedido.IncluirICMSBaseCalculoFilialEmissora || cargaPedido.CSTFilialEmissora == "60"))
                                valorICMS = 0;

                            decimal valorFretePedido = (!utilizarFreteFilialEmissoraEmbarcador ? cargaPedido.ValorFreteAPagar : cargaPedido.ValorFreteAPagarFilialEmissora) - valorISS - valorICMS;
                            cargaPedido.ValorFreteFilialEmissora = Math.Round(valorFretePedido, 2, MidpointRounding.AwayFromZero);
                        }

                        if (!cargaPedido.ImpostoInformadoPeloEmbarcador)
                        {
                            cargaPedido.IncluirICMSBaseCalculoFilialEmissora = true;
                            cargaPedido.PercentualIncluirBaseCalculoFilialEmissora = 100;
                            serRateioFrete.CalcularImpostos(ref carga, cargaOrigem, cargaPedido, cargaPedido.ValorFreteFilialEmissora, true, tipoServicoMultisoftware, unitOfWork, configuracao, cargaPedidoComponentesFretes, pedagioEstadosBaseCalculo, cargaPedidoProdutos, tabelaAliquotas, tomadoresFilialEmissora, configuracaoGeralCarga);
                            serRateioFrete.GerarComponenteICMS(cargaPedido, true, componenteFreteICMS, cargaPedidoComponentesFretes, unitOfWork);
                        }

                        repCargaPedido.Atualizar(cargaPedido);
                    }
                }
                else
                {
                    if (cargaPedido.CargaPedidoProximoTrecho != null && cargaPedido.CargaPedidoProximoTrecho.ValorFreteAPagar > 0 && (cargaPedido.CargaPedidoProximoTrecho.Carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Operador || cargaPedido.CargaPedidoProximoTrecho.Carga.SituacaoAlteracaoFreteCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlteracaoFreteCarga.Aprovada || cargaPedido.CargaPedidoProximoTrecho.Carga.SituacaoAlteracaoFreteCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlteracaoFreteCarga.NaoInformada))
                    {
                        //quando possuir redespacho o valor do CT-e filial emissora deve ser a soma dos dois fretes.
                        decimal valorISS = cargaPedido.ValorISS - cargaPedido.ValorRetencaoISS;
                        if (!cargaPedido.IncluirISSBaseCalculo)
                            valorISS = 0;

                        decimal valorICMS = cargaPedido.ValorICMS;
                        if (!cargaPedido.IncluirICMSBaseCalculo || cargaPedido.CST == "60")
                            valorICMS = 0;

                        decimal valorISSProximo = cargaPedido.CargaPedidoProximoTrecho.ValorISS - cargaPedido.CargaPedidoProximoTrecho.ValorRetencaoISS;
                        if (!cargaPedido.CargaPedidoProximoTrecho.IncluirISSBaseCalculo)
                            valorISSProximo = 0;

                        decimal valorICMSProximo = cargaPedido.CargaPedidoProximoTrecho.ValorICMS;
                        if (!cargaPedido.CargaPedidoProximoTrecho.IncluirICMSBaseCalculo || cargaPedido.CargaPedidoProximoTrecho.CST == "60")
                            valorICMSProximo = 0;

                        decimal valorFretePedido = (cargaPedido.ValorFreteAPagar - valorICMS - valorISS) + (cargaPedido.CargaPedidoProximoTrecho.ValorFreteAPagar - valorICMSProximo - valorISSProximo);
                        cargaPedido.ValorFreteFilialEmissora = Math.Round(valorFretePedido, 2, MidpointRounding.AwayFromZero);
                        cargaPedido.IncluirICMSBaseCalculoFilialEmissora = true;
                        cargaPedido.PercentualIncluirBaseCalculoFilialEmissora = 100;
                        serRateioFrete.CalcularImpostos(ref carga, cargaOrigem, cargaPedido, cargaPedido.ValorFreteFilialEmissora, true, tipoServicoMultisoftware, unitOfWork, configuracao, cargaPedidoComponentesFretes, pedagioEstadosBaseCalculo, cargaPedidoProdutos, tabelaAliquotas, tomadoresFilialEmissora, configuracaoGeralCarga);
                        serRateioFrete.GerarComponenteICMS(cargaPedido, true, componenteFreteICMS, cargaPedidoComponentesFretes, unitOfWork);
                        cargaPedido.AgValorRedespacho = false;

                        repCargaPedido.Atualizar(cargaPedido);
                    }
                    else
                    {
                        cargaPedido.AgValorRedespacho = true;
                        carga.AgValorRedespacho = true;
                        repCargaPedido.Atualizar(cargaPedido);
                    }
                }
            }
            else if (cargaPedido.CargaPedidoTrechoAnterior != null && cargaPedido.CargaPedidoTrechoAnterior.CargaPedidoFilialEmissora)
            {
                bool podeRecalcularTrechoAnterior = cargaPedido.CargaPedidoTrechoAnterior.AgValorRedespacho;

                if (!podeRecalcularTrechoAnterior && !cargaPedido.CargaPedidoTrechoAnterior.PendenteGerarCargaDistribuidor)
                {
                    if (cargaPedido.CargaPedidoTrechoAnterior.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                    {
                        if (cargaPedido.CargaPedidoTrechoAnterior.Carga.Carregamento == null || !cargaPedido.CargaPedidoTrechoAnterior.Carga.DataEnvioUltimaNFe.HasValue)
                            podeRecalcularTrechoAnterior = true;
                        //else
                        //{
                        //    foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaCarregamento in cargaPedido.CargaPedidoTrechoAnterior.Carga.Carregamento.CargasFrete.ToList())
                        //    {
                        //        if (cargaCarregamento.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete || cargaCarregamento.DataEnvioUltimaNFe.HasValue)
                        //            break;
                        //    }
                        //}
                    }
                }

                if (podeRecalcularTrechoAnterior && cargaPedido.ValorFreteAPagar > 0)
                    possuiTrechoAnterior = true;

            }
        }

        public static void VerificarCargaAguardaValorRedespacho(ref Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga.AgValorRedespacho)
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                if (!repCargaPedido.VerificarCargasAgValorRedespacho(carga.Codigo))
                {
                    carga.AgValorRedespacho = false;
                }
            }
        }

        #endregion
    }
}
