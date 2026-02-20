using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/GuaritaTMS")]
    public class GuaritaTMSController : BaseController
    {
		#region Construtores

		public GuaritaTMSController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoVeiculo = 0, codigoCarga = 0, codigoOrdemServico = 0, codigoMotorista = 0, kmAtual = 0;
                int.TryParse(Request.Params("Veiculo"), out codigoVeiculo);
                int.TryParse(Request.Params("Carga"), out codigoCarga);
                int.TryParse(Request.Params("OrdemServico"), out codigoOrdemServico);
                int.TryParse(Request.Params("Motorista"), out codigoMotorista);
                int.TryParse(Request.Params("KMAtual"), out kmAtual);

                List<int> reboques = Request.GetListParam<int>("Reboque");

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                TipoEntradaSaida tipoEntradaSaida;
                Enum.TryParse(Request.Params("TipoEntradaSaida"), out tipoEntradaSaida);

                DateTime dataInicial, dataFinal;
                DateTime.TryParse(Request.Params("DataInicial"), out dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out dataFinal);

                String placaVeiculoTerceiro = Request.Params("PlacaVeiculoTerceiro").ToString();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº Frota", "NumeroFrota", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Reboques", "Reboques", 10, Models.Grid.Align.left, false, false, true, false, true);
                grid.AdicionarCabecalho("Nº Frota Reboques", "NumeroFrotaReboques", 10, Models.Grid.Align.left, false, false, true);
                grid.AdicionarCabecalho("Status Veículo", "DescricaoVeiculoVazio", 10, Models.Grid.Align.left, false, true, true);
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    grid.AdicionarCabecalho("Carga", "Carga", 10, Models.Grid.Align.left, false, true, true);
                grid.AdicionarCabecalho("Data", "DataSaidaEntrada", 10, Models.Grid.Align.center, true, true, true);
                grid.AdicionarCabecalho("KM", "KMAtual", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Situação", "DescricaoTipoEntradaSaida", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Tipo Veículo", "DescricaoTipoVeiculo", 15, Models.Grid.Align.left, true);

                string propOrdena = SetarPropriedadeOrdenacao(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.Embarcador.Logistica.GuaritaTMS repGuaritaTMS = new Repositorio.Embarcador.Logistica.GuaritaTMS(unitOfWork);

                List<Dominio.Entidades.Embarcador.Logistica.GuaritaTMS> listaGuaritaTMS = repGuaritaTMS.Consultar(codigoVeiculo, codigoCarga, codigoOrdemServico, codigoMotorista, kmAtual, tipoEntradaSaida, dataInicial, dataFinal, codigoEmpresa, reboques, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repGuaritaTMS.ContarConsulta(codigoVeiculo, codigoCarga, codigoOrdemServico, codigoMotorista, kmAtual, tipoEntradaSaida, dataInicial, dataFinal, codigoEmpresa, reboques));

                var lista = (from p in listaGuaritaTMS
                             select new
                             {
                                 p.Codigo,
                                 Veiculo = p.Veiculo?.Placa ?? p.PlacaVeiculoTerceiro,
                                 NumeroFrota = p.Veiculo?.NumeroFrota ?? string.Empty,
                                 DescricaoVeiculoVazio = p.VeiculoVazio ? "Vazio" : "Carregado",
                                 Carga = p.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                                 DataSaidaEntrada = p.DataSaidaEntrada.ToString("dd/MM/yyyy"),
                                 KMAtual = p.KMAtual.ToString("n0"),
                                 DescricaoTipoEntradaSaida = p.TipoEntradaSaida.ObterDescricao(),
                                 Reboques = p.Reboques.Count > 0 ? string.Join(", ", p.Reboques.Select(o => o.Placa)) : string.Empty,
                                 NumeroFrotaReboques = p.Reboques.Count > 0 ? string.Join(", ", p.Reboques.Where(o => !string.IsNullOrWhiteSpace(o.NumeroFrota)).Select(o => o.NumeroFrota)) : string.Empty,
                                 DescricaoTipoVeiculo = p.TipoVeiculo == 0 ? TipoPropriedadeVeiculo.Proprio.ObterDescricao() : p.TipoVeiculo.ObterDescricao(),
                             }).ToList();

                grid.AdicionaRows(lista);

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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Logistica.GuaritaTMS repGuaritaTMS = new Repositorio.Embarcador.Logistica.GuaritaTMS(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServicoFrota = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unitOfWork);
                Repositorio.Embarcador.Veiculos.SituacaoVeiculo repSituacaoVeiculo = new Repositorio.Embarcador.Veiculos.SituacaoVeiculo(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.GestaoPatio.CheckListTipo repCheckListTipo = new Repositorio.Embarcador.GestaoPatio.CheckListTipo(unitOfWork);

                int.TryParse(Request.Params("Operador"), out int codigoOperador);
                int.TryParse(Request.Params("Veiculo"), out int codigoVeiculo);
                int.TryParse(Request.Params("Empresa"), out int codigoEmpresa);
                int.TryParse(Request.Params("Carga"), out int codigoCarga);
                int.TryParse(Request.Params("OrdemServicoFrota"), out int codigoOrdemServico);
                int.TryParse(Request.Params("Motorista"), out int codigoMotorista);
                int.TryParse(Request.Params("KMAtual"), out int kmAtual);
                int.TryParse(Request.Params("CheckListTipo"), out int checkListTipo);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Enum.TryParse(Request.Params("TipoEntradaSaida"), out TipoEntradaSaida tipoEntradaSaida);

                DateTime.TryParseExact(Request.Params("DataSaidaEntrada"), "dd/MM/yyyy", null, DateTimeStyles.None, out DateTime dataSaidaEntrada);

                TimeSpan.TryParseExact(Request.Params("HoraSaidaEntrada"), "g", null, out TimeSpan horaSaidaEntrada);

                string observacao = Request.Params("Observacao");

                bool.TryParse(Request.Params("FinalizouViagem"), out bool finalizouViagem);
                bool.TryParse(Request.Params("RetornouComReboque"), out bool retornouComReboque);
                bool.TryParse(Request.Params("HorarioInformadoManualmente"), out bool horarioInformadoManualmente);
                bool.TryParse(Request.Params("VeiculoVazio"), out bool veiculoVazio);
                bool.TryParse(Request.Params("GerarCheckList"), out bool gerarCheckList);
                bool.TryParse(Request.Params("EntrouCarregado"), out bool entrouCarregado);
                bool.TryParse(Request.Params("AlterarSituacaoVeiculoParaLiberado"), out bool alterarSituacaoVeiculoParaLiberado);
                bool.TryParse(Request.Params("AlterarReboquesVeiculo"), out bool alterarReboquesVeiculo);
                string motoristaTerceiro = Request.GetStringParam("MotoristaTerceiro");


                Enum.TryParse(Request.Params("TipoVeiculo"), out TipoPropriedadeVeiculo tipoVeiculo);
                string placaVeiculoTerceiro = Request.Params("PlacaVeiculoTerceiro");

                string erro = string.Empty;
                if (!ValidarLancamentoGuarita(kmAtual, codigoVeiculo, tipoEntradaSaida, 0, codigoCarga, tipoVeiculo, unitOfWork, out erro))
                    return new JsonpResult(false, erro);
                if (dataSaidaEntrada.Date > DateTime.Now.Date)
                    return new JsonpResult(false, "A data não pode ser maior que a atual.");

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Logistica.GuaritaTMS guaritaTMS = new Dominio.Entidades.Embarcador.Logistica.GuaritaTMS();

                guaritaTMS.Carga = repCarga.BuscarPorCodigo(codigoCarga);
                guaritaTMS.DataLancamento = DateTime.Now;
                guaritaTMS.HorarioInformadoManualmente = horarioInformadoManualmente;

                if (horarioInformadoManualmente)
                {
                    guaritaTMS.DataSaidaEntrada = dataSaidaEntrada;
                    guaritaTMS.HoraSaidaEntrada = horaSaidaEntrada;
                }
                else
                {
                    guaritaTMS.DataSaidaEntrada = DateTime.Now;
                    guaritaTMS.HoraSaidaEntrada = DateTime.Now.TimeOfDay;
                }

                guaritaTMS.KMAtual = kmAtual;
                guaritaTMS.Motorista = repUsuario.BuscarPorCodigo(codigoMotorista);
                guaritaTMS.Observacao = observacao;
                guaritaTMS.Operador = this.Usuario;
                guaritaTMS.OrdemServicoFrota = repOrdemServicoFrota.BuscarPorCodigo(codigoOrdemServico);
                guaritaTMS.RetornouComReboque = retornouComReboque;
                guaritaTMS.TipoEntradaSaida = tipoEntradaSaida;
                guaritaTMS.EntrouCarregado = entrouCarregado;
                guaritaTMS.GerarCheckList = gerarCheckList;
                guaritaTMS.CheckListTipo = checkListTipo > 0 ? repCheckListTipo.BuscarPorCodigo(checkListTipo) : null;
                guaritaTMS.Empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null;

                AlterarSituacaoVeiculo(guaritaTMS, unitOfWork, veiculoVazio, finalizouViagem, codigoVeiculo, alterarSituacaoVeiculoParaLiberado);
                guaritaTMS.FinalizouViagem = finalizouViagem;
                guaritaTMS.VeiculoVazio = veiculoVazio;
                guaritaTMS.AlterarSituacaoVeiculoParaLiberado = alterarSituacaoVeiculoParaLiberado;
                guaritaTMS.AlterarReboquesVeiculo = alterarReboquesVeiculo;
                guaritaTMS.MotoristaTerceiro = motoristaTerceiro;

                guaritaTMS.TipoVeiculo = tipoVeiculo;
                if (tipoVeiculo == TipoPropriedadeVeiculo.Proprio)
                {
                    guaritaTMS.Veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);
                    guaritaTMS.PlacaVeiculoTerceiro = string.Empty;
                }
                else
                {
                    guaritaTMS.Veiculo = null;
                    guaritaTMS.PlacaVeiculoTerceiro = placaVeiculoTerceiro;
                }

                if (tipoVeiculo == TipoPropriedadeVeiculo.Proprio)
                {
                    if (alterarReboquesVeiculo)
                        SalvarReboquesAlterados(guaritaTMS, unitOfWork, ignorarValidacaoEmail: true);
                    else
                    {
                        guaritaTMS.Reboques = new List<Dominio.Entidades.Veiculo>();
                        foreach (Dominio.Entidades.Veiculo reboque in guaritaTMS.Veiculo?.VeiculosVinculados)
                            guaritaTMS.Reboques.Add(reboque);
                    }
                }

                repGuaritaTMS.Inserir(guaritaTMS, Auditado);

                //if (guaritaTMS.Veiculo != null && !Servicos.Embarcador.GestaoPatio.GuaritaCheckList.VerificarVencimentoCheckList(unitOfWork, guaritaTMS))
                //{
                //    return new JsonpResult(false, "Existe um Check List vencido e pendente de finalização para este veículo, favor finalize ele no fluxo do Check List");
                //}

                if (guaritaTMS.TipoEntradaSaida == TipoEntradaSaida.Saida)
                {
                    if (!Servicos.Embarcador.GestaoPatio.GuaritaCheckList.AtualizarDataSaidaPedido(guaritaTMS, unitOfWork, Auditado, ConfiguracaoEmbarcador))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, "Ocorreu uma falha ao atualizar a data de saída dos pedidos.");
                    }
                }
                else
                {
                    if (!Servicos.Embarcador.GestaoPatio.GuaritaCheckList.AtualizarDataEntradaPedido(guaritaTMS, unitOfWork, Auditado))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, "Ocorreu uma falha ao atualizar a data de entrada dos pedidos.");
                    }
                }

                if ((guaritaTMS.TipoEntradaSaida == TipoEntradaSaida.Entrada && guaritaTMS.FinalizouViagem) || guaritaTMS.GerarCheckList)
                    Servicos.Embarcador.GestaoPatio.GuaritaCheckList.GerarCheckList(guaritaTMS, unitOfWork, guaritaTMS.KMAtual, guaritaTMS.TipoEntradaSaida, guaritaTMS.Carga, guaritaTMS.OrdemServicoFrota, guaritaTMS.Veiculo, "GERADO PELA GUARITA", guaritaTMS.CheckListTipo?.Codigo ?? 0, guaritaTMS.Empresa?.Codigo ?? 0);

                AtualizarKMVeiculoPneu(guaritaTMS, unitOfWork);
                Servicos.Embarcador.Pedido.Pedido.AtualizarSituacaoPlanejamentoPedidoTMS(guaritaTMS.Carga, null, SituacaoPlanejamentoPedidoTMS.PassouPelaGuarita, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Logistica.GuaritaTMS repGuaritaTMS = new Repositorio.Embarcador.Logistica.GuaritaTMS(unitOfWork);
                Repositorio.Embarcador.GestaoPatio.GuaritaCheckList repGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckList(unitOfWork);
                Repositorio.Embarcador.Veiculos.SituacaoVeiculo repSituacaoVeiculo = new Repositorio.Embarcador.Veiculos.SituacaoVeiculo(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServicoFrota = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("Operador"), out int codigoOperador);
                int.TryParse(Request.Params("Veiculo"), out int codigoVeiculo);
                int.TryParse(Request.Params("Carga"), out int codigoCarga);
                int.TryParse(Request.Params("OrdemServicoFrota"), out int codigoOrdemServico);
                int.TryParse(Request.Params("Motorista"), out int codigoMotorista);
                int.TryParse(Request.Params("KMAtual"), out int kmAtual);
                int.TryParse(Request.Params("Empresa"), out int codigoEmpresa);

                Enum.TryParse(Request.Params("TipoEntradaSaida"), out TipoEntradaSaida tipoEntradaSaida);

                DateTime.TryParseExact(Request.Params("DataSaidaEntrada"), "dd/MM/yyyy", null, DateTimeStyles.None, out DateTime dataSaidaEntrada);

                TimeSpan.TryParseExact(Request.Params("HoraSaidaEntrada"), "g", null, out TimeSpan horaSaidaEntrada);

                string observacao = Request.Params("Observacao");

                bool.TryParse(Request.Params("FinalizouViagem"), out bool finalizouViagem);
                bool.TryParse(Request.Params("RetornouComReboque"), out bool retornouComReboque);
                bool.TryParse(Request.Params("HorarioInformadoManualmente"), out bool horarioInformadoManualmente);
                bool.TryParse(Request.Params("VeiculoVazio"), out bool veiculoVazio);
                bool.TryParse(Request.Params("EntrouCarregado"), out bool entrouCarregado);
                bool.TryParse(Request.Params("AlterarSituacaoVeiculoParaLiberado"), out bool alterarSituacaoVeiculoParaLiberado);
                bool.TryParse(Request.Params("AlterarReboquesVeiculo"), out bool alterarReboquesVeiculo);
                string motoristaTerceiro = Request.GetStringParam("MotoristaTerceiro");


                Enum.TryParse(Request.Params("TipoVeiculo"), out TipoPropriedadeVeiculo tipoVeiculo);
                string placaVeiculoTerceiro = Request.Params("PlacaVeiculoTerceiro");

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                string erro = string.Empty;
                if (!ValidarLancamentoGuarita(kmAtual, codigoVeiculo, tipoEntradaSaida, codigo, codigoCarga, tipoVeiculo, unitOfWork, out erro))
                    return new JsonpResult(false, erro);

                if (dataSaidaEntrada.Date > DateTime.Now.Date)
                    return new JsonpResult(false, "A data não pode ser maior que a atual.");

                Dominio.Entidades.Embarcador.Logistica.GuaritaTMS guaritaTMS = repGuaritaTMS.BuscarPorCodigo(codigo, true);

                unitOfWork.Start();

                AlterarSituacaoVeiculo(guaritaTMS, unitOfWork, veiculoVazio, finalizouViagem, codigoVeiculo, alterarSituacaoVeiculoParaLiberado);

                guaritaTMS.Carga = repCarga.BuscarPorCodigo(codigoCarga);
                guaritaTMS.HorarioInformadoManualmente = horarioInformadoManualmente;

                if (horarioInformadoManualmente)
                {
                    guaritaTMS.DataSaidaEntrada = dataSaidaEntrada;
                    guaritaTMS.HoraSaidaEntrada = horaSaidaEntrada;
                }
                guaritaTMS.Empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null;
                guaritaTMS.FinalizouViagem = finalizouViagem;
                guaritaTMS.KMAtual = kmAtual;
                guaritaTMS.Motorista = repUsuario.BuscarPorCodigo(codigoMotorista);
                guaritaTMS.Observacao = observacao;
                guaritaTMS.OrdemServicoFrota = repOrdemServicoFrota.BuscarPorCodigo(codigoOrdemServico);
                guaritaTMS.RetornouComReboque = retornouComReboque;
                guaritaTMS.TipoEntradaSaida = tipoEntradaSaida;
                guaritaTMS.VeiculoVazio = veiculoVazio;

                guaritaTMS.EntrouCarregado = entrouCarregado;
                guaritaTMS.AlterarSituacaoVeiculoParaLiberado = alterarSituacaoVeiculoParaLiberado;
                guaritaTMS.AlterarReboquesVeiculo = alterarReboquesVeiculo;
                guaritaTMS.MotoristaTerceiro = motoristaTerceiro;

                guaritaTMS.TipoVeiculo = tipoVeiculo;
                if (tipoVeiculo == TipoPropriedadeVeiculo.Proprio)
                {
                    guaritaTMS.Veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);
                    guaritaTMS.PlacaVeiculoTerceiro = string.Empty;
                }
                else
                {
                    guaritaTMS.Veiculo = null;
                    guaritaTMS.PlacaVeiculoTerceiro = placaVeiculoTerceiro;
                }

                repGuaritaTMS.Atualizar(guaritaTMS, Auditado);

                if (tipoVeiculo == TipoPropriedadeVeiculo.Proprio)
                {
                    if (alterarReboquesVeiculo)
                        SalvarReboquesAlterados(guaritaTMS, unitOfWork, ignorarValidacaoEmail: true);
                    else
                    {
                        guaritaTMS.Reboques.Clear();
                        foreach (Dominio.Entidades.Veiculo reboque in guaritaTMS.Veiculo?.VeiculosVinculados)
                            guaritaTMS.Reboques.Add(reboque);
                    }
                }

                if (guaritaTMS.TipoEntradaSaida == TipoEntradaSaida.Saida)
                {
                    if (!Servicos.Embarcador.GestaoPatio.GuaritaCheckList.AtualizarDataSaidaPedido(guaritaTMS, unitOfWork, Auditado, ConfiguracaoEmbarcador))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, "Ocorreu uma falha ao atualizar a data de saída dos pedidos.");
                    }
                }
                else
                {
                    if (!Servicos.Embarcador.GestaoPatio.GuaritaCheckList.AtualizarDataEntradaPedido(guaritaTMS, unitOfWork, Auditado))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, "Ocorreu uma falha ao atualizar a data de entrada dos pedidos.");
                    }
                }

                if ((guaritaTMS.TipoEntradaSaida == TipoEntradaSaida.Entrada && guaritaTMS.FinalizouViagem)
                    && !repGuaritaCheckList.ContemCheckListPorGuarita(guaritaTMS.Codigo))
                    Servicos.Embarcador.GestaoPatio.GuaritaCheckList.GerarCheckList(guaritaTMS, unitOfWork, guaritaTMS.KMAtual, guaritaTMS.TipoEntradaSaida, guaritaTMS.Carga, guaritaTMS.OrdemServicoFrota, guaritaTMS.Veiculo, "GERADO PELA GUARITA", guaritaTMS.CheckListTipo?.Codigo ?? 0, guaritaTMS.Empresa?.Codigo ?? 0);

                AtualizarKMVeiculoPneu(guaritaTMS, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Repositorio.Embarcador.Logistica.GuaritaTMS repGuaritaTMS = new Repositorio.Embarcador.Logistica.GuaritaTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Logistica.GuaritaTMS guaritaTMS = repGuaritaTMS.BuscarPorCodigo(codigo);

                var dynGuaritaTMS = new
                {
                    guaritaTMS.Codigo,
                    Operador = new { Codigo = guaritaTMS.Operador?.Codigo ?? 0, Descricao = guaritaTMS.Operador?.Nome ?? "" },
                    Carga = new { Codigo = guaritaTMS.Carga?.Codigo ?? 0, Descricao = guaritaTMS.Carga?.CodigoCargaEmbarcador ?? "" },
                    OrdemServicoFrota = new { Codigo = guaritaTMS.OrdemServicoFrota?.Codigo ?? 0, Descricao = guaritaTMS.OrdemServicoFrota?.Numero.ToString("n0") ?? "" },
                    Motorista = new { Codigo = guaritaTMS.Motorista?.Codigo ?? 0, Descricao = guaritaTMS.Motorista?.Nome ?? "" },
                    Veiculo = new { Codigo = guaritaTMS.Veiculo?.Codigo ?? 0, Descricao = guaritaTMS.VeiculoComReboque },
                    KMAtual = guaritaTMS.KMAtual.ToString("n0"),
                    DataSaidaEntrada = guaritaTMS.DataSaidaEntrada.ToString("dd/MM/yyyy"),
                    HoraSaidaEntrada = string.Format("{0:00}:{1:00}", guaritaTMS.HoraSaidaEntrada.Hours, guaritaTMS.HoraSaidaEntrada.Minutes),
                    guaritaTMS.TipoEntradaSaida,
                    guaritaTMS.FinalizouViagem,
                    guaritaTMS.RetornouComReboque,
                    guaritaTMS.Observacao,
                    guaritaTMS.HorarioInformadoManualmente,
                    this.Usuario.UsuarioAdministrador,
                    guaritaTMS.VeiculoVazio,
                    guaritaTMS.EntrouCarregado,
                    guaritaTMS.AlterarSituacaoVeiculoParaLiberado,
                    guaritaTMS.AlterarReboquesVeiculo,
                    Empresa = new { Codigo = guaritaTMS.Empresa?.Codigo ?? 0, Descricao = guaritaTMS.Empresa?.Descricao ?? "" },
                    Reboques = (from obj in guaritaTMS.Reboques
                                select new
                                {
                                    VEICULO = new
                                    {
                                        obj.Codigo,
                                        obj.Placa,
                                        ModeloVeicularCarga = obj.ModeloVeicularCarga?.Descricao ?? string.Empty,
                                        TipoVeiculo = obj.DescricaoTipoVeiculo,
                                        obj.DescricaoTipo
                                    }
                                }).ToList(),
                    guaritaTMS.TipoVeiculo,
                    guaritaTMS.PlacaVeiculoTerceiro,
                    guaritaTMS.MotoristaTerceiro,
                };
                return new JsonpResult(dynGuaritaTMS);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);

                unitOfWork.Start();

                Repositorio.Embarcador.Logistica.GuaritaTMS repGuaritaTMS = new Repositorio.Embarcador.Logistica.GuaritaTMS(unitOfWork);
                Repositorio.Embarcador.GestaoPatio.GuaritaCheckList repGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckList(unitOfWork);
                Repositorio.Embarcador.GestaoPatio.GuaritaCheckListPerguntas repGuaritaCheckListPerguntas = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckListPerguntas(unitOfWork);
                Repositorio.Embarcador.GestaoPatio.GuaritaCheckListPerguntasAlternativa repGuaritaCheckListPerguntasAlternativa = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckListPerguntasAlternativa(unitOfWork);

                //List<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListPerguntasAlternativa> listaAlternativa = repGuaritaCheckListPerguntasAlternativa.BuscarPorGuarita(codigo);
                //foreach (var alternativa in listaAlternativa)
                //  repGuaritaCheckListPerguntasAlternativa.Deletar(alternativa);

                //List<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListPerguntas> listaPerguntas = repGuaritaCheckListPerguntas.BuscarPorGuarita(codigo);
                //foreach (var pergunta in listaPerguntas)
                //    repGuaritaCheckListPerguntas.Deletar(pergunta);

                List<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList> listaCheckList = repGuaritaCheckList.BuscarPorGuarita(codigo);
                foreach (var check in listaCheckList)
                {
                    check.Guarita = null;
                    repGuaritaCheckList.Atualizar(check);
                }
                //    repGuaritaCheckList.Deletar(check);

                Dominio.Entidades.Embarcador.Logistica.GuaritaTMS guaritaTMS = repGuaritaTMS.BuscarPorCodigo(codigo);

                if (guaritaTMS.Reboques != null && guaritaTMS.Reboques.Count > 0)
                    repGuaritaTMS.DeletarReboques(codigo);

                repGuaritaTMS.Deletar(guaritaTMS, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(true, false, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                }
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> RetornarVeiculoMotorista()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoMotorista = Request.GetIntParam("CodigoMotorista");

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorMotorista(codigoMotorista, "0");

                if (veiculo == null)
                    return new JsonpResult(false, true, "Sem registros");

                dynamic dynGuaritaTMS = new
                {
                    veiculo.Codigo,
                    veiculo.Placa,
                    Reboque = string.Join(", ", veiculo.VeiculosVinculados.Select(o => o.Placa)),
                    Tracao = string.Join(", ", veiculo.VeiculosTracao.Select(o => o.Placa)),
                    CodigoTracao = veiculo.VeiculosTracao?.FirstOrDefault()?.Codigo ?? 0,
                };

                return new JsonpResult(dynGuaritaTMS, true, "Com registro");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> RetornarNumeroFrota()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoVeiculo = 0;
                int.TryParse(Request.Params("CodigoVeiculo"), out codigoVeiculo);

                Servicos.Veiculo serVeiculo = new Servicos.Veiculo(unidadeDeTrabalho);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);

                if (veiculo == null)
                    return new JsonpResult(false);

                return new JsonpResult(new
                {
                    NumeroFrota = serVeiculo.BuscarConjuntoFrota(veiculo)
                });

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarGuaritaVeiculo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoVeiculo = 0, codigoGuarita = 0;
                int.TryParse(Request.Params("CodigoVeiculo"), out codigoVeiculo);
                int.TryParse(Request.Params("CodigoGuarita"), out codigoGuarita);

                Repositorio.Embarcador.Logistica.GuaritaTMS repGuaritaTMS = new Repositorio.Embarcador.Logistica.GuaritaTMS(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Logistica.GuaritaTMS ultimoRegistroVeiculo = repGuaritaTMS.BuscarUltimoRegistro(codigoVeiculo, codigoGuarita);

                if (ultimoRegistroVeiculo != null)
                {
                    dynamic dynGuaritaTMS = new
                    {
                        ultimoRegistroVeiculo.Codigo,
                        ultimoRegistroVeiculo.TipoEntradaSaida,
                        ultimoRegistroVeiculo.KMAtual,
                        ultimoRegistroVeiculo.DataLancamento
                    };
                    return new JsonpResult(dynGuaritaTMS, true, "Com registro");
                }
                else
                    return new JsonpResult(false, true, "Sem registros");

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ValidarCheckListEntrada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("CodigoVeiculo"), out int codigoVeiculo);
                int.TryParse(Request.Params("CodigoCarga"), out int codigoCarga);

                Repositorio.Embarcador.GestaoPatio.GuaritaCheckList repGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckList(unitOfWork);

                Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList checkList = repGuaritaCheckList.BuscarPorCargaVeiculo(codigoCarga, codigoVeiculo);

                var dynGuaritaTMS = new
                {
                    ContemCheckListEntrada = checkList != null
                };
                return new JsonpResult(dynGuaritaTMS, true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os check list's gerados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> CarregarReboquesAtuaisVeiculo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("CodigoVeiculo"), out int codigoVeiculo);

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);

                if (veiculo == null)
                    return new JsonpResult(false, true, "Veículo não encontrado");

                var retorno = new
                {
                    Reboques = (from obj in veiculo.VeiculosVinculados
                                select new
                                {
                                    VEICULO = new
                                    {
                                        obj.Codigo,
                                        obj.Placa,
                                        ModeloVeicularCarga = obj.ModeloVeicularCarga?.Descricao ?? string.Empty,
                                        TipoVeiculo = obj.DescricaoTipoVeiculo,
                                        obj.DescricaoTipo
                                    }
                                }).ToList()
                };
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar reboques.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterEmpresaPadraoGuarita()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Empresa empresaPadrao = repEmpresa.BuscarEmpresaPadraoLancamentoGuarita();

                var dynGuaritaTMS = new
                {
                    Empresa = new { Codigo = empresaPadrao?.Codigo ?? 0, Descricao = empresaPadrao?.RazaoSocial ?? string.Empty }
                };

                return new JsonpResult(dynGuaritaTMS);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar a empresa padrão para guarita.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private bool ValidarLancamentoGuarita(int kmPassagem, int codigoVeiculo, TipoEntradaSaida tipoEntradaSaida, int codigoGuarita, int codigoCarga, TipoPropriedadeVeiculo tipoPropriedadeVeiculo, Repositorio.UnitOfWork unitOfWork, out string strRetorno)
        {
            if (tipoPropriedadeVeiculo == TipoPropriedadeVeiculo.Terceiros)
            {
                strRetorno = string.Empty;
                return true;
            }

            Repositorio.Embarcador.Logistica.GuaritaTMS repGuaritaTMS = new Repositorio.Embarcador.Logistica.GuaritaTMS(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);
            Dominio.Entidades.Embarcador.Logistica.GuaritaTMS guaritaTMS = repGuaritaTMS.BuscarPorCodigo(codigoGuarita, true);

            strRetorno = string.Empty;

            if (repGuaritaTMS.RegistroDuplicado(codigoVeiculo, kmPassagem, codigoGuarita, tipoEntradaSaida))
            {
                strRetorno = "Já existe um lançamento de Guarita para este veículo com o mesmo KM e Tipo digitado.";
                return false;
            }
            if (codigoGuarita != 0)
            {
                if (kmPassagem < veiculo.KilometragemAtual)
                {
                    strRetorno = "A quilometragem informada na Guarita é menor do que a quilometragem atual do veículo.";
                    return false;
                }
                else
                {
                    AtualizarKMVeiculoPneu(guaritaTMS, unitOfWork);
                    return true;
                }
            }
            if (codigoGuarita == 0)
            {
                Dominio.Entidades.Embarcador.Logistica.GuaritaTMS ultimoRegistroVeiculo = repGuaritaTMS.BuscarUltimoRegistro(codigoVeiculo, codigoGuarita);
                if (ultimoRegistroVeiculo != null)
                {
                    if (ultimoRegistroVeiculo.TipoEntradaSaida == tipoEntradaSaida)
                    {
                        strRetorno = "O último lançamento de Guarita para este veículo também é de " + ultimoRegistroVeiculo.TipoEntradaSaida.ObterDescricao() + ".";
                        return false;
                    }
                    if (codigoGuarita == 0)
                    {
                        if (ultimoRegistroVeiculo.KMAtual > kmPassagem)
                        {
                            strRetorno = "O último lançamento de Guarita para este veículo possui a quilometragem maior que a digitada.";
                            return false;
                        }
                        if (kmPassagem < veiculo.KilometragemAtual)
                        {
                            strRetorno = "A quilometragem informada na Guarita é menor do que a quilometragem atual do veículo.";
                            return false;
                        }
                    }
                    else
                    if (ultimoRegistroVeiculo.KMAtual > kmPassagem && ultimoRegistroVeiculo.Codigo < codigoGuarita)
                    {
                        strRetorno = "O último lançamento de Guarita para este veículo possui a quilometragem maior que a digitada.";
                        return false;
                    }
                }
            }

            return true;
        }

        private void AlterarSituacaoVeiculo(Dominio.Entidades.Embarcador.Logistica.GuaritaTMS guaritaTMS, Repositorio.UnitOfWork unitOfWork, bool veiculoVazio, bool finalizouViagem, int codigoVeiculo, bool alterarSituacaoVeiculoParaLiberado)
        {
            if (codigoVeiculo == 0)
                return;

            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
            Repositorio.Embarcador.Veiculos.SituacaoVeiculo repSituacaoVeiculo = new Repositorio.Embarcador.Veiculos.SituacaoVeiculo(unitOfWork);
            Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServicoFrota = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unitOfWork);
            Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo, true);
            Dominio.Entidades.Usuario veiculoMotorista = null;

            if (veiculo != null)
                veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);


            if (!guaritaTMS.VeiculoVazio && veiculoVazio)
            {
                veiculo.VeiculoVazio = true;
                veiculo.DataHoraVeiculoVazio = DateTime.Now;

                repVeiculo.Atualizar(veiculo, Auditado);

                Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo situacaoVeiculo = new Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo();
                situacaoVeiculo.DataHoraEmissao = DateTime.Now;
                situacaoVeiculo.DataHoraSituacao = DateTime.Now;
                situacaoVeiculo.Motorista = veiculoMotorista;
                situacaoVeiculo.Usuario = this.Usuario;
                situacaoVeiculo.Veiculo = veiculo;
                situacaoVeiculo.VeiculoVazio = true;
                situacaoVeiculo.Situacao = SituacaoVeiculo.Vazio;
                if (veiculo.VeiculosVinculados != null && veiculo.VeiculosVinculados.Count > 0)
                {
                    if (situacaoVeiculo.VeiculosVinculadosSituacao == null)
                        situacaoVeiculo.VeiculosVinculadosSituacao = new List<Dominio.Entidades.Veiculo>();
                    situacaoVeiculo.VeiculosVinculadosSituacao.Clear();
                    foreach (var reb in veiculo.VeiculosVinculados)
                    {
                        situacaoVeiculo.VeiculosVinculadosSituacao.Add(reb);
                    }
                }
                repSituacaoVeiculo.Inserir(situacaoVeiculo, Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, situacaoVeiculo, null, "Veículo vazio pela guarita", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, null, "Veículo vazio pela guarita", unitOfWork);
            }

            if (!guaritaTMS.FinalizouViagem && finalizouViagem)
            {
                veiculo.SituacaoVeiculo = SituacaoVeiculo.Disponivel;
                veiculo.VeiculoVazio = true;
                veiculo.DataHoraPrevisaoDisponivel = null;
                veiculo.LocalidadeAtual = null;
                veiculo.AvisadoCarregamento = false;

                repVeiculo.Atualizar(veiculo);

                Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo situacao = new Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo();
                situacao.DataHoraEmissao = DateTime.Now;
                situacao.DataHoraSituacao = DateTime.Now;
                situacao.Veiculo = veiculo;
                situacao.Localidade = null;
                situacao.Motorista = null;
                situacao.DataHoraRetornoViagem = DateTime.Now.Date;
                situacao.LocalidadeRetornoViagem = null;
                situacao.Situacao = SituacaoVeiculo.Disponivel;

                repSituacaoVeiculo.Inserir(situacao, Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, situacao, null, "Finalizou viagem pela guarita", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, null, "Finalizou viagem pela guarita", unitOfWork);
            }

            if (!guaritaTMS.AlterarSituacaoVeiculoParaLiberado && alterarSituacaoVeiculoParaLiberado)
            {
                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = guaritaTMS.OrdemServicoFrota;

                veiculo.SituacaoVeiculo = SituacaoVeiculo.Disponivel;
                veiculo.DataHoraPrevisaoDisponivel = null;
                veiculo.VeiculoVazio = true;
                if (ordemServico != null && ordemServico.LocalManutencao != null && ordemServico.LocalManutencao.Localidade != null)
                    veiculo.LocalidadeAtual = ordemServico.LocalManutencao.Localidade;
                else
                    veiculo.LocalidadeAtual = null;

                repVeiculo.Atualizar(veiculo);

                Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo situacao = new Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo();
                situacao.DataHoraEmissao = DateTime.Now;
                situacao.DataHoraSituacao = ordemServico?.DataProgramada;
                situacao.Motorista = veiculoMotorista;
                situacao.Usuario = this.Usuario;
                if (ordemServico != null && ordemServico.LocalManutencao != null && ordemServico.LocalManutencao.Localidade != null)
                    situacao.Localidade = ordemServico.LocalManutencao.Localidade;
                situacao.Veiculo = veiculo;
                situacao.DataHoraSaidaManutencao = ordemServico?.DataFechamento;
                situacao.OrdemServicoFrota = ordemServico;
                situacao.Situacao = SituacaoVeiculo.EmManutencao;

                repSituacaoVeiculo.Inserir(situacao);

                if (ordemServico != null)
                {
                    ordemServico.Situacao = SituacaoOrdemServicoFrota.AgNotaFiscal;
                    repOrdemServicoFrota.Atualizar(ordemServico);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, ordemServico, null, "Ordem de Serviço liberada pela guarita", unitOfWork);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, situacao, null, "Veículo liberado pela guarita", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, null, "Veículo liberado pela guarita", unitOfWork);
            }
        }

        private string SetarPropriedadeOrdenacao(string propOrdena)
        {
            if (propOrdena == "Veiculo") propOrdena = "Veiculo.Placa";
            if (propOrdena == "NumeroFrota") propOrdena = "Veiculo.NumeroFrota";

            return propOrdena;
        }

        private void AtualizarKMVeiculoPneu(Dominio.Entidades.Embarcador.Logistica.GuaritaTMS guaritaTMS, Repositorio.UnitOfWork unitOfWork)
        {
            if (guaritaTMS.KMAtual > 0 && guaritaTMS.Veiculo != null)
            {
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unitOfWork);
                Repositorio.Embarcador.Frota.Pneu repPneu = new Repositorio.Embarcador.Frota.Pneu(unitOfWork);

                //atualiza km do veículo e seus pneus
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(guaritaTMS.Veiculo.Codigo);
                decimal qtdKMRodado = 0;
                if (veiculo != null && veiculo.KilometragemAtual < guaritaTMS.KMAtual)
                {
                    qtdKMRodado = guaritaTMS.KMAtual - (decimal)veiculo.KilometragemAtual;

                    if (veiculo.Pneus != null && veiculo.Pneus.Count > 0 && qtdKMRodado > 0)
                    {
                        foreach (var eixo in veiculo.Pneus)
                        {
                            Dominio.Entidades.Embarcador.Frota.Pneu pneu = repPneu.BuscarPorCodigo(eixo.Pneu.Codigo);
                            if (pneu != null)
                            {
                                pneu.KmAtualRodado = (int)(pneu.KmAtualRodado + qtdKMRodado);
                                if (pneu.ValorCustoAtualizado > 0 && pneu.KmAtualRodado > 0)
                                    pneu.ValorCustoKmAtualizado = pneu.ValorCustoAtualizado / pneu.KmAtualRodado;
                                repPneu.Atualizar(pneu);
                            }
                        }
                    }
                    veiculo.KilometragemAtual = (int)guaritaTMS.KMAtual;
                    repVeiculo.Atualizar(veiculo, Auditado, null, "Atualizada a Quilometragem Atual do Veículo via Guarita");
                }

                if (veiculo != null && veiculo.Equipamentos != null && veiculo.Equipamentos.Count > 0)
                {
                    foreach (var equip in veiculo.Equipamentos)
                    {
                        if (equip != null)
                        {
                            Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = repEquipamento.BuscarPorCodigo(equip.Codigo);
                            if (equipamento != null && equipamento.Hodometro < guaritaTMS.KMAtual)
                            {
                                equipamento.Hodometro = (int)guaritaTMS.KMAtual;
                                repEquipamento.Atualizar(equipamento);
                            }
                        }
                    }
                }

                //atualiza km dos reboques e seus pneus
                if (veiculo != null && veiculo.VeiculosVinculados != null && veiculo.VeiculosVinculados.Count > 0)
                {
                    foreach (var reboque in veiculo.VeiculosVinculados)
                    {
                        if (reboque != null && qtdKMRodado > 0)
                        {
                            if (reboque.Pneus != null && reboque.Pneus.Count > 0 && qtdKMRodado > 0)
                            {
                                foreach (var eixo in reboque.Pneus)
                                {
                                    Dominio.Entidades.Embarcador.Frota.Pneu pneu = repPneu.BuscarPorCodigo(eixo.Pneu.Codigo);
                                    if (pneu != null)
                                    {
                                        pneu.KmAtualRodado = (int)(pneu.KmAtualRodado + qtdKMRodado);
                                        if (pneu.ValorCustoAtualizado > 0 && pneu.KmAtualRodado > 0)
                                            pneu.ValorCustoKmAtualizado = pneu.ValorCustoAtualizado / pneu.KmAtualRodado;
                                        repPneu.Atualizar(pneu);
                                    }
                                }
                            }
                            reboque.KilometragemAtual = (int)qtdKMRodado + reboque.KilometragemAtual;
                            repVeiculo.Atualizar(reboque, Auditado, null, "Atualizada a Quilometragem Atual do Reboque via Guarita");
                        }
                    }
                }
            }
        }

        private void SalvarReboquesAlterados(Dominio.Entidades.Embarcador.Logistica.GuaritaTMS guaritaTMS, Repositorio.UnitOfWork unitOfWork, bool ignorarValidacaoEmail = false)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            string reboqueRemovido = "", reboqueAdicionado = "";

            dynamic veiculos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Reboques"));
            if (guaritaTMS.Reboques != null && guaritaTMS.Reboques.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var veiculo in veiculos)
                    if (veiculo.VEICULO.Codigo != null)
                        codigos.Add((int)veiculo.VEICULO.Codigo);

                List<Dominio.Entidades.Veiculo> veiculoRemover = (from obj in guaritaTMS.Reboques where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < veiculoRemover.Count; i++)
                {
                    Dominio.Entidades.Veiculo veiculo = veiculoRemover[i];
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, guaritaTMS, "Removeu o reboque " + veiculo.Descricao, unitOfWork);
                    guaritaTMS.Reboques.Remove(veiculo);
                    reboqueRemovido += string.IsNullOrWhiteSpace(reboqueRemovido) ? veiculo.Placa_Formatada : ", " + veiculo.Placa_Formatada;
                }
            }
            else
                guaritaTMS.Reboques = new List<Dominio.Entidades.Veiculo>();

            foreach (var veiculo in veiculos)
            {
                int.TryParse((string)veiculo.VEICULO.Codigo, out int codigoVeiculo);

                Dominio.Entidades.Veiculo reboque = codigoVeiculo > 0 ? (from obj in guaritaTMS.Reboques where obj.Codigo == codigoVeiculo select obj).FirstOrDefault() : null;
                if (reboque == null)
                {
                    reboque = new Dominio.Entidades.Veiculo();
                    Dominio.Entidades.Veiculo novoReboque = repVeiculo.BuscarPorCodigo(codigoVeiculo);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, guaritaTMS, "Adicionou o reboque " + novoReboque.Descricao, unitOfWork);
                    guaritaTMS.Reboques.Add(novoReboque);
                    reboqueAdicionado += string.IsNullOrWhiteSpace(reboqueAdicionado) ? novoReboque.Placa_Formatada : ", " + novoReboque.Placa_Formatada;
                }
            }
            
            try 
            { 
                EnviarEmailAlteracaoReboque(guaritaTMS, reboqueRemovido, reboqueAdicionado, unitOfWork); 
            }
            catch
            {
                if (!ignorarValidacaoEmail)
                    throw;
            }
        }

        private void EnviarEmailAlteracaoReboque(Dominio.Entidades.Embarcador.Logistica.GuaritaTMS guaritaTMS, string reboqueRemovido, string reboqueAdicionado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Veiculos.ResponsavelVeiculo repResponsavelVeiculo = new Repositorio.Embarcador.Veiculos.ResponsavelVeiculo(unitOfWork);
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(guaritaTMS.Empresa?.Codigo ?? 0);

            if (!string.IsNullOrWhiteSpace(reboqueRemovido) || !string.IsNullOrWhiteSpace(reboqueAdicionado))
            {
                if (email == null)
                    throw new ControllerException("Não há um e-mail configurado para realizar o envio.");

                string assunto = "Modificação de Reboques na Guarita do Veículo " + guaritaTMS.Veiculo.Placa_Formatada;
                string mensagemEmail = "Olá,<br/><br/>Foi realizado as seguintes alterações:";

                if (!string.IsNullOrWhiteSpace(reboqueRemovido))
                    mensagemEmail += "<br/>Removido a(s) placa(s): " + reboqueRemovido;
                if (!string.IsNullOrWhiteSpace(reboqueAdicionado))
                    mensagemEmail += "<br/>Adicionado a(s) placa(s): " + reboqueAdicionado;

                mensagemEmail += ".<br/><br/>";
                mensagemEmail += "E-mail enviado automaticamente. Por favor, não responda.";
                if (!string.IsNullOrWhiteSpace(email.MensagemRodape))
                    mensagemEmail += "<br/>" + "<br/>" + "<br/>" + email.MensagemRodape.Replace("#qLinha#", "<br/>");

                string mensagemErro = "Erro ao enviar e-mail";
                List<string> emails = new List<string>();

                List<Dominio.Entidades.Embarcador.Veiculos.ResponsavelVeiculo> responsaveisVeiculo = repResponsavelVeiculo.BuscarResponsaveisVeiculo(guaritaTMS.Veiculo.Codigo);
                foreach (Dominio.Entidades.Embarcador.Veiculos.ResponsavelVeiculo responsavel in responsaveisVeiculo)
                {
                    if (!string.IsNullOrWhiteSpace(responsavel.FuncionarioResponsavel.Email))
                        emails.AddRange(responsavel.FuncionarioResponsavel.Email.Split(';').ToList());
                }

                emails = emails.Distinct().ToList();
                if (emails.Count > 0)
                {
                    bool sucesso = Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emails.ToArray(), null, assunto, mensagemEmail, email.Smtp, out mensagemErro, email.DisplayEmail, null, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unitOfWork, guaritaTMS.Empresa?.Codigo ?? 0);
                    if (!sucesso)
                        throw new ControllerException("Problemas ao enviar as alterações de reboque por e-mail: " + mensagemErro);
                }
                else
                    throw new ControllerException("Os responsáveis pelo veículo não possuem e-mail configurado! Favor configurar antes de confirmar a alteração.");
            }
        }

        #endregion
    }
}
