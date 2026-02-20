using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frota
{
    [CustomAuthorize("Frota/TipoMovimentoMotorista")]
    public class TipoMovimentoMotoristaController : BaseController
    {
		#region Construtores

		public TipoMovimentoMotoristaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string descricao = Request.Params("Descricao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos;
                Enum.TryParse(Request.Params("Situacao"), out ativo);
                int codigoPlanoConta = 0;
                int.TryParse(Request.Params("PlanoConta"), out codigoPlanoConta);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("TipoMovimentoEntidade", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 45, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Plano", "Plano", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo Entidade", "DescricaoTipoMovimentoEntidade", 10, Models.Grid.Align.left, false);
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                string ordenacao = grid.header[grid.indiceColunaOrdena].data;
                if (ordenacao == "Plano")
                    ordenacao = "PlanoConta.Descricao";

                Repositorio.Embarcador.Financeiro.TipoMovimentoMotorista repTipoMovimentoMotorista = new Repositorio.Embarcador.Financeiro.TipoMovimentoMotorista(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoMotorista> listaTipoMovimentoMotorista = repTipoMovimentoMotorista.Consultar(codigoPlanoConta, descricao, ativo, ordenacao, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTipoMovimentoMotorista.ContarConsulta(codigoPlanoConta, descricao, ativo));
                var lista = from p in listaTipoMovimentoMotorista
                            select new
                            {
                                p.Codigo,
                                p.TipoMovimentoEntidade,
                                p.Descricao,
                                Plano = p.PlanoConta != null ? "(" + p.PlanoConta.Plano + ") " + p.PlanoConta.Descricao : string.Empty,
                                p.DescricaoTipoMovimentoEntidade,
                                p.DescricaoAtivo
                            };
                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                unitOfWork.Dispose();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Financeiro.TipoMovimentoMotorista repTipoMovimentoMotorista = new Repositorio.Embarcador.Financeiro.TipoMovimentoMotorista(unitOfWork);
                Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoMotorista tipoMovimentoMotorista = new Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoMotorista();
                int codigoPlanoConta = 0;
                int.TryParse(Request.Params("PlanoConta"), out codigoPlanoConta);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade tipoEntidade;
                Enum.TryParse(Request.Params("TipoMovimentoEntidade"), out tipoEntidade);

                tipoMovimentoMotorista.PlanoConta = repPlanoConta.BuscarPorCodigo(codigoPlanoConta);
                tipoMovimentoMotorista.Ativo = bool.Parse(Request.Params("Situacao"));
                tipoMovimentoMotorista.Descricao = Request.Params("Descricao");
                tipoMovimentoMotorista.TipoMovimentoEntidade = tipoEntidade;

                repTipoMovimentoMotorista.Inserir(tipoMovimentoMotorista, Auditado);
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
                Repositorio.Embarcador.Financeiro.PlanoConta repPlanoDeConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimentoMotorista repTipoMovimentoMotorista = new Repositorio.Embarcador.Financeiro.TipoMovimentoMotorista(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoMotorista tipoMovimentoMotorista = repTipoMovimentoMotorista.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);
                int codigoPlanoConta = 0;
                int.TryParse(Request.Params("PlanoConta"), out codigoPlanoConta);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade tipoEntidade;
                Enum.TryParse(Request.Params("TipoMovimentoEntidade"), out tipoEntidade);

                tipoMovimentoMotorista.PlanoConta = repPlanoDeConta.BuscarPorCodigo(codigoPlanoConta);
                tipoMovimentoMotorista.Ativo = bool.Parse(Request.Params("Situacao"));
                tipoMovimentoMotorista.Descricao = Request.Params("Descricao");
                tipoMovimentoMotorista.TipoMovimentoEntidade = tipoEntidade;

                repTipoMovimentoMotorista.Atualizar(tipoMovimentoMotorista, Auditado);
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
                Repositorio.Embarcador.Financeiro.TipoMovimentoMotorista repTipoMovimentoMotorista = new Repositorio.Embarcador.Financeiro.TipoMovimentoMotorista(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoMotorista tipoMovimentoMotorista = repTipoMovimentoMotorista.BuscarPorCodigo(codigo);
                var dynPlanoConta = new
                {
                    tipoMovimentoMotorista.Codigo,
                    tipoMovimentoMotorista.Descricao,
                    PlanoConta = new { Codigo = tipoMovimentoMotorista.PlanoConta != null ? tipoMovimentoMotorista.PlanoConta.Codigo : 0, Descricao = tipoMovimentoMotorista.PlanoConta != null ? tipoMovimentoMotorista.PlanoConta.Descricao : "" },
                    Situacao = tipoMovimentoMotorista.Ativo,
                    tipoMovimentoMotorista.TipoMovimentoEntidade
                };
                return new JsonpResult(dynPlanoConta);
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
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Financeiro.TipoMovimentoMotorista repTipoMovimentoMotorista = new Repositorio.Embarcador.Financeiro.TipoMovimentoMotorista(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoMotorista tipoMovimentoMotorista = repTipoMovimentoMotorista.BuscarPorCodigo(codigo);
                repTipoMovimentoMotorista.Deletar(tipoMovimentoMotorista, Auditado);
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
