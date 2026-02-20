using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/CapacidadeCarregamentoAdicional")]
    public class CapacidadeCarregamentoAdicionalController : BaseController
    {
		#region Construtores

		public CapacidadeCarregamentoAdicionalController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Logistica.CapacidadeCarregamentoAdicional capacidadeCarregamentoAdicional = new Dominio.Entidades.Embarcador.Logistica.CapacidadeCarregamentoAdicional();

                PreencherCapacidadeCarregamentoAdicional(capacidadeCarregamentoAdicional, unitOfWork);

                Repositorio.Embarcador.Logistica.CapacidadeCarregamentoAdicional repositorio = new Repositorio.Embarcador.Logistica.CapacidadeCarregamentoAdicional(unitOfWork);

                repositorio.Inserir(capacidadeCarregamentoAdicional, Auditado);

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
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar os dados.");
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
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.CapacidadeCarregamentoAdicional repositorio = new Repositorio.Embarcador.Logistica.CapacidadeCarregamentoAdicional(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.CapacidadeCarregamentoAdicional capacidadeCarregamentoAdicional = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (capacidadeCarregamentoAdicional == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                PreencherCapacidadeCarregamentoAdicional(capacidadeCarregamentoAdicional, unitOfWork);

                repositorio.Atualizar(capacidadeCarregamentoAdicional, Auditado);

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
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar os dados.");
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.CapacidadeCarregamentoAdicional repositorio = new Repositorio.Embarcador.Logistica.CapacidadeCarregamentoAdicional(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.CapacidadeCarregamentoAdicional capacidadeCarregamentoAdicional = repositorio.BuscarPorCodigo(codigo, auditavel: false);

                if (capacidadeCarregamentoAdicional == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    capacidadeCarregamentoAdicional.Codigo,
                    CentroCarregamento = new
                    {
                        capacidadeCarregamentoAdicional.CentroCarregamento.Codigo,
                        capacidadeCarregamentoAdicional.CentroCarregamento.Descricao,
                        capacidadeCarregamentoAdicional.CentroCarregamento.TipoCapacidadeCarregamentoPorPeso,
                        capacidadeCarregamentoAdicional.CentroCarregamento.TipoCapacidadeCarregamento
                    },
                    capacidadeCarregamentoAdicional.CapacidadeCarregamento,
                    Data = capacidadeCarregamentoAdicional.Data.ToString("dd/MM/yyyy"),
                    HoraInicio = capacidadeCarregamentoAdicional.PeriodoInicio.HasValue ? capacidadeCarregamentoAdicional.PeriodoInicio.Value.ToString("HH:mm") : "",
                    HoraTermino = capacidadeCarregamentoAdicional.PeriodoTermino.HasValue ? capacidadeCarregamentoAdicional.PeriodoTermino.Value.ToString("HH:mm") : "",
                    capacidadeCarregamentoAdicional.Observacao,
                    PermitirEdicao = capacidadeCarregamentoAdicional.Data >= DateTime.Now.Date,
                    PeriodoCarregamento = new
                    {
                        Codigo = capacidadeCarregamentoAdicional.PeriodoInicio.HasValue ? Guid.NewGuid().ToString() : "0",
                        Descricao = capacidadeCarregamentoAdicional.PeriodoInicio.HasValue ? $"De {capacidadeCarregamentoAdicional.PeriodoInicio.Value.ToString("HH:mm")} até {capacidadeCarregamentoAdicional.PeriodoTermino.Value.ToString("HH:mm")}" : ""
                    },
                    capacidadeCarregamentoAdicional.CapacidadeCarregamentoVolume,
                    capacidadeCarregamentoAdicional.CapacidadeCarregamentoCubagem,
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.CapacidadeCarregamentoAdicional repositorio = new Repositorio.Embarcador.Logistica.CapacidadeCarregamentoAdicional(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.CapacidadeCarregamentoAdicional capacidadeCarregamentoAdicional = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (capacidadeCarregamentoAdicional == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(capacidadeCarregamentoAdicional, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
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

        #endregion

        #region Métodos Privados

        private void PreencherCapacidadeCarregamentoAdicional(Dominio.Entidades.Embarcador.Logistica.CapacidadeCarregamentoAdicional capacidadeCarregamentoAdicional, Repositorio.UnitOfWork unitOfWork)
        {
            int codigoCentroCarregamento = Request.GetIntParam("CentroCarregamento");
            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repositorioCentroCarregamento.BuscarPorCodigo(codigoCentroCarregamento) ?? throw new ControllerException("O centro de carregamento deve ser informado.");
            TimeSpan? horaInicio = Request.GetNullableTimeParam("HoraInicio");
            TimeSpan? horaTermino = Request.GetNullableTimeParam("HoraTermino");

            capacidadeCarregamentoAdicional.CapacidadeCarregamento = Request.GetIntParam("CapacidadeCarregamento");
            capacidadeCarregamentoAdicional.CentroCarregamento = centroCarregamento;
            capacidadeCarregamentoAdicional.Data = Request.GetDateTimeParam("Data");
            capacidadeCarregamentoAdicional.PeriodoInicio = (horaInicio.HasValue) ? (DateTime?)capacidadeCarregamentoAdicional.Data.Date.Add(horaInicio.Value) : null;
            capacidadeCarregamentoAdicional.PeriodoTermino = (horaTermino.HasValue) ? (DateTime?)capacidadeCarregamentoAdicional.Data.Date.Add(horaTermino.Value) : null;
            capacidadeCarregamentoAdicional.Observacao = Request.GetNullableStringParam("Observacao");
            capacidadeCarregamentoAdicional.CapacidadeCarregamentoVolume = Request.GetIntParam("CapacidadeCarregamentoVolume");
            capacidadeCarregamentoAdicional.CapacidadeCarregamentoCubagem = Request.GetIntParam("CapacidadeCarregamentoCubagem");

            if (capacidadeCarregamentoAdicional.Codigo == 0)
                capacidadeCarregamentoAdicional.Usuario = this.Usuario;

            if (capacidadeCarregamentoAdicional.Data < DateTime.Now.Date)
                throw new ControllerException("A data deve ser maior ou igual a data atual.");
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCapacidadeCarregamentoAdicional ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCapacidadeCarregamentoAdicional()
            {
                CodigoCentroCarregamento = Request.GetIntParam("CentroCarregamento"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite")
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Centro de Carregamento", "CentroCarregamento", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Capacidade", "CapacidadeCarregamento", 15, Models.Grid.Align.right, false);

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCapacidadeCarregamentoAdicional filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Logistica.CapacidadeCarregamentoAdicional repositorio = new Repositorio.Embarcador.Logistica.CapacidadeCarregamentoAdicional(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Logistica.CapacidadeCarregamentoAdicional> listaCapacidadeCarregamentoAdicional = (totalRegistros > 0) ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.CapacidadeCarregamentoAdicional>();

                var listaCapacidadeCarregamentoAdicionalRetornar = (
                    from capacidadeCarregamentoAdicional in listaCapacidadeCarregamentoAdicional
                    select new
                    {
                        capacidadeCarregamentoAdicional.Codigo,
                        Data = capacidadeCarregamentoAdicional.Data.ToString("dd/MM/yyyy"),
                        CentroCarregamento = capacidadeCarregamentoAdicional.CentroCarregamento.Descricao,
                        CapacidadeCarregamento = capacidadeCarregamentoAdicional.CapacidadeCarregamento.ToString("n0")
                    }
                ).ToList();

                grid.AdicionaRows(listaCapacidadeCarregamentoAdicionalRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "CentroCarregamento")
                return "CentroCarregamento.Descricao";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
