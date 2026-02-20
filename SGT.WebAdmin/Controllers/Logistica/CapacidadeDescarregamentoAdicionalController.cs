using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/CapacidadeDescarregamentoAdicional")]
    public class CapacidadeDescarregamentoAdicionalController : BaseController
    {
		#region Construtores

		public CapacidadeDescarregamentoAdicionalController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Logistica.CapacidadeDescarregamentoAdicional capacidadeDescarregamentoAdicional = new Dominio.Entidades.Embarcador.Logistica.CapacidadeDescarregamentoAdicional();

                PreencherCapacidadeDescarregamentoAdicional(capacidadeDescarregamentoAdicional, unitOfWork);
                PreencherCapacidadeDescarregamentoAdicionalCanaisVenda(capacidadeDescarregamentoAdicional, unitOfWork);

                Repositorio.Embarcador.Logistica.CapacidadeDescarregamentoAdicional repositorio = new Repositorio.Embarcador.Logistica.CapacidadeDescarregamentoAdicional(unitOfWork);

                repositorio.Inserir(capacidadeDescarregamentoAdicional, Auditado);

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
                Repositorio.Embarcador.Logistica.CapacidadeDescarregamentoAdicional repositorio = new Repositorio.Embarcador.Logistica.CapacidadeDescarregamentoAdicional(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.CapacidadeDescarregamentoAdicional capacidadeDescarregamentoAdicional = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (capacidadeDescarregamentoAdicional == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                PreencherCapacidadeDescarregamentoAdicional(capacidadeDescarregamentoAdicional, unitOfWork);

                repositorio.Atualizar(capacidadeDescarregamentoAdicional, Auditado);

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
                Repositorio.Embarcador.Logistica.CapacidadeDescarregamentoAdicional repositorio = new Repositorio.Embarcador.Logistica.CapacidadeDescarregamentoAdicional(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.CapacidadeDescarregamentoAdicional capacidadeDescarregamentoAdicional = repositorio.BuscarPorCodigo(codigo, auditavel: false);

                if (capacidadeDescarregamentoAdicional == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    capacidadeDescarregamentoAdicional.Codigo,
                    CentroDescarregamento = new
                    {
                        capacidadeDescarregamentoAdicional.CentroDescarregamento.Codigo,
                        capacidadeDescarregamentoAdicional.CentroDescarregamento.Descricao,
                        capacidadeDescarregamentoAdicional.CentroDescarregamento.TipoCapacidadeDescarregamentoPorPeso
                    },
                    capacidadeDescarregamentoAdicional.CapacidadeDescarregamento,
                    Data = capacidadeDescarregamentoAdicional.Data.ToString("dd/MM/yyyy"),
                    HoraInicio = capacidadeDescarregamentoAdicional.PeriodoInicio.HasValue ? capacidadeDescarregamentoAdicional.PeriodoInicio.Value.ToString("HH:mm") : "",
                    HoraTermino = capacidadeDescarregamentoAdicional.PeriodoTermino.HasValue ? capacidadeDescarregamentoAdicional.PeriodoTermino.Value.ToString("HH:mm") : "",
                    capacidadeDescarregamentoAdicional.Observacao,
                    PermitirEdicao = capacidadeDescarregamentoAdicional.Data >= DateTime.Now.Date,
                    PeriodoDescarregamento = new
                    {
                        Codigo = capacidadeDescarregamentoAdicional.PeriodoInicio.HasValue ? Guid.NewGuid().ToString() : "0",
                        Descricao = capacidadeDescarregamentoAdicional.PeriodoInicio.HasValue ? $"De {capacidadeDescarregamentoAdicional.PeriodoInicio.Value.ToString("HH:mm")} até {capacidadeDescarregamentoAdicional.PeriodoTermino.Value.ToString("HH:mm")}" : ""
                    }
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
                Repositorio.Embarcador.Logistica.CapacidadeDescarregamentoAdicional repositorio = new Repositorio.Embarcador.Logistica.CapacidadeDescarregamentoAdicional(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.CapacidadeDescarregamentoAdicional capacidadeDescarregamentoAdicional = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (capacidadeDescarregamentoAdicional == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(capacidadeDescarregamentoAdicional, Auditado);

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

        private void PreencherCapacidadeDescarregamentoAdicional(Dominio.Entidades.Embarcador.Logistica.CapacidadeDescarregamentoAdicional capacidadeDescarregamentoAdicional, Repositorio.UnitOfWork unitOfWork)
        {
            int codigoCentroDescarregamento = Request.GetIntParam("CentroDescarregamento");
            Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = repositorioCentroDescarregamento.BuscarPorCodigo(codigoCentroDescarregamento) ?? throw new ControllerException("O centro de desccarregamento deve ser informado.");
            TimeSpan? horaInicio = Request.GetNullableTimeParam("HoraInicio");
            TimeSpan? horaTermino = Request.GetNullableTimeParam("HoraTermino");

            capacidadeDescarregamentoAdicional.CapacidadeDescarregamento = Request.GetIntParam("CapacidadeDescarregamento");
            capacidadeDescarregamentoAdicional.CentroDescarregamento = centroDescarregamento;
            capacidadeDescarregamentoAdicional.Data = Request.GetDateTimeParam("Data");
            capacidadeDescarregamentoAdicional.PeriodoInicio = (horaInicio.HasValue) ? (DateTime?)capacidadeDescarregamentoAdicional.Data.Date.Add(horaInicio.Value) : null;
            capacidadeDescarregamentoAdicional.PeriodoTermino = (horaTermino.HasValue) ? (DateTime?)capacidadeDescarregamentoAdicional.Data.Date.Add(horaTermino.Value) : null;
            capacidadeDescarregamentoAdicional.Observacao = Request.GetNullableStringParam("Observacao");

            if (capacidadeDescarregamentoAdicional.Codigo == 0)
                capacidadeDescarregamentoAdicional.Usuario = this.Usuario;

            if (capacidadeDescarregamentoAdicional.Data < DateTime.Now.Date)
                throw new ControllerException("A data deve ser maior ou igual a data atual.");
        }

        private void PreencherCapacidadeDescarregamentoAdicionalCanaisVenda(Dominio.Entidades.Embarcador.Logistica.CapacidadeDescarregamentoAdicional capacidadeDescarregamentoAdicional, Repositorio.UnitOfWork unitOfWork)
        {
            int codigoPeriodoDescarregamento = Request.GetIntParam("PeriodoDescarregamento");
            Repositorio.Embarcador.Logistica.PeriodoDescarregamento repositorioPeriodoDescarregamento = new Repositorio.Embarcador.Logistica.PeriodoDescarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodoDescarregamento = repositorioPeriodoDescarregamento.BuscarPorCodigo(codigoPeriodoDescarregamento);

            if (periodoDescarregamento == null)
                return;

            capacidadeDescarregamentoAdicional.CanaisVenda = periodoDescarregamento.CanaisVenda.Select(canalVenda => canalVenda.CanalVenda).ToList();
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCapacidadeDescarregamentoAdicional ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCapacidadeDescarregamentoAdicional()
            {
                CodigoCentroDescarregamento = Request.GetIntParam("CentroDescarregamento"),
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
                grid.AdicionarCabecalho("Centro de Descarregamento", "CentroDescarregamento", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Capacidade", "CapacidadeDescarregamento", 15, Models.Grid.Align.right, false);

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCapacidadeDescarregamentoAdicional filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Logistica.CapacidadeDescarregamentoAdicional repositorio = new Repositorio.Embarcador.Logistica.CapacidadeDescarregamentoAdicional(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Logistica.CapacidadeDescarregamentoAdicional> listaCapacidadeDescarregamentoAdicional = (totalRegistros > 0) ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.CapacidadeDescarregamentoAdicional>();

                var listaCapacidadeDescarregamentoAdicionalRetornar = (
                    from capacidadeDescarregamentoAdicional in listaCapacidadeDescarregamentoAdicional
                    select new
                    {
                        capacidadeDescarregamentoAdicional.Codigo,
                        Data = capacidadeDescarregamentoAdicional.Data.ToString("dd/MM/yyyy"),
                        CentroDescarregamento = capacidadeDescarregamentoAdicional.CentroDescarregamento.Descricao,
                        CapacidadeDescarregamento = capacidadeDescarregamentoAdicional.CapacidadeDescarregamento.ToString("n0")
                    }
                ).ToList();

                grid.AdicionaRows(listaCapacidadeDescarregamentoAdicionalRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "CentroDescarregamento")
                return "CentroDescarregamento.Descricao";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
