using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Rateios
{
    [CustomAuthorize("Rateios/RateioFormula")]
    public class RateioFormulaController : BaseController
    {
		#region Construtores

		public RateioFormulaController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string descricao = Request.Params("Descricao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("ParametroRateioFormula", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 80, Models.Grid.Align.left, true);
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", 20, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Rateio.RateioFormula repRateioFormula = new Repositorio.Embarcador.Rateio.RateioFormula(unitOfWork);
                List<Dominio.Entidades.Embarcador.Rateio.RateioFormula> rateioFormulas = repRateioFormula.Consultar(descricao, ativo, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repRateioFormula.ContarConsulta(descricao, ativo));

                var retorno = (from obj in rateioFormulas
                               select new
                               {
                                   obj.Codigo,
                                   obj.Descricao,
                                   obj.DescricaoAtivo,
                                   obj.ParametroRateioFormula
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
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

                Repositorio.Embarcador.Rateio.RateioFormula repRateioFormula = new Repositorio.Embarcador.Rateio.RateioFormula(unitOfWork);

                Dominio.Entidades.Embarcador.Rateio.RateioFormula rateioFormula = new Dominio.Entidades.Embarcador.Rateio.RateioFormula();
                rateioFormula.Ativo = bool.Parse(Request.Params("Ativo"));
                rateioFormula.Descricao = Request.Params("Descricao");
                rateioFormula.ParametroRateioFormula = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula)int.Parse(Request.Params("ParametroRateioFormula"));
                rateioFormula.RatearPrimeiroIgualmenteEntrePedidos = Request.GetBoolParam("RatearPrimeiroIgualmenteEntrePedidos");
                rateioFormula.ArredondarParaNumeroParMaisProximo = Request.GetBoolParam("ArredondarParaNumeroParMaisProximo");
                rateioFormula.RatearEmBlocoDeEmissao = Request.GetBoolParam("RatearEmBlocoDeEmissao");
                rateioFormula.PercentualAcrescentarPesoTotalCarga = Request.GetDecimalParam("PercentualAcrescentarPesoTotalCarga");
                rateioFormula.ExigirConferenciaManual = Request.GetBoolParam("ExigirConferenciaManual");

                repRateioFormula.Inserir(rateioFormula, Auditado);
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
                Repositorio.Embarcador.Rateio.RateioFormula repRateioFormula = new Repositorio.Embarcador.Rateio.RateioFormula(unitOfWork);
                Dominio.Entidades.Embarcador.Rateio.RateioFormula rateioFormula = repRateioFormula.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);
                rateioFormula.Ativo = bool.Parse(Request.Params("Ativo"));
                rateioFormula.Descricao = Request.Params("Descricao");
                rateioFormula.ParametroRateioFormula = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula)int.Parse(Request.Params("ParametroRateioFormula"));
                rateioFormula.RatearPrimeiroIgualmenteEntrePedidos = Request.GetBoolParam("RatearPrimeiroIgualmenteEntrePedidos");
                rateioFormula.ArredondarParaNumeroParMaisProximo = Request.GetBoolParam("ArredondarParaNumeroParMaisProximo");
                rateioFormula.RatearEmBlocoDeEmissao = Request.GetBoolParam("RatearEmBlocoDeEmissao");
				rateioFormula.PercentualAcrescentarPesoTotalCarga = Request.GetDecimalParam("PercentualAcrescentarPesoTotalCarga");
                rateioFormula.ExigirConferenciaManual = Request.GetBoolParam("ExigirConferenciaManual");

                repRateioFormula.Atualizar(rateioFormula, Auditado);
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
                Repositorio.Embarcador.Rateio.RateioFormula repRateioFormula = new Repositorio.Embarcador.Rateio.RateioFormula(unitOfWork);
                Dominio.Entidades.Embarcador.Rateio.RateioFormula rateioFormula = repRateioFormula.BuscarPorCodigo(codigo);
                var dynRateioFormula = new
                {
                    rateioFormula.Codigo,
                    rateioFormula.ParametroRateioFormula,
                    rateioFormula.Descricao,
                    rateioFormula.Ativo,
                    rateioFormula.RatearPrimeiroIgualmenteEntrePedidos,
                    rateioFormula.ArredondarParaNumeroParMaisProximo,
                    rateioFormula.RatearEmBlocoDeEmissao,
                    PercentualAcrescentarPesoTotalCarga = rateioFormula.PercentualAcrescentarPesoTotalCarga > 0m ? rateioFormula.PercentualAcrescentarPesoTotalCarga.ToString("n2") : string.Empty,
                    rateioFormula.ExigirConferenciaManual
            };
                return new JsonpResult(dynRateioFormula);
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
                Repositorio.Embarcador.Rateio.RateioFormula repRateioFormula = new Repositorio.Embarcador.Rateio.RateioFormula(unitOfWork);
                Dominio.Entidades.Embarcador.Rateio.RateioFormula rateioFormula = repRateioFormula.BuscarPorCodigo(codigo);
                repRateioFormula.Deletar(rateioFormula, Auditado);
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


    }
}
