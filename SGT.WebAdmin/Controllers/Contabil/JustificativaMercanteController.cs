using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Contabil
{
    [CustomAuthorize("Contabils/JustificativaMercante")]
    public class JustificativaMercanteController : BaseController
    {
		#region Construtores

		public JustificativaMercanteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Contabeis.JustificativaMercante repJustificativaMercante = new Repositorio.Embarcador.Contabeis.JustificativaMercante(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Contabil.FiltroPesquisaJustificativaMercante filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 60, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 20, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                List<Dominio.Entidades.Embarcador.Contabeis.JustificativaMercante> gruposTransportadores = repJustificativaMercante.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repJustificativaMercante.ContarConsulta(filtrosPesquisa));

                var lista = (from obj in gruposTransportadores
                             select new
                             {
                                 obj.Codigo,
                                 obj.Descricao,
                                 obj.DescricaoAtivo
                             }).ToList();

                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
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
                Repositorio.Embarcador.Contabeis.JustificativaMercante repJustificativaMercante = new Repositorio.Embarcador.Contabeis.JustificativaMercante(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Contabeis.JustificativaMercante justificativaMercante = repJustificativaMercante.BuscarPorCodigo(codigo);

                if (justificativaMercante == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                var retorno = new
                {
                    justificativaMercante.Codigo,
                    justificativaMercante.Descricao,
                    justificativaMercante.Ativo,
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
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
                unitOfWork.Start();

                Repositorio.Embarcador.Contabeis.JustificativaMercante repJustificativaMercante = new Repositorio.Embarcador.Contabeis.JustificativaMercante(unitOfWork);

                Dominio.Entidades.Embarcador.Contabeis.JustificativaMercante justificativaMercante = new Dominio.Entidades.Embarcador.Contabeis.JustificativaMercante();

                PreencherJustificativaMercante(justificativaMercante, unitOfWork);

                repJustificativaMercante.Inserir(justificativaMercante, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
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
                unitOfWork.Start();

                Repositorio.Embarcador.Contabeis.JustificativaMercante repJustificativaMercante = new Repositorio.Embarcador.Contabeis.JustificativaMercante(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Contabeis.JustificativaMercante justificativaMercante = repJustificativaMercante.BuscarPorCodigo(codigo);

                if (justificativaMercante == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                PreencherJustificativaMercante(justificativaMercante, unitOfWork);

                repJustificativaMercante.Atualizar(justificativaMercante, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
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

                Repositorio.Embarcador.Contabeis.JustificativaMercante repJustificativaMercante = new Repositorio.Embarcador.Contabeis.JustificativaMercante(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Contabeis.JustificativaMercante justificativaMercante = repJustificativaMercante.BuscarPorCodigo(codigo);

                if (justificativaMercante == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                repJustificativaMercante.Deletar(justificativaMercante, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelExcluirRegistro);
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Contabil.FiltroPesquisaJustificativaMercante ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Contabil.FiltroPesquisaJustificativaMercante()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Situacao = Request.GetEnumParam<SituacaoAtivoPesquisa>("Situacao")
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricatoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }

        private void PreencherJustificativaMercante(Dominio.Entidades.Embarcador.Contabeis.JustificativaMercante justificativaMercante, Repositorio.UnitOfWork unitOfWork)
        {
            justificativaMercante.Descricao = Request.GetStringParam("Descricao");
            justificativaMercante.Ativo = Request.GetBoolParam("Ativo");
        }

        #endregion
    }
}
