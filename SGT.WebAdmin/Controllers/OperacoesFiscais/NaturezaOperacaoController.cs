using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.OperacoesFiscais
{
    [CustomAuthorize("OperacoesFiscais/NaturezaOperacao")]
    public class NaturezaOperacaoController : BaseController
    {
		#region Construtores

		public NaturezaOperacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public async Task<IActionResult> Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                string descricao = Request.Params("Descricao");
                string codigoIntegracao = Request.Params("CodigoIntegracao");

                if (string.IsNullOrWhiteSpace(descricao))
                    return new JsonpResult(false, "Descrição inválida!");

                Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);
                Dominio.Entidades.NaturezaDaOperacao natureza;

                if (codigo > 0)
                {
                    natureza = repNaturezaDaOperacao.BuscarPorId(codigo);
                    natureza.Initialize();
                }
                else
                    natureza = new Dominio.Entidades.NaturezaDaOperacao();

                natureza.Descricao = descricao;
                natureza.CodigoIntegracao = codigoIntegracao;
                natureza.Status = "A";

                if (codigo > 0)
                    repNaturezaDaOperacao.Atualizar(natureza, Auditado);
                else
                    repNaturezaDaOperacao.Inserir(natureza, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar a natureza da operação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public async Task<IActionResult> Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Codigo, "CodigoIntegracao", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 60, Models.Grid.Align.left, true);

                Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);

                List<Dominio.Entidades.NaturezaDaOperacao> listaNaturezas = repNaturezaDaOperacao.Consultar(descricao, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                int countNaturezas = repNaturezaDaOperacao.ContarConsulta(descricao);

                grid.setarQuantidadeTotal(countNaturezas);

                grid.AdicionaRows((from obj in listaNaturezas
                                   select new
                                   {
                                       obj.Codigo,
                                       CodigoIntegracao = obj.CodigoIntegracao,
                                       Descricao = obj.Descricao
                                   }).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, Localization.Resources.Consultas.NaturezaOperacao.OcorreuUmaFalhaAoBuscarAsNaturezasDasOperacoes);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.NaturezaDaOperacao repNaturezaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);
                Dominio.Entidades.NaturezaDaOperacao naturezaOperacao = repNaturezaOperacao.BuscarPorId(codigo);

                var entidade = new
                {
                    naturezaOperacao.Codigo,
                    naturezaOperacao.CodigoIntegracao,
                    naturezaOperacao.Descricao
                };

                return new JsonpResult(entidade);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter a natureza de operação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

    }
}
