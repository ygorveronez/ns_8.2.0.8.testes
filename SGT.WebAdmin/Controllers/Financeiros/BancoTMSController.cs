using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/BancoTMS")]
    public class BancoTMSController : BaseController
    {
		#region Construtores

		public BancoTMSController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);                

                string descricao = Request.Params("Descricao");
                int numero = 0;
                int.TryParse(Request.Params("Numero"), out numero);


                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 70, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Número", "Numero", 20, Models.Grid.Align.center, true);

                List<Dominio.Entidades.Banco> listaBanco = repBanco.Consultar(numero, descricao, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repBanco.ContarConsulta(numero, descricao));
                var lista = (from p in listaBanco
                            select new
                            {
                                p.Codigo,
                                p.Descricao,
                                p.Numero
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

                Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);

                Dominio.Entidades.Banco banco = new Dominio.Entidades.Banco();
                
                string descricao = Request.Params("Descricao");
                int numero = 0;
                int.TryParse(Request.Params("Numero"), out numero);

                if (repBanco.ContemBancoCadastrado(numero, 0))
                    return new JsonpResult(false, true, "Já existe um banco cadastrado com o mesmo número.");

                banco.Descricao = descricao;
                banco.Numero = numero;
                banco.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");

                repBanco.Inserir(banco, Auditado);
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

                Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);

                Dominio.Entidades.Banco banco = repBanco.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);

                string descricao = Request.Params("Descricao");
                int numero = 0;
                int.TryParse(Request.Params("Numero"), out numero);

                banco.Descricao = descricao;
                banco.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
                banco.Numero = numero;

                if (repBanco.ContemBancoCadastrado(numero, banco.Codigo))
                    return new JsonpResult(false, true, "Já existe um banco cadastrado com o mesmo número.");

                repBanco.Atualizar(banco, Auditado);

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
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);
                Dominio.Entidades.Banco banco = repBanco.BuscarPorCodigo(codigo);
                var dynProcessoMovimento = new
                {
                    banco.Codigo,
                    banco.Descricao,
                    banco.CodigoIntegracao,
                    banco.Numero
                };
                return new JsonpResult(dynProcessoMovimento);
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
                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);
                Dominio.Entidades.Banco banco = repBanco.BuscarPorCodigo(codigo);
                repBanco.Deletar(banco, Auditado);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
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
    }
}
