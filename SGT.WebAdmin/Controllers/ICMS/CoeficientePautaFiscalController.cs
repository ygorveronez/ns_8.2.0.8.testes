using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.ICMS
{
    [CustomAuthorize("ICMS/CoeficientePautaFiscal")]
    public class CoeficientePautaFiscalController : BaseController
    {
		#region Construtores

		public CoeficientePautaFiscalController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.ICMS.CoeficientePautaFiscal repCoeficientePautaFiscal = new Repositorio.Embarcador.ICMS.CoeficientePautaFiscal(unitOfWork);

                string siglaEstado = Request.GetStringParam("Estado");
                int mes = Request.GetIntParam("Mes");
                int ano = Request.GetIntParam("Ano");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = Request.GetEnumParam("Ativo", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Estado", "Estado", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Mês/Ano", "MesAno", 50, Models.Grid.Align.left, true);
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.center, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                List<Dominio.Entidades.Embarcador.ICMS.CoeficientePautaFiscal> listaGrid = repCoeficientePautaFiscal.Consultar(siglaEstado, mes, ano, ativo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repCoeficientePautaFiscal.ContarConsulta(siglaEstado, mes, ano, ativo);

                var lista = from obj in listaGrid
                            select new
                            {
                                obj.Codigo,
                                Estado = obj.Estado.Nome,
                                MesAno = obj.DescricaoMesAno,
                                obj.DescricaoAtivo
                            };

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista.ToList());
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.ICMS.CoeficientePautaFiscal repCoeficientePautaFiscal = new Repositorio.Embarcador.ICMS.CoeficientePautaFiscal(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.ICMS.CoeficientePautaFiscal coeficientePautaFiscal = repCoeficientePautaFiscal.BuscarPorCodigo(codigo);

                if (coeficientePautaFiscal == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var retorno = new
                {
                    coeficientePautaFiscal.Codigo,
                    Estado = new { Codigo = coeficientePautaFiscal.Estado.Sigla, Descricao = coeficientePautaFiscal.Estado.Nome },
                    coeficientePautaFiscal.Ano,
                    coeficientePautaFiscal.Mes,
                    Valor = coeficientePautaFiscal.Valor.ToString("n2"),
                    PercentualCoeficiente = coeficientePautaFiscal.PercentualCoeficiente.ToString("n2"),
                    coeficientePautaFiscal.Observacao,
                    coeficientePautaFiscal.Ativo
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
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.ICMS.CoeficientePautaFiscal repCoeficientePautaFiscal = new Repositorio.Embarcador.ICMS.CoeficientePautaFiscal(unitOfWork);

                Dominio.Entidades.Embarcador.ICMS.CoeficientePautaFiscal coeficientePautaFiscal = new Dominio.Entidades.Embarcador.ICMS.CoeficientePautaFiscal();

                PreencheEntidade(coeficientePautaFiscal, unitOfWork);

                if (!ValidaEntidade(coeficientePautaFiscal, out string erro))
                    return new JsonpResult(false, true, erro);

                repCoeficientePautaFiscal.Inserir(coeficientePautaFiscal, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
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

                Repositorio.Embarcador.ICMS.CoeficientePautaFiscal repCoeficientePautaFiscal = new Repositorio.Embarcador.ICMS.CoeficientePautaFiscal(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.ICMS.CoeficientePautaFiscal coeficientePautaFiscal = repCoeficientePautaFiscal.BuscarPorCodigo(codigo, true);

                if (coeficientePautaFiscal == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencheEntidade(coeficientePautaFiscal, unitOfWork);

                if (!ValidaEntidade(coeficientePautaFiscal, out string erro))
                    return new JsonpResult(false, true, erro);

                repCoeficientePautaFiscal.Atualizar(coeficientePautaFiscal, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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

                Repositorio.Embarcador.ICMS.CoeficientePautaFiscal repCoeficientePautaFiscal = new Repositorio.Embarcador.ICMS.CoeficientePautaFiscal(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.ICMS.CoeficientePautaFiscal coeficientePautaFiscal = repCoeficientePautaFiscal.BuscarPorCodigo(codigo);

                if (coeficientePautaFiscal == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repCoeficientePautaFiscal.Deletar(coeficientePautaFiscal, Auditado);

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
                    return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencheEntidade(Dominio.Entidades.Embarcador.ICMS.CoeficientePautaFiscal coeficientePautaFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);

            coeficientePautaFiscal.Estado = repEstado.BuscarPorSigla(Request.GetStringParam("Estado"));
            coeficientePautaFiscal.Mes = Request.GetIntParam("Mes");
            coeficientePautaFiscal.Ano = Request.GetIntParam("Ano");
            coeficientePautaFiscal.Valor = Request.GetDecimalParam("Valor");
            coeficientePautaFiscal.PercentualCoeficiente = Request.GetDecimalParam("PercentualCoeficiente");
            coeficientePautaFiscal.Observacao = Request.GetStringParam("Observacao");
            coeficientePautaFiscal.Ativo = Request.GetBoolParam("Ativo");
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.ICMS.CoeficientePautaFiscal coeficientePautaFiscal, out string msgErro)
        {
            msgErro = "";

            if (coeficientePautaFiscal.Estado == null)
            {
                msgErro = "Estado é obrigatório.";
                return false;
            }

            if (coeficientePautaFiscal.Mes < 1 || coeficientePautaFiscal.Mes > 12)
            {
                msgErro = "Mês inválido.";
                return false;
            }

            if (coeficientePautaFiscal.Ano < 2000 || coeficientePautaFiscal.Ano > 3000)
            {
                msgErro = "Ano inválido.";
                return false;
            }

            if (coeficientePautaFiscal.Valor <= 0)
            {
                msgErro = "Valor é obrigatório.";
                return false;
            }

            if (coeficientePautaFiscal.Observacao.Length == 0)
            {
                msgErro = "Observação é obrigatória.";
                return false;
            }

            return true;
        }

        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "Estado") propOrdenar = "Estado.Sigla";
        }

        #endregion
    }
}
