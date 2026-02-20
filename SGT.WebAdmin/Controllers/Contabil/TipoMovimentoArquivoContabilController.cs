using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Contabil
{
    [CustomAuthorize("Contabils/TipoMovimentoArquivoContabil")]
    public class TipoMovimentoArquivoContabilController : BaseController
    {
		#region Construtores

		public TipoMovimentoArquivoContabilController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string descricao = Request.GetStringParam("Descricao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa>("Ativo");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 70, Models.Grid.Align.left, true);

                if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 20, Models.Grid.Align.center, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                Repositorio.Embarcador.Financeiro.TipoMovimentoArquivoContabil repTipoMovimentoArquivoContabil = new Repositorio.Embarcador.Financeiro.TipoMovimentoArquivoContabil(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoArquivoContabil> tipos = repTipoMovimentoArquivoContabil.Consultar(descricao, status, parametrosConsulta);
                grid.setarQuantidadeTotal(repTipoMovimentoArquivoContabil.ContarConsulta(descricao, status));

                var lista = (from p in tipos
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.DescricaoAtivo
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

                Repositorio.Embarcador.Financeiro.TipoMovimentoArquivoContabil repTipoMovimentoArquivoContabil = new Repositorio.Embarcador.Financeiro.TipoMovimentoArquivoContabil(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoArquivoContabil tipoMovimentoArquivoContabil = new Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoArquivoContabil();

                PreencherTipoMovimentoArquivoContabil(tipoMovimentoArquivoContabil, unitOfWork);

                repTipoMovimentoArquivoContabil.Inserir(tipoMovimentoArquivoContabil, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
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

                Repositorio.Embarcador.Financeiro.TipoMovimentoArquivoContabil repTipoMovimentoArquivoContabil = new Repositorio.Embarcador.Financeiro.TipoMovimentoArquivoContabil(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoArquivoContabil tipoMovimentoArquivoContabil = repTipoMovimentoArquivoContabil.BuscarPorCodigo(codigo, true);

                PreencherTipoMovimentoArquivoContabil(tipoMovimentoArquivoContabil, unitOfWork);

                repTipoMovimentoArquivoContabil.Atualizar(tipoMovimentoArquivoContabil, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
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

                Repositorio.Embarcador.Financeiro.TipoMovimentoArquivoContabil repTipoMovimentoArquivoContabil = new Repositorio.Embarcador.Financeiro.TipoMovimentoArquivoContabil(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoArquivoContabil tipoMovimentoArquivoContabil = repTipoMovimentoArquivoContabil.BuscarPorCodigo(codigo, false);

                var dynTipoMovimentoArquivoContabil = new
                {
                    tipoMovimentoArquivoContabil.Codigo,
                    tipoMovimentoArquivoContabil.Descricao,
                    tipoMovimentoArquivoContabil.Ativo,
                    TiposMovimentos = (from obj in tipoMovimentoArquivoContabil.TiposMovimentos
                                       select new
                                       {
                                           obj.Codigo,
                                           obj.Descricao
                                       }).ToList()
                };

                return new JsonpResult(dynTipoMovimentoArquivoContabil);
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

                Repositorio.Embarcador.Financeiro.TipoMovimentoArquivoContabil repTipoMovimentoArquivoContabil = new Repositorio.Embarcador.Financeiro.TipoMovimentoArquivoContabil(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoArquivoContabil tipoMovimentoArquivoContabil = repTipoMovimentoArquivoContabil.BuscarPorCodigo(codigo, true);

                if (tipoMovimentoArquivoContabil == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repTipoMovimentoArquivoContabil.Deletar(tipoMovimentoArquivoContabil, Auditado);

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

        private void PreencherTipoMovimentoArquivoContabil(Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoArquivoContabil tipoMovimentoArquivoContabil, Repositorio.UnitOfWork unitOfWork)
        {
            tipoMovimentoArquivoContabil.Descricao = Request.GetStringParam("Descricao");
            tipoMovimentoArquivoContabil.Ativo = Request.GetBoolParam("Ativo");

            SalvarTiposMovimentos(tipoMovimentoArquivoContabil, unitOfWork);
        }

        private void SalvarTiposMovimentos(Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoArquivoContabil tipoMovimentoArquivoContabil, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);

            if (tipoMovimentoArquivoContabil.TiposMovimentos == null)
                tipoMovimentoArquivoContabil.TiposMovimentos = new List<Dominio.Entidades.Embarcador.Financeiro.TipoMovimento>();
            else
                tipoMovimentoArquivoContabil.TiposMovimentos.Clear();

            dynamic dynTiposMovimentos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposMovimentos"));

            foreach (dynamic dynTipoMovimento in dynTiposMovimentos)
                tipoMovimentoArquivoContabil.TiposMovimentos.Add(repTipoMovimento.BuscarPorCodigo((int)dynTipoMovimento.Codigo));
        }

        #endregion
    }
}
