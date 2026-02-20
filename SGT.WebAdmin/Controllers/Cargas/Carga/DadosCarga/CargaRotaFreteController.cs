using Dominio.Excecoes.Embarcador;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.DadosCarga
{
    [CustomAuthorize("Cargas/Carga", "Logistica/JanelaCarregamento", "GestaoPatio/FluxoPatio")]
    public class CargaRotaFreteController : BaseController
    {
        #region Construtores

        public CargaRotaFreteController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> AlterarRotaAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                int codigoRota = Request.GetIntParam("RotaFrete");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork, cancellationToken);

                Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Servicos.Embarcador.Pedido.Pedido servicoPedido = new Servicos.Embarcador.Pedido.Pedido();

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NaoPermitidoAlterarRotaDaCarga);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.CagaNaoEncontrada);

                if (carga.CargaAgrupamento != null || carga.CargaVinculada != null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.CargaFoiAgrupadaSendoAssimNaoPossivelAlterala);

                if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete &&
                    carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe &&
                    carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador &&
                    carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova &&
                    !((carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte ||
                      carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada) &&
                      ConfiguracaoEmbarcador.PermiteAlterarRotaEmCargaFinalizada))
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.SitaucaoDaCargaNaoPermiteQueRotaSejaAlterada);

                Dominio.Entidades.RotaFrete rotaFrete = await repRotaFrete.BuscarPorCodigoAsync(codigoRota);

                await unitOfWork.StartAsync();

                if (rotaFrete == null)
                {
                    if (carga.CargaAgrupamento != null || carga.CargaVinculada != null)
                        return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.CargaFoiAgrupadaSendoAssimNaoPossivelRemoverRota);

                    if (!serCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware))
                        return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NaoPossivelRemoverRotaNaAtualSituacaoDaCarga + " (" + carga.DescricaoSituacaoCarga + ").");

                    await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, carga, null, Localization.Resources.Cargas.Carga.RemoveuRotaDeFrete + " " + (carga?.Rota?.Descricao ?? "") + ".", unitOfWork);

                    carga.Rota = null;
                    carga.AgSelecaoRotaOperador = true;

                    if (carga.SituacaoRoteirizacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Aguardando)
                    {
                        carga.SituacaoRoteirizacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Aguardando;
                        await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, carga, null, Localization.Resources.Cargas.Carga.SolicitouNovaRoteirizacaoDaCarga, unitOfWork);
                        await repCarga.AtualizarAsync(carga);
                    }
                }
                else if ((carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte ||
                      carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada) &&
                      ConfiguracaoEmbarcador.PermiteAlterarRotaEmCargaFinalizada)
                {
                    carga.Rota = rotaFrete;
                    carga.AgSelecaoRotaOperador = false;

                    new Servicos.Embarcador.Logistica.RestricaoRodagem(unitOfWork).ValidaAtualizaZonaExclusaoRota(rotaFrete);

                    await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, carga, null, Localization.Resources.Cargas.Carga.InformouRotaDeFrete + " " + rotaFrete.Descricao + ".", unitOfWork);
                }
                else
                {
                    carga.PendenciaEmissaoAutomatica = false;

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = await repCargaPedido.BuscarPorCargaAsync(carga.Codigo);
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscals = await repPedidoXMLNotaFiscal.BuscarPorCargaAsync(carga.Codigo);
                    await Servicos.Embarcador.Carga.RotaFrete.SetarRotaCargaAsync(carga, cargaPedidos, pedidoXMLNotaFiscals, rotaFrete, ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware);
                    await servicoPedido.AtualizarLocalParqueamentoPedidoAsync(carga, cargaPedidos, TipoServicoMultisoftware, unitOfWork);
                    await serCargaDadosSumarizados.AlterarDadosSumarizadosCargaAsync(carga, cargaPedidos, ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware);

                    carga.AgSelecaoRotaOperador = false;

                    await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, carga, null, Localization.Resources.Cargas.Carga.InformouRotaDeFrete + " " + rotaFrete.Descricao + ".", unitOfWork);
                }

                await repCarga.AtualizarAsync(carga);

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(new { Codigo = rotaFrete?.Codigo ?? 0, Descricao = rotaFrete?.Descricao ?? "" });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoAlterarRota);
            }
            finally
            {
                await unitOfWork.CommitChangesAsync();
            }
        }

        public async Task<IActionResult> BuscarRotasAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork, cancellationToken);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NaoPermitidoAlterarRotaDaCarga);

                Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(codigoCarga);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = (List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>)
                    await repCargaPedido.BuscarPorCargaAsync(carga.Codigo);
                if (carga == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.CargaNaoEncontrada);

                if (
                    !carga.EmitindoCTes &&
                    !carga.CalculandoFrete &&
                    carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete &&
                    carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe &&
                    carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador &&
                    carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.SituacaoDaCargaNaoPermiteQueRotaSejaAlterada);


                await unitOfWork.StartAsync();

                if (ConfiguracaoEmbarcador.ObrigarSelecaoRotaQuandoExistirMultiplas && await new Servicos.Embarcador.Carga.RotaFrete(unitOfWork).PossuiVariasRotasFreteParaMesmaOrigemDestinoAsync(carga, cargaPedidos, ConfiguracaoEmbarcador, TipoServicoMultisoftware))
                {
                    carga.Rota = null;
                    carga.AgSelecaoRotaOperador = true;
                }
                else
                {
                    Dominio.Entidades.RotaFrete rotaFreteAnterior = carga.Rota;
                    await new Servicos.Embarcador.Carga.RotaFrete(unitOfWork).SetarRotaFreteCargaAsync(carga, cargaPedidos, ConfiguracaoEmbarcador, TipoServicoMultisoftware);

                    bool rotaModificada = (carga.Rota?.Codigo ?? 0) != (rotaFreteAnterior?.Codigo ?? 0);

                    if (carga.Rota != null)
                    {
                        if (rotaModificada && carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                        {
                            string retornoMontagem = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork).CalcularFreteTodoCarregamento(carga);
                            if (!string.IsNullOrWhiteSpace(retornoMontagem))
                            {
                                unitOfWork.Rollback();
                                return new JsonpResult(false, true, retornoMontagem);
                            }
                        }
                        carga.DataInicioCalculoFrete = DateTime.Now;
                        carga.CalculandoFrete = true;
                        carga.PendenciaEmissaoAutomatica = false;
                        carga.AgSelecaoRotaOperador = false;
                    }

                    await serCargaDadosSumarizados.AlterarDadosSumarizadosCargaAsync(carga, cargaPedidos, ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware);
                }

                await repCarga.AtualizarAsync(carga);
                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, carga, null, Localization.Resources.Cargas.Carga.BuscouRotaDaCarga, unitOfWork);

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(new
                {
                    Codigo = carga.Rota?.Codigo ?? 0,
                    Descricao = carga.Rota?.Descricao ?? "",
                    carga.AgSelecaoRotaOperador,
                    Fronteiras = carga.Rota != null ? (from o in carga.Rota.Fronteiras
                                                       select new
                                                       {
                                                           o.Cliente.Codigo,
                                                           o.Cliente.Nome,
                                                           o.Cliente.Descricao,
                                                           o.Cliente.Latitude,
                                                           o.Cliente.Longitude,
                                                       }).ToList() : null,
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoAlterarRota);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> BuscarRotasCompativeisAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork, cancellationToken);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 16, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Remetente, "Origem", 16, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Destinatario, "Destino", 16, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Distancia, "Quilometros", 6, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.TempoMinutos, "TempoDeViagemEmMinutos", 7, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho("FilialDistribuidora", false);
                }
                else
                {
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 40, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.FilialDistribuidora, "FilialDistribuidora", 20, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho("Origem", false);
                    grid.AdicionarCabecalho("Destino", false);
                    grid.AdicionarCabecalho("Quilometros", false);
                    grid.AdicionarCabecalho("TempoDeViagemEmMinutos", false);
                }
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.CodigoIntegracao, "CodigoIntegracao", 12, Models.Grid.Align.center, false);
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.GrupoDePessoa, "GrupoPessoa", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.TipoDeOperacao, "TipoOperacao", 12, Models.Grid.Align.center, false);

                int codigoCarga = Request.GetIntParam("Carga");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(codigoCarga);
                if (carga == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.CargaNaoEncontrada);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = await repCargaPedido.BuscarPorCargaAsync(carga.Codigo);

                List<Dominio.Entidades.RotaFrete> rotas = new List<Dominio.Entidades.RotaFrete>();

                if (ConfiguracaoEmbarcador.ObrigarSelecaoRotaQuandoExistirMultiplas)
                    rotas = await new Servicos.Embarcador.Carga.RotaFrete(unitOfWork).ObterRotasFreteParaMesmaOrigemDestinoAsync(carga, cargaPedidos, ConfiguracaoEmbarcador, TipoServicoMultisoftware);

                int quantidade = rotas.Count;

                var listaRotaFrete = (from p in rotas
                                      select new
                                      {
                                          p.Codigo,
                                          p.Descricao,
                                          Origem = p.Remetente?.Descricao ?? "",
                                          Destino = string.Join(", ", (from d in p.Destinatarios select d.Cliente.Descricao)),
                                          p.FilialDistribuidora,
                                          Quilometros = (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ? p.Quilometros.ToString("n0") : p.Quilometros.ToString("n2")),
                                          p.TempoDeViagemEmMinutos,
                                          GrupoPessoa = p.GrupoPessoas?.Descricao,
                                          p.DescricaoAtivo,
                                          TipoOperacao = p.TipoOperacao?.Descricao ?? "",
                                          CodigoIntegracao = p.CodigoIntegracao ?? ""
                                      }).ToList();

                grid.setarQuantidadeTotal(quantidade);
                grid.AdicionaRows(listaRotaFrete);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> PesquisarRotaAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unidadeDeTrabalho, cancellationToken);
                Repositorio.CargaPracaPedagio repCargaPracaPedagio = new Repositorio.CargaPracaPedagio(unidadeDeTrabalho, cancellationToken);

                Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = await repCargaRotaFrete.BuscarPorCargaAsync(codigoCarga);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(codigoCarga);

                var listaPracasRota = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota>();
                List<Dominio.Entidades.CargaPracaPedagio> cargaPracasPedagio = await repCargaPracaPedagio.BuscarPorCargaAsync(codigoCarga);

                foreach (var praca in cargaPracasPedagio)
                {

                    if ((praca.PracaPedagio.Latitude != null) && (praca.PracaPedagio.Longitude != null))
                    {
                        var lat = Convert.ToDouble(praca.PracaPedagio.Latitude.Replace('.', ','));
                        var lng = Convert.ToDouble(praca.PracaPedagio.Longitude.Replace('.', ','));

                        var pontos = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota
                        {
                            descricao = praca.PracaPedagio.Descricao,
                            lat = lat,
                            lng = lng,
                            pedagio = true,
                            pontopassagem = false
                        };

                        listaPracasRota.Add(pontos);
                    }
                }


                var retorno = new
                {
                    cargaRotaFrete.PolilinhaRota,
                    PontosDaRota = await Servicos.Embarcador.Carga.RotaFrete.ObterPontosPassagemCargaRotaFreteSerializadaAsync(cargaRotaFrete, unidadeDeTrabalho),
                    PracasPedagio = listaPracasRota,
                    PermiteReordenarEntregasCarga = carga?.TipoOperacao?.PermiteReordenarEntregasCarga ?? false
                };

                return new JsonpResult(retorno);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmErroAoBuscarPolilinha);
            }
            finally
            {
                await unidadeDeTrabalho.DisposeAsync();
            }
        }

        public async Task<IActionResult> SalvarOrdemEntregaAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                decimal km = Request.GetDecimalParam("Km") / 1000;
                int tempo = Request.GetIntParam("Tempo");
                string polilinha = Request.GetStringParam("PolilinhaRota");
                string pontosDaRota = Request.GetStringParam("PontosDaRota");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unidadeDeTrabalho, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeDeTrabalho, cancellationToken);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unidadeDeTrabalho, cancellationToken);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.CargaNaoEncontrada);

                if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete &&
                    carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe &&
                    carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador &&
                    carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.SituacaoDaCargaNaoPermiteQueRotaSejaAlterada);

                Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = await repCargaRotaFrete.BuscarPorCargaAsync(codigoCarga);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = (List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>)
                    await repCargaPedido.BuscarPorCargaAsync(carga.Codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> cargaPedidosXMLs = (List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>)
                    await repPedidoXMLNotaFiscal.BuscarPorCargaAsync(carga.Codigo);

                await unidadeDeTrabalho.StartAsync();

                cargaRotaFrete.PolilinhaRota = polilinha;
                cargaRotaFrete.TempoDeViagemEmMinutos = tempo;
                await repCargaRotaFrete.AtualizarAsync(cargaRotaFrete);

                if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                {
                    carga.Distancia = (int)km;
                    carga.CalculandoFrete = true;
                }

                List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosPassagem =
                    await Servicos.Embarcador.Carga.RotaFrete.SetarPontosPassagemCargaAsync(carga, cargaPedidos, cargaRotaFrete, pontosDaRota, null, null, unidadeDeTrabalho);
                await Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.GerarCargaEntregaAsync(carga, cargaPedidos, cargaPedidosXMLs, cargaRotaFrete, pontosPassagem, true, ConfiguracaoEmbarcador, unidadeDeTrabalho, TipoServicoMultisoftware);
                await Servicos.Embarcador.Carga.ValePedagio.CargaValePedagioRota.CriarCargaValePedagioPorRotaFreteAsync(carga, cargaPedidos, ConfiguracaoEmbarcador, unidadeDeTrabalho, TipoServicoMultisoftware);
                //setar a flag carga.IntegrandoValePedagio para true caso exista consultas valor pedagio
                await Servicos.Embarcador.Carga.ValePedagio.CargaValePedagioRota.SetarCargaIntegrandoConsultaValePedagioAsync(carga, unidadeDeTrabalho);

                Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unidadeDeTrabalho);
                await serCargaDadosSumarizados.AlterarDadosSumarizadosCargaAsync(carga, cargaPedidos, ConfiguracaoEmbarcador, unidadeDeTrabalho, TipoServicoMultisoftware);

                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, carga, null, Localization.Resources.Cargas.Carga.AlterouManualmenteOrdemDasEntregasDaCarga, unidadeDeTrabalho);

                await unidadeDeTrabalho.CommitChangesAsync();

                new Servicos.Embarcador.Hubs.Carga().InformarCargaAlterada(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada);

                return new JsonpResult(true);
            }
            catch (ServicoException ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmErroAoSalvarRotaDaCarga);
            }
            finally
            {
                await unidadeDeTrabalho.DisposeAsync();
            }
        }

        #endregion
    }
}
