using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fechamento
{
    [CustomAuthorize("Fechamento/FechamentoJustificativaAcrescimoDesconto")]
    public class FechamentoJustificativaAcrescimoDescontoController : BaseController
    {
		#region Construtores

		public FechamentoJustificativaAcrescimoDescontoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return new JsonpResult(ObterGridPesquisa());
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

                Repositorio.Embarcador.Fechamento.FechamentoJustificativaAcrescimoDesconto repFechamentoJustificativaAcrescimoDesconto = new Repositorio.Embarcador.Fechamento.FechamentoJustificativaAcrescimoDesconto(unitOfWork);
                Dominio.Entidades.Embarcador.Fechamento.FechamentoJustificativaAcrescimoDesconto fechamentoJustificativaAcrescimoDesconto = new Dominio.Entidades.Embarcador.Fechamento.FechamentoJustificativaAcrescimoDesconto();

                PreencherDados(fechamentoJustificativaAcrescimoDesconto, unitOfWork);
                if (repFechamentoJustificativaAcrescimoDesconto.ExisteDuplicidade(fechamentoJustificativaAcrescimoDesconto))
                    throw new ControllerException("Já existe uma Justificativa cadastrada com os mesmos dados");

                unitOfWork.Start();

                repFechamentoJustificativaAcrescimoDesconto.Inserir(fechamentoJustificativaAcrescimoDesconto, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
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
                Repositorio.Embarcador.Fechamento.FechamentoJustificativaAcrescimoDesconto repFechamentoJustificativaAcrescimoDesconto = new Repositorio.Embarcador.Fechamento.FechamentoJustificativaAcrescimoDesconto(unitOfWork);
                Dominio.Entidades.Embarcador.Fechamento.FechamentoJustificativaAcrescimoDesconto fechamentoJustificativaAcrescimoDesconto = repFechamentoJustificativaAcrescimoDesconto.BuscarPorCodigo(codigo, false);

                PreencherDados(fechamentoJustificativaAcrescimoDesconto, unitOfWork);
                if (repFechamentoJustificativaAcrescimoDesconto.ExisteDuplicidade(fechamentoJustificativaAcrescimoDesconto))
                    throw new ControllerException("Já existe uma Justificativa cadastrada com os mesmos dados");

                unitOfWork.Start();

                repFechamentoJustificativaAcrescimoDesconto.Atualizar(fechamentoJustificativaAcrescimoDesconto, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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

                Repositorio.Embarcador.Fechamento.FechamentoJustificativaAcrescimoDesconto repFechamentoJustificativaAcrescimoDesconto = new Repositorio.Embarcador.Fechamento.FechamentoJustificativaAcrescimoDesconto(unitOfWork);

                Dominio.Entidades.Embarcador.Fechamento.FechamentoJustificativaAcrescimoDesconto fechamentoJustificativaAcrescimoDesconto = repFechamentoJustificativaAcrescimoDesconto.BuscarPorCodigo(codigo, false);

                if (fechamentoJustificativaAcrescimoDesconto == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    fechamentoJustificativaAcrescimoDesconto.Codigo,
                    fechamentoJustificativaAcrescimoDesconto.Situacao,
                    fechamentoJustificativaAcrescimoDesconto.TipoJustificativa,
                    fechamentoJustificativaAcrescimoDesconto.Descricao

                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao editar.");
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

                Repositorio.Embarcador.Fechamento.FechamentoJustificativaAcrescimoDesconto repFechamentoJustificativaAcrescimoDesconto = new Repositorio.Embarcador.Fechamento.FechamentoJustificativaAcrescimoDesconto(unitOfWork);

                Dominio.Entidades.Embarcador.Fechamento.FechamentoJustificativaAcrescimoDesconto fechamentoJustificativaAcrescimoDesconto = repFechamentoJustificativaAcrescimoDesconto.BuscarPorCodigo(codigo, false);

                if (fechamentoJustificativaAcrescimoDesconto == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repFechamentoJustificativaAcrescimoDesconto.Deletar(fechamentoJustificativaAcrescimoDesconto, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possivel excluir registro!");

                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");

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
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Dominio.ObjetosDeValor.Embarcador.Fechamento.FiltroPesquisaFechamentoJustificativaAcrescimoDesconto filtrosPesquisa = ObterFiltrosPesquisa();

            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tipo de Justificativa", "TipoJustificativaDescricao", 20, Models.Grid.Align.left, false);
            if (filtrosPesquisa.Situacao == SituacaoAtivoPesquisa.Todos)
                grid.AdicionarCabecalho("Situação", "Situacao", 20, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("TipoJustificativa", false);

            Repositorio.Embarcador.Fechamento.FechamentoJustificativaAcrescimoDesconto repFechamentoJustificativaAcrescimoDesconto = new Repositorio.Embarcador.Fechamento.FechamentoJustificativaAcrescimoDesconto(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

            int totalRegistros = repFechamentoJustificativaAcrescimoDesconto.ContarConsulta(filtrosPesquisa);

            List<Dominio.Entidades.Embarcador.Fechamento.FechamentoJustificativaAcrescimoDesconto> listaFechamentoJustificativaAcrescimoDesconto = totalRegistros <= 0 ? new List<Dominio.Entidades.Embarcador.Fechamento.FechamentoJustificativaAcrescimoDesconto>() : repFechamentoJustificativaAcrescimoDesconto.Consultar(parametrosConsulta, filtrosPesquisa);

            var lista = (from p in listaFechamentoJustificativaAcrescimoDesconto
                         select new
                         {
                             p.Codigo,
                             p.Descricao,
                             Situacao = p.Situacao ? "Ativo" : "Inativo",
                             TipoJustificativaDescricao = p.TipoJustificativa.ObterDescricao(),
                             p.TipoJustificativa
                         }).ToList();

            grid.AdicionaRows(lista);
            grid.setarQuantidadeTotal(totalRegistros);

            return grid;
        }

        private void PreencherDados(Dominio.Entidades.Embarcador.Fechamento.FechamentoJustificativaAcrescimoDesconto fechamentoJustificativaAcrescimoDesconto, Repositorio.UnitOfWork unitOfWork)
        {
            fechamentoJustificativaAcrescimoDesconto.Descricao = Request.GetStringParam("Descricao");
            fechamentoJustificativaAcrescimoDesconto.Situacao = Request.GetBoolParam("Situacao");
            fechamentoJustificativaAcrescimoDesconto.TipoJustificativa = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativaPesquisa>("TipoJustificativa");
        }

        private Dominio.ObjetosDeValor.Embarcador.Fechamento.FiltroPesquisaFechamentoJustificativaAcrescimoDesconto ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Fechamento.FiltroPesquisaFechamentoJustificativaAcrescimoDesconto()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Situacao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa>("Situacao"),
                TipoJustificativa = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativaPesquisa>("TipoJustificativa"),
            };
        }

        #endregion
    }
}
