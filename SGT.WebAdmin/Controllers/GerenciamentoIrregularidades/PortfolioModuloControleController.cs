using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GerenciamentoIrregularidades
{
    [CustomAuthorize("GerenciamentoIrregularidades/PortfolioModuloControle")]
    public class PortfolioModuloControleController : BaseController
    {
		#region Construtores

		public PortfolioModuloControleController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true); 
                grid.AdicionarCabecalho("Código de Integração", "CodigoIntegracao", 20, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaPortfolioModuloControle filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle repositorioPortfolioModuloControle = new Repositorio.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle(unitOfWork);
                int totalRegistro = repositorioPortfolioModuloControle.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle> PortfoliosModuloControle = (totalRegistro > 0) ? repositorioPortfolioModuloControle.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle>();

                var PortfoliosModuloControleRetornar = (
                    from PortfolioModuloControle in PortfoliosModuloControle
                    select new
                    {
                        PortfolioModuloControle.Codigo,
                        PortfolioModuloControle.Descricao,
                        PortfolioModuloControle.CodigoIntegracao
                    }
                ).ToList();

                grid.AdicionaRows(PortfoliosModuloControleRetornar);
                grid.setarQuantidadeTotal(totalRegistro);

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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle repositorioPortfolioModuloControle = new Repositorio.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle(unitOfWork);
                Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle PortfolioModuloControle = repositorioPortfolioModuloControle.BuscarPorCodigo(codigo);

                if (PortfolioModuloControle == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                var retorno = new
                {
                    PortfolioModuloControle.Codigo,
                    PortfolioModuloControle.Descricao,
                    PortfolioModuloControle.CodigoIntegracao
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscar);
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

                Repositorio.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle repositorioPortfolioModuloControle = new Repositorio.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle(unitOfWork);
                Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle PortfolioModuloControle = new Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle();

                PreencherPortfolioModuloControle(PortfolioModuloControle, unitOfWork);

                if (repositorioPortfolioModuloControle.ExisteDuplicidade(PortfolioModuloControle))
                    throw new ControllerException("já existe um Portfolio cadastrado com os mesmos dados");

                unitOfWork.Start();

                repositorioPortfolioModuloControle.Inserir(PortfolioModuloControle, Auditado);

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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
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
                Repositorio.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle repositorioPortfolioModuloControle = new Repositorio.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle(unitOfWork);
                Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle PortfolioModuloControle = repositorioPortfolioModuloControle.BuscarPorCodigo(codigo);

                if (PortfolioModuloControle == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                PreencherPortfolioModuloControle(PortfolioModuloControle, unitOfWork);

                if (repositorioPortfolioModuloControle.ExisteDuplicidade(PortfolioModuloControle))
                    throw new ControllerException(Localization.Resources.Gerais.Geral.RegistroDuplicado);

                unitOfWork.Start();

                repositorioPortfolioModuloControle.Atualizar(PortfolioModuloControle, Auditado);

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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
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
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle repositorioPortfolioModuloControle = new Repositorio.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle(unitOfWork);
                Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle PortfolioModuloControle = repositorioPortfolioModuloControle.BuscarPorCodigo(codigo);

                if (PortfolioModuloControle == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                repositorioPortfolioModuloControle.Deletar(PortfolioModuloControle, Auditado);

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

                if (ExcessaoPorPossuirDependeciasNoBanco(excecao))
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelExcluirRegistro);

                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private string ObterPropriedadeOrdenar(string prop)
        {
            return prop;
        }

        private Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaPortfolioModuloControle ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaPortfolioModuloControle()
            {
                Descricao = Request.GetStringParam("Descricao"),
                CodigoIntegracao = Request.GetStringParam("CodigoIntegracao")
            };
        }

        private void PreencherPortfolioModuloControle(Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle PortfolioModuloControle, Repositorio.UnitOfWork unitOfWork)
        {
            PortfolioModuloControle.Descricao = Request.GetStringParam("Descricao");
            PortfolioModuloControle.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
        }

        #endregion
    }
}
