using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Contabil
{
    [CustomAuthorize("Contabils/CodigoIntegracaoCFOPCST")]
    public class CodigoIntegracaoCFOPCSTController : BaseController
    {
		#region Construtores

		public CodigoIntegracaoCFOPCSTController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {                
                string codigoIntegracao = Request.GetStringParam("CodigoServico");
                string cst = Request.GetStringParam("CST");
                string cfop = Request.GetStringParam("CFOP");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);                
                grid.AdicionarCabecalho("Código Integração", "CodigoIntegracao", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("CST", "CST", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("CFOP", "CFOP", 20, Models.Grid.Align.left, true);

                Repositorio.Embarcador.Contabeis.CodigoIntegracaoCFOPCST repCodigoIntegracaoCFOPCST = new Repositorio.Embarcador.Contabeis.CodigoIntegracaoCFOPCST(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Contabeis.CodigoIntegracaoCFOPCST> codigoIntegracaoCFOPCSTs = repCodigoIntegracaoCFOPCST.Consultar(codigoIntegracao, cst, cfop, parametrosConsulta);
                grid.setarQuantidadeTotal(repCodigoIntegracaoCFOPCST.ContarConsulta(codigoIntegracao, cst, cfop));

                var lista = (from p in codigoIntegracaoCFOPCSTs
                             select new
                             {
                                 p.Codigo,
                                 p.CodigoIntegracao,
                                 p.CST,
                                 p.CFOP
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

                Repositorio.Embarcador.Contabeis.CodigoIntegracaoCFOPCST repCodigoIntegracaoCFOPCST = new Repositorio.Embarcador.Contabeis.CodigoIntegracaoCFOPCST(unitOfWork);
                Dominio.Entidades.Embarcador.Contabeis.CodigoIntegracaoCFOPCST codigoIntegracaoCFOPCST = new Dominio.Entidades.Embarcador.Contabeis.CodigoIntegracaoCFOPCST();

                PreencherCodigoIntegracaoCFOPCST(codigoIntegracaoCFOPCST, unitOfWork);

                repCodigoIntegracaoCFOPCST.Inserir(codigoIntegracaoCFOPCST, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
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

                Repositorio.Embarcador.Contabeis.CodigoIntegracaoCFOPCST repCodigoIntegracaoCFOPCST = new Repositorio.Embarcador.Contabeis.CodigoIntegracaoCFOPCST(unitOfWork);
                Dominio.Entidades.Embarcador.Contabeis.CodigoIntegracaoCFOPCST codigoIntegracaoCFOPCST = repCodigoIntegracaoCFOPCST.BuscarPorCodigo(codigo, true);

                if (codigoIntegracaoCFOPCST == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherCodigoIntegracaoCFOPCST(codigoIntegracaoCFOPCST, unitOfWork);

                repCodigoIntegracaoCFOPCST.Atualizar(codigoIntegracaoCFOPCST, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
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

                Repositorio.Embarcador.Contabeis.CodigoIntegracaoCFOPCST repCodigoIntegracaoCFOPCST = new Repositorio.Embarcador.Contabeis.CodigoIntegracaoCFOPCST(unitOfWork);
                Dominio.Entidades.Embarcador.Contabeis.CodigoIntegracaoCFOPCST codigoIntegracaoCFOPCST = repCodigoIntegracaoCFOPCST.BuscarPorCodigo(codigo, false);

                if (codigoIntegracaoCFOPCST == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var dynCodigoIntegracaoCFOPCST = new
                {
                    codigoIntegracaoCFOPCST.Codigo,
                    codigoIntegracaoCFOPCST.CodigoIntegracao,
                    codigoIntegracaoCFOPCST.CST,
                    codigoIntegracaoCFOPCST.CFOP
                };

                return new JsonpResult(dynCodigoIntegracaoCFOPCST);
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Contabeis.CodigoIntegracaoCFOPCST repCodigoIntegracaoCFOPCST = new Repositorio.Embarcador.Contabeis.CodigoIntegracaoCFOPCST(unitOfWork);
                Dominio.Entidades.Embarcador.Contabeis.CodigoIntegracaoCFOPCST codigoIntegracaoCFOPCST = repCodigoIntegracaoCFOPCST.BuscarPorCodigo(codigo, true);

                if (codigoIntegracaoCFOPCST == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repCodigoIntegracaoCFOPCST.Deletar(codigoIntegracaoCFOPCST, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherCodigoIntegracaoCFOPCST(Dominio.Entidades.Embarcador.Contabeis.CodigoIntegracaoCFOPCST codigoIntegracaoCFOPCST, Repositorio.UnitOfWork unitOfWork)
        {
            codigoIntegracaoCFOPCST.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            codigoIntegracaoCFOPCST.CST = Request.GetStringParam("CST");
            codigoIntegracaoCFOPCST.CFOP = Request.GetStringParam("CFOP");
        }

        #endregion
    }
}
