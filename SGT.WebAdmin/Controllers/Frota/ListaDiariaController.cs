using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Controllers.Frota
{
    [CustomAuthorize("Frota/ListaDiaria", "Frota/SugestaoMensal")]
    public class ListaDiariaController : BaseController
    {
		#region Construtores

		public ListaDiariaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                var grid = ObterGridTransportador(Request);
                grid.scrollHorizontal = true;
                var rows = ObterDadosGridTransportador(unitOfWork);
                //string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                grid.setarQuantidadeTotal(rows.Count);
                grid.AdicionaRows(rows);

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
        public async Task<IActionResult> PesquisaEmbarcador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = ObterGridEmbarcador(Request);
                grid.scrollHorizontal = true;
                var rows = ObterDadosGridEmbarcador(unitOfWork);
                //string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                grid.setarQuantidadeTotal(rows.Count);
                grid.AdicionaRows(rows);
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
        public async Task<IActionResult> PesquisaAgrupada()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                var grid = ObterGridAgrupada(Request);
                var rows = ObterDadosGridAgrupada(unidadeTrabalho, grid.inicio, grid.limite);
                //string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                grid.setarQuantidadeTotal(rows.Item2);
                grid.AdicionaRows(rows.Item1);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ExportarPesquisaEmbarcador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = ObterGridEmbarcador(Request);
                var rows = ObterDadosGridEmbarcador(unitOfWork);

                grid.setarQuantidadeTotal(rows.Count);
                grid.AdicionaRows(rows);
                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
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

        public async Task<IActionResult> ExportarPesquisaAgrupada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = ObterGridAgrupada(Request);
                var rows = ObterDadosGridAgrupada(unitOfWork, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(rows.Item2);
                grid.AdicionaRows(rows.Item1);
                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
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

        public async Task<IActionResult> ExportarPesquisaTransportador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = ObterGridTransportador(Request);
                var rows = ObterDadosGridTransportador(unitOfWork);

                grid.setarQuantidadeTotal(rows.Count);
                grid.AdicionaRows(rows);
                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
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

        public async Task<IActionResult> DefinirJustificativa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoPlanejamentoFrotaDiaVeiculo = Request.GetIntParam("CodigoPlanejamentoFrotaDiaVeiculo");
                int codigoJustificativaDeIndisponibilidadeDeFrota = Request.GetIntParam("CodigoJustificativa");

                if (codigoJustificativaDeIndisponibilidadeDeFrota == 0)
                    throw new ControllerException("A justificativa de indisponibilidade deve ser informada.");

                Repositorio.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo repositorioPlanejamentoFrotaDiaVeiculo = new Repositorio.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo(unitOfWork);
                Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo planejamentoDiaVeiculo = repositorioPlanejamentoFrotaDiaVeiculo.BuscarPorCodigo(codigoPlanejamentoFrotaDiaVeiculo, auditavel: true);

                if (planejamentoDiaVeiculo == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                ValidarBloqueioEdicao(planejamentoDiaVeiculo.PlanejamentoFrotaDia);

                planejamentoDiaVeiculo.JustificativaIndisponibilidade = new Dominio.Entidades.Embarcador.Frotas.JustificativaDeIndisponibilidadeDeFrota { Codigo = codigoJustificativaDeIndisponibilidadeDeFrota };
                planejamentoDiaVeiculo.Indisponivel = true;

                repositorioPlanejamentoFrotaDiaVeiculo.Atualizar(planejamentoDiaVeiculo);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, planejamentoDiaVeiculo.PlanejamentoFrotaDia, planejamentoDiaVeiculo.GetChanges(), $"Definiu a justificativa de indisponibilidade do veículo {planejamentoDiaVeiculo.Veiculo.Placa}.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao definir a justificativa de indisponibilidade.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> MarcarComoDisponivel()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoPlanejamentoFrotaDiaVeiculo = Request.GetIntParam("CodigoPlanejamentoFrotaDiaVeiculo");
                Repositorio.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo repositorioPlanejamentoFrotaDiaVeiculo = new Repositorio.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo(unitOfWork);
                Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo planejamentoDiaVeiculo = repositorioPlanejamentoFrotaDiaVeiculo.BuscarPorCodigo(codigoPlanejamentoFrotaDiaVeiculo, auditavel: true);

                if (planejamentoDiaVeiculo == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                ValidarBloqueioEdicao(planejamentoDiaVeiculo.PlanejamentoFrotaDia);

                planejamentoDiaVeiculo.JustificativaIndisponibilidade = null;
                planejamentoDiaVeiculo.Indisponivel = false;

                repositorioPlanejamentoFrotaDiaVeiculo.Atualizar(planejamentoDiaVeiculo);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, planejamentoDiaVeiculo.PlanejamentoFrotaDia, planejamentoDiaVeiculo.GetChanges(), $"Alterou o disponibilidade veículo {planejamentoDiaVeiculo.Veiculo.Placa}.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar a disponibilidade.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> MarcarOuDesmarcarRoteirizado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoPlanejamentoFrotaDiaVeiculo = Request.GetIntParam("CodigoPlanejamentoFrotaDiaVeiculo");
                Repositorio.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo repositorioPlanejamentoFrotaDiaVeiculo = new Repositorio.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo(unitOfWork);
                Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo planejamentoDiaVeiculo = repositorioPlanejamentoFrotaDiaVeiculo.BuscarPorCodigo(codigoPlanejamentoFrotaDiaVeiculo, auditavel: true);

                if (planejamentoDiaVeiculo == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                ValidarBloqueioEdicao(planejamentoDiaVeiculo.PlanejamentoFrotaDia);

                planejamentoDiaVeiculo.Roteirizado = Request.GetBoolParam("Roteirizado");

                repositorioPlanejamentoFrotaDiaVeiculo.Atualizar(planejamentoDiaVeiculo);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, planejamentoDiaVeiculo.PlanejamentoFrotaDia, planejamentoDiaVeiculo.GetChanges(), $"Alterou roteirizado do veículo {planejamentoDiaVeiculo.Veiculo.Placa}.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar roteirizado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarVeiculo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                var codigosVeiculos = Request.GetListParam<int>("CodigoVeiculos");

                if (codigosVeiculos.Count == 0)
                    throw new ControllerException("Um ou mais veículos devem ser informados.");

                int codigoPlanejamentoFrotaDia = Request.GetIntParam("CodigoPlanejamento");
                Repositorio.Embarcador.Frotas.PlanejamentoFrotaDia repositorioPlanejamentoFrotaDia = new Repositorio.Embarcador.Frotas.PlanejamentoFrotaDia(unitOfWork);
                Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDia planejamentoDia = repositorioPlanejamentoFrotaDia.BuscarPorCodigo(codigoPlanejamentoFrotaDia, auditavel: false);

                if (planejamentoDia == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                ValidarBloqueioEdicao(planejamentoDia);

                Repositorio.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo repositorioPlanejamentoFrotaDiaVeiculo = new Repositorio.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo(unitOfWork);
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                List<string> jaExistentes = new List<string>();
                List<string> msgsTesteFrio = new List<string>();
                List<string> msgsModeloVeicular = new List<string>();
                List<string> sucesso = new List<string>();

                foreach (int codigoVeiculo in codigosVeiculos)
                {
                    Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorCodigo(codigoVeiculo);
                    DateTime dataUltimoCarregamento = repositorioPlanejamentoFrotaDiaVeiculo.ObterDataDoUltimoCarregamento(veiculo.Codigo);

                    if (repositorioPlanejamentoFrotaDiaVeiculo.VerificarSePlacaJaExisteNaData(planejamentoDia.Data.Date, veiculo.Placa))
                    {
                        jaExistentes.Add(veiculo.Placa);
                        continue;
                    }

                    string msgTesteFrio = string.Empty;

                    if (veiculo.LicencasVeiculo.IsNullOrEmpty())
                        msgsTesteFrio.Add($"O veículo {veiculo.Placa} não possui Teste de Frio cadastrado.");
                    else
                    {
                        DateTime? vencimento = veiculo.LicencasVeiculo.OrderByDescending(x => x.DataVencimento).Select(x => x.DataVencimento).FirstOrDefault();

                        if (vencimento.IsNullOrMinValue())
                            msgsTesteFrio.Add($"O veículo {veiculo.Placa} não possui Data de Vencimento do Teste de Frio cadastrado.");
                        else if (vencimento.Value.Date < planejamentoDia.Data.Date)
                            msgsTesteFrio.Add($"O veículo {veiculo.Placa} estará com o Teste de Frio vencido na data selecionada.");
                    }

                    if (veiculo.ModeloVeicularCarga == null)
                        msgsModeloVeicular.Add($"Não foi possível adicionar {veiculo.Placa}. O Veículo não possui um Modelo Veicular cadastrado.");

                    Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo planoDiaVeiculo = new Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo()
                    {
                        GeradoPeloSistema = false,
                        Indisponivel = false,
                        JustificativaIndisponibilidade = null,
                        ModeloVeicular = veiculo.ModeloVeicularCarga,
                        ObservacaoMarfrig = string.Empty,
                        ObservacaoTransportador = string.Empty,
                        PlacaVeiculo = veiculo.Placa,
                        Roteirizado = false,
                        UltimoEmbarque = dataUltimoCarregamento == DateTime.MinValue ? (DateTime?)null : dataUltimoCarregamento,
                        Veiculo = veiculo,
                        PlanejamentoFrotaDia = planejamentoDia,
                        RotaDeConhecimento = string.Empty
                    };
                    repositorioPlanejamentoFrotaDiaVeiculo.Inserir(planoDiaVeiculo);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, planejamentoDia, null, $"Adicionou o veículo {veiculo.Placa} no planejamento diário.", unitOfWork);
                    sucesso.Add(veiculo.Placa);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(new {
                    Sucesso = sucesso,
                    MsgTesteFrio = msgsTesteFrio,
                    JaExiste = jaExistentes,
                    MsgModeloVeicular = msgsModeloVeicular
                });
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar veículo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReplicarRegistro()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoPlanejamentoFrotaDiaVeiculo = Request.GetIntParam("CodigoPlanejamentoFrotaDiaVeiculo");
                Repositorio.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo repositorioPlanejamentoFrotaDiaVeiculo = new Repositorio.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo(unitOfWork);
                Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo planejamentoDiaVeiculo = repositorioPlanejamentoFrotaDiaVeiculo.BuscarPorCodigo(codigoPlanejamentoFrotaDiaVeiculo, auditavel: true);

                if (planejamentoDiaVeiculo == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                DateTime dataInicial = Request.GetDateTimeParam("DataInicial");
                DateTime dataFinal = Request.GetDateTimeParam("DataFinal");

                if ((dataInicial == DateTime.MinValue) || (dataFinal == DateTime.MinValue))
                    throw new ControllerException("Informe a data inicial e final.");

                if (dataInicial > dataFinal)
                    throw new ControllerException("A data inicial não pode ser maior que a data final.");

                if (dataInicial < DateTime.Now.Date)
                    throw new ControllerException("A data inicial não pode ser inferior à data atual.");

                int mesDoPlanejamento = planejamentoDiaVeiculo.PlanejamentoFrotaDia.Data.Month;

                if ((mesDoPlanejamento != dataInicial.Month) || (mesDoPlanejamento != dataFinal.Month))
                    throw new ControllerException($"A data inicial e final devem pertencer ao mês do planejamento. (Mês {mesDoPlanejamento})");

                Repositorio.Embarcador.Frotas.PlanejamentoFrotaDia repositorioPlanejamentoFrotaDia = new Repositorio.Embarcador.Frotas.PlanejamentoFrotaDia(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDia> planejamentosDiarios = repositorioPlanejamentoFrotaDia.BuscarPorData(planejamentoDiaVeiculo.PlanejamentoFrotaDia.Filial.Codigo, dataInicial, dataFinal);

                if (!planejamentosDiarios.IsNullOrEmpty() && !EhEmbarcador())
                {
                    Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDia primeiroPlanejamentoFrotaDia = planejamentosDiarios[0];

                    if (!IsPeriodoDeHorasValido(primeiroPlanejamentoFrotaDia))
                        throw new ControllerException($"Não é possível fazer alterações fora do horário estabelecido. ({primeiroPlanejamentoFrotaDia.Filial.HoraInicialPlanejamentoDiario.ToHourMinuteString()} e {primeiroPlanejamentoFrotaDia.Filial.HoraFinalPlanejamentoDiario.ToHourMinuteString()})");
                }

                foreach (Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDia planejamentoFrotaDia in planejamentosDiarios)
                {
                    if (!IsDiasDeToleranciaValido(planejamentoFrotaDia))
                        continue;

                    if (repositorioPlanejamentoFrotaDiaVeiculo.VerificarSePlacaJaExisteNaData(planejamentoFrotaDia.Data.Date, planejamentoDiaVeiculo.PlacaVeiculo))
                        continue;

                    Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo planejamentoFrotaDiaVeiculoDuplicado = new Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo
                    {
                        GeradoPeloSistema = false,
                        Indisponivel = planejamentoDiaVeiculo.Indisponivel,
                        JustificativaIndisponibilidade = planejamentoDiaVeiculo.JustificativaIndisponibilidade,
                        ModeloVeicular = planejamentoDiaVeiculo.ModeloVeicular,
                        ObservacaoMarfrig = string.Empty,
                        ObservacaoTransportador = string.Empty,
                        PlacaVeiculo = planejamentoDiaVeiculo.PlacaVeiculo,
                        PlanejamentoFrotaDia = planejamentoFrotaDia,
                        Roteirizado = planejamentoDiaVeiculo.Roteirizado,
                        UltimoEmbarque = planejamentoDiaVeiculo.UltimoEmbarque,
                        Veiculo = planejamentoDiaVeiculo.Veiculo,
                        RotaDeConhecimento = string.Empty
                    };

                    repositorioPlanejamentoFrotaDiaVeiculo.Inserir(planejamentoFrotaDiaVeiculoDuplicado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, planejamentoFrotaDia, null, $"Adicionou o veículo {planejamentoFrotaDiaVeiculoDuplicado.Veiculo.Placa} no planejamento diário.", unitOfWork);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true, true, "Registro replicado com sucesso.");
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao replicar os veículos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirRegistros()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoPlanejamentoFrotaDiaVeiculo = Request.GetIntParam("CodigoPlanejamentoFrotaDiaVeiculo");
                Repositorio.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo repositorioPlanejamentoFrotaDiaVeiculo = new Repositorio.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo(unitOfWork);
                Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo planejamentoDiaVeiculo = repositorioPlanejamentoFrotaDiaVeiculo.BuscarPorCodigo(codigoPlanejamentoFrotaDiaVeiculo, auditavel: true);

                if (planejamentoDiaVeiculo == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                DateTime dataInicial = Request.GetDateTimeParam("DataInicial");
                DateTime dataFinal = Request.GetDateTimeParam("DataFinal");

                if ((dataInicial == DateTime.MinValue) || (dataFinal == DateTime.MinValue))
                    throw new ControllerException("Informe a data inicial e final.");

                if (dataInicial > dataFinal)
                    throw new ControllerException("A data inicial não pode ser maior que a data final.");

                if (dataInicial < DateTime.Now.Date)
                    throw new ControllerException("A data inicial não pode ser inferior à data atual.");

                int mesDoPlanejamento = planejamentoDiaVeiculo.PlanejamentoFrotaDia.Data.Month;

                if ((mesDoPlanejamento != dataInicial.Month) || (mesDoPlanejamento != dataFinal.Month))
                    throw new ControllerException($"A data inicial e final devem pertencer ao mês do planejamento. (Mês {mesDoPlanejamento})");

                Repositorio.Embarcador.Frotas.PlanejamentoFrotaDia repositorioPlanejamentoFrotaDia = new Repositorio.Embarcador.Frotas.PlanejamentoFrotaDia(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo> veiculosPlanejamentoDiario = repositorioPlanejamentoFrotaDiaVeiculo.BuscarPorData(planejamentoDiaVeiculo.PlanejamentoFrotaDia.Filial.Codigo, dataInicial, dataFinal, planejamentoDiaVeiculo.PlacaVeiculo);

                if (!veiculosPlanejamentoDiario.IsNullOrEmpty() && !EhEmbarcador())
                {
                    Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo primeiroPlanejamentoFrotaDiaVeiculo = veiculosPlanejamentoDiario[0];

                    if (!IsPeriodoDeHorasValido(primeiroPlanejamentoFrotaDiaVeiculo.PlanejamentoFrotaDia))
                        throw new ControllerException($"Não é possível fazer alterações fora do horário estabelecido. ({primeiroPlanejamentoFrotaDiaVeiculo.PlanejamentoFrotaDia.Filial.HoraInicialPlanejamentoDiario.ToHourMinuteString()} e {primeiroPlanejamentoFrotaDiaVeiculo.PlanejamentoFrotaDia.Filial.HoraFinalPlanejamentoDiario.ToHourMinuteString()})");
                }

                bool teveRegistroQueNaoPodeSerExcluido = false;

                foreach (Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo veiculoASerExcluido in veiculosPlanejamentoDiario)
                {
                    if (!IsDiasDeToleranciaValido(veiculoASerExcluido.PlanejamentoFrotaDia))
                    {
                        teveRegistroQueNaoPodeSerExcluido = true;
                        continue;
                    }

                    repositorioPlanejamentoFrotaDiaVeiculo.Deletar(veiculoASerExcluido);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculoASerExcluido.PlanejamentoFrotaDia, null, $"Removeu o veículo {veiculoASerExcluido.Veiculo.Placa} do planejamento diário.", unitOfWork);
                }

                unitOfWork.CommitChanges();

                if(teveRegistroQueNaoPodeSerExcluido)
                    return new JsonpResult(true, true, "Registros excluídos conforme regras do Embarcador.");
                else
                    return new JsonpResult(true, true, "Registros excluídos com sucesso.");
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir registros.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoPlanejamentoFrotaDiaVeiculo = Request.GetIntParam("CodigoPlanejamentoFrotaDiaVeiculo");
                Repositorio.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo repositorioPlanejamentoFrotaDiaVeiculo = new Repositorio.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo(unitOfWork);
                Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo planejamentoDiaVeiculo = repositorioPlanejamentoFrotaDiaVeiculo.BuscarPorCodigo(codigoPlanejamentoFrotaDiaVeiculo, true);

                if (planejamentoDiaVeiculo == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                ValidarBloqueioEdicao(planejamentoDiaVeiculo.PlanejamentoFrotaDia);

                if (EhEmbarcador())
                {
                    planejamentoDiaVeiculo.ObservacaoMarfrig = Request.GetStringParam("ObservacaoMarfrig");
                    planejamentoDiaVeiculo.RotaDeConhecimento = Request.GetStringParam("RotaDeConhecimento");
                }
                else
                    planejamentoDiaVeiculo.ObservacaoTransportador = Request.GetStringParam("ObservacaoTransportador");
                
                repositorioPlanejamentoFrotaDiaVeiculo.Atualizar(planejamentoDiaVeiculo);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, planejamentoDiaVeiculo.PlanejamentoFrotaDia, planejamentoDiaVeiculo.GetChanges(), $"Alterou dados do veículo {planejamentoDiaVeiculo.Veiculo.Placa}.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar as alterações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private Repositorio.Embarcador.Frotas.FiltroPesquisaListaDiaria ObterFiltrosPesquisa()
        {
            return new Repositorio.Embarcador.Frotas.FiltroPesquisaListaDiaria()
            {
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                Filial = Request.GetListParam<int>("Filial"),
                ModeloVeicular = Request.GetListParam<int>("ModeloVeicular"),
                Rodizio = Request.GetBoolParam("Rodizio"),
                SituacaoRota = Request.GetEnumParam<Repositorio.Embarcador.Frotas.EnumFiltroSituacoesRota>("SituacaoRota"),
                Status = Request.GetEnumParam<Repositorio.Embarcador.Frotas.EnumFiltroStatusFrotaDiaria>("Status"),
                Transportador = Request.GetListParam<int>("Transportador")
            };
        }

        private Repositorio.Embarcador.Frotas.FiltroPesquisaListaDiariaVeiculos ObterFiltrosPesquisaVeiculos()
        {
            Repositorio.Embarcador.Frotas.FiltroPesquisaListaDiariaVeiculos filtros = new Repositorio.Embarcador.Frotas.FiltroPesquisaListaDiariaVeiculos()
            {
                CodigoFilial = Request.GetIntParam("CodigoFilial"),
                CodigoModeloVeicular = Request.GetIntParam("ModeloVeicular"),
                CodigosTransportadores = Request.GetListParam<int>("Transportador"),
                Data = Request.GetDateTimeParam("Data"),
                Roteirizado = Request.GetEnumParam<Repositorio.Embarcador.Frotas.EnumFiltroSituacoesRota>("Roteirizado")
            };

            if (!EhEmbarcador())
                filtros.CodigosTransportadores = new List<int>() { Empresa.Codigo };

            return filtros;
        }

        private Models.Grid.Grid ObterGridAgrupada(HttpRequest request)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(request);
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("CodigoPlanejamento", false);
            grid.AdicionarCabecalho("CodigoFilial", false);
            grid.AdicionarCabecalho("CodigoModeloVeicular", false);
            grid.AdicionarCabecalho("Filial", "Filial", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Modelo", "ModeloVeicular", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data", "Data", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Quantidade Sugerida", "QuantidadeSugerida", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Quantidade Disponível", "QuantidadeDisponivel", 10, Models.Grid.Align.left, false); //checkbox Indisponivel (bool)
            grid.AdicionarCabecalho("Quantidade Roteirizada", "QuantidadeRoteirizada", 10, Models.Grid.Align.left, false); // checkbox Indisponivel e justificativaDeIndisponibilidade
            grid.AdicionarCabecalho("Obs. Transportador", "ObsTransportador", 10, Models.Grid.Align.left, false); //(editável somente transportador)
            grid.AdicionarCabecalho("Obs. Marfrig", "ObsMarfrig", 10, Models.Grid.Align.left, false); //(editável somente Marfrig),
            return grid;
        }

        private Models.Grid.Grid ObterGridEmbarcador(HttpRequest request)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(request);
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Roteirizado", "Roteirizado", 5, Models.Grid.Align.center, false, true).Editable(new Models.Grid.EditableCell(TipoColunaGrid.aBool));
            grid.AdicionarCabecalho("Filial", "Filial", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Disponível em", "DisponivelEm", 10, Models.Grid.Align.left, false); // (Data)//é a data de planejamento diario do veiculo
            grid.AdicionarCabecalho("Placa", "Placa", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Modelo Veicular", "ModeloVeicular", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Ano do Modelo", "AnoModelo", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Capacidade", "Capacidade", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Teste de Frio", "TesteFrio", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Status da Vigência", "StatusVigenciaVeiculo", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Vencimento Teste de Frio", "DataVencimentoTesteFrio", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Transportadora", "Transportadora", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("CNPJ Transportadora", "CNPJTransportadora", 13, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Rota de Conhecimento", "RotaDeConhecimento", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Em Rodízio", "EmRodizio", 10, Models.Grid.Align.left, false); //Sim/Nao #51074
            grid.AdicionarCabecalho("Paletizado", "Paletizado", 10, Models.Grid.Align.left, false); //Sim/Nao
            grid.AdicionarCabecalho("Indisponível", "Indisponivel", 5, Models.Grid.Align.center, false, true).Editable(new Models.Grid.EditableCell(TipoColunaGrid.aBool));
            grid.AdicionarCabecalho("Motivo Indisponibilidade", "MotivoIndisponibilidade", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Sugerido pelo Sistema", "SugeridoPeloSistema", 10, Models.Grid.Align.left, false); //bool Sim/Nao,
            grid.AdicionarCabecalho("Carga Vinculada", "CargaVinculada", 10, Models.Grid.Align.left, false); //(quando já houver, informação vinda na integração das Nfs, somente informativo sem opção de edição, considerando cargas de redespacho e data/hora de corte de faturamento.),
            grid.AdicionarCabecalho("Último Embarque", "UltimoEmbarque", 10, Models.Grid.Align.left, false); //data
            grid.AdicionarCabecalho("Motorista", "Motorista", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("CPF", "CPF", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Telefone", "Celular", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("GR Motorista Válida", "ValidadeGrMotorista", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("GR Veículo Válida", "ValidadeGrVeiculo", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Tipo", "Tipo", 10, Models.Grid.Align.left, false); //Proprio ou Agregado.
            grid.AdicionarCabecalho("Redespacho", "Redespacho", 10, Models.Grid.Align.left, false); //(quando já houver, informação vinda na integração das Nfs, somente informativo sem opção de edição, considerando cargas de redespacho e data/hora de corte de faturamento.),
            grid.AdicionarCabecalho("Obs. Marfrig", "ObsMarfrig", 10, Models.Grid.Align.left, false); //(editável somente Marfrig),
            grid.AdicionarCabecalho("Obs. Transportador", "ObsTransportador", 10, Models.Grid.Align.left, false); //(editável somente transportador)

            return grid;
        }

        private Models.Grid.Grid ObterGridTransportador(HttpRequest request)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Placa", "Placa", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Modelo Veicular", "ModeloVeicular", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Capacidade", "Capacidade", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Teste de frio", "TesteFrio", 10, Models.Grid.Align.left, false); //(Aprovado/Reprovado) (mostrar data de vencimento).
            grid.AdicionarCabecalho("Vencimento Teste de Frio", "DataVencimentoTesteFrio", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Indisponível", "Indisponivel", 5, Models.Grid.Align.center, false, true).Editable(new Models.Grid.EditableCell(TipoColunaGrid.aBool));
            grid.AdicionarCabecalho("Motivo indisponibilidade", "MotivoIndisponibilidade", 10, Models.Grid.Align.left, false); // checkbox Indisponivel e justificativaDeIndisponibilidade
            grid.AdicionarCabecalho("Obs. Transportador", "ObsTransportador", 10, Models.Grid.Align.left, false); //(editável somente transportador)
            grid.AdicionarCabecalho("Obs. Marfrig", "ObsMarfrig", 10, Models.Grid.Align.left, false); //(editável somente Marfrig),
            grid.AdicionarCabecalho("Em Rodízio", "EmRodizio", 10, Models.Grid.Align.left, false); //Sim/Nao #51074
            grid.AdicionarCabecalho("Último Embarque", "UltimoEmbarque", 10, Models.Grid.Align.left, false); //data
            grid.AdicionarCabecalho("Carga Vinculada", "CargaVinculada", 10, Models.Grid.Align.left, false); //(quando já houver, informação vinda na integração das Nfs, somente informativo sem opção de edição, considerando cargas de redespacho e data/hora de corte de faturamento.),
            grid.AdicionarCabecalho("Sugerido pelo Sistema", "SugeridoPeloSistema", 10, Models.Grid.Align.left, false); //bool Sim/Nao,
            grid.AdicionarCabecalho("Roteirizado", "Roteirizado", 10, Models.Grid.Align.left, false); //bool Sim/Nao,
            return grid;
        }

        private IList ObterDadosGridTransportador(Repositorio.UnitOfWork unitOfWork)
        {
            var filtros = ObterFiltrosPesquisaVeiculos();
            var repoPlanejamentoFrotaDiaVeiculo = new Repositorio.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo(unitOfWork);
            var repoTesteFrio = new Repositorio.Embarcador.Veiculos.LicencaVeiculo(unitOfWork);
            var repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            var resultadoPesquisa = repoPlanejamentoFrotaDiaVeiculo.PesquisarListaDiariaVeiculos(filtros);
            List<int> codigosVeiculos = resultadoPesquisa.Select(x => x.Veiculo.Codigo).Distinct().ToList();
            var relacaoTesteFrioVeiculo = repoTesteFrio.ObterRelacaoVencimentoTesteFrioVeiculo(codigosVeiculos);
            var relacaoVeiculoCarga = repCarga.BuscaCargasEmAbertoPorListaDeVeiculos(codigosVeiculos, resultadoPesquisa.Any() ? resultadoPesquisa[0].PlanejamentoFrotaDia.Data.Date : DateTime.MinValue);

            Func<int, string> obterDescricaoDoStatusDoTesteDeFrio = (int codigoVeiculo) =>
            {
                if (relacaoTesteFrioVeiculo.IsNullOrEmpty())
                    return "Inexistente";

                var descicao = relacaoTesteFrioVeiculo.Where(x => x.CodigoVeiculo == codigoVeiculo).FirstOrDefault()?.Status.ObterDescricao();
                return string.IsNullOrEmpty(descicao) ? "Inexistente" : descicao;
            };
            Func<int, string> obterDataVencimentoDoTesteDeFrio = (int codigoVeiculo) =>
            {
                if (relacaoTesteFrioVeiculo.IsNullOrEmpty())
                    return string.Empty;

                var descicao = relacaoTesteFrioVeiculo.Where(x => x.CodigoVeiculo == codigoVeiculo).FirstOrDefault()?.Vencimento.ToDateString();
                return string.IsNullOrEmpty(descicao) ? string.Empty : descicao;
            };

            Func<int, DateTime, string> obterCarregamentoDoDia = (int codigoVeiculo, DateTime data) =>
            {
                return relacaoVeiculoCarga.Where(x => x.CodigoVeiculo == codigoVeiculo)
                    .Where(x => x.DataCriacao.Date == data.Date)
                    .Select(y => y.CodigoCarga)
                    .FirstOrDefault() ?? string.Empty;
            };

            var rows = resultadoPesquisa.Select(x => new
            {
                Codigo = x.Codigo,
                Placa = x.PlacaVeiculo,
                ModeloVeicular = x.ModeloVeicular.Descricao,
                Capacidade = x.Veiculo.CapacidadeKG,
                TesteFrio = obterDescricaoDoStatusDoTesteDeFrio(x.Veiculo.Codigo),
                DataVencimentoTesteFrio = obterDataVencimentoDoTesteDeFrio(x.Veiculo.Codigo),
                Indisponivel = x.Indisponivel,
                MotivoIndisponibilidade = x.JustificativaIndisponibilidade?.Descricao ?? string.Empty,
                ObsTransportador = x.ObservacaoTransportador,
                ObsMarfrig = x.ObservacaoMarfrig,
                UltimoEmbarque = x.UltimoEmbarque.ToDateString(),
                CargaVinculada = obterCarregamentoDoDia(x.Veiculo.Codigo, x.PlanejamentoFrotaDia.Data),
                SugeridoPeloSistema = x.GeradoPeloSistema ? "Sim" : "Não",
                EmRodizio = x.Rodizio ? "Sim" : "Não",
                Roteirizado = x.Roteirizado ? "Sim" : "Não",
                DT_RowClass = ObterCorPorStatus(x.Indisponivel)
            }).ToList();

            return rows;
        }

        private IList ObterDadosGridEmbarcador(Repositorio.UnitOfWork unitOfWork)
        {
            var filtros = ObterFiltrosPesquisaVeiculos();
            var repoPlanejamentoFrotaDiaVeiculo = new Repositorio.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo(unitOfWork);
            var repoTesteFrio = new Repositorio.Embarcador.Veiculos.LicencaVeiculo(unitOfWork);
            var repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
            var repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            var resultadoPesquisa = repoPlanejamentoFrotaDiaVeiculo.PesquisarListaDiariaVeiculos(filtros);
            List<int> codigosVeiculos = resultadoPesquisa.Select(x => x.Veiculo.Codigo).Distinct().ToList();
            var relacaoTesteFrioVeiculo = repoTesteFrio.ObterRelacaoVencimentoTesteFrioVeiculo(codigosVeiculos);
            var relacaoMotoristaVeiculo = repVeiculoMotorista.ObterRelacaoMotoristaVeiculo(codigosVeiculos);
            var relacaoVeiculoCarga = repCarga.BuscaCargasEmAbertoPorListaDeVeiculos(codigosVeiculos, resultadoPesquisa.Any() ? resultadoPesquisa[0].PlanejamentoFrotaDia.Data.Date : DateTime.MinValue);

            Func<int, string> obterDescricaoDoStatusDoTesteDeFrio = (int codigoVeiculo) =>
            {
                if (relacaoTesteFrioVeiculo.IsNullOrEmpty())
                    return "Inexistente";

                var descicao = relacaoTesteFrioVeiculo.Where(x => x.CodigoVeiculo == codigoVeiculo).FirstOrDefault()?.Status.ObterDescricao();
                return string.IsNullOrEmpty(descicao) ? "Inexistente" : descicao;
            };
            Func<int, string> obterDataVencimentoDoTesteDeFrio = (int codigoVeiculo) =>
            {
                if (relacaoTesteFrioVeiculo.IsNullOrEmpty())
                    return string.Empty;

                var descicao = relacaoTesteFrioVeiculo.Where(x => x.CodigoVeiculo == codigoVeiculo).FirstOrDefault()?.Vencimento.ToDateString();
                return string.IsNullOrEmpty(descicao) ? string.Empty : descicao;
            };
            Func<int, string> obterDataFaturamento = (int codigoVeiculo) =>
            {
                var data = relacaoVeiculoCarga.Where(y => y.CodigoVeiculo == codigoVeiculo).Select(y => y.DataCriacao).FirstOrDefault();
                if (data == DateTime.MinValue || data == default(DateTime))
                    return string.Empty;
                return data.ToDateTimeString();
            };
            Func<int, DateTime, string> obterCarregamentoDoDia = (int codigoVeiculo, DateTime data) =>
            {
                return relacaoVeiculoCarga.Where(x => x.CodigoVeiculo == codigoVeiculo)
                    .Where(x => x.DataCriacao.Date == data.Date)
                    .Select(y => y.CodigoCarga)
                    .FirstOrDefault() ?? string.Empty;
            };

            var rows = resultadoPesquisa.Select(x => new
            {
                Codigo = x.Codigo,
                Filial = x.PlanejamentoFrotaDia.Filial.Descricao,
                Placa = x.PlacaVeiculo,
                ModeloVeicular = x.ModeloVeicular.Descricao,
                Capacidade = x.Veiculo.CapacidadeKG,
                TesteFrio = obterDescricaoDoStatusDoTesteDeFrio(x.Veiculo.Codigo),
                StatusVigenciaVeiculo = relacaoTesteFrioVeiculo != null ? relacaoTesteFrioVeiculo.OrderByDescending(a => a.CodigoVeiculo).Select(o => o.Status.ObterDescricao()).FirstOrDefault() : string.Empty,
                DataVencimentoTesteFrio = obterDataVencimentoDoTesteDeFrio(x.Veiculo.Codigo),
                AnoModelo = x.Veiculo.AnoModelo,
                Transportadora = x.Veiculo.Empresa.RazaoSocial,
                CNPJTransportadora = x.Veiculo.Empresa.CNPJ_Formatado,
                Motorista = relacaoMotoristaVeiculo.Where(y => y.CodigoVeiculo == x.Veiculo.Codigo).Select(y => y.Nome).FirstOrDefault() ?? string.Empty,
                CPF = relacaoMotoristaVeiculo.Where(y => y.CodigoVeiculo == x.Veiculo.Codigo).Select(y => y.CPF).FirstOrDefault() ?? string.Empty,
                Celular = relacaoMotoristaVeiculo.Where(y => y.CodigoVeiculo == x.Veiculo.Codigo).Select(y => y.ObterTelefoneFormatado()).FirstOrDefault() ?? string.Empty,
                RotaDeConhecimento = x.RotaDeConhecimento,
                ValidadeGrMotorista = relacaoMotoristaVeiculo.Where(y => y.CodigoVeiculo == x.Veiculo.Codigo).Select(y => y.GR_MotoristaValida()).FirstOrDefault() ?? string.Empty,
                ValidadeGrVeiculo = relacaoMotoristaVeiculo.Where(y => y.CodigoVeiculo == x.Veiculo.Codigo).Select(y => y.GR_VeiculoValida()).FirstOrDefault() ?? string.Empty,
                Tipo = x.Veiculo.Tipo == "P" ? "Próprio" : "Agregado",
                DisponivelEm = resultadoPesquisa.First().PlanejamentoFrotaDia.Data.ToDateString(), //é a data de planejamento diario do veiculo
                Indisponivel = x.Indisponivel,
                MotivoIndisponibilidade = x.JustificativaIndisponibilidade?.Descricao ?? string.Empty,
                ObsTransportador = x.ObservacaoTransportador,
                ObsMarfrig = x.ObservacaoMarfrig,
                Paletizado = x.Veiculo.Paletizado ? "Sim" : "Não",
                UltimoEmbarque = x.UltimoEmbarque.ToDateString(),
                CargaVinculada = obterCarregamentoDoDia(x.Veiculo.Codigo, x.PlanejamentoFrotaDia.Data),
                Redespacho = relacaoVeiculoCarga.Where(y => y.CodigoVeiculo == x.Veiculo.Codigo).Select(y => y.Redespacho ? "Sim" : "Não").FirstOrDefault() ?? string.Empty,
                DataFaturamento = obterDataFaturamento(x.Veiculo.Codigo),
                SugeridoPeloSistema = x.GeradoPeloSistema ? "Sim" : "Não",
                EmRodizio = x.Rodizio ? "Sim" : "Não",
                Roteirizado = x.Roteirizado,
                DT_RowClass = ObterCorPorStatus(x.Indisponivel),
                DT_RowColor = x.Roteirizado == true ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Verde : string.Empty,
            }).OrderByDescending(x => x.AnoModelo).ToList();

            return rows;
        }

        private Tuple<IList, int> ObterDadosGridAgrupada(Repositorio.UnitOfWork unitOfWork, int start, int limit)
        {
            var filtros = ObterFiltrosPesquisa();
            var repoPlanejamentoFrotaDiaVeiculo = new Repositorio.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo(unitOfWork);

            if (!EhEmbarcador())
                filtros.Transportador = new List<int> { Empresa.Codigo };

            var resultadoPesquisaComTotal = repoPlanejamentoFrotaDiaVeiculo.PesquisarListaDiaria(filtros, start, limit);
            var resultadoPesquisa = resultadoPesquisaComTotal.Item1;
            
            int codigo = 1;

            Func<int, string> obterDescricaoModeloVeicular = (int cod) =>
            {
                return resultadoPesquisa.Where(x => x.ModeloVeicular.Codigo == cod).Select(x => x.ModeloVeicular.Descricao).FirstOrDefault();
            };
            Func<int, string> obterDescricaoFilial = (int cod) =>
            {
                return resultadoPesquisa.Where(x => x.PlanejamentoFrotaDia.Filial.Codigo == cod).Select(x => x.PlanejamentoFrotaDia.Filial.Descricao).FirstOrDefault();
            };
            Func<int, DateTime, int> obterCodigoPlanejamento = (int codFilial, DateTime data) =>
            {
                return resultadoPesquisa.Where(x => x.PlanejamentoFrotaDia.Filial.Codigo == codFilial)
                    .Where(x => x.PlanejamentoFrotaDia.Data.Date == data.Date)
                    .Select(x => x.PlanejamentoFrotaDia.Codigo).FirstOrDefault();
            };

            if (limit <= 0)
                limit = 9999;

            var rows = resultadoPesquisa
                .GroupBy(x => new { Modelo = x.ModeloVeicular.Codigo, Filial = x.PlanejamentoFrotaDia.Filial.Codigo, Data = x.PlanejamentoFrotaDia.Data })
                .OrderBy(x => x.Key.Data).ThenBy(x => x.Key.Filial)
                .Select(x => new
                {
                    Codigo = codigo++,
                    CodigoPlanejamento = obterCodigoPlanejamento(x.Key.Filial, x.Key.Data),
                    CodigoFilial = x.Key.Filial,
                    CodigoModeloVeicular = x.Key.Modelo,
                    Filial = obterDescricaoFilial(x.Key.Filial),
                    ModeloVeicular = obterDescricaoModeloVeicular(x.Key.Modelo),
                    Data = x.Key.Data.ToDateString(),
                    QuantidadeSugerida = x.Count(),
                    QuantidadeDisponivel = x.Where(y => !y.Indisponivel).Count(),
                    QuantidadeRoteirizada = x.Where(y => y.Roteirizado).Count(),
                    ObsTransportador = string.Join(" ", x.Select(y => y.ObservacaoTransportador)),
                    ObsMarfrig = string.Join(" ", x.Select(y => y.ObservacaoMarfrig))
                });

            var contagemRegistros = rows.Count();

            return new Tuple<IList, int>(rows.Skip(start).Take(limit).ToList(), contagemRegistros);
        }

        private void ValidarBloqueioEdicao(Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDia planejamento)
        {
            if (EhEmbarcador())
                return;

            if (DateTime.Now.Date > planejamento.Data.Date)
                throw new ControllerException("Não é possível alterar um registro em uma data posterior ao planejamento.");

            if (!IsPeriodoDeHorasValido(planejamento))
                throw new ControllerException($"Não é possível fazer alterações fora do horário estabelecido. ({planejamento.Filial.HoraInicialPlanejamentoDiario.ToHourMinuteString()} e {planejamento.Filial.HoraFinalPlanejamentoDiario.ToHourMinuteString()})");

            if (!IsDiasDeToleranciaValido(planejamento))
                throw new ControllerException($"Não é possível fazer alterações. A tolerância é de {planejamento.Filial.DiasDeCortePlanejamentoDiario} dias úteis de diferença.");
        }

        private bool IsPeriodoDeHorasValido(Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDia planejamento)
        {
            //Validar periodo de hora:
            bool estaDentroDoPeriodoDeHoras = true;
            if (!planejamento.Filial.HoraInicialPlanejamentoDiario.IsNullOrMinValue())
            {
                var dataInicialConfigurada = DateTime.Now.Date.AddHours(planejamento.Filial.HoraInicialPlanejamentoDiario.Value.Hour)
                    .AddMinutes(planejamento.Filial.HoraInicialPlanejamentoDiario.Value.Minute);
                if (DateTime.Now < dataInicialConfigurada)
                    estaDentroDoPeriodoDeHoras = false;
            }
            if (!planejamento.Filial.HoraFinalPlanejamentoDiario.IsNullOrMinValue())
            {
                var dataFinalConfigurada = DateTime.Now.Date.AddHours(planejamento.Filial.HoraFinalPlanejamentoDiario.Value.Hour)
                    .AddMinutes(planejamento.Filial.HoraFinalPlanejamentoDiario.Value.Minute);
                if (DateTime.Now > dataFinalConfigurada)
                    estaDentroDoPeriodoDeHoras = false;
            }
            return estaDentroDoPeriodoDeHoras;
        }

        private bool IsDiasDeToleranciaValido(Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDia planejamento)
        {
            //validar dias de corte a partida da data de geracao da lista diaria (desconsiderar sabado e domingo):
            if (planejamento.Filial.DiasDeCortePlanejamentoDiario > 0)
            {
                var publicacao = planejamento.Data.Date;
                bool estaDentroDaDataLimite = false;

                int contagemDias = 0;
                int contagemDeSabadosEDomingos = 0;
                while (contagemDias <= planejamento.Filial.DiasDeCortePlanejamentoDiario + contagemDeSabadosEDomingos)
                {
                    var dia = publicacao.AddDays(contagemDias * -1);

                    if (dia.DayOfWeek == DayOfWeek.Sunday || dia.DayOfWeek == DayOfWeek.Saturday)
                    {
                        contagemDeSabadosEDomingos++;
                        contagemDias++;
                        continue;
                    }

                    var totalDeDiasDeTolerancia = (publicacao - dia).Days; // + contagemDeSabadosEDomingos;
                    var dataComTolerancia = publicacao.AddDays(totalDeDiasDeTolerancia * -1);
                    if (DateTime.Now.Date >= dataComTolerancia)
                        estaDentroDaDataLimite = true;
                    else
                        estaDentroDaDataLimite = false;

                    contagemDias++;
                }
                return estaDentroDaDataLimite;
            }
            return true;
        }

        private string ObterCorPorStatus(bool indisponivel)
        {
            if (indisponivel)
                return ClasseCorFundo.Danger(IntensidadeCor._100);

            return string.Empty;
        }

        private bool EhEmbarcador()
        {
            return TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador;
        }

        #endregion Métodos Privados
    }
}
