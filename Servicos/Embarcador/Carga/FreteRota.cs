using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga
{
    public class FreteRota : ServicoBase
    {
        public FreteRota(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete CalcularFretePorRota(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Repositorio.UnitOfWork unitOfWork, bool apenasVerificar, bool adicionarComponentesCarga, bool calculoFreteFilialEmissora, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete();
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(unitOfWork);
            Repositorio.Embarcador.Logistica.Rota repRota = new Repositorio.Embarcador.Logistica.Rota(unitOfWork);
            Servicos.Embarcador.Carga.ComponetesFrete serCargaComponentesFrete = new ComponetesFrete(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete> composicaoFretes = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete>();

            List<Dominio.Entidades.Embarcador.Cargas.CargaPercurso> cargaPercursos = (from obj in repCargaPercurso.ConsultarPorCarga(carga.Codigo) where obj.TipoRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRota.descarregamento select obj).ToList();
            Dominio.ObjetosDeValor.Embarcador.Frete.FreteRota freteRota = null;

            if (cargaPercursos.Count > 0)
            {
                Dominio.Entidades.Embarcador.Logistica.Rota ultimaRota = repRota.BuscarRotaPorOrigemDestino(cargaPercursos[0].Origem.Codigo, cargaPercursos[cargaPercursos.Count - 1].Destino.Codigo);

                freteRota = ObterFretePorRota(ultimaRota, carga, tabelaFrete, apenasVerificar, calculoFreteFilialEmissora, unitOfWork);

                if (freteRota.situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoFreteRota.FreteValido)
                {
                    carga.MotivoPendenciaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete.NenhumPendencia;

                    if (!apenasVerificar)
                    {
                        if (!calculoFreteFilialEmissora)
                            carga.TabelaFrete = tabelaFrete;
                        else
                            carga.TabelaFreteFilialEmissora = tabelaFrete;

                        if (tabelaFrete != null && (tabelaFrete.UtilizaModeloVeicularVeiculo || tabelaFrete.UtilizarModeloVeicularDaCargaParaCalculo))
                        {
                            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCalculo = null;
                            if (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count > 0 && carga.VeiculosVinculados.FirstOrDefault().ModeloVeicularCarga != null)
                                modeloVeicularCalculo = carga.VeiculosVinculados.FirstOrDefault().ModeloVeicularCarga;
                            else if (carga.Veiculo != null && carga.Veiculo.ModeloVeicularCarga != null)
                                modeloVeicularCalculo = carga.Veiculo.ModeloVeicularCarga;
                            if (modeloVeicularCalculo != null)
                                carga.ModeloVeicularCarga = modeloVeicularCalculo;
                        }

                        carga.ValorFrete = freteRota.ValorFrete;

                        composicaoFretes.Add(Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor por Rota", freteRota.DescricaoDestinos + " = " + freteRota.ValorFrete.ToString("n2"), freteRota.ValorFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ValorFreteLiquido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, "Valor do Frete por Rota", 0, freteRota.ValorFrete));

                        if (adicionarComponentesCarga)
                        {
                            if (freteRota.ValorPedagio > 0)
                            {

                                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO);

                                serCargaComponentesFrete.AdicionarCargaComplementoFretePorTipo(carga, freteRota.ValorPedagio, 0, cargaPercursos[0].Origem.Estado.Sigla, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO, calculoFreteFilialEmissora, unitOfWork);
                                composicaoFretes.Add(Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor de Pedágio para a Rota", " Valor de Pedágio para a rota " + freteRota.DescricaoDestinos + " = " + freteRota.ValorPedagio.ToString("n2"), freteRota.ValorPedagio, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ComponenteFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor, "PEDÁGIO", componenteFrete?.Codigo ?? 0, freteRota.ValorPedagio));
                            }
                        }

                        Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente = carga.Veiculo?.ModeloCarroceria?.ComponenteFrete;

                        if (componente == null && carga.VeiculosVinculados != null)
                            componente = (from obj in carga.VeiculosVinculados where obj.ModeloCarroceria != null && obj.ModeloCarroceria.ComponenteFrete != null select obj.ModeloCarroceria.ComponenteFrete).FirstOrDefault();

                        Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicaoFreteAdicionarCarroceria = new Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete();
                        decimal valorAdicionalCarroceria = Frete.CalcularValorFreteAdicionalPorModeloCarroceriaVeiculo(carga.Veiculo, carga.VeiculosVinculados, ref composicaoFreteAdicionarCarroceria, componente, carga.ValorFrete);

                        if (componente != null && valorAdicionalCarroceria > 0)
                        {
                            serCargaComponentesFrete.AdicionarComponenteFreteCarga(carga, componente, valorAdicionalCarroceria, 0, calculoFreteFilialEmissora, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, componente.TipoComponenteFrete, null, true, false, null, tipoServicoMultisoftware, null, unitOfWork, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.TabelaFrete, true);
                            composicaoFretes.Add(composicaoFreteAdicionarCarroceria);
                        }

                        Servicos.Embarcador.Carga.RateioFrete serCargaRateioFrete = new RateioFrete(unitOfWork);
                        serCargaRateioFrete.RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, configuracao, calculoFreteFilialEmissora, unitOfWork, tipoServicoMultisoftware);

                        if (!calculoFreteFilialEmissora)
                            carga.ValorFreteTabelaFrete = carga.ValorFreteAPagar;
                        else
                            carga.ValorFreteTabelaFreteFilialEmissora = carga.ValorFreteAPagar;

                        Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.SetarComposicaoFrete(carga, null, null, null, calculoFreteFilialEmissora, composicaoFretes, unitOfWork, null);
                    }

                    bool destacarComponenteTabelaFrete = Servicos.Embarcador.Carga.Frete.DestacarComponenteTabelaFrete(carga.TabelaFrete, carga.ContratoFreteTransportador?.ComponenteFreteValorContrato);
                    bool descontarComponenteFreteLiquido = (destacarComponenteTabelaFrete ? carga.TabelaFrete?.DescontarComponenteFreteLiquido : carga.ContratoFreteTransportador?.ComponenteFreteValorContrato?.DescontarComponenteFreteLiquido) ?? false;

                    retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido;
                    retorno.valorFrete = carga.ValorFrete;
                    retorno.ValorFreteLiquido = carga.ValorFreteLiquido + (((carga.ContratoFreteTransportador?.ComponenteFreteValorContrato?.SomarComponenteFreteLiquido ?? false) || descontarComponenteFreteLiquido) ? 0m : carga.ValorFreteContratoFreteTotal);
                    retorno.valorFreteAPagar = carga.ValorFreteAPagar;
                    retorno.ValorFreteNegociado = carga.ValorFreteNegociado;
                    retorno.valorFreteTabelaFrete = carga.ValorFreteTabelaFrete;
                    retorno.valorFreteOperador = carga.ValorFreteOperador;
                    retorno.valorFreteLeilao = carga.ValorFreteLeilao;
                    retorno.valorICMS = carga.ValorICMS;
                    retorno.valorISS = carga.ValorISS;
                    retorno.ValorRetencaoISS = carga.ValorRetencaoISS;
                    retorno.valorFreteAPagarComICMSeISS = carga.ValorTotalAReceberComICMSeISS;
                    retorno.aliquotaICMS = repCargaPedido.BuscarMediaAliquotaICMSdaCarga(carga.Codigo);
                    retorno.csts = repCargaPedido.BuscarCSTICMSdaCarga(carga.Codigo);
                    retorno.taxaDocumentacao = Servicos.Embarcador.Carga.Frete.RetornarTaxaDocumental(carga);
                    retorno.aliquotaISS = repCargaPedido.BuscarMediaAliquotaISSdaCarga(carga.Codigo);
                    retorno.valorMercadoria = repPedidoXMLNotaFiscal.ObterValorTotalPorCarga(carga.Codigo);
                    retorno.peso = repPedidoXMLNotaFiscal.ObterPesoTotalPorCarga(carga.Codigo);
                    retorno.Moeda = carga.Moeda ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real;
                    retorno.ValorCotacaoMoeda = carga.ValorCotacaoMoeda ?? 0m;
                    retorno.ValorTotalMoeda = carga.ValorTotalMoeda ?? 0m;
                    retorno.ValorTotalMoedaPagar = carga.ValorTotalMoedaPagar ?? 0m;
                    retorno.CustoFrete = carga.DadosSumarizados?.CustoFrete ?? string.Empty;
                    retorno.PercentualBonificacaoTransportador = carga.PercentualBonificacaoTransportador;
                    retorno.DescricaoBonificacaoTransportador = (carga.PercentualBonificacaoTransportador != 0m) ? carga.BonificacaoTransportador?.ComponenteFrete?.Descricao ?? string.Empty : string.Empty;

                    freteRota.Origem = ultimaRota.Origem.DescricaoCidadeEstado;
                    freteRota.Destino = ultimaRota.Destino.DescricaoCidadeEstado;
                    freteRota.TipoCarga = carga.TipoDeCarga.Descricao;
                    freteRota.ModeloVeicularCarga = carga.ModeloVeicularCarga.Descricao;
                    freteRota.ValorFrete = carga.ValorFrete;
                    freteRota.ValorFreteOperador = carga.ValorFreteOperador;
                    freteRota.ValorFreteLeilao = carga.ValorFreteLeilao;
                    freteRota.ValorFreteAPagar = carga.ValorFreteAPagar;
                    freteRota.ValorFreteTabelaFrete = carga.ValorFreteTabelaFrete;

                    ComponetesFrete serComponentesFrete = new ComponetesFrete(unitOfWork);
                    serComponentesFrete.BuscarComponentesDeFreteDaCarga(ref retorno, carga, calculoFreteFilialEmissora, unitOfWork, tipoServicoMultisoftware);
                }
                else
                {
                    carga.ValorFreteAPagar = 0;

                    if (freteRota.situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoFreteRota.MaisQueUmaTabelaParaRota)
                    {
                        carga.MotivoPendenciaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete.AgOperador;
                    }
                    else
                    {
                        carga.MotivoPendenciaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete.ProblemaCalculoFrete;
                    }
                    retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete;
                    freteRota.rotaSemFrete = new
                    {
                        ultimaRota.Codigo,
                        Destino = new { ultimaRota.Destino.Codigo, Descricao = ultimaRota.Destino.DescricaoCidadeEstado },
                        Origem = new { ultimaRota.Origem.Codigo, Descricao = ultimaRota.Origem.DescricaoCidadeEstado }
                    };
                }
            }
            else
            {
                freteRota = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteRota();
                freteRota.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoFreteRota.TabelaNaoEncontrada;
            }

            retorno.dadosRetornoTipoFrete = freteRota;

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.FreteRota ObterFretePorRota(Dominio.Entidades.Embarcador.Logistica.Rota rota, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, bool apenasVerificar, bool calculoFreteFilialEmissora, Repositorio.UnitOfWork unitOfWork)
        {

            Dominio.ObjetosDeValor.Embarcador.Frete.FreteRota dynFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteRota();
            Repositorio.Embarcador.Frete.TabelaFreteRota repTabelaFreteRota = new Repositorio.Embarcador.Frete.TabelaFreteRota(unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteRotaTipoCarga repTabelaFreteRotaTipoCarga = new Repositorio.Embarcador.Frete.TabelaFreteRotaTipoCarga(unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga repTabelaFreteRotaTipoCargaModeloVeicularCarga = new Repositorio.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteRota cargaTabelaFreteRota = null;

            Repositorio.Embarcador.Cargas.CargaTabelaFreteRota repCargaTabelaFreteRota = new Repositorio.Embarcador.Cargas.CargaTabelaFreteRota(unitOfWork);

            cargaTabelaFreteRota = repCargaTabelaFreteRota.BuscarPorCarga(carga.Codigo, calculoFreteFilialEmissora);

            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteRota> tabelasRota;
            if (cargaTabelaFreteRota == null)
            {
                tabelasRota = new List<Dominio.Entidades.Embarcador.Frete.TabelaFreteRota>();
                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteRota> tempTabelaRota = repTabelaFreteRota.BuscarPorOrigemDestino(tabelaFrete.Codigo, rota.Origem.Codigo, rota.Destino.Codigo);
                foreach (Dominio.Entidades.Embarcador.Frete.TabelaFreteRota tabelaRota in tempTabelaRota)
                {
                    Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCarga tabelaFreteRotaTipoCarga = repTabelaFreteRotaTipoCarga.BuscarPorTabelaTipoCarga(tabelaRota.Codigo, carga.TipoDeCarga?.Codigo ?? 0);
                    if (tabelaFreteRotaTipoCarga != null)
                    {
                        tabelasRota.Add(tabelaRota);
                    }
                }

                if (tabelasRota.Count == 1)
                {
                    cargaTabelaFreteRota = new Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteRota();
                    cargaTabelaFreteRota.Carga = carga;
                    cargaTabelaFreteRota.TabelaFreteFilialEmissora = calculoFreteFilialEmissora;
                    cargaTabelaFreteRota.TabelaFreteRota = tabelasRota[0];
                    repCargaTabelaFreteRota.Inserir(cargaTabelaFreteRota);
                    if (!calculoFreteFilialEmissora)
                        carga.TabelaFrete = cargaTabelaFreteRota.TabelaFreteRota.TabelaFrete;
                    else
                        carga.TabelaFreteFilialEmissora = cargaTabelaFreteRota.TabelaFreteRota.TabelaFrete;

                    if (!apenasVerificar)
                    {
                        Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                        repCarga.Atualizar(carga);
                    }
                }
            }
            else
            {
                tabelasRota = new List<Dominio.Entidades.Embarcador.Frete.TabelaFreteRota>();
                tabelasRota.Add(cargaTabelaFreteRota.TabelaFreteRota);
            }

            if (tabelasRota.Count == 1)
            {

                if (cargaTabelaFreteRota.TabelaFreteRotaTipoCargaModeloVeicularCarga == null)
                {
                    Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCarga tabelaFreteRotaTipoCarga = repTabelaFreteRotaTipoCarga.BuscarPorTabelaTipoCarga(tabelasRota[0].Codigo, carga.TipoDeCarga.Codigo);
                    if (tabelaFreteRotaTipoCarga != null)
                    {
                        Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga freteTipoCargaModeloVeicular = repTabelaFreteRotaTipoCargaModeloVeicularCarga.BuscarPorTipoCargaModeloVeicular(tabelaFreteRotaTipoCarga.Codigo, carga.ModeloVeicularCarga.Codigo);
                        if (freteTipoCargaModeloVeicular != null)
                        {
                            if (!apenasVerificar)
                            {
                                cargaTabelaFreteRota.TabelaFreteRotaTipoCargaModeloVeicularCarga = freteTipoCargaModeloVeicular;
                                repCargaTabelaFreteRota.Atualizar(cargaTabelaFreteRota);
                            }

                            dynFrete.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoFreteRota.FreteValido;
                            dynFrete.ValorFrete = freteTipoCargaModeloVeicular.ValorFrete;
                            dynFrete.ValorPedagio = freteTipoCargaModeloVeicular.ValorPedagio;
                            dynFrete.Tabela = Servicos.Embarcador.Carga.Frete.DescricaoTabelaFrete(freteTipoCargaModeloVeicular.TabelaFreteRotaTipoCarga.TabelaFreteRota.TabelaFrete);
                            dynFrete.Codigo = !string.IsNullOrWhiteSpace(cargaTabelaFreteRota.TabelaFreteRota.CodigoEmbarcador) ? cargaTabelaFreteRota.TabelaFreteRota.CodigoEmbarcador : "";
                            dynFrete.DescricaoDestinos = !string.IsNullOrWhiteSpace(cargaTabelaFreteRota.TabelaFreteRota.DescricaoDestinos) ? cargaTabelaFreteRota.TabelaFreteRota.DescricaoDestinos : "";
                        }
                        else
                        {
                            dynFrete.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoFreteRota.NaoEncontrouModeloVeicularCarga;
                        }
                    }
                    else
                    {
                        dynFrete.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoFreteRota.NaoEncontrouTipoCarga;
                    }
                }
                else
                {
                    dynFrete.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoFreteRota.FreteValido;
                    dynFrete.ValorFrete = cargaTabelaFreteRota.TabelaFreteRotaTipoCargaModeloVeicularCarga.ValorFrete;
                    dynFrete.ValorPedagio = cargaTabelaFreteRota.TabelaFreteRotaTipoCargaModeloVeicularCarga.ValorPedagio;
                    dynFrete.Tabela = Servicos.Embarcador.Carga.Frete.DescricaoTabelaFrete(cargaTabelaFreteRota.TabelaFreteRotaTipoCargaModeloVeicularCarga.TabelaFreteRotaTipoCarga.TabelaFreteRota.TabelaFrete);
                    dynFrete.Codigo = cargaTabelaFreteRota.TabelaFreteRota.CodigoEmbarcador;
                    dynFrete.DescricaoDestinos = cargaTabelaFreteRota.TabelaFreteRota.DescricaoDestinos;
                }
            }
            else
            {
                if (tabelasRota.Count == 0)
                {
                    dynFrete.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoFreteRota.TabelaNaoEncontrada;
                }
                else
                {
                    dynFrete.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoFreteRota.MaisQueUmaTabelaParaRota;
                    dynFrete.tabelas = from obj in tabelasRota
                                       select new
                                       {
                                           obj.Codigo,
                                           Destino = new
                                           {
                                               obj.Destino.Codigo,
                                               obj.Destino.Descricao
                                           },
                                           Origem = new
                                           {
                                               obj.Origem.Codigo,
                                               obj.Origem.Descricao
                                           },
                                           DescricaoDestinos = !string.IsNullOrWhiteSpace(obj.DescricaoDestinos) ? obj.CodigoEmbarcador + " - " + obj.DescricaoDestinos : obj.CodigoEmbarcador
                                       };
                }
            }
            return dynFrete;
        }
    }
}
