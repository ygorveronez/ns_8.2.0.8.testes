using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NotaFiscal
{
    [CustomAuthorize("NotasFiscais/ModeloDocumentoFiscal")]
    public class ModeloNotaFiscalController : BaseController
    {
		#region Construtores

		public ModeloNotaFiscalController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);

                string descricao = Request.Params("Descricao");
                string numero = Request.Params("Numero");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 70, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Número", "Numero", 15, Models.Grid.Align.left, true);

                List<Dominio.Entidades.ModeloDocumentoFiscal> listaModeloDocumentoFiscal = repModeloDocumentoFiscal.Consultar(descricao, numero, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repModeloDocumentoFiscal.ContarConsulta(descricao, numero));
                var lista = (from p in listaModeloDocumentoFiscal
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

                Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);

                Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = new Dominio.Entidades.ModeloDocumentoFiscal();

                string descricao = Request.Params("Descricao");
                string numero = Request.Params("Numero");
                string status = Request.Params("Status");

                modeloDocumentoFiscal.Codigo = repModeloDocumentoFiscal.BuscarProximoCodigo();
                modeloDocumentoFiscal.Descricao = descricao;
                modeloDocumentoFiscal.Numero = numero;
                modeloDocumentoFiscal.Status = status;
                modeloDocumentoFiscal.Data = DateTime.Now;

                Dominio.Entidades.ModeloDocumentoFiscal modeloExistente = repModeloDocumentoFiscal.BuscarPorModelo(modeloDocumentoFiscal.Numero);

                if (modeloExistente == null)
                {
                    repModeloDocumentoFiscal.Inserir(modeloDocumentoFiscal, Auditado);
                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, true, "Já existe um Modelo de Documento cadastrado com o Número " + modeloExistente.Numero);
                }
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

                Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);

                Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorId(int.Parse(Request.Params("Codigo")));
                modeloDocumentoFiscal.Initialize();

                string descricao = Request.Params("Descricao");
                string numero = Request.Params("Numero");
                string status = Request.Params("Status");

                modeloDocumentoFiscal.Descricao = descricao;
                modeloDocumentoFiscal.Numero = numero;
                modeloDocumentoFiscal.Status = status;
                modeloDocumentoFiscal.Data = DateTime.Now;

                Dominio.Entidades.ModeloDocumentoFiscal modeloExistente = repModeloDocumentoFiscal.BuscarPorModelo(modeloDocumentoFiscal.Numero);

                if (modeloExistente == null || modeloExistente.Codigo == modeloDocumentoFiscal.Codigo)
                {
                    repModeloDocumentoFiscal.Atualizar(modeloDocumentoFiscal, Auditado);
                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, true, "Já existe um Modelo de Documento cadastrado com o Número " + modeloExistente.Numero);
                }
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
                Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
                Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorId(codigo);
                var dynProcessoMovimento = new
                {
                    modeloDocumentoFiscal.Codigo,
                    modeloDocumentoFiscal.Descricao,
                    modeloDocumentoFiscal.Numero,
                    modeloDocumentoFiscal.Status
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
                Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
                Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorId(codigo);
                repModeloDocumentoFiscal.Deletar(modeloDocumentoFiscal, Auditado);
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
