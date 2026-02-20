using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NotaFiscal
{
    [CustomAuthorize("NotasFiscais/RetornoSefaz")]
    public class RetornoSefazController : BaseController
    {
		#region Construtores

		public RetornoSefazController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.NotaFiscal.RetornoSefaz repRetornoSefaz = new Repositorio.Embarcador.NotaFiscal.RetornoSefaz(unitOfWork);

                string mensagemRetornoSefaz = Request.Params("MensagemRetornoSefaz");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status = Request.GetEnumParam("Status", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Código", "Codigo", 8, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Retorno Sefaz", "MensagemRetornoSefaz", 55, Models.Grid.Align.left, true);

                List<Dominio.Entidades.Embarcador.NotaFiscal.RetornoSefaz> listaRetornoSefaz = repRetornoSefaz.Consultar(0, mensagemRetornoSefaz, status, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repRetornoSefaz.ContarConsulta(0, mensagemRetornoSefaz, status));
                var lista = (from p in listaRetornoSefaz
                             select new
                             {
                                 p.Codigo,
                                 MensagemRetornoSefaz = p.MensagemRetornoSefaz
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

                Repositorio.Embarcador.NotaFiscal.RetornoSefaz repRetornoSefaz = new Repositorio.Embarcador.NotaFiscal.RetornoSefaz(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.RetornoSefaz retornoSefaz = new Dominio.Entidades.Embarcador.NotaFiscal.RetornoSefaz();

                PreencherRetornoSefaz(retornoSefaz, unitOfWork);

                repRetornoSefaz.Inserir(retornoSefaz, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
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

                Repositorio.Embarcador.NotaFiscal.RetornoSefaz repRetornoSefaz = new Repositorio.Embarcador.NotaFiscal.RetornoSefaz(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.RetornoSefaz retornoSefaz = repRetornoSefaz.BuscarPorCodigo(codigo, true);

                if (retornoSefaz == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherRetornoSefaz(retornoSefaz, unitOfWork);

                repRetornoSefaz.Atualizar(retornoSefaz, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
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

                Repositorio.Embarcador.NotaFiscal.RetornoSefaz repRetornoSefaz = new Repositorio.Embarcador.NotaFiscal.RetornoSefaz(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.RetornoSefaz retornoSefaz = repRetornoSefaz.BuscarPorCodigo(codigo);

                if (retornoSefaz == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var dynRetornoSefaz = new
                {
                    retornoSefaz.Codigo,
                    retornoSefaz.MensagemRetornoSefaz,
                    retornoSefaz.AbreviacaoRetornoSefaz,
                    retornoSefaz.Status
                };

                return new JsonpResult(dynRetornoSefaz);
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

                Repositorio.Embarcador.NotaFiscal.RetornoSefaz repRetornoSefaz = new Repositorio.Embarcador.NotaFiscal.RetornoSefaz(unitOfWork);                

                Dominio.Entidades.Embarcador.NotaFiscal.RetornoSefaz retornoSefaz = repRetornoSefaz.BuscarPorCodigo(codigo, true);
                if (retornoSefaz == null)
                    return new JsonpResult(false, true, "Cadastro não encontrado.");

                repRetornoSefaz.Deletar(retornoSefaz, Auditado);

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

        #region Metodos Privados

        private void PreencherRetornoSefaz(Dominio.Entidades.Embarcador.NotaFiscal.RetornoSefaz retornoSefaz, Repositorio.UnitOfWork unitOfWork)
        {            
            string mensagemRetornoSefaz = Request.Params("MensagemRetornoSefaz");
            string abreviacaoRetornoSefaz = Request.Params("AbreviacaoRetornoSefaz");
            bool.TryParse(Request.Params("Status"), out bool status);

            retornoSefaz.MensagemRetornoSefaz = mensagemRetornoSefaz;
            retornoSefaz.AbreviacaoRetornoSefaz = abreviacaoRetornoSefaz;
            retornoSefaz.Status = status;
        }

        #endregion
    }
}
