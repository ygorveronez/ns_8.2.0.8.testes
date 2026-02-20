using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pallets
{
    [CustomAuthorize("Pallets/SituacaoDevolucao")]
    public class SituacaoDevolucaoController : BaseController
    {
		#region Construtores

		public SituacaoDevolucaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            var unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao;
                if (!Enum.TryParse(Request.Params("Ativo"), out situacao))
                    situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor Unitário", "ValorUnitario", 20, Models.Grid.Align.right, true);

                if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Pallets.SituacaoDevolucaoPallet repSituacaoDevolucao = new Repositorio.Embarcador.Pallets.SituacaoDevolucaoPallet(unidadeTrabalho);

                List<Dominio.Entidades.Embarcador.Pallets.SituacaoDevolucaoPallet> listaSituacaoDevolucao = repSituacaoDevolucao.Consultar(descricao, situacao, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repSituacaoDevolucao.ContarConsulta(descricao, situacao));

                grid.AdicionaRows((from p in listaSituacaoDevolucao
                                   select new
                                   {
                                       p.Codigo,
                                       p.Descricao,
                                       ValorUnitario = p.ValorUnitario.ToString("n2"),
                                       p.DescricaoAtivo
                                   }).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            var unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var repositorio = new Repositorio.Embarcador.Pallets.SituacaoDevolucaoPallet(unidadeTrabalho);
                var situacaoDevolucao = new Dominio.Entidades.Embarcador.Pallets.SituacaoDevolucaoPallet();

                PreencherSituacaoDevolucao(situacaoDevolucao);

                repositorio.Inserir(situacaoDevolucao, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            var unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Pallets.SituacaoDevolucaoPallet(unidadeTrabalho);
                var situacaoDevolucao = repositorio.BuscarPorCodigo(codigo, true);

                if (situacaoDevolucao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherSituacaoDevolucao(situacaoDevolucao);

                repositorio.Atualizar(situacaoDevolucao, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            var unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Pallets.SituacaoDevolucaoPallet(unidadeTrabalho);
                var situacaoDevolucao = repositorio.BuscarPorCodigo(codigo);

                if (situacaoDevolucao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var retorno = new
                {
                    situacaoDevolucao.Ativo,
                    situacaoDevolucao.Codigo,
                    situacaoDevolucao.Descricao,
                    ValorUnitario = situacaoDevolucao.ValorUnitario.ToString("n2"),
                    situacaoDevolucao.AcresceSaldo,
                    situacaoDevolucao.SituacaoPalletAvariado,
                    situacaoDevolucao.SituacaoPalletDescartado
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            var unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Pallets.SituacaoDevolucaoPallet(unidadeTrabalho);
                var situacaoDevolucao = repositorio.BuscarPorCodigo(codigo);

                if (situacaoDevolucao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repositorio.Deletar(situacaoDevolucao, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherSituacaoDevolucao(Dominio.Entidades.Embarcador.Pallets.SituacaoDevolucaoPallet situacaoDevolucao)
        {
            situacaoDevolucao.Ativo = Request.GetBoolParam("Ativo");
            situacaoDevolucao.Descricao = Request.GetStringParam("Descricao");
            situacaoDevolucao.ValorUnitario = Request.GetDecimalParam("ValorUnitario");
            situacaoDevolucao.AcresceSaldo = Request.GetBoolParam("AcresceSaldo");
            situacaoDevolucao.SituacaoPalletAvariado = Request.GetBoolParam("SituacaoPalletAvariado");
            situacaoDevolucao.SituacaoPalletDescartado = Request.GetBoolParam("SituacaoPalletDescartado");
        }

        #endregion
    }
}
