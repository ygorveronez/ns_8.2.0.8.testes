using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Pessoas/ColaboradorSituacao")]
    public class ColaboradorSituacaoController : BaseController
    {
		#region Construtores

		public ColaboradorSituacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string descricao = Request.Params("Descricao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status = Request.GetEnumParam("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 60, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação Colaborador", "SituacaoColaborador", 20, Models.Grid.Align.center, true);

                Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacao repColaboradorSituacao = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacao> colaboradorSituacao = repColaboradorSituacao.Consultar(codigoEmpresa, descricao, status, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repColaboradorSituacao.ContarConsulta(codigoEmpresa, descricao, status));

                var lista = (from p in colaboradorSituacao
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 SituacaoColaborador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaboradorHelper.ObterDescricao(p.SituacaoColaborador)
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
                unitOfWork.Start();

                Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacao repColaboradorSituacao = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacao(unitOfWork);
                Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacao colaboradorSituacao = new Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacao();

                PreencherColaboradorSituacao(colaboradorSituacao, unitOfWork);
                repColaboradorSituacao.Inserir(colaboradorSituacao, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacao repColaboradorSituacao = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacao(unitOfWork);
                Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacao colaboradorSituacao = repColaboradorSituacao.BuscarPorCodigo(codigo, true);

                PreencherColaboradorSituacao(colaboradorSituacao, unitOfWork);
                repColaboradorSituacao.Atualizar(colaboradorSituacao, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacao repColaboradorSituacao = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacao(unitOfWork);
                Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacao colaboradorSituacao = repColaboradorSituacao.BuscarPorCodigo(codigo, false);

                var dynColaboradorSituacao = new
                {
                    colaboradorSituacao.Codigo,
                    colaboradorSituacao.Descricao,
                    colaboradorSituacao.Situacao,
                    colaboradorSituacao.SituacaoColaborador,
                    colaboradorSituacao.Observacao,
                    colaboradorSituacao.Cores,
                    colaboradorSituacao.CodigoContabil,
                    colaboradorSituacao.CodigoIntegracao
                };

                return new JsonpResult(dynColaboradorSituacao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacao repColaboradorSituacao = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacao(unitOfWork);
                Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacao colaboradorSituacao = repColaboradorSituacao.BuscarPorCodigo(codigo, true);

                if (colaboradorSituacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repColaboradorSituacao.Deletar(colaboradorSituacao, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherColaboradorSituacao(Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacao colaboradorSituacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacao repColaboradorSituacao = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacao(unitOfWork);

            string descricao = Request.Params("Descricao");
            string observacao = Request.Params("Observacao");

            bool.TryParse(Request.Params("Situacao"), out bool situacao);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador situacaoColaborador;
            Enum.TryParse(Request.Params("SituacaoColaborador"), out situacaoColaborador);

            int.TryParse(Request.Params("CodigoContabil"), out int codigoContabil);
            string codigoIntegracao = Request.Params("CodigoIntegracao");

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            colaboradorSituacao.Descricao = descricao;
            colaboradorSituacao.Situacao = situacao;
            colaboradorSituacao.Observacao = observacao;
            colaboradorSituacao.SituacaoColaborador = situacaoColaborador;
            colaboradorSituacao.Empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null;
            colaboradorSituacao.Cores = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.Cores>("Cores");
            colaboradorSituacao.CodigoContabil = codigoContabil;
            colaboradorSituacao.CodigoIntegracao = codigoIntegracao;
        }

        #endregion
    }
}
