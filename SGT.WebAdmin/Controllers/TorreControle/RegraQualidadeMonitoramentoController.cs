using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.TorreControle
{
    [CustomAuthorize("TorreControle/RegraQualidadeMonitoramento")]
    public class RegraQualidadeMonitoramentoController : BaseController
    {
        #region Construtores

        public RegraQualidadeMonitoramentoController(Conexao conexao) : base(conexao) { }

        #endregion
        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return new JsonpResult(ObterGridPesquisa(unitOfWork));
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                string descricao = Request.GetStringParam("Descricao");
                bool ativo = Request.GetBoolParam("Status");
                TipoRegraQualidadeMonitoramento tipoRegra = Request.GetEnumParam<TipoRegraQualidadeMonitoramento>("TipoRegra");

                Repositorio.Embarcador.TorreControle.RegraQualidadeMonitoramento repositorioRegraQualidadeMonitoramento = new Repositorio.Embarcador.TorreControle.RegraQualidadeMonitoramento(unitOfWork);

                Dominio.Entidades.Embarcador.TorreControle.RegraQualidadeMonitoramento regraQualidadeMonitoramento = new Dominio.Entidades.Embarcador.TorreControle.RegraQualidadeMonitoramento()
                {
                    Descricao = descricao,
                    Ativo = ativo,
                    TipoRegraQualidadeMonitoramento = tipoRegra
                };

                unitOfWork.Start();

                repositorioRegraQualidadeMonitoramento.Inserir(regraQualidadeMonitoramento);

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
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar a regra.");
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
                int codigo = Request.GetIntParam("Codigo");
                string descricao = Request.GetStringParam("Descricao");
                bool ativo = Request.GetBoolParam("Status");
                TipoRegraQualidadeMonitoramento tipoRegra = Request.GetEnumParam<TipoRegraQualidadeMonitoramento>("TipoRegra");

                Repositorio.Embarcador.TorreControle.RegraQualidadeMonitoramento repositorioRegraQualidadeMonitoramento = new Repositorio.Embarcador.TorreControle.RegraQualidadeMonitoramento(unitOfWork);
                Dominio.Entidades.Embarcador.TorreControle.RegraQualidadeMonitoramento regraQualidadeMonitoramento = repositorioRegraQualidadeMonitoramento.BuscarPorCodigo(codigo, false);

                if (regraQualidadeMonitoramento == null)
                    throw new ControllerException("Regra não encontrada");

                regraQualidadeMonitoramento.Descricao = descricao;
                regraQualidadeMonitoramento.Ativo = ativo;
                regraQualidadeMonitoramento.TipoRegraQualidadeMonitoramento = tipoRegra;

                unitOfWork.Start();

                repositorioRegraQualidadeMonitoramento.Atualizar(regraQualidadeMonitoramento);

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
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar a regra.");
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

                Repositorio.Embarcador.TorreControle.RegraQualidadeMonitoramento repositorioRegraQualidadeMonitoramento = new Repositorio.Embarcador.TorreControle.RegraQualidadeMonitoramento(unitOfWork);
                Dominio.Entidades.Embarcador.TorreControle.RegraQualidadeMonitoramento regraQualidadeMonitoramento = repositorioRegraQualidadeMonitoramento.BuscarPorCodigo(codigo, false);

                if (regraQualidadeMonitoramento == null)
                    return new JsonpResult(false, "Regra não encontrada");

                var retorno = new
                {
                    regraQualidadeMonitoramento.Codigo,
                    regraQualidadeMonitoramento.Descricao,
                    Status = regraQualidadeMonitoramento.Ativo,
                    TipoRegra = regraQualidadeMonitoramento.TipoRegraQualidadeMonitoramento
                };

                return new JsonpResult(retorno);
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

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descrição", "Descricao", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tipo da Regra", "TipoRegra", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Status", "Status", 7, Models.Grid.Align.left, true);

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
            Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRegraQualidadeMonitoramento filtroPesquisa = ObterFiltrosPesquisa();

            Repositorio.Embarcador.TorreControle.RegraQualidadeMonitoramento repositorioRegraQualidadeMonitoramento = new Repositorio.Embarcador.TorreControle.RegraQualidadeMonitoramento(unitOfWork);

            int totalRegistros = repositorioRegraQualidadeMonitoramento.ContarConsulta(filtroPesquisa);
            List<Dominio.Entidades.Embarcador.TorreControle.RegraQualidadeMonitoramento> regrasQualidadeMonitoramento = totalRegistros > 0 ? repositorioRegraQualidadeMonitoramento.Consultar(filtroPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.TorreControle.RegraQualidadeMonitoramento>();

            var listaRetornar = (
                from regra in regrasQualidadeMonitoramento
                select new
                {
                    regra.Codigo,
                    regra.Descricao,
                    TipoRegra = regra.TipoRegraQualidadeMonitoramento.ObterDescricao() ?? string.Empty,
                    Status = regra.Ativo ? "Ativo" : "Inativo",
                }
            ).ToList();

            grid.AdicionaRows(listaRetornar);
            grid.setarQuantidadeTotal(totalRegistros);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRegraQualidadeMonitoramento ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRegraQualidadeMonitoramento()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Ativo = Request.GetNullableBoolParam("Status"),
                TipoRegra = Request.GetNullableEnumParam<TipoRegraQualidadeMonitoramento>("TipoRegra")
            };
        }

        #endregion
    }
}