using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;


namespace SGT.WebAdmin.Controllers.Cargas.Carga.Frete
{
    [CustomAuthorize(new string[] { "VerificarFrete" }, "Cargas/Carga", "Logistica/JanelaCarregamento", "Cargas/CargaFretePendente")]
    public class CargaFreteController : BaseController
    {
        #region Construtores

        public CargaFreteController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> RatearFreteEntreAsNotasAsync(CancellationToken cancellation)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork, cancellation);
                Repositorio.Embarcador.Cargas.CargaPedidoTabelaFreteCliente repCargaPedidoTabelaFreteCliente = new Repositorio.Embarcador.Cargas.CargaPedidoTabelaFreteCliente(unitOfWork, cancellation);
                Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork, cancellation);
                List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente> componentesPorCarga = new List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente>();
                Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork, cancellation);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork, cancellation);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = await repConfiguracao.BuscarConfiguracaoPadraoAsync();


                int.TryParse(Request.Params("Codigo"), out int codigo);


                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(codigo);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = await repCargaPedido.BuscarPorCargaAsync(carga.Codigo);

                bool rateioFreteFilialEmissora = false;
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidosComponentesFreteCarga = await repCargaPedidoComponenteFrete.BuscarPorCargaAsync(carga.Codigo, rateioFreteFilialEmissora, cancellation);
                Servicos.Embarcador.Carga.RateioNotaFiscal serRateioNotaFiscal = new Servicos.Embarcador.Carga.RateioNotaFiscal(unitOfWork);

                await unitOfWork.StartAsync();
                serRateioNotaFiscal.RatearFreteCargaPedidoEntreNotas(carga, cargaPedidos, cargaPedidosComponentesFreteCarga, rateioFreteFilialEmissora, TipoServicoMultisoftware, unitOfWork, configuracao);

                if (carga.EmpresaFilialEmissora != null)
                {
                    rateioFreteFilialEmissora = true;
                    cargaPedidosComponentesFreteCarga = await repCargaPedidoComponenteFrete.BuscarPorCargaAsync(carga.Codigo, rateioFreteFilialEmissora, cancellation);

                    serRateioNotaFiscal.RatearFreteCargaPedidoEntreNotas(carga, cargaPedidos, cargaPedidosComponentesFreteCarga, rateioFreteFilialEmissora, TipoServicoMultisoftware, unitOfWork, configuracao);

                }
                await unitOfWork.CommitChangesAsync();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao recalcular o frete.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ConfirmarConferenciaDeFreteAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repositorioCarga.BuscarPorCodigoAsync(codigo);
                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível localizar a carga");

                await unitOfWork.StartAsync();

                carga.ConfirmouConferenciaManualDeFrete = true;
                await repositorioCarga.AtualizarAsync(carga);

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(true);
            }
            catch (ServicoException ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao confirmar a conferência de frete.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> AplicarAjusteConferenciaDeFreteAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_CalcularFreteNovamente) || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(codigo);
                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível localizar a carga");

                dynamic listaConferenciaFrete = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaConferenciaFrete"));

                if (listaConferenciaFrete == null)
                    return new JsonpResult(false, true, "Não foi possível obter a lista de alterações.");

                await unitOfWork.StartAsync();

                decimal valorLiquitoTotal = 0;

                foreach (dynamic conferenciaFrete in listaConferenciaFrete)
                {
                    List<int> codigosPedidoXML = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>((string)conferenciaFrete.CodigosPedidoXML);

                    decimal valorLiquido = 0;
                    decimal.TryParse((string)conferenciaFrete.ValorLiquido, out valorLiquido);

                    valorLiquitoTotal += valorLiquido;

                    decimal valorFreteAjusteManual = 0;
                    if (codigosPedidoXML.Count() == 0)
                        valorFreteAjusteManual = valorLiquido;
                    else
                        valorFreteAjusteManual = Math.Round(valorLiquido / codigosPedidoXML.Count(), 2, MidpointRounding.AwayFromZero);

                    decimal valorLiquidoSomaItem = 0;
                    foreach (int codigoPedidoXML in codigosPedidoXML.OrderBy(o => o))
                    {
                        Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = await repPedidoXMLNotaFiscal.BuscarPorCodigoAsync(codigoPedidoXML, cancellationToken);

                        valorLiquidoSomaItem += valorFreteAjusteManual;
                        pedidoXMLNotaFiscal.ValorFreteAjusteManual = valorFreteAjusteManual;

                        if (codigoPedidoXML == codigosPedidoXML.Max(o => o) && valorLiquidoSomaItem != valorLiquido)
                        {
                            if (valorLiquidoSomaItem > valorLiquido)
                                pedidoXMLNotaFiscal.ValorFreteAjusteManual -= (valorLiquidoSomaItem - valorLiquido);
                            else
                                pedidoXMLNotaFiscal.ValorFreteAjusteManual += (valorLiquido - valorLiquidoSomaItem);
                        }

                        await repPedidoXMLNotaFiscal.AtualizarAsync(pedidoXMLNotaFiscal);
                    }
                }

                if (carga.ValorFrete != valorLiquitoTotal)
                    throw new ServicoException($"Soma dos itens {Math.Round(valorLiquitoTotal, 2)} diverge do valor líquido de frete {Math.Round(carga.ValorFrete, 2)}.");

                Servicos.Embarcador.Carga.Frete servicoFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork);
                servicoFrete.SolicitarRecalculoFrete(carga, Usuario, TipoServicoMultisoftware, Auditado, true, unitOfWork);

                await unitOfWork.CommitChangesAsync();

                Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete() { situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.CalculandoFrete };

                return new JsonpResult(retorno);
            }
            catch (ServicoException ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao recalcular o frete.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> AlterarFreteConferenciaDeFreteAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(codigo);
                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível localizar a carga");


                decimal Percentual = Request.GetDecimalParam("Percentual");

                if (Percentual <= 0)
                    return new JsonpResult(false, true, "Percentual não pode ser igual a zero.");

                if (Percentual > 100)
                    return new JsonpResult(false, true, "Percentual não pode ser maior que 100.");

                decimal ValorTotalLiquido = carga.ValorFrete;
                decimal NovoValorLiquido = (Percentual * ValorTotalLiquido) / 100;

                var dynAtualizarLinhaConferenciaDeFrete = new
                {
                    CodigosPedidoXML = Request.Params("CodigosPedidoXML"),
                    NumeroPedido = Request.Params("NumeroPedido"),
                    Remetente = Request.Params("Remetente"),
                    Destinatario = Request.Params("Destinatario"),
                    ValorLiquido = NovoValorLiquido.ToString("n2"),
                    Percentual = Percentual.ToString("n3"),
                    ValorComponentes = Request.Params("ValorComponentes"),
                    ValorImpostos = Request.Params("ValorImpostos"),
                    ValorTotal = Request.Params("ValorTotal"),
                };

                var retorno = new
                {
                    dynAtualizarLinhaConferenciaDeFrete
                };

                return new JsonpResult(retorno);
            }
            catch (ServicoException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar o percentual de frete.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> RecalcularFreteAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_CalcularFreteNovamente) || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigo = Request.GetIntParam("Codigo");
                int codigoTabelaFreteRota = Request.GetIntParam("TabelaFreteRota");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Frete.TabelaFreteRota repositorioTabelaFreteRota = new Repositorio.Embarcador.Frete.TabelaFreteRota(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repositorioCarga.BuscarPorCodigoAsync(codigo);
                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível localizar a carga");

                unitOfWork.Start();

                carga.TabelaFreteRota = codigoTabelaFreteRota > 0 ? await repositorioTabelaFreteRota.BuscarPorCodigoAsync(codigoTabelaFreteRota) : null;

                Servicos.Embarcador.Carga.Frete servicoFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork);
                servicoFrete.SolicitarRecalculoFrete(carga, Usuario, TipoServicoMultisoftware, Auditado, false, unitOfWork);

                await unitOfWork.CommitChangesAsync();

                Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete() { situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.CalculandoFrete };

                return new JsonpResult(retorno);
            }
            catch (ServicoException ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao recalcular o frete.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> RecalcularFreteBIDAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = await repositorioConfiguracao.BuscarConfiguracaoPadraoAsync();
                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repositorioCarga.BuscarPorCodigoAsync(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível localizar a carga.");

                await unitOfWork.StartAsync();

                Servicos.Embarcador.Carga.Frete servicoFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork, cancellationToken);
                servicoFrete.SolicitarRecalculoFreteBID(carga, Usuario, TipoServicoMultisoftware, Auditado);

                await unitOfWork.CommitChangesAsync();

                Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete() { situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.CalculandoFrete };

                return new JsonpResult(retorno);
            }
            catch (ServicoException ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao recalcular o frete pelo BID.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ObterListaValorFretePorPedidoAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCarga = Request.GetIntParam("Codigo");
                bool AlteraValorFreteFilialEmissora = Request.GetBoolParam("AlteraValorFreteFilialEmissora");

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork, cancellationToken);
                List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoValoresDeFrete> cargaPedidoValoresDeFrete = await repCargaPedido.BuscaCargaPedidoValoresDeFreteAsync(codigoCarga, cancellationToken);

                if (cargaPedidoValoresDeFrete == null || cargaPedidoValoresDeFrete.Count == 0)
                    return new JsonpResult(false, "Carga não possue pedidos.");
                else
                {
                    dynamic stageAgrupadasFormatadas = new
                    {
                        StagesColetas = new List<dynamic>()
                    };
                    int i = 0;
                    foreach (var item in cargaPedidoValoresDeFrete)
                    {
                        i++;
                        dynamic agrupamento = new
                        {
                            DT_Enable = true,
                            DT_RowClass = ObterCorAlteracaoFrete(item, AlteraValorFreteFilialEmissora),
                            CodigoCargaPedido = item.CodigoCargaPedido,
                            CodigoPedido = item.CodigoPedido,
                            NomeRemetente = item.NomeRemetente,
                            NomeDestinatario = item.NomeDestinatario,
                            NumeroPedido = item.NumeroPedido,
                            ValorFrete = item.ValorFrete.ToString("n2"),
                            ValorFreteFilialEmissora = item.ValorFreteFilialEmissora.ToString("n2"),
                            ValorFreteAntesDaAlteracaoManual = item.ValorFreteAntesDaAlteracaoManual.ToString("n2"),
                            ValorFreteFilialEmissoraAntesDaAlteracaoManual = item.ValorFreteFilialEmissoraAntesDaAlteracaoManual.ToString("n2"),
                            ValorFreteDatabase = item.ValorFreteDatabase.ToString("n2"),
                            ValorFreteFilialEmissoraDatabase = item.ValorFreteFilialEmissoraDatabase.ToString("n2")
                        };
                        stageAgrupadasFormatadas.StagesColetas.Add(agrupamento);
                    }

                    return new JsonpResult(stageAgrupadasFormatadas);

                }
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter as Configurações Gerais dos Pedidos.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> SalvarValorFretePorPedidoAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);
                //int.TryParse(Request.Params("TabelaFreteRota"), out int tabelaFrete);
                bool.TryParse(Request.Params("AlteraValorFrete"), out bool AlteraValorFrete);
                bool.TryParse(Request.Params("AlteraValorFreteFilialEmissora"), out bool AlteraValorFreteFilialEmissora);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Frete.TabelaFreteRota repTabelaFreteRota = new Repositorio.Embarcador.Frete.TabelaFreteRota(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = await repConfiguracaoTMS.BuscarConfiguracaoPadraoAsync();
                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(codigo);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem = Servicos.Embarcador.Carga.Carga.ObterCargasOrigem(carga, unitOfWork);

                if (carga.SituacaoCarga != SituacaoCarga.CalculoFrete)
                    return new JsonpResult(false, true, "Não é possível alterar o valor do frete na atual situação da carga (" + carga.DescricaoSituacaoCarga + ").");

                Servicos.Embarcador.Carga.Frete srvFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork, cancellationToken);
                Servicos.Embarcador.Carga.CargaAprovacaoFrete servicoCargaAprovacaoFrete = new Servicos.Embarcador.Carga.CargaAprovacaoFrete(unitOfWork, configuracaoTMS);
                Servicos.Embarcador.Pedido.CalculoFreteStagePedidoAgrupado servicoCalculoFreteStagesPedidos = new Servicos.Embarcador.Pedido.CalculoFreteStagePedidoAgrupado(unitOfWork, TipoServicoMultisoftware, configuracaoTMS);

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork, cancellationToken);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = await repCargaPedido.BuscarPorCargaAsync(carga.Codigo);
                List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoValoresDeFrete> lstCargaPedidoValoresDeFrete = new List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoValoresDeFrete>();
                dynamic lstValoresDeFretePorPedido = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("lstValoresDeFretePorPedido"));
                foreach (var item in lstValoresDeFretePorPedido)
                {
                    int codigoCargaPedido = (int)item.CodigoCargaPedido;
                    int codigoPedido = (int)item.CodigoPedido;
                    string nomeRemetente = item.NomeRemetente ?? "";
                    string nomeDestinatario = item.NomeDestinatario.Value ?? "";
                    string numeroPedido = item.NumeroPedido.Value ?? "";

                    decimal valorFrete = 0;
                    decimal valorFreteFilialEmissora = 0;
                    if (item.ValorFrete.Value is long)
                        valorFrete = (decimal)item.ValorFrete.Value;
                    if (item.ValorFrete.Value is string)
                        decimal.TryParse(item.ValorFrete.Value, out valorFrete);
                    if (item.ValorFreteFilialEmissora.Value is long)
                        valorFreteFilialEmissora = (decimal)item.ValorFreteFilialEmissora.Value;

                    decimal valorFreteAntesDaAlteracaoManual = 0;
                    decimal valorFreteFilialEmissoraAntesDaAlteracaoManual = 0;
                    decimal valorFreteDatabase = 0;
                    decimal valorFreteFilialEmissoraDatabase = 0;


                    if (item.ValorFreteAntesDaAlteracaoManual.Value is long)
                        valorFreteAntesDaAlteracaoManual = (decimal)item.ValorFreteAntesDaAlteracaoManual.Value;
                    else
                        decimal.TryParse(item.ValorFreteAntesDaAlteracaoManual.Value, out valorFreteAntesDaAlteracaoManual);

                    if (item.ValorFreteFilialEmissoraAntesDaAlteracaoManual.Value is long)
                        valorFreteFilialEmissoraAntesDaAlteracaoManual = (decimal)item.ValorFreteFilialEmissoraAntesDaAlteracaoManual.Value;
                    else
                        decimal.TryParse(item.ValorFreteFilialEmissoraAntesDaAlteracaoManual.Value, out valorFreteFilialEmissoraAntesDaAlteracaoManual);

                    if (item.ValorFreteDatabase.Value is long)
                        valorFreteDatabase = (decimal)item.ValorFreteDatabase.Value;
                    else
                        decimal.TryParse(item.ValorFreteDatabase.Value, out valorFreteDatabase);

                    if (item.ValorFreteFilialEmissoraDatabase.Value is long)
                        valorFreteFilialEmissoraDatabase = (decimal)item.ValorFreteFilialEmissoraDatabase.Value;
                    else
                        decimal.TryParse(item.ValorFreteFilialEmissoraDatabase.Value, out valorFreteFilialEmissoraDatabase);

                    lstCargaPedidoValoresDeFrete.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoValoresDeFrete()
                    {
                        CodigoCargaPedido = codigoCargaPedido,
                        CodigoPedido = codigoPedido,
                        NomeRemetente = nomeRemetente,
                        NomeDestinatario = nomeDestinatario,
                        NumeroPedido = numeroPedido,
                        ValorFrete = valorFrete,
                        ValorFreteFilialEmissora = valorFreteFilialEmissora,
                        ValorFreteAntesDaAlteracaoManual = valorFreteAntesDaAlteracaoManual,
                        ValorFreteFilialEmissoraAntesDaAlteracaoManual = valorFreteFilialEmissoraAntesDaAlteracaoManual,
                        ValorFreteDatabase = valorFreteDatabase,
                        ValorFreteFilialEmissoraDatabase = valorFreteFilialEmissoraDatabase
                    });
                }

                await unitOfWork.StartAsync();
                srvFrete.SalvarValorManualFretePorPorPedido(lstCargaPedidoValoresDeFrete, carga, cargasOrigem, cargaPedidos, unitOfWork, AlteraValorFrete, AlteraValorFreteFilialEmissora, TipoServicoMultisoftware, configuracaoTMS);

                await repCarga.AtualizarAsync(carga);
                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, carga, null, "Alterado valor do frete manualmente.", unitOfWork);
                await unitOfWork.CommitChangesAsync();
                Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete() { situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.CalculandoFrete };
                return new JsonpResult(retorno);
            }
            catch (ServicoException ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar valor do frete.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        /// <summary>
        /// Remove o vinculo do encaixe para que o valor do pedido seja calculado normalmente e rateiado normalmente pela tabela de frete.
        /// </summary>
        /// <param name="carga"></param>
        /// <param name="unitOfWork"></param>
        public async Task<IActionResult> InformarValorFreteManualAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");

                Dominio.ObjetosDeValor.WebService.Rest.Frete.DadosInformarValorFreteOperador dadosInformarValorFreteOperador = new Dominio.ObjetosDeValor.WebService.Rest.Frete.DadosInformarValorFreteOperador
                {
                    CodigoCarga = Request.GetIntParam("Carga"),
                    CodigoMotivo = Request.GetIntParam("Motivo"),
                    ValorFrete = Request.GetDecimalParam("ValorFrete"),
                    Observacao = Request.GetStringParam("Observacao"),
                    FreteFilialEmissoraOperador = Request.GetBoolParam("FreteFilialEmissora"),
                    Moeda = Request.GetEnumParam<MoedaCotacaoBancoCentral>("Moeda"),
                    ValorCotacaoMoeda = Request.GetDecimalParam("ValorCotacaoMoeda"),
                    ValorTotalMoeda = Request.GetDecimalParam("ValorTotalMoeda")
                };

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(dadosInformarValorFreteOperador.CodigoCarga);
                Servicos.Embarcador.Frete.TabelaFreteCliente serTabelaFreteCliente = new Servicos.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = await ObterOperadorLogisticaAsync(unitOfWork);

                if (carga.TabelaFrete != null)
                {
                    if ((!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_AlterarValorFreteApenasComTabelaFrete) && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_AlterarValorFrete)) || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                        return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");
                }
                else
                {
                    if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_AlterarValorFrete) || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                        return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");
                }

                await serTabelaFreteCliente.InformarValorManualAsync(dadosInformarValorFreteOperador, carga, operadorLogistica, cancellationToken, TipoServicoMultisoftware, ConfiguracaoEmbarcador, Usuario, Auditado);

                new Servicos.Embarcador.Integracao.IntegracaoCarga(unitOfWork, TipoServicoMultisoftware).ReenviarIntegracoesCargaDadosTransportePosFreteAsync(carga, unitOfWork).GetAwaiter().GetResult();

                return new JsonpResult("Valor do frete informado com sucesso.");
            }
            catch (BaseException ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao re-calcular o frete.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> VerificarFreteAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Frete.ContratoFreteTransportador repositorioContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork, cancellationToken);

                Servicos.Embarcador.Carga.Frete serCargaFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork, TipoServicoMultisoftware);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = await repositorioConfiguracaoPedido.BuscarConfiguracaoPadraoAsync();
                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoFetchAsync(codigo, cancellationToken);
                bool permiteAlterarValorFretePedidoPosCalculoFrete = carga?.TabelaFrete?.PermiteAlterarValorFretePedidoPosCalculoFrete ?? false;

                Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = null;
                bool cargaDeTrecho = false;

                if (repositorioTipoIntegracao.ExistePorTipo(TipoIntegracao.Unilever) && carga.DadosSumarizados.CargaTrecho == CargaTrechoSumarizada.SubCarga)
                {
                    Repositorio.Embarcador.Pedidos.StageAgrupamento repositorioStageAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(unitOfWork, cancellationToken);
                    Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento agrupamentoPrincipal = await repositorioStageAgrupamento.BuscarPrimeiroPorCargaGeradaAsync(carga.Codigo, cancellationToken);

                    if (agrupamentoPrincipal != null)
                    {
                        bool possuiValorFrete = carga.ValorFrete > 0 && carga.ValorFreteLiquido > 0;
                        if (((carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete && agrupamentoPrincipal.ValorFreteTotal == 0) || carga.CalculandoFrete) && !possuiValorFrete)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete;

                            retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete()
                            {
                                situacao = situacao,
                                mensagem = agrupamentoPrincipal.MensagemRetornoDadosFrete,
                                Moeda = carga.Moeda ?? MoedaCotacaoBancoCentral.Real,
                                ValorCotacaoMoeda = carga.ValorCotacaoMoeda ?? 0m
                            };
                            cargaDeTrecho = true;
                        }
                    }
                }

                if (((carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete && carga.PossuiPendencia && carga.ValorFreteTabelaFrete == 0) || carga.CalculandoFrete) && !cargaDeTrecho)
                {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete;

                    if (carga.CalculandoFrete)
                    {
                        situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.CalculandoFrete;
                    }

                    retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete()
                    {
                        situacao = situacao,
                        mensagem = carga.MotivoPendencia,
                        Moeda = carga.Moeda ?? MoedaCotacaoBancoCentral.Real,
                        ValorCotacaoMoeda = carga.ValorCotacaoMoeda ?? 0m
                    };
                }
                else if (!cargaDeTrecho)
                {
                    retorno = serCargaFrete.VerificarFrete(ref carga, unitOfWork, configuracaoPedido, false, this.Usuario);
                }

                if (retorno.situacao != SituacaoRetornoDadosFrete.FreteValido)
                {
                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                    if ((configuracaoEmbarcador?.ValidarVeiculoVinculadoContratoDeFrete ?? false) && (carga.Veiculo != null))
                    {
                        Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contratoFreteTransportador = repositorioContratoFreteTransportador.BuscarContratosPorVeiculo(DateTime.Now, carga.Veiculo.Codigo);
                        if (contratoFreteTransportador != null)
                            retorno.VeiculoPossuiContratoFrete = true;
                    }

                    if (retorno.situacao == SituacaoRetornoDadosFrete.CalculandoFrete && (configuracaoEmbarcador.ExigirCargaRoteirizada || (carga.TipoOperacao?.ExigirCargaRoteirizada ?? false)) && (carga.SituacaoRoteirizacaoCarga == SituacaoRoteirizacao.Erro || carga.SituacaoRoteirizacaoCarga == SituacaoRoteirizacao.SemDefinicao))
                        retorno.PermiteRoterizarNovamente = true;

                }
                retorno.PermiteAlterarValorFretePedidoPosCalculoFrete = permiteAlterarValorFretePedidoPosCalculoFrete;
                retorno.ExigirConferenciaManual = carga.CargaOrigemPedidos.FirstOrDefault(o => (o.FormulaRateio?.ExigirConferenciaManual ?? false) == true) != null ? true : false;
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar as pendências no frete.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaConferenciaDeFrete()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisaPesquisaConferenciaDeFrete());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> ObterDadosRateioFreteAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repositorioCarga.BuscarPorCodigoAsync(codigoCarga);

                if (!(carga.TipoOperacao?.PermiteEmitirCargaDiferentesOrigensParcialmente ?? false) || (carga.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn))
                    return new JsonpResult(new List<dynamic>());

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork, cancellationToken);
                List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoDadosRateioFrete> listaDadosRateio = await repositorioCargaPedido.BuscarDadosRateioPorCargaEmitidaParcialmenteAsync(carga.Codigo, cancellationToken);

                if (listaDadosRateio.Count == 0)
                    return new JsonpResult(new List<dynamic>());

                List<DateTime> datasEmissaoCte = listaDadosRateio.Where(dados => dados.DataEmissaoCte.HasValue).Select(dados => dados.DataEmissaoCte.Value).Distinct().ToList();
                List<dynamic> listaDadosRateioRetornar = new List<dynamic>();

                foreach (DateTime dataEmissaoCte in datasEmissaoCte)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoDadosRateioFrete> listaDadosRateioPorDataEmissaoCte = listaDadosRateio.Where(dados => !dados.DataPedidoAdicionado.HasValue || dados.DataPedidoAdicionado < dataEmissaoCte).ToList();
                    long ordemAtual = listaDadosRateioPorDataEmissaoCte.Where(dados => dados.DataEmissaoCte == dataEmissaoCte).Min(dados => dados.Ordem);
                    Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoDadosRateioFrete dadosRateioPorOrdemAtual = listaDadosRateioPorDataEmissaoCte.Where(dados => dados.Ordem == ordemAtual).First();
                    List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoDadosRateioFrete> listaDadosRateioPorOrdemAtual = listaDadosRateioPorDataEmissaoCte.Where(dados => dados.Ordem >= ordemAtual).ToList();
                    decimal valorFreteTotal = listaDadosRateioPorOrdemAtual.Sum(dados => dados.ValorFrete);
                    decimal pesoTotal = listaDadosRateioPorOrdemAtual.Sum(dados => (dados.DataEmissaoCte == dataEmissaoCte) ? dados.PesoParaCalculo : dados.PesoPedidoParaCalculo);
                    List<dynamic> pedidosRetornar = new List<dynamic>();

                    foreach (Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoDadosRateioFrete dados in listaDadosRateioPorDataEmissaoCte)
                    {
                        if (dados.Ordem >= ordemAtual)
                        {
                            bool dadosCteAtual = (dados.DataEmissaoCte == dataEmissaoCte);
                            decimal pesoParaCalculo = dadosCteAtual ? dados.PesoParaCalculo : dados.PesoPedidoParaCalculo;
                            decimal percentualRateio = (pesoParaCalculo / pesoTotal);
                            decimal valorFrete = (percentualRateio * valorFreteTotal);

                            pedidosRetornar.Add(new
                            {
                                Ordem = dados.Ordem,
                                NumeroPedido = dados.NumeroPedido,
                                NumeroStage = dados.NumeroStage,
                                PesoNota = dadosCteAtual ? dados.PesoParaCalculo.ToString("n3") : "",
                                PesoPedido = dados.PesoPedidoParaCalculo.ToString("n3"),
                                Percentual = (percentualRateio * 100).ToString("n3"),
                                Valor = valorFrete.ToString("n2"),
                                DT_RowColor = dadosCteAtual ? "#dff0d8" : "",
                                DT_FontColor = dadosCteAtual ? "#212529" : "",
                            });
                        }
                        else
                        {
                            pedidosRetornar.Add(new
                            {
                                Ordem = dados.Ordem,
                                NumeroPedido = dados.NumeroPedido,
                                NumeroStage = dados.NumeroStage,
                                PesoNota = dados.PesoParaCalculo.ToString("n3"),
                                PesoPedido = dados.PesoPedidoParaCalculo.ToString("n3"),
                                Percentual = "",
                                Valor = dados.ValorFrete.ToString("n2"),
                                DT_RowColor = "#bebebe",
                                DT_FontColor = "#212529"
                            });
                        }
                    }

                    listaDadosRateioRetornar.Add(new
                    {
                        NumeroCte = dadosRateioPorOrdemAtual.NumeroCte,
                        ValorCte = dadosRateioPorOrdemAtual.ValorCte.ToString("n2"),
                        Pedidos = pedidosRetornar
                    });
                }

                return new JsonpResult(listaDadosRateioRetornar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar o racional do rateio do frete.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ObterDadosComposicaoRateioFreteAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repositorioPontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repositorioCarga.BuscarPorCodigoAsync(codigoCarga);
                List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoComposicaoRateioFrete> listaDadosRateio = await repositorioCargaPedido.BuscarDadosComposicaoRateioFreteAsync(carga.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosPassagem = await repositorioPontosPassagem.BuscarPorCargaAsync(carga.Codigo);
                List<dynamic> pedidosRetornar = new List<dynamic>();

                if (listaDadosRateio.Count == 0)
                    return new JsonpResult(new List<dynamic>());

                foreach (Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoComposicaoRateioFrete dados in listaDadosRateio)
                {
                    int distancia = 0;
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = await repositorioCargaPedido.BuscarPorCodigoAsync(dados.CodigoPedido);
                    if (cargaPedido != null)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem pontoPassagem = pontosPassagem.Where(o => o.Cliente != null && ((cargaPedido.Recebedor != null && cargaPedido.Recebedor.CPF_CNPJ == o.Cliente.CPF_CNPJ) || (cargaPedido.Recebedor == null && cargaPedido.Pedido.Destinatario.CPF_CNPJ == o.Cliente.CPF_CNPJ))).FirstOrDefault();
                        if (cargaPedido.FormulaRateio == null || pontoPassagem == null)
                            distancia = 0;
                        else
                        {
                            distancia = (int)Math.Round(pontoPassagem.DistanciaDireta / 1000.0, MidpointRounding.AwayFromZero);
                            if (pontoPassagem.DistanciaDireta > 0 && pontoPassagem.DistanciaDireta < 500)
                                distancia = 1;
                        }
                    }

                    pedidosRetornar.Add(new
                    {
                        DisponibilizarComposicaoRateioCarga = carga.TipoOperacao.ConfiguracaoEmissao != null ? carga.TipoOperacao.ConfiguracaoEmissao.DisponibilizarComposicaoRateioCarga : false,
                        dados.NumeroPedido,
                        dados.DescricaoRateio,
                        PesoPedido = dados.PesoPedido.ToString("n2"),
                        DistanciaPedido = distancia,
                        TaxaElemento = dados.TaxaElemento.ToString("n2"),
                        ValorPedido = dados.ValorPedido.ToString("n2"),
                        ValorCalculado = dados.ValorCalculado.ToString("n2"),
                        FatorPonderacao = ((dados.PesoPedido + (dados.Peso * (dados.TaxaElemento / 100))) * distancia).ToString("n2"),
                        dados.CodigoTabela,
                        dados.Origem,
                        dados.Destino,
                        DT_RowColor = "#bebebe",
                        DT_FontColor = "#212529"
                    });
                }

                return new JsonpResult(pedidosRetornar);

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados da composição do rateio do frete.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> SolicitarRoteirizacaoCargaAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Frete.ContratoFreteTransportador repositorioContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork, cancellationToken);
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = await repConfiguracao.BuscarConfiguracaoPadraoAsync();

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int.TryParse(Request.Params("Codigo"), out int codigo); ;

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(codigo);

                if (carga.CargaAgrupamento != null || carga.CargaVinculada != null)
                    return new JsonpResult(false, true, "A carga foi agrupada, sendo assim não é possível alterá-la.");

                if (!await serCarga.VerificarSeCargaEstaNaLogisticaAsync(carga, TipoServicoMultisoftware))
                    return new JsonpResult(false, true, "Não é buscar a rota na atual situação da carga (" + carga.DescricaoSituacaoCarga + ").");

                if (configuracao.ExigirRotaRoteirizadaNaCarga && !(carga.TipoOperacao?.NaoExigeRotaRoteirizada ?? false) && (carga.Rota == null || carga.Rota.SituacaoDaRoteirizacao != SituacaoRoteirizacao.Concluido))
                    return new JsonpResult(false, true, "Necessário que a rota frete esteja roteirizada, favor verificar o cadastro da rota.");

                if (carga.SituacaoRoteirizacaoCarga != SituacaoRoteirizacao.Aguardando)
                {
                    carga.SituacaoRoteirizacaoCarga = SituacaoRoteirizacao.Aguardando;
                    await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, carga, null, "Solicitou nova roteirização da carga.", unitOfWork);
                    await repCarga.AtualizarAsync(carga);
                }

                Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete() { situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.CalculandoFrete };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao solicitar a roteirização da carga.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> LiberarUsoContratoFreteCargaAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Frete.ContratoFreteTransportador repositorioContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork, cancellationToken);
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Frete.TabelaFreteRota repTabelaFreteRota = new Repositorio.Embarcador.Frete.TabelaFreteRota(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = await repConfiguracaoTMS.BuscarConfiguracaoPadraoAsync();

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(codigo);


                if (carga.CargaAgrupamento != null || carga.CargaVinculada != null)
                    return new JsonpResult(false, true, "A carga foi agrupada, sendo assim não é possível alterá-la.");

                if (!await serCarga.VerificarSeCargaEstaNaLogisticaAsync(carga, TipoServicoMultisoftware))
                    return new JsonpResult(false, true, "Não é possível liberar o contrato na atual situação da carga (" + carga.DescricaoSituacaoCarga + ").");

                if (carga.CalculandoFrete)
                    return new JsonpResult(false, true, "Não é possível liberar o contrato enquanto a carga estiver sendo calculada.");

                if ((configuracaoTMS?.ValidarVeiculoVinculadoContratoDeFrete ?? false) && (carga.Veiculo != null))
                {
                    Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contratoFreteTransportador = repositorioContratoFreteTransportador.BuscarContratosPorVeiculo(DateTime.Now, carga.Veiculo.Codigo);
                    if (contratoFreteTransportador != null)
                    {
                        List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasFreteContrato = repTabelaFrete.BuscarPorContratoFreteTransportador(contratoFreteTransportador.Codigo);
                        if (tabelasFreteContrato.Count == 0)
                            return new JsonpResult(false, true, "Não existe nenhuma tabela de frete para o contrato de frete " + contratoFreteTransportador.Descricao + ".");
                        if (tabelasFreteContrato.Count > 1)
                            return new JsonpResult(false, true, "Existe mais de uma tabela de frete para o contrato de frete " + contratoFreteTransportador.Descricao + ".");

                        carga.TabelaFrete = tabelasFreteContrato.FirstOrDefault();
                        carga.ContratoFreteTransportador = contratoFreteTransportador;
                        carga.FixarUtilizacaoContratoTransportador = true;
                    }
                    else
                    {
                        return new JsonpResult(false, true, "O veiculo da carga não possui um contrato de frete.");
                    }
                }
                else
                {
                    return new JsonpResult(false, true, "Ação inválida, a configuração para tal não existe ou foi desativada.");
                }

                carga.DataInicioCalculoFrete = DateTime.Now;
                carga.DadosPagamentoInformadosManualmente = false;
                carga.CalculandoFrete = true;
                carga.PendenciaEmissaoAutomatica = false;
                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, carga, null, "Liberou a utilização do contrato do transportador manualmente nesta carga.", unitOfWork);
                await repCarga.AtualizarAsync(carga);

                Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete() { situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.CalculandoFrete };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao recalcular o frete.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> LiberarSemConfirmacaoERPAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_LiberarCargaSemConfirmacaoERP))
                    return new JsonpResult(false, true, "Você não possui permissão para executar esta ação.");

                int codigoCarga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(codigoCarga);
                unitOfWork.Start();

                repCargaPedido.SetarCienciaDoEnvioInformado(codigoCarga);
                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, carga, null, "Liberou carga mesmo sem a confirmação do ERP.", unitOfWork);

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao tentar liberar a carga sem confirmação do ERP.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ValidarInicioEmissaoDocumentosAsync()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga");

                int codigoCarga = Request.GetIntParam("Carga");

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                dynamic retorno = servicoCarga.ValidarInicioEmissaoDocumentos(codigoCarga, TipoServicoMultisoftware, unitOfWork, permissoesPersonalizadas);

                return new JsonpResult(retorno);
            }
            catch (ServicoException ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Não foi possível validar a carga para realizar o envio dos documentos.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> AutorizarDiferencaValorFreteAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_PermiteLiberarComDiferencaNoValorFrete))
                    return new JsonpResult(false, true, "Você não possui permissão para executar esta ação.");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);

                int codigoCarga = Request.GetIntParam("Carga");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(codigoCarga);

                if (carga.CargaAgrupamento != null || carga.CargaVinculada != null)
                    return new JsonpResult(false, true, "A carga foi agrupada, sendo assim não é possível alterá-la.");

                await unitOfWork.StartAsync();

                carga.BloqueadaDiferencaValorFrete = false;

                await repCarga.AtualizarAsync(carga);

                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, carga, null, "Autorizou a emissão com diferença no valor do frete.", unitOfWork);

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, "Ocorreu uma falha ao autorizar o valor do frete.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ConfirmarFreteIniciarEmissaoAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_AutorizarEmissaoDocumentos) && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Pedidos.TipoOperacaoTipoCarga repositorioTipoOperacaoTipoCarga = new Repositorio.Embarcador.Pedidos.TipoOperacaoTipoCarga(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaFreteIntegracao repositorioCargaFreteIntegracao = new Repositorio.Embarcador.Cargas.CargaFreteIntegracao(unitOfWork, cancellationToken);

                int codigoCarga = Request.GetIntParam("Carga");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(codigoCarga);

                List<int> tiposCargaTipoOperacao = await repositorioTipoOperacaoTipoCarga.BuscarCodigosTiposCargasPorTipoOperacaoAsync(carga.TipoOperacao?.Codigo ?? 0, cancellationToken);

                if (tiposCargaTipoOperacao.Count > 0 && carga.TipoDeCarga != null)
                    if (tiposCargaTipoOperacao.Contains(carga.TipoDeCarga.Codigo))
                        return new JsonpResult(false, true, "Esse tipo de carga não permite que os Documentos sejam emitidos.");

                if (((carga.TipoOperacao?.AvancarEtapaFreteAutomaticamente ?? false) || (carga.TipoOperacao?.AvancarCargaAutomaticaAposMontagem ?? false)) &&
                    !carga.NaoAvancarAutomaticamenteEtapaFretePorPendencia &&
                    !carga.CargaSVM &&
                    !carga.PossuiPendencia &&
                    (carga.SituacaoAlteracaoFreteCarga == SituacaoAlteracaoFreteCarga.NaoInformada || carga.SituacaoAlteracaoFreteCarga == SituacaoAlteracaoFreteCarga.Aprovada || carga.SituacaoAlteracaoFreteCarga == SituacaoAlteracaoFreteCarga.Reprovada))
                    return new JsonpResult(false, true, "O avanço da carga ocorrerá automaticamente, não sendo possível realizar esta ação.");

                if (carga.ModeloVeicularCarga != null && carga.ModeloVeicularCarga.AlertarOperadorPesoExcederCapacidade)
                {
                    var PesoCarga = carga.DadosSumarizados != null ? carga.DadosSumarizados.PesoLiquidoTotal : 0;
                    var capacidadeModeloVeicular = carga.ModeloVeicularCarga.CapacidadePesoTransporte + carga.ModeloVeicularCarga.ToleranciaPesoExtra;
                    if (PesoCarga > capacidadeModeloVeicular)
                        //o usuario foi alertado e liberou, sistema deve armazenar na auditoria da carta que o usuario liberou a emissão;
                        await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, carga, null, "Liberou a emissão da carga ciente que o peso era superior a capacidade do modelo veicular", unitOfWork);
                }

                if ((carga.TipoOperacao?.ExigeProcImportacaoPedido ?? false) && await repCargaPedido.PedidosNaoPossuiProcImportacaoAsync(carga.Codigo, cancellationToken))
                    return new JsonpResult(false, true, "Existem pedidos na carga que não possuem Proc. Importação, não sendo possível realizar esta ação.");

                if (carga.CargaOrigemPedidos.FirstOrDefault(o => (o.FormulaRateio?.ExigirConferenciaManual ?? false) == true) != null && !carga.ConfirmouConferenciaManualDeFrete)
                    return new JsonpResult(false, true, "Necessário efetuar conferência manual de frete, não sendo possível realizar esta ação.");

                bool sucesso = svcCarga.IniciarEmissaoDocumentosCarga(out string mensagem, out object retorno, carga, Usuario, Auditado, ConfiguracaoEmbarcador, TipoServicoMultisoftware, unitOfWork, WebServiceConsultaCTe, permissoesPersonalizadas);

                if (retorno != null)
                    return new JsonpResult(retorno, sucesso, mensagem);
                else
                    return new JsonpResult(sucesso, true, mensagem);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao autorizar a emissão.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ConfirmarFreteIniciarEtapaTransportador(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_AutorizarEmissaoDocumentos) && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);

                int codigoCarga = Request.GetIntParam("Carga");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repositorioCarga.BuscarPorCodigoAsync(codigoCarga);

                if (carga.ExigeNotaFiscalParaCalcularFrete)
                    throw new ControllerException("Esta ação não é permitida, pois exige nota fiscal para cálcular o frete.");

                if (carga.SituacaoCarga != SituacaoCarga.CalculoFrete)
                    throw new ControllerException($"Não é possível avançar de frete para transportador, pois a carga está na situação: {carga.SituacaoCarga.ObterDescricao()}");

                await unitOfWork.StartAsync(cancellationToken);

                carga.SituacaoCarga = SituacaoCarga.AgTransportador;

                await repositorioCarga.AtualizarAsync(carga);

                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, carga, "Avançou a carga da etapa de frete para transportador.", unitOfWork, cancellationToken);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
                serHubCarga.InformarCargaAtualizada(carga.Codigo, TipoAcaoCarga.Alterada, unitOfWork.StringConexao);

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao confirmar frete e avançar para etapa transportador.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> DownloadDetalhamentoFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                bool filialEmissora = Request.GetBoolParam("FilialEmissora");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                byte[] relatorio = new Servicos.Embarcador.Carga.Frete(unitOfWork).GeraComposicaoDeFrete(carga, filialEmissora);

                return Arquivo(relatorio, "application/pdf", "Composição do Frete da Carga " + carga.CodigoCargaEmbarcador + ".pdf");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, "Ocorreu uma falha realizar o download da composição do frete.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarMoedaCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_AlterarMoeda))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                if (!ConfiguracaoEmbarcador.UtilizaMoedaEstrangeira)
                    return new JsonpResult("Não é possível utilizar moeda estrangeira neste ambiente.");

                int codigoCarga = Request.GetIntParam("Codigo");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? moeda = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral>("Moeda");
                decimal valorCotacaoMoeda = Request.GetDecimalParam("ValorCotacaoMoeda");

                if (valorCotacaoMoeda <= 0m)
                    return new JsonpResult(false, true, "O valor da cotação da moeda deve ser maior que zero.");

                if (!moeda.HasValue)
                    return new JsonpResult(false, true, "Moeda inválida.");

                if (!new Servicos.Embarcador.Carga.Moeda(unitOfWork, Auditado).AlterarMoedaCarga(out string erro, codigoCarga, moeda.Value, valorCotacaoMoeda))
                    return new JsonpResult(false, true, erro);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar a moeda da carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverPreCalculoFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPreCalculoFrete repCargaPreCalculoFrete = new Repositorio.Embarcador.Cargas.CargaPreCalculoFrete(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigo);

                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Servicos.Embarcador.Carga.CargaAprovacaoFrete servicoCargaAprovacaoFrete = new Servicos.Embarcador.Carga.CargaAprovacaoFrete(unitOfWork, configuracaoTMS);

                if (servicoCargaAprovacaoFrete.IsUtilizarAlcadaAprovacaoAlteracaoValorFrete() && (carga.SituacaoAlteracaoFreteCarga == SituacaoAlteracaoFreteCarga.Aprovada))
                    return new JsonpResult(false, true, "O valor de frete está aprovado e não pode mais ser alterado.");

                if (carga.CargaAgrupamento != null || carga.CargaVinculada != null)
                    return new JsonpResult(false, true, "A carga foi agrupada, sendo assim não é possível alterá-la.");

                unitOfWork.Start();

                carga.CargaPossuiPreCalculoFrete = false;

                if (repCargaPreCalculoFrete.ExistePorCarga(carga.Codigo))
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPreCalculoFrete cargaPreCalculo = repCargaPreCalculoFrete.BuscarPorCarga(carga.Codigo);
                    repCargaPreCalculoFrete.Deletar(cargaPreCalculo);
                }

                repCarga.Atualizar(carga);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Removeu o pré calculo do frete.", unitOfWork);

                unitOfWork.CommitChanges();

                Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete() { situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.CalculandoFrete };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao recalcular o frete.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RecriarFreteRacionalCargasConsolidado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCargaDT = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCargaDT);

                Repositorio.Embarcador.Pedidos.StageAgrupamento repStageAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> stagesAgrupamentos = repStageAgrupamento.BuscarPorCargaDt(carga.Codigo);

                foreach (var stageAgrupamento in stagesAgrupamentos)
                {
                    if (stageAgrupamento.CargaGerada != null)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> todosCargaPedidoCargaGerada = repCargaPedido.BuscarPorCarga(stageAgrupamento.CargaGerada.Codigo);

                        if (!(stageAgrupamento.CargaGerada.TipoOperacao?.ExigeNotaFiscalParaCalcularFrete ?? false) && stageAgrupamento.CargaGerada.SituacaoCarga != SituacaoCarga.AgIntegracao && stageAgrupamento.CargaGerada.SituacaoCarga != SituacaoCarga.EmTransporte && stageAgrupamento.CargaGerada.SituacaoCarga != SituacaoCarga.AgImpressaoDocumentos)
                        {
                            stageAgrupamento.CargaGerada.SituacaoCarga = SituacaoCarga.CalculoFrete;
                            stageAgrupamento.CargaGerada.CalculandoFrete = true;
                            stageAgrupamento.CargaGerada.PossuiPendencia = false;
                            stageAgrupamento.CargaGerada.ProblemaIntegracaoValePedagio = false;
                            stageAgrupamento.CargaGerada.MotivoPendencia = "";
                            stageAgrupamento.CargaGerada.MotivoPendenciaFrete = MotivoPendenciaFrete.NenhumPendencia;
                            stageAgrupamento.CargaGerada.DadosPagamentoInformadosManualmente = false;
                            stageAgrupamento.CargaGerada.DataInicioCalculoFrete = DateTime.Now;
                            stageAgrupamento.CargaGerada.PendenciaEmissaoAutomatica = false;

                            Servicos.Auditoria.Auditoria.Auditar(Auditado, stageAgrupamento.CargaGerada, null, "Recalculo do frete ao recriar racional do frete do consolidado", unitOfWork);
                        }

                        repCarga.Atualizar(stageAgrupamento.CargaGerada);
                    }
                }

                return new JsonpResult(true);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, "Ocorreu uma falha ao recalcular frete cargas racional frete.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaCargaFretePendente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga()
                {
                    DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                    DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                    CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                    CodigosFilial = Request.GetListParam<int>("Filial"),
                    CodigosTipoOperacao = Request.GetListParam<int>("TipoOperacao"),
                    CodigosTransportador = Request.GetListParam<int>("Transportador"),
                    SituacaoCarga = SituacaoCarga.CalculoFrete,
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "DataEmissao", 5, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Nº da Carga", "CodigoCargaEmbarcador", 5, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Origem e Destino", "OrigemDestino", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Transportador", "Transportador", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Motivo Pendêcia", "Motivo", 15, Models.Grid.Align.left, false);

                Servicos.Embarcador.Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = repositorioCarga.ContarCargasFretePendente(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCarga = (totalRegistros > 0) ? repositorioCarga.BuscarCargasFretePendente(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

                var listaCargaRetornar = (
                     from carga in listaCarga
                     select new
                     {
                         carga.Codigo,
                         carga.CodigoCargaEmbarcador,
                         DataEmissao = carga.DataCriacaoCarga.ToString("dd/MM/yyyy"),
                         Filial = carga.Filial != null ? carga.Filial.Descricao : "",
                         OrigemDestino = servicoCargaDadosSumarizados.ObterOrigemDestinos(carga, true, TipoServicoMultisoftware),
                         Transportador = carga.Empresa != null ? carga.Empresa.RazaoSocial : string.Empty,
                         Motivo = !string.IsNullOrWhiteSpace(carga.MotivoPendencia) ? carga.MotivoPendencia : ""
                     }
                 ).ToList();

                grid.AdicionaRows(listaCargaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaComponenteImpostosFretePedido()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisaPesquisaComponenteImpostosFretePedido());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> ExportarPesquisaComponenteImpostosFretePedido()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisaPesquisaComponenteImpostosFretePedido();
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", "ComponenteImpostosFretePedido." + grid.extensaoCSV);

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaComponenteImpostosFretePedidoNotaFiscal()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisaPesquisaComponenteImpostosFretePedidoNotaFiscal());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisaComponenteImpostosFretePedidoNotaFiscal()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisaPesquisaComponenteImpostosFretePedidoNotaFiscal();
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", "ComponenteImpostosFretePedidoNotaFiscal." + grid.extensaoCSV);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        public async Task<IActionResult> ReprocessarFreteCargas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                List<int> ListaCodigosCargasProcessar = new List<int>();

                bool selecionarTodos = Request.GetBoolParam("SelecionarTodos");
                var listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>((string)Request.Params("ItensSelecionados"));

                if (selecionarTodos)
                {
                    //buscar os primeiros 1000 registros;
                    Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                    {
                        InicioRegistros = 0
                    };

                    Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga()
                    {
                        DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                        DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                        CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                        CodigosFilial = Request.GetListParam<int>("Filial"),
                        CodigosTipoOperacao = Request.GetListParam<int>("TipoOperacao"),
                        SituacaoCarga = SituacaoCarga.CalculoFrete,
                    };

                    List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCarga = repositorioCarga.BuscarCargasFretePendente(filtrosPesquisa, parametrosConsulta);
                    List<int> listaCodigosCarga = listaCarga.Select(x => x.Codigo).Where(x => !listaItensSelecionados.Contains(x)).ToList();

                    ListaCodigosCargasProcessar = listaCodigosCarga;
                }
                else
                    ListaCodigosCargasProcessar = listaItensSelecionados;


                repositorioCarga.AtualizarReprocessamentoCalculoFreteCargas(ListaCodigosCargasProcessar);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar frete das cargas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RecalcularPrecalculo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int.TryParse(Request.Params("Carga"), out int codigoCarga);

                Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporte = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao = repositorioCargaDadosTransporte.BuscarPorCargaETipoIntegracao(codigoCarga, TipoIntegracao.Unilever);

                if (cargaDadosTransporteIntegracao == null)
                    return new JsonpResult(false, "Registros não encontrados");

                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "";

                if (cargaDadosTransporteIntegracao.NumeroTentativas >= 3)
                    cargaDadosTransporteIntegracao.NumeroTentativas = 2;

                repositorioCargaDadosTransporte.Atualizar(cargaDadosTransporteIntegracao);

                return new JsonpResult(true, "Pre calculo enviado para recalcular");
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar precalculo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais

        #region Métodos Globais Exclusivo TMS

        public async Task<IActionResult> RetornarEtapaNotaFiscaTMS()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                Servicos.Embarcador.Carga.CargaAprovacaoFrete servicoCargaAprovacaoFrete = new Servicos.Embarcador.Carga.CargaAprovacaoFrete(unitOfWork);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_RetornarEtapaNotasFiscais))
                        return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");
                }

                int codigo = int.Parse(Request.Params("Carga"));

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPreviaDocumento repCargaPreviaDocumento = new Repositorio.Embarcador.Cargas.CargaPreviaDocumento(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega repOcorrenciaColetaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalContaContabilContabilizacao repPedidoXMLNotaFiscalContaContabilContabilizacao = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalContaContabilContabilizacao(unitOfWork);
                Repositorio.Embarcador.Logistica.PermanenciaCliente repPermanenciaCliente = new Repositorio.Embarcador.Logistica.PermanenciaCliente(unitOfWork);
                Repositorio.Embarcador.Logistica.PermanenciaSubarea repPermanenciaSubarea = new Repositorio.Embarcador.Logistica.PermanenciaSubarea(unitOfWork);
                Repositorio.Embarcador.Logistica.AlertaMonitor repAlertaMonitor = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);
                Repositorio.Embarcador.Integracao.IntegracaoAVIPED repIntegracaoAVIPED = new Repositorio.Embarcador.Integracao.IntegracaoAVIPED(unitOfWork);

                Servicos.Embarcador.Seguro.Seguro serSeguro = new Servicos.Embarcador.Seguro.Seguro(unitOfWork);
                Servicos.Embarcador.Carga.CTeSubContratacao serCTeSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(unitOfWork);
                Servicos.Embarcador.CTe.CTEsImportados serCTEsImportados = new Servicos.Embarcador.CTe.CTEsImportados(unitOfWork);
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigo);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS &&
                    repCargaPedido.ExisteCTeEmitidoNoEmbarcador(carga.Codigo) &&
                    repCargaDocumentoParaEmissaoNFSManual.ExistePorCarga(carga.Codigo))
                    return new JsonpResult(false, true, "Não é possível retornar a etapa de uma carga importada do embarcador que tenha documentos para emissão de NFS Manual vinculados.");

                if (carga.CargaAgrupamento != null || carga.CargaVinculada != null)
                    return new JsonpResult(false, true, "A carga foi agrupada, sendo assim não é possível alterá-la.");

                if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                    return new JsonpResult(false, true, "Não é possível retornar para a etapa de documentos na atual situação da carga (" + carga.DescricaoSituacaoCarga + ").");

                if (carga.CalculandoFrete)
                    return new JsonpResult(false, true, "A carga está em processo de cálculo de valores do frete, não sendo possível retornar para a etapa de documentos.");


                if (serCarga.VerificarSeCargaEmEmissao(carga, TipoServicoMultisoftware))
                    return new JsonpResult(false, true, "Não é possível retornar a etapa de documentos pois a carga já está em processo de emissão.");

                unitOfWork.Start();

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = repCargaEntrega.BuscarPorCarga(carga.Codigo);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in cargaEntregas)
                    {
                        repOcorrenciaColetaEntrega.ExcluirTodosPorCargaEntrega(cargaEntrega.Codigo);
                        repPermanenciaSubarea.ExcluirTodosPorCargaEntrega(cargaEntrega.Codigo);
                        repPermanenciaCliente.ExcluirTodosPorCargaEntrega(cargaEntrega.Codigo);
                        repAlertaMonitor.ExcluirTodosPorCargaEntrega(cargaEntrega.Codigo);

                        cargaEntrega.NotasFiscais = null;
                        cargaEntrega.Pedidos = null;

                        repCargaEntrega.Deletar(cargaEntrega);
                    }
                }
                else
                {
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
                    repCargaEntregaNotaFiscal.ExcluirTodosPorCarga(carga.Codigo);

                    if (carga.CargaAgrupada)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOriginais = repCarga.BuscarCargasOriginais(carga.Codigo);
                        foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaOriginal in cargasOriginais)
                            repCargaEntregaNotaFiscal.ExcluirTodosPorCarga(cargaOriginal.Codigo);
                    }
                }

                repPedidoXMLNotaFiscalContaContabilContabilizacao.DeletarPorCarga(carga.Codigo);

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && carga.TipoOperacao != null && !carga.TipoOperacao.PermiteImportarDocumentosManualmente)
                {
                    carga.DataEnvioUltimaNFe = DateTime.Now;
                    carga.DataRecebimentoUltimaNFe = DateTime.Now;
                    carga.DataInicioEmissaoDocumentos = DateTime.Now;
                }

                carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe;
                carga.DadosPagamentoInformadosManualmente = false;
                carga.CargaRetornadaEtapaNFeManualmente = true;

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo, TipoPedido.Entrega, true, true);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    if (cargaPedido.CTeEmitidoNoEmbarcador)
                    {
                        serCTEsImportados.RemoverDocumentosParaRetornarEtapa(cargaPedido, unitOfWork);
                    }
                    else if (cargaPedido.Pedido.PedidoTransbordo)
                    {
                        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in cargaPedido.NotasFiscais)
                        {
                            repPedidoXMLNotaFiscalComponenteFrete.DeletarPorPedidoXMLNotaFiscal(pedidoXMLNotaFiscal.Codigo, false);
                            repIntegracaoAVIPED.DeletarPorPedidoXMLNotaFiscal(pedidoXMLNotaFiscal.Codigo);
                            repPedidoXMLNotaFiscal.Deletar(pedidoXMLNotaFiscal);
                        }
                    }
                    else
                    {
                        if ((cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SVMTerceiro
                            || cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada
                            || cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho
                            || cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario
                            || cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SVMProprio
                            || cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
                            && !cargaPedido.PedidoEncaixado && !cargaPedido.EmitirComplementarFilialEmissora) //o luli deixou eu fazer dia 25/09
                            serCTeSubContratacao.ExcluirNotasFiscaisDaCarga(cargaPedido, unitOfWork);

                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> xmlNotasFiscaisPedidosExistentes = repPedidoXMLNotaFiscalComponenteFrete.BuscarTodosdaCargaPedido(cargaPedido.Codigo);
                        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete xmlNotasFiscaisPedidoExistente in xmlNotasFiscaisPedidosExistentes)
                            repPedidoXMLNotaFiscalComponenteFrete.Deletar(xmlNotasFiscaisPedidoExistente);

                        cargaPedido.PedidoSemNFe = false;
                        cargaPedido.SituacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.AgEnvioNF;
                        repCargaPedido.Atualizar(cargaPedido);
                    }
                }

                repCargaPreviaDocumento.DeletarPorCarga(carga.Codigo);

                serSeguro.ExtornarAutorizacoesApolices(carga, unitOfWork, "A carga foi retornada para a etapa de documentos pelo operador " + this.Usuario.Nome + ", desta forma o valor da mercadoria pode ser afetado sendo necessário uma nova autorização.");
                Servicos.Embarcador.Veiculo.Veiculo.ExtornarAutorizacoesPeso(carga, unitOfWork, "A carga foi retornada para a etapa de documentos pelo operador " + this.Usuario.Nome + ", desta forma o peso pode ser afetado sendo necessário uma nova autorização.");
                Servicos.Embarcador.Frota.OrdemServicoManutencao.ExtornarAutorizacoesServicoVeiculo(carga, unitOfWork, "A carga foi retornada para a etapa de documentos pelo operador " + this.Usuario.Nome + ", desta forma o veículo pode ser alterado, sendo necessário uma nova autorização.");
                Servicos.Embarcador.Financeiro.Pagamento.ExtornarAutorizacoesValorMaximoPendentePagamento(carga, unitOfWork, "A carga foi retornada para a etapa de documentos pelo operador " + this.Usuario.Nome + ", desta forma o valor pendente de pagamento pode ser alterado, sendo necessário uma nova autorização.");
                servicoCargaAprovacaoFrete.RemoverAprovacao(carga);
                repCarga.Atualizar(carga);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Retornou para etapa de documentos.", unitOfWork);

                unitOfWork.CommitChanges();

                Servicos.Embarcador.Hubs.Carga svcHubCarga = new Servicos.Embarcador.Hubs.Carga();
                svcHubCarga.InformarCargaAtualizada(carga.Codigo, TipoAcaoCarga.Alterada, _conexao.StringConexao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha retornar Etapa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaCargaFreteIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho("Integração", "TipoIntegracao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº Tentativas", "NumeroTentativas", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data do Envio", "DataIntegracao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "SituacaoIntegracao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Retorno", "Retorno", 30, Models.Grid.Align.left, false);


                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaFreteIntegracao filtroPesquisa = ObterFiltroPesquisaIntegracao();
                Repositorio.Embarcador.Cargas.CargaFreteIntegracao repositorioCargaFreteIntegracao = new Repositorio.Embarcador.Cargas.CargaFreteIntegracao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao> listaIntegracoes = repositorioCargaFreteIntegracao.Consultar(filtroPesquisa, grid.ObterParametrosConsulta());
                int totalIntegracoes = repositorioCargaFreteIntegracao.ContarConsulta(filtroPesquisa);

                var listaIntegracoesRetornar = (
                    from integracao in listaIntegracoes
                    select new
                    {
                        integracao.Codigo,
                        Situacao = integracao.SituacaoIntegracao,
                        SituacaoIntegracao = integracao.DescricaoSituacaoIntegracao,
                        TipoIntegracao = integracao.TipoIntegracao.DescricaoTipo,
                        Retorno = integracao.ProblemaIntegracao,
                        integracao.NumeroTentativas,
                        DataIntegracao = integracao.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss"),
                        DT_RowColor = integracao.SituacaoIntegracao.ObterCorLinha(),
                        DT_FontColor = integracao.SituacaoIntegracao.ObterCorFonte(),
                    }
                ).ToList();

                grid.AdicionaRows(listaIntegracoesRetornar);
                grid.setarQuantidadeTotal(totalIntegracoes);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarHistoricoIntegracaoCargaFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaFreteIntegracao repositorioCargaFreIntegracao = new Repositorio.Embarcador.Cargas.CargaFreteIntegracao(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao existeCargaFrete = repositorioCargaFreIntegracao.BuscarPorCodigo(codigo, false);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> integracoesArquivos = existeCargaFrete.ArquivosTransacao.OrderByDescending(obj => obj.Data).ToList();

                grid.setarQuantidadeTotal(integracoesArquivos.Count);

                var retorno = integracoesArquivos
                    .Select(obj => new
                    {
                        obj.Codigo,
                        Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                        obj.DescricaoTipo,
                        obj.Mensagem
                    })
                    .ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        [AllowAuthenticate]
        public async Task<IActionResult> ObterTotaisIntegracoesCargaFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaFreteIntegracao repositorioCargaFreteIntegracao = new Repositorio.Embarcador.Cargas.CargaFreteIntegracao(unitOfWork);

                int totalAguardandoIntegracao = repositorioCargaFreteIntegracao.ContarConsulta(new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaFreteIntegracao() { Codigo = codigo, Situacao = SituacaoIntegracao.AgIntegracao });
                int totalIntegrado = repositorioCargaFreteIntegracao.ContarConsulta(new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaFreteIntegracao() { Codigo = codigo, Situacao = SituacaoIntegracao.Integrado });
                int totalProblemaIntegracao = repositorioCargaFreteIntegracao.ContarConsulta(new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaFreteIntegracao() { Codigo = codigo, Situacao = SituacaoIntegracao.ProblemaIntegracao });
                int totalAguardandoRetorno = repositorioCargaFreteIntegracao.ContarConsulta(new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaFreteIntegracao() { Codigo = codigo, Situacao = SituacaoIntegracao.AgRetorno });

                var retorno = new
                {
                    TotalAguardandoIntegracao = totalAguardandoIntegracao,
                    TotalAguardandoRetorno = totalAguardandoRetorno,
                    TotalIntegrado = totalIntegrado,
                    TotalProblemaIntegracao = totalProblemaIntegracao,
                    TotalGeral = totalAguardandoIntegracao + totalIntegrado + totalProblemaIntegracao + totalAguardandoRetorno
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoObterOsTotaisDasIntegracoes);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReenviarIntegracaoCargaFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaFreteIntegracao repositorioCargaFreteIntegracao = new Repositorio.Embarcador.Cargas.CargaFreteIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao integracao = repositorioCargaFreteIntegracao.BuscarPorCodigo(codigo, false);

                if (integracao == null)
                    return new JsonpResult(false, true, "Registro Integração não encontrado");

                integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                integracao.ProblemaIntegracao = "";
                integracao.Carga.AguardandoIntegracaoFrete = true;

                repositorioCargaFreteIntegracao.Atualizar(integracao);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao.Carga, null, "Reenviou as integrações da Carga", unitOfWork);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoIntegracao = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repositorioCtArquivos = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo cargaFreteIntegracao = repositorioCtArquivos.BuscarPorCodigo(codigoIntegracao, false);

                if (cargaFreteIntegracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao> aquivosBaixar = new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>();

                aquivosBaixar.Add(cargaFreteIntegracao.ArquivoRequisicao);
                aquivosBaixar.Add(cargaFreteIntegracao.ArquivoResposta);

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(aquivosBaixar);

                return Arquivo(arquivo, "application/zip", "Arquivos Integração Frete.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos xmls de integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> LiberarSemIntegracaoCargaFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigo);
                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                if (carga == null)
                    return new JsonpResult(false, "Carga não encontrada");

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_PermitirLiberarSemIntegracaoFrete))
                    return new JsonpResult(false, "Você não possui permissão para realizar esta ação");

                carga.LiberadaComPendenciaIntegracaoFrete = true;
                repositorioCarga.Atualizar(carga);

                string message = string.Empty;

                if (!servicoCarga.IniciarEmissaoDocumentosCarga(out message, out object retorno, carga, Usuario, Auditado, ConfiguracaoEmbarcador, TipoServicoMultisoftware, unitOfWork, WebServiceConsultaCTe, permissoesPersonalizadas))
                    return new JsonpResult(false, message);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Carga Liberada sem integração Frete", unitOfWork);

                return new JsonpResult(true, true, "Etapa Avançada");

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos xmls de integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais Exclusivo TMS

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaFreteIntegracao ObterFiltroPesquisaIntegracao()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaFreteIntegracao()
            {
                Codigo = Request.GetIntParam("Codigo"),
                Situacao = Request.GetNullableEnumParam<SituacaoIntegracao>("Situacao")
            };
        }

        private string ObterCorAlteracaoFrete(Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoValoresDeFrete obj, bool AlteraValorFreteFilialEmissora)
        {
            if (AlteraValorFreteFilialEmissora && obj.FreteFilialEmissoraAlteradoManualmente())
                return ClasseCorFundo.Sucess(IntensidadeCor._100);
            else if (!AlteraValorFreteFilialEmissora && obj.FreteAlteradoManualmente())
                return ClasseCorFundo.Sucess(IntensidadeCor._100);
            return null;
        }

        private Models.Grid.Grid ObterGridPesquisaPesquisaComponenteImpostosFretePedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                // Busca Dados
                int totalRegistros = 0;
                int codigo = Request.GetIntParam("Codigo");

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoPedido", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.NumeroPedido, "NumeroPedido", 5, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.BaseCalculo, "BaseCalculo", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Regra, "Regra", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Aliquota, "Aliquota", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.ValorICMS, "ValorICMS", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.ValorISS, "ValorISS", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.RegraICMS, "RegraICMS", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Tomador, "Tomador", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.CFOP, "CFOP", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.CST, "CST", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor IBS UF", "ValorIBSUF", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor IBS Municipal", "ValorIBSMunicipal", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor CBS", "ValorCBS", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Valor, "Valor", 15, Models.Grid.Align.left, false);

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(codigo, TipoPedido.Entrega, true, true);

                totalRegistros = repCargaPedido.ContarPorCarga(codigo);

                var lista = (from obj in cargaPedidos
                             select new
                             {
                                 obj.Codigo,
                                 CodigoPedido = obj.Pedido?.Codigo,
                                 NumeroPedido = obj.Pedido?.Numero,
                                 ValorICMS = obj.ValorICMS,
                                 ValorISS = obj.ValorISS,
                                 RegraICMS = obj.RegraICMS?.Descricao ?? "",
                                 CST = obj.CST ?? string.Empty,
                                 CFOP = obj.CFOP?.Descricao ?? string.Empty,
                                 BaseCalculo = ObterBaseCalculoPorCargaPedido(obj),
                                 Regra = obj.RegraICMS?.Descricao != null ? obj.RegraICMS.Descricao : string.Empty,
                                 Aliquota = obj.PercentualAliquota != 0 ? obj.PercentualAliquota : obj?.PercentualAliquotaISS ?? 0,
                                 Valor = obj.ValorICMS != 0 ? obj.ValorICMS : obj?.ValorISS ?? 0,
                                 Tomador = obj.Pedido?.ObterTomador()?.Descricao ?? string.Empty,
                                 ValorIBSUF = obj.ValorIBSEstadual.ToString("n2"),
                                 AliquotaIBSUF = obj.AliquotaIBSEstadual,
                                 ValorIBSMunicipal = obj.ValorIBSMunicipal.ToString("n2"),
                                 AliquotaIBSMunicipal = obj.AliquotaIBSMunicipal,
                                 ValorCBS = obj.ValorCBS.ToString("n2"),
                                 AliquotaCBS = obj.AliquotaCBS,
                             }).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private static decimal ObterBaseCalculoPorCargaPedido(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            bool naoIncluirValorICMSBasecalculoQuandoValorFreteInformadoOperador = cargaPedido.Carga.TipoOperacao?.ConfiguracaoCalculoFrete?.NaoIncluirValorICMSBasecalculoQuandoValorFreteInformadoOperador ?? false;

            if (naoIncluirValorICMSBasecalculoQuandoValorFreteInformadoOperador && cargaPedido.Carga.TipoFreteEscolhido == TipoFreteEscolhido.Operador)
                return cargaPedido.ValorFrete;

            return cargaPedido?.BaseCalculoICMS != 0 ? cargaPedido.BaseCalculoICMS : 0;
        }

        private Models.Grid.Grid ObterGridPesquisaPesquisaComponenteImpostosFretePedidoNotaFiscal()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                // Busca Dados
                int totalRegistros = 0;
                int codigo = Request.GetIntParam("CodigoPedido");

                grid.AdicionarCabecalho("CodigoPedido", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.NumeroDaNotaFiscal, "NumeroNotaFiscal", 5, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.BaseCalculo, "BaseCalculo", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Regra, "Regra", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Aliquota, "Aliquota", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.ValorICMS, "ValorICMS", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.ValorISS, "ValorISS", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.RegraICMS, "RegraICMS", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.CFOP, "CFOP", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.CST, "CST", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor IBS UF", "ValorIBSUF", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor IBS Municipal", "ValorIBSMunicipal", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor CBS", "ValorCBS", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Valor, "Valor", 15, Models.Grid.Align.left, false);

                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscais = repPedidoNotaFiscal.BuscarPorPedido(codigo);
                totalRegistros = repPedidoNotaFiscal.BuscarPorPedido(codigo).Count();

                var lista = (from obj in pedidoXMLNotaFiscais
                             select new
                             {
                                 CodigoPedido = obj.Codigo,
                                 NumeroNotaFiscal = obj.XMLNotaFiscal?.Numero,
                                 ValorICMS = obj.ValorICMS,
                                 ValorISS = obj.ValorISS,
                                 RegraICMS = obj.RegraICMS?.DescricaoRegra ?? "",
                                 CST = obj.CST ?? string.Empty,
                                 CFOP = obj.CFOP?.Descricao ?? string.Empty,
                                 BaseCalculo = obj.BaseCalculoICMS != 0 ? obj.BaseCalculoICMS : obj?.BaseCalculoISS ?? 0,
                                 Regra = obj.RegraICMS?.DescricaoRegra != null ? obj.RegraICMS.Descricao : string.Empty,
                                 Aliquota = obj.PercentualAliquota != 0 ? obj.PercentualAliquota : obj?.PercentualAliquotaISS ?? 0,
                                 Valor = obj.ValorICMS != 0 ? obj.ValorICMS : obj?.ValorISS ?? 0,
                                 ValorIBSUF = obj.ValorIBSEstadual.ToString("n2"),
                                 ValorIBSMunicipal = obj.ValorIBSMunicipal.ToString("n2"),
                                 ValorCBS = obj.ValorCBS.ToString("n2")
                             }).ToList();

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Models.Grid.Grid ObterGridPesquisaPesquisaConferenciaDeFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                // Busca Dados
                int totalRegistros = 0;
                int codigoCarga = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                Models.Grid.EditableCell campoDecimal = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aDecimal, 7, 3);

                grid.AdicionarCabecalho("CodigosPedidoXML", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Pedido, "NumeroPedido", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Remetente, "Remetente", 20, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Destinatario, "Destinatario", 20, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.ValorLiquido, "ValorLiquido", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Percentual, "Percentual", 15, Models.Grid.Align.left, false, false, false, false, true, campoDecimal);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.ValorComponentes, "ValorComponentes", 15, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.ValorImpostos, "ValorImpostos", 15, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.ValorTotal, "ValorTotal", 15, Models.Grid.Align.left, false, false);

                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorCarga(codigoCarga);

                var queryGroup = pedidosXMLNotaFiscal.GroupBy(x => new { x.CargaPedido, x.XMLNotaFiscal.Emitente, x.XMLNotaFiscal.Destinatario })
                    .Select(y => new { CargaPedido = y.Key.CargaPedido, Emitente = y.Key.Emitente, Destinatario = y.Key.Destinatario, listFrete = y });

                totalRegistros = queryGroup.Count();

                dynamic lista = (from obj in queryGroup
                                 select new
                                 {
                                     CodigosPedidoXML = Newtonsoft.Json.JsonConvert.SerializeObject(obj.listFrete.Select(o => o.Codigo)),
                                     NumeroPedido = obj.CargaPedido.Pedido.Numero,
                                     Remetente = $"{obj.Emitente.Nome} ({obj.Emitente.CPF_CNPJ_Formatado})",
                                     Destinatario = $"{obj.Destinatario.Nome} ({obj.Destinatario.CPF_CNPJ_Formatado})",
                                     ValorLiquido = obj.listFrete.Sum(o => o.ValorFrete),
                                     Percentual = ((obj.listFrete.Sum(o => o.ValorFrete) * 100) / carga.ValorFrete).ToString("n3"),
                                     ValorComponentes = obj.listFrete.Sum(o => o.ValorTotalComponentes),
                                     ValorImpostos = obj.listFrete.Sum(o => o.ValorICMS) + obj.listFrete.Sum(o => o.ValorISS),
                                     ValorTotal = obj.listFrete.Sum(o => o.ValorFrete) + obj.listFrete.Sum(o => o.ValorTotalComponentes) + obj.listFrete.Sum(o => o.ValorICMS) + obj.listFrete.Sum(o => o.ValorISS)
                                 }).ToList();

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Privados
    }
}
