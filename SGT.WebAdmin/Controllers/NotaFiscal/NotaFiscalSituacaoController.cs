using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NotaFiscal
{
    [CustomAuthorize(new string[] { "NotasFiscais/NotaFiscalSituacao" })]
    public class NotaFiscalSituacaoController : BaseController
    {
		#region Construtores

		public NotaFiscalSituacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Públicos

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.NotaFiscal.NotaFiscalSituacao repositorioNotaFiscalSituacao = new Repositorio.Embarcador.NotaFiscal.NotaFiscalSituacao(unitOfWork);

                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalSituacao notaFiscalSituacao = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalSituacao();

                PreencherEntidade(notaFiscalSituacao);
                VerificarGatilho(notaFiscalSituacao, unitOfWork);

                repositorioNotaFiscalSituacao.Inserir(notaFiscalSituacao);

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
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false);
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

                Repositorio.Embarcador.NotaFiscal.NotaFiscalSituacao repositorioNotaFiscalSituacao = new Repositorio.Embarcador.NotaFiscal.NotaFiscalSituacao(unitOfWork);

                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalSituacao notaFiscalSituacao = repositorioNotaFiscalSituacao.BuscarPorCodigo(codigo, false);

                if (notaFiscalSituacao == null)
                    return new JsonpResult(false, "Registro não encontrado.");

                PreencherEntidade(notaFiscalSituacao);
                VerificarGatilho(notaFiscalSituacao, unitOfWork);

                repositorioNotaFiscalSituacao.Atualizar(notaFiscalSituacao);

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
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false);
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

                Repositorio.Embarcador.NotaFiscal.NotaFiscalSituacao repositorioNotaFiscalSituacao = new Repositorio.Embarcador.NotaFiscal.NotaFiscalSituacao(unitOfWork);

                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalSituacao notaFiscalSituacao = repositorioNotaFiscalSituacao.BuscarPorCodigo(codigo, false);

                if (notaFiscalSituacao == null)
                    return new JsonpResult(false, "Registro não encontrado.");

                return new JsonpResult(new
                {
                    notaFiscalSituacao.Codigo,
                    notaFiscalSituacao.Descricao,
                    notaFiscalSituacao.Observacao,
                    Gatilho = notaFiscalSituacao.NotaFiscalSituacaoGatilho,
                    Situacao = notaFiscalSituacao.Ativo,
                    notaFiscalSituacao.BloquearVisualizacaoAgendamentoEntregaPedido,
                    notaFiscalSituacao.FinalizarAgendamentoEntregaPedido
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();

                Repositorio.Embarcador.NotaFiscal.NotaFiscalSituacao repositorioNotaFiscalSituacao = new Repositorio.Embarcador.NotaFiscal.NotaFiscalSituacao(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaNotaFiscalSituacao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = repositorioNotaFiscalSituacao.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalSituacao> listaRetorno = totalRegistros > 0 ? repositorioNotaFiscalSituacao.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalSituacao>();

                grid.setarQuantidadeTotal(totalRegistros);

                var lista = (from p in listaRetorno
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 Situacao = p.DescricaoAtivo,
                                 p.Observacao
                             }).ToList();

                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao pesquisar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descrição", "Descricao", 35, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação", "Situacao", 35, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Observação", "Observacao", 35, Models.Grid.Align.center, true);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaNotaFiscalSituacao ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaNotaFiscalSituacao filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaNotaFiscalSituacao();

            int situacao = Request.GetIntParam("Situacao");
            bool? ativo = null;

            if (situacao == 1)
                ativo = true;
            else if (situacao == 2)
                ativo = false;

            filtrosPesquisa.Descricao = Request.GetStringParam("Descricao");
            filtrosPesquisa.SituacaoAtivo = ativo;
            
            return filtrosPesquisa;
        }

        private void PreencherEntidade(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalSituacao notaFiscalSituacao)
        {
            notaFiscalSituacao.Ativo = Request.GetBoolParam("Situacao");
            notaFiscalSituacao.Descricao = Request.GetStringParam("Descricao");
            notaFiscalSituacao.Observacao = Request.GetStringParam("Observacao");
            notaFiscalSituacao.NotaFiscalSituacaoGatilho = Request.GetEnumParam<NotaFiscalSituacaoGatilho>("Gatilho");
            notaFiscalSituacao.BloquearVisualizacaoAgendamentoEntregaPedido = Request.GetBoolParam("BloquearVisualizacaoAgendamentoEntregaPedido");
            notaFiscalSituacao.FinalizarAgendamentoEntregaPedido = Request.GetBoolParam("FinalizarAgendamentoEntregaPedido");
        }

        private void VerificarGatilho(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalSituacao notaFiscalSituacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscalSituacao repositorio = new Repositorio.Embarcador.NotaFiscal.NotaFiscalSituacao(unitOfWork);

            if (notaFiscalSituacao.NotaFiscalSituacaoGatilho == NotaFiscalSituacaoGatilho.SemGatilho)
                return;

            if (repositorio.VerificarExistenciaRegistroAtivoGatilho(notaFiscalSituacao))
                throw new ControllerException("Já existe um registro ativo com o gatilho selecionado.");
        }

        #endregion
    }
}
