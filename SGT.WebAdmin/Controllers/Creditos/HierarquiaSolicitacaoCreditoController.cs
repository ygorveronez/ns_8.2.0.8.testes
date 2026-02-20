using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Creditoss
{
    [CustomAuthorize("Creditos/HierarquiaSolicitacaoCredito")]
    public class HierarquiaSolicitacaoCreditoController : BaseController
    {
		#region Construtores

		public HierarquiaSolicitacaoCreditoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Solicitante", "Solicitante", 83, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("CPF", "CPF", 10, Models.Grid.Align.center, false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdena == "Solicitante")
                    propOrdena += ".Nome";

                Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito repHierarquiaSolicitacaoCredito = new Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito(unitOfWork);
                List<Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito> listaHierarquiaSolicitacaoCredito = repHierarquiaSolicitacaoCredito.ConsultarPorCreditor(this.Usuario.Codigo, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repHierarquiaSolicitacaoCredito.ContarPorCreditor(this.Usuario.Codigo));
                var lista = (from p in listaHierarquiaSolicitacaoCredito
                            select new
                            {
                                p.Codigo,
                                Solicitante = p.Solicitante.Nome,
                                CPF = p.Solicitante.CPF_Formatado
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
                Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito repHierarquiaSolicitacaoCredito = new Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito(unitOfWork);

                Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito hierarquiaSolicitacaoCredito = new Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito();
                hierarquiaSolicitacaoCredito.Solicitante = new Dominio.Entidades.Usuario() { Codigo = int.Parse(Request.Params("Solicitante")) };
                hierarquiaSolicitacaoCredito.Creditor = this.Usuario;

                if (repHierarquiaSolicitacaoCredito.BuscarPorCreditorRecebedor(hierarquiaSolicitacaoCredito.Creditor.Codigo, hierarquiaSolicitacaoCredito.Solicitante.Codigo) == null)
                {
                    repHierarquiaSolicitacaoCredito.Inserir(hierarquiaSolicitacaoCredito, Auditado);
                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Este solicitante já foi adicionado");
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
                Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito repHierarquiaSolicitacaoCredito = new Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito(unitOfWork);

                Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito hierarquiaSolicitacaoCredito = repHierarquiaSolicitacaoCredito.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);
                hierarquiaSolicitacaoCredito.Solicitante = new Dominio.Entidades.Usuario() { Codigo = int.Parse(Request.Params("Solicitante")) };
                hierarquiaSolicitacaoCredito.Creditor = this.Usuario;

                Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito hierarquiaSolicitacaoCreditoExiste = repHierarquiaSolicitacaoCredito.BuscarPorCreditorRecebedor(hierarquiaSolicitacaoCredito.Creditor.Codigo, hierarquiaSolicitacaoCredito.Solicitante.Codigo);


                if (hierarquiaSolicitacaoCreditoExiste == null || (hierarquiaSolicitacaoCreditoExiste.Codigo == hierarquiaSolicitacaoCredito.Codigo))
                {
                    repHierarquiaSolicitacaoCredito.Atualizar(hierarquiaSolicitacaoCredito, Auditado);
                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Este solicitante já foi adicionado");
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
                Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito repHierarquiaSolicitacaoCredito = new Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito(unitOfWork);
                Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito hierarquiaSolicitacaoCredito = repHierarquiaSolicitacaoCredito.BuscarPorCodigo(codigo);
                var dynHierarquiaSolicitacaoCredito = new
                {
                    hierarquiaSolicitacaoCredito.Codigo,
                    Creditor = new { Codigo = hierarquiaSolicitacaoCredito.Creditor.Codigo, Descricao = hierarquiaSolicitacaoCredito.Creditor.Nome },
                    Solicitante= new { Codigo = hierarquiaSolicitacaoCredito.Solicitante.Codigo, Descricao = hierarquiaSolicitacaoCredito.Solicitante.Nome }
                };
                return new JsonpResult(dynHierarquiaSolicitacaoCredito);
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
                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito repHierarquiaSolicitacaoCredito = new Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito(unitOfWork);
                Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito hierarquiaSolicitacaoCredito = repHierarquiaSolicitacaoCredito.BuscarPorCodigo(codigo);
                repHierarquiaSolicitacaoCredito.Deletar(hierarquiaSolicitacaoCredito, Auditado);
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


        [AllowAuthenticate]
        public async Task<IActionResult> BuscarOperadoresAbaixoHierarquia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                string nome = Request.Params("Nome");
            
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Nome", "Nome", 64, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("CPF", "CPF_Formatado", 14, Models.Grid.Align.left, false);
   

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                propOrdena = "Solicitante." + propOrdena;

                Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito repHierarquiaSolicitacaoCredito = new Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito(unitOfWork);
                List<Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito> listaHierarquiaSolicitacaoCredito = repHierarquiaSolicitacaoCredito.ConsultarPorCreditor(this.Usuario.Codigo, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repHierarquiaSolicitacaoCredito.ContarPorCreditor(this.Usuario.Codigo));
                var lista = (from p in listaHierarquiaSolicitacaoCredito
                            select new
                            {
                                p.Solicitante.Codigo,
                                p.Solicitante.Nome,
                                p.Solicitante.CPF_Formatado
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

        #endregion
    }
}
