using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.Database;
using Dominio.ObjetosDeValor.Embarcador.Carga.OcultarInformacoesCarga;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Carga
{
    public class ComponetesFrete : ServicoBase
    {                
        public ComponetesFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public static List<Dominio.Entidades.Embarcador.Pessoas.ClienteComponente> ObterComponentesCliente(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Pessoas.ClienteComponente> listaComponentesCliente = new List<Dominio.Entidades.Embarcador.Pessoas.ClienteComponente>();
            if (configuracao.UtilizarComponentesCliente && carga != null)
            {
                Repositorio.Embarcador.Pessoas.ClienteComponente repClienteComponente = new Repositorio.Embarcador.Pessoas.ClienteComponente(unitOfWork);

                List<double> destinatario = (from obj in cargaPedidos where obj.Recebedor == null select obj.Pedido.Destinatario.CPF_CNPJ).Distinct().ToList();
                destinatario.AddRange((from obj in cargaPedidos where obj.Recebedor != null select obj.Recebedor.CPF_CNPJ).Distinct().ToList());
                listaComponentesCliente = repClienteComponente.BuscarComponentesPorClienteFilialEmpresa(destinatario.Distinct().ToList(), carga.Filial.Codigo, carga.Empresa?.Codigo ?? 0);
            }
            return listaComponentesCliente;
        }

        public void BuscarComponentesDeFreteDaCarga(ref Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool calculoFreteFilialEmissora, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool possuiOcultarInformacoesCarga = false, Dominio.Entidades.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga ocultarInformacoesCarga = null)
        {
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponenteFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.StageAgrupamento repStageAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(unitOfWork);
            Repositorio.Embarcador.Pedidos.StageAgrupamentoComposicaoFrete repStageAgrupamentoComposicao = new Repositorio.Embarcador.Pedidos.StageAgrupamentoComposicaoFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.Stage repStage = new Repositorio.Embarcador.Pedidos.Stage(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repcargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            Servicos.Embarcador.Carga.OcultarInformacoesCarga.OcultarInformacoesCarga serOcultarInformacoesCarga = new OcultarInformacoesCarga.OcultarInformacoesCarga(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork).BuscarConfiguracaoPadrao();
            List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFreteSumarizados = new List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFrete = repCargaComponenteFrete.BuscarPorCargaComImpostosSemComponenteFreteLiquido(carga.Codigo, calculoFreteFilialEmissora);

            decimal valorTotalNotasFiscaisSemPallets = repPedidoXMLNotaFiscal.ObterValorTotalPorCarga(carga.Codigo, true);

            if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete> tiposComponentes = (from obj in cargaComponentesFrete select obj.TipoComponenteFrete).Distinct().ToList();
                foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponente in tiposComponentes)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> componenteFreteTipoComponente = (from obj in cargaComponentesFrete where obj.TipoComponenteFrete == tipoComponente select obj).ToList();
                    List<Dominio.Entidades.Embarcador.Frete.ComponenteFrete> componenentesFrete = (from obj in componenteFreteTipoComponente select obj.ComponenteFrete).Distinct().ToList();

                    foreach (Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenenteFrete in componenentesFrete)
                    {
                        decimal valorComponente = (from obj in cargaComponentesFrete where obj.TipoComponenteFrete == tipoComponente && obj.ComponenteFrete == componenenteFrete select obj.ValorComponente).Sum();

                        if ((componenenteFrete?.NaoDeveIncidirSobreNotasFiscaisPateles ?? false) && configuracaoEmbarcador.UtilizaEmissaoMultimodal)
                            valorComponente = valorTotalNotasFiscaisSemPallets * ((from obj in cargaComponentesFrete where obj.TipoComponenteFrete == tipoComponente && obj.ComponenteFrete == componenenteFrete select obj.Percentual).FirstOrDefault() / 100);

                        Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete componenteFreteSumarizado = new Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete();
                        componenteFreteSumarizado.TipoComponenteFrete = tipoComponente;
                        componenteFreteSumarizado.ComponenteFrete = componenenteFrete;

                        Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponentesFreteAgrupado = (from obj in cargaComponentesFrete where obj.TipoComponenteFrete == tipoComponente && obj.ComponenteFrete == componenenteFrete select obj).FirstOrDefault();
                        componenteFreteSumarizado.ModeloDocumentoFiscal = cargaComponentesFreteAgrupado.ModeloDocumentoFiscal;
                        componenteFreteSumarizado.TipoValor = cargaComponentesFreteAgrupado.TipoValor;
                        componenteFreteSumarizado.Percentual = cargaComponentesFreteAgrupado.Percentual;
                        componenteFreteSumarizado.DescontarValorTotalAReceber = cargaComponentesFreteAgrupado.DescontarValorTotalAReceber;
                        componenteFreteSumarizado.AcrescentaValorTotalAReceber = cargaComponentesFreteAgrupado.AcrescentaValorTotalAReceber;
                        componenteFreteSumarizado.NaoSomarValorTotalAReceber = cargaComponentesFreteAgrupado.NaoSomarValorTotalAReceber;
                        componenteFreteSumarizado.NaoSomarValorTotalPrestacao = cargaComponentesFreteAgrupado.NaoSomarValorTotalPrestacao;
                        componenteFreteSumarizado.Tipo = cargaComponentesFreteAgrupado.Tipo;
                        componenteFreteSumarizado.DescontarComponenteFreteLiquido = cargaComponentesFreteAgrupado.DescontarComponenteFreteLiquido;

                        Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponenteInformadoManualmente = (from obj in cargaComponentesFrete where obj.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.Manual && obj.TipoComponenteFrete == tipoComponente && obj.ComponenteFrete == componenenteFrete select obj).FirstOrDefault();
                        if (cargaComponenteInformadoManualmente != null)
                        {
                            componenteFreteSumarizado.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.Manual;
                            componenteFreteSumarizado.UsuarioInformou = cargaComponenteInformadoManualmente.UsuarioInformou;
                        }

                        componenteFreteSumarizado.ValorTotalMoeda = cargaComponentesFrete.Where(o => o.TipoComponenteFrete == tipoComponente && o.ComponenteFrete == componenenteFrete).Sum(o => o.ValorTotalMoeda ?? 0m);
                        componenteFreteSumarizado.ValorCotacaoMoeda = cargaComponentesFreteAgrupado.ValorCotacaoMoeda ?? 0m;
                        componenteFreteSumarizado.Moeda = cargaComponentesFreteAgrupado.Moeda ?? MoedaCotacaoBancoCentral.Real;
                        componenteFreteSumarizado.ValorTotalMoeda = cargaComponentesFrete.Where(o => o.TipoComponenteFrete == tipoComponente && o.ComponenteFrete == componenenteFrete).Sum(o => o.ValorTotalMoeda ?? 0m);
                        componenteFreteSumarizado.ValorComponente = possuiOcultarInformacoesCarga ? serOcultarInformacoesCarga.ValidarOcultarValor(ocultarInformacoesCarga, TipoValorOcultarInformacoesCarga.ValorFrete, valorComponente) : valorComponente;
                        cargaComponentesFreteSumarizados.Add(componenteFreteSumarizado);
                    }
                }
            }
            else
            {
                cargaComponentesFreteSumarizados = cargaComponentesFrete;
            }

            retorno.componentesFrete = (from obj in cargaComponentesFreteSumarizados
                                        select new
                                        {
                                            Codigo = obj.Codigo,
                                            ComponenteFrete = obj.ComponenteFrete != null ? obj.ComponenteFrete.Codigo : 0,
                                            DescricaoComponente = obj.ComponenteFrete != null ? obj.ComponenteFrete.Descricao : "",
                                            obj.TipoComponenteFrete,
                                            obj.Tipo,
                                            obj.DescontarValorTotalAReceber,
                                            obj.AcrescentaValorTotalAReceber,
                                            obj.NaoSomarValorTotalAReceber,
                                            obj.NaoSomarValorTotalPrestacao,
                                            CobrarOutroDocumento = obj.ModeloDocumentoFiscal != null ? true : false,
                                            AbreviacaoDocumentoFiscal = obj.ModeloDocumentoFiscal != null ? obj.ModeloDocumentoFiscal.Abreviacao : "CT-e",
                                            DescricaoModeloDocumentoFiscal = obj.ModeloDocumentoFiscal != null ? obj.ModeloDocumentoFiscal.Descricao : "",
                                            CodigoModeloDocumentoFiscal = obj.ModeloDocumentoFiscal != null ? obj.ModeloDocumentoFiscal.Codigo : 0,
                                            UsuarioInformou = obj.UsuarioInformou != null ? new { Descricao = obj.UsuarioInformou.Nome, obj.UsuarioInformou.Codigo } : null,
                                            DescricaoInformadoManualmente = carga.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador ? "Valor do CT-e emitido pelo embarcador" : (obj.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.Manual ? (obj.UsuarioInformou?.Nome ?? "") : obj.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.Ocorrencia ? "Valor de ocorrências" : "Valor da tabela de frete"),
                                            Descricao = carga.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador && !string.IsNullOrWhiteSpace(obj.OutraDescricaoCTe) ? obj.OutraDescricaoCTe : (obj.ComponenteFrete != null ? obj.ComponenteFrete.Descricao : obj.DescricaoComponente),
                                            Valor = obj.ValorComponente,
                                            obj.Moeda,
                                            obj.ValorCotacaoMoeda,
                                            obj.ValorTotalMoeda,
                                            Percentual = obj.Percentual,
                                            obj.TipoValor,
                                            DT_RowColor = obj.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.INCONSISTENTE ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Vermelho : "",
                                            DT_FontColor = obj.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.INCONSISTENTE ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Branco : "",
                                        }).ToList();


            Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete repCargaComposicaoFrete = new Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaTabelaFreteCliente repCargaTabelaFreteCliente = new Repositorio.Embarcador.Cargas.CargaTabelaFreteCliente(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoTabelaFreteCliente repCargaPedidoTabelaFreteCliente = new Repositorio.Embarcador.Cargas.CargaPedidoTabelaFreteCliente(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete> cargaComposicaoFretes = repCargaComposicaoFrete.BuscarPorCarga(carga.Codigo, calculoFreteFilialEmissora);
            if (carga.CargaAgrupada && carga.AgrupadaPosEmissaoDocumento)
            {
                retorno.AgrupadaPosEmissaoDocumento = carga.AgrupadaPosEmissaoDocumento;


                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasAgrupadas = repCarga.BuscarCargasOriginais(carga.Codigo);
                List<int> codigosCargasAgrupadas = new List<int>();
                retorno.cargas = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ListaCargasRetornoFrete>();

                foreach (var cargaAgrupada in cargasAgrupadas)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frete.ListaCargasRetornoFrete cargaRetorno = new Dominio.ObjetosDeValor.Embarcador.Frete.ListaCargasRetornoFrete();
                    cargaRetorno.Codigo = cargaAgrupada.Codigo;
                    cargaRetorno.CodigoCargaEmbarcador = cargaAgrupada.CodigoCargaEmbarcador;
                    retorno.cargas.Add(cargaRetorno);

                    codigosCargasAgrupadas.Add(cargaAgrupada.Codigo);
                }

                cargaComposicaoFretes = repCargaComposicaoFrete.BuscarPorCargaAgrupada(codigosCargasAgrupadas, calculoFreteFilialEmissora);
            }

            Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = null;
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente> cargaPedidoTabelaFreteCliente = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente>();

            if (cargaComposicaoFretes.Where(obj => obj.CargaPedidos?.Any() ?? false).Any())
                cargaPedidoTabelaFreteCliente = repCargaPedidoTabelaFreteCliente.BuscarPorCargaPedido(cargaComposicaoFretes.Select(o => o.CargaPedidos?.FirstOrDefault()?.Codigo ?? 0).ToList(), calculoFreteFilialEmissora);
            else if (cargaComposicaoFretes.Where(obj => !(obj.PedidoCTesParaSubContratacao?.Any() ?? false) && !(obj.PedidoXMLNotasFiscais?.Any() ?? false) && !(obj.CargaPedidos?.Any() ?? false)).Any())
                tabelaFreteCliente = repCargaTabelaFreteCliente.BuscarPorCarga(carga.Codigo, calculoFreteFilialEmissora)?.TabelaFreteCliente ?? null;

            if (carga.TipoOperacao != null && carga.TipoOperacao.TipoConsolidacao == EnumTipoConsolidacao.AutorizacaoEmissao)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> stagesAgrupamentoDT = repStageAgrupamento.BuscarPorCargaDt(carga.Codigo);

                List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamentoComposicaoFrete> stagesComposicaoFrete = repStageAgrupamentoComposicao.BuscarPorCodigosAgrupamentos(stagesAgrupamentoDT.Select(x => x.Codigo).ToList());

                retorno.ComposicaoFreteStage = (from obj in stagesComposicaoFrete
                                                group obj by new { Numero = string.Join(", ", repStage.ObterNumerosStagesPorAgrupamento(obj.StageAgrupamento.Codigo)), Codigo = obj.StageAgrupamento.Codigo }
                                                into grupo
                                                select new
                                                {
                                                    Numero = grupo.Key.Numero,
                                                    ComposicaoFrete = grupo.Select(composicao => new
                                                    {
                                                        composicao.Formula,
                                                        composicao.ValoresFormula,
                                                        composicao.Descricao,
                                                        composicao.TipoCampoValor,
                                                        composicao.TipoParametro,
                                                        DescricaoTipoCampoValor = composicao.TipoCampoValor.ObterDescricao(),
                                                        Valor = composicao.Valor.ToString("n6"),
                                                        ValorCalculado = composicao.ValorCalculado.ToString("n2")
                                                    }).ToList()
                                                }).ToList();

            }


            retorno.ComposicaoFretePedido = (from obj in cargaComposicaoFretes
                                             where obj.CargaPedidos?.Count > 0
                                             group obj by new
                                             {
                                                 Numero = string.Join(", ", obj.CargaPedidos.Select(cargaPedido => tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? cargaPedido.Pedido.Numero + " - " + cargaPedido.Pedido.NumeroPedidoEmbarcador : cargaPedido.Pedido.NumeroPedidoEmbarcador)),
                                                 Codigo = obj.CargaPedidos.FirstOrDefault()?.Codigo
                                             }
                                             into grupo
                                             select new
                                             {
                                                 Numero = grupo.Key.Numero,
                                                 ComposicaoFrete = grupo.Select(composicao => new
                                                 {
                                                     composicao.Formula,
                                                     composicao.ValoresFormula,
                                                     composicao.Descricao,
                                                     composicao.TipoCampoValor,
                                                     composicao.TipoParametro,
                                                     DescricaoTipoCampoValor = composicao.TipoCampoValor.ObterDescricao(),
                                                     Valor = composicao.Valor.ToString("n6"),
                                                     ValorCalculado = composicao.ValorCalculado.ToString("n2"),
                                                     CodigoTabela = cargaPedidoTabelaFreteCliente.Where(c => c.CargaPedido.Codigo == grupo.Key.Codigo)?.FirstOrDefault()?.TabelaFreteCliente?.CodigoIntegracao ?? string.Empty,
                                                     Origem = cargaPedidoTabelaFreteCliente.Where(c => c.CargaPedido.Codigo == grupo.Key.Codigo)?.FirstOrDefault()?.TabelaFreteCliente?.DescricaoOrigem ?? string.Empty,
                                                     Destino = cargaPedidoTabelaFreteCliente.Where(c => c.CargaPedido.Codigo == grupo.Key.Codigo)?.FirstOrDefault()?.CargaPedido?.Destino?.DescricaoCidadeEstado ?? string.Empty,
                                                     Cliente = cargaPedidoTabelaFreteCliente.Where(c => c.CargaPedido.Codigo == grupo.Key.Codigo)?.FirstOrDefault()?.CargaPedido?.Pedido?.Destinatario?.Descricao ?? string.Empty
                                                 })
                                             }).ToList();

            retorno.ComposicaoFreteDocumento = new
            {
                CTesParaSubcontratacao = (from obj in cargaComposicaoFretes
                                          where obj.PedidoCTesParaSubContratacao?.Count > 0
                                          group obj by new { Numero = string.Join(", ", obj.PedidoCTesParaSubContratacao.Select(o => o.CTeTerceiro.Numero + "-" + o.CTeTerceiro.Serie)), Codigos = string.Join(", ", obj.PedidoCTesParaSubContratacao.Select(o => o.Codigo)) }
                                          into grupo
                                          select new
                                          {
                                              Numero = grupo.Key.Numero,
                                              ComposicaoFrete = grupo.Select(composicao => new
                                              {
                                                  composicao.Formula,
                                                  composicao.ValoresFormula,
                                                  composicao.Descricao,
                                                  composicao.TipoCampoValor,
                                                  composicao.TipoParametro,
                                                  DescricaoTipoCampoValor = composicao.TipoCampoValor.ObterDescricao(),
                                                  Valor = composicao.Valor.ToString("n6"),
                                                  ValorCalculado = composicao.ValorCalculado.ToString("n2")
                                              })
                                          }).ToList(),
                NotasFiscais = (from obj in cargaComposicaoFretes
                                where obj.PedidoXMLNotasFiscais?.Count > 0
                                group obj by new { Numero = string.Join(", ", obj.PedidoXMLNotasFiscais.Select(o => o.XMLNotaFiscal.Numero)), Codigos = string.Join(", ", obj.PedidoXMLNotasFiscais.Select(o => o.Codigo)) }
                                into grupo
                                select new
                                {
                                    Numero = grupo.Key.Numero,
                                    ComposicaoFrete = grupo.Select(composicao => new
                                    {
                                        composicao.Formula,
                                        composicao.ValoresFormula,
                                        composicao.Descricao,
                                        composicao.TipoCampoValor,
                                        composicao.TipoParametro,
                                        ValorCalculadoPorTaxa = composicao.ValorCalculado * composicao.Valor,
                                        DescricaoTipoCampoValor = composicao.TipoCampoValor.ObterDescricao(),
                                        Valor = composicao.Valor.ToString("n6"),
                                        ValorCalculado = composicao.ValorCalculado.ToString("n2")
                                    })
                                }).ToList()
            };

            retorno.ComposicaoFreteCarga = (from obj in cargaComposicaoFretes
                                            where
                                            obj.PedidoCTesParaSubContratacao?.Count == 0 &&
                                            obj.PedidoXMLNotasFiscais?.Count == 0 &&
                                            obj.CargaPedidos?.Count == 0
                                            select new
                                            {
                                                obj.Carga.CodigoCargaEmbarcador,
                                                obj.Formula,
                                                ValoresFormula = possuiOcultarInformacoesCarga ? serOcultarInformacoesCarga.ValidarOcultarValor(ocultarInformacoesCarga, TipoValorOcultarInformacoesCarga.ValorFrete, obj.ValoresFormula) : obj.ValoresFormula,
                                                obj.Descricao,
                                                obj.TipoCampoValor,
                                                obj.TipoParametro,
                                                DescricaoTipoCampoValor = obj.TipoCampoValor.ObterDescricao(),
                                                Valor = possuiOcultarInformacoesCarga ? serOcultarInformacoesCarga.ValidarOcultarValor(ocultarInformacoesCarga, TipoValorOcultarInformacoesCarga.ValorFrete, obj.Valor.ToString("n6")) : obj.Valor.ToString("n6"),
                                                ValorCalculado = possuiOcultarInformacoesCarga ? serOcultarInformacoesCarga.ValidarOcultarValor(ocultarInformacoesCarga, TipoValorOcultarInformacoesCarga.ValorFrete, obj.ValorCalculado.ToString("n2")) : obj.ValorCalculado.ToString("n2"),
                                                CodigoTabela = tabelaFreteCliente?.CodigoIntegracao ?? string.Empty,
                                                Origem = tabelaFreteCliente?.DescricaoOrigem ?? string.Empty,
                                                Destino = tabelaFreteCliente?.DescricaoDestino ?? string.Empty
                                            }).ToList();


            if (carga.DadosSumarizados?.CargaTrecho == CargaTrechoSumarizada.Agrupadora && carga.TipoOperacao != null && carga.TipoOperacao.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn)
            {
                // buscar as composicoes das cargas filho
                List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> stagesAgrupamentoDT = repStageAgrupamento.BuscarPorCargaDt(carga.Codigo);
                cargaPedidoTabelaFreteCliente = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente>();
                cargaComposicaoFretes = new List<Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete>();

                foreach (var agrupamento in stagesAgrupamentoDT)
                    cargaComposicaoFretes.AddRange(repCargaComposicaoFrete.BuscarPorCarga(agrupamento.CargaGerada?.Codigo ?? 0, calculoFreteFilialEmissora));

                if (cargaComposicaoFretes.Where(obj => obj.CargaPedidos?.Any() ?? false).Any())
                    cargaPedidoTabelaFreteCliente.AddRange(repCargaPedidoTabelaFreteCliente.BuscarPorCargaPedido(cargaComposicaoFretes.Select(o => o.CargaPedidos?.FirstOrDefault()?.Codigo ?? 0).ToList(), calculoFreteFilialEmissora));

                foreach (var composicaoFreteSemCargaPedido in cargaComposicaoFretes.Where(x => x.CargaPedidos.Count == 0))
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente cptabelafrete = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente();
                    cptabelafrete.CargaPedido = repcargaPedido.BuscarPrimeiraPorCarga(composicaoFreteSemCargaPedido.Carga.Codigo);
                    cargaPedidoTabelaFreteCliente.Add(cptabelafrete);
                }

                retorno.ComposicaoFreteCargaSubTrecho = new
                {
                    CargasSubTrecho = (from obj in cargaComposicaoFretes
                                       where obj.CargaPedidos?.Count >= 0
                                       group obj by new
                                       {
                                           Numero = string.Join(", ", "Carga: " + obj.Carga.CodigoCargaEmbarcador + " - " + obj.Carga.DadosSumarizados.Origens + " - " + obj.Carga.DadosSumarizados.Destinos),
                                           Codigo = obj.Carga.Codigo
                                       }
                                 into grupo
                                       select new
                                       {
                                           Numero = grupo.Key.Numero,
                                           ComposicaoFrete = grupo.Select(composicao => new
                                           {
                                               composicao.Formula,
                                               composicao.ValoresFormula,
                                               composicao.Descricao,
                                               composicao.TipoCampoValor,
                                               composicao.TipoParametro,
                                               DescricaoTipoCampoValor = composicao.TipoCampoValor.ObterDescricao(),
                                               Valor = composicao.Valor.ToString("n6"),
                                               ValorCalculado = composicao.ValorCalculado.ToString("n2"),
                                               Remetente = composicao.CargaPedidos?.FirstOrDefault()?.Pedido?.Remetente?.Descricao ?? string.Empty,
                                               CodigoTabela = cargaPedidoTabelaFreteCliente.Where(c => c.CargaPedido.Carga.Codigo == grupo.Key.Codigo)?.FirstOrDefault()?.TabelaFreteCliente?.CodigoIntegracao ?? string.Empty,
                                               Origem = cargaPedidoTabelaFreteCliente.Where(c => c.CargaPedido.Carga.Codigo == grupo.Key.Codigo)?.FirstOrDefault()?.TabelaFreteCliente?.DescricaoOrigem ?? string.Empty,
                                               Destino = cargaPedidoTabelaFreteCliente.Where(c => c.CargaPedido.Carga.Codigo == grupo.Key.Codigo)?.FirstOrDefault()?.CargaPedido?.Destino?.DescricaoCidadeEstado ?? string.Empty,
                                               Cliente = cargaPedidoTabelaFreteCliente.Where(c => c.CargaPedido.Carga.Codigo == grupo.Key.Codigo)?.FirstOrDefault()?.CargaPedido?.Pedido?.Destinatario?.Descricao ?? string.Empty
                                           })
                                       }).ToList()
                };

                retorno.ComposicaoFretePedido = null;

            }
        }

        public void AdicionarCargaComplementoFretePorTipo(Dominio.Entidades.Embarcador.Cargas.Carga carga, decimal valorComponente, decimal percentual, string ufOrigem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete TipoValor, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComplementoFrete, bool componenteFilialEmissora, Repositorio.UnitOfWork unitOfWork)
        {
            if (valorComponente > 0)
            {
                Servicos.Embarcador.Carga.ICMS serICMS = new ICMS(unitOfWork);
                Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(tipoComplementoFrete);
                Dominio.Entidades.Empresa empresa = carga.EmpresaFilialEmissora ?? carga.Empresa;
                Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete();

                cargaComponenteFrete.Carga = carga;
                cargaComponenteFrete.ComponenteFrete = componenteFrete;
                cargaComponenteFrete.TipoValor = TipoValor;
                cargaComponenteFrete.ComponenteFilialEmissora = componenteFilialEmissora;
                cargaComponenteFrete.Percentual = percentual;
                cargaComponenteFrete.IncluirBaseCalculoICMS = serICMS.IncluirComponenteFreteBaseCalculoIcms(cargaComponenteFrete.TipoComponenteFrete, empresa, ufOrigem, unitOfWork);
                cargaComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                cargaComponenteFrete.TipoComponenteFrete = tipoComplementoFrete;
                cargaComponenteFrete.ValorComponente = valorComponente;
                repCargaComponentesFrete.Inserir(cargaComponenteFrete);
            }

        }

        public void AdicionarComponenteFreteCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete, decimal valorComponente, decimal percentual, bool componenteFilialEmissora, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete TipoValor, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComplementoFrete, Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete cargaComplementoFrete, bool incluirBaseCalculoICMS, bool incluirIntegralmenteContratoFreteTerceiro, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork, bool ratearValorFreteEntrePedidos = true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.Manual, bool sempreExtornar = true, bool agruparComponentes = false, Dominio.Entidades.Cliente tomador = null, bool naoSomarComponente = false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? moeda = null, decimal valorCotacaoMoeda = 0m, decimal valorTotalMoeda = 0m)
        {
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContraatcao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Servicos.Embarcador.Moedas.Cotacao serCotacao = new Servicos.Embarcador.Moedas.Cotacao(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiraPorCarga(carga.Codigo);

            if (tomador == null)
                tomador = cargaPedido.ObterTomador();

            decimal valorCotacao = cargaPedido?.Pedido?.ValorTaxaFeeder ?? 0;
            //if (tomador != null && tomador.CPF_CNPJ > 0D)
            //    valorCotacao = serCotacao.BuscarValorCotacaoCliente(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.DolarCompra, unitOfWork, tomador.CPF_CNPJ, cargaPedido?.Pedido?.ValorTaxaFeeder ?? 0);

            Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponenteFrete = null;
            List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> listaComponentes = new List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();
            if (agruparComponentes)
            {
                listaComponentes = repCargaComponentesFrete.BuscarPorCargaPorCompomente(carga.Codigo, tipoComplementoFrete, componenteFrete);
                if (listaComponentes != null && listaComponentes.Count > 0)
                    cargaComponenteFrete = listaComponentes.FirstOrDefault();
            }

            if (cargaComponenteFrete == null)
                cargaComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete();

            //alterar o valor aqui
            if (valorComponente <= 0m && percentual > 0m && componenteFrete.TipoComponenteFrete == TipoComponenteFrete.ADVALOREM)
            {
                decimal valorMercadoria = repPedidoXMLNotaFiscal.ObterValorTotalPorCarga(carga.Codigo, componenteFrete.NaoDeveIncidirSobreNotasFiscaisPateles);
                if (valorMercadoria == 0)
                    valorMercadoria = repPedidoCTeParaSubContraatcao.BuscarValorTotalMercadoriaPorCarga(carga.Codigo);

                if (valorMercadoria > 0)
                    valorComponente = ((percentual / 100) * (valorCotacao > 0 ? (valorMercadoria * valorCotacao) : valorMercadoria));

                //Servicos.Log.TratarErro("Ad valorem com valor zerado, total mercadoria: " + valorMercadoria.ToString("n2") + ", valor componente: " + valorComponente.ToString("n2"));
            }

            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = carga.TabelaFrete;
            bool destacarComponenteTabelaFrete = Servicos.Embarcador.Carga.Frete.DestacarComponenteTabelaFrete(tabelaFrete, componenteFrete);

            cargaComponenteFrete.Carga = carga;
            cargaComponenteFrete.ComponenteFrete = componenteFrete;
            cargaComponenteFrete.DescontarValorTotalAReceber = componenteFrete?.DescontarValorTotalAReceber ?? false;
            cargaComponenteFrete.AcrescentaValorTotalAReceber = componenteFrete?.AcrescentaValorTotalAReceber ?? false;
            cargaComponenteFrete.SomarComponenteFreteLiquido = componenteFrete?.SomarComponenteFreteLiquido ?? false;
            cargaComponenteFrete.DescontarComponenteFreteLiquido = (destacarComponenteTabelaFrete ? tabelaFrete?.DescontarComponenteFreteLiquido : componenteFrete?.DescontarComponenteFreteLiquido) ?? false;
            cargaComponenteFrete.NaoSomarValorTotalAReceber = (destacarComponenteTabelaFrete ? tabelaFrete?.NaoSomarValorTotalAReceber : componenteFrete?.NaoSomarValorTotalAReceber) ?? false;
            cargaComponenteFrete.NaoSomarValorTotalPrestacao = (destacarComponenteTabelaFrete ? tabelaFrete?.NaoSomarValorTotalPrestacao : componenteFrete?.NaoSomarValorTotalPrestacao) ?? false;
            cargaComponenteFrete.TipoComponenteFrete = tipoComplementoFrete;
            cargaComponenteFrete.ValorComponente = cargaComponenteFrete.Codigo == 0 || naoSomarComponente ? valorComponente : cargaComponenteFrete.ValorComponente + valorComponente;
            cargaComponenteFrete.ComponenteFilialEmissora = componenteFilialEmissora;
            cargaComponenteFrete.CargaComplementoFrete = cargaComplementoFrete;
            cargaComponenteFrete.TipoValor = TipoValor;

            SetarConfiguracoesComponente(ref cargaComponenteFrete, carga, tomador, unitOfWork);

            cargaComponenteFrete.ModeloDocumentoFiscal = modeloDocumentoFiscal;

            if (modeloDocumentoFiscal != null && modeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe && modeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe)
                cargaComponenteFrete.IncluirBaseCalculoICMS = false;
            else
                cargaComponenteFrete.IncluirBaseCalculoICMS = incluirBaseCalculoICMS;

            cargaComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = incluirIntegralmenteContratoFreteTerceiro;
            cargaComponenteFrete.SempreExtornar = sempreExtornar;
            cargaComponenteFrete.Percentual = percentual;
            cargaComponenteFrete.Tipo = tipo;
            cargaComponenteFrete.UsuarioInformou = usuario;
            cargaComponenteFrete.Moeda = moeda;
            cargaComponenteFrete.ValorCotacaoMoeda = valorCotacaoMoeda;
            cargaComponenteFrete.ValorTotalMoeda = valorTotalMoeda;

            if (cargaComponenteFrete.Codigo == 0)
                repCargaComponentesFrete.Inserir(cargaComponenteFrete);
            else
                repCargaComponentesFrete.Atualizar(cargaComponenteFrete);

            if (ratearValorFreteEntrePedidos)
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configEmbarcador = repConfiguracao.BuscarConfiguracaoPadrao();

                Servicos.Embarcador.Carga.RateioFrete serCargaFreteRateio = new RateioFrete(unitOfWork);
                serCargaFreteRateio.RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, configEmbarcador, componenteFilialEmissora, unitOfWork, tipoServicoMultisoftware);
            }
        }
        public async Task AdicionarComponenteFreteCargaAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete, decimal valorComponente, decimal percentual,
            bool componenteFilialEmissora, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete TipoValor, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComplementoFrete,
            Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete cargaComplementoFrete, bool incluirBaseCalculoICMS, bool incluirIntegralmenteContratoFreteTerceiro, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork, bool ratearValorFreteEntrePedidos = true,
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.Manual, bool sempreExtornar = true, bool agruparComponentes = false,
            Dominio.Entidades.Cliente tomador = null, bool naoSomarComponente = false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? moeda = null, decimal valorCotacaoMoeda = 0m, decimal valorTotalMoeda = 0m)
        {
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContraatcao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Servicos.Embarcador.Moedas.Cotacao serCotacao = new Servicos.Embarcador.Moedas.Cotacao(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = await repCargaPedido.BuscarPrimeiraPorCargaAsync(carga.Codigo);

            if (tomador == null)
                tomador = cargaPedido.ObterTomador();

            decimal valorCotacao = cargaPedido?.Pedido?.ValorTaxaFeeder ?? 0;

            Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponenteFrete = null;
            List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> listaComponentes = new List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();
            if (agruparComponentes)
            {
                listaComponentes = await repCargaComponentesFrete.BuscarPorCargaPorCompomenteAsync(carga.Codigo, tipoComplementoFrete, componenteFrete);
                if (listaComponentes != null && listaComponentes.Count > 0)
                    cargaComponenteFrete = listaComponentes.FirstOrDefault();
            }

            if (cargaComponenteFrete == null)
                cargaComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete();

            //alterar o valor aqui
            if (valorComponente <= 0m && percentual > 0m && componenteFrete.TipoComponenteFrete == TipoComponenteFrete.ADVALOREM)
            {
                decimal valorMercadoria = await repPedidoXMLNotaFiscal.ObterValorTotalPorCargaAsync(carga.Codigo, componenteFrete.NaoDeveIncidirSobreNotasFiscaisPateles);
                if (valorMercadoria == 0)
                    valorMercadoria = await repPedidoCTeParaSubContraatcao.BuscarValorTotalMercadoriaPorCargaAsync(carga.Codigo);

                if (valorMercadoria > 0)
                    valorComponente = ((percentual / 100) * (valorCotacao > 0 ? (valorMercadoria * valorCotacao) : valorMercadoria));
            }

            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = carga.TabelaFrete;
            bool destacarComponenteTabelaFrete = await Servicos.Embarcador.Carga.Frete.DestacarComponenteTabelaFreteAsync(tabelaFrete, componenteFrete);

            cargaComponenteFrete.Carga = carga;
            cargaComponenteFrete.ComponenteFrete = componenteFrete;
            cargaComponenteFrete.DescontarValorTotalAReceber = componenteFrete?.DescontarValorTotalAReceber ?? false;
            cargaComponenteFrete.AcrescentaValorTotalAReceber = componenteFrete?.AcrescentaValorTotalAReceber ?? false;
            cargaComponenteFrete.SomarComponenteFreteLiquido = componenteFrete?.SomarComponenteFreteLiquido ?? false;
            cargaComponenteFrete.DescontarComponenteFreteLiquido = (destacarComponenteTabelaFrete ? tabelaFrete?.DescontarComponenteFreteLiquido : componenteFrete?.DescontarComponenteFreteLiquido) ?? false;
            cargaComponenteFrete.NaoSomarValorTotalAReceber = (destacarComponenteTabelaFrete ? tabelaFrete?.NaoSomarValorTotalAReceber : componenteFrete?.NaoSomarValorTotalAReceber) ?? false;
            cargaComponenteFrete.NaoSomarValorTotalPrestacao = (destacarComponenteTabelaFrete ? tabelaFrete?.NaoSomarValorTotalPrestacao : componenteFrete?.NaoSomarValorTotalPrestacao) ?? false;
            cargaComponenteFrete.TipoComponenteFrete = tipoComplementoFrete;
            cargaComponenteFrete.ValorComponente = cargaComponenteFrete.Codigo == 0 || naoSomarComponente ? valorComponente : cargaComponenteFrete.ValorComponente + valorComponente;
            cargaComponenteFrete.ComponenteFilialEmissora = componenteFilialEmissora;
            cargaComponenteFrete.CargaComplementoFrete = cargaComplementoFrete;
            cargaComponenteFrete.TipoValor = TipoValor;

            SetarConfiguracoesComponente(ref cargaComponenteFrete, carga, tomador, unitOfWork);

            cargaComponenteFrete.ModeloDocumentoFiscal = modeloDocumentoFiscal;

            if (modeloDocumentoFiscal != null && modeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe && modeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe)
                cargaComponenteFrete.IncluirBaseCalculoICMS = false;
            else
                cargaComponenteFrete.IncluirBaseCalculoICMS = incluirBaseCalculoICMS;

            cargaComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = incluirIntegralmenteContratoFreteTerceiro;
            cargaComponenteFrete.SempreExtornar = sempreExtornar;
            cargaComponenteFrete.Percentual = percentual;
            cargaComponenteFrete.Tipo = tipo;
            cargaComponenteFrete.UsuarioInformou = usuario;
            cargaComponenteFrete.Moeda = moeda;
            cargaComponenteFrete.ValorCotacaoMoeda = valorCotacaoMoeda;
            cargaComponenteFrete.ValorTotalMoeda = valorTotalMoeda;

            if (cargaComponenteFrete.Codigo == 0)
                await repCargaComponentesFrete.InserirAsync(cargaComponenteFrete);
            else
                await repCargaComponentesFrete.AtualizarAsync(cargaComponenteFrete);

            if (ratearValorFreteEntrePedidos)
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = await repCargaPedido.BuscarPorCargaAsync(carga.Codigo);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configEmbarcador = await repConfiguracao.BuscarConfiguracaoPadraoAsync();

                Servicos.Embarcador.Carga.RateioFrete serCargaFreteRateio = new RateioFrete(unitOfWork);
                serCargaFreteRateio.RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, configEmbarcador, componenteFilialEmissora, unitOfWork, tipoServicoMultisoftware);
            }
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete AdicionarComponenteFreteCargaJanelaCarregamentoTransportador(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, int codigoComponenteFrete, int codigoModeloDocumentoFiscal, bool cobrarOutroDocumento, decimal percentual, decimal valorComponente, decimal valorSugerido, decimal valorTotalMoeda, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete repositorioCargaJanelaCarregamentoTransportadorComponenteFrete = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repositorioPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repositorioComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repositorioModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga;
            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repositorioComponenteFrete.BuscarPorCodigo(codigoComponenteFrete);

            if (componenteFrete.TipoComponenteFrete == TipoComponenteFrete.ICMS || componenteFrete.TipoComponenteFrete == TipoComponenteFrete.ISS)
                throw new ServicoException("Não é possível adicionar um componente de imposto.");

            decimal cotacaoMoeda = carga.ValorCotacaoMoeda ?? 0m;
            MoedaCotacaoBancoCentral moeda = carga.Moeda ?? MoedaCotacaoBancoCentral.Real;
            TipoCampoValorTabelaFrete tipoValor = TipoCampoValorTabelaFrete.AumentoValor;
            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = null;

            if (cobrarOutroDocumento)
                modeloDocumentoFiscal = repositorioModeloDocumentoFiscal.BuscarPorId(codigoModeloDocumentoFiscal);

            if (componenteFrete.TipoComponenteFrete == TipoComponenteFrete.ADVALOREM || componenteFrete.TipoValor == TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal)
            {
                decimal valorNotas = 0;

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in carga.Pedidos)
                {
                    if (cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.Normal || cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.NormalESubContratada)
                        valorNotas += repositorioPedidoXMLNotaFiscal.BuscarTotalPorCargaPedido(cargaPedido.Codigo, componenteFrete.NaoDeveIncidirSobreNotasFiscaisPateles);
                    else if (cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.SubContratada || cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.SVMTerceiro)
                        valorNotas += repositorioPedidoCTeParaSubContratacao.BuscarTotalPorCargaPedido(cargaPedido.Codigo);
                }

                tipoValor = TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal;

                if (moeda != MoedaCotacaoBancoCentral.Real && cotacaoMoeda > 0m)
                {
                    valorTotalMoeda = Math.Round(valorNotas * percentual / 100 / cotacaoMoeda, 2, MidpointRounding.AwayFromZero);
                    valorComponente = Math.Round(valorTotalMoeda * cotacaoMoeda, 2, MidpointRounding.AwayFromZero);
                }
                else
                    valorComponente = valorNotas * (percentual / 100);
            }
            else if (moeda != MoedaCotacaoBancoCentral.Real)
                valorComponente = Math.Round(valorTotalMoeda * cotacaoMoeda, 2, MidpointRounding.AwayFromZero);

            if (componenteFrete.DescontarValorTotalAReceber)
                valorComponente = -valorComponente;

            if (valorComponente <= 0m && percentual > 0m && componenteFrete.TipoComponenteFrete == TipoComponenteFrete.ADVALOREM)
            {
                decimal valorMercadoria = repositorioPedidoXMLNotaFiscal.ObterValorTotalPorCarga(carga.Codigo, componenteFrete.NaoDeveIncidirSobreNotasFiscaisPateles);

                if (valorMercadoria == 0)
                    valorMercadoria = repositorioPedidoCTeParaSubContratacao.BuscarValorTotalMercadoriaPorCarga(carga.Codigo);

                if (valorMercadoria > 0)
                {
                    Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido primeiroCargaPedido = repositorioCargaPedido.BuscarPrimeiraPorCarga(carga.Codigo);
                    decimal valorCotacao = primeiroCargaPedido?.Pedido?.ValorTaxaFeeder ?? 0;

                    valorComponente = ((percentual / 100) * (valorCotacao > 0 ? (valorMercadoria * valorCotacao) : valorMercadoria));
                }
            }

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete cargaJanelaCarregamentoTransportadorComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete();

            cargaJanelaCarregamentoTransportadorComponenteFrete.CargaJanelaCarregamentoTransportador = cargaJanelaCarregamentoTransportador;
            cargaJanelaCarregamentoTransportadorComponenteFrete.ComponenteFrete = componenteFrete;
            cargaJanelaCarregamentoTransportadorComponenteFrete.ValorComponente = valorComponente;
            cargaJanelaCarregamentoTransportadorComponenteFrete.TipoValor = tipoValor;
            cargaJanelaCarregamentoTransportadorComponenteFrete.ModeloDocumentoFiscal = modeloDocumentoFiscal;
            cargaJanelaCarregamentoTransportadorComponenteFrete.Percentual = percentual;
            cargaJanelaCarregamentoTransportadorComponenteFrete.Moeda = moeda;
            cargaJanelaCarregamentoTransportadorComponenteFrete.ValorCotacaoMoeda = cotacaoMoeda;
            cargaJanelaCarregamentoTransportadorComponenteFrete.ValorTotalMoeda = valorTotalMoeda;

            repositorioCargaJanelaCarregamentoTransportadorComponenteFrete.Inserir(cargaJanelaCarregamentoTransportadorComponenteFrete);

            return cargaJanelaCarregamentoTransportadorComponenteFrete;
        }

        public void AdicionarComponenteFreteCargaUnicoPorTipo(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete, decimal valorComponente, decimal percentual, bool componenteFilialEmissora, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete TipoValor, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComplementoFrete, Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete cargaComplementoFrete, bool incluirBaseCalculoICMS, bool incluirIntegralmenteContratoFreteTerceiro, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFretes, bool ratearValorFreteEntrePedidos = true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.Manual, bool sempreExtornar = true, bool porQuantidadeDocumentos = false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete? tipoCalculoQuantidadeDocumentos = null, int quantidadeTotalDocumentos = 0, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalRateio = null, Dominio.Entidades.Cliente tomador = null)
        {
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponenteFrete = (from obj in cargaComponentesFretes where obj.Carga.Codigo == carga.Codigo && obj.TipoComponenteFrete == tipoComplementoFrete && obj.ComponenteFilialEmissora == componenteFilialEmissora && ((componenteFrete == null && obj.ComponenteFrete == null) || (componenteFrete != null && obj.ComponenteFrete.Codigo == componenteFrete.Codigo)) select obj).FirstOrDefault();
            if (cargaComponenteFrete == null)
                cargaComponentesFretes.Add(SalvarComponenteFreteCargaUnicoPorTipo(carga, componenteFrete, valorComponente, percentual, componenteFilialEmissora, TipoValor, tipoComplementoFrete, cargaComplementoFrete, incluirBaseCalculoICMS, incluirIntegralmenteContratoFreteTerceiro, modeloDocumentoFiscal, tipoServicoMultisoftware, usuario, unitOfWork, ratearValorFreteEntrePedidos, tipo, sempreExtornar, porQuantidadeDocumentos, tipoCalculoQuantidadeDocumentos, quantidadeTotalDocumentos, modeloDocumentoFiscalRateio, tomador, cargaComponenteFrete));
            else
                SalvarComponenteFreteCargaUnicoPorTipo(carga, componenteFrete, valorComponente, percentual, componenteFilialEmissora, TipoValor, tipoComplementoFrete, cargaComplementoFrete, incluirBaseCalculoICMS, incluirIntegralmenteContratoFreteTerceiro, modeloDocumentoFiscal, tipoServicoMultisoftware, usuario, unitOfWork, ratearValorFreteEntrePedidos, tipo, sempreExtornar, porQuantidadeDocumentos, tipoCalculoQuantidadeDocumentos, quantidadeTotalDocumentos, modeloDocumentoFiscalRateio, tomador, cargaComponenteFrete);
        }

        public void AdicionarComponenteFreteCargaUnicoPorTipo(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete, decimal valorComponente, decimal percentual, bool componenteFilialEmissora, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete TipoValor, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComplementoFrete, Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete cargaComplementoFrete, bool incluirBaseCalculoICMS, bool incluirIntegralmenteContratoFreteTerceiro, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork, bool ratearValorFreteEntrePedidos = true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.Manual, bool sempreExtornar = true, bool porQuantidadeDocumentos = false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete? tipoCalculoQuantidadeDocumentos = null, int quantidadeTotalDocumentos = 0, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalRateio = null, Dominio.Entidades.Cliente tomador = null)
        {
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponenteFrete = repCargaComponentesFrete.BuscarPorCargaETipo(carga.Codigo, tipoComplementoFrete, componenteFrete, componenteFilialEmissora);
            SalvarComponenteFreteCargaUnicoPorTipo(carga, componenteFrete, valorComponente, percentual, componenteFilialEmissora, TipoValor, tipoComplementoFrete, cargaComplementoFrete, incluirBaseCalculoICMS, incluirIntegralmenteContratoFreteTerceiro, modeloDocumentoFiscal, tipoServicoMultisoftware, usuario, unitOfWork, ratearValorFreteEntrePedidos, tipo, sempreExtornar, porQuantidadeDocumentos, tipoCalculoQuantidadeDocumentos, quantidadeTotalDocumentos, modeloDocumentoFiscalRateio, tomador, cargaComponenteFrete);
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete SalvarComponenteFreteCargaUnicoPorTipo(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete, decimal valorComponente, decimal percentual, bool componenteFilialEmissora, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete TipoValor, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComplementoFrete, Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete cargaComplementoFrete, bool incluirBaseCalculoICMS, bool incluirIntegralmenteContratoFreteTerceiro, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork, bool ratearValorFreteEntrePedidos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete tipo, bool sempreExtornar, bool porQuantidadeDocumentos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete? tipoCalculoQuantidadeDocumentos, int quantidadeTotalDocumentos, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalRateio, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponenteFrete)
        {
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            if (cargaComponenteFrete == null)
                cargaComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete();

            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = carga.TabelaFrete;
            bool destacarComponenteTabelaFrete = Servicos.Embarcador.Carga.Frete.DestacarComponenteTabelaFrete(carga.TabelaFrete, componenteFrete);

            cargaComponenteFrete.Carga = carga;
            cargaComponenteFrete.ComponenteFilialEmissora = componenteFilialEmissora;
            cargaComponenteFrete.ComponenteFrete = componenteFrete;
            cargaComponenteFrete.DescontarValorTotalAReceber = componenteFrete?.DescontarValorTotalAReceber ?? false;
            cargaComponenteFrete.AcrescentaValorTotalAReceber = componenteFrete?.AcrescentaValorTotalAReceber ?? false;
            cargaComponenteFrete.SomarComponenteFreteLiquido = componenteFrete?.SomarComponenteFreteLiquido ?? false;
            cargaComponenteFrete.DescontarComponenteFreteLiquido = (destacarComponenteTabelaFrete ? tabelaFrete.DescontarComponenteFreteLiquido : componenteFrete?.DescontarComponenteFreteLiquido) ?? false;
            cargaComponenteFrete.NaoSomarValorTotalAReceber = (destacarComponenteTabelaFrete ? tabelaFrete.NaoSomarValorTotalAReceber : componenteFrete?.NaoSomarValorTotalAReceber) ?? false;
            cargaComponenteFrete.NaoSomarValorTotalPrestacao = (destacarComponenteTabelaFrete ? tabelaFrete.NaoSomarValorTotalPrestacao : componenteFrete?.NaoSomarValorTotalPrestacao) ?? false;
            cargaComponenteFrete.TipoComponenteFrete = tipoComplementoFrete;
            cargaComponenteFrete.ValorComponente += valorComponente;
            cargaComponenteFrete.CargaComplementoFrete = cargaComplementoFrete;
            cargaComponenteFrete.TipoValor = TipoValor;

            if (tomador == null)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiraPorCarga(carga.Codigo);
                tomador = cargaPedido.ObterTomador();
            }
            SetarConfiguracoesComponente(ref cargaComponenteFrete, carga, tomador, unitOfWork);

            cargaComponenteFrete.ModeloDocumentoFiscal = modeloDocumentoFiscal;

            if (modeloDocumentoFiscal != null && modeloDocumentoFiscal.Numero != "57")
                cargaComponenteFrete.IncluirBaseCalculoICMS = false;
            else
                cargaComponenteFrete.IncluirBaseCalculoICMS = incluirBaseCalculoICMS;

            cargaComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = incluirIntegralmenteContratoFreteTerceiro;
            cargaComponenteFrete.SempreExtornar = sempreExtornar;
            cargaComponenteFrete.Percentual = percentual;
            cargaComponenteFrete.Tipo = tipo;
            cargaComponenteFrete.UsuarioInformou = usuario;

            if (porQuantidadeDocumentos)
            {
                cargaComponenteFrete.ModeloDocumentoFiscalRateio = modeloDocumentoFiscalRateio;
                cargaComponenteFrete.PorQuantidadeDocumentos = porQuantidadeDocumentos;
                cargaComponenteFrete.QuantidadeTotalDocumentos += quantidadeTotalDocumentos;
                cargaComponenteFrete.TipoCalculoQuantidadeDocumentos = tipoCalculoQuantidadeDocumentos;
            }

            if (cargaComponenteFrete.Codigo <= 0)
                repCargaComponentesFrete.Inserir(cargaComponenteFrete);
            else
                repCargaComponentesFrete.Atualizar(cargaComponenteFrete);

            if (ratearValorFreteEntrePedidos)
            {

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configEmbarcador = repConfiguracao.BuscarConfiguracaoPadrao();
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);

                Servicos.Embarcador.Carga.RateioFrete serCargaFreteRateio = new RateioFrete(unitOfWork);
                serCargaFreteRateio.RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, configEmbarcador, componenteFilialEmissora, unitOfWork, tipoServicoMultisoftware);
            }

            return cargaComponenteFrete;
        }

        public void AdicionarCargaPedidoComponente(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, decimal valorComponente, decimal percentual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete TipoValor, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComplementoFrete, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete, bool incluirBaseCalculo, bool incluirIntegralmenteContratoFreteTerceiro, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal, Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaRateio, string OutraDescricaoCTe, bool descontarValorTotalAReceber, bool acrescentarValorTotalAReceber, bool componenteFilialEmissora, Repositorio.UnitOfWork unitOfWork, bool porQuantidadeDocumentos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete? tipoCalculoQuantidadeDocumentos, int quantidadeTotalDocumentos, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalRateio, bool naoSomarValorTotalAReceber, bool naoSomarValorTotalPrestacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moeda = MoedaCotacaoBancoCentral.Real, decimal cotacaoMoeda = 0m, decimal valorTotalMoeda = 0m)
        {
            if (valorComponente > 0m || descontarValorTotalAReceber)
            {
                Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponentesFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponentesFrete = repCargaPedidoComponentesFrete.BuscarPorCompomente(cargaPedido.Codigo, tipoComplementoFrete, componenteFrete, componenteFilialEmissora);
                SalvarCargaPedidoComponente(cargaPedido, valorComponente, percentual, TipoValor, tipoComplementoFrete, componenteFrete, incluirBaseCalculo, incluirIntegralmenteContratoFreteTerceiro, modeloDocumentoFiscal, formulaRateio, OutraDescricaoCTe, descontarValorTotalAReceber, acrescentarValorTotalAReceber, componenteFilialEmissora, unitOfWork, porQuantidadeDocumentos, tipoCalculoQuantidadeDocumentos, quantidadeTotalDocumentos, modeloDocumentoFiscalRateio, naoSomarValorTotalAReceber, cargaPedidoComponentesFrete, naoSomarValorTotalPrestacao, moeda, cotacaoMoeda, valorTotalMoeda);
            }
        }

        public void AdicionarCargaPedidoComponente(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, decimal valorComponente, decimal percentual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete TipoValor, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComplementoFrete, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete, bool incluirBaseCalculo, bool incluirIntegralmenteContratoFreteTerceiro, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal, Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaRateio, string OutraDescricaoCTe, bool descontarValorTotalAReceber, bool acrescentarValorTotalAReceber, bool componenteFilialEmissora, Repositorio.UnitOfWork unitOfWork, bool porQuantidadeDocumentos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete? tipoCalculoQuantidadeDocumentos, int quantidadeTotalDocumentos, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalRateio, bool naoSomarValorTotalAReceber, ref List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFretes, bool naoSomarValorTotalPrestacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moeda, decimal cotacaoMoeda, decimal valorTotalMoeda, bool UtilizarFormulaRateioCarga = false)
        {
            if (valorComponente > 0m || percentual > 0m || descontarValorTotalAReceber)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponentesFrete = (from obj in cargaPedidoComponentesFretes where obj.CargaPedido.Codigo == cargaPedido.Codigo && obj.TipoComponenteFrete == tipoComplementoFrete && obj.ComponenteFilialEmissora == componenteFilialEmissora && ((componenteFrete == null && obj.ComponenteFrete == null) || (componenteFrete != null && obj.ComponenteFrete.Codigo == componenteFrete.Codigo)) select obj).FirstOrDefault();

                if (cargaPedidoComponentesFrete == null)
                    cargaPedidoComponentesFretes.Add(SalvarCargaPedidoComponente(cargaPedido, valorComponente, percentual, TipoValor, tipoComplementoFrete, componenteFrete, incluirBaseCalculo, incluirIntegralmenteContratoFreteTerceiro, modeloDocumentoFiscal, formulaRateio, OutraDescricaoCTe, descontarValorTotalAReceber, acrescentarValorTotalAReceber, componenteFilialEmissora, unitOfWork, porQuantidadeDocumentos, tipoCalculoQuantidadeDocumentos, quantidadeTotalDocumentos, modeloDocumentoFiscalRateio, naoSomarValorTotalAReceber, cargaPedidoComponentesFrete, naoSomarValorTotalPrestacao, moeda, cotacaoMoeda, valorTotalMoeda, UtilizarFormulaRateioCarga));
                else
                    SalvarCargaPedidoComponente(cargaPedido, valorComponente, percentual, TipoValor, tipoComplementoFrete, componenteFrete, incluirBaseCalculo, incluirIntegralmenteContratoFreteTerceiro, modeloDocumentoFiscal, formulaRateio, OutraDescricaoCTe, descontarValorTotalAReceber, acrescentarValorTotalAReceber, componenteFilialEmissora, unitOfWork, porQuantidadeDocumentos, tipoCalculoQuantidadeDocumentos, quantidadeTotalDocumentos, modeloDocumentoFiscalRateio, naoSomarValorTotalAReceber, cargaPedidoComponentesFrete, naoSomarValorTotalPrestacao, moeda, cotacaoMoeda, valorTotalMoeda, UtilizarFormulaRateioCarga);
            }
        }

        public void AdicionarComponentesCargaAgrupada(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool filialEmissora, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFretes, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga.CargaAgrupada)
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasAgrupadas = repCarga.BuscarCargasOriginais(carga.Codigo);
                foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaAgrupada in cargasAgrupadas)
                {
                    List<Dominio.Entidades.Embarcador.Frete.ComponenteFrete> componentes = (from obj in cargaPedidoComponentesFretes select obj.ComponenteFrete).Distinct().ToList();
                    foreach (Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente in componentes)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFretesCargaAgrupada = (from obj in cargaPedidoComponentesFretes where obj.CargaPedido.CargaOrigem.Codigo == cargaAgrupada.Codigo && obj.ComponenteFrete == componente && obj.ComponenteFilialEmissora == filialEmissora select obj).ToList();
                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponentesFrete = cargaPedidoComponentesFretesCargaAgrupada.FirstOrDefault();
                        if (cargaPedidoComponentesFrete != null)
                            AdicionarComponenteFreteCarga(cargaAgrupada, componente, cargaPedidoComponentesFretesCargaAgrupada.Sum(obj => obj.ValorComponente), cargaPedidoComponentesFrete.Percentual, filialEmissora, cargaPedidoComponentesFrete.TipoValor, cargaPedidoComponentesFrete.TipoComponenteFrete, null, cargaPedidoComponentesFrete.IncluirBaseCalculoICMS, cargaPedidoComponentesFrete.IncluirIntegralmenteContratoFreteTerceiro, cargaPedidoComponentesFrete.ModeloDocumentoFiscal, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, null, unitOfWork, false, TipoCargaComponenteFrete.TabelaFrete, true, true, cargaPedidoComponentesFrete.CargaPedido.ObterTomador());
                    }
                }
            }
        }

        public void AdicionarComponentesCargaMultimodal(Dominio.Entidades.Embarcador.Cargas.Carga cargaAgrupada, bool filialEmissora, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFretes, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Frete.ComponenteFrete> componentes = (from obj in cargaPedidoComponentesFretes select obj.ComponenteFrete).Distinct().ToList();
            foreach (Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente in componentes)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFretesCargaAgrupada = (from obj in cargaPedidoComponentesFretes where obj.CargaPedido.CargaOrigem.Codigo == cargaAgrupada.Codigo && obj.ComponenteFrete == componente && obj.ComponenteFilialEmissora == filialEmissora select obj).ToList();
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponentesFrete = cargaPedidoComponentesFretesCargaAgrupada.FirstOrDefault();
                if (cargaPedidoComponentesFrete != null)
                    AdicionarComponenteFreteCarga(cargaAgrupada, componente, cargaPedidoComponentesFretesCargaAgrupada.Sum(obj => obj.ValorComponente), cargaPedidoComponentesFrete.Percentual, filialEmissora, cargaPedidoComponentesFrete.TipoValor, cargaPedidoComponentesFrete.TipoComponenteFrete, null, cargaPedidoComponentesFrete.IncluirBaseCalculoICMS, cargaPedidoComponentesFrete.IncluirIntegralmenteContratoFreteTerceiro, cargaPedidoComponentesFrete.ModeloDocumentoFiscal, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, null, unitOfWork, false, TipoCargaComponenteFrete.TabelaFrete, true, true, cargaPedidoComponentesFrete.CargaPedido.ObterTomador(), true);
            }
        }

        private Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete SalvarCargaPedidoComponente(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, decimal valorComponente, decimal percentual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete TipoValor, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComplementoFrete, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete, bool incluirBaseCalculo, bool incluirIntegralmenteContratoFreteTerceiro, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal, Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaRateio, string OutraDescricaoCTe, bool descontarValorTotalAReceber, bool acrescentarValorTotalAReceber, bool componenteFilialEmissora, Repositorio.UnitOfWork unitOfWork, bool porQuantidadeDocumentos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete? tipoCalculoQuantidadeDocumentos, int quantidadeTotalDocumentos, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalRateio, bool naoSomarValorTotalAReceber, Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponentesFrete, bool naoSomarValorTotalPrestacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moeda, decimal cotacaoMoeda, decimal valorTotalMoeda, bool UtilizarFormulaRateioCarga = false)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponentesFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContraatcao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);

            Servicos.Embarcador.Moedas.Cotacao serCotacao = new Servicos.Embarcador.Moedas.Cotacao(unitOfWork);

            Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();

            decimal valorCotacao = cargaPedido?.Pedido?.ValorTaxaFeeder ?? 0;
            //if (tomador != null && tomador.CPF_CNPJ > 0D)
            //    valorCotacao = serCotacao.BuscarValorCotacaoCliente(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.DolarCompra, unitOfWork, tomador.CPF_CNPJ, cargaPedido?.Pedido?.ValorTaxaFeeder ?? 0);

            if (cargaPedidoComponentesFrete == null)
            {
                Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = cargaPedido.Carga?.TabelaFrete ?? null;
                bool destacarComponenteTabelaFrete = Servicos.Embarcador.Carga.Frete.DestacarComponenteTabelaFrete(tabelaFrete, componenteFrete);

                cargaPedidoComponentesFrete = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete
                {
                    CargaPedido = cargaPedido,
                    ComponenteFrete = componenteFrete,
                    TipoValor = TipoValor,
                    ComponenteFilialEmissora = componenteFilialEmissora,
                    DescontarValorTotalAReceber = descontarValorTotalAReceber,
                    AcrescentaValorTotalAReceber = acrescentarValorTotalAReceber,
                    NaoSomarValorTotalAReceber = naoSomarValorTotalAReceber,
                    NaoSomarValorTotalPrestacao = naoSomarValorTotalPrestacao,
                    Percentual = percentual,
                    TipoComponenteFrete = tipoComplementoFrete,
                    ModeloDocumentoFiscal = modeloDocumentoFiscal,
                    IncluirBaseCalculoICMS = incluirBaseCalculo,
                    RateioFormula = formulaRateio,
                    OutraDescricaoCTe = OutraDescricaoCTe,
                    Moeda = moeda,
                    ValorCotacaoMoeda = cotacaoMoeda,
                    ValorTotalMoeda = valorTotalMoeda,
                    IncluirIntegralmenteContratoFreteTerceiro = incluirIntegralmenteContratoFreteTerceiro,
                    UtilizarFormulaRateioCarga = UtilizarFormulaRateioCarga,
                    DescontarDoValorAReceberValorComponente = (destacarComponenteTabelaFrete ? tabelaFrete?.DescontarDoValorAReceberValorComponente : componenteFrete?.DescontarValorTotalAReceber) ?? false,
                    DescontarDoValorAReceberOICMSDoComponente = tabelaFrete?.DescontarDoValorAReceberOICMSDoComponente ?? false,
                    ValorICMSComponenteDestacado = tabelaFrete?.ValorICMSComponenteDestacado ?? 0,
                };

                if (valorComponente <= 0m && percentual > 0m && componenteFrete.TipoComponenteFrete == TipoComponenteFrete.ADVALOREM)
                {
                    decimal valorMercadoria = repPedidoXMLNotaFiscal.ObterValorTotalPorCargaPedido(cargaPedido.Codigo, componenteFrete.NaoDeveIncidirSobreNotasFiscaisPateles);
                    if (valorMercadoria == 0m)
                        valorMercadoria = repPedidoCTeParaSubContraatcao.BuscarValorTotalMercadoriaPorCargaPedido(cargaPedido.Codigo);
                    if (valorMercadoria > 0m)
                    {
                        valorComponente = ((percentual / 100) * (valorCotacao > 0m ? (valorMercadoria * valorCotacao) : valorMercadoria));
                        cargaPedidoComponentesFrete.ValorComponente = Math.Round(valorComponente, 2, MidpointRounding.AwayFromZero);
                    }
                    else
                        cargaPedidoComponentesFrete.ValorComponente = Math.Round(valorComponente, 2, MidpointRounding.AwayFromZero);

                    //Servicos.Log.TratarErro("Ad valorem com valor zerado ao adicionar na carga pedido, total mercadoria: " + valorMercadoria.ToString("n2") + ", valor componente: " + valorComponente.ToString("n2"));
                }
                else
                    cargaPedidoComponentesFrete.ValorComponente = Math.Round(valorComponente, 2, MidpointRounding.AwayFromZero);

                if (porQuantidadeDocumentos)
                {
                    cargaPedidoComponentesFrete.PorQuantidadeDocumentos = porQuantidadeDocumentos;
                    cargaPedidoComponentesFrete.TipoCalculoQuantidadeDocumentos = tipoCalculoQuantidadeDocumentos;
                    cargaPedidoComponentesFrete.ModeloDocumentoFiscalRateio = modeloDocumentoFiscalRateio;
                    cargaPedidoComponentesFrete.QuantidadeTotalDocumentos = quantidadeTotalDocumentos;
                }

                repCargaPedidoComponentesFrete.Inserir(cargaPedidoComponentesFrete);
            }
            else
            {
                cargaPedidoComponentesFrete.DescontarDoValorAReceberValorComponente = cargaPedido.Carga.TabelaFrete?.DescontarDoValorAReceberValorComponente ?? false;
                cargaPedidoComponentesFrete.DescontarDoValorAReceberOICMSDoComponente = cargaPedido.Carga.TabelaFrete?.DescontarDoValorAReceberOICMSDoComponente ?? false;
                cargaPedidoComponentesFrete.ValorICMSComponenteDestacado = cargaPedido.Carga.TabelaFrete?.ValorICMSComponenteDestacado ?? 0;

                if (valorComponente <= 0m && percentual > 0m && componenteFrete.TipoComponenteFrete == TipoComponenteFrete.ADVALOREM)
                {
                    decimal valorMercadoria = repPedidoXMLNotaFiscal.ObterValorTotalPorCargaPedido(cargaPedido.Codigo, componenteFrete.NaoDeveIncidirSobreNotasFiscaisPateles);
                    if (valorMercadoria > 0)
                    {
                        valorComponente = ((percentual / 100) * (valorCotacao > 0m ? (valorMercadoria * valorCotacao) : valorMercadoria));
                        valorComponente = Math.Round(valorComponente, 2, MidpointRounding.AwayFromZero);
                    }
                    else
                        valorComponente = Math.Round(valorComponente, 2, MidpointRounding.AwayFromZero);

                    //Servicos.Log.TratarErro("Ad valorem com valor zerado ao adicionar na carga pedido, total mercadoria: " + valorMercadoria.ToString("n2") + ", valor componente: " + valorComponente.ToString("n2"));
                }

                cargaPedidoComponentesFrete.ValorTotalMoeda += valorTotalMoeda;
                cargaPedidoComponentesFrete.ValorComponente = Math.Round(cargaPedidoComponentesFrete.ValorComponente + valorComponente, 2, MidpointRounding.AwayFromZero);

                if (porQuantidadeDocumentos)
                    cargaPedidoComponentesFrete.QuantidadeTotalDocumentos += quantidadeTotalDocumentos;

                repCargaPedidoComponentesFrete.Atualizar(cargaPedidoComponentesFrete);
            }

            return cargaPedidoComponentesFrete;
        }

        public void AdicionarRateoProdutoComplementoFrete(Dominio.Entidades.Embarcador.Rateio.RateioCargaPedidoProduto rateioNotaCteProduto, decimal valorComponente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComplementoFrete, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete, bool descontarValorTotalAReceber, Repositorio.UnitOfWork unitOfWork)
        {
            if (valorComponente > 0 || descontarValorTotalAReceber)
            {
                Repositorio.Embarcador.Rateio.RateioProdutoComponenteFrete repRateioProdutoComponenteFrete = new Repositorio.Embarcador.Rateio.RateioProdutoComponenteFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Rateio.RateioProdutoComponenteFrete rateioProdutoComponente = new Dominio.Entidades.Embarcador.Rateio.RateioProdutoComponenteFrete();
                rateioProdutoComponente.RateioCargaPedidoProduto = rateioNotaCteProduto;
                rateioProdutoComponente.TipoComponenteFrete = tipoComplementoFrete;
                rateioProdutoComponente.ComponenteFrete = componenteFrete;
                rateioProdutoComponente.ValorComponente = valorComponente;
                repRateioProdutoComponenteFrete.Inserir(rateioProdutoComponente);
            }
        }

        public bool MudarComplementoDaCargaCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional> componentesAdicionais, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponentesFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unitOfWork);
            bool alterouCompomentes = false;

            if (componentesAdicionais != null)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete> cargaCTeComponentesFreteAtivos = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete>();
                foreach (Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componenteAdicional in componentesAdicionais)
                {
                    Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorCodigo(componenteAdicional.Componente.Codigo);
                    Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete cargaCTeCompomenteFrete = repCargaCTeComponentesFrete.BuscarPorCargaCTeEComponente(cargaCTe.Codigo, componenteFrete.Codigo);

                    var inserir = false;
                    if (cargaCTeCompomenteFrete == null)
                    {
                        cargaCTeCompomenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete();
                        inserir = true;
                    }

                    if (cargaCTeCompomenteFrete.ValorComponente != componenteAdicional.ValorComponente || componenteAdicional.IncluirBaseCalculoICMS != componenteAdicional.IncluirBaseCalculoICMS)
                    {
                        alterouCompomentes = true;

                        Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = cargaCTe.Carga?.TabelaFrete;
                        bool destacarComponenteTabelaFrete = Servicos.Embarcador.Carga.Frete.DestacarComponenteTabelaFrete(tabelaFrete, componenteFrete);

                        cargaCTeCompomenteFrete.CargaCTe = cargaCTe;
                        cargaCTeCompomenteFrete.ComponenteFrete = componenteFrete;
                        cargaCTeCompomenteFrete.IncluirBaseCalculoICMS = componenteAdicional.IncluirBaseCalculoICMS;
                        cargaCTeCompomenteFrete.TipoComponenteFrete = componenteFrete.TipoComponenteFrete;
                        cargaCTeCompomenteFrete.DescontarValorTotalAReceber = componenteFrete?.DescontarValorTotalAReceber ?? false;
                        cargaCTeCompomenteFrete.AcrescentaValorTotalAReceber = componenteFrete?.AcrescentaValorTotalAReceber ?? false;
                        cargaCTeCompomenteFrete.NaoSomarValorTotalAReceber = (destacarComponenteTabelaFrete ? tabelaFrete?.NaoSomarValorTotalAReceber : componenteFrete.NaoSomarValorTotalAReceber) ?? false;
                        cargaCTeCompomenteFrete.DescontarDoValorAReceberValorComponente = (destacarComponenteTabelaFrete ? tabelaFrete?.DescontarDoValorAReceberValorComponente : componenteFrete.DescontarValorTotalAReceber) ?? false;
                        cargaCTeCompomenteFrete.DescontarDoValorAReceberOICMSDoComponente = tabelaFrete?.DescontarDoValorAReceberOICMSDoComponente ?? false;
                        cargaCTeCompomenteFrete.ValorICMSComponenteDestacado = tabelaFrete?.ValorICMSComponenteDestacado ?? 0;
                        cargaCTeCompomenteFrete.NaoSomarValorTotalPrestacao = (destacarComponenteTabelaFrete ? tabelaFrete?.NaoSomarValorTotalPrestacao : componenteFrete.NaoSomarValorTotalPrestacao) ?? false;
                        cargaCTeCompomenteFrete.TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo;
                        cargaCTeCompomenteFrete.ValorComponente = componenteAdicional.ValorComponente;
                        cargaCTeCompomenteFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                        cargaCTeCompomenteFrete.DescontarComponenteFreteLiquido = (destacarComponenteTabelaFrete ? tabelaFrete?.DescontarComponenteFreteLiquido : componenteFrete.DescontarComponenteFreteLiquido) ?? false;

                        if (inserir)
                            repCargaCTeComponentesFrete.Inserir(cargaCTeCompomenteFrete);
                        else
                            repCargaCTeComponentesFrete.Atualizar(cargaCTeCompomenteFrete);
                    }

                    cargaCTeComponentesFreteAtivos.Add(cargaCTeCompomenteFrete);
                }

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete> cargaCTeComponentesFreteSalvoNoBanco = repCargaCTeComponentesFrete.BuscarPorCargaCTe(cargaCTe.Codigo);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete cargaCTeComponenteSalvoNoBanco in cargaCTeComponentesFreteSalvoNoBanco)
                {
                    if (!cargaCTeComponentesFreteAtivos.Exists(obj => obj.Codigo == cargaCTeComponenteSalvoNoBanco.Codigo))
                        repCargaCTeComponentesFrete.Deletar(cargaCTeComponenteSalvoNoBanco);
                }
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete cargaCTeComponenteFreteICMS = repCargaCTeComponentesFrete.BuscarPorCargaCTeETipoComponente(cargaCTe.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS);

            if (cargaCTe.CTe.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim && cargaCTe.CTe.CST != "60" && cargaCTe.CTe.ValorICMS > 0)
            {
                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFreteICMS = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS);

                if (cargaCTeComponenteFreteICMS == null)
                    cargaCTeComponenteFreteICMS = new Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete();

                cargaCTeComponenteFreteICMS.NaoSomarValorTotalPrestacao = false;
                cargaCTeComponenteFreteICMS.NaoSomarValorTotalAReceber = false;
                cargaCTeComponenteFreteICMS.AcrescentaValorTotalAReceber = false;
                cargaCTeComponenteFreteICMS.ComponenteFrete = componenteFreteICMS;
                cargaCTeComponenteFreteICMS.DescontarValorTotalAReceber = false;
                cargaCTeComponenteFreteICMS.IncluirBaseCalculoICMS = false;
                cargaCTeComponenteFreteICMS.TipoComponenteFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS;
                cargaCTeComponenteFreteICMS.ValorComponente = cargaCTe.CTe.ValorICMS;
                cargaCTeComponenteFreteICMS.CargaCTe = cargaCTe;
                cargaCTeComponenteFreteICMS.IncluirIntegralmenteContratoFreteTerceiro = false;

                if (cargaCTeComponenteFreteICMS.Codigo <= 0)
                    repCargaCTeComponentesFrete.Inserir(cargaCTeComponenteFreteICMS);
                else
                    repCargaCTeComponentesFrete.Atualizar(cargaCTeComponenteFreteICMS);
            }
            else if (cargaCTeComponenteFreteICMS != null)
            {
                repCargaCTeComponentesFrete.Deletar(cargaCTeComponenteFreteICMS);
            }

            return alterouCompomentes;
        }

        public static void AdicionarComponentesObrigatoriosNotaFiscal(Dominio.Entidades.Embarcador.Cargas.Carga carga, UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalComponente repXMLNotaFiscalComponente = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalComponente(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);

            Servicos.Embarcador.Carga.ComponetesFrete svcComponenteFrete = new Servicos.Embarcador.Carga.ComponetesFrete(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Pedido.XMLNotaFiscalComponente> componentesXMLNotaFiscal = repXMLNotaFiscalComponente.BuscarSumarizadoPorCarga(carga.Codigo);

            foreach (Dominio.ObjetosDeValor.Embarcador.Pedido.XMLNotaFiscalComponente componenteXMLNotaFiscal in componentesXMLNotaFiscal)
            {
                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorCodigo(componenteXMLNotaFiscal.CodigoComponenteFrete);

                svcComponenteFrete.AdicionarComponenteFreteCarga(carga, componenteFrete, componenteXMLNotaFiscal.Valor, 0m, false, TipoCampoValorTabelaFrete.AumentoValor, componenteFrete.TipoComponenteFrete, null, (componenteXMLNotaFiscal.IncluirICMS.HasValue ? componenteXMLNotaFiscal.IncluirICMS.Value : true), (componenteXMLNotaFiscal.IncluirIntegralmenteContratoFreteTerceiro.HasValue ? componenteXMLNotaFiscal.IncluirIntegralmenteContratoFreteTerceiro.Value : false), null, tipoServicoMultisoftware, null, unitOfWork, false, TipoCargaComponenteFrete.TabelaFrete, false);
            }
        }

        public void RemoverComponentesCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponenteFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);

            repCargaComponenteFrete.DeletarPorCarga(carga.Codigo, false);
        }

        public void RemoverComponentesCargaPedido(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);

            repCargaPedidoComponenteFrete.DeletarPorCargaPedido(cargaPedido.Codigo, false);
        }

        public void RemoverComponentesCargaJanelaCarregamentoTransportador(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete repositorioCargaJanelaCarregamentoTransportadorComponenteFrete = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete(unitOfWork);

            repositorioCargaJanelaCarregamentoTransportadorComponenteFrete.DeletarPorCargaJanelaCarregamentoTransportador(cargaJanelaCarregamentoTransportador.Codigo);
        }

        public void SetarConfiguracoesComponente(ref Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponentesFrete, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Cliente tomador, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente dadosComponente = null;

            SetarConfiguracoesComponente(ref cargaComponentesFrete, ref dadosComponente, carga, tomador, unitOfWork);
        }

        public void SetarConfiguracoesComponente(ref Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente dadosComponente, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponentesFrete = null;

            SetarConfiguracoesComponente(ref cargaComponentesFrete, ref dadosComponente, parametros, unitOfWork);
        }

        #endregion

        #region Métodos Privados

        private void SetarConfiguracoesComponente(ref Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponentesFrete, ref Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente dadosComponente, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Cliente tomador, Repositorio.UnitOfWork unitOfWork)
        {
            if (tomador != null)
            {
                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = null;

                if (cargaComponentesFrete != null)
                    componenteFrete = cargaComponentesFrete.ComponenteFrete;
                else if (dadosComponente != null)
                    componenteFrete = dadosComponente.ComponenteFrete;

                Dominio.Entidades.Embarcador.Rateio.RateioFormula rateioFormula = null;
                Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = null;
                string outraDescricaoCTe = null;
                bool achouConfiguracoes = false;

                if (carga.TipoOperacao != null && carga.TipoOperacao.UsarConfiguracaoEmissao)
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoConfiguracaoComponentes tipoOperacaoConfiguracaoComponentes = (from obj in carga.TipoOperacao.TipoOperacaoConfiguracoesComponentes where obj.ComponenteFrete.Codigo == componenteFrete.Codigo select obj).FirstOrDefault();
                    if (tipoOperacaoConfiguracaoComponentes != null)
                    {
                        achouConfiguracoes = true;
                        modeloDocumentoFiscal = tipoOperacaoConfiguracaoComponentes.ModeloDocumentoFiscal;
                        outraDescricaoCTe = tipoOperacaoConfiguracaoComponentes.OutraDescricaoCTe;
                        rateioFormula = tipoOperacaoConfiguracaoComponentes.RateioFormula;
                    }
                }
                else if (tomador.NaoUsarConfiguracaoEmissaoGrupo || tomador.GrupoPessoas == null)
                {
                    Dominio.Entidades.Embarcador.Pessoas.ClienteConfiguracaoComponentes clienteConfiguracaoComponentes = null;

                    if (tomador.ClienteConfiguracoesComponentes != null)
                        clienteConfiguracaoComponentes = (from obj in tomador.ClienteConfiguracoesComponentes where obj.ComponenteFrete.Codigo == componenteFrete.Codigo select obj).FirstOrDefault();

                    if (clienteConfiguracaoComponentes != null)
                    {
                        achouConfiguracoes = true;
                        modeloDocumentoFiscal = clienteConfiguracaoComponentes.ModeloDocumentoFiscal;
                        outraDescricaoCTe = clienteConfiguracaoComponentes.OutraDescricaoCTe;
                        rateioFormula = clienteConfiguracaoComponentes.RateioFormula;
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasConfiguracaoComponentesFrete grupoPessoasConfiguracaoComponentesFrete = (from obj in tomador.GrupoPessoas.GrupoPessoasConfiguracaoComponentesFretes where obj.ComponenteFrete.Codigo == componenteFrete.Codigo select obj).FirstOrDefault();
                    if (grupoPessoasConfiguracaoComponentesFrete != null)
                    {
                        achouConfiguracoes = true;
                        modeloDocumentoFiscal = grupoPessoasConfiguracaoComponentesFrete.ModeloDocumentoFiscal;
                        outraDescricaoCTe = grupoPessoasConfiguracaoComponentesFrete.OutraDescricaoCTe;
                        rateioFormula = grupoPessoasConfiguracaoComponentesFrete.RateioFormula;
                    }
                }

                if (achouConfiguracoes)
                {
                    if (dadosComponente != null)
                    {
                        dadosComponente.ModeloDocumentoFiscal = modeloDocumentoFiscal;
                        dadosComponente.OutraDescricaoCTe = outraDescricaoCTe;
                        dadosComponente.RateioFormula = rateioFormula;
                    }
                    else if (cargaComponentesFrete != null)
                    {
                        cargaComponentesFrete.ModeloDocumentoFiscal = modeloDocumentoFiscal;
                        cargaComponentesFrete.OutraDescricaoCTe = outraDescricaoCTe;
                        cargaComponentesFrete.RateioFormula = rateioFormula;
                    }
                }

                if (string.IsNullOrWhiteSpace(outraDescricaoCTe) && componenteFrete != null && componenteFrete.ImprimirOutraDescricaoCTe)
                {
                    if (cargaComponentesFrete != null)
                        cargaComponentesFrete.OutraDescricaoCTe = componenteFrete.DescricaoCTe;
                    else if (dadosComponente != null)
                        dadosComponente.OutraDescricaoCTe = componenteFrete.DescricaoCTe;
                }

                if ((cargaComponentesFrete != null && cargaComponentesFrete.ModeloDocumentoFiscal != null && cargaComponentesFrete.ModeloDocumentoFiscal.Numero != "57") || (dadosComponente != null && dadosComponente.ModeloDocumentoFiscal != null && dadosComponente.ModeloDocumentoFiscal.Numero != "57"))
                {
                    if (cargaComponentesFrete != null)
                        cargaComponentesFrete.IncluirBaseCalculoICMS = false;
                    else if (dadosComponente != null)
                        dadosComponente.IncluirBaseCalculoICMS = false;
                }
            }
        }

        private void SetarConfiguracoesComponente(ref Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponentesFrete, ref Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente dadosComponente, Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametros, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Cliente tomador = parametros.Tomador;

            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = null;

            if (cargaComponentesFrete != null)
                componenteFrete = cargaComponentesFrete.ComponenteFrete;
            else if (dadosComponente != null)
                componenteFrete = dadosComponente.ComponenteFrete;

            Dominio.Entidades.Embarcador.Rateio.RateioFormula rateioFormula = null;
            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = null;
            string outraDescricaoCTe = null;
            bool achouConfiguracoes = false;

            if (parametros.TipoOperacao != null && parametros.TipoOperacao.UsarConfiguracaoEmissao)
            {
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoConfiguracaoComponentes tipoOperacaoConfiguracaoComponentes = (from obj in parametros.TipoOperacao.TipoOperacaoConfiguracoesComponentes where obj.ComponenteFrete.Codigo == componenteFrete.Codigo select obj).FirstOrDefault();
                if (tipoOperacaoConfiguracaoComponentes != null)
                {
                    achouConfiguracoes = true;
                    modeloDocumentoFiscal = tipoOperacaoConfiguracaoComponentes.ModeloDocumentoFiscal;
                    outraDescricaoCTe = tipoOperacaoConfiguracaoComponentes.OutraDescricaoCTe;
                    rateioFormula = tipoOperacaoConfiguracaoComponentes.RateioFormula;
                }
            }
            else if (tomador != null && (tomador.NaoUsarConfiguracaoEmissaoGrupo || tomador.GrupoPessoas == null))
            {
                Dominio.Entidades.Embarcador.Pessoas.ClienteConfiguracaoComponentes clienteConfiguracaoComponentes = (from obj in tomador.ClienteConfiguracoesComponentes where obj.ComponenteFrete.Codigo == componenteFrete.Codigo select obj).FirstOrDefault();
                if (clienteConfiguracaoComponentes != null)
                {
                    achouConfiguracoes = true;
                    modeloDocumentoFiscal = clienteConfiguracaoComponentes.ModeloDocumentoFiscal;
                    outraDescricaoCTe = clienteConfiguracaoComponentes.OutraDescricaoCTe;
                    rateioFormula = clienteConfiguracaoComponentes.RateioFormula;
                }
            }
            else if (tomador != null && tomador.GrupoPessoas != null)
            {
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasConfiguracaoComponentesFrete grupoPessoasConfiguracaoComponentesFrete = (from obj in tomador.GrupoPessoas.GrupoPessoasConfiguracaoComponentesFretes where obj.ComponenteFrete.Codigo == componenteFrete.Codigo select obj).FirstOrDefault();
                if (grupoPessoasConfiguracaoComponentesFrete != null)
                {
                    achouConfiguracoes = true;
                    modeloDocumentoFiscal = grupoPessoasConfiguracaoComponentesFrete.ModeloDocumentoFiscal;
                    outraDescricaoCTe = grupoPessoasConfiguracaoComponentesFrete.OutraDescricaoCTe;
                    rateioFormula = grupoPessoasConfiguracaoComponentesFrete.RateioFormula;
                }
            }

            if (achouConfiguracoes)
            {
                if (dadosComponente != null)
                {
                    dadosComponente.ModeloDocumentoFiscal = modeloDocumentoFiscal;
                    dadosComponente.OutraDescricaoCTe = outraDescricaoCTe;
                    dadosComponente.RateioFormula = rateioFormula;
                }
                else if (cargaComponentesFrete != null)
                {
                    cargaComponentesFrete.ModeloDocumentoFiscal = modeloDocumentoFiscal;
                    cargaComponentesFrete.OutraDescricaoCTe = outraDescricaoCTe;
                    cargaComponentesFrete.RateioFormula = rateioFormula;
                }
            }

            if (string.IsNullOrWhiteSpace(outraDescricaoCTe) && componenteFrete != null && componenteFrete.ImprimirOutraDescricaoCTe)
            {
                if (cargaComponentesFrete != null)
                    cargaComponentesFrete.OutraDescricaoCTe = componenteFrete.DescricaoCTe;
                else if (dadosComponente != null)
                    dadosComponente.OutraDescricaoCTe = componenteFrete.DescricaoCTe;
            }

            if ((cargaComponentesFrete != null && cargaComponentesFrete.ModeloDocumentoFiscal != null && cargaComponentesFrete.ModeloDocumentoFiscal.Numero != "57") || (dadosComponente != null && dadosComponente.ModeloDocumentoFiscal != null && dadosComponente.ModeloDocumentoFiscal.Numero != "57"))
            {
                if (cargaComponentesFrete != null)
                    cargaComponentesFrete.IncluirBaseCalculoICMS = false;
                else if (dadosComponente != null)
                    dadosComponente.IncluirBaseCalculoICMS = false;
            }

        }

        #endregion
    }
}
