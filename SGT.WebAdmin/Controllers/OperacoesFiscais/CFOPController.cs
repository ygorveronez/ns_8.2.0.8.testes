using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.OperacoesFiscais
{
    [CustomAuthorize("OperacoesFiscais/CFOP")]
    public class CFOPController : BaseController
    {
		#region Construtores

		public CFOPController(Conexao conexao) : base(conexao) { }

		#endregion

        [AcceptVerbs("POST")]
        public async Task<IActionResult> Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo, codigoNaturezaOperacao, cfop;
                int.TryParse(Request.Params("Codigo"), out codigo);
                int.TryParse(Request.Params("NaturezaOperacao"), out codigoNaturezaOperacao);
                int.TryParse(Request.Params("CFOP"), out cfop);

                Dominio.Enumeradores.TipoCFOP tipoCFOP;
                Enum.TryParse<Dominio.Enumeradores.TipoCFOP>(Request.Params("TipoCFOP"), out tipoCFOP);

                Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
                Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);

                Dominio.Entidades.CFOP objCFOP;

                if (codigo > 0)
                {
                    objCFOP = repCFOP.BuscarPorId(codigo);
                    objCFOP.Initialize();
                }
                else
                    objCFOP = new Dominio.Entidades.CFOP();

                objCFOP.NaturezaDaOperacao = repNaturezaDaOperacao.BuscarPorId(codigoNaturezaOperacao);
                objCFOP.CodigoCFOP = cfop;
                objCFOP.Tipo = tipoCFOP;
                objCFOP.Status = "A";

                if (codigo > 0)
                    repCFOP.Atualizar(objCFOP, Auditado);
                else
                    repCFOP.Inserir(objCFOP, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar a CFOP.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AllowAuthenticate]
        public async Task<IActionResult> Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int cfop, naturezaOperacao;
                int.TryParse(Request.Params("CFOP"), out cfop);
                int.TryParse(Request.Params("NaturezaOperacao"), out naturezaOperacao);

                Dominio.Enumeradores.TipoCFOP? tipoCFOP = null;
                Dominio.Enumeradores.TipoCFOP tipoCFOPAux;
                if (Enum.TryParse<Dominio.Enumeradores.TipoCFOP>(Request.Params("Tipo"), out tipoCFOPAux))
                    tipoCFOP = tipoCFOPAux;

                bool? ativo = Request.GetNullableBoolParam("Ativo");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.CFOP.DescricaoCFOP, "CodigoCFOP", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.CFOP.NaturezaDaOperacao, "NaturezaDaOperacao", 80, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenar == "NaturezaDaOperacao")
                    propOrdenar += ".Descricao";

                Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);

                var listaCFOPs = repCFOP.Consultar(cfop, naturezaOperacao, tipoCFOP, ativo, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                var countCFOPs = repCFOP.ContarConsulta(cfop, naturezaOperacao, tipoCFOP, ativo);


                grid.setarQuantidadeTotal(countCFOPs);

                var retorno = (from obj in listaCFOPs
                               select new
                               {
                                   obj.Codigo,
                                   Descricao = obj.CodigoCFOP.ToString(),
                                   CodigoCFOP = obj.CodigoCFOP,
                                   NaturezaDaOperacao = obj.NaturezaDaOperacao != null ? obj.NaturezaDaOperacao.Descricao : string.Empty,
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Consultas.CFOP.OcorreuUmaFalhaAoConsultarAsCFOPs);
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
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);

                Dominio.Entidades.CFOP cfop = repCFOP.BuscarPorId(codigo);

                if (cfop == null)
                    return new JsonpResult(false, "CFOP não encontrado.");

                return new JsonpResult(new
                {
                    cfop.Codigo,
                    CFOP = cfop.CodigoCFOP,
                    Tipo = cfop.Tipo,
                    NaturezaOperacao = cfop.NaturezaDaOperacao != null ? new { cfop.NaturezaDaOperacao.Codigo, cfop.NaturezaDaOperacao.Descricao } : null
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter o CFOP.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
