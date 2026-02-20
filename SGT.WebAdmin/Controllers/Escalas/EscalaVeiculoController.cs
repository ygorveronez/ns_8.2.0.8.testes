using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Escalas
{
    [CustomAuthorize("Escalas/EscalaVeiculo")]
    public class EscalaVeiculoController : BaseController
    {
		#region Construtores

		public EscalaVeiculoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoVeiculo = Request.GetIntParam("Veiculo");
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorCodigo(codigoVeiculo);

                if (veiculo == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                Repositorio.Embarcador.Escalas.EscalaVeiculo repositorioEscalaVeiculo = new Repositorio.Embarcador.Escalas.EscalaVeiculo(unitOfWork);
                Dominio.Entidades.Embarcador.Escalas.EscalaVeiculo escalaVeiculo = repositorioEscalaVeiculo.BuscarPorVeiculo(codigoVeiculo);

                if (escalaVeiculo != null)
                    throw new ControllerException("O veículo já está adicionado na escala.");

                if ((veiculo.ModeloVeicularCarga?.CapacidadePesoTransporte ?? 0m) <= 0m)
                    throw new ControllerException("O veículo não possui uma capacidade de carregamento definida em seu modelo veicular de carga.");

                escalaVeiculo = new Dominio.Entidades.Embarcador.Escalas.EscalaVeiculo()
                {
                    Situacao = SituacaoEscalaVeiculo.EmEscala,
                    Veiculo = veiculo
                };

                repositorioEscalaVeiculo.Inserir(escalaVeiculo);

                Repositorio.Embarcador.Escalas.EscalaVeiculoHistorico repositorioEscalaVeiculoHistorico = new Repositorio.Embarcador.Escalas.EscalaVeiculoHistorico(unitOfWork);
                Dominio.Entidades.Embarcador.Escalas.EscalaVeiculoHistorico historico = new Dominio.Entidades.Embarcador.Escalas.EscalaVeiculoHistorico()
                {
                    Data = DateTime.Now,
                    Descricao = "Veículo adicionado na escala",
                    EscalaVeiculo = escalaVeiculo,
                    Situacao = SituacaoEscalaVeiculo.EmEscala
                };

                repositorioEscalaVeiculoHistorico.Inserir(historico);

                escalaVeiculo.UltimoHistorico = historico;

                repositorioEscalaVeiculo.Atualizar(escalaVeiculo);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar o veículo na escala.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaDetalhes()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisaDetalhes());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> RemoverSuspensao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoEscalaVeiculo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Escalas.EscalaVeiculo repositorioEscalaVeiculo = new Repositorio.Embarcador.Escalas.EscalaVeiculo(unitOfWork);
                Dominio.Entidades.Embarcador.Escalas.EscalaVeiculo escalaVeiculo = repositorioEscalaVeiculo.BuscarPorCodigo(codigoEscalaVeiculo, auditavel: false);

                if (escalaVeiculo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (escalaVeiculo.Situacao != SituacaoEscalaVeiculo.Suspenso)
                    return new JsonpResult(false, true, "A situação atual do veículo na escala não permite a suspensão.");

                if ((escalaVeiculo.Veiculo.ModeloVeicularCarga?.CapacidadePesoTransporte ?? 0m) <= 0m)
                    throw new ControllerException("O veículo não possui uma capacidade de carregamento definida em seu modelo veicular de carga.");

                unitOfWork.Start();

                Repositorio.Embarcador.Escalas.EscalaVeiculoHistorico repositorioEscalaVeiculoHistorico = new Repositorio.Embarcador.Escalas.EscalaVeiculoHistorico(unitOfWork);
                Dominio.Entidades.Embarcador.Escalas.EscalaVeiculoHistorico historico = new Dominio.Entidades.Embarcador.Escalas.EscalaVeiculoHistorico()
                {
                    Data = DateTime.Now,
                    Descricao = $"Suspensão do veículo da escala removida",
                    EscalaVeiculo = escalaVeiculo,
                    Situacao = SituacaoEscalaVeiculo.EmEscala
                };

                repositorioEscalaVeiculoHistorico.Inserir(historico);

                escalaVeiculo.Situacao = SituacaoEscalaVeiculo.EmEscala;
                escalaVeiculo.UltimoHistorico = historico;

                repositorioEscalaVeiculo.Atualizar(escalaVeiculo);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao remover a suspensão do veículo da escala.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Suspender()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoEscalaVeiculo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Escalas.EscalaVeiculo repositorioEscalaVeiculo = new Repositorio.Embarcador.Escalas.EscalaVeiculo(unitOfWork);
                Dominio.Entidades.Embarcador.Escalas.EscalaVeiculo escalaVeiculo = repositorioEscalaVeiculo.BuscarPorCodigo(codigoEscalaVeiculo, auditavel: false);

                if (escalaVeiculo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (escalaVeiculo.Situacao != SituacaoEscalaVeiculo.EmEscala)
                    return new JsonpResult(false, true, "A situação atual do veículo na escala não permite a suspensão.");

                int codigoMotivoRemocaoVeiculoEscala = Request.GetIntParam("MotivoRemocao");
                Repositorio.Embarcador.Escalas.MotivoRemocaoVeiculoEscala repositorioMotivoRemocaoVeiculoEscala = new Repositorio.Embarcador.Escalas.MotivoRemocaoVeiculoEscala(unitOfWork);
                Dominio.Entidades.Embarcador.Escalas.MotivoRemocaoVeiculoEscala motivoRemocao = repositorioMotivoRemocaoVeiculoEscala.BuscarPorCodigo(codigoMotivoRemocaoVeiculoEscala, auditavel: false);

                if (motivoRemocao == null)
                    return new JsonpResult(false, true, "O motivo de suspensão do veículo da escala deve ser informado.");

                DateTime? dataPrevisaoRetorno = Request.GetNullableDateTimeParam("DataPrevisaoRetorno");

                if (dataPrevisaoRetorno.HasValue && (dataPrevisaoRetorno.Value.Date < DateTime.Now.Date))
                    return new JsonpResult(false, true, "A data prevista para o retorno do veícula a escala deve ser maior ou igual a atual.");

                unitOfWork.Start();

                Repositorio.Embarcador.Escalas.EscalaVeiculoHistorico repositorioEscalaVeiculoHistorico = new Repositorio.Embarcador.Escalas.EscalaVeiculoHistorico(unitOfWork);
                Dominio.Entidades.Embarcador.Escalas.EscalaVeiculoHistorico historico = new Dominio.Entidades.Embarcador.Escalas.EscalaVeiculoHistorico()
                {
                    Data = DateTime.Now,
                    DataPrevisaoRetorno = dataPrevisaoRetorno,
                    Descricao = $"Veículo suspenso da escala. Motivo: {motivoRemocao.Descricao}",
                    EscalaVeiculo = escalaVeiculo,
                    MotivoRemocao = motivoRemocao,
                    Situacao = SituacaoEscalaVeiculo.Suspenso
                };

                repositorioEscalaVeiculoHistorico.Inserir(historico);

                escalaVeiculo.Situacao = SituacaoEscalaVeiculo.Suspenso;
                escalaVeiculo.UltimoHistorico = historico;

                repositorioEscalaVeiculo.Atualizar(escalaVeiculo);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao suspender o veículo da escala.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Escalas.FiltroPesquisaEscalaVeiculo ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Escalas.FiltroPesquisaEscalaVeiculo filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Escalas.FiltroPesquisaEscalaVeiculo()
            {
                CodigoModeloVeicularCarga = Request.GetIntParam("ModeloVeicularCarga"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                Situacao = Request.GetNullableEnumParam<SituacaoEscalaVeiculo>("Situacao"),
                SomenteVeiculosDataPrevisaoRetornoExcedida = Request.GetBoolParam("SomenteVeiculosDataPrevisaoRetornoExcedida")
            };

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPesquisa()
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
                grid.AdicionarCabecalho("Veículo", "Placa", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Modelo Veicular", "ModeloVeicularCarga", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "SituacaoDescricao", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Previsão de Retorno", "DataPrevisaoRetorno", 15, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.Escalas.FiltroPesquisaEscalaVeiculo filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Escalas.EscalaVeiculo repositorio = new Repositorio.Embarcador.Escalas.EscalaVeiculo(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Escalas.EscalaVeiculo> listaEscalaVeiculo = (totalRegistros > 0) ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Escalas.EscalaVeiculo>();

                var listaEscalaVeiculoRetornar = (
                    from escalaVeiculo in listaEscalaVeiculo
                    select new
                    {
                        escalaVeiculo.Codigo,
                        Placa = escalaVeiculo.Veiculo.Placa_Formatada,
                        ModeloVeicularCarga = escalaVeiculo.Veiculo.ModeloVeicularCarga?.Descricao ?? "",
                        escalaVeiculo.Situacao,
                        SituacaoDescricao = escalaVeiculo.Situacao.ObterDescricao(),
                        DataPrevisaoRetorno = escalaVeiculo.UltimoHistorico.DataPrevisaoRetorno?.ToString("dd/MM/yyyy") ?? ""
                    }
                ).ToList();

                grid.AdicionaRows(listaEscalaVeiculoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Models.Grid.Grid ObterGridPesquisaDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 45, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "SituacaoDescricao", 15, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Previsão de Retorno", "DataPrevisaoRetorno", 20, Models.Grid.Align.center, false);

                int codigoVeiculoEscala = Request.GetIntParam("Codigo");
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Escalas.EscalaVeiculoHistorico repositorio = new Repositorio.Embarcador.Escalas.EscalaVeiculoHistorico(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(codigoVeiculoEscala);
                List<Dominio.Entidades.Embarcador.Escalas.EscalaVeiculoHistorico> listaEscalaVeiculoHistorico = (totalRegistros > 0) ? repositorio.Consultar(codigoVeiculoEscala, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Escalas.EscalaVeiculoHistorico>();

                var listaEscalaVeiculoHistoricoRetornar = (
                    from historico in listaEscalaVeiculoHistorico
                    select new
                    {
                        historico.Codigo,
                        historico.Descricao,
                        Data = historico.Data.ToString("dd/MM/yyyy HH:mm"),
                        DataPrevisaoRetorno = historico.DataPrevisaoRetorno?.ToString("dd/MM/yyyy") ?? "",
                        SituacaoDescricao = historico.Situacao.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(listaEscalaVeiculoHistoricoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Placa")
                return "Veiculo.Placa";

            return propriedadeOrdenar;
        }

        #endregion
    }
}

