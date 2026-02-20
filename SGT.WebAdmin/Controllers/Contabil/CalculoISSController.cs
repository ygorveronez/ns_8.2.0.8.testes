using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Contabil
{
    [CustomAuthorize("Contabils/CalculoISS")]
    public class CalculoISSController : BaseController
    {
		#region Construtores

		public CalculoISSController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoLocalidade = Request.GetIntParam("Localidade");
                string codigoServico = Request.GetStringParam("CodigoServico");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Localidade", "Localidade", 40, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Código do Serviço", "CodigoServico", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Alíquota", "Aliquota", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("% Retenção", "PercentualRetencao", 10, Models.Grid.Align.right, true);

                Repositorio.Embarcador.Contabeis.CalculoISS repCalculoISS = new Repositorio.Embarcador.Contabeis.CalculoISS(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Contabeis.CalculoISS> calculoISSs = repCalculoISS.Consultar(codigoLocalidade, codigoServico, parametrosConsulta);
                grid.setarQuantidadeTotal(repCalculoISS.ContarConsulta(codigoLocalidade, codigoServico));

                var lista = (from p in calculoISSs
                             select new
                             {
                                 p.Codigo,
                                 Localidade = p.Localidade.DescricaoCidadeEstado,
                                 p.CodigoServico,
                                 Aliquota = p.Aliquota.ToString("n2"),                                 
                                 PercentualRetencao = p.PercentualRetencao.ToString("n2")
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

                Repositorio.Embarcador.Contabeis.CalculoISS repCalculoISS = new Repositorio.Embarcador.Contabeis.CalculoISS(unitOfWork);
                Dominio.Entidades.Embarcador.Contabeis.CalculoISS calculoISS = new Dominio.Entidades.Embarcador.Contabeis.CalculoISS();

                PreencherCalculoISS(calculoISS, unitOfWork);

                repCalculoISS.Inserir(calculoISS, Auditado);

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

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Contabeis.CalculoISS repCalculoISS = new Repositorio.Embarcador.Contabeis.CalculoISS(unitOfWork);
                Dominio.Entidades.Embarcador.Contabeis.CalculoISS calculoISS = repCalculoISS.BuscarPorCodigo(codigo, true);

                if (calculoISS == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherCalculoISS(calculoISS, unitOfWork);

                repCalculoISS.Atualizar(calculoISS, Auditado);

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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Contabeis.CalculoISS repCalculoISS = new Repositorio.Embarcador.Contabeis.CalculoISS(unitOfWork);
                Dominio.Entidades.Embarcador.Contabeis.CalculoISS calculoISS = repCalculoISS.BuscarPorCodigo(codigo, false);

                if (calculoISS == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var dynCalculoISS = new
                {
                    calculoISS.Codigo,
                    calculoISS.CodigoServico,
                    Aliquota = calculoISS.Aliquota.ToString("n2"),
                    PercentualRetencao = calculoISS.PercentualRetencao.ToString("n2"),
                    Localidade = new { calculoISS.Localidade.Codigo, Descricao = calculoISS.Localidade.DescricaoCidadeEstado }
                };

                return new JsonpResult(dynCalculoISS);
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

                Repositorio.Embarcador.Contabeis.CalculoISS repCalculoISS = new Repositorio.Embarcador.Contabeis.CalculoISS(unitOfWork);
                Dominio.Entidades.Embarcador.Contabeis.CalculoISS calculoISS = repCalculoISS.BuscarPorCodigo(codigo, true);

                if (calculoISS == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repCalculoISS.Deletar(calculoISS, Auditado);

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

        private void PreencherCalculoISS(Dominio.Entidades.Embarcador.Contabeis.CalculoISS calculoISS, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

            int codigoLocalidade = Request.GetIntParam("Localidade");

            calculoISS.CodigoServico = Request.GetStringParam("CodigoServico");
            calculoISS.Aliquota = Request.GetDecimalParam("Aliquota");
            calculoISS.PercentualRetencao = Request.GetDecimalParam("PercentualRetencao");

            calculoISS.Localidade = repLocalidade.BuscarPorCodigo(codigoLocalidade);
        }

        #endregion
    }
}
